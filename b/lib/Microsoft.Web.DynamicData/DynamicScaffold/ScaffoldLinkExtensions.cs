using System;
using System.Security.Permissions;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class ScaffoldLinkExtensions {
        public static string ScaffoldLink(this HtmlHelper html, string text, MetaTable table, string action) {
            return ScaffoldLink(html, text, table.Name, action, (RouteValueDictionary)null);
        }

        public static string ScaffoldLink(this HtmlHelper html, string text, string tableName, string action) {
            return ScaffoldLink(html, text, tableName, action, (RouteValueDictionary)null);
        }

        public static string ScaffoldLink(this HtmlHelper html, string text, MetaTable table, string action, object values) {
            return ScaffoldLink(html, text, table.Name, action, new RouteValueDictionary(values));
        }

        public static string ScaffoldLink(this HtmlHelper html, string text, string tableName, string action, object values) {
            return ScaffoldLink(html, text, tableName, action, new RouteValueDictionary(values));
        }

        public static string ScaffoldLink(this HtmlHelper html, string text, MetaTable table, string action, RouteValueDictionary routeValues) {
            return ScaffoldLink(html, text, table.Name, action, routeValues);
        }

        public static string ScaffoldLink(this HtmlHelper html, string text, string tableName, string action, RouteValueDictionary routeValues) {
            var url = new UrlHelper(html.ViewContext.RequestContext);
            return String.Format("<a href=\"{0}\">{1}</a>", url.ScaffoldUrl(tableName, action, routeValues), html.Encode(text));
        }
    }
}