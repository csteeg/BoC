<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>My Tables</h1>
    <ul>
        <% foreach (var table in MetaModel.Default.Tables.Where(t => t.Scaffold).OrderBy(t => t.DisplayName))
 {%>
        <li><%=
     Html.ScaffoldLink(table.DisplayName, table, "list")%></li>
        <%
 }%>
    </ul>
</asp:Content>
