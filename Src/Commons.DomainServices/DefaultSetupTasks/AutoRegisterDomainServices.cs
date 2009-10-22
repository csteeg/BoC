using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Web.DomainServices;
using System.Web.DynamicData;
using BoC.DomainServices;
using BoC.Persistence;
using BoC.Tasks;
using BoC.Validation;
using BoC.DomainServices.Extensions;

namespace BoC.DomainServices.DefaultSetupTasks
{
    public class AutoRegisterDomainServices : IBootstrapperTask
    {
        public static bool CreateMissingDomainServices = true;

        private readonly MetaModel model;

        public AutoRegisterDomainServices(MetaModel model)
        {
            this.model = model;
        }

        public void Execute()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var domainservices = assemblies
                    .SelectMany(s => s.GetTypes())
                    .Where(t => t.IsClass && !t.IsAbstract &&
                        !t.IsGenericType &&
                        !t.Assembly.GetName().Name.StartsWith("Microsoft") && 
                        !t.Assembly.GetName().Name.StartsWith("System") 
                        && typeof(DomainService).IsAssignableFrom(t));

            foreach (var service in domainservices)
            {
                model.RegisterContext(
                    new FixDomainModelProvider(service),
                    new ContextConfiguration()
                    {
                        ScaffoldAllTables = true
                    });
            }

            if (CreateMissingDomainServices)
                CreateDomainservices(assemblies);
            
            model.FixForeignKeyColumns();
        }

        void CreateDomainservices(IEnumerable<Assembly> assemblies)
        {
            if (model != null)
            {
                var isbasetype = new Func<Type, bool>(basetype =>
                                                      basetype.IsGenericType &&
                                                      basetype.GetGenericTypeDefinition() == typeof(BaseEntity<>));

                var entities = from t in assemblies
                                         .SelectMany(s => GetTypes(s))
                               where t.IsClass
                                        && !t.IsAbstract
                                        && typeof(IBaseEntity).IsAssignableFrom(t)
                                        && !isbasetype(t)
                               select t;

                var baseType = typeof(RepositoryDomainService<>);

                int typeNum = 1;
                ModuleBuilder mb = null;
                foreach (var entityType in entities)
                {
                    MetaTable dummy;
                    if (!model.TryGetTable(entityType, out dummy))
                    {
                        var serviceBaseType = baseType.MakeGenericType(entityType);
                        var name = "DynamicGeneratedDomainService" + typeNum++;

                        if (mb == null)
                            mb = GetRepositoriesModuleBuilder();
                        var tb = mb.DefineType(name, TypeAttributes.AutoLayout | TypeAttributes.Public,
                                               serviceBaseType);
                        var constructorParams =
                            new[] { typeof(IModelValidator), typeof(IRepository<>).MakeGenericType(entityType) };
                        var baseConstructor = serviceBaseType.GetConstructor(constructorParams);
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

                        model.RegisterContext(
                            new FixDomainModelProvider(tb.CreateType()),
                            new ContextConfiguration()
                            {
                                ScaffoldAllTables = true
                            });
                        
                    }
                }
            }
        }

        private static IEnumerable<Type> GetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetExportedTypes();
            }
            catch (NotSupportedException)
            {
                return new List<Type>(0);
            }
        }

        private static ModuleBuilder GetRepositoriesModuleBuilder()
        {
            AssemblyName aName = new AssemblyName("DynamicDomainServicesAssembly");
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