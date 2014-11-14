using System;
using System.Collections;
using System.Collections.Generic;

namespace BoC.InversionOfControl
{
    public interface IDependencyResolver : IDisposable
    {
        void RegisterInstance<T>(T instance);
        void RegisterSingleton<TFrom, TTo>() where TTo : TFrom;
        void RegisterSingleton(Type from, Type to);
        void RegisterType<TFrom, TTo>() where TTo : TFrom;
        void RegisterType(Type from, Type to);
        void RegisterFactory(Type from, Func<object> factory);
        void RegisterFactory<TFrom>(Func<TFrom> factory);
        IDependencyResolver BeginScope();

        object Resolve(Type type);
        object Resolve(Type type, string name);
        T Resolve<T>();
        T Resolve<T>(string name);
        IEnumerable<T> ResolveAll<T>();
        IEnumerable ResolveAll(Type type);
        bool IsRegistered(Type type);
        bool IsRegistered<T>();
    }
}