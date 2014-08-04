using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BoC.InversionOfControl;
using BoC.Services;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;

namespace BoC.Persistence.SitecoreGlass.Mvc
{
    public class SitecoreValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new SitecoreValueProvider();
        }

        private class SitecoreValueProvider : IValueProvider
        {
            public bool ContainsPrefix(string prefix)
            {
                return
                    "contextItem".Equals(prefix, StringComparison.OrdinalIgnoreCase)
                    || "renderingItem".Equals(prefix, StringComparison.OrdinalIgnoreCase)
                    || "dataSource".Equals(prefix, StringComparison.OrdinalIgnoreCase);
            }

            public ValueProviderResult GetValue(string key)
            {
                var context = RenderingContext.CurrentOrNull;
                if (context == null) return null;

                var keyval = key.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
                var prefix = keyval[0];
                var property = keyval.Length > 1 ? keyval[1] : null;
                switch (prefix.ToLowerInvariant())
                {
                    case "contextitem":
                        if (context.ContextItem == null)
                            return new ValueProviderResult(null, "contextitem", CultureInfo.CurrentCulture);
                        return GetValueResult(context.ContextItem, property);

                    case "renderingitem":
                        if (context.Rendering == null || context.Rendering.RenderingItem == null)
                            return new ValueProviderResult(null, "renderingitem", CultureInfo.CurrentCulture);;
                        return GetValueResult(context.Rendering.RenderingItem.InnerItem, property);

                    case "datasource":
                        if (context.Rendering == null || string.IsNullOrEmpty(context.Rendering.DataSource))
                            return new ValueProviderResult(null, "datasource", CultureInfo.CurrentCulture);
                        if (!ID.IsID(context.Rendering.DataSource))
                            return new ValueProviderResult(context.Rendering.DataSource, context.Rendering.DataSource, CultureInfo.CurrentCulture);;
                        var item = context.PageContext.Database.GetItem(context.Rendering.DataSource);
                        return GetValueResult(item, property);
                    default:
                        return null;
                }
            }


            private ValueProviderResult GetValueResult(Item item, string key)
            {
                if (item == null)
                    return new ValueProviderResult(null, key, CultureInfo.CurrentCulture);
                if (string.IsNullOrEmpty(key))
                {
                    return new SitecoreValueProviderResult(item, item.ID.ToString(), CultureInfo.CurrentCulture);
                }
                if ("id".Equals(key, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new ValueProviderResult(item.ID.ToGuid(), item.ID.ToString(), CultureInfo.CurrentCulture);
                }
                if ("path".Equals(key, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new ValueProviderResult(item.Paths.FullPath, item.Paths.FullPath, CultureInfo.CurrentCulture);
                }
                var field = item.Fields.FirstOrDefault(f => f.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));
                return field != null ? new ValueProviderResult(field.GetValue(true, true), field.GetValue(true, true), CultureInfo.CurrentCulture) : null;
            }
        }
    }

    internal class SitecoreValueProviderResult : ValueProviderResult
    {
        private readonly Item _item;
        private readonly string _stringValue;

        public SitecoreValueProviderResult(Item item, string stringValue, CultureInfo currentCulture)
        {
            _item = item;
            _stringValue = stringValue;
        }

        public override object ConvertTo(Type type, CultureInfo culture)
        {
            if (_item == null)
                return null;

            if (typeof (IBaseEntity).IsAssignableFrom(type))
            {
                var serviceType = typeof(IModelService<>).MakeGenericType(type);
                if (IoC.Resolver.IsRegistered(serviceType))
                {
                    var service = IoC.Resolver.Resolve(serviceType) as IModelService;
                    if (service != null)
                    {
                        return service.Get(_item.ID.ToGuid());
                    }
                }

            }
            return base.ConvertTo(type, culture);
        }
    }
}
