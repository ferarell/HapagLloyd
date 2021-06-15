Imports System.Threading
Imports System.Diagnostics

Public Class ProcessIdentifier

    Friend Sub MessageAnalizer(oMailItems As Outlook.MailItem)
        'Dim oMailItems As Outlook.MailItem = Items
        Dim oLogFileUpdate As New LogFileGenerate
        Try
            If oMailItems.Subject.ToUpper.Contains("OBL RELEASE") And oMailItems.Body.ToUpper.Contains("BSL ORIGINALES EN DESTINO") And oMailItems.Attachments.Count > 0 Then
                Dim oTramarsaBillOfLadingIssued As New TramarsaBillOfLadingIssued
                oTramarsaBillOfLadingIssued.StartProcess(oMailItems)
            ElseIf oMailItems.Subject.ToUpper.Contains("HL-") And oMailItems.Body.ToUpper.Contains("BOOKING CONFIRMATION") And oMailItems.Attachments.Count > 0 Then
                Dim oTramarsaGatesOut As New TramarsaGatesOut
                oTramarsaGatesOut.StartProcess(oMailItems)
                If oMailItems.Body.ToUpper.Contains("CANCELLATION") Then
                    Dim oBookingCancellation As New BookingCancellation
                    oBookingCancellation.StartProcess(oMailItems)
                End If
            ElseIf oMailItems.Subject.ToUpper.Contains("SCHD0301 - LPE") Then
                Dim oLocalVoyageControlUpdate As New LocalVoyageControlUpdate
                oLocalVoyageControlUpdate.StartProcess(oMailItems)
            ElseIf oMailItems.Subject.ToUpper.Contains("VOYC2502") Then
                Dim oScheduleLocalVoyageUpdate As New ScheduleLocalVoyageUpdate
                oScheduleLocalVoyageUpdate.StartProcess(oMailItems)
            ElseIf oMailItems.Subject.ToUpper.Contains("INVS0201") Then
                Dim oLocalChargesInvoicing As New LocalChargesInvoicing
                oLocalChargesInvoicing.StartProcess(oMailItems)
            ElseIf oMailItems.Subject.ToUpper.Contains("EQEO1601") Then
                Dim oEquipmentEvents As New EquipmentEvents
                oEquipmentEvents.StartProcess(oMailItems)
                'ElseIf oMailItems.Subject.ToUpper.Contains("EQEO0801") Then
                '    Dim oReeferDataMasterUpdate As New ReeferDataMasterUpdate
                '    tProc = New Thread(Sub() oReeferDataMasterUpdate.DataProcess(sFileName))
                'ElseIf oMailItems.Subject.ToUpper.Contains("VOYC3001") Then
                '    Dim oScheduleTranshipmentVoyageUpdate As New ScheduleTranshipmentVoyageUpdate
                '    tProc = New Thread(Sub() oScheduleTranshipmentVoyageUpdate.DataProcess(sFileName))
                'ElseIf oMailItems.Subject.ToUpper.Contains("FLETESONLINE") Then
                '    Dim oFletesOnLine As New FletesOnLineFromSAP
                '    oFletesOnLine.StartProcess(oMailItems)
            Else
                'If oMailItems.Attachments.Count > 0 Then
                '    DataProcess1(oMailItems)
                'End If
            End If
        Catch ex As Exception
            oLogFileUpdate.TextFileUpdate("PAYBOT", ex.Message)
            oLogFileUpdate.TextFileUpdate("PAYBOT", Process.GetCurrentProcess.ProcessName)
            oLogFileUpdate.TextFileUpdate("PAYBOT", "Utilización del Procesador: " & GetAverageCPU.ToString)
            SendErrorMessage(oMailItems, "PAYBOT", ex.Message, Nothing)
        End Try
    End Sub

    'Public Shared ProcessorUtilization As Single

    'Public Shared Function GetAverageCPU() As Single
    '    Dim cpuCounter As New PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName)
    '    Dim i As Integer = 0
    '    While i < 11
    '        ProcessorUtilization += (cpuCounter.NextValue() / Environment.ProcessorCount)
    '        System.Threading.Interlocked.Increment(i)
    '    End While
    '    ' Remember the first value is 0, so we don't want to average that in.
    '    Console.Writeline(ProcessorUtilization / 10)
    '    Return ProcessorUtilization / 10
    'End Function

    'Public Shared Function GetMemoryUsage(ByVal ProcessName As String) As String
    '    Dim _Process As Process = Nothing
    '    Dim _Return As String = ""
    '    For Each _Process In Process.GetProcessesByName(ProcessName)
    '        If _Process.ToString.Remove(0, 27).ToLower = "(" & ProcessName.ToLower & ")" Then
    '            _Return = (_Process.WorkingSet64 / 1024).ToString("0,000") & " K"
    '        End If
    '    Next
    '    If Not _Process Is Nothing Then
    '        _Process.Dispose()
    '        _Process = Nothing
    '    End If
    '    Return _Return
    'End Function


End Class
