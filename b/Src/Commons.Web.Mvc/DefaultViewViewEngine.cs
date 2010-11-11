using System.Web.Mvc;

namespace BoC.Web.Mvc
{
    public class DefaultViewViewEngine : WebFormViewEngine
    {
        public DefaultViewViewEngine(): base()
        {
            ViewLocationFormats = new[] {
                "~/Views/{1}/{0}.aspx",
                "~/Views/Shared/{0}.aspx"
            };

            PartialViewLocationFormats = new[] {
                "~/Views/{1}/{0}.ascx",
                "~/Views/Shared/{0}.ascx"
            };

            AreaViewLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.aspx",
                "~/Areas/{2}/Views/Shared/{0}.aspx"
            };

            AreaPartialViewLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.ascx",
                "~/Areas/{2}/Views/Shared/{0}.ascx"
            };
        }
    }
}
