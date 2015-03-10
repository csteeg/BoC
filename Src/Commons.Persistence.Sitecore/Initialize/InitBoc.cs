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
            BoC.Initializer.Execute();
        }
        public static void Disable()
        {
            //disable startup init of BoC, need to go after sitecore, so do it in a pipeline or in postapplicationstartup if the pipeline version hasn't been triggered
            ApplicationStarterHttpModule.Disabled = true;
        }
    }
}
