using System;
using System.ComponentModel;

namespace BoC.ComponentModel.TypeExtension
{
    public class ExtendedTypeDescriptionProvider : TypeDescriptionProvider
    {
        public ExtendedTypeDescriptionProvider(Type type): base(TypeDescriptor.GetProvider(type))
        {
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new ExtendedTypeDescriptor(objectType, base.GetTypeDescriptor(objectType, instance));
        }
    }
}
