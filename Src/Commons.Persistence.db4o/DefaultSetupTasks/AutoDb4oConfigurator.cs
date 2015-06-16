using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using BoC.InversionOfControl;
using BoC.Persistence.db4o.DataContext;
using BoC.DataContext;

namespace BoC.Persistence.db4o.DefaultSetupTasks
{
    public class AutoDb4oConfigurator : IContainerInitializer
    {
        private readonly IDependencyResolver _dependencyResolver;

        public AutoDb4oConfigurator(IDependencyResolver dependencyResolver)
        {
            this._dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            var orm = ConfigurationManager.AppSettings["BoC.Persistence.Orm"];
            if (orm != null && !orm.Equals("db4o", StringComparison.InvariantCultureIgnoreCase))
                return;

            if (!_dependencyResolver.IsRegistered<ISessionFactory>())
            {
                _dependencyResolver.RegisterInstance<ISessionFactory>(
                    Db4oSessionFactory.InitializeSessionFactory(_dependencyResolver));
            }

            if (!_dependencyResolver.IsRegistered<ISessionManager>())
            {
                _dependencyResolver.RegisterSingleton<ISessionManager, Db4oDataContextSessionManager>();
            }

            if (!_dependencyResolver.IsRegistered<IDataContext>())
            {
                _dependencyResolver.RegisterType<IDataContext, Db4oDataContext>();
            }
        }
    }
}
