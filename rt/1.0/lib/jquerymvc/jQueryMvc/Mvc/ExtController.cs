using JqueryMvc.Attributes;
using System.Web.Mvc;
namespace JqueryMvc.Mvc
{
    [AjaxController]
	[HandleError]
	[DefaultView("default")]
	public class ExtController : Controller
    {
    }
}
