<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%= Html.ActionLink("View " + Model.GetType().Name, "list", Model.GetType().Name + "Controller") %>