<%@ Control Language="C#" Inherits="Microsoft.Web.DynamicData.Mvc.MvcFieldTemplate" %>
<script runat="server">
    protected MetaChildrenColumn ChildColumn {
        get { return (MetaChildrenColumn)Column; }
    }
    protected RouteValueDictionary ChildRouteValues {
        get {
            var result = new RouteValueDictionary();
            IList<object> primaryKeyValues = base.Table.GetPrimaryKeyValues(Entity);
            MetaForeignKeyColumn columnInOtherTable = ChildColumn.ColumnInOtherTable as MetaForeignKeyColumn;
            
            if (columnInOtherTable != null)
                for (int i = 0; i < columnInOtherTable.ForeignKeyNames.Count; i++)
                    result.Add(columnInOtherTable.ForeignKeyNames[i], primaryKeyValues[i]);

            return result;
        }
    }
</script>
<%= Html.ScaffoldLink("View " + Column.Name, ChildColumn.ChildTable, "list", ChildRouteValues) %>