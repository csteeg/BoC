using System;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using JqueryMvc.Mvc;

namespace JqueryMvc.Attributes
{
    public class AjaxControllerAttribute: System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            ViewResult result = filterContext.Result as ViewResult;

            if (result == null && filterContext.HttpContext.Request.IsJqAjaxRequest() && filterContext.Exception == null && filterContext.Result is RedirectToRouteResult)
            {
                var redir = filterContext.Result as RedirectToRouteResult;
                if (!redir.RouteValues.ContainsKey("__mvcajax"))
                    //without this we can't detect an ajax request in FireFox :(
                {
                    redir.RouteValues["__mvcajax"] = "true";
                }
                return;
            }

            if (filterContext.HttpContext.Request.IsJqAjaxRequest())
            {
                var responseType = filterContext.HttpContext.Request.GetPreferedResponseType();
                if (result != null && responseType == ResponseType.Html)
				{
				    var viewResult = filterContext.Result as ViewResult;
					//prefer partial
                    if (viewResult != null)
                    {
                        filterContext.Result = new PartialViewResult()
                                                   {
                                                       TempData = viewResult.TempData,
                                                       ViewData = viewResult.ViewData,
                                                       ViewName = viewResult.ViewName
                                                   };
                    }
				}
				else if (responseType == ResponseType.Json || responseType == ResponseType.Xml)
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
					    filterContext.ExceptionHandled = true;
						model = new SimpleException(exc);
					}

                    if (responseType == ResponseType.Json)
                    {
                        //see if custom JsonRequestBehavior has been set
                        var attrib = filterContext.ActionDescriptor.GetCustomAttributes(typeof (JsonRequestBehaviorAttribute), true).OfType<JsonRequestBehaviorAttribute>().FirstOrDefault();
                        if (attrib == null)
                        {
                            filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof (JsonRequestBehaviorAttribute), true).OfType<JsonRequestBehaviorAttribute>().FirstOrDefault();
                        }

                        if (!(filterContext.Result is JsonResult))
                        {
                            filterContext.Result = new JsonResult()
                                                       {
                                                           Data = model,
                                                           JsonRequestBehavior = (attrib == null) ? JsonRequestBehavior.DenyGet : attrib.JsonRequestBehavior
                                                       };
                        } 
                        else if (attrib != null)
                        {
                            (filterContext.Result as JsonResult).JsonRequestBehavior = attrib.JsonRequestBehavior;
                        }
                    }
                    else if (!(filterContext.Result is XmlResult))
                    {
                        filterContext.Result = new XmlResult(model);
                    }
				}
            }
        }

        /* In asp.net mvc 2, we OnActionExecuted seems to be triggered always, so we don't need this seperate function anymore 
        void IExceptionFilter.OnException(ExceptionContext filterContext)
        {
            if (filterContext.Result == null && filterContext.HttpContext.Request.GetPreferedResponseType() == ResponseType.Json)
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
        */
    }
}
