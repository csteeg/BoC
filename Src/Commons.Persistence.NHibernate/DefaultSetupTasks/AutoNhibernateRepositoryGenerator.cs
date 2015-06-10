using System;
using System.Configuration;
using BoC.Helpers;
using BoC.InversionOfControl;
using BoC.Persistence.NHibernate;

namespace BoC.Persistence.DefaultSetupTasks
{
    public class AutoNhibernateRepositoryGenerator : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly IAppDomainHelper[] _appDomainHelpers;

        public AutoNhibernateRepositoryGenerator(IDependencyResolver dependencyResolver, IAppDomainHelper[] appDomainHelpers)
        {
            this.dependencyResolver = dependencyResolver;
            _appDomainHelpers = appDomainHelpers;
        }

        public void Execute()
        {
            var orm = ConfigurationManager.AppSettings["BoC.Persistence.Orm"];
            if (orm != null && !orm.Equals("nhibernate", StringComparison.InvariantCultureIgnoreCase))
                return;

            var defaultBaseType = typeof(NHRepository<>);
            var constructorParams = new[] { typeof(ISessionManager) };

            RepositoryGenerator.GenerateRepositories(dependencyResolver, defaultBaseType, constructorParams, _appDomainHelpers);
        }
    }
}