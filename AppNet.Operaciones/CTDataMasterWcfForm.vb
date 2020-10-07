Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraGrid.Columns

Public Class CTDataMasterWcfForm
    Dim dsDataTarget As New dsMain
    Dim dtContainerList, dtNewDataCT, dtResult, dtVoyage, dtVoyageTS, dtSourceFile1, dtSourceFile2, dtSourceFile3 As New DataTable
    Dim MaxTemp As Decimal = My.Settings.MaxTemp
    Dim oAppService As New AppService.HapagLloydServiceClient

    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().

    End Sub

    Private Sub DataSourceForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'GridView2.RestoreLayoutFromRegistry(Directory.GetCurrentDirectory)
        'GridView2.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom
        'beDataFileTarget.EditValue = My.Settings.DataSourcePath & "\" & My.Settings.DBFileName
        LoadOperationsCodes()
        beiShowGap.EditValue = False
        SplitContainerControl3.Collapsed = True
        dtResult = oAppService.ExecuteSQL("SELECT TOP 0 * FROM tck.ColdTreatment").Tables(0)
        gcDataColdTreatment.DataSource = dtResult
        'Timer1.Start()
    End Sub

    Private Sub bbiProcesss_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiImport.ItemClick
        LoadValidations()
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
                'dtVoyage = ExecuteAccessQuery("select * from ScheduleVoyage where [POL] = '" & drSource(23) & "' and [DPVOYAGE]='" & drSource(0) & "'").Tables(0)
                dtVoyage = oAppService.ExecuteSQL("select * from tck.ScheduleVoyage where [POL] = '" & drSource("selected POL") & "' and [DPVOYAGE]='" & drSource("DP-Voyage") & "'").Tables(0)
                'dtVoyageTS = ExecuteAccessQuery("select * from ScheduleVoyage where [POL] = '" & drSource(23) & "' and [DPVOYAGE]='" & drSource("[MC DP-Voyage No.]") & "'").Tables(0)
                If dtVoyage.Rows.Count = 0 Then
                    SplashScreenManager.CloseForm(False)
                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The schedule voyage was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
                Dim drVoyage As DataRow = dtVoyage.Rows(0)
                Vessel = drVoyage(2).trim & " " & drVoyage(3).trim
                For r = 0 To dtSourceFile1.Rows.Count - 1
                    Dim dtRow As DataRow = dtSourceFile1.Rows(r)
                    Try
                        oAppService.ExecuteSQLNonQuery("delete from tck.ColdTreatment where [CONTAINER]='" & Replace(dtRow(3), " ", "") & "' and [BOOKING]='" & dtRow(2) & "' and [VESSEL]<>'" & Vessel & "'")
                    Catch ex As Exception
                        DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "There was an error while delete the existing data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End Try
                Next
                DataProcess2()
            End If
            bbiSearch.PerformClick()
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
        Dim Booking As String = ""
        SplashScreenManager.Default.SetWaitFormDescription("Update Transhipment Data of Cold Treatment")
        For r = 1 To dtSourceFile3.Rows.Count - 1
            Dim oRow As DataRow = dtSourceFile3.Rows(r)
            If IsDBNull(oRow(0)) Or IsDBNull(oRow(1)) Then
                Continue For
            End If
            Dim aTranshipment As New ArrayList
            ContainerNumber = IIf(Replace(oRow(1), " ", "") Is Nothing, "", Replace(oRow(1), " ", ""))
            Booking = oRow(0).ToString
            dtQuery = oAppService.ExecuteSQL("SELECT * FROM tck.ColdTreatment WHERE [CONTAINER] = '" & ContainerNumber & "' and [BOOKING] = '" & Booking & "'").Tables(0)
            Dim drColdTreatment As DataRow = dtQuery.Rows(0)
            aTranshipment = GetTranshipmentData(oRow(0).ToString, oRow(1).ToString)
            If aTranshipment.Count > 0 Then
                If Not IsDBNull(aTranshipment(0)) And Not IsDBNull(aTranshipment(1)) Then
                    'UpdateAccess("ColdTreatment", "[CONTAINER] = '" & ContainerNumber & "' and [BOOKING] = '" & Booking & "'", "ETA2=" & Format(aTranshipment(0), "#MM/dd/yyyy HH:mm:ss#") & ", ETD2=" & Format(aTranshipment(1), "#MM/dd/yyyy HH:mm:ss#") & ", UpdatedBy='" & My.User.Name & "', UpdatedDate=" & Format(Now, "#MM/dd/yyyy HH:mm:ss#"))
                    '"ETA2=" & Format(aTranshipment(0), "#MM/dd/yyyy HH:mm:ss#") & ", ETD2=" & Format(aTranshipment(1), "#MM/dd/yyyy HH:mm:ss#") & ", UpdatedBy='" & My.User.Name & "', UpdatedDate=" & Format(Now, "#MM/dd/yyyy HH:mm:ss#"
                    drColdTreatment("ETA2") = CDate(aTranshipment(0))
                    drColdTreatment("ETD2") = CDate(aTranshipment(1))
                    drColdTreatment("UpdatedBy") = My.User.Name
                    drColdTreatment("UpdatedDate") = Today
                    'drColdTreatment.AcceptChanges()
                    oAppService.UpdateColdTreatment(dtQuery)
                End If
            End If
        Next
    End Sub

    Private Sub DataProcess2()
        SplashScreenManager.Default.SetWaitFormDescription("Update Data Master of Cold Treatment")
        Dim ContainerNumber, Booking, Vessel As String
        Dim dtDBColdTreatment As New DataTable
        Dim drSource1, drSource, drVoyage As DataRow
        Dim dtQuery As New DataTable
        Dim dEta2 As DateTime = Nothing
        dtContainerList = dsDataTarget.Tables("ContainerList")
        dtNewDataCT = oAppService.ExecuteSQL("select TOP 0 * from tck.ColdTreatment").Tables(0) 'dsDataTarget.Tables("MasterColdTreatment")
        dtContainerList.Rows.Clear()
        'dtContainerList = SelectDistinct(dtSourceFile1, "([Special Product] = 'CTRF' OR [Special Product] = '2PRD') AND [Container Number] <> ''", "Shipment", "Container Number")
        dtContainerList = SelectDistinct(dtSourceFile1, "([Special Product] = 'CTRF' OR [Special Product] = '2PRD' OR [Special Product] = 'RACT')", "Shipment", "Container Number")
        Dim iPosition As Integer = 0
        ContainerNumber = ""
        Booking = ""
        Vessel = ""
        For r = 0 To dtContainerList.Rows.Count - 1
            dtNewDataCT.Rows.Clear()
            Dim row As DataRow = dtContainerList.Rows(r)
            drVoyage = dtVoyage.Rows(0)
            Vessel = drVoyage(2).trim & " " & drVoyage(3).trim
            ContainerNumber = IIf(Replace(row(1), " ", "") Is Nothing, "", Replace(row(1), " ", ""))
            Booking = row(0).ToString
            dtQuery = oAppService.ExecuteSQL("select * from tck.ColdTreatment where [CONTAINER] = '" & ContainerNumber & "' and [BOOKING] = '" & Booking & "'").Tables(0)
            Dim aTranshipment As New ArrayList
            If dtSourceFile3.Rows.Count > 0 Then
                aTranshipment = GetTranshipmentData(row(0).ToString, row(1).ToString)
                If dtQuery.Rows.Count > 0 Then
                    Dim drColdTreatment As DataRow = dtQuery.Rows(0)
                    If aTranshipment.Count > 0 Then
                        'UpdateAccess("ColdTreatment", "[CONTAINER] = '" & ContainerNumber & "' and [BOOKING] = '" & row(0).ToString & "'", "ETA2=" & Format(aTranshipment(0), "#MM/dd/yyyy HH:mm:ss#") & ", ETD2=" & Format(aTranshipment(1), "#MM/dd/yyyy HH:mm:ss#") & ", UpdatedBy='" & My.User.Name & "', UpdatedDate=" & Format(Now, "#MM/dd/yyyy HH:mm:ss#"))
                        drColdTreatment("ETA2") = CDate(aTranshipment(0))
                        drColdTreatment("ETD2") = CDate(aTranshipment(1))
                        drColdTreatment("UpdatedBy") = My.User.Name
                        drColdTreatment("UpdatedDate") = Today
                        'drColdTreatment.AcceptChanges()
                        oAppService.UpdateColdTreatment(dtQuery)
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
                    dtNewDataCT.Rows(iPosition).Item(0) = ContainerNumber 'CONTAINER
                    dtNewDataCT.Rows(iPosition).Item(1) = drSource(1) 'BOOKING
                    dtNewDataCT.Rows(iPosition).Item(2) = drSource("Cargo Description") 'CGODESC
                    dtNewDataCT.Rows(iPosition).Item(3) = Replace(drSource1("Temp Celcius"), ",", ".") 'TEMPERATURE
                    dtNewDataCT.Rows(iPosition).Item(4) = drSource(23) 'POL
                    dtNewDataCT.Rows(iPosition).Item(5) = "" 'CHKDL
                    dtNewDataCT.Rows(iPosition).Item(6) = DBNull.Value 'INIDATE
                    dtNewDataCT.Rows(iPosition).Item(7) = drSource(24) 'POD
                    dtNewDataCT.Rows(iPosition).Item(8) = drSource(22) 'FDP
                    dtNewDataCT.Rows(iPosition).Item(9) = drSource("Export Party") 'EXP_PARTY
                    dtNewDataCT.Rows(iPosition).Item(10) = drSource("Routing Party") 'ROU_PARTY
                    dtNewDataCT.Rows(iPosition).Item(11) = drSource(59) 'DEPOT
                    dtNewDataCT.Rows(iPosition).Item(12) = Vessel 'VESSEL
                    dtNewDataCT.Rows(iPosition).Item(13) = drVoyage(4) 'SERVICE
                    dtNewDataCT.Rows(iPosition).Item(14) = Format(drVoyage(6), "dd/MM/yyyy HH:mm:ss") 'ETA1
                    dtNewDataCT.Rows(iPosition).Item(15) = drSource(24) 'TSP
                    dtNewDataCT.Rows(iPosition).Item(16) = DBNull.Value 'ETA2
                    dtNewDataCT.Rows(iPosition).Item(17) = DBNull.Value 'ETD2
                    If dtSourceFile2.Rows.Count > 0 Then
                        If aTranshipment.Count > 0 Then
                            dtNewDataCT.Rows(iPosition).Item(16) = aTranshipment(0) 'ETA2
                            dtNewDataCT.Rows(iPosition).Item(17) = aTranshipment(1) 'ETD2
                        End If
                    End If
                    dtNewDataCT.Rows(iPosition).Item(18) = "" 'TSCHKDL
                    dtNewDataCT.Rows(iPosition).Item(19) = DBNull.Value 'FINDATE
                    dtNewDataCT.Rows(iPosition).Item(20) = DBNull.Value 'CTDAYS
                    dtNewDataCT.Rows(iPosition).Item(21) = "" 'REMARKS
                    dtNewDataCT.Rows(iPosition).Item(22) = "" 'SENASA
                    dtNewDataCT.Rows(iPosition).Item(23) = "N" 'SHARED
                    dtNewDataCT.Rows(iPosition).Item("CreatedBy") = My.User.Name
                    dtNewDataCT.Rows(iPosition).Item("CreatedDate") = Now.ToString
                    'InsertIntoAccess("ColdTreatment", dtNewDataCT.Rows(iPosition))
                    oAppService.InsertColdTreatment(dtNewDataCT)
                    'End If
                End If
            End If
        Next
        'Elimina combinaciones (bkg & ctn) que ya no existan en la nave
        dtQuery = oAppService.ExecuteSQL("select * from tck.ColdTreatment where VESSEL='" & Vessel & "'").Tables(0)
        If dtQuery.Rows.Count = 0 Then
            Return
        End If
        For Each oRow As DataRow In dtQuery.Rows
            If dtContainerList.Select("[Container Number]='" & Trim(Mid(oRow("CONTAINER"), 1, 4) & Space(2) & Mid(oRow("CONTAINER"), 5, 7)) & "' and Shipment='" & oRow("BOOKING") & "'").Length = 0 Then
                Dim aSource As New ArrayList
                aSource.AddRange({Trim(Mid(oRow("CONTAINER"), 1, 4) & Space(2) & Mid(oRow("CONTAINER"), 5, 7)), oRow("BOOKING")})
                If Not oAppService.DeleteColdTreatment(aSource.ToArray) Then
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

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSearch.ItemClick
        dtResult.Rows.Clear()
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            'dtResult = ExecuteAccessQuery("SELECT *,  IIf(REMARKS='CT PASSED',0,IIf(INIDATE Is Not Null,1,IIf(TSCHK2DL='INTERRUPTION',2,3))) AS STATUS, 0 AS GAP, 0 AS BROKE FROM ColdTreatment").Tables(0)
            dtResult = oAppService.ExecuteSQL("EXEC tck.upGetAllColdTreatment").Tables(0)
            gcDataColdTreatment.DataSource = dtResult
            FormatDataGrid(GridView2)
            GridView2.BestFitColumns()
            SplashScreenManager.CloseForm(False)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
        End Try
    End Sub

    Private Sub FormatDataGrid(oGridView As GridView)
        For c = 0 To oGridView.Columns.Count - 1
            If oGridView.Columns(c).ReadOnly = False Then
                oGridView.Columns(c).AppearanceCell.BackColor = Color.LightGray
            End If
        Next
    End Sub
    Private Sub bbiUpdate_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiUpdate.ItemClick
        If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Are you sure to update?", "Confirmation", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.No Then
            Return
        End If
        Validate()
        Dim aResult As New ArrayList
        Dim sConditions, sValues, sFields As String
        GridView2.OptionsLayout.StoreAllOptions = True
        GridView2.ActiveFilterEnabled = False
        GridView2.ClearSorting()
        Dim info As GridViewInfo = TryCast(GridView2.GetViewInfo(), GridViewInfo)
        Dim GridRowInfo As GridRowInfo = info.GetGridRowInfo(GridView2.FocusedRowHandle)
        For r = 0 To GridView2.RowCount - 1
            If dtResult.Rows(r).RowState = DataRowState.Modified Then
                sValues = ""
                sFields = ""
                sConditions = "CONTAINER='" & GridView2.GetRowCellValue(r, "CONTAINER") & "' AND BOOKING='" & GridView2.GetRowCellValue(r, "BOOKING") & "'"
                For c = 0 To GridView2.Columns.Count - 1
                    If Not GridView2.Columns(c).OptionsColumn.ReadOnly Then
                        If IsDBNull(GridView2.GetRowCellValue(r, GridView2.Columns(c).FieldName)) Then
                            sValues = sValues & IIf(sValues = "", "", ", ") & GridView2.Columns(c).FieldName & "=NULL"
                        Else
                            If IsDate(GridView2.GetRowCellValue(r, GridView2.Columns(c).FieldName)) Then
                                sValues = sValues & IIf(sValues = "", "", ", ") & GridView2.Columns(c).FieldName & "='" & Format(CDate(GridView2.GetRowCellValue(r, GridView2.Columns(c).FieldName)), "yyyyMMdd") & "'"
                            Else
                                sValues = sValues & IIf(sValues = "", "", ", ") & GridView2.Columns(c).FieldName & "='" & GridView2.GetRowCellValue(r, GridView2.Columns(c).FieldName) & "'"
                            End If

                        End If
                    End If
                Next
                sValues += IIf(sValues = "", "", ", ") & GridView2.Columns("UpdatedBy").FieldName & "='" & My.User.Name & "'"
                sValues += IIf(sValues = "", "", ", ") & GridView2.Columns("UpdatedDate").FieldName & "='" & Format(Now, "yyyyMMdd HH:mm") & "'"
                'UpdateAccess("ColdTreatment", sConditions, sValues)
                oAppService.ExecuteSQLNonQuery("UPDATE tck.ColdTreatment SET " & sValues & " WHERE " & sConditions)
            End If
            If dtResult.Rows(r).RowState = DataRowState.Added Then
                For c = 0 To GridView2.Columns.Count - 1
                    If Not GridView2.Columns(c).OptionsColumn.ReadOnly Then
                        If IsDBNull(GridView2.GetRowCellValue(r, GridView2.Columns(c).FieldName)) Then
                            sFields = sFields & IIf(sFields = "", "", ", ") & GridView2.Columns(c).FieldName
                            sValues = sValues & IIf(sValues = "", "", ", ") & "NULL"
                        Else
                            If IsDate(GridView2.GetRowCellValue(r, GridView2.Columns(c).FieldName)) Then
                                sFields = sFields & IIf(sFields = "", "", ", ") & GridView2.Columns(c).FieldName
                                sValues = sValues & IIf(sValues = "", "", ", ") & "'" & Format(CDate(GridView2.GetRowCellValue(r, GridView2.Columns(c).FieldName)), "yyyyMMdd") & "'"
                            Else
                                sFields = sFields & IIf(sFields = "", "", ", ") & GridView2.Columns(c).FieldName
                                sValues = sValues & IIf(sValues = "", "", ", ") & "'" & GridView2.GetRowCellValue(r, GridView2.Columns(c).FieldName) & "'"
                            End If

                        End If
                    End If
                Next
                sFields = sFields & IIf(sFields = "", "", ", ") & "CreatedBy, CreatedDate"
                sValues += IIf(sValues = "", "", ", ") & "'" & My.User.Name & "'"
                sValues += IIf(sValues = "", "", ", ") & "'" & Format(Now, "yyyyMMdd HH:mm") & "'"
                aResult.AddRange(oAppService.ExecuteSQLNonQuery("INSERT INTO tck.ColdTreatment (" & sFields & ") VALUES (" & sValues & ")"))
            End If
        Next
        GridView2.ActiveFilterEnabled = True
        'bbiSearch.PerformClick()
        GridView2.MoveBy(GridRowInfo.RowHandle)
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

    Private Sub RepositoryItemHyperLinkEdit1_Click(sender As Object, e As EventArgs) Handles RepositoryItemHyperLinkEdit1.Click
        Dim TrendForm As New GraphicTrendWcfForm
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
        bbiSearch.PerformClick()
        GridView2.MoveBy(GridRowInfo.RowHandle)
    End Sub

    Private Sub beiRefresh_EditValueChanged(sender As Object, e As EventArgs) Handles beiShowGap.EditValueChanged
        'Timer1.Enabled = beiShowGap.EditValue
    End Sub

    Private Sub GridView2_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView2.FocusedRowChanged
        If GridView2.FocusedRowHandle < 0 Then
            Return
        End If
        Dim dtQueryEvt, dtQueryRdg As New DataTable
        dtQueryRdg = oAppService.ExecuteSQL("select * from tck.ColdTreatmentReadings where [BOOKING]='" & GridView2.GetFocusedRowCellValue("BOOKING") & "' and [CONTAINER] = '" & GridView2.GetFocusedRowCellValue("CONTAINER") & "'").Tables(0)
        gcVendorReadings.DataSource = dtQueryRdg
        dtQueryEvt = oAppService.ExecuteSQL("select * from tck.ColdTreatmentEvents where [BOOKING]='" & GridView2.GetFocusedRowCellValue("BOOKING") & "' and [CONTAINER] = '" & GridView2.GetFocusedRowCellValue("CONTAINER") & "'").Tables(0)
        gcEvents.DataSource = dtQueryEvt

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

    Private Sub GridView2_RowCellStyle(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs) Handles GridView2.RowCellStyle
        Dim View As GridView = sender
        If (e.RowHandle >= 0) Then
            If e.Column.FieldName = "STATUS" Then
                Dim C1 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("STATUS"))
                If C1 = "0" Then
                    e.Appearance.BackColor = Color.Green
                    e.Appearance.BackColor2 = Color.LightGreen
                ElseIf C1 = "1" Then
                    e.Appearance.BackColor = Color.Yellow
                    e.Appearance.BackColor2 = Color.LightYellow
                ElseIf C1 = "2" Then
                    e.Appearance.BackColor = Color.Red
                    e.Appearance.BackColor2 = Color.LightSalmon
                ElseIf C1 = "3" Then
                    e.Appearance.BackColor = Color.Gray
                    e.Appearance.BackColor2 = Color.LightSlateGray

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
        GridView2.MoveLast()
        GridView2.ActiveFilterString = ""
        If sender.SelectedIndex = 1 Then
            GridView2.ActiveFilterString = My.Settings.CustomDataSourceFilter
        End If
    End Sub

    Private Sub LoadOperationsCodes()
        Dim dtQuery As New DataTable
        dtQuery = oAppService.ExecuteSQL("SELECT OPS_CODE, DESCRIPTION FROM tck.OperationCode").Tables(0)
        RepositoryItemLookUpEdit2.DataSource = dtQuery
        RepositoryItemLookUpEdit2.DisplayMember = "OPS_CODE"
        RepositoryItemLookUpEdit2.ValueMember = "OPS_CODE"
    End Sub

    Private Sub bbiDelete_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiDelete.ItemClick
        If XtraMessageBox.Show("Está seguro de eliminar el registro seleccionado?", "Confirmación", MessageBoxButtons.YesNo) <> DialogResult.Yes Then Return
        oAppService.ExecuteSQLNonQuery("DELETE FROM tck.ColdTreatment WHERE [CONTAINER]='" & GridView2.GetFocusedRowCellValue("CONTAINER") & "' AND [BOOKING]='" & GridView2.GetFocusedRowCellValue("BOOKING") & "' AND [VESSEL]='" & GridView2.GetFocusedRowCellValue("VESSEL") & "'")
        bbiSearch.PerformClick()
    End Sub

    Private Sub GridView2_FocusedColumnChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs) Handles GridView2.FocusedColumnChanged
        'Dim bReadOnly As Boolean = Nothing
        'Dim editor As RepositoryItemTextEdit = gcDataColdTreatment.RepositoryItems.Add("TextEdit")
        'editor.CharacterCasing = CharacterCasing.Upper
        'For Each col As GridColumn In GridView2.Columns
        '    If col.ColumnEditName <> "RepositoryItemHyperLinkEdit1" Then '   ColumnType Is GetType(Date) Then
        '        col.ColumnEdit = editor
        '    End If
        'Next
        'Try
        '    If GridView2.FocusedColumn.FieldName <> "" Then
        '        bReadOnly = GridView2.FocusedColumn.ReadOnly
        '    End If
        '    If GridView2.FocusedRowHandle < 0 Then
        '        If GridView2.FocusedColumn.FieldName <> "" Then
        '            GridView2.Columns(GridView2.FocusedColumn.FieldName).OptionsColumn.ReadOnly = False
        '            GridView2.Columns(GridView2.FocusedColumn.FieldName).FieldName.ToUpper()
        '        End If
        '    End If
        'Catch ex As Exception
        '    'GridView2.Columns(GridView2.FocusedColumn.FieldName).OptionsColumn.ReadOnly = bReadOnly
        'End Try

    End Sub

    Private Sub bbiInsert_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiInsert.ItemClick
        GridView2.AddNewRow()
    End Sub
End Class