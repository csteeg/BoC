using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using JqueryMvc.Mvc;

namespace JqMvcSampleWeb.Controllers
{
	public class HomeController : ExtController
	{
		public ActionResult Index()
		{
			ViewData["Title"] = "Home Page";
			ViewData["Message"] = "Welcome to ASP.NET MVC!";

			return View();
		}

		public ActionResult About()
		{
			ViewData["Title"] = "About Page";

			return View();
		}

        public ActionResult NonPartial()
        {
            ViewData["Title"] = "About Page";

            return View();
        }
        
        public ActionResult SayHello(string name)
		{
			if (name == "Britney")
				throw new Exception("Go away!");
			else if (name == "Luke")
				ViewData.AddMessage("May the force be with you!");

			if (!string.IsNullOrEmpty(name))
				ViewData["HelloWorld"] = "Hi " + name + "!";

			return View();
		}

	}
}
