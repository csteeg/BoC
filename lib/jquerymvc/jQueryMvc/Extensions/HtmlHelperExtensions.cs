using System;
using System.Collections;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.Routing;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Microsoft.Web.Mvc;

namespace System.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static string UnsortedList(this HtmlHelper helper, object data)
        {
            if (data != null && (data is IEnumerable))
            {
                return data.ToUnsortedList();

            }
            return String.Empty;
        }

        public static IDisposable Form(this HtmlHelper helper, string controller, string action, FormMethod method, Object attributes)
        {
            return helper.Form(controller, action, method, attributes.ToDictionary());
        }

        public static string Label(this HtmlHelper helper, string forItem, string text)
        {
            return String.Format("<label for=\"{0}\">{1}</label>", forItem, text);
        }

    }
}
