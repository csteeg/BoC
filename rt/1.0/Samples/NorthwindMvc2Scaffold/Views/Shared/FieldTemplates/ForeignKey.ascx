<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<a href="<%= this.Model.GetType() %>/list"><%= this.Model.GetType() %></a>