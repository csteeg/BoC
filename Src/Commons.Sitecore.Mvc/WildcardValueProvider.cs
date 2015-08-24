using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Mvc.Presentation;
using Sitecore.SecurityModel;
using Sitecore.Web;

namespace BoC.Sitecore.Mvc
{
    public class WildcardValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            return new WildcardValueProvider();
        }
    }

    public class WildcardValueProvider : IValueProvider
    {
        public bool ContainsPrefix(string prefix)
        {
            return !String.IsNullOrEmpty(prefix) &&
                   prefix.StartsWith("wildcardItem") &&
                   PageContext.CurrentOrNull != null &&
                   PageContext.Current.Item != null &&
                   PageContext.Current.Item.Paths.FullPath.Contains("/*");
        }

        public ValueProviderResult GetValue(string key)
        {
            if (string.IsNullOrEmpty(key) || PageContext.CurrentOrNull == null || PageContext.Current.Item == null
                || !key.StartsWith("wildcardItem"))
                return null;

            var requestedWildcard = 0;
            if (key.Length > "wildcardItem".Length)
            {
                requestedWildcard = Int16.Parse(key.Substring("wildcardItem".Length)) - 1;
            }
            var wildcardItem = PageContext.Current.Item;
            var i = 0;
            using (new SecurityDisabler())
            {
                var currentWild = wildcardItem.Paths.FullPath.Count(c => c == '*')-1;
                if (currentWild < 0)
                    return null;
                while (wildcardItem != null)
                {
                    if (wildcardItem.Name == "*")
                    {
                        if (requestedWildcard == currentWild)
                        {
                            var val = WebUtil.GetUrlName(i);
                            return new ValueProviderResult(val, val, CultureInfo.InvariantCulture);
                        }
                        currentWild--;
                    }
                    wildcardItem = wildcardItem.Parent;
                    if (wildcardItem != null && !wildcardItem.Paths.FullPath.Contains("*"))
                        break;
                    i++;
                }
            }
            return null;
        }
    }
}

