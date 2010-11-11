using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace BoC.Web.Mvc
{
    public class CommonViewUserControl<TModel> : ViewUserControl<TModel> where TModel: class
    {
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (Request.IsJqAjaxRequest())
                this.Html.ValidationSummary();
            base.Render(writer);
        }

    }

    public class CommonViewUserControl : ViewUserControl
    {
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (Request.IsJqAjaxRequest())
                this.Html.ValidationSummary();
            base.Render(writer);
        }

    }
}