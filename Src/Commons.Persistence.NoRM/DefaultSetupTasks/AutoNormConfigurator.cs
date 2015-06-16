using System;
using System.Configuration;
using BoC.InversionOfControl;
using BoC.Persistence.Norm;
using BoC.Persistence.Norm.DataContext;
using BoC.DataContext;

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
            var orm = ConfigurationManager.AppSettings["BoC.Persistence.Orm"];
            if (orm != null && !orm.Equals("norm", StringComparison.InvariantCultureIgnoreCase))
                return;

            if (!dependencyResolver.IsRegistered<ISessionFactory>())
            {
                dependencyResolver.RegisterType<ISessionFactory, DefaultSessionFactory>();
            }

            if (!dependencyResolver.IsRegistered<ISessionManager>())
            {
                //IoC.RegisterSingleton<ISessionManager, CurrentContextSessionManager>();
                dependencyResolver.RegisterSingleton<ISessionManager, NormDataContextSessionManager>();
            }

            if (!dependencyResolver.IsRegistered<IDataContext>())
            {
                dependencyResolver.RegisterType<IDataContext, NormDataContext>();
            }
        }
    }
}