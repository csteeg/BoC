using System;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace BoC.Web.Mvc.MetaData
{
    public class ExtraModelMetadataTypeDescriptionProvider : TypeDescriptionProvider
    {
        public ExtraModelMetadataTypeDescriptionProvider(Type type): base(TypeDescriptor.GetProvider(type))
        {
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new ExtraModelMetadataTypeDescriptor(objectType, base.GetTypeDescriptor(objectType, instance));
        }
    }
    public class ExtraModelMetadataTypeDescriptor : CustomTypeDescriptor
    {
        private readonly Type componentType;

        public ExtraModelMetadataTypeDescriptor(Type componentType, ICustomTypeDescriptor parent): base(parent)
        {
            this.componentType = componentType;
        }

        /*public override AttributeCollection GetAttributes()
        {
            return new AttributeCollection(
                base.GetAttributes().Cast<Attribute>().Union(metadataProvider.ExtraModelMetaDataRegistry.GetExtraAttributes(type)).ToArray());
        }*/

        public override PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(
                base.GetProperties().Cast<PropertyDescriptor>()
                    .Union(TypeDescriptor.GetAttributes(componentType)
                        .OfType<ExtendWithTypeAttribute>()
                        .SelectMany(et => 
                            from prop in TypeDescriptor.GetProperties(et.With).Cast<PropertyDescriptor>()
                            select new ExtraMetaDataPropertyDescriptor(prop, et.With)
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
                                select new ExtraMetaDataPropertyDescriptor(prop, et.With)
                    )).ToArray());
        }
    }
}
