using BoC.Persistence.SitecoreGlass.DefaultSetupTasks;
using BoC.Persistence.SitecoreGlass.Initialize;
using BoC.Web;
using Sitecore.Pipelines;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(InitBoc), "Start")]
[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(InitBoc), "Disable")]
namespace BoC.Persistence.SitecoreGlass.Initialize
{

    public class InitBoc
    {
        public static void Start()
        {
            BoC.Initializer.Execute();
        }
        public static void Disable()
        {
            //disable startup init of BoC, need to go after sitecore, so do it in a pipeline
            ApplicationStarterHttpModule.Disabled = true;
        }
    }
}
