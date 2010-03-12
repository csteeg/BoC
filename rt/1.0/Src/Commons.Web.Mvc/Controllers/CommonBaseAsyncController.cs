using System.Web.Mvc;
using BoC.Web.Mvc.Attributes;
using JqueryMvc.Attributes;
using JqueryMvc.Mvc;

namespace BoC.Web.Mvc.Controllers
{
    [EventTrigger]
    [AjaxController]
    [HandleError]
    [DefaultView("default")]
    public class CommonBaseAsyncController : AsyncController
    {
    }
}
