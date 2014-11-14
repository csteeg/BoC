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
        private const string DefaultBindingName = "";

        protected NinjectDependencyResolver(): this(new StandardKernel())
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

        public T Resolve<T>()
        {
            return _container.Get<T>(DefaultBindingName);
        }

        public object Resolve(Type t)
        {
            return _container.Get(t, DefaultBindingName);
        }

        public object Resolve(Type t, string name)
        {
            return _container.Get(t, name);
        }

        public T Resolve<T>(string name)
        {
            return _container.Get<T>(name);
        }

        public void RegisterType<TRegisteredAs, TResolvedTo>() where TResolvedTo : TRegisteredAs
        {
            var bind = _kernel.Bind<TRegisteredAs>().To<TResolvedTo>();
            if (IsRegistered(typeof(TRegisteredAs)))
            {
                bind.Named(Guid.NewGuid().ToString());
                return;
            }

            bind.Named(DefaultBindingName);
        }

        public void RegisterType(Type from, Type to)
        {
            var bind = _kernel.Bind(@from).To(to);
            if (IsRegistered(from))
            {
                bind.Named(Guid.NewGuid().ToString());
                return;
            }

            bind.Named(DefaultBindingName);
        }

        public bool IsRegistered(Type t)
        {
            return
                _container.CanResolve(_container.CreateRequest(t, null, new List<IParameter>(), false, false),
                    true);
        }

        public bool IsRegistered<T>()
        {
            return IsRegistered(typeof (T));
        }

        public void RegisterFactory(Type @from, Func<object> factory)
        {
            _kernel.Bind(@from).ToMethod(c => factory());
        }

        public void RegisterFactory<TFrom>(Func<TFrom> factory)
        {
            _kernel.Bind<TFrom>().ToMethod(c => factory());
        }

        public IDependencyResolver BeginScope()
        {
            return new NinjectDependencyResolver(_kernel.BeginBlock(), _kernel);
        }

        public void RegisterType<TRegisteredAs, TResolvedTo>(string name) where TResolvedTo : TRegisteredAs
        {
            _kernel.Bind<TRegisteredAs>().To<TResolvedTo>().Named(name);
        }

        public void RegisterInstance<TRegisteredAs>(TRegisteredAs instance)
        {
            _kernel.Bind<TRegisteredAs>().ToMethod(x => instance).Named(DefaultBindingName);
        }

        public void RegisterInstance<TRegisteredAs>(string name, TRegisteredAs instance)
        {
            _kernel.Bind<TRegisteredAs>().ToMethod(x => instance).Named(name);
        }

        public void RegisterSingleton<TFrom, TTo>() where TTo : TFrom
        {
            _kernel.Bind<TFrom>().To<TTo>().InSingletonScope();
        }

        public void RegisterSingleton(Type @from, Type to)
        {
            _kernel.Bind(@from).To(to).InSingletonScope();
        }

        public IEnumerable<TResolvedTo> ResolveAll<TResolvedTo>()
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