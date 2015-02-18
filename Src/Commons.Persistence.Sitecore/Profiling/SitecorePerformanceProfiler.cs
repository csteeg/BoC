using BoC.Profiling;

namespace BoC.Persistence.SitecoreGlass.Profiling
{
    public class SitecorePerformanceProfiler: IPerformanceProfiler
    {
        public void BeginSession(string key)
        {
            Sitecore.Diagnostics.Profiler.StartOperation(key);
        }

        public void EndSession(string key)
        {
            Sitecore.Diagnostics.Profiler.EndOperation(key);
        }

    }
}
