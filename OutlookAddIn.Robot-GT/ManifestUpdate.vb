Imports DevExpress.XtraRichEdit
Imports Microsoft.Office.Interop
Imports System.Data
Imports System.IO

Public Class ManifestUpdate
    Dim oLogFileGenerate As New LogFileGenerate
    Dim oDataAccess As New DataAccess
    Dim oCreateMailItem As New CreateMailItem
    Friend oMailItem As Outlook.MailItem
    Friend drConfiguration As DataRow
    Dim dIniDateTime, dFinDateTime As DateTime
    Friend Sub StartProcess()
        dIniDateTime = Now
        Dim sFileName = FileIO.FileSystem.GetTempFileName
        Dim dtSource, dtResult As New DataTable

        For a = 1 To oMailItem.Attachments.Count
            If oMailItem.Attachments(a).FileName.ToUpper.Contains("XLS") Then
                sFileName = My.Settings.ProcessFilePath & "\" & Format(Now, "ddMMyyyy HHmmss") & " - " & oMailItem.Attachments(a).FileName
                oMailItem.Attachments(a).SaveAsFile(sFileName)
                If Not IO.File.Exists(sFileName) Then
                    oLogFileGenerate.TextFileUpdate(drConfiguration("Identifier"), "No se descargó el archivo adjunto.")
                    oCreateMailItem.SendErrorMessage(oMailItem, drConfiguration("Identifier"), "No se descargó el archivo adjunto.")
                    Return
                End If
            End If
        Next
        dtSource = LoadExcel(sFileName, "{0}").Tables(0)
        If dtSource.Rows.Count = 0 Then
            oLogFileGenerate.TextFileUpdate(drConfiguration("Identifier"), "El archivo adjunto no contiene datos.")
            Return
        End If
        Try
            dtResult = oDataAccess.ExecuteAccessQuery("SELECT * FROM ManifestControl WHERE BillOfLading='#'").Tables(0)
            Dim iPos As Integer = 0
            Dim _FilasImportadas, _FilasExcluidas, _TotalFilas As Integer
            _TotalFilas = dtSource.Rows.Count
            For r = 0 To dtSource.Rows.Count - 1
                Dim oRow As DataRow = dtSource.Rows(r)
                If IsDBNull(oRow(0)) Then
                    Continue For
                End If
                If oDataAccess.ExecuteAccessQuery("SELECT BillOfLading FROM ManifestControl WHERE BillOfLading = '" & oRow("CODIGO BL") & "'").Tables(0).Rows.Count > 0 Then
                    _FilasExcluidas += 1
                    Continue For
                End If
                dtResult.Rows.Add()
                iPos = dtResult.Rows.Count - 1
                dtResult.Rows(iPos)("SourceCountry") = My.Settings.SourceCountry
                dtResult.Rows(iPos)("VesselName") = Mid(oRow("NOMBRE ITINERARIO"), 1, InStrRev(oRow("NOMBRE ITINERARIO"), " ")).Trim
                dtResult.Rows(iPos)("ScheduleVoyage") = Mid(oRow("NOMBRE ITINERARIO"), InStrRev(oRow("NOMBRE ITINERARIO"), " "), Len(oRow("NOMBRE ITINERARIO"))).Trim
                dtResult.Rows(iPos)("Port_Locode") = oRow("PUERTO ARRIBO")
                dtResult.Rows(iPos)("OriginPort") = oRow("PUERTO ORIGEN")
                'dtResult.Rows(iPos)("DischargePort") = oRow("PUERTO DESTINO")
                dtResult.Rows(iPos)("FinalPort") = oRow("PUERTO DESTINO")
                dtResult.Rows(iPos)("ArrivalDate") = oRow("fec_arribo")
                dtResult.Rows(iPos)("ManifestNumber") = oRow("MRN")
                dtResult.Rows(iPos)("SecuencialNumber") = oRow("SECUENCIA BL")
                dtResult.Rows(iPos)("BillOfLading") = oRow("CODIGO BL")
                dtResult.Rows(iPos)("TerminalName") = oRow("NOMBRE TERMINAL")
                dtResult.Rows(iPos)("ConsigneeName") = oRow("NOMBRE CONSIGNATARIO")
                If oDataAccess.InsertIntoAccess("ManifestControl", dtResult.Rows(iPos)) Then
                    _FilasImportadas += 1
                End If
            Next
            Dim oMailBody As New RichEditControl
            dFinDateTime = Now
            oMailBody.LoadDocument(AppPath & "\Layout\Respuesta a Proceso.docx")
            oMailBody.Text = oMailBody.Text.Replace("[Sender]", oMailItem.SenderName)
            oMailBody.Text = oMailBody.Text.Replace("[Asunto]", oMailItem.Subject)
            oMailBody.Text = oMailBody.Text.Replace("[FechaHoraInicial]", dIniDateTime.ToString)
            oMailBody.Text = oMailBody.Text.Replace("[FechaHoraFinal]", dFinDateTime.ToString)
            oMailBody.Text = oMailBody.Text.Replace("[TiempoTranscurrido]", DateDiff(DateInterval.Minute, dIniDateTime, dFinDateTime).ToString & " minutos")

            oMailBody.Text = oMailBody.Text.Replace("[FilasImportadas]", _FilasImportadas.ToString)
            oMailBody.Text = oMailBody.Text.Replace("[FilasExcluidas]", _FilasExcluidas.ToString)
            oMailBody.Text = oMailBody.Text.Replace("[TotalFilas]", _TotalFilas.ToString)

            oCreateMailItem.ProcessMessageResponse(oMailItem, "CONFIRMACIÓN DE PROCESO", oMailBody)
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate(drConfiguration("Identifier"), ex.Message)
            oCreateMailItem.SendErrorMessage(oMailItem, drConfiguration("Identifier"), ex.Message)
        End Try

    End Sub

End Class
