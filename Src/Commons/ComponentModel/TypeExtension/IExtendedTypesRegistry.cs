using System;

namespace BoC.ComponentModel.TypeExtension
{
    public interface IExtendedTypesRegistry
    {
        void Extend<TFrom,TWith>();
        void Extend(Type type, Type with);
    }
}