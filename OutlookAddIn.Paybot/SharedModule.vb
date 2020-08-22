Imports DevExpress.XtraRichEdit
Imports System.Windows.Forms
Imports System.IO
Imports System.Data
Imports System.Collections
Imports Microsoft.Office.Interop
Imports System.Runtime.InteropServices
Imports DevExpress.XtraGrid.Views.Grid
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.Diagnostics

Module SharedModule
    Friend MDBFileName As String = My.Settings.DBFileName
    Friend BlList, sException As New ArrayList
    Friend dtConfig, dtQuery, dtSubjects, dtCnfgLayout, dtCustomerList As New DataTable
    Friend Filter As String = ""
    Friend LstSpr = ";"
    Dim bIssued As Boolean = False
    Dim Msg As New RichTextBox
    Friend oIdentifier, oHtmlFile As String
    Friend drConfig As DataRow
    Dim oDirectory As String = Path.GetDirectoryName(My.Settings.DBFileName)

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

    Friend Function LoadExcelHDR(ByVal FileName As String, ByRef Hoja As String, ByRef RangeCell As String) As DataSet
        Dim dsResult As New DataSet
        Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties='Excel 12.0 Xml;HDR=Yes';"
        Using connection As New System.Data.OleDb.OleDbConnection(ExcelConnectionString)
            Try
                connection.Open()
                If Hoja = "{0}" Then
                    Hoja = connection.GetSchema("Tables").Rows(0)("TABLE_NAME")
                End If
                Dim Command As New System.Data.OleDb.OleDbDataAdapter("select * from [" & Hoja & IIf(Hoja.Contains("$"), "", "$") & RangeCell & "]", connection)
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

    Friend Function ExecuteAccessQuery(QueryString As String, DBFile As String) As DataSet
        Dim oLogFileGenerate As New LogFileGenerate
        Dim oAccessDB As String = ""
        If DBFile = "" Then
            oAccessDB = MDBFileName
        Else
            oAccessDB = Path.GetDirectoryName(MDBFileName) & "\" & DBFile
        End If
        Dim dsResult As New DataSet
        Dim ConnectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & oAccessDB & "';User ID=Admin;Password=;"
        Using connection As New System.Data.OleDb.OleDbConnection(ConnectionString)
            Try
                connection.Open()
                Dim Command As New System.Data.OleDb.OleDbDataAdapter(QueryString, connection)
                Command.Fill(dsResult)
            Catch ex As Exception
                sException.Add(ex.Message)
                oLogFileGenerate.TextFileUpdate("PAYBOT", "Función:ExecuteAccessQuery / DataBase:" & oAccessDB & " / QueryString:" & QueryString & " / Error:" & ex.Message)
                'DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

    Friend Function ExecuteAccessNonQuery(QueryString As String, DBFile As String) As Boolean
        Dim oAccessDB As String = ""
        If DBFile = "" Then
            oAccessDB = MDBFileName
        Else
            oAccessDB = Path.GetDirectoryName(MDBFileName) & "\" & DBFile
        End If
        Dim bResult As Boolean = True
        Dim ConnectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & oAccessDB & "'; Persist Security Info=False;"
        Using connection As New System.Data.OleDb.OleDbConnection(ConnectionString)
            Try
                connection.Open()
                Dim Command As New System.Data.OleDb.OleDbDataAdapter(QueryString, connection)
                Command.SelectCommand.ExecuteNonQuery()
            Catch ex As Exception
                sException.Add(ex.Message)
                'DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    Friend Function InsertIntoAccess(ByRef Table As String, ByVal drValues As DataRow, DBFile As String, mailItem As Outlook.MailItem, FileAttached As String) As Boolean
        Dim drColumns As OleDb.OleDbDataReader
        Dim bResult As Boolean = True
        Dim sQuery, sColumns, sValues As String
        Dim MailObject As New ArrayList
        sColumns = ""
        sValues = ""
        sQuery = ""
        MailObject.Add(My.Settings.SupportMailAddress) 'Mail TO
        MailObject.Add(My.Settings.CCMailAddress) 'Mail CC
        MailObject.Add(My.Settings.BCCMailAddress) 'Mail BCC
        'mailItem.To = My.Settings.SupportMailAddress
        'mailItem.CC = ""
        'mailItem.BCC = ""
        Dim dtSchema As New DataTable
        Dim oAccessDB As String = ""
        If DBFile = "" Then
            oAccessDB = MDBFileName
        Else
            oAccessDB = Path.GetDirectoryName(MDBFileName) & "\" & DBFile
        End If
        Dim AccessConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & oAccessDB & "';"
        Using connection As New System.Data.OleDb.OleDbConnection(AccessConnectionString)
            Try
                connection.Open()
                Dim Command As New System.Data.OleDb.OleDbDataAdapter("select * from [" & Table & "]", connection)
                drColumns = Command.SelectCommand.ExecuteReader()
                dtSchema = drColumns.GetSchemaTable
                For Each row As DataRow In dtSchema.Rows
                    'If drValues.Table.Columns.Contains(row.ItemArray(0)) Then
                    If Not IsDBNull(drValues.Item(dtSchema.Rows.IndexOf(row))) Then
                        sColumns = sColumns + IIf(dtSchema.Rows.IndexOf(row) = 0, "", ", ") & "[" & row.ItemArray(0) & "]"
                        If Not drValues.Table.Columns(dtSchema.Rows.IndexOf(row)).DataType = GetType(Boolean) Then
                            sValues = sValues + IIf(dtSchema.Rows.IndexOf(row) = 0, "'", ", '") & drValues.Item(dtSchema.Rows.IndexOf(row)) & "'"
                        Else
                            sValues = sValues & ", " & drValues.Item(dtSchema.Rows.IndexOf(row))
                        End If
                    End If
                    'End If
                Next
                sQuery = "insert into [" & Table & "] (" & sColumns & ") values (" & sValues & ")"
                Dim Command2 As New System.Data.OleDb.OleDbDataAdapter(sQuery, connection)
                Command2.SelectCommand.ExecuteNonQuery()
            Catch ex As Exception
                If Not mailItem Is Nothing Then
                    sException.Add(sQuery)
                    sException.Add(ex.Message)
                    MailObject.Add(mailItem.Subject & " (PROCESS WITH ERROR)")
                    MailObject.Add("Error: " & ex.Message & "<br><br>" & "Query: " & sQuery)
                    Dim sFileName As String = ""
                    If mailItem.Attachments.Count > 0 Then
                        SendExceptionMessage(FileAttached, MailObject)
                    Else
                        SendExceptionMessage("", MailObject)
                    End If
                End If
                bResult = False
            Finally
                connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    Friend Function UpdateAccess(Table As String, Condition As String, SetValues As String, DBFile As String) As Boolean
        Dim oAccessDB As String = ""
        If DBFile = "" Then
            oAccessDB = MDBFileName
        Else
            oAccessDB = Path.GetDirectoryName(MDBFileName) & "\" & DBFile
        End If
        Dim bResult As Boolean = True
        Dim sQuery As String = ""
        Dim AccessConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & oAccessDB & "';"
        Using connection As New System.Data.OleDb.OleDbConnection(AccessConnectionString)
            Try
                connection.Open()
                sQuery = "UPDATE [" & Table & "] SET " & SetValues & " WHERE " & Condition
                Dim Command As New System.Data.OleDb.OleDbDataAdapter(sQuery, connection)
                Command.SelectCommand.ExecuteNonQuery()
            Catch ex As Exception
                sException.Add(sQuery)
                sException.Add(ex.Message)
                'DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                bResult = False
            Finally
                connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    Friend Sub FillDataQry1()
        Dim sIssued As String = ""
        Dim sFecha As String = ""
        Dim dtDataQry As New DataTable
        dtQuery = ExecuteAccessQuery("SELECT blno AS [N° BL], 'NO' AS ESTADO, fecha_release1 AS FECHA FROM " & drConfig("Tabla") & " WHERE blno = '#'", "").Tables(0)
        For i = 0 To BlList.Count - 1
            If BlList(i).Trim <> "" And dtQuery.Select("[N° BL]='" & BlList(i).Trim & "'").Length = 0 Then
                dtDataQry = ExecuteAccessQuery("SELECT blno, 'NO', fecha_release1 FROM " & drConfig("Tabla") & " WHERE blno = '" & BlList(i) & "'", "").Tables(0)
                sIssued = "NO"
                If dtDataQry.Rows.Count > 0 Then
                    sIssued = "SI"
                    sFecha = dtDataQry.Rows(0)(2).ToString
                    dtQuery.Rows.Add(BlList(i), sIssued, sFecha)
                Else
                    dtQuery.Rows.Add(BlList(i), sIssued)
                End If
            End If
        Next
    End Sub

    Friend Sub FillDataQry2()
        dtQuery = ExecuteAccessQuery("SELECT * FROM CuentasBancariasImpo", "").Tables(0)
    End Sub

    Friend Sub FillDataQry3()
        dtQuery = ExecuteAccessQuery("SELECT * FROM CuentasBancariasExpo", "").Tables(0)
    End Sub

    Friend Function SendNewMessage(TypMsg As String, oMailItem As Outlook.MailItem, Identifier As String, Msg As String) As Boolean
        Dim NewMessage As Outlook.MailItem
        Dim AppOutlook As New Outlook.Application
        Dim bResult As Boolean = True
        NewMessage = AppOutlook.CreateItem(Outlook.OlItemType.olMailItem)
        Dim Recipents As Outlook.Recipients = NewMessage.Recipients
        Recipents.Add(oMailItem.SenderEmailAddress)
        NewMessage.BCC = My.Settings.SupportMailAddress
        NewMessage.BodyFormat = Outlook.OlBodyFormat.olFormatHTML
        If TypMsg = "OK" Then
            'Valid Subject
            NewMessage.Subject = oMailItem.Subject
            NewMessage.HTMLBody = GetValidMessageBody(oMailItem.SenderName)
        ElseIf TypMsg = "PRC_OK" Then
            NewMessage.Subject = oMailItem.Subject
            NewMessage.HTMLBody = NewMessage.HTMLBody & "<br><br>" & Msg
            If Identifier = "FLETES ONLINE" Then
                NewMessage.BCC = "pamela.marques@hlag.com"
            End If
        ElseIf TypMsg = "MSG_ERROR" Then
            'Invalid Subject
            NewMessage.Subject = "Asunto de mensaje inválido"
            NewMessage.HTMLBody = GetInvalidMessageBody(oMailItem.SenderName)
        ElseIf TypMsg = "PRC_ERROR" Then
            NewMessage.Subject = "Error al procesar " & Filter
            Recipents.Remove(1)
            Recipents.Add("aremonfe@gmail.com")
            NewMessage.HTMLBody = "El proceso asociado al identificador " & Identifier & " ha generado un error.<br><br>" ', los datos no han sido actualizados.<br><br> "
            If Msg <> "" Then
                NewMessage.HTMLBody += "MENSAJE DE ERROR:<br>"
                NewMessage.HTMLBody += Msg
            End If
        End If
        NewMessage.Send()
        oMailItem.Close(Microsoft.Office.Interop.Outlook.OlInspectorClose.olDiscard)
        Return bResult
    End Function

    Friend Function SendErrorMessage(oMailItem As Outlook.MailItem, Identifier As String, Msg As String, Attachment As ArrayList) As Boolean
        Dim NewMessage As Outlook.MailItem
        Dim AppOutlook As New Outlook.Application
        Dim bResult As Boolean = True
        NewMessage = AppOutlook.CreateItem(Outlook.OlItemType.olMailItem)
        Dim Recipents As Outlook.Recipients = NewMessage.Recipients
        Recipents.Add(oMailItem.SenderEmailAddress)
        If Not Attachment Is Nothing Then
            For a = 0 To Attachment.Count - 1
                NewMessage.Attachments.Add(Attachment(a))
            Next

        End If
        NewMessage.BCC = My.Settings.SupportMailAddress
        If Identifier = "GATE OUT" Then
            NewMessage.BCC = "aremonfe@gmail.com;mespinozac@tramarsa.com.pe;wwienera@tramarsa.com.pe"
        End If
        If Identifier = "FLETES ONLINE" Then
            NewMessage.BCC += "; pamela.marques@hlag.com"
        End If
        If Identifier = "PAYBOT" Then
            NewMessage.BCC += "; luis.avalos@hlag.com"
        End If
        NewMessage.BodyFormat = Outlook.OlBodyFormat.olFormatHTML
        NewMessage.Subject = "Error al procesar " & Identifier
        Recipents.Remove(1)
        'Recipents.Add("aremonfe@gmail.com")
        If Msg <> "" Then
            NewMessage.HTMLBody += "REFERENCIA: " & oMailItem.Subject & "<br><br>"
            NewMessage.HTMLBody += "MENSAJE DE ERROR:<br>"
            NewMessage.HTMLBody += Msg
        End If
        NewMessage.Send()
        oMailItem.Close(Microsoft.Office.Interop.Outlook.OlInspectorClose.olDiscard)
        Return bResult
    End Function

    Friend Function GetInvalidMessageBody(sender As String) As String
        Dim oText As New DevExpress.XtraRichEdit.RichEditControl
        bIssued = False
        Msg.Text = ""
        'Body
        Msg.AppendText("<html><body lang=ES style='tab-interval:35.4pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
        'Msg.AppendText("Estimado(a) " & sender & "<br><br>")
        Msg.AppendText(GetHtmlText(Filter, "Mensaje1", 1))
        'Signature
        Msg.AppendText(GetHtmlText(Filter, "Firma", 1))
        Return Replace(Msg.Text, "[Sender]", sender)
    End Function

    Friend Function GetValidMessageBody(sender As String) As String
        Dim oText As New DevExpress.XtraRichEdit.RichEditControl
        Dim sResponseType As Integer = dtConfig.Rows(0)("TipoRespuesta")
        bIssued = False
        Msg.Text = ""
        Msg.AppendText("<html><body lang=ES style='tab-interval:35.4pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
        'Msg.AppendText("Estimado(a) " & sender & "<br>")
        Msg.AppendText(GetHtmlText(Filter, "Mensaje1", sResponseType))
        If sResponseType = 3 Then
            GetHtmlTable(sender, dtQuery)
        End If
        If sResponseType = 3 Then
            If bIssued Then
                Msg.AppendText(GetHtmlText(Filter, "Mensaje2", sResponseType))
            Else
                Msg.AppendText(GetHtmlText(Filter, "Mensaje2", 1))
            End If
        Else
            Msg.AppendText(GetHtmlText(Filter, "Mensaje2", sResponseType))
        End If
        If ActiveNotice() Then
            Msg.AppendText(GetHtmlText(Filter, "Noticia", sResponseType))
        End If
        'Signature
        Msg.AppendText(GetHtmlText(Filter, "Firma", sResponseType))
        Msg.AppendText("</html></body>")
        Return Replace(Msg.Text, "[Sender]", sender)
    End Function

    Friend Function GetHtmlText(Identifier As String, FieldName As String, ResponseType As Integer) As String
        Dim sResult As String = ""
        Dim sCondition As String = ""
        drConfig = Nothing
        If ResponseType = 1 Then
            sCondition = "TipoRespuesta=" & ResponseType.ToString
        Else
            sCondition = "Identificador='" & Identifier & "' and TipoRespuesta=" & ResponseType.ToString
        End If
        If dtConfig.Select(sCondition).Length > 0 Then
            drConfig = dtConfig.Select(sCondition)(0)
            If Not IsDBNull(drConfig(FieldName)) Then
                sResult = drConfig(FieldName)
            End If
        End If
        Return sResult
    End Function

    Friend Function ActiveNotice() As Boolean
        Dim bResult As Boolean = False
        Dim IniDate, EndDate As Date
        If IsDBNull(dtConfig.Rows(0)("NoticiaVigenteDesde")) Then
            Return bResult
        End If
        IniDate = dtConfig.Rows(0)("NoticiaVigenteDesde")
        If IsDBNull(dtConfig.Rows(0)("NoticiaVigenteHasta")) Then
            EndDate = Date.Now
        Else
            EndDate = dtConfig.Rows(0)("NoticiaVigenteHasta")
        End If
        If Date.Now.ToShortDateString >= IniDate And Date.Now.ToShortDateString <= EndDate Then
            bResult = True
        End If
        Return bResult
    End Function

    Friend Function GetHtmlTable(sender As String, dtSource As DataTable) As String
        Dim sResponseType As Integer = dtConfig.Rows(0)("TipoRespuesta")

        If Filter.Contains("BL") Then
            AssignIssued(dtSource)
        Else
            Return ""
        End If
        'Inicio de Tabla
        Msg.AppendText("<table class=MsoTableGrid border=1 cellspacing=0 cellpadding=0")
        'Columns
        Msg.AppendText("<tr style='mso-yfti-irow:0;mso-yfti-firstrow:yes'>")
        For col = 0 To dtSource.Columns.Count - 1
            If col = 2 Then
                If Not bIssued Then
                    Continue For
                End If
            End If
            Msg.AppendText("<td width=auto valign=top style='width:134.45pt;border:solid windowtext 1.0pt;")
            Msg.AppendText("mso-border-alt:solid windowtext .5pt;background:#FFC000;padding:0cm 5.4pt 0cm 5.4pt'>")
            Msg.AppendText("<p class=MsoNormal align=center style='margin-bottom:0cm;margin-bottom:.0001pt;")
            Msg.AppendText("text-align:center;line-height:normal;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>" & dtSource.Columns(col).ColumnName & "</p></td>")
        Next
        Msg.AppendText("</tr>")
        'DataRows
        Msg.AppendText("<tr style='mso-yfti-irow:1;mso-yfti-lastrow:yes'>")
        For r = 0 To dtSource.Rows.Count - 1
            For c = 0 To dtSource.Columns.Count - 1
                If dtSource.Columns(c).DataType.Name = "String" Then
                    If IsDBNull(dtSource.Rows(r)(c)) Then
                        dtSource.Rows(r)(c) = ""
                    End If
                End If
                If dtSource.Columns(c).DataType.Name = "DateTime" Then
                    If IsDBNull(dtSource.Rows(r)(c)) Then
                        dtSource.Rows(r)(c) = "01/01/1900"
                    End If
                End If
                'DataColumn
                If Filter <> "OBLI" Then
                    Msg.AppendText("<td align=center width=auto valign=top style='width:134.45pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
                    Msg.AppendText("<p>" & dtSource.Rows(r)(c).trim & "</p></td>")
                Else
                    If (c <> 2) Or bIssued Then
                        Msg.AppendText("<td align=center width=auto valign=top style='width:134.45pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
                    End If
                    If IsDate(dtSource.Rows(r)(c)) Then
                        'If (c = 2 And dtSource.Rows.Count > 1 And dtSource.Rows(r)(2) <> "01/01/1900") Then
                        If (c = 2 And dtSource.Rows(r)(2) <> "01/01/1900") Then
                            Msg.AppendText("<p>" & Format(dtSource.Rows(r)(c), "dd/MM/yyyy") & "</p></td>")
                        Else
                            Msg.AppendText("<p>" & Space(10) & "</p></td>")
                        End If
                    Else
                        If (c <> 2) Or (c = 2 And dtSource.Rows.Count > 1) Then
                            If IsDate(dtSource.Rows(r)(2)) Then
                                Msg.AppendText("<p>" & IIf(IsDBNull(dtSource.Rows(r)(c)), "", IIf(c = 0, dtSource.Rows(r)(c), "")) & IIf(c = 1, dtCnfgLayout.Rows(0)("Resultado1"), "") & "</p></td>")
                            Else
                                Msg.AppendText("<p>" & IIf(IsDBNull(dtSource.Rows(r)(c)), "", IIf(c = 0, dtSource.Rows(r)(c), "")) & IIf(c = 1, dtCnfgLayout.Rows(0)("Resultado2"), "") & "</p></td>")
                            End If
                        End If
                    End If
                    'If (dtSource.Columns(c).ColumnName <> "FECHA") Or bIssued Then
                    '    Msg.AppendText("<td align=center width=auto valign=top style='width:134.45pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
                    'End If
                    'If IsDate(dtSource.Rows(r)(c)) Then
                    '    If (dtSource.Columns(c).ColumnName = "FECHA" And dtSource.Rows.Count > 1) Or (dtSource.Rows(0)("ESTADO") = "SI") Then
                    '        Msg.AppendText("<p>" & Format(dtSource.Rows(r)(c), "dd/MM/yyyy") & "</p></td>")
                    '    Else
                    '        Msg.AppendText("<p>" & Space(10) & "</p></td>")
                    '    End If
                    'Else
                    '    If (dtSource.Columns(c).ColumnName <> "FECHA") Or (dtSource.Columns(c).ColumnName = "FECHA" And dtSource.Rows.Count > 1) Then
                    '        Msg.AppendText("<p>" & IIf(IsDBNull(dtSource.Rows(r)(c)), "", dtSource.Rows(r)(c)) & IIf(dtSource.Columns(c).ColumnName = "ESTADO", " cuenta con emisión en destino", "") & "</p></td>")
                    '    End If
                    'End If
                End If
            Next
            Msg.AppendText("</tr>")
        Next
        Msg.AppendText("</table><br>")
        'Fin de Tabla
        Return Msg.Text
    End Function

    Private Sub AssignIssued(dtSource As DataTable)
        bIssued = False
        For r = 0 To dtSource.Rows.Count - 1
            If Not IsDBNull(dtSource.Rows(r).Item(2)) Then
                If IsDate(dtSource.Rows(r).Item(2)) Then
                    bIssued = True
                End If
            End If
        Next
    End Sub

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

    Friend Function ReplyMessage(oMailItem As Outlook.MailItem, sFileName As String) As Boolean
        Dim NewMessage As Outlook.MailItem
        Dim AppOutlook As New Outlook.Application
        Dim bResult As Boolean = True
        NewMessage = AppOutlook.CreateItem(Outlook.OlItemType.olMailItem)
        Dim Recipents As Outlook.Recipients = NewMessage.Recipients
        Recipents.Add(oMailItem.SenderEmailAddress)
        NewMessage.BodyFormat = Outlook.OlBodyFormat.olFormatHTML
        NewMessage.Subject = oMailItem.Subject
        NewMessage.HTMLBody = oMailItem.HTMLBody
        If sFileName <> "" Then
            NewMessage.Attachments.Add(sFileName)
        End If
        NewMessage.Send()
        Return bResult
    End Function

    Friend Function SendExceptionMessage(sFileName As String, MailObject As ArrayList) As Boolean
        Dim NewMessage As Outlook.MailItem
        Dim AppOutlook As New Outlook.Application
        Dim bResult As Boolean = True
        NewMessage = AppOutlook.CreateItem(Outlook.OlItemType.olMailItem)
        'Dim Recipents As Outlook.Recipients = NewMessage.Recipients
        'Recipents.Add(MailObject(0))
        NewMessage.To = MailObject(0)
        NewMessage.CC = MailObject(1)
        NewMessage.BCC = MailObject(2)
        NewMessage.Subject = MailObject(3)
        NewMessage.BodyFormat = Outlook.OlBodyFormat.olFormatHTML
        NewMessage.HTMLBody = MailObject(4)
        If sFileName <> "" Then
            NewMessage.Attachments.Add(sFileName)
        End If
        NewMessage.Send()
        Return bResult
    End Function

    Friend Function ExtraeAlfanumerico(TextIn As String) As String
        Dim TextOut As String = ""
        For c = 1 To TextIn.Length
            If Not (IsNumeric(Mid(TextIn, c, 1)) Or Mid(TextIn, c, 1).GetType = GetType(String)) Then
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

    Friend Function SaveToExcel(oGridView As GridView) As String
        Dim sPath As String = My.Settings.AttachedFilePath
        Dim sFileName = My.Settings.LogFilePath & "\" & "LOG" & Format(Today, "yyyyMMddHHmm") & ".xlsx"
        'oGridView.OptionsPrint.ExpandAllDetails = True
        'oGridView.OptionsPrint.AutoWidth = False
        'oGridView.BestFitMaxRowCount = oGridView.RowCount
        oGridView.ExportToXlsx(sFileName)
        Return sFileName
    End Function

    Function GetDPVoyage(Vessel As String, Voyage As String, Port As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("SELECT DPVoyage FROM [Local Voyage Control] WHERE VesselName='" & Vessel & "' AND ScheduleVoyage='" & Voyage & "' AND Port_Locode='" & Port & "'", "").Tables(0)
        If dtQuery.Rows.Count = 0 Then
            dtQuery = ExecuteAccessQuery("SELECT DPVOYAGE FROM ScheduleVoyage WHERE VESSEL_NAME='" & Vessel & "' AND SCHEDULE='" & Voyage & "' AND POL='" & Port & "'", "").Tables(0)
        End If
        If dtQuery.Rows.Count = 0 Then
            Return sResult
        End If
        sResult = dtQuery.Rows(0)(0)
        Return sResult
    End Function

    Friend Function GetMailProcess(ProcessCode As String) As DataRow
        Dim drMail As DataRow
        drMail = ExecuteAccessQuery("SELECT * FROM MailProcess WHERE ProcessCode='" & ProcessCode & "'", "").Tables(0).Rows(0)
        Return drMail
    End Function

    Friend Function GetVendorContact(ProcessCode As String, VendorCode As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("SELECT * FROM VendorContact WHERE ProcessCode='" & ProcessCode & "' AND VendorCode='" & VendorCode & "'", "").Tables(0)
        If dtQuery.Rows.Count > 0 Then
            For r = 0 To dtQuery.Rows.Count - 1
                sResult += dtQuery.Rows(r)("VendorContactMail") & ";"
            Next
        End If
        Return sResult
    End Function

    Friend Function GetVendorContactByTaxCode(ProcessCode As String, VendorTaxCode As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("SELECT * FROM VendorContact WHERE ProcessCode='" & ProcessCode & "' AND VendorCode='" & VendorTaxCode & "'", "").Tables(0)
        If dtQuery.Rows.Count > 0 Then
            For r = 0 To dtQuery.Rows.Count - 1
                sResult += dtQuery.Rows(r)("VendorContactMail") & ";"
            Next
        End If
        Return sResult
    End Function

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

    Public Sub ExportDataTableToExcel(ByVal DataGridView1 As DataTable, ByVal titulo As String)
        Dim m_Excel As New Excel.Application
        m_Excel.Visible = True
        Dim objLibroExcel As Excel.Workbook = m_Excel.Workbooks.Add
        Dim objHojaExcel As Excel.Worksheet = objLibroExcel.Worksheets(1)
        With objHojaExcel
            .Visible = Excel.XlSheetVisibility.xlSheetVisible
            .Activate()
            'Encabezado   
            .Range("A1:L1").Merge()
            .Range("A1:L1").Value = ""
            .Range("A1:L1").Font.Bold = True
            .Range("A1:L1").Font.Size = 15
            'Copete   
            .Range("A2:L2").Merge()
            .Range("A2:L2").Value = titulo
            .Range("A2:L2").Font.Bold = True
            .Range("A2:L2").Font.Size = 12

            Const primeraLetra As Char = "A"
            Const primerNumero As Short = 3
            Dim Letra As Char, UltimaLetra As Char
            Dim Numero As Integer, UltimoNumero As Integer
            Dim cod_letra As Byte = Asc(primeraLetra) - 1
            Dim sepDec As String = Application.CurrentCulture.NumberFormat.NumberDecimalSeparator
            Dim sepMil As String = Application.CurrentCulture.NumberFormat.NumberGroupSeparator
            'Establecer formatos de las columnas de la hija de cálculo   
            Dim strColumna As String = ""
            Dim LetraIzq As String = ""
            Dim cod_LetraIzq As Byte = Asc(primeraLetra) - 1
            Letra = primeraLetra
            Numero = primerNumero
            Dim objCelda As Excel.Range
            For Each c As DataGridViewColumn In DataGridView1.Columns
                If c.Visible Then
                    If Letra = "Z" Then
                        Letra = primeraLetra
                        cod_letra = Asc(primeraLetra)
                        cod_LetraIzq += 1
                        LetraIzq = Chr(cod_LetraIzq)
                    Else
                        cod_letra += 1
                        Letra = Chr(cod_letra)
                    End If
                    strColumna = LetraIzq + Letra + Numero.ToString
                    objCelda = .Range(strColumna, Type.Missing)
                    objCelda.Value = c.HeaderText
                    objCelda.EntireColumn.Font.Size = 8
                    'objCelda.EntireColumn.NumberFormat = c.DefaultCellStyle.Format   
                    If c.ValueType Is GetType(Decimal) OrElse c.ValueType Is GetType(Double) Then
                        objCelda.EntireColumn.NumberFormat = "#" + sepMil + "0" + sepDec + "00"
                    End If
                End If
            Next

            Dim objRangoEncab As Excel.Range = .Range(primeraLetra + Numero.ToString, LetraIzq + Letra + Numero.ToString)
            objRangoEncab.BorderAround(1, Excel.XlBorderWeight.xlMedium)
            UltimaLetra = Letra
            Dim UltimaLetraIzq As String = LetraIzq

            'CARGA DE DATOS   
            Dim i As Integer = Numero + 1

            For Each reg As DataGridViewRow In DataGridView1.Rows
                LetraIzq = ""
                cod_LetraIzq = Asc(primeraLetra) - 1
                Letra = primeraLetra
                cod_letra = Asc(primeraLetra) - 1
                For Each c As DataGridViewColumn In DataGridView1.Columns
                    If c.Visible Then
                        If Letra = "Z" Then
                            Letra = primeraLetra
                            cod_letra = Asc(primeraLetra)
                            cod_LetraIzq += 1
                            LetraIzq = Chr(cod_LetraIzq)
                        Else
                            cod_letra += 1
                            Letra = Chr(cod_letra)
                        End If
                        strColumna = LetraIzq + Letra
                        ' acá debería realizarse la carga   
                        .Cells(i, strColumna) = IIf(IsDBNull(reg.ToString), "", reg.Cells(c.Index).Value)
                    End If
                Next
                Dim objRangoReg As Excel.Range = .Range(primeraLetra + i.ToString, strColumna + i.ToString)
                objRangoReg.Rows.BorderAround()
                objRangoReg.Select()
                i += 1
            Next
            UltimoNumero = i
            'Dibujar las líneas de las columnas   
            LetraIzq = ""
            cod_LetraIzq = Asc("A")
            cod_letra = Asc(primeraLetra)
            Letra = primeraLetra
            For Each c As DataGridViewColumn In DataGridView1.Columns
                If c.Visible Then
                    objCelda = .Range(LetraIzq + Letra + primerNumero.ToString, LetraIzq + Letra + (UltimoNumero - 1).ToString)
                    objCelda.BorderAround()
                    If Letra = "Z" Then
                        Letra = primeraLetra
                        cod_letra = Asc(primeraLetra)
                        LetraIzq = Chr(cod_LetraIzq)
                        cod_LetraIzq += 1
                    Else
                        cod_letra += 1
                        Letra = Chr(cod_letra)
                    End If
                End If
            Next
            'Dibujar el border exterior grueso   
            Dim objRango As Excel.Range = .Range(primeraLetra + primerNumero.ToString, UltimaLetraIzq + UltimaLetra + (UltimoNumero - 1).ToString)
            objRango.Select()
            objRango.Columns.AutoFit()
            objRango.Columns.BorderAround(1, Excel.XlBorderWeight.xlMedium)
        End With

    End Sub

    Public ProcessorUtilization As Single

    Public Function GetAverageCPU() As Single
        Dim cpuCounter As New PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName)
        Dim i As Integer = 0
        While i < 11
            ProcessorUtilization += (cpuCounter.NextValue() / Environment.ProcessorCount)
            System.Threading.Interlocked.Increment(i)
        End While
        ' Remember the first value is 0, so we don't want to average that in.
        Console.WriteLine(ProcessorUtilization / 10)
        Return ProcessorUtilization / 10
    End Function

    Public Function GetMemoryUsage(ByVal ProcessName As String) As String
        Dim _Process As Process = Nothing
        Dim _Return As String = ""
        For Each _Process In Process.GetProcessesByName(ProcessName)
            If _Process.ToString.Remove(0, 27).ToLower = "(" & ProcessName.ToLower & ")" Then
                _Return = (_Process.WorkingSet64 / 1024).ToString("0,000") & " K"
            End If
        Next
        If Not _Process Is Nothing Then
            _Process.Dispose()
            _Process = Nothing
        End If
        Return _Return
    End Function

End Module
