Imports System.Data
Imports System.Windows.Forms

Public Class BlIssuedQuery
    Dim oDataAccess As New DataAccess
    Dim oCreateMailItem As New CreateMailItem
    Friend oMailItem As Outlook.MailItem
    Friend drConfiguration As DataRow

    Friend Sub StartProcess()
        Dim bIssued As Boolean
        Dim sFecha As String = ""
        Dim sQuery As String = ""
        Dim dtCnfgLayout, dtDataQry, dtQuery As New DataTable
        BlList.Clear()
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
        dtCnfgLayout = oDataAccess.ExecuteAccessQuery("SELECT Label, Result1, Result2 FROM ConfigurationLayout WHERE IdConfiguration=" & drConfiguration("IdConfiguration") & " and ColumnType='R'").Tables(0)
        sQuery = drConfiguration("QuerySQL")
        If drConfiguration("Identifier").Contains("BL") Or drConfiguration("Identifier").Contains("SWB") Then
            dtQuery = oDataAccess.ExecuteAccessQuery(Replace(sQuery, "[BLNO]", "'#'")).Tables(0)
            For i = 0 To BlList.Count - 1
                If BlList(i).Trim <> "" Then
                    dtDataQry = oDataAccess.ExecuteAccessQuery(Replace(sQuery, "[BLNO]", "'" & BlList(i) & "'")).Tables(0)
                    bIssued = False
                    If dtDataQry.Rows.Count > 0 Then
                        bIssued = True
                        sFecha = dtDataQry.Rows(0)(2)
                        dtQuery.Rows.Add(BlList(i), dtCnfgLayout.Rows(0)("Result1"), sFecha)
                    Else
                        dtQuery.Rows.Add(BlList(i), dtCnfgLayout.Rows(0)("Result2"))
                    End If
                End If
            Next
        End If
        oMailItem.HTMLBody = oCreateMailItem.GetMessageBody(drConfiguration, oMailItem.SenderName, True, dtQuery) + oMailItem.HTMLBody
        oCreateMailItem.MessageResponse(oMailItem, oMailItem.Subject, oMailItem.HTMLBody)
    End Sub




End Class
