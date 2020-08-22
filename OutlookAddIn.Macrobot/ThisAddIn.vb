Imports System.Collections
Imports System.Data
Imports System.Windows.Forms
Imports System.Threading

Public Class ThisAddIn
    Dim outlookNameSpace As Outlook.NameSpace
    Dim inbox As Outlook.MAPIFolder
    Dim WithEvents items As Outlook.Items
    Dim oLogFileUpdate As New LogFileGenerate
    Dim oSendMessage As New SendMessage

    Private Sub ThisAddIn_Startup() Handles Me.Startup
        Try
            outlookNameSpace = Me.Application.GetNamespace("MAPI")
            inbox = outlookNameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox)
            items = inbox.Items
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        If Not My.Settings.GetPreviousVersion("DBFileName") Is Nothing Then
            If My.Computer.Name <> "FARELLANO" Then
                My.Settings.Upgrade()
            End If
        End If
        If Not IO.File.Exists(My.Settings.DBFileName) Then
            'oSendMessage.SendErrorMessage(items, "MACROBOT", "No se encontró la base de datos de configuración, el complemento no se activará hasta que se asigne la base de datos.", Nothing)
            MessageBox.Show("No se encontró la base de datos de configuración, el complemento no se activará hasta que se asigne la base de datos.")
            Return
        End If
        MDBFileName = My.Settings.DBFileName
    End Sub

    Private Sub Items_ItemAdd(ByVal item As Object) Handles items.ItemAdd
        Dim mailItem As Outlook.MailItem = item
        Dim tProc As Thread = Nothing
        If mailItem.Subject Is Nothing Then
            mailItem.Subject = " "
        End If
        If mailItem.Body Is Nothing Then
            mailItem.Body = " "
        End If
        If TypeOf (item) Is Outlook.MailItem Then
            If mailItem.Sender Is Nothing Then
                Return
            End If
            Try
                Dim oProcess As New ProcessIdentifier
                tProc = New Thread(Sub() oProcess.MessageAnalizer(item))
                tProc.Start()
            Catch ex As Exception
                oLogFileUpdate.TextFileUpdate("MACROBOT", ex.Message)
                oSendMessage.SendErrorMessage(item, "MACROBOT", ex.Message, Nothing)
            End Try

        End If
    End Sub

    Private Sub ThisAddIn_Shutdown() Handles Me.Shutdown

    End Sub

End Class
