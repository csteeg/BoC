using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using System.Web.WebPages;

namespace Commons.Web.Mvc.PrecompiledViews
{
	public class CompiledVirtualPathProvider: VirtualPathProvider
	{
		private readonly VirtualPathProvider virtualPathProvider;

		public CompiledVirtualPathProvider(VirtualPathProvider defaultPathProvider)
		{
			this.virtualPathProvider = defaultPathProvider;
		}

		/// <summary>
		/// Gets a value that indicates whether a file exists in the virtual file system.
		/// </summary>
		/// <returns>
		/// true if the file exists in the virtual file system; otherwise, false.
		/// </returns>
		/// <param name="virtualPath">The path to the virtual file.</param>
		public override bool FileExists(string virtualPath)
		{
			return
				GetCompiledType(virtualPath) != null 
				|| virtualPathProvider.FileExists(virtualPath);
		}

		private Type GetCompiledType(string virtualPath)
		{
			return ApplicationPartRegistry.Instance.GetCompiledType(virtualPath);
		}

		/// <summary>
		/// Gets a virtual file from the virtual file system.
		/// </summary>
		/// <returns>
		/// A descendent of the <see cref="T:System.Web.Hosting.VirtualFile"/> class that represents a file in the virtual file system.
		/// </returns>
		/// <param name="virtualPath">The path to the virtual file.</param>
		public override VirtualFile GetFile(string virtualPath)
		{
			if (virtualPathProvider.FileExists(virtualPath))
			{
				return virtualPathProvider.GetFile(virtualPath);
			}
			var compiledType = GetCompiledType(virtualPath);
			if (compiledType != null)
			{
				return new CompiledVirtualFile(virtualPath, compiledType);
			}
			return null;
		}

	}
}