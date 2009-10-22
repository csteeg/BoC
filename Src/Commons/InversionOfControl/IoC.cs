using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BoC.Helpers;

namespace BoC.InversionOfControl
{
    public static class IoC
    {
        private static IDependencyResolver resolver;
        private static readonly object resolverLock = new object();

        public static void InitializeWith(IDependencyResolver resolver)
        {
            Check.Argument.IsNotNull(resolver, "resolver");
            lock (resolverLock)
            {
                if (IoC.resolver != null)
                    throw new ArgumentException("IoC already initialized");
                
                IoC.resolver = resolver;

                Configuration.ContainerInitializer.Execute();
            }
        }

        public static bool IsInitialized()
        {
            return resolver != null;
        }

        public static void RegisterInstance<T>(T instance)
        {
            Check.Argument.IsNotNull(instance, "instance");

            resolver.RegisterInstance(instance);
        }
        public static void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            resolver.RegisterType<TFrom, TTo>();
        }

        public static void RegisterType(Type from, Type to)
        {
            resolver.RegisterType(from, to);
        }

        public static bool IsRegistered(Type type)
        {
            return resolver.IsRegistered(type);
        }

        public static bool IsRegistered<T>()
        {
            return resolver.IsRegistered(typeof(T));
        }

        public static void Inject<T>(T existing)
        {
            Check.Argument.IsNotNull(existing, "existing");

            resolver.Inject(existing);
        }

        [DebuggerStepThrough]
        public static object Resolve(Type type)
        {
            Check.Argument.IsNotNull(type, "type");

            return resolver.Resolve(type);
        }

        [DebuggerStepThrough]
        public static object Resolve(Type type, string name)
        {
            Check.Argument.IsNotNull(type, "type");
            Check.Argument.IsNotEmpty(name, "name");

            return resolver.Resolve(type, name);
        }

        [DebuggerStepThrough]
        public static T Resolve<T>()
        {
            return resolver.Resolve<T>();
        }

        [DebuggerStepThrough]
        public static T Resolve<T>(string name)
        {
            Check.Argument.IsNotEmpty(name, "name");

            return resolver.Resolve<T>(name);
        }

        public static IEnumerable<T> ResolveAll<T>()
        {
            return resolver.ResolveAll<T>();
        }

        public static void Reset()
        {
            if (resolver != null)
            {
                resolver.Dispose();
                resolver = null;
            }
        }

        #region helpers

        public static void AddFromSameNamespaceAs<T>(bool useInterfaces)
        {
            AddFromSameNamespaceAs<T>(useInterfaces, false);
        }

        public static void AddFromSameNamespaceAs<T>(bool useInterfaces, bool asSingleton)
        {
            AddFromSameNamespaceAs<T>(useInterfaces, false, asSingleton);
        }

        public static void AddFromSameNamespaceAs<T>(bool useInterfaces, bool recursive, bool asSingleton)
        {
            var typeInNs = typeof (T);
            Type[] types = typeInNs.Assembly.GetTypes();
            foreach (Type type in types)
            {
                if (!type.IsInterface && !type.IsAbstract && !type.IsEnum && !type.IsNotPublic && !type.IsPointer &&
                    !type.IsPrimitive && !type.IsValueType)
                {
                    if ((recursive && type.Namespace != null && type.Namespace.StartsWith(typeInNs.Namespace))
                        ||
                        (type.Namespace == typeInNs.Namespace))
                    {
                        AddType(type, useInterfaces);
                        if (asSingleton)
                        {
                            RegisterInstance(Resolve(type));
                        }
                    }
                }
            }
        }

        private static void AddType(Type type, bool useInterfaces)
        {
            if (type.IsAbstract || !type.IsClass)
                return;
            if (useInterfaces)
            {
                Type interfaceType = GetInterface(type);
                if (interfaceType != null)
                    RegisterType(interfaceType, type);
            }
            RegisterType(type, type);
        }

        private static Type GetInterface(Type type)
        {
            Type[] interfaces = type.FindInterfaces((t, o) => true, null);
            foreach (Type iface in interfaces)
            {
                return iface;
            }
            return null;
        }

        #endregion

        public static void RegisterSingleton<I,T>() where T:I
        {
            resolver.RegisterSingleton<I,T>();
        }
        public static void RegisterSingleton(Type tFrom, Type tTo)
        {
            resolver.RegisterSingleton(tFrom, tTo);
        }
    }
}