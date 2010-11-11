<%@ Page Title="" Language="C#" MasterPageFile="~/Views/InputBuilders/Field.Master" Inherits="System.Web.Mvc.ViewPage<MvcContrib.UI.InputBuilder.ModelProperty<object>>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Label" runat="server"><label for="<%=Model.Name%>"><%=Model.Label%></label></asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Input" runat="server"><%=Html.TextBox(Model.Name,Model.Value) %></asp:Content>
