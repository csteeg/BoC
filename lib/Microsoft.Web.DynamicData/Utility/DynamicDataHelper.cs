using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Web;
using System.Web.DomainServices;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Ria.Data;
using System.Web.Routing;
using System.Web.UI;
using ChangeSet=System.Web.DomainServices.ChangeSet;

namespace Microsoft.Web.DynamicData.Mvc {
    // REVIEW: This stuff is all hard-coded to DLINQ right now.
    //
    // I also don't like having access to ViewData here; this class should be ignorant of
    // how errors are reported, so things which can fail should optionally return model
    // errors and property errors. Ideally, we could factor something like this into the
    // base framework for Dynamic Data, once we figure out what the CUD interface is.

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DynamicDataHelper : IDisposable {
        object _context;
        MetaTable _table;
        ViewDataDictionary _viewData;
        IViewDataContainer _viewDataContainer;
        private IList<EntityOperation> _changeSet;

        public DynamicDataHelper(IViewDataContainer viewDataContainer, Type entityType) {
            _viewDataContainer = viewDataContainer;
            EntityType = entityType;
        }

        public DynamicDataHelper(IViewDataContainer viewDataContainer, string tableName) {
            _viewDataContainer = viewDataContainer;
            _table = Model.GetTable(tableName);
            EntityType = _table.EntityType;
        }

        public DynamicDataHelper(ViewDataDictionary viewData, Type entityType) {
            _viewData = viewData;
            EntityType = entityType;
        }

        public DynamicDataHelper(ViewDataDictionary viewData, string tableName) {
            _viewData = viewData;
            _table = Model.GetTable(tableName);
            EntityType = _table.EntityType;
        }

        public virtual void Dispose() {
            if (_context != null && _context is IDisposable) {
                ((IDisposable)_context).Dispose();
                _context = null;
            }
        }

        public IEnumerable<MetaColumn> Columns {
            get { return Table.Columns; }
        }

        protected object Context
        {
            get {
                if (_context == null) {
                    _context = Table.CreateContext();
                    if (_context is DomainService)
                    {
                        _changeSet = new List<EntityOperation>();
                    }
                }

                return _context;
            }
        }

        public IEnumerable<MetaColumn> DisplayColumns {
            get { return Table.Columns.Where(c => c.Scaffold); }
        }

        public IEnumerable<MetaColumn> DisplayShortColumns {
            get { return Table.Columns.Where(c => c.Scaffold && !c.IsLongString); }
        }

        protected Type EntityType {
            get;
            private set;
        }

        public bool HasErrors {
            get;
            protected set;
        }

        public MetaModel Model {
            get { return ViewData.MetaModel(); }
            set { ViewData.SetMetaModel(value); }
        }

        public MetaTable Table {
            get {
                if (_table == null)
                    _table = Model.GetTable(EntityType);

                return _table;
            }
        }

        protected ViewDataDictionary ViewData {
            get {
                if (_viewData == null)
                    _viewData = _viewDataContainer.ViewData;

                return _viewData;
            }
        }

        public bool Delete(object entity) {
            if (Context is DomainService)
            {
                _changeSet.Add(new EntityOperation
                                   {
                                       Operation = DomainOperation.Delete,
                                       Entity = entity,
                                       OperationName = DomainOperation.Delete.ToString()
                                   });
            }
            return SubmitChanges();
        }

        public RouteValueDictionary GetRouteData(object entity) {
            RouteValueDictionary result = new RouteValueDictionary();
            GetRouteData(entity, result);
            return result;
        }

        public void GetRouteData(object entity, RouteValueDictionary routeValues) {
            foreach (var column in Table.PrimaryKeyColumns) {
                routeValues.Add(column.Name, DataBinder.GetPropertyValue(entity, column.Name));
            }
        }

        protected virtual IQueryable GetQuery() {
            return Table.GetQuery(Context);
        }

        public bool Insert(object entity) {
            if (Context is DomainService)
            {
                _changeSet.Add(new EntityOperation
                                   {
                                       Operation = DomainOperation.Insert,
                                       Entity = entity,
                                       OperationName = DomainOperation.Insert.ToString()
                                   });
            }
            return SubmitChanges();
        }

        bool SubmitChanges() {
            try {
                var domainservice = Context as DomainService;
                if (domainservice != null)
                {
                    var chngSet = new ChangeSet(_changeSet);
                    domainservice.Submit(chngSet);
                    if (HandleErrors(chngSet))
                    {
                        _changeSet.Clear();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    var datacontext = Context as DataContext;
                    if (datacontext != null)
                    {
                        datacontext.SubmitChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e) {
                //ViewData.ModelState.AddModelError("_FORM", e); //<-- grr, this should do that:
                var modelstate = ViewData.ModelState["_FORM"];
                if (modelstate == null)
                {
                    modelstate = new ModelState();
                    ViewData.ModelState["_FORM"] = modelstate;
                }
                modelstate.Errors.Add(new ModelError(e, e.Message));
                return false;
            }
        }

        private bool HandleErrors(ChangeSet set)
        {
            var errorOps = set.EntityOperations.Where(op => op.HasError);
            
            if (errorOps.Count() == 0)
                return true;

            foreach (var operation in errorOps)
            {
                var errors = operation.Errors ?? operation.ValidationErrors;
                if (operation.Errors != null && operation.ValidationErrors != null)
                    errors = errors.Union(operation.ValidationErrors);
                if (errors != null)
                {
                    foreach (var error in errors)
                    {
                        foreach (var name in error.SourceMemberNames)
                        {
                            ViewData.ModelState.AddModelError(name, error.Message);
                        }
                    }
                }
                if (operation.HasConflict)
                {
                    ViewData.ModelState.AddModelError("_FORM", 
                        string.Format("Conflict encountered. Members in conflict are '{0}'.", string.Join(",", operation.ConflictMembers.ToArray<string>())));
                }

            }

            return false;
        }

        // REVIEW: Will we assume that everybody will support object tracking? If so, we
        // can get rid of the entity passed here.
        public bool Update(object entity) {

            if (Context is DomainService)
            {
                _changeSet.Add(new EntityOperation
                                   {
                                       Operation = DomainOperation.Update,
                                       Entity = entity,
                                       OperationName = DomainOperation.Update.ToString()
                                   });
            }
            return SubmitChanges();
        }
    }

    public class DynamicDataHelper<TEntity> : DynamicDataHelper where TEntity : class, new() {
        public DynamicDataHelper(IViewDataContainer viewDataContainer) : base(viewDataContainer, typeof(TEntity)) { }

        public DynamicDataHelper(ViewDataDictionary viewData) : base(viewData, typeof(TEntity)) { }

        public IQueryable<TEntity> Query {
            get { return (IQueryable<TEntity>)GetQuery(); }
        }

        public bool Delete(TEntity entity) {
            return base.Delete(entity);
        }

        public bool Insert(TEntity entity) {
            return base.Insert(entity);
        }

        public bool Update(TEntity entity) {
            return base.Update(entity);
        }
    }
}