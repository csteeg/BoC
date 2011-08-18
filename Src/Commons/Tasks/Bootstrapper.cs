using System;
using System.Collections.Generic;
using System.Linq;
using BoC.Helpers;
using BoC.InversionOfControl;

namespace BoC.Tasks
{
    public class Bootstrapper
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly IAppDomainHelper[] appDomainHelpers;

        public Bootstrapper(IDependencyResolver dependencyResolver, IAppDomainHelper[] appDomainHelpers)
        {
            this.dependencyResolver = dependencyResolver;
            this.appDomainHelpers = appDomainHelpers;
        }

        ICollection<Func<Type, bool>> taskFilters = new List<Func<Type, bool>>() 
            { 
                type => typeof(IBootstrapperTask).IsAssignableFrom(type),
                type => !type.IsAbstract,
                type => type.IsClass
            };

        private void RegisterAllTasks()
        {
            var tasks = appDomainHelpers.SelectMany(helper => helper.GetTypes(t => taskFilters.All(func => func(t))));
            foreach (var t in tasks)
            {
                dependencyResolver.RegisterType(typeof(IBootstrapperTask), t);
            }
        }

        private void ExecuteTasks()
        {
            var tasks = dependencyResolver.ResolveAll<IBootstrapperTask>();
            foreach (var task in tasks)
            {
                task.Execute();
            }
        }

        public void Run()
        {
            RegisterAllTasks();
            ExecuteTasks();
        }
    }
}