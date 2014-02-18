<%@ Control Language="vb" AutoEventWireup="true" CodeBehind="Settings.ascx.vb" Inherits="Connect.Modules.MailChimpConnector.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>


<div class="dnnFormItem">
    <dnn:Label ID="lblApiKey" runat="server" />  
    <asp:TextBox ID="txtApiKey" runat="server" />&nbsp;<asp:LinkButton ID="cmdLoadLists" runat="server"></asp:LinkButton>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lblList" runat="server" />
    <asp:DropDownList ID="drpLists" runat="server"></asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lblUseDoubleOptIn" runat="server" />  
    <asp:checkbox ID="chkUseDoubleOptIn" runat="server" />
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lblSendWelcomeEmailOnSubscribe" runat="server" />  
    <asp:checkbox ID="chkSendWelcomeEmailOnSubscribe" runat="server" />
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lblDeleteMemberOnUnsubscribe" runat="server" />  
    <asp:checkbox ID="chkDeleteMemberOnUnsubscribe" runat="server" />
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lblSendGoodbyEmailOnUnSubscribe" runat="server" />  
    <asp:checkbox ID="chkSendGoodbyEmailOnUnSubscribe" runat="server" />
</div>
<div class="dnnFormItem">
    <dnn:Label ID="lblSendAdminNotificationOnUnsubscribe" runat="server" />  
    <asp:checkbox ID="chkSendAdminNotificationOnUnsubscribe" runat="server" />
</div>



