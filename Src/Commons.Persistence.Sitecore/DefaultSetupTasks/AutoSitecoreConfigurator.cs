using System;
using System.Configuration;
using BoC.InversionOfControl;
using BoC.Persistence.SitecoreGlass.UnitOfWork;
using BoC.UnitOfWork;

namespace BoC.Persistence.SitecoreGlass.DefaultSetupTasks
{
    public class AutoSitecoreConfigurator : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;

        public AutoSitecoreConfigurator(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            var orm = ConfigurationManager.AppSettings["BoC.Persistence.Orm"];
            if (orm != null && !orm.Equals("sitecoreglass", StringComparison.InvariantCultureIgnoreCase))
                return;

            if (!dependencyResolver.IsRegistered<IDatabaseProvider>())
            {
                dependencyResolver.RegisterSingleton<IDatabaseProvider, ContextDatabaseProvider>();
            }

            if (!dependencyResolver.IsRegistered<ISitecoreServiceProvider>())
            {
                dependencyResolver.RegisterSingleton<ISitecoreServiceProvider, SitecoreServiceProvider>();
            }

            if (!dependencyResolver.IsRegistered<IIndexNameProvider>())
            {
                dependencyResolver.RegisterSingleton<IIndexNameProvider, ContentSearchContextIndexNameProvider>();
            }

            if (!dependencyResolver.IsRegistered<IUnitOfWork>())
            {
                dependencyResolver.RegisterType<IUnitOfWork, SitecoreUnitOfWork>();
            }

            if (!dependencyResolver.IsRegistered<IProviderSearchContextProvider>())
            {
                dependencyResolver.RegisterSingleton<IProviderSearchContextProvider, SitecoreUnitOfWorkIndexSearchContextProvider>();
            }
            
        }
    }
}