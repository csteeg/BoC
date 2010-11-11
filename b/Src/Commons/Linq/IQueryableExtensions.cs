using System;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace BoC.Linq
{
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class IQueryableExtensions {
        public static object FirstOrDefault(this IQueryable source) {
            if (source == null) {
                throw new ArgumentNullException("source");
            }

            foreach (object obj in source)
                return obj;

            return null;
        }
    }
}