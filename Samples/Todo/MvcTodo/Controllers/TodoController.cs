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
        private IRepository<Todo> _repository;

        public TodoController(IRepository<Todo> repository)
        {
            _repository = repository;
        }

        //
        // GET: /Todo/

        public ActionResult Index()
        {
            return View(_repository.ToList());
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
            return View(new Todo());
        } 

        //
        // POST: /Todo/Create

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(Todo todo)
        {
            try
            {
                // TODO: Add insert logic here

                _repository.SaveOrUpdate(todo);

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
            return View(_repository.Get(id));
        }

        //
        // POST: /Todo/Edit/5

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(Todo entity)
        {
            try
            {

                _repository.SaveOrUpdate(entity);

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
