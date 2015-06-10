using System;
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
            var appdomainhelper = AppDomainHelper.CreateDefault();
            appdomainhelper.AssemblyFilters.Add(a =>
                !a.FullName.StartsWith("system.", StringComparison.InvariantCultureIgnoreCase) &&
                !a.FullName.StartsWith("glimpse.", StringComparison.InvariantCultureIgnoreCase) &&
                !a.FullName.StartsWith("ironruby.", StringComparison.InvariantCultureIgnoreCase) &&
                !a.FullName.StartsWith("sitecore.", StringComparison.InvariantCultureIgnoreCase) &&
                !a.FullName.StartsWith("lucene.", StringComparison.InvariantCultureIgnoreCase) &&
                !a.FullName.StartsWith("microsoft.", StringComparison.InvariantCultureIgnoreCase) &&
                !a.FullName.StartsWith("telerik.", StringComparison.InvariantCultureIgnoreCase) &&
                !a.FullName.StartsWith("webgrease.", StringComparison.InvariantCultureIgnoreCase) &&
                !a.FullName.StartsWith("yahoo.", StringComparison.InvariantCultureIgnoreCase) &&
                !a.FullName.StartsWith("stimulsoft.", StringComparison.InvariantCultureIgnoreCase) &&
                !a.FullName.StartsWith("mongodb.", StringComparison.InvariantCultureIgnoreCase) &&
                !a.FullName.StartsWith("google.", StringComparison.InvariantCultureIgnoreCase)
            );
            BoC.Initializer.Execute(appdomainhelper);
        }
        public static void Disable()
        {
            //disable startup init of BoC, need to go after sitecore, so do it in a pipeline or in postapplicationstartup if the pipeline version hasn't been triggered
            ApplicationStarterHttpModule.Disabled = true;
        }
    }
}
