<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DefaultViewsWeb.Models.Ninja>" %>

    <fieldset>
        <legend>Fields</legend>
        <p>
            Name:
            <%= Html.Encode(Model.Name) %>
        </p>
        <p>
            Shurikens:
            <%= Html.Encode(Model.ShurikenCount) %>
        </p>
        <p>
            Blowgun Darts:
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


