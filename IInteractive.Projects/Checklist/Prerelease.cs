using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Atlassian.Jira.Remote;

namespace IInteractive.Projects.Checklist
{
    public class Prerelease
    {
        public Prerelease()
        {
            
        }

        public RemoteIssue GenerateIssue()
        {
            var issue = new RemoteIssue();
            return issue;
        }

        [Display(Name = "Release Name"),
        Required]
        public String Name { get; set; }

        [Display(Name = "Version of the Release", Description = "The version of the release to be pushed.  This must match the version in JIRA.")]
        public String Version { get; set; }

        [Display(Name = "Description")]
        public String Description { get; set; }
        public String Assignee { get; set; }
        public DateTime DueDate { get; set; }
    }
}
