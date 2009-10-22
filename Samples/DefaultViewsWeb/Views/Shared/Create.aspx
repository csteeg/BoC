<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create <%= Html.Encode(ViewData.Eval("Title")) %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>
    <fieldset class="default-view">
        <legend>Create <%= Html.Encode(ViewData.Eval("Title")) %></legend>
        <% using (Html.BeginForm()) { %>
        
            <% ViewData["__MyModel"] = Model; %>
            <%= Html.Editor("__MyModel")%>
            
            <input type="submit" />
        <% } %>
    </fieldset>
</asp:Content>
