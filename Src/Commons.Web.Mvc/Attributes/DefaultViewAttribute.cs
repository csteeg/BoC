using System;
using System.Web.Mvc;

namespace BoC.Web.Mvc.Attributes
{
    public class DefaultViewAttribute: ActionFilterAttribute
    {
        private ViewEngineResult result;

        public DefaultViewAttribute(string viewName): base()
        {
            this.ViewName = viewName;
        }

        public string ViewName {get; set;}

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //this method assigns the View property _always_ (otherwise ViewResultBase.ExecuteResult has to do a FindView again)
            //be sure that this code matches the View-assigning part in ViewResultBase.ExecuteResult

            result = null;
            var filterResult = filterContext.Result as ViewResultBase;
            string viewName = null;
            if (filterResult != null)
            {
                viewName = String.IsNullOrEmpty(filterResult.ViewName)
                               ? filterContext.RouteData.GetRequiredString("action")
                               : filterResult.ViewName;
                
            }

            ViewEngineResult findView = null;

            if (filterResult is ViewResult)
            {
                findView = ViewEngines.Engines.FindView(
                    filterContext.Controller.ControllerContext,
                    viewName,
                    ((ViewResult)filterResult).MasterName);

                if (findView == null || findView.View == null)
                {
                    findView = ViewEngines.Engines.FindView(filterContext.Controller.ControllerContext, ViewName, ((ViewResult)filterContext.Result).MasterName);
                }
            }
            else if (filterResult is PartialViewResult)
            {
                findView = ViewEngines.Engines.FindPartialView(filterContext.Controller.ControllerContext, viewName);

                if (findView == null || findView.View == null)
                {
                    findView = ViewEngines.Engines.FindPartialView(filterContext.Controller.ControllerContext, ViewName);

                    if (findView != null && findView.View != null)
                    {
                        //found the default template, set viewname so that template can use that.
                        filterResult.ViewName = viewName;
                    }
                }
            }

            if (findView != null && findView.View != null)
            {
                filterResult.ViewData["OriginalViewName"] = viewName;
                result = findView;
                filterResult.View = findView.View;
            }

            base.OnResultExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (result != null && filterContext.Result is ViewResultBase)
            {
                result.ViewEngine.ReleaseView(filterContext.Controller.ControllerContext, ((ViewResultBase)filterContext.Result).View);
            }
            base.OnResultExecuted(filterContext);
        }
    }
}