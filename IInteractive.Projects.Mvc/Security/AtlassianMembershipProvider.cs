using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Web.Security;

using System.Configuration.Provider;
using Atlassian.Jira.Remote;
using IInteractive.Projects.Security;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

using IInteractive.Projects.Logging;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace IInteractive.Projects.Mvc.Security
{
    public class AtlassianMembershipProvider : MembershipProvider
    {
        private LogWriter writer = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
        private TraceManager traceMgr = EnterpriseLibraryContainer.Current.GetInstance<TraceManager>();

        private IProjectsService service;

        // MembershipProvider Methods
        public override void Initialize(string name, NameValueCollection config)
        {
            // Verify that config isn't null
            if (config == null)
                throw new ArgumentNullException("config");

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
                name = "AtlassianMembershipProvider";

            // Add a default "description" attribute to config if the
            // attribute doesn't exist or is empty
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Atlassian web service membership provider");
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            using (IUnityContainer container = new UnityContainer()
                .LoadConfiguration())
            {
                service = container.Resolve<IProjectsService>();
            }

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException("Unrecognized attribute: " + attr);
            }
        }

        public override string ApplicationName { get; set; }

        #region Not Supported

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            MembershipUser user = null;
            if(providerUserKey != null && providerUserKey is string)
            {
                var token = (string) providerUserKey;

                var authenticationToken = string.Empty;
                var username = string.Empty;

                var tokens = token.Split(';');
                if(tokens != null && tokens.Length == 2)
                {
                    authenticationToken = tokens[0];
                    username = tokens[1];
                }

                service.GetUser(new StringAuthenticationToken(authenticationToken), username);
            }
            return user;
        }

        #endregion

        public override bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
                return false;

            try
            {
                IAuthenticationToken value = service.Login(username, password);
                return value != null && !string.IsNullOrWhiteSpace(value.Token);
            }
            catch (Exception exception)
            {
                writer.Error("Error attempting to log user into the web service.", exception);
                return false;
            }
        }
    }
}
