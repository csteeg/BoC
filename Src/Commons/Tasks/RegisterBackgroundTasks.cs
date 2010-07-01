using System;
using System.Linq;
using BoC.InversionOfControl;

namespace BoC.Tasks
{
    public class RegisterBackgroundTasks: IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;
        public RegisterBackgroundTasks(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        volatile private static Func<Type, bool> taskFilter = t => true;
        public static Func<Type, bool> TaskFilter
        {
            get { return taskFilter; }
            set { taskFilter = value; }
        }


        public void Execute()
        {
            var tasks = AppDomain.CurrentDomain.GetAssemblies().ToList()
                    .SelectMany(s => s.GetTypes())
                    .Where(t => t.IsClass && !t.IsAbstract && !t.Assembly.GetName().Name.StartsWith("System") 
                            && typeof(IBackgroundTask).IsAssignableFrom(t) && TaskFilter(t));

            foreach (var type in tasks)
            {
                dependencyResolver.RegisterType(typeof(IBackgroundTask), type);
            }
        }
    }
}