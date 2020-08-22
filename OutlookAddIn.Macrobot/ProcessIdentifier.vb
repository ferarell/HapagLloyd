Imports System.Threading

Public Class ProcessIdentifier
    Dim oSendMessage As New SendMessage

    Friend Sub MessageAnalizer(Items As Object)
        Dim oMailItems As Outlook.MailItem = Items
        Dim oLogFileUpdate As New LogFileGenerate
        Try
            If oMailItems.Subject.ToUpper.Contains("FLETESONLINE") Then
                Dim oFletesOnLine As New FletesOnLineFromSAP
                oFletesOnLine.StartProcess(oMailItems)
            End If
        Catch ex As Exception
            oLogFileUpdate.TextFileUpdate("MACROBOT", ex.Message)
            oSendMessage.SendErrorMessage(oMailItems, "MACROBOT", ex.Message, Nothing)
        End Try
    End Sub

End Class
