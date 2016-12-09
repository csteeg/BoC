using System;
using System.Globalization;
using System.Reflection;
using System.Web.Mvc;
using Glass.Mapper.Sc;
using Sitecore.Mvc.Presentation;

namespace BoC.Persistence.SitecoreGlass
{
    public class ParametersTemplateValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new ParametersTemplateValueProvider();
        }

        private class ParametersTemplateValueProvider : IValueProvider
        {
            public bool ContainsPrefix(string prefix)
            {
                return "renderingParameters".Equals(prefix, StringComparison.OrdinalIgnoreCase);
            }

            public ValueProviderResult GetValue(string key)
            {
                var context = RenderingContext.CurrentOrNull;
                if (context == null) return null;

                var keyval = key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                var prefix = keyval[0];
                switch (prefix.ToLowerInvariant())
                {
                    case "renderingparameters":
                        if (RenderingContext.CurrentOrNull == null)
                            return new ValueProviderResult(null, "renderingParameters", CultureInfo.CurrentCulture);
                        return new ParametersTemplateValueProviderResult(RenderingContext.CurrentOrNull.Rendering[GlassHtml.Parameters], CultureInfo.CurrentCulture);
                    default:
                        return null;
                }
            }

        }
    }

    internal class ParametersTemplateValueProviderResult : ValueProviderResult
    {
        private readonly string _parameters;

        public ParametersTemplateValueProviderResult(string parameters, CultureInfo cultureInfo): base(parameters, parameters, cultureInfo)
        {
            _parameters = parameters;
        }

        static MethodInfo method = typeof(GlassHtml).GetMethod("GetRenderingParameters", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string) }, null);
        public override object ConvertTo(Type type, CultureInfo culture)
        {
            if (String.IsNullOrEmpty(_parameters))
                return null;

            var glassHtml = new GlassHtml(SitecoreContext.GetFromHttpContext());
            var genericMethod = method.MakeGenericMethod(type);
            return genericMethod.Invoke(glassHtml, new object[]{_parameters}) ??  base.ConvertTo(type, culture);
        }
    }
}
