using System;
using System.Web.Mvc;

namespace BoC.Web.Mvc.Attributes
{
    public class JsonRequestBehaviorAttribute: ActionFilterAttribute
    {
        public JsonRequestBehaviorAttribute(JsonRequestBehavior jsonRequestBehavior)
        {
            this.JsonRequestBehavior = jsonRequestBehavior;
        }

        public JsonRequestBehavior JsonRequestBehavior { get; set;}

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if ((filterContext.Result is JsonResult))
            {
                ((JsonResult) filterContext.Result).JsonRequestBehavior = JsonRequestBehavior;
            }
            base.OnResultExecuting(filterContext);
        }
    }
}