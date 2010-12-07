using System;
using System.Web.Mvc;
using BoC.Security.Mvc.Controllers;
using BoC.Web.Mvc.PrecompiledViews;

namespace BoC.Security.Mvc
{
    public class SecurityAreaRegistration : AreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context)
        {
			ApplicationPartRegistry.Register(this.GetType().Assembly, "~/Areas/Security/");
			context.MapRoute("security_default", "Security/{controller}/{action}/{id}",
                             new {controller = "Home", action = "index", id = ""},
                             new [] {typeof(AccountController).Namespace});
            
            context.MapRoute(
                "OpenIdDiscover",
                "Security/Auth/Discover");
        }

        public override string AreaName
        {
            get { return "Security"; }
        }
    }
}