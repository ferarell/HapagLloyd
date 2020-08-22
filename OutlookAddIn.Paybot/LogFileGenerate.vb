Imports System.Collections

Public Class LogFileGenerate

    Public Sub TextFileUpdate(ProcessName As String, TextLog As String)
        Dim sFile As System.IO.StreamWriter
        sFile = My.Computer.FileSystem.OpenTextFileWriter(My.Settings.LogFilePath & "\" & ProcessName & ".Log", True)
        sFile.WriteLine("[" & DateTime.Now.ToString & "]" & Space(2) & Replace(TextLog, "<br>", "") & Space(2))
        sFile.Close()
    End Sub

End Class
