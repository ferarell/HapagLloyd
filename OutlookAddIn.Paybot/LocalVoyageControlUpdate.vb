Imports System.Data
Imports System.Collections

Public Class LocalVoyageControlUpdate
    Dim oSharePointTransactions As New SharePointListTransactions
    Dim oLogFileGenerate As New LogFileGenerate
    Dim ProcessLogName As String = "LOCVOYCTRL"
    Dim dtSurce, dtList, dtCoordinator As New DataTable
    Dim sFileName As String = ""

    Friend Sub StartProcess(Items As Object)
        Dim CodeProcess As String = "LVC"
        Dim oMailItems As Outlook.MailItem = Items
        Dim aAttachments As New ArrayList
        Dim aResult As New ArrayList

        For a = 1 To oMailItems.Attachments.Count
            If oMailItems.Attachments(a).FileName.ToUpper.Contains("XLS") Then
                sFileName = My.Settings.AttachedFilePath & "\" & Format(Now, "ddMMyyyy HHmmss") & " - " & oMailItems.Attachments(a).FileName
                Try
                    oMailItems.Attachments(a).SaveAsFile(sFileName)
                    aAttachments.Add(sFileName)
                Catch ex As Exception
                    oLogFileGenerate.TextFileUpdate(ProcessLogName, ex.Message)
                    SendErrorMessage(oMailItems, ProcessLogName, ex.Message, Nothing)
                End Try
                If Not IO.File.Exists(sFileName) Then
                    oLogFileGenerate.TextFileUpdate(ProcessLogName, "No se descargó el archivo adjunto.")
                    SendErrorMessage(oMailItems, ProcessLogName, "No se descargó el archivo adjunto.", Nothing)
                End If
            End If
        Next
        If sFileName = "" Then
            oLogFileGenerate.TextFileUpdate(ProcessLogName, "No se encontró archivo para procesar")
            SendErrorMessage(oMailItems, ProcessLogName, "No se encontró archivo para procesar", Nothing)
            Return
        End If
        Try
            oSharePointTransactions.SharePointUrl = My.Settings.SharePoint_Url
            oSharePointTransactions.SharePointList = "CoordinatorByServiceList"
            oSharePointTransactions.FieldsList.Clear()
            oSharePointTransactions.FieldsList.Add({"Coordinator_Area"})
            oSharePointTransactions.FieldsList.Add({"Coordinator_Service"})
            oSharePointTransactions.FieldsList.Add({"Coordinator_x0020_UserAccount"})
            oSharePointTransactions.FieldsList.Add({"Coordinator_x0020_UserName"})
            dtCoordinator = oSharePointTransactions.GetItems()
            oSharePointTransactions.SharePointList = "Local Voyage Control"
            oSharePointTransactions.FieldsList.Clear()
            oSharePointTransactions.FieldsList.Add({"ID"})
            oSharePointTransactions.FieldsList.Add({"SSY"})
            oSharePointTransactions.FieldsList.Add({"Port_Locode"})
            oSharePointTransactions.FieldsList.Add({"TerminalCode"})
            oSharePointTransactions.FieldsList.Add({"DPVoyage"})
            oSharePointTransactions.FieldsList.Add({"VesselName"})
            oSharePointTransactions.FieldsList.Add({"ScheduleVoyage"})
            oSharePointTransactions.FieldsList.Add({"Arrival_Date"})
            oSharePointTransactions.FieldsList.Add({"Departure_Date"})
            oSharePointTransactions.FieldsList.Add({"Close_Document_Date"})
            oSharePointTransactions.FieldsList.Add({"Coordinator_Name"})
            oSharePointTransactions.FieldsList.Add({"Coordinator_UserAccount"})
            oSharePointTransactions.FieldsList.Add({"Coordinator_x0020_UserName"})
            oSharePointTransactions.FieldsList.Add({"Local_Transmition_Date"})
            oSharePointTransactions.FieldsList.Add({"Manifest_Number"})
            dtList = oSharePointTransactions.GetItems()
            Sincronize(oMailItems)
            oLogFileGenerate.TextFileUpdate(ProcessLogName, "El proceso asociado al mensaje: " & oMailItems.Subject & " finalizó satisfactoriamente.")
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate(ProcessLogName, "SharePointList: " & oSharePointTransactions.SharePointList & " - Error: " & ex.Message)
            SendErrorMessage(oMailItems, ProcessLogName, ex.Message, Nothing)
        End Try

    End Sub

    Private Sub Sincronize(oMailItems As Outlook.MailItem)
        Dim dtSource As New DataTable
        Try
            dtSource = LoadExcelHDR(sFileName, "Data_Landscape_color$", "A3:N3000").Tables(0)
            For r = 0 To dtSource.Rows.Count - 1
                Dim sArrDateTime, sDepDateTime, sCloDateTime As String
                If IsDBNull(dtSource.Rows(r)(1)) Then
                    Continue For
                End If
                For c = 0 To dtSource.Rows(r).ItemArray.Count - 1
                    If IsDBNull(dtSource.Rows(r).Item(c)) Then
                        dtSource.Rows(r).Item(c) = ""
                    End If
                Next
                If dtSource.Rows(r)(1) = "" Then
                    Continue For
                End If
                If dtSource.Rows(r)("DP Voyage") = 0 Then
                    Continue For
                End If
                If dtCoordinator.Select("Coordinator_Service='" & dtSource.Rows(r)("SSY") & "'").Length = 0 Then
                    Continue For
                End If
                Dim oDPVoyage, oPol As String
                oDPVoyage = dtSource.Rows(r)("DP Voyage")
                oPol = dtSource.Rows(r)("Port Locode")
                Dim IdRow As Integer = 0
                If dtList.Select("DPVoyage = '" & oDPVoyage & "' AND Port_Locode = '" & oPol & "'").Length > 0 Then
                    IdRow = dtList.Select("DPVoyage = '" & oDPVoyage & "' AND Port_Locode = '" & oPol & "'")(0)("ID")
                End If
                If dtSource.Rows(r)("Arr Date") <> "" Then
                    sArrDateTime = Format(CDate(dtSource.Rows(r)("Arr Date") & Space(1) & IIf(dtSource.Rows(r)("Arr Time") = "", "00:00", dtSource.Rows(r)("Arr Time"))), "M/d/yyyy HH:mm")
                End If
                If dtSource.Rows(r)("Dep Date") <> "" Then
                    sDepDateTime = Format(CDate(dtSource.Rows(r)("Dep Date") & Space(1) & IIf(dtSource.Rows(r)("Dep Time") = "", "00:00", dtSource.Rows(r)("Dep Time"))), "M/d/yyyy HH:mm")
                End If
                If dtSource.Rows(r)("Close Docu Date") <> "" Then
                    sCloDateTime = Format(CDate(dtSource.Rows(r)("Close Docu Date") & Space(1) & IIf(dtSource.Rows(r)("Close Docu Time") = "", "00:00", dtSource.Rows(r)("Close Docu Time"))), "M/d/yyyy HH:mm")
                End If
                If IdRow = 0 Then
                    Try
                        oSharePointTransactions.ValuesList.Clear()
                        oSharePointTransactions.ValuesList.Add({"SSY", dtSource.Rows(r)("SSY")})
                        oSharePointTransactions.ValuesList.Add({"Port_Locode", oPol})
                        oSharePointTransactions.ValuesList.Add({"TerminalCode", dtSource.Rows(r)("Terminal")})
                        oSharePointTransactions.ValuesList.Add({"DPVoyage", oDPVoyage})
                        oSharePointTransactions.ValuesList.Add({"VesselName", dtSource.Rows(r)("Vessel")})
                        oSharePointTransactions.ValuesList.Add({"ScheduleVoyage", dtSource.Rows(r)("Schedule Voyage No#")})

                        If IsDate(sArrDateTime) Then
                            oSharePointTransactions.ValuesList.Add({"Arrival_Date", sArrDateTime})
                        End If
                        If IsDate(sDepDateTime) Then
                            oSharePointTransactions.ValuesList.Add({"Departure_Date", sDepDateTime})
                        End If
                        If IsDate(sCloDateTime) Then
                            oSharePointTransactions.ValuesList.Add({"Close_Document_Date", sCloDateTime})
                        End If
                        If dtCoordinator.Select("Coordinator_Service='" & dtSource.Rows(r)("SSY") & "'").Length > 0 Then
                            oSharePointTransactions.ValuesList.Add({"Coordinator_Name", dtCoordinator.Select("Coordinator_Service='" & dtSource.Rows(r)("SSY") & "'")(0)("Coordinator_x0020_UserName")})
                            'oSharePointTransactions.ValuesList.Add({"Coordinator_UserAccount", dtCoordinator.Select("Coordinator_Service='" & dtSource.Rows(r)("SSY") & "'")(0)("Coordinator_x0020_UserAccount")})
                        End If
                        'oSharePointTransactions.FieldsList.Add({"Coordinator_x0020_UserName", dtSource.Rows(r)("Coordinator_x0020_UserName")})
                        'oSharePointTransactions.FieldsList.Add({"Local_Transmition_Date", dtSource.Rows(r)("Local_Transmition_Date")})
                        oSharePointTransactions.InsertItem()
                    Catch ex As Exception

                    End Try
                Else
                    Try
                        oSharePointTransactions.ValuesList.Clear()
                        Dim drItem As DataRow = dtList.Select("ID='" & IdRow.ToString & "'")(0)
                        If IsDate(sArrDateTime) Then
                            If CDate(sArrDateTime) <> drItem("Arrival_Date") Then
                                oSharePointTransactions.ValuesList.Add({"Arrival_Date", sArrDateTime})
                            End If
                        End If
                        If IsDate(sDepDateTime) Then
                            If CDate(sDepDateTime) <> drItem("Departure_Date") Then
                                oSharePointTransactions.ValuesList.Add({"Departure_Date", sDepDateTime})
                            End If
                        End If
                        If IsDate(sCloDateTime) Then
                            If CDate(sCloDateTime) <> drItem("Close_Document_Date") Then
                                oSharePointTransactions.ValuesList.Add({"Close_Document_Date", sCloDateTime})
                            End If
                        End If
                        If oSharePointTransactions.ValuesList.Count > 0 Then
                            oSharePointTransactions.UpdateItem(IdRow)
                        End If
                    Catch ex As Exception

                    End Try
                End If
            Next
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate(ProcessLogName, "Sincronize Error: " & ex.Message)
            SendErrorMessage(oMailItems, ProcessLogName, ex.Message, Nothing)
        End Try
    End Sub

End Class
