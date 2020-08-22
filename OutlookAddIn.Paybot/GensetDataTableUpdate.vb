Imports System
Imports System.Data
Imports System.IO

Public Class GensetDataTableUpdate
    Dim dtSource, dtResult As New DataTable
    Dim oLogFileUpdate As New LogFileGenerate

    Friend Function DataProcess(oFileName As String, oMailItem As Outlook.MailItem) As Boolean
        Dim bResult As Boolean = True
        Try
            dtResult = ExecuteAccessQuery("SELECT * FROM GensetsDataQV WHERE DPVoyage=NULL", "").Tables(0)
            dtSource = LoadExcelWithConditions(oFileName, "{0}", "[MonthYear] IS NOT NULL").Tables(0)
            bResult = DataUpdate()
        Catch ex As Exception
            oLogFileUpdate.TextFileUpdate("UPDATE-GENSET", ex.Message)
        End Try
        SendNewMessage("PRC_OK", oMailItem, "UPDATE-GENSET", "The process has finalized successfully.")
        Return bResult
    End Function

    Friend Function DataUpdate() As Boolean
        Dim bResult As Boolean = True
        Dim dtQuery As New DataTable
        Dim iPos As Integer = 0
        Dim SetValues, ContainerNumber, BookingNumber As String

        For r = 0 To dtSource.Rows.Count - 1
            Try
                Dim oRow As DataRow = dtSource.Rows(r)
                For c = 0 To oRow.ItemArray.Count - 1
                    If IsDBNull(oRow(c)) Then
                        If oRow.Table.Columns(c).DataType = GetType(String) Then
                            oRow(c) = ""
                        End If
                        If oRow.Table.Columns(c).DataType = GetType(Double) Then
                            oRow(c) = 0
                        End If
                    End If
                Next
                ContainerNumber = ExtraeAlfanumerico(Replace(oRow("Container No"), " ", ""))
                BookingNumber = ExtraeAlfanumerico(Replace(oRow("Shipment"), " ", ""))
                dtQuery = ExecuteAccessQuery("select * from GensetsDataQV where Container = '" & ContainerNumber & "' and Booking = '" & BookingNumber & "'", "").Tables(0)
                If dtQuery.Rows.Count = 0 Then
                    dtResult.Rows.Add()
                    iPos = dtResult.Rows.Count - 1
                    dtResult.Rows(iPos)("MonthYear") = oRow("MonthYear")
                    dtResult.Rows(iPos)("DPVoyage") = oRow("DP-VOY")
                    dtResult.Rows(iPos)("POL") = oRow("POL")
                    dtResult.Rows(iPos)("VesselName") = oRow("VESSEL-NAME")
                    dtResult.Rows(iPos)("ScheduleVoyage") = oRow("SCHED-VOY")
                    dtResult.Rows(iPos)("CUSTOMER") = oRow("CUSTOMER")
                    dtResult.Rows(iPos)("MRName") = Replace(oRow("MR Name"), ",", " ")
                    dtResult.Rows(iPos)("Booking") = BookingNumber
                    dtResult.Rows(iPos)("Container") = ContainerNumber
                    dtResult.Rows(iPos)("Commodity") = Replace(oRow("Commodity"), ",", ";")
                    dtResult.Rows(iPos)("RA") = oRow("RA No")
                    dtResult.Rows(iPos)("WO") = oRow("WO")
                    dtResult.Rows(iPos)("Amount") = oRow("Usd")
                    dtResult.Rows(iPos)("CreatedBy") = My.User.Name
                    dtResult.Rows(iPos)("CreatedDate") = Now
                    InsertIntoAccess("GensetsDataQV", dtResult.Rows(iPos), "")
                Else
                    SetValues = "RA='" & oRow("RA No") & "', WO='" & oRow("WO") & "', Amount=" & oRow("Usd").ToString & ", UpdatedBy='" & My.User.Name & "', UpdatedDate=" & Format(Now, "#MM/dd/yyyy HH:mm:ss#")
                    UpdateAccess("GensetsDataQV", "Container = '" & ContainerNumber & "' and Booking = '" & BookingNumber & "'", SetValues, "")
                End If
            Catch ex As Exception
                Dim Msg As String = "Booking: " & BookingNumber & " | " & "Container: " & ContainerNumber & " | " & "Error: " & ex.Message
                oLogFileUpdate.TextFileUpdate("UPDATE-GENSET", Msg)
            End Try
        Next
        Return bResult
    End Function

End Class
