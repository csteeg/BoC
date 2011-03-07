using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using BoC.InversionOfControl;
using Commons.Persistence.db4o.AutoIncrement;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.TA;

namespace Commons.Persistence.db4o
{
    public class Db4oSessionFactory : ISessionFactory
    {
        private readonly IObjectContainer _rootObjectContainer;

        private Db4oSessionFactory(IObjectContainer rootObjectContainer)
        {
            _rootObjectContainer = rootObjectContainer;
        }

        public static ISessionFactory InitializeSessionFactory(IDependencyResolver dependencyResolver)
        {
            String databaseName = null;
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                if (!String.IsNullOrEmpty(connectionString.Name) &&
                    connectionString.Name.EndsWith("db4o", StringComparison.InvariantCultureIgnoreCase))
                {
                    databaseName = connectionString.ConnectionString;
                }
            }

            if (databaseName == null)
            {
                throw new Exception("Could not find a connectionstring that ends with 'db4o'.");
            }

            databaseName = databaseName.Replace(
                "|DataDirectory|", 
                AppDomain.CurrentDomain.GetData("DataDirectory") + "\\");

            IEmbeddedConfiguration config = Db4oEmbedded.NewConfiguration();
            config.Common.Add(new TransparentActivationSupport());
            config.Common.Add(new TransparentPersistenceSupport());
            config.Common.DetectSchemaChanges = true;
            config.Common.ExceptionsOnNotStorable = true;
            config.Common.OptimizeNativeQueries = true;
            config.Common.UpdateDepth = Int32.MaxValue;

            dependencyResolver.RegisterInstance<IEmbeddedConfiguration>(config);
            
            var db = Db4oEmbedded.OpenFile(config, databaseName);
            AutoIncrementSupport.Install(db);
            return new Db4oSessionFactory(db);
        }

        public IObjectContainer CreateSession()
        {
            return _rootObjectContainer.Ext().OpenSession();
        }

        public void Dispose()
        {
            if(_rootObjectContainer != null)
            {
                _rootObjectContainer.Close();
                _rootObjectContainer.Dispose();
            }
        }
    }
}
