Imports System.Windows.Forms
Imports System.Data
Imports System.IO
Imports DevExpress.XtraSplashScreen

Public Class StatisticsForm
    Dim dtStatistics, dtMessages, dtIdentifiers As New DataTable

    Private Sub bbiProcess_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcess.ItemClick
        Dim dsStatistics As New dsRobot
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        Try
            dtIdentifiers = ExecuteAccessQuery("select * from RobotIdentifiersQry", "RobotStatistics.accdb").Tables(0)
            dtStatistics = dsStatistics.Tables(0)
            dtStatistics = ExecuteAccessQuery("select * from RobotStatistcs where ResponseType IS NULL", "RobotStatistics.accdb").Tables(0)
            dtMessages = ExecuteAccessQuery("select * from RobotMessagesQry where Received between " & Format(deDateFrom.EditValue, "#MM/dd/yyyy#") & " and " & Format(DateAdd(DateInterval.Day, 1, deDateTo.EditValue), "#MM/dd/yyyy#"), "RobotStatistics.accdb").Tables(0)
            gcMessages.DataSource = dtMessages
            MessagesProcess()
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
        End Try

        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub MessagesProcess()
        Dim sPeriod As String = ""
        Dim iResponseType As Integer = 0
        Dim iPos As Integer = 0
        Dim iMessage As Integer = 0
        Try
            For m = 0 To dtMessages.Rows.Count - 1
                iMessage = m
                SplashScreenManager.Default.SetWaitFormDescription("Analizando mensajes (" & (m + 1).ToString & " de " & (dtMessages.Rows.Count).ToString & ")")
                Dim drMessage As DataRow = dtMessages.Rows(m)
                sPeriod = Format(drMessage("Received"), "yyyyMM")
                For i = 0 To dtIdentifiers.Rows.Count - 1
                    Dim drIdentifier As DataRow = dtIdentifiers.Rows(i)
                    iResponseType = drIdentifier("ResponseType")
                    If drMessage("Normalized Subject").ToString.ToUpper.StartsWith(drIdentifier("Identifier")) Then
                        If dtStatistics.Select("ResponseType=" & iResponseType.ToString & " and Identifier='" & drIdentifier("Identifier") & "' and Period='" & sPeriod & "'").Length = 0 Then
                            dtStatistics.Rows.Add()
                            iPos = dtStatistics.Rows.Count - 1
                            dtStatistics.Rows(iPos)("ResponseType") = iResponseType
                            dtStatistics.Rows(iPos)("Identifier") = drIdentifier("Identifier")
                            dtStatistics.Rows(iPos)("Period") = sPeriod
                            dtStatistics.Rows(iPos)("Quantity") = 0
                        End If
                        Dim drStatistics As DataRow = dtStatistics.Select("ResponseType=" & iResponseType.ToString & " and Identifier='" & drIdentifier("Identifier") & "' and Period='" & sPeriod & "'")(0)
                        drStatistics("Quantity") += 1
                    End If
                Next
            Next
        Catch ex As Exception
            MessageBox.Show("Mensaje " & iMessage.ToString & "." & ex.Message)
        End Try
        dtStatistics = dtStatistics.Select("", "Period,ResponseType,Identifier").CopyToDataTable
        gcStatistics.DataSource = dtStatistics
        GridView1.BestFitColumns()
        ccRobotStatistics.DataSource = gcStatistics.DataSource
        ccRobotStatistics.RefreshData()
    End Sub

    Private Sub bbiSynchronize_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSynchronize.ItemClick


    End Sub

    Private Sub bbiMessage_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiMessage.ItemClick

    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub StatisticsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        bsiVersion.Caption = "Versión : " & My.Application.Info.Version.ToString
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        If gcMessages.IsFocused Then
            ExportaToExcel(gcMessages)
        End If
        If gcStatistics.IsFocused Then
            ExportaToExcel(gcStatistics)
        End If
    End Sub
End Class