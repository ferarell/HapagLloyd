Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.XtraEditors

Public Class CommodityByShipmentWcfForm
    Dim dtResult As New DataTable
    Dim oAppService As New AppService.HapagLloydServiceClient

    Private Sub beDataFileTarget_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs)
        OpenFileDialog2.Filter = "Excel Files (*.xls*)|*.xls*"
        OpenFileDialog2.FileName = ""
        'OpenFileDialog1.InitialDirectory = IIf(My.Settings.LedgerSourceDirectory <> "", My.Settings.LedgerSourceDirectory, "")
    End Sub

    Private Sub bbiProcesss_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiImport.ItemClick
        LoadValidations()
        If Not vpInputs.Validate Then
            Return
        End If
        Dim dtQuery As New DataTable
        Dim iPos As Integer = 0
        dtResult = oAppService.ExecuteSQL("SELECT * FROM tck.CommodityShipment WHERE Booking='#'").Tables(0)
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            For i = 0 To OpenFileDialog1.FileNames.Count - 1
                'SplashScreenManager.Default.SetWaitFormDescription("Loading Data Sources (File " & (i + 1).ToString & " of " & OpenFileDialog1.FileNames.Count.ToString & ")")
                If OpenFileDialog1.FileNames(i).ToUpper.Contains(".XLS") Then
                    'WEBFOCUS
                    If LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Rows.Count > 0 Then
                        dtQuery = QueryExcel(OpenFileDialog1.FileNames(i), "SELECT F6, F7, F8, F9, F10 FROM [AdhocRequest$] WHERE F1 IS NOT NULL").Tables(0)
                        If dtQuery.Rows.Count > 0 Then
                            For r = 0 To dtQuery.Rows.Count - 1
                                SplashScreenManager.Default.SetWaitFormDescription("Loading Data Source (Row " & (r + 1).ToString & " of " & dtQuery.Rows.Count.ToString & ")")
                                If IsDBNull(dtQuery.Rows(r)(0)) Then
                                    Continue For
                                End If
                                If Not IsNumeric(dtQuery.Rows(r)(0)) Then
                                    Continue For
                                End If
                                If dtQuery.Rows(r)(0).ToString.ToUpper = "N/A" Then
                                    Continue For
                                End If
                                If oAppService.ExecuteSQL("SELECT * FROM tck.CommodityShipment WHERE Booking='" & dtQuery.Rows(r)(0).ToString & "' AND HSCode='" & dtQuery.Rows(r)(1).ToString & "'").Tables(0).Rows.Count > 0 Then
                                    Continue For
                                End If
                                dtResult.Rows.Add()
                                iPos = dtResult.Rows.Count - 1
                                dtResult.Rows(iPos)(0) = dtQuery.Rows(r)(0)
                                dtResult.Rows(iPos)(1) = dtQuery.Rows(r)(1)
                                dtResult.Rows(iPos)(2) = dtQuery.Rows(r)(2)
                                dtResult.Rows(iPos)(3) = dtQuery.Rows(r)(3)
                                dtResult.Rows(iPos)(4) = dtQuery.Rows(r)(4)
                                dtResult.Rows(iPos)(5) = My.User.Name
                                dtResult.Rows(iPos)(6) = Today
                                InsertIntoAccess("CommodityShipment", dtResult.Rows(iPos))
                            Next
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(gcMainData)
    End Sub

    Private Sub beDataSource_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beDataSource.Properties.ButtonClick
        Dim FileNames() As String
        OpenFileDialog1.Filter = "FIS Source Files (*.xls*)|*.xls*"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.InitialDirectory = IIf(My.Settings.DataTargetPath <> "", My.Settings.DataTargetPath, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            FileNames = OpenFileDialog1.FileNames
            beDataSource.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub CommodityByShipmentForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        bbiExport.Enabled = False
        GridView1.RestoreLayoutFromRegistry(Directory.GetCurrentDirectory)
    End Sub

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSearch.ItemClick
        Dim dtQuery As New DataTable
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            dtQuery = oAppService.ExecuteSQL("SELECT * FROM tck.CommodityShipment").Tables(0)
            gcMainData.DataSource = dtQuery
            GridView1.BestFitColumns()
            SplashScreenManager.CloseForm(False)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
        End Try
    End Sub

    Private Sub LoadValidations()
        Validate()
        Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

        containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
        containsValidationRule.ErrorText = "Assign value."
        containsValidationRule.ErrorType = ErrorType.Critical

        Dim customValidationRule As New CustomValidationRule()
        customValidationRule.ErrorText = "Required value."
        customValidationRule.ErrorType = ErrorType.Critical

        vpInputs.SetValidationRule(Me.beDataSource, customValidationRule)
        'vpInputs.SetValidationRule(Me.beDataFileTarget, customValidationRule)

    End Sub

    'Private Sub RepositoryItemHyperLinkEdit1_Click(sender As Object, e As EventArgs)
    '    Dim TrendForm As New GraphicTrendForm
    '    TrendForm.pBooking = GridView1.GetFocusedRowCellValue("BOOKING")
    '    TrendForm.pContainer = GridView1.GetFocusedRowCellValue("CONTAINER")
    '    TrendForm.ShowDialog()
    'End Sub

    'Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
    '    Dim info As GridViewInfo = TryCast(GridView1.GetViewInfo(), GridViewInfo)
    '    Dim GridRowInfo As GridRowInfo = info.GetGridRowInfo(GridView1.FocusedRowHandle)
    '    bbiShowAll.PerformClick()
    '    GridView1.MoveBy(GridRowInfo.RowHandle)
    'End Sub

    'Private Sub beiRefresh_EditValueChanged(sender As Object, e As EventArgs) Handles beiRefresh.EditValueChanged
    '    Timer1.Enabled = beiRefresh.EditValue
    'End Sub

    Private Sub DataSourceForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        GridView1.ActiveFilter.Clear()
        GridView1.SaveLayoutToRegistry(Directory.GetCurrentDirectory)
        My.Settings.CustomDataSourceFilter = GridView1.ActiveFilterString
        My.Settings.Save()
    End Sub

    Private Sub GridView2_FocusedRowChanged_1(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView1.FocusedRowChanged
        Dim dgrItem As DataRow = GridView1.GetDataRow(e.FocusedRowHandle)
        ucAuditPanel.CreatedBy = Nothing
        ucAuditPanel.CreatedDate = Nothing
        ucAuditPanel.UpdatedBy = Nothing
        ucAuditPanel.UpdatedDate = Nothing
        If Not dgrItem Is Nothing Then
            If Not IsDBNull(dgrItem("CreatedBy")) Then
                ucAuditPanel.CreatedBy = dgrItem("CreatedBy")
                ucAuditPanel.CreatedDate = dgrItem("CreatedDate")
            End If
            If Not IsDBNull(dgrItem("UpdatedBy")) Then
                ucAuditPanel.UpdatedBy = dgrItem("UpdatedBy")
                ucAuditPanel.UpdatedDate = dgrItem("UpdatedDate")
            End If
            ucAuditPanel.pnlAuditoria.Refresh()
        End If
    End Sub

End Class