<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="BoC.Web.Mvc.UI.InputBuilder" %>
<script runat="server">
    protected override void OnInit(EventArgs e) {
        Title = ViewData["Title"] as string;
        base.OnInit(e);
    }
</script>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dyndata">
        <h1><%= ViewData["Title"]%></h1>
        <br />
        <%= Html.ValidationSummary() %>
        <%= Html.EditorForModel() %>
            <div class="scaffold-actions">
                <input type="submit" value="Update" />
                &nbsp;
                <%= this.Context %>
                <%= Html.ActionLink("Cancel", "list")%>
            </div>
    </div>
</asp:Content>