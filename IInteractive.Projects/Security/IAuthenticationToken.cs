using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IInteractive.Projects.Security
{
    public interface IAuthenticationToken
    {
        string Token { get; set; }
        string Username { get; set; }
        string Email { get; set; }
        string FullName { get; set; }
    }
}
