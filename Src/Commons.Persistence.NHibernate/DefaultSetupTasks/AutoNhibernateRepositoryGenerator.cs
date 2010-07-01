using BoC.InversionOfControl;
using BoC.Persistence.NHibernate;

namespace BoC.Persistence.DefaultSetupTasks
{
    public class AutoNhibernateRepositoryGenerator : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;

        public AutoNhibernateRepositoryGenerator(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            NHibernateConfigHelper.AutoRegisterRepositories(dependencyResolver);
        }
    }
}