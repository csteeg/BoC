using System;
using System.Reflection;
using BoC.Helpers;
using BoC.Persistence.SitecoreGlass.Initialize;
using BoC.Web;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(InitBoc), "Start")]
[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(InitBoc), "Disable")]
namespace BoC.Persistence.SitecoreGlass.Initialize
{

	public class InitBoc
	{
		public static void Start()
		{
			var appdomainhelper = CreateSitecoreAppDomainHelper();
			Initializer.Execute(appdomainhelper);
		}

		public static IAppDomainHelper CreateSitecoreAppDomainHelper()
		{
			var appdomainhelper = AppDomainHelper.CreateDefault();
			appdomainhelper.AssemblyFilters.Add(
				a => !a.FullName.StartsWith("system.", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("glimpse.", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("ironruby.", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("lucene.", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("microsoft.", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("telerik.", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("webgrease.", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("yahoo.", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("stimulsoft.", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("HtmlAgilityPack", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("Lucene.Net", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("RadEditor", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("Stimulsoft", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("ComponentArt.", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("Antlr3.Runtime", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("Newtonsoft.Json", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("google.", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("ucommerce.", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("unicorn.", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("xunit.", StringComparison.InvariantCultureIgnoreCase) &&
					 !a.FullName.StartsWith("mongodb.", StringComparison.InvariantCultureIgnoreCase) &&
					 (!a.FullName.StartsWith("sitecore.", StringComparison.InvariantCultureIgnoreCase) ||
					  a.FullName.StartsWith("sitecore.feature", StringComparison.InvariantCultureIgnoreCase) ||
					  a.FullName.StartsWith("sitecore.foundation", StringComparison.InvariantCultureIgnoreCase) ||
					  a.FullName.StartsWith("sitecore.common", StringComparison.InvariantCultureIgnoreCase) ||
					  a.FullName.StartsWith("sitecore.habitat", StringComparison.InvariantCultureIgnoreCase)
					 ));
			return appdomainhelper;
		}

		public static void Disable()
		{
			//disable startup init of BoC, need to go after sitecore, so do it in a pipeline or in postapplicationstartup if the pipeline version hasn't been triggered
			ApplicationStarterHttpModule.Disabled = true;
		}
	}
}
