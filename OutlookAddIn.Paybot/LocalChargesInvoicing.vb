Imports System
Imports System.Data
Imports System.IO
Imports System.Collections
Imports DevExpress.XtraRichEdit

Public Class LocalChargesInvoicing
    Dim oAppService As New AppService.HapagLloydServiceClient
    Dim oSharePointTransactions As New SharePointListTransactions
    Dim oLogFileGenerate As New LogFileGenerate
    Dim oCreateMailItem As New CreateMailItem
    Dim ProcessLogName As String = "INVS0201"
    Dim sFileName As String = ""

    Friend Sub StartProcess(oMailItems As Outlook.MailItem)
        Dim dtChargeList, dtResult, dtSource As New DataTable
        Dim dIniDateTime, dFinDateTime As DateTime
        Dim iPosition As Integer = 0
        Dim CodeProcess As String = "201"
        Dim aAttachments As New ArrayList
        Dim aResult As New ArrayList

        dIniDateTime = Now
        dtChargeList = ExecuteAccessQuery("SELECT Code FROM ChargeList", Nothing).Tables(0)
        If dtChargeList.Rows.Count = 0 Then
            SendErrorMessage(oMailItems, ProcessLogName, "No fue posible obtener la lista de charges.", Nothing)
            Return
        End If

        For a = 1 To oMailItems.Attachments.Count
            If oMailItems.Attachments(a).FileName.ToUpper.Contains("CSV") Then
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
        dtResult = oAppService.ExecuteSQL("SELECT TOP 0 * FROM ctr.LocalChargesInvoicing").Tables(0)
        dtSource = LoadCSV(sFileName, True)
        If dtSource.Rows.Count = 0 Or (dtSource.Rows.Count = 1 And IsDBNull(dtSource.Rows(0)("INVOICE"))) Then
            oLogFileGenerate.TextFileUpdate(ProcessLogName, "El archivo plano no contiene datos")
            SendErrorMessage(oMailItems, ProcessLogName, "El archivo plano no contiene datos", Nothing)
            Return
        End If
        Dim iPos As Integer = 0
        For r = 0 To dtSource.Rows.Count - 1
            Dim oRow As DataRow = dtSource.Rows(r)
            For c = 0 To dtChargeList.Rows.Count - 1
                Dim oCol As DataRow = dtChargeList.Rows(c)
                If dtSource.Columns.Contains(oCol("Code")) And oCol("Code") <> "POD" Then
                    If dtSource.Columns(oCol("Code")) IsNot Nothing Then
                        If CDec(oRow(oCol("Code")).ToString.Trim) > 0 Then
                            dtResult.Rows.Add()
                            iPos = dtResult.Rows.Count - 1
                            dtResult.Rows(iPos)("CHARGE_CODE") = oCol("Code")
                            dtResult.Rows(iPos)("CHARGE_AMOUNT") = CDec(oRow(oCol("Code")).ToString.Trim)
                            dtResult.Rows(iPos)("INVOICE") = oRow("INVOICE")
                            dtResult.Rows(iPos)("VESSEL") = oRow("VESSEL")
                            dtResult.Rows(iPos)("VOYAGE") = oRow("VOYAGE")
                            dtResult.Rows(iPos)("DP_VOYAGE") = oRow("DP-VOY")
                            dtResult.Rows(iPos)("DIR") = oRow("DIR")
                            dtResult.Rows(iPos)("CONTAINER") = oRow("CONTAINER")
                            dtResult.Rows(iPos)("CTR_TYPE") = oRow("CTR-TYPE")
                            dtResult.Rows(iPos)("POL") = oRow("POL")
                            dtResult.Rows(iPos)("POD") = oRow("POD")
                            dtResult.Rows(iPos)("BL") = oRow("BL")
                            dtResult.Rows(iPos)("REF_DATE") = Mid(oRow("DATE"), 7, 4) & "-" & Mid(oRow("DATE"), 4, 2) & "-" & Mid(oRow("DATE"), 1, 2)
                            dtResult.Rows(iPos)("QTY") = oRow("QTY")
                            dtResult.Rows(iPos)("WEIGHT") = oRow("WEIGHT")
                            dtResult.Rows(iPos)("CUR") = oRow("CUR")
                            dtResult.Rows(iPos)("INV_REV") = oRow("INV/REV")
                            dtResult.Rows(iPos)("INV_TYPE") = oRow("INV-TYPE")
                            dtResult.Rows(iPos)("SHIPMENT") = oRow("SHIPMENT")
                            dtResult.Rows(iPos)("MTD_GOODS_FLAG") = oRow("MTD-GOODS-FLAG")
                            dtResult.Rows(iPos)("Item") = oRow("/")
                            dtResult.Rows(iPos)("FC") = oRow("FC")
                            dtResult.Rows(iPos)("FP") = oRow("FP")
                        End If
                    End If
                End If
            Next
        Next
        Dim aParams As New ArrayList
        aResult.AddRange(oAppService.CustomStoredProcedureExecution("ctr.upLocalChargesInvoicingByTable_Insert", aParams.ToArray, dtResult))
        If aResult(0) = False Then
            Throw New Exception(aResult(1))
        End If
        oLogFileGenerate.TextFileUpdate(ProcessLogName, "El proceso asociado al mensaje: " & oMailItems.Subject & " finalizó satisfactoriamente.")
        Dim oMailBody As New RichEditControl
        dFinDateTime = Now
        oMailBody.LoadDocument(AppPath & "\Layout\Respuesta a Proceso.docx")
        oMailBody.Text = oMailBody.Text.Replace("[Sender]", oMailItems.SenderName)
        oMailBody.Text = oMailBody.Text.Replace("[Asunto]", oMailItems.Subject)
        oMailBody.Text = oMailBody.Text.Replace("[FechaHoraInicial]", dIniDateTime.ToString)
        oMailBody.Text = oMailBody.Text.Replace("[FechaHoraFinal]", dFinDateTime.ToString)
        oMailBody.Text = oMailBody.Text.Replace("[TiempoTranscurrido]", DateDiff(DateInterval.Minute, dIniDateTime, dFinDateTime).ToString & " minutos")
        oMailBody.Text = oMailBody.Text.Replace("[FilasProcesadas]", dtResult.Rows.Count.ToString)

        oCreateMailItem.ProcessMessageResponse(oMailItems, "CONFIRMACIÓN DE PROCESO", oMailBody, ProcessLogName)
    End Sub
End Class
