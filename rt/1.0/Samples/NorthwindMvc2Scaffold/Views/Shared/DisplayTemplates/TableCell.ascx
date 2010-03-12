<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<% if (ViewData.ModelMetadata.IsComplexType && ViewData.ModelMetadata.Properties.Any(p => p.PropertyName == "Id"))
   { %>
    <%= Html.ActionLink(
            ViewData.ModelMetadata.SimpleDisplayText, 
            "Show", 
            ViewData.ModelMetadata.Model.GetType().BaseType.Name, 
            new {Id = ViewData.ModelMetadata.Properties.FirstOrDefault(p => p.PropertyName == "Id").Model},
            null) %>
<% } else { %>
<%= ViewData.ModelMetadata.SimpleDisplayText %>
<% } %>