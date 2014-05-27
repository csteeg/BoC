using System.Collections.Generic;
using System.Net;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BoC.Web.Mvc.ActionResults
{
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class HttpStatusCodeResult : ActionResult {
        IDictionary<string, object> _httpHeaders;
        HttpStatusCode _statusCode;

        public HttpStatusCodeResult(HttpStatusCode statusCode)
            : this(statusCode, new RouteValueDictionary()) { }

        public HttpStatusCodeResult(HttpStatusCode statusCode, object httpHeaders)
            : this(statusCode, new RouteValueDictionary(httpHeaders ?? new object())) { }

        public HttpStatusCodeResult(HttpStatusCode statusCode, IDictionary<string, object> httpHeaders) {
            _statusCode = statusCode;
            _httpHeaders = httpHeaders;
        }

        public override void ExecuteResult(ControllerContext context) {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.StatusCode = (int)_statusCode;
            context.HttpContext.Response.StatusDescription = _statusCode.ToString();

            foreach (var kvp in _httpHeaders)
                context.HttpContext.Response.AddHeader(kvp.Key, kvp.Value.ToString());

            context.HttpContext.Response.Write(((int)_statusCode).ToString() + " " + _statusCode.ToString());
        }
    }
}