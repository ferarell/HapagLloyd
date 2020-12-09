Imports DevExpress.XtraSplashScreen
Imports Microsoft.Office.Interop
Imports System.Collections
Imports System.Data
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms

Public Class CreateMailItem
    'Dim AppOutlook As New Outlook.Application
    Dim AppOutlook As Microsoft.Office.Interop.Outlook.Application = CType(Activator.CreateInstance(Type.GetTypeFromCLSID(New Guid("0006F03A-0000-0000-C000-000000000046"))), Microsoft.Office.Interop.Outlook.Application)
    Dim mail As Outlook.MailItem = Nothing
    Dim mailRecipients As Outlook.Recipients = Nothing
    Dim mailRecipient As Outlook.Recipient = Nothing
    Dim mailBody As String = ""
    Friend mailHtmlBody As New RichTextBox
    Friend mailSubject, mailTo, mailCc, mailBcc As String
    Friend mailAttachment As New ArrayList
    Friend Identifier As String
    Dim bIssued As Boolean = Nothing

    Friend Function CreateCustomMessage(TypeMessage As String, IsNewMessage As Boolean, RecipientInclude As Boolean) As String
        Dim sResult As String = ""
        Try
            If Not IsNewMessage Then
                mail = CType(AppOutlook.CreateItem(Outlook.OlItemType.olMailItem), Outlook.MailItem)
            End If
            Dim oInspector As Outlook.Inspector = mail.GetInspector
            If Not mailTo Is Nothing Then
                mail.To = mailTo
            End If
            If Not mailCc Is Nothing Then
                mail.CC = mailCc
            End If
            If Not mailBcc Is Nothing Then
                mail.BCC = mailBcc
            End If
            If Not mailAttachment Is Nothing Then
                For a = 0 To mailAttachment.Count - 1
                    If IO.File.Exists(mailAttachment(a)) Then
                        mail.Attachments.Add(mailAttachment(a))
                    End If
                Next
            End If
            mail.Subject = mailSubject
            mail.HTMLBody = mailHtmlBody.Text '+ mail.HTMLBody
            If RecipientInclude Then
                'mail.Recipients.Add(mailRecipient)
            End If
            If TypeMessage = "Display" Then
                mail.Display(mail)
            Else
                mail.Send()
            End If
            SplashScreenManager.CloseForm(False)
        Catch ex As Exception
            sResult = ex.Message
            SplashScreenManager.CloseForm(False)
            System.Windows.Forms.MessageBox.Show(ex.Message,
                "An exception is occured in the code of add-in.")
        Finally
            If Not IsNothing(mailRecipient) Then Marshal.ReleaseComObject(mailRecipient)
            If Not IsNothing(mailRecipients) Then Marshal.ReleaseComObject(mailRecipients)
            If Not IsNothing(mail) Then Marshal.ReleaseComObject(mail)
        End Try
        Return sResult
    End Function

    Friend Function MessageResponse(oMailItem As Outlook.MailItem, ResponseMailSubject As String, ResponseMailBody As String) As Boolean
        Dim NewMessage As Outlook.MailItem
        Dim bResult As Boolean = True
        'Dim AppOutlook As New Outlook.Application
        NewMessage = AppOutlook.CreateItem(Outlook.OlItemType.olMailItem)
        Dim Recipents As Outlook.Recipients = NewMessage.Recipients
        Recipents.Add(oMailItem.SenderEmailAddress)
        NewMessage.BodyFormat = Outlook.OlBodyFormat.olFormatHTML
        NewMessage.Subject = ResponseMailSubject
        NewMessage.HTMLBody = ResponseMailBody
        NewMessage.Send()
        Return bResult
    End Function

    Friend Function GetInvalidMessageBody(sender As String) As String
        Dim oText As New DevExpress.XtraRichEdit.RichEditControl
        mailHtmlBody.Text = ""
        'Body
        mailHtmlBody.AppendText("<html><body lang=ES style='tab-interval:35.4pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
        mailHtmlBody.AppendText(GetHtmlText(Identifier, "Body", 0))
        'Signature
        mailHtmlBody.AppendText(GetHtmlText(Identifier, "Signature", 0))
        Return Replace(mailHtmlBody.Text, "[Sender]", sender)
    End Function

    'Friend Function GetValidMessageBody(sender As String) As String
    '    Dim oText As New DevExpress.XtraRichEdit.RichEditControl
    '    Dim sResponseType As Integer = dtConfig.Rows(0)("ResponseType")
    '    bIssued = False
    '    mailHtmlBody.Text = ""
    '    mailHtmlBody.AppendText("<html><body lang=ES style='tab-interval:35.4pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
    '    'Msg.AppendText("Estimado(a) " & sender & "<br>")
    '    mailHtmlBody.AppendText(GetHtmlText(Identifier, "Header", sResponseType))
    '    If sResponseType = 3 Then
    '        GetHtmlTable(sender, dtQuery)
    '    End If
    '    If sResponseType = 3 Then
    '        If bIssued Then
    '            mailHtmlBody.AppendText(GetHtmlText(Identifier, "Body", sResponseType))
    '        Else
    '            mailHtmlBody.AppendText(GetHtmlText(Identifier, "Body", 1))
    '        End If
    '    Else
    '        mailHtmlBody.AppendText(GetHtmlText(Identifier, "Body", sResponseType))
    '    End If
    '    If ActiveNotice(drConfiguration) Then
    '        mailHtmlBody.AppendText(GetHtmlText(Identifier, "News", sResponseType))
    '    End If
    '    'Signature
    '    mailHtmlBody.AppendText(GetHtmlText(Identifier, "Signature", sResponseType))
    '    mailHtmlBody.AppendText("</html></body>")
    '    Return Replace(mailHtmlBody.Text, "[Sender]", sender)
    'End Function

    Friend Function GetHtmlText(Identifier As String, FieldName As String, ResponseType As Integer) As String
        Dim sResult As String = ""
        Dim sCondition As String = ""
        If ResponseType = 0 Then
            sCondition = "ResponseType=" & ResponseType.ToString
        Else
            sCondition = "Identifier='" & Identifier & "' and ResponseType=" & ResponseType.ToString
        End If
        'If dtConfig.Select(sCondition).Length > 0 Then
        '    drConfig = dtConfig.Select(sCondition)(0)
        '    If Not IsDBNull(drConfig(FieldName)) Then
        '        sResult = drConfig(FieldName)
        '    End If
        'End If
        Return sResult
    End Function

    Friend Function ActiveNotice(drConfiguration As DataRow) As Boolean
        Dim bResult As Boolean = False
        Dim IniDate, EndDate As Date
        If IsDBNull(drConfiguration("NewsValidityFrom")) Then
            Return bResult
        End If
        IniDate = drConfiguration("NewsValidityFrom")
        If IsDBNull(drConfiguration("NewsValidityTo")) Then
            EndDate = Date.Now
        Else
            EndDate = drConfiguration("NewsValidityTo")
        End If
        If Date.Now.ToShortDateString >= IniDate And Date.Now.ToShortDateString <= EndDate Then
            bResult = True
        End If
        Return bResult
    End Function

    'Friend Function GetHtmlTable(sender As String, dtSource As DataTable) As String
    '    Dim sResponseType As Integer = dtConfig.Rows(0)("ResponseType")

    '    If Identifier.Contains("BL") Then
    '        AssignIssued(dtSource)
    '    Else
    '        Return ""
    '    End If
    '    'Inicio de Tabla
    '    mailHtmlBody.AppendText("<table class=MsoTableGrid border=1 cellspacing=0 cellpadding=0")
    '    'Columns
    '    mailHtmlBody.AppendText("<tr style='mso-yfti-irow:0;mso-yfti-firstrow:yes'>")
    '    For col = 0 To dtSource.Columns.Count - 1
    '        If col = 2 Then
    '            If Not bIssued Then
    '                Continue For
    '            End If
    '        End If
    '        mailHtmlBody.AppendText("<td width=auto valign=top style='width:134.45pt;border:solid windowtext 1.0pt;")
    '        mailHtmlBody.AppendText("mso-border-alt:solid windowtext .5pt;background:#FFC000;padding:0cm 5.4pt 0cm 5.4pt'>")
    '        mailHtmlBody.AppendText("<p class=MsoNormal align=center style='margin-bottom:0cm;margin-bottom:.0001pt;")
    '        mailHtmlBody.AppendText("text-align:center;line-height:normal;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>" & dtSource.Columns(col).ColumnName & "</p></td>")
    '    Next
    '    mailHtmlBody.AppendText("</tr>")
    '    'DataRows
    '    mailHtmlBody.AppendText("<tr style='mso-yfti-irow:1;mso-yfti-lastrow:yes'>")
    '    For r = 0 To dtSource.Rows.Count - 1
    '        For c = 0 To dtSource.Columns.Count - 1
    '            If dtSource.Columns(c).DataType.Name = "String" Then
    '                If IsDBNull(dtSource.Rows(r)(c)) Then
    '                    dtSource.Rows(r)(c) = ""
    '                End If
    '            End If
    '            If dtSource.Columns(c).DataType.Name = "DateTime" Then
    '                If IsDBNull(dtSource.Rows(r)(c)) Then
    '                    dtSource.Rows(r)(c) = "01/01/1900"
    '                End If
    '            End If
    '            'DataColumn
    '            If Identifier <> "OBLI" Then
    '                mailHtmlBody.AppendText("<td align=center width=auto valign=top style='width:134.45pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
    '                mailHtmlBody.AppendText("<p>" & dtSource.Rows(r)(c).trim & "</p></td>")
    '            Else
    '                If (c <> 2) Or bIssued Then
    '                    mailHtmlBody.AppendText("<td align=center width=auto valign=top style='width:134.45pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
    '                End If
    '                If IsDate(dtSource.Rows(r)(c)) Then
    '                    'If (c = 2 And dtSource.Rows.Count > 1 And dtSource.Rows(r)(2) <> "01/01/1900") Then
    '                    If (c = 2 And dtSource.Rows(r)(2) <> "01/01/1900") Then
    '                        mailHtmlBody.AppendText("<p>" & Format(dtSource.Rows(r)(c), "dd/MM/yyyy") & "</p></td>")
    '                    Else
    '                        mailHtmlBody.AppendText("<p>" & Space(10) & "</p></td>")
    '                    End If
    '                Else
    '                    If (c <> 2) Or (c = 2 And dtSource.Rows.Count > 1) Then
    '                        If IsDate(dtSource.Rows(r)(2)) Then
    '                            If c > 2 Then
    '                                mailHtmlBody.AppendText("<p>" & dtSource.Rows(r)(c).trim & "</p></td>")
    '                            Else
    '                                mailHtmlBody.AppendText("<p>" & IIf(IsDBNull(dtSource.Rows(r)(c)), "", IIf(c = 0, dtSource.Rows(r)(c), "")) & IIf(c = 1, dtCnfgLayout.Rows(0)("Resultado1"), "") & "</p></td>")
    '                            End If
    '                        Else
    '                            If c > 2 Then
    '                                mailHtmlBody.AppendText("<p>" & dtSource.Rows(r)(c).trim & "</p></td>")
    '                            Else
    '                                mailHtmlBody.AppendText("<p>" & IIf(IsDBNull(dtSource.Rows(r)(c)), "", IIf(c = 0, dtSource.Rows(r)(c), "")) & IIf(c = 1, dtCnfgLayout.Rows(0)("Resultado2"), "") & "</p></td>")
    '                            End If
    '                        End If
    '                    End If
    '                End If
    '            End If
    '        Next
    '        mailHtmlBody.AppendText("</tr>")
    '    Next
    '    mailHtmlBody.AppendText("</table><br>")
    '    'Fin de Tabla
    '    Return mailHtmlBody.Text
    'End Function

    Friend Sub SendErrorMessage(oMailItem As Outlook.MailItem, Identifier As String, Message As String)
        mail = oMailItem
        mailBcc = My.Settings.SupportMailAddress
        mailSubject = "Error al procesar " & Identifier
        If Message.Length > 0 Then
            mailHtmlBody.AppendText("REFERENCIA: " & oMailItem.Subject & "<br><br>")
            mailHtmlBody.AppendText("MENSAJE DE ERROR:<br>")
            mailHtmlBody.AppendText(Message)
        End If
        CreateCustomMessage("Send", True, True)
    End Sub

    Private Sub AssignIssued(dtSource As DataTable)
        bIssued = False
        For r = 0 To dtSource.Rows.Count - 1
            If Not IsDBNull(dtSource.Rows(r).Item(2)) Then
                If IsDate(dtSource.Rows(r).Item(2)) Then
                    bIssued = True
                End If
            End If
        Next
    End Sub

    Friend Function GetMessageBody(drConfiguration As DataRow, SenderName As String, ResponseHtmlTable As Boolean, HtmlDataTable As DataTable) As String
        Dim mailHtmlBody As New RichTextBox
        Dim oGetHtmlTable As New GetHtmlTable
        mailHtmlBody.Text = ""
        mailHtmlBody.AppendText("<html><body lang=ES style='tab-interval:35.4pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
        If Not IsDBNull(drConfiguration("Header")) Then
            mailHtmlBody.AppendText(drConfiguration("Header"))
        End If
        If ResponseHtmlTable Then
            mailHtmlBody.AppendText(oGetHtmlTable.GenerateTable(HtmlDataTable))
        End If
        If Not IsDBNull(drConfiguration("Body")) Then
            mailHtmlBody.AppendText(drConfiguration("Body"))
        End If
        If Not IsDBNull(drConfiguration("Signature")) Then
            mailHtmlBody.AppendText(drConfiguration("Signature"))
        End If
        If ActiveNotice(drConfiguration) Then
            If Not IsDBNull(drConfiguration("News")) Then
                mailHtmlBody.AppendText(drConfiguration("News"))
            End If
        End If
        mailHtmlBody.AppendText("</html></body>")
        Return Replace(mailHtmlBody.Text, "[Sender]", SenderName)
    End Function

End Class
