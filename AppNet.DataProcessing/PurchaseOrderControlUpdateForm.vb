Imports DevExpress.XtraSplashScreen
Imports Microsoft.Office.Interop
Imports System.Runtime.InteropServices
Imports System.Reflection

Public Class PurchaseOrderControlUpdateForm
    'Dim oSharePointTransactions As New SharePointListTransactions
    Dim dtList, dtCoordinator As New DataTable
    Dim oAppService As New AppService.HapagLloydServiceClient

    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        'oSharePointTransactions.SharePointUrl = My.Settings.SharePoint_Url
    End Sub

    Private Sub PurchaseOrderControlUpdateForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'bbiShowAll.PerformClick()
    End Sub

    Private Sub PurchaseOrderControlUpdateForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown

    End Sub

    Private Sub beDataSource_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beDataSource.Properties.ButtonClick
        Dim FileNames() As String
        OpenFileDialog1.Filter = "FIS Source Files (*.xls*;*.csv)|*.xls*;*.csv"
        OpenFileDialog1.FileName = ""
        'OpenFileDialog1.InitialDirectory = IIf(My.Settings.DataTargetPath <> "", My.Settings.DataTargetPath, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            FileNames = OpenFileDialog1.FileNames
            beDataSource.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub ShowAllData()
        GridControl1.DataSource = Nothing
        dtList = ExecuteAccessQuery("SELECT * FROM PurchaseOrderControl", "").Tables(0)
        If dtList.Rows.Count > 0 Then
            GridControl1.DataSource = dtList
        End If
    End Sub

    Private Sub Sincronize()
        Dim sFileName = FileIO.FileSystem.GetTempFileName
        Dim sUserProcess As String = ""
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        For f = 0 To OpenFileDialog1.FileNames.Count - 1
            If OpenFileDialog1.FileNames(f).ToUpper.Contains("XLS") Then
                SplashScreenManager.Default.SetWaitFormDescription("Importing File " & (f + 1).ToString & " of " & OpenFileDialog1.FileNames.Count.ToString)
                sFileName = OpenFileDialog1.FileNames(f).ToString
                Try
                    UpdateControlTable(sFileName)
                Catch ex As Exception
                    SplashScreenManager.CloseForm(False)
                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

            End If
        Next
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub UpdateControlTable(sFilename As String)
        Dim iPos As Integer = 0
        Dim sUserProcess As String = ""
        Dim ProcessCode As String = "POC"
        Dim iLastRowItems As Integer = 0
        Dim oLogProcessUpdate As New LogProcessUpdate
        Dim iLogProcess As Integer = oLogProcessUpdate.GetIdLogProcess(ProcessCode)
        'Dim oXls As New Excel.Application
        Dim oXls As Object = CreateObject("Excel.Application")
        oXls.Workbooks.Open(Filename:=sFilename, ReadOnly:=True)
        'oXls.Visible = False
        'Dim oSheet As New Excel.Worksheet
        'oSheet = oXls.Sheets(1)
        'Dim oRange As Excel.Range = oSheet.Range("A1:L500")
        Dim oRange As Excel.Range = oXls.Sheets(1).Range("A1:L500")
        Dim dtPurchaseOrderControl As New DataTable
        Dim WorkOrder As String = oRange.Cells(10, 5).Value.ToString
        Dim Liquidation As String = GetLiquidation(oRange, "LIQ")
        sUserProcess = oRange.Cells(12, 5).Value.ToString.Trim
        dtPurchaseOrderControl = ExecuteAccessQuery("SELECT * FROM PurchaseOrderControl WHERE WorkOrder='" & WorkOrder & "'", "").Tables(0)
        If dtPurchaseOrderControl.Rows.Count > 0 Then
            ExecuteAccessNonQuery("DELETE FROM PurchaseOrderControl WHERE WorkOrder='" & WorkOrder & "'", "")
        End If
        iLastRowItems = GetLastRowNo(oRange)
        Dim iRows As Integer = iLastRowItems
        Dim VendorCode As String = oRange.Cells(17, 1).Value.ToString()
        Dim sVessel, sVoyage As String
        sVessel = oRange.Cells(22, 1).Value.ToString.TrimEnd
        sVoyage = oRange.Cells(22, 3).Value.ToString.TrimEnd
        Try
            For r = 26 To iRows
                dtPurchaseOrderControl.Rows.Add()
                iPos = dtPurchaseOrderControl.Rows.Count - 1
                dtPurchaseOrderControl.Rows(iPos)(0) = iLogProcess
                dtPurchaseOrderControl.Rows(iPos)(1) = GetDPVoyage(oRange.Cells(22, 1).Value.ToString.Trim, oRange.Cells(22, 3).Value.ToString.Trim, oRange.Cells(22, 5).Value.ToString)
                dtPurchaseOrderControl.Rows(iPos)(2) = sVessel
                dtPurchaseOrderControl.Rows(iPos)(3) = sVoyage
                dtPurchaseOrderControl.Rows(iPos)(4) = oRange.Cells(22, 5).Value.ToString
                dtPurchaseOrderControl.Rows(iPos)(5) = oRange.Cells(17, 1).Value.ToString
                dtPurchaseOrderControl.Rows(iPos)(6) = oRange.Cells(9, 1).Value.ToString
                dtPurchaseOrderControl.Rows(iPos)(7) = WorkOrder
                dtPurchaseOrderControl.Rows(iPos)(8) = Liquidation.ToString
                dtPurchaseOrderControl.Rows(iPos)(9) = oRange.Cells(r, 1).Value
                dtPurchaseOrderControl.Rows(iPos)(10) = Replace(oRange.Cells(r, 2).Value, "'", "")
                dtPurchaseOrderControl.Rows(iPos)(11) = oRange.Cells(r, 6).Value
                dtPurchaseOrderControl.Rows(iPos)(12) = oRange.Cells(r, 7).Value
                dtPurchaseOrderControl.Rows(iPos)(13) = Now
                dtPurchaseOrderControl.Rows(iPos)(14) = GetSubjectFromFileName(sFilename)
                dtPurchaseOrderControl.Rows(iPos)(15) = "HL Perú Info"
                dtPurchaseOrderControl.Rows(iPos)(16) = sUserProcess 'Environment.UserDomainName & "\" & Environment.UserName
                dtPurchaseOrderControl.Rows(iPos)(17) = Now
                InsertIntoAccess("PurchaseOrderControl", dtPurchaseOrderControl.Rows(iPos), "")
                oLogProcessUpdate.SetLogProcessItem(iLogProcess, ProcessCode, WorkOrder.ToString, dtPurchaseOrderControl.Rows(iPos)(8), sUserProcess)
            Next
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error al actualizar la tabla PurchaseOrderControl. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Try
            GC.Collect()
            GC.WaitForPendingFinalizers()
            oXls.ActiveWorkbook.Close(False, sFilename, Missing.Value)
            oXls.Workbooks.Close()
            oXls.Quit()
            If Not oXls.Workbooks Is Nothing Then
                Marshal.ReleaseComObject(oXls.Workbooks)
            End If
            If Not oXls Is Nothing Then
                Marshal.ReleaseComObject(oXls)
            End If
            If Not oRange Is Nothing Then
                Marshal.ReleaseComObject(oRange)
            End If
            oXls = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Function GetSubjectFromFileName(sFilename As String) As String
        Dim sResult As String = ""
        sResult = Replace(Mid(sFilename, InStr(sFilename, "PTWO0001"), sFilename.Length), ".xls", "")
        Return sResult
    End Function

    Function GetLiquidation(ByVal SheetRng As Excel.Range, ByVal searchTxt As String) As String
        Dim sResult As String = ""
        Dim oRange As Excel.Range = Nothing
        oRange = FindAll(SheetRng, searchTxt)
        If Not oRange Is Nothing Then
            sResult = Replace(Replace(oRange.Value, searchTxt, ""), ".", "").Trim
        End If
        Return sResult
    End Function

    Function GetLastRowNo(oRange As Object) As Integer
        Dim iResult As Integer = 0
        Dim iPos As Integer = 26
        While IsNumeric(oRange.Cells(iPos, 1).Value)
            iResult += 1
            iPos += 1
        End While
        Return iResult + 25
    End Function

    Function GetDPVoyage(Vessel As String, Voyage As String, Port As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("SELECT DPVOYAGE FROM [Local Voyage Control] WHERE VesselName='" & Vessel & "' AND ScheduleVoyage='" & Voyage & "' AND Port_Locode='" & Port & "'", "").Tables(0)
        If dtQuery.Rows.Count = 0 Then
            dtQuery = ExecuteAccessQuery("SELECT DPVOYAGE FROM ScheduleVoyage WHERE VESSEL_NAME='" & Vessel & "' AND SCHEDULE='" & Voyage & "' AND POL='" & Port & "'", "").Tables(0)
        End If
        If dtQuery.Rows.Count = 0 Then
            Return sResult
        End If
        sResult = dtQuery.Rows(0)(0)
        Return sResult
    End Function

    Function FindAll(ByVal SheetRng As Excel.Range, ByVal searchTxt As String) As Excel.Range
        Dim currentFind As Excel.Range = Nothing
        Dim firstFind As Excel.Range = Nothing

        currentFind = SheetRng.Find(searchTxt, ,
        Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart,
        Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, False)
        While Not currentFind Is Nothing
            ' Keep track of the first range you find.
            If firstFind Is Nothing Then
                firstFind = currentFind
                ' If you didn't move to a new range, you are done.
            ElseIf currentFind.Address = firstFind.Address Then
                Exit While
            End If
            With currentFind.Font
                .Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
                .Bold = True
            End With
            currentFind = SheetRng.FindNext(currentFind)
        End While
        Return currentFind
    End Function

    Private Sub bbiSincronize_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiImport.ItemClick
        Sincronize()
    End Sub

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiShowAll.ItemClick
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Get All Purchase Orders")
            ShowAllData()
            SplashScreenManager.CloseForm(False)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(GridControl1)
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

End Class