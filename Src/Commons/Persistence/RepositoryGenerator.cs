using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BoC.Helpers;
using BoC.InversionOfControl;

namespace BoC.Persistence
{
    public static class RepositoryGenerator
    {
        public static void GenerateRepositories(IDependencyResolver dependencyResolver, Type defaultBaseType, Type[] constructorParams, IAppDomainHelper[] appDomainHelpers)
        {
            const string idPropertyName = "Id";
            var interfaceToFind = typeof(IRepository<>);

            var isbasetype = new Func<Type, bool>(basetype =>
                                                  basetype.IsGenericType &&
                                                  basetype.GetGenericTypeDefinition() == typeof (BaseEntity<>));

            //below should work fine for all situations :)
            ModuleBuilder mb = null;
            var types = appDomainHelpers.SelectMany(
                helper => helper.GetTypes(t => true)).ToArray();

            var entities = from t in types
                           where t.IsClass
                                 && !t.IsAbstract
                                 && typeof (IBaseEntity).IsAssignableFrom(t)
                                 && !isbasetype(t)
                           select t;

            int typeNum = 0;
            foreach (var type in entities.ToArray())
            {
                var interfaceType = interfaceToFind.MakeGenericType(new Type[] { type });

                if (interfaceType == null)
                {
                    continue;
                }

                var toFind = (from i in types
                              where i.IsInterface &&
                                    interfaceType.IsAssignableFrom(i)
                              select i).FirstOrDefault() ?? interfaceType;

                if (dependencyResolver.IsRegistered(toFind))
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
                    try
                    {
                        repo = tb.CreateType();
                    }
                    catch (TypeLoadException exception)
                    {
                        throw new TypeLoadException("Error creating an automatic generated Repository for " + toFind, exception);
                    }
                }
                dependencyResolver.RegisterType(interfaceType, repo);
                if (interfaceType != toFind)
                    dependencyResolver.RegisterType(toFind, repo);
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