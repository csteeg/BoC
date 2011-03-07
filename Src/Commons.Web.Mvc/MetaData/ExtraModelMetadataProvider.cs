using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BoC.Web.Mvc.MetaData
{
    public class ExtraModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
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
    }
}
