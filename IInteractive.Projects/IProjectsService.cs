using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Atlassian.Jira.Remote;
using IInteractive.Projects.Security;

namespace IInteractive.Projects
{
    public interface IProjectsService
    {
        IEnumerable<RemoteProject> GetProjects(IAuthenticationToken token);
        RemoteProject GetProject(IAuthenticationToken token, string key);
        RemoteIssue CreateIssue(IAuthenticationToken token, RemoteIssue issue);

        IAuthenticationToken GetUser(IAuthenticationToken token, string username);

        IAuthenticationToken Login(string username, string password);

        bool Logout(IAuthenticationToken authenticationToken);
    }
}
