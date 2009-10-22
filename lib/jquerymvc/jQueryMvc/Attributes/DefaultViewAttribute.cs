using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using Microsoft.Web.Mvc;

namespace JqueryMvc.Attributes
{
    public class DefaultViewAttribute: ActionFilterAttribute
    {
        public DefaultViewAttribute(string viewName): base()
        {
            this.ViewName = viewName;
        }

        public string ViewName {get; set;}

    }
}
