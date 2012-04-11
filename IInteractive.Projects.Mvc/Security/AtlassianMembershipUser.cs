using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using System.Web.Security;
using IInteractive.Projects.Security;

namespace IInteractive.Projects.Mvc.Security
{
    public class AtlassianMembershipUser : MembershipUser, IAuthenticationToken
    {
        public AtlassianMembershipUser(string token)
        {
            Token = token;
        }
        public AtlassianMembershipUser(string username, string token)
            : base(null, username, null, null, null, null, true, false, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue)
        {
            Token = token;
        }

        public string Token { get; set; }

        public static IAuthenticationToken GetCurrentToken()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["membership-token"];

            if(cookie != null)
            {
                return new AtlassianMembershipUser(cookie.Value);
            }

            return null;
        }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
