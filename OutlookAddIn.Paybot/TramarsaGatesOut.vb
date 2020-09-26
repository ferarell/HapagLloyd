Imports System.Windows.Forms
Imports System.Data
Imports BigStick.Http
Imports System.Collections
Imports System.Text.RegularExpressions

Public Class TramarsaGatesOut
    Dim oLogProcessUpdate As New LogProcessUpdate
    Dim oLogFileGenerate As New LogFileGenerate
    Dim sBooking As String = ""
    Dim oMailItems As Outlook.MailItem
    'Dim oGateOutService As New GateOutService.GateOutServicioClient

    Friend Sub StartProcess(Items As Outlook.MailItem)
        oMailItems = Items
        Dim oTxtboxPdf As New RichTextBox
        Dim sFileName = FileIO.FileSystem.GetTempFileName
        Dim oLogFileUpdate As New LogFileGenerate
        Dim sIdioma As String = "ES"
        Dim sField As String = ""
        Dim sPort As String = ""
        Dim aAttachments As New ArrayList
        'Dim Booking, RateAgreement As String
        For a = 1 To oMailItems.Attachments.Count
            If oMailItems.Attachments(a).FileName.ToUpper.Contains("PDF") Then
                sFileName = My.Settings.AttachedFilePath & "\" & Format(Now, "ddMMyyyy HHmmss") & " - " & oMailItems.Attachments(a).FileName
                oMailItems.Attachments(a).SaveAsFile(sFileName)
                aAttachments.Add(sFileName)
                If Not IO.File.Exists(sFileName) Then
                    'DevExpress.XtraEditors.XtraMessageBox.Show("No se descargó el archivo adjunto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    oLogFileUpdate.TextFileUpdate("GATE OUT", "No se descargó el archivo adjunto.")
                    SendErrorMessage(oMailItems, "GATE OUT", "No se descargó el archivo adjunto.", aAttachments)
                    Return
                End If
            End If
        Next

        Dim sFindTxt = ""
        Dim iPos As Integer = 0
        Dim oBooking As New BookingDTO
        Dim oContainer As New ContenedorDTO
        Dim oPlazo As New PlazoDTO
        oTxtboxPdf.Text = GetTextFromPDF(sFileName)
        If oTxtboxPdf.Text.IndexOf("Received from") > 0 Then
            sIdioma = "EN"
        End If
        Dim dtLines As New DataTable
        dtLines.Columns.Add("Linea", GetType(String))
        For r = 0 To oTxtboxPdf.Lines.Count - 1
            dtLines.Rows.Add(oTxtboxPdf.Lines(r))
        Next
        sFileName = Replace(sFileName.ToUpper, "PDF", "TXT")
        oTxtboxPdf.Refresh()
        If oTxtboxPdf.TextLength > 0 Then
            oTxtboxPdf.SaveFile(sFileName, RichTextBoxStreamType.PlainText)
            aAttachments.Add(sFileName)
        End If
        Dim _BookingListErr, _ContainerListErr, _PlazoListErr As New RichTextBox
        Dim sListName As String = "[Booking List] "
        'Booking List
        'Try
        sField = sListName & "(Numero) "
        Try
            If sIdioma = "ES" Then
                oBooking.Numero = GetRowCellValueByPosition(dtLines, "Nuestra Referencia", 8, 0, " ")
            Else
                oBooking.Numero = GetRowCellValueByPosition(dtLines, "Our Reference", 8, 0, " ")
            End If
            sBooking = oBooking.Numero
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & ". El proceso se cancela debido a que noo fue posible obtener el Número de Booking.")
            Return
        End Try
        sListName += " (BK:" & oBooking.Numero.ToString & ") "
        sField = sListName & "(CallSign) "
        Try
            oBooking.CallSign = GetRowCellValueByPosition(dtLines, "Call Sign", 10, 0, "")
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(ClienteNombre) "
        Try
            If sIdioma = "ES" Then
                oBooking.ClienteNombre = GetRowCellValueByPosition(dtLines, "Recibido de", 0, 1, "")
            Else
                oBooking.ClienteNombre = GetRowCellValueByPosition(dtLines, "Received from", 0, 1, "")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(ClienteDireccion) "
        Try
            oBooking.ClienteDireccion = GetPartnerAddress(oBooking.ClienteNombre)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(ClienteIdentificacion) "
        Try
            oBooking.ClienteIdentificacion = GetPartnerTaxCode(oBooking.ClienteNombre)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(ClienteTipoIdentificacion) "
        Try
            oBooking.ClienteTipoIdentificacion = IIf(oBooking.ClienteIdentificacion.Length > 0, "6", "")
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(CondicionExportacion) "
        Try
            If sIdioma = "ES" Then
                oBooking.CondicionExportacion = GetRowCellValueByPosition(dtLines, "Exportación", 0, 0, "")
            Else
                oBooking.CondicionExportacion = GetRowCellValueByPosition(dtLines, "Export", 0, 0, "")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(CondicionImportacion) "
        Try
            If sIdioma = "ES" Then
                oBooking.CondicionImportacion = GetRowCellValueByPosition(dtLines, "Importación", 0, 0, "")
            Else
                oBooking.CondicionImportacion = GetRowCellValueByPosition(dtLines, "Import", 0, 0, "")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(DepositoVacioNombre) "
        Try
            If sIdioma = "ES" Then
                oBooking.DepositoVacioNombre = GetRowCellValueByPosition(dtLines, "Retiro contr. vacío desde depósitos", 0, 2, "")
            Else
                oBooking.DepositoVacioNombre = GetRowCellValueByPosition(dtLines, "Export empty pick up depot(s)", 0, 2, "")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(DepositoVacioCodigoLocalidad) "
        Try
            oBooking.DepositoVacioCodigoLocalidad = GetPartnerCity(oBooking.DepositoVacioNombre)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(DepositoVacioDireccion) "
        Try
            oBooking.DepositoVacioDireccion = GetPartnerAddress(oBooking.DepositoVacioNombre)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(DepositoVacioIdentificacion) "
        Try
            oBooking.DepositoVacioIdentificacion = GetPartnerTaxCode(oBooking.DepositoVacioNombre)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(DepositoVacioPais) "
        Try
            oBooking.DepositoVacioPais = GetPartnerCountry(oBooking.DepositoVacioNombre)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(DetalleAduanas) "
        Try
            If sIdioma = "ES" Then
                oBooking.DetalleAduanas = GetRowCellValueByPosition(dtLines, "Detalles de Aduanas", 0, 1, "Observaciones")
            Else
                oBooking.DetalleAduanas = GetRowCellValueByPosition(dtLines, "Customs Details", 0, 1, "Remarks")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(EmailContacto) "
        Try
            oBooking.EmailContacto = GetRowCellValueByPosition(dtLines, "EmailContacto", 0, 0, "")
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(Estado) "
        Try
            If sIdioma = "ES" Then
                '0=Activo,3=Anulado/Cancelado
                oBooking.Estado = IIf(GetRowCellValueByPosition(dtLines, "Confirmación de Reserva", 0, 0, "").Contains({"ORIGINAL", "UPDATE"}), 0, 3)
            Else
                oBooking.Estado = IIf(GetRowCellValueByPosition(dtLines, "Booking Confirmation", 0, 0, "").Contains({"ORIGINAL", "UPDATE"}), 0, 3)
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(FechaEmision) "
        Try
            oBooking.FechaEmision = CDate(GetRowCellValueByPosition(dtLines, "Date of Issue", 20, 0, ""))
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(FechaEstimadaArribo) "
        oBooking.FechaEstimadaArribo = CDate("01/01/1900")
        Try
            If oBooking.Estado = "0" Then
                sFindTxt = GetRowCellValueByPosition(dtLines, "Flag", 0, 1, "") & Space(1) & GetRowCellValueByPosition(dtLines, "Flag", 5, 2, "")
                If sFindTxt.Trim <> "" Then
                    sField = sListName & "(FechaEstimadaArribo) "
                    If IsDate(sFindTxt) Then
                        oBooking.FechaEstimadaArribo = CDate(sFindTxt)
                    End If
                End If
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(FechaEstimadaZarpe) "
        oBooking.FechaEstimadaZarpe = CDate("01/01/1900")
        Try
            If oBooking.Estado = "0" Then
                sFindTxt = GetRowCellValueByPosition(dtLines, "Flag", 0, 3, "") & Space(1) & GetRowCellValueByPosition(dtLines, "Flag", 5, 4, "")
                If sFindTxt.Trim <> "" Then
                    sField = sListName & "(FechaEstimadaZarpe) "
                    If IsDate(sFindTxt) Then
                        oBooking.FechaEstimadaZarpe = CDate(sFindTxt)
                    End If
                End If
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(ForwarderNombre) "
        Try
            If sIdioma = "ES" Then
                oBooking.ForwarderNombre = GetRowCellValueByPosition(dtLines, "PORT FORWARDER", 0, 1, "")
            Else
                oBooking.ForwarderNombre = GetRowCellValueByPosition(dtLines, "FREIGHT FORWARDER", 0, 1, "")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(ForwarderDireccion) "
        Try
            oBooking.ForwarderDireccion = GetPartnerAddress(oBooking.ForwarderNombre)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(ForwarderIdentificacion) "
        Try
            oBooking.ForwarderIdentificacion = GetPartnerTaxCode(oBooking.ForwarderNombre)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(ForwarderTipoIdentificacion) "
        Try
            oBooking.ForwarderTipoIdentificacion = IIf(oBooking.ForwarderIdentificacion.Length > 0, "6", "")
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(IndicadorDG) "
        Try
            oBooking.IndicadorDG = ValidateValueByPosition(dtLines, "✖ DG", 0)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(IndicadorOOG) "
        Try
            oBooking.IndicadorOOG = ValidateValueByPosition(dtLines, "✖ OOG", 0)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(IndicadorSOW) "
        Try
            oBooking.IndicadorSOW = ValidateValueByPosition(dtLines, "✖ SOW", 0)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(IndicadorTemp) "
        Try
            oBooking.IndicadorTemp = ValidateValueByPosition(dtLines, "✖ Temp", 0)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(NombreContacto) "
        Try
            If sIdioma = "ES" Then
                oBooking.NombreContacto = GetRowCellValueByPosition(dtLines, "Nombre", 0, 0, "")
            Else
                oBooking.NombreContacto = GetRowCellValueByPosition(dtLines, "Name", 0, 0, "")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(NombreNave) "
        Try
            If oBooking.Estado = "0" Then
                oBooking.NombreNave = GetRowCellValueByPosition(dtLines, "Vessel", 0, 1, "")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(NroContrato) "
        Try
            oBooking.NroContrato = GetRowCellValueByPosition(dtLines, "No. de Contrato", 10, 0, "")
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(NroIMONave) "
        Try
            If oBooking.Estado = "0" Then
                oBooking.NroIMONave = GetRowCellValueByPosition(dtLines, "IMO No", 8, 0, "")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(NroViaje) "
        Try
            If oBooking.Estado = "0" Then
                oBooking.NroViaje = GetRowCellValueByPosition(dtLines, "Voy. No", 8, 0, "")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(NumeroBL) "
        Try
            If sIdioma = "ES" Then
                oBooking.NumeroBL = GetRowCellValueByPosition(dtLines, "No. de BL/SWB", 17, 0, "")
            Else
                oBooking.NumeroBL = GetRowCellValueByPosition(dtLines, "BL/SWB No(s).", 17, 0, "")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(Observaciones) "
        Try
            If sIdioma = "ES" Then
                oBooking.Observaciones = GetRowCellValueByPosition(dtLines, "Observaciones", 0, 1, "Términos Legales")
            Else
                oBooking.Observaciones = GetRowCellValueByPosition(dtLines, "Remarks", 0, 1, "Legal Terms")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(Puertos) "
        If oBooking.Estado = "0" Then
            Dim aPorts As New ArrayList
            Try
                aPorts = GetPortByLocation(dtLines)
            Catch ex As Exception
                _BookingListErr.AppendText(sField & ex.Message & "<br>")
            End Try

            If aPorts.Count > 0 Then
                sField = sListName & "(PuertoOrigen) "
                Try
                    'If sIdioma = "ES" Then
                    oBooking.PuertoOrigen = aPorts(0) 'OnlyLetters(GetRowCellValueByPosition(dtLines, "Desde Hacia Por", 0, 7, ""))
                    'Else
                    'oBooking.PuertoOrigen = GetPortByLocation(dtLines)(0) 'OnlyLetters(GetRowCellValueByPosition(dtLines, "From To By", 0, 3, ""))
                    'End If
                Catch ex As Exception
                    _BookingListErr.AppendText(sField & ex.Message & "<br>")
                End Try
                If aPorts.Count > 1 Then
                    sField = sListName & "(PuertoDestino) "
                    Try
                        'If sIdioma = "ES" Then
                        oBooking.PuertoDestino = aPorts(1) 'OnlyLetters(GetRowCellValueByPosition(dtLines, "Desde Hacia Por", 0, 10, ""))
                        'Else
                        'oBooking.PuertoDestino = OnlyLetters(GetRowCellValueByPosition(dtLines, "From To By", 0, 6, ""))
                        'End If
                    Catch ex As Exception
                        _BookingListErr.AppendText(sField & ex.Message & "<br>")
                    End Try
                    'Else
                    '    _BookingListErr.AppendText(sField & "" & "<br>")
                End If
            End If
            sField = sListName & "(DPvoyage) "
            If Not oBooking.PuertoOrigen Is Nothing Then
                sPort = IIf(oBooking.PuertoOrigen.Contains("PE"), oBooking.PuertoOrigen, "")
            End If
            If sPort = "" And Not oBooking.PuertoDestino Is Nothing Then
                sPort = IIf(oBooking.PuertoDestino.Contains("PE"), oBooking.PuertoDestino, "")
            End If
            Try
                If sPort = "" Then
                    oLogFileGenerate.TextFileUpdate("GATE OUT", "(" & "BK:" & oBooking.Numero & ")" & " El booking no tiene puerto peruano asociado, no se transferirá a Tramarsa")
                    Return
                Else
                    oBooking.DPvoyage = GetDPVoyage(oBooking.NombreNave, oBooking.NroViaje, sPort)
                End If
            Catch ex As Exception
                _BookingListErr.AppendText(sField & ex.Message & "<br>")
            End Try
        End If
        sField = sListName & "(ReferenciaExterna) "
        Try
            If sIdioma = "ES" Then
                oBooking.ReferenciaExterna = GetRowCellValueByPosition(dtLines, "Su Referencia", 0, 0, "")
            Else
                oBooking.ReferenciaExterna = GetRowCellValueByPosition(dtLines, "Your Reference", 0, 0, "")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(TelefonoContacto) "
        Try
            If sIdioma = "ES" Then
                oBooking.TelefonoContacto = GetRowCellValueByPosition(dtLines, "Teléfono", 0, 0, "")
            Else
                oBooking.TelefonoContacto = GetRowCellValueByPosition(dtLines, "Tel.", 0, 0, "")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(TerminosLegales) "
        Try
            If sIdioma = "ES" Then
                oBooking.TerminosLegales = GetValueLinesByPosition(dtLines, "Términos Legales", 8, 2)
            Else
                oBooking.TerminosLegales = GetValueLinesByPosition(dtLines, "Legal Terms", 8, 2)
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(TExportacionNombre) "
        Try
            If sIdioma = "ES" Then
                oBooking.TExportacionNombre = GetRowCellValueByPosition(dtLines, "Dirección Terminal de Exportaciones", 0, 1, "")
            Else
                oBooking.TExportacionNombre = GetRowCellValueByPosition(dtLines, "Export terminal delivery address", 0, 1, "")
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(TExportacionCodigoLocalidad) "
        Try
            oBooking.TExportacionCodigoLocalidad = GetPartnerCity(oBooking.TExportacionNombre)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(TExportacionDireccion) "
        Try
            oBooking.TExportacionDireccion = GetPartnerAddress(oBooking.TExportacionNombre)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(TExportacionIdentificacion) "
        Try
            oBooking.TExportacionIdentificacion = GetPartnerTaxCode(oBooking.TExportacionNombre)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(TExportacionPais) "
        Try
            oBooking.TExportacionPais = GetPartnerCountry(oBooking.TExportacionNombre)
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        sField = sListName & "(TImportacionNombre) "
        Try
            If sIdioma = "ES" Then
                oBooking.TImportacionNombre = IIf(GetRowCellValueByPosition(dtLines, "Dirección Terminal de Importaciones", 0, 1, "") <> "BOBC0201-059TB", GetRowCellValueByPosition(dtLines, "Dirección Terminal de Importaciones", 0, 10, ""), GetRowCellValueByPosition(dtLines, "Dirección Terminal de Importaciones", 0, 1, ""))
            Else
                oBooking.TImportacionNombre = IIf(GetRowCellValueByPosition(dtLines, "Import terminal delivery addresss", 0, 1, "") <> "BOBC0201-059TB", GetRowCellValueByPosition(dtLines, "Import terminal delivery addresss", 0, 10, ""), GetRowCellValueByPosition(dtLines, "Import terminal delivery addresss", 0, 1, ""))
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        If oBooking.TImportacionNombre <> "" Then
            sField = sListName & "(TImportacionCodigoLocalidad) "
            Try
                oBooking.TImportacionCodigoLocalidad = GetPartnerCity(oBooking.TImportacionCodigoLocalidad)
            Catch ex As Exception
                _BookingListErr.AppendText(sField & ex.Message & "<br>")
            End Try
            sField = sListName & "(TImportacionDireccion) "
            Try
                oBooking.TImportacionDireccion = GetPartnerAddress(oBooking.TImportacionCodigoLocalidad)
            Catch ex As Exception
                _BookingListErr.AppendText(sField & ex.Message & "<br>")
            End Try
            sField = sListName & "(TImportacionIdentificacion) "
            Try
                oBooking.TImportacionIdentificacion = GetPartnerTaxCode(oBooking.TImportacionCodigoLocalidad)
            Catch ex As Exception
                _BookingListErr.AppendText(sField & ex.Message & "<br>")
            End Try
            sField = sListName & "(TImportacionPais) "
            Try
                oBooking.TImportacionPais = GetPartnerCountry(oBooking.TImportacionCodigoLocalidad)
            Catch ex As Exception
                _BookingListErr.AppendText(sField & ex.Message & "<br>")
            End Try
        End If
        sField = sListName & "(VersionDocumento) "
        Try
            oBooking.VersionDocumento = 0
            If Not GetRowCellValueByPosition(dtLines, "Confirmación de Reserva", 0, 0, "").Contains("ORIGINAL") Then
                If sIdioma = "ES" Then
                    sFindTxt = "Confirmación de Reserva"
                Else
                    sFindTxt = "Booking Confirmation"
                End If
                If GetRowCellValueByPosition(dtLines, sFindTxt, 0, 0, "").Contains("ORIGINAL") Then
                    oBooking.VersionDocumento = 0
                Else
                    oBooking.VersionDocumento = OnlyNumbers(Mid(GetRowCellValueByPosition(dtLines, sFindTxt, 0, 0, ""), InStrRev(GetRowCellValueByPosition(dtLines, sFindTxt, 0, 0, ""), "-"), 6))
                End If
            End If
        Catch ex As Exception
            _BookingListErr.AppendText(sField & ex.Message & "<br>")
        End Try
        If oBooking.DPvoyage = "" And oBooking.Estado = "0" Then
            Dim _DPVoyageErr As New RichTextBox
            _DPVoyageErr.AppendText(sListName & "(DPVoyage) No fue posible obtener el dato con los siguientes valores:" & "<br>")
            _DPVoyageErr.AppendText("Nave: " & oBooking.NombreNave & " / Viaje: " & oBooking.NroViaje & " / Puerto: " & sPort)
            _BookingListErr.AppendText(_DPVoyageErr.Text)
        End If

        If _BookingListErr.TextLength > 0 Then
            oLogFileGenerate.TextFileUpdate("GATE OUT", _BookingListErr.Text)
            SendErrorMessage(oMailItems, "GATE OUT", _BookingListErr.Text, aAttachments)
        End If

        'If oBooking.Estado = "0" Then

        'Contenedor List
        sListName = "[Container List] (BK:" & oBooking.Numero.ToString & ") "
            Try
                Dim iContenedorIni, iContenedorFin As Integer
                sFindTxt = IIf(sIdioma = "ES", IIf(oBooking.CondicionExportacion.Contains("LCL /"), "Su Referencia", "Resumen"), IIf(oBooking.CondicionExportacion.Contains("LCL /"), "Your Reference", "Summary"))
                Dim sEqpType As String = GetRowCellValueByPosition(dtLines, sFindTxt, 0, 0, "")
                Dim aEqpTypeDet As ArrayList = GetEqpTypeDetail(sEqpType)

                Dim sTxtIni As String = IIf(sIdioma = "ES", "Info adicional", "Add. Info")
                Dim sTxtFin As String = IIf(sIdioma = "ES", "Detalles de Aduanas", "Customs Details")

                'Try
                iContenedorIni = GetPositionByValue(dtLines, sTxtIni) + 1
                iContenedorFin = GetPositionByValue(dtLines, sTxtFin) - 1

                Dim iCol As Integer = 0
                Dim sFechaRetiro As String = ""

                If oBooking.CondicionExportacion.Contains("LCL /") Then
                    aEqpTypeDet = GetEqpTypeDetailLcl(sEqpType)
                    For r = 0 To aEqpTypeDet.Count - 1
                        For d = 1 To aEqpTypeDet(r)(0)
                            oContainer.Item = d
                            oContainer.TipoContenedor = aEqpTypeDet(r)(1)
                            oContainer.DepositoRetiro = ""
                            oContainer.IndicadorShipperOwn = "N"
                            oContainer.InfoAdicional = ""
                            oContainer.Mercancia = ""
                            oContainer.NroContenedor = ""
                            oBooking.ContenedorList.Add(New ContenedorDTO With {.DepositoRetiro = oContainer.DepositoRetiro, _
                                                                .Detalle = oContainer.Detalle, _
                                                                .FechaHoraRetiro = oContainer.FechaHoraRetiro, _
                                                                .IndicadorShipperOwn = oContainer.IndicadorShipperOwn, _
                                                                .InfoAdicional = oContainer.InfoAdicional, _
                                                                .Item = oContainer.Item, _
                                                                .Mercancia = oContainer.Mercancia, _
                                                                .NroContenedor = oContainer.NroContenedor, _
                                                                .TipoContenedor = oContainer.TipoContenedor})
                        Next
                    Next
                Else
                    For dr = iContenedorIni To iContenedorFin
                        Try
                            sEqpType = GetValueFound(dtLines.Rows(dr)(0).ToString, aEqpTypeDet)
                            iPos = InStr(dtLines.Rows(dr)(0), sEqpType)
                            If sEqpType <> "" Then
                                sField = sListName & "(Item) "
                                oContainer.Item = Mid(dtLines.Rows(dr)(0), 1, 2)
                                sField = sListName & "(TipoContenedor) "
                                oContainer.TipoContenedor = sEqpType
                                If IsDate(Mid(dtLines.Rows(dr)(0).ToString, 23, 11)) Then 'Not Mid(dtLines.Rows(dr)(0).ToString, 8, 2).Trim.Contains({"N", "Y"}) Then
                                    sField = sListName & "(NroContenedor) "
                                    oContainer.NroContenedor = Replace(Mid(dtLines.Rows(dr)(0).ToString, iPos + 5, 12).Trim, " ", "")
                                    sField = sListName & "(IndicadorShipperOwn) "
                                    oContainer.IndicadorShipperOwn = Mid(dtLines.Rows(dr)(0).ToString, iPos + 18, 2).Trim
                                    sFechaRetiro = Mid(dtLines.Rows(dr)(0).ToString, iPos + 20, 12).Trim
                                    If Mid(dtLines.Rows(dr)(0).ToString, iPos + 31, 3).Contains("-") Then
                                        sFechaRetiro += Space(1) & Replace(Mid(dtLines.Rows(dr)(0).ToString, iPos + 34, 5), "-", ":")
                                    End If
                                    sField = sListName & "(FechaHoraRetiro) "
                                    If IsDate(sFechaRetiro) Then
                                        oContainer.FechaHoraRetiro = Convert.ToDateTime(sFechaRetiro)
                                    End If
                                Else
                                    sField = sListName & "(NroContenedor) "
                                    oContainer.NroContenedor = ""
                                    sField = sListName & "(IndicadorShipperOwn) "
                                    oContainer.IndicadorShipperOwn = Mid(dtLines.Rows(dr)(0).ToString, iPos + 5, 2).Trim
                                    sFechaRetiro = Mid(dtLines.Rows(dr)(0).ToString, iPos + 7, 12)
                                    If Mid(dtLines.Rows(dr)(0).ToString, iPos + 18, 3).Contains("-") Then
                                        sFechaRetiro += Space(1) & Replace(Mid(dtLines.Rows(dr)(0).ToString, iPos + 21, 5), "-", ":")
                                    End If
                                    sField = sListName & "(FechaHoraRetiro) "
                                    If IsDate(sFechaRetiro) Then
                                        oContainer.FechaHoraRetiro = Convert.ToDateTime(sFechaRetiro)
                                    End If
                                End If
                                dr += 1
                                If Not dtLines.Rows(dr)(0).ToString.Contains(sEqpType) Then
                                    For i = 1 To 30
                                        If dtLines.Rows(dr + 1)(0).ToString.Contains({"Commodity", "Mercancía"}) Then
                                            sField = sListName & "(Mercancia) "
                                            dr += IIf(dtLines.Rows(dr + 1)(0).ToString.Contains({"Description", "Descripción"}), 1, 2)
                                            For c = 1 To 30
                                                oContainer.Mercancia += dtLines.Rows(dr)(0).ToString + Space(1)
                                                sEqpType = GetValueFound(dtLines.Rows(dr)(0).ToString, aEqpTypeDet)
                                                If dtLines.Rows(dr + 1)(0).ToString.Contains({"DG Details"}) Or sEqpType <> "" Then
                                                    Exit For
                                                End If
                                                dr += 1
                                            Next
                                        End If
                                        sField = sListName & "(Detalle) "
                                        If dtLines.Rows(dr + 1)(0).ToString.Contains({"DG Details"}) Then
                                            dr += 2
                                            For c = 1 To 30
                                                oContainer.Detalle += dtLines.Rows(dr)(0).ToString + Space(1)
                                                sEqpType = GetValueFound(dtLines.Rows(dr)(0).ToString, aEqpTypeDet)
                                                If sEqpType <> "" Then
                                                    Exit For
                                                End If
                                                dr += 1
                                            Next
                                        End If
                                        sEqpType = GetValueFound(dtLines.Rows(dr)(0).ToString, aEqpTypeDet)
                                        If sEqpType <> "" Then
                                            dr -= 1
                                            Exit For
                                        End If
                                    Next i
                                End If


                                oBooking.ContenedorList.Add(New ContenedorDTO With {.DepositoRetiro = oContainer.DepositoRetiro, _
                                                                                    .Detalle = oContainer.Detalle, _
                                                                                    .FechaHoraRetiro = oContainer.FechaHoraRetiro, _
                                                                                    .IndicadorShipperOwn = oContainer.IndicadorShipperOwn, _
                                                                                    .InfoAdicional = oContainer.InfoAdicional, _
                                                                                    .Item = oContainer.Item, _
                                                                                    .Mercancia = oContainer.Mercancia, _
                                                                                    .NroContenedor = oContainer.NroContenedor, _
                                                                                    .TipoContenedor = oContainer.TipoContenedor})

                                oContainer.Mercancia = ""
                                oContainer.Detalle = ""
                            End If
                        Catch ex As Exception
                            _ContainerListErr.AppendText(sField & ex.Message & "<br>")
                        End Try
                    Next dr
                End If
            Catch ex As Exception
                _ContainerListErr.AppendText(sField & ex.Message & "<br>")
            End Try
        'End If
        If _ContainerListErr.TextLength > 0 Then
            oLogFileGenerate.TextFileUpdate("GATE OUT", _ContainerListErr.Text)
            SendErrorMessage(oMailItems, "GATE OUT", _ContainerListErr.Text, aAttachments)
        End If

        'Catch ex As Exception
        '    oLogFileGenerate.TextFileUpdate("GATE OUT", "[Contenedor List] - Booking: " & sBooking & " - " & ex.Message)
        '    SendErrorMessage(oMailItems, "GATE OUT", "[Contenedor List] - Booking: " & sBooking & " - " & ex.Message, sFileName)
        'End Try

        'Plazo List
        sListName = "[Plazo List] (BK:" & oBooking.Numero.ToString & ") "
        Try
            Dim iPlazoIni, iPLazoFin As Integer
            Dim sDeadline() As String = {"Shipping instruction closing", "VGM cut-off", "booking closing", "container delivery date", "delivery cut-off"}
            iPlazoIni = GetPositionByValue(dtLines, "Plazo límite") + 1
            If sIdioma = "EN" Then
                iPlazoIni = GetPositionByValue(dtLines, "Deadline") + 1
            End If
            iPLazoFin = GetPositionByValue(dtLines, "Please send your shipping instruction to") - 1
            For dr = iPlazoIni To iPLazoFin
                Try
                    'Deadline (tipo)
                    If dtLines.Rows(dr)(0).ToString.Contains(sDeadline) Then
                        For i = 1 To 3
                            sField = sListName & "(Tipo) "
                            oPlazo.Tipo += dtLines.Rows(dr)(0) + Space(1)
                            dr += 1
                            If dtLines.Rows(dr + 1)(0).ToString.Contains({"(", ")"}) Then
                                dr += 1
                                Exit For
                            End If
                        Next i
                    End If
                    sField = sListName & "(Localidad) "
                    'Location (localidad)
                    oPlazo.Localidad = OnlyLetters(dtLines.Rows(dr)(0))
                    'Date/Time (FechaHora)
                    dr += 1
                    While Not IsDate(dtLines.Rows(dr)(0))
                        dr += 1
                        If dr >= iPLazoFin Then
                            Exit While
                        End If
                    End While
                    sField = sListName & "(FechaHora) "
                    oPlazo.FechaHora = CDate(dtLines.Rows(dr)(0) & Space(1) & dtLines.Rows(dr + 1)(0))
                    dr += 2
                    'Required Action (Accion)
                    If Not dtLines.Rows(dr)(0).ToString.Contains(sDeadline) Then
                        For i = 1 To 3
                            If dr > iPLazoFin Then
                                Exit For
                            End If
                            sField = sListName & "(Accion) "
                            oPlazo.Accion += dtLines.Rows(dr)(0) + Space(1)
                            dr += 1
                            If dtLines.Rows(dr)(0).ToString.Contains(sDeadline) Then
                                dr -= 1
                                Exit For
                            End If
                        Next i
                    End If

                    oBooking.PlazoList.Add(New PlazoDTO With {.Accion = oPlazo.Accion, _
                                          .FechaHora = oPlazo.FechaHora, _
                                          .Localidad = oPlazo.Localidad, _
                                          .Tipo = oPlazo.Tipo})

                    oPlazo.Accion = ""
                    oPlazo.Tipo = ""
                Catch ex As Exception
                    _PlazoListErr.AppendText(sField & ex.Message & "<br>")
                End Try
            Next dr

        Catch ex As Exception
            _PlazoListErr.AppendText(sField & ex.Message & "<br>")
        End Try

        If _PlazoListErr.TextLength > 0 Then
            oLogFileGenerate.TextFileUpdate("GATE OUT", _PlazoListErr.Text)
            SendErrorMessage(oMailItems, "GATE OUT", _PlazoListErr.Text, aAttachments)
        End If

        Dim aResult As New ArrayList
        aResult = SendData(oBooking)

        If _BookingListErr.TextLength > 0 Or _ContainerListErr.TextLength > 0 Or _PlazoListErr.TextLength > 0 Then
            Dim outlookNameSpace As Outlook.NameSpace = ThisAddIn.outlookNameSpace
            Dim inbox As Outlook.MAPIFolder
            inbox = outlookNameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox)
            Dim DestinationFolder As Outlook.Folder = inbox.Folders("Errores GateOut")
            oMailItems.Move(DestinationFolder)
        End If

        Me.Finalize()

    End Sub

    Function GetValueFound(sLine As String, aEqpTypeDet As ArrayList) As String
        Dim sResult As String = ""
        If sLine.ToUpper.Contains({"SU REFERENCIA", "YOUR REFERENCE"}) Then
            Return sResult
        End If
        For i = 0 To aEqpTypeDet.Count - 1
            If sLine.Contains(aEqpTypeDet(i)) Then
                sResult = aEqpTypeDet(i)
            End If
        Next
        Return sResult
    End Function

    Function GetEqpTypeDetail(sEqpType As String) As ArrayList
        Dim aReturn As New ArrayList
        Dim iCantidad As Integer = 0
        For c = 1 To sEqpType.Length
            If Mid(sEqpType, c, 1).ToUpper = "X" Then
                aReturn.Add(Mid(sEqpType, c + 1, 4))
            End If
            If Mid(sEqpType, c, 1) = "/" Then
                Exit For
            End If
        Next
        Return aReturn
    End Function

    Function GetEqpTypeDetailLcl(sEqpType As String) As ArrayList
        Dim aReturn As New ArrayList
        Dim iCantidad As Integer = 0
        For c = 1 To sEqpType.Length
            If Mid(sEqpType, c, 1).ToUpper = "X" Then
                aReturn.Add({CInt(Mid(sEqpType, c - 1, 1)), Mid(sEqpType, c + 1, 4)})
            End If
            If Mid(sEqpType, c, 1) = "/" Then
                Exit For
            End If
        Next
        Return aReturn
    End Function

    Function GetPositionByValue(dtLines As DataTable, sValue As String) As Integer
        Dim iResult As Integer = 0
        For dr = 0 To dtLines.Rows.Count - 1
            If dtLines.Rows(dr)(0).ToString.Contains(sValue) Then
                iResult = dr
                Exit For
            End If
        Next
        Return iResult
    End Function

    Function GetPositionByValues(dtLines As DataTable, sValue() As String) As Integer
        Dim iResult As Integer = 0
        For dr = 0 To dtLines.Rows.Count - 1
            If dtLines.Rows(dr)(0).ToString.Contains(sValue) Then
                iResult = dr
            End If
        Next
        Return iResult
    End Function

    Function OnlyLetters(sValue As String) As String
        Dim sResult As String = ""
        For c = 0 To sValue.Length - 1
            sResult += IIf(Char.IsLetter(sValue.Chars(c)), sValue.Chars(c), "")
        Next
        Return sResult
    End Function

    Function OnlyNumbers(sValue As String) As String
        Dim sResult As String = ""
        For c = 0 To sValue.Length - 1
            sResult += IIf(Char.IsNumber(sValue.Chars(c)), sValue.Chars(c), "")
        Next
        Return sResult
    End Function

    Function GetPartnerTaxCode(PartnerName As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        'dtQuery = ExecuteAccessQuery("SELECT [Tax Number 1] FROM CustomerList WHERE [Name]='" & Replace(PartnerName, "'", "''") & "'", "").Tables(0)
        If dtCustomerList.Select("[Name]='" & Replace(PartnerName, "'", "''") & "'").Length > 0 Then
            dtQuery = dtCustomerList.Select("[Name]='" & Replace(PartnerName, "'", "''") & "'").CopyToDataTable
            sResult = dtQuery.Rows(0)("Tax Number 1")
        End If
        Return sResult
    End Function

    Function GetPartnerAddress(PartnerName As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        'dtQuery = ExecuteAccessQuery("SELECT [Street], [Street 2] FROM CustomerList WHERE [Name]='" & Replace(PartnerName, "'", "''") & "'", "").Tables(0)
        'If dtQuery.Rows.Count > 0 Then
        If dtCustomerList.Select("[Name]='" & Replace(PartnerName, "'", "''") & "'").Length > 0 Then
            dtQuery = dtCustomerList.Select("[Name]='" & Replace(PartnerName, "'", "''") & "'").CopyToDataTable
            sResult = dtQuery.Rows(0)("Street") & Space(1) & dtQuery.Rows(0)("Street 2")
        End If
        Return sResult
    End Function

    Function GetPartnerCity(PartnerName As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        'dtQuery = ExecuteAccessQuery("SELECT [City] FROM CustomerList WHERE [Name]='" & Replace(PartnerName, "'", "''") & "'", "").Tables(0)
        'If dtQuery.Rows.Count > 0 Then
        If dtCustomerList.Select("[Name]='" & Replace(PartnerName, "'", "''") & "'").Length > 0 Then
            dtQuery = dtCustomerList.Select("[Name]='" & Replace(PartnerName, "'", "''") & "'").CopyToDataTable
            sResult = dtQuery.Rows(0)("City")
        End If
        Return sResult
    End Function

    Function GetPartnerCountry(PartnerName As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        'dtQuery = ExecuteAccessQuery("SELECT [Country Key] FROM CustomerList WHERE [Name]='" & Replace(PartnerName, "'", "''") & "'", "").Tables(0)
        'If dtQuery.Rows.Count > 0 Then
        If dtCustomerList.Select("[Name]='" & Replace(PartnerName, "'", "''") & "'").Length > 0 Then
            dtQuery = dtCustomerList.Select("[Name]='" & Replace(PartnerName, "'", "''") & "'").CopyToDataTable
            sResult = dtQuery.Rows(0)("Country Key")
        End If
        Return sResult
    End Function

    Function SendData(oBooking As BookingDTO) As ArrayList
        Dim aResult As New ArrayList
        Dim request As New ImportarBookingRequest()
        request.Token = Guid.NewGuid.ToString.ToUpper
        request.BookingList.Add(New BookingDTO With {.CallSign = oBooking.CallSign, _
        .ClienteDireccion = oBooking.ClienteDireccion, _
        .ClienteIdentificacion = oBooking.ClienteIdentificacion, _
        .ClienteNombre = oBooking.ClienteNombre, _
        .ClienteTipoIdentificacion = oBooking.ClienteTipoIdentificacion, _
        .CondicionExportacion = oBooking.CondicionExportacion, _
        .CondicionImportacion = oBooking.CondicionImportacion, _
        .DPvoyage = oBooking.DPvoyage, _
        .ContenedorList = oBooking.ContenedorList, _
        .PlazoList = oBooking.PlazoList, _
        .DepositoVacioCodigoLocalidad = oBooking.DepositoVacioCodigoLocalidad, _
        .DepositoVacioDireccion = oBooking.DepositoVacioDireccion, _
        .DepositoVacioIdentificacion = oBooking.DepositoVacioIdentificacion, _
        .DepositoVacioNombre = oBooking.DepositoVacioNombre, _
        .DepositoVacioPais = oBooking.DepositoVacioPais, _
        .DetalleAduanas = oBooking.DetalleAduanas, _
        .EmailContacto = oBooking.EmailContacto, _
        .Estado = oBooking.Estado, _
        .FechaEmision = oBooking.FechaEmision, _
        .FechaEstimadaArribo = oBooking.FechaEstimadaArribo, _
        .FechaEstimadaZarpe = oBooking.FechaEstimadaZarpe, _
        .ForwarderDireccion = oBooking.ForwarderDireccion, _
        .ForwarderIdentificacion = oBooking.ForwarderIdentificacion, _
        .ForwarderNombre = oBooking.ForwarderNombre, _
        .ForwarderTipoIdentificacion = oBooking.ForwarderTipoIdentificacion, _
        .Numero = oBooking.Numero, _
        .IndicadorDG = oBooking.IndicadorDG, _
        .IndicadorOOG = oBooking.IndicadorOOG, _
        .IndicadorSOW = oBooking.IndicadorSOW, _
        .IndicadorTemp = oBooking.IndicadorTemp, _
        .NombreContacto = oBooking.NombreContacto, _
        .NombreNave = oBooking.NombreNave, _
        .NroContrato = oBooking.NroContrato, _
        .NroIMONave = oBooking.NroIMONave, _
        .NroViaje = oBooking.NroViaje, _
        .NumeroBL = oBooking.NumeroBL, _
        .Observaciones = oBooking.Observaciones, _
        .PuertoDestino = oBooking.PuertoDestino, _
        .PuertoOrigen = oBooking.PuertoOrigen, _
        .ReferenciaExterna = oBooking.ReferenciaExterna, _
        .TExportacionCodigoLocalidad = oBooking.TExportacionCodigoLocalidad, _
        .TExportacionDireccion = oBooking.TExportacionDireccion, _
        .TExportacionIdentificacion = oBooking.TExportacionIdentificacion, _
        .TExportacionNombre = oBooking.TExportacionNombre, _
        .TExportacionPais = oBooking.TExportacionPais, _
        .TImportacionCodigoLocalidad = oBooking.TImportacionCodigoLocalidad, _
        .TImportacionDireccion = oBooking.TImportacionDireccion, _
        .TImportacionIdentificacion = oBooking.TImportacionIdentificacion, _
        .TImportacionNombre = oBooking.TImportacionNombre, _
        .TImportacionPais = oBooking.TImportacionPais, _
        .TelefonoContacto = oBooking.TelefonoContacto, _
        .TerminosLegales = oBooking.TerminosLegales, _
        .VersionDocumento = oBooking.VersionDocumento
        })

        Dim bSent, bError As Boolean
        Try
            bSent = True
            bError = False
            Dim bl, CodErr, sMessage As String
            Dim response = Importar(request)
            Dim result = response.Result.Success
            If Not result Then
                Throw New Exception(response.Result.Message)
                bSent = False
            End If
            sMessage = "Envío Satisfactorio (" & "BK: " & oBooking.Numero & ")"
            oLogFileGenerate.TextFileUpdate("GATE OUT", sMessage)
            If response.ErrorList.Count > 0 Then
                bl = response.ErrorList(0).BL
                CodErr = response.ErrorList(0).CodigoError
                'If CodErr <> "" Then
                '    If CodErr.Contains({"21", "22", "23", "24"}) Then
                '        sMessage = "Error: " & CodErr & " - " & dtErrorProcess.Select("CodeError=" & CodErr)(0)("ErrorDescription")
                '    Else
                '        bError = True
                '        sMessage = "Error: " & CodErr & " - " & dtErrorProcess.Select("CodeError=" & CodErr)(0)("ErrorDescription")
                '        Throw New Exception(sMessage)
                '    End If
                'End If
            End If
            aResult.Add(bSent)
            aResult.Add(bError)
            aResult.Add(sMessage)
        Catch ex As Exception
            aResult.Add(bSent)
            aResult.Add(bError)
            aResult.Add(ex.Message)
            'oLogProcessUpdate.SetDescriptionLogProcess(iLogProcess, iLogProcessItem, ex.Message)
            oLogFileGenerate.TextFileUpdate("GATE OUT", "(" & "BK:" & oBooking.Numero & ")" & " El servicio web retornó el siguiente mensaje: " & ex.Message)
        End Try
        Return aResult
    End Function

    Public Function Importar(request As ImportarBookingRequest) As ImportarBookingResponse
        'Dim url As String = "http://10.72.20.29:3036/GateOutServicio.svc/RegistrarBooking"
        Dim url As String = "http://104.45.136.32:3036/GateOutServicio.svc/RegistrarBooking"
        Dim restDialer As New RestDialer
        Dim response As ImportarBookingResponse = restDialer.PostJSON(Of ImportarBookingResponse, ImportarBookingRequest)(request, url, "")
        If response Is Nothing Then
            Throw New Exception("Formato no valido en respuesta de url: " & url)
            SendErrorMessage(oMailItems, "GATE OUT", "Formato no valido en respuesta de url: " & url, Nothing)
        End If
        Return response
    End Function

    Public Class ImportarBookingRequest
        Inherits BaseRequest
        Public Sub New()
            Me.BookingList = New List(Of BookingDTO)
            'Me.ContenedorList = New List(Of ContenedorDTO)
            'Booking.ContenedorList = New List(Of ContenedorDTO)(ContenedorList)
            'BookingList.Add(Booking)
        End Sub
        Public Property Token() As String
        Public Property BookingList() As List(Of BookingDTO)
        'Public Property ContenedorList() As List(Of ContenedorDTO)
        'Public Property Booking As New BookingDTO
    End Class

    Public Class BookingDTO
        Public Property CallSign() As String
        Public Property ClienteDireccion() As String
        Public Property ClienteIdentificacion() As String
        Public Property ClienteNombre() As String
        Public Property ClienteTipoIdentificacion() As String
        Public Property CondicionExportacion() As String
        Public Property CondicionImportacion() As String
        Public Property DPvoyage() As String
        Public Property DepositoVacioCodigoLocalidad() As String
        Public Property DepositoVacioDireccion() As String
        Public Property DepositoVacioIdentificacion() As String
        Public Property DepositoVacioNombre() As String
        Public Property DepositoVacioPais() As String
        Public Property DetalleAduanas() As String
        Public Property EmailContacto() As String
        Public Property Estado() As String
        Public Property FechaEmision() As Date
        Public Property FechaEstimadaArribo() As Date
        Public Property FechaEstimadaZarpe() As Date
        Public Property ForwarderDireccion() As String
        Public Property ForwarderIdentificacion() As String
        Public Property ForwarderNombre() As String
        Public Property ForwarderTipoIdentificacion() As String
        Public Property Numero() As String
        Public Property IndicadorDG() As String
        Public Property IndicadorOOG() As String
        Public Property IndicadorSOW() As String
        Public Property IndicadorTemp() As String
        Public Property NombreContacto() As String
        Public Property NombreNave() As String
        Public Property NroContrato() As String
        Public Property NroIMONave() As String
        Public Property NroViaje() As String
        Public Property NumeroBL() As String
        Public Property Observaciones() As String
        Public Property PuertoDestino() As String
        Public Property PuertoOrigen() As String
        Public Property ReferenciaExterna() As String
        Public Property TExportacionCodigoLocalidad() As String
        Public Property TExportacionDireccion() As String
        Public Property TExportacionIdentificacion() As String
        Public Property TExportacionNombre() As String
        Public Property TExportacionPais() As String
        Public Property TImportacionCodigoLocalidad() As String
        Public Property TImportacionDireccion() As String
        Public Property TImportacionIdentificacion() As String
        Public Property TImportacionNombre() As String
        Public Property TImportacionPais() As String
        Public Property TelefonoContacto() As String
        Public Property TerminosLegales() As String
        Public Property VersionDocumento() As String
        Public Property ContenedorList As New List(Of ContenedorDTO)
        Public Property PlazoList As New List(Of PlazoDTO)
    End Class

    Public Class ContenedorDTO
        Public Property Detalle() As String
        Public Property Item() As String
        Public Property TipoContenedor() As String
        Public Property NroContenedor() As String
        Public Property IndicadorShipperOwn() As String
        Public Property FechaHoraRetiro() As Nullable(Of DateTime)
        Public Property DepositoRetiro() As String
        Public Property Mercancia() As String
        Public Property InfoAdicional() As String
    End Class

    Public Class PlazoDTO
        Public Property Tipo As String
        Public Property Localidad() As String
        Public Property FechaHora() As DateTime
        Public Property Accion() As String
    End Class

    Public Class BaseRequest
        Public Sub New()
            Me.Meta = New MetaRequest()
        End Sub

        Public Property Meta() As MetaRequest
    End Class

    Public Class MetaRequest
        Public Property Usuario() As String

        Public Property CurrentPage() As Integer
        Public Property Size() As Integer
    End Class

    Public Class ImportarBookingResponse
        Inherits BaseResponse
        Public Sub New()
            Me.ErrorList = New List(Of DeudaError)()
        End Sub

        Public Property ErrorList() As List(Of DeudaError)
    End Class

    Public Class DeudaError
        Public Property BL() As String
        Public Property Comprobante() As String
        Public Property CodigoCobro() As String
        Public Property CodigoError() As String
        Public Property [Error]() As String
    End Class

    Public Class BaseResponse
        Public Sub New()
            Me.Result = New Result()
            Me.Meta = New MetaResponse()
        End Sub

        Public Property Result() As Result
        Public Property Meta() As MetaResponse
    End Class

    Public Class Result
        Public Sub New()
            Me.Success = False
            Me.ErrCode = ""
            Me.Message = ""
            Me.Messages = New List(Of Result)()
        End Sub

        Public Property Success() As Boolean
        Public Property ErrCode() As String
        Public Property Message() As String
        Public Property IdError() As Guid
        Public Property Messages() As List(Of Result)
    End Class

    Public Class MetaResponse
        Public Property Total() As Integer
    End Class

    Function GetPortByLocation(dtLines As DataTable) As ArrayList
        Dim aResult As New ArrayList
        Dim sLine As String = ""
        Dim iLinePosition As Integer = 0
        Dim iTextPosition As Integer = 0
        Dim iStartLine As Integer = GetPositionByValues(dtLines, {"Desde Hacia Por"})
        If iStartLine = 0 Then
            iStartLine = GetPositionByValues(dtLines, {"From To By"})
        End If
        Dim sValue As String = "(PE"
        For l = iStartLine To dtLines.Rows.Count - 1
            sLine = dtLines.Rows(l).ItemArray(0)
            iTextPosition = InStrRev(sLine, sValue)
            If iTextPosition > 0 And aResult.Count = 0 Then
                iLinePosition = l
                aResult.Add(Replace(Replace(Mid(sLine, iTextPosition, 7), "(", ""), ")", ""))
                sLine = Mid(sLine, iTextPosition + 7, sLine.Length).TrimEnd
                iTextPosition = InStrRev(sLine, "(")
                If iTextPosition > 0 And Mid(sLine, iTextPosition + 6, 1) = ")" Then
                    aResult.Add(Replace(Replace(Mid(sLine, iTextPosition, 7), "(", ""), ")", ""))
                End If
                Continue For
            End If
            If aResult.Count = 2 Then
                Exit For
            End If
            If aResult.Count > 0 And Not sLine.Contains(sValue) Then
                iTextPosition = InStrRev(sLine, "(")
                If iTextPosition > 0 And Mid(sLine, iTextPosition + 6, 1) = ")" Then
                    aResult.Add(Replace(Replace(Mid(sLine, iTextPosition, 7), "(", ""), ")", ""))
                End If
            End If
        Next
        Return aResult
    End Function

    Function GetRowCellValueByPosition(dtLines As DataTable, sValue As String, iLenght As Integer, iLines As Integer, sDelimiter As String) As String
        Dim sResult As String = ""
        Dim sLine As String = ""
        Dim iPos As Integer = 0
        Try
            For dr = 0 To dtLines.Rows.Count - 1
                sLine = dtLines.Rows(dr).ItemArray(0)
                If Not sLine.Contains(sValue) Then
                    Continue For
                End If
                iPos = IIf(iLines = 0, GetTextPosition(sLine, sValue), 1)
                If iPos > 0 Then
                    If iLines = 0 Then
                        iLenght = IIf(iLenght = 0, sLine.Length, iLenght)
                        sResult = Mid(sLine, iPos, iLenght).Trim
                        Return sResult
                    Else
                        If sDelimiter.Trim.Length > 0 Then
                            For line = dr + iLines To dr + 10
                                If dtLines.Rows(line).ItemArray(0).ToString.Contains(sDelimiter) Then
                                    Exit For
                                End If
                                'iLenght = IIf(iLenght = 0, dtLines.Rows(line).ItemArray(0).Length, iLenght)
                                iLenght = dtLines.Rows(line).ItemArray(0).Length
                                sResult += Mid(dtLines.Rows(line).ItemArray(0), iPos, iLenght).TrimEnd & Space(1)
                            Next
                        Else
                            iLenght = IIf(iLenght = 0, dtLines.Rows(dr + iLines).ItemArray(0).Length, iLenght)
                            sResult += Mid(dtLines.Rows(dr + iLines).ItemArray(0), iPos, iLenght).TrimEnd & Space(1)
                            Return sResult
                        End If
                    End If
                End If
            Next
        Catch ex As Exception

        End Try

        Return sResult
    End Function

    Function GetTextPosition(sTxtSource As String, sTxtFind As String) As Integer
        Dim iResult As Integer = 0
        Dim sTxtTarget As String = ""
        Try
            iResult = InStrRev(sTxtSource, sTxtFind) + sTxtFind.Length + 2
        Catch ex As Exception

        End Try
        Return iResult
    End Function

    Function GetValuePosition(sTxtSource As String, sTxtFind As String) As Integer
        Dim iResult As Integer = 0
        Dim sTxtTarget As String = ""
        Try
            iResult = InStrRev(sTxtSource, sTxtFind)
        Catch ex As Exception

        End Try
        Return iResult
    End Function

    Function ValidateValueByPosition(dtLines As DataTable, oValue As Object, iPositions As Integer) As Boolean
        Dim iPos As Integer = 0
        Dim bResult As Boolean = False
        For dr = 0 To dtLines.Rows.Count - 1
            If Not dtLines.Rows(dr).ItemArray(0).Contains(oValue) Then
                Continue For
            End If
            iPos = InStrRev(dtLines.Rows(dr).ItemArray(0), oValue)
            If dtLines.Rows(dr).ItemArray(0).ToString.Contains(oValue) Then
                bResult = True
                Exit For
            End If
        Next
        Return bResult
    End Function

    Function GetValueLinesByPosition(dtLines As DataTable, sValue As String, iLines As Integer, iStartLine As Integer) As String
        Dim sResult As String = ""
        Dim sLine As String = ""
        Try
            For dr = 0 To dtLines.Rows.Count - 1
                sLine = dtLines.Rows(dr).ItemArray(0)
                If Not sLine.Contains(sValue) Then
                    Continue For
                End If
                Dim iPos As Integer = 0
                For l = 0 To iLines
                    iPos = dr + l + iStartLine
                    sLine = dtLines.Rows(iPos).ItemArray(0)
                    sResult += Mid(dtLines.Rows(iPos).ItemArray(0), 1, dtLines.Rows(iPos).ItemArray(0).ToString.Length).TrimEnd & Space(1)
                Next
            Next
        Catch ex As Exception

        End Try
        Return sResult
    End Function

End Class
