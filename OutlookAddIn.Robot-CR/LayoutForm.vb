Imports DevExpress.XtraRichEdit
Imports DevExpress.XtraRichEdit.Export
Imports DevExpress.XtraRichEdit.Services
Imports System.Windows.Forms
Imports System.Data
Imports System.IO
Imports DevExpress.XtraEditors

Public Class LayoutForm
    Dim oDataAccess As New DataAccess
    Dim dtConfiguration, dtMessageConfig, dtChild As New System.Data.DataTable
    Dim FileName As String = ""
    Dim oRichTextEdit As New DevExpress.XtraRichEdit.RichEditControl

    Private Sub SettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        rgResponse.SelectedIndex = 0
        rgFieldName.SelectedIndex = 0
        SplitContainerControl2.Panel2.Visible = False
        GridView1.Columns("NewsValidityFrom").Visible = False
        GridView1.Columns("NewsValidityTo").Visible = False
        If Not IO.File.Exists(My.Settings.DBFileName) Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The database was not found, please check the setting option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        LoadConfiguration()
        GetMessageConfig()
    End Sub

    Private Sub FillRichTextControl(drConfig As DataRow)
        If GridView1.FocusedRowHandle < 0 Then
            gcMainData = Nothing
            Return
        End If
        recText.RtfText = Nothing
        If Not IsDBNull(drConfig(rgFieldName.EditValue)) Then
            recText.HtmlText = drConfig(rgFieldName.EditValue)
        End If
        SplitContainerControl2.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1
    End Sub

    Private Sub LoadConfiguration()
        dtConfiguration.Rows.Clear()
        dtConfiguration = oDataAccess.ExecuteAccessQuery("SELECT * FROM " & My.Settings.ConfigTableName).Tables(0)
        If dtConfiguration.Rows.Count > 0 Then
            gcMainData.DataSource = dtConfiguration
            FillRichTextControl(dtConfiguration.Rows(GridView1.FocusedRowHandle))
        End If
    End Sub

    Private Sub GetMessageConfig()
        If dtConfiguration.Rows.Count = 0 Then
            Return
        End If
        dtMessageConfig = dtConfiguration.Select("ResponseType=" & rgResponse.SelectedIndex.ToString).CopyToDataTable
        If dtMessageConfig.Rows.Count > 0 Then
            gcMainData.DataSource = dtMessageConfig
            GridView1.BestFitColumns()
            FillRichTextControl(dtMessageConfig.Rows(GridView1.FocusedRowHandle))
        End If
    End Sub

    Private Sub bbiSave_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSave.ItemClick
        Validate()
        Try
            If Not UpdateConfiguration() Then
                XtraMessageBox.Show(Me.LookAndFeel, "An error occurred while saving changes. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            XtraMessageBox.Show(Me.LookAndFeel, "Changes were saved successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            XtraMessageBox.Show(Me.LookAndFeel, "An error occurred while saving changes. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        bbiRefresh.PerformClick()
    End Sub

    Friend Function UpdateConfiguration() As Boolean
        Dim bResult As Boolean = True
        Dim oCondition, oValues As String
        Try
            Dim drView1 As DataRow = GridView1.GetFocusedDataRow

            oCondition = "IdConfiguration=" & drView1("IdConfiguration").ToString & " AND ResponseType=" & drView1("ResponseType").ToString

            If Not IsDBNull(drView1("Identifier")) Then
                oValues = "[Identifier]='" & drView1("Identifier").ToString & "'"
            End If
            If Not IsDBNull(drView1("ResponseMailSubject")) Then
                oValues = "[ResponseMailSubject]='" & drView1("ResponseMailSubject").ToString & "'"
            End If
            If Not IsDBNull(drView1("Description")) Then
                oValues += ", " & "[Description]='" & drView1("Description").ToString & "'"
            End If
            If Not IsDBNull(drView1("Header")) Then
                oValues += ", " & "Header='" & Replace(drView1("Header"), "'", "") & "'"
            End If
            If Not IsDBNull(drView1("Body")) Then
                oValues += ", " & "[Body]='" & Replace(drView1("Body"), "'", "") & "'"
            End If
            If Not IsDBNull(drView1("Signature")) Then
                oValues += ", " & "[Signature]='" & Replace(drView1("Signature"), "'", "") & "'"
            End If
            If Not IsDBNull(drView1("News")) Then
                oValues += ", " & "[News]='" & Replace(drView1("News"), "'", "") & "'"
            End If
            If Not IsDBNull(drView1("QuerySQL")) Then
                oValues += ", " & "[QuerySQL]='" & drView1("QuerySQL").ToString & "'"
            End If
            If IsDBNull(drView1("NewsValidityFrom")) Then
                oValues += ", " & "[NewsValidityFrom]=NULL"
            Else
                oValues += ", " & "[NewsValidityFrom]='" & Format(drView1("NewsValidityFrom"), "yyyy-MM-dd") & "'"
            End If
            If IsDBNull(drView1("NewsValidityTo")) Then
                oValues += ", " & "[NewsValidityTo]=NULL"
            Else
                oValues += ", " & "[NewsValidityTo]='" & Format(drView1("NewsValidityTo"), "yyyy-MM-dd") & "'"
            End If

            If drView1.RowState = DataRowState.Added Then
                bResult = oDataAccess.InsertIntoAccess(My.Settings.ConfigTableName, drView1)
            Else
                bResult = oDataAccess.UpdateAccess(My.Settings.ConfigTableName, oCondition, oValues)
            End If

            For r = 0 To GridView3.RowCount - 1
                Dim drView3 As DataRow = GridView3.GetDataRow(r)
                If drView3("SubjectIdentifier").ToString = "" Then
                    Continue For
                End If
                drView3("IdConfiguration") = drView1("IdConfiguration")
                oCondition = "IdConfiguration=" & drView3("IdConfiguration").ToString & " AND SubjectIdentifier='" & drView3("SubjectIdentifier").ToString & "'"
                oValues = "[SubjectIdentifier]='" & drView3("SubjectIdentifier").ToString & "'"
                If drView3.RowState = DataRowState.Added Then
                    bResult = oDataAccess.InsertIntoAccess("InputSubject", drView3)
                Else
                    bResult = oDataAccess.UpdateAccess("InputSubject", oCondition, oValues)
                End If
            Next

        Catch ex As Exception
            bResult = False
        End Try
        Return bResult
    End Function

    Private Sub bbiRefresh_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiRefresh.ItemClick
        gcMainData.EmbeddedNavigator.Buttons.CustomButtons.Item(0).Enabled = True
        gcMainData.EmbeddedNavigator.Buttons.CustomButtons.Item(1).Enabled = True
        gcSubjects.EmbeddedNavigator.Buttons.CustomButtons.Item(0).Enabled = True
        gcSubjects.EmbeddedNavigator.Buttons.CustomButtons.Item(1).Enabled = True
        LoadConfiguration()
        GetMessageConfig()
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub rgResponse_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rgResponse.SelectedIndexChanged
        rgFieldName.SelectedIndex = 0
        rgFieldName.Enabled = True
        recText.Enabled = True
        rgFieldName.Properties.Items(4).Enabled = False

        If rgResponse.EditValue = "I" Then
            'rgFieldName.Enabled = False
            'recText.Enabled = False
        End If
        If rgResponse.EditValue = "C" Then
            rgFieldName.Properties.Items(4).Enabled = True
            rgFieldName.EditValue = "QuerySQL"
        End If
        GetMessageConfig()
    End Sub

    Private Sub rgFieldName_Properties_EditValueChanging(sender As Object, e As Controls.ChangingEventArgs) Handles rgFieldName.Properties.EditValueChanging
        If rgFieldName.EditValue = "QuerySQL" Then
            GridView1.SetFocusedRowCellValue(rgFieldName.EditValue, recText.Text)
        Else
            GridView1.SetFocusedRowCellValue(rgFieldName.EditValue, recText.HtmlText)
        End If
    End Sub

    Private Sub bbiMessagePreview_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiMessagePreview.ItemClick
        Dim oMailItem As New CreateMailItem
        oMailItem.mailSubject = GridView1.GetFocusedRowCellValue("ResponseMailSubject")
        oMailItem.mailHtmlBody.AppendText(GridView1.GetFocusedRowCellValue("Header") & "<br>")
        oMailItem.mailHtmlBody.AppendText(GridView1.GetFocusedRowCellValue("Body") & "<br>")
        oMailItem.mailHtmlBody.AppendText(GridView1.GetFocusedRowCellValue("Signature") & "<br>")
        If oMailItem.ActiveNotice(GridView1.GetFocusedDataRow) Then
            oMailItem.mailHtmlBody.AppendText(GridView1.GetFocusedRowCellValue("News") & "<br>")
        End If
        oMailItem.CreateCustomMessage("Display", False, False)
    End Sub

    Private Sub GridView1_CellValueChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs) Handles GridView1.CellValueChanged
        GridView1.UpdateCurrentRow()
        If GridView1.FocusedRowHandle >= 0 Then
            Dim oRow As DataRow = GridView1.GetDataRow(GridView1.FocusedRowHandle)
            oRow("IdConfiguration") = IIf(oRow("IdConfiguration").ToString = "", dtConfiguration.Compute("MAX(IdConfiguration)", "") + 1, oRow("IdConfiguration"))
            oRow("ResponseType") = rgResponse.SelectedIndex
        End If
    End Sub

    Private Sub gcMainData_EmbeddedNavigator_ButtonClick(sender As Object, e As NavigatorButtonClickEventArgs) Handles gcMainData.EmbeddedNavigator.ButtonClick
        Dim bError As Boolean = False
        If e.Button.Hint = "New Row" Then
            GridView1.AddNewRow()
            e.Button.Enabled = False
            gcMainData.EmbeddedNavigator.Buttons.CustomButtons.Item(1).Enabled = False
            LoadInputSubject()
        End If
        If e.Button.Hint = "Remove Row" Then
            If XtraMessageBox.Show("Are you sure you want to delete this row?", "Confirmation", MessageBoxButtons.YesNo) = DialogResult.No Then
                Return
            End If
            If GridView1.FocusedRowHandle >= 0 Then
                Dim oRow As DataRow = GridView1.GetDataRow(GridView1.FocusedRowHandle)
                If Not oDataAccess.ExecuteAccessNonQuery("DELETE FROM " & My.Settings.ConfigTableName & " WHERE IdConfiguration=" & oRow("IdConfiguration").ToString & " AND ResponseType=" & oRow("ResponseType").ToString) Then
                    bError = True
                End If
                If Not oDataAccess.ExecuteAccessNonQuery("DELETE FROM InputSubject WHERE IdConfiguration=" & oRow("IdConfiguration").ToString) Then
                    bError = True
                End If
                If bError Then
                    XtraMessageBox.Show(Me.LookAndFeel, "An error occurred while saving changes. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
                XtraMessageBox.Show(Me.LookAndFeel, "Changes were saved successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
            e.Button.Enabled = True
            bbiRefresh.PerformClick()
        End If
    End Sub

    Private Sub gcSubjects_EmbeddedNavigator_ButtonClick(sender As Object, e As NavigatorButtonClickEventArgs) Handles gcSubjects.EmbeddedNavigator.ButtonClick
        If e.Button.Hint = "New Row" Then
            GridView3.AddNewRow()
            gcSubjects.EmbeddedNavigator.Buttons.CustomButtons.Item(1).Enabled = False
        End If
        If e.Button.Hint = "Remove Row" Then
            If XtraMessageBox.Show("Are you sure you want to delete this row?", "Confirmation", MessageBoxButtons.YesNo) = DialogResult.No Then
                Return
            End If
            If GridView3.FocusedRowHandle >= 0 Then
                Dim oRow As DataRow = GridView3.GetDataRow(GridView3.FocusedRowHandle)
                If Not oDataAccess.ExecuteAccessNonQuery("DELETE FROM InputSubject WHERE IdConfiguration=" & oRow("IdConfiguration").ToString & " AND SubjectIdentifier='" & oRow("SubjectIdentifier").ToString & "'") Then
                    XtraMessageBox.Show(Me.LookAndFeel, "An error occurred while saving changes. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Else
                    XtraMessageBox.Show(Me.LookAndFeel, "Changes were saved successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
            e.Button.Enabled = True
            bbiRefresh.PerformClick()
        End If

    End Sub

    Private Sub recText_TextChanged(sender As Object, e As EventArgs) Handles recText.TextChanged
        If rgFieldName.EditValue = "QuerySQL" Then
            GridView1.SetFocusedRowCellValue(rgFieldName.EditValue, recText.Text)
        Else
            GridView1.SetFocusedRowCellValue(rgFieldName.EditValue, recText.HtmlText)
        End If
    End Sub

    Private Sub GridView1_DataSourceChanged(sender As Object, e As EventArgs) Handles GridView1.DataSourceChanged
        LoadInputSubject()
    End Sub

    Private Sub rgOleFieldName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rgFieldName.SelectedIndexChanged
        GridView1.Columns("NewsValidityTo").Visible = False
        GridView1.Columns("NewsValidityFrom").Visible = False
        If dtMessageConfig.Rows.Count > 0 Then
            FillRichTextControl(GridView1.GetFocusedDataRow)
        End If
        If rgFieldName.SelectedIndex = 3 Then
            GridView1.Columns("NewsValidityTo").Visible = True
            GridView1.Columns("NewsValidityFrom").Visible = True
        End If
    End Sub

    Private Sub GridView1_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView1.FocusedRowChanged
        If e.FocusedRowHandle < 0 Or dtMessageConfig.Rows.Count = 0 Then
            Return
        End If
        If dtConfiguration.Rows.Count > 0 Then
            LoadInputSubject()
            FillRichTextControl(dtMessageConfig.Rows(GridView1.FocusedRowHandle))
        End If
    End Sub

    Private Sub LoadInputSubject()
        Dim dtInputSubject As New DataTable
        If GridView1.FocusedRowHandle < 0 Or GridView1.GetFocusedRowCellValue("IdConfiguration") Is Nothing Or IsDBNull(GridView1.GetFocusedRowCellValue("IdConfiguration")) Then
            dtInputSubject = oDataAccess.ExecuteAccessQuery("SELECT * FROM InputSubject WHERE IdConfiguration = -1").Tables(0)
        Else
            dtInputSubject = oDataAccess.ExecuteAccessQuery("SELECT * FROM InputSubject WHERE IdConfiguration = " & GridView1.GetFocusedRowCellValue("IdConfiguration").ToString).Tables(0)
        End If
        gcSubjects.DataSource = dtInputSubject
    End Sub

End Class