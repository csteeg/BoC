using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.WebPages;
using BoC.Security.Model;
using BoC.Web;
using Norm.Configuration;
using ToDoList;

namespace ToDoList
{
	public class VirtualPathFactoryManagerViewEngine : VirtualPathProviderViewEngine
	{
		public VirtualPathFactoryManagerViewEngine()
		{
            AreaViewLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.vbhtml"
            };
            AreaMasterLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.vbhtml"
            };
            AreaPartialViewLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.vbhtml"
            };

            ViewLocationFormats = new[] {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.vbhtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Shared/{0}.vbhtml"
            };
            MasterLocationFormats = new[] {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.vbhtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Shared/{0}.vbhtml"
            };
            PartialViewLocationFormats = new[] {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.vbhtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Shared/{0}.vbhtml"
            };

            ViewStartFileExtensions = new[] {
                "cshtml",
                "vbhtml",
            };
        }

		protected string[] ViewStartFileExtensions { get; set; }

		protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
		{
			throw new NotImplementedException();
		}

		protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
		{
			return new RazorView(controllerContext, viewPath,
									 layoutPath: masterPath, runViewStartPages: true, viewStartFileExtensions: ViewStartFileExtensions);
		}

		protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
		{
			return ExistsMethod(virtualPath, true);
		}

		VirtualPathFactoryManager virtualPathFactoryManager;
		private VirtualPathFactoryManager VirtualPathFactoryManager
		{
			get
			{
				if (virtualPathFactoryManager == null)
				{
					var getter = typeof (VirtualPathFactoryManager)
						.GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic);
					virtualPathFactoryManager = getter.GetValue(null, null) as VirtualPathFactoryManager;
				}
				return virtualPathFactoryManager;

			}
		}

		private Func<string, bool, bool> existsMethod;
		private Func<string, bool, bool> ExistsMethod
		{
			get
			{
				if (existsMethod == null)
					existsMethod = FindExistsMethod();
				return existsMethod;
			}
		}
		private Func<string, bool, bool> FindExistsMethod()
		{
			var method = typeof(VirtualPathFactoryManager)
				.GetMethod("PageExists", BindingFlags.NonPublic | BindingFlags.Instance, null,
					new[] { typeof(String), typeof(bool) }, null);

			return (Func<string, bool, bool>)
				Delegate.CreateDelegate(typeof(Func<string, bool, bool>), VirtualPathFactoryManager, method, true);
		}

	}

	public class MvcApplication : CommonHttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        protected override void InitializeApplication()
        {
			base.InitializeApplication();
            ViewEngines.Engines.Add(new VirtualPathFactoryManagerViewEngine());
            System.Web.WebPages.ApplicationPart.Register(new ApplicationPart(typeof(MvcApplication).Assembly, "~/Areas/Security/Views/Auth/"));

            MongoConfiguration.Initialize(config =>
                            config.For<User>(c => c.ForProperty(u => u.Identity).Ignore()));
        }
    }
}