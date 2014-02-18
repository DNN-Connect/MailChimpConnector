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
Imports DotNetNuke.Framework

Public Class ModuleBase
    Inherits PortalModuleBase

    Public ReadOnly Property ModuleTheme() As String
        Get
            If Settings.Contains("ModuleTheme") Then
                Return CType(Settings("ModuleTheme"), String)
            Else
                Return "Default"
            End If
        End Get
    End Property

    Public ReadOnly Property CurrentLocale() As String
        Get
            Return CType(Page, PageBase).PageCulture.Name
        End Get
    End Property

    Public ReadOnly Property TemplateBasePath() As String
        Get
            Return Server.MapPath(Me.TemplateSourceDirectory & "/Templates/")
        End Get
    End Property

    Public Function GetTemplate(SelectedTheme As String, TemplateName As String, Locale As String, CreateIfNotExists As Boolean) As String

        'make sure the basepath is not in the theme name. That way we can use it also from the view controls...
        SelectedTheme = SelectedTheme.ToLower.Replace(TemplateBasePath.ToLower, "")

        Dim path As String = TemplateBasePath & SelectedTheme & "\" & TemplateName.Replace(Constants.TemplateName_Extension, "." & Locale & Constants.TemplateName_Extension)

        If PortalSettings.DefaultLanguage.ToLower = Locale.ToLower Or String.IsNullOrEmpty(Locale) Then
            path = TemplateBasePath & SelectedTheme & "\" & TemplateName
        End If

        If System.IO.File.Exists(path) Then
            Return TemplateController.GetTemplate(path)
        End If

        If CreateIfNotExists Then
            Dim sourcePath As String = SelectedTheme & "\" & TemplateName
            Dim targetPath As String = SelectedTheme & "\" & TemplateName.Replace(Constants.TemplateName_Extension, "." & Locale & Constants.TemplateName_Extension)

            If PortalSettings.DefaultLanguage.ToLower = Locale.ToLower Or String.IsNullOrEmpty(Locale) Then
                targetPath = SelectedTheme & "\" & TemplateName
            End If

            Try
                System.IO.File.Copy(sourcePath, targetPath, True)
                Return TemplateController.GetTemplate(targetPath)
            Catch
            End Try
        End If

        If Not System.IO.File.Exists(path) Then
            path = TemplateBasePath & TemplateBasePath & "Default\" & TemplateName
        End If

        Return TemplateController.GetTemplate(path)

    End Function

End Class
