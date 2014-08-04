using System;
using System.Configuration;
using BoC.InversionOfControl;

namespace BoC.Persistence.UmbracoGlass.DefaultSetupTask
{
    public class AutoUmbracoConfigurator : IContainerInitializer
    {
        private readonly IDependencyResolver _dependencyResolver;

        public AutoUmbracoConfigurator(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            var orm = ConfigurationManager.AppSettings["BoC.Persistence.Orm"];
            if (orm != null && !orm.Equals("umbracoglass", StringComparison.InvariantCultureIgnoreCase))
                return;

            if (!_dependencyResolver.IsRegistered<IUmbracoServiceProvider>())
            {
                _dependencyResolver.RegisterSingleton<IUmbracoServiceProvider, UmbracoServiceProvider>();
            }
        }
    }
}
