using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using BoC.Helpers;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace BoC.InversionOfControl.Unity
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer container;

        public UnityDependencyResolver() : this(new UnityContainer())
        {
            var configuration = (UnityConfigurationSection) ConfigurationManager.GetSection("unity");
            if (configuration != null)
                configuration.Configure(container);
        }
        
        public UnityDependencyResolver(IUnityContainer container)
        {
            Check.Argument.IsNotNull(container, "container");

            this.container = container;
            this.container.AddExtension(new TypeTrackingExtension());
        }

        public IUnityContainer Container { get { return container; } }

        public void RegisterInstance<T>(T instance)
        {
            Check.Argument.IsNotNull(instance, "instance");

            container.RegisterInstance(instance);
        }

        public void RegisterSingleton<TFrom, TTo>() where TTo : TFrom
        {
            RegisterSingleton(typeof(TFrom), typeof(TTo));
        }

        public virtual void RegisterSingleton(Type from, Type to)
        {
            container.RegisterType(from, to, new ContainerControlledLifetimeManager());
        }

        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            container.RegisterType<TFrom, TTo>();
        }

        public virtual void RegisterType(Type from, Type to)
        {
            Check.Argument.IsNotNull(from, "from");
            Check.Argument.IsNotNull(to, "to");

            container.RegisterType(from, to);
        }
        
        public void Inject<T>(T existing)
        {
            Check.Argument.IsNotNull(existing, "existing");

            container.BuildUp(existing);
        }
        
        public object Resolve(Type type)
        {
            Check.Argument.IsNotNull(type, "type");
            if (container.Configure<TypeTrackingExtension>().CanResolve(type))
            {
                return container.Resolve(type);
            }
            else
            {
                return null;
            }
        }
        
        public object Resolve(Type type, string name)
        {
            Check.Argument.IsNotNull(type, "type");
            if (Container.Configure<TypeTrackingExtension>().CanResolve(type, name))
            {
                var result = container.Resolve(type, name);
                if (typeof(IEnumerable).IsAssignableFrom(type) && 
                    ((result == null) || ((IEnumerable)result).Cast<Object>().Count() == 0))
                {
                    result = this.ResolveAll(type);
                }

                return result;
            }
            else
            {
                return null;
            }
        }

        
        public T Resolve<T>()
        {
            return (T) Resolve(typeof(T));
        }

        
        public T Resolve<T>(string name)
        {
            Check.Argument.IsNotEmpty(name, "name");
            return (T) Resolve(typeof(T), name);
        }


        public IEnumerable ResolveAll(Type t)
        {
            if (!CanResolve(t))
                return Enumerable.Empty<object>();

            return container.ResolveAll(t);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            if (!CanResolve(typeof(T)))
                return Enumerable.Empty<T>();

            return container.ResolveAll<T>();
        }

        public bool IsRegistered<T>()
        {
            return IsRegistered(typeof (T));
        }

        public bool IsRegistered(Type type)
        {
            return container.Configure<TypeTrackingExtension>().IsRegistered(type);
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
            return CanResolve(typeof(T));
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
            if (isResolvableClass(type))
                return true;
            return IsRegistered(type);
        }

        private bool isResolvableClass(Type type)
        {
            return type.IsClass && !type.IsAbstract;
        }


        ~UnityDependencyResolver()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
        }
        protected void Dispose(bool disposing)
        {
            if (disposing && container != null)
            {
                container.Dispose();
            }
        }
    }
}