<%@ Control Language="vb" AutoEventWireup="true" CodeBehind="View.ascx.vb" Inherits="Connect.Modules.MailChimpConnector.View" %>

    <asp:Panel ID="pnlModuleSettings" runat="server">
        <p><asp:Literal ID="lblConfigure" runat="server"></asp:Literal></p>
    </asp:Panel>
    <asp:Panel ID="pnlError" runat="server" CssClass="errorbox" Visible="false">
        <asp:Literal ID="lblError" runat="server"></asp:Literal>
    </asp:Panel>

    <asp:Panel ID="pnlSubscribe" runat="server"></asp:Panel>
    <asp:Panel ID="pnlSubscribeResult" runat="server"></asp:Panel>
    <asp:Panel ID="pnlUnSubscribe" runat="server"></asp:Panel>
    <asp:Panel ID="pnlUnSubscribeResult" runat="server"></asp:Panel>
    <asp:Panel ID="pnlSettings" runat="server"></asp:Panel>
    <asp:Panel ID="pnlSettingsResult" runat="server"></asp:Panel>

