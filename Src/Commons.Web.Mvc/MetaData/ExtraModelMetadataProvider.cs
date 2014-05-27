using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using BoC.Extensions;
using ModelMetadataExtensions;
using ModelMetadataExtensions.Extensions;

namespace BoC.Web.Mvc.MetaData
{
    public class ExtraModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        public ExtraModelMetadataProvider()
        {
            RequireConventionAttribute = false;
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

        /* localization by convention by Haacked: http://haacked.com/archive/2011/07/14/model-metadata-and-validation-localization-using-conventions.aspx */
        
        // Whether or not the conventions only apply to classes with the MetadatawonventionsAttribute attribute applied.
        public bool RequireConventionAttribute
        {
            get;
            set;
        }

        public static Type DefaultResourceType
        {
            get;
            set;
        }

        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            Func<IEnumerable<Attribute>, ModelMetadata> metadataFactory = (attr) => base.CreateMetadata(attr, containerType, modelAccessor, modelType, propertyName);

            var conventionType = containerType ?? modelType;

            var defaultResourceType = DefaultResourceType;
            var conventionAttribute = conventionType.GetAttributeOnTypeOrAssembly<MetadataConventionsAttribute>();
            if (conventionAttribute != null && conventionAttribute.ResourceType != null)
            {
                defaultResourceType = conventionAttribute.ResourceType;
            }
            else if (RequireConventionAttribute)
            {
                return metadataFactory(attributes);
            }

            ApplyConventionsToValidationAttributes(attributes, containerType, propertyName, defaultResourceType);

            var foundDisplayAttribute = attributes.FirstOrDefault(a => a is DisplayAttribute) as DisplayAttribute;

            if (foundDisplayAttribute.CanSupplyDisplayName())
            {
                return metadataFactory(attributes);
            }

            // Our displayAttribute is lacking. Time to get busy.
            DisplayAttribute displayAttribute = foundDisplayAttribute.Copy() ?? new DisplayAttribute();

            var rewrittenAttributes = attributes.Replace(foundDisplayAttribute, displayAttribute);

            // ensure resource type.
            displayAttribute.ResourceType = displayAttribute.ResourceType ?? defaultResourceType;

            if (displayAttribute.ResourceType != null)
            {
                // ensure resource name
                string displayAttributeName = GetDisplayAttributeName(containerType, propertyName, displayAttribute);
                if (displayAttributeName != null)
                {
                    displayAttribute.Name = displayAttributeName;
                }
                if (!displayAttribute.ResourceType.PropertyExists(displayAttribute.Name))
                {
                    displayAttribute.ResourceType = null;
                }
            }

            var metadata = metadataFactory(rewrittenAttributes);
            if (metadata.DisplayName == metadata.PropertyName)
            {
                metadata.DisplayName = metadata.DisplayName.SplitUpperCaseToString();
            }
            return metadata;
        }

        private static void ApplyConventionsToValidationAttributes(IEnumerable<Attribute> attributes, Type containerType, string propertyName, Type defaultResourceType)
        {
            foreach (ValidationAttribute validationAttribute in attributes.Where(a => (a as ValidationAttribute != null)))
            {
                if (string.IsNullOrEmpty(validationAttribute.ErrorMessage))
                {
                    string attributeShortName = validationAttribute.GetType().Name.Replace("Attribute", "");
                    var resourceType = validationAttribute.ErrorMessageResourceType ?? defaultResourceType;

                    //most extensive eg ClassName_PropertyName_Required
                    string resourceKey = GetResourceKey(containerType, propertyName) + "_" + attributeShortName;

                    if (!resourceType.PropertyExists(resourceKey))
                    {
                        //properytname only eg PropertyName_Required
                        resourceKey = propertyName + "_" + attributeShortName;
                        if (!resourceType.PropertyExists(resourceKey))
                        {
                            //most generic error, eg Error_Required
                            resourceKey = "Error_" + attributeShortName;
                            if (!resourceType.PropertyExists(resourceKey))
                            {
                                continue;
                            }
                        }
                    }
                    validationAttribute.ErrorMessageResourceType = resourceType;
                    validationAttribute.ErrorMessageResourceName = resourceKey;
                }
            }
        }

        private static string GetDisplayAttributeName(Type containerType, string propertyName, DisplayAttribute displayAttribute)
        {
            if (containerType != null)
            {
                if (String.IsNullOrEmpty(displayAttribute.Name))
                {
                    // check to see that resource key exists.
                    string resourceKey = GetResourceKey(containerType, propertyName);
                    if (displayAttribute.ResourceType.PropertyExists(resourceKey))
                    {
                        return resourceKey;
                    }
                    else
                    {
                        return propertyName;
                    }
                }

            }
            return null;
        }

        private static string GetResourceKey(Type containerType, string propertyName)
        {
            return containerType.Name + "_" + propertyName;
        }
    }
}
