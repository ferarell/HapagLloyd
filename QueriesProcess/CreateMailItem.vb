Imports DevExpress.XtraSplashScreen
Imports Microsoft.Office.Interop
Imports System.Collections
Imports System.Data
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms

Public Class CreateMailItem
    Friend mailHtmlBody As New RichTextBox
    Friend mailSubject, mailTo, mailCc, mailBcc As String
    Friend mailAttachment As New ArrayList
    Friend Identifier As String
    Dim bIssued As Boolean = Nothing

    Friend Function CreateCustomMessage(TypeMessage As String, IsNewMessage As Boolean, RecipientInclude As Boolean) As String
        Dim sResult As String = ""
        Dim AppOutlook As New Microsoft.Office.Interop.Outlook.Application '= CType(Activator.CreateInstance(Type.GetTypeFromCLSID(New Guid("0006F03A-0000-0000-C000-000000000046"))), Microsoft.Office.Interop.Outlook.Application)
        Dim mail As Outlook.MailItem
        Dim oInspector As Outlook.Inspector
        Dim mailRecipients As Outlook.Recipients = Nothing
        Dim mailRecipient As Outlook.Recipient = Nothing
        Dim mailBody As String = ""
        Try
            If IsNewMessage Then
                mail = AppOutlook.CreateItem(Outlook.OlItemType.olMailItem)
            Else
                mail = CType(AppOutlook.CreateItem(Outlook.OlItemType.olMailItem), Outlook.MailItem)
                oInspector = mail.GetInspector
            End If
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
        Dim bResult As Boolean = True
        Dim NewMessage As Outlook.MailItem
        Dim AppOutlook As Microsoft.Office.Interop.Outlook.Application = CType(Activator.CreateInstance(Type.GetTypeFromCLSID(New Guid("0006F03A-0000-0000-C000-000000000046"))), Microsoft.Office.Interop.Outlook.Application)
        NewMessage = AppOutlook.CreateItem(Outlook.OlItemType.olMailItem)
        Dim Recipents As Outlook.Recipients = NewMessage.Recipients
        Recipents.Add(oMailItem.SenderEmailAddress)

        Try
            NewMessage.BodyFormat = Outlook.OlBodyFormat.olFormatHTML
            NewMessage.Subject = ResponseMailSubject
            NewMessage.HTMLBody = ResponseMailBody
            NewMessage.Send()
        Catch ex As Exception
            bResult = False
            SplashScreenManager.CloseForm(False)
            System.Windows.Forms.MessageBox.Show(ex.Message,
                "An exception is occured in the code of add-in.")
        Finally
            'If Not IsNothing(mailRecipient) Then Marshal.ReleaseComObject(mailRecipient)
            If Not IsNothing(Recipents) Then Marshal.ReleaseComObject(Recipents)
            If Not IsNothing(NewMessage) Then Marshal.ReleaseComObject(NewMessage)
        End Try
        Return bResult
    End Function

    Friend Sub SendErrorMessage(oMailItem As Outlook.MailItem, Identifier As String, Message As String)
        Dim AppOutlook As Microsoft.Office.Interop.Outlook.Application = CType(Activator.CreateInstance(Type.GetTypeFromCLSID(New Guid("0006F03A-0000-0000-C000-000000000046"))), Microsoft.Office.Interop.Outlook.Application)
        Dim NewMessage As Outlook.MailItem
        Dim bResult As Boolean = True
        NewMessage = AppOutlook.CreateItem(Outlook.OlItemType.olMailItem)
        NewMessage.BodyFormat = Outlook.OlBodyFormat.olFormatHTML
        NewMessage.Subject = "Error al procesar " & Identifier
        If Message.Length > 0 Then
            mailHtmlBody.AppendText("REFERENCIA: " & oMailItem.Subject & "<br><br>")
            mailHtmlBody.AppendText("MENSAJE DE ERROR:<br>")
            mailHtmlBody.AppendText(Message)
        End If
        NewMessage.HTMLBody = mailHtmlBody.Text
        NewMessage.BCC = My.Settings.SupportMail
        NewMessage.Send()
    End Sub

End Class
