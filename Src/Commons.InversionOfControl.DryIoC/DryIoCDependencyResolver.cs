using System;
using System.Collections;
using System.Collections.Generic;
using DryIoc;

namespace BoC.InversionOfControl.DryIoC
{
    public class DryIoCDependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        private DryIoCDependencyResolver(IContainer container)
        {
            _container = container;
        }
        public DryIoCDependencyResolver()
        {
            var container = new Container(
                rules => rules.With(Constructor.WithAllResolvableArguments)
                            .WithNotRegisteredServiceResolvers(rules.NotRegisteredServiceResolvers.Append(GetUnregisteredFactory))
            );
            _container = container;
        }

        private static Factory GetUnregisteredFactory(Request request)
        {
            var serviceType = request.ServiceType;
            return serviceType.IsClass && !serviceType.IsAbstract && !typeof (IEnumerable).IsAssignableFrom(serviceType)
                   && !serviceType.IsValueType
                ? (Factory) new ReflectionFactory(serviceType)
                : null;

        }

        public T Resolve<T>() where T : class
        {
            
            return _container.Resolve<T>(IfUnresolved.ReturnDefault);
        }

        public object Resolve(Type t)
        {
            return _container.Resolve(t, IfUnresolved.ReturnDefault);
        }

        public object Resolve(Type t, string name)
        {
            return _container.Resolve(t, name, IfUnresolved.ReturnDefault);
        }

        public T Resolve<T>(string name)
        {
            return _container.Resolve<T>(name, IfUnresolved.ReturnDefault);
        }

        public void RegisterType<TRegisteredAs, TResolvedTo>(LifetimeScope scope = LifetimeScope.Transient) where TResolvedTo : class, TRegisteredAs where TRegisteredAs : class
        {
            _container.Register<TRegisteredAs, TResolvedTo>(GetReuse(scope));
        }

        public void RegisterType(Type from, Type to, LifetimeScope scope = LifetimeScope.Transient)
        {
            _container.Register(from, to, GetReuse(scope));
        }

        public bool IsRegistered(Type t)
        {
            return
                _container.IsRegistered(t);
        }

        public bool IsRegistered<T>() where T : class
        {
            return IsRegistered(typeof (T));
        }

        public void RegisterFactory(Type @from, Func<object> lambda)
        {
            string name = IsRegistered(@from) ? Guid.NewGuid().ToString() : null;
            _container.RegisterDelegate(@from, resolver => lambda, named: name);
        }

        public void RegisterFactory<TFrom>(Func<TFrom> factory) where TFrom : class
        {
            string name = IsRegistered<TFrom>() ? Guid.NewGuid().ToString() : null;
            _container.RegisterDelegate<TFrom>(r => factory(), named: name);
        }

        public IDependencyResolver CreateChildResolver()
        {
            return new DryIoCDependencyResolver(_container.CreateChildContainer());
        }

        public void RegisterType<TRegisteredAs, TResolvedTo>(string name) where TResolvedTo : TRegisteredAs
        {
            _container.Register<TRegisteredAs, TResolvedTo>(named: name);
        }

        public void RegisterInstance<TRegisteredAs>(TRegisteredAs instance) where TRegisteredAs : class
        {
            string name = IsRegistered<TRegisteredAs>() ? Guid.NewGuid().ToString() : null;
            _container.RegisterInstance<TRegisteredAs>(instance, named: name);
        }

        public void RegisterInstance<TRegisteredAs>(string name, TRegisteredAs instance)
        {
            _container.RegisterInstance<TRegisteredAs>(instance, named: name);
        }

        public void RegisterSingleton<TFrom, TTo>() where TTo : class, TFrom where TFrom : class
        {
            string name = IsRegistered<TFrom>() ? Guid.NewGuid().ToString() : null;
            _container.Register<TFrom, TTo>(Reuse.Singleton, named: name);
        }

        public void RegisterSingleton(Type @from, Type to)
        {
            string name = IsRegistered(@from) ? Guid.NewGuid().ToString() : null;
            _container.Register(@from, to, Reuse.Singleton, named: name);
        }

        public IEnumerable<TResolvedTo> ResolveAll<TResolvedTo>() where TResolvedTo : class
        {
            return _container.Resolve<IEnumerable<TResolvedTo>>(IfUnresolved.ReturnDefault) as IEnumerable<TResolvedTo>;
        }

        public IEnumerable ResolveAll(Type type)
        {
            return _container.Resolve(typeof(IEnumerable<>).MakeGenericType(type), IfUnresolved.ReturnDefault) as IEnumerable;
        }

        ~DryIoCDependencyResolver()
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

        private IReuse GetReuse(LifetimeScope lifetimeScope)
        {
            switch (lifetimeScope)
            {
                case LifetimeScope.Unowned:
                    return Reuse.Transient;
                case LifetimeScope.PerHttpRequest:
                    return WebReuse.InHttpContext;
                case LifetimeScope.PerThread:
                    return Reuse.InThreadScope;
                default:
                    return Reuse.Transient;
            }
        }

    }
}