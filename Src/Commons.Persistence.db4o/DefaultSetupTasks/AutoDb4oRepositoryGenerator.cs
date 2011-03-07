using System;
using System.Configuration;
using BoC.InversionOfControl;
using BoC.Persistence;
using BoC.Persistence.db4o.Repository;

namespace BoC.Persistence.db4o.DefaultSetupTasks
{
    public class AutoDb4oRepositoryGenerator : IContainerInitializer
    {
        private readonly IDependencyResolver _dependencyResolver;

        public AutoDb4oRepositoryGenerator(IDependencyResolver dependencyResolver)
        {
            this._dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            var orm = ConfigurationManager.AppSettings["BoC.Persistence.Orm"];
            if (orm != null && !orm.Equals("db4o", StringComparison.InvariantCultureIgnoreCase))
                return;

            var defaultBaseType = typeof(Db4oRepository<>);
            var constructorParams = new[] { typeof(ISessionManager) };

            RepositoryGenerator.GenerateRepositories(_dependencyResolver, defaultBaseType, constructorParams);
        }
    }
}