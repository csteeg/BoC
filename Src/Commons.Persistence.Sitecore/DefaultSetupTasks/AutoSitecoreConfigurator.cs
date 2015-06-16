using System;
using System.Configuration;
using BoC.InversionOfControl;
using BoC.Persistence.SitecoreGlass.DataContext;
using BoC.DataContext;

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
                dependencyResolver.RegisterSingleton<IDatabaseProvider, SitecoreDataContextDatabaseProvider>();
            }

            if (!dependencyResolver.IsRegistered<ISitecoreServiceProvider>())
            {
                dependencyResolver.RegisterSingleton<ISitecoreServiceProvider, SitecoreServiceProvider>();
            }

            if (!dependencyResolver.IsRegistered<IIndexNameProvider>())
            {
                dependencyResolver.RegisterSingleton<IIndexNameProvider, ContentSearchContextIndexNameProvider>();
            }

            if (!dependencyResolver.IsRegistered<IDataContext>())
            {
                dependencyResolver.RegisterType<IDataContext, SitecoreDataContext>();
            }

            if (!dependencyResolver.IsRegistered<IProviderSearchContextProvider>())
            {
                dependencyResolver.RegisterSingleton<IProviderSearchContextProvider, SitecoreDataContextIndexSearchContextProvider>();
            }
            
        }
    }
}