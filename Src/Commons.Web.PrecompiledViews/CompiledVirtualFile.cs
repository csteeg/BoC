using System.IO;
using System.Web.Hosting;

namespace Commons.Web.Mvc.PrecompiledViews
{
	public class CompiledVirtualFile : VirtualFile
	{
		public CompiledVirtualFile(string virtualPath): base(virtualPath)
		{
		}

		/// <summary>
		/// When overridden in a derived class, returns a read-only stream to the virtual resource.
		/// </summary>
		/// <returns>
		/// A read-only stream to the virtual file.
		/// </returns>
		public override Stream Open()
		{
			return Stream.Null;
		}
	}
}