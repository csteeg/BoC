<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>SQLite Providers Usage Examples</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:WebPartManager ID="WebPartManager1" runat="server">
    </asp:WebPartManager>
    <div style="width: 222px; position: absolute; top: 107px; left: 481px; height: 681px;">
        try username:&quot;test1&quot; password:&quot;test1&quot;<br />
        <asp:Login ID="Login1" runat="server" DestinationPageUrl="~/Welcome.aspx" OnLoggedIn="Login1_LoggedIn">
        </asp:Login>
        <br />
        <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" OnCreatedUser="CreateUserWizard1_CreatedUser">
            <WizardSteps>
                <asp:CreateUserWizardStep runat="server">
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep runat="server">
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>
        <asp:ChangePassword ID="ChangePassword1" runat="server">
        </asp:ChangePassword>
    
    </div>
    <div style="width: 282px; position: absolute; top: 111px; left: 729px; height: 122px;">
        <asp:PasswordRecovery ID="PasswordRecovery1" runat="server">
        </asp:PasswordRecovery>
        </div>
    <div style="width: 293px; position: absolute; top: 55px; left: 43px; height: 578px;">
        <asp:CatalogZone ID="CatalogZone1" runat="server">
            <ZoneTemplate>
                <asp:PageCatalogPart ID="PageCatalogPart1" runat="server" />
                <asp:DeclarativeCatalogPart ID="DeclarativeCatalogPart1" runat="server">
                    <WebPartsTemplate>
                        declarative catalog 1
                    </WebPartsTemplate>
                </asp:DeclarativeCatalogPart>
            </ZoneTemplate>
        </asp:CatalogZone>
        <asp:WebPartZone ID="WebPartZone1" runat="server" BorderColor="#CCCCCC" 
            Font-Names="Verdana" Padding="6">
            <EmptyZoneTextStyle Font-Size="0.8em"></EmptyZoneTextStyle>
            <PartStyle Font-Size="0.8em" ForeColor="#333333"></PartStyle>
            <TitleBarVerbStyle Font-Size="0.6em" Font-Underline="False" ForeColor="White">
            </TitleBarVerbStyle>
            <MenuLabelHoverStyle ForeColor="#E2DED6"></MenuLabelHoverStyle>
            <MenuPopupStyle BackColor="#5D7B9D" BorderColor="#CCCCCC" BorderWidth="1px" Font-Names="Verdana" Font-Size="0.6em">
            </MenuPopupStyle>
            <MenuVerbStyle BorderColor="#5D7B9D" BorderWidth="1px" BorderStyle="Solid" ForeColor="White">
            </MenuVerbStyle>
            <PartTitleStyle BackColor="#5D7B9D" Font-Bold="True" Font-Size="0.8em" ForeColor="White">
            </PartTitleStyle>
            <ZoneTemplate>
                <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
                <asp:Button ID="Button1" runat="server" Text="Button" />
            </ZoneTemplate>
            <MenuVerbHoverStyle BackColor="#F7F6F3" BorderColor="#CCCCCC" BorderWidth="1px" BorderStyle="Solid" ForeColor="#333333">
            </MenuVerbHoverStyle>
            <PartChromeStyle BackColor="#F7F6F3" BorderColor="#E2DED6" Font-Names="Verdana" ForeColor="White">
            </PartChromeStyle>
            <HeaderStyle HorizontalAlign="Center" Font-Size="0.7em" ForeColor="#CCCCCC">
            </HeaderStyle>
            <MenuLabelStyle ForeColor="White"></MenuLabelStyle>
        </asp:WebPartZone>
        <asp:EditorZone ID="EditorZone1" runat="server" BackColor="#FFFBD6" 
            BorderColor="#CCCCCC" BorderWidth="1px" Font-Names="Verdana" Padding="6">
            <FooterStyle BackColor="#FFCC66" HorizontalAlign="Right" />
            <PartTitleStyle Font-Bold="True" Font-Size="0.8em" ForeColor="#333333" />
            <PartChromeStyle BorderColor="#FFCC66" BorderStyle="Solid" BorderWidth="1px" />
            <PartStyle BorderColor="#FFFBD6" BorderWidth="5px" />
            <LabelStyle Font-Size="0.8em" ForeColor="#333333" />
            <VerbStyle Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" />
            <ErrorStyle Font-Size="0.8em" />
            <EmptyZoneTextStyle Font-Size="0.8em" ForeColor="#333333" />
            <ZoneTemplate>
                <asp:LayoutEditorPart ID="LayoutEditorPart1" runat="server" />
            </ZoneTemplate>
            <EditUIStyle Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" />
            <HeaderStyle BackColor="#FFCC66" Font-Bold="True" Font-Size="0.8em" 
                ForeColor="#333333" />
            <HeaderVerbStyle Font-Bold="False" Font-Size="0.8em" Font-Underline="False" 
                ForeColor="#333333" />
            <InstructionTextStyle Font-Size="0.8em" ForeColor="#333333" />
        </asp:EditorZone>
        <br />
    </div>
    
    </form>
    
</body>
</html>
