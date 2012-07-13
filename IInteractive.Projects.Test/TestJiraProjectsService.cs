using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using IInteractive.Projects;
using IInteractive.Projects.Security;
using Atlassian.Jira.Remote;

namespace IInteractive.Projects.Test
{
    [TestClass]
    public class TestJiraProjectsService
    {
        private IProjectsService _service;
        private IAuthenticationToken _token;

        private string Username { get; set; }
        private string Password { get; set; }

        [TestInitialize]
        public void PreTestInitialize()
        {
            Username = ConfigurationManager.AppSettings["TestJiraUsername"];
            Password = ConfigurationManager.AppSettings["TestJiraPassword"];

            using (IUnityContainer container = new UnityContainer()
                .LoadConfiguration())
            {
                _service = container.Resolve<IProjectsService>();
            }

            _token = _service.Login(Username, Password);
        }

        [TestMethod]
        public void TestGetProjects()
        {
            IEnumerable<RemoteProject> projects = _service.GetProjects(_token);

            Assert.IsNotNull(projects);
            Assert.IsTrue(projects.Count<RemoteProject>() > 0);

            Assert.AreEqual<int>(1, projects.Count<RemoteProject>(
                project => string.Equals(project.key, "AGENCYTEST")));

            Assert.AreEqual<int>(1, projects.Count<RemoteProject>(
                project => string.Equals(project.key, "ALLEGRA")));
        }

        [TestMethod]
        public void TestGetProjectByKey()
        {
            RemoteProject project = _service.GetProject(_token, "AGENCYTEST");

            Assert.IsNotNull(project);
            Assert.AreEqual<string>("AGENCYTEST", project.key);
        }

        [TestMethod]
        public void TestLoginUser()
        {
            try
            {
                IAuthenticationToken authenticationToken = _service.Login(Username, Password);

                Assert.IsNotNull(authenticationToken);
                Assert.IsFalse(string.IsNullOrWhiteSpace(authenticationToken.Token));
            }
            catch (Exception exception)
            {
                Assert.Fail("Failed to login ({0},{1}) due to exception: {2}", Username, Password, exception.Message);
            }
        }

        [TestMethod]
        public void TestLogoffUser()
        {
            IAuthenticationToken authenticationToken = _service.Login(Username, Password);

            Assert.IsNotNull(authenticationToken);
            Assert.IsFalse(string.IsNullOrWhiteSpace(authenticationToken.Token));

            bool success = _service.Logout(authenticationToken);
            Assert.IsTrue(success);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _service.Logout(_token);
        }
    }
}
