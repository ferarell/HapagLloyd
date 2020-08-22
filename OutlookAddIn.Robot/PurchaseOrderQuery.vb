Imports System.Windows.Forms
Imports Microsoft.Office.Interop
Imports System.Data
Imports System.Collections

Public Class PurchaseOrderQuery
    Dim oLogProcessUpdate As New LogProcessUpdate
    Dim oLogFileGenerate As New LogFileGenerate
    Dim iLogProcess As Integer = 0
    Dim oMailItems As Outlook.MailItem = Nothing

    Friend Sub StartProcess(oItems As Object, sIdentifier As String)
        oMailItems = oItems
        Dim ProcessCode As String = "POC"
        Dim QueryList As New ArrayList
        Dim _Subject As String = Replace(oMailItems.Subject.ToUpper, sIdentifier, "").TrimStart
        'sBody = Split(oMailItems.Body, vbNewLine)
        'Replace(oMailItems.Subject.ToUpper, sIdentifier, "").TrimEnd.TrimStart
        Try
            Dim dtResponse As New DataTable
            dtResponse = GetDataResult(_Subject)
            SendMessageResponse(dtResponse)
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate("PTWOQRY", "StartProcess - " & ex.Message)
        End Try

    End Sub

    Function GetDataResult(MailSubject As String) As DataTable
        Dim dtQuery, dtResult As New DataTable
        Dim aLiquidation As New ArrayList
        Dim sLiqNo As String = ""
        'Dim iPos As Integer = 0
        'Dim sStatus As String = ""
        'Find in Subject
        For s = 1 To MailSubject.Length
            If Mid(MailSubject, s, 1) = " " Then
                Continue For
            End If
            sLiqNo += Mid(MailSubject, s, 1)
            If Mid(MailSubject, s, 1) = "," Then
                aLiquidation.Add(Replace(sLiqNo, ",", ""))
                sLiqNo = ""
            End If
        Next
        aLiquidation.Add(Replace(sLiqNo, ",", ""))
        If aLiquidation.Count = 0 Then
            Return dtResult
        End If
        Try
            dtResult = ExecuteAccessQuery(drConfig("ConsultaSQL") & " WHERE [LiqNo] = '#'", "").Tables(0)
            For r = 0 To aLiquidation.Count - 1
                dtQuery = ExecuteAccessQuery(drConfig("ConsultaSQL") & " WHERE [LiqNo] = '" & aLiquidation(r) & "'", "").Tables(0)
                If dtQuery.Rows.Count = 0 Then
                    dtQuery.Rows.Add()
                    dtQuery.Rows(0)(2) = aLiquidation(r)
                End If
                For i = 0 To dtQuery.Rows.Count - 1
                    dtResult.Rows.Add(dtQuery.Rows(i).ItemArray)
                Next
            Next
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate("PTWOQRY", "GetDataResult - " & ex.Message)
        End Try
        Return dtResult
    End Function

    Private Sub SendMessageResponse(dtResponse As DataTable)
        Dim oMessage As New SendMessage
        'Dim SubjectResponse As String = "LIQWO " & dtResponse(0)("LiqNo") & " - " & dtResponse(0)("VendorName")
        Dim SubjectResponse, VendorName As String
        VendorName = ""
        For r = 0 To dtResponse.Rows.Count - 1
            If IsDBNull(dtResponse(r)("VendorName")) Then
                Continue For
            End If
            If dtResponse(r)("VendorName") Is Nothing Then
                Continue For
            End If
            VendorName = " - " & dtResponse(r)("VendorName")
        Next
        SubjectResponse = oMailItems.Subject
        If VendorName.Length > 0 Then
            SubjectResponse += VendorName
        End If
        Try
            dtResponse.Columns.Remove("LiqNo")
            dtResponse.Columns.Remove("VendorName")
            oMessage.dtSourceHtml = dtResponse
            oMessage.Response(oMailItems, Nothing, SubjectResponse)
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate("PTWOQRY", "SendMessageResponse - " & ex.Message)
        End Try
    End Sub

    Private Sub SendErrorMessageResponse(dtResponse As DataTable)
        Dim oMessage As New SendMessage
    End Sub
End Class
