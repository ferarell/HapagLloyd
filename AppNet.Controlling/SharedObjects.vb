Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Imports System.Collections
Imports System.Net.Mail
Imports System.Globalization
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.IO
Imports Microsoft.Office.Interop
Imports DevExpress.XtraGrid.Views.Grid

Module SharedObjects
    Friend DBFileName As String = ""
    Friend SkinName As String
    Friend LstSpr As String = ";" 'CultureInfo.CurrentCulture.TextInfo.ListSeparator
    Dim oAppService As New AppService.HapagLloydServiceClient

    Friend Function ExecuteSQL(ByVal QueryString As String) As DataSet
        Dim dsResult As New DataSet
        dsResult = oAppService.ExecuteSQL(QueryString)
        Return dsResult
    End Function

    Friend Function ExecuteSQLNonQuery(ByVal QueryString As String) As ArrayList
        Dim aResult As New ArrayList
        aResult.AddRange(oAppService.ExecuteSQLNonQuery(QueryString))
        Return aResult
    End Function

    Friend Function InsertIntoSQL(ByRef Table As String, ByVal drValues As DataRow) As Boolean
        Dim bResult As Boolean = True
        Dim sQuery, sColumns, sValues As String
        Dim dtSchema As New DataTable
        Dim aResult As New ArrayList
        Try
            dtSchema = ExecuteSQL("SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & Mid(Table, InStr(Table, ".") + 1, Len(Table)) & "'").Tables(0)
            For Each row As DataRow In dtSchema.Rows
                If Not IsDBNull(drValues.Item(dtSchema.Rows.IndexOf(row))) Then
                    sColumns = sColumns + IIf(dtSchema.Rows.IndexOf(row) = 0, "", ", ") & "[" & row.ItemArray(0) & "]"
                    sValues = sValues + IIf(dtSchema.Rows.IndexOf(row) = 0, "'", ", '") & Strings.RTrim(drValues.Item(dtSchema.Rows.IndexOf(row))) & "'"
                End If
                'End If
            Next
            sQuery = "insert into [" & Table & "] (" & sColumns & ") values (" & sValues & ");"
            aResult.AddRange(ExecuteSQLNonQuery(sQuery))
            bResult = aResult(0)
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            bResult = False
        End Try
        Return bResult
    End Function

    'Friend Function UpdateAccess(Table As String, Condition As String, SetValues As String) As Boolean
    '    Dim bResult As Boolean = True
    '    Dim sQuery As String
    '    Dim FileName As String = My.Settings.DBDirectory & "\" & My.Settings.DBFileName
    '    Dim AccessConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "';"
    '    Using connection As New System.Data.OleDb.OleDbConnection(AccessConnectionString)
    '        Try
    '            connection.Open()
    '            sQuery = "UPDATE [" & Table & "] SET " & SetValues & " WHERE " & Condition
    '            Dim Command As New System.Data.OleDb.OleDbDataAdapter(sQuery, connection)
    '            Command.SelectCommand.ExecuteNonQuery()
    '        Catch ex As Exception
    '            DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '            bResult = False
    '        Finally
    '            connection.Close()
    '        End Try
    '        Return bResult
    '    End Using
    'End Function

    Friend Function LoadExcel(ByVal FileName As String, ByRef Hoja As String) As DataSet
        Dim dsResult As New DataSet
        Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties='Excel 12.0 Xml; IMEX=1'"
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
                Dim Command As New System.Data.OleDb.OleDbDataAdapter("select * from [" & Hoja & "] ", connection)
                Command.Fill(dsResult)
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

    Friend Function LoadExcelWC(ByVal FileName As String, ByRef Hoja As String, Condition As String) As DataSet
        Dim dsResult As New DataSet
        Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties='Excel 12.0 Xml; IMEX=1'"
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
                Dim Command As New System.Data.OleDb.OleDbDataAdapter("select * from [" & Hoja & "] " & IIf(Condition <> "", " where " & Condition, ""), connection)
                Command.Fill(dsResult)
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

    Friend Function LoadExcelWH(ByVal FileName As String, ByRef Hoja As String, ByRef Condition As String) As DataSet
        Dim dsResult As New DataSet
        Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties='Excel 12.0 Xml; IMEX=1; HDR=No';"
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
                Dim Command As New System.Data.OleDb.OleDbDataAdapter("select * from [" & Hoja & "] " & IIf(Condition <> "", " where " & Condition, ""), connection)
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

    Friend Function InsertIntoExcel(ByVal FileName As String, ByRef Hoja As String, ByVal drValues As DataRow) As Boolean
        Dim drColumns As OleDb.OleDbDataReader
        Dim bResult As Boolean = True
        Dim sQuery, sColumns, sValues As String
        Dim col As DataColumn
        Dim dtSchema As New DataTable
        Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties=Excel 8.0;"
        Using connection As New System.Data.OleDb.OleDbConnection(ExcelConnectionString)
            Try
                connection.Open()
                If Hoja = "{0}" Then
                    Hoja = connection.GetSchema("Tables").Rows(0)("TABLE_NAME")
                End If
                Dim Command As New System.Data.OleDb.OleDbDataAdapter("select * from [" & Hoja & "]", connection)
                drColumns = Command.SelectCommand.ExecuteReader()
                dtSchema = drColumns.GetSchemaTable
                For Each row As DataRow In dtSchema.Rows
                    sColumns = sColumns + IIf(dtSchema.Rows.IndexOf(row) = 0, "", ", ") & "[" & row.ItemArray(0) & "]"
                    sValues = sValues + IIf(dtSchema.Rows.IndexOf(row) = 0, "'", ", '") & drValues.Item(dtSchema.Rows.IndexOf(row)) & "'"
                Next
                sQuery = "insert into [" & Hoja & "] (" & sColumns & ") values (" & sValues & ")"
                Dim Command2 As New System.Data.OleDb.OleDbDataAdapter(sQuery, connection)
                Command2.SelectCommand.ExecuteNonQuery()
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                bResult = False
            Finally
                connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    Friend Function UpdateExcel(ByVal FileName As String, ByRef Hoja As String, Condition As String, SetValues As String) As Boolean
        Dim bResult As Boolean = True
        Dim sQuery As String
        Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties=Excel 8.0;"
        Using connection As New System.Data.OleDb.OleDbConnection(ExcelConnectionString)
            Try
                connection.Open()
                sQuery = "UPDATE [" & Hoja & "] SET " & SetValues & " WHERE " & Condition
                Dim Command As New System.Data.OleDb.OleDbDataAdapter(sQuery, connection)
                Command.SelectCommand.ExecuteNonQuery()
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                bResult = False
            Finally
                connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    Friend Function InsertRowIntoExcel(ByVal FileName As String, ByRef Hoja As String, ByVal drValues As DataRow) As Boolean
        Dim drColumns As OleDb.OleDbDataReader
        Dim bResult As Boolean = True
        Dim sQuery, sColumns, sValues As String
        Dim col As DataColumn
        Dim dtSchema As New DataTable
        Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties=Excel 8.0;"
        Using connection As New System.Data.OleDb.OleDbConnection(ExcelConnectionString)
            Try
                connection.Open()
                If Hoja = "{0}" Then
                    Hoja = connection.GetSchema("Tables").Rows(0)("TABLE_NAME")
                End If
                Dim Command As New System.Data.OleDb.OleDbDataAdapter("select [F1] from [" & Hoja & "]", connection)
                drColumns = Command.SelectCommand.ExecuteReader()
                dtSchema = drColumns.GetSchemaTable
                For Each row As DataRow In dtSchema.Rows
                    sColumns = sColumns + IIf(dtSchema.Rows.IndexOf(row) = 0, "", ", ") & "[" & row.ItemArray(0) & "]"
                    sValues = sValues + IIf(dtSchema.Rows.IndexOf(row) = 0, "'", ", '") & drValues.Item(dtSchema.Rows.IndexOf(row)) & "'"
                Next
                sQuery = "insert into [" & Hoja & "] (" & sColumns & ") values (" & sValues & ")"
                Dim Command2 As New System.Data.OleDb.OleDbDataAdapter(sQuery, connection)
                Command2.SelectCommand.ExecuteNonQuery()
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                bResult = False
            Finally
                connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    'Friend Function LoadCSV(ByVal FilePath As String, ByVal FileName As String) As DataSet
    '    Dim dsResult As New DataSet
    '    Dim ConnectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" & FileName & "'; Extended Properties=text; Format=Delimited;"
    '    Using connection As New System.Data.OleDb.OleDbConnection(ConnectionString)
    '        Try
    '            connection.Open()
    '            Dim Command As New System.Data.OleDb.OleDbDataAdapter("select * from [" & FileName & "]", connection)
    '            Command.Fill(dsResult)
    '        Catch ex As Exception
    '            DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        Finally
    '            connection.Close()
    '        End Try
    '        Return dsResult
    '    End Using
    'End Function

    Friend Function CreateTextDelimiterFile(ByVal fileName As String,
                                         ByVal dt As DataTable,
                                         ByVal separatorChar As Char,
                                         ByVal hdr As Boolean,
                                         ByVal textDelimiter As Boolean) As Boolean

        ' Si no se ha especificado un nombre de archivo,
        ' o el objeto DataTable no es válido, provocamos
        ' una excepción de argumentos no válidos.
        '
        If (fileName = String.Empty) OrElse
       (dt Is Nothing) Then Throw New System.ArgumentException("Argumentos no válidos.")

        ' Si el archivo existe, solicito confirmación para sobreescribirlo.
        '
        If (System.IO.File.Exists(fileName)) Then
            If (DevExpress.XtraEditors.XtraMessageBox.Show("Ya existe un archivo de texto con el mismo nombre." & Environment.NewLine &
                           "¿Desea sobrescribirlo?",
                           "Crear archivo de texto delimitado",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Information) = DialogResult.No) Then Return False
        End If

        Dim sw As System.IO.StreamWriter

        Try
            Dim col As Integer = 0
            Dim value As String = String.Empty

            ' Creamos el archivo de texto con la codificación por defecto.
            '
            sw = New System.IO.StreamWriter(fileName, False, System.Text.Encoding.Default)

            If (hdr) Then
                ' La primera línea del archivo de texto contiene
                ' el nombre de los campos.
                For Each dc As DataColumn In dt.Columns

                    If (textDelimiter) Then
                        ' Incluimos el nombre del campo entre el caracter
                        ' delimitador de texto especificado.
                        '
                        value &= """" & dc.ColumnName & """" & separatorChar

                    Else
                        ' No se incluye caracter delimitador de texto alguno.
                        '
                        value &= dc.ColumnName & separatorChar

                    End If

                Next

                sw.WriteLine(value.Remove(value.Length - 1, 1))
                value = String.Empty

            End If

            ' Recorremos todas las filas del objeto DataTable
            ' incluido en el conjunto de datos.
            '
            For Each dr As DataRow In dt.Rows

                For Each dc As DataColumn In dt.Columns

                    If ((dc.DataType Is System.Type.GetType("System.String")) And
                   (textDelimiter = True)) Then

                        ' Incluimos el dato alfanumérico entre el caracter
                        ' delimitador de texto especificado.
                        '
                        value &= """" & dr.Item(col).ToString & """" & separatorChar

                    Else
                        ' No se incluye caracter delimitador de texto alguno
                        '
                        value &= dr.Item(col).ToString & separatorChar

                    End If

                    ' Siguiente columna
                    col += 1

                Next

                ' Al escribir los datos en el archivo, elimino el
                ' último carácter delimitador de la fila.
                '
                sw.WriteLine(value.Remove(value.Length - 1, 1))
                value = String.Empty
                col = 0

            Next ' Siguiente fila

            ' Nos aseguramos de cerrar el archivo
            '
            sw.Close()

            ' Se ha creado con éxito el archivo de texto.
            '
            Return True

        Catch ex As Exception
            Return False

        Finally
            sw = Nothing

        End Try
    End Function

    Friend Function CreateFormatTable() As DataTable
        Dim dtProcess As New Data.DataTable
        dtProcess.Columns.Add("CompanyCode").AllowDBNull = True
        dtProcess.Columns.Add("PostingKey").AllowDBNull = True
        dtProcess.Columns.Add("AccountNumber").AllowDBNull = True
        dtProcess.Columns.Add("AmountDocumentCurrency").AllowDBNull = True
        dtProcess.Columns.Add("CurrencyKey").AllowDBNull = True
        dtProcess.Columns.Add("Text").AllowDBNull = True
        dtProcess.Columns.Add("ReferenceDocumentNumber").AllowDBNull = True
        dtProcess.Columns.Add("ValueDate").AllowDBNull = True
        dtProcess.Columns.Add("AssignmentNumber").AllowDBNull = True
        dtProcess.Columns.Add("PostingDate").AllowDBNull = True
        dtProcess.Columns.Add("DocumentDate").AllowDBNull = True
        dtProcess.Columns.Add("DocumentType").AllowDBNull = True
        Return dtProcess
    End Function

    Friend Function FillDataTable(Sheet As String) As DataTable
        Return LoadExcel(DBFileName, Sheet).Tables(0)
    End Function

    Friend Sub ExportarExcel(sender As System.Object)
        Dim oGridView As New GridView
        oGridView = sender.MainView
        Dim sPath As String = Path.GetTempPath
        Dim sFileName = (FileIO.FileSystem.GetTempFileName).Replace(".tmp", ".xls")
        'oGridView.OptionsPrint.ExpandAllDetails = True
        oGridView.OptionsPrint.AutoWidth = False
        oGridView.BestFitMaxRowCount = oGridView.RowCount
        oGridView.ExportToXls(sFileName)
        If System.IO.File.Exists(sFileName) Then
            Dim oXls As New Excel.Application 'Crea el objeto excel 
            oXls.Workbooks.Open(sFileName, , False) 'El true es para abrir el archivo en modo Solo lectura (False si lo quieres de otro modo)
            oXls.Visible = True
            oXls.WindowState = Excel.XlWindowState.xlMaximized 'Para que la ventana aparezca maximizada.
        End If
    End Sub

    Friend Sub ExportGraphToExcel(sender As System.Object)
        Dim sPath As String = Path.GetTempPath
        Dim sFileName = FileIO.FileSystem.GetTempFileName + ".xls"
        sender.ExportToXlsx(sFileName)
        Dim oXls As New Excel.Application 'Crea el objeto excel 
        oXls.Workbooks.Open(sFileName, , True) 'El true es para abrir el archivo en modo Solo lectura (False si lo quieres de otro modo)
        oXls.Visible = True
        oXls.WindowState = Excel.XlWindowState.xlMaximized 'Para que la ventana aparezca maximizada.
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

    Friend Function GetReadingDate(CustomDate As String) As String
        Dim sResult As String = ""
        sResult = CustomDate.Substring(4, 2) & "/" & ConvertShortMonthAsNumber(CustomDate.Substring(0, 3)) & "/" & CustomDate.Substring(8, 4)
        Return sResult
    End Function

    Friend Function ConvertShortMonthAsNumber(month As String) As String
        Dim sResult As String = ""
        If month.ToUpper.Contains({"ENE", "JAN"}) Then
            sResult = "01"
        End If
        If month.ToUpper.Contains({"FEB"}) Then
            sResult = "02"
        End If
        If month.ToUpper.Contains({"MAR"}) Then
            sResult = "03"
        End If
        If month.ToUpper.Contains({"ABR", "APR"}) Then
            sResult = "04"
        End If
        If month.ToUpper.Contains({"MAY"}) Then
            sResult = "05"
        End If
        If month.ToUpper.Contains({"JUN"}) Then
            sResult = "06"
        End If
        If month.ToUpper.Contains({"JUL"}) Then
            sResult = "07"
        End If
        If month.ToUpper.Contains({"AGO", "AUG"}) Then
            sResult = "08"
        End If
        If month.ToUpper.Contains({"SET", "SEP"}) Then
            sResult = "09"
        End If
        If month.ToUpper.Contains({"OCT"}) Then
            sResult = "10"
        End If
        If month.ToUpper.Contains({"NOV"}) Then
            sResult = "11"
        End If
        If month.ToUpper.Contains({"DIC"}) Then
            sResult = "12"
        End If
        Return sResult
    End Function

    Friend Function SelectDistinct(ByVal SourceTable As DataTable, ByVal Condition As String, ByVal ParamArray FieldNames() As String) As DataTable
        Dim lastValues() As Object
        Dim newTable As DataTable

        If FieldNames Is Nothing OrElse FieldNames.Length = 0 Then
            Throw New ArgumentNullException("FieldNames")
        End If

        lastValues = New Object(FieldNames.Length - 1) {}
        newTable = New DataTable

        For Each field As String In FieldNames
            newTable.Columns.Add(field, SourceTable.Columns(field).DataType)
        Next

        For Each Row As DataRow In SourceTable.Select(Condition, String.Join(", ", FieldNames))
            If Not fieldValuesAreEqual(lastValues, Row, FieldNames) Then
                newTable.Rows.Add(createRowClone(Row, newTable.NewRow(), FieldNames))

                setLastValues(lastValues, Row, FieldNames)
            End If
        Next

        Return newTable
    End Function

    Friend Function fieldValuesAreEqual(ByVal lastValues() As Object, ByVal currentRow As DataRow, ByVal fieldNames() As String) As Boolean
        Dim areEqual As Boolean = True

        For i As Integer = 0 To fieldNames.Length - 1
            If lastValues(i) Is Nothing OrElse Not lastValues(i).Equals(currentRow(fieldNames(i))) Then
                areEqual = False
                Exit For
            End If
        Next

        Return areEqual
    End Function

    Private Function createRowClone(ByVal sourceRow As DataRow, ByVal newRow As DataRow, ByVal fieldNames() As String) As DataRow
        For Each field As String In fieldNames
            newRow(field) = sourceRow(field)
        Next

        Return newRow
    End Function

    Private Sub setLastValues(ByVal lastValues() As Object, ByVal sourceRow As DataRow, ByVal fieldNames() As String)
        For i As Integer = 0 To fieldNames.Length - 1
            lastValues(i) = sourceRow(fieldNames(i))
        Next
    End Sub

    'Friend Sub SendMail(MailSubject As String, MailBody As String, Attachments As Boolean)
    '    Dim smtp As New SmtpClient
    '    Dim mail As New MailMessage
    '    Dim bError As Boolean = False
    '    Try
    '        smtp.Timeout = 15000
    '        smtp.UseDefaultCredentials = False
    '        smtp.EnableSsl = My.Settings.MailServerSsl
    '        If smtp.EnableSsl Then
    '            smtp.Credentials = New System.Net.NetworkCredential(My.Settings.MailServerUser, My.Settings.MailServerPassword)
    '        Else
    '            smtp.UseDefaultCredentials = True
    '        End If
    '        smtp.Host = My.Settings.MailServerHost
    '        smtp.Port = My.Settings.MailServerPort
    '        smtp.DeliveryMethod = SmtpDeliveryMethod.Network
    '        'smtp.SendMailAsync(My.Settings.MailSender, My.Settings.MailRecipients, MailSubject, MailBody)
    '        'smtp.Send(My.Settings.MailSender, My.Settings.MailRecipients, MailSubject, MailBody)
    '        mail.From = New MailAddress(My.Settings.MailSender)
    '        mail.To.Add(My.Settings.MailRecipients)
    '        mail.Subject = MailSubject
    '        mail.Body = MailBody
    '        If Attachments Then
    '            mail.Attachments.Add(New Attachment("C:\Users\ferar_000\Google Drive\Proyectos\HLAG\Operations\WordingC1.mht"))
    '        End If
    '        smtp.Send(mail)
    '    Catch se As SmtpException
    '        bError = True
    '        DevExpress.XtraEditors.XtraMessageBox.Show(se.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    Catch ex As Exception
    '        bError = True
    '        DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    End Try
    'End Sub

    Sub AttachmentFromFile()
        'create the mail message
        Dim mail As New MailMessage()

        'set the addresses
        mail.From = New MailAddress("me@mycompany.com")
        mail.To.Add("you@yourcompany.com")

        'set the content
        mail.Subject = "This is an email"
        mail.Body = "this content is in the body"

        'add an attachment from the filesystem
        mail.Attachments.Add(New Attachment("c:\temp\example.txt"))

        'to add additional attachments, simply call .Add(...) again
        mail.Attachments.Add(New Attachment("c:\temp\example2.txt"))
        mail.Attachments.Add(New Attachment("c:\temp\example3.txt"))

        'send the message
        Dim smtp As New SmtpClient("127.0.0.1")
        smtp.Send(mail)

    End Sub 'AttachmentFromFile

    'Private Sub OnItemSend(Item As System.Object, ByRef Cancel As Boolean) _
    '                   Handles Application.ItemSend
    '    Dim recipient As Outlook.Recipient = Nothing
    '    Dim recipients As Outlook.Recipients = Nothing
    '    Dim mail As Outlook.MailItem = TryCast(Item, Outlook.MailItem)
    '    If Not IsNothing(mail) Then
    '        Dim addToSubject As String = " !IMPORTANT"
    '        Dim addToBody As String = "Sent from my Outlook 2010"
    '        If Not mail.Subject.Contains(addToSubject) Then
    '            mail.Subject += addToSubject
    '        End If
    '        If Not mail.Body.EndsWith(addToBody) Then
    '            mail.Body += addToBody
    '        End If
    '        recipients = mail.Recipients
    '        recipient = recipients.Add("Eugene Astafiev")
    '        recipient.Type = Outlook.OlMailRecipientType.olBCC
    '        recipient.Resolve()
    '        If Not IsNothing(recipient) Then Marshal.ReleaseComObject(recipient)
    '        If Not IsNothing(recipients) Then Marshal.ReleaseComObject(recipients)
    '    End If
    'End Sub

    'Private Sub MessageGenerate()
    '    Dim message As Microsoft.Office.Interop.Outlook.MailItem
    '    message.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML
    '    message.Attachments.Add("Templates\MessageC1.msg")
    'End Sub

    Friend Function TextContain(text As String, type As String) As Boolean
        Dim bResult As Boolean = True
        If type = "MonthOfYear" Then
            If Not text.ToUpper.Contains("ENE ", "FEB ", "MAR ", "ABR ", "MAY ", "JUN ", "JUL ", "AGO ", "SET ", "OCT ", "NOV ", "DIC ") And Not text.ToUpper.Contains("JAN ", "FEB ", "MAR ", "APR ", "MAY ", "JUN ", "JUL ", "AUG ", "SEP ", "OCT ", "NOV ", "DEC ") Then
                bResult = False
            End If
        End If
        If type = "OnlyNumbers" Then
            If text.Length > 0 Then
                For i As Integer = 1 To text.Length
                    If Not Mid(text, i, 1).Contains(",", ".", "-") Then
                        If Not Mid(text, i, 1).Contains("0", "1", "2", "3", "4", "5", "6", "7", "8", "9") Then
                            bResult = False
                            Exit For
                        End If
                    End If
                    i = i + 1
                Next
            End If
        End If
        Return bResult
    End Function

    'Friend Sub TextToSpeak(sText As String)
    '    If My.Settings.AudioEnabled Then
    '        Dim t As New System.Threading.Thread(AddressOf SpeechThread)
    '        t.Start(sText)
    '    End If
    'End Sub

    'Private Sub SpeechThread(sText As String)
    '    Try
    '        Dim sapi
    '        sapi = CreateObject("sapi.spvoice")
    '        sapi.speak(sText)
    '    Catch ex As Exception
    '        My.Settings.AudioEnabled = False
    '        My.Settings.Save()
    '    End Try
    'End Sub

    Friend Function LoadCSV(FileName As String, Header As Boolean) As DataTable
        Dim dtReading As New DataTable
        Dim sColumn As String = ""
        Dim txtpos As String = ""
        Dim iPosCol As Integer = 0
        Dim iCols As Integer = 0
        Dim line As New StreamReader(FileName, False)
        Dim sFila As String = line.ReadLine
        For i = 1 To sFila.Count + 1
            txtpos = Mid(sFila, i, 1)
            If (txtpos = LstSpr Or i = sFila.Count + 1) Then
                If Header Then
                    If sColumn <> "" Then
                        dtReading.Columns.Add(Strings.RTrim(sColumn)).AllowDBNull = True
                    End If
                Else
                    dtReading.Columns.Add("C" & (dtReading.Columns.Count + 1).ToString).AllowDBNull = True
                End If
                sColumn = ""
            Else
                sColumn = sColumn & txtpos
            End If
        Next
        iCols = dtReading.Columns.Count - 1
        Using sr As New StreamReader(FileName)
            Dim lines As List(Of String) = New List(Of String)
            Dim bExit As Boolean = False
            Dim sColumnValue As String = ""
            Do While Not sr.EndOfStream
                lines.Add(sr.ReadLine())
            Loop
            For r As Integer = 1 To lines.Count - 1
                iPosCol = 0
                txtpos = ""
                dtReading.Rows.Add()
                For c = 1 To lines.Item(r).Length + 1
                    If iPosCol >= dtReading.Columns.Count Then
                        Continue For
                    End If
                    txtpos = Mid(lines(r), c, 1)
                    If iCols = iPosCol And Mid(lines(r), c, 1) = LstSpr And c <> lines.Item(r).Length Then
                        txtpos = ""
                    End If

                    If (txtpos = LstSpr Or r = sFila.Count + 1) Then
                        dtReading.Rows(r - 1).Item(iPosCol) = sColumnValue.TrimEnd
                        iPosCol = iPosCol + 1
                        sColumnValue = ""
                    Else
                        sColumnValue = sColumnValue + txtpos.Replace("'", "")
                    End If
                Next
            Next
        End Using
        Return dtReading
    End Function

    Friend Function GetTextFromPDF(PdfFileName As String) As String
        Dim oReader As New iTextSharp.text.pdf.PdfReader(PdfFileName)

        Dim sOut = ""

        For i = 1 To oReader.NumberOfPages
            Dim its As New iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy

            sOut &= iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(oReader, i, its)
        Next

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

End Module
