using System;
using System.Linq;
using BoC.InversionOfControl;

namespace BoC.Tasks
{
    public class BootstrapperTasksInitializer: IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;
        public BootstrapperTasksInitializer(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        volatile private static Func<Type, bool> taskFilter = t => true;
        public static Func<Type, bool> TaskFilter
        {
            get { return taskFilter; }
            set { taskFilter = value; }
        }

        public void RegisterAllTasks()
        {
            var tasks = AppDomain.CurrentDomain.GetAssemblies().ToList()
                    .SelectMany(s => s.GetTypes())
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IBootstrapperTask).IsAssignableFrom(t) && TaskFilter(t));
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