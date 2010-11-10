using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BoC.Security.Model;
using BoC.Web;
using Norm.Configuration;

namespace ToDoList
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : CommonHttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        protected override void InitializeApplication()
        {
            base.InitializeApplication();
            MongoConfiguration.Initialize(config =>
                            config.For<User>(c => c.ForProperty(u => u.Identity).Ignore()));
        }
    }
}