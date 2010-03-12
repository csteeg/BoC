<%@ Control Language="C#" Inherits="JqueryMvc.Mvc.ExtViewUserControl" %>
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <p>
        To learn more about ASP.NET MVC visit <a href="http://asp.net/mvc" title="ASP.NET MVC Website">http://asp.net/mvc</a>.
    </p>
    
<% using (Ajax.BeginForm("SayHello", new AjaxOptions { UpdateTargetId = "dynamiccontent" }))
   { %>
    <%= Html.Label("name", "Name:")%> <%= Html.TextBox("name", Request["Name"])%>
    <br /><br />
    <%= Html.SubmitButton(string.Empty, "Say hello")%>
    <%= Ajax.JsonSubmitButton(string.Empty, "Say hello - json", "form:first", 
        "function(obj) {jQuery('#dynamiccontent').html(obj.HelloWorld)}") %>
<% } %>

    <div id="dynamiccontent"></div>
