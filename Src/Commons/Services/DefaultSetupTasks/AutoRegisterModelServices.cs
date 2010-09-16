using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BoC.EventAggregator;
using BoC.Extensions;
using BoC.Helpers;
using BoC.InversionOfControl;
using BoC.Persistence;
using BoC.Validation;

namespace BoC.Services.DefaultSetupTasks
{
    public class AutoRegisterModelServices : IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly IAppDomainHelper[] appDomainHelpers;

        public AutoRegisterModelServices(IDependencyResolver dependencyResolver, IAppDomainHelper appDomainHelpers)
        {
            this.dependencyResolver = dependencyResolver;
            this.appDomainHelpers = new[] { appDomainHelpers };
        }

        public static bool CreateMissingModelServices = true;

        public void Execute()
        {
            var modelservices = appDomainHelpers
                .SelectMany(helpers => helpers
                    .GetTypes(t => t.IsClass && !t.IsAbstract &&
                            !t.Assembly.GetName().Name.StartsWith("Microsoft") && 
                            !t.Assembly.GetName().Name.StartsWith("System") 
                            && typeof(IModelService).IsAssignableFrom(t)));

            foreach (var service in modelservices)
            {
                var interfaces = service.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (typeof (IModelService<>).IsGenericAssignableFrom(@interface)
                        && !dependencyResolver.IsRegistered(@interface))
                    {
                        dependencyResolver.RegisterType(@interface, service);
                    }
                }
            }

            if (CreateMissingModelServices)
                CreateModelServices();
            
        }

        void CreateModelServices()
        {
            var isbasetype = new Func<Type, bool>(basetype =>
                                                  basetype.IsGenericType &&
                                                  basetype.GetGenericTypeDefinition() == typeof (BaseEntity<>));

            var types = (from t in appDomainHelpers
                            .SelectMany(s => s.GetTypes(type => true))
                        select t).ToList();

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
                var interfaceType = baseInterface.MakeGenericType(entityType);

                var interfaceToFind = (from i in types
                                       where i.IsInterface &&
                                             interfaceType.IsAssignableFrom(i)
                                       select i).FirstOrDefault() ?? interfaceType;

                if (dependencyResolver.IsRegistered(interfaceToFind))
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
                    //add same constructors as basetype:
                    var baseConstructors = serviceBaseType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (var baseConstructor in baseConstructors)
                    {
                        var constructorParams = baseConstructor.GetParameters();
                        var constructor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                                                               constructorParams.Select(pi => pi.ParameterType).ToArray());
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
                    }
                    serviceType = tb.CreateType();
                }
                dependencyResolver.RegisterType(interfaceToFind, serviceType);
                if (interfaceToFind != interfaceType)
                {
                    dependencyResolver.RegisterType(interfaceType, serviceType);
                }
            }


        }

        private static ModuleBuilder GetRepositoriesModuleBuilder()
        {
            var aName = new AssemblyName("DynamicModelServicesAssembly");
            var ab =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    aName,
                    AssemblyBuilderAccess.RunAndSave);

            // For a single-module assembly, the module name is usually
            // the assembly name plus an extension.
            return ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");
        }


    }
}