using System.Web.Mvc;
using System.Web;
using JqueryMvc.Attributes;

namespace JqueryMvc.Mvc
{
    public class DefaultViewViewEngine : WebFormViewEngine //if we create an independent viewengine, caching is disabled :(, just override the default then
    {
        override public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            //TODO: Use caching when asp.net mvc 2's cache is fixed
            var result = base.FindView(controllerContext, viewName, masterName, false);

            if (controllerContext == null || controllerContext.Controller == null)
                return result;

            if (result == null || result.View == null)
			{
                if (controllerContext.HttpContext.Items["_ORIGINGAL_VIEWNAME"] == null)
                {
                    controllerContext.HttpContext.Items["_ORIGINGAL_VIEWNAME"] = viewName;
                }
			    if (!viewName.StartsWith("_"))
                {
                    object[] attribs = controllerContext.Controller.GetType().GetCustomAttributes(typeof(DefaultViewAttribute), true);
                    if (attribs != null && attribs.Length > 0)
                    {
                        foreach (DefaultViewAttribute attrib in attribs)
                        {
                            if (attrib.ViewName != viewName)
                            {
                                result = ViewEngines.Engines.FindView(controllerContext, attrib.ViewName, masterName);
                            }
                        }
                    }
                }
                else if (controllerContext.HttpContext.Request.IsJqAjaxRequest())
                {
                    result = ViewEngines.Engines.FindView(controllerContext, viewName.Substring(1), masterName);
                }
			}
            else if (controllerContext.HttpContext.Items["_ORIGINGAL_VIEWNAME"] == null && result != null && result.View!=null)
            {
                controllerContext.HttpContext.Items["_ORIGINGAL_VIEWNAME"] = viewName;
            }

			return result;
		}
    }
}
