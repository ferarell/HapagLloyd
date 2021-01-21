Imports DevExpress.XtraRichEdit
Imports System.Windows.Forms
Imports System.Data
Imports System.Collections
Imports Microsoft.Office.Interop
Imports System.Runtime.InteropServices
Imports DevExpress.XtraGrid.Views.Grid
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.IO

Module SharedModule
    'Friend MDBFileName As String = ""
    Friend BlList, sException As New ArrayList
    'Friend dtConfig, dtQuery, dtCnfgLayout, 
    Friend dtLogProcess As New DataTable
    'Friend Filter As String = ""
    Friend LstSpr = ";"
    Dim bIssued As Boolean = False
    Dim Msg As New RichTextBox
    Friend oIdentifier, oHtmlFile As String
    'Friend drConfig As DataRow
    Dim oDirectory As String = Path.GetDirectoryName(My.Settings.DBFileName)
    Friend AppPath As String = Path.GetDirectoryName(New Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath)

    Friend Function LoadExcel(ByVal FileName As String, ByRef Hoja As String) As DataSet
        Dim dsResult As New DataSet
        Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties=Excel 8.0;"
        Using connection As New System.Data.OleDb.OleDbConnection(ExcelConnectionString)
            Try
                connection.Open()
                If Hoja = "{0}" Then
                    For r = 0 To connection.GetSchema("Tables").Rows.Count - 1
                        If Not connection.GetSchema("Tables").Rows(r)("TABLE_NAME").toupper.contains("FILTER") Then
                            Hoja = connection.GetSchema("Tables").Rows(r)("TABLE_NAME")
                            Exit For
                        End If
                    Next
                End If
                Dim Command As New System.Data.OleDb.OleDbDataAdapter("select * from [" & Hoja & "]", connection)
                Command.Fill(dsResult)
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

    Friend Function LoadExcelWithConditions(FileName As String, Hoja As String, Condition As String) As DataSet
        Dim dsResult As New DataSet
        Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties=Excel 8.0;"
        Using connection As New System.Data.OleDb.OleDbConnection(ExcelConnectionString)
            Try
                connection.Open()
                If Hoja = "{0}" Then
                    For r = 0 To connection.GetSchema("Tables").Rows.Count - 1
                        If Not connection.GetSchema("Tables").Rows(r)("TABLE_NAME").toupper.contains("FILTER") Then
                            Hoja = connection.GetSchema("Tables").Rows(r)("TABLE_NAME")
                            Exit For
                        End If
                    Next
                End If
                Dim Command As New System.Data.OleDb.OleDbDataAdapter("select * from [" & Hoja & "] " & IIf(Condition <> "", " WHERE " & Condition, ""), connection)
                Command.Fill(dsResult)
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

    Friend Function QueryExcel(FileName As String, Query As String) As DataSet
        Dim dsResult As New DataSet
        Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties=Excel 8.0;"
        Using connection As New System.Data.OleDb.OleDbConnection(ExcelConnectionString)
            Try
                connection.Open()
                Dim Command As New System.Data.OleDb.OleDbDataAdapter(Query, connection)
                Command.Fill(dsResult)
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

    'Friend Sub FillDataQry1()
    '    Dim sIssued As String = ""
    '    Dim sFecha As String = ""
    '    Dim dtDataQry As New DataTable
    '    dtQuery = ExecuteAccessQuery("SELECT blno AS [N° BL], 'NO' AS ESTADO, fecha_release1 AS FECHA FROM " & drConfig("Tabla") & " WHERE blno = '#'").Tables(0)
    '    For i = 0 To BlList.Count - 1
    '        If BlList(i).Trim <> "" And dtQuery.Select("[N° BL]='" & BlList(i).Trim & "'").Length = 0 Then
    '            dtDataQry = ExecuteAccessQuery("SELECT blno, 'NO', fecha_release1 FROM " & drConfig("Tabla") & " WHERE blno = '" & BlList(i) & "'").Tables(0)
    '            sIssued = "NO"
    '            If dtDataQry.Rows.Count > 0 Then
    '                sIssued = "SI"
    '                sFecha = dtDataQry.Rows(0)(2).ToString
    '                dtQuery.Rows.Add(BlList(i), sIssued, sFecha)
    '            Else
    '                dtQuery.Rows.Add(BlList(i), sIssued)
    '            End If
    '        End If
    '    Next
    'End Sub

    'Friend Sub FillDataQry2()
    '    dtQuery = ExecuteAccessQuery("SELECT * FROM CuentasBancariasImpo").Tables(0)
    'End Sub

    'Friend Sub FillDataQry3()
    '    dtQuery = ExecuteAccessQuery("SELECT * FROM CuentasBancariasExpo").Tables(0)
    'End Sub

    'Friend Function SendNewMessage(TypMsg As String, oMailItem As Outlook.MailItem, ResponseMailSubject As String, Msg As String) As Boolean
    '    Dim NewMessage As Outlook.MailItem
    '    Dim AppOutlook As New Outlook.Application
    '    Dim bResult As Boolean = True
    '    NewMessage = AppOutlook.CreateItem(Outlook.OlItemType.olMailItem)
    '    Dim Recipents As Outlook.Recipients = NewMessage.Recipients
    '    Recipents.Add(oMailItem.SenderEmailAddress)
    '    NewMessage.BodyFormat = Outlook.OlBodyFormat.olFormatHTML
    '    If TypMsg = "OK" Then
    '        'Valid Subject
    '        NewMessage.Subject = ResponseMailSubject
    '        NewMessage.HTMLBody = GetValidMessageBody(oMailItem.SenderName)
    '    ElseIf TypMsg = "PRC_OK" Then
    '        NewMessage.Subject = ResponseMailSubject
    '        NewMessage.HTMLBody = NewMessage.HTMLBody & "<br><br>" & Msg
    '    ElseIf TypMsg = "MSG_ERROR" Then
    '        'Invalid Subject
    '        NewMessage.Subject = ResponseMailSubject
    '        NewMessage.HTMLBody = GetInvalidMessageBody(oMailItem.SenderName)
    '    ElseIf TypMsg = "PRC_ERROR" Then
    '        NewMessage.Subject = "Error al procesar " & Filter
    '        Recipents.Remove(0)
    '        Recipents.Add("aremonfe@gmail.com")
    '        NewMessage.HTMLBody = "El proceso asociado al identificador " & Filter & " ha generado un error, los datos no han sido actualizados.<br><br> "
    '        If Msg <> "" Then
    '            NewMessage.HTMLBody += "MENSAJE DE ERROR:<br>"
    '            NewMessage.HTMLBody += Msg
    '        End If
    '    End If
    '    NewMessage.Send()
    '    Return bResult
    'End Function

    'Friend Function GetInvalidMessageBody(sender As String, Identifier As String) As String
    '    Dim oText As New DevExpress.XtraRichEdit.RichEditControl
    '    bIssued = False
    '    Msg.Text = ""
    '    'Body
    '    Msg.AppendText("<html><body lang=ES style='tab-interval:35.4pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
    '    Msg.AppendText(GetHtmlText(Identifier, "Body", 0))
    '    'Signature
    '    Msg.AppendText(GetHtmlText(Identifier, "Signature", 0))
    '    Return Replace(Msg.Text, "[Sender]", sender)
    'End Function

    'Friend Function GetValidMessageBody(sender As String, Identifier As String) As String
    '    Dim oText As New DevExpress.XtraRichEdit.RichEditControl
    '    Dim sResponseType As Integer = dtConfig.Rows(0)("ResponseType")
    '    bIssued = False
    '    Msg.Text = ""
    '    Msg.AppendText("<html><body lang=ES style='tab-interval:35.4pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
    '    'Msg.AppendText("Estimado(a) " & sender & "<br>")
    '    Msg.AppendText(GetHtmlText(Identifier, "Header", sResponseType))
    '    If sResponseType = 3 Then
    '        GetHtmlTable(sender, dtQuery)
    '    End If
    '    If sResponseType = 3 Then
    '        If bIssued Then
    '            Msg.AppendText(GetHtmlText(Identifier, "Body", sResponseType))
    '        Else
    '            Msg.AppendText(GetHtmlText(Identifier, "Body", 1))
    '        End If
    '    Else
    '        Msg.AppendText(GetHtmlText(Identifier, "Body", sResponseType))
    '    End If
    '    If ActiveNotice() Then
    '        Msg.AppendText(GetHtmlText(Identifier, "News", sResponseType))
    '    End If
    '    'Signature
    '    Msg.AppendText(GetHtmlText(Identifier, "Signature", sResponseType))
    '    Msg.AppendText("</html></body>")
    '    Return Replace(Msg.Text, "[Sender]", sender)
    'End Function

    'Friend Function GetHtmlText(Identifier As String, FieldName As String, ResponseType As Integer) As String
    '    Dim sResult As String = ""
    '    Dim sCondition As String = ""
    '    drConfig = Nothing
    '    If ResponseType = 0 Then
    '        sCondition = "ResponseType=" & ResponseType.ToString
    '    Else
    '        sCondition = "Identifier='" & Identifier & "' and ResponseType=" & ResponseType.ToString
    '    End If
    '    If dtConfig.Select(sCondition).Length > 0 Then
    '        drConfig = dtConfig.Select(sCondition)(0)
    '        If Not IsDBNull(drConfig(FieldName)) Then
    '            sResult = drConfig(FieldName)
    '        End If
    '    End If
    '    Return sResult
    'End Function

    'Friend Function ActiveNotice() As Boolean
    '    Dim bResult As Boolean = False
    '    Dim IniDate, EndDate As Date
    '    If IsDBNull(dtConfig.Rows(0)("NewsValidityFrom")) Then
    '        Return bResult
    '    End If
    '    IniDate = dtConfig.Rows(0)("NewsValidityFrom")
    '    If IsDBNull(dtConfig.Rows(0)("NewsValidityTo")) Then
    '        EndDate = Date.Now
    '    Else
    '        EndDate = dtConfig.Rows(0)("NewsValidityTo")
    '    End If
    '    If Date.Now.ToShortDateString >= IniDate And Date.Now.ToShortDateString <= EndDate Then
    '        bResult = True
    '    End If
    '    Return bResult
    'End Function

    'Friend Function GetHtmlTable(sender As String, dtSource As DataTable) As String
    '    Dim sResponseType As Integer = dtConfig.Rows(0)("ResponseType")

    '    If Filter.Contains("BL") Then
    '        AssignIssued(dtSource)
    '    Else
    '        Return ""
    '    End If
    '    'Inicio de Tabla
    '    Msg.AppendText("<table class=MsoTableGrid border=1 cellspacing=0 cellpadding=0")
    '    'Columns
    '    Msg.AppendText("<tr style='mso-yfti-irow:0;mso-yfti-firstrow:yes'>")
    '    For col = 0 To dtSource.Columns.Count - 1
    '        If col = 2 Then
    '            If Not bIssued Then
    '                Continue For
    '            End If
    '        End If
    '        Msg.AppendText("<td width=auto valign=top style='width:134.45pt;border:solid windowtext 1.0pt;")
    '        Msg.AppendText("mso-border-alt:solid windowtext .5pt;background:#FFC000;padding:0cm 5.4pt 0cm 5.4pt'>")
    '        Msg.AppendText("<p class=MsoNormal align=center style='margin-bottom:0cm;margin-bottom:.0001pt;")
    '        Msg.AppendText("text-align:center;line-height:normal;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>" & dtSource.Columns(col).ColumnName & "</p></td>")
    '    Next
    '    Msg.AppendText("</tr>")
    '    'DataRows
    '    Msg.AppendText("<tr style='mso-yfti-irow:1;mso-yfti-lastrow:yes'>")
    '    For r = 0 To dtSource.Rows.Count - 1
    '        For c = 0 To dtSource.Columns.Count - 1
    '            If dtSource.Columns(c).DataType.Name = "String" Then
    '                If IsDBNull(dtSource.Rows(r)(c)) Then
    '                    dtSource.Rows(r)(c) = ""
    '                End If
    '            End If
    '            If dtSource.Columns(c).DataType.Name = "DateTime" Then
    '                If IsDBNull(dtSource.Rows(r)(c)) Then
    '                    dtSource.Rows(r)(c) = "01/01/1900"
    '                End If
    '            End If
    '            'DataColumn
    '            If Filter <> "OBLI" Then
    '                Msg.AppendText("<td align=center width=auto valign=top style='width:134.45pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
    '                Msg.AppendText("<p>" & dtSource.Rows(r)(c).trim & "</p></td>")
    '            Else
    '                If (c <> 2) Or bIssued Then
    '                    Msg.AppendText("<td align=center width=auto valign=top style='width:134.45pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
    '                End If
    '                If IsDate(dtSource.Rows(r)(c)) Then
    '                    'If (c = 2 And dtSource.Rows.Count > 1 And dtSource.Rows(r)(2) <> "01/01/1900") Then
    '                    If (c = 2 And dtSource.Rows(r)(2) <> "01/01/1900") Then
    '                        Msg.AppendText("<p>" & Format(dtSource.Rows(r)(c), "dd/MM/yyyy") & "</p></td>")
    '                    Else
    '                        Msg.AppendText("<p>" & Space(10) & "</p></td>")
    '                    End If
    '                Else
    '                    If (c <> 2) Or (c = 2 And dtSource.Rows.Count > 1) Then
    '                        If IsDate(dtSource.Rows(r)(2)) Then
    '                            If c > 2 Then
    '                                Msg.AppendText("<p>" & dtSource.Rows(r)(c).trim & "</p></td>")
    '                            Else
    '                                Msg.AppendText("<p>" & IIf(IsDBNull(dtSource.Rows(r)(c)), "", IIf(c = 0, dtSource.Rows(r)(c), "")) & IIf(c = 1, dtCnfgLayout.Rows(0)("Resultado1"), "") & "</p></td>")
    '                            End If
    '                        Else
    '                            If c > 2 Then
    '                                Msg.AppendText("<p>" & dtSource.Rows(r)(c).trim & "</p></td>")
    '                            Else
    '                                Msg.AppendText("<p>" & IIf(IsDBNull(dtSource.Rows(r)(c)), "", IIf(c = 0, dtSource.Rows(r)(c), "")) & IIf(c = 1, dtCnfgLayout.Rows(0)("Resultado2"), "") & "</p></td>")
    '                            End If
    '                        End If
    '                    End If
    '                End If
    '                'If (dtSource.Columns(c).ColumnName <> "FECHA") Or bIssued Then
    '                '    Msg.AppendText("<td align=center width=auto valign=top style='width:134.45pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
    '                'End If
    '                'If IsDate(dtSource.Rows(r)(c)) Then
    '                '    If (dtSource.Columns(c).ColumnName = "FECHA" And dtSource.Rows.Count > 1) Or (dtSource.Rows(0)("ESTADO") = "SI") Then
    '                '        Msg.AppendText("<p>" & Format(dtSource.Rows(r)(c), "dd/MM/yyyy") & "</p></td>")
    '                '    Else
    '                '        Msg.AppendText("<p>" & Space(10) & "</p></td>")
    '                '    End If
    '                'Else
    '                '    If (dtSource.Columns(c).ColumnName <> "FECHA") Or (dtSource.Columns(c).ColumnName = "FECHA" And dtSource.Rows.Count > 1) Then
    '                '        Msg.AppendText("<p>" & IIf(IsDBNull(dtSource.Rows(r)(c)), "", dtSource.Rows(r)(c)) & IIf(dtSource.Columns(c).ColumnName = "ESTADO", " cuenta con emisión en destino", "") & "</p></td>")
    '                '    End If
    '                'End If
    '            End If
    '        Next
    '        Msg.AppendText("</tr>")
    '    Next
    '    Msg.AppendText("</table><br>")
    '    'Fin de Tabla
    '    Return Msg.Text
    'End Function


    <System.Runtime.CompilerServices.Extension> _
    Public Function Contains(ByVal str As String, ByVal ParamArray values As String()) As Boolean
        For Each value In values
            If str.Contains(value) Then
                Return True
            End If
        Next
        Return False
    End Function

    Friend Function LoadCSV(FileName As String, Header As Boolean) As System.Data.DataTable
        Dim dtReading As New System.Data.DataTable
        Dim sColumn As String = ""
        Dim txtpos As String = ""
        Dim iPosCol As Integer = 0
        Dim line As New StreamReader(FileName, False)
        Dim sFila As String = line.ReadLine
        For i = 1 To sFila.Count + 1
            txtpos = Mid(sFila, i, 1)
            If txtpos = LstSpr Then 'Or i = sFila.Count + 1 Then
                If Header Then
                    If dtReading.Columns.Contains(sColumn) Then
                        sColumn = sColumn & "1"
                    End If
                    If sColumn <> "" Then
                        dtReading.Columns.Add(Replace(sColumn.TrimStart.TrimEnd, ".", "#")).AllowDBNull = True
                    End If
                Else
                    dtReading.Columns.Add("C" & (dtReading.Columns.Count + 1).ToString).AllowDBNull = True
                End If
                sColumn = ""
            Else
                sColumn = sColumn & txtpos
            End If
        Next
        Using sr As New StreamReader(FileName)
            Dim lines As List(Of String) = New List(Of String)
            Dim bExit As Boolean = False
            Dim sColumnValue As String = ""
            Do While Not sr.EndOfStream
                lines.Add(sr.ReadLine())
            Loop
            For i As Integer = 1 To lines.Count - 1
                iPosCol = 0
                txtpos = ""
                dtReading.Rows.Add()
                'lines(i) = lines(i).Trim
                For c = 1 To lines(i).Length + 1
                    txtpos = Mid(lines(i), c, 1)
                    If txtpos = LstSpr And iPosCol < dtReading.Columns.Count Then 'Or c = lines.Item(i).Length + 1 Then
                        dtReading.Rows(i - 1).Item(iPosCol) = sColumnValue.TrimEnd
                        iPosCol = iPosCol + 1
                        sColumnValue = ""
                    Else
                        If c = 1 Then
                            sColumnValue = ""
                        End If
                        sColumnValue = sColumnValue + txtpos.Replace("'", "")
                    End If
                Next
            Next
        End Using
        Return dtReading
    End Function

    'Friend Function ScheduleVoyageUpdate(row As DataRow) As Boolean
    '    Dim bResult As Boolean = True
    '    Try
    '        If ExecuteAccessQuery("select * from ScheduleVoyage where [DPVOYAGE]='" & row("DPVOYAGE") & "' and [POL]='" & row("POL") & "'").Tables(0).Rows.Count > 0 Then
    '            ExecuteAccessNonQuery("delete from ScheduleVoyage where [DPVOYAGE]='" & row("DPVOYAGE") & "' and [POL]='" & row("POL") & "'")
    '        End If
    '        InsertIntoAccess("ScheduleVoyage", row)
    '    Catch ex As Exception
    '        bResult = False
    '    End Try
    '    Return bResult
    'End Function

    Friend Function ReplyMessage(oMailItem As Outlook.MailItem, oFileName As String) As Boolean
        Dim NewMessage As Outlook.MailItem
        Dim AppOutlook As New Outlook.Application
        Dim bResult As Boolean = True
        NewMessage = AppOutlook.CreateItem(Outlook.OlItemType.olMailItem)
        Dim Recipents As Outlook.Recipients = NewMessage.Recipients
        Recipents.Add(oMailItem.SenderEmailAddress)
        NewMessage.BodyFormat = Outlook.OlBodyFormat.olFormatHTML
        NewMessage.Subject = oMailItem.Subject
        NewMessage.HTMLBody = oMailItem.HTMLBody
        NewMessage.Attachments.Add(oFileName)
        NewMessage.Send()
        Return bResult
    End Function

    Friend Function ExtraeAlfanumerico(TextIn As String) As String
        Dim TextOut As String = ""
        For c = 1 To TextIn.Length
            If Char.IsLetterOrDigit(Mid(TextIn, c, 1)) = False Then
                Continue For
            End If
            TextOut += Mid(TextIn, c, 1)
        Next
        Return TextOut
    End Function

    Friend Sub ExportaToExcel(sender As System.Object)
        Dim oGridView As New GridView
        oGridView = sender.MainView
        Dim sPath As String = Path.GetTempPath
        Dim sFileName = (FileIO.FileSystem.GetTempFileName).Replace(".tmp", ".xlsx")
        'oGridView.OptionsPrint.ExpandAllDetails = True
        oGridView.OptionsPrint.AutoWidth = False
        oGridView.BestFitMaxRowCount = oGridView.RowCount
        oGridView.ExportToXlsx(sFileName)
        If System.IO.File.Exists(sFileName) Then
            Dim oXls As New Excel.Application 'Crea el objeto excel 
            oXls.Workbooks.Open(sFileName, , False) 'El true es para abrir el archivo en modo Solo lectura (False si lo quieres de otro modo)
            oXls.Visible = True
            oXls.WindowState = Excel.XlWindowState.xlMaximized 'Para que la ventana aparezca maximizada.
        End If
    End Sub

    'Friend Function GetMailProcess(ProcessCode As String) As DataRow
    '    Dim drMail As DataRow
    '    drMail = ExecuteAccessQuery("SELECT * FROM MailProcess WHERE ProcessCode='" & ProcessCode & "'").Tables(0).Rows(0)
    '    Return drMail
    'End Function

    'Friend Function GetVendorContact(ProcessCode As String, VendorCode As String) As String
    '    Dim sResult As String = ""
    '    Dim dtQuery As New DataTable
    '    dtQuery = ExecuteAccessQuery("SELECT * FROM VendorContact WHERE ProcessCode='" & ProcessCode & "' AND VendorCode='" & VendorCode & "'").Tables(0)
    '    If dtQuery.Rows.Count > 0 Then
    '        For r = 0 To dtQuery.Rows.Count - 1
    '            sResult += dtQuery.Rows(r)("VendorContactMail") & ";"
    '        Next
    '    End If
    '    Return sResult
    'End Function

    'Friend Function GetVendorContactByTaxCode(ProcessCode As String, VendorTaxCode As String) As String
    '    Dim sResult As String = ""
    '    Dim dtQuery As New DataTable
    '    dtQuery = ExecuteAccessQuery("SELECT * FROM VendorContact WHERE ProcessCode='" & ProcessCode & "' AND VendorCode='" & VendorTaxCode & "'").Tables(0)
    '    If dtQuery.Rows.Count > 0 Then
    '        For r = 0 To dtQuery.Rows.Count - 1
    '            sResult += dtQuery.Rows(r)("VendorContactMail") & ";"
    '        Next
    '    End If
    '    Return sResult
    'End Function

    Friend Function GetTextFromPDF(PdfFileName As String) As String
        Dim oReader As New iTextSharp.text.pdf.PdfReader(PdfFileName)
        Dim sOut = ""
        For i = 1 To oReader.NumberOfPages
            Dim its As New iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy
            sOut &= iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(oReader, i, its)
        Next
        oReader.Close()
        oReader.Dispose()
        Return sOut
    End Function

    Public Sub PDFTxtToPdf(ByVal sTxtfile As String, ByVal sPDFSourcefile As String)
        Dim sr As StreamReader = New StreamReader(sTxtfile)
        Dim doc As New Document()
        PdfWriter.GetInstance(doc, New FileStream(sPDFSourcefile, FileMode.Create))
        doc.Open()
        doc.Add(New Paragraph(sr.ReadToEnd()))
        doc.Close()
    End Sub

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

End Module
