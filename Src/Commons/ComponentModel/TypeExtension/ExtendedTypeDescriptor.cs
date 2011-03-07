using System;
using System.ComponentModel;
using System.Linq;

namespace BoC.ComponentModel.TypeExtension
{
    public class ExtendedTypeDescriptor : CustomTypeDescriptor
    {
        private readonly Type componentType;

        public ExtendedTypeDescriptor(Type componentType, ICustomTypeDescriptor parent): base(parent)
        {
            this.componentType = componentType;
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(
                base.GetProperties().Cast<PropertyDescriptor>()
                    .Union(TypeDescriptor.GetAttributes(componentType)
                               .OfType<ExtendWithTypeAttribute>()
                               .SelectMany(et => 
                                           from prop in TypeDescriptor.GetProperties(et.With).Cast<PropertyDescriptor>()
                                           select new ExtendedPropertyDescriptor(prop, et.With)
                               )).ToArray());
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(
                base.GetProperties(attributes).Cast<PropertyDescriptor>()
                    .Union(TypeDescriptor.GetAttributes(componentType)
                               .OfType<ExtendWithTypeAttribute>()
                               .SelectMany(et =>
                                           from prop in TypeDescriptor.GetProperties(et.With, attributes).Cast<PropertyDescriptor>()
                                           select new ExtendedPropertyDescriptor(prop, et.With)
                               )).ToArray());
        }
    }
}