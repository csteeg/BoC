<%@ Control Language="C#" AutoEventWireup="true" Inherits="Microsoft.Web.DynamicData.Mvc.MvcEntityTemplate" %>
<table cellpadding="0" cellspacing="0" border="0" class="table-view details-view">
<% foreach (var column in DisplayColumns) { %>
    <tr>
        <td valign="top"><b><%= DynamicFieldTitle(column) %></b></td>
        <td valign="top"><%= DynamicField(column) %></td>
    </tr>
<% } %>
</table>