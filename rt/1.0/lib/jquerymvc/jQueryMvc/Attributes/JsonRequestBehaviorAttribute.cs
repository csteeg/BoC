using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace JqueryMvc.Attributes
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
