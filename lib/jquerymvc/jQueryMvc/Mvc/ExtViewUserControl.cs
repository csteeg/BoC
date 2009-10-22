using System.Web.Mvc.Html;
using System.Web;

namespace JqueryMvc.Mvc
{
    public class ExtViewUserControl: System.Web.Mvc.ViewUserControl
    {
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (Request.IsJqAjaxRequest())
                this.Html.ValidationSummary();
            base.Render(writer);
        }
    }

    public class ExtViewUserControl<TModel> : System.Web.Mvc.ViewUserControl<TModel> where TModel: class
    {
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (Request.IsJqAjaxRequest())
                this.Html.ValidationSummary();
            base.Render(writer);
        }
    }
}
