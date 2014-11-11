using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Ninject.Parameters;

namespace BoC.InversionOfControl.Ninject
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel _container;
        private const string DefaultBindingName = "";

        protected NinjectDependencyResolver()
            : this(new StandardKernel())
        {
        }

        public NinjectDependencyResolver(IKernel container)
        {
            _container = container;
        }

        public T Resolve<T>()
        {
            return _container.Get<T>(DefaultBindingName);
        }

        public void Inject<T>(T existing)
        {
            throw new NotImplementedException();
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
            if (IsRegistered(typeof(TRegisteredAs)))
            {
                RegisterType<TRegisteredAs, TResolvedTo>(Guid.NewGuid().ToString());
                return;
            }

            _container.Bind<TRegisteredAs>().To<TResolvedTo>().Named(DefaultBindingName);
        }

        public void RegisterType(Type from, Type to)
        {
            if (IsRegistered(from))
            {
                _container.Bind(from).To(to).Named(Guid.NewGuid().ToString());
                return;
            }

            _container.Bind(from).To(to).Named(DefaultBindingName);
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

        public void RegisterType<TRegisteredAs, TResolvedTo>(string name) where TResolvedTo : TRegisteredAs
        {
            _container.Bind<TRegisteredAs>().To<TResolvedTo>().Named(name);
        }

        public void RegisterInstance<TRegisteredAs>(TRegisteredAs instance)
        {
            _container.Bind<TRegisteredAs>().ToMethod(x => instance).Named(DefaultBindingName);
        }

        public void RegisterInstance<TRegisteredAs>(string name, TRegisteredAs instance)
        {
            _container.Bind<TRegisteredAs>().ToMethod(x => instance).Named(name);
        }

        public void RegisterSingleton<TFrom, TTo>() where TTo : TFrom
        {
            _container.Bind<TFrom>().To<TTo>().InSingletonScope();
        }

        public void RegisterSingleton(Type @from, Type to)
        {
            _container.Bind(@from).To(to).InSingletonScope();
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
            if (disposing && _container != null)
            {
                _container.Dispose();
            }
        }
    }
}