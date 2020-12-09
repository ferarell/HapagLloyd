Imports DevExpress.XtraSplashScreen
Imports Microsoft.Office.Interop
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms

Public Class CreateMailItemOld
    Dim Application As New Outlook.Application
    'Dim Application As Microsoft.Office.Interop.Outlook.Application = CType(Activator.CreateInstance(Type.GetTypeFromCLSID(New Guid("0006F03A-0000-0000-C000-000000000046"))), Microsoft.Office.Interop.Outlook.Application)
    Dim mail As Outlook.MailItem = Nothing
    Dim mailRecipients As Outlook.Recipients = Nothing
    Dim mailRecipient As Outlook.Recipient = Nothing
    Dim mailBody As String = ""
    Friend mailHtmlBody As New RichTextBox
    Friend mailSubject, mailTo, mailCc, mailBcc As String
    Friend mailAttachment As New ArrayList

    Friend Function CreateCustomMessage(TypeMessage As String) As String
        Dim sResult As String = ""
        Try
            mail = CType(Application.CreateItem(Outlook.OlItemType.olMailItem), Outlook.MailItem)
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

End Class
