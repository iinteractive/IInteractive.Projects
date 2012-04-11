using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IInteractive.Projects.Security
{
    public class StringAuthenticationToken : IAuthenticationToken
    {
        public StringAuthenticationToken(string token)
        {
            Token = token;
        }

        public string Token { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }
}
