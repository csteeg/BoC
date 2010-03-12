using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

namespace System.Web.Mvc
{
    public static class ViewDataExtensions
    {
        public static void AddMessage(this ViewDataDictionary viewData, string message)
        {
            if (viewData == null)
                return;

            if (!viewData.ContainsKey("messages"))
                viewData["messages"] = new List<string>();

            if (!(viewData["messages"] is IList<string>))
            {
                throw new NotSupportedException("Messages in viewData should be of type IList<string>");
            }
            else
            {
                (viewData["messages"] as IList<string>).Add(message);
            }
        }

        public static void AddError(this ViewDataDictionary viewData, string error)
        {
            if (viewData == null)
                return;

            if (!viewData.ContainsKey("errors"))
                viewData["errors"] = new List<string>();

            if (!(viewData["errors"] is IList<string>))
            {
                throw new NotSupportedException("Errors in viewData should be of type IList<string>");
            }
            else
            {
                (viewData["errors"] as IList<string>).Add(error);
            }
        }
    }
}
