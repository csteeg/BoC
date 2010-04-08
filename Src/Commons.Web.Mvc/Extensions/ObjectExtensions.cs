using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace System
{
    public static class ObjectExtensions
    {
        static public IDictionary<string, object> ToDictionary(this object o)
        {
            return o.GetType().GetProperties()
                .Select(n => n.Name)
                .ToDictionary(k => k, k => o.GetType().GetProperty(k).GetValue(o, null));
        }

        public static string ToUnsortedList(this object data)
        {
            if (data != null && (data is IEnumerable))
            {
                StringBuilder ul = new StringBuilder("<ul>\n");
                bool hasItems = false;
                foreach (object m in (IEnumerable)data)
                {
                    hasItems = true;
                    ul.Append("<li>").Append(m).AppendLine("</li>");
                }
                if (!hasItems)
                    return String.Empty;
                else
                    return ul.AppendLine("</ul>").ToString(); ;

            }
            return String.Empty;
        }
    }
}
