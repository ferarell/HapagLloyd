Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.XtraEditors

Public Class ReeferDataMasterForm
    'Dim dsDataTarget As New dsMain
    'Dim dtContainerList, dtNewDataCT, dtResult, dtVoyage, dtVoyageTS, dtSourceFile1, dtSourceFile2 As New DataTable
    'Dim ContainerNumber As String = ""
    Dim dtReeferDM, dtVoyageTS, dtResult, dtSourceFile1, dtSourceFile2 As New DataTable
    Dim MaxTemp As Decimal = My.Settings.MaxTemp
    'Friend oFunctions As New NetStore.CommonObjects

    Private Sub beDataFileTarget_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs)
        OpenFileDialog2.Filter = "Excel Files (*.xls*)|*.xls*"
        OpenFileDialog2.FileName = ""
        'OpenFileDialog1.InitialDirectory = IIf(My.Settings.LedgerSourceDirectory <> "", My.Settings.LedgerSourceDirectory, "")
    End Sub

    Private Sub bbiProcesss_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesss.ItemClick
        If Not vpInputs.Validate Then
            Return
        End If
        Dim dtQuery As New DataTable
        Dim dtSourceTmp1, dtSourceTmp2 As New DataTable
        dtSourceFile1.Rows.Clear()
        dtSourceFile2.Rows.Clear()
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            Dim dtMainName As String = ""
            For i = 0 To OpenFileDialog1.FileNames.Count - 1
                SplashScreenManager.Default.SetWaitFormDescription("Loading Data Sources (File " & (i + 1).ToString & " of " & OpenFileDialog1.FileNames.Count.ToString & ")")

                If OpenFileDialog1.FileNames(i).ToUpper.Contains(".XLS") Then
                    'FIS (EQEO0801)
                    If LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Columns.Count <= 33 Then
                        dtSourceTmp1 = QueryExcel(OpenFileDialog1.FileNames(i), "SELECT F1 AS [POL],F4 AS [EqpType],F5 AS [Container],F12 AS [Booking],F13 AS [MainType],F16 AS [DPVoyage],F20 AS [SpecialProduct],F25 AS [TSP],F27 AS [ShipperMR_Name],F28 AS [ShipperMR_Code],F29 AS [POD],F30 AS [CommodityDescription] FROM [Stock Change Units$] WHERE F1 IS NOT NULL").Tables(0)
                        InsertDataFile1(dtSourceTmp1)
                        Continue For
                    End If
                    'FIS (T8000)
                    'If LoadExcel(OpenFileDialog1.FileNames(i), "Transshipment Request List$").Tables(0).Columns.Count >= 65 Then
                    If QueryExcel(OpenFileDialog1.FileNames(i), "SELECT TOP 1 * FROM [Transshipment Request List$]").Tables(0).Columns.Count >= 65 Then
                        dtSourceTmp2 = QueryExcel(OpenFileDialog1.FileNames(i), "SELECT CStr(F4) AS [Booking],F13 AS [Container],F50 AS [TSP],CStr(F51) AS [DPVoyage2],F52 AS [VesselName2],F45 AS [ArrivalTSP],F53 AS [Departure2],F67 AS [POD] FROM [Transshipment Request List$] WHERE F1 IS NOT NULL").Tables(0)
                        InsertDataFile2(dtSourceTmp2)
                        Continue For
                    End If
                    'FIS (T8500)
                    'If LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Columns.Count >= 65 And LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Columns.Count <= 69 Then
                    '    dtSourceTmp2 = LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0)
                    '    InsertDataFile2(dtSourceTmp2)
                    '    Continue For
                    'End If
                End If
            Next
            If dtSourceFile1.Rows.Count > 0 Then
                If DataProcess1() Then
                    SplashScreenManager.CloseForm(False)
                    gcMainData.DataSource = dtReeferDM
                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The process has been completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                If DataProcess2() Then
                    SplashScreenManager.CloseForm(False)
                    gcMainData.DataSource = dtReeferDM
                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The process has been completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
    End Sub

    Friend Function DataProcess1() As Boolean
        Dim bResult As Boolean = True
        Dim ContainerNumber, Vessel, DPVoyage, sCondition, sValues, sTransaction As String
        Dim dtQuery, dtColdTreatment, dtScheduleVoyage1, dtTranshipment As New DataTable
        Dim iPos As Integer = 0
        dtReeferDM.Rows.Clear()
        dtReeferDM = ExecuteAccessQuery("SELECT * FROM ReeferDataMaster WHERE Booking IS NULL").Tables(0)
        For r = 0 To dtSourceFile1.Rows.Count - 1
            Try
                SplashScreenManager.Default.SetWaitFormDescription("Update Reefer Data Master (Row: " & (r + 1).ToString & " of " & dtSourceFile1.Rows.Count.ToString & ")")
                Dim oRow As DataRow = dtSourceFile1.Rows(r + 1)
                oRow("Booking") = IIf(IsDBNull(oRow("Booking")), 0, oRow("Booking"))
                oRow("Container") = IIf(IsDBNull(oRow("Container")), "", oRow("Container"))
                If oRow("Booking") = 0 Or oRow("Container") = "" Then
                    Continue For
                End If
                dtTranshipment.Rows.Clear()
                ContainerNumber = Replace(oRow("Container"), " ", "")
                DPVoyage = "000000"
                If Not IsDBNull(oRow("DPVoyage")) Then
                    DPVoyage = Format(CInt(oRow("DPVoyage")), "000000")
                End If
                dtColdTreatment = ExecuteAccessQuery("select * from ColdTreatment where Container = '" & ContainerNumber & "' and Booking = '" & oRow("Booking").ToString & "'").Tables(0)
                If dtSourceFile2.Rows.Count > 0 Then
                    If dtSourceFile2.Select("Container = '" & oRow("Container") & "' and Booking = '" & oRow("Booking").ToString & "'").Length > 0 Then
                        dtTranshipment = dtSourceFile2.Select("Container = '" & oRow("Container") & "' and Booking = '" & oRow("Booking").ToString & "'").CopyToDataTable
                    End If
                End If
                dtScheduleVoyage1 = ExecuteAccessQuery("select * from ScheduleVoyage where POL = '" & oRow("POL") & "' and DPVOYAGE = '" & DPVoyage & "'").Tables(0)
                dtQuery = ExecuteAccessQuery("select * from ReeferDataMaster where Container = '" & ContainerNumber & "' and Booking = '" & oRow("Booking").ToString & "'").Tables(0)
                If dtQuery.Rows.Count > 0 Then
                    sTransaction = "Update"
                    sCondition = "Container = '" & ContainerNumber & "' and Booking = '" & oRow("Booking").ToString & "'"
                    sValues = ""
                    If Not IsDBNull(oRow("EqpType")) Then
                        sValues += IIf(sValues = "", "", ", ") & "EqpType='" & oRow("EqpType") & "'"
                    End If
                    If Not IsDBNull(oRow("MainType")) Then
                        sValues += IIf(sValues = "", "", ", ") & "MainType='" & oRow("MainType") & "'"
                    End If
                    If Not IsDBNull(oRow("SpecialProduct")) Then
                        sValues += IIf(sValues = "", "", ", ") & "SpecialProduct='" & oRow("SpecialProduct") & "'"
                    End If
                    If dtColdTreatment.Rows.Count > 0 Then
                        sValues += IIf(sValues = "", "", ", ") & "IsColdTreatment=1"
                    End If
                    If dtScheduleVoyage1.Rows.Count > 0 Then
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("POL")) Then
                            sValues += IIf(sValues = "", "", ", ") & "POL='" & dtScheduleVoyage1.Rows(0)("POL") & "'"
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("ETD")) Then
                            sValues += IIf(sValues = "", "", ", ") & "Departure1='" & dtScheduleVoyage1.Rows(0)("ETD") & "'"
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("DPVOYAGE")) Then
                            sValues += IIf(sValues = "", "", ", ") & "DPVoyage1='" & Format(CInt(dtScheduleVoyage1.Rows(0)("DPVOYAGE")), "000000") & "'"
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("VESSEL_NAME")) Then
                            sValues += IIf(sValues = "", "", ", ") & "VesselName1='" & dtScheduleVoyage1.Rows(0)("VESSEL_NAME") & "'"
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("SCHEDULE")) Then
                            sValues += IIf(sValues = "", "", ", ") & "VesselVoyage1='" & dtScheduleVoyage1.Rows(0)("SCHEDULE") & "'"
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("SERVICE")) Then
                            sValues += IIf(sValues = "", "", ", ") & "Service='" & dtScheduleVoyage1.Rows(0)("SERVICE") & "'"
                        End If
                    End If
                    If Not IsDBNull(oRow("TSP")) Then
                        sValues += IIf(sValues = "", "", ", ") & "TSP='" & oRow("TSP") & "'"
                    End If
                    If dtTranshipment.Rows.Count > 0 Then
                        If Not IsDBNull(dtTranshipment.Rows(0)("ArrivalTSP")) Then
                            sValues += IIf(sValues = "", "", ", ") & "ArrivalTSP='" & dtTranshipment.Rows(0)("ArrivalTSP") & "'"
                        End If
                        dtReeferDM.Rows(iPos).Item("Notify2") = 0
                        If Not IsDBNull(dtTranshipment.Rows(0)("Departure2")) Then
                            sValues += IIf(sValues = "", "", ", ") & "Departure2='" & dtTranshipment.Rows(0)("Departure2") & "'"
                        End If
                        If Not IsDBNull(dtTranshipment.Rows(0)("DPVoyage2")) Then
                            sValues += IIf(sValues = "", "", ", ") & "DPVoyage2='" & Format(CInt(dtTranshipment.Rows(0)("DPVoyage2")), "000000") & "'"
                        End If
                        If Not IsDBNull(dtTranshipment.Rows(0)("VesselName2")) Then
                            sValues += IIf(sValues = "", "", ", ") & "VesselName2='" & dtTranshipment.Rows(0)("VesselName2") & "'"
                        End If
                        If Not IsDBNull(dtTranshipment.Rows(0)("ArrivalPOD")) Then
                            sValues += IIf(sValues = "", "", ", ") & "ArrivalPOD='" & dtTranshipment.Rows(0)("ArrivalPOD") & "'"
                        End If
                    End If
                    If Not IsDBNull(oRow("POD")) Then
                        sValues += IIf(sValues = "", "", ", ") & "POD='" & oRow("POD") & "'"
                    End If
                    sValues += IIf(sValues = "", "", ", ") & "UpdatedBy='" & My.User.Name & "'"
                    sValues += IIf(sValues = "", "", ", ") & "UpdatedDate='" & Now.ToString & "'"

                    UpdateAccess("ReeferDataMaster", sCondition, sValues)
                Else
                    sTransaction = "Insert"
                    dtReeferDM.Rows.Add()
                    iPos = dtReeferDM.Rows.Count - 1
                    dtReeferDM.Rows(iPos).Item("Booking") = oRow("Booking")
                    dtReeferDM.Rows(iPos).Item("Container") = ContainerNumber
                    dtReeferDM.Rows(iPos).Item("EqpType") = oRow("EqpType")
                    dtReeferDM.Rows(iPos).Item("MainType") = oRow("MainType")
                    If Not IsDBNull(oRow("SpecialProduct")) Then
                        dtReeferDM.Rows(iPos).Item("SpecialProduct") = oRow("SpecialProduct")
                    End If
                    If dtColdTreatment.Rows.Count > 0 Then
                        dtReeferDM.Rows(iPos).Item("IsColdTreatment") = 1
                    End If
                    If Not IsDBNull(oRow("ShipperMR_Name")) And Not IsDBNull(oRow("ShipperMR_Code")) Then
                        dtReeferDM.Rows(iPos).Item("ShipperMR") = oRow("ShipperMR_Name").ToString.Trim & Space(1) & Format(CInt(oRow("ShipperMR_Code")), "000")
                    End If
                    If Not IsDBNull(oRow("CommodityDescription")) Then
                        dtReeferDM.Rows(iPos).Item("CommodityDescription") = Replace(oRow("CommodityDescription"), "'", " ")
                    End If
                    If dtScheduleVoyage1.Rows.Count > 0 Then
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("POL")) Then
                            dtReeferDM.Rows(iPos).Item("POL") = dtScheduleVoyage1.Rows(0)("POL")
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("ETD")) Then
                            dtReeferDM.Rows(iPos).Item("Departure1") = dtScheduleVoyage1.Rows(0)("ETD")
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("DPVOYAGE")) Then
                            dtReeferDM.Rows(iPos).Item("DPVoyage1") = Format(CInt(dtScheduleVoyage1.Rows(0)("DPVOYAGE")), "000000")
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("VESSEL_NAME")) Then
                            dtReeferDM.Rows(iPos).Item("VesselName1") = dtScheduleVoyage1.Rows(0)("VESSEL_NAME")
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("SCHEDULE")) Then
                            dtReeferDM.Rows(iPos).Item("VesselVoyage1") = dtScheduleVoyage1.Rows(0)("SCHEDULE")
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("SERVICE")) Then
                            dtReeferDM.Rows(iPos).Item("Service") = dtScheduleVoyage1.Rows(0)("SERVICE")
                        End If
                    End If
                    If Not IsDBNull(oRow("TSP")) Then
                        dtReeferDM.Rows(iPos).Item("TSP") = oRow("TSP")
                    End If
                    If dtTranshipment.Rows.Count > 0 Then
                        If Not IsDBNull(dtTranshipment.Rows(0)("ArrivalTSP")) Then
                            dtReeferDM.Rows(iPos).Item("ArrivalTSP") = dtTranshipment.Rows(0)("ArrivalTSP")
                        End If
                        dtReeferDM.Rows(iPos).Item("Notify2") = 0
                        If Not IsDBNull(dtTranshipment.Rows(0)("Departure2")) Then
                            dtReeferDM.Rows(iPos).Item("Departure2") = dtTranshipment.Rows(0)("Departure2")
                        End If
                        If Not IsDBNull(dtTranshipment.Rows(0)("DPVoyage2")) Then
                            dtReeferDM.Rows(iPos).Item("DPVoyage2") = Format(CInt(dtTranshipment.Rows(0)("DPVoyage2")), "000000")
                        End If
                        If Not IsDBNull(dtTranshipment.Rows(0)("VesselName2")) Then
                            dtReeferDM.Rows(iPos).Item("VesselName2") = dtTranshipment.Rows(0)("VesselName2")
                        End If
                        If Not IsDBNull(dtTranshipment.Rows(0)("ArrivalPOD")) Then
                            dtReeferDM.Rows(iPos).Item("ArrivalPOD") = dtTranshipment.Rows(0)("ArrivalPOD")
                        End If
                    End If
                    If Not IsDBNull(oRow("POD")) Then
                        dtReeferDM.Rows(iPos).Item("POD") = oRow("POD")
                    End If
                    dtReeferDM.Rows(iPos).Item("CreatedBy") = My.User.Name
                    dtReeferDM.Rows(iPos).Item("CreatedDate") = Now
                    'dtReeferDM.Rows(iPos).Item("TransitDays") = oRow("")
                    'dtReeferDM.Rows(iPos).Item("Comments") = "" 'oRow("")
                    InsertIntoAccess("ReeferDataMaster", dtReeferDM.Rows(iPos))
                End If
            Catch ex As Exception
                'DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Next
        Return bResult
    End Function

    Friend Function DataProcess2() As Boolean
        Dim bResult As Boolean = True
        Dim ContainerNumber, DPVoyage, sCondition, sValues As String
        Dim dtQuery, dtColdTreatment, dtScheduleVoyage1, dtTranshipment As New DataTable
        Dim drScheduleVoyage2 As DataRow
        dtReeferDM.Rows.Clear()
        dtReeferDM = ExecuteAccessQuery("SELECT * FROM ReeferDataMaster WHERE Booking IS NULL").Tables(0)
        For r = 0 To dtSourceFile2.Rows.Count - 1
            Try
                SplashScreenManager.Default.SetWaitFormDescription("Update Reefer Data Master (Row: " & (r + 1).ToString & " of " & dtSourceFile2.Rows.Count.ToString & ")")
                Dim oRow As DataRow = dtSourceFile2.Rows(r)
                oRow("Booking") = IIf(IsDBNull(oRow("Booking")), 0, oRow("Booking"))
                oRow("Container") = IIf(IsDBNull(oRow("Container")), "", oRow("Container"))
                If oRow("Booking").ToString = "" Or oRow("Container") = "" Then
                    Continue For
                End If
                If oRow(0) = "Shipment" Then
                    Continue For
                End If
                dtTranshipment.Rows.Clear()
                ContainerNumber = Replace(oRow("Container"), " ", "")
                DPVoyage = Format(CInt(oRow("DPVoyage2")), "000000")
                dtScheduleVoyage1 = ExecuteAccessQuery("select * from ScheduleVoyage where POL = '" & oRow("TSP") & "' and DPVOYAGE = '" & DPVoyage & "'").Tables(0)
                'If ExecuteAccessQuery("select * from ScheduleVoyage where POL = '" & oRow("POD") & "' and DPVOYAGE = '" & DPVoyage & "'").Tables(0).Rows.Count > 0 Then
                '    drScheduleVoyage2 = ExecuteAccessQuery("select * from ScheduleVoyage where POL = '" & oRow("POD") & "' and DPVOYAGE = '" & DPVoyage & "'").Tables(0)(0)
                'End If
                dtQuery = ExecuteAccessQuery("select * from ReeferDataMaster where Container = '" & ContainerNumber & "' and Booking = '" & oRow("Booking").ToString & "'").Tables(0)
                If dtQuery.Rows.Count = 0 Then
                    Continue For
                End If
                sCondition = "Container = '" & ContainerNumber & "' and Booking = '" & oRow("Booking").ToString & "'"
                sValues = ""
                If Not IsDBNull(oRow("ArrivalTSP")) Then
                    If dtSourceFile2.Columns("ArrivalTSP").DataType = GetType(Date) Then
                        sValues += IIf(sValues = "", "", ", ") & "ArrivalTSP=" & Format(CDate(oRow("ArrivalTSP")), "#MM/dd/yyyy#")
                    Else
                        sValues += IIf(sValues = "", "", ", ") & "ArrivalTSP=#" & oRow("ArrivalTSP") & "#"
                    End If
                End If
                If Not IsDBNull(oRow("Departure2")) Then
                    If dtSourceFile2.Columns("Departure2").DataType = GetType(Date) Then
                        sValues += IIf(sValues = "", "", ", ") & "Departure2=" & Format(CDate(oRow("Departure2")), "#MM/dd/yyyy#")
                    Else
                        sValues += IIf(sValues = "", "", ", ") & "Departure2=#" & oRow("Departure2") & "#"
                    End If
                End If
                If Not IsDBNull(oRow("DPVoyage2")) Then
                    sValues += IIf(sValues = "", "", ", ") & "DPVoyage2='" & DPVoyage & "'"
                End If
                If Not IsDBNull(oRow("VesselName2")) Then
                    sValues += IIf(sValues = "", "", ", ") & "VesselName2='" & oRow("VesselName2") & "'"
                End If
                If Not IsDBNull(oRow("POD")) Then
                    sValues += IIf(sValues = "", "", ", ") & "POD='" & oRow("POD") & "'"
                End If

                UpdateAccess("ReeferDataMaster", sCondition, sValues)
            Catch ex As Exception
                'DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Next
        Return bResult
    End Function

    Private Sub FindEta2(book As String, ctn As String)
        Dim dResult As DateTime = Nothing
        Dim dpvoyage2 As String = ""
        If dtSourceFile2.Select("[Shipment] = '" & book & "' and [Cont#Number] = '" & ctn & "'").Length > 0 Then
            dpvoyage2 = Format(CInt(dtSourceFile2.Select("[Shipment] = '" & book & "' and [Cont#Number] = '" & ctn & "'")(0)("MC DP-Voyage No#")), "000000")
            dtVoyageTS.Rows.Clear()
            dtVoyageTS = ExecuteAccessQuery("select * from ScheduleVoyage where [DPVOYAGE]='" & dpvoyage2 & "'").Tables(0)
        End If
    End Sub

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

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(gcMainData)
    End Sub

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

    Private Sub ReeferDataMasterForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        bbiExport.Enabled = False
        bbiUpdate.Enabled = False
        bbiMessage.Enabled = False
        LoadValidations()
        beiRefresh.EditValue = False
        GridView1.RestoreLayoutFromRegistry(Directory.GetCurrentDirectory)
        'Timer1.Start()
    End Sub

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiShowAll.ItemClick
        dtResult.Rows.Clear()
        dtResult = ExecuteAccessQuery("SELECT * FROM qryReeferDataMaster").Tables(0)
        For Each oRow As DataRow In dtResult.Rows
            oRow("IsColdTreatment") = oRow("IsColdTreatment1")
        Next
        gcMainData.DataSource = dtResult
    End Sub

    Private Sub bbiUpdate_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiUpdate.ItemClick
        Dim dtResult2 As DataTable = ExecuteAccessQuery("SELECT * FROM qryReeferDataMaster").Tables(0)
        Validate()
        Dim sConditions, sValues As String
        GridView1.OptionsLayout.StoreAllOptions = True
        GridView1.ActiveFilterEnabled = False
        GridView1.ClearSorting()
        Dim info As GridViewInfo = TryCast(GridView1.GetViewInfo(), GridViewInfo)
        Dim GridRowInfo As GridRowInfo = info.GetGridRowInfo(GridView1.FocusedRowHandle)
        Try

            For r = 0 To GridView1.RowCount - 1
                Dim dtRowR As DataRow = dtResult2.Rows(r)
                Dim dtRowQ As DataRow = GridView1.GetDataRow(r)
                Dim comparer As IEqualityComparer(Of DataRow) = DataRowComparer.Default
                Dim bEqual = comparer.Equals(dtRowR, dtRowQ)
                If bEqual Then
                    Continue For
                End If
                sConditions = "Container='" & GridView1.GetRowCellValue(r, "Container") & "' AND Booking='" & GridView1.GetRowCellValue(r, "Booking") & "'"
                sValues = ""
                For c = 0 To GridView1.Columns.Count - 1
                    If Not GridView1.Columns(c).OptionsColumn.ReadOnly Then
                        'If Not drValues.Table.Columns(dtSchema.Rows.IndexOf(row)).DataType = GetType(Boolean) Then
                        '    sValues = sValues + IIf(dtSchema.Rows.IndexOf(row) = 0, "'", ", '") & drValues.Item(dtSchema.Rows.IndexOf(row)) & "'"
                        'Else
                        '    sValues = sValues & ", " & drValues.Item(dtSchema.Rows.IndexOf(row))
                        'End If
                        If IsDBNull(GridView1.GetRowCellValue(r, GridView1.Columns(c).FieldName)) Then
                            sValues = sValues & IIf(sValues = "", "", ", ") & GridView1.Columns(c).FieldName & "=NULL"
                        Else
                            If GridView1.Columns(c).ColumnType = GetType(Boolean) Then
                                sValues = sValues & IIf(sValues = "", "", ", ") & GridView1.Columns(c).FieldName & "=" & GridView1.GetRowCellValue(r, GridView1.Columns(c).FieldName)
                            Else
                                sValues = sValues & IIf(sValues = "", "", ", ") & GridView1.Columns(c).FieldName & "='" & GridView1.GetRowCellValue(r, GridView1.Columns(c).FieldName) & "'"
                            End If
                        End If
                    End If
                Next
                UpdateAccess("ReeferDataMaster", sConditions, sValues)
            Next
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The data has been updated successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The data was not updated successfully", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        GridView1.ActiveFilterEnabled = True
        bbiShowAll.PerformClick()
        GridView1.MoveBy(GridRowInfo.RowHandle)
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

    Private Sub RepositoryItemHyperLinkEdit1_Click(sender As Object, e As EventArgs)
        Dim TrendForm As New GraphicTrendForm
        TrendForm.pBooking = GridView1.GetFocusedRowCellValue("BOOKING")
        TrendForm.pContainer = GridView1.GetFocusedRowCellValue("CONTAINER")
        TrendForm.ShowDialog()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim info As GridViewInfo = TryCast(GridView1.GetViewInfo(), GridViewInfo)
        Dim GridRowInfo As GridRowInfo = info.GetGridRowInfo(GridView1.FocusedRowHandle)
        bbiShowAll.PerformClick()
        GridView1.MoveBy(GridRowInfo.RowHandle)
    End Sub

    Private Sub beiRefresh_EditValueChanged(sender As Object, e As EventArgs) Handles beiRefresh.EditValueChanged
        Timer1.Enabled = beiRefresh.EditValue
    End Sub

    Private Sub GridView2_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView1.FocusedRowChanged
        bbiExport.Enabled = True
        bbiUpdate.Enabled = True
        bbiMessage.Enabled = True
        If GridView1.RowCount = 0 Then
            bbiExport.Enabled = False
            bbiUpdate.Enabled = False
            bbiMessage.Enabled = False
            Return
        End If
    End Sub

    Private Sub SeleccionaFilas(caso As Integer)
        Dim i As Integer = 0
        Do While i < GridView1.RowCount
            Dim row As DataRow = GridView1.GetDataRow(i)
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
        GridView1.ActiveFilter.Clear()
        GridView1.SaveLayoutToRegistry(Directory.GetCurrentDirectory)
        My.Settings.CustomDataSourceFilter = GridView1.ActiveFilterString
        My.Settings.Save()
    End Sub

    Private Sub rgFilter_SelectedIndexChanged(sender As Object, e As EventArgs)
        GridView1.ActiveFilterString = ""
        If sender.SelectedIndex = 1 Then
            GridView1.ActiveFilterString = My.Settings.CustomDataSourceFilter
        End If
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

    Private Sub bbiMessage_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiMessage.ItemClick
        Dim sPath As String = Path.GetTempPath
        Dim sFileName = (FileIO.FileSystem.GetTempFileName).Replace(".tmp", ".xlsx")
        GridView2.ActiveFilterString = GridView1.ActiveFilterString
        gcMainData.MainView = GridView2
        GridView2.OptionsPrint.AutoWidth = False
        GridView2.BestFitMaxRowCount = GridView2.RowCount
        GridView2.ExportToXlsx(sFileName)
        If IO.File.Exists(sFileName) Then
            CreateSendItem("QLIK REEFER SALES", "", "RS", sFileName)
        End If
        gcMainData.MainView = GridView1
    End Sub

    Private Sub ShownEditor(ByVal sender As Object, ByVal e As EventArgs) Handles GridView1.ShownEditor
        Dim view As GridView = TryCast(sender, GridView)
        view.GridControl.BeginInvoke(New MethodInvoker(Sub()
                                                           Dim edit As PopupBaseEdit = TryCast(view.ActiveEditor, PopupBaseEdit)
                                                           If edit Is Nothing Then
                                                               Return
                                                           End If
                                                           edit.ShowPopup()
                                                       End Sub))
    End Sub
End Class