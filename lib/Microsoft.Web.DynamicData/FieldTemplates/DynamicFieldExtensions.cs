using System;
using System.Linq.Expressions;
using System.Security.Permissions;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class DynamicFieldExtensions {
        // Dynamic fields based on lambda expressions (experimental!)

        public static string DynamicField(this HtmlHelper html, Expression<Func<object>> expression) {
            return DynamicField(html, null /* uiHint */, DataBoundControlMode.ReadOnly, expression);
        }

        public static string DynamicField(this HtmlHelper html, string uiHint, Expression<Func<object>> expression) {
            return DynamicField(html, uiHint, DataBoundControlMode.ReadOnly, expression);
        }

        public static string DynamicField(this HtmlHelper html, DataBoundControlMode mode, Expression<Func<object>> expression) {
            return DynamicField(html, null /* uiHint */, mode, expression);
        }

        public static string DynamicField(this HtmlHelper html, string uiHint, DataBoundControlMode mode, Expression<Func<object>> expression) {
            object entity;
            string fieldName;

            if (!ExpressionUtility.TryGetEntityAndFieldNameFromExpression(expression.Body, out entity, out fieldName)) {
                throw new ArgumentException("DynamicField expression of unknown type: " + expression.Body.GetType().FullName + "\r\n" + expression.Body.ToString());
            }

            return DynamicField(html, entity, fieldName, uiHint, mode);
        }

        // Dynamic fields based on entity + field name

        public static string DynamicField(this HtmlHelper html, object entity, string fieldName) {
            return DynamicField(html, entity, fieldName, null /* uiHint */, DataBoundControlMode.ReadOnly);
        }

        public static string DynamicField(this HtmlHelper html, object entity, string fieldName, string uiHint) {
            return DynamicField(html, entity, fieldName, uiHint, DataBoundControlMode.ReadOnly);
        }

        public static string DynamicField(this HtmlHelper html, object entity, string fieldName, DataBoundControlMode mode) {
            return DynamicField(html, entity, fieldName, null /* uiHint */, mode);
        }

        public static string DynamicField(this HtmlHelper html, object entity, string fieldName, string uiHint, DataBoundControlMode mode) {
            // REVIEW: How can we pass along the model to avoid MetaModel.Default?
            var table = MetaModel.Default.GetTable(entity.GetType());
            var column = table.GetColumn(fieldName);

            return DynamicField(html, entity, column, uiHint, mode);
        }

        // Dynamic fields based on entity + column

        public static string DynamicField(this HtmlHelper html, object entity, MetaColumn column) {
            return DynamicField(html, entity, column.Name, null /* uiHint */, DataBoundControlMode.ReadOnly);
        }

        public static string DynamicField(this HtmlHelper html, object entity, MetaColumn column, string uiHint) {
            return DynamicField(html, entity, column.Name, uiHint, DataBoundControlMode.ReadOnly);
        }

        public static string DynamicField(this HtmlHelper html, object entity, MetaColumn column, DataBoundControlMode mode) {
            return DynamicField(html, entity, column.Name, null /* uiHint */, mode);
        }

        public static string DynamicField(this HtmlHelper html, object entity, MetaColumn column, string uiHint, DataBoundControlMode mode) {
            var host = new SimpleFieldTemplateHost() { Column = column, Mode = mode };

            IFieldTemplate fieldTemplate = MetaModel.Default.FieldTemplateFactory.CreateFieldTemplate(column, mode, uiHint ?? column.UIHint);
            ViewUserControl fieldTemplateControl = fieldTemplate as ViewUserControl;

            if (fieldTemplateControl == null)
                throw new InvalidOperationException("Cannot render a dynamic field whose field template is not a ViewUserControl");

            fieldTemplate.SetHost(host);
            return html.RenderViewUserControl(fieldTemplateControl, entity);
        }

        class SimpleFieldTemplateHost : IFieldTemplateHost {
            public MetaColumn Column { get; set; }

            public IFieldFormattingOptions FormattingOptions {
                get { return Column; }
            }

            public DataBoundControlMode Mode { get; set; }

            public string ValidationGroup {
                get { return String.Empty; }
            }
        }
    }
}
