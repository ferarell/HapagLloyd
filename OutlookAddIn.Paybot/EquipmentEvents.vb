Imports System
Imports System.Data
Imports System.IO
Imports System.Collections
Imports DevExpress.XtraRichEdit

Public Class EquipmentEvents
    Dim oAppService As New AppService.HapagLloydServiceClient
    Dim oSharePointTransactions As New SharePointListTransactions
    Dim oLogFileGenerate As New LogFileGenerate
    Dim oCreateMailItem As New CreateMailItem
    Dim ProcessLogName As String = "EQEO1601"
    Dim sFileName As String = ""

    Friend Sub StartProcess(oMailItems As Outlook.MailItem)
        Dim dtColdTreatmentDevice, dtSource As New DataTable
        Dim dIniDateTime, dFinDateTime As DateTime
        Dim iPosition As Integer = 0
        Dim CodeProcess As String = "EQE"
        Dim aAttachments As New ArrayList
        Dim aResult As New ArrayList

        dIniDateTime = Now
        dtColdTreatmentDevice = ExecuteAccessQuery("SELECT * FROM ColdTreatmentDevice", Nothing).Tables(0)
        If dtColdTreatmentDevice.Rows.Count = 0 Then
            SendErrorMessage(oMailItems, ProcessLogName, "No fue posible obtener la lista de bookings con HID.", Nothing)
            Return
        End If

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
        dtSource = LoadExcelWCH(sFileName, "{0}", 3, "")
        If dtSource.Rows.Count = 0 Then
            oLogFileGenerate.TextFileUpdate(ProcessLogName, "El archivo plano no contiene datos")
            SendErrorMessage(oMailItems, ProcessLogName, "El archivo plano no contiene datos", Nothing)
            Return
        End If
        Dim iPos As Integer = 0
        For r = 0 To dtSource.Rows.Count - 1
            Dim oRowSource As DataRow = dtSource.Rows(r)
            If IsDBNull(oRowSource(0)) Or IsDBNull(oRowSource("Event Code")) Then
                Continue For
            End If
            If oRowSource("Event Code") = "GOMT" Then
                If dtColdTreatmentDevice.Select("BookingNumber='" & oRowSource("Shipment No") & "'").Length > 0 Then
                    Dim oRowTarget As DataRow = dtColdTreatmentDevice.Select("BookingNumber='" & oRowSource("Shipment No") & "'")(0)
                    oSharePointTransactions.SharePointUrl = My.Settings.SharePoint_Url
                    oSharePointTransactions.SharePointList = "ColdTreatmentDevice"
                    oSharePointTransactions.ValuesList.Clear()
                    If oRowTarget("ContainerNumber").ToString = "" Or oRowTarget("ContainerNumber").ToString <> oRowSource("EQ Number").ToString Then
                        oSharePointTransactions.ValuesList.Add({"ContainerType", oRowSource("Type Group")})
                        oSharePointTransactions.ValuesList.Add({"ContainerNumber", oRowSource("EQ Number")})
                    End If
                    oSharePointTransactions.ValuesList.Add({"EventDate", oRowSource("Event Date")})
                    oSharePointTransactions.ValuesList.Add({"ReportingDate", oRowSource("Reporting Date")})
                    oSharePointTransactions.UpdateItem(oRowTarget("ID"))
                End If
            End If
            If oRowSource("Event Code") = "LOFU" Then
                If dtColdTreatmentDevice.Select("BookingNumber='" & oRowSource("Shipment No") & "' AND ContainerNumber='" & oRowSource("EQ Number") & "'").Length > 0 Then
                    Dim oRowTarget As DataRow = dtColdTreatmentDevice.Select("BookingNumber='" & oRowSource("Shipment No") & "' AND ContainerNumber='" & oRowSource("EQ Number") & "'")(0)
                    oSharePointTransactions.SharePointUrl = My.Settings.SharePoint_Url
                    oSharePointTransactions.SharePointList = "ColdTreatmentDevice"
                    oSharePointTransactions.ValuesList.Clear()
                    oSharePointTransactions.ValuesList.Add({"EventDate_LOFU", oRowSource("Event Date")})
                    oSharePointTransactions.ValuesList.Add({"ReportingDate_LOFU", oRowSource("Reporting Date")})
                    oSharePointTransactions.ValuesList.Add({"DPVoyage_LOFU", oRowSource("DP Voyage")})
                    oSharePointTransactions.ValuesList.Add({"VesselName_LOFU", oRowSource("Vessel Name")})
                    oSharePointTransactions.UpdateItem(oRowTarget("ID"))
                End If
            End If
        Next
        oLogFileGenerate.TextFileUpdate(ProcessLogName, "El proceso asociado al mensaje: " & oMailItems.Subject & " finalizó satisfactoriamente.")
        Dim oMailBody As New RichEditControl
        dFinDateTime = Now
        oMailBody.LoadDocument(AppPath & "\Layout\Respuesta a Proceso.docx")
        oMailBody.Text = oMailBody.Text.Replace("[Sender]", oMailItems.SenderName)
        oMailBody.Text = oMailBody.Text.Replace("[Asunto]", oMailItems.Subject)
        oMailBody.Text = oMailBody.Text.Replace("[FechaHoraInicial]", dIniDateTime.ToString)
        oMailBody.Text = oMailBody.Text.Replace("[FechaHoraFinal]", dFinDateTime.ToString)
        oMailBody.Text = oMailBody.Text.Replace("[TiempoTranscurrido]", DateDiff(DateInterval.Minute, dIniDateTime, dFinDateTime).ToString & " minutos")
        oMailBody.Text = oMailBody.Text.Replace("[FilasProcesadas]", dtSource.Rows.Count.ToString)

        oCreateMailItem.ProcessMessageResponse(oMailItems, "CONFIRMACIÓN DE PROCESO", oMailBody, ProcessLogName)
    End Sub
End Class
