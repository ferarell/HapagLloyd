Imports System.Collections
Imports System.Data
Imports System.Windows.Forms

Public Class SendMessage
    Friend dtSourceHtml As New DataTable
    Dim TextMessage As New RichTextBox
    'Dim oGetHtmlTable As New GetHtmlTable

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
            'If oRow("TipoRespuesta") = 3 Then
            '    TextMessage.AppendText(oGetHtmlTable.GenerateTable(dtSourceHtml))
            '    TextMessage.AppendText("<br>")
            'End If
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

    Friend Function SendNewMessage(TypMsg As String, oMailItem As Outlook.MailItem, Identifier As String, Msg As String) As Boolean
        Dim NewMessage As Outlook.MailItem
        Dim AppOutlook As New Outlook.Application
        Dim bResult As Boolean = True
        NewMessage = AppOutlook.CreateItem(Outlook.OlItemType.olMailItem)
        Dim Recipents As Outlook.Recipients = NewMessage.Recipients
        Recipents.Add(oMailItem.SenderEmailAddress)
        NewMessage.BCC = My.Settings.SupportMailAddress
        NewMessage.BodyFormat = Outlook.OlBodyFormat.olFormatHTML
        If TypMsg = "OK" Then
            'Valid Subject
            NewMessage.Subject = oMailItem.Subject
            NewMessage.HTMLBody = GetValidMessageBody(oMailItem.SenderName)
        ElseIf TypMsg = "PRC_OK" Then
            NewMessage.Subject = oMailItem.Subject
            NewMessage.HTMLBody = NewMessage.HTMLBody & "<br><br>" & Msg
            If Identifier = "FLETES ONLINE" Then
                NewMessage.BCC = "pamela.marques@hlag.com"
            End If
        ElseIf TypMsg = "MSG_ERROR" Then
            'Invalid Subject
            NewMessage.Subject = "Asunto de mensaje inválido"
            NewMessage.HTMLBody = GetInvalidMessageBody(oMailItem.SenderName)
        ElseIf TypMsg = "PRC_ERROR" Then
            NewMessage.Subject = "Error al procesar " & Identifier
            Recipents.Remove(1)
            Recipents.Add("aremonfe@gmail.com")
            NewMessage.HTMLBody = "El proceso asociado al identificador " & Identifier & " ha generado un error, los datos no han sido actualizados.<br><br> "
            If Msg <> "" Then
                NewMessage.HTMLBody += "MENSAJE DE ERROR:<br>"
                NewMessage.HTMLBody += Msg
            End If
        End If
        NewMessage.Send()
        oMailItem.Close(Microsoft.Office.Interop.Outlook.OlInspectorClose.olDiscard)
        Return bResult
    End Function

    Friend Function SendErrorMessage(oMailItem As Outlook.MailItem, Identifier As String, Msg As String, Attachment As String) As Boolean
        Dim NewMessage As Outlook.MailItem
        Dim AppOutlook As New Outlook.Application
        Dim bResult As Boolean = True
        NewMessage = AppOutlook.CreateItem(Outlook.OlItemType.olMailItem)
        Dim Recipents As Outlook.Recipients = NewMessage.Recipients
        Recipents.Add(oMailItem.SenderEmailAddress)
        If Not Attachment Is Nothing Then
            If Attachment.Count > 0 Then
                NewMessage.Attachments.Add(Attachment)
            End If
        End If
        NewMessage.BCC = My.Settings.SupportMailAddress
        If Identifier = "FLETES ONLINE" Then
            NewMessage.BCC += "; pamela.marques@hlag.com"
        End If
        NewMessage.BodyFormat = Outlook.OlBodyFormat.olFormatHTML
        NewMessage.Subject = "Error al procesar " & Identifier
        Recipents.Remove(1)
        Recipents.Add("aremonfe@gmail.com")
        If Msg <> "" Then
            NewMessage.HTMLBody += "MENSAJE DE ERROR:<br>"
            NewMessage.HTMLBody += Msg
        End If
        NewMessage.Send()
        oMailItem.Close(Microsoft.Office.Interop.Outlook.OlInspectorClose.olDiscard)
        Return bResult
    End Function

    Friend Function GetInvalidMessageBody(sender As String) As String
        'Dim oText As New DevExpress.XtraRichEdit.RichEditControl
        'bIssued = False
        'Msg.Text = ""
        ''Body
        'Msg.AppendText("<html><body lang=ES style='tab-interval:35.4pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
        ''Msg.AppendText("Estimado(a) " & sender & "<br><br>")
        'Msg.AppendText(GetHtmlText(Filter, "Mensaje1", 1))
        ''Signature
        'Msg.AppendText(GetHtmlText(Filter, "Firma", 1))
        'Return Replace(Msg.Text, "[Sender]", sender)
    End Function

    Friend Function GetValidMessageBody(sender As String) As String
        'Dim oText As New DevExpress.XtraRichEdit.RichEditControl
        'Dim sResponseType As Integer = dtConfig.Rows(0)("TipoRespuesta")
        'bIssued = False
        'Msg.Text = ""
        'Msg.AppendText("<html><body lang=ES style='tab-interval:35.4pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
        ''Msg.AppendText("Estimado(a) " & sender & "<br>")
        'Msg.AppendText(GetHtmlText(Filter, "Mensaje1", sResponseType))
        'If sResponseType = 3 Then
        '    GetHtmlTable(sender, dtQuery)
        'End If
        'If sResponseType = 3 Then
        '    If bIssued Then
        '        Msg.AppendText(GetHtmlText(Filter, "Mensaje2", sResponseType))
        '    Else
        '        Msg.AppendText(GetHtmlText(Filter, "Mensaje2", 1))
        '    End If
        'Else
        '    Msg.AppendText(GetHtmlText(Filter, "Mensaje2", sResponseType))
        'End If
        'If ActiveNotice() Then
        '    Msg.AppendText(GetHtmlText(Filter, "Noticia", sResponseType))
        'End If
        ''Signature
        'Msg.AppendText(GetHtmlText(Filter, "Firma", sResponseType))
        'Msg.AppendText("</html></body>")
        'Return Replace(Msg.Text, "[Sender]", sender)
    End Function


End Class
