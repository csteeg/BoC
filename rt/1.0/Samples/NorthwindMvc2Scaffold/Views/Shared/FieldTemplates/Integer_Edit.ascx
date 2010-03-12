<%@ Control Language="C#" Inherits="Microsoft.Web.DynamicData.Mvc.MvcFieldTemplate" %>
<%= Html.TextBox(Column.Name, FieldValueEditString,
                 new { @class = "text-box" + (Column.IsRequired ? " field-required" : "")
                 }) %>