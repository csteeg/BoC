using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BoC.EventAggregator;
using BoC.InversionOfControl;
using BoC.Tasks;
using BoC.Web.Events;
using BoC.Web.Mvc.Attributes;
using BoC.Web.Mvc.Binders;
using IDependencyResolver = BoC.InversionOfControl.IDependencyResolver;

namespace BoC.Web.Mvc.Init
{
    public class RegisterDefaultRoutes : IBootstrapperTask
    {
        public void Execute()
        {
            Register(RouteTable.Routes);
        }

        public void Register(RouteCollection routes)
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
                routes.MapRoute("JsonActionId", "{controller}/{action}/{id}.{resultformat}.aspx", new { }, new { controller = @"[^\.]*", action = @"[^\.]*" }, new { __mvcajax = "true" });

                routes.MapRoute(
                    "Default",
                    "{controller}.aspx/{action}/{id}",
                    new { controller = "Home", action = "Index", id = ""},
                    new { controller = "[^\\.]*", action = "[^\\.]*" }
                );
            }
            else
            {
                routes.MapRoute("JsonActionId", "{controller}/{action}/{id}.{resultformat}", new { }, new { controller = @"[^\.]*", action = @"[^\.]*" }, new { __mvcajax = "true" });
                routes.MapRoute("JsonAction", "{controller}/{action}.{resultformat}", new { id = "" }, new { controller = @"[^\.]*", action = @"[^\.]*" }, new { __mvcajax = "true" });
                routes.MapRoute("Json", "{controller}.{resultformat}", new { action = "Index", id = "" }, new { controller = @"[^\.]*", action = @"[^\.]*" }, new { __mvcajax = "true" });
                routes.MapRoute(
                    "Default",
                    "{controller}/{action}/{id}",
                    new { controller = "Home", action = "Index", id = "" },
                    new { controller = "[^\\.]*", action = "[^\\.]*" },
                    new []{"ToDoList.Controllers"}
                );
            }
        }

    }
}