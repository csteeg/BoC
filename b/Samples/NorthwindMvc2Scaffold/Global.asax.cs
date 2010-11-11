using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using BoC.InversionOfControl;
using BoC.InversionOfControl.Unity;
using BoC.Persistence.NHibernate;
using BoC.Tasks;
using NHibernate;

namespace NorthwindMvc2Scaffold
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static bool initialized = false;
        protected void Application_BeginRequest()
        {
            if (!initialized)
                Application_Start();
        }
        
        protected void Application_Start()
        {
            if (!IoC.IsInitialized())
            {
                try
                {
                    //RegisterRoutes(RouteTable.Routes);
                    IoC.InitializeWith(new ExtendedUnityContainer());
                    Bootstrapper.RegisterAllTasksAndRunThem(type => true);
                    initialized = true;
                }
                catch
                {
                    IoC.Reset();
                    throw;
                }
            }
        }

        protected void Application_EndRequest()
        {
            WebSessionManager.CleanUp(HttpContext.Current, IoC.Resolve<ISessionFactory>());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

        }
        
    }
}