<%@ Control Language="vb" AutoEventWireup="false" Inherits="Connect.Modules.MailChimpConnector.Templates" Codebehind="Templates.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<div id="MailChimpTemplates" class="dnnForm dnnClear">

    <div class="dnnFormItem">
        <dnn:label id="plTheme" controlname="drpThemes" runat="server" />
        <asp:DropDownList ID="drpThemes" runat="server" AutoPostBack="true"></asp:DropDownList>
        &nbsp;
        <asp:LinkButton ID="cmdCopySelected" runat="server"></asp:LinkButton>
        &nbsp;
        <asp:LinkButton ID="cmdDeleteSelected" runat="server"></asp:LinkButton>
    </div>

    <div class="dnnFormItem" runat="server" id="pnlLocales" visible="false">
        <dnn:label id="plLocale" controlname="drpLocales" runat="server" />
        <asp:DropDownList ID="drpLocales" runat="server" AutoPostBack="true"></asp:DropDownList>
    </div>

    <div class="dnnFormItem">
        <dnn:label id="plUseTheme" controlname="txtTemplateName" runat="server" />
        <asp:CheckBox ID="chkUseTheme" runat="server" />
    </div>       

    <div class="dnnFormItem" runat="server" id="pnlTemplateName" visible="false">
        <dnn:label id="plTemplateName" controlname="txtTemplateName" runat="server" />
        <asp:TextBox ID="txtTemplateName" runat="server"></asp:TextBox>
    </div>

    <ul id="MailChimpTemplatesTabs" runat="Server" class="dnnAdminTabNav dnnClear">
        <li><a href="#dvFormSubscribe"><asp:Label id="lblFormSubscribe" runat="server" resourcekey="lblFormSubscribe" /></a></li>
        <li><a href="#dvFormSubscribeResult"><asp:Label id="lblFormSubscribeResult" runat="server" resourcekey="lblFormSubscribeResult" /></a></li>
        <li><a href="#dvUnSubscribe"><asp:Label id="lblUnSubscribe" runat="server" resourcekey="lblUnSubscribe" /></a></li>
        <li><a href="#dvUnSubscribeResult"><asp:Label id="lblUnSubscribeResult" runat="server" resourcekey="lblUnSubscribeResult" /></a></li>
        <li><a href="#dvSettings"><asp:Label id="lblSettings" runat="server" resourcekey="lblSettings" /></a></li>
        <li><a href="#dvSettingsResult"><asp:Label id="lblSettingsResult" runat="server" resourcekey="lblSettingsResult" /></a></li>
    </ul>

    <div class="dnnFormItem dnnClear" id="dvFormSubscribe">
        <asp:TextBox ID="txtFormSubscribe" runat="server" TextMode="MultiLine" rows="20"></asp:TextBox>
    </div>

    <div class="dnnFormItem dnnClear" id="dvFormSubscribeResult">
        <asp:TextBox ID="txtFormSubscribeResult" runat="server" TextMode="MultiLine" rows="20"></asp:TextBox>
    </div>

    <div class="dnnFormItem dnnClear" id="dvUnSubscribe">
        <asp:TextBox ID="txtUnSubscribe" runat="server" TextMode="MultiLine" rows="20"></asp:TextBox>
    </div>

    <div class="dnnFormItem dnnClear" id="dvUnSubscribeResult">
        <asp:TextBox ID="txtUnSubscribeResult" runat="server" TextMode="MultiLine" rows="20"></asp:TextBox>
    </div>

    <div class="dnnFormItem dnnClear" id="dvSettings">
        <asp:TextBox ID="txtSettings" runat="server" TextMode="MultiLine" rows="20"></asp:TextBox>
    </div>

    <div class="dnnFormItem dnnClear" id="dvSettingsResult">
        <asp:TextBox ID="txtSettingsResult" runat="server" TextMode="MultiLine" rows="20"></asp:TextBox>
    </div>

    <ul class="dnnActions">
        <li><asp:LinkButton ID="cmdUpdateSettings" runat="server" CssClass="dnnPrimaryAction"></asp:LinkButton></li>
        <li><asp:LinkButton ID="cmdCancel" runat="server" CssClass="dnnSecondaryAction"></asp:LinkButton></li>
    </ul>

</div>

<script language="javascript" type="text/javascript">
/*globals jQuery, window, Sys */
(function ($, Sys) {

    function setupFormSettings() {
        $('#MailChimpTemplates').dnnTabs();
    }

    $(document).ready(function () {
        setupFormSettings();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            setupFormSettings();
        });
    });

} (jQuery, window.Sys));
</script>

