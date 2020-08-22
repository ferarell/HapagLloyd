Imports Microsoft.Office.Interop.Access
Imports System.IO

Module Module1

    Dim sFile As System.IO.StreamWriter

    Sub Main()
        Dim oArgs As ObjectModel.ReadOnlyCollection(Of String) = My.Application.CommandLineArgs()
        'Dim sFile As String = ""
        'For Each arg As String In oArgs
        '    sFile = arg
        '    Continue For
        'Next
        sFile = My.Computer.FileSystem.OpenTextFileWriter(IO.Directory.GetCurrentDirectory & "\CompactLog.txt", True)
        Dim sourceFilePath As String = ""
        Dim dtQuery As New DataTable
        Dim sFileName As String = IO.Directory.GetCurrentDirectory & "\AccessFileList.accdb"
        If IO.File.Exists(sFileName) Then
            dtQuery = ExecuteAccessQuery("select FileName from FilesToFix", sFileName).Tables(0)
        End If
        If dtQuery.Rows.Count > 0 Then
            For r = 0 To dtQuery.Rows.Count - 1
                Dim oRow As DataRow = dtQuery.Rows(r)
                CompactDB(oRow(0))
            Next
        End If
        sFile.Close()
    End Sub

    Private Sub CompactDB(sourceFilePath As String)
        Dim destFilePath As String = Mid(sourceFilePath, 1, InStr(sourceFilePath, ".") - 1) & "Tmp" & Mid(sourceFilePath, InStr(sourceFilePath, "."), Len(sourceFilePath))
        Dim bakFilePath As String = Mid(sourceFilePath, 1, InStr(sourceFilePath, ".") - 1) & "Bak" & Mid(sourceFilePath, InStr(sourceFilePath, "."), Len(sourceFilePath))
        If IO.File.Exists(bakFilePath) Then
            sFile.WriteLine("[" & DateTime.Now.ToString & "]" & Space(2) & "Delete backup file " & bakFilePath & Space(2))
            IO.File.Delete(bakFilePath)
        End If
        sFile.WriteLine("[" & DateTime.Now.ToString & "]" & Space(2) & "Copy to new backup file " & bakFilePath & Space(2))
        IO.File.Copy(sourceFilePath, bakFilePath)
        If IO.File.Exists(destFilePath) Then
            sFile.WriteLine("[" & DateTime.Now.ToString & "]" & Space(2) & "Delete destination file " & destFilePath & Space(2))
            IO.File.Delete(destFilePath)
        End If
        Dim dao As New Dao.DBEngine
        sFile.WriteLine("[" & DateTime.Now.ToString & "]" & Space(2) & "Compact source file " & sourceFilePath & Space(1) & " and create new destination file " & destFilePath & Space(2))
        dao.CompactDatabase(sourceFilePath, destFilePath)
        If IO.File.Exists(destFilePath) Then
            IO.File.Delete(sourceFilePath)
        End If
        sFile.WriteLine("[" & DateTime.Now.ToString & "]" & Space(2) & "Rename destination file " & destFilePath & Space(1) & " to source file " & sourceFilePath & Space(2))
        My.Computer.FileSystem.RenameFile(destFilePath, Path.GetFileName(sourceFilePath))
    End Sub

End Module
