using System;
using BoC.Helpers;
using BoC.InversionOfControl;
using BoC.Persistence.NHibernate;

namespace BoC.Persistence.Xpo.DefaultSetupTasks
{
    public class AutoXpoRepositoryGenerator : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly IAppDomainHelper[] _appDomainHelpers;

        public AutoXpoRepositoryGenerator(IDependencyResolver dependencyResolver, IAppDomainHelper[] appDomainHelpers)
        {
            this.dependencyResolver = dependencyResolver;
            _appDomainHelpers = appDomainHelpers;
        }

        public void Execute()
        {
            var defaultBaseType = typeof(XpoRepository<>);
            var constructorParams = new Type[] { typeof(ISessionManager) };

            RepositoryGenerator.GenerateRepositories(dependencyResolver, defaultBaseType, constructorParams, _appDomainHelpers);
        }
    }
}