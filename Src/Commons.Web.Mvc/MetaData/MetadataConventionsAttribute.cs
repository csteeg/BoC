using System;

namespace ModelMetadataExtensions {
    public class MetadataConventionsAttribute : Attribute {
        public Type ResourceType { get; set; }
    }
}