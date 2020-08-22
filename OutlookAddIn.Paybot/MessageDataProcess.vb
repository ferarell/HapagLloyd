Imports System.Windows.Forms
Imports System.Collections
Imports System.Data
Imports System.IO

Public Class MessageDataProcess
    Dim sFileName As String
    Dim oItem As Object
    Dim sRUC As String = "20492185087" 'ExecuteAccessQuery("select CompanyTaxCode from Company where CompanyCode='0098'", "DBFinance.accdb").Tables(0)(0)(0)

    Friend Sub StartProcess(FileName As String, item As Object, ProcessIndex As Integer)
        sFileName = FileName
        oItem = item
        If ProcessIndex = 1 Then
            'CallByName(Nothing, "DoProcess" & ProcessIndex.ToString, CallType.Method)
            DataProcess1()
        ElseIf ProcessIndex = 2 Then
            DataProcess2()
        End If
    End Sub

    Private Sub DataProcess1()
        Dim aResult As New ArrayList
        Dim dsPaybot As New dsPaybot
        Dim dtInvoice As New DataTable
        Dim bLocalInvoice As Boolean = False
        Dim bSent As Boolean = True
        Dim bReply As Boolean = True
        Dim oTxtboxPdf, oTxtboxBody As New RichTextBox
        Dim sWSResponse As String = ""
        Dim sLocalInvoice As String = ""
        Dim FileXml As String = ""
        Dim mailItem As Outlook.MailItem = oItem
        Dim MailObject As New ArrayList
        Try
            oTxtboxPdf.Text = GetTextFromPDF(sFileName)
            oTxtboxBody.Text = mailItem.Body
            'mailItem.To = My.Settings.TOMailAddress
            MailObject.Add(My.Settings.TOMailAddress)
            dtInvoice = dsPaybot.Tables(1)
            dtInvoice.Rows.Add()
            dtInvoice.Rows(0)("C11") = ""
            For r = 0 To oTxtboxBody.Lines.Count - 1
                If oTxtboxBody.Lines(r).Contains("SAP_COMPANY") Then
                    dtInvoice.Rows(0)("C1") = "00" & Mid(oTxtboxBody.Lines(r), InStr(oTxtboxBody.Lines(r), ":") + 2, 2).Trim 'Company Code
                End If
                If oTxtboxBody.Lines(r).Contains("DEBTOR_ACC_NO") Then
                    dtInvoice.Rows(0)("C2") = Mid(oTxtboxBody.Lines(r), InStr(oTxtboxBody.Lines(r), ":") + 1, 10).Trim 'Customer
                End If
                If oTxtboxBody.Lines(r).Contains("INVOICE_NO") Then
                    dtInvoice.Rows(0)("C4") = Mid(oTxtboxBody.Lines(r), InStr(oTxtboxBody.Lines(r), ":") + 1, 10).Trim 'Document Number
                End If
                If oTxtboxBody.Lines(r).Contains("SAP_DOC_TYPE") Then
                    dtInvoice.Rows(0)("C5") = Mid(oTxtboxBody.Lines(r), InStr(oTxtboxBody.Lines(r), ":") + 1, 5).Trim 'Document type
                End If
                If oTxtboxBody.Lines(r).Contains("INVOICE_DATE") Then
                    dtInvoice.Rows(0)("C6") = Mid(oTxtboxBody.Lines(r), InStr(oTxtboxBody.Lines(r), ":") + 1, 15).Trim 'Document Date
                End If
                If oTxtboxBody.Lines(r).Contains("INVOICE_CURRENCY") Then
                    dtInvoice.Rows(0)("C7") = Mid(oTxtboxBody.Lines(r), InStr(oTxtboxBody.Lines(r), ":") + 1, 5).Trim 'For.currency
                End If
                If oTxtboxBody.Lines(r).Contains("INV_AMOUNT") Then
                    dtInvoice.Rows(0)("C8") = Mid(oTxtboxBody.Lines(r), InStr(oTxtboxBody.Lines(r), ":") + 1, 20).Trim 'Amount in foreign cur.
                End If
                If oTxtboxBody.Lines(r).Contains("PRE_GOV_INV_NO") Then
                    sLocalInvoice = Strings.Right("00" & Mid(oTxtboxBody.Lines(r), InStr(oTxtboxBody.Lines(r), ":") + 1, 20).Trim, 2) 'Reference (serie for local invoice)
                End If
                If oTxtboxBody.Lines(r).Contains("GOVRNMNT_INV_NO") Then
                    If Mid(oTxtboxBody.Lines(r), InStr(oTxtboxBody.Lines(r), ":") + 1, 20).Trim <> "0" Then
                        bLocalInvoice = True
                        sLocalInvoice += Strings.Right("00000000" & Mid(oTxtboxBody.Lines(r), InStr(oTxtboxBody.Lines(r), ":") + 1, 20).Trim, 8) 'Reference (number for local invoice)
                    Else
                        dtInvoice.Rows(0)("C11") += Mid(oTxtboxBody.Lines(r), InStr(oTxtboxBody.Lines(r), ":") + 1, 20).Trim 'Reference
                    End If
                End If
            Next
            If Not bLocalInvoice Then
                For l = 0 To oTxtboxPdf.Lines.Length - 1
                    If oTxtboxPdf.Lines(l).Contains("HAPAG") Then
                        dtInvoice.Rows(0)("C3") = Mid(oTxtboxPdf.Lines(l), 1, InStr(oTxtboxPdf.Lines(l), "HAPAG") - 1).Trim 'Name 1
                    End If
                    If oTxtboxPdf.Lines(l).Contains("HLCU") Then
                        dtInvoice.Rows(0)("C9") = Mid(oTxtboxPdf.Lines(l), InStr(oTxtboxPdf.Lines(l), "HLCU"), 30).Trim 'B/L
                        dtInvoice.Rows(0)("C11") = dtInvoice.Rows(0)("C9").ToString.Replace("HLCU", "")
                    End If
                    If oTxtboxPdf.Lines(l).Contains("SHIPMENT") Then
                        dtInvoice.Rows(0)("C10") = Mid(oTxtboxPdf.Lines(l), 10, 12).Trim 'FIS Shipment Number
                    End If
                Next
            Else
                dtInvoice.Rows(0)("C3") = Mid(oTxtboxPdf.Lines(1), 1, 200).Trim 'Name 1
                dtInvoice.Rows(0)("C9") = Mid(oTxtboxPdf.Lines(22), 10, 30).Trim 'B/L
                dtInvoice.Rows(0)("C10") = Mid(oTxtboxPdf.Lines(9), 10, 12).Trim 'FIS Shipment Number
                dtInvoice.Rows(0)("C11") = sLocalInvoice 'Local Invoice
            End If
            dtInvoice.Rows(0)("C12") = bLocalInvoice
            dtInvoice.Rows(0)("C13") = mailItem.SenderEmailAddress
            dtInvoice.Rows(0)("C14") = Now
            Dim iPos = InStr(sFileName, "-")
            FileXml = My.Settings.AttachedFilePath & "\" & Replace(Mid(sFileName.ToUpper, iPos + 1, Len(sFileName) - iPos), "PDF", "xml").Trim
            Try
                If Not bLocalInvoice Then
                    sWSResponse = "" 'Insertar llamada a WS
                End If
                dtInvoice.WriteXml(FileXml)
                dtInvoice.Rows(0)("C16") = sWSResponse
            Catch ex As Exception
                bSent = False
            End Try
            dtInvoice.Rows(0)("C15") = IIf(bLocalInvoice, False, bSent)
            bReply = InsertIntoAccess("Audit", dtInvoice.Rows(0), "", mailItem, FileXml)
            If Not bReply Then
                Throw New Exception("Error: " & sException(1) & "<br><br>" & "Query: " & sException(0))
            End If
        Catch ex As Exception
            bReply = False
            MailObject.Add(My.Settings.CCMailAddress)
            MailObject.Add(My.Settings.BCCMailAddress)
            MailObject.Add(mailItem.Subject & " (PROCESS WITH ERROR)")
            MailObject.Add(ex.Message & "<br><br>" & mailItem.HTMLBody)
            SendExceptionMessage(FileXml, MailObject)
        End Try
        If My.Settings.ReplyAllMails Then
            If bReply Then
                If Not bLocalInvoice Or My.Settings.ReplyAllMails Then
                    If My.Settings.CCMailAddress <> "" Then
                        mailItem.CC = My.Settings.CCMailAddress
                    End If
                    If My.Settings.BCCMailAddress <> "" Then
                        mailItem.BCC = My.Settings.BCCMailAddress
                    End If
                    ReplyMessage(mailItem, FileXml)
                End If
            End If
        End If
    End Sub

    Private Sub DataProcess2()
        Dim aResult As New ArrayList
        Dim dtFile, dtInvoice As New DataTable
        Dim bLocalInvoice As Boolean = False
        Dim bSent As Boolean = True
        Dim bReply As Boolean = True
        Dim oTxtBox, oDocText As New RichTextBox
        Dim sTrama As String = ""
        Dim sWSResponse As String = ""
        Dim sFila As String = ""
        Dim FileXml As String = ""
        Dim mailItem As Outlook.MailItem = oItem
        Dim MailObject As New ArrayList
        Dim mlCell As New List(Of List(Of String))
        Try
            dtFile.Columns.Add("Linea", GetType(String))
            oTxtBox.Text = My.Computer.FileSystem.ReadAllText(sFileName)
            'mailItem.To = My.Settings.TOMailAddress
            MailObject.Add(My.Settings.TOMailAddress)
            'dtInvoice = dsPaybot.Tables(1)
            For r = 0 To oTxtBox.Lines.Count - 1
                sFila = ""
                For p = 1 To oTxtBox.Lines(r).Length
                    If Mid(oTxtBox.Lines(r), p, 1) <> "'" Then
                        sFila += Mid(oTxtBox.Lines(r), p, 1)
                    Else
                        dtFile.Rows.Add(sFila)
                        sFila = ""
                    End If
                Next
            Next
            Dim iPos = InStr(sFileName, "-")
            FileXml = My.Settings.AttachedFilePath & "\" & Replace(Mid(sFileName.ToUpper, iPos + 1, Len(sFileName) - iPos), "TXT", "xml").Trim
            'dtFile.TableName = "INVOICE"
            'dtFile.WriteXml(FileXml)
            Dim stm As StreamWriter = New StreamWriter(Replace(FileXml, "xml", "txt"), False)
            'Genera Archivo Texto
            For dr = 0 To dtFile.Rows.Count - 1
                Dim drLine As DataRow = dtFile.Rows(dr)
                stm.WriteLine(drLine(0))
            Next
            stm.Close()

            'Genera Documento Texto que será enviado por el Servicio Web

            '---------------------------------------------Linea EN
            sTrama = ""
            mlCell.Add(New List(Of String))
            'Position (0)
            mlCell(0).Add("EN")
            'Position (1) - Tipo de Documento
            If GetCellValueByPosition(dtFile, "BGM+", 1, ":") = "780" Then
                If GetCellValueByPosition(dtFile, "FTX+ACB+", 0, "").ToString.Contains("BOLETA") Then
                    mlCell(0).Add("03")
                Else
                    mlCell(0).Add("01")
                End If

            ElseIf GetCellValueByPosition(dtFile, "BGM+", 1, ":").ToString.Contains({"381", "389"}) Then
                mlCell(0).Add("07")
            ElseIf GetCellValueByPosition(dtFile, "BGM+", 1, ":") = "383" Then
                mlCell(0).Add("08")
            End If
            'Position (2) - Serie y Correlativo Documento
            mlCell(0).Add(GetCellValueByPosition(dtFile, "RFF+IV:", 0, ""))
            'Position (3) - Tipo de Nota de crédito/Nota de Débito (Motivo de NC/ND)
            mlCell(0).Add(IIf(mlCell(0)(1).ToString.Contains({"07", "08"}), "01", ""))
            'Position (4) - Factura que referencia la Nota de Crédito/Nota de Débito / Boleta que referencia la Nota de Crédito/Nota de Débito
            mlCell(0).Add(IIf(mlCell(0)(1) = "07", GetCellValueByPosition(dtFile, "RFF+OI:", 0, ""), ""))
            'Position (5) - Sustento
            mlCell(0).Add(IIf(mlCell(0)(1) = "07", Replace(GetRowCellValueByPosition(dtFile, "TCC+609004:132:", 1, 1, ":"), "INV.", "FACT"), ""))
            'Position (6) - Fecha Emision
            mlCell(0).Add(Mid(GetCellValueByPosition(dtFile, "DTM+3:", 1, ":"), 1, 4) & "-" & Mid(GetCellValueByPosition(dtFile, "DTM+3:", 1, ":"), 5, 2) & "-" & Mid(GetCellValueByPosition(dtFile, "DTM+3:", 1, ":"), 7, 2))
            'Position (7) - Tipo de Moneda
            mlCell(0).Add(GetCellValueByPosition(dtFile, "CUX+5:", 1, ":"))
            'Position (8) - RUC Emisor
            mlCell(0).Add(My.Settings.CompanyTaxCode)
            'Position (9) - Tipo de Identificación Emisor
            mlCell(0).Add("6")
            'Position (10) - Nombre Comercial Emisor
            mlCell(0).Add(My.Settings.CompanyName)
            'Position (11) - Apellidos y nombres, denominación o razón social
            mlCell(0).Add(My.Settings.CompanyName)
            'Position (12) - Codigo UBIGEO Emisor
            mlCell(0).Add(My.Settings.CompanyZipCode)
            'Position (13) - Direccion Emisor
            mlCell(0).Add(My.Settings.CompanyAddress)
            'Position (14) - Departamento Emisor (Ciudad)
            mlCell(0).Add(My.Settings.CompanyDepartment)
            'Position (15) - Provincia Emisor 
            mlCell(0).Add(My.Settings.CompanyProvince)
            'Position (16) - Distrito Emisor
            mlCell(0).Add(My.Settings.CompanyDistrict)
            'Position (17) - Número de documento de identidad del adquirente o usuario
            mlCell(0).Add(GetCellValueByPosition(dtFile, "RFF+VA:", 0, ""))
            'Position (18) - Tipo de identidad del adquirente o usuario
            mlCell(0).Add(IIf(mlCell(0)(1).ToString.Contains({"01", "07", "08"}), "6", "0"))
            'Position (19) - Apellidos y nombres, denominación o razón social del adquirente o usuario 
            mlCell(0).Add(GetRowCellValueByPosition(dtFile, "NAD+FP+", 1, 2, ":"))
            mlCell(0)(19) = Mid(mlCell(0)(19), 4, mlCell(0)(19).Length - 4)
            'Position (20) - Direccion Receptor
            mlCell(0).Add(GetRowCellValueByPosition(dtFile, "NAD+FP+", 1, 3, ":") & Space(1) & GetRowCellValueByPosition(dtFile, "NAD+FP+", 1, 4, ":") & Space(1) & GetRowCellValueByPosition(dtFile, "NAD+FP+", 1, 5, ":"))
            'Position (21) - Monto Neto/Total Valor de Venta
            mlCell(0).Add(GetCellValueByPosition(dtFile, "MOA+389:", 1, ":"))
            'Position (22) - Monto Total de Impuestos
            mlCell(0).Add(GetCellValueByPosition(dtFile, "MOA+150:", 1, ":"))
            'Position (23) - Monto Descuentos (Que no afectan la base)
            mlCell(0).Add("")
            'Position (24) - Monto Recargos (Que no afectan la base)
            mlCell(0).Add("")
            'Position (25) - Monto Total/Importe Total
            mlCell(0).Add(GetCellValueByPosition(dtFile, "MOA+388:", 1, ":"))
            'Position (26) - Códigos de otros conceptos tributarios o comerciales recomendados
            mlCell(0).Add("")
            'Position (27) - Total de Valor Venta Neto
            mlCell(0).Add(GetCellValueByPosition(dtFile, "MOA+389:", 1, ":"))
            'Position (28) - Número de documento de identidad del comprador
            mlCell(0).Add("")
            'Position (29) - Tipo de documento de identidad del comprador
            mlCell(0).Add("")
            'Position (30) - Código País Emisor
            mlCell(0).Add("PE")
            'Position (31) - Urbanización Emisor
            mlCell(0).Add("")
            For i = 0 To mlCell(0).Count - 1
                sTrama += mlCell(0)(i) & IIf(i < mlCell(0).Count - 1, "|", "")
            Next
            oDocText.AppendText(sTrama & vbNewLine)

            '-----------------------------------------Linea ENEX
            sTrama = ""
            mlCell.Add(New List(Of String))
            'Position (0)
            mlCell(1).Add("ENEX")
            'Position (1) - Version UBL
            mlCell(1).Add("2.1")
            'Position (2) - Tipo de Operación 
            mlCell(1).Add("0101")
            'Position (3) - Orden de Compra
            mlCell(1).Add("")
            'Position (4) - Redondeo()
            mlCell(1).Add("")
            'Position (5) - Total Anticipos
            mlCell(1).Add("")
            'Position (6) - Fecha de Vencimiento de la Factura / Fecha de Pago
            mlCell(1).Add("")
            'Position (7) - Hora de Emisión
            mlCell(1).Add("")
            'Position (8) - Código asignado por SUNAT para el establecimiento anexo declarado en el RUC
            mlCell(1).Add("0000")
            'Position (9) - Total Precio de Venta
            mlCell(1).Add("")
            'Position (10) - Número de documento de identidad de otros participantes asociados a la transacción 
            mlCell(1).Add("")
            'Position (11) - Tipo de documento de identidad de otros participantes asociados a la transacción 
            mlCell(1).Add("")
            'Position (12) - Apellidos y nombres, denominación o razón social de otros participantes asociados a la transacción 
            mlCell(1).Add("")
            For i = 0 To mlCell(1).Count - 1
                sTrama += mlCell(1)(i) & IIf(i < mlCell(1).Count - 1, "|", "")
            Next
            oDocText.AppendText(sTrama & vbNewLine)

            '----------------------------------------------Linea DN
            sTrama = ""
            mlCell.Add(New List(Of String))
            'Position (0)
            mlCell(2).Add("DN")
            'Position (1) - Número de Línea de Nota
            mlCell(2).Add("1")
            'Position (2) - Código de la leyenda
            mlCell(2).Add("1000")
            'Position (3) - Glosa de la leyenda
            mlCell(2).Add(GetCellValueByPosition(dtFile, "FTX+ACF+3++", 0, ""))
            For i = 0 To mlCell(2).Count - 1
                sTrama += mlCell(2)(i) & IIf(i < mlCell(2).Count - 1, "|", "")
            Next
            oDocText.AppendText(sTrama & vbNewLine)

            '--------------------------------------------------------------- ITEMS
            Dim iItems As Integer = GetTimesFound(dtFile, "TCC+609004:132:6:")
            For itm = 1 To iItems
                '----------------------------------------------- Linea DE
                sTrama = ""
                mlCell.Add(New List(Of String))
                'Position (0)
                mlCell(3).Add("DE")
                'Position (1) - Correlativo de Línea de Detalle (Número de Orden del Item)
                mlCell(3).Add(itm.ToString)
                'Position (2) - Precio de venta unitario por item
                mlCell(3).Add(GetRowCellValueByPosition(dtFile, "PRI+INV:", itm, 0, ""))
                'Position (3) - Unidad de Medida
                mlCell(3).Add(GetUnitMap(GetRowCellValueByPosition(dtFile, "QTY+2:", itm, 2, ":")))
                'Position (4) - Cantidad de unidades vendidas pot item (Q)
                mlCell(3).Add(GetRowCellValueByPosition(dtFile, "QTY+2:", itm, 1, ":"))
                'Position (5) - Valor de venta por item
                mlCell(3).Add(GetRowCellValueByPosition(dtFile, "MOA+14:", itm, 1, ":"))
                'Position (6) - Codigo de Producto
                mlCell(3).Add(Mid(GetRowCellValueByPosition(dtFile, "TCC+609004:132:6:", itm, 5, ":"), 5, 3))
                'Position (7) - Tipo de Precio de Venta (Código de precio)
                mlCell(3).Add("01")
                'Position (8) - Valor de venta unitario por ítem
                mlCell(3).Add(GetRowCellValueByPosition(dtFile, "PRI+INV:", itm, 0, ""))
                'Position (9) - Valor de venta por item
                mlCell(3).Add(GetRowCellValueByPosition(dtFile, "MOA+14:", itm, 1, ":"))
                'Position (10) - Número de lote
                mlCell(3).Add("")
                'Position (11) - Marca
                mlCell(3).Add("")
                'Position (12) - Pais de origen
                mlCell(3).Add("")
                'Position (13) - Nª de Posicion que el Item comprado tiene en la Orden de Compra
                mlCell(3).Add("")
                For i = 0 To mlCell(3).Count - 1
                    sTrama += mlCell(3)(i) & IIf(i < mlCell(3).Count - 1, "|", "")
                Next
                oDocText.AppendText(sTrama & vbNewLine)
                mlCell(3).Clear()

                '------------------------------------------------Linea DEDI
                sTrama = ""
                mlCell.Add(New List(Of String))
                'Position (0)
                mlCell(4).Add("DEDI")
                'Position (1) - Descripcion del Item
                mlCell(4).Add(GetRowCellValueByPosition(dtFile, "TCC+609004:132:6:", itm, 3, ":"))
                If mlCell(0)(1) = "07" Then
                    mlCell(4)(1) = Replace(mlCell(4)(1), "INV.", "FACT")
                End If
                'Position (2) - Nota complementarias a descripción del ítem
                mlCell(4).Add(GetContainerValuesByCode(dtFile, "RFF+AAQ:"))
                'Position (3) - Nombre del Concepto (Información Adicional - Gastos art.37° Renta)
                mlCell(4).Add("")
                'Position (4) - Codigo del Concepto (Información Adicional - Gastos art.37° Renta)
                mlCell(4).Add("")
                'Position (5) - Número de placa del vehículo (Información Adicional - Gastos art.37° Renta)
                mlCell(4).Add("")
                'Position (6) - Codigo producto de SUNAT
                mlCell(4).Add("")
                'Position (7) - Código de producto GS1
                mlCell(4).Add("")
                'Position (8) - Tipo de estructura GTIN
                mlCell(4).Add("")
                For i = 0 To mlCell(4).Count - 1
                    sTrama += mlCell(4)(i) & IIf(i < mlCell(4).Count - 1, "|", "")
                Next
                oDocText.AppendText(sTrama & vbNewLine)
                mlCell(4).Clear()

                '------------------------------------------------Linea DEIM
                sTrama = ""
                mlCell.Add(New List(Of String))
                'Position (0)
                mlCell(5).Add("DEIM")
                'Position (1) -  Monto total de impuestos del ítem  (Monto total de impuestos por linea)
                mlCell(5).Add(GetRowCellValueByPosition(dtFile, "MOA+1:", itm, 1, ":"))
                'Position (2) -  Afectación al IGV por la línea,  Afectación IVAP por la línea (Monto Base)
                mlCell(5).Add(GetRowCellValueByPosition(dtFile, "MOA+10", itm, 1, ":"))
                'Position (3) -  (Monto de IGV/IVAP de la línea)
                mlCell(5).Add(GetRowCellValueByPosition(dtFile, "MOA+1:", itm, 1, ":"))
                'Position (4) -  (Tasa del IGV o  Tasa del IVAP)
                mlCell(5).Add("18")
                'Position (5) -  Tipo de Impuesto
                mlCell(5).Add("1000")
                'Position (6) -  Afectación del IGV  (Afectación al IGV o IVAP cuando corresponda)
                mlCell(5).Add("10")
                'Position (7) -  Sistema de ISC (Tipo de sistema de ISC)
                mlCell(5).Add("")
                'Position (8) -  Identificación del tributo (Código de tributo por línea)
                mlCell(5).Add("1000")
                'Position (9) -  Nombre del Tributo
                mlCell(5).Add("IGV")
                'Position (10) -  Código del Tipo de Tributo  (Código internacional de tributo)
                mlCell(5).Add("VAT")
                For i = 0 To mlCell(5).Count - 1
                    sTrama += mlCell(5)(i) & IIf(i < mlCell(5).Count - 1, "|", "")
                Next
                oDocText.AppendText(sTrama & vbNewLine)
                mlCell(5).Clear()
            Next
            '--------------------------------------------------Linea DI
            sTrama = ""
            mlCell.Add(New List(Of String))
            'Position (0)
            mlCell(6).Add("DI")
            'Position (1) - Monto total del Impuesto
            mlCell(6).Add(GetCellValueByPosition(dtFile, "MOA+150:", 1, ":"))
            'Position (2) - Sumatoria por Tributo (IGV,ISC, Otros)
            mlCell(6).Add(GetCellValueByPosition(dtFile, "MOA+150:", 1, ":"))
            'Position (3) - Identificación del tributo (codigo del tributo)
            mlCell(6).Add("1000")
            'Position (4) - Nombre del Tributo
            mlCell(6).Add("IGV")
            'Position (5) - Código del Tipo de Tributo (Código Internacional del tributo)
            mlCell(6).Add("VAT")
            'Position (6) - Monto Base (Total valor de venta de operaciones gravadas)
            mlCell(6).Add(GetCellValueByPosition(dtFile, "MOA+389:", 1, ":"))
            For i = 0 To mlCell(6).Count - 1
                sTrama += mlCell(6)(i) & IIf(i < mlCell(6).Count - 1, "|", "")
            Next
            oDocText.AppendText(sTrama & vbNewLine)
            '------------------------------------------------Lineas PE
            'sTrama = ""
            Dim sRefNro As String = Mid(GetRowCellValueByPosition(dtFile, "BGM+", 1, 3, ":"), 7, 10)
            'mlCell.Add(New List(Of String))
            'Position (0) - CodCliente
            oDocText.AppendText("PE|CodCliente|" & Mid(GetRowCellValueByPosition(dtFile, "NAD+FP+", 1, 10, ":"), 1, 8) & "|" & vbNewLine)
            'Position (1) - TelEmi
            oDocText.AppendText("PE|TelEmi|511411-6500" & "|" & vbNewLine)
            'Position (2) - FaxEmi
            oDocText.AppendText("PE|FaxEmi|511421-7533" & "|" & vbNewLine)
            'Position (3) - CorreoCliente
            oDocText.AppendText("PE|CorreoCliente|" & GetMailValuesByCode(dtFile, "COM+", ":EM") & "|" & vbNewLine)
            If GetRowCellValueByPosition(dtFile, "RFF+BN", 1, 1, ":") <> "" Then
                'Position (4) - Valor1
                oDocText.AppendText("PE|Valor1|SHIPMENT: " & GetRowCellValueByPosition(dtFile, "RFF+BN", 1, 1, ":") & "|" & vbNewLine)
                'Position (5) - Valor2
                oDocText.AppendText("PE|Valor2|FCL/FCL" & "|" & vbNewLine)
                'Position (6) - Valor3
                oDocText.AppendText("PE|Valor3|" & GetRowCellValueByPosition(dtFile, "TDT+10+", 1, 6, ":") & "|" & vbNewLine)
                'Position (7) - Valor4
                oDocText.AppendText("PE|Valor4|" & GetRowCellValueByPosition(dtFile, "TDT+20+", 1, 6, ":") & "|" & vbNewLine)
                'Position (8) - Valor5
                oDocText.AppendText("PE|Valor5|DE " & GetRowCellValueByPosition(dtFile, "LOC+9+", 1, 3, ":") & "|" & vbNewLine)
                'Position (9) - Valor6
                oDocText.AppendText("PE|Valor6|" & Replace(Mid(GetRowCellValueByPosition(dtFile, "TDT+10+", 1, 0, ""), 1, 7), "+", "") & "/" & Replace(Mid(GetRowCellValueByPosition(dtFile, "TDT+10+", 1, 3, ":"), 1, 8), "+", "") & "|" & vbNewLine)
                'Position (10) - Valor7
                oDocText.AppendText("PE|Valor7|" & Replace(Mid(GetRowCellValueByPosition(dtFile, "TDT+20+", 1, 0, ""), 1, 7), "+", "") & "/" & Replace(Mid(GetRowCellValueByPosition(dtFile, "TDT+20+", 1, 3, ":"), 1, 8), "+", "") & "|" & vbNewLine)
                'Position (11) - Valor8
                oDocText.AppendText("PE|Valor8|A " & GetRowCellValueByPosition(dtFile, "LOC+11+", 1, 3, ":") & "|" & vbNewLine)
                'Position (12) - Valor9
                oDocText.AppendText("PE|Valor9|SALIDA: " & Format(Date.ParseExact(GetRowCellValueByPosition(dtFile, "DTM+133", dtFile.Select("Linea LIKE 'DTM+133:%'").Length, 1, ":"), "yyyyMMdd", Globalization.CultureInfo.InvariantCulture), "MMM dd, yyyy").ToUpper & "|" & vbNewLine)
                'Position (13) - Valor10
                oDocText.AppendText("PE|Valor10|LLEGADA: " & Format(Date.ParseExact(GetRowCellValueByPosition(dtFile, "DTM+132", 1, 1, ":"), "yyyyMMdd", Globalization.CultureInfo.InvariantCulture), "MMM dd, yyyy").ToUpper & "|" & vbNewLine)
            End If
            'Position (14) - RefNro
            oDocText.AppendText("PE|RefNro|" & sRefNro & "|" & vbNewLine)
            'Position (15) - SubTotal
            oDocText.AppendText("PE|SubTotal|" & mlCell(0)(27) & "|" & vbNewLine)
            'Position (16) - ValorIGV
            oDocText.AppendText("PE|ValorIGV|18|" & vbNewLine)
            'Position (17) - NroBL
            oDocText.AppendText("PE|NroBL|" & GetRowCellValueByPosition(dtFile, "RFF+BM", 1, 1, ":") & "|" & vbNewLine)
            'Position (18) - NomConsignee
            oDocText.AppendText("PE|NomConsignee|" & Mid(GetRowCellValueByPosition(dtFile, "NAD+CN+", 1, 2, ":"), 4, 100) & "|" & vbNewLine)
            'Position (19) - DirConsignee
            oDocText.AppendText("PE|DirConsignee|" & GetRowCellValueByPosition(dtFile, "NAD+CN+", 1, 3, ":") & Space(1) & GetRowCellValueByPosition(dtFile, "NAD+CN+", 1, 4, ":") & "|" & vbNewLine)
            'Position (20) - PagWeb
            oDocText.AppendText("PE|PagWeb|http://hapagprd.paperless.com.pe/BoletaHAPAG/" & "|" & vbNewLine)
            'Position (21) - MensajesAt
            oDocText.AppendText("PE|MensajesAt|""LAS ACLARACIONES A LOS CARGOS DEBERAN PRESENTARSE DENTRO DE LOS 14 DIAS SIGUIENTES A LA EMISION DE LA FACTURA, DE LO CONTRARIO NO SERAN ACEPTADAS""" & "|" & vbNewLine)
            'Position (22) - CodInternoHapag
            oDocText.AppendText("PE|CodInternoHapag|" & sRefNro & "|" & vbNewLine)
            'Position (23) - CodEstSUNAT
            oDocText.AppendText("PE|CodEstSUNAT||" & vbNewLine)
            'Position (24) - NombreArchivo
            oDocText.AppendText("PE|NombreArchivo|" & My.Settings.CompanyTaxCode & "_" & mlCell(0)(2).ToString & "_" & Replace(mlCell(0)(6), "-", "") & "_" & GetCellValueByPosition(dtFile, "BGM+", 1, ":").ToString & "_" & sRefNro & ".txt" & "|" & vbNewLine)
            'Position (25) - PPLOrigen
            oDocText.AppendText("PE|PPLOrigen|SHF|")
            '--------------------------------------------------

            'oDocText.SaveFile(My.Settings.LogFilePath & "\" & "PPL-" & mlCell(0)(2) & ".txt", RichTextBoxStreamType.PlainText)
            oDocText.SaveFile(My.Settings.LogFilePath & "\" & My.Settings.CompanyTaxCode & "_" & mlCell(0)(2).ToString & "_" & Replace(mlCell(0)(6), "-", "") & "_" & GetCellValueByPosition(dtFile, "BGM+", 1, ":").ToString & "_" & sRefNro & ".txt", RichTextBoxStreamType.PlainText)
            Dim oAppService As New PaperlessQA.Online
            oAppService.Url = My.Settings.PPL_UrlSoap
            oAppService.InitializeLifetimeService()
            oAppService.Timeout = 40000
            sWSResponse = oAppService.OnlineGeneration(sRUC, My.Settings.PPL_UserSoap, My.Settings.PPL_PasswordSoap, oDocText.Text, My.Settings.PPL_TipoFolioSoap, True, My.Settings.PPL_TipoRetornoSoap, True)
            Dim oResponse As New RichTextBox
            oResponse.AppendText(sWSResponse)
            oResponse.SaveFile(My.Settings.LogFilePath & "\" & "PPL-" & mlCell(0)(2) & ".xml", RichTextBoxStreamType.PlainText)
            'Dim srXmlData As New System.IO.StringReader(sWSResponse)
            'Dim dsXmlData As New DataSet
            'dsXmlData.ReadXml(srXmlData)
            'If dsXmlData.Tables(0).Rows(0)("Codigo") = 0 Then
            '    SendNewMessage("PRC_OK", mailItem, "PAPERL045", dsXmlData.Tables(0).Rows(0)("Mensaje"))
            'End If
            oAppService.Dispose()
        Catch ex As Exception
            bReply = False
            MailObject.Add(My.Settings.CCMailAddress)
            MailObject.Add(My.Settings.BCCMailAddress)
            MailObject.Add(mailItem.Subject & " (PROCESS WITH ERROR)")
            MailObject.Add(ex.Message & "<br><br>" & ex.StackTrace.ToString & "<br><br>" & mailItem.HTMLBody & "<br><br>" & sWSResponse & "<br><br>")
            SendExceptionMessage(sFileName, MailObject)
        End Try
        If My.Settings.ReplyAllMails Then
            If bReply Then
                If Not bLocalInvoice Or My.Settings.ReplyAllMails Then
                    If My.Settings.CCMailAddress <> "" Then
                        mailItem.CC = My.Settings.CCMailAddress
                    End If
                    If My.Settings.BCCMailAddress <> "" Then
                        mailItem.BCC = My.Settings.BCCMailAddress
                    End If
                    ReplyMessage(mailItem, FileXml)
                End If
            End If
        End If
    End Sub

    Function GetTimesFound(dtLines As DataTable, sValue As String) As Integer
        Dim iTimes As Integer = 0
        For r = 0 To dtLines.Rows.Count - 1
            If dtLines.Rows(r)(0).ToString.Contains(sValue) Then
                iTimes += 1
            End If
        Next
        Return iTimes
    End Function

    Function GetMailValuesByCode(dtLines As DataTable, sValue As String, sIdentifier As String) As String
        Dim sResult As String = ""
        For r = 0 To dtLines.Rows.Count - 1
            If dtLines.Rows(r)(0).ToString.Contains(sValue) Then
                If Right(dtLines.Rows(r)(0).ToString, 3) = sIdentifier Then
                    sResult += Mid(dtLines.Rows(r)(0).ToString, 5, Len(dtLines.Rows(r)(0).ToString) - 7) + ";"
                End If
            End If
        Next
        Return sResult
    End Function

    Function GetContainerValuesByCode(dtLines As DataTable, sValue As String) As String
        Dim sResult As String = ""
        For r = 0 To dtLines.Rows.Count - 1
            If dtLines.Rows(r)(0).ToString.Contains(sValue) Then
                sResult += Mid(Replace(dtLines.Rows(r)(0).ToString, " ", ""), Len(sValue) + 1, Len(dtLines.Rows(r)(0).ToString)) + Space(2)
            End If
        Next
        Return sResult
    End Function

    Function GetCellValueByPosition(dtLines As DataTable, sValue As String, iTimes As Integer, sDelimiter As String) As String
        Dim sResult As String = ""
        Dim sLine As String = ""
        Dim iFinded As Integer = 0
        Dim iValPos1, iValPos2 As Integer
        For dr = 0 To dtLines.Rows.Count - 1
            sLine = dtLines.Rows(dr).ItemArray(0)

            If Not sLine.Contains(sValue) Then
                Continue For
            Else
                iValPos1 = sValue.Length + 1
                If iTimes = 0 Then
                    sResult = Mid(sLine, iValPos1, sLine.Length)
                    If sValue.Contains("RFF+VA:") Then
                        Return sResult
                    End If
                Else
                    For i = iValPos1 To sLine.Length
                        If Mid(sLine, i, 1) = sDelimiter Then
                            iFinded += 1
                            iValPos2 = i
                            If iFinded = iTimes Then
                                sResult = Mid(sLine, iValPos1, iValPos2 - iValPos1)
                                Exit For
                            End If
                        End If
                    Next
                End If
            End If
        Next
        Return sResult
    End Function

    Function GetRowCellValueByPosition(dtLines As DataTable, sValue As String, iTimes As Integer, iPosition As Integer, sDelimiter As String) As String
        Dim sResult As String = ""
        Dim sLine As String = ""
        Dim iFinded As Integer = 0
        Dim iValPos1, iValPos2 As Integer
        Dim iRow As Integer = 0
        For iFounds = 1 To iTimes

            For dr = 0 To dtLines.Rows.Count - 1
                sLine = dtLines.Rows(dr).ItemArray(0)
                If sLine.Contains("QTY+2:.000") Then
                    Continue For
                End If
                If sLine.Contains(sValue) Then
                    iRow += 1
                Else
                    Continue For
                End If
                If iRow <> iTimes Then
                    Continue For
                End If
                If iPosition = 0 Then
                    iValPos1 = sValue.Length + 1
                Else
                    iValPos1 = GetTextPosition(sLine, sDelimiter, iPosition)
                End If
                If iValPos1 = 0 Then
                    Return sResult
                End If
                If iPosition = 0 Then
                    sResult = Mid(sLine, iValPos1, sLine.Length)
                    If sValue.Contains("RFF+VA:") Then
                        Return sResult
                    End If
                Else
                    'For i = iValPos1 To sLine.Length
                    'If Mid(sLine, i, 1) = sDelimiter Then
                    iFinded += 1
                    'iValPos2 = i
                    'If iFinded = iPosition Then
                    If InStr(iValPos1, sLine, sDelimiter) = 0 Then
                        iValPos2 = sLine.Length + 1
                    Else
                        iValPos2 = InStr(iValPos1, sLine, sDelimiter)
                    End If
                    sResult = Mid(sLine, iValPos1, iValPos2 - iValPos1)
                    Return sResult
                    'End If
                    'End If
                    'Next
                End If
            Next
        Next
        Return sResult
    End Function

    Function GetTextPosition(sTxtSource As String, sTxtFind As String, iTimes As Integer) As Integer
        Dim iResult As Integer = 0
        Dim iLocation As Integer = 0
        For i = 1 To sTxtSource.Length - 1
            If Mid(sTxtSource, i, 1) <> sTxtFind Then
                Continue For
            End If
            iLocation += 1
            If iLocation = iTimes Then
                If InStr(i, sTxtSource, sTxtFind) > 0 Then
                    iResult = InStr(i, sTxtSource, sTxtFind) + 1
                End If
            End If
        Next
        Return iResult
    End Function

    Function GetUnitMap(sValue As String) As String
        Dim sResult As String = ""
        Dim dtSource As New DataTable
        'dtSource.ReadXmlSchema("UnitOfMeasurement.xsd")
        'dtSource.ReadXml("UnitOfMeasurement.xml")
        dtSource = LoadExcel("UnitOfMeasurement.xlsx", "{0}").Tables(0)
        If dtSource.Rows.Count > 0 Then
            If dtSource.Select("HL_Code='" & sValue & "'").Length > 0 Then
                sResult = dtSource.Select("HL_Code='" & sValue & "'")(0)("SUNAT_Code")
            End If
        End If
        Return sResult
    End Function

    'Friend Function LoadTXT(TextFile As String) As String
    '    Dim sResult As String = ""
    '    Try
    '    Catch ex As Exception
    '    End Try
    '    Return sResult
    'End Function

End Class
