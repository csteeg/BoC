<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="BoC.Persistence"%>
<%@ Import Namespace="NorthwindMvcScaffold.Models"%>
<%@ Import Namespace="MvcContrib.UI.Grid"%>
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
        <%=
        Html.Grid((IEnumerable<Product>) Model)
                    .AutoGenerateColumns()
                    .Columns(
                            col => col.For(x => Html.ActionLink("Edit", "Edit", new { Id = x.Id})).Named("").DoNotEncode()
                    )
                    .RenderUsing(new DynamicDataGridRenderer<Product>())
            %>

    </div>
</asp:Content>