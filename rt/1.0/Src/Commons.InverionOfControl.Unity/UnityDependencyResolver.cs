using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
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
                configuration.Containers.Default.Configure(container);
        }
        
        public UnityDependencyResolver(IUnityContainer container)
        {
            Check.Argument.IsNotNull(container, "container");

            this.container = container;
            this.container.AddExtension(new TypeTrackingExtension());
            this.container.RegisterInstance<IUnityContainer>(container);
            this.container.RegisterInstance<IDependencyResolver>(this);
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
                return container.Resolve(type, name);
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

        
        public IEnumerable<T> ResolveAll<T>()
        {
            var namedInstances = container.ResolveAll<T>();
            var unnamedInstance = default(T);

            try
            {
                unnamedInstance = container.Resolve<T>();
            }
            catch (ResolutionFailedException)
            {
                //When default instance is missing
            }

            if (Equals(unnamedInstance, default(T)))
            {
                return namedInstances;
            }

            return new ReadOnlyCollection<T>(new List<T>(namedInstances) { unnamedInstance });
        }

        public bool IsRegistered(Type type)
        {
            return container.Configure<TypeTrackingExtension>().IsRegistered(type);
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
            if (disposing)
            {
                container.Dispose();
            }
        }
    }
}