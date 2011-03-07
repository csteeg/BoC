using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using BoC.InversionOfControl;
using BoC.UnitOfWork;
using Commons.Persistence.db4o.UnitOfWork;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.TA;

namespace Commons.Persistence.db4o.DefaultSetupTasks
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
                _dependencyResolver.RegisterSingleton<ISessionManager, Db4oUnitOfWorkSessionManager>();
            }

            if (!_dependencyResolver.IsRegistered<IUnitOfWork>())
            {
                _dependencyResolver.RegisterType<IUnitOfWork, Db4oUnitOfWork>();
            }
        }
    }
}
