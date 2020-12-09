Imports System.Windows.Forms
Imports Microsoft.Office.Interop
Imports System.Data
Imports System.Runtime.InteropServices
Imports System.Reflection

Public Class PurchaseOrderControl
    Dim oLogProcessUpdate As New LogProcessUpdate
    Dim oLogFileGenerate As New LogFileGenerate
    Dim iLogProcess As Integer = 0
    Dim iLastRowItems As Integer = 0
    Dim drMailProcess As DataRow
    Dim sVendorContact As String

    Friend Sub StartProcess(Items As Object)
        Dim ProcessCode As String = "POC"
        Dim iPos As Integer = 0
        Dim oMailItems As Outlook.MailItem = Items
        Dim oTxtboxPdf As New RichTextBox
        Dim sFileName = FileIO.FileSystem.GetTempFileName
        Dim sUserProcess As String = ""

        drMailProcess = GetMailProcess(ProcessCode)
        iLogProcess = oLogProcessUpdate.GetIdLogProcess(ProcessCode)

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
        'Dim oXls As New Excel.Application
        Dim oXls As Object = CreateObject("Excel.Application")
        oXls.Workbooks.Open(Filename:=sFileName, ReadOnly:=True)
        'oXls.Visible = False
        'Dim oSheet As New Excel.Worksheet
        'oSheet = oXls.Sheets(1)
        'Dim oRange As Excel.Range = oSheet.Range("A1:L500")
        Dim oRange As Excel.Range = oXls.Sheets(1).Range("A1:L500")
        Dim dtPurchaseOrderControl As New DataTable
        Dim WorkOrder As String = oRange.Cells(10, 5).Value.ToString
        Dim Liquidation As String = GetLiquidation(oRange, "LIQ")
        sUserProcess = oRange.Cells(12, 5).Value.ToString.Trim
        dtPurchaseOrderControl = ExecuteAccessQuery("SELECT * FROM PurchaseOrderControl WHERE WorkOrder='" & WorkOrder & "'", "").Tables(0)
        If dtPurchaseOrderControl.Rows.Count > 0 Then
            ExecuteAccessNonQuery("DELETE FROM PurchaseOrderControl WHERE WorkOrder='" & WorkOrder & "'", "")
        End If
        iLastRowItems = GetLastRowNo(oRange)
        Dim iRows As Integer = iLastRowItems
        Dim VendorCode As String = oRange.Cells(17, 1).Value.ToString()
        Dim sVessel, sVoyage As String
        sVessel = oRange.Cells(22, 1).Value.ToString.TrimEnd
        sVoyage = oRange.Cells(22, 3).Value.ToString.TrimEnd
        Try
            For r = 26 To iRows
                dtPurchaseOrderControl.Rows.Add()
                iPos = dtPurchaseOrderControl.Rows.Count - 1
                dtPurchaseOrderControl.Rows(iPos)(0) = iLogProcess
                dtPurchaseOrderControl.Rows(iPos)(1) = GetDPVoyage(oRange.Cells(22, 1).Value.ToString.Trim, oRange.Cells(22, 3).Value.ToString.Trim, oRange.Cells(22, 5).Value.ToString)
                dtPurchaseOrderControl.Rows(iPos)(2) = sVessel
                dtPurchaseOrderControl.Rows(iPos)(3) = sVoyage
                dtPurchaseOrderControl.Rows(iPos)(4) = oRange.Cells(22, 5).Value.ToString
                dtPurchaseOrderControl.Rows(iPos)(5) = oRange.Cells(17, 1).Value.ToString
                dtPurchaseOrderControl.Rows(iPos)(6) = oRange.Cells(9, 1).Value.ToString
                dtPurchaseOrderControl.Rows(iPos)(7) = WorkOrder
                dtPurchaseOrderControl.Rows(iPos)(8) = Liquidation.ToString
                dtPurchaseOrderControl.Rows(iPos)(9) = oRange.Cells(r, 1).Value
                dtPurchaseOrderControl.Rows(iPos)(10) = Replace(oRange.Cells(r, 2).Value, "'", "")
                dtPurchaseOrderControl.Rows(iPos)(11) = oRange.Cells(r, 6).Value
                dtPurchaseOrderControl.Rows(iPos)(12) = oRange.Cells(r, 7).Value
                dtPurchaseOrderControl.Rows(iPos)(13) = Now
                dtPurchaseOrderControl.Rows(iPos)(14) = oMailItems.Subject
                dtPurchaseOrderControl.Rows(iPos)(15) = oMailItems.To
                dtPurchaseOrderControl.Rows(iPos)(16) = sUserProcess 'Environment.UserDomainName & "\" & Environment.UserName
                dtPurchaseOrderControl.Rows(iPos)(17) = Now
                InsertIntoAccess("PurchaseOrderControl", dtPurchaseOrderControl.Rows(iPos), "")
                oLogProcessUpdate.SetLogProcessItem(iLogProcess, ProcessCode, WorkOrder.ToString, dtPurchaseOrderControl.Rows(iPos)(8), sUserProcess)
            Next
        Catch ex As Exception
            oLogProcessUpdate.SetDescriptionLogProcess(iLogProcess, iPos, sUserProcess, "Error al actualizar la tabla PurchaseOrderControl. " & ex.Message)
        End Try
        Try
            GC.Collect()
            GC.WaitForPendingFinalizers()
            oXls.ActiveWorkbook.Close(False, sFileName, Missing.Value)
            oXls.Workbooks.Close()
            oXls.Quit()
            If Not oXls.Workbooks Is Nothing Then
                Marshal.ReleaseComObject(oXls.Workbooks)
            End If
            If Not oXls Is Nothing Then
                Marshal.ReleaseComObject(oXls)
            End If
            If Not oRange Is Nothing Then
                Marshal.ReleaseComObject(oRange)
            End If
            oXls = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate("PURCHASE ORDER CONTROL", ex.Message)
            SendNewMessage("PRC_ERROR", oMailItems, "PURCHASE ORDER CONTROL", "Error al cerrar Excel. " & ex.Message)
        End Try

        'Reenviar Correo al Proveedor
        sVendorContact = GetVendorContact(ProcessCode, VendorCode)
        Dim oInspector As Outlook.Inspector
        Dim oForwardMail As Outlook.MailItem = Items
        oInspector = Items.GetInspector
        oForwardMail = oInspector.CurrentItem.Forward
        'oForwardMail.Forward()
        oForwardMail.HTMLBody = "Estimado Proveedor, <br><br>"
        oForwardMail.HTMLBody += "Adjuntamos WO " & WorkOrder
        If Liquidation.ToString <> "" Then
            oForwardMail.HTMLBody += " para DRAFT " & Liquidation.ToString & " ( " & sVessel & Space(1) & sVoyage & " )"
        End If
        oForwardMail.HTMLBody += ", si tiene alguna observación agradeceremos enviarnos su consulta a los siguientes correos: <br><br>"
        oForwardMail.HTMLBody += "Perú Port Terminal Ops - PeruPortTerminalOps@hlag.com <br>"
        oForwardMail.HTMLBody += "Paipay, Richard - Richard.Paipay@hlag.com <br><br><br>"
        oForwardMail.HTMLBody += drConfig("Firma")
        If sVendorContact <> "" Then
            oForwardMail.To = sVendorContact
        End If
        If Not drMailProcess Is Nothing Then
            If Not IsDBNull(drMailProcess("MailCC")) Then
                oForwardMail.CC = drMailProcess("MailCC")
            End If
            If Not IsDBNull(drMailProcess("MailBCC")) Then
                oForwardMail.BCC = drMailProcess("MailBCC")
            End If
        End If
        'oForwardMail.Display()
        oForwardMail.Send()
    End Sub

    Function GetLiquidation(ByVal SheetRng As Excel.Range, ByVal searchTxt As String) As String
        Dim sResult As String = ""
        Dim oRange As Excel.Range = Nothing
        oRange = FindAll(SheetRng, searchTxt)
        If Not oRange Is Nothing Then
            sResult = Replace(Replace(oRange.Value, searchTxt, ""), ".", "").Trim
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

    Function GetDPVoyage(Vessel As String, Voyage As String, Port As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("SELECT DPVOYAGE FROM ScheduleVoyage WHERE VESSEL_NAME='" & Vessel & "' AND SCHEDULE='" & Voyage & "' AND POL='" & Port & "'", "dbColdTreatment.accdb").Tables(0)
        If dtQuery.Rows.Count = 0 Then
            Return sResult
        End If
        sResult = dtQuery.Rows(0)(0)
        Return sResult
    End Function

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
