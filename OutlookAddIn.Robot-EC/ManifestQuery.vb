Imports System.Windows.Forms
Imports Microsoft.Office.Interop
Imports System.Data
Imports System.Collections

Public Class ManifestQuery
    Dim oDataAccess As New DataAccess
    Dim oCreateMailItem As New CreateMailItem
    Friend oMailItem As Outlook.MailItem
    Friend drConfiguration As DataRow

    'Dim oLogProcessUpdate As New LogProcessUpdate
    Dim oLogFileGenerate As New LogFileGenerate
    'Dim iLogProcess As Integer = 0
    'Dim oMailItems As Outlook.MailItem = Nothing

    Friend Sub StartProcess()
        'Dim ProcessCode As String = "MNF"
        Dim BlList As New ArrayList
        Dim dtQuery, dtResult As New DataTable
        Dim sQuery As String = ""
        'Dim sSubject(), sBody() As String
        'sSubject = Split(Replace(oMailItem.Subject.ToUpper, sIdentifier, "").TrimStart, vbNewLine)
        'sBody = Split(oMailItem.Body, vbNewLine)
        'Replace(oMailItems.Subject.ToUpper, sIdentifier, "").TrimEnd.TrimStart
        Try
            'Find in Subject
            For s = 1 To oMailItem.Subject.Length - 1
                If Mid(oMailItem.Subject.ToUpper, s, 4) = "HLCU" Then
                    If BlList.IndexOf(Mid(oMailItem.Subject.ToUpper, s, 16)) = -1 Then
                        BlList.Add(Mid(oMailItem.Subject.ToUpper, s, 16))
                    End If
                End If
            Next
            'Find in Body
            For b = 1 To oMailItem.Body.Length - 1
                If Mid(oMailItem.Body.ToUpper, b, 4) = "HLCU" Then
                    If BlList.IndexOf(Mid(oMailItem.Body.ToUpper, b, 16)) = -1 Then
                        BlList.Add(Mid(oMailItem.Body.ToUpper, b, 16))
                    End If
                End If
            Next
            sQuery = drConfiguration("QuerySQL")
            dtResult = oDataAccess.ExecuteAccessQuery(Replace(sQuery, "[BLNO]", "'#'")).Tables(0)
            For i = 0 To BlList.Count - 1
                If BlList(i).Trim <> "" Then
                    dtQuery = oDataAccess.ExecuteAccessQuery(Replace(sQuery, "[BLNO]", "'" & BlList(i) & "'")).Tables(0)
                    If dtQuery.Rows.Count > 0 Then
                        'sFecha = dtDataQry.Rows(0)(2)
                        Dim oRow As DataRow = dtQuery.Rows(0)
                        dtResult.Rows.Add(BlList(i), oRow("ITINERARIO"), oRow("FECHA ARRIBO"), oRow("PUERTO ARRIBO"), oRow("TERMINAL"), oRow("MRN"), oRow("SECUENCIA BL"))
                    Else
                        dtResult.Rows.Add(BlList(i), Nothing, Nothing, Nothing, Nothing, Nothing, Nothing)
                    End If
                End If
            Next
            oMailItem.HTMLBody = oCreateMailItem.GetMessageBody(drConfiguration, oMailItem.SenderName, True, dtResult) '+ oMailItem.HTMLBody
            oCreateMailItem.MessageResponse(oMailItem, oMailItem.Subject, oMailItem.HTMLBody)
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate("MANIFESTQRY", "StartProcess - " & ex.Message)
        End Try

    End Sub

    'Function GetDataResult(VesselName() As String) As DataTable
    '    Dim dtResult As New DataTable
    '    Dim iPos As Integer = 0
    '    Dim sStatus As String = ""
    '    Try
    '        If VesselName(0) = "" Then
    '            dtResult = oDataAccess.ExecuteAccessQuery(drConfiguration("ConsultaSQL") & " WHERE [Arrival Date] BETWEEN " & Format(DateAdd(DateInterval.Day, -7, Today), "#M/d/yyyy#") & " AND " & Format(DateAdd(DateInterval.Day, 7, Today), "#M/d/yyyy#")).Tables(0)
    '        Else
    '            dtResult = oDataAccess.ExecuteAccessQuery(drConfiguration("ConsultaSQL") & " WHERE [Vessel Name]='" & VesselName(0) & "'").Tables(0)
    '                    dtResult = dtResult.Select("SELECT TOP 4").CopyToDataTable
    '        End If
    '    Catch ex As Exception
    '        oLogFileGenerate.TextFileUpdate("MANIFESTQRY", "GetDataResult - " & ex.Message)
    '    End Try
    '    Return dtResult
    'End Function


End Class
