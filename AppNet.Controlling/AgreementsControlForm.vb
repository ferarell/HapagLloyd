Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.Collections

Public Class AgreementsControlForm
    Dim dtSourceHalo, dtResultRates, dtSourceWebFocus As New DataTable
    Dim iPrc As Integer = 0
    Dim oAppService As New AppService.HapagLloydServiceClient
    Dim sTable As String = ""

    Private Sub bbiProcesss_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesss.ItemClick
        Dim aResult As New ArrayList
        Dim dtList As New DataTable
        sTable = "ctr.AgreementsControlling" & rgCargoType.EditValue
        LoadInputValidations()
        If Not vpInputs.Validate Then
            Return
        End If
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            If rgProcessType.SelectedIndex = 2 Then
                Dim dtInvoices As New DataTable
                dtInvoices = LoadExcel(OpenFileDialog1.FileName, "{0}").Tables(0)
                If dtInvoices.Rows.Count > 0 Then
                    For r = 0 To dtInvoices.Rows.Count - 1
                        Dim oRow As DataRow = dtInvoices.Rows(r)
                        aResult.Clear()
                        SplashScreenManager.Default.SetWaitFormDescription("Updating Invoices of Agreements Control")
                        aResult.AddRange(ExecuteSQLNonQuery("EXEC ctr.UpdateInvoiceByBL '" & oRow(0) & "','" & oRow(1) & "'"))
                    Next
                End If
            ElseIf rgProcessType.SelectedIndex = 3 Then
                Dim dtInvoices As New DataTable
                dtInvoices = LoadExcel(OpenFileDialog1.FileName, "{0}").Tables(0)
                If dtInvoices.Rows.Count > 0 Then
                    For r = 0 To dtInvoices.Rows.Count - 1
                        Dim oRow As DataRow = dtInvoices.Rows(r)
                        aResult.Clear()
                        SplashScreenManager.Default.SetWaitFormDescription("Updating Invoices of Agreements Control")
                        aResult.AddRange(ExecuteSQLNonQuery("EXEC ctr.UpdateInvoiceByBL '" & oRow(0) & "','" & oRow(1) & "'"))
                    Next
                End If
            Else
                If LoadDataSources1() Then
                    If dtSourceHalo.Rows.Count > 0 Then
                        If rgProcessType.SelectedIndex = 0 Then
                            dtList = SelectDistinct(dtSourceHalo, "Tipo='" & Mid(rgCargoType.EditValue, 1, 1) & "'", "bl")
                            gcMainData.MainView = GridView4
                            gcMainData.DataSource = dtList
                            Return
                        Else
                            Dim oParams, oValues As New ArrayList
                            oParams.Add("@User")
                            oValues.Add(My.User.Name)
                            SplashScreenManager.Default.SetWaitFormDescription("Updating Rates Agreements Control")
                            aResult.AddRange(oAppService.UpdatingUsingTableAsParameter("ctr.spAgreementControlUpdate", oParams.ToArray, oValues.ToArray, dtSourceHalo))
                        End If
                    End If
                End If
            End If
            SplashScreenManager.CloseForm(False)
            If Convert.ToInt32(aResult(0)) = 1 Then
                bbiShowAll.PerformClick()
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The process has been completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, aResult(1), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
    End Sub

    Friend Function LoadDataSources1() As Boolean
        Dim bResult As Boolean = True
        Dim dtBridge As New DataTable
        dtSourceHalo.Rows.Clear()
        dtResultRates.Rows.Clear()
        dtSourceWebFocus.Rows.Clear()
        Dim dtMainName As String = ""
        For i = 0 To OpenFileDialog1.FileNames.Count - 1
            If OpenFileDialog1.FileNames(i).ToUpper.Contains(".XLS") Then
                SplashScreenManager.Default.SetWaitFormDescription("Loading Data Sources...")
                dtBridge = LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0)
                If dtBridge.Rows.Count = 0 Then
                    Continue For
                End If
                If dtBridge.Rows(0)(6).ToString.Contains({"I", "E"}) Then
                    'HALO (TRAMARSA)
                    SplashScreenManager.Default.SetWaitFormDescription("Loading Data of Halo " & rgCargoType.EditValue & " (Tramarsa)")
                    dtSourceHalo = LoadExcelWC(OpenFileDialog1.FileNames(i), "{0}", "Sucursal IS NOT NULL").Tables(0)
                    'dtSourceTmp1 = LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Select("[Special Product] = 'CTRF' OR [Special Product] = '2PRD'").CopyToDataTable
                    'InsertDataFile1(dtSourceTmp1)
                    Continue For
                End If
            End If
        Next
        Return bResult
    End Function

    Private Sub ProcessDataSources()
        Dim TargetTable As String = "ctr.AgreementsControlling" & IIf(rgCargoType.SelectedIndex = 0, "Import", "Export")
        Dim dtResult As DataTable = ExecuteSQL("SELECT * FROM " & TargetTable & " WHERE BL_TIPO=''").Tables(0)
        Dim dtBookRates As DataTable = ExecuteSQL("SELECT BL_TYPE, RATE_CODE, RATE_AMOUNT FROM ctr.BookRates").Tables(0).Select("BL_TYPE='" & Mid(rgCargoType.EditValue, 1, 1) & "'").CopyToDataTable
        Dim dtCommercialAgreements As New DataTable
        Dim iPos As Integer = 0
        Dim iBoxes As Integer
        SplashScreenManager.Default.SetWaitFormDescription("Combining all data sources...")
        For r = 0 To dtSourceHalo.Rows.Count - 1
            Try
                SplashScreenManager.Default.SetWaitFormDescription("Processing Row " & r.ToString & " de " & (dtSourceHalo.Rows.Count - 1).ToString)
                Dim oRow As DataRow = dtSourceHalo.Rows(r)
                If IsDBNull(dtSourceHalo.Rows(r)(0)) Or dtSourceHalo.Rows(r)(0) = "" Then
                    Continue For
                End If
                If rgCargoType.SelectedIndex = 1 And oRow(6).ToString = "I" Then
                    Continue For
                End If
                If rgCargoType.SelectedIndex = 0 And oRow(6).ToString = "E" Then
                    Continue For
                End If
                iBoxes = 0
                dtResultRates = ExecuteSQL("SELECT * FROM ctr.CalculatedRates WHERE BL='" & oRow("BL") & "'").Tables(0)
                ExecuteSQL("DELETE FROM " & TargetTable & " WHERE BLNO='" & oRow("BL") & "'")
                If dtSourceHalo.Select("BL='" & oRow("BL") & "'").Length > 0 Then
                    iBoxes = dtSourceHalo.Compute("COUNT(Contenedor)", "BL='" & oRow("BL") & "'")
                End If
                iPos = dtResult.Rows.Count
                dtResult.Rows.Add()
                dtResult.Rows(iPos)("BL_TIPO") = oRow("Tipo")
                dtResult.Rows(iPos)("SUCURSAL") = oRow("Sucursal")
                dtResult.Rows(iPos)("BLNO") = oRow("BL")
                dtResult.Rows(iPos)("BOOKING") = GetBookingByBL(oRow("BL"))
                dtResult.Rows(iPos)("NAVE") = oRow("Nave")
                dtResult.Rows(iPos)("VIAJE") = oRow("Viaje")
                dtResult.Rows(iPos)("ETD") = oRow("fecha_zarpe")
                dtResult.Rows(iPos)("FECHA_VB") = oRow("Fecha VB")
                dtResult.Rows(iPos)("BOXES") = iBoxes
                dtResult.Rows(iPos)("RA") = oRow("RA")
                dtResult.Rows(iPos)("LCL") = oRow("LCL")
                dtResult.Rows(iPos)("FECHA_VB_FLAG") = IIf(IsDBNull(oRow("Fecha VB")), 1, 0)
                If rgCargoType.SelectedIndex = 0 Then

                    If oRow("Consignatario").ToString.ToUpper.Contains({"ORDER", "ORDEN", "BANK", "BANCO", "SPEED", "."}) Then
                        dtResult.Rows(iPos)("CLIENTE") = oRow("Notificante")
                    Else
                        dtResult.Rows(iPos)("CLIENTE") = oRow("Consignatario")
                    End If
                Else
                    dtResult.Rows(iPos)("CLIENTE") = oRow("Embarcador")
                End If

                If rgCargoType.SelectedIndex = 0 Then
                    If IsDBNull(oRow("GDCI")) Then
                        oRow("GDCI") = 0
                    End If
                    If IsDBNull(oRow("SACI")) Then
                        oRow("SACI") = 0
                    End If
                    dtResult.Rows(iPos)("LIB_TDI") = dtBookRates.Select("RATE_CODE='TDI'")(0)("RATE_AMOUNT")
                    dtResult.Rows(iPos)("LIB_GDCI") = dtBookRates.Select("RATE_CODE='GDCI'")(0)("RATE_AMOUNT") * iBoxes
                    dtResult.Rows(iPos)("LIB_SACI") = dtBookRates.Select("RATE_CODE='SACI'")(0)("RATE_AMOUNT") * iBoxes
                    dtResult.Rows(iPos)("HAL_TDI") = oRow("TDI")
                    dtResult.Rows(iPos)("HAL_GDCI") = oRow("GDCI") * iBoxes
                    dtResult.Rows(iPos)("HAL_SACI") = oRow("SACI") * iBoxes
                    dtResult.Rows(iPos)("HAL_CONCESION_TDI") = oRow("Concesion_TDI")
                    dtResult.Rows(iPos)("HAL_CONCESION_GDCI") = oRow("Concesion_GDCI")
                    dtResult.Rows(iPos)("HAL_CONCESION_SACI") = oRow("Concesion_SACI")
                Else
                    If IsDBNull(oRow("GDCE")) Then
                        oRow("GDCE") = 0
                    End If
                    If IsDBNull(oRow("SACE")) Then
                        oRow("SACE") = 0
                    End If
                    dtResult.Rows(iPos)("LIB_TDE") = dtBookRates.Select("RATE_CODE='TDE'")(0)("RATE_AMOUNT")
                    dtResult.Rows(iPos)("LIB_GDCE") = dtBookRates.Select("RATE_CODE='GDCE'")(0)("RATE_AMOUNT") * iBoxes
                    dtResult.Rows(iPos)("LIB_SACE") = dtBookRates.Select("RATE_CODE='SACE'")(0)("RATE_AMOUNT") * iBoxes
                    dtResult.Rows(iPos)("HAL_TDE") = oRow("TDE")
                    dtResult.Rows(iPos)("HAL_GDCE") = oRow("GDCE") * iBoxes
                    dtResult.Rows(iPos)("HAL_SACE") = oRow("SACE") * iBoxes
                    dtResult.Rows(iPos)("HAL_CONCESION_TDE") = oRow("Concesion_TDE")
                    dtResult.Rows(iPos)("HAL_CONCESION_GDCE") = oRow("Concesion_GDCE")
                    dtResult.Rows(iPos)("HAL_CONCESION_SACE") = oRow("Concesion_SACE")
                End If
                If dtResultRates.Rows.Count > 0 Then
                    'Dim drAComercial As DataRow = dtResultRates.Rows(f)
                    If rgCargoType.SelectedIndex = 0 Then
                        dtResult.Rows(iPos)("ACM_TDI") = dtResultRates.Compute("SUM(TDI)", "BL='" & oRow("BL") & "'")
                        dtResult.Rows(iPos)("ACM_GDCI") = dtResultRates.Compute("SUM(GDCI)", "BL='" & oRow("BL") & "'")
                        dtResult.Rows(iPos)("ACM_SACI") = dtResultRates.Compute("SUM(SACI)", "BL='" & oRow("BL") & "'")
                        dtResult.Rows(iPos)("ACM_CONCESION_TDI") = dtResultRates.Compute("MAX(Concesion_TDI)", "BL='" & oRow("BL") & "'")
                        dtResult.Rows(iPos)("ACM_CONCESION_GDCI") = dtResultRates.Compute("MAX(Concesion_GDCI)", "BL='" & oRow("BL") & "'")
                        dtResult.Rows(iPos)("ACM_CONCESION_SACI") = dtResultRates.Compute("MAX(Concesion_SACI)", "BL='" & oRow("BL") & "'")
                    Else
                        dtResult.Rows(iPos)("ACM_TDE") = dtResultRates.Compute("SUM(TDE)", "BL='" & oRow("BL") & "'")
                        dtResult.Rows(iPos)("ACM_GDCE") = dtResultRates.Compute("SUM(GDCE)", "BL='" & oRow("BL") & "'")
                        dtResult.Rows(iPos)("ACM_SACE") = dtResultRates.Compute("SUM(SACE)", "BL='" & oRow("BL") & "'")
                        dtResult.Rows(iPos)("ACM_CONCESION_TDE") = dtResultRates.Compute("MAX(Concesion_TDE)", "BL='" & oRow("BL") & "'")
                        dtResult.Rows(iPos)("ACM_CONCESION_GDCE") = dtResultRates.Compute("MAX(Concesion_GDCE)", "BL='" & oRow("BL") & "'")
                        dtResult.Rows(iPos)("ACM_CONCESION_SACE") = dtResultRates.Compute("MAX(Concesion_SACE)", "BL='" & oRow("BL") & "'")
                    End If
                End If
                dtCommercialAgreements = GetRatesHLP(dtResult.Rows(iPos))
                If dtCommercialAgreements.Rows.Count > 0 Then
                    If rgCargoType.SelectedIndex = 0 Then
                        If dtCommercialAgreements.Select("Tarifa='TDI'").Length > 0 Then
                            dtResult.Rows(iPos)("HLP_TDI") = dtCommercialAgreements.Select("Tarifa='TDI'")(0)("Monto")
                        End If
                        If dtCommercialAgreements.Select("Tarifa='GDCI'").Length > 0 Then
                            dtResult.Rows(iPos)("HLP_GDCI") = dtCommercialAgreements.Select("Tarifa='GDCI'")(0)("Monto") * iBoxes
                        End If
                        If dtCommercialAgreements.Select("Tarifa='SACI'").Length > 0 Then
                            dtResult.Rows(iPos)("HLP_SACI") = dtCommercialAgreements.Select("Tarifa='SACI'")(0)("Monto") * iBoxes
                        End If
                    Else
                        If dtCommercialAgreements.Select("Tarifa='TDE'").Length > 0 Then
                            dtResult.Rows(iPos)("HLP_TDE") = dtCommercialAgreements.Select("Tarifa='TDE'")(0)("Monto")
                        End If
                        If dtCommercialAgreements.Select("Tarifa='GDCE'").Length > 0 Then
                            dtResult.Rows(iPos)("HLP_GDCE") = dtCommercialAgreements.Select("Tarifa='GDCE'")(0)("Monto") * iBoxes
                        End If
                        If dtCommercialAgreements.Select("Tarifa='SACE'").Length > 0 Then
                            dtResult.Rows(iPos)("HLP_SACE") = dtCommercialAgreements.Select("Tarifa='SACE'")(0)("Monto") * iBoxes
                        End If
                    End If
                End If
                dtResult.Rows(iPos)("CreatedBy") = My.User.Name
                dtResult.Rows(iPos)("CreatedDate") = Now
                'InsertIntoSQL(TargetTable, dtResult.Rows(iPos))
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "There was an error into data process. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Next
        bbiShowAll.PerformClick()
    End Sub

    Friend Function GetRatesHLP(oRow As DataRow) As DataTable
        Dim dtResult As New DataTable
        Dim sWhere As String = ""
        sWhere = "RateAgreement='" & oRow("RA") & "'"
        dtResult = ExecuteSQL("select * from ctr.viCommercialAgreements where " & sWhere).Tables(0)
        If dtResult.Rows.Count > 0 Then
            Return dtResult
        End If
        sWhere = "LEFT(Cliente,12)='" & Mid(oRow("Cliente"), 1, 12) & "'"
        dtResult = ExecuteSQL("select * from ctr.viCommercialAgreements where " & sWhere).Tables(0)
        If dtResult.Rows.Count > 0 Then
            Return dtResult
        End If

        Return dtResult
    End Function

    Private Function GetBookingByBL(BlNo As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        dtQuery = ExecuteSQL("select Booking from ctr.WebFocus" & rgCargoType.EditValue & " where BLNO='" & BlNo & "'").Tables(0)
        If dtQuery.Rows.Count > 0 Then
            sResult = dtQuery.Rows(0)("Booking")
        End If
        Return sResult
    End Function

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(gcMainData)
    End Sub

    Private Sub beDataSource_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beDataSource.Properties.ButtonClick
        Dim FileNames() As String
        OpenFileDialog1.Filter = "Source File (*.xls*)|*.xls*"
        OpenFileDialog1.FileName = ""
        'OpenFileDialog1.InitialDirectory = My.Settings.SDRDataSourcePath
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            FileNames = OpenFileDialog1.FileNames
            beDataSource.Text = OpenFileDialog1.FileName
        End If
    End Sub

    'Private Sub beDataFileTarget_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs)
    '    SaveFileDialog1.Filter = "Excel File (*.xls*)|*.xls*"
    '    SaveFileDialog1.FileName = ""
    '    'nSaveFileDialog1.InitialDirectory = My.Settings.SDRDataTargetPath
    '    If SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
    '        beDataFileTarget.Text = SaveFileDialog1.FileName & IIf(beDataFileTarget.Text.Contains(".xlsx"), "", ".xlsx")
    '    End If
    'End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub LoadInputValidations()
        Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

        containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
        containsValidationRule.ErrorText = "Assign value."
        containsValidationRule.ErrorType = ErrorType.Critical

        Dim customValidationRule As New CustomValidationRule()
        customValidationRule.ErrorText = "Required value."
        customValidationRule.ErrorType = ErrorType.Critical

        vpInputs.SetValidationRule(Me.beDataSource, Nothing)
        vpInputs.SetValidationRule(Me.beDataSource, customValidationRule)

    End Sub

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiShowAll.ItemClick
        Dim dtQuery As New DataTable
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get all data table rows")
        If rgCargoType.SelectedIndex = 0 Then
            dtQuery = ExecuteSQL("EXEC ctr.spGetAgreementsControllingImport '" & Format(deDateFrom.EditValue, "yyyyMMdd") & "','" & Format(deDateTo.EditValue, "yyyyMMdd") & "'").Tables(0)
            gcMainData.MainView = GridView1
        Else
            dtQuery = ExecuteSQL("EXEC ctr.spGetAgreementsControllingExport '" & Format(deDateFrom.EditValue, "yyyyMMdd") & "','" & Format(deDateTo.EditValue, "yyyyMMdd") & "'").Tables(0)
            gcMainData.MainView = GridView3
        End If
        gcMainData.DataSource = dtQuery

        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub AgreementsControlForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GridView1.RestoreLayoutFromRegistry(Directory.GetCurrentDirectory)
        deDateFrom.EditValue = Now.AddDays(-30)
        deDateTo.EditValue = Now

    End Sub

    Private Sub AgreementsControlForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        GridView1.ActiveFilter.Clear()
        GridView1.SaveLayoutToRegistry(Directory.GetCurrentDirectory)
    End Sub

    Private Sub GridView1_RowCellStyle(ByVal sender As Object, ByVal e As RowCellStyleEventArgs) Handles GridView1.RowCellStyle, GridView3.RowCellStyle
        Dim View As GridView = sender
        If (e.RowHandle >= 0) Then
            If e.Column.FieldName = "DIFF_BOXES" Then
                Dim C10 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("DIFF_BOXES"))
                If IsDBNull(C10) Or C10 = "" Then
                    C10 = "0"
                End If
                If CInt(C10) <> 0 Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If

            End If
            If e.Column.FieldName = "HLP_TD" & Mid(rgCargoType.EditValue, 1, 1) Then
                Dim C25 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("HAL_TD" & Mid(rgCargoType.EditValue, 1, 1)))
                Dim C33 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("HLP_TD" & Mid(rgCargoType.EditValue, 1, 1)))
                If IsDBNull(C25) Or C25 = "" Then
                    'C25 = "0"
                    Return
                End If
                'If CInt(C25) <> 0 Then
                'If IsDBNull(C33) Or C33 = "" Then
                '    C33 = "0"
                'End If
                If CInt(C25) <> CInt(C33) Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If
                'End If
            End If
            If e.Column.FieldName = "HLP_GDC" & Mid(rgCargoType.EditValue, 1, 1) Then
                Dim C26 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("HAL_GDC" & Mid(rgCargoType.EditValue, 1, 1)))
                Dim C34 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("HLP_GDC" & Mid(rgCargoType.EditValue, 1, 1)))
                If IsDBNull(C26) Or C26 = "" Then
                    'C26 = "0"
                    Return
                End If
                'If CInt(C26) <> 0 Then
                'If IsDBNull(C34) Or C34 = "" Then
                '    C34 = "0"
                'End If
                If CInt(C26) <> CInt(C34) Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If
                'End If
            End If
            If e.Column.FieldName = "HLP_SAC" & Mid(rgCargoType.EditValue, 1, 1) Then
                Dim C27 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("HAL_SAC" & Mid(rgCargoType.EditValue, 1, 1)))
                Dim C35 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("HLP_SAC" & Mid(rgCargoType.EditValue, 1, 1)))
                If IsDBNull(C27) Or C27 = "" Then
                    'C27 = "0"
                    Return
                End If
                'If CInt(C27) <> 0 Then
                'If IsDBNull(C35) Or C35 = "" Then
                '    C35 = "0"
                'End If
                If CInt(C27) <> CInt(C35) Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If
                'End If
            End If
            If e.Column.FieldName = "HLP_GDBB" & Mid(rgCargoType.EditValue, 1, 1) Then
                Dim C28 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("HAL_GDBB" & Mid(rgCargoType.EditValue, 1, 1)))
                Dim C36 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("HLP_GDBB" & Mid(rgCargoType.EditValue, 1, 1)))
                If IsDBNull(C28) Or C28 = "" Then
                    'C28 = "0"
                    Return
                End If
                'If CInt(C28) <> 0 Then
                'If IsDBNull(C36) Or C36 = "" Then
                '    C36 = "0"
                'End If
                If CInt(C28) <> CInt(C36) Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If
                'End If
            End If
            'Dim C1 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("TipoCliente"))
            'Concesion = View.GetRowCellDisplayText(e.RowHandle, View.Columns("HAL_CONCESION_TD" & Mid(rgCargoType.EditValue, 1, 1))) & View.GetRowCellDisplayText(e.RowHandle, View.Columns("HAL_CONCESION_GDC" & Mid(rgCargoType.EditValue, 1, 1))) & View.GetRowCellDisplayText(e.RowHandle, View.Columns("HAL_CONCESION_SAC" & Mid(rgCargoType.EditValue, 1, 1)))
            'If C1 = "R" And Concesion = "" Then
            '    e.Appearance.BackColor = Color.LightBlue
            '    e.Appearance.BackColor2 = Color.LightGray
            'End If
        End If
    End Sub

    Private Sub bbiUpdate_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiUpdate.ItemClick
        Dim aReturn As New ArrayList
        If gcMainData.FocusedView.Name = "GridView2" Or gcMainData.MainView.RowCount = 0 Then
            Return
        End If
        If DevExpress.XtraEditors.XtraMessageBox.Show("Are you sure you want to update the assigned invoice(s)? ", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
            Return
        End If
        For r = 0 To GridView1.RowCount - 1
            Dim oRow As DataRow = GridView1.GetDataRow(r)
            If oRow("INVOICE") <> "" Then
                aReturn.AddRange(ExecuteSQLNonQuery("EXEC ctr.UpdateInvoiceByBL '" & oRow("BLNO") & "','" & oRow("INVOICE") & "'"))
            End If
        Next
    End Sub

    Private Sub bbiViewStandard_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiViewStandard.ItemClick
        If rgCargoType.SelectedIndex = 0 Then
            gcMainData.MainView = GridView1
        Else
            gcMainData.MainView = GridView3
        End If
        'GridView1.PopulateColumns()
        gcMainData.MainView.RefreshData()
    End Sub

    Private Sub bbiViewSmall_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiViewSmall.ItemClick
        gcMainData.MainView = GridView2
        'GridView1.PopulateColumns()
    End Sub

    Private Sub bbiDelete_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiDelete.ItemClick
        If gcMainData.FocusedView.Name <> "GridView1" Or gcMainData.FocusedView.RowCount = 0 Then
            Return
        End If
        Dim aReturn As New ArrayList
        If DevExpress.XtraEditors.XtraMessageBox.Show("Are you sure you want to delete the selected record? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
            Dim oGridView As New GridView
            If GridView1.IsFocusedView Then
                oGridView = GridView1
            Else
                oGridView = GridView2
            End If
            oGridView.ActiveFilterEnabled = False
            Try
                oGridView.DeleteRow(oGridView.FocusedRowHandle)
                aReturn.AddRange(ExecuteSQLNonQuery("DELETE FROM " & sTable & " where BLNO = '" & oGridView.GetFocusedRowCellValue("BLNO") & "'"))
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show("Ocurrió un error al eliminar el registro.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
            oGridView.RefreshData()
            oGridView.ActiveFilterEnabled = True
            DevExpress.XtraEditors.XtraMessageBox.Show("El registro ha sido eliminado satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub


End Class