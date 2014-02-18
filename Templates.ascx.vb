' Copyright (c) 2014  Philipp Becker
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security.Membership
Imports Telerik.Web.UI
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Framework.JavaScriptLibraries


Partial Class Templates
    Inherits ModuleBase

    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        JavaScript.RequestRegistration(CommonJs.DnnPlugins)

    End Sub

    Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        LocalizeForm()

        If Not Page.IsPostBack Then

            BindThemes()

            If Settings.Contains("ModuleTheme") Then
                Try
                    SelectTheme(CType(Settings("ModuleTheme"), String))
                Catch
                End Try
            End If

            BindSelectedTheme()
            VerifyPasswordSettings()

        End If

    End Sub

    Private Sub drpThemes_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles drpThemes.SelectedIndexChanged
        BindSelectedTheme()
    End Sub

    Private Sub cmdUpdateSettings_Click(sender As Object, e As System.EventArgs) Handles cmdUpdateSettings.Click

        Dim blnSucess As Boolean = False

        SaveTemplates(blnSucess)

        If blnSucess Then
            UpdateSettings()
        End If

        Response.Redirect(NavigateURL())

    End Sub

    Private Sub cmdDeleteSelected_Click(sender As Object, e As System.EventArgs) Handles cmdDeleteSelected.Click

        Try
            DeleteTheme()
        Catch ex As Exception
            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("lblDeleteThemeError", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError)
        End Try

    End Sub

    Private Sub SelectTheme(ThemeName As String)
        drpThemes.Items.FindByText(ThemeName).Selected = True
    End Sub

    Private Sub drpLocales_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles drpLocales.SelectedIndexChanged
        BindSelectedTheme()
    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As System.EventArgs) Handles cmdCancel.Click
        Response.Redirect(NavigateURL())
    End Sub

    Private Sub VerifyPasswordSettings()

        If MembershipProvider.Instance().PasswordRetrievalEnabled = False Then

            Dim strNote As String = Localization.GetString("lblPasswordRetrievalDisabled", LocalResourceFile)
            If MembershipProvider.Instance().RequiresQuestionAndAnswer Then
                strNote += Localization.GetString("lblRequiresQuestionAndAnswer", LocalResourceFile)
            End If
            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, strNote, ModuleMessage.ModuleMessageType.BlueInfo)

        End If

    End Sub

    Private Sub LocalizeForm()

        cmdCancel.Text = Localization.GetString("cmdCancel", LocalResourceFile)
        cmdUpdateSettings.Text = Localization.GetString("cmdUpdateSettings", LocalResourceFile)
        cmdCopySelected.Text = Localization.GetString("cmdCopySelected", LocalResourceFile)
        cmdDeleteSelected.Text = Localization.GetString("cmdDeleteSelected", LocalResourceFile)

    End Sub

    Private Sub cmdCopySelected_Click(sender As Object, e As System.EventArgs) Handles cmdCopySelected.Click
        pnlTemplateName.Visible = True
    End Sub

    Private Sub BindThemes()

        drpThemes.Items.Clear()
        Dim basepath As String = Server.MapPath(Me.TemplateSourceDirectory & "/templates/")

        For Each folder As String In System.IO.Directory.GetDirectories(basepath)
            Dim foldername As String = folder.Substring(folder.LastIndexOf("\") + 1)

            drpThemes.Items.Add(New ListItem(foldername, folder))

        Next

    End Sub

    Private Sub BindSelectedTheme()

        cmdDeleteSelected.Visible = (drpThemes.SelectedIndex <> 0)

        If Settings.Contains("ModuleTheme") Then
            Try
                If CType(Settings("ModuleTheme"), String) = drpThemes.SelectedItem.Text Then
                    chkUseTheme.Checked = True
                    DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdDeleteSelected, Localization.GetSafeJSString(Localization.GetString("lblThemeInUse", LocalResourceFile)))
                Else
                    chkUseTheme.Checked = False
                    DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdDeleteSelected, Localization.GetSafeJSString(Localization.GetString("lblConfirmDelete", LocalResourceFile)))
                End If
            Catch
            End Try
        Else
            DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdDeleteSelected, Localization.GetSafeJSString(Localization.GetString("lblConfirmDelete", LocalResourceFile)))
            chkUseTheme.Checked = False
        End If

        Dim path As String = drpThemes.SelectedValue

        For Each file As String In System.IO.Directory.GetFiles(path)

            If file.EndsWith(Constants.TemplateName_SettingsForm) Then
                txtSettings.Text = GetTemplate(drpThemes.SelectedItem.Value, Constants.TemplateName_SettingsForm, drpLocales.SelectedValue, True)
            End If
            If file.EndsWith(Constants.TemplateName_SettingsResult) Then
                txtSettingsResult.Text = GetTemplate(drpThemes.SelectedItem.Value, Constants.TemplateName_SettingsResult, drpLocales.SelectedValue, True)
            End If
            If file.EndsWith(Constants.TemplateName_SubscribeForm) Then
                txtFormSubscribe.Text = GetTemplate(drpThemes.SelectedItem.Value, Constants.TemplateName_SubscribeForm, drpLocales.SelectedValue, True)
            End If
            If file.EndsWith(Constants.TemplateName_SubscribeResult) Then
                txtFormSubscribeResult.Text = GetTemplate(drpThemes.SelectedItem.Value, Constants.TemplateName_SubscribeResult, drpLocales.SelectedValue, True)
            End If
            If file.EndsWith(Constants.TemplateName_UnsubscribeForm) Then
                txtUnSubscribe.Text = GetTemplate(drpThemes.SelectedItem.Value, Constants.TemplateName_UnsubscribeForm, drpLocales.SelectedValue, True)
            End If
            If file.EndsWith(Constants.TemplateName_UnsubscribeResult) Then
                txtUnSubscribeResult.Text = GetTemplate(drpThemes.SelectedItem.Value, Constants.TemplateName_UnsubscribeResult, drpLocales.SelectedValue, True)
            End If

        Next

    End Sub

    Private Sub SaveTemplate(SelectedTheme As String, TemplateName As String, Locale As String)

        Dim path As String = SelectedTheme & "\" & TemplateName.Replace(Constants.TemplateName_Extension, "." & Locale & Constants.TemplateName_Extension)

        If (PortalSettings.DefaultLanguage.ToLower = Locale.ToLower) Or String.IsNullOrEmpty(Locale) Then
            path = SelectedTheme & "\" & TemplateName
        End If

        Dim sw As New System.IO.StreamWriter(path, False)

        If TemplateName = Constants.TemplateName_SettingsForm Then
            sw.Write(txtSettings.Text)
        End If
        If TemplateName = Constants.TemplateName_SettingsResult Then
            sw.Write(txtSettingsResult.Text)
        End If
        If TemplateName = Constants.TemplateName_SubscribeForm Then
            sw.Write(txtFormSubscribe.Text)
        End If
        If TemplateName = Constants.TemplateName_SubscribeResult Then
            sw.Write(txtFormSubscribeResult.Text)
        End If
        If TemplateName = Constants.TemplateName_UnsubscribeForm Then
            sw.Write(txtUnSubscribe.Text)
        End If
        If TemplateName = Constants.TemplateName_UnsubscribeResult Then
            sw.Write(txtUnSubscribeResult.Text)
        End If

        sw.Close()
        sw.Dispose()

    End Sub

    Private Sub SaveTemplates(ByRef blnSucess As Boolean)

        Dim basepath As String = drpThemes.SelectedValue

        If pnlTemplateName.Visible Then

            If String.IsNullOrEmpty(txtTemplateName.Text) Then
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("lblMustEnterTemplateName", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError)
                blnSucess = False
                Exit Sub
            End If

            If String.IsNullOrEmpty(txtFormSubscribe.Text) Or String.IsNullOrEmpty(txtFormSubscribeResult.Text) Or String.IsNullOrEmpty(txtSettings.Text) Or String.IsNullOrEmpty(txtSettingsResult.Text) Or String.IsNullOrEmpty(txtUnSubscribe.Text) Or String.IsNullOrEmpty(txtUnSubscribeResult.Text) Then
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("lblMustEnterTemplate", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError)
                blnSucess = False
                Exit Sub
            End If

            Dim newpath As String = Server.MapPath(Me.TemplateSourceDirectory & "/templates/") & txtTemplateName.Text
            Try
                System.IO.Directory.CreateDirectory(newpath)
            Catch
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("lblInvalidFolderName", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError)
                blnSucess = False
                Exit Sub
            End Try

            Try
                For Each file As String In System.IO.Directory.GetFiles(basepath)
                    Dim destinationpath As String = newpath & "\" & file.Substring(file.LastIndexOf("\") + 1)
                    System.IO.File.Copy(file, destinationpath)
                Next
                basepath = newpath
            Catch ex As Exception
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("lblCouldNotCopyTheme", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError)
                blnSucess = False
                Exit Sub
            End Try

            pnlTemplateName.Visible = False
            BindThemes()
            SelectTheme(txtTemplateName.Text)
            cmdDeleteSelected.Visible = True

        End If

        Try

            For Each file As String In System.IO.Directory.GetFiles(basepath)

                If file.EndsWith(Constants.TemplateName_SettingsForm) Then
                    SaveTemplate(drpThemes.SelectedValue, Constants.TemplateName_SettingsForm, drpLocales.SelectedValue)
                End If
                If file.EndsWith(Constants.TemplateName_SettingsResult) Then
                    SaveTemplate(drpThemes.SelectedValue, Constants.TemplateName_SettingsResult, drpLocales.SelectedValue)
                End If
                If file.EndsWith(Constants.TemplateName_SubscribeForm) Then
                    SaveTemplate(drpThemes.SelectedValue, Constants.TemplateName_SubscribeForm, drpLocales.SelectedValue)
                End If
                If file.EndsWith(Constants.TemplateName_SubscribeResult) Then
                    SaveTemplate(drpThemes.SelectedValue, Constants.TemplateName_SubscribeResult, drpLocales.SelectedValue)
                End If
                If file.EndsWith(Constants.TemplateName_UnsubscribeForm) Then
                    SaveTemplate(drpThemes.SelectedValue, Constants.TemplateName_UnsubscribeForm, drpLocales.SelectedValue)
                End If
                If file.EndsWith(Constants.TemplateName_UnsubscribeResult) Then
                    SaveTemplate(drpThemes.SelectedValue, Constants.TemplateName_UnsubscribeResult, drpLocales.SelectedValue)
                End If

            Next

        Catch ex As Exception
            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("lblCouldNotWriteTheme", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError)
            blnSucess = False
            Exit Sub
        End Try


        blnSucess = True

    End Sub

    Private Sub UpdateSettings()

        Dim ctrl As New ModuleController
        ctrl.UpdateTabModuleSetting(TabModuleId, "ModuleTheme", drpThemes.SelectedItem.Text)

    End Sub

    Private Sub DeleteTheme()

        Dim basepath As String = drpThemes.SelectedValue
        For Each file As String In System.IO.Directory.GetFiles(basepath)
            System.IO.File.Delete(file)
        Next
        System.IO.Directory.Delete(basepath)
        BindThemes()
        UpdateSettings()
        BindSelectedTheme()

    End Sub

    Private Sub BindLocales()

        Dim dicLocales As Dictionary(Of String, DotNetNuke.Services.Localization.Locale) = LocaleController.Instance().GetLocales(PortalId)

        If dicLocales.Count > 1 Then
            pnlLocales.Visible = True
        End If

        For Each objLocale As DotNetNuke.Services.Localization.Locale In dicLocales.Values

            Dim item As New ListItem
            item.Text = objLocale.Text
            item.Value = objLocale.Code

            Me.drpLocales.Items.Add(item)

        Next

        Try
            drpLocales.Items(0).Selected = True
        Catch
        End Try


    End Sub

End Class



