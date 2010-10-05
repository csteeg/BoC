using System;
using System.Web.Mvc;
using BoC.Security.Mvc.Controllers;

namespace BoC.Security.Mvc
{
    public class SecurityAreaRegistration : AreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("security_default", "Security/{controller}/{action}/{id}",
                             new {controller = "Home", action = "index", id = ""},
                             new [] {typeof(AccountController).Namespace});
        }

        public override string AreaName
        {
            get { return "Security"; }
        }
    }
}