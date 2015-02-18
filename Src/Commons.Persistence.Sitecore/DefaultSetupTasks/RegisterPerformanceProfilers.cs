using BoC.InversionOfControl;
using BoC.Persistence.SitecoreGlass.Profiling;
using BoC.Profiling;

namespace BoC.Persistence.SitecoreGlass.DefaultSetupTasks
{

    public class ContainerRegistrations: IContainerInitializer
    {
        private readonly IDependencyResolver _resolver;

        public ContainerRegistrations(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public void Execute()
        {
            _resolver.RegisterSingleton<IPerformanceProfiler, SitecorePerformanceProfiler>();
        }
    }
}
