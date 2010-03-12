using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI.WebControls;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class MvcEntityTemplate : ViewUserControl, IEntityTemplate {
        DynamicDataHelper _dynamicDataHelper;

        public override void Dispose() {
            if (_dynamicDataHelper != null) {
                _dynamicDataHelper.Dispose();
                _dynamicDataHelper = null;
            }

            base.Dispose();
        }

        protected IEnumerable<MetaColumn> Columns {
            get { return DynamicData.Columns; }
        }

        protected IEnumerable<MetaColumn> DisplayColumns {
            get { return DynamicData.DisplayColumns; }
        }

        protected DynamicDataHelper DynamicData {
            get {
                if (_dynamicDataHelper == null)
                    _dynamicDataHelper = CreateDynamicData();

                return _dynamicDataHelper;
            }
        }

        protected IEnumerable<MetaColumn> EditColumns {
            get { return DynamicData.DisplayColumns; }
        }

        protected object Entity {
            get { return ViewData.Model; }
        }

        protected IEntityTemplateHost Host {
            get;
            private set;
        }

        protected IEnumerable<MetaColumn> InsertColumns {
            get { return DynamicData.DisplayColumns.Where(x => !(x is MetaChildrenColumn)); }
        }

        protected DataBoundControlMode Mode {
            get { return Host.Mode; }
        }

        protected MetaTable Table {
            get { return Host.Table; }
        }

        protected virtual DynamicDataHelper CreateDynamicData() {
            if (Entity == null)
                throw new InvalidOperationException("Cannot create DynamicDataHelper because entity is null (entity type is unknown)");

            return new DynamicDataHelper(this, Entity.GetType());
        }

        public void SetHost(IEntityTemplateHost host) {
            Host = host;
        }

        // Helpers to forward to the more generalized dynamic data extension methods

        protected string DynamicField(MetaColumn column) {
            return Html.DynamicField(Entity, column, null /* uiHint */, Mode);
        }

        protected string DynamicField(MetaColumn column, string uiHint) {
            return Html.DynamicField(Entity, column, uiHint, Mode);
        }

        protected string DynamicFieldErrors(MetaColumn column) {
            return Html.ValidationMessage(column.Name);
        }

        protected string DynamicFieldTitle(MetaColumn column) {
            return Html.DynamicFieldTitle(Entity, column);
        }
    }
}