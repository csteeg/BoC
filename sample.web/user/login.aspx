<%@ Page MasterPageFile="~/mainmaster.master" Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="user_login" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
<atlas:ScriptManager ID="ScriptManager1" runat="server" enablepartialrendering="true" />
<atlas:UpdatePanel runat="server" ID="CreateUserPanel" RenderMode="Inline" EnableViewState="false">
    <ContentTemplate>
    <asp:Login 
        id="LoginComponent"
        CreateUserUrl="~/user/register.aspx"
        CreateUserText="Nog geen lid?"
        PasswordRecoveryText="Wachtwoord vergeten?"
        PasswordRecoveryUrl="~/user/forgotpassword.aspx"
        runat="server">
    </asp:Login>
</ContentTemplate>
</atlas:UpdatePanel>
</asp:Content>