using System;
using System.Configuration;
using BoC.Helpers;
using BoC.InversionOfControl;
using BoC.Logging;

namespace BoC.Persistence.SitecoreGlass.DefaultSetupTasks
{
    public class AutoSitecoreRepositoryGenerator : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly IAppDomainHelper[] _appDomainHelpers;

        public AutoSitecoreRepositoryGenerator(IDependencyResolver dependencyResolver, IAppDomainHelper[] appDomainHelpers)
        {
            this.dependencyResolver = dependencyResolver;
            _appDomainHelpers = appDomainHelpers;
        }

        public void Execute()
        {
            var orm = ConfigurationManager.AppSettings["BoC.Persistence.Orm"];
            if (orm != null && !orm.Equals("sitecoreglass", StringComparison.InvariantCultureIgnoreCase))
                return;

            var defaultBaseType = typeof(SitecoreRepository<>);
            var constructorParams = new[] { typeof(IDatabaseProvider), typeof(ISitecoreServiceProvider), typeof(IProviderSearchContextProvider), typeof(ILogger) };

            RepositoryGenerator.GenerateRepositories(dependencyResolver, defaultBaseType, constructorParams, _appDomainHelpers);
        }
    }
}