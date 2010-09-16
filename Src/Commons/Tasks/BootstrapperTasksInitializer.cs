using System;
using System.Linq;
using BoC.Helpers;
using BoC.InversionOfControl;

namespace BoC.Tasks
{
    public class BootstrapperTasksInitializer: IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly IAppDomainHelper[] appDomainHelpers;

        public BootstrapperTasksInitializer(IDependencyResolver dependencyResolver, IAppDomainHelper appDomainHelpers)
        {
            this.dependencyResolver = dependencyResolver;
            this.appDomainHelpers = new[] { appDomainHelpers };
        }

        volatile private static Func<Type, bool> taskFilter = t => true;
        public static Func<Type, bool> TaskFilter
        {
            get { return taskFilter; }
            set { taskFilter = value; }
        }

        public void RegisterAllTasks()
        {
            var tasks = appDomainHelpers
                        .SelectMany(helper => helper
                            .GetTypes(t => t.IsClass && !t.IsAbstract && typeof(IBootstrapperTask).IsAssignableFrom(t) && TaskFilter(t)));
            foreach (var t in tasks)
            {
                dependencyResolver.RegisterType(typeof(IBootstrapperTask), t);
            }

        }

        public void Run()
        {
            var tasks = dependencyResolver.ResolveAll<IBootstrapperTask>();
            foreach (var task in tasks)
            {
                task.Execute();
            }
        }

        public void Execute()
        {
            RegisterAllTasks();
            Run();
        }
    }
}