Imports DevExpress.XtraEditors
Imports DevExpress.Skins
Imports System.Threading
Imports System.Globalization
Imports System.IO

Public Class MainForm

    Public Sub New()
        Dim currentWithOverriddenNumber As CultureInfo = New CultureInfo(CultureInfo.CurrentCulture.Name)
        currentWithOverriddenNumber.NumberFormat.CurrencyPositivePattern = 0 '; // make sure there is no space between symbol and number
        'currentWithOverriddenNumber.NumberFormat.CurrencySymbol = "" '; // no currency symbol
        currentWithOverriddenNumber.NumberFormat.CurrencyDecimalSeparator = "." '; //decimal separator
        currentWithOverriddenNumber.NumberFormat.CurrencyGroupSizes = {3} '; //no digit groupings
        currentWithOverriddenNumber.NumberFormat.CurrencyGroupSeparator = ","
        currentWithOverriddenNumber.NumberFormat.NumberGroupSizes = {3} ';
        currentWithOverriddenNumber.NumberFormat.NumberGroupSeparator = ","
        currentWithOverriddenNumber.NumberFormat.NumberDecimalSeparator = "." '; //decimal separator
        currentWithOverriddenNumber.DateTimeFormat.FullDateTimePattern = "dd/MM/yyyy hh:mm"
        currentWithOverriddenNumber.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy"
        Thread.CurrentThread.CurrentCulture = currentWithOverriddenNumber
        InitializeComponent()
        DevExpress.Skins.SkinManager.EnableFormSkins()
        DevExpress.UserSkins.BonusSkins.Register()

        SkinName = My.Settings.LookAndFeel

    End Sub

    Private Sub MainForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Me.Dispose()
        End
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(SkinName)
        nbcMainMenu.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.NavigationPane
        nbcMainMenu.PaintStyleName = My.Settings.LookAndFeel
        If Not My.Settings.GetPreviousVersion("DBFileName") Is Nothing Then
            If My.Computer.Name <> "FARELLANO" Then
                My.Settings.Upgrade()
            End If
        End If
        bbiUserApp.Caption = "User: " & My.User.Name
        'nbcMainMenu.RestoreFromRegistry(Directory.GetCurrentDirectory)
    End Sub

    Private Sub NavBarItem12_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem12.LinkClicked
        OpenForm(New PreferencesForm)
    End Sub

    Private Sub NavBarItem13_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem13.LinkClicked
        OpenForm(New LookAndFeelForm)
    End Sub

    Private Sub NavBarItem1_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem1.LinkClicked
        OpenForm(New FletesOnLineForm)
    End Sub

    Private Sub SelectPage(ByVal FormName As String)
        For Each myChildForm In MdiChildren
            If myChildForm.Name = FormName Then
                myChildForm.Focus()
            End If
        Next
    End Sub

    Private Sub OpenForm(AppForm As Windows.Forms.Form)
        Try
            Dim myForm As New Windows.Forms.Form
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
        If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Are you sure to exit?", "Exit", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.No Then
            e.Cancel = True
        End If
        nbcMainMenu.SaveToRegistry(Directory.GetCurrentDirectory)
    End Sub

    Private Sub NavBarItem20_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem20.LinkClicked
        OpenForm(New VesselScheduleSincronizeForm)

    End Sub

    Private Sub NavBarItem21_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem21.LinkClicked
        OpenForm(New LocalVoyageControlSincronizeForm)
    End Sub

    Private Sub NavBarItem14_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem14.LinkClicked
        OpenForm(New PurchaseOrderControlUpdateForm)
    End Sub

    Private Sub NavBarItem15_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem15.LinkClicked
        OpenForm(New CustomerDataForm)
    End Sub

    Private Sub NavBarItem16_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem16.LinkClicked
        OpenForm(New AccrualDataForm)
    End Sub
End Class