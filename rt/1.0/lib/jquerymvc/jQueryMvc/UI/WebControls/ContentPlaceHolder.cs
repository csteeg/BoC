using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Collections;
using System.Reflection;
using System.Web.UI;

namespace JqueryMvc.UI.WebControls
{
	public class ContentPlaceHolder : System.Web.UI.WebControls.ContentPlaceHolder
	{
        static PropertyInfo templatesP = typeof(MasterPage).GetProperty("ContentTemplates",
                System.Reflection.BindingFlags.GetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic);

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (this.Context.Items.Contains("_ORIGINGAL_VIEWNAME"))
            {
                IDictionary templates = templatesP.GetValue(this.Page.Master, null) as IDictionary;
                if (this.Page is ViewPage && this.Controls.Count == 0 && ((templates == null) || templates[this.ID] == null))
                {
                    ((ViewPage)Page).Html.RenderPartial("_" + Context.Items["_ORIGINGAL_VIEWNAME"]);
                }
                base.Render(writer);
            }
        }
	}
}
