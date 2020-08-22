Imports System.Collections
Imports System.Data
Imports System.Windows.Forms

Public Class SendMessage
    Friend dtSourceHtml As New DataTable
    Dim TextMessage As New RichTextBox
    Dim oGetHtmlTable As New GetHtmlTable

    Friend Sub Response(oMailItem As Outlook.MailItem, aFiles As ArrayList)
        Dim oNewMessage As Outlook.MailItem
        Dim AppOutlook As New Outlook.Application
        Dim bResult As Boolean = True
        oNewMessage = AppOutlook.CreateItem(Outlook.OlItemType.olMailItem)
        Dim Recipents As Outlook.Recipients = oNewMessage.Recipients
        Recipents.Add(oMailItem.SenderEmailAddress)
        oNewMessage.BodyFormat = Outlook.OlBodyFormat.olFormatHTML
        oNewMessage.Subject = oMailItem.Subject & IIf(oMailItem.Subject.Contains("(RESPUESTA)"), "", " (RESPUESTA)")
        oNewMessage.HTMLBody = GetMessageBody(oMailItem.SenderName)
        If Not aFiles Is Nothing Then
            For r = 0 To aFiles.Count - 1
                oNewMessage.Attachments.Add(aFiles(r))
            Next
        End If
        oNewMessage.Send()
    End Sub

    Friend Function GetMessageBody(SenderName As String) As String
        TextMessage.Text = ""
        TextMessage.AppendText("<html><body lang=ES style='tab-interval:35.4pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
        For r = 0 To dtConfig.Rows.Count - 1
            Dim oRow As DataRow = dtConfig.Rows(r)
            If oRow("Mensaje1") <> "" Then
                TextMessage.AppendText(oRow("Mensaje1"))
                TextMessage.AppendText("<br>")
            End If
            If oRow("TipoRespuesta") = 3 Then
                TextMessage.AppendText(oGetHtmlTable.GenerateTable(dtSourceHtml))
                TextMessage.AppendText("<br>")
            End If
            If oRow("Mensaje2") <> "" Then
                TextMessage.AppendText(oRow("Mensaje2"))
                TextMessage.AppendText("<br>")
            End If

            If oRow("Firma") <> "" Then
                TextMessage.AppendText(oRow("Firma"))
                TextMessage.AppendText("<br>")
            End If
        Next
        'If ActiveNotice() Then
        '    NewMessage.AppendText(GetHtmlText(Filter, "Noticia", sResponseType))
        'End If
        TextMessage.AppendText("</html></body>")
        TextMessage.Text = Replace(TextMessage.Text, "[Sender]", SenderName)
        Return TextMessage.Text
    End Function

End Class
