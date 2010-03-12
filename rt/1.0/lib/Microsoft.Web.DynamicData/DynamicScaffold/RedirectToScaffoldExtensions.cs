using System.Security.Permissions;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class RedirectToScaffoldExtensions {
        public static ActionResult RedirectToScaffold(this Controller controller, MetaTable table, string action) {
            return RedirectToScaffold(controller, table.Name, action, (RouteValueDictionary)null);
        }

        public static ActionResult RedirectToScaffold(this Controller controller, string tableName, string action) {
            return RedirectToScaffold(controller, tableName, action, (RouteValueDictionary)null);
        }

        public static ActionResult RedirectToScaffold(this Controller controller, MetaTable table, string action, object values) {
            return RedirectToScaffold(controller, table.Name, action, new RouteValueDictionary(values));
        }

        public static ActionResult RedirectToScaffold(this Controller controller, string tableName, string action, object values) {
            return RedirectToScaffold(controller, tableName, action, new RouteValueDictionary(values));
        }

        public static ActionResult RedirectToScaffold(this Controller controller, MetaTable table, string action, RouteValueDictionary routeValues) {
            return RedirectToScaffold(controller, table.Name, action, routeValues);
        }

        public static ActionResult RedirectToScaffold(this Controller controller, string tableName, string action, RouteValueDictionary routeValues) {
            RouteValueDictionary finalValues = new RouteValueDictionary(routeValues ?? new RouteValueDictionary());
            finalValues.Add("table", tableName);
            finalValues.Add("action", action);

            return new RedirectToRouteResult(finalValues);
        }
    }
}