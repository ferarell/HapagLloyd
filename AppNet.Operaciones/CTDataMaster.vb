Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo

Public Class CTDataMaster
    Dim dsDataTarget As New dsMain
    Dim dtContainerList, dtNewDataCT, dtResult, dtVoyage, dtVoyageTS, dtSourceFile1, dtSourceFile2, dtSourceFile3 As New DataTable
    'Dim ContainerNumber As String = ""
    Dim MaxTemp As Decimal = My.Settings.MaxTemp
    'Friend oFunctions As New NetStore.CommonObjects

    Private Sub beDataFileTarget_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beDataFileTarget.Properties.ButtonClick
        OpenFileDialog2.Filter = "Excel Files (*.xls*)|*.xls*"
        OpenFileDialog2.FileName = ""
        'OpenFileDialog1.InitialDirectory = IIf(My.Settings.LedgerSourceDirectory <> "", My.Settings.LedgerSourceDirectory, "")
        If OpenFileDialog2.ShowDialog() = DialogResult.OK Then
            beDataFileTarget.Text = OpenFileDialog2.FileName
        End If
    End Sub

    Private Sub bbiProcesss_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesss.ItemClick
        If Not vpInputs.Validate Then
            Return
        End If
        Dim dtQuery As New DataTable
        Dim dtSourceTmp1, dtSourceTmp2, dtSourceTmp3 As New DataTable
        dtSourceFile1.Rows.Clear()
        dtSourceFile2.Rows.Clear()
        dtSourceFile3.Rows.Clear()
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            Dim dtMainName As String = ""
            For i = 0 To OpenFileDialog1.FileNames.Count - 1
                If OpenFileDialog1.FileNames(i).ToUpper.Contains(".XLS") Then
                    'FIS (T8500)
                    If LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Columns.Count >= 58 And LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Columns.Count <= 65 Then
                        SplashScreenManager.Default.SetWaitFormDescription("Loading Data Source " & (i + 1).ToString)
                        dtSourceTmp1 = LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Select("[Special Product] = 'CTRF' OR [Special Product] = '2PRD' OR [Special Product] = 'RACT'").CopyToDataTable
                        InsertDataFile1(dtSourceTmp1)
                        Continue For
                    End If
                    'FIS (A1060)
                    If LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Columns.Count >= 66 And LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Columns.Count <= 69 Then
                        SplashScreenManager.Default.SetWaitFormDescription("Loading Data Source " & (i + 1).ToString)
                        dtSourceTmp2 = LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0)
                        InsertDataFile2(dtSourceTmp2)
                        Continue For
                    End If
                    'FIS (T8000)
                    If LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Columns.Count >= 65 Then
                        SplashScreenManager.Default.SetWaitFormDescription("Loading Data Source " & (i + 1).ToString)
                        dtSourceTmp3 = QueryExcel(OpenFileDialog1.FileNames(i), "SELECT F4 AS [Booking],F13 AS [Container],F43 AS [DPVoyage2],F44 AS [VesselName2],FORMAT(F45,'MM/dd/yyyy') + space(1) + FORMAT(F46,'HH:mm:ss') AS [ArrivalTSP],FORMAT(F53,'MM/dd/yyyy') + space(1) + FORMAT(F54,'HH:mm:ss') AS [DepartureTSP] FROM [Transshipment Request List$] WHERE F1 IS NOT NULL").Tables(0)
                        InsertDataFile3(dtSourceTmp3)
                        Continue For
                    End If
                End If
                If OpenFileDialog1.FileNames(i).ToUpper.Contains(".CSV") Then
                    'FIS (T8500)
                    If LoadCSV(OpenFileDialog1.FileNames(i), True).Columns(0).ToString.Contains("Traffic Light") Then 'LoadCSV(OpenFileDialog1.FileNames(i), True).Columns.Count >= 58 And LoadCSV(OpenFileDialog1.FileNames(i), True).Columns.Count <= 65 Then
                        SplashScreenManager.Default.SetWaitFormDescription("Loading Data Source " & (i + 1).ToString)
                        dtSourceTmp1 = LoadCSV(OpenFileDialog1.FileNames(i), True)
                        InsertDataFile1(dtSourceTmp1)
                        Continue For
                    End If
                    'FIS (A1060)
                    If LoadCSV(OpenFileDialog1.FileNames(i), True).Columns(0).ToString.Contains("DP-Voyage") Then 'LoadCSV(OpenFileDialog1.FileNames(i), True).Columns.Count >= 66 And LoadCSV(OpenFileDialog1.FileNames(i), True).Columns.Count <= 69 Then
                        SplashScreenManager.Default.SetWaitFormDescription("Loading Data Source " & (i + 1).ToString)
                        dtSourceTmp2 = LoadCSV(OpenFileDialog1.FileNames(i), True)
                        InsertDataFile2(dtSourceTmp2)
                        Continue For
                    End If
                End If
            Next
            Dim Vessel As String = ""
            If dtSourceFile1.Rows.Count = 0 And dtSourceFile2.Rows.Count = 0 And dtSourceFile3.Rows.Count > 0 Then
                DataProcess1()
            Else
                Dim drSource As DataRow = dtSourceFile2.Rows(0)
                drSource(0) = Format(CInt(drSource(0)), "000000")
                dtVoyage = ExecuteAccessQuery("select * from ScheduleVoyage where [POL] = '" & drSource(23) & "' and [DPVOYAGE]='" & drSource(0) & "'").Tables(0)
                'dtVoyageTS = ExecuteAccessQuery("select * from ScheduleVoyage where [POL] = '" & drSource(23) & "' and [DPVOYAGE]='" & drSource("[MC DP-Voyage No.]") & "'").Tables(0)
                If dtVoyage.Rows.Count = 0 Then
                    SplashScreenManager.CloseForm(False)
                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The schedule voyage was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
                Dim drVoyage As DataRow = dtVoyage.Rows(0)
                Vessel = drVoyage(2).trim & " " & drVoyage(3).trim
                'If ExecuteAccessQuery("select distinct ").Tables(0).Rows.Count > 1 Then
                '    If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The vessel name has more than 1 dp-voyage code, do you want continue?", "Question", MessageBoxButtons.OK, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                '        Return
                '    End If
                'End If
                For Each dtRow As DataRow In dtSourceFile1.Rows
                    If Not ExecuteAccessNonQuery("delete from ColdTreatment where [CONTAINER]='" & Replace(dtRow(3), " ", "") & "' and [BOOKING]='" & dtRow(2) & "' and [VESSEL]<>'" & Vessel & "'") Then
                        DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "There was an error while delete the existing data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                Next
                DataProcess2()
            End If
            'gcContainerList.DataSource = dtContainerList
            'gcDataColdTreatment.DataSource = dtNewDataCT
            'GridView2.MoveLast()
            bbiShowAll.PerformClick()
            GridView2.ActiveFilterString = Nothing
            If Vessel <> "" Then
                GridView2.ActiveFilterString = "VESSEL='" & Vessel & "'"
            Else

            End If
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The process has been completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
    End Sub

    Private Sub DataProcess1()
        Dim dtQuery As New DataTable
        Dim ContainerNumber As String = ""
        SplashScreenManager.Default.SetWaitFormDescription("Update Transhipment Data of Cold Treatment")
        For r = 1 To dtSourceFile3.Rows.Count - 1
            Dim oRow As DataRow = dtSourceFile3.Rows(r)
            If IsDBNull(oRow(0)) Or IsDBNull(oRow(1)) Then
                Continue For
            End If
            Dim aTranshipment As New ArrayList
            ContainerNumber = IIf(Replace(oRow(1), " ", "") Is Nothing, "", Replace(oRow(1), " ", ""))
            aTranshipment = GetTranshipmentData(oRow(0).ToString, oRow(1).ToString)
            If aTranshipment.Count > 0 Then
                If Not IsDBNull(aTranshipment(0)) And Not IsDBNull(1) Then
                    'dtNewDataCT.Rows(iPosition).Item(16) = aTranshipment(0) 'ETA2
                    'dtNewDataCT.Rows(iPosition).Item(17) = aTranshipment(1) 'ETD2
                    UpdateAccess("ColdTreatment", "[CONTAINER] = '" & ContainerNumber & "' and [BOOKING] = '" & oRow(0).ToString & "'", "ETA2=" & Format(aTranshipment(0), "#MM/dd/yyyy HH:mm:ss#") & ", ETD2=" & Format(aTranshipment(1), "#MM/dd/yyyy HH:mm:ss#") & ", UpdatedBy='" & My.User.Name & "', UpdatedDate=" & Format(Now, "#MM/dd/yyyy HH:mm:ss#"))
                End If
            End If
        Next
    End Sub

    Private Sub DataProcess2()
        SplashScreenManager.Default.SetWaitFormDescription("Update Data Master of Cold Treatment")
        Dim ContainerNumber, Vessel As String
        Dim dtDBColdTreatment As New DataTable
        Dim drSource1, drSource, drVoyage As DataRow
        Dim dtQuery As New DataTable
        Dim dEta2 As DateTime = Nothing
        dtContainerList = dsDataTarget.Tables("ContainerList")
        dtNewDataCT = ExecuteAccessQuery("select * from ColdTreatment where [CONTAINER]=''").Tables(0) 'dsDataTarget.Tables("MasterColdTreatment")
        dtContainerList.Rows.Clear()
        'dtContainerList = SelectDistinct(dtSourceFile1, "([Special Product] = 'CTRF' OR [Special Product] = '2PRD') AND [Container Number] <> ''", "Shipment", "Container Number")
        dtContainerList = SelectDistinct(dtSourceFile1, "([Special Product] = 'CTRF' OR [Special Product] = '2PRD' OR [Special Product] = 'RACT')", "Shipment", "Container Number")
        dtNewDataCT.Rows.Clear()
        Dim iPosition As Integer = 0
        ContainerNumber = ""
        Vessel = ""
        For Each row As DataRow In dtContainerList.Rows
            drVoyage = dtVoyage.Rows(0)
            Vessel = drVoyage(2).trim & " " & drVoyage(3).trim
            ContainerNumber = IIf(Replace(row(1), " ", "") Is Nothing, "", Replace(row(1), " ", ""))
            dtQuery = ExecuteAccessQuery("select * from ColdTreatment where [CONTAINER] = '" & ContainerNumber & "' and [BOOKING] = '" & row(0).ToString & "'").Tables(0)
            Dim aTranshipment As New ArrayList
            If dtSourceFile3.Rows.Count > 0 Then
                aTranshipment = GetTranshipmentData(row(0).ToString, row(1).ToString)
                If dtQuery.Rows.Count > 0 Then
                    If aTranshipment.Count > 0 Then
                        'dtNewDataCT.Rows(iPosition).Item(16) = aTranshipment(0) 'ETA2
                        'dtNewDataCT.Rows(iPosition).Item(17) = aTranshipment(1) 'ETD2
                        UpdateAccess("ColdTreatment", "[CONTAINER] = '" & ContainerNumber & "' and [BOOKING] = '" & row(0).ToString & "'", "ETA2=" & Format(aTranshipment(0), "#MM/dd/yyyy HH:mm:ss#") & ", ETD2=" & Format(aTranshipment(1), "#MM/dd/yyyy HH:mm:ss#") & ", UpdatedBy='" & My.User.Name & "', UpdatedDate=" & Format(Now, "#MM/dd/yyyy HH:mm:ss#"))
                        Continue For
                    End If
                End If
            End If
            'FindEta2(row(0).ToString, row(1).ToString)
            'dtVoyageTS = ExecuteAccessQuery("select * from ScheduleVoyage where [POL] = '" & drSource(23) & "' and [DPVOYAGE]='" & drSource("[MC DP-Voyage No.]") & "'").Tables(0)
            If dtQuery.Rows.Count = 0 Then
                drSource1 = Nothing
                If dtSourceFile1.Select("[Shipment] = '" & row(0).ToString & "' and [Container Number] = '" & row(1) & "'").Length > 0 Then
                    drSource1 = dtSourceFile1.Select("[Shipment] = '" & row(0).ToString & "' and [Container Number] = '" & row(1) & "'")(0)
                End If
                If dtSourceFile2.Select("[Shipment] = '" & row(0).ToString & "' and [Cont#Number] = '" & row(1) & "'").Length > 0 Then
                    If drSource1("Temp Celcius") = "" Then
                        drSource1("Temp Celcius") = "0.0"
                    End If
                    If drSource1("Special Product") = "2PRD" And CDbl(Replace(drSource1("Temp Celcius"), ",", ".")) > 1.5 Then
                        Continue For
                    End If
                    drSource = dtSourceFile2.Select("[Shipment] = '" & row(0).ToString & "' and [Cont#Number] = '" & row(1) & "'")(0)
                    'drVoyage = dtVoyage.Select("[POL] = '" & drSource(23) & "' and [DPVOYAGE] = '" & drSource(0).ToString & "'")(0)
                    dtNewDataCT.Rows.Add()
                    iPosition = dtNewDataCT.Rows.Count - 1
                    'dtNewDataCT.Rows(iPosition).Item("C1") = (dtDBColdTreatment.Rows.Count + 1).ToString
                    dtNewDataCT.Rows(iPosition).Item(0) = ContainerNumber 'CONTAINER
                    dtNewDataCT.Rows(iPosition).Item(1) = drSource(1) 'BOOKING
                    dtNewDataCT.Rows(iPosition).Item(2) = drSource("Cargo Description") 'CGODESC
                    dtNewDataCT.Rows(iPosition).Item(3) = Replace(drSource1("Temp Celcius"), ",", ".") 'TEMPERATURE
                    dtNewDataCT.Rows(iPosition).Item(4) = drSource(23) 'POL
                    dtNewDataCT.Rows(iPosition).Item(5) = "" 'CHKDL
                    'dtNewDataCT.Rows(iPosition).Item(6) = "" 'INIDATE
                    dtNewDataCT.Rows(iPosition).Item(7) = drSource(24) 'POD
                    dtNewDataCT.Rows(iPosition).Item(8) = drSource(22) 'FDP
                    dtNewDataCT.Rows(iPosition).Item(9) = drSource("Export Party") 'EXP_PARTY
                    dtNewDataCT.Rows(iPosition).Item(10) = drSource("Routing Party") 'ROU_PARTY
                    dtNewDataCT.Rows(iPosition).Item(11) = drSource(59) 'DEPOT
                    dtNewDataCT.Rows(iPosition).Item(12) = Vessel 'VESSEL
                    dtNewDataCT.Rows(iPosition).Item(13) = drVoyage(4) 'SERVICE
                    dtNewDataCT.Rows(iPosition).Item(14) = Format(drVoyage(6), "dd/MM/yyyy HH:mm:ss") 'ETA1
                    dtNewDataCT.Rows(iPosition).Item(15) = drSource(24) 'TSP
                    If dtSourceFile2.Rows.Count > 0 Then
                        If aTranshipment.Count > 0 Then
                            dtNewDataCT.Rows(iPosition).Item(16) = aTranshipment(0) 'ETA2
                            dtNewDataCT.Rows(iPosition).Item(17) = aTranshipment(1) 'ETD2
                        End If
                    End If
                    dtNewDataCT.Rows(iPosition).Item(18) = "" 'TSCHKDL
                    'dtNewDataCT.Rows(iPosition).Item(19) = "" 'FINDATE
                    'dtNewDataCT.Rows(iPosition).Item(20) = "" 'CTDAYS
                    dtNewDataCT.Rows(iPosition).Item(21) = "" 'REMARKS
                    dtNewDataCT.Rows(iPosition).Item(22) = "" 'SENASA
                    dtNewDataCT.Rows(iPosition).Item(23) = "N" 'SHARED
                    dtNewDataCT.Rows(iPosition).Item("CreatedBy") = My.User.Name
                    dtNewDataCT.Rows(iPosition).Item("CreatedDate") = Now.ToString
                    'If LoadExcel(beDataFileTarget.Text, "{0}").Tables(0).Select("BOOKING = '" & drSource(1).ToString & "' and CONTAINER = '" & drSource(35) & "'").Length > 0 Then
                    'If ExecuteAccessQuery("select * from ColdTreatment where [BOOKING] = '" & drSource(1).ToString & "' and CONTAINER = '" & drSource(35) & "'").Tables(0).Rows.Count > 0 Then
                    '    dtNewDataCT.Rows(iPosition).Item(20) = "The Container: " & drSource(35) & " of Booking: " & drSource(1).ToString & " already exists in the target file."
                    'Else
                    'InsertIntoExcel(beDataFileTarget.EditValue, "{0}", dtNewDataCT.Rows(iPosition))
                    InsertIntoAccess("ColdTreatment", dtNewDataCT.Rows(iPosition))
                    'End If
                End If
            End If
        Next
        'Elimina combinaciones (bkg & ctn) que ya no existan en la nave
        dtQuery = ExecuteAccessQuery("select * from ColdTreatment where VESSEL='" & Vessel & "'").Tables(0)
        If dtQuery.Rows.Count = 0 Then
            Return
        End If
        For Each oRow As DataRow In dtQuery.Rows
            If dtContainerList.Select("[Container Number]='" & Trim(Mid(oRow("CONTAINER"), 1, 4) & Space(2) & Mid(oRow("CONTAINER"), 5, 7)) & "' and Shipment='" & oRow("BOOKING") & "'").Length = 0 Then
                If Not ExecuteAccessNonQuery("delete from ColdTreatment where [CONTAINER]='" & oRow("CONTAINER") & "' and [BOOKING]='" & oRow("BOOKING") & "' and [VESSEL]='" & Vessel & "'") Then
                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "There was an error while delete the existing data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            End If
        Next
    End Sub

    Friend Function GetTranshipmentData(book As String, ctn As String) As ArrayList
        Dim aResult As New ArrayList
        If dtSourceFile3.Select("[Booking] = '" & book & "' and [Container] = '" & ctn & "'").Length > 0 Then
            Dim drSource As DataRow = dtSourceFile3.Select("[Booking] = '" & book & "' and [Container] = '" & ctn & "'")(0)
            Dim date1 As Date = Date.ParseExact(drSource("ArrivalTSP"), "MM/dd/yyyy HH:mm:ss", Nothing)
            Dim date2 As Date = Date.ParseExact(drSource("DepartureTSP"), "MM/dd/yyyy HH:mm:ss", Nothing)
            aResult.Add(CDate(date1))
            aResult.Add(CDate(date2))
        End If
        Return aResult
    End Function

    Private Sub InsertDataFile1(dtFile1 As DataTable)
        If dtSourceFile1.Rows.Count = 0 Then
            If dtFile1.Rows.Count > 0 Then
                dtSourceFile1 = dtFile1 '.Select("[Container Number]<>''").CopyToDataTable
            End If
        Else
            For Each row As DataRow In dtFile1.Rows
                'If row(3) <> "" Then
                dtSourceFile1.ImportRow(row)
                'End If
            Next
        End If
    End Sub

    Private Sub InsertDataFile2(dtFile2 As DataTable)
        If dtSourceFile2.Rows.Count = 0 Then
            dtSourceFile2 = dtFile2
        Else
            For Each row As DataRow In dtFile2.Rows
                dtSourceFile2.ImportRow(row)
            Next
        End If
    End Sub

    Private Sub InsertDataFile3(dtFile3 As DataTable)
        If dtSourceFile3.Rows.Count = 0 Then
            dtSourceFile3 = dtFile3
        Else
            For Each row As DataRow In dtFile3.Rows
                dtSourceFile3.ImportRow(row)
            Next
        End If
    End Sub

    Private Sub ProcessesVendorExcelData(dtData As DataTable)

        Dim iPosition As Integer = 0

        Try
            For Each row As DataRow In dtData.Rows
                dtResult.Rows.Add()
                iPosition = dtResult.Rows.Count - 1
                dtResult.Rows(iPosition).Item("C1") = Format(row(0), "dd/MM/yyyy")
                dtResult.Rows(iPosition).Item("C2") = Format(row(0), "hh:mm tt")
                dtResult.Rows(iPosition).Item("C3") = row(11)
                dtResult.Rows(iPosition).Item("C4") = row(12)
                dtResult.Rows(iPosition).Item("C5") = row(13)
            Next
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        If gcVendorReadings.FocusedView.IsFocusedView Then
            ExportarExcel(gcVendorReadings)
        ElseIf gcEvents.FocusedView.IsFocusedView Then
            ExportarExcel(gcEvents)
        Else
            ExportarExcel(gcDataColdTreatment)
        End If
    End Sub

    'Private Sub GridView2_RowStyle(ByVal sender As Object, ByVal e As RowStyleEventArgs)
    '    Dim View As GridView = sender
    '    If (e.RowHandle >= 0) Then
    '        Dim C22 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("C22"))
    '        If C22 <> "" Then
    '            e.Appearance.BackColor = Color.Salmon
    '            e.Appearance.BackColor2 = Color.SeaShell
    '        End If
    '    End If
    'End Sub

    Private Sub beDataSource_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beDataSource.Properties.ButtonClick
        Dim FileNames() As String
        OpenFileDialog1.Filter = "FIS Source Files (*.xls*;*.csv)|*.xls*;*.csv"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.InitialDirectory = IIf(My.Settings.DataTargetPath <> "", My.Settings.DataTargetPath, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            FileNames = OpenFileDialog1.FileNames
            beDataSource.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub DataSourceForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GridView2.RestoreLayoutFromRegistry(Directory.GetCurrentDirectory)
        beDataFileTarget.EditValue = My.Settings.DataSourcePath & "\" & My.Settings.DBFileName
        LoadValidations()
        LoadOperationsCodes()
        beiShowGap.EditValue = False
        SplitContainerControl3.Collapsed = True
        'Timer1.Start()
    End Sub

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiShowAll.ItemClick
        dtResult.Rows.Clear()
        'dtResult = ExecuteAccessQuery("SELECT *,  IIf(REMARKS='CT PASSED',0,IIf(INIDATE Is Not Null,1,IIf(TSCHK2DL='INTERRUPTION',2,3))) AS STATUS, 0 AS GAP, 0 AS BROKE FROM ColdTreatment").Tables(0)
        dtResult = ExecuteAccessQuery("SELECT * FROM qryDataSource").Tables(0)
        gcDataColdTreatment.DataSource = dtResult
    End Sub

    Private Sub bbiUpdate_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiUpdate.ItemClick
        If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Are you sure to update?", "Confirmation", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.No Then
            Return
        End If
        Validate()
        Dim sConditions, sValues As String
        GridView2.OptionsLayout.StoreAllOptions = True
        GridView2.ActiveFilterEnabled = False
        GridView2.ClearSorting()
        Dim info As GridViewInfo = TryCast(GridView2.GetViewInfo(), GridViewInfo)
        Dim GridRowInfo As GridRowInfo = info.GetGridRowInfo(GridView2.FocusedRowHandle)
        For r = 0 To GridView2.RowCount - 1
            If dtResult.Rows(r).RowState = DataRowState.Modified Then
                sConditions = "CONTAINER='" & GridView2.GetRowCellValue(r, "CONTAINER") & "' AND BOOKING='" & GridView2.GetRowCellValue(r, "BOOKING") & "'"
                sValues = ""
                For c = 0 To GridView2.Columns.Count - 1
                    If Not GridView2.Columns(c).OptionsColumn.ReadOnly Then
                        If IsDBNull(GridView2.GetRowCellValue(r, GridView2.Columns(c).FieldName)) Then
                            sValues = sValues & IIf(sValues = "", "", ", ") & GridView2.Columns(c).FieldName & "=NULL"
                        Else
                            sValues = sValues & IIf(sValues = "", "", ", ") & GridView2.Columns(c).FieldName & "='" & GridView2.GetRowCellValue(r, GridView2.Columns(c).FieldName) & "'"
                        End If
                    End If
                Next
                sValues += IIf(sValues = "", "", ", ") & GridView2.Columns("UpdatedBy").FieldName & "='" & My.User.Name & "'"
                sValues += IIf(sValues = "", "", ", ") & GridView2.Columns("UpdatedDate").FieldName & "='" & Now.ToString & "'"
                UpdateAccess("ColdTreatment", sConditions, sValues)
            End If
        Next
        GridView2.ActiveFilterEnabled = True
        bbiShowAll.PerformClick()
        GridView2.MoveBy(GridRowInfo.RowHandle)
    End Sub

    Private Sub LoadValidations()
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

    Private Sub RepositoryItemHyperLinkEdit1_Click(sender As Object, e As EventArgs) Handles RepositoryItemHyperLinkEdit1.Click
        Dim TrendForm As New GraphicTrendForm
        TrendForm.pBooking = GridView2.GetFocusedRowCellValue("BOOKING")
        TrendForm.pContainer = GridView2.GetFocusedRowCellValue("CONTAINER")
        TrendForm.dtEvents = gcEvents.DataSource
        TrendForm.pGap = IIf(beiShowGap.EditValue = True, "Y", "N")
        TrendForm.pSetpoint = GridView2.GetFocusedRowCellValue("TEMPERATURE")
        TrendForm.ShowDialog()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim info As GridViewInfo = TryCast(GridView2.GetViewInfo(), GridViewInfo)
        Dim GridRowInfo As GridRowInfo = info.GetGridRowInfo(GridView2.FocusedRowHandle)
        bbiShowAll.PerformClick()
        GridView2.MoveBy(GridRowInfo.RowHandle)
    End Sub

    Private Sub beiRefresh_EditValueChanged(sender As Object, e As EventArgs) Handles beiShowGap.EditValueChanged
        'Timer1.Enabled = beiShowGap.EditValue
    End Sub

    Private Sub GridView2_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView2.FocusedRowChanged
        Dim dtQueryEvt, dtQueryRdg As New DataTable
        dtQueryRdg = ExecuteAccessQuery("select * from ColdTreatmentReadings where [BOOKING]='" & GridView2.GetFocusedRowCellValue("BOOKING") & "' and [CONTAINER] = '" & GridView2.GetFocusedRowCellValue("CONTAINER") & "'").Tables(0)
        gcVendorReadings.DataSource = dtQueryRdg
        dtQueryEvt = ExecuteAccessQuery("select * from ColdTreatmentEvents where [BOOKING]='" & GridView2.GetFocusedRowCellValue("BOOKING") & "' and [CONTAINER] = '" & GridView2.GetFocusedRowCellValue("CONTAINER") & "'").Tables(0)
        gcEvents.DataSource = dtQueryEvt
    End Sub

    Private Sub GridView3_RowCellStyle(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs) Handles GridView3.RowCellStyle
        Dim View As GridView = sender
        If (e.RowHandle >= 0) Then
            If e.Column.FieldName = "CT_USDA1" Then 'USDA1
                Dim C3 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("CT_USDA1"))
                If C3 = "" Or C3 > MaxTemp Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If
            End If
            If e.Column.FieldName = "CT_USDA2" Then 'USDA2
                Dim C4 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("CT_USDA2"))
                If C4 = "" Or C4 > MaxTemp Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If
            End If
            If e.Column.FieldName = "CT_USDA3" Then 'USDA3
                Dim C5 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("CT_USDA3"))
                If C5 = "" Or C5 > MaxTemp Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If
            End If
        End If
    End Sub

    Private Sub SeleccionaFilas(caso As Integer)
        Dim i As Integer = 0
        Do While i < GridView2.RowCount
            Dim row As DataRow = GridView2.GetDataRow(i)
            If caso = 0 Then
                row("SHARED") = "Y"
            End If
            If caso = 1 Then
                row("SHARED") = "N"
            End If
            If caso = 2 Then
                If row("SHARED") = "Y" Then
                    row("SHARED") = "N"
                Else
                    row("SHARED") = "Y"
                End If
            End If
            i += 1
        Loop
    End Sub

    Private Sub SeleccionaTodosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SeleccionaTodosToolStripMenuItem.Click
        SeleccionaFilas(0)
    End Sub

    Private Sub DeseleccionaTodosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeseleccionaTodosToolStripMenuItem.Click
        SeleccionaFilas(1)
    End Sub

    Private Sub InvertirSelecciónToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InvertirSelecciónToolStripMenuItem.Click
        SeleccionaFilas(2)
    End Sub

    Private Sub DataSourceForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        GridView2.SaveLayoutToRegistry(Directory.GetCurrentDirectory)
        My.Settings.CustomDataSourceFilter = GridView2.ActiveFilterString
        My.Settings.Save()
    End Sub

    Private Sub rgFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rgFilter.SelectedIndexChanged
        GridView2.ActiveFilterString = ""
        If sender.SelectedIndex = 1 Then
            GridView2.ActiveFilterString = My.Settings.CustomDataSourceFilter
        End If
    End Sub

    Private Sub GridView2_FocusedRowChanged_1(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView2.FocusedRowChanged
        Dim dgrItem As DataRow = GridView2.GetDataRow(e.FocusedRowHandle)
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

    Private Sub LoadOperationsCodes()
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("SELECT OPS_CODE, DESCRIPTION FROM OperationCode").Tables(0)
        RepositoryItemLookUpEdit2.DataSource = dtQuery
        RepositoryItemLookUpEdit2.DisplayMember = "OPS_CODE"
        RepositoryItemLookUpEdit2.ValueMember = "OPS_CODE"
    End Sub

End Class