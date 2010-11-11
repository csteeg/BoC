<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%= Html.ScaffoldLink("View " + Model.GetType().Name, "list", null) %>