<%@ Control Language="C#" Inherits="Microsoft.Web.DynamicData.Mvc.MvcFieldTemplate" %>
<script runat="server">
    private string SelectedAttribute(object entity) {
        if (entity == FieldValue)
            return @"selected = ""true""";
        return String.Empty;
    }
</script>
<% var parentTable = ForeignKeyColumn.ParentTable; %>
<%--<%= Html.Select(Column.Name, parentTable.Query,
        parentTable.PrimaryKeyColumns[0].Name,
        parentTable.DisplayColumn.Name,
        DataBinder.GetPropertyValue(FieldValue, parentTable.PrimaryKeyColumns[0].Name))%>
--%>
<select name="<%= Column.Name %>.<%= parentTable.PrimaryKeyColumns[0].Name %>">
<% foreach (var entry in parentTable.GetQuery()) {%>
    <option value="<%= DataBinder.GetPropertyValue(entry, parentTable.PrimaryKeyColumns[0].Name) %>" <%= SelectedAttribute(entry) %>>
        <%= DataBinder.GetPropertyValue(entry, parentTable.DisplayColumn.Name) %>
    </option>
<% }%>
</select>