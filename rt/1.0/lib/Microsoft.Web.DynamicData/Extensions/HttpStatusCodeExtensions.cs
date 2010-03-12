using System;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class HttpStatusCodeExtensions {
        public static ActionResult HttpStatusCode(this Controller controller, HttpStatusCode statusCode) {
            return HttpStatusCode(controller, statusCode, null);
        }

        public static ActionResult HttpStatusCode(this Controller controller, HttpStatusCode statusCode, object httpHeaders) {
            return new HttpStatusCodeResult(statusCode, httpHeaders);
        }

        public static ActionResult HttpStatusCode(this Controller controller, int statusCode) {
            return HttpStatusCode(controller, statusCode, null);
        }

        public static ActionResult HttpStatusCode(this Controller controller, int statusCode, object httpHeaders) {
            return new HttpStatusCodeResult((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), statusCode.ToString()), httpHeaders);
        }
    }
}