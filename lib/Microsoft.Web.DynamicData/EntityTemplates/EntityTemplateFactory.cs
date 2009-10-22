using System;
using System.Globalization;
using System.Security.Permissions;
using System.Web;
using System.Web.Compilation;
using System.Web.DynamicData;
using System.Web.Hosting;
using System.Web.UI.WebControls;

namespace Microsoft.Web.DynamicData {
    // REVIEW: No caching or folder watching here, as we hope the lookup will be replaced with a view engine lookup
    // REVIEW: A lot of code was borrowed from FieldTemplateFactory, an opportunity for breaking out helper classes?

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class EntityTemplateFactory : IEntityTemplateFactory {
        bool _needToResolveVirtualPath;
        string _templateFolderVirtualPath;
        VirtualPathProvider _vpp;

        public MetaModel Model {
            get;
            private set;
        }

        public string TemplateFolderVirtualPath {
            get {
                if (_templateFolderVirtualPath == null) {
                    TemplateFolderVirtualPath = "EntityTemplates";
                }

                if (_needToResolveVirtualPath) {
                    _templateFolderVirtualPath = VirtualPathUtility.AppendTrailingSlash(_templateFolderVirtualPath);

                    if (this.Model != null) {
                        _templateFolderVirtualPath = VirtualPathUtility.Combine(Model.DynamicDataFolderVirtualPath, _templateFolderVirtualPath);
                    }

                    _needToResolveVirtualPath = false;
                }

                return _templateFolderVirtualPath;
            }
            set {
                this._templateFolderVirtualPath = value;
                this._needToResolveVirtualPath = true;
            }
        }

        VirtualPathProvider VirtualPathProvider {
            get {
                if (_vpp == null) {
                    _vpp = HostingEnvironment.VirtualPathProvider;
                }

                return _vpp;
            }
        }

        public virtual string BuildVirtualPath(string templateName, DataBoundControlMode mode) {
            if (String.IsNullOrEmpty(templateName)) {
                throw new ArgumentNullException("uiHint");
            }

            string str = null;

            switch (mode) {
                case DataBoundControlMode.ReadOnly:
                    str = string.Empty;
                    break;

                case DataBoundControlMode.Edit:
                    str = "_Edit";
                    break;

                case DataBoundControlMode.Insert:
                    str = "_Insert";
                    break;
            }

            return String.Format(CultureInfo.InvariantCulture, TemplateFolderVirtualPath + "{0}{1}.ascx", new object[] { templateName, str });
        }

        public virtual IEntityTemplate CreateEntityTemplate(MetaTable table, ref DataBoundControlMode mode, string uiHint) {
            mode = PreprocessMode(table, mode);

            string virtualPath = this.GetEntityTemplateVirtualPath(table, mode, uiHint);
            if (virtualPath == null) {
                return null;
            }

            return (IEntityTemplate)BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof(IEntityTemplate));
        }

        public virtual string GetEntityTemplateVirtualPath(MetaTable table, DataBoundControlMode mode, string uiHint) {
            for (DataBoundControlMode attemptedMode = mode; attemptedMode >= DataBoundControlMode.ReadOnly; attemptedMode -= 1) {
                string str = this.GetVirtualPathForMode(uiHint, table, attemptedMode);
                if (str != null) {
                    return str;
                }
            }

            return null;
        }

        string GetVirtualPathForMode(string uiHint, MetaTable table, DataBoundControlMode mode) {
            if (!String.IsNullOrEmpty(uiHint)) {
                string str = GetVirtualPathIfExists(uiHint, table, mode);
                if (str != null) {
                    return str;
                }
            }

            // REVIEW: MetaTable doesn't have a DataTypeAttribute; should it?

            //if (column.DataTypeAttribute != null) {
            //    string dataTypeName = column.DataTypeAttribute.GetDataTypeName();
            //    result = GetVirtualPathIfExists(dataTypeName, column, mode);
            //    if (result != null) {
            //        return result;
            //    }
            //}

            return GetVirtualPathIfExists(table.EntityType.FullName, table, mode) ??
                   GetVirtualPathIfExists(table.EntityType.Name, table, mode) ??
                   GetVirtualPathIfExists("Default", table, mode);
        }

        string GetVirtualPathIfExists(string templateName, MetaTable table, DataBoundControlMode mode) {
            string virtualPath = BuildVirtualPath(templateName, mode);
            if (VirtualPathProvider.FileExists(virtualPath)) {
                return virtualPath;
            }

            return null;
        }

        public void Initialize(MetaModel model) {
            Model = model;
        }

        public virtual DataBoundControlMode PreprocessMode(MetaTable table, DataBoundControlMode mode) {
            return table.IsReadOnly ? DataBoundControlMode.ReadOnly : mode;
        }
    }
}