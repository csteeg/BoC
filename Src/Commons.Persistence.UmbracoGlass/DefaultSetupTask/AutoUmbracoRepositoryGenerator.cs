using System;
using System.Configuration;
using BoC.InversionOfControl;

namespace BoC.Persistence.UmbracoGlass.DefaultSetupTask
{
    public class AutoUmbracoRepositoryGenerator : IContainerInitializer
    {
        private readonly IDependencyResolver _dependencyResolver;

        public AutoUmbracoRepositoryGenerator(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            var orm = ConfigurationManager.AppSettings["BoC.Persistence.Orm"];
            if (orm != null && !orm.Equals("umbracoglass", StringComparison.InvariantCultureIgnoreCase))
                return;

            var defaultBaseType = typeof(UmbracoRepository<>);
            var constructorParams = new[] {typeof (IUmbracoServiceProvider)};

            RepositoryGenerator.GenerateRepositories(_dependencyResolver, defaultBaseType, constructorParams);
        }
    }
}
