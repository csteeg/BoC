using System;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using BoC.InversionOfControl;
using BoC.Web.Mvc.ActionResults;

namespace BoC.Web.Mvc.Attributes
{
    /// <summary>
    /// The AjaxControllerAttribute does a few things
    ///     - if the current request is a json-request, it returns the model as a json result
    ///     - if the current request is an xml-request, it returns the model as an xml result
    ///     - if the current request is an ajax-request, it returns a partialviewresult
    /// If your controller uses this attribute, you can always return a normal ViewResult, it will be transformed by this actionfilter
    /// </summary>
    public class AjaxControllerAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            
            var responseType = filterContext.HttpContext.Request.GetPreferedResponseType();

            if (filterContext.HttpContext.Request.IsJqAjaxRequest() 
                && filterContext.Exception == null
                && ((filterContext.Result is RedirectToRouteResult) || filterContext.Result is RedirectResult))
            {
                //If we have redirect result, and it is an ajax request:
                // - IF it is json or xml:
                // we'll return 200 OK with the URL in the header, the user can decide if he wants to redirect
                // - if it is an html request, we'll redirect to the new action but send info about the ajax-request with it
                
                var redir = filterContext.Result as RedirectToRouteResult;
                if (responseType == ResponseType.Json || responseType == ResponseType.Xml) //we'll return a redirect object
                {
                    string url;
                    if (redir != null)
                    {
                        url = UrlHelper.GenerateUrl(
                            redir.RouteName,
                            null /* actionName */,
                            null /* controllerName */,
                            redir.RouteValues, 
                            RouteTable.Routes, 
                            filterContext.RequestContext,
                            false /* includeImplicitMvcValues */);
                    }
                    else
                    {
                        url = ((RedirectResult) filterContext.Result).Url;
                    }
                    filterContext.RequestContext.HttpContext.Response.AddHeader("Location", url);
                }
                else if (redir != null && !redir.RouteValues.ContainsKey("__mvcajax")) //without these extra params we can't detect an ajax request in FireFox :(
                {
                    redir.RouteValues["__mvcajax"] = "true";
                    redir.RouteValues["resultformat"] = filterContext.RouteData.Values["resultformat"] ??
                                                        filterContext.RouteData.DataTokens["resultformat"];
                    redir.RouteValues["format"] = filterContext.RouteData.Values["format"];
                    return;
                }

            }
            
            if (filterContext.IsChildAction || filterContext.HttpContext.Request.IsJqAjaxRequest())
            {
                var result = filterContext.Result as ViewResult;
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
                                                       ViewName = viewResult.ViewName,
                                                   };
                    }
                }
                else if (responseType == ResponseType.Json || responseType == ResponseType.Xml)
                {
                    ViewDataDictionary data = filterContext.Result is ViewResultBase ? ((ViewResultBase)filterContext.Result).ViewData : filterContext.Controller.ViewData;
					
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
                        if (!(filterContext.Result is JsonResult))
                        {
                            var jsonresult =    IoC.Resolver.IsRegistered<JsonResult>() ?
                                                IoC.Resolver.Resolve<JsonResult>() : new JsonResult();
                            jsonresult .Data = model ?? new object();
                            filterContext.Result = jsonresult;
                        } 
                    }
                    else if (!(filterContext.Result is XmlResult))
                    {
                        filterContext.Result = new XmlResult(model);
                    }
                }
            }
        }

        /* In asp.net mvc 2, the OnActionExecuted seems to be triggered always, so we don't need this seperate function anymore 
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