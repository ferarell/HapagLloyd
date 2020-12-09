Imports System.Windows

Public Class MainForm

    Public Sub New()

        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        DevExpress.Skins.SkinManager.EnableFormSkins()
        DevExpress.UserSkins.BonusSkins.Register()

    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(My.Settings.LookAndFeel)
        'bsiVersion.Caption = "Versión : " & My.Application.Info.Version.ToString
        'If My.Settings.PaintStyle = "ExplorerBar" Then
        '    nbcMainMenu.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.ExplorerBar
        'Else
        '    nbcMainMenu.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.NavigationPane
        'End If
        'AccordionControl1.StyleController.LookAndFeel.ActiveSkinName( = My.Settings.LookAndFeel
    End Sub

    Private Sub SelectPage(ByVal FormName As String)
        For Each myChildForm In MdiChildren
            If myChildForm.Name = FormName Then
                myChildForm.Focus()
            End If
        Next
    End Sub

    Private Sub OpenForm(AppForm As Form)
        Try
            Dim myForm As New Form
            myForm = AppForm
            If Me.Controls.Find(myForm.Name, True).Count = 0 Then
                myForm.MdiParent = Me
                myForm.Show()
            Else
                SelectPage(myForm.Name)
            End If
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub MainForm_TextChanged(sender As Object, e As EventArgs) Handles MyBase.TextChanged
        Me.Text = My.Application.Info.ProductName + " [" + My.Application.Info.Version.ToString + "]"
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Are you sure to exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
            e.Cancel = True
        End If
    End Sub

    Private Sub MainForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Me.Dispose()
        End
    End Sub

    Private Sub AccordionControlElement2_Click(sender As Object, e As EventArgs) Handles AccordionControlElement2.Click
        OpenForm(LayoutForm)
    End Sub

    Private Sub AccordionControlElement4_Click(sender As Object, e As EventArgs) Handles AccordionControlElement4.Click
        OpenForm(SettingsForm)
    End Sub

    Private Sub AccordionControlElement3_Click(sender As Object, e As EventArgs) Handles AccordionControlElement3.Click
        OpenForm(StatisticsForm)
    End Sub
End Class
