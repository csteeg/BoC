using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace JqueryMvc.Attributes
{
    /// <summary>
    /// Allows to load components through instantiation of UserControls if the files don't exist on disk
    /// </summary>
    public class ComponentViewsAttribute: ActionFilterAttribute
    {
        public ComponentViewsAttribute(string nameSpace):base()
        {
            this.NameSpace = nameSpace;
        }

        public string NameSpace { get; set;  }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
    }
}
