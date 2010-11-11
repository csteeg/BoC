using System;
using System.Linq;
using BoC.Helpers;
using BoC.InversionOfControl;

namespace BoC.Tasks
{
    public class RegisterBackgroundTasks: IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly IAppDomainHelper[] appDomainHelpers;

        public RegisterBackgroundTasks(IDependencyResolver dependencyResolver, IAppDomainHelper appDomainHelpers)
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


        public void Execute()
        {
            var tasks = appDomainHelpers.SelectMany(
                            helpers => helpers.GetTypes(
                                t => t.IsClass && !t.IsAbstract && !t.Assembly.GetName().Name.StartsWith("System") 
                                && typeof(IBackgroundTask).IsAssignableFrom(t) && TaskFilter(t)));

            foreach (var type in tasks)
            {
                dependencyResolver.RegisterType(typeof(IBackgroundTask), type);
            }
        }
    }
}