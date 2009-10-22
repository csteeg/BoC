using System;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class FormExtensions {
        public static IDisposable FormRoute(this HtmlHelper html, string routeName, FormMethod method, object values) {
            return FormRoute(html, routeName, method, new RouteValueDictionary(values));
        }

        public static IDisposable FormRoute(this HtmlHelper html, string routeName, FormMethod method, RouteValueDictionary valuesDictionary) {
            VirtualPathData virtualPath = RouteTable.Routes.GetVirtualPath(html.ViewContext.RequestContext, routeName, valuesDictionary);
            string formAction = (virtualPath == null) ? null : virtualPath.VirtualPath;
            return html.BeginRouteForm(routeName, method, valuesDictionary);
        }
    }
}