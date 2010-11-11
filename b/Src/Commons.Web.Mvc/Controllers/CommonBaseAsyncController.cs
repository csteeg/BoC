using System.Web.Mvc;
using BoC.Web.Mvc.Attributes;

namespace BoC.Web.Mvc.Controllers
{
    [EventTrigger]
    [AjaxController]
    [HandleError]
    [Attributes.DefaultView("default")]
    public class CommonBaseAsyncController : AsyncController
    {
    }
}
