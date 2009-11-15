<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable>" %>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">
    <%
        var properties =
            ViewData.ModelMetadata.Properties.Where(pm => pm.ShowForDisplay && !ViewData.TemplateInfo.Visited(pm));
    %>
    <div class="dyndata">
        <h1><%= ViewData["Title"]%></h1>
        <br />
        <table cellpadding="0" cellspacing="0" border="0" class="table-view list-view">
            <thead>
                <tr>
                    <th></th>
                    <% foreach (var prop in properties) { %>
                    <th><%= Html.Label(prop.PropertyName) %></th>
                    <% } %>
                </tr>
            </thead>
            <tbody>
                <% int count = 0; %>
                <% foreach (var row in Model) {
                       var rowId = DataBinder.GetPropertyValue(row, "Id");
                       %>
                    <tr class="<%= (++count) % 2 == 0 ? "even" : "odd" %>">
                        <td class="scaffold-actions" nowrap="nowrap">
                            <%= Html.ActionLink("Edit", "edit", new {Id = rowId})%>
                            <% using (Html.BeginForm("Delete", null))
                               { %>
                                <%= Html.Hidden("returnTo", Request.Url) %>
                                <%= Html.Hidden("Id", rowId) %>
                                <input type="submit" value="Delete" onclick = "return confirm('Are you sure you want to delete this item?');" />
                            <% } %>
                            <%= Html.ActionLink("Details", "show", new { Id = rowId })%>
                        </td>
                        <% foreach (var prop in properties) {
                                var metaData = ModelMetadataProviders.Current.GetMetadataForType(() => row, row.GetType());
                               %>
                        <td><%= prop.SimpleDisplayText%></td>
                        <% } %>
                    </tr>
                <% } %>
            </tbody>
        </table>
            <br />
            <div>
                <img src="<%= Url.Content("~/Content/DynamicData/plus.gif") %>" border="0" />
                <%= Html.ActionLink("Insert new item", "new") %>
            </div>
    </div>
</asp:Content>