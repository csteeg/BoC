using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BoC.InversionOfControl;
using BoC.Persistence.NHibernate.Cache;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using FluentNHibernate.Cfg;
using NHibernate.ByteCode.LinFu;
using NHibernate.Caches.SysCache;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace BoC.Persistence.NHibernate
{
    public static class NHibernateConfigHelper
    {
        public static string CurrentSessionContextClass = typeof (AutoContextSessionContext).FullName +
                                                          ", Commons.Persistence.NHibernate";

        public static IPersistenceConfigurer GetDefaultSqlServerConfig(string connectionStringKey)
        {
            return MsSqlConfiguration.MsSql2005.ConnectionString(c => c.FromConnectionStringWithKey(connectionStringKey))
                .UseReflectionOptimizer()
                .Cache(cache => cache.UseQueryCache().ProviderClass<SysCacheProvider>().QueryCacheFactory<ProjectionEnabledQueryCacheFactory>())
            #if DEBUG
                .ShowSql()
            #endif
                .ProxyFactoryFactory("NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu")
                .CurrentSessionContext(CurrentSessionContextClass);
        }

        public static IPersistenceConfigurer GetDefaultSQLiteConfig(string connectionStringKey)
        {
            return SQLiteConfiguration.Standard.ConnectionString(c => c.FromConnectionStringWithKey(connectionStringKey))
                .UseReflectionOptimizer()
                .Cache(cache => cache.UseQueryCache().ProviderClass<SysCacheProvider>().QueryCacheFactory<ProjectionEnabledQueryCacheFactory>())
#if DEBUG
                .ShowSql()
#endif
                .ProxyFactoryFactory(typeof(ProxyFactoryFactory))
                .CurrentSessionContext(CurrentSessionContextClass);
        }

        public static void SetupAutoMapperForEntities(IDependencyResolver dependencyResolver, params Assembly[] assemblies)
        {
            var database = dependencyResolver.Resolve<IPersistenceConfigurer>();
            var config = Fluently.Configure().Database(database);
            var stringPropertyconvention = ConventionBuilder.Property.When(x => x.Expect(p => p.Property.PropertyType == typeof (string)), a => a.Length(255));
            
            var cacheConvention = ConventionBuilder.Class.Always(c => c.Cache.ReadWrite());
            
            var collectionsConventionMany = ConventionBuilder.HasMany.When(
                x => x.Expect(p => !(p.Member is DummyPropertyInfo)),
                instance => { instance.Cascade.SaveUpdate(); instance.Cache.ReadWrite(); });

            var collectionsConventionManyToMany = ConventionBuilder.HasManyToMany.When(
                x => x.Expect(p => !(p.Member is DummyPropertyInfo)),
                instance => { instance.Cascade.SaveUpdate(); instance.Cache.ReadWrite(); });
            var lazyConvention = ConventionBuilder.Reference.Always(c => c.LazyLoad());

            IEnumerable<Assembly> ass;
            if (assemblies == null || assemblies.Length == 0)
            {
                ass = AppDomain.CurrentDomain.GetAssemblies().Where(
                    a => !a.FullName.StartsWith("System.") &&
                         !a.FullName.StartsWith("Microsoft.") &&
                         !a.FullName.Contains("mscorlib") &&
                         a != typeof(ISession).Assembly &&
                         a != typeof(AutoMap).Assembly
               );
            }
            else
            {
                ass = assemblies;
            }

            var autoPersistenceModel = new AutoPersistenceModel()
                .Conventions.Add(cacheConvention, collectionsConventionMany, collectionsConventionManyToMany, stringPropertyconvention, lazyConvention)
                .IgnoreBase(typeof (BaseEntity<>))
                .IgnoreBase(typeof (IBaseEntity));

            foreach (var assembly in ass)
            {
                try
                {
                    assembly.GetExportedTypes();
                }
                catch (NotSupportedException)
                {
                    continue;//cannot do dynamic assemblies
                }
                if (assembly != null)
                {
                    Assembly automapper = assembly;

                    autoPersistenceModel.AddEntityAssembly(automapper)
                        .Conventions.AddAssembly(automapper)
                        .Alterations(alterations => alterations.AddFromAssembly(automapper))
                        .Alterations(collection => collection.Add(new AutoMappingOverrideAlteration(automapper))) //same as: UseOverridesFromAssemblyOf<Tentity>()
                        .Where(t => typeof (IBaseEntity).IsAssignableFrom(t));

                    // MORE Evil hack, since adding to the Alterations does NOT work.
                    //new AutoMappingOverrideAlteration(automapper).Alter(autoPersistenceModel);
                }
            }

            // Evil hack, since adding to the Alterations does NOT work.
            //foreach (var overrideAssembly in ass)
            //{
            //    new AutoMappingOverrideAlteration(overrideAssembly).Alter(autoPersistenceModel);
            //}

            config.Mappings(m => m.AutoMappings.Add(autoPersistenceModel)
#if DEBUG
            .ExportTo(@"C:\temp\")
#endif
            );

            var nhconfig = config.BuildConfiguration();
            var sessionFactory = config.BuildSessionFactory();

            dependencyResolver.RegisterInstance<Configuration>(nhconfig);
            dependencyResolver.RegisterInstance<ISessionFactory>(sessionFactory);

            new SchemaUpdate(nhconfig).Execute(true, true);
        }
    }
}