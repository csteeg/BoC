using System;
using System.Collections.Generic;
using System.ComponentModel;
using BoC.InverionOfControl.Unity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace BoC.InversionOfControl.Unity
{
    
    public class TypeTrackingExtension : UnityContainerExtension
    {
        private readonly Dictionary<Type, HashSet<string>> registeredTypes = new Dictionary<Type, HashSet<string>>();

        protected override void Initialize()
        {
            Context.Container.Configure<UnityDefaultBehaviorExtension>().Remove();
            Context.Container.Configure<InjectedMembers>().Remove();
            //we have to be the first in the chain
            Context.RegisteringInstance += OnNewInstance;
            Context.Registering += OnNewType;

            //our own array resolver:
            Context.Strategies.AddNew<CustomArrayResolutionStrategy>(UnityBuildStage.Creation);

            Context.Container.Configure<UnityDefaultBehaviorExtension>().InitializeExtension(Context);
            Context.Container.Configure<InjectedMembers>().InitializeExtension(Context);


        }

        public override void Remove()
        {
            base.Remove();
            Context.RegisteringInstance -= OnNewInstance;
            Context.Registering -= OnNewType;
        }
        private void OnNewInstance(object sender, RegisterInstanceEventArgs e)
        {
            HashSet<string> names;
            string name = string.IsNullOrEmpty(e.Name) ? string.Empty : e.Name;

            if (!registeredTypes.TryGetValue(e.RegisteredType, out names))
            { //  not found, so add it
                registeredTypes.Add(e.RegisteredType, new HashSet<string> { name });
            }
            else
            { //  already added type, so add name
                if (name == String.Empty && names.Contains(name))
                {
                    //default instance is already registered, let's give it a name so we can use ResolveAll() like it works in all other ioc containers
                    name = e.Name = Guid.NewGuid().ToString();
                }
                names.Add(name);
            }
        }
        public bool IsRegistered(Type t)
        {
            return registeredTypes.ContainsKey(t);
        }
        private void OnNewType(object sender, RegisterEventArgs e)
        {
            HashSet<string> names;
            string name = string.IsNullOrEmpty(e.Name) ? string.Empty : e.Name;
            if (!registeredTypes.TryGetValue(e.TypeFrom, out names))
            { //  not found, so add it
                registeredTypes.Add(e.TypeFrom, new HashSet<string> { name });
            }
            else
            { //  already added type, so add name
                if (name == String.Empty && names.Contains(name))
                {
                    //default instance is already registered, let's give it a name so we can use ResolveAll() like it works in all other ioc containers
                    name = e.Name = Guid.NewGuid().ToString();
                }
                names.Add(name);
            }
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
            return CanResolve(typeof (T), name);
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
                return names.Contains(name ?? string.Empty);
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

        /// <summary>
        /// Tries to resolve the type, returning null if not found.
        /// </summary>
        /// <typeparam name="T">The type to try and resolve.</typeparam>
        /// <returns>An object of type <see cref="T"/> if found, or <c>null</c> if not.</returns>
        public T TryResolve<T>()
        {
            return TryResolve<T>(default(T));
        }
        /// <summary>
        /// Tries to resolve the type with the specified of name, returning null if not found.
        /// </summary>
        /// <typeparam name="T">The type to try and resolve.</typeparam>
        /// <param name="name">The name associated with the type.</param>
        /// <returns>An object of type <see cref="T"/> if found, or <c>null</c> if not.</returns>
        public T TryResolve<T>(string name)
        {
            return TryResolve<T>(name, default(T));
        }
        /// <summary>
        /// Tries to resolve the type, returning null if not found.
        /// </summary>
        /// <typeparam name="T">The type to try and resolve.</typeparam>
        /// <param name="defaultValue">The default value to return if type not found.</param>
        /// <returns>An object of type <see cref="T"/> if found, or the <see cref="defaultValue"/> if not.</returns>
        public T TryResolve<T>(T defaultValue)
        {
            if (!CanResolve<T>())
                return defaultValue;
            return Container.Resolve<T>();
        }
        /// <summary>
        /// Tries to resolve the type with the specified of name, returning null if not found.
        /// </summary>
        /// <typeparam name="T">The type to try and resolve.</typeparam>
        /// <param name="name">The name associated with the type.</param>
        /// <param name="defaultValue">The default value to return if type not found.</param>
        /// <returns>An object of type <see cref="T"/> if found, or the <see cref="defaultValue"/> if not.</returns>
        public T TryResolve<T>(string name, T defaultValue)
        {
            if (!CanResolve<T>(name))
                return defaultValue;
            return Container.Resolve<T>(name);
        }
    }
}
