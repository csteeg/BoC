using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using BoC.Persistence;
using MvcTodo.Models.Entity;

namespace MvcTodo.Controllers
{
    public class CatalogController : Controller
    {
        private IRepository<Catalog> _repository;
        //
        // GET: /Catalog/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Catalog/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Catalog/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Catalog/Create

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
        // GET: /Catalog/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Catalog/Edit/5

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
