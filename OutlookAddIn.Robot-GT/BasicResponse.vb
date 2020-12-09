Imports System.Collections
Imports System.Data
Imports System.Windows.Forms

Public Class BasicResponse
    Dim oCreateMailItem As New CreateMailItem
    Friend oMailItem As Outlook.MailItem
    Friend drConfiguration As DataRow

    Friend Sub StartProcess()

        oMailItem.HTMLBody = oCreateMailItem.GetMessageBody(drConfiguration, oMailItem.SenderName, False, Nothing) + oMailItem.HTMLBody
        oCreateMailItem.MessageResponse(oMailItem, oMailItem.Subject, oMailItem.HTMLBody)
    End Sub

End Class
