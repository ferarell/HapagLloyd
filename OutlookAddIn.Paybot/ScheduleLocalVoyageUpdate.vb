Imports System
Imports System.Data
Imports System.IO
Imports System.Collections

Public Class ScheduleLocalVoyageUpdate
    Dim oAppService As New AppService.HapagLloydServiceClient
    Dim oSharePointTransactions As New SharePointListTransactions
    Dim oLogFileGenerate As New LogFileGenerate
    Dim ProcessLogName As String = "SCHLOCVOYUPD"
    Dim sFileName As String = ""

    Friend Sub StartProcess(oMailItems As Outlook.MailItem)
        Dim dtSource As New DataTable
        Dim iPosition As Integer = 0

        Dim CodeProcess As String = "SLV"
        'Dim oMailItems As Outlook.MailItem = Items
        Dim aAttachments As New ArrayList
        Dim aResult As New ArrayList

        For a = 1 To oMailItems.Attachments.Count
            If oMailItems.Attachments(a).FileName.ToUpper.Contains("TXT") Then
                sFileName = My.Settings.AttachedFilePath & "\" & Format(Now, "ddMMyyyy HHmmss") & " - " & oMailItems.Attachments(a).FileName
                Try
                    oMailItems.Attachments(a).SaveAsFile(sFileName)
                    aAttachments.Add(sFileName)
                Catch ex As Exception
                    oLogFileGenerate.TextFileUpdate(ProcessLogName, ex.Message)
                    SendErrorMessage(oMailItems, ProcessLogName, ex.Message, Nothing)
                End Try
                If Not IO.File.Exists(sFileName) Then
                    oLogFileGenerate.TextFileUpdate(ProcessLogName, "No se descargó el archivo adjunto.")
                    SendErrorMessage(oMailItems, ProcessLogName, "No se descargó el archivo adjunto.", Nothing)
                End If
            End If
        Next
        If sFileName = "" Then
            oLogFileGenerate.TextFileUpdate(ProcessLogName, "No se encontró archivo para procesar")
            SendErrorMessage(oMailItems, ProcessLogName, "No se encontró archivo para procesar", Nothing)
            Return
        End If

        dtSource = oAppService.ExecuteSQL("select * from tck.ScheduleVoyage where [DPVOYAGE]=''").Tables(0)
        Try
            Using sr As New StreamReader(sFileName)
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
                        ScheduleVoyageUpdate(dtSource.Rows(iPosition))
                    End If
                Next
            End Using
            UpdateSharePointList(dtSource)
            oLogFileGenerate.TextFileUpdate(ProcessLogName, "El proceso asociado al mensaje: " & oMailItems.Subject & " finalizó satisfactoriamente.")
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate(ProcessLogName, "SharePointList: " & oSharePointTransactions.SharePointList & " - Error: " & ex.Message)
            SendErrorMessage(oMailItems, ProcessLogName, ex.Message, Nothing)
        End Try

    End Sub

    Friend Function ScheduleVoyageUpdate(row As DataRow) As Boolean
        Dim bResult As Boolean = True
        Dim dtSource As New DataTable
        Try
            If oAppService.ExecuteSQL("select * from tck.ScheduleVoyage where [DPVOYAGE]='" & row("DPVOYAGE") & "' and [POL]='" & row("POL") & "'").Tables(0).Rows.Count > 0 Then
                oAppService.ExecuteSQL("delete from tck.ScheduleVoyage where [DPVOYAGE]='" & row("DPVOYAGE") & "' and [POL]='" & row("POL") & "'")
            End If
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

    Private Sub UpdateSharePointList(dtSource As DataTable)
        oSharePointTransactions.SharePointUrl = My.Settings.SharePoint_Url
        oSharePointTransactions.SharePointList = "ScheduleVoyageList"
        For r = 0 To dtSource.Rows.Count - 1
            Dim oDPVoyage, oPol As String
            oDPVoyage = dtSource.Rows(r)("DPVOYAGE")
            oPol = dtSource.Rows(r)("POL")
            If ExecuteAccessQuery("SELECT DPVOYAGE FROM ScheduleVoyage WHERE DPVOYAGE = '" & oDPVoyage & "' AND POL = '" & oPol & "'", "").Tables(0).Rows.Count = 0 Then
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

End Class
