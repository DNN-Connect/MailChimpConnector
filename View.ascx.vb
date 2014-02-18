' Copyright (c) 2014  Philipp Becker
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
Imports DotNetNuke
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports PerceptiveMCAPI
Imports PerceptiveMCAPI.Types
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common.Globals
Imports PerceptiveMCAPI.Methods
Imports DotNetNuke.Web.UI.WebControls


Public Class View
    Inherits ModuleBase
    Implements IActionable

#Region "Private Members"

    Private _apikey As String = Null.NullString
    Private _listid As String = Null.NullString
    Private _mergefields As List(Of listMergeVarsResults)
    Private _listmember As listMemberInfoResults

    Private ReadOnly Property UseDoubleOptIn() As Boolean
        Get

            If Not Settings("Mailchimp_UseDoubleOptIn") Is Nothing Then
                Try
                    Return Boolean.Parse(CType(Settings("Mailchimp_UseDoubleOptIn"), String))
                Catch
                End Try
            End If

            Return False

        End Get
    End Property

    Private ReadOnly Property SendWelcomeEmailOnSubscribe() As Boolean
        Get

            If Not Settings("Mailchimp_SendWelcomeEmailOnSubscribe") Is Nothing Then
                Try
                    Return Boolean.Parse(CType(Settings("Mailchimp_SendWelcomeEmailOnSubscribe"), String))
                Catch
                End Try
            End If

            Return False

        End Get
    End Property

    Private ReadOnly Property DeleteMemberOnUnsubscribe() As Boolean
        Get

            If Not Settings("Mailchimp_DeleteMemberOnUnsubscribe") Is Nothing Then
                Try
                    Return Boolean.Parse(CType(Settings("Mailchimp_DeleteMemberOnUnsubscribe"), String))
                Catch
                End Try
            End If

            Return False

        End Get
    End Property

    Private ReadOnly Property SendGoodbyEmailOnUnSubscribe() As Boolean
        Get

            If Not Settings("Mailchimp_SendGoodbyEmailOnUnSubscribe") Is Nothing Then
                Try
                    Return Boolean.Parse(CType(Settings("Mailchimp_SendGoodbyEmailOnUnSubscribe"), String))
                Catch
                End Try
            End If

            Return True

        End Get
    End Property

    Private ReadOnly Property SendAdminNotificationOnUnsubscribe() As Boolean
        Get

            If Not Settings("Mailchimp_SendAdminNotificationOnUnsubscribe") Is Nothing Then
                Try
                    Return Boolean.Parse(CType(Settings("Mailchimp_SendAdminNotificationOnUnsubscribe"), String))
                Catch
                End Try
            End If

            Return True

        End Get
    End Property

#End Region

#Region "Event Handlers"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        DotNetNuke.Framework.AJAX.RegisterScriptManager()
        LoadList()

    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load


    End Sub

    Protected Sub cmdShowUnSubscribe_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        pnlSettings.Visible = False
        pnlSettingsResult.Visible = False
        pnlSubscribe.Visible = False
        pnlSubscribeResult.Visible = False
        pnlUnSubscribe.Visible = True
        pnlUnSubscribeResult.Visible = False
        pnlError.Visible = False

    End Sub

    Protected Sub cmdUnSubscribe_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        pnlSettings.Visible = False
        pnlSettingsResult.Visible = False
        pnlSubscribe.Visible = False
        pnlUnSubscribe.Visible = False
        pnlUnSubscribeResult.Visible = False
        pnlError.Visible = False

        If UnSubscribe() Then
            pnlUnSubscribeResult.Visible = True
            pnlUnSubscribe.Visible = False
        Else
            pnlUnSubscribeResult.Visible = False
            pnlUnSubscribe.Visible = True
        End If

    End Sub

    Protected Sub cmdSubscribe_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        pnlSettings.Visible = False
        pnlSettingsResult.Visible = False
        pnlSubscribe.Visible = False
        pnlUnSubscribe.Visible = False
        pnlUnSubscribeResult.Visible = False
        pnlError.Visible = False

        If Subscribe() Then
            pnlSubscribeResult.Visible = True
            pnlSubscribe.Visible = False
        Else
            pnlSubscribeResult.Visible = False
            pnlSubscribe.Visible = True
        End If

    End Sub

    Protected Sub cmdUpdateSettings_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        pnlSubscribe.Visible = False
        pnlUnSubscribe.Visible = False
        pnlUnSubscribeResult.Visible = False
        pnlSubscribeResult.Visible = False
        pnlSubscribe.Visible = False
        pnlError.Visible = False

        If UpdateSubscription() Then
            pnlSettings.Visible = False
            pnlSettingsResult.Visible = True
        Else
            pnlSettings.Visible = True
            pnlSettingsResult.Visible = False
        End If

    End Sub

#End Region

#Region "Private Methods"

    Private Function UpdateSubscription() As Boolean

        Dim strEmail As String = Null.NullString
        Dim merges As New Dictionary(Of String, Object)
        Dim subscriptionType As EnumValues.emailType = EnumValues.emailType.html

        ParseForm(pnlSettings, merges, strEmail, subscriptionType)

        'add to list
        If strEmail <> Null.NullString Then
            Dim input As New listUpdateMemberInput(_apikey, _listid, strEmail, merges, subscriptionType, True)
            Dim cmd As New listUpdateMember
            Dim output As listUpdateMemberOutput = cmd.Execute(input)
            If Not output Is Nothing Then
                Return output.result
            End If
        End If

        Return False

    End Function

    Private Function Subscribe() As Boolean

        Dim strEmail As String = Null.NullString
        Dim merges As New Dictionary(Of String, Object)
        Dim subscriptionType As EnumValues.emailType = EnumValues.emailType.html

        ParseForm(pnlSubscribe, merges, strEmail, subscriptionType)

        'add to list
        If strEmail <> Null.NullString Then
            Dim input As New listSubscribeInput(_apikey, _listid, strEmail, merges, subscriptionType, UseDoubleOptIn, True, True, SendWelcomeEmailOnSubscribe)
            Dim cmd As New listSubscribe
            Dim output As listSubscribeOutput = cmd.Execute(input)
            If Not output Is Nothing Then
                Return output.result
            End If
        End If

        Return False

    End Function

    Private Function UnSubscribe() As Boolean

        Dim strEmail As String = Null.NullString
        Dim txtEmail As TextBox = FindControlRecursive(pnlUnSubscribe, pnlUnSubscribe.ID & "_txtEMAIL")
        If Not txtEmail Is Nothing Then
            If txtEmail.Text.Length > 0 Then
                strEmail = txtEmail.Text
            Else
                ShowErrorDialog(Localization.GetString("FillInAllFields", LocalResourceFile))
                Return False
            End If
        Else
            If Request.IsAuthenticated Then
                strEmail = UserInfo.Email
            Else
                ShowErrorDialog(Localization.GetString("FormSetupError", LocalResourceFile))
                Return False
            End If

        End If

        'add to list
        If strEmail <> Null.NullString Then
            Dim input As New listUnsubscribeInput(_apikey, _listid, strEmail, DeleteMemberOnUnsubscribe, SendGoodbyEmailOnUnSubscribe, SendAdminNotificationOnUnsubscribe)
            Dim cmd As New listUnsubscribe
            Dim output As listUnsubscribeOutput = cmd.Execute(input)
            If Not output Is Nothing Then
                Return output.result
            End If
        End If

        Return False

    End Function

    Private Sub ParseForm(ByVal Container As Control, ByRef merges As Dictionary(Of String, Object), ByRef strEmail As String, ByRef subscriptionType As EnumValues.emailType)

        Dim txtEmail As TextBox = FindControlRecursive(Container, Container.ID & "_txtEMAIL")
        If Not txtEmail Is Nothing Then
            If txtEmail.Text.Length > 0 Then
                strEmail = txtEmail.Text
            Else
                ShowErrorDialog(Localization.GetString("FillInAllFields", LocalResourceFile))
                Exit Sub
            End If
        Else
            If Request.IsAuthenticated Then
                strEmail = UserInfo.Email
            Else
                ShowErrorDialog(Localization.GetString("FormSetupError", LocalResourceFile))
                Exit Sub
            End If
        End If

        Dim rblOptions As RadioButtonList = FindControlRecursive(Container, Container.ID & "_rblOPTIONS")
        If Not rblOptions Is Nothing Then
            If rblOptions.SelectedValue = "html" Then
                subscriptionType = EnumValues.emailType.html
            ElseIf rblOptions.SelectedValue = "text" Then
                subscriptionType = EnumValues.emailType.text
            ElseIf rblOptions.SelectedValue = "mobile" Then
                subscriptionType = EnumValues.emailType.mobile
            Else
                subscriptionType = EnumValues.emailType.NotSpecified
            End If
        End If

        For Each fieldItem As listMergeVarsResults In _mergefields

            Dim strKey As String = fieldItem.tag
            Dim obj As Object = Nothing

            Select Case fieldItem.field_type.ToLower
                Case "radio"

                    Dim rbl As RadioButtonList = FindControlRecursive(Container, Container.ID & "_rbl" & fieldItem.tag)
                    If Not rbl Is Nothing Then
                        obj = rbl.SelectedValue
                    Else

                        If Request.IsAuthenticated Then
                            Try
                                obj = UserInfo.Profile.GetPropertyValue(fieldItem.tag)
                            Catch
                            End Try
                        End If
                        If fieldItem.req AndAlso obj Is Nothing Then
                            ShowErrorDialog(Localization.GetString("FormSetupError", LocalResourceFile))
                            Exit Sub
                        End If
                    End If

                Case "dropdown"

                    Dim drp As DropDownList = FindControlRecursive(Container, Container.ID & "_drp" & fieldItem.tag)
                    If Not drp Is Nothing Then
                        obj = drp.SelectedValue
                    Else
                        If Request.IsAuthenticated Then
                            Try
                                obj = UserInfo.Profile.GetPropertyValue(fieldItem.tag)
                            Catch
                            End Try
                        End If
                        If fieldItem.req AndAlso obj Is Nothing Then
                            ShowErrorDialog(Localization.GetString("FormSetupError", LocalResourceFile))
                            Exit Sub
                        End If
                    End If

                Case "date"

                    Dim ctl As DnnDatePicker = FindControlRecursive(Container, Container.ID & "_ctl" & fieldItem.tag)
                    If Not ctl Is Nothing Then
                        If Not ctl.SelectedDate Is Nothing Then
                            If Not ctl.DbSelectedDate Is Nothing Then
                                obj = ctl.DbSelectedDate
                            Else
                                If fieldItem.req Then
                                    ShowErrorDialog(Localization.GetString("FillInAllFields", LocalResourceFile))
                                    Exit Sub
                                End If
                            End If
                        Else
                            If Request.IsAuthenticated Then
                                Try
                                    obj = UserInfo.Profile.GetPropertyValue(fieldItem.tag)
                                Catch
                                End Try
                            End If
                            If fieldItem.req AndAlso obj Is Nothing Then
                                ShowErrorDialog(Localization.GetString("FillInAllFields", LocalResourceFile))
                                Exit Sub
                            End If
                        End If
                    Else
                        If fieldItem.req Then
                            ShowErrorDialog(Localization.GetString("FormSetupError", LocalResourceFile))
                            Exit Sub
                        End If
                    End If

                Case "number"

                    Dim txt As DnnNumericTextBox = FindControlRecursive(Container, Container.ID & "_txt" & fieldItem.tag)
                    If Not txt Is Nothing Then
                        If Not txt.Value Is Nothing Then
                            If txt.Value > CDec(0) Then
                                obj = Convert.ToInt32(txt.Value)
                            Else
                                If Request.IsAuthenticated Then
                                    Try
                                        obj = UserInfo.Profile.GetPropertyValue(fieldItem.tag)
                                    Catch
                                    End Try
                                End If
                                If fieldItem.req AndAlso obj Is Nothing Then
                                    ShowErrorDialog(Localization.GetString("FillInAllFields", LocalResourceFile))
                                    Exit Sub
                                End If
                            End If
                        Else
                            If Request.IsAuthenticated Then
                                Try
                                    obj = UserInfo.Profile.GetPropertyValue(fieldItem.tag)
                                Catch
                                End Try
                            End If
                            If fieldItem.req AndAlso obj Is Nothing Then
                                ShowErrorDialog(Localization.GetString("FillInAllFields", LocalResourceFile))
                                Exit Sub
                            End If
                        End If
                    Else
                        If Request.IsAuthenticated Then
                            Try
                                obj = UserInfo.Profile.GetPropertyValue(fieldItem.tag)
                            Catch
                            End Try
                        End If
                        If fieldItem.req AndAlso obj Is Nothing Then
                            ShowErrorDialog(Localization.GetString("FormSetupError", LocalResourceFile))
                            Exit Sub
                        End If
                    End If

                Case Else

                    Dim txt As TextBox = FindControlRecursive(Container, Container.ID & "_txt" & fieldItem.tag)
                    If Not txt Is Nothing Then
                        If txt.Text.Length > 0 Then
                            obj = txt.Text
                        Else
                            If Request.IsAuthenticated Then
                                Try
                                    obj = UserInfo.Profile.GetPropertyValue(fieldItem.tag)
                                Catch
                                End Try
                            End If
                            If fieldItem.req AndAlso obj Is Nothing Then
                                ShowErrorDialog(Localization.GetString("FillInAllFields", LocalResourceFile))
                                Exit Sub
                            End If
                        End If
                    Else
                        If Request.IsAuthenticated Then
                            If fieldItem.tag.ToLower = "lname" Then
                                obj = UserInfo.LastName
                            ElseIf fieldItem.tag.ToLower = "fname" Then
                                obj = UserInfo.FirstName
                            Else
                                If Not String.IsNullOrEmpty(UserInfo.Profile.GetPropertyValue(fieldItem.tag)) Then
                                    obj = UserInfo.Profile.GetPropertyValue(fieldItem.tag)
                                End If
                            End If
                        End If
                    End If

            End Select

            If strKey <> "" AndAlso Not obj Is Nothing Then
                merges.Add(strKey, obj)
            End If

        Next

    End Sub

    Public Function FindControlRecursive(ByVal objRoot As Control, ByVal id As String) As Control
        If objRoot.ID = id Then
            Return objRoot
        End If
        For Each c As Control In objRoot.Controls
            Dim t As Control = FindControlRecursive(c, id)
            If Not t Is Nothing Then
                Return t
            End If
        Next
        Return Nothing
    End Function

    Private Sub LoadList()

        If Settings.Contains("Mailchimp_ApiKey") Then
            _apikey = CType(Settings("Mailchimp_ApiKey"), String)
        End If

        If Settings.Contains("Mailchimp_ListId") Then
            _listid = CType(Settings("Mailchimp_ListId"), String)
        End If

        If _apikey = Null.NullString Or _listid = Null.NullString Then
            ShowModuleSettingsNote()
            Exit Sub
        End If

        Dim input As New listMergeVarsInput(_apikey, _listid)
        Dim cmd As New listMergeVars
        Dim output As listMergeVarsOutput = cmd.Execute(input)
        If Not output Is Nothing Then
            If output.result.Count > 0 Then
                _mergefields = output.result
            Else
                ShowModuleSettingsNote()
                Exit Sub
            End If
        Else
            ShowModuleSettingsNote()
            Exit Sub
        End If


        If Not Page.IsPostBack Then
            If IsSubscribed() Then
                pnlSettings.Visible = True
                pnlSettingsResult.Visible = False
                pnlSubscribe.Visible = False
                pnlSubscribeResult.Visible = False
                pnlUnSubscribe.Visible = False
                pnlUnSubscribeResult.Visible = False
                pnlError.Visible = False
            Else
                pnlSettings.Visible = False
                pnlSettingsResult.Visible = False
                pnlSubscribe.Visible = True
                pnlSubscribeResult.Visible = False
                pnlUnSubscribe.Visible = False
                pnlUnSubscribeResult.Visible = False
                pnlError.Visible = False
            End If
        End If

        ProcessTemplates()

    End Sub

    Private Sub ShowModuleSettingsNote()

        lblConfigure.Text = Localization.GetString("lblConfigure", LocalResourceFile)

        pnlModuleSettings.Visible = True
        pnlSettings.Visible = False
        pnlSettingsResult.Visible = False
        pnlSubscribe.Visible = False
        pnlSubscribeResult.Visible = False
        pnlUnSubscribe.Visible = False
        pnlUnSubscribeResult.Visible = False

    End Sub

    Private Sub ProcessTemplates()

        Dim strTemplate As String = GetTemplate(ModuleTheme, Constants.TemplateName_SubscribeForm, CurrentLocale, False)
        ParseTemplate(strTemplate, pnlSubscribe)

        strTemplate = GetTemplate(ModuleTheme, Constants.TemplateName_SubscribeResult, CurrentLocale, False)
        ParseTemplate(strTemplate, pnlSubscribeResult)

        strTemplate = GetTemplate(ModuleTheme, Constants.TemplateName_UnsubscribeForm, CurrentLocale, False)
        ParseTemplate(strTemplate, pnlUnSubscribe)

        strTemplate = GetTemplate(ModuleTheme, Constants.TemplateName_UnsubscribeResult, CurrentLocale, False)
        ParseTemplate(strTemplate, pnlUnSubscribeResult)

        strTemplate = GetTemplate(ModuleTheme, Constants.TemplateName_SettingsForm, CurrentLocale, False)
        ParseTemplate(strTemplate, pnlSettings)

        strTemplate = GetTemplate(ModuleTheme, Constants.TemplateName_SettingsResult, CurrentLocale, False)
        ParseTemplate(strTemplate, pnlSettingsResult)

    End Sub

    Private Sub ParseTemplate(strTemplate As String, Target As Panel)


        Dim literal As New Literal
        Dim delimStr As String = "[]"
        Dim delimiter As Char() = delimStr.ToCharArray()

        Dim templateArray As String()
        templateArray = strTemplate.Split(delimiter)

        For iPtr As Integer = 0 To templateArray.Length - 1 Step 2

            Dim strHTML As String = templateArray(iPtr).ToString()
            Target.Controls.Add(New LiteralControl(strHTML))

            If iPtr < templateArray.Length - 1 Then

                Dim strToken As String = templateArray(iPtr + 1)

                If strToken.StartsWith("MERGE:") Then

                    Dim strKey As String = strToken.Split(Char.Parse(":"))(1)
                    Dim strType As String = strToken.Split(Char.Parse(":"))(2)

                    Select Case strKey.ToLower
                        Case "email"
                            If strType.ToLower = "label" Then

                                Dim strLabelText As String = ""
                                Dim strHelpText As String = ""

                                If Not String.IsNullOrEmpty(Localization.GetString("Label_" & strKey, LocalResourceFile)) Then
                                    strLabelText = Localization.GetString("Label_" & strKey, LocalResourceFile)
                                Else
                                    strLabelText = strKey
                                End If

                                If Not String.IsNullOrEmpty(Localization.GetString("Label_" & strKey & ".Help", LocalResourceFile)) Then
                                    strHelpText = Localization.GetString("Label_" & strKey & ".Help", LocalResourceFile)
                                Else
                                    strHelpText = strKey
                                End If

                                Dim oControl As New System.Web.UI.Control
                                oControl = CType(LoadControl("~/controls/LabelControl.ascx"), DotNetNuke.UI.UserControls.LabelControl)
                                Dim dnnLabel As DotNetNuke.UI.UserControls.LabelControl = CType(oControl, DotNetNuke.UI.UserControls.LabelControl)
                                dnnLabel.Text = strLabelText
                                dnnLabel.HelpText = strHelpText
                                dnnLabel.ID = Target.ID & "_RESX_" & strKey

                                Target.Controls.Add(dnnLabel)

                            ElseIf strType.ToLower = "required" Then

                                Dim objLiteral As New Literal
                                objLiteral.ID = Target.ID & "_" & strKey & "_Required"
                                objLiteral.EnableViewState = False

                                If String.IsNullOrEmpty(Localization.GetString("Label_Required", LocalResourceFile)) Then
                                    objLiteral.Text = "*"
                                Else
                                    objLiteral.Text = Localization.GetString("Label_Required", LocalResourceFile)
                                End If

                                Target.Controls.Add(objLiteral)

                            Else

                                Dim objTextBox As New TextBox
                                objTextBox.ID = Target.ID & "_txt" & strKey
                                objTextBox.EnableViewState = True
                                If Request.IsAuthenticated Then
                                    objTextBox.Text = UserInfo.Email
                                End If
                                Target.Controls.Add(objTextBox)

                            End If


                        Case "options"

                            If strType.ToLower = "label" Then

                                Dim objLiteral As New Literal
                                objLiteral.ID = Target.ID & "_" & strKey
                                objLiteral.EnableViewState = False

                                If String.IsNullOrEmpty(Localization.GetString("Label_" & strKey, LocalResourceFile)) Then
                                    objLiteral.Text = strKey
                                Else
                                    objLiteral.Text = Localization.GetString("Label_" & strKey, LocalResourceFile)
                                End If

                                Target.Controls.Add(objLiteral)

                            Else

                                Dim objRadio As New RadioButtonList
                                objRadio.ID = Target.ID & "_rbl" & strKey
                                objRadio.EnableViewState = True
                                objRadio.Items.Add(New ListItem("HTML", "html"))
                                objRadio.Items.Add(New ListItem("Text", "text"))
                                objRadio.Items.Add(New ListItem("Mobile", "mobile"))
                                objRadio.SelectedValue = "html"
                                Target.Controls.Add(objRadio)

                            End If


                        Case Else

                            For Each item As listMergeVarsResults In _mergefields
                                If item.tag.ToLower = strKey.ToLower Then

                                    If strType.ToLower = "label" Then



                                        Dim strLabelText As String = ""
                                        Dim strHelpText As String = ""

                                        If Not String.IsNullOrEmpty(Localization.GetString("Label_" & item.tag, LocalResourceFile)) Then
                                            strLabelText = Localization.GetString("Label_" & item.tag, LocalResourceFile)
                                        Else
                                            strLabelText = strKey
                                        End If

                                        If Not String.IsNullOrEmpty(Localization.GetString("Label_" & item.tag & ".Help", LocalResourceFile)) Then
                                            strHelpText = Localization.GetString("Label_" & item.tag & ".Help", LocalResourceFile)
                                        Else
                                            strHelpText = strKey
                                        End If

                                        Dim oControl As New System.Web.UI.Control
                                        oControl = CType(LoadControl("~/controls/LabelControl.ascx"), DotNetNuke.UI.UserControls.LabelControl)
                                        Dim dnnLabel As DotNetNuke.UI.UserControls.LabelControl = CType(oControl, DotNetNuke.UI.UserControls.LabelControl)
                                        dnnLabel.Text = strLabelText
                                        dnnLabel.HelpText = strHelpText
                                        dnnLabel.ID = Target.ID & "_RESX_" & item.tag

                                        Target.Controls.Add(dnnLabel)



                                    ElseIf strType.ToLower = "required" Then

                                        If item.req Then

                                            Dim objLiteral As New Literal
                                            objLiteral.ID = Target.ID & "_" & item.tag & "_Required"
                                            objLiteral.EnableViewState = False

                                            If String.IsNullOrEmpty(Localization.GetString("Label_Required", LocalResourceFile)) Then
                                                objLiteral.Text = "*"
                                            Else
                                                objLiteral.Text = Localization.GetString("Label_Required", LocalResourceFile)
                                            End If

                                            Target.Controls.Add(objLiteral)


                                        End If

                                    Else

                                        Select Case item.field_type.ToLower

                                            Case "date"

                                                Dim objDate As New DnnDatePicker
                                                objDate.ID = Target.ID & "_ctl" & item.tag
                                                objDate.EnableViewState = True
                                                If Request.IsAuthenticated Then
                                                    Try
                                                        objDate.SelectedDate = Date.Parse(UserInfo.Profile.GetPropertyValue(strKey))
                                                    Catch
                                                    End Try
                                                End If
                                                Target.Controls.Add(objDate)

                                            Case "dropdown"

                                                Dim objCombo As New DropDownList
                                                objCombo.ID = Target.ID & "_drp" & item.tag
                                                objCombo.EnableViewState = True

                                                For Each strValue As String In item.choices
                                                    objCombo.Items.Add(New ListItem(strValue, strValue))
                                                Next

                                                If Request.IsAuthenticated Then
                                                    Try
                                                        objCombo.SelectedValue = UserInfo.Profile.GetPropertyValue(strKey)
                                                    Catch
                                                    End Try
                                                End If
                                                Target.Controls.Add(objCombo)

                                            Case "radio"

                                                Dim objRadio As New RadioButtonList
                                                objRadio.ID = Target.ID & "_rbl" & item.tag
                                                objRadio.EnableViewState = True

                                                For Each strValue As String In item.choices
                                                    objRadio.Items.Add(New ListItem(strValue, strValue))
                                                Next

                                                If Request.IsAuthenticated Then
                                                    Try
                                                        objRadio.SelectedValue = UserInfo.Profile.GetPropertyValue(strKey)
                                                    Catch
                                                    End Try
                                                End If
                                                Target.Controls.Add(objRadio)

                                            Case "number"

                                                Dim objTextBox As New DnnNumericTextBox
                                                objTextBox.ID = Target.ID & "_txt" & item.tag
                                                objTextBox.EnableViewState = True
                                                objTextBox.NumberFormat.DecimalDigits = 0
                                                objTextBox.ShowSpinButtons = True
                                                If Request.IsAuthenticated Then
                                                    Try
                                                        objTextBox.Value = Integer.Parse(UserInfo.Profile.GetPropertyValue(strKey))
                                                    Catch
                                                    End Try
                                                End If
                                                Target.Controls.Add(objTextBox)

                                            Case Else

                                                Dim objTextBox As New TextBox
                                                objTextBox.ID = Target.ID & "_txt" & item.tag
                                                objTextBox.EnableViewState = True
                                                If Request.IsAuthenticated Then

                                                    If strKey.ToLower = "lname" Then

                                                        If Not _listmember Is Nothing Then
                                                            Try
                                                                objTextBox.Text = _listmember.merges("LNAME")
                                                            Catch
                                                            End Try
                                                        Else
                                                            objTextBox.Text = UserInfo.LastName
                                                        End If


                                                    ElseIf strKey.ToLower = "fname" Then
                                                        If Not _listmember Is Nothing Then
                                                            Try
                                                                objTextBox.Text = _listmember.merges("FNAME")
                                                            Catch
                                                            End Try
                                                        Else
                                                            objTextBox.Text = UserInfo.FirstName
                                                        End If

                                                    Else

                                                        Try
                                                            objTextBox.Text = UserInfo.Profile.GetPropertyValue(strKey)
                                                        Catch
                                                        End Try

                                                    End If


                                                End If
                                                Target.Controls.Add(objTextBox)


                                        End Select
                                    End If

                                End If
                            Next

                    End Select



                ElseIf strToken.StartsWith("BUTTON:") Then

                    Dim strKey As String = strToken.Split(Char.Parse(":"))(1)
                    Dim strText As String = ""

                    If String.IsNullOrEmpty(Localization.GetString(strKey, LocalResourceFile)) Then
                        strText = strKey
                    Else
                        strText = Localization.GetString(strKey, LocalResourceFile)
                    End If

                    Dim cmd As New LinkButton
                    cmd.ID = Target.ID & "_" & strKey
                    cmd.Text = strText
                    cmd.CssClass = strKey
                    Select Case strKey.ToLower
                        Case "showunsubscribe"
                            AddHandler cmd.Click, AddressOf cmdShowUnSubscribe_Click
                            cmd.CssClass += " dnnSecondaryAction"
                        Case "subscribe"
                            AddHandler cmd.Click, AddressOf cmdSubscribe_Click
                            cmd.CssClass += " dnnPrimaryAction"
                        Case "unsubscribe"
                            AddHandler cmd.Click, AddressOf cmdUnSubscribe_Click
                            cmd.CssClass += " dnnSecondaryAction"
                        Case "updatesettings"
                            AddHandler cmd.Click, AddressOf cmdUpdateSettings_Click
                            cmd.CssClass += " dnnPrimaryAction"
                    End Select

                    Target.Controls.Add(cmd)

                ElseIf strToken.StartsWith("LABEL:") Then

                    Dim strKey As String = strToken.Split(Char.Parse(":"))(1)

                    Dim objLiteral As New Literal
                    objLiteral.ID = Target.ID & "_" & strKey
                    objLiteral.EnableViewState = False

                    If String.IsNullOrEmpty(Localization.GetString(strKey, LocalResourceFile)) Then
                        objLiteral.Text = strKey
                    Else
                        objLiteral.Text = Localization.GetString(strKey, LocalResourceFile)
                    End If

                    Target.Controls.Add(objLiteral)


                ElseIf strToken.StartsWith("LNK:") Then

                    Dim strKey As String = strToken.Split(Char.Parse(":"))(1)
                    Dim strText As String = Localization.GetString(strKey, LocalResourceFile)

                    Dim lnk As New HyperLink
                    lnk.ID = strKey
                    lnk.Text = strText

                    If strKey.ToLower = "lnkregister" Then
                        lnk.NavigateUrl = NavigateURL(PortalSettings.RegisterTabId)
                    ElseIf strKey.ToLower = "myprofile" Then
                        lnk.NavigateUrl = NavigateURL(PortalSettings.UserTabId)
                    ElseIf strKey.ToLower = "signout" Then
                        lnk.NavigateUrl = NavigateURL(TabId, "", "ctl=logoff")
                    End If

                    Target.Controls.Add(lnk)

                End If
            End If

        Next

    End Sub

    Private Function IsSubscribed() As Boolean

        If Request.IsAuthenticated = False Then
            Return False
        End If

        Dim input As listsForEmailInput = New listsForEmailInput(_apikey, UserInfo.Email)

        Dim cmd As New PerceptiveMCAPI.Methods.listsForEmail
        Dim output As listsForEmailOutput = cmd.Execute(input)

        If output.result.Count > 0 Then
            For Each objList As String In output.result
                If objList = _listid Then

                    Dim inputMember As listMemberInfoInput = New listMemberInfoInput(_apikey, _listid, UserInfo.Email)
                    Dim cmdMember As New listMemberInfo
                    Dim outputMember As listMemberInfoOutput = cmdMember.Execute(inputMember)
                    If Not outputMember Is Nothing Then
                        Try
                            _listmember = outputMember.result
                        Catch
                        End Try
                    End If

                    Return True
                End If
            Next
        End If

        Return False

    End Function

    Private Sub ShowErrorDialog(strText As String)
        pnlError.Visible = True
        lblError.Text = strText
    End Sub

#End Region

#Region "Optional Interfaces"

    Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
        Get
            Dim Actions As New Entities.Modules.Actions.ModuleActionCollection
            Actions.Add(GetNextActionID, Localization.GetString("ManageTemplates.Action", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.AddContent, "", "", EditUrl("ManageTemplates"), False, DotNetNuke.Security.SecurityAccessLevel.Edit, True, False)
            Return Actions
        End Get
    End Property

#End Region

End Class