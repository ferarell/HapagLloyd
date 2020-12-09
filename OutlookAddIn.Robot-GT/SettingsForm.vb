Imports DevExpress.XtraRichEdit
Imports DevExpress.XtraRichEdit.Export
Imports DevExpress.XtraRichEdit.Services
Imports System.Windows.Forms
Imports System.Data
Imports System.IO
Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.DXErrorProvider

Public Class SettingsForm
    Dim oDataAccess As New DataAccess
    Dim dtConfiguration, dtOriginConfig, dtChild As New System.Data.DataTable
    Dim FileName As String = ""

    Private Sub SettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If IO.File.Exists(My.Settings.DBFileName) Then
            beDatabase.Text = My.Settings.DBFileName
            'lueConfigTable.EditValue = My.Settings.ConfigTableName
            teConfigTable.EditValue = My.Settings.ConfigTableName
            teUserName.EditValue = My.Settings.DBUserName
            tePassword.EditValue = My.Settings.DBPassword
        End If
        For Each cnt In DevExpress.Skins.SkinManager.Default.Skins
            lbcEstilos.Items.Add(cnt.SkinName)
        Next
        lbcEstilos.SelectedValue = DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveLookAndFeel.ActiveSkinName
        If Not My.Settings.GetPreviousVersion("DBFileName") Is Nothing Then
            My.Settings.Upgrade()
        End If
        beAttachedFilePath.EditValue = My.Settings.AttachedFilePath
        beLogFilePath.EditValue = My.Settings.LogFilePath
        beProcessFilePath.EditValue = My.Settings.ProcessFilePath
    End Sub

    Private Sub lbcEstilos_Click(sender As Object, e As EventArgs) Handles lbcEstilos.Click
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("")
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(lbcEstilos.Text)
    End Sub

    Private Sub beDatabase_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beDatabase.Properties.ButtonClick
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beDatabase.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub beAttachedFilePath_Properties_ButtonClick(sender As Object, e As Controls.ButtonPressedEventArgs) Handles beAttachedFilePath.Properties.ButtonClick
        If XtraFolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            beAttachedFilePath.EditValue = XtraFolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub beLogFilePath_Properties_ButtonClick(sender As Object, e As Controls.ButtonPressedEventArgs) Handles beLogFilePath.Properties.ButtonClick
        If XtraFolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            beLogFilePath.EditValue = XtraFolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub beProcessFilePath_ButtonClick(sender As Object, e As Controls.ButtonPressedEventArgs) Handles beProcessFilePath.ButtonClick
        If XtraFolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            beProcessFilePath.EditValue = XtraFolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub bbiSave_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSave.ItemClick
        If Not vpInputs.Validate Then
            Return
        End If
        Try
            My.Settings.DBFileName = beDatabase.Text
            My.Settings.ConfigTableName = teConfigTable.Text
            'My.Settings.ConfigTableName = lueConfigTable.EditValue.ToString
            My.Settings.LookAndFeel = DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveLookAndFeel.ActiveSkinName
            My.Settings.DBUserName = teUserName.Text
            My.Settings.DBPassword = tePassword.Text
            My.Settings.AttachedFilePath = beAttachedFilePath.Text
            My.Settings.LogFilePath = beLogFilePath.Text
            My.Settings.ProcessFilePath = beProcessFilePath.Text
            My.Settings.Save()
            If Not IO.File.Exists(beDatabase.Text.Trim) Then
                XtraMessageBox.Show(Me.LookAndFeel, "Database file not exists. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            If oDataAccess.ExecuteAccessQuery("SELECT * FROM " & teConfigTable.Text.Trim).Tables.Count = 0 Then
                XtraMessageBox.Show(Me.LookAndFeel, "The table of configuration not exists in database. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            XtraMessageBox.Show(Me.LookAndFeel, "An error occurred while saving changes. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            XtraMessageBox.Show(Me.LookAndFeel, "Changes were saved successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub bbiRefresh_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiRefresh.ItemClick

    End Sub

    Private Sub lueConfigTable_Enter(sender As Object, e As EventArgs) Handles lueConfigTable.Enter
        Dim dtQuery As New DataTable
        dtQuery = oDataAccess.ExecuteAccessQuery("SELECT * FROM ConfigTableListQry").Tables(0)
        lueConfigTable.Properties.DataSource = dtQuery
        lueConfigTable.Properties.DisplayMember = "table_name"
        lueConfigTable.Properties.ValueMember = "table_name"
    End Sub

    Private Sub bbiOpenDB_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiOpenDB.ItemClick
        Dim oFile As String = My.Settings.DBFileName 'GetDBFileName()
        System.Diagnostics.Process.Start(oFile)
    End Sub

    Private Sub SettingsForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not IO.File.Exists(beDatabase.Text) Then
            DialogResult = DialogResult.No
        End If
    End Sub

    Private Sub LoadInputValidations()
        Validate()
        Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

        containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
        containsValidationRule.ErrorText = "Assign value."
        containsValidationRule.ErrorType = ErrorType.Critical

        Dim customValidationRule As New CustomValidationRule()
        customValidationRule.ErrorText = "Required value."
        customValidationRule.ErrorType = ErrorType.Critical

        vpInputs.SetValidationRule(Me.beDatabase, customValidationRule)
        vpInputs.SetValidationRule(Me.teConfigTable, customValidationRule)

    End Sub

End Class