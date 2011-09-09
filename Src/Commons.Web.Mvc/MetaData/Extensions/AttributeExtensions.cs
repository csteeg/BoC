using System;
using System.Linq;
using System.Reflection;

namespace ModelMetadataExtensions.Extensions {
    public static class AttributeExtensions {
        public static TAttribute GetAttributeOnTypeOrAssembly<TAttribute>(this Type type) where TAttribute : Attribute {
            var attribute = type.First<TAttribute>();
            if (attribute == null) {
                attribute = type.Assembly.First<TAttribute>();
            }
            return attribute;
        }

        public static TAttribute First<TAttribute>(this ICustomAttributeProvider attributeProvider) where TAttribute : Attribute {
            return attributeProvider.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
        }
    }
}
