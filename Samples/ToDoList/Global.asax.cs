using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Web.WebPages.Razor;
using BoC.Security.Model;
using BoC.Web;
using Commons.Web.Mvc.PrecompiledViews;
using Norm.Configuration;

namespace ToDoList
{

	public class MvcApplication : CommonHttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        protected override void InitializeApplication()
        {
			base.InitializeApplication();
			//ViewEngines.Engines.Add(new VirtualPathFactoryManagerViewEngine());
        	ApplicationPartRegistry.Register(typeof (MvcApplication).Assembly, "~/Areas/Security/Views/Auth/");

			MongoConfiguration.Initialize(config =>
                            config.For<User>(c => c.ForProperty(u => u.Identity).Ignore()));
        }

    }

}