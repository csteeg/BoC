using System;
using System.Web.Mvc;

namespace BoC.Security.Mvc
{
    public class SecurityAreaRegistration : AreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("security_default", "Security/{controller}/{action}/{id}",
                             new {controller = "Home", action = "index", id = ""});
        }

        public override string AreaName
        {
            get { return "Security"; }
        }
    }
}