using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Permissions;
using System.Web;
using System.Web.DynamicData;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class ExpressionUtility {
        public static IQueryable<T> ApplyOrderByClause<T>(IQueryable<T> query, string column) {
            PropertyInfo columnPropInfo = typeof(T).GetProperty(column);

            var entityParam = Expression.Parameter(typeof(T), "e");                    // {e}
            var columnExpr = Expression.MakeMemberAccess(entityParam, columnPropInfo); // {e.column}
            var lambda = Expression.Lambda(columnExpr, entityParam);                   // {e => e.column}

            var call = Expression.Call(typeof(Queryable), "OrderBy", new Type[] { typeof(T), columnPropInfo.PropertyType }, query.Expression, lambda);

            query = query.Provider.CreateQuery<T>(call);

            return query;
        }

        public static IQueryable<T> ApplyOrderByDescendingClause<T>(IQueryable<T> query, string column) {
            PropertyInfo columnPropInfo = typeof(T).GetProperty(column);

            var entityParam = Expression.Parameter(typeof(T), "e");                    // {e}
            var columnExpr = Expression.MakeMemberAccess(entityParam, columnPropInfo); // {e.column}
            var lambda = Expression.Lambda(columnExpr, entityParam);                   // {e => e.column}

            var call = Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { typeof(T), columnPropInfo.PropertyType }, query.Expression, lambda);

            query = query.Provider.CreateQuery<T>(call);

            return query;
        }

        public static IQueryable ApplyWhereClause(IQueryable query, MetaTable table, MetaColumn column, object value)
        {
            return ApplyWhereClause(query, table, column, value, null);
        }

        public static IQueryable ApplyWhereClause(IQueryable query, MetaTable table, MetaColumn column, object value, string subProperties) {
            
            object typedValue = ChangeType(value, column.ColumnType);

            var entityParam = Expression.Parameter(table.EntityType, "e");                           // {e}
            var columnExpr = Expression.MakeMemberAccess(entityParam, column.EntityTypeProperty);    // {e.column}
            ConstantExpression valueExpr = Expression.Constant(typedValue, column.EntityTypeProperty.PropertyType); // {value}
            var equalExpr = Expression.Equal(columnExpr, valueExpr);                                 // {e.column == value}
            var lambda = Expression.Lambda(equalExpr, entityParam);                                  // {e => e.column == value}

            var call = Expression.Call(typeof(Queryable), "Where", new Type[] { table.EntityType }, query.Expression, lambda);

            return query.Provider.CreateQuery(call);
        }

        public static IQueryable<T> ApplyWhereClause<T>(IQueryable<T> query, MetaTable table, MetaColumn column, object value, string subProperties)
        {
            return (IQueryable<T>)ApplyWhereClause((IQueryable)query, table, column, value, subProperties);
        }

        public static IQueryable<T> ApplyWhereClause<T>(IQueryable<T> query, MetaTable table, MetaColumn column, object value)
        {
            return (IQueryable<T>)ApplyWhereClause((IQueryable)query, table, column, value, null);
        }

        static object ChangeType(object value, Type conversionType)
        {
            if (value.GetType().IsAssignableFrom(conversionType))
                return value;

            if (conversionType == typeof(Guid))
                return new Guid(value.ToString());

            return Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
        }

        public static bool TryGetEntityAndFieldNameFromExpression(Expression expression, out object entity, out string fieldName) {
            entity = null;
            fieldName = null;

            try {
                MemberExpression memberExpression = expression as MemberExpression;

                if (memberExpression != null) {
                    var entityLambda = Expression.Lambda<Func<object>>(memberExpression.Expression);
                    entity = entityLambda.Compile()();
                    fieldName = memberExpression.Member.Name;
                    return true;
                }

                UnaryExpression unaryExpression = expression as UnaryExpression;

                if (unaryExpression != null)
                    if (unaryExpression.NodeType == ExpressionType.Convert || unaryExpression.NodeType == ExpressionType.MemberAccess)
                        return TryGetEntityAndFieldNameFromExpression(unaryExpression.Operand, out entity, out fieldName);
            }
            catch (Exception) { }

            return false;
        }
    }
}