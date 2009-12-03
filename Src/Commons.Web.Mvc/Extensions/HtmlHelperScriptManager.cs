using System.Web.Mvc;
using BoC.Web.Mvc.ScriptManager;

namespace System.Web.Mvc
{
    public static class HtmlHelperScriptManager
    {
        public static SimpleScriptManager ScriptManager(this HtmlHelper helper)
        {
            return new SimpleScriptManager(helper);
        }
    }
}
