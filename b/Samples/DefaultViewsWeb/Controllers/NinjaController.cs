using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using DefaultViewsWeb.Models;

namespace DefaultViewsWeb.Controllers
{
    public class NinjaController : Controller
    {
        
        //
        // GET: /Ninja/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Ninja/Details/5
        static Ninja _ninja = new Ninja { 
            Name = "Ask a Ninja", 
            Clan = "Yokoyama", 
            BlowgunDartCount = 23, 
            ShurikenCount = 42 };

        public ActionResult Details(int id)
        {
            ViewData["Title"] = "A Very Cool Ninja";
            return View(_ninja);
        }

        //
        // GET: /Ninja/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Ninja/Create

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Ninja/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View(_ninja);
        }

        //
        // POST: /Ninja/Edit/5

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
