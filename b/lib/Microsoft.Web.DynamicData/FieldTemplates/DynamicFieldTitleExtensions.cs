using System;
using System.Linq.Expressions;
using System.Security.Permissions;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class DynamicFieldTitleExtensions {
        // Dynamic field titles based on lambda expressions (experimental!)

        public static string DynamicFieldTitle(this HtmlHelper html, Expression<Func<object>> expression) {
            object entity;
            string fieldName;

            if (!ExpressionUtility.TryGetEntityAndFieldNameFromExpression(expression.Body, out entity, out fieldName))
                throw new ArgumentException("DynamicField expression of unknown type: " + expression.Body.GetType().FullName + "\r\n" + expression.Body.ToString());

            return DynamicFieldTitle(html, entity, fieldName);
        }

        // Dynamic field titles based on entity + field name

        public static string DynamicFieldTitle(this HtmlHelper html, object entity, string fieldName) {
            // REVIEW: How can we pass along the model to avoid MetaModel.Default?
            var table = MetaModel.Default.GetTable(entity.GetType());
            var column = table.GetColumn(fieldName);

            return DynamicFieldTitle(html, entity, column);
        }

        // Dynamic field titles based on entity + column

        public static string DynamicFieldTitle(this HtmlHelper html, object entity, MetaColumn column) {
            return column.DisplayName;
        }
    }
}
