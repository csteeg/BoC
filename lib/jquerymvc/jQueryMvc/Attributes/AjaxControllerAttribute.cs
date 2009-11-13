using System;
using System.Web.Mvc;
using System.Web;
using JqueryMvc.Mvc;

namespace JqueryMvc.Attributes
{
    public class AjaxControllerAttribute: System.Web.Mvc.ActionFilterAttribute, IExceptionFilter
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            ViewResult result = filterContext.Result as ViewResult;

            if (result == null && (filterContext.HttpContext.Request.IsJqAjaxRequest() && filterContext.Exception != null))
            {
                if (filterContext.Result is RedirectToRouteResult)
                {
                    var redir = filterContext.Result as RedirectToRouteResult;
                    if (!redir.RouteValues.ContainsKey("__mvcajax")) //without this we can't detect an ajax request in FireFox :(
                    {
                        redir.RouteValues["__mvcajax"] = "true";
                    }
                }
                return;
            }
            if (filterContext.HttpContext.Request.IsJqAjaxRequest())
            {
                var responseType = filterContext.HttpContext.Request.GetPreferedResponseType();
                if (result != null && responseType == ResponseType.Html)
				{
					//load partial, only webformviewengine is supported now
					//if (result.ViewEngine is WebFormViewEngine)
					//	((WebFormViewEngine)result.ViewEngine).ViewLocator = DependencyResolver.GetImplementationOf<ComponentViewLocator>();
                    if (filterContext.Result is ViewResult)
                    {
                        string viewName = ((ViewResult)filterContext.Result).ViewName;
                        if (String.IsNullOrEmpty(viewName))
                            viewName = filterContext.RouteData.GetRequiredString("action");
                        ((ViewResult)filterContext.Result).ViewName = "_" + viewName;
                    }
				}
				else if (responseType == ResponseType.Json ||
                    responseType == ResponseType.Xml)
				{
					ViewDataDictionary data = result != null ? result.ViewData : null;
					object model = data;
					if (filterContext.Exception != null)
						model = filterContext.Exception;
                    else if (data != null && data.Model != null)
                        model = data.Model;

					if (model is Exception)
					{
					    filterContext.Result = null;

						Exception exc = ((Exception)model).InnerException ?? ((Exception)model);
						if (exc is HttpException)
						{
							filterContext.HttpContext.Response.StatusCode = ((HttpException)exc).GetHttpCode();
						}
						else
						{
							filterContext.HttpContext.Response.StatusCode = 500;
						}
						filterContext.HttpContext.Response.StatusDescription = exc.Message;

						model = exc;
					}

                    if (responseType == ResponseType.Json)
                    {
                        if (!(filterContext.Result is JsonResult))
                        {
                            filterContext.Result = new JsonResult()
                                                       {
                                                           Data = model
                                                       };
                        }
                    }
                    else if (!(filterContext.Result is XmlResult))
                    {
                        filterContext.Result = new XmlResult(model);
                    }
				}
            }
        }

		public void OnException(ExceptionContext filterContext)
        {
			if (filterContext.HttpContext.Request.GetPreferedResponseType() == ResponseType.Json)
			{
				filterContext.Result = new JsonResult()
				{
					Data = new {errors = new SimpleException[] {new SimpleException(filterContext.Exception)}}
				};
				filterContext.HttpContext.Response.Clear();
				filterContext.ExceptionHandled = true;
				if (filterContext.Exception is HttpException)
				{
					filterContext.HttpContext.Response.StatusCode = ((HttpException)filterContext.Exception).GetHttpCode();
				}
				else
				{
					filterContext.HttpContext.Response.StatusCode = 500;
				}
				filterContext.HttpContext.Response.StatusDescription = filterContext.Exception.Message;
			}
        }

	}
}
