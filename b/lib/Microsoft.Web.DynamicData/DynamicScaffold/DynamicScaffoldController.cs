using System;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DynamicScaffoldController<TEntity> : Controller where TEntity : class, new() {
        protected virtual IQueryable<TEntity> EntitiesQuery {
            get {
                var query = DynamicData.Query;

                foreach (string name in Request.QueryString)
                {
                    var prop = name;
                    if (prop.Contains("."))
                        prop = prop.Substring(0, prop.IndexOf("."));

                    //test if there is a meta column available for this property
                    MetaColumn column;
                    if (!DynamicData.Table.TryGetColumn(prop, out column))
                        continue;
                    query = query.Where(name + " == @0", Request.QueryString[name]);
                    //query = ExpressionUtility.ApplyWhereClause(query, DynamicData.Table, column, Request.QueryString[name]);
                }

                foreach (string name in RouteData.Values.Keys)
                {
                    var prop = name;
                    if (prop.Contains("."))
                        prop = prop.Substring(0, prop.IndexOf("."));

                    //test if there is a meta column available for this property
                    MetaColumn column;
                    if (!DynamicData.Table.TryGetColumn(prop, out column))
                        continue;
                    query = query.Where(name + " == @0", Request.QueryString[name]);
                }

                return query;
            }
        }

        /// <inheritdoc/>
        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            DynamicData = new DynamicDataHelper<TEntity>(ViewData);
            ViewData.SetDynamicData(DynamicData);

            base.OnActionExecuting(filterContext);
        }

        /// <inheritdoc/>
        protected override void OnResultExecuted(ResultExecutedContext filterContext) {
            base.OnResultExecuted(filterContext);

            if (DynamicData != null) {
                DynamicData.Dispose();
                DynamicData = null;
            }
            /*
            if (!ViewData.ModelState.IsValid)
            {
                //workaround asp.net modelstate bug
                foreach (var key in ViewData.ModelState.Keys)
                {
                    if (ViewData.ModelState[key].Errors != null)
                    {
                        for (int i = ViewData.ModelState[key].Errors.Count - 1; i > 0;i-- ; )
                        {
                            var error = ViewData.ModelState[key].Errors[i];
                            if (String.IsNullOrEmpty(error.ErrorMessage) && error.Exception != null)
                            {
                                error.ErrorMessage = error.Exception.Message;
                            }
                        }
                    }
                }
            }*/
        }

        protected DynamicDataHelper<TEntity> DynamicData {
            get;
            private set;
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Delete)]
        public virtual ActionResult Delete(TEntity entity, string returnTo)
        {
            if (entity == null)
                return this.HttpStatusCode(HttpStatusCode.NotFound);

            return OnDelete(entity, returnTo);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public virtual ActionResult Edit(TEntity entity) {
            if (entity == null)
                return this.HttpStatusCode(HttpStatusCode.NotFound);

            return OnEdit(entity);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Update(TEntity entity) {
            if (entity == null)
                return this.HttpStatusCode(HttpStatusCode.NotFound);
            return OnUpdate(entity);
        }

        public virtual ActionResult List(string sort, int? show, int? page) {
            var query = EntitiesQuery;

            if (sort != null) {
                if (sort.StartsWith("-"))
                    query = ExpressionUtility.ApplyOrderByDescendingClause(query, sort.Substring(1));
                else
                    query = ExpressionUtility.ApplyOrderByClause(query, sort);
            }

            //return OnList(new PagedList<TEntity>(query, page ?? 1, show ?? 10), sort);
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
                return this.HttpStatusCode(HttpStatusCode.NotFound);

            return OnShow(entity);
        }

        // Overrides for performing the specific actions for the controller

        protected virtual ActionResult OnDelete(TEntity entity, string returnTo) {
            DynamicData.Delete(entity);
            if (String.IsNullOrEmpty(returnTo))
                return this.RedirectToScaffold(DynamicData.Table, "list");
            return Redirect(returnTo);
        }

        protected virtual ActionResult OnEdit(TEntity entity) {
            return View("PageTemplates/Edit", entity);
        }

        protected virtual ActionResult OnInsert(TEntity entity) {
            if (!ViewData.ModelState.IsValid) //input validation errors (eg cannot convert to int)
                return OnNew(entity);

            DynamicData.Insert(entity);
            
            if (!ViewData.ModelState.IsValid) //entity validation errors
                return OnNew(entity);

            return this.RedirectToScaffold(DynamicData.Table, "list");
        }

        protected virtual ActionResult OnList(IQueryable<TEntity> entities, string sort) {
            ViewData["sort"] = sort;

            return View("PageTemplates/List", entities);
        }

        protected virtual ViewResult OnNew(TEntity entity) {
            return View("PageTemplates/New", entity);
        }

        protected virtual ActionResult OnShow(TEntity entity) {
            return View("PageTemplates/Show", entity);
        }

        protected virtual ActionResult OnUpdate(TEntity entity) {
            if (!ViewData.ModelState.IsValid) //input validation errors (eg cannot convert to int)
                return OnEdit(entity);

            DynamicData.Update(entity);

            if (!ViewData.ModelState.IsValid)
                return OnEdit(entity);

            return this.RedirectToScaffold(DynamicData.Table, "list");
        }
    }
}