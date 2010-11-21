using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.WebPages;
using Commons.Web.Mvc.PrecompiledViews;

namespace Commons.Web.Mvc.PrecompiledViews
{
	public static class ApplicationPartRegistry
	{
		static ApplicationPartRegistry()
		{
			Instance = new DictionaryBasedApplicationPartRegistry();
		}
		public static IApplicationPartRegistry Instance
		{
			get;
			set;
		}

		public static Type GetCompiledType(string virtualPath)
		{
			return Instance.GetCompiledType(virtualPath);
		}

		public static void Register(Assembly applicationPart)
		{
			Register(applicationPart);
		}

		public static void Register(Assembly applicationPart, string rootVirtualPath)
		{
			Instance.Register(applicationPart, rootVirtualPath);
		}

		public static void RegisterWebPage(Type type)
		{
			RegisterWebPage(type);
		}
		public static void RegisterWebPage(Type type, string rootVirtualPath)
		{
			Instance.RegisterWebPage(type, rootVirtualPath);
		}
	}
}
