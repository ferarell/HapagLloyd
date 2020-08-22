Imports System
Imports System.Data
Imports System.IO
Imports Microsoft.Office.Interop

Public Class ReturnRequestGenset

    Dim dtSource, dtResult As New DataTable
    Dim drSource As DataRow
    Dim oLogFileUpdate As New LogFileGenerate
    Dim mailItem As Outlook.MailItem

    Friend Function DataProcess(item As Outlook.MailItem, oFileName As String) As Boolean
        mailItem = item
        Dim bResult As Boolean = True
        Try
            dtResult = ExecuteAccessQuery("SELECT * FROM GensetsDataQV WHERE DPVoyage=NULL", "").Tables(0)
            dtSource = LoadExcel(oFileName, "{0}").Tables(0)
            bResult = DataProcess(oFileName)
            mailItem.CC = "ferarell@hotmail.com" '"amperueqd@hlag.com"
            ReplyMessage(mailItem, oFileName)
        Catch ex As Exception
            oLogFileUpdate.TextFileUpdate("RETORNO GENSET", ex.Message)
        End Try
        Return bResult
    End Function

    Friend Function DataProcess(oFileName As String) As Boolean
        Dim bResult As Boolean = True
        Dim dtQuery As New DataTable
        Dim iPos As Integer = 0
        Dim ContainerNumber, BookingNumber As String
        dtSource.Columns.Add("WorkOrder", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("Monto", GetType(Double)).DefaultValue = 0
        dtSource.Columns.Add("Observaciones", GetType(String)).AllowDBNull = True
        For r = 0 To dtSource.Rows.Count - 1
            Try
                drSource = dtSource.Rows(r)
                Dim QueriedTimes As Integer = 0
                ContainerNumber = ExtraeAlfanumerico(Replace(drSource(2), " ", ""))
                BookingNumber = ExtraeAlfanumerico(Replace(drSource(1).ToString, " ", ""))
                dtQuery = ExecuteAccessQuery("select * from GensetsDataQV where Container = '" & ContainerNumber & "' and Booking = '" & BookingNumber & "'", "").Tables(0)
                If dtQuery.Rows.Count > 0 Then
                    QueriedTimes = ExecuteAccessQuery("select COUNT(QueriedTimes) from GensetsQueryLog where Container = '" & ContainerNumber & "' and Booking = '" & BookingNumber & "'", "").Tables(0)(0)(0)
                    drSource("WorkOrder") = dtQuery.Rows(0)("WO")
                    drSource("Monto") = dtQuery.Rows(0)("Amount")
                    drSource("Observaciones") = GetRemarks(BookingNumber, ContainerNumber, drSource(7), QueriedTimes, dtQuery.Rows(0)("WO"), dtQuery.Rows(0)("Amount"))
                End If
            Catch ex As Exception
                oLogFileUpdate.TextFileUpdate("RETORNO GENSET", ex.Message)
            End Try
        Next
        UpdateExcelFile(oFileName)
        Return bResult
    End Function

    Friend Function GetRemarks(BookingNo As String, ContainerNo As String, CustomerId As String, QTimes As Integer, WO As String, Amount As Double) As String
        Dim sResult As String = ""
        Dim dtQuery, dtResult As New DataTable
        dtQuery = ExecuteAccessQuery("select * from GensetsQueryLog where Container = '" & ContainerNo & "' and Booking = '" & BookingNo & "'", "").Tables(0)
        dtResult = dtQuery.Copy
        If dtQuery.Rows.Count = 0 Then
            dtResult.Rows.Add()
        End If
        dtResult.Rows(0)("Booking") = BookingNo
        dtResult.Rows(0)("Container") = ContainerNo
        dtResult.Rows(0)("CustomerId") = CustomerId
        dtResult.Rows(0)("ProcessDate") = Now
        If WO = "" Then
            If Amount = 0 Then
                sResult = "Reserva sin beneficio aplicable, por favor contactar al ejecutivo comercial."
            Else
                sResult = "Aún no se ha generado la WO, por favor solicitarla al siguiente correo: amperueqd@hlag.com"
            End If
        Else
            If QTimes > 0 Then
                sResult = "WO previamente informada"
            End If
            If ExecuteAccessQuery("select * from GensetsQueryLog where Container = '" & ContainerNo & "' and Booking = '" & BookingNo & "' AND CustomerId <> '" & CustomerId & "'", "").Tables(0).Rows.Count > 0 Then
                sResult = "Favor contactar con amperueqd@hlag.com"
                drSource("WorkOrder") = DBNull.Value
                drSource("Monto") = DBNull.Value
            End If
        End If
        dtResult.Rows(0)("Remarks") = sResult
        dtResult.Rows(0)("QueriedTimes") = QTimes + 1
        dtResult.Rows(0)("ExistsWO") = IIf(WO <> "", True, False)
        dtResult.Rows(0)("MailSender") = mailItem.SenderEmailAddress

        InsertIntoAccess("GensetsQueryLog", dtResult.Rows(0), "")
        Return sResult
    End Function

    Private Sub UpdateExcelFile(oFileName As String)
        Dim strFileName As String = oFileName

        Dim _excel As New Excel.Application
        Dim wBook As Excel.Workbook
        Dim wSheet As Excel.Worksheet

        wBook = _excel.Workbooks.Open(strFileName)
        wSheet = wBook.ActiveSheet()

        Dim dt As System.Data.DataTable = dtSource
        Dim dc As System.Data.DataColumn
        Dim dr As System.Data.DataRow
        Dim colIndex As Integer = 0
        Dim rowIndex As Integer = 0

        For Each dc In dt.Columns
            colIndex = colIndex + 1
            wSheet.Cells(1, colIndex) = dc.ColumnName
        Next

        For Each dr In dt.Rows
            rowIndex = rowIndex + 1
            colIndex = 0
            For Each dc In dt.Columns
                colIndex = colIndex + 1
                wSheet.Cells(rowIndex + 1, colIndex) = dr(dc.ColumnName)
            Next
        Next
        wSheet.Columns.AutoFit()
        wBook.Save()

        ReleaseObject(wSheet)
        wBook.Close(False)
        ReleaseObject(wBook)
        _excel.Quit()
        ReleaseObject(_excel)
        GC.Collect()
    End Sub

    Private Sub ReleaseObject(ByVal o As Object)
        Try
            While (System.Runtime.InteropServices.Marshal.ReleaseComObject(o) > 0)
            End While
        Catch
        Finally
            o = Nothing
        End Try
    End Sub

End Class
