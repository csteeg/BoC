using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Extensions.ExecutionContextScoping;
using SimpleInjector.Integration.Web;

namespace BoC.InversionOfControl.SimpleInjector
{
    /// <summary>
    /// 
    /// </summary>
    public class SimpleInjectorDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// The _container
        /// </summary>
        private readonly Container _container;
        /// <summary>
        /// The web request with disposal
        /// </summary>
        internal static readonly WebRequestLifestyle WebRequestWithDisposal = new WebRequestLifestyle();
        /// <summary>
        /// The execution context with disposal
        /// </summary>
        internal static readonly ExecutionContextScopeLifestyle ExecutionContextWithDisposal = new ExecutionContextScopeLifestyle();
        /// <summary>
        /// The _scope
        /// </summary>
        private Scope _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleInjectorDependencyResolver"/> class.
        /// </summary>
        public SimpleInjectorDependencyResolver()
            : this(AllowResolveSingleAsEnumerable(AllowToResolveArraysAndLists(new Container())))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleInjectorDependencyResolver"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="beginScope">if set to <c>true</c> [begin scope].</param>
        public SimpleInjectorDependencyResolver(Container container, bool beginScope = false)
        {
            _container = container;
            if (beginScope)
            {
                _scope = container.BeginExecutionContextScope();
            }
        }


        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class
        {
            return this.Resolve(typeof (T)) as T;
        }

        /// <summary>
        /// Resolves the specified t.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public object Resolve(Type t)
        {
            if ((!t.IsClass || t.IsAbstract) && !IsRegistered(t))
                return null;
            return _container.GetInstance(t);
        }


        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <typeparam name="TRegisteredAs">The type of the registered as.</typeparam>
        /// <typeparam name="TResolvedTo">The type of the resolved to.</typeparam>
        /// <param name="scope">The scope.</param>
        public void RegisterType<TRegisteredAs, TResolvedTo>(LifetimeScope scope = LifetimeScope.Transient) where TResolvedTo : class, TRegisteredAs where TRegisteredAs : class
        {
            this.RegisterType(typeof(TRegisteredAs), typeof(TResolvedTo), scope);
        }

        /// <summary>
        /// Registers the first in collection.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
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

        /// <summary>
        /// Gets the lifestyle.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="scope">The scope.</param>
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

        /// <summary>
        /// Determines whether the specified t is registered.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public bool IsRegistered(Type t)
        {
            return _container.GetCurrentRegistrations().Any(x => x.ServiceType == t);
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

        /// <summary>
        /// Registers the factory.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="factory">The factory.</param>
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

        /// <summary>
        /// Registers the factory.
        /// </summary>
        /// <typeparam name="TFrom">The type of from.</typeparam>
        /// <param name="factory">The factory.</param>
        public void RegisterFactory<TFrom>(Func<TFrom> factory) where TFrom : class
        {
            this.RegisterFactory(typeof(TFrom), factory);
        }

        /// <summary>
        /// Creates the child resolver.
        /// </summary>
        /// <returns></returns>
        public IDependencyResolver CreateChildResolver()
        {
            return new SimpleInjectorDependencyResolver(_container, beginScope: _container.GetCurrentExecutionContextScope() == null);
        }

        /// <summary>
        /// The singleton instance lifestyle registration
        /// </summary>
        private static readonly Type SingletonInstanceLifestyleRegistration = typeof(Container).Assembly.GetType("SimpleInjector.Lifestyles.SingletonLifestyle+SingletonInstanceLifestyleRegistration");
        /// <summary>
        /// The singleton instance lifestyle registration constructor
        /// </summary>
        private static readonly ConstructorInfo SingletonInstanceLifestyleRegistrationConstructor = SingletonInstanceLifestyleRegistration.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[]{typeof (Type), typeof (Type), typeof (object), typeof (Lifestyle), typeof (Container)}, null);
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TRegisteredAs">The type of the registered as.</typeparam>
        /// <param name="instance">The instance.</param>
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

        /// <summary>
        /// Registers the singleton.
        /// </summary>
        /// <typeparam name="TFrom">The type of from.</typeparam>
        /// <typeparam name="TTo">The type of to.</typeparam>
        public void RegisterSingleton<TFrom, TTo>() where TTo : class, TFrom where TFrom : class
        {
            this.RegisterSingleton(typeof(TFrom), typeof(TTo));
        }

        /// <summary>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// ldResolver();
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

        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <typeparam name="TResolvedTo">The type of the resolved to.</typeparam>
        /// <returns></returns>
        public IEnumerable<TResolvedTo> ResolveAll<TResolvedTo>() where TResolvedTo : class
        {
            return _container.GetAllInstances<TResolvedTo>();
        }

        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IEnumerable ResolveAll(Type type)
        {
            return _container.GetAllInstances(type);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SimpleInjectorDependencyResolver"/> class.
        /// </summary>
        ~SimpleInjectorDependencyResolver()
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
            if (disposing && _scope != null)
            {
                _scope.Dispose();
            }
        }

        public static Container AllowResolveSingleAsEnumerable(Container container)
        {
            container.ResolveUnregisteredType += (sender, e) =>
            {
                var serviceType = e.UnregisteredServiceType;
                if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
                {
                    var producer = container.GetRegistration(serviceType.GetGenericArguments()[0]);
                    if (producer != null)
                    {
                        var listType = typeof (List<>).MakeGenericType(serviceType.GetGenericArguments()[0]);
                        e.Register(() =>
                        {
                            IList list = Activator.CreateInstance(listType) as IList;
                            list.Add(producer.GetInstance());
                            return list;
                        });
                    }
                }
            };
            return container;
        }

        public static Container AllowToResolveArraysAndLists(Container container)
        {
            container.ResolveUnregisteredType += (sender, e) =>
            {
                var serviceType = e.UnregisteredServiceType;

                if (serviceType.IsArray)
                {
                    RegisterArrayResolver(e, container, serviceType.GetElementType());
                }
                else if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    RegisterArrayResolver(e, container, serviceType.GetGenericArguments()[0]);
                }
            };
            return container;
        }

        private static void RegisterArrayResolver(UnregisteredTypeEventArgs e, Container container, Type elementType)
        {
            var producer = container.GetRegistration(typeof(IEnumerable<>).MakeGenericType(elementType));
            var enumerableExpression = producer.BuildExpression();
            var arrayMethod = typeof(Enumerable).GetMethod("ToArray").MakeGenericMethod(elementType);
            var arrayExpression = Expression.Call(arrayMethod, enumerableExpression);

            e.Register(arrayExpression);
        }
    }
}