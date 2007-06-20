<%@ Page MasterPageFile="~/mainmaster.master" Language="C#" AutoEventWireup="true" CodeFile="register.aspx.cs" Inherits="user_register" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
<atlas:ScriptManager runat="server" enablepartialrendering="true" />
<atlas:UpdatePanel runat="server" ID="CreateUserPanel">
    <ContentTemplate>
        <asp:CreateUserWizard runat="server" ID="UserCreator"
            DisableCreatedUser="true"
            
            DisplaySideBar="false"
        ></asp:CreateUserWizard>
   </ContentTemplate>
</atlas:UpdatePanel>
</asp:Content>