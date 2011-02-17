using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace BoC.Web.Mvc.MetaData
{
    public class ExtraModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        private readonly IExtraModelMetadataRegistry registry;
        public ExtraModelMetadataProvider() : this(new ExtraModelMetadataRegistry()) {}
        public ExtraModelMetadataProvider(IExtraModelMetadataRegistry registry)
        {
            this.registry = registry;
        }

        public IExtraModelMetadataRegistry ExtraModelMetaDataRegistry
        {
            get { return registry; }
        }

        public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
        {
            return base.GetMetadataForProperties(container, containerType).Select(Modify);
        }

        private ModelMetadata Modify(ModelMetadata metadata)
        {
            if (metadata == null)
                return null;

            if (metadata.ModelType.IsEnum && String.IsNullOrEmpty(metadata.TemplateHint))
            {
                metadata.TemplateHint = "Enum";
            }

            return metadata;
        }

        public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName)
        {
            return Modify(base.GetMetadataForProperty(modelAccessor, containerType, propertyName));
        }

        public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
        {
            return Modify(base.GetMetadataForType(modelAccessor, modelType));
        }

        protected override System.ComponentModel.ICustomTypeDescriptor GetTypeDescriptor(Type type)
        {
            return GetExtraModelMetaDataTypeDescriptor(type);
        }

        internal System.ComponentModel.ICustomTypeDescriptor GetExtraModelMetaDataTypeDescriptor(Type type)
        {
            return new ExtraModelMetadataTypeDescriptor(type, this, GetDefaultDataTypeDescriptor(type));
        }

        internal System.ComponentModel.ICustomTypeDescriptor GetDefaultDataTypeDescriptor(Type type)
        {
            return base.GetTypeDescriptor(type);
        }
    }
}
