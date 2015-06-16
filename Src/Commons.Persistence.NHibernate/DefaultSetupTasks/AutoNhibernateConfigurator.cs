using System;
using System.Configuration;
using System.Web;
using BoC.InversionOfControl;
using BoC.Persistence.NHibernate;
using BoC.Persistence.NHibernate.DataContext;
using BoC.DataContext;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Caches.SysCache2;
using NHibernate.Context;

namespace BoC.Persistence.DefaultSetupTasks
{
    public class AutoNhibernateConfigurator : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;

        public AutoNhibernateConfigurator(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Execute()
        {
            var orm = ConfigurationManager.AppSettings["BoC.Persistence.Orm"];
            if (orm != null && !orm.Equals("nhibernate", StringComparison.InvariantCultureIgnoreCase))
                return;

            IPersistenceConfigurer db = null;
            if (!dependencyResolver.IsRegistered<IPersistenceConfigurer>())
            {
                foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
                {
                    if (!String.IsNullOrEmpty(connectionString.Name) && connectionString.Name.EndsWith("nhibernate", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string providerName = connectionString.ProviderName ?? "System.Data.SqlClient";

                        if (providerName.IndexOf("sqlite", StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            db = NHibernateConfigHelper.GetDefaultSQLiteConfig(connectionString.Name);
                        }
                        else
                        {
                            db = NHibernateConfigHelper.GetDefaultSqlServerConfig(connectionString.Name);
                        }
                        break;
                    }
                }
            }
            else
            {
                db = dependencyResolver.Resolve<IPersistenceConfigurer>();
            }

            if (db == null)
            {
                throw new Exception("Could not find a connectionstring that ends with 'nhibernate'. If you want to configure your own db connection, you should register an IPersistenceConfigurer");
            }

            dependencyResolver.RegisterInstance<IPersistenceConfigurer>(db);
            if (!dependencyResolver.IsRegistered<ISessionFactory>())
            {
                NHibernateConfigHelper.SetupAutoMapperForEntities(dependencyResolver);
            }

            if (!dependencyResolver.IsRegistered<ISessionManager>())
            {
                //IoC.RegisterSingleton<ISessionManager, CurrentContextSessionManager>();
                dependencyResolver.RegisterSingleton<ISessionManager, DataContextSessionManager>();
            }

            if (!dependencyResolver.IsRegistered<IDataContext>())
            {
                dependencyResolver.RegisterType<IDataContext, NHibernateDataContext>();
            }
        }
    }
}