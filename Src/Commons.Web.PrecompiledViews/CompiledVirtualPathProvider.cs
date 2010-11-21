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
				CompiledViewExists(virtualPath)
				|| virtualPathProvider.FileExists(virtualPath);
		}

		private bool CompiledViewExists(string virtualPath)
		{
			if (virtualPath.StartsWith("/"))
				virtualPath = "~" + virtualPath;
			return VirtualPathFactories.Any(f => f.Exists(virtualPath));
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
			return
				virtualPathProvider.FileExists(virtualPath)
					? virtualPathProvider.GetFile(virtualPath)
					: CompiledViewExists(virtualPath)
					  	? new CompiledVirtualFile(virtualPath)
					  	: null;
		}

		#region reflection stuff

		static VirtualPathFactoryManager virtualPathFactoryManager;
		static internal VirtualPathFactoryManager VirtualPathFactoryManager
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

		private List<IVirtualPathFactory> virtualPathFactories;
		private List<IVirtualPathFactory> VirtualPathFactories
		{
			get
			{
				if (virtualPathFactories == null)
					virtualPathFactories = FindVirtualPathFactories();
				return virtualPathFactories;
			}
		}
		private List<IVirtualPathFactory> FindVirtualPathFactories()
		{
			return
				typeof (VirtualPathFactoryManager)
					.GetField("_virtualPathFactories", BindingFlags.NonPublic | BindingFlags.Instance)
					.GetValue(VirtualPathFactoryManager) as List<IVirtualPathFactory>;
		}
		#endregion
	}
}