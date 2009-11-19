using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BoC.InversionOfControl;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using FluentNHibernate.Cfg;
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
                .Cache(cache => cache.UseQueryCache().ProviderClass<SysCacheProvider>())
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
                .Cache(cache => cache.UseQueryCache().ProviderClass<SysCacheProvider>())
#if DEBUG
                .ShowSql()
#endif
                .ProxyFactoryFactory("NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu")
                .CurrentSessionContext(CurrentSessionContextClass);
        }

        public static void SetupAutoMapperForEntities(IPersistenceConfigurer database, params Assembly[] assemblies)
        {
            var config = Fluently.Configure().Database(database);
            var stringPropertyconvention = ConventionBuilder.Property.When(x => x.Expect(p => p.Property.PropertyType == typeof (string)),
                                                                           a => a.Length(255));
            var isbasetype = new Func<Type, bool>(basetype =>
                                                  basetype.IsGenericType &&
                                                  basetype.GetGenericTypeDefinition() == typeof (BaseEntity<>));

            IEnumerable<Assembly> ass;
            if (assemblies == null || assemblies.Length == 0)
            {
                ass = AppDomain.CurrentDomain.GetAssemblies().Where(
                    a => !a.FullName.StartsWith("System.") &&
                            !a.FullName.StartsWith("Microsoft."));
            }
            else
            {
                ass = assemblies;
            }

            var autoPersistenceModel = new AutoPersistenceModel();

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
                if (assembly != null && assembly != typeof(AutoMap).Assembly)
                {
                    Assembly automapper = assembly;

                    autoPersistenceModel.AddEntityAssembly(automapper)
                        .Conventions.AddAssembly(automapper)
                        .Conventions.Add(stringPropertyconvention)
                        .Alterations(alterations => alterations.AddFromAssembly(automapper))
                        //.Alterations(collection => collection.Add(new AutoMappingOverrideAlteration(automapper)))
                        //same as: UseOverridesFromAssemblyOf<Tentity>()
                        .Setup(c => c.IsBaseType = isbasetype)
                        .Where(t => typeof (IBaseEntity).IsAssignableFrom(t));

                    // MORE Evil hack, since adding to the Alterations does NOT work.
                    new AutoMappingOverrideAlteration(automapper).Alter(autoPersistenceModel);
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
            
            IoC.RegisterInstance<Configuration>(nhconfig);
            IoC.RegisterInstance<ISessionFactory>(sessionFactory);

            new SchemaUpdate(nhconfig).Execute(true, true);
        }

        public static bool CreateNonExistingRepositories = true;
        public static void AutoRegisterRepositories()
        {
            //these 3 could vary per project:
            const string idPropertyName = "Id";
            var interfaceToFind = typeof(IRepository<>);
            var defaultBaseType = typeof(NHRepository<>);
            var constructorParams = new Type[] {typeof (ISessionManager)};
            
            var isbasetype = new Func<Type, bool>(basetype =>
                                                  basetype.IsGenericType &&
                                                  basetype.GetGenericTypeDefinition() == typeof (BaseEntity<>));

            //below should work fine for all situations :)
            ModuleBuilder mb = null;
            var types =
                AppDomain.CurrentDomain.GetAssemblies().ToList()
                    .SelectMany(s =>
                                    {
                                        try{return s.GetTypes() as IEnumerable<Type>;}
                                        catch (NotSupportedException)
                                        {
                                            return new List<Type>() as IEnumerable<Type>;
                                        }
                                    });


            var entities = from t in types
                           where t.IsClass
                                 && !t.IsAbstract
                                 && typeof (IBaseEntity).IsAssignableFrom(t)
                                 && !isbasetype(t)
                           select t;

            int typeNum = 0;
            foreach (var type in entities)
            {
                var idProp = type.GetProperty(idPropertyName);
                var idType = idProp == null ? typeof(object) : idProp.PropertyType;
                var interfaceType = interfaceToFind.MakeGenericType(new Type[] { type });

                if (interfaceType == null)
                {
                    continue;
                }

                var toFind = (from i in types
                          where i.IsInterface &&
                                interfaceType.IsAssignableFrom(i)
                          select i).FirstOrDefault() ?? interfaceType;

                if (IoC.IsRegistered(toFind))
                {
                    //this repository is already registered, if you have multiple repositories implementing the same interface, 
                    //you'll have to register the correct one 'by hand'
                    continue;
                }

                var repo = (from r in types
                            where !r.IsInterface &&
                                  toFind.IsAssignableFrom(r)
                            select r
                           ).FirstOrDefault();

                if (repo == null)
                {
                    if (mb == null)
                        mb = GetRepositoriesModuleBuilder();
                    var name = "DynamicGeneratedRepository" + typeNum++;

                    var baseType = defaultBaseType.MakeGenericType(new Type[] { type });
                    var baseConstructor = baseType.GetConstructor(constructorParams);
                    var tb = mb.DefineType(name, TypeAttributes.AutoLayout | TypeAttributes.Public,
                                           baseType,
                                           new Type[] {toFind});
                    var constructor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                                                           constructorParams);
                    for (int i = 1; i <= constructorParams.Length; i++)
                    {
                        constructor.DefineParameter(i, ParameterAttributes.None, "param" + i);
                    }

                    ILGenerator ilGenerator = constructor.GetILGenerator();
                    ilGenerator.Emit(OpCodes.Ldarg_0);                      // Load "this"
                    for (int i = 1; i <= constructorParams.Length; i++)
                    {
                        ilGenerator.Emit(OpCodes.Ldarg, i);
                    }
                    ilGenerator.Emit(OpCodes.Call, baseConstructor);    // Call the base constructor
                    ilGenerator.Emit(OpCodes.Ret); 
                    
                    repo = tb.CreateType();
                }
                IoC.RegisterType(interfaceType, repo);
                IoC.RegisterType(toFind, repo);
            }
        }

        private static ModuleBuilder GetRepositoriesModuleBuilder()
        {
            AssemblyName aName = new AssemblyName("DynamicRepositoriesAssembly");
            AssemblyBuilder ab =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    aName,
                    AssemblyBuilderAccess.RunAndSave);

            // For a single-module assembly, the module name is usually
            // the assembly name plus an extension.
            return ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");
        }

    }
}