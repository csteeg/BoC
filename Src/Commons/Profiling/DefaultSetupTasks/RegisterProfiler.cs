using System.ComponentModel;
using BoC.ComponentModel.TypeExtension;
using BoC.InversionOfControl;

namespace BoC.Profiling.DefaultSetupTasks
{
    public class RegisterProfiler : IContainerInitializer
    {
        private readonly IDependencyResolver _resolver;

        public RegisterProfiler(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public void Execute()
        {
            _resolver.RegisterSingleton<Profiler, Profiler>();
        }
   }
}