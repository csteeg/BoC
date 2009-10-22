using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using BoC.Helpers;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace BoC.InversionOfControl.Unity
{
    public class ExtendedUnityContainer : UnityContainer, IDependencyResolver
    {
        private readonly Dictionary<Type, HashSet<string>> registeredTypes = new Dictionary<Type, HashSet<string>>();

        public ExtendedUnityContainer() : base()
        {
            var configuration = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;
            if (configuration != null)
                configuration.Containers.Default.Configure(this);
            this.RegisterInstance<IDependencyResolver>(this);
        }

        public override IUnityContainer RegisterInstance(Type t, string name, object instance, LifetimeManager lifetime)
        {
            HashSet<string> names;

            if (name == null)
                name = "";

            if (!registeredTypes.TryGetValue(t, out names))
            { //  not found, so add it
                registeredTypes.Add(t, new HashSet<string> { name });
            }
            else
            { //  already added type, so add name
                if (name == "" && names.Contains(name))
                {
                    //default instance is already registered, let's give it a name so we can use ResolveAll() like it works in all other ioc containers
                    name = Guid.NewGuid().ToString();
                }
                names.Add(name);
            }

            return base.RegisterInstance(t, name, instance, lifetime);
        }

        public override IUnityContainer RegisterType(Type from, Type to, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            if (name == null)
                name = "";

            HashSet<string> names;
            if (!registeredTypes.TryGetValue(from, out names))
            { //  not found, so add it
                registeredTypes.Add(from, new HashSet<string> { name });
            }
            else
            { //  already added type, so add name
                if (name == "" && names.Contains(name))
                {
                    //default instance is already registered, let's give it a name so we can use ResolveAll() like it works in all other ioc containers
                    name = Guid.NewGuid().ToString();
                }
                names.Add(name);
            }

            return base.RegisterType(from, to, name, lifetimeManager, injectionMembers);
        }

        public override object Resolve(Type t, string name)
        {
            if (CanResolve(t, name))
                return base.Resolve(t, name);
            return null;
        }

        public bool IsRegistered(Type type)
        {
            return registeredTypes.ContainsKey(type);
        }

        public override IEnumerable<object> ResolveAll(Type t)
        {
            if (!CanResolveAny(t))
                yield break;
            
            object unnamedInstance = Resolve(t);
            if (unnamedInstance != null)
                yield return unnamedInstance;

            foreach (var o in ResolveAllImpl(t))
            {
                yield return o;
            }
        }

        private IEnumerable ResolveAllImpl(Type type)
        {
            return base.ResolveAll(type);
        }

        /// <summary>
        /// Determines whether this type can be resolved as the default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// 	<c>true</c> if this instance can resolve; otherwise, <c>false</c>.
        /// </returns>
        public bool CanResolve<T>()
        {
            return CanResolve(typeof(T), null);
        }
        /// <summary>
        /// Determines whether this type can be resolved as the default.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can resolve; otherwise, <c>false</c>.
        /// </returns>
        public bool CanResolve(Type type)
        {
            return CanResolve(type, null);
        }
        /// <summary>
        /// Determines whether this type can be resolved with the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can be resolved with the specified name; otherwise, <c>false</c>.
        /// </returns>
        public bool CanResolve<T>(string name)
        {
            return CanResolve(typeof(T), name);
        }

        /// <summary>
        /// Determines whether this type can be resolved with the specified name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can be resolved with the specified name; otherwise, <c>false</c>.
        /// </returns>
        public bool CanResolve(Type type, string name)
        {
            if (isResolvableClass(type))
                return true;

            HashSet<string> names;
            if (registeredTypes.TryGetValue(type, out names))
            {
                return names.Contains(name ?? "");
            }
            return false;
        }

        private bool isResolvableClass(Type type)
        {
            return type.IsClass && !type.IsAbstract;
        }
        /// <summary>
        /// Determines whether this instance can be resolved at all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// 	<c>true</c> if this instance can be resolved at all; otherwise, <c>false</c>.
        /// </returns>
        public bool CanResolveAny<T>()
        {
            return CanResolveAny(typeof(T));
        }

        /// <summary>
        /// Determines whether this instance can be resolved at all.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can be resolved at all; otherwise, <c>false</c>.
        /// </returns>
        public bool CanResolveAny(Type type)
        {
            return isResolvableClass(type) || registeredTypes.ContainsKey(type);
        }

        private bool alreadyDisposing = false;
        protected override void Dispose(bool disposing)
        {
            if (!alreadyDisposing) //prevent disposing yourself again
            {
                alreadyDisposing = disposing;
                base.Dispose(disposing);
            }
        }

        #region IDependencyResolver Members

        void IDependencyResolver.RegisterInstance<T>(T instance)
        {
            this.RegisterInstance<T>(instance);
        }

        public void RegisterSingleton<TFrom, TTo>() where TTo : TFrom
        {
            RegisterSingleton(typeof(TFrom), typeof(TTo));
        }

        public void RegisterSingleton(Type from, Type to)
        {
            RegisterType(from, to, new ContainerControlledLifetimeManager());
        }

        void IDependencyResolver.RegisterType<TFrom, TTo>()
        {
            this.RegisterType<TFrom, TTo>();
        }

        void IDependencyResolver.RegisterType(Type from, Type to)
        {
            this.RegisterType(from, to);
        }

        void IDependencyResolver.Inject<T>(T existing)
        {
            this.BuildUp<T>(existing);
        }

        object IDependencyResolver.Resolve(Type type)
        {
            return this.Resolve(type);
        }

        object IDependencyResolver.Resolve(Type type, string name)
        {
            return this.Resolve(type, name);
        }

        T IDependencyResolver.Resolve<T>()
        {
            return this.Resolve<T>();
        }

        T IDependencyResolver.Resolve<T>(string name)
        {
            return this.Resolve<T>(name);
        }

        IEnumerable<T> IDependencyResolver.ResolveAll<T>()
        {
            return this.ResolveAll<T>();
        }

        #endregion

    }
}