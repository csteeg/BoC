using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.WebPages;
using Commons.Web.Mvc.PrecompiledViews;

namespace Commons.Web.Mvc.PrecompiledViews
{
	public class ApplicationPartRegistry : IApplicationPartRegistry
	{
		private readonly Dictionary<string, Type> registeredPaths = new Dictionary<string, Type>();
		private static readonly Type webPageType = typeof(WebPageRenderingBase);

		static ApplicationPartRegistry()
		{
			Instance = new ApplicationPartRegistry();
		}
		public static IApplicationPartRegistry Instance
		{
			get;
			private set;
		}

		public Type GetCompiledType(string virtualPath)
		{
			if (virtualPath == null) throw new ArgumentNullException("virtualPath");
			return registeredPaths[virtualPath.ToLower()];
		}

		public bool CompiledTypeExists(string virtualPath)
		{
			if (virtualPath == null) throw new ArgumentNullException("virtualPath");
			return registeredPaths.ContainsKey(virtualPath.ToLower());
		}

		public void Register(Assembly applicationPart)
		{
			Register(applicationPart, null);
		}
		
		public virtual void Register(Assembly applicationPart, string rootVirtualPath)
		{
			foreach (var type in applicationPart.GetTypes().Where(type => type.IsSubclassOf(webPageType)))
			{
				RegisterWebPage(type, rootVirtualPath);
			}
		}

		public virtual void RegisterWebPage(Type type)
		{
			RegisterWebPage(type, string.Empty);
		}
		public virtual void RegisterWebPage(Type type, string rootVirtualPath)
		{
			var attribute = webPageType.GetCustomAttributes(typeof(PageVirtualPathAttribute), false).Cast<PageVirtualPathAttribute>().SingleOrDefault<PageVirtualPathAttribute>();
			if (attribute != null)
			{
				var rootRelativeVirtualPath = GetRootRelativeVirtualPath(rootVirtualPath ?? "", attribute.VirtualPath);
				registeredPaths[rootRelativeVirtualPath.ToLower()] = type;
			}
		}
		internal static string GetRootRelativeVirtualPath(string rootVirtualPath, string pageVirtualPath)
		{
			string relativePath = pageVirtualPath;
			if (relativePath.StartsWith("~/", StringComparison.Ordinal))
			{
				relativePath = relativePath.Substring(2);
			}
			if (!rootVirtualPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				rootVirtualPath = rootVirtualPath + "/";
			}
			relativePath = VirtualPathUtility.Combine(rootVirtualPath, relativePath);
			if (!relativePath.StartsWith("~"))
			{
				return relativePath.StartsWith("/") ? "~/" + relativePath : "~" + relativePath;
			}
			return relativePath;
		}


	}
}
