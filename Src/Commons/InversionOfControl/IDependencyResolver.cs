using System;
using System.Collections;
using System.Collections.Generic;

namespace BoC.InversionOfControl
{
    public interface IDependencyResolver : IDisposable
    {
        void RegisterInstance<T>(T instance) where T : class;
        void RegisterSingleton<TFrom, TTo>() where TTo : class, TFrom where TFrom : class;
        void RegisterSingleton(Type from, Type to);
        void RegisterType<TFrom, TTo>(LifetimeScope scope = LifetimeScope.Transient) where TTo : class, TFrom  where TFrom : class;
        void RegisterType(Type from, Type to, LifetimeScope scope = LifetimeScope.Transient);
        void RegisterFactory(Type from, Func<object> factory);
        void RegisterFactory(Type from, Func<object> factory, LifetimeScope scope);
        void RegisterFactory<TFrom>(Func<TFrom> factory, LifetimeScope scope) where TFrom: class;
		void RegisterFactory<TFrom>(Func<TFrom> factory) where TFrom : class;
		IDependencyResolver CreateChildResolver();

        object Resolve(Type type);
        T Resolve<T>() where T:class;
        IEnumerable<T> ResolveAll<T>() where T:class;
        IEnumerable ResolveAll(Type type);
        bool IsRegistered(Type type);
        bool IsRegistered<T>() where T : class;
    }
}