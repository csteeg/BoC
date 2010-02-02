using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Mapping;
using FluentNHibernate.Mapping.Providers;
using FluentNHibernate.Utils;

namespace BoC.Persistence.NHibernate.FluentExtensions
{
    public static class AutoMappingExtensions
    {
        private static readonly FieldInfo fi = typeof(ComponentPartBase<IDictionary>).GetField("propertyName",
                                                                                          BindingFlags.GetField |
                                                                                          BindingFlags.Instance |
                                                                                          BindingFlags.NonPublic);

        public static DynamicComponentPart<IDictionary> AddDynamicComponent<T>(this AutoMapping<T> autoMapping, Expression<Func<T, IDictionary>> expression, Action<DynamicComponentPart<IDictionary>> action)
        {
            var propertyAccessor = autoMapping.GetType().GetProperty("Components",
                                                                     BindingFlags.GetProperty |
                                                                     BindingFlags.Instance |
                                                                     BindingFlags.NonPublic);
            var components = propertyAccessor.GetValue(autoMapping, null) as IEnumerable<IComponentMappingProvider>;

            if (components == null)
            {
                return autoMapping.DynamicComponent(expression, action);
            }

            var component = (from c in components
                             where
                                 (c is DynamicComponentPart<IDictionary> &&
                                  fi.GetValue(c) == ReflectionHelper.GetProperty(expression).Name)
                             select c as DynamicComponentPart<IDictionary>).FirstOrDefault();

            if (component == null)
            {
                return autoMapping.DynamicComponent(expression, action);
            }

            action(component);
            return component;
        }
    }
}
