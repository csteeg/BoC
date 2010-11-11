using System.Web.Mvc;
using BoC.Web.Mvc.Attributes;

namespace BoC.Web.Mvc.Controllers
{
    [EventTrigger]
    [AjaxController]
    [HandleError]
    [DefaultView("default")]
    public class CommonBaseController : Controller
    {
        public CommonBaseController() {}
    }
}
