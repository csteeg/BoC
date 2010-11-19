using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Commons.Web.Mvc.PrecompiledViews
{
	/// <summary>
	/// Allow to embed precompiled razorviews (or any other webviewpage)
	/// Things that would be nice to have in Microsoft's code:
	///     - public access to VirtualPathFactoryManager.Instance
	///     - public access to VirtualPathFactoryManager.PageExists
	///		- let either store IVirtualPathFactory just the Type to construct instead of a constructor,
	///			or: let DictionaryBasedVirtualPathFactory use IoC if it's registered
	///		- allow to override BuildManager implementations, 
	///			that way we could hook into the framework on that level, instead of creating an extra viewengine
	/// </summary>
	public class VirtualPathFactoryManagerViewEngine : VirtualPathProviderViewEngine
	{
		internal static readonly string ViewStartFileName = "_ViewStart";

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
			return CreateView(controllerContext, partialPath, null);
		}

		protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
		{
			return new CompiledWebViewPageView(controllerContext, viewPath,
								 masterPath, true, 
								 ViewStartFileExtensions,
								 CreateInstance(viewPath)
								 );
		}

		protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
		{
			return ExistsMethod(virtualPath, true);
		}
		#region reflection stuff
		VirtualPathFactoryManager virtualPathFactoryManager;
		private VirtualPathFactoryManager VirtualPathFactoryManager
		{
			get
			{
				if (virtualPathFactoryManager == null)
				{
					var getter = typeof(VirtualPathFactoryManager).GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic);
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

		private Func<string, WebViewPage> createMethod;
		private Func<string, WebViewPage> CreateInstance
		{
			get
			{
				if (createMethod == null)
					createMethod = FindCreateMethod();
				return createMethod;
			}
		}
		private Func<string, WebViewPage> FindCreateMethod()
		{
			var method = typeof(VirtualPathFactoryManager)
				.GetMethod("CreateInstance", BindingFlags.NonPublic | BindingFlags.Instance, null,
						   new[] { typeof(String) }, null)
				.MakeGenericMethod(typeof (WebViewPage));

			return (Func<string, WebViewPage>)
				   Delegate.CreateDelegate(typeof(Func<string, WebViewPage>), VirtualPathFactoryManager, method, true);
		}
		#endregion
	}
}