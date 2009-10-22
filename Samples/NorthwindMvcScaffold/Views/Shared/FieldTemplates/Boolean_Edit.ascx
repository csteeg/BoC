<%@ Control Language="C#" Inherits="Microsoft.Web.DynamicData.Mvc.MvcFieldTemplate" %>
<%= Html.CheckBox(Column.Name, FieldValue != null && ((bool)FieldValue)) %>