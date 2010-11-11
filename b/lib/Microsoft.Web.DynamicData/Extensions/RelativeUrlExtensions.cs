using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class RelativeUrlExtensions {
        public static string RelativeUrl(this UrlHelper url, string relativeUrl) {
            if (relativeUrl == "~")
                return url.RequestContext.HttpContext.Request.ApplicationPath;

            if (!relativeUrl.StartsWith("~/"))
                return relativeUrl;

            return url.RequestContext.HttpContext.Request.ApplicationPath.TrimEnd('/') + relativeUrl.Substring(1);
        }
    }
}