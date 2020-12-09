Imports System.Threading
Imports System.Diagnostics
Imports System.Data

Public Class ProcessIdentifier
    Dim oCreateMailItem As New CreateMailItem

    Friend Sub MessageAnalizer(oMailItem As Outlook.MailItem, dtConfiguration As DataTable)
        Dim oLogFileUpdate As New LogFileGenerate
        Dim drConfiguration As DataRow = dtConfiguration.Select("ResponseType<>0")(0)

        Try
            If drConfiguration("ResponseType") = 1 Then
                Dim oBasicResponse As New BasicResponse
                oBasicResponse.oMailItem = oMailItem
                oBasicResponse.drConfiguration = drConfiguration
                oBasicResponse.StartProcess()
            End If
            'oMailItem.SenderEmailType = OlAddressEntryUserType.olSmtpAddressEntry
            Dim _MailAddress As String = oMailItem.Sender.Address.ToString.ToUpper
            If drConfiguration("ResponseType") = 3 And _MailAddress.ToUpper.Contains({"HAPAG-LLOYD", "HLAG.COM"}) Then
                If drConfiguration("Identifier") = "OBL RELEASE" Then
                    Dim oBlIssuedUpdate As New BlIssuedUpdate
                    oBlIssuedUpdate.oMailItem = oMailItem
                    oBlIssuedUpdate.drConfiguration = drConfiguration
                    oBlIssuedUpdate.StartProcess()
                End If
                If drConfiguration("Identifier") = "CORRECCION MN" Then
                    Dim oFcnIssuedUpdate As New FcnIssuedUpdate
                    oFcnIssuedUpdate.oMailItem = oMailItem
                    oFcnIssuedUpdate.drConfiguration = drConfiguration
                    oFcnIssuedUpdate.StartProcess()
                End If
            End If
            If drConfiguration("ResponseType") = 2 Then
                If drConfiguration("Identifier") = "OBLI" Then
                    Dim oBlIssuedQuery As New BlIssuedQuery
                    oBlIssuedQuery.oMailItem = oMailItem
                    oBlIssuedQuery.dtConfiguration = dtConfiguration
                    oBlIssuedQuery.StartProcess()
                End If
                If drConfiguration("Identifier") = "CORRECTORES" Then
                    Dim oFcnIssuedQuery As New FcnIssuedQuery
                    oFcnIssuedQuery.oMailItem = oMailItem
                    oFcnIssuedQuery.drConfiguration = drConfiguration
                    oFcnIssuedQuery.StartProcess()
                End If
            End If
        Catch ex As Exception
            oLogFileUpdate.TextFileUpdate("ROBOT", ex.Message)
            oLogFileUpdate.TextFileUpdate("ROBOT", Process.GetCurrentProcess.ProcessName)
            oLogFileUpdate.TextFileUpdate("ROBOT", "Utilización del Procesador: " & GetAverageCPU.ToString)
            oCreateMailItem.SendErrorMessage(oMailItem, drConfiguration("Identifier").ToString, ex.Message)
        End Try
    End Sub

    Public ProcessorUtilization As Single

    Public Function GetAverageCPU() As Single
        Dim cpuCounter As New PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName)
        Dim i As Integer = 0
        While i < 11
            ProcessorUtilization += (cpuCounter.NextValue() / Environment.ProcessorCount)
            System.Threading.Interlocked.Increment(i)
        End While
        ' Remember the first value is 0, so we don't want to average that in.
        Console.WriteLine(ProcessorUtilization / 10)
        Return ProcessorUtilization / 10
    End Function

    Public Function GetMemoryUsage(ByVal ProcessName As String) As String
        Dim _Process As Process = Nothing
        Dim _Return As String = ""
        For Each _Process In Process.GetProcessesByName(ProcessName)
            If _Process.ToString.Remove(0, 27).ToLower = "(" & ProcessName.ToLower & ")" Then
                _Return = (_Process.WorkingSet64 / 1024).ToString("0,000") & " K"
            End If
        Next
        If Not _Process Is Nothing Then
            _Process.Dispose()
            _Process = Nothing
        End If
        Return _Return
    End Function

End Class
