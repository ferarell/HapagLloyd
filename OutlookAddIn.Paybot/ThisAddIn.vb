Imports System.Collections
Imports System.Data
Imports System.Windows.Forms
Imports System.Threading
Imports DevExpress.XtraEditors
Imports System.Threading.Tasks
Imports System.Diagnostics
Imports System.IO

Public Class ThisAddIn
    Public Shared outlookNameSpace As Outlook.NameSpace
    Dim inbox As Outlook.MAPIFolder
    Dim WithEvents items As Outlook.Items
    Dim oLogFileUpdate As New LogFileGenerate

    Private Sub ThisAddIn_Startup() Handles Me.Startup
        'Try
        '    If Not My.Settings.GetPreviousVersion("DBFileName") Is Nothing Then
        '        If My.Computer.Name <> "FARELLANO" Then
        '            My.Settings.Upgrade()
        '        End If
        '    End If
        '    outlookNameSpace = Me.Application.GetNamespace("MAPI")
        '    inbox = outlookNameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox)
        '    items = inbox.Items
        '    If Not IO.File.Exists(My.Settings.DBFileName) Then
        '        'SendNewMessage("PRC_ERROR", items, "PAYBOT", "No se encontró la base de datos de configuración, el complemento no se activará hasta que se asigne la base de datos.")
        '        Return
        '    End If
        '    MDBFileName = My.Settings.DBFileName
        'Catch ex As Exception
        '    'SendNewMessage("PRC_ERROR", items, "PAYBOT", ex.Message)
        'End Try
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
        dtCustomerList = ExecuteAccessQuery("SELECT * FROM CustomerList", "").Tables(0)
    End Sub

    Private Sub ThisAddIn_Shutdown() Handles Me.Shutdown

    End Sub

    Private Sub Items_ItemAdd(ByVal item As Object) Handles items.ItemAdd
        Dim mailItem As Outlook.MailItem = item
        'Dim tProc As Thread = Nothing
        ThreadPool.SetMaxThreads(1, 1)
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
                Dim new_job As New System.Threading.Thread(New System.Threading.ParameterizedThreadStart(AddressOf do_Job))
                new_job.Start(item)

                'Dim oProcess As New ProcessIdentifier
                'tProc = New Thread(Sub() oProcess.MessageAnalizer(item))
                '    Parallel.For(0, 10,
                '(New ParallelOptions() With {.MaxDegreeOfParallelism = 1}),
                ' Sub(i As Integer)
                '     do_Job(item)
                '     'tProc.Start()
                '     System.Threading.Thread.Sleep(2000)
                ' End Sub)
            Catch ex As Exception
                oLogFileUpdate.TextFileUpdate("PAYBOT", ex.Message)
                oLogFileUpdate.TextFileUpdate("PAYBOT", Process.GetCurrentProcess.ProcessName)
                oLogFileUpdate.TextFileUpdate("PAYBOT", "Utilización del Procesador: " & GetAverageCPU.ToString)
                SendErrorMessage(mailItem, "PAYBOT", ex.Message, Nothing)
            End Try
            'If mailItem.Attachments.Count > 0 Then
            'ElseIf mailItem.Subject.Contains("PAPERL045") Then
            '    DataProcess2(mailItem)
            'Else
            '    If mailItem.Attachments.Count > 0 Then
            '        DataProcess1(mailItem)
            '    End If
            'End If

        End If
    End Sub

    ReadOnly Sem As New Semaphore(1, 1)

    Public Sub do_Job(ByVal item As Object)

        Sem.WaitOne()
        Dim oProcess As New ProcessIdentifier
        oProcess.MessageAnalizer(item)
        'only 3 or 4 threads at time can do task
        Sem.Release()
    End Sub

    'Private Sub DataProcess1(item As Object)
    '    Dim mailItem As Outlook.MailItem = item
    '    Dim sFileName = FileIO.FileSystem.GetTempFileName
    '    Dim oLogFileUpdate As New LogFileGenerate

    '    For a = 1 To mailItem.Attachments.Count
    '        If mailItem.Attachments(a).FileName.ToUpper.Contains("PDF") Then
    '            sFileName = My.Settings.AttachedFilePath & "\" & Format(Now, "ddMMyyyy HHmmss") & " - " & mailItem.Attachments(a).FileName
    '            mailItem.Attachments(a).SaveAsFile(sFileName)
    '            If Not IO.File.Exists(sFileName) Then
    '                DevExpress.XtraEditors.XtraMessageBox.Show("No se descargó el archivo adjunto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '                Return
    '            End If
    '        End If
    '    Next
    '    Dim tProc As Thread = Nothing
    '    Dim oMessageDataProcess As New MessageDataProcess
    '    tProc = New Thread(Sub() oMessageDataProcess.StartProcess(sFileName, mailItem, 1))
    '    Try
    '        tProc.Start()
    '    Catch ex As Exception
    '        oLogFileUpdate.TextFileUpdate("PAYBOT", ex.Message)
    '        SendNewMessage("PRC_ERROR", item, "PAYBOT", ex.Message)
    '    End Try
    'End Sub

    'Private Sub DataProcess2(item As Object)
    '    Dim mailItem As Outlook.MailItem = item
    '    Dim sFileName = FileIO.FileSystem.GetTempFileName
    '    Dim oLogFileUpdate As New LogFileGenerate

    '    For a = 1 To mailItem.Attachments.Count
    '        If mailItem.Attachments(a).FileName.ToUpper.Contains("TXT") Then
    '            sFileName = My.Settings.AttachedFilePath & "\" & Format(Now, "ddMMyyyy HHmmss") & " - " & mailItem.Attachments(a).FileName
    '            mailItem.Attachments(a).SaveAsFile(sFileName)
    '            If Not IO.File.Exists(sFileName) Then
    '                DevExpress.XtraEditors.XtraMessageBox.Show("No se descargó el archivo adjunto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '                Return
    '            End If
    '        End If
    '    Next
    '    Dim tProc As Thread = Nothing
    '    Dim oMessageDataProcess As New MessageDataProcess
    '    tProc = New Thread(Sub() oMessageDataProcess.StartProcess(sFileName, mailItem, 2))
    '    Try
    '        tProc.Start()
    '    Catch ex As Exception
    '        oLogFileUpdate.TextFileUpdate("PAYBOT", ex.Message)
    '        SendNewMessage("PRC_ERROR", item, "PAYBOT", ex.Message)
    '    End Try
    'End Sub

    Private Sub Application_Startup() Handles Application.Startup
        'Dim oExcelMacro As New CallOfficeMacro
        'oExcelMacro.macro("C:\Users\ferar\Downloads", "Prueba", "C:\Users\ferar\Downloads\Ejmplo Macro.xlsm", "TestParams")
    End Sub

End Class
