using BoC.InversionOfControl.Configuration;
using BoC.Persistence.NHibernate;

namespace BoC.Persistence.DefaultSetupTasks
{
    public class AutoNhibernateRepositoryGenerator : IContainerInitializer
    {
        public void Execute()
        {
            NHibernateConfigHelper.AutoRegisterRepositories();
        }
    }
}