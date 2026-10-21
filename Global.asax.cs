using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using Student_Att.Logging;

namespace Student_Att
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup (from the Visual Studio template)
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // Runs for any error that no page handled.
        // We log the details; <customErrors> in Web.config then shows Error.aspx to the user.
        void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                Logger.LogError(ex);
            }
        }
    }
}
