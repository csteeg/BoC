using BoC.InversionOfControl;
using BoC.Persistence.Xpo.DataContext;
using BoC.DataContext;

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
                dependencyResolver.RegisterType<ISessionFactory, XpoDataContextSessionFactory>();
            }

            if (!dependencyResolver.IsRegistered<ISessionManager>())
            {
                //IoC.RegisterSingleton<ISessionManager, CurrentContextSessionManager>();
                dependencyResolver.RegisterSingleton<ISessionManager, XpoDataContextSessionManager>();
            }

            if (!dependencyResolver.IsRegistered<IDataContext>())
            {
                dependencyResolver.RegisterType<IDataContext, XpoDataContext>();
            }
        }
    }
}