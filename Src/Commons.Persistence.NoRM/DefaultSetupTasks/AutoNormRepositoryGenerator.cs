using System;
using System.Configuration;
using BoC.Helpers;
using BoC.InversionOfControl;
using BoC.Persistence.Norm;

namespace BoC.Persistence.Norm.DefaultSetupTasks
{
    public class AutoNormRepositoryGenerator : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly IAppDomainHelper[] _appDomainHelpers;

        public AutoNormRepositoryGenerator(IDependencyResolver dependencyResolver, IAppDomainHelper[] appDomainHelpers)
        {
            this.dependencyResolver = dependencyResolver;
            _appDomainHelpers = appDomainHelpers;
        }

        public void Execute()
        {
            var orm = ConfigurationManager.AppSettings["BoC.Persistence.Orm"];
            if (orm != null && !orm.Equals("norm", StringComparison.InvariantCultureIgnoreCase))
                return;

            var defaultBaseType = typeof(NormRepository<>);
            var constructorParams = new Type[] { typeof(ISessionManager) };

            RepositoryGenerator.GenerateRepositories(dependencyResolver, defaultBaseType, constructorParams, _appDomainHelpers);
        }
    }
}