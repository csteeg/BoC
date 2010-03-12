using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BoC.EventAggregator;
using BoC.InversionOfControl;
using BoC.InversionOfControl.Configuration;
using BoC.Persistence;
using BoC.Services;
using BoC.Validation;


namespace BoC.Tasks
{
    public class AutoRegisterModelServices : IContainerInitializer
    {
        public static bool CreateMissingModelServices = true;

        public void Execute()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var modelservices = assemblies
                .SelectMany(s => s.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract &&
                            !t.Assembly.GetName().Name.StartsWith("Microsoft") && 
                            !t.Assembly.GetName().Name.StartsWith("System") 
                            && typeof(IModelService<>).IsAssignableFrom(t));

            foreach (var service in modelservices)
            {
                var interfaces = service.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (typeof(IModelService<>).IsAssignableFrom(@interface)
                        && !IoC.IsRegistered(@interface))
                    {
                        IoC.RegisterType(@interface, service);
                    }
                }
            }

            if (CreateMissingModelServices)
                CreateModelServices(assemblies);
            
        }

        static void CreateModelServices(IEnumerable<Assembly> assemblies)
        {
            var isbasetype = new Func<Type, bool>(basetype =>
                                                  basetype.IsGenericType &&
                                                  basetype.GetGenericTypeDefinition() == typeof (BaseEntity<>));

            var types = from t in assemblies
                            .SelectMany(s => GetTypes(s))
                        select t;

            var entities = from t in types
                           where t.IsClass
                                 && !t.IsAbstract
                                 && typeof (IBaseEntity).IsAssignableFrom(t)
                                 && !isbasetype(t)
                           select t;

            var baseType = typeof (BaseModelService<>);
            var baseInterface = typeof (IModelService<>);
            int typeNum = 1;
            ModuleBuilder mb = null;
            foreach (var entityType in entities)
            {
                var inferfaceType = baseInterface.MakeGenericType(entityType);
                if (inferfaceType == null)
                {
                    continue;
                }

                var interfaceToFind = (from i in types
                                       where i.IsInterface &&
                                             inferfaceType.IsAssignableFrom(i)
                                       select i).FirstOrDefault() ?? inferfaceType;

                if (IoC.IsRegistered(interfaceToFind))
                {
                    //this repository is already registered, if you have multiple repositories implementing the same interface, 
                    //you'll have to register the correct one 'by hand'
                    continue;
                }

                var serviceType = (from r in types
                            where !r.IsInterface &&
                                  interfaceToFind.IsAssignableFrom(r)
                            select r
                           ).FirstOrDefault();

                if (serviceType == null)
                {
                    var serviceBaseType = baseType.MakeGenericType(entityType);
                    var name = "DynamicGeneratedModelService" + typeNum++;

                    if (mb == null)
                        mb = GetRepositoriesModuleBuilder();
                    var tb = mb.DefineType(name, TypeAttributes.AutoLayout | TypeAttributes.Public,
                                           serviceBaseType);
                    var constructorParams = new[]
                                                {
                                                    typeof (IModelValidator),
                                                    typeof (IEventAggregator),
                                                    typeof (IRepository<>).MakeGenericType(entityType)
                                                };

                    var baseConstructor = serviceBaseType.GetConstructor(constructorParams);
                    var constructor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                                                           constructorParams);
                    for (int i = 1; i <= constructorParams.Length; i++)
                    {
                        constructor.DefineParameter(i, ParameterAttributes.None, "param" + i);
                    }

                    ILGenerator ilGenerator = constructor.GetILGenerator();
                    ilGenerator.Emit(OpCodes.Ldarg_0); // Load "this"
                    for (int i = 1; i <= constructorParams.Length; i++)
                    {
                        ilGenerator.Emit(OpCodes.Ldarg, i);
                    }
                    ilGenerator.Emit(OpCodes.Call, baseConstructor); // Call the base constructor
                    ilGenerator.Emit(OpCodes.Ret);

                    serviceType = tb.CreateType();
                }
                IoC.RegisterType(interfaceToFind, serviceType);
                IoC.RegisterType(inferfaceType, serviceType);
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
            AssemblyName aName = new AssemblyName("DynamicModelServicesAssembly");
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