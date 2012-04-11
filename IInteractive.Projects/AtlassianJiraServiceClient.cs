using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;
using Atlassian.Jira.Remote;

namespace IInteractive.Projects
{
    public class AtlassianJiraServiceClient : JiraSoapServiceClient
    {
        public AtlassianJiraServiceClient(string jiraBaseUrl)
            : base(GenerateBinding(jiraBaseUrl), GenerateEndPoint(jiraBaseUrl))
        {
            Endpoint.Behaviors.Add(new RemoteWorklogPatchBehavior());
        }

        private static BasicHttpBinding GenerateBinding(string jiraBaseUrl)
        {
            var endPointUri = GenerateEndPointUri(jiraBaseUrl);

            BasicHttpBinding binding = null;
            if (endPointUri.Scheme == "https")
            {
                binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            }
            else
            {
                binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            }
            binding.TransferMode = TransferMode.Buffered;
            binding.UseDefaultWebProxy = true;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.ReaderQuotas = new XmlDictionaryReaderQuotas() { MaxStringContentLength = 2147483647 };
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

            return binding;
        }

        private static EndpointAddress GenerateEndPoint(string jiraBaseUrl)
        {
            return new EndpointAddress(GenerateEndPointUri(jiraBaseUrl));
        }

        private static Uri GenerateEndPointUri(string jiraBaseUrl)
        {
            if (!jiraBaseUrl.EndsWith("/"))
            {
                jiraBaseUrl += "/";
            }

            var endPointUri = new Uri(jiraBaseUrl + "rpc/soap/jirasoapservice-v2");
            return endPointUri;
        }
    }

    internal class RemoteWorklogPatchBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new RemoteWorklogMessageInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        private class RemoteWorklogMessageInspector : IClientMessageInspector
        {
            private static string _correlationState = "worklog";

            public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
            {
                if (correlationState != null
                    && String.Equals(correlationState, _correlationState)
                    && !reply.ToString().Contains("<soapenv:Fault>"))
                {
                    var memoryStream = new MemoryStream();
                    var writer = XmlWriter.Create(memoryStream);
                    reply.WriteMessage(writer);
                    writer.Flush();

                    memoryStream.Position = 0;
                    var doc = new XmlDocument();
                    doc.Load(memoryStream);

                    UpdateMessage(doc);

                    memoryStream.SetLength(0);
                    writer = XmlWriter.Create(memoryStream);
                    doc.WriteTo(writer);
                    writer.Flush();

                    memoryStream.Position = 0;
                    var reader = XmlReader.Create(memoryStream);
                    reply = Message.CreateMessage(reader, int.MaxValue, reply.Version);
                }
            }

            private void UpdateMessage(XmlDocument doc)
            {
                var ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
                foreach (XmlElement multiRefElement in doc.SelectNodes("//soapenv:Body/multiRef", ns))
                {
                    multiRefElement.SetAttribute("xsi:type", "ns2:RemoteWorklog");
                    multiRefElement.SetAttribute("xmlns:ns2", "http://beans.soap.rpc.jira.atlassian.com");
                }
            }

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                var requestContent = request.ToString();

                if (requestContent.Contains("addWorklogAndAutoAdjustRemainingEstimate")
                    || requestContent.Contains("addWorklogAndRetainRemainingEstimate")
                    || requestContent.Contains("addWorklogWithNewRemainingEstimate")
                    || requestContent.Contains("getWorklogs"))
                {
                    return _correlationState;
                }
                return null;
            }
        }
    }
}
