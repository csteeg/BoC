<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DefaultViewsWeb.Models.Ninja>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Details</h2>

    <fieldset>
        <legend>Fields</legend>
        <p>
            Name:
            <%= Html.Encode(Model.Name) %>
        </p>
        <p>
            ShurikenCount:
            <%= Html.Encode(Model.ShurikenCount) %>
        </p>
        <p>
            BlowgunDartCount:
            <%= Html.Encode(Model.BlowgunDartCount) %>
        </p>
        <p>
            Clan:
            <%= Html.Encode(Model.Clan) %>
        </p>
    </fieldset>
    <p>
        <%=Html.ActionLink("Edit", "Edit", new { /* id=Model.PrimaryKey */ }) %> |
        <%=Html.ActionLink("Back to List", "Index") %>
    </p>

</asp:Content>

