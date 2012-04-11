using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IInteractive.Projects.Security;

namespace IInteractive.Projects.Test
{
    public class TestToken : IAuthenticationToken
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
    }
}
