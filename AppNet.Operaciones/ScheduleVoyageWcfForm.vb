Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading


Public Class ScheduleVoyageWcfForm
    Dim dtSPList, dtLocalPort, dtExternalPort As New DataTable
    Dim oAppService As New AppService.HapagLloydServiceClient
    Dim oSharePointTransactions As New SharePointListTransactions

    Private Sub ScheduleVoyageWcfForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Get All Schedule Voyage from SharePoint")
            oSharePointTransactions.SharePointUrl = My.Settings.SharePoint_Url
            oSharePointTransactions.SharePointList = "ScheduleVoyageList"
            oSharePointTransactions.FieldsList.Clear()
            oSharePointTransactions.FieldsList.Add({"POL"})
            oSharePointTransactions.FieldsList.Add({"DPVOYAGE"})
            oSharePointTransactions.FieldsList.Add({"VESSEL_NAME"})
            oSharePointTransactions.FieldsList.Add({"SCHEDULE"})
            oSharePointTransactions.FieldsList.Add({"SERVICE"})
            oSharePointTransactions.FieldsList.Add({"DOC_CLOSE"})
            oSharePointTransactions.FieldsList.Add({"ETA"})
            oSharePointTransactions.FieldsList.Add({"ETD"})
            dtSPList = oSharePointTransactions.GetItems()
            SplashScreenManager.CloseForm(False)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub beSourceFile_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beSourceFile.Properties.ButtonClick
        Dim FileNames() As String
        OpenFileDialog1.Filter = "FIS Source File (*.txt)|*.txt"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.InitialDirectory = IIf(My.Settings.DataTargetPath <> "", My.Settings.DataTargetPath, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            FileNames = OpenFileDialog1.FileNames
            beSourceFile.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub bbiProcesss_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesss.ItemClick
        If Not vpInputs.Validate Then
            Return
        End If
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        Try
            If rgOriginPort.SelectedIndex = 1 Then
                SplashScreenManager.Default.SetWaitFormDescription("Update Local Schedule Voyage")
                dtLocalPort = LoadTXT1(beSourceFile.Text)
                SplashScreenManager.Default.SetWaitFormDescription("Update Local Schedule Voyage (SharePoint)")
                If dtLocalPort.Rows.Count > 0 Then
                    UpdateSharePointList(dtLocalPort)
                End If
                gcMasterData.DataSource = dtLocalPort
            Else
                SplashScreenManager.Default.SetWaitFormDescription("Update Transhipment Schedule Voyage")
                dtExternalPort = LoadTXT2(beSourceFile.Text)
                gcMasterData.DataSource = dtExternalPort
            End If
            'bbiShowAll.PerformClick()
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The process has been completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateSharePointList(dtSource As DataTable)
        For r = 0 To dtSource.Rows.Count - 1
            Dim oDPVoyage, oPol As String
            oDPVoyage = dtSource.Rows(r)("DPVOYAGE")
            oPol = dtSource.Rows(r)("POL")
            If dtSPList.Select("DPVOYAGE = '" & oDPVoyage & "' AND POL = '" & oPol & "'").Length = 0 Then
                oSharePointTransactions.ValuesList.Clear()
                oSharePointTransactions.ValuesList.Add({"POL", dtSource.Rows(r)("POL")})
                oSharePointTransactions.ValuesList.Add({"DPVOYAGE", dtSource.Rows(r)("DPVOYAGE")})
                oSharePointTransactions.ValuesList.Add({"VESSEL_NAME", dtSource.Rows(r)("VESSEL_NAME")})
                oSharePointTransactions.ValuesList.Add({"SCHEDULE", dtSource.Rows(r)("SCHEDULE")})
                oSharePointTransactions.ValuesList.Add({"SERVICE", dtSource.Rows(r)("SERVICE")})
                If dtSource.Rows(r)("DOC_CLOSE").ToString <> "" Then
                    oSharePointTransactions.ValuesList.Add({"DOC_CLOSE", dtSource.Rows(r)("DOC_CLOSE")})
                End If
                oSharePointTransactions.ValuesList.Add({"ETA", dtSource.Rows(r)("ETA")})
                oSharePointTransactions.ValuesList.Add({"ETD", dtSource.Rows(r)("ETD")})
                oSharePointTransactions.InsertItem()
            End If
        Next
    End Sub

    Friend Function LoadTXT1(FileName As String) As DataTable
        Dim dtSource As New DataTable
        Dim iPosition As Integer = 0
        dtSource = oAppService.ExecuteSQL("select * from tck.ScheduleVoyage where [DPVOYAGE]=''").Tables(0)
        Try
            'SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            Using sr As New StreamReader(FileName)
                Dim lines As List(Of String) = New List(Of String)
                Dim bExit As Boolean = False
                Do While Not sr.EndOfStream
                    lines.Add(sr.ReadLine())
                Loop
                Dim bSkip As Boolean = True
                For i As Integer = 0 To lines.Count - 1
                    If Mid(lines(i), 1, 5).Trim = "-----" Then
                        i = i + 1
                    End If
                    If Mid(lines(i), 1, 6).Trim.Length = 5 Then
                        dtSource.Rows.Add()
                        iPosition = dtSource.Rows.Count - 1
                        dtSource.Rows(iPosition).Item(0) = Mid(lines(i), 1, 5)
                        dtSource.Rows(iPosition).Item(1) = Mid(lines(i), 7, 6)
                        dtSource.Rows(iPosition).Item(2) = Mid(lines(i), 14, 14)
                        dtSource.Rows(iPosition).Item(3) = Mid(lines(i), 29, 8)
                        dtSource.Rows(iPosition).Item(4) = Mid(lines(i), 38, 3)
                        dtSource.Rows(iPosition).Item(5) = CDate(Replace(Replace(Mid(lines(i), 44, 16), "-", "/"), ".", ":"))
                        dtSource.Rows(iPosition).Item(6) = CDate(Replace(Replace(Mid(lines(i), 83, 16), "-", "/"), ".", ":"))
                        dtSource.Rows(iPosition).Item(7) = CDate(Replace(Replace(Mid(lines(i), 102, 16), "-", "/"), ".", ":"))
                        DBTableUpdate(dtSource.Rows(iPosition))
                    End If
                Next
            End Using
            ''bbiShowAll.PerformClick()
            'SplashScreenManager.CloseForm(False)
            'DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The process has been completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            'SplashScreenManager.CloseForm(False)
            'DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return dtSource
    End Function

    Friend Function LoadTXT2(FileName As String) As DataTable
        Dim dtSource, dtTextFile As New DataTable
        dtTextFile = LoadCSV(FileName, True)
        If dtTextFile.Rows.Count = 0 Then
            Return dtTextFile
        End If
        dtSource = oAppService.ExecuteSQL("select * from tck.ScheduleVoyage where [DPVOYAGE]=''").Tables(0)
        Try
            'SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            For r = 0 To dtTextFile.Rows.Count - 1
                Dim oRow As DataRow = dtTextFile.Rows(r)
                dtSource.Rows.Add()
                dtSource.Rows(r).Item(0) = oRow("LOCDE")
                dtSource.Rows(r).Item(1) = oRow("DPVOY")
                dtSource.Rows(r).Item(2) = oRow("VESSEL")
                dtSource.Rows(r).Item(3) = oRow("SCHED")
                dtSource.Rows(r).Item(4) = oRow("SSY")
                'dtSource.Rows(r).Item(5) = CDate(oRow(""))
                dtSource.Rows(r).Item(6) = CDate(Replace(Replace(oRow("ARR DATE") & Space(1) & oRow("ARR TIME"), "-", "/"), ".", ":"))
                dtSource.Rows(r).Item(7) = CDate(Replace(Replace(oRow("DEP DATE") & Space(1) & oRow("DEP TIME"), "-", "/"), ".", ":"))
                DBTableUpdate(dtSource.Rows(r))
            Next
            'bbiShowAll.PerformClick()
            'SplashScreenManager.CloseForm(False)
            'DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The process has been completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            'SplashScreenManager.CloseForm(False)
            'DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return dtSource
    End Function

    Friend Function DBTableUpdate(row As DataRow) As Boolean
        Dim bResult As Boolean = True
        Dim dtSource As New DataTable
        Try
            If oAppService.ExecuteSQL("select * from tck.ScheduleVoyage where [DPVOYAGE]='" & row("DPVOYAGE") & "' and [POL]='" & row("POL") & "'").Tables(0).Rows.Count > 0 Then
                oAppService.ExecuteSQL("delete from tck.ScheduleVoyage where [DPVOYAGE]='" & row("DPVOYAGE") & "' and [POL]='" & row("POL") & "'")
            End If
            'Insertar
            'InsertIntoAccess("ScheduleVoyage", row)
            row("CreatedBy") = My.User.Name
            row("CreatedDate") = Now
            dtSource = row.Table.Clone
            dtSource.ImportRow(row)
            oAppService.InsertScheduleVoyage(dtSource)
        Catch ex As Exception
            bResult = False
        End Try
        Return bResult
    End Function

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSearch.ItemClick
        Dim dtQuery As New DataTable
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            If rgOriginPort.EditValue = "L" Then
                dtQuery = oAppService.ExecuteSQL("SELECT * FROM tck.ScheduleVoyage WHERE LEFT(POL,2) IN ('PE','CO') ").Tables(0)
            ElseIf rgOriginPort.EditValue = "T" Then
                dtQuery = oAppService.ExecuteSQL("SELECT * FROM tck.ScheduleVoyage WHERE LEFT(POL,2) NOT IN ('PE','CO') ").Tables(0)
            Else
                dtQuery = oAppService.ExecuteSQL("SELECT * FROM tck.ScheduleVoyage").Tables(0)
            End If
            gcMasterData.DataSource = dtQuery
            SplashScreenManager.CloseForm(False)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
        End Try

    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(gcMasterData)
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub LoadValidations()
        Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

        containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
        containsValidationRule.ErrorText = "Assign value."
        containsValidationRule.ErrorType = ErrorType.Critical

        Dim customValidationRule As New CustomValidationRule()
        customValidationRule.ErrorText = "Required value."
        customValidationRule.ErrorType = ErrorType.Critical

        vpInputs.SetValidationRule(Me.beSourceFile, customValidationRule)

    End Sub

    Private Sub ScheduleVoyageForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadValidations()
    End Sub
End Class