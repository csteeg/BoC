using System;
using BoC.InversionOfControl;
using BoC.Persistence.NHibernate;

namespace BoC.Persistence.Xpo.DefaultSetupTasks
{
    public class AutoXpoRepositoryGenerator : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;

        public AutoXpoRepositoryGenerator(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            var defaultBaseType = typeof(XpoRepository<>);
            var constructorParams = new Type[] { typeof(ISessionManager) };

            RepositoryGenerator.GenerateRepositories(dependencyResolver, defaultBaseType, constructorParams);
        }
    }
}