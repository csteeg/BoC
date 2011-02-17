using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace BoC.Web.Mvc.MetaData
{
    public class ExtraModelMetadataTypeDescriptor : CustomTypeDescriptor
    {
        private readonly Type type;
        private readonly ExtraModelMetadataProvider metadataProvider;

        public ExtraModelMetadataTypeDescriptor(
            Type type,
            ExtraModelMetadataProvider metadataProvider, 
            ICustomTypeDescriptor parent): base(parent)
        {
            this.type = type;
            this.metadataProvider = metadataProvider;
        }

        public override AttributeCollection GetAttributes()
        {
            return new AttributeCollection(
                base.GetAttributes().Cast<Attribute>().Union(metadataProvider.ExtraModelMetaDataRegistry.GetExtraAttributes(type)).ToArray());
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(
                base.GetProperties().Cast<PropertyDescriptor>()
                    .Union(GetAttributes()
                        .OfType<ExtendWithTypeAttribute>()
                        .SelectMany(et => metadataProvider
                                .GetExtraModelMetaDataTypeDescriptor(et.With)
                                .GetProperties().Cast<PropertyDescriptor>()))
                    .ToArray());
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(
                base.GetProperties(attributes).Cast<PropertyDescriptor>()
                    .Union(GetAttributes()
                        .OfType<ExtendWithTypeAttribute>()
                        .SelectMany(et => metadataProvider
                                .GetExtraModelMetaDataTypeDescriptor(et.With)
                                .GetProperties(attributes).Cast<PropertyDescriptor>()))
                    .ToArray());
        }
    }
}
