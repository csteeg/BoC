using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BoC.Extensions
{
    public static class ObjectExtensions
    {
        static public IDictionary<string, object> ToDictionary(this object o)
        {
            return o.GetType().GetProperties()
                .Select(n => n.Name)
                .ToDictionary(k => k, k => o.GetType().GetProperty(k).GetValue(o, null));
        }

    }
}
