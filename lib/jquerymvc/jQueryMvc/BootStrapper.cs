using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using JqueryMvc.Mvc;

namespace JqueryMvc
{
    static public class BootStrapper
    {
        static public void Run()
        {
            SetDefaultViewEngine();
            RegisterDefaultRoutes(RouteTable.Routes);
        }

        static public void SetDefaultViewEngine()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new DefaultViewViewEngine());
        }

        public static void RegisterDefaultRoutes(RouteCollection routes)
		{
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

		    //if you live on a server that doesn't need the 
            //default.aspx... you can delete the default.aspx,
            //and routing will be skipped for existing files...
            if (HttpContext.Current != null && System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/default.aspx")))
            {
                routes.IgnoreRoute("{*rest}", new { rest = "^Content\\/.*" });
                routes.RouteExistingFiles = true;
                routes.MapRoute("Default.aspx", "default.aspx",
                    new
                    {
                        controller = "Home",
                        action = "Index",
                        id = ""
                    }
                );
                routes.MapRoute("Json", "{controller}.{resultformat}.aspx", new { action = "Index", id = "" }, new { controller = @"[^\.]*", action = @"[^\.]*" }, new { __mvcajax = "true" });
                routes.MapRoute("JsonAction", "{controller}/{action}.{resultformat}.aspx", new { id = "" }, new { controller = @"[^\.]*", action = @"[^\.]*" }, new { __mvcajax = "true" });
                routes.MapRoute("JsonActionId", "{controller}/{action}/{id}.{resultformat}.aspx", new {}, new { controller = @"[^\.]*", action = @"[^\.]*" }, new { __mvcajax = "true" });

                routes.MapRoute(
                    "Default",                     
                    "{controller}.aspx/{action}/{id}",  
                    new { 
                        controller = "Home", 
                        action = "Index", 
                        id = "" },                  
                    new { controller = "[^\\.]*", action = "[^\\.]*" }
                );
            }
            else
            {

                routes.MapRoute("JsonActionId", "{controller}/{action}/{id}.{resultformat}", new {}, new { controller = @"[^\.]*", action = @"[^\.]*" }, new { __mvcajax = "true" });
                routes.MapRoute("JsonAction", "{controller}/{action}.{resultformat}", new { id = "" }, new { controller = @"[^\.]*", action = @"[^\.]*" }, new { __mvcajax = "true" });
                routes.MapRoute("Json", "{controller}.{resultformat}", new { action = "Index", id = "" }, new { controller = @"[^\.]*", action = @"[^\.]*" }, new { __mvcajax = "true" });
                routes.MapRoute(
                    "Default",                                               
                    "{controller}/{action}/{id}",                            
                    new { controller = "Home", action = "Index", id = "" },  
                    new { controller = "[^\\.]*", action = "[^\\.]*" }
                );
            }
        }
    }
}
