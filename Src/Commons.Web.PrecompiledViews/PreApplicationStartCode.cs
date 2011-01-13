using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using BoC.Web.Mvc.PrecompiledViews;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStartCode), "Start")]

namespace BoC.Web.Mvc.PrecompiledViews
{
	public static class PreApplicationStartCode
	{
		private static bool _startWasCalled;

		public static void Start()
		{
			if (_startWasCalled)
			{
				return;
			}
			_startWasCalled = true;

			//be sure default buildproviders are registered first
			System.Web.WebPages.PreApplicationStartCode.Start();
			System.Web.WebPages.Razor.PreApplicationStartCode.Start();

			HostingEnvironment.RegisterVirtualPathProvider(new CompiledVirtualPathProvider());
			BuildProvider.RegisterBuildProvider(".cshtml", typeof(CompiledRazorBuildProvider));
		}
	}
}