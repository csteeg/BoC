<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
	Details for <%= Html.Encode(ViewData.Eval("Title")) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <fieldset class="default-view">
        <legend><%= Html.Encode(ViewData.Eval("Title")) %></legend>
    
        <% ViewData["__MyModel"] = Model; %>
        <%= Html.Display("__MyModel") %>
    </fieldset>
</asp:Content>
