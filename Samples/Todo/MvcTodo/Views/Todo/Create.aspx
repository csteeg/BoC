<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MvcTodo.Models.Entity.Todo>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="Action">Action:</label>
                <%= Html.TextBox("Action") %>
                <%= Html.ValidationMessage("Action", "*") %>
                
                <%= Html.DropDownList("Catalog") %>
                
            </p>
      <%--      <p>
                <label for="Id">Id:</label>
                <%= Html.TextBox("Id") %>
                <%= Html.ValidationMessage("Id", "*") %>
            </p>--%>
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

