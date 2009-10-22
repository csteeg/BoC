using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;
using BoC.Persistence;
using BoC.Services;
using BoC.Validation;
using BoC.Web.Mvc.Validation;

namespace BoC.Web.Mvc.Controllers
{
    public class DynamicScaffoldController<TEntity> : CommonBaseController where TEntity : IBaseEntity, new()
    {
        private readonly IModelService<TEntity> service;

        public DynamicScaffoldController(IModelService<TEntity> service)
        {
            this.service = service;
        }

        static Dictionary<String, bool> propCache = new Dictionary<string, bool>();
        private IQueryable<TEntity> addToQuery(IQueryable<TEntity> query, string propName, object value)
        {
            if (value == null || value == "")
                return query;

            string prop = propName;
            if (prop.Contains("."))
            {
                prop = prop.Substring(0, prop.IndexOf("."));
            }

            if (!propCache.ContainsKey(prop.ToLower()))
            {
                var propertyInfo = typeof(TEntity).GetProperty(prop);
                if (propertyInfo != null)
                {
                    var attrib = propertyInfo.GetCustomAttributes(typeof(ScaffoldColumnAttribute), true);
                    var scaffold = true;
                    if (attrib != null && attrib.Length > 0)
                    {
                        scaffold = ((ScaffoldColumnAttribute)attrib[0]).Scaffold;
                    }
                    propCache.Add(prop.ToLower(), scaffold);
                }
                else
                {
                    propCache.Add(prop.ToLower(), false);
                }
            }

            if (propCache[prop.ToLower()])
            {
                query = query.Where(propName + " == @0", value);
            }

            return query;
        }
        protected virtual IQueryable<TEntity> EntitiesQuery
        {
            get
            {
                IQueryable<TEntity> query = service.Query();

                foreach (string name in Request.QueryString)
                {
                    addToQuery(query, name, Request.QueryString[name]);
                }

                foreach (string name in RouteData.Values.Keys)
                {
                    addToQuery(query, name, RouteData.Values[name]);
                }

                return query;
            }
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Delete)]
        public virtual ActionResult Delete(TEntity entity, string returnTo)
        {
            if (entity == null)
            {
                return this.HttpStatus(HttpStatusCode.NotFound);
            }

            return OnDelete(entity, returnTo);
        }

        private ActionResult HttpStatus(HttpStatusCode status)
        {
            return new HttpStatusCodeResult((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), status.ToString()));
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public virtual ActionResult Edit(TEntity entity)
        {
            if (entity == null)
            {
                return this.HttpStatus(HttpStatusCode.NotFound);
            }

            return OnEdit(entity);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Update(TEntity entity)
        {
            if (entity == null)
            {
                return this.HttpStatus(HttpStatusCode.NotFound);
            }
            return OnUpdate(entity);
        }

        public virtual ActionResult List(string sort, int? show, int? page)
        {
            IQueryable<TEntity> query = EntitiesQuery;

            if (sort != null)
            {
                if (sort.StartsWith("-"))
                {
                    query = query.OrderBy(sort.Substring(1) + " desc");
                }
                else
                {
                    query = query.OrderBy(sort);
                }
            }

            return OnList(query, sort);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public virtual ActionResult New()
        {
            return OnNew(new TEntity());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult New(TEntity entity)
        {
            return OnInsert(entity);
        }

        public virtual ActionResult Show(TEntity entity)
        {
            if (entity == null)
            {
                return this.HttpStatus(HttpStatusCode.NotFound);
            }

            return OnShow(entity);
        }

        // Overrides for performing the specific actions for the controller

        protected virtual ActionResult OnDelete(TEntity entity, string returnTo)
        {
            service.Delete(entity);
            if (String.IsNullOrEmpty(returnTo))
            {
                return RedirectToAction("list");
            }
            return Redirect(returnTo);
        }

        protected virtual ActionResult OnEdit(TEntity entity)
        {
            return View("PageTemplates/Edit", entity);
        }

        protected virtual ActionResult OnInsert(TEntity entity)
        {
            if (!ViewData.ModelState.IsValid) //input validation errors (eg cannot convert to int)
            {
                return OnNew(entity);
            }

            try
            {
                service.Insert(entity);
            }
            catch (RulesException rules)
            {
                rules.AddModelStateErrors(ViewData.ModelState);
            }
            catch (Exception exc)
            {
                ViewData.ModelState.AddModelError("_FORM", exc);
            }

            if (!ViewData.ModelState.IsValid) //entity validation errors
            {
                return OnNew(entity);
            }

            return this.RedirectToAction("List");
        }

        protected virtual ActionResult OnList(IEnumerable<TEntity> entities, string sort)
        {
            ViewData["sort"] = sort;

            return View("PageTemplates/List", entities);
        }

        protected virtual ViewResult OnNew(TEntity entity)
        {
            return View("PageTemplates/New", entity);
        }

        protected virtual ActionResult OnShow(TEntity entity)
        {
            return View("PageTemplates/Show", entity);
        }

        protected virtual ActionResult OnUpdate(TEntity entity)
        {
            if (!ViewData.ModelState.IsValid) //input validation errors (eg cannot convert to int)
            {
                return OnEdit(entity);
            }

            service.Update(entity);

            if (!ViewData.ModelState.IsValid)
            {
                return OnEdit(entity);
            }

            return this.RedirectToAction("List");
        }
    }
}