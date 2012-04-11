using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace IInteractive.Projects.Mvc
{
    public class IInteractiveHttpApplication : HttpApplication
    {
        private LogWriter writer = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
        private TraceManager traceMgr = EnterpriseLibraryContainer.Current.GetInstance<TraceManager>();

        public IInteractiveHttpApplication()
        {
            Error += new EventHandler(IInteractiveHttpApplication_Error);
            AuthorizeRequest += new EventHandler(IInteractiveHttpApplication_AuthorizeRequest);
            BeginRequest += new EventHandler(IInteractiveHttpApplication_BeginRequest);
            EndRequest += new EventHandler(IInteractiveHttpApplication_EndRequest);
        }

        void IInteractiveHttpApplication_EndRequest(object sender, EventArgs e)
        {
            
        }

        void IInteractiveHttpApplication_BeginRequest(object sender, EventArgs e)
        {
        }

        void IInteractiveHttpApplication_AuthorizeRequest(object sender, EventArgs e)
        {
        }

        void IInteractiveHttpApplication_Error(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        public void Application_Stop()
        {
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }
    }
}
