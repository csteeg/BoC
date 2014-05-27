using System;
using System.Collections;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace BoC.Web.Mvc.Extensions
{
    public static class HtmlHelperExtensions
    {

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
