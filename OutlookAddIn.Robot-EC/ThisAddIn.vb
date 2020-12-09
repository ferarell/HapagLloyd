Imports System.Collections
Imports System.Data
Imports System.Windows.Forms
Imports System.Threading
Imports DevExpress.XtraEditors

Public Class ThisAddIn
    Dim outlookNameSpace As Outlook.NameSpace
    Dim inbox As Outlook.MAPIFolder
    Dim WithEvents items As Outlook.Items
    Dim oDataAccess As New DataAccess
    Dim drConfiguration As DataRow
    Dim dtConfig As New DataTable
    Dim IdConfiguration As Integer

    Private Sub ThisAddIn_Startup() Handles Me.Startup
        Try
            If Not My.Settings.GetPreviousVersion("DBFileName") Is Nothing Then
                'If My.Computer.Name <> "FARELLANO" Then
                My.Settings.Upgrade()
            End If
            'End If
            If Not IO.File.Exists(My.Settings.DBFileName) Then
                DevExpress.XtraEditors.XtraMessageBox.Show("Cannot find the database, please assign it from the configuration option. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            outlookNameSpace = Me.Application.GetNamespace("MAPI")
            inbox = outlookNameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox)
            items = inbox.Items
            'MDBFileName = My.Settings.DBFileName
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub ThisAddIn_Shutdown() Handles Me.Shutdown

    End Sub

    Private Sub Items_ItemAdd(ByVal item As Object) Handles items.ItemAdd
        Dim mailItem As Outlook.MailItem = item
        Dim tProc As Thread = Nothing
        ThreadPool.SetMaxThreads(1, 1)
        Dim dtMailBlackList As New DataTable
        Dim oCreateMailItem As New CreateMailItem
        Dim oLogFileUpdate As New LogFileGenerate
        'Valid Mail Address
        dtMailBlackList = oDataAccess.ExecuteAccessQuery("SELECT * FROM MailBlackList WHERE MailAddress='" & mailItem.Sender.Address & "'").Tables(0)
        If dtMailBlackList.Rows.Count > 0 Then
            Return
        End If
        'Valid Mail Domain
        dtMailBlackList = oDataAccess.ExecuteAccessQuery("SELECT * FROM MailBlackList WHERE MailAddress='*' AND MailDomain='" & Mid(mailItem.Sender.Address, InStr(mailItem.Sender.Address, "@") + 1, 80) & "'").Tables(0)
        If dtMailBlackList.Rows.Count > 0 Then
            Return
        End If
        Dim Identifier As String = Nothing
        'Identifier = GetFilter(mailItem)
        IdConfiguration = GetIdConfiguration(mailItem)
        If IdConfiguration = -1 Then
            If mailItem.Recipients.Item(1).Address <> mailItem.Sender.Address Then
                dtConfig = oDataAccess.ExecuteAccessQuery("SELECT TOP 1 * FROM " & My.Settings.ConfigTableName & " WHERE IdConfiguration=0 AND ResponseType=0").Tables(0)
                Dim mailBody As String = oCreateMailItem.GetMessageBody(dtConfig.Rows(0), mailItem.SenderName, False, Nothing)
                If Not mailBody Is Nothing Then
                    oCreateMailItem.MessageResponse(item, dtConfig.Rows(0)("ResponseMailSubject"), mailBody)
                End If
            End If
            Return
        End If
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
                If mailItem.Recipients.Item(1).Address <> mailItem.Sender.Address Then
                    dtConfig = oDataAccess.ExecuteAccessQuery("SELECT * FROM " & My.Settings.ConfigTableName & " WHERE IdConfiguration = " & IdConfiguration.ToString).Tables(0)
                    If dtConfig.Rows.Count > 0 Then
                        'Dim oProcessIdentifier = New ProcessIdentifier
                        'oProcessIdentifier.MessageAnalizer(mailItem, dtConfig.Rows(0))
                        'Dim oProcess As New ProcessIdentifier
                        'tProc = New Thread(Sub() oProcess.MessageAnalizer(mailItem, dtConfig.Rows(0)))
                        'tProc.Start()
                        Dim new_job As New System.Threading.Thread(New System.Threading.ParameterizedThreadStart(AddressOf do_Job))
                        new_job.Start(item)
                    End If
                End If

            Catch ex As Exception
                oLogFileUpdate.TextFileUpdate("ROBOT", ex.Message)
                oCreateMailItem.SendErrorMessage(mailItem, "ROBOT", ex.Message)
            End Try
        End If
    End Sub

    ReadOnly Sem As New Semaphore(1, 1)

    Public Sub do_Job(ByVal item As Object)

        Sem.WaitOne()
        Dim oProcess As New ProcessIdentifier
        oProcess.MessageAnalizer(item, dtConfig)
        'only 3 or 4 threads at time can do task
        Sem.Release()
    End Sub

    Friend Function GetFilter(mailItem As Outlook.MailItem) As String
        Dim sResult As String = ""
        If mailItem.Subject Is Nothing Then
            mailItem.Subject = " "
        End If
        Dim dtSubjects As New DataTable
        dtSubjects = oDataAccess.ExecuteAccessQuery("SELECT * FROM InputSubjectQry").Tables(0)
        For r = 0 To dtSubjects.Rows.Count - 1
            If IsDBNull(dtSubjects.Rows(r)("SubjectIdentifier")) Then
                Continue For
            End If
            If mailItem.Subject.ToUpper.Contains(dtSubjects.Rows(r)("SubjectIdentifier")) Then
                sResult = dtSubjects.Rows(r)("SubjectIdentifier")
                'drConfiguration = dtSubjects.Rows(r)("IdConfiguration")
                Exit For
            End If
        Next
        Return sResult
    End Function

    Friend Function GetIdConfiguration(mailItem As Outlook.MailItem) As Integer
        Dim iResult As Integer = -1
        If mailItem.Subject Is Nothing Then
            mailItem.Subject = " "
        End If
        Dim dtSubjects As New DataTable
        dtSubjects = oDataAccess.ExecuteAccessQuery("SELECT * FROM InputSubjectQry").Tables(0)
        For r = 0 To dtSubjects.Rows.Count - 1
            If IsDBNull(dtSubjects.Rows(r)("SubjectIdentifier")) Then
                Continue For
            End If
            If mailItem.Subject.ToUpper.StartsWith(dtSubjects.Rows(r)("SubjectIdentifier")) Then
                iResult = dtSubjects.Rows(r)("IdConfiguration")
                Exit For
            End If
        Next
        Return iResult
    End Function

    Private Sub Application_Startup() Handles Application.Startup

    End Sub

End Class
