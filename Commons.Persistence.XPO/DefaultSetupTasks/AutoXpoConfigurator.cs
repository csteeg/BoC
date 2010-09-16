using BoC.InversionOfControl;
using BoC.Persistence.Xpo.UnitOfWork;
using BoC.UnitOfWork;

namespace BoC.Persistence.Xpo.DefaultSetupTasks
{
    public class AutoXpoConfigurator : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;

        public AutoXpoConfigurator(IDependencyResolver dependencyResolver)
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
                dependencyResolver.RegisterSingleton<ISessionManager, XpoUnitOfWorkSessionManager>();
            }

            if (!dependencyResolver.IsRegistered<IUnitOfWork>())
            {
                dependencyResolver.RegisterType<IUnitOfWork, XpoUnitOfWork>();
            }
        }
    }
}