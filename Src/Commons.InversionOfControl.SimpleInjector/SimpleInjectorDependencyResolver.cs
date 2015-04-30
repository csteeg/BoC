using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Extensions.ExecutionContextScoping;
using SimpleInjector.Integration.Web;

namespace BoC.InversionOfControl.SimpleInjector
{
    public class SimpleInjectorDependencyResolver : IDependencyResolver
    {
        private readonly Container _container;
        internal static readonly WebRequestLifestyle WebRequestWithDisposal = new WebRequestLifestyle();
        internal static readonly ExecutionContextScopeLifestyle ExecutionContextWithDisposal = new ExecutionContextScopeLifestyle();
        private Scope _scope;

        public SimpleInjectorDependencyResolver(): this(new Container())
        {
        }

        public SimpleInjectorDependencyResolver(Container container, bool beginScope = false)
        {
            _container = container;
            if (beginScope)
            {
                _scope = container.BeginExecutionContextScope();
            }
        }


        public T Resolve<T>() where T : class
        {
            return this.Resolve(typeof (T)) as T;
        }

        public object Resolve(Type t)
        {
            if ((!t.IsClass || t.IsAbstract) && !IsRegistered(t))
                return null;
            return _container.GetInstance(t);
        }


        public void RegisterType<TRegisteredAs, TResolvedTo>(LifetimeScope scope = LifetimeScope.Transient) where TResolvedTo : class, TRegisteredAs where TRegisteredAs : class
        {
            this.RegisterType(typeof(TRegisteredAs), typeof(TResolvedTo), scope);
        }

        private void RegisterFirstInCollection(Type serviceType)
        {
            var enumerable = typeof (IEnumerable<>).MakeGenericType(serviceType);
            if (!IsRegistered(enumerable))
            {
                var registration =
                    _container.GetCurrentRegistrations().First(x => x.ServiceType == serviceType).Registration;
                    //add first registration to collection
                _container.AppendToCollection(serviceType, registration);
            }
        }

        private Lifestyle GetLifestyle(LifetimeScope scope)
        {
            switch (scope)
            {
                case LifetimeScope.Unowned:
                    return Lifestyle.Transient;
                case LifetimeScope.PerHttpRequest:
                    return WebRequestWithDisposal;
                case LifetimeScope.PerThread:
                    return ExecutionContextWithDisposal;
                default:
                    return Lifestyle.Transient;
            }

        }

        public void RegisterType(Type from, Type to, LifetimeScope scope = LifetimeScope.Transient)
        {
            if (IsRegistered(from))
            {
                RegisterFirstInCollection(from);
                _container.AppendToCollection(from, GetLifestyle(scope).CreateRegistration(from, to, _container));
            }
            else
                _container.Register(from, to, GetLifestyle(scope));

            
        }

        public bool IsRegistered(Type t)
        {
            return _container.GetCurrentRegistrations().Any(x => x.ServiceType == t);
        }

        public bool IsRegistered<T>() where T : class
        {
            return IsRegistered(typeof (T));
        }

        public void RegisterFactory(Type @from, Func<object> factory)
        {
            if (IsRegistered(from))
            {
                RegisterFirstInCollection(from);
                _container.AppendToCollection(@from, Lifestyle.Transient.CreateRegistration(@from, factory, _container));
            }
            else
                _container.AddRegistration(@from, Lifestyle.Transient.CreateRegistration(@from, factory, _container));
        }

        public void RegisterFactory<TFrom>(Func<TFrom> factory) where TFrom : class
        {
            this.RegisterFactory(typeof(TFrom), factory);
        }

        public IDependencyResolver CreateChildResolver()
        {
            return new SimpleInjectorDependencyResolver(_container, beginScope: _container.GetCurrentExecutionContextScope() == null);
        }

        private static readonly Type SingletonInstanceLifestyleRegistration = typeof(Container).Assembly.GetType("SimpleInjector.Lifestyles.SingletonLifestyle+SingletonInstanceLifestyleRegistration");
        private static readonly ConstructorInfo SingletonInstanceLifestyleRegistrationConstructor = SingletonInstanceLifestyleRegistration.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[]{typeof (Type), typeof (Type), typeof (object), typeof (Lifestyle), typeof (Container)}, null);
        public void RegisterInstance<TRegisteredAs>(TRegisteredAs instance) where TRegisteredAs : class
        {
            if (IsRegistered<TRegisteredAs>())
            {
                RegisterFirstInCollection(typeof(TRegisteredAs));
                _container.AppendToCollection(typeof (TRegisteredAs),
                    SingletonInstanceLifestyleRegistrationConstructor.Invoke(new object[]
                    {
                        typeof (TRegisteredAs), instance.GetType(), instance, Lifestyle.Singleton, _container
                    }) as Registration);
            }
            else
                _container.RegisterSingle<TRegisteredAs>(instance);
        }

        public void RegisterSingleton<TFrom, TTo>() where TTo : class, TFrom where TFrom : class
        {
            this.RegisterSingleton(typeof(TFrom), typeof(TTo));
        }

        public void RegisterSingleton(Type @from, Type to)
        {
            if (IsRegistered(from))
            {
                RegisterFirstInCollection(from);
                _container.AppendToCollection(@from, Lifestyle.Singleton.CreateRegistration(@from, to, _container));
            }
            else
                _container.RegisterSingle(@from, to);
        }

        public IEnumerable<TResolvedTo> ResolveAll<TResolvedTo>() where TResolvedTo : class
        {
            return _container.GetAllInstances<TResolvedTo>();
        }

        public IEnumerable ResolveAll(Type type)
        {
            return _container.GetAllInstances(type);
        }

        ~SimpleInjectorDependencyResolver()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing && _scope != null)
            {
                _scope.Dispose();
            }
        }
    }
}