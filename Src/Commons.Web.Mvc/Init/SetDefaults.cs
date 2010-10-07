using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BoC.InversionOfControl;
using BoC.Tasks;
using BoC.Web.Mvc.Binders;
using IDependencyResolver = BoC.InversionOfControl.IDependencyResolver;

namespace BoC.Web.Mvc.Init
{
    public class SetDefaults : IBootstrapperTask
    {
        private readonly IDependencyResolver dependencyResolver;

        public SetDefaults(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            System.Web.Mvc.DependencyResolver.SetResolver(
                this.dependencyResolver.Resolve,
                (t) => this.dependencyResolver.ResolveAll(t).Cast<object>());
            SetDefaultViewEngine();
            RegisterAllAreas();
            RegisterDefaultRoutes(RouteTable.Routes);
            
            dependencyResolver.RegisterType<IControllerFactory,AutoScaffoldControllerFactory>();

            ModelBinders.Binders.DefaultBinder = new CommonModelBinder(dependencyResolver);
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
        }

        protected virtual void RegisterAllAreas()
        {
            AreaRegistration.RegisterAllAreas();
        }

        static public void SetDefaultViewEngine()
        {
            //replace webformviewengine with our version (that searches only .ascx for partials and only .aspx for fullviews)
            for (var i = ViewEngines.Engines.Count - 1; i >= 0; i--)
            {
                if (ViewEngines.Engines[i] is WebFormViewEngine)
                {
                    ViewEngines.Engines[i] = new DefaultViewViewEngine();
                    break; //only do the first one
                }
            }
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
                    new { controller = "[^\\.]*", action = "[^\\.]*" }
                );
            }
        }

    }
}