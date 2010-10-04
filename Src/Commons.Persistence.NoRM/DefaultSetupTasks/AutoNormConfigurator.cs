using BoC.InversionOfControl;
using BoC.Persistence.Norm;
using BoC.Persistence.Norm.UnitOfWork;
using BoC.UnitOfWork;

namespace BoC.Persistence.Norm.DefaultSetupTasks
{
    public class AutoNormConfigurator : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;

        public AutoNormConfigurator(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            if (!dependencyResolver.IsRegistered<ISessionFactory>())
            {
                dependencyResolver.RegisterType<ISessionFactory, DefaultSessionFactory>();
            }

            if (!dependencyResolver.IsRegistered<ISessionManager>())
            {
                //IoC.RegisterSingleton<ISessionManager, CurrentContextSessionManager>();
                dependencyResolver.RegisterSingleton<ISessionManager, NormUnitOfWorkSessionManager>();
            }

            if (!dependencyResolver.IsRegistered<IUnitOfWork>())
            {
                dependencyResolver.RegisterType<IUnitOfWork, NormUnitOfWork>();
            }
        }
    }
}