<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable>" %>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dyndata">
        <h1><%= ViewData["Title"]%></h1>
        <br />
        <table cellpadding="0" cellspacing="0" border="0" class="table-view list-view">
            <thead>
                <tr>
                    <th></th>
                    <% foreach (var prop in ViewData.ModelMetadata.Properties.Where(pm => pm.ShowForDisplay && !ViewData.TemplateInfo.Visited(pm))) { %>
                    <th><%= Html.Label(prop.PropertyName) %></th>
                    <% } %>
                </tr>
            </thead>
            <tbody>
                <% int count = 0; %>
                <% foreach (var row in Model) { %>
                    <tr class="<%= (++count) % 2 == 0 ? "even" : "odd" %>">
                        <td class="scaffold-actions" nowrap="nowrap">
                            <%= Html.ActionLink("Edit", "edit")%>
                            <% using (Html.BeginForm("Delete", null))
                               { %>
                                <%= Html.Hidden("returnTo", Request.Url) %>
                                <%= Html.Hidden("Id", DataBinder.GetPropertyValue(row, "Id")) %>
                                <input type="submit" value="Delete" onclick = "return confirm('Are you sure you want to delete this item?');" />
                            <% } %>
                            <%= Html.ActionLink("Details", "show") %>
                        </td>
                        <% foreach (var prop in ViewData.ModelMetadata.Properties.Where(pm => pm.ShowForDisplay && !ViewData.TemplateInfo.Visited(pm))) { %>
                        <td><%= Html.Display(prop.PropertyName) %></td>
                        <% } %>
                    </tr>
                <% } %>
            </tbody>
            <tfoot>
                <tr>
                    <td colspan="<%= ViewData.ModelMetadata.Properties.Where(pm => pm.ShowForDisplay && !ViewData.TemplateInfo.Visited(pm))).Count() + 1 %>" class="pager" nowrap="nowrap">
                        <div style="float: right; margin-left: 3em">
                            Show
                            <% foreach(int showSize in new int[] { 5, 10, 15, 20 }) { %>
                                <% if (Entities.PageSize == showSize) { %>
                                    <b><%= showSize %></b>
                                <% } else { %>
                                    <a href="<%= Url.SetQueryParams(new { show = showSize, page = 1 }) %>"><%= showSize %></a>
                                <% } %>
                            <% } %>
                            items
                        </div>
                        <% if (Entities.HasPreviousPage) { %>
                            <a href="<%= Url.SetQueryParams(new { page = 1 }) %>"><img src="<%= Url.RelativeUrl("~/Content/DynamicData/page_first.gif") %>" border="0" /></a>
                            <a href="<%= Url.SetQueryParams(new { page = Entities.CurrentPage - 1 }) %>"><img src="<%= Url.RelativeUrl("~/Content/DynamicData/page_prev.gif") %>" border="0" /></a>
                        <% } else { %>
                            <img src="<%= Url.RelativeUrl("~/Content/DynamicData/page_first.gif") %>" border="0" />
                            <img src="<%= Url.RelativeUrl("~/Content/DynamicData/page_prev.gif") %>" border="0" />
                        <% } %>
                        &nbsp; Page <%= Entities.CurrentPage %> of <%= Entities.TotalPages %> &nbsp;
                        <% if (Entities.HasNextPage) { %>
                            <a href="<%= Url.SetQueryParams(new { page = Entities.CurrentPage + 1 }) %>"><img src="<%= Url.RelativeUrl("~/Content/DynamicData/page_next.gif") %>" border="0" /></a>
                            <a href="<%= Url.SetQueryParams(new { page = Entities.TotalPages }) %>"><img src="<%= Url.RelativeUrl("~/Content/DynamicData/page_last.gif") %>" border="0" /></a>
                        <% } else { %>
                            <img src="<%= Url.RelativeUrl("~/Content/DynamicData/page_next.gif") %>" border="0" />
                            <img src="<%= Url.RelativeUrl("~/Content/DynamicData/page_last.gif") %>" border="0" />
                        <% } %>
                    </td>
                </tr>
            </tfoot>
        </table>
            <br />
            <div>
                <img src="<%= Url.RelativeUrl("~/Content/DynamicData/plus.gif") %>" border="0" />
                <%= Html.ScaffoldLink("Insert new item", DynamicData.Table, "new") %>
            </div>
    </div>
</asp:Content>