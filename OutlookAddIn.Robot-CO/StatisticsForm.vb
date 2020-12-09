Imports System.Windows.Forms
Imports System.Data
Imports System.IO
Imports DevExpress.XtraSplashScreen
Imports DevExpress.XtraEditors

Public Class StatisticsForm
    Dim dtStatistics, dtMessages, dtIdentifiers As New DataTable
    Dim oDataAccess As New DataAccess
    Dim sLanguaje, ReceivedFieldName, NormalizedFieldName As String

    Private Sub bbiProcess_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcess.ItemClick
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        Try
            sLanguaje = IIf(dtMessages.Columns.Contains("Received"), "US", "ES")
            NormalizedFieldName = IIf(sLanguaje = "US", "Normalized Subject", "Asunto normalizado")
            ReceivedFieldName = IIf(sLanguaje = "US", "Received", "Recibido")
            dtIdentifiers = oDataAccess.ExecuteAccessQuery("select * from IdentifiersQry").Tables(0)
            dtStatistics = oDataAccess.ExecuteAccessQuery("select * from Statistcs where ResponseType IS NULL").Tables(0)
            dtMessages = oDataAccess.ExecuteAccessQuery("select top 1 * from MessagesQry").Tables(0)
            If sLanguaje = "US" Then
                dtMessages = oDataAccess.ExecuteAccessQuery("select From, Normalized Subject AS Subject, Receive, Message Size As Size FROM MessagesQry WHERE Received between " & Format(deDateFrom.EditValue, "#MM/dd/yyyy#") & " and " & Format(DateAdd(DateInterval.Day, 1, deDateTo.EditValue), "#MM/dd/yyyy#")).Tables(0)
            Else
                dtMessages = oDataAccess.ExecuteAccessQuery("select [De] AS [From], [Asunto normalizado] AS [Subject], [Recibido] AS [Receive], [Tamaño del mensaje] As [Size] FROM MessagesQry WHERE Recibido between " & Format(deDateFrom.EditValue, "#MM/dd/yyyy#") & " and " & Format(DateAdd(DateInterval.Day, 1, deDateTo.EditValue), "#MM/dd/yyyy#")).Tables(0)
            End If
            LoadMessages()
            gcMessages.DataSource = dtMessages
            MessagesProcess()
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            XtraMessageBox.Show(Me.LookAndFeel, "An error occurred. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub LoadMessages()

    End Sub

    Private Sub MessagesProcess()
        Dim sPeriod As String = ""
        Dim iResponseType As Integer = 0
        Dim iPos As Integer = 0
        Dim iMessage As Integer = 0
        Try
            For m = 0 To dtMessages.Rows.Count - 1
                iMessage = m
                SplashScreenManager.Default.SetWaitFormDescription("Analyzing messages (" & (m + 1).ToString & " de " & (dtMessages.Rows.Count).ToString & ")")
                Dim drMessage As DataRow = dtMessages.Rows(m)
                sPeriod = Format(drMessage("Receive"), "yyyyMM")
                For i = 0 To dtIdentifiers.Rows.Count - 1
                    Dim drIdentifier As DataRow = dtIdentifiers.Rows(i)
                    iResponseType = drIdentifier("ResponseType")

                    If drMessage("Subject").ToString.ToUpper.StartsWith(drIdentifier("Identifier")) Then
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
            XtraMessageBox.Show(Me.LookAndFeel, "An error occurred while processing messages. " & iMessage.ToString & "." & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        If dtStatistics.Select("", "Period,ResponseType,Identifier").Length > 0 Then
            dtStatistics = dtStatistics.Select("", "Period,ResponseType,Identifier").CopyToDataTable
            gcStatistics.DataSource = dtStatistics
        End If
        GridView1.BestFitColumns()
        ccRobotStatistics.DataSource = gcStatistics.DataSource
        ccRobotStatistics.RefreshData()
    End Sub

    Private Sub bbiSynchronize_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSynchronize.ItemClick


    End Sub

    Private Sub bbiMessage_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiMessage.ItemClick

    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem1.ItemClick
        Close()
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