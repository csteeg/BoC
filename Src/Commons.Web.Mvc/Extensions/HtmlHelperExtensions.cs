using System.Collections;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BoC.Web.Mvc.ScriptManager;

namespace System.Web.Mvc
{
    public static class HtmlHelperExtensions
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

        #region GetResourceUrl
        private static readonly Func<Type, string, bool, string> GetWebResourceUrlInternal = FindMethod();
        private static Func<Type, string, bool, string> FindMethod()
        {
            var method = typeof(System.Web.Handlers.AssemblyResourceLoader)
                .GetMethod("GetWebResourceUrl", BindingFlags.Static | BindingFlags.NonPublic,null,
                    new [] {typeof(Type), typeof(String), typeof(bool)}, null);

            return (Func<Type, string, bool, string>)
                Delegate.CreateDelegate(typeof(Func<Type, string, bool, string>), method, true);
        }

        public static string GetResourceUrl(this HtmlHelper helper, Type typeInAssembly, string resourcePath)
        {
            if (null == typeInAssembly) throw new ArgumentNullException("typeInAssembly");
            if (string.IsNullOrEmpty(resourcePath)) throw new ArgumentNullException("resourcePath");

            string result = GetWebResourceUrlInternal(typeInAssembly, resourcePath, false);
            return VirtualPathUtility.ToAbsolute("~/" + result);
        }

        public static string GetResourceUrl<TAssemblyObject>(this HtmlHelper helper, string resourcePath)
        {
            return GetResourceUrl(helper, typeof(TAssemblyObject), resourcePath);
        }

        #endregion
    }
}
