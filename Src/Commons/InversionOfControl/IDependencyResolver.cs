using System;
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
        void Inject<T>(T existing);

        object Resolve(Type type);
        object Resolve(Type type, string name);
        T Resolve<T>();
        T Resolve<T>(string name);

        IEnumerable<T> ResolveAll<T>();
        bool IsRegistered(Type type);
    }
}