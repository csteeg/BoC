using System;
using System.Collections.Generic;
using System.Linq;
using BoC.Helpers;
using BoC.InversionOfControl;

namespace BoC.Tasks
{
    public class RegisterBootstrapTasks: IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly IAppDomainHelper[] appDomainHelpers;

        public RegisterBootstrapTasks(IDependencyResolver dependencyResolver, IAppDomainHelper[] appDomainHelpers)
        {
            this.dependencyResolver = dependencyResolver;
            this.appDomainHelpers = appDomainHelpers;
        }

        public void Execute()
        {
            var tasks = appDomainHelpers.SelectMany(helper => helper.GetTypes(t => Bootstrapper.TaskFilters.All(func => func(t))));
            foreach (var t in tasks)
            {
                dependencyResolver.RegisterType(typeof(IBootstrapperTask), t);
            }
            dependencyResolver.RegisterSingleton<Bootstrapper, Bootstrapper>();
        }
    }
}