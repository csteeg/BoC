using System;
using System.Collections.Generic;
using System.Linq;
using BoC.Helpers;
using BoC.InversionOfControl;

namespace BoC.Tasks
{
    public class RegisterBackgroundTasks: IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly IAppDomainHelper[] appDomainHelpers;

        public RegisterBackgroundTasks(IDependencyResolver dependencyResolver, IAppDomainHelper[] appDomainHelpers)
        {
            this.dependencyResolver = dependencyResolver;
            this.appDomainHelpers = appDomainHelpers;
        }

        volatile private static ICollection<Func<Type, bool>> taskFilters = new List<Func<Type,bool>>(){type => true};
        public static ICollection<Func<Type, bool>> TaskFilters
        {
            get { return taskFilters; }
        }


        public void Execute()
        {
            var tasks = appDomainHelpers.SelectMany(
                helpers => helpers.GetTypes(
                    t => t.IsClass && !t.IsAbstract && !t.Assembly.GetName().Name.StartsWith("System")
                         && typeof (IBackgroundTask).IsAssignableFrom(t)
                         && TaskFilters.All(func => func(t))));
            foreach (var type in tasks)
            {
                dependencyResolver.RegisterType(typeof(IBackgroundTask), type);
            }
        }
    }
}