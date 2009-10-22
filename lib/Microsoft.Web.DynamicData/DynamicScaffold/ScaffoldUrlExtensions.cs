using System.Security.Permissions;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class ScaffoldUrlExtensions {
        public static string ScaffoldUrl(this UrlHelper url, MetaTable table, string action) {
            return ScaffoldUrl(url, table.Name, action, (RouteValueDictionary)null /* routeValues */);
        }

        public static string ScaffoldUrl(this UrlHelper url, string tableName, string action) {
            return ScaffoldUrl(url, tableName, action, (RouteValueDictionary)null /* routeValues */);
        }

        public static string ScaffoldUrl(this UrlHelper url, MetaTable table, string action, object values) {
            return ScaffoldUrl(url, table.Name, action, new RouteValueDictionary(values));
        }

        public static string ScaffoldUrl(this UrlHelper url, string tableName, string action, object values) {
            return ScaffoldUrl(url, tableName, action, new RouteValueDictionary(values));
        }

        public static string ScaffoldUrl(this UrlHelper url, MetaTable table, string action, RouteValueDictionary routeValues) {
            return ScaffoldUrl(url, table.Name, action, routeValues);
        }

        public static string ScaffoldUrl(this UrlHelper url, string tableName, string action, RouteValueDictionary routeValues) {
            RouteValueDictionary finalValues = new RouteValueDictionary(routeValues ?? new RouteValueDictionary());
            finalValues.Add("table", tableName);
            finalValues.Add("action", action);

            return url.RouteUrl(finalValues);
        }
    }
}