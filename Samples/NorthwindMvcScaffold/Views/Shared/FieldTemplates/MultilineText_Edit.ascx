<%@ Control Language="C#" Inherits="Microsoft.Web.DynamicData.Mvc.MvcFieldTemplate" %>
<%= Html.TextArea(Column.Name, FieldValueEditString, 10, 80,
                  new { @class = "text-box" +
                                 (Column.IsRequired ? " field-required" : "") 
                  })%>