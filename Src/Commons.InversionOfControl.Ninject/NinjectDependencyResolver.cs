using System;
using System.Collections;
using System.Collections.Generic;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;

namespace BoC.InversionOfControl.Ninject
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IResolutionRoot _container;
        private readonly IKernel _kernel;

        public NinjectDependencyResolver()
            : this(new StandardKernel(new NinjectSettings { AllowNullInjection = true }))
        {
        }

        public NinjectDependencyResolver(IKernel kernel): this(kernel, kernel)
        {
        }

        public NinjectDependencyResolver(IResolutionRoot container, IKernel kernel)
        {
            _container = container;
            _kernel = kernel;
        }

        public T Resolve<T>() where T : class
        {
            return _container.TryGet<T>();
        }

        public object Resolve(Type t)
        {
            return _container.TryGet(t);
        }

        public object Resolve(Type t, string name)
        {
            return _container.TryGet(t, name);
        }

        public T Resolve<T>(string name)
        {
            return _container.TryGet<T>(name);
        }

        public void RegisterType<TRegisteredAs, TResolvedTo>(LifetimeScope scope = LifetimeScope.Transient) where TResolvedTo : class, TRegisteredAs where TRegisteredAs : class
        {
            _kernel.Bind<TRegisteredAs>().To<TResolvedTo>().SetLifeStyle(scope);
        }

        public void RegisterType(Type from, Type to, LifetimeScope scope = LifetimeScope.Transient)
        {
            _kernel.Bind(@from).To(to).SetLifeStyle(scope);
        }

        public bool IsRegistered(Type t)
        {
            return
                _container.CanResolve(_container.CreateRequest(t, null, new List<IParameter>(), false, false),
                    true);
        }

        public bool IsRegistered<T>() where T : class
        {
            return IsRegistered(typeof (T));
        }

	    public void RegisterFactory(Type @from, Func<object> factory)
	    {
		    RegisterFactory(@from, factory, LifetimeScope.Transient);
	    }

		public void RegisterFactory(Type @from, Func<object> factory, LifetimeScope scope)
        {
            _kernel.Bind(@from).ToMethod(c => factory()).SetLifeStyle(scope);
        }

	    public void RegisterFactory<TFrom>(Func<TFrom> factory) where TFrom : class
	    {
			RegisterFactory<TFrom>(factory, LifetimeScope.Transient);
	    }

		public void RegisterFactory<TFrom>(Func<TFrom> factory, LifetimeScope scope) where TFrom : class
		{ 
			_kernel.Bind<TFrom>().ToMethod(c => factory()).SetLifeStyle(scope);
        }

        public IDependencyResolver CreateChildResolver()
        {
            return new NinjectDependencyResolver(_kernel.BeginBlock(), _kernel);
        }

        public void RegisterType<TRegisteredAs, TResolvedTo>(string name, LifetimeScope scope = LifetimeScope.Transient) where TResolvedTo : TRegisteredAs
        {
            _kernel.Bind<TRegisteredAs>().To<TResolvedTo>().SetLifeStyle(scope).Named(name);
        }

        public void RegisterInstance<TRegisteredAs>(TRegisteredAs instance) where TRegisteredAs : class
        {
            _kernel.Bind<TRegisteredAs>().ToMethod(x => instance);
        }

        public void RegisterInstance<TRegisteredAs>(string name, TRegisteredAs instance)
        {
            _kernel.Bind<TRegisteredAs>().ToMethod(x => instance).Named(name);
        }

        public void RegisterSingleton<TFrom, TTo>() where TTo : class, TFrom where TFrom : class
        {
            _kernel.Bind<TFrom>().To<TTo>().InSingletonScope();
        }

        public void RegisterSingleton(Type @from, Type to)
        {
            _kernel.Bind(@from).To(to).InSingletonScope();
        }

        public IEnumerable<TResolvedTo> ResolveAll<TResolvedTo>() where TResolvedTo : class
        {
            return _container.GetAll<TResolvedTo>();
        }

        public IEnumerable ResolveAll(Type type)
        {
            return _container.GetAll(type);
        }

        ~NinjectDependencyResolver()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing && _kernel != null)
            {
                _kernel.Dispose();
            }
        }
    }
}