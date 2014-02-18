' Copyright (c) 2014  Philipp Becker
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
Public Class TemplateController

    Public Shared Function GetTemplate(ByVal strPath As String) As String


        If System.IO.File.Exists(strPath) Then
            Dim templ As String = ""
            Dim sr As New System.IO.StreamReader(strPath)
            templ = sr.ReadToEnd
            sr.Close()
            sr.Dispose()
            Return templ
        Else
            Return "Could not load template, sorry..."
        End If

    End Function

End Class
