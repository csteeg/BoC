using System.Security.Permissions;
using System.Web;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class IsPostExtensions {
        public static bool IsPost(this HttpRequest request) {
            return request.HttpMethod.ToUpperInvariant() == "POST";
        }

        public static bool IsPost(this HttpRequestBase request) {
            return request.HttpMethod.ToUpperInvariant() == "POST";
        }
    }
}