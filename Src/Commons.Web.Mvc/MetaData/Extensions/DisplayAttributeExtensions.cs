using System.ComponentModel.DataAnnotations;

namespace ModelMetadataExtensions.Extensions {
    public static class DisplayAttributeExtensions {
        public static DisplayAttribute Copy(this DisplayAttribute attribute) {
            if (attribute == null) {
                return null;
            }
            var copy = new DisplayAttribute();

            // DisplayAttribute is sealed, so safe to copy.
            copy.Name = attribute.Name;
            copy.GroupName = attribute.GroupName;
            copy.Description = attribute.Description;
            copy.ResourceType = attribute.ResourceType;
            copy.ShortName = attribute.ShortName;
            
            return copy;
        }

        public static bool CanSupplyDisplayName(this DisplayAttribute attribute) {
            return attribute != null 
                && attribute.ResourceType != null 
                && !string.IsNullOrEmpty(attribute.Name);
        }
    }
}
