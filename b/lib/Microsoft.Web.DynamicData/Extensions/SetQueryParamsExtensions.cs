using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class SetQueryParamsExtensions {
        public static string SetQueryParams(this UrlHelper helper, object values) {
            return SetQueryParams(helper, new RouteValueDictionary(values));
        }

        // REVIEW: Tried to use UriBuilder here but it ended up adding extra question marks
        public static string SetQueryParams(this UrlHelper helper, IDictionary<string, object> values) {
            var segments = helper.RequestContext.HttpContext.Request.RawUrl.Split(new char[] { '?' }, 2);
            var path = segments[0];
            var query = segments.Length > 1 ? segments[1] : "";
            var queryParts = QuerySplit(query);

            foreach (KeyValuePair<string, object> kvp in values)
                queryParts[kvp.Key] = kvp.Value.ToString();

            query = QueryJoin(queryParts);
            return path + "?" + query;
        }

        // REVIEW: Is there a standard and safe query parser and joiner?
        static string QueryJoin(Dictionary<string, string> queryParts) {
            return String.Join(
                "&",
                queryParts.Aggregate(
                    new List<string>(),
                    (list, kvp) => { list.Add(kvp.Key + "=" + kvp.Value); return list; }
                ).ToArray());
        }

        // REVIEW: Is there a standard and safe query parser and joiner?
        static Dictionary<string, string> QuerySplit(string query) {
            return query.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(
                s => (s.Split('='))[0],
                s => (s.Split('='))[1],
                StringComparer.InvariantCultureIgnoreCase);
        }
    }
}