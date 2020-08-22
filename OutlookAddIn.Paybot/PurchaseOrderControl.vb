Imports System.Windows.Forms
Imports Microsoft.Office.Interop
'Imports Microsoft.Office.Interop.Excel
Imports System.Data

Public Class PurchaseOrderControl
    Dim oLogProcessUpdate As New LogProcessUpdate
    Dim oLogFileGenerate As New LogFileGenerate
    Dim iLastRowItems As Integer = 0

    Friend Sub StartProcess(Items As Object)
        Dim CodeProcess As String = "POC"
        Dim iPos As Integer = 0
        Dim oMailItems As Outlook.MailItem = Items
        Dim oTxtboxPdf As New RichTextBox
        Dim sFileName = FileIO.FileSystem.GetTempFileName

        For a = 1 To oMailItems.Attachments.Count
            If oMailItems.Attachments(a).FileName.ToUpper.Contains("XLS") Then
                sFileName = My.Settings.AttachedFilePath & "\" & Format(Now, "ddMMyyyy HHmmss") & " - " & oMailItems.Attachments(a).FileName
                oMailItems.Attachments(a).SaveAsFile(sFileName)
                If Not IO.File.Exists(sFileName) Then
                    oLogFileGenerate.TextFileUpdate("PURCHASE ORDER CONTROL", "No se descargó el archivo adjunto.")
                    SendNewMessage("PRC_ERROR", oMailItems, "PURCHASE ORDER CONTROL", "No se descargó el archivo adjunto.")
                    Return
                End If
            End If
        Next
        Dim oXls As New Excel.Application
        oXls.Workbooks.Open(sFileName, , False)
        oXls.Visible = False
        Dim oSheet As New Excel.Worksheet
        oSheet = oXls.Sheets(1)
        Dim oRange As Excel.Range = oSheet.Range("A1:L500")
        Dim dtPurchaseOrderControl As New DataTable
        Dim WorkOrder As String = oRange.Cells(10, 5).Value.ToString
        dtPurchaseOrderControl = ExecuteAccessQuery("SELECT * FROM PurchaseOrderControl WHERE WorkOrder='" & WorkOrder & "'", "").Tables(0)
        If dtPurchaseOrderControl.Rows.Count > 0 Then
            ExecuteAccessNonQuery("DELETE FROM PurchaseOrderControl WHERE WorkOrder='" & WorkOrder & "'", "")
        End If
        iLastRowItems = GetLastRowNo(oRange)
        Dim iRows As Integer = iLastRowItems
        For r = 26 To iRows
            dtPurchaseOrderControl.Rows.Add()
            iPos = dtPurchaseOrderControl.Rows.Count - 1
            dtPurchaseOrderControl.Rows(iPos)(0) = GetDPVoyage(oRange.Cells(22, 1).Value.ToString.Trim, oRange.Cells(22, 3).Value.ToString.Trim, oRange.Cells(22, 5).Value.ToString)
            dtPurchaseOrderControl.Rows(iPos)(1) = oRange.Cells(22, 1).Value.ToString.TrimEnd
            dtPurchaseOrderControl.Rows(iPos)(2) = oRange.Cells(22, 3).Value.ToString.TrimEnd
            dtPurchaseOrderControl.Rows(iPos)(3) = oRange.Cells(22, 5).Value.ToString
            dtPurchaseOrderControl.Rows(iPos)(4) = oRange.Cells(17, 1).Value.ToString
            dtPurchaseOrderControl.Rows(iPos)(5) = oRange.Cells(9, 1).Value.ToString
            dtPurchaseOrderControl.Rows(iPos)(6) = WorkOrder
            dtPurchaseOrderControl.Rows(iPos)(7) = GetLiquidation(oRange, "LIQ.")
            dtPurchaseOrderControl.Rows(iPos)(8) = oRange.Cells(r, 1).Value
            dtPurchaseOrderControl.Rows(iPos)(9) = Replace(oRange.Cells(r, 2).Value, "'", "")
            dtPurchaseOrderControl.Rows(iPos)(10) = oRange.Cells(r, 6).Value
            dtPurchaseOrderControl.Rows(iPos)(11) = oRange.Cells(r, 7).Value
            dtPurchaseOrderControl.Rows(iPos)(12) = Now
            dtPurchaseOrderControl.Rows(iPos)(13) = oMailItems.Subject
            dtPurchaseOrderControl.Rows(iPos)(14) = oMailItems.To
            dtPurchaseOrderControl.Rows(iPos)(15) = Environment.UserDomainName & "\" & Environment.UserName
            dtPurchaseOrderControl.Rows(iPos)(16) = Now
            InsertIntoAccess("PurchaseOrderControl", dtPurchaseOrderControl.Rows(iPos), "", Nothing, Nothing)
        Next

    End Sub

    Function GetLiquidation(ByVal SheetRng As Excel.Range, ByVal searchTxt As String) As String
        Dim sResult As String = ""
        Dim oRange As Excel.Range = Nothing
        oRange = FindAll(SheetRng, searchTxt)
        If Not oRange Is Nothing Then
            sResult = Replace(oRange.Value, searchTxt, "").Trim
        End If
        Return sResult
    End Function

    Function GetLastRowNo(oRange As Object) As Integer
        Dim iResult As Integer = 0
        Dim iPos As Integer = 26
        While IsNumeric(oRange.Cells(iPos, 1).Value)
            iResult += 1
            iPos += 1
        End While
        Return iResult + 25
    End Function

    'Function GetDPVoyage(Vessel As String, Voyage As String, Port As String) As String
    '    Dim sResult As String = ""
    '    Dim dtQuery As New DataTable
    '    dtQuery = ExecuteAccessQuery("SELECT DPVOYAGE FROM ScheduleVoyage WHERE VESSEL_NAME='" & Vessel & "' AND SCHEDULE='" & Voyage & "' AND POL='" & Port & "'", "").Tables(0)
    '    If dtQuery.Rows.Count = 0 Then
    '        Return sResult
    '    End If
    '    sResult = dtQuery.Rows(0)(0)
    '    Return sResult
    'End Function

    Function FindAll(ByVal SheetRng As Excel.Range, ByVal searchTxt As String) As Excel.Range
        Dim currentFind As Excel.Range = Nothing
        Dim firstFind As Excel.Range = Nothing

        currentFind = SheetRng.Find(searchTxt, ,
        Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart,
        Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, False)
        While Not currentFind Is Nothing
            ' Keep track of the first range you find.
            If firstFind Is Nothing Then
                firstFind = currentFind
                ' If you didn't move to a new range, you are done.
            ElseIf currentFind.Address = firstFind.Address Then
                Exit While
            End If
            With currentFind.Font
                .Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
                .Bold = True
            End With
            currentFind = SheetRng.FindNext(currentFind)
        End While
        Return currentFind
    End Function
End Class
