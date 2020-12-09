Imports System.Windows.Forms
Imports Microsoft.Office.Interop
Imports System.Data
Imports System.Collections

Public Class ManifestQuery
    Dim oLogProcessUpdate As New LogProcessUpdate
    Dim oLogFileGenerate As New LogFileGenerate
    Dim iLogProcess As Integer = 0
    Dim oMailItems As Outlook.MailItem = Nothing

    Friend Sub StartProcess(oItems As Object, sIdentifier As String)
        oMailItems = oItems
        Dim ProcessCode As String = "MNF"
        Dim QueryList As New ArrayList
        Dim sSubject(), sBody() As String
        sSubject = Split(Replace(oMailItems.Subject.ToUpper, sIdentifier, "").TrimStart, vbNewLine)
        sBody = Split(oMailItems.Body, vbNewLine)
        'Replace(oMailItems.Subject.ToUpper, sIdentifier, "").TrimEnd.TrimStart
        Try
            Dim dtResponse As New DataTable
            dtResponse = GetDataResult(sSubject)
            SendMessageResponse(dtResponse)
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate("MANIFESTQRY", "StartProcess - " & ex.Message)
        End Try

    End Sub

    Function GetDataResult(VesselName() As String) As DataTable
        Dim dtResult As New DataTable
        Dim iPos As Integer = 0
        Dim sStatus As String = ""
        Try
            If VesselName(0) = "" Then
                dtResult = ExecuteAccessQuery(drConfig("ConsultaSQL") & " WHERE [Arrival Date] BETWEEN " & Format(DateAdd(DateInterval.Day, -7, Today), "#M/d/yyyy#") & " AND " & Format(DateAdd(DateInterval.Day, 7, Today), "#M/d/yyyy#"), "").Tables(0)
            Else
                dtResult = ExecuteAccessQuery(drConfig("ConsultaSQL") & " WHERE [Vessel Name]='" & VesselName(0) & "'", "").Tables(0)
                dtResult = dtResult.Select("SELECT TOP 4").CopyToDataTable
            End If
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate("MANIFESTQRY", "GetDataResult - " & ex.Message)
        End Try
        Return dtResult
    End Function

    'Function GetTextFromMessage(sSubject() As String, sBody() As String) As ArrayList
    '    Dim aContent, aResult As New ArrayList
    '    Dim sValueText As String = ""
    '    Dim bInsert As Boolean = False
    '    For l = 0 To sSubject.Length - 1
    '        If sSubject(l) = "" Then
    '            Continue For
    '        End If
    '        If aContent.IndexOf(sSubject(l)) < 0 Then
    '            aContent.Add(sSubject(l))
    '        End If
    '    Next
    '    For l = 0 To sBody.Length - 1
    '        If sBody(l) = "" Then
    '            Continue For
    '        End If
    '        If aContent.IndexOf(sBody(l)) < 0 Then
    '            aContent.Add(sBody(l))
    '        End If
    '    Next
    '    'sSubject.
    '    'For p = 1 To sSubject.Length + 1
    '    '    If Mid(sSubject, p, 1).Contains({" ", ",", ";"}) Or p = sSubject.Length + 1 Then
    '    '        If aContent.IndexOf(sValueText) < 0 Then
    '    '            aContent.Add(sValueText)
    '    '            sValueText = ""
    '    '        End If
    '    '    End If
    '    '    sValueText += Mid(sSubject, p, 1)
    '    'Next
    '    'For p = 1 To sBody.Length + 1
    '    '    If Mid(sBody, p, 1).Contains({oSpace, ",", ";"}) Or p = sBody.Length + 1 Then
    '    '        If aContent.IndexOf(sValueText) < 0 Then
    '    '            aContent.Add(sValueText)
    '    '            sValueText = ""
    '    '        End If
    '    '    End If
    '    '    sValueText += Mid(sBody, p, 1)
    '    'Next
    '    For c = 0 To aContent.Count - 1
    '        bInsert = False
    '        If IsNumeric(aContent(c)) And aContent(c).ToString.Length = 8 Then
    '            bInsert = True
    '        End If
    '        If aContent(c).ToString.Contains("HLCU") Then 'and sFoundText.Length = 16
    '            bInsert = True
    '        End If
    '        'If sFoundText.Length = 12 And IsNumeric(Mid(sFoundText, 7, 10)) Then
    '        '    bInsert = True
    '        'End If
    '        If bInsert Then
    '            If aResult.IndexOf(aContent(c)) < 0 Then
    '                aResult.Add(aContent(c))
    '            End If
    '        End If
    '    Next
    '    Return aResult
    'End Function

    Private Sub SendMessageResponse(dtResponse As DataTable)
        Dim oMessage As New SendMessage
        Try
            oMessage.dtSourceHtml = dtResponse
            oMessage.Response(oMailItems, Nothing)
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate("MANIFESTQRY", "SendMessageResponse - " & ex.Message)
        End Try
    End Sub

    Private Sub SendErrorMessageResponse(dtResponse As DataTable)
        Dim oMessage As New SendMessage
    End Sub


End Class
