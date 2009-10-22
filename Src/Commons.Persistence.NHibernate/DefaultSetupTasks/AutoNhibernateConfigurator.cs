using System;
using System.Configuration;
using System.Web;
using BoC.InversionOfControl;
using BoC.InversionOfControl.Configuration;
using BoC.Persistence.NHibernate;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Caches.SysCache;
using NHibernate.Context;

namespace BoC.Persistence.DefaultSetupTasks
{
    public class AutoNhibernateConfigurator : IContainerInitializer
    {
        public void Execute()
        {
            //string currentSessionContextClass = HttpContext.Current != null ? "managed_web" : "thread_static";
            string currentSessionContextClass = typeof(AutoContextSessionContext).FullName + ", Commons.Persistence.NHibernate";

            IPersistenceConfigurer db = null;
            if (!IoC.IsRegistered<IPersistenceConfigurer>())
            {
                foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
                {
                    if (!String.IsNullOrEmpty(connectionString.Name) && connectionString.Name.EndsWith("nhibernate", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string providerName = connectionString.ProviderName ?? "System.Data.SqlClient";

                        if (providerName.IndexOf("sqlite", StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            db = NHibernateConfigHelper.GetDefaultSQLiteConfig(connectionString.Name, currentSessionContextClass);
                        }
                        else
                        {
                            db = NHibernateConfigHelper.GetDefaultSqlServerConfig(connectionString.Name, currentSessionContextClass);
                        }
                        break;
                    }
                }
            }
            else
            {
                db = IoC.Resolve<IPersistenceConfigurer>();
            }

            if (db == null)
            {
                throw new Exception("Could not find a connectionstring that ends with 'nhibernate'. If you want to configure your own db connection, you should register an IPersistenceConfigurer");
            }


            if (!IoC.IsRegistered<ISessionFactory>())
            {
                NHibernateConfigHelper.SetupAutoMapperForEntities(db);
            }

            if (!IoC.IsRegistered<ISessionManager>())
            {
                IoC.RegisterSingleton<ISessionManager, AutoContextSessionManager>();
            }
        }
    }
}