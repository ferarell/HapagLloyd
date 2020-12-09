Imports System.Windows.Forms
Imports System.Data
Imports System.Collections

Public Class BookingCancellation
    Dim oLogProcessUpdate As New LogProcessUpdate
    Dim oLogFileGenerate As New LogFileGenerate
    Dim oMailItems As Outlook.MailItem
    Dim ProcessCode As String = "BKC"
    Dim drMailProcess As DataRow
    Dim sVendorContact As String

    Friend Sub StartProcess(Items As Outlook.MailItem)
        oMailItems = Items
        Dim oTxtboxPdf As New RichTextBox
        Dim sFileName = FileIO.FileSystem.GetTempFileName
        Dim sIdioma As String = "ES"

        For a = 1 To oMailItems.Attachments.Count
            If oMailItems.Attachments(a).FileName.ToUpper.Contains("PDF") Then
                sFileName = My.Settings.AttachedFilePath & "\" & Format(Now, "ddMMyyyy HHmmss") & " - " & oMailItems.Attachments(a).FileName
                oMailItems.Attachments(a).SaveAsFile(sFileName)
                If Not IO.File.Exists(sFileName) Then
                    oLogFileGenerate.TextFileUpdate("BOOKING CANCELLATION", "No se descargó el archivo adjunto.")
                    SendNewMessage("PRC_ERROR", oMailItems, "BOOKING CANCELLATION", "No se descargó el archivo adjunto.")
                    Return
                End If
            End If
        Next
        Dim sFindTxt = ""
        Dim iPos As Integer = 0
        Dim oBooking As New BookingDTO
        Dim oContainer As New ContenedorDTO
        Dim oPlazo As New PlazoDTO
        Try
            oTxtboxPdf.Text = GetTextFromPDF(sFileName)
            If oTxtboxPdf.Text.IndexOf("Received from") > 0 Then
                sIdioma = "EN"
            End If
            Dim dtLines As New DataTable
            dtLines.Columns.Add("Linea", GetType(String))
            For r = 0 To oTxtboxPdf.Lines.Count - 1
                dtLines.Rows.Add(oTxtboxPdf.Lines(r))
            Next
            'oBooking.CallSign = GetRowCellValueByPosition(dtLines, "Call Sign", 10, 0, "")
            'If sIdioma = "ES" Then
            '    oBooking.ClienteNombre = GetRowCellValueByPosition(dtLines, "Recibido de", 0, 1, "")
            'Else
            '    oBooking.ClienteNombre = GetRowCellValueByPosition(dtLines, "Received from", 0, 1, "")
            'End If
            'oBooking.ClienteDireccion = GetPartnerAddress(oBooking.ClienteNombre)
            'oBooking.ClienteIdentificacion = GetPartnerTaxCode(oBooking.ClienteNombre)
            'oBooking.ClienteTipoIdentificacion = IIf(oBooking.ClienteIdentificacion.Length > 0, "6", "")
            'If sIdioma = "ES" Then
            '    oBooking.CondicionExportacion = GetRowCellValueByPosition(dtLines, "Exportación", 0, 0, "")
            'Else
            '    oBooking.CondicionExportacion = GetRowCellValueByPosition(dtLines, "Export", 0, 0, "")
            'End If
            'If sIdioma = "ES" Then
            '    oBooking.CondicionImportacion = GetRowCellValueByPosition(dtLines, "Importación", 0, 0, "")
            'Else
            '    oBooking.CondicionImportacion = GetRowCellValueByPosition(dtLines, "Import", 0, 0, "")
            'End If
            If sIdioma = "ES" Then
                oBooking.DepositoVacioNombre = GetRowCellValueByPosition(dtLines, "Retiro contr. vacío desde depósitos", 0, 2, "")
            Else
                oBooking.DepositoVacioNombre = GetRowCellValueByPosition(dtLines, "Export empty pick up depot(s)", 0, 2, "")
            End If
            oBooking.DepositoVacioCodigoLocalidad = GetPartnerCity(oBooking.DepositoVacioNombre)
            oBooking.DepositoVacioDireccion = GetPartnerAddress(oBooking.DepositoVacioNombre)
            oBooking.DepositoVacioIdentificacion = GetPartnerTaxCode(oBooking.DepositoVacioNombre)
            oBooking.DepositoVacioPais = GetPartnerCountry(oBooking.DepositoVacioNombre)
            'If sIdioma = "ES" Then
            '    oBooking.DetalleAduanas = GetRowCellValueByPosition(dtLines, "Detalles de Aduanas", 0, 1, "Observaciones")
            'Else
            '    oBooking.DetalleAduanas = GetRowCellValueByPosition(dtLines, "Customs Details", 0, 1, "Remarks")
            'End If
            'oBooking.EmailContacto = GetRowCellValueByPosition(dtLines, "E-mail", 0, 0, "")
            'If sIdioma = "ES" Then
            '    oBooking.Estado = IIf(GetRowCellValueByPosition(dtLines, "Confirmación de Reserva", 0, 0, "").Contains({"ORIGINAL", "UPDATE"}), 1, 0)
            'Else
            '    oBooking.Estado = IIf(GetRowCellValueByPosition(dtLines, "Booking confirmation", 0, 0, "").Contains({"ORIGINAL", "UPDATE"}), 1, 0)
            'End If
            'oBooking.FechaEmision = CDate(GetRowCellValueByPosition(dtLines, "Date of Issue", 20, 0, ""))
            'oBooking.FechaEstimadaArribo = CDate(GetRowCellValueByPosition(dtLines, "Flag", 0, 1, "") & Space(1) & GetRowCellValueByPosition(dtLines, "Flag", 0, 2, ""))
            'oBooking.FechaEstimadaZarpe = CDate(GetRowCellValueByPosition(dtLines, "Flag", 0, 3, "") & Space(1) & GetRowCellValueByPosition(dtLines, "Flag", 0, 4, ""))
            'If sIdioma = "ES" Then
            '    oBooking.ForwarderNombre = GetRowCellValueByPosition(dtLines, "PORT FORWARDER", 0, 1, "")
            'Else
            '    oBooking.ForwarderNombre = GetRowCellValueByPosition(dtLines, "FREIGHT FORWARDER", 0, 1, "")
            'End If
            'oBooking.ForwarderDireccion = GetPartnerAddress(oBooking.ForwarderNombre)
            'oBooking.ForwarderIdentificacion = GetPartnerTaxCode(oBooking.ForwarderNombre)
            'oBooking.ForwarderTipoIdentificacion = IIf(oBooking.ForwarderIdentificacion.Length > 0, "6", "")
            'If sIdioma = "ES" Then
            '    oBooking.Numero = GetRowCellValueByPosition(dtLines, "Nuestra Referencia", 8, 0, " ")
            'Else
            '    oBooking.Numero = GetRowCellValueByPosition(dtLines, "Our Reference", 8, 0, " ")
            'End If
            'oBooking.IndicadorDG = ValidateValueByPosition(dtLines, "✖ DG", 0)
            'oBooking.IndicadorOOG = ValidateValueByPosition(dtLines, "✖ OOG", 0)
            'oBooking.IndicadorSOW = ValidateValueByPosition(dtLines, "✖ SOW", 0)
            'oBooking.IndicadorTemp = ValidateValueByPosition(dtLines, "✖ Temp", 0)
            'If sIdioma = "ES" Then
            '    oBooking.NombreContacto = GetRowCellValueByPosition(dtLines, "Nombre", 0, 0, "")
            'Else
            '    oBooking.NombreContacto = GetRowCellValueByPosition(dtLines, "Name", 0, 0, "")
            'End If
            'oBooking.NombreNave = GetRowCellValueByPosition(dtLines, "Vessel", 0, 1, "")
            'oBooking.NroContrato = GetRowCellValueByPosition(dtLines, "No. de Contrato", 10, 0, "")
            'oBooking.NroIMONave = GetRowCellValueByPosition(dtLines, "IMO No", 8, 0, "")
            'oBooking.NroViaje = GetRowCellValueByPosition(dtLines, "Voy. No", 8, 0, "")
            'If sIdioma = "ES" Then
            '    oBooking.NumeroBL = GetRowCellValueByPosition(dtLines, "No. de BL/SWB", 17, 0, "")
            'Else
            '    oBooking.NumeroBL = GetRowCellValueByPosition(dtLines, "BL/SWB No(s).", 17, 0, "")
            'End If
            'If sIdioma = "ES" Then
            '    oBooking.Observaciones = GetRowCellValueByPosition(dtLines, "Observaciones", 0, 1, "Términos Legales")
            'Else
            '    oBooking.Observaciones = GetRowCellValueByPosition(dtLines, "Remarks", 0, 1, "Legal Terms")
            'End If
            'If sIdioma = "ES" Then
            '    oBooking.PuertoDestino = OnlyLetters(GetRowCellValueByPosition(dtLines, "Desde Hacia Por", 0, 7, ""))
            'Else
            '    oBooking.PuertoDestino = OnlyLetters(GetRowCellValueByPosition(dtLines, "From To By", 0, 7, ""))
            'End If
            'If sIdioma = "ES" Then
            '    oBooking.PuertoOrigen = OnlyLetters(GetRowCellValueByPosition(dtLines, "Desde Hacia Por", 0, 10, ""))
            'Else
            '    oBooking.PuertoOrigen = OnlyLetters(GetRowCellValueByPosition(dtLines, "From To By", 0, 10, ""))
            'End If
            'If sIdioma = "ES" Then
            '    oBooking.ReferenciaExterna = GetRowCellValueByPosition(dtLines, "Su Referencia:", 0, 0, "")
            'Else
            '    oBooking.ReferenciaExterna = GetRowCellValueByPosition(dtLines, "Your Reference:", 0, 0, "")
            'End If
            'If sIdioma = "ES" Then
            '    oBooking.TelefonoContacto = GetRowCellValueByPosition(dtLines, "Teléfono", 0, 0, "")
            'Else
            '    oBooking.TelefonoContacto = GetRowCellValueByPosition(dtLines, "Tel.", 0, 0, "")
            'End If
            'If sIdioma = "ES" Then
            '    oBooking.TerminosLegales = GetValueLinesByPosition(dtLines, "Términos Legales", 8, 2)
            'Else
            '    oBooking.TerminosLegales = GetValueLinesByPosition(dtLines, "Legal Terms", 8, 2)
            'End If
            If sIdioma = "ES" Then
                oBooking.TExportacionNombre = GetRowCellValueByPosition(dtLines, "Dirección Terminal de Exportaciones", 0, 1, "")
            Else
                oBooking.TExportacionNombre = GetRowCellValueByPosition(dtLines, "Export terminal delivery address", 0, 1, "")
            End If
            oBooking.TExportacionCodigoLocalidad = GetPartnerCity(oBooking.TExportacionNombre)
            oBooking.TExportacionDireccion = GetPartnerAddress(oBooking.TExportacionNombre)
            oBooking.TExportacionIdentificacion = GetPartnerTaxCode(oBooking.TExportacionNombre)
            oBooking.TExportacionPais = GetPartnerCountry(oBooking.TExportacionNombre)
            If sIdioma = "ES" Then
                oBooking.TImportacionNombre = IIf(GetRowCellValueByPosition(dtLines, "Dirección Terminal de Importaciones", 0, 1, "") <> "BOBC0201-059TB", GetRowCellValueByPosition(dtLines, "Dirección Terminal de Importaciones", 0, 10, ""), GetRowCellValueByPosition(dtLines, "Dirección Terminal de Importaciones", 0, 1, ""))
            Else
                oBooking.TImportacionNombre = IIf(GetRowCellValueByPosition(dtLines, "Import terminal delivery addresss", 0, 1, "") <> "BOBC0201-059TB", GetRowCellValueByPosition(dtLines, "Import terminal delivery addresss", 0, 10, ""), GetRowCellValueByPosition(dtLines, "Import terminal delivery addresss", 0, 1, ""))
            End If
            If oBooking.TImportacionNombre <> "" Then
                oBooking.TImportacionCodigoLocalidad = GetPartnerCity(oBooking.TImportacionCodigoLocalidad)
                oBooking.TImportacionDireccion = GetPartnerAddress(oBooking.TImportacionCodigoLocalidad)
                oBooking.TImportacionIdentificacion = GetPartnerTaxCode(oBooking.TImportacionCodigoLocalidad)
                oBooking.TImportacionPais = GetPartnerCountry(oBooking.TImportacionCodigoLocalidad)
            End If
            'oBooking.VersionDocumento = 0
            'If Not GetRowCellValueByPosition(dtLines, "Confirmación de Reserva", 0, 0, "").Contains("ORIGINAL") Then
            '    If sIdioma = "ES" Then
            '        sFindTxt = "Confirmación de Reserva"
            '    Else
            '        sFindTxt = "Booking Confirmation"
            '    End If
            '    oBooking.VersionDocumento = OnlyNumbers(Mid(GetRowCellValueByPosition(dtLines, sFindTxt, 0, 0, ""), InStrRev(GetRowCellValueByPosition(dtLines, sFindTxt, 0, 0, ""), "-"), 6))
            'End If

            ''Lista Contenedor
            'Dim iContenedorIni, iContenedorFin As Integer
            'Dim iCantidad As Integer = 0
            'sFindTxt = IIf(sIdioma = "ES", "Resumen", "Summary")
            'Dim sEqpType As String = GetRowCellValueByPosition(dtLines, sFindTxt, 0, 0, "")
            'iCantidad = Mid(sEqpType, 1, InStr(sEqpType, "x") - 1)
            'sEqpType = Mid(sEqpType, InStr(sEqpType, "x") + 1, sEqpType.Length)
            'Dim sTxtIni As String = IIf(sIdioma = "ES", "Info adicional", "Add. Info")
            'Dim sTxtFin As String = IIf(sIdioma = "ES", "Detalles de Aduanas", "Customs Details")

            'iContenedorIni = GetPositionByValue(dtLines, sTxtIni) + 1
            'iContenedorFin = GetPositionByValue(dtLines, sTxtFin) - 1

            'Dim iCol As Integer = 0
            'For dr = iContenedorIni To iContenedorFin
            '    iPos = InStr(dtLines.Rows(dr)(0), sEqpType)
            '    If iPos > 0 Then
            '        oContainer.Item = Mid(dtLines.Rows(dr)(0), 1, iPos - 1)
            '        oContainer.TipoContenedor = sEqpType
            '        If Not Mid(dtLines.Rows(dr)(0).ToString, 8, 2).Contains({"N ", "Y "}) Then
            '            oContainer.NroContenedor = Mid(dtLines.Rows(dr)(0).ToString, iPos + 5, 12).Trim
            '            oContainer.IndicadorShipperOwn = Mid(dtLines.Rows(dr)(0).ToString, iPos + 18, 2).Trim
            '            oContainer.FechaHoraRetiro = CDate(Mid(dtLines.Rows(dr)(0).ToString, iPos + 20, 12).Trim)
            '        Else
            '            oContainer.NroContenedor = ""
            '            oContainer.IndicadorShipperOwn = Mid(dtLines.Rows(dr)(0).ToString, iPos + 5, 2).Trim
            '            oContainer.FechaHoraRetiro = CDate(Mid(dtLines.Rows(dr)(0).ToString, iPos + 18, 12).Trim)
            '        End If
            '        dr += 1
            '        If Not dtLines.Rows(dr)(0).ToString.Contains(sEqpType) Then
            '            For i = 1 To 30
            '                If dtLines.Rows(dr + 1)(0).ToString.Contains({"Commodity", "Mercancía"}) Then
            '                    dr += 2
            '                    For c = 1 To 30
            '                        oContainer.Mercancia += dtLines.Rows(dr)(0).ToString + Space(1)
            '                        If dtLines.Rows(dr + 1)(0).ToString.Contains({"DG Details"}) Then
            '                            Exit For
            '                        End If
            '                        dr += 1
            '                    Next
            '                End If
            '                If dtLines.Rows(dr + 1)(0).ToString.Contains({"DG Details"}) Then
            '                    dr += 2
            '                    For c = 1 To 30
            '                        oContainer.Detalle += dtLines.Rows(dr)(0).ToString + Space(1)
            '                        If dtLines.Rows(dr + 1)(0).ToString.Contains(sEqpType) Then
            '                            Exit For
            '                        End If
            '                        dr += 1
            '                    Next
            '                End If
            '                If dtLines.Rows(dr + 1)(0).ToString.Contains(sEqpType) Then
            '                    Exit For
            '                End If
            '            Next i
            '        End If

            '        oBooking.ContenedorList.Add(New ContenedorDTO With {.DepositoRetiro = oContainer.DepositoRetiro, _
            '                                                            .Detalle = oContainer.Detalle, _
            '                                                            .FechaHoraRetiro = oContainer.FechaHoraRetiro, _
            '                                                            .IndicadorShipperOwn = oContainer.IndicadorShipperOwn, _
            '                                                            .InfoAdicional = oContainer.InfoAdicional, _
            '                                                            .Item = oContainer.Item, _
            '                                                            .Mercancia = oContainer.Mercancia, _
            '                                                            .NroContenedor = oContainer.NroContenedor, _
            '                                                            .TipoContenedor = oContainer.TipoContenedor})

            '        oContainer.Mercancia = ""
            '        oContainer.Detalle = ""
            '    End If

            'Next dr

            ''Lista Plazo
            'Dim iPlazoIni, iPLazoFin As Integer
            'Dim sDeadline() As String = {"Shipping instruction closing", "VGM cut-off", "booking closing", "container delivery date", "delivery cut-off"}
            'iPlazoIni = GetPositionByValue(dtLines, "Plazo límite") + 1
            'If sIdioma = "EN" Then
            '    iPlazoIni = GetPositionByValue(dtLines, "Deadline") + 1
            'End If
            'iPLazoFin = GetPositionByValue(dtLines, "Please send your shipping instruction to") - 1
            'For dr = iPlazoIni To iPLazoFin
            '    'Deadline (tipo)
            '    If dtLines.Rows(dr)(0).ToString.Contains(sDeadline) Then
            '        For i = 1 To 3
            '            oPlazo.Tipo += dtLines.Rows(dr)(0) + Space(1)
            '            dr += 1
            '            If dtLines.Rows(dr + 1)(0).ToString.Contains({"(", ")"}) Then
            '                dr += 1
            '                Exit For
            '            End If
            '        Next i
            '    End If
            '    'Location (localidad)
            '    oPlazo.Localidad = OnlyLetters(dtLines.Rows(dr)(0))
            '    'Date/Time (FechaHora)
            '    dr += 1
            '    While Not IsDate(dtLines.Rows(dr)(0))
            '        dr += 1
            '        If dr >= iPLazoFin Then
            '            Exit While
            '        End If
            '    End While
            '    oPlazo.FechaHora = CDate(dtLines.Rows(dr)(0) & Space(1) & dtLines.Rows(dr + 1)(0))
            '    dr += 2
            '    'Required Action (Accion)
            '    If Not dtLines.Rows(dr)(0).ToString.Contains(sDeadline) Then
            '        For i = 1 To 3
            '            If dr > iPLazoFin Then
            '                Exit For
            '            End If
            '            oPlazo.Accion += dtLines.Rows(dr)(0) + Space(1)
            '            dr += 1
            '            If dtLines.Rows(dr)(0).ToString.Contains(sDeadline) Then
            '                dr -= 1
            '                Exit For
            '            End If
            '        Next i
            '    End If

            '    oBooking.PlazoList.Add(New PlazoDTO With {.Accion = oPlazo.Accion, _
            '                          .FechaHora = oPlazo.FechaHora, _
            '                          .Localidad = oPlazo.Localidad, _
            '                          .Tipo = oPlazo.Tipo})

            '    oPlazo.Accion = ""
            '    oPlazo.Tipo = ""
            'Next dr


        Catch ex As Exception

        End Try

        sFileName = Replace(sFileName.ToUpper, "PDF", "TXT")
        oTxtboxPdf.Refresh()
        If oTxtboxPdf.TextLength > 0 Then
            oTxtboxPdf.SaveFile(sFileName, RichTextBoxStreamType.PlainText)
        End If

        'Reenviar mensaje a Depot
        If Not oBooking.DepositoVacioIdentificacion Is Nothing Then
            ForwardMessage(oBooking.DepositoVacioIdentificacion, oBooking.Numero)
        End If

    End Sub

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

    Function GetPartnerTaxCode(PartnerName As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("SELECT [Tax Number 1] FROM CustomerList WHERE [Name 1]='" & PartnerName & "'", "").Tables(0)
        If dtQuery.Rows.Count > 0 Then
            sResult = dtQuery.Rows(0)(0)
        End If
        Return sResult
    End Function

    Function GetPartnerAddress(PartnerName As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("SELECT [Street], [Street 2] FROM CustomerList WHERE [Name 1]='" & PartnerName & "'", "").Tables(0)
        If dtQuery.Rows.Count > 0 Then
            sResult = dtQuery.Rows(0)(0) & Space(1) & dtQuery.Rows(0)(1)
        End If
        Return sResult
    End Function

    Function GetPartnerCity(PartnerName As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("SELECT [City] FROM CustomerList WHERE [Name 1]='" & PartnerName & "'", "").Tables(0)
        If dtQuery.Rows.Count > 0 Then
            sResult = dtQuery.Rows(0)(0)
        End If
        Return sResult
    End Function

    Function GetPartnerCountry(PartnerName As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("SELECT [Country Key] FROM CustomerList WHERE [Name 1]='" & PartnerName & "'", "").Tables(0)
        If dtQuery.Rows.Count > 0 Then
            sResult = dtQuery.Rows(0)(0)
        End If
        Return sResult
    End Function

    Public Class BookingDTO
        Public Property CallSign() As String
        Public Property ClienteDireccion() As String
        Public Property ClienteIdentificacion() As String
        Public Property ClienteNombre() As String
        Public Property ClienteTipoIdentificacion() As String
        Public Property CondicionExportacion() As String
        Public Property CondicionImportacion() As String
        Public Property DPVoyage() As String
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
        Public Property FechaHoraRetiro() As DateTime
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

    Private Sub ForwardMessage(sVendorTaxCode As String, sBookNo As String)
        'Reenviar Correo al Depot
        sVendorContact = GetVendorContactByTaxCode(ProcessCode, sVendorTaxCode)
        Dim oInspector As Outlook.Inspector
        Dim oForwardMail As Outlook.MailItem = oMailItems
        oInspector = oMailItems.GetInspector
        oForwardMail = oInspector.CurrentItem.Forward
        oForwardMail.Subject = "CANCELACIÓN DE BOOKING - " & sBookNo
        oForwardMail.HTMLBody = drConfig("Mensaje1")
        oForwardMail.HTMLBody += "<p>Favor notar que el siguiente&nbsp;<strong>Booking " & sBookNo & "</strong>&nbsp;se encuentra&nbsp;<span style=""color: #ff0000;""><strong>CANCELADO</strong></span>&nbsp;y por tal motivo agradecemos NO entregar contenedor(es), si tiene alguna observaci&oacute;n agradeceremos enviarnos su consulta al siguiente correo:</p>"
        oForwardMail.HTMLBody += "<br>Peru EQD Ops - AMPERUEQD@hlag.com <br><br>"
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

    Private Sub SendErrorMessageResponse(dtResponse As DataTable)
        Dim oMessage As New SendMessage
    End Sub

End Class
