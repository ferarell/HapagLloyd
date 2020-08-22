Imports System.Net.Mail

Public Class MailNotification

    Public Sub SendMessage(ByVal myParamList As ArrayList)
        Dim mySmtpClient As New SmtpClient
        Dim bError As Boolean = False
        Try
            mySmtpClient.Timeout = 10000
            mySmtpClient.UseDefaultCredentials = False
            'mySmtpClient.EnableSsl = My.Settings.MailServerSsl
            If mySmtpClient.EnableSsl Then
                'mySmtpClient.Credentials = New System.Net.NetworkCredential(My.Settings.MailServerUser, My.Settings.MailServerPassword)
            Else
                mySmtpClient.UseDefaultCredentials = True
            End If
            'mySmtpClient.Host = My.Settings.MailServerHost
            'mySmtpClient.Port = My.Settings.MailServerPort
            mySmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
            mySmtpClient.Send(myParamList(0), myParamList(1), myParamList(2), myParamList(3))
        Catch se As SmtpException
            bError = True
        Catch ex As Exception
            bError = True
        End Try
    End Sub
End Class
