Imports System.IO
Imports EModelView

Public Class Form1
    Public hostContainer As edDWHost = Nothing
    Public WithEvents emvControl As EModelViewControl = Nothing

    Public filepath As String = ""

    Private Sub Form1_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        emvControl.CloseActiveDoc("")

        emvControl = Nothing

        hostContainer = Nothing

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        hostContainer = New edDWHost

        ' Add the container to this form before trying to get the underlying OCX
        Panel1.Controls.Add(hostContainer)
        hostContainer.Dock = DockStyle.Fill

        emvControl = hostContainer.GetOcx()

        If filepath <> "" Then
            emvControl.OpenDoc(filepath, False, False, True, "")
        End If

    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click

        filepath = "D:\09_Study\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\bodies.sldasm"

        emvControl.OpenDoc(filepath, False, False, True, "")

    End Sub

    Public Sub OpenFile(ByVal _filepath As String)

        ' ""

        On Error Resume Next
        filepath = _filepath

        emvControl.OpenDoc(filepath, False, False, True, "")

        Err.Clear()

    End Sub

    Private Sub Form1_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize

        Me.Refresh()

        Me.DoubleBuffered = True

    End Sub

End Class