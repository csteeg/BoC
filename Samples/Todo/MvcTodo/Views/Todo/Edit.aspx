<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MvcTodo.Models.Entity.Todo>" %>

<%@ Import Namespace="MvcTodo.Models.Entity" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Edit</h2>
    <%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>
    <% using (Html.BeginForm())
       {%>
    <fieldset>
        <legend>Fields</legend>
        <p>
            <label for="Action">
                Action:</label>
            <%= Html.TextBox("Action") %>
            <%= Html.ValidationMessage("Action", "*") %>
        </p>
        <p>
            <%= Html.DropDownList("Catalog.Id", ViewData["catalogs"] as SelectList) %>
        </p>
        <p>
            <label for="Id">
                Id:</label>
            <%= Html.TextBox("Id") %>
            <%= Html.ValidationMessage("Id", "*") %>
        </p>
        <p>
            <input type="submit" value="Save" />
        </p>
    </fieldset>
    <% } %>
    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>
</asp:Content>
