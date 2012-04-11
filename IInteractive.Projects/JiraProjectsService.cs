using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlassian.Jira.Remote;
using IInteractive.Projects.Security;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace IInteractive.Projects
{
    public class JiraProjectsService : IProjectsService
    {
        private AtlassianJiraServiceClient _soapServiceClient;

        public JiraProjectsService(string jiraBaseUrl)
        {
            _soapServiceClient = new AtlassianJiraServiceClient(jiraBaseUrl);
        }

        public IEnumerable<RemoteProject> GetProjects(IAuthenticationToken token)
        {
            return _soapServiceClient.getProjectsNoSchemes(token.Token).ToList<RemoteProject>();
        }

        public RemoteProject GetProject(IAuthenticationToken token, string key)
        {
            return _soapServiceClient.getProjectByKey(token.Token, key);
        }

        public IAuthenticationToken GetUser(IAuthenticationToken token, string username)
        {
            IAuthenticationToken user = null;
            var jiraUser = _soapServiceClient.getUser(token.Token, username);
            
            using(IUnityContainer container = new UnityContainer()
                .LoadConfiguration())
            {
                user = container.Resolve<IAuthenticationToken>();

                user.Email = jiraUser.email;
                user.FullName = jiraUser.fullname;
                user.Username = jiraUser.name;
            }

            return user;
        }

        public IAuthenticationToken Login(string username, string password)
        {
            string token = _soapServiceClient.login(username, password);
            
            using (IUnityContainer container = new UnityContainer()
                .LoadConfiguration())
            {
                var authenticationToken = container.Resolve<IAuthenticationToken>();
                authenticationToken.Token = token;

                return authenticationToken;
            }
        }

        public bool Logout(IAuthenticationToken authenticationToken)
        {
            return _soapServiceClient.logout(authenticationToken.Token);
        }
    }
}
