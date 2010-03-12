<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IPagedList>" %>
<script runat="server">
    protected override void OnInit(EventArgs e) {
        Title = ViewData["Title"] as string;
        base.OnInit(e);
    }
    protected string Sort {
        get { return (string)ViewData["sort"]; }
    }
</script>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dyndata">
        <h1><%= ViewData["Title"]%></h1>
        <br />
        <table cellpadding="0" cellspacing="0" border="0" class="table-view list-view">
            <thead>
                <tr>
                    <th></th>
                    <% 
                        foreach (var column in DynamicData.DisplayShortColumns) { %>
                        <th>
                            <% if (String.IsNullOrEmpty(column.SortExpression)) { %>
                                <%= column.DisplayName %>
                            <% } else { %>
                                <% var sortQuery = (Sort == column.SortExpression ? "-" : "") + column.SortExpression; %>
                                <a href="<%= Url.SetQueryParams(new { sort = Html.Encode(sortQuery) }) %>"><%= column.DisplayName %></a>
                            <% } %>
                        </th>
                    <% } %>
                </tr>
            </thead>
            <tbody>
                <% int count = 0; %>
                <% foreach (var row in Entities) { %>
                    <% var routeData = DynamicData.GetRouteData(row); %>
                    <tr class="<%= (++count) % 2 == 0 ? "even" : "odd" %>">
                        <td class="scaffold-actions" nowrap="nowrap">
                            <% if (!DynamicData.Table.IsReadOnly)
                               { %>
                                <%= Html.ScaffoldLink("Edit", DynamicData.Table, "edit", routeData)%>
                                <% using (Html.BeginForm("Delete", null))
                                   { %>
                                    <%= Html.Hidden("returnTo", Request.Url) %>
                                    <% foreach (var keycolumn in DynamicData.Table.PrimaryKeyColumns) { %>
                                        <%= Html.Hidden(keycolumn.Name, DataBinder.GetPropertyValue(row, keycolumn.Name)) %>
                                    <% } %>
                                    <input type="submit" value="Delete" onclick = "return confirm('Are you sure you want to delete this item?');" />
                            <%     }
                               } %>
                            <%= Html.ScaffoldLink("Details", DynamicData.Table, "show", routeData) %>
                        </td>
                        <% foreach (var column in DynamicData.DisplayShortColumns) { %>
                            <td><%= Html.DynamicField(row, column.Name) %></td>
                        <% } %>
                    </tr>
                <% } %>
                <% if (count == 0) { %>
                    <tr>
                        <td colspan="<%= DynamicData.DisplayShortColumns.Count() + 1 %>">There are no data rows to display for this table.</td>
                    </tr>
                <% } %>
            </tbody>
            <tfoot>
                <tr>
                    <td colspan="<%= DynamicData.DisplayShortColumns.Count() + 1 %>" class="pager" nowrap="nowrap">
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
        <% if (!DynamicData.Table.IsReadOnly) { %>
            <br />
            <div>
                <img src="<%= Url.RelativeUrl("~/Content/DynamicData/plus.gif") %>" border="0" />
                <%= Html.ScaffoldLink("Insert new item", DynamicData.Table, "new") %>
            </div>
        <% } %>
    </div>
</asp:Content>