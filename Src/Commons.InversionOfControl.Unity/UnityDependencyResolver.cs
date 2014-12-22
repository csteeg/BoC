using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using BoC.Helpers;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace BoC.InversionOfControl.Unity
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer _container;

        public UnityDependencyResolver() : this(new UnityContainer())
        {
            var configuration = (UnityConfigurationSection) ConfigurationManager.GetSection("unity");
            if (configuration != null)
                configuration.Configure(_container);
        }
        
        public UnityDependencyResolver(IUnityContainer container)
        {
            Check.Argument.IsNotNull(container, "container");

            this._container = container;
            this._container.AddExtension(new TypeTrackingExtension());
        }

        public IUnityContainer Container { get { return _container; } }

        public void RegisterInstance<T>(T instance)
        {
            Check.Argument.IsNotNull(instance, "instance");

            _container.RegisterInstance(instance);
        }

        public void RegisterSingleton<TFrom, TTo>() where TTo : TFrom
        {
            RegisterSingleton(typeof(TFrom), typeof(TTo));
        }

        public virtual void RegisterSingleton(Type from, Type to)
        {
            _container.RegisterType(from, to, new ContainerControlledLifetimeManager());
        }

        public void RegisterType<TFrom, TTo>(LifetimeScope scope = LifetimeScope.Transient) where TTo : TFrom
        {
            _container.RegisterType<TFrom, TTo>(GetLifetimeManager(scope));
        }

        public void RegisterFactory<TFrom>(Func<TFrom> factory)
        {
            _container.RegisterType<TFrom>(new InjectionFactory(c => factory()));
        }

        public IDependencyResolver CreateChildResolver()
        {
            return new UnityDependencyResolver(_container.CreateChildContainer());
        }

        public void RegisterFactory(Type from, Func<object> factory)
        {
            _container.RegisterType(from, new InjectionFactory(c => factory()));
        }

        public virtual void RegisterType(Type from, Type to, LifetimeScope scope = LifetimeScope.Transient)
        {
            Check.Argument.IsNotNull(from, "from");
            Check.Argument.IsNotNull(to, "to");

            _container.RegisterType(from, to, GetLifetimeManager(scope));
        }
        
        public object Resolve(Type type)
        {
            Check.Argument.IsNotNull(type, "type");
            if (_container.Configure<TypeTrackingExtension>().CanResolve(type))
            {
                return _container.Resolve(type);
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
                var result = _container.Resolve(type, name);
                if (typeof(IEnumerable).IsAssignableFrom(type) && 
                    ((result == null) || !((IEnumerable)result).Cast<Object>().Any()))
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

            return _container.ResolveAll(t);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            if (!CanResolve(typeof(T)))
                return Enumerable.Empty<T>();

            return _container.ResolveAll<T>();
        }

        public bool IsRegistered<T>()
        {
            return IsRegistered(typeof (T));
        }

        public bool IsRegistered(Type type)
        {
            return _container.Configure<TypeTrackingExtension>().IsRegistered(type);
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

        private LifetimeManager GetLifetimeManager(LifetimeScope lifetimeScope)
        {
            switch (lifetimeScope)
            {
                case LifetimeScope.Unowned:
                    return new TransientLifetimeManager();
                case LifetimeScope.PerHttpRequest:
                    return new PerRequestLifetimeManager();
                case LifetimeScope.PerThread:
                    return new PerThreadLifetimeManager();
                default:
                    return new TransientLifetimeManager();
            }
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
            if (disposing && _container != null)
            {
                _container.Dispose();
            }
        }
    }
}