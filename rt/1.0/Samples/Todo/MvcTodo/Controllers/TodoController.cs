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
    public class TodoController : Controller
    {
        private IRepository<Todo> _repositoryTodo;
        private IRepository<Catalog> _repositoryCatalog;

        public TodoController(IRepository<Todo> repositoryTodo, IRepository<Catalog> repositoryCatalog)
        {
            _repositoryTodo = repositoryTodo;
            _repositoryCatalog = repositoryCatalog;
        }

        //
        // GET: /Todo/

        public ActionResult Index()
        {
            return View(_repositoryTodo.ToList());
        }

        //
        // GET: /Todo/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Todo/Create

        public ActionResult Create()
        {
            ViewData["catalogs"] =
               new SelectList(_repositoryCatalog.ToList(), "Id", "Name");

            return View();
        }

        //
        // POST: /Todo/Create

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(Todo todo)
        {
            try
            {
                // TODO: Add insert logic here

                _repositoryTodo.SaveOrUpdate(todo);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Todo/Edit/5

        public ActionResult Edit(int id)
        {
            Todo todo = _repositoryTodo.Get(id);

            ViewData["catalogs"] =
                new SelectList(_repositoryCatalog.ToList(), "Id", "Name", todo.Catalog.Id);

            return View(todo);
        }

        //
        // POST: /Todo/Edit/5

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(Todo entity)
        {
            try
            {
                _repositoryTodo.SaveOrUpdate(entity);

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
