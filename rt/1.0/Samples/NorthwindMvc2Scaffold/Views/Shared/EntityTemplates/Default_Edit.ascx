<%@ Control Language="C#" AutoEventWireup="true" Inherits="Microsoft.Web.DynamicData.Mvc.MvcEntityTemplate" %>
<% foreach (var keycolumn in Table.PrimaryKeyColumns) { %>
    <%= Html.Hidden(keycolumn.Name, DataBinder.GetPropertyValue(this.Entity, keycolumn.Name)) %>
<% } %>
<table cellpadding="0" cellspacing="0" border="0" class="table-view details-view">
<% foreach (var column in EditColumns) { %>
    <tr>
        <td valign="top"><b><%= DynamicFieldTitle(column) %></b></td>
        <td valign="top">
            <%= DynamicField(column) %><br />
            <%= DynamicFieldErrors(column) %>
        </td>
    </tr>
<% } %>
</table>