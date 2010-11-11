using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BoC.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsGenericAssignableFrom(this Type genericType, Type other)
        {
            if (genericType == null)
                throw new ArgumentNullException("genericType");
            if (!genericType.IsGenericType)
                throw new ArgumentException("this function is for generic types onl. Use IsAssignableFrom for non generic types");
            if (other == null)
                throw new ArgumentNullException("other");

            var allOthers = new List<Type> {other};
            if (genericType.IsInterface)
            {
                allOthers.AddRange(other.GetInterfaces());
            }

            foreach (var type in allOthers)
            {
                var currentType = type;
                while (currentType != null)
                {
                    if (currentType.IsGenericType)
                    {
                        currentType = currentType.GetGenericTypeDefinition();
                    }
                    if (currentType.IsSubclassOf(genericType) || currentType == genericType)
                        return true;

                    currentType = currentType.BaseType;
                }
            }
            return false;
        }
    }
}
