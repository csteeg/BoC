<%@ Page Language="C#" AutoEventWireup="true" CodeFile="listprofiles.aspx.cs" 
    Inherits="Profiles_ListProfiles" MasterPageFile="~/mainmaster.master" %>

<%@ Register Src="ListOfProfiles.ascx" TagName="ListOfProfiles" TagPrefix="uc1" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent" EnableViewState="false">
    <uc1:ListOfProfiles ID="ListOfProfiles1" runat="server" />
</asp:Content>