<%@ Control Language="C#" Inherits="Microsoft.Web.DynamicData.Mvc.MvcFieldTemplate" %>
<%= Html.TextBox(Column.Name, null, 
                 new { @class = "text-box" +
                                (Column.IsRequired ? " field-required" : "")
                 })%>