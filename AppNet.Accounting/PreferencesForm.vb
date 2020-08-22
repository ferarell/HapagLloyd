Public Class PreferencesForm

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beBankSourceDirectory.Properties.ButtonClick, beBankTargetDirectory.Properties.ButtonClick, beLedgerSourceDirectory5.Properties.ButtonClick, beLedgerTargetDirectory5.Properties.ButtonClick, beLedgerTargetDirectory8.Properties.ButtonClick, beLedgerSourceDirectory8.Properties.ButtonClick, beLedgerTargetDirectory14.Properties.ButtonClick, beLedgerSourceDirectory14.Properties.ButtonClick, beLedgerTargetDirectory1.Properties.ButtonClick, beLedgerSourceDirectory1.Properties.ButtonClick, beLedgerTargetDirectory6.Properties.ButtonClick, beLedgerSourceDirectory6.Properties.ButtonClick, beDetraTargetDirectory.Properties.ButtonClick, beDetraSourceDirectory.Properties.ButtonClick, beRetenTargetDirectory.Properties.ButtonClick, beRetenSourceDirectory.Properties.ButtonClick, beLedgerTargetDirectory3.Properties.ButtonClick, beLedgerSourceDirectory3.Properties.ButtonClick, beMDBDirectory.Properties.ButtonClick, beLedgerTargetDirectory7.Properties.ButtonClick, beLedgerSourceDirectory7.Properties.ButtonClick
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
            My.Settings.MDBFileName = teMDBFileName.Text
            My.Settings.MDBDirectory = beMDBDirectory.Text
            My.Settings.BankSourceDirectory = beBankSourceDirectory.Text
            My.Settings.BankTargetDirectory = beBankTargetDirectory.Text
            My.Settings.LedgerSourceDirectory1 = beLedgerSourceDirectory1.Text
            My.Settings.LedgerTargetDirectory1 = beLedgerTargetDirectory1.Text
            My.Settings.LedgerSourceDirectory3 = beLedgerSourceDirectory3.Text
            My.Settings.LedgerTargetDirectory3 = beLedgerTargetDirectory3.Text
            My.Settings.LedgerSourceDirectory5 = beLedgerSourceDirectory5.Text
            My.Settings.LedgerTargetDirectory5 = beLedgerTargetDirectory5.Text
            My.Settings.LedgerSourceDirectory6 = beLedgerSourceDirectory6.Text
            My.Settings.LedgerTargetDirectory6 = beLedgerTargetDirectory6.Text
            My.Settings.LedgerSourceDirectory7 = beLedgerSourceDirectory7.Text
            My.Settings.LedgerTargetDirectory7 = beLedgerTargetDirectory7.Text
            My.Settings.LedgerSourceDirectory8 = beLedgerSourceDirectory8.Text
            My.Settings.LedgerTargetDirectory8 = beLedgerTargetDirectory8.Text
            My.Settings.LedgerSourceDirectory14 = beLedgerSourceDirectory14.Text
            My.Settings.LedgerTargetDirectory14 = beLedgerTargetDirectory14.Text
            My.Settings.DetraSourceDirectory = beDetraSourceDirectory.Text
            My.Settings.DetraTargetDirectory = beDetraTargetDirectory.Text
            My.Settings.RetenSourceDirectory = beRetenSourceDirectory.Text
            My.Settings.RetenTargetDirectory = beRetenTargetDirectory.Text
            My.Settings.Save()
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Los cambios se aplicaron satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Private Sub PreferencesForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        teMDBFileName.EditValue = My.Settings.MDBFileName
        beMDBDirectory.EditValue = My.Settings.MDBDirectory
        beBankSourceDirectory.EditValue = My.Settings.BankSourceDirectory
        beBankTargetDirectory.EditValue = My.Settings.BankTargetDirectory
        beLedgerSourceDirectory1.EditValue = My.Settings.LedgerSourceDirectory1
        beLedgerTargetDirectory1.EditValue = My.Settings.LedgerTargetDirectory1
        beLedgerSourceDirectory3.EditValue = My.Settings.LedgerSourceDirectory3
        beLedgerTargetDirectory3.EditValue = My.Settings.LedgerTargetDirectory3
        beLedgerSourceDirectory5.EditValue = My.Settings.LedgerSourceDirectory5
        beLedgerTargetDirectory5.EditValue = My.Settings.LedgerTargetDirectory5
        beLedgerSourceDirectory6.EditValue = My.Settings.LedgerSourceDirectory6
        beLedgerTargetDirectory6.EditValue = My.Settings.LedgerTargetDirectory6
        beLedgerSourceDirectory7.EditValue = My.Settings.LedgerSourceDirectory7
        beLedgerTargetDirectory7.EditValue = My.Settings.LedgerTargetDirectory7
        beLedgerSourceDirectory8.EditValue = My.Settings.LedgerSourceDirectory8
        beLedgerTargetDirectory8.EditValue = My.Settings.LedgerTargetDirectory8
        beLedgerSourceDirectory14.EditValue = My.Settings.LedgerSourceDirectory14
        beLedgerTargetDirectory14.EditValue = My.Settings.LedgerTargetDirectory14
        beDetraSourceDirectory.EditValue = My.Settings.DetraSourceDirectory
        beDetraTargetDirectory.EditValue = My.Settings.DetraTargetDirectory
        beRetenSourceDirectory.EditValue = My.Settings.RetenSourceDirectory
        beRetenTargetDirectory.EditValue = My.Settings.RetenTargetDirectory
    End Sub

    Private Sub bbiReset_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiReset.ItemClick
        If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Are you sure to reset?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
            My.Settings.Reset()
            Me.PreferencesForm_Load(sender, e)
        End If
    End Sub

    Private Sub bbiAbrirBD_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiAbrirBD.ItemClick
        Dim oFile As String = GetDBFileName()
        System.Diagnostics.Process.Start(oFile)
    End Sub
End Class