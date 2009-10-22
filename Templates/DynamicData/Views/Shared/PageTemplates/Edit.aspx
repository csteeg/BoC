<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="Microsoft.Web.DynamicData.Mvc.DynamicScaffoldViewPage" %>
<script runat="server">
    protected override void OnInit(EventArgs e) {
        if (DynamicData.Table.IsReadOnly)
            throw new HttpException(404, "Not Found");

        Title = DynamicData.Table.DisplayName;
        base.OnInit(e);
    }
</script>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dyndata">
        <h1>Edit entry from table <%= DynamicData.Table.DisplayName %></h1>
        <br />
        <%= Html.ValidationSummary() %>
        <% using (Html.BeginForm("Update", null))
           { %>
            <%= Html.DynamicEntity(ViewData.Model, DataBoundControlMode.Edit)%>
            <div class="scaffold-actions">
                <input type="submit" value="Update" />
                &nbsp;
                <%= Html.ScaffoldLink("Cancel", DynamicData.Table, "list")%>
            </div>
        <% } %>
    </div>
</asp:Content>