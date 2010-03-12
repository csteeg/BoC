<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" 
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
	Edit <%= Html.Encode(ViewData.Eval("Title")) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <fieldset class="default-view">
        <legend>Edit <%= Html.Encode(ViewData.Eval("Title")) %></legend>
        <% using(Html.BeginForm()) { %>
        
            <% ViewData["__MyModel"] = Model; %>
            <%= Html.Editor("__MyModel") %>
        <hr />
            <%= Html.EditorFor(ninja => Model) %>
        <% } %>
    </fieldset>
</asp:Content>
