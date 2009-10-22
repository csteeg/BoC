using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using JqueryMvc;

namespace System.Web
{
	public enum ResponseType
	{
		None,
		Html,
		Xml,
		Json,
	}

	static class HttpRequestBaseExtensions
	{

		public static ResponseType GetPreferedResponseType(this HttpRequestBase request)
		{
			ResponseType result = ResponseType.None;

            RouteData routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            
            // The requested format 
			// is specified via the querystring
            string format = request.QueryString["format"] ?? routeData.Values["resultformat"] as string ?? routeData.DataTokens["resultformat"] as string;

			if (String.IsNullOrEmpty(format))
			{
				if (request.AcceptTypes != null &&
					request.AcceptTypes.Length > 0 &&
					!String.IsNullOrEmpty(request.AcceptTypes[0]) &&
					request.AcceptTypes[0].IndexOf("json", StringComparison.InvariantCultureIgnoreCase) > 0)
					result = ResponseType.Json;
				else
					// If no querystring was specified, 
					// assume the default HTML format
					result = ResponseType.Html;
			}
			else
			{
				if (String.Equals(format, "html", StringComparison.InvariantCultureIgnoreCase))
				{
					result = ResponseType.Html;
				}
				else if (String.Equals(format, "xml", StringComparison.InvariantCultureIgnoreCase))
				{
					result = ResponseType.Xml;
				}
				else if (String.Equals(format, "json", StringComparison.InvariantCultureIgnoreCase))
				{
					result = ResponseType.Json;
				}
			}

			return result;
		}

		public static bool IsJqAjaxRequest(this HttpRequestBase request)
		{
            RouteData routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));

			return request != null && request.QueryString != null && request.Headers != null &&
				(      "true".Equals(routeData.DataTokens["__mvcajax"])
                    || "true".Equals(request.QueryString["__mvcajax"]) 
                    || !String.IsNullOrEmpty(request.Headers["Ajax"]) 
                    || "XMLHttpRequest".Equals(request.Headers["X-Requested-With"], StringComparison.InvariantCultureIgnoreCase)
                );
		}

        public static bool IsJqAjaxRequest(this HttpRequest request)
        {
            RouteData routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));

            return request != null && request.QueryString != null && request.Headers != null &&
                   ("true".Equals(routeData.DataTokens["__mvcajax"]) 
                   || "true".Equals(request.QueryString["__mvcajax"]) 
                   || !String.IsNullOrEmpty(request.Headers["Ajax"]) 
                   || "XMLHttpRequest".Equals(request.Headers["X-Requested-With"],StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
