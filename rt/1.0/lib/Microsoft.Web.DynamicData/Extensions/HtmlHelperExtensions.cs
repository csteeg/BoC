using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class HtmlHelperExtensions {
        public static string RenderViewUserControl(this HtmlHelper html, ViewUserControl control, object model) {
            ViewPage viewPage = new ViewPage();
            viewPage.ViewContext = html.ViewContext;
            viewPage.Controls.Add(control);
            viewPage.Url = new UrlHelper(html.ViewContext.RequestContext);
            viewPage.Html = new HtmlHelper(html.ViewContext, viewPage);

            control.ViewData = new ViewDataDictionary(html.ViewContext.ViewData);
            control.ViewData.Model = model;
            control.InitializeAsUserControl(viewPage);

            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb, CultureInfo.CurrentCulture);
            HttpContext.Current.Server.Execute(viewPage, writer, true);
            return sb.ToString();
        }
    }
}