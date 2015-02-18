namespace BoC.Profiling
{
    public interface IPerformanceProfiler
    {
        void BeginSession(string key);
        void EndSession(string key);
    }
}
