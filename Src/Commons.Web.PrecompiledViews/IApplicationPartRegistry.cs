using System;
using System.Reflection;

namespace Commons.Web.Mvc.PrecompiledViews
{
	public interface IApplicationPartRegistry
	{
		Type GetCompiledType(string virtualPath);
		bool CompiledTypeExists(string virtualPath);
		void Register(Assembly applicationPart);
		void Register(Assembly applicationPart, string rootVirtualPath);
		void RegisterWebPage(Type type);
		void RegisterWebPage(Type type, string rootVirtualPath);
	}
}