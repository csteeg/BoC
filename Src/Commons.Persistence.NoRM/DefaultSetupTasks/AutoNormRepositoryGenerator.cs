using System;
using System.Configuration;
using BoC.InversionOfControl;
using BoC.Persistence.Norm;

namespace BoC.Persistence.Norm.DefaultSetupTasks
{
    public class AutoNormRepositoryGenerator : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;

        public AutoNormRepositoryGenerator(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            var orm = ConfigurationManager.AppSettings["BoC.Persistence.Orm"];
            if (orm != null && !orm.Equals("norm", StringComparison.InvariantCultureIgnoreCase))
                return;

            var defaultBaseType = typeof(NormRepository<>);
            var constructorParams = new Type[] { typeof(ISessionManager) };

            RepositoryGenerator.GenerateRepositories(dependencyResolver, defaultBaseType, constructorParams);
        }
    }
}