Imports System.Collections

Public Class LogFileGenerate

    Public Sub TextFileUpdate(ProcessName As String, TextLog As String)
        'ProcessName += " - " & Format(Today, "yyyyMMddHHmm")
        Dim sFile As System.IO.StreamWriter
        If Not IO.Directory.Exists(My.Settings.LogFilePath) Then
            IO.Directory.CreateDirectory(My.Settings.LogFilePath)
        End If
        sFile = My.Computer.FileSystem.OpenTextFileWriter(My.Settings.LogFilePath & "\" & ProcessName & ".Log", True)
        sFile.WriteLine("[" & DateTime.Now.ToString & "]" & Space(2) & TextLog & Space(2))
        sFile.Close()
    End Sub

End Class
