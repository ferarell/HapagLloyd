Imports DevExpress.LookAndFeel

Public Class PreferencesForm
    Dim oAppService As New AppService.HapagLloydServiceClient

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs)
        FolderBrowserDialog1.SelectedPath = sender.text
        If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            sender.EditValue = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiGuardar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiGuardar.ItemClick
        Validate()
        Try
            My.Settings.EmailQuantityBySend = seEQuantity.Text
            My.Settings.TimeBetweenMails = seWaitTime.Text
            My.Settings.Country = lueCountry.EditValue
            My.Settings.SendMailBehalf = tsSendMailBehalf.EditValue
            My.Settings.MailFrom = teMailFrom.Text
            My.Settings.DaysBeforeArrival = seDaysBeforeArrival.EditValue
            My.Settings.DateFormat = teDateFormat.Text
            My.Settings.Save()
            NotificationsWcfForm.bsiCountry.Caption = "Country: " & My.Settings.Country
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Changes applied successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try

    End Sub

    Private Sub PreferencesForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Application.ApplicationContext.MainForm.Icon
        seEQuantity.Text = My.Settings.EmailQuantityBySend
        seWaitTime.Text = My.Settings.TimeBetweenMails
        tsSendMailBehalf.EditValue = My.Settings.SendMailBehalf
        teMailFrom.EditValue = My.Settings.MailFrom
        seDaysBeforeArrival.EditValue = My.Settings.DaysBeforeArrival
        teDateFormat.EditValue = My.Settings.DateFormat
        For Each cnt In DevExpress.Skins.SkinManager.Default.Skins
            lbcEstilos.Items.Add(cnt.SkinName)
        Next
        lbcEstilos.SelectedValue = DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveLookAndFeel.ActiveSkinName
        LoadCountry()
    End Sub

    Private Sub LoadCountry()
        Dim dtQuery As New DataTable
        dtQuery = oAppService.ExecuteSQL("SELECT * FROM dbo.Country").Tables(0)
        lueCountry.Properties.ValueMember = "CountryCode"
        lueCountry.Properties.DisplayMember = "CountryDescription"
        lueCountry.Properties.DataSource = dtQuery
        lueCountry.EditValue = My.Settings.Country
    End Sub

    Private Sub bbiReset_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiReset.ItemClick
        If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Are you sure to reset?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
            My.Settings.Reset()
            Me.PreferencesForm_Load(sender, e)
        End If
    End Sub

    Private Sub lbcEstilos_Click(sender As Object, e As EventArgs) Handles lbcEstilos.Click
        Dim skinName As String
        skinName = lbcEstilos.Text
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("")
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(skinName)
        My.Settings.LookAndFeel = DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveLookAndFeel.ActiveSkinName
        My.Settings.Save()
    End Sub

    Private Sub rgPaintStyle_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rgPaintStyle.SelectedIndexChanged
        My.Settings.PaintStyle = rgPaintStyle.EditValue
        My.Settings.Save()
        'If My.Settings.PaintStyle = "ExplorerBar" Then
        '    MainForm.nbcMainMenu.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.ExplorerBar
        'Else
        '    MainForm.nbcMainMenu.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.NavigationPane
        'End If
    End Sub

End Class