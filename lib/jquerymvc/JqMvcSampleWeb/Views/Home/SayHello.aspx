<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.RenderPartial("_Index"); %>
    <% Html.RenderPartial("_SayHello");  %>
</asp:Content>
