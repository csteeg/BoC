using System;
using System.ComponentModel;

namespace BoC.ComponentModel.TypeExtension
{
    public class ExtendedTypesRegistry : IExtendedTypesRegistry
    {
        public void Extend<TFrom,TWith>()
        {
            Extend(typeof (TFrom), typeof (TWith));
        }

        public void Extend(Type type, Type with)
        {
            TypeDescriptor.AddAttributes(type, new ExtendWithTypeAttribute(with));
        }
    }
}
