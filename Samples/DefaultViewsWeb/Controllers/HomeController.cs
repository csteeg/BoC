using System.Web.Mvc;

namespace DefaultViewsWeb.Controllers
{
    // The real action is in the NinjaController.

    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }
    }
}
