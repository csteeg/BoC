using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.WebPages;
using BoC.Web.Mvc.PrecompiledViews;

namespace BoC.Web.Mvc.PrecompiledViews
{
	public class DictionaryBasedApplicationPartRegistry : IApplicationPartRegistry
	{
		private static readonly Type webPageType = typeof(WebPageRenderingBase); 
		private readonly Dictionary<string, Type> registeredPaths = new Dictionary<string, Type>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="virtualPath"></param>
		/// <returns></returns>
		public virtual Type GetCompiledType(string virtualPath)
		{
			if (virtualPath == null) throw new ArgumentNullException("virtualPath");
			if (virtualPath.StartsWith("/"))
				virtualPath = VirtualPathUtility.ToAppRelative(virtualPath);
			if (!virtualPath.StartsWith("~"))
				virtualPath = !virtualPath.StartsWith("/") ? "~/" + virtualPath : "~" + virtualPath;
			virtualPath = virtualPath.ToLower();
			return registeredPaths.ContainsKey(virtualPath)
			       	? registeredPaths[virtualPath]
			       	: null;
		}

		public void Register(Assembly applicationPart)
		{
			((IApplicationPartRegistry)this).Register(applicationPart, null);
		}

		public virtual void Register(Assembly applicationPart, string rootVirtualPath)
		{
			foreach (var type in applicationPart.GetTypes().Where(type => type.IsSubclassOf(webPageType)))
			{
				((IApplicationPartRegistry)this).RegisterWebPage(type, rootVirtualPath);
			}
		}

		public void RegisterWebPage(Type type)
		{
			((IApplicationPartRegistry)this).RegisterWebPage(type, string.Empty);
		}

		public virtual void RegisterWebPage(Type type, string rootVirtualPath)
		{
			var attribute = type.GetCustomAttributes(typeof(PageVirtualPathAttribute), false).Cast<PageVirtualPathAttribute>().SingleOrDefault<PageVirtualPathAttribute>();
			if (attribute != null)
			{
				var rootRelativeVirtualPath = GetRootRelativeVirtualPath(rootVirtualPath ?? "", attribute.VirtualPath);
				registeredPaths[rootRelativeVirtualPath.ToLower()] = type;
			}
		}

		static string GetRootRelativeVirtualPath(string rootVirtualPath, string pageVirtualPath)
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
				return !relativePath.StartsWith("/") ? "~/" + relativePath : "~" + relativePath;
			}
			return relativePath;
		}
	}
}
