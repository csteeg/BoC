using System;
using System.Collections;
using System.Collections.Generic;
using DryIoc;

namespace BoC.InversionOfControl.DryIoC
{
    /// <summary>
    /// 
    /// </summary>
    public class DryIoCDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// The _container
        /// </summary>
        private readonly IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DryIoCDependencyResolver"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        private DryIoCDependencyResolver(IContainer container)
        {
            _container = container;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DryIoCDependencyResolver"/> class.
        /// </summary>
        public DryIoCDependencyResolver()
        {
            var container = new Container(
                rules => rules
                    .With(FactoryMethod.ConstructorWithResolvableArguments)
					.WithUnknownServiceResolvers(GetUnregisteredFactory)
            );
            _container = container;
        }

        /// <summary>
        /// Gets the unregistered factory.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private static Factory GetUnregisteredFactory(Request request)
        {
            var serviceType = request.ServiceType;
            return serviceType.IsClass && !serviceType.IsAbstract && !typeof (IEnumerable).IsAssignableFrom(serviceType)
                   && !serviceType.IsValueType
                ? (Factory) new ReflectionFactory(serviceType)
                : null;

        }

        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class
        {
            
            return _container.Resolve<T>(IfUnresolved.ReturnDefault);
        }

        /// <summary>
        /// Resolves the specified t.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public object Resolve(Type t)
        {
            return _container.Resolve(t, IfUnresolved.ReturnDefault);
        }

        /// <summary>
        /// Resolves the specified t.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public object Resolve(Type t, string name)
        {
            return _container.Resolve(t, name, IfUnresolved.ReturnDefault);
        }

        /// <summary>
        /// Resolves the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public T Resolve<T>(string name)
        {
            return _container.Resolve<T>(name, IfUnresolved.ReturnDefault);
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <typeparam name="TRegisteredAs">The type of the registered as.</typeparam>
        /// <typeparam name="TResolvedTo">The type of the resolved to.</typeparam>
        /// <param name="scope">The scope.</param>
        public void RegisterType<TRegisteredAs, TResolvedTo>(LifetimeScope scope = LifetimeScope.Transient) where TResolvedTo : class, TRegisteredAs where TRegisteredAs : class
        {
            _container.Register<TRegisteredAs, TResolvedTo>(GetReuse(scope));
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="scope">The scope.</param>
        public void RegisterType(Type from, Type to, LifetimeScope scope = LifetimeScope.Transient)
        {
            _container.Register(from, to, GetReuse(scope));
        }

        /// <summary>
        /// Determines whether the specified t is registered.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public bool IsRegistered(Type t)
        {
            return
                _container.IsRegistered(t);
        }

        /// <summary>
        /// Determines whether this instance is registered.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsRegistered<T>() where T : class
        {
            return IsRegistered(typeof (T));
        }

	    public void RegisterFactory(Type @from, Func<object> lambda)
	    {
			RegisterFactory(@from, lambda, LifetimeScope.Transient);
	    }
		/// <summary>
		/// Registers the factory.
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="lambda">The lambda.</param>
		public void RegisterFactory(Type @from, Func<object> lambda, LifetimeScope scope)
        {
            _container.RegisterDelegate(@from, resolver => lambda, GetReuse(scope));
        }

        /// <summary>
        /// Registers the factory.
        /// </summary>
        /// <typeparam name="TFrom">The type of from.</typeparam>
        /// <param name="factory">The factory.</param>
        public void RegisterFactory<TFrom>(Func<TFrom> factory, LifetimeScope scope) where TFrom : class
        {
            _container.RegisterDelegate<TFrom>(r => factory(), GetReuse(scope));
        }

		public void RegisterFactory<TFrom>(Func<TFrom> factory) where TFrom : class
		{
			RegisterFactory<TFrom>(factory, LifetimeScope.Transient);
		}

		/// <summary>
		/// Creates the child resolver.
		/// </summary>
		/// <returns></returns>
		public IDependencyResolver CreateChildResolver()
        {
            return new DryIoCDependencyResolver(_container.OpenScope());
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <typeparam name="TRegisteredAs">The type of the registered as.</typeparam>
        /// <typeparam name="TResolvedTo">The type of the resolved to.</typeparam>
        /// <param name="name">The name.</param>
        public void RegisterType<TRegisteredAs, TResolvedTo>(string name) where TResolvedTo : TRegisteredAs
        {
            _container.Register<TRegisteredAs, TResolvedTo>(serviceKey: name);
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TRegisteredAs">The type of the registered as.</typeparam>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance<TRegisteredAs>(TRegisteredAs instance) where TRegisteredAs : class
        {
            _container.RegisterInstance<TRegisteredAs>(instance);
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TRegisteredAs">The type of the registered as.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance<TRegisteredAs>(string name, TRegisteredAs instance)
        {
            _container.RegisterInstance<TRegisteredAs>(instance, serviceKey: name);
        }

        /// <summary>
        /// Registers the singleton.
        /// </summary>
        /// <typeparam name="TFrom">The type of from.</typeparam>
        /// <typeparam name="TTo">The type of to.</typeparam>
        public void RegisterSingleton<TFrom, TTo>() where TTo : class, TFrom where TFrom : class
        {
            _container.Register<TFrom, TTo>(Reuse.Singleton);
        }

        /// <summary>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// ldResolver();
        public void RegisterSingleton(Type @from, Type to)
        {
            _container.Register(@from, to, Reuse.Singleton);
        }

        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <typeparam name="TResolvedTo">The type of the resolved to.</typeparam>
        /// <returns></returns>
        public IEnumerable<TResolvedTo> ResolveAll<TResolvedTo>() where TResolvedTo : class
        {
            return _container.Resolve<IEnumerable<TResolvedTo>>(IfUnresolved.ReturnDefault) as IEnumerable<TResolvedTo>;
        }

        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IEnumerable ResolveAll(Type type)
        {
            return _container.Resolve(typeof(IEnumerable<>).MakeGenericType(type), IfUnresolved.ReturnDefault) as IEnumerable;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DryIoCDependencyResolver"/> class.
        /// </summary>
        ~DryIoCDependencyResolver()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (disposing && _container != null)
            {
                _container.Dispose();
            }
        }

        /// <summary>
        /// Gets the reuse.
        /// </summary>
        /// <param name="lifetimeScope">The lifetime scope.</param>
        /// <returns></returns>
        private IReuse GetReuse(LifetimeScope lifetimeScope)
        {
            switch (lifetimeScope)
            {
                case LifetimeScope.Unowned:
                    return Reuse.Transient;
                case LifetimeScope.PerHttpRequest:
                    return Reuse.InWebRequest;
                case LifetimeScope.PerThread:
                    return Reuse.InThread;
                default:
                    return Reuse.Transient;
            }
        }

    }
}