using System;
using System.Security.Permissions;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class DynamicEntityExtensions {
        public static string DynamicEntity<T>(this HtmlHelper html) where T : class, new() {
            return DynamicEntity(html, new T(), null /* UIHint */, DataBoundControlMode.ReadOnly);
        }

        public static string DynamicEntity<T>(this HtmlHelper html, DataBoundControlMode mode) where T : class, new() {
            return DynamicEntity(html, new T(), null /* UIHint */, mode);
        }

        public static string DynamicEntity<T>(this HtmlHelper html, string uiHint) where T : class, new() {
            return DynamicEntity(html, new T(), uiHint, DataBoundControlMode.ReadOnly);
        }

        public static string DynamicEntity<T>(this HtmlHelper html, string uiHint, DataBoundControlMode mode) where T : class, new() {
            return DynamicEntity(html, new T(), uiHint, mode);
        }

        public static string DynamicEntity(this HtmlHelper html, Type entityType) {
            return DynamicEntity(html, Activator.CreateInstance(entityType), null /* UIHint */, DataBoundControlMode.ReadOnly);
        }

        public static string DynamicEntity(this HtmlHelper html, Type entityType, DataBoundControlMode mode) {
            return DynamicEntity(html, Activator.CreateInstance(entityType), null /* UIHint */, mode);
        }

        public static string DynamicEntity(this HtmlHelper html, Type entityType, string uiHint) {
            return DynamicEntity(html, Activator.CreateInstance(entityType), uiHint, DataBoundControlMode.ReadOnly);
        }

        public static string DynamicEntity(this HtmlHelper html, Type entityType, string uiHint, DataBoundControlMode mode) {
            return DynamicEntity(html, Activator.CreateInstance(entityType), uiHint, mode);
        }

        public static string DynamicEntity(this HtmlHelper html, object entity) {
            return DynamicEntity(html, entity, null /* UIHint */, DataBoundControlMode.ReadOnly);
        }

        public static string DynamicEntity(this HtmlHelper html, object entity, DataBoundControlMode mode) {
            return DynamicEntity(html, entity, null /* UIHint */, mode);
        }

        public static string DynamicEntity(this HtmlHelper html, object entity, string uiHint) {
            return DynamicEntity(html, entity, uiHint, DataBoundControlMode.ReadOnly);
        }

        // REVIEW: This eventually needs to use Html.RenderView with view engine-independent rendering
        public static string DynamicEntity(this HtmlHelper html, object entity, string uiHint, DataBoundControlMode mode) {
            // REVIEW: How can we pass along the model to avoid MetaModel.Default?
            var table = MetaModel.Default.GetTable(entity.GetType());

            // REVIEW: MetaTable does not have a UIHint. Should it?
            IEntityTemplate entityTemplate = MetaModel.Default.GetEntityTemplateFactory().CreateEntityTemplate(table, ref mode, uiHint /* ?? table.UIHint */);
            ViewUserControl entityTemplateControl = entityTemplate as ViewUserControl;

            if (entityTemplateControl == null)
                throw new InvalidOperationException("Cannot render a dynamic entity whose entity template is not a ViewUserControl");

            entityTemplate.SetHost(new SimpleEntityTemplateHost() { Table = table, Mode = mode });
            return html.RenderViewUserControl(entityTemplateControl, entity);
        }

        class SimpleEntityTemplateHost : IEntityTemplateHost {
            public DataBoundControlMode Mode {
                get;
                set;
            }

            public MetaTable Table {
                get;
                set;
            }
        }
    }
}