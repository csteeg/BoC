using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BoC.Extensions {
    public static class EnumerableExtensions {
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> collection, T source, T replacement) {
            IEnumerable<T> collectionWithout = collection;
            if (source != null) {
                collectionWithout = collectionWithout.Except(new[] { source });
            }
            return collectionWithout.Union(new[] { replacement });
        }

        public static HtmlString ToUnsortedHtmlList(this IEnumerable data)
        {
            if (data != null)
            {
                var ul = new StringBuilder("<ul>\n");
                var hasItems = false;
                foreach (var m in data)
                {
                    hasItems = true;
                    ul.Append("<li>").Append(m).AppendLine("</li>");
                }
                return !hasItems ? new HtmlString(String.Empty) : new HtmlString(ul.AppendLine("</ul>").ToString());

            }
            return new HtmlString(String.Empty);
        }

    }
}
