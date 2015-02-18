using BoC.Profiling;
using BoC.Tasks;

namespace BoC.Persistence.SitecoreGlass.DefaultSetupTasks
{
    public class EnableProfiling: IBootstrapperTask
    {
        public void Execute()
        {
            Profiler.Enabled = global::Sitecore.Configuration.Settings.GetBoolSetting("BoC.Profiler.Enabled", false);
        }
    }
}
