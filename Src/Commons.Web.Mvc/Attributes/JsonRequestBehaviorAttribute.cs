using System;
using System.Web.Mvc;

namespace BoC.Web.Mvc.Attributes
{
    public class JsonRequestBehaviorAttribute: Attribute
    {
        public JsonRequestBehaviorAttribute(JsonRequestBehavior jsonRequestBehavior)
        {
            this.JsonRequestBehavior = jsonRequestBehavior;
        }

        public JsonRequestBehavior JsonRequestBehavior { get; set;}
    }
}