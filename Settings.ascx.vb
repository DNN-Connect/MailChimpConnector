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
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports PerceptiveMCAPI.Types
Imports PerceptiveMCAPI.Methods
Imports PerceptiveMCAPI


Public Class Settings
    Inherits ModuleSettingsBase

#Region "Base Method Implementations"

    Private Sub cmdLoadLists_Click(sender As Object, e As EventArgs) Handles cmdLoadLists.Click
        BindLists()
    End Sub

    Public Overrides Sub LoadSettings()

        cmdLoadLists.Text = Localization.GetString("cmdLoadLists", LocalResourceFile)

        Try
            If (Page.IsPostBack = False) Then

                If Settings.Contains("Mailchimp_ApiKey") Then
                    txtApiKey.Text = CType(Settings("Mailchimp_ApiKey"), String)
                    If txtApiKey.Text.Length > 0 Then
                        BindLists()
                    End If
                End If

                If Settings.Contains("Mailchimp_UseDoubleOptIn") Then
                    chkUseDoubleOptIn.Checked = CType(Settings("Mailchimp_UseDoubleOptIn"), Boolean)
                End If
                If Settings.Contains("Mailchimp_SendWelcomeEmailOnSubscribe") Then
                    chkSendWelcomeEmailOnSubscribe.Checked = CType(Settings("Mailchimp_SendWelcomeEmailOnSubscribe"), Boolean)
                End If
                If Settings.Contains("Mailchimp_DeleteMemberOnUnsubscribe") Then
                    chkDeleteMemberOnUnsubscribe.Checked = CType(Settings("Mailchimp_DeleteMemberOnUnsubscribe"), Boolean)
                End If
                If Settings.Contains("Mailchimp_SendGoodbyEmailOnUnSubscribe") Then
                    chkSendGoodbyEmailOnUnSubscribe.Checked = CType(Settings("Mailchimp_SendGoodbyEmailOnUnSubscribe"), Boolean)
                End If
                If Settings.Contains("Mailchimp_SendAdminNotificationOnUnsubscribe") Then
                    chkSendAdminNotificationOnUnsubscribe.Checked = CType(Settings("Mailchimp_SendAdminNotificationOnUnsubscribe"), Boolean)
                End If


            End If
        Catch exc As Exception           'Module failed to load
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    Public Overrides Sub UpdateSettings()
        Try
            Dim objModules As New Entities.Modules.ModuleController

            If txtApiKey.Text.Length > 0 Then
                objModules.UpdateModuleSetting(ModuleId, "Mailchimp_ApiKey", txtApiKey.Text)
            End If

            If drpLists.Items.Count > 0 Then
                objModules.UpdateModuleSetting(ModuleId, "Mailchimp_ListId", drpLists.SelectedValue)
            End If


            objModules.UpdateModuleSetting(ModuleId, "Mailchimp_UseDoubleOptIn", chkUseDoubleOptIn.Checked)
            objModules.UpdateModuleSetting(ModuleId, "Mailchimp_SendWelcomeEmailOnSubscribe", chkSendWelcomeEmailOnSubscribe.Checked)
            objModules.UpdateModuleSetting(ModuleId, "Mailchimp_DeleteMemberOnUnsubscribe", chkDeleteMemberOnUnsubscribe.Checked)
            objModules.UpdateModuleSetting(ModuleId, "Mailchimp_SendGoodbyEmailOnUnSubscribe", chkSendGoodbyEmailOnUnSubscribe.Checked)
            objModules.UpdateModuleSetting(ModuleId, "Mailchimp_SendAdminNotificationOnUnsubscribe", chkSendAdminNotificationOnUnsubscribe.Checked)

        Catch exc As Exception           'Module failed to load
            ProcessModuleLoadException(Me, exc)
        End Try
    End Sub

    Private Sub BindLists()

        drpLists.Items.Clear()

        Dim input As New listsInput(txtApiKey.Text)
        Dim cmd As lists = New lists
        Dim output As listsOutput = cmd.Execute(input)

        If Not output Is Nothing Then
            If output.result.Count > 0 Then

                For Each item As listsResults In output.result
                    drpLists.Items.Add(New ListItem(item.name, item.id))
                Next

                'lists found, set label
                lblList.Text = Localization.GetString("ListsFound", LocalResourceFile)
                lblList.HelpText = Localization.GetString("ListsFound.Help", LocalResourceFile)
                drpLists.Text = Localization.GetString("SelectList", LocalResourceFile)

                If Settings.Contains("Mailchimp_ListId") Then

                    'get listid from settings
                    Dim strList As String = CType(Settings("Mailchimp_ListId"), String)

                    'try to load list id in dropdown
                    If Not drpLists.Items.FindByValue(strList) Is Nothing Then
                        drpLists.Items.FindByValue(strList).Selected = True
                    Else
                        'no list itme with that id
                        lblList.Text = Localization.GetString("ListRemoved", LocalResourceFile)
                    End If

                End If

            Else

                'lists list is empty
                lblList.Text = Localization.GetString("NoListFound", LocalResourceFile)

            End If
        Else

            'no connection?
            lblList.Text = Localization.GetString("NoResponseFromChimp", LocalResourceFile)

        End If



    End Sub

#End Region




End Class