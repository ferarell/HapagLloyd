Public Class PreferencesForm

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beDataSourcePath.Properties.ButtonClick, ButtonEdit1.Properties.ButtonClick, beDataTargetPath.Properties.ButtonClick, beVendorSourcePath.Properties.ButtonClick, beDatabasePath.Properties.ButtonClick
        If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            sender.EditValue = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiGuardar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiGuardar.ItemClick
        Try
            Validate()
            My.Settings.MailEnabled = ceMailEnabled.Checked
            My.Settings.DBFileName = teBDFileName.EditValue
            My.Settings.MDBDirectory = beDatabasePath.EditValue
            My.Settings.MDBFileName = teDBFileName.EditValue
            My.Settings.DataSourcePath = beDataSourcePath.EditValue
            My.Settings.DataTargetPath = beDataTargetPath.EditValue
            My.Settings.VendorSourcePath = beVendorSourcePath.EditValue
            My.Settings.MaxTemp = teMaxTemp.EditValue
            My.Settings.MailServerHost = teSMTPServer.EditValue
            My.Settings.MailServerPort = teSMTPPort.EditValue
            My.Settings.MailServerSsl = ceSMTPSsl.Checked
            My.Settings.MailServerUser = teSMTPUser.EditValue
            My.Settings.MailServerPassword = teSMTPPassword.EditValue
            My.Settings.MailTo = teMailTo.EditValue
            My.Settings.MailCC = teMailCC.EditValue
            My.Settings.MailSender = teSMTPSender.EditValue
            My.Settings.Save()
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The changes was applied successfully.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try

    End Sub

    Private Sub PreferencesForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ceMailEnabled.Checked = My.Settings.MailEnabled
        teBDFileName.EditValue = My.Settings.DBFileName
        beDataSourcePath.EditValue = My.Settings.DataSourcePath
        beDataTargetPath.EditValue = My.Settings.DataTargetPath
        beDatabasePath.EditValue = My.Settings.MDBDirectory
        teDBFileName.EditValue = My.Settings.MDBFileName
        beVendorSourcePath.EditValue = My.Settings.VendorSourcePath
        teMaxTemp.EditValue = My.Settings.MaxTemp
        teSMTPServer.EditValue = My.Settings.MailServerHost
        teSMTPPort.EditValue = My.Settings.MailServerPort
        ceSMTPSsl.Checked = My.Settings.MailServerSsl
        teSMTPUser.EditValue = My.Settings.MailServerUser
        teSMTPPassword.EditValue = My.Settings.MailServerPassword
        teMailTo.EditValue = My.Settings.MailTo
        teMailCC.EditValue = My.Settings.MailCC
        teSMTPSender.EditValue = My.Settings.MailSender
    End Sub

    Private Sub sbMailTest_Click(sender As Object, e As EventArgs) Handles sbMailTest.Click
        SendMail("Mail Test", "This is a test mail, please don't reply.", False)
    End Sub

    Private Sub bbiReset_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiReset.ItemClick
        If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Are you sure to reset?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
            My.Settings.Reset()
            Me.PreferencesForm_Load(sender, e)
        End If
    End Sub
End Class