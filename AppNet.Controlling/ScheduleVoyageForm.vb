Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading

Public Class ScheduleVoyageForm
    Dim dtSource As New DataTable
    Dim oAppService As New AppService.HapagLloydServiceClient

    Friend Function LoadTXT(FileName As String) As DataTable
        Dim dtSource As New DataTable
        Dim iPosition As Integer = 0
        dtSource = ExecuteAccessQuery("select * from ScheduleVoyage where [DPVOYAGE]=''").Tables(0)
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
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
            bbiShowAll.PerformClick()
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The process has been completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return dtSource
    End Function

    Friend Function DBTableUpdate(row As DataRow) As Boolean
        Dim bResult As Boolean = True
        Try
            If ExecuteAccessQuery("select * from ScheduleVoyage where [DPVOYAGE]='" & row("DPVOYAGE") & "' AND [POL]='" & row("POL") & "'").Tables(0).Rows.Count > 0 Then
                ExecuteAccessNonQuery("delete from ScheduleVoyage where [DPVOYAGE]='" & row("DPVOYAGE") & "' AND [POL]='" & row("POL") & "'")
            End If
            InsertIntoAccess("ScheduleVoyage", row)
        Catch ex As Exception
            bResult = False
        End Try
        Return bResult
    End Function

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiShowAll.ItemClick
        gcMasterData.DataSource = ExecuteAccessQuery("SELECT * FROM ScheduleVoyage IN '" & My.Settings.DBDirectory & "\" & "dbColdTreatment.accdb'").Tables(0)
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(gcMasterData)
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    'Private Sub LoadValidations()
    '    Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

    '    containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
    '    containsValidationRule.ErrorText = "Assign value."
    '    containsValidationRule.ErrorType = ErrorType.Critical

    '    Dim customValidationRule As New CustomValidationRule()
    '    customValidationRule.ErrorText = "Required value."
    '    customValidationRule.ErrorType = ErrorType.Critical

    '    vpInputs.SetValidationRule(Me.beSourceFile, customValidationRule)

    'End Sub

    Private Sub ScheduleVoyageForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'LoadValidations()
    End Sub
End Class