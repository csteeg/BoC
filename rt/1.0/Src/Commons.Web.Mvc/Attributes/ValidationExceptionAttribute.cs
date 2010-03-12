using System;
using System.Web.Mvc;
using System.Web;
using BoC.Validation;
using BoC.Web.Mvc.Validation;
using JqueryMvc.Mvc;

namespace BoC.Web.Mvc.Attributes
{
    public class ValidationExceptionAttribute: ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null || filterContext.Exception == null)
                return;

            var exception = filterContext.Exception;
            if (exception.InnerException is RulesException)
                exception = exception.InnerException;

            if (exception is RulesException)
            {
                ((RulesException) exception).AddModelStateErrors(filterContext.Controller.ViewData.ModelState);
                filterContext.ExceptionHandled = true;
            }
        }

    }
}