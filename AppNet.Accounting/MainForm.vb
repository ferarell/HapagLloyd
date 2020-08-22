Imports DevExpress.XtraEditors
Imports DevExpress.Skins
Imports System.Threading
'Imports Microsoft.Office.Interop.Access
Imports System.Globalization

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
        currentWithOverriddenNumber.DateTimeFormat.FullDateTimePattern = "dd/MM/yyyy HH:mm:ss"
        currentWithOverriddenNumber.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy"
        Thread.CurrentThread.CurrentCulture = currentWithOverriddenNumber
        InitializeComponent()
        TextToSpeak("Staring Office Customization for Hapag Lloyd")
        DevExpress.Skins.SkinManager.EnableFormSkins()
        DevExpress.UserSkins.BonusSkins.Register()
        SkinName = My.Settings.LookAndFeel

        'DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("")
        'DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(SkinName)
        'nbcMainMenu.PaintStyleName = SkinName

        'Dim userLookAndFeel As DevExpress.LookAndFeel.UserLookAndFeel = New DevExpress.LookAndFeel.UserLookAndFeel(Me)
        'userLookAndFeel.UseDefaultLookAndFeel = False
        'userLookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin
        'userLookAndFeel.SkinName = Me.LookAndFeel.SkinName

    End Sub

    Private Sub MainForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Me.Dispose()
        End
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False
        If Not My.Settings.GetPreviousVersion("MDBFileName") Is Nothing Then
            If My.Computer.Name <> "FERARELL" Then
                My.Settings.Upgrade()
            End If
        End If
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(SkinName)
        If My.Settings.PaintStyle = "ExplorerBar" Then
            nbcMainMenu.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.ExplorerBar
        Else
            nbcMainMenu.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.NavigationPane
        End If
        nbcMainMenu.PaintStyleName = My.Settings.LookAndFeel
        MDBFileName = My.Settings.MDBDirectory & "\" & My.Settings.MDBFileName
        UserApp = Environment.UserName & "@" & Environment.UserDomainName
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

    Private Sub NavBarItem1_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem1.LinkClicked
        OpenForm(New BankForm)
    End Sub

    Private Sub MainForm_TextChanged(sender As Object, e As EventArgs) Handles MyBase.TextChanged
        Me.Text = My.Application.Info.ProductName + " [" + My.Application.Info.Version.ToString + "]" ' & UserApp
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Esta seguro de cerrar la aplicación?", "Salir", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.No Then
            e.Cancel = True
        End If
    End Sub

    Private Sub NavBarItem13_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem13.LinkClicked
        OpenForm(New LookAndFeelForm)
    End Sub

    Private Sub NavBarItem12_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem12.LinkClicked
        OpenForm(New PreferencesForm)
    End Sub

    Private Sub NavBarItem5_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem5.LinkClicked
        OpenForm(New RegistroVentasForm)
    End Sub

    Private Sub NavBarItem6_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem6.LinkClicked
        OpenForm(New RegistroComprasForm)
    End Sub

    Private Sub NavBarItem2_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem2.LinkClicked
        OpenForm(New LibroDiarioForm)
    End Sub

    Private Sub NavBarItem3_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem3.LinkClicked
        OpenForm(New LibroMayorForm)
    End Sub

    Private Sub NavBarItem4_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem4.LinkClicked
        OpenForm(New LibroCajaBancosForm)
    End Sub

    Private Sub NavBarItem7_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem7.LinkClicked
        OpenForm(New BalanceGeneralForm)
    End Sub

    Private Sub NavBarItem8_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem8.LinkClicked
        OpenForm(New BalanceComprobacionForm)
    End Sub

    Private Sub NavBarItem9_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem9.LinkClicked
        OpenForm(New EstadosFinancierosForm)
    End Sub

    Private Sub NavBarItem14_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem14.LinkClicked
        'System.Diagnostics.Process.Start("msaccess.exe """ & My.Settings.DBDirectory & "\DBFinance.mdb""")
        OpenForm(New PagoProveedoresForm)

    End Sub

    Private Sub NavBarItem15_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem15.LinkClicked
        OpenForm(New DetraccionesConstanciasForm)
    End Sub

    Private Sub NavBarItem16_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem16.LinkClicked
        OpenForm(New TiposCambioForm)
    End Sub

    Private Sub NavBarItem17_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem17.LinkClicked
        OpenForm(New DetraccionesPagosForm)
    End Sub

    Private Sub NavBarItem18_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem18.LinkClicked
        OpenForm(New RetencionesForm)
    End Sub

    Private Sub NavBarItem19_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem19.LinkClicked
        OpenForm(New ProveedoresForm)
    End Sub

    Private Sub NavBarItem20_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem20.LinkClicked
        OpenForm(New DatosAsociadosForm)
    End Sub

    Private Sub NavBarItem21_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem21.LinkClicked
        OpenForm(New PosicionMonetariaForm)
    End Sub

    Private Sub NavBarItem22_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem22.LinkClicked
        OpenForm(New ImportaDatosExternosForm)
    End Sub

    Private Sub NavBarItem23_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem23.LinkClicked
        OpenForm(New AperturaEjercicioForm)
    End Sub

    Private Sub NavBarItem24_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem24.LinkClicked
        OpenForm(New RegistroActivosFijosForm)
    End Sub

    Private Sub NavBarItem25_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem25.LinkClicked
        OpenForm(New LibroInventariosBalancesForm)
    End Sub
End Class