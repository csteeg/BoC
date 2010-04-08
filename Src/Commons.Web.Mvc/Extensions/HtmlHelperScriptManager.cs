using System.Collections;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BoC.Web.Mvc.ScriptManager;

namespace System.Web.Mvc
{
    public static class HtmlHelperScriptManager
    {
        public static SimpleScriptManager ScriptManager(this HtmlHelper helper)
        {
            return new SimpleScriptManager(helper);
        }

        public static void RenderPartial(this HtmlHelper helper)
        {
            helper.RenderPartial(helper.ViewData["OriginalViewName"] as string);
        }

        public static string UnsortedList(this HtmlHelper helper, object data)
        {
            if (data != null && (data is IEnumerable))
            {
                return data.ToUnsortedList();

            }
            return String.Empty;
        }

    }
}
