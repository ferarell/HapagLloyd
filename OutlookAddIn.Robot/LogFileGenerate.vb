Imports System.Collections

Public Class LogFileGenerate

    Public Sub TextFileUpdate(Process As String, TextLog As String)
        Dim sFile As System.IO.StreamWriter
        sFile = My.Computer.FileSystem.OpenTextFileWriter("C:\Robot\" & Process & ".Log", True)
        sFile.WriteLine("[" & DateTime.Now.ToString & "]" & Space(2) & TextLog & Space(2))
        sFile.Close()
    End Sub

End Class
