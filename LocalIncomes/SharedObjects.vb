﻿Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Net.Mail
Imports Microsoft.Office.Interop
Imports System.Runtime.InteropServices
Imports DevExpress.XtraGrid.Views.Grid

Module SharedObjects
    Friend AppUser As String = ""
    Friend DBFileName As String = ""
    Friend MDBFileName As String = ""
    Friend SkinName As String
    Friend LstSpr As String = Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator
    Dim oLogFileGenerate As New LogFileGenerate

    'Friend Function ExecuteSQL(ByVal QueryString As String) As DataSet
    '    Dim dsResult As New DataSet
    '    Using connection As New SqlConnection(My.Settings.DBConnectionString)
    '        Dim Command As New SqlCommand(QueryString, connection)
    '        Try
    '            Command.Connection.Open()
    '            Dim reader As SqlDataReader = Command.ExecuteReader()
    '            dsResult.Tables.Add()
    '            dsResult.Tables(0).Load(reader)
    '        Catch ex As Exception
    '            Throw
    '        Finally
    '            Command.Connection.Close()
    '        End Try
    '        Return dsResult
    '    End Using
    'End Function

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

    Friend Function QueryExcel(FileName As String, Query As String) As DataSet
        Dim dsResult As New DataSet
        'Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties=Excel 8.0;"
        Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties='Excel 8.0;HDR=No';"
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

    Friend Function LoadExcelWH(ByVal FileName As String, ByRef Hoja As String) As DataSet
        Dim dsResult As New DataSet
        Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties='Excel 12.0 Xml;HDR=No';"
        Using connection As New System.Data.OleDb.OleDbConnection(ExcelConnectionString)
            Try
                connection.Open()
                If Hoja = "{0}" Then
                    Hoja = connection.GetSchema("Tables").Rows(0)("TABLE_NAME")
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

    Friend Function InsertIntoExcel(ByVal FileName As String, ByRef Hoja As String, ByVal drValues As DataRow) As Boolean
        Dim drColumns As OleDb.OleDbDataReader
        Dim bResult As Boolean = True
        Dim sQuery, sColumns, sValues As String
        Dim col As DataColumn
        Dim dtSchema As New System.Data.DataTable
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

    Friend Function DeleteExcel(ByVal FileName As String, ByRef Hoja As String, Condition As String) As Boolean
        Dim bResult As Boolean = True
        Dim sQuery As String
        Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties=Excel 8.0;"
        Using connection As New System.Data.OleDb.OleDbConnection(ExcelConnectionString)
            Try
                connection.Open()
                sQuery = "DELETE FROM [" & Hoja & "] WHERE " & Condition
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

    Friend Function DeleteRowFromExcel(ByVal FileName As String, ByRef Hoja As String, Value As String) As Boolean
        Dim bResult As Boolean = True
        Dim xlApp As New Excel.Application
        Dim xlWorkBook As Excel.Workbook
        Dim xlWorkSheet As Excel.Worksheet
        Dim xlCell As Excel.Range
        Dim iCols As Integer = 0
        Dim iRows As Integer = 0
        Try
            'xlApp = New Excel.ApplicationClass
            xlWorkBook = xlApp.Workbooks.Open(FileName)
            xlWorkSheet = xlWorkBook.Worksheets(Hoja)
            xlCell = xlWorkSheet.UsedRange
            iCols = xlCell.Columns.Count
            iRows = xlCell.Rows.Count
            For iRowPos = 1 To iRows
                For iColPos = 1 To iCols
                    If CType(xlCell(iRowPos, iColPos).value, String) = Value Then
                        xlCell(iRowPos, iColPos).EntireRow.Delete()
                        iRowPos = iRowPos - 1
                    End If
                Next
            Next
            xlWorkBook.Save()
            xlWorkBook.Close()
            xlApp.Quit()
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            bResult = False
        End Try

        Return bResult
    End Function

    Friend Function InsertRowIntoExcel(ByVal FileName As String, ByRef Hoja As String, ByVal drValues As DataRow) As Boolean
        Dim drColumns As OleDb.OleDbDataReader
        Dim bResult As Boolean = True
        Dim sQuery, sColumns, sValues As String
        Dim col As DataColumn
        Dim dtSchema As New System.Data.DataTable
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

    Friend Function LoadCSV(ByVal FilePath As String, ByVal FileName As String) As DataSet
        Dim dsResult As New DataSet
        Dim ConnectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" & FileName & "'; Extended Properties=text; Format=Delimited;"
        Using connection As New System.Data.OleDb.OleDbConnection(ConnectionString)
            Try
                connection.Open()
                Dim Command As New System.Data.OleDb.OleDbDataAdapter("select * from [" & FileName & "]", connection)
                Command.Fill(dsResult)
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

    Friend Function CreateTextDelimiterFile(ByVal fileName As String,
                                         ByVal dt As System.Data.DataTable,
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
        If (IO.File.Exists(fileName)) Then
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
            sw = New IO.StreamWriter(fileName, False, System.Text.Encoding.Default)

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

    Friend Function CreateFormatTable() As System.Data.DataTable
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

    Friend Function FillDataTable(Sheet As String) As System.Data.DataTable
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
        If IO.File.Exists(sFileName) Then
            Dim oXls As New Excel.Application 'Crea el objeto excel 
            oXls.Workbooks.Open(sFileName, , False) 'El true es para abrir el archivo en modo Solo lectura (False si lo quieres de otro modo)
            oXls.Visible = True
            oXls.WindowState = Excel.XlWindowState.xlMaximized 'Para que la ventana aparezca maximizada.
        End If
    End Sub

    Friend Sub ExportGraphToExcel(sender As System.Object)
        Dim sPath As String = Path.GetTempPath
        Dim sFileName = FileIO.FileSystem.GetTempFileName + ".xlsx"
        sender.MainView.ExportToXlsx(sFileName)
        If IO.File.Exists(sFileName) Then
            Dim oXls As New Excel.Application 'Crea el objeto excel 
            oXls.Workbooks.Open(sFileName, , False) 'El true es para abrir el archivo en modo Solo lectura (False si lo quieres de otro modo)
            oXls.Visible = True
            oXls.WindowState = Excel.XlWindowState.xlMaximized 'Para que la ventana aparezca maximizada.
        End If
    End Sub

    <System.Runtime.CompilerServices.Extension>
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
        If month.ToUpper.Contains({"DIC", "DEC"}) Then
            sResult = "12"
        End If
        Return sResult
    End Function

    Friend Function SelectDistinct(ByVal SourceTable As System.Data.DataTable, ByVal Condition As String, ByVal ParamArray FieldNames() As String) As System.Data.DataTable
        Dim lastValues() As Object
        Dim newTable As System.Data.DataTable

        If FieldNames Is Nothing OrElse FieldNames.Length = 0 Then
            Throw New ArgumentNullException("FieldNames")
        End If

        lastValues = New Object(FieldNames.Length - 1) {}
        newTable = New System.Data.DataTable

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
    '        mail.To.Add(My.Settings.MailTo)
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

    Private Sub MessageGenerate()
        Dim message As Microsoft.Office.Interop.Outlook.MailItem
        message.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML
        message.Attachments.Add("Templates\MessageC1.msg")
    End Sub

    Friend Function TextContain(text As String, type As String) As Boolean
        Dim bResult As Boolean = True
        'text = text.Trim
        Try
            If type = "MonthOfYear" Then
                If Not text.ToUpper.Contains("ENE ", "FEB ", "MAR ", "ABR ", "MAY ", "JUN ", "JUL ", "AGO ", "SET ", "OCT ", "NOV ", "DIC ", "JAN ", "APR ", "AUG ", "SEP ", "DEC") Then
                    bResult = False
                    'Else
                    '    bResult = True
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
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return bResult
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

    Friend Function ExecuteAccessQuery(QueryString As String, DBFile As String) As DataSet
        Dim oAccessDB As String = ""
        If DBFile = "" Then
            oAccessDB = My.Settings.DBFileName
        Else
            oAccessDB = Path.GetDirectoryName(My.Settings.DBFileName) & "\" & DBFile
        End If
        Dim dsResult As New DataSet
        Dim ConnectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & oAccessDB & "';User ID=Admin;Password=;"
        Using connection As New System.Data.OleDb.OleDbConnection(ConnectionString)
            Try
                connection.Open()
                Dim Command As New System.Data.OleDb.OleDbDataAdapter(QueryString, connection)
                Command.Fill(dsResult)
            Catch ex As Exception
                oLogFileGenerate.TextFileUpdate("FLETESONLINE", "Función:ExecuteAccessQuery / DataBase:" & oAccessDB & " / QueryString:" & QueryString & " / Error:" & ex.Message)
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

    Friend Function ExecuteAccessNonQuery(QueryString As String, DBFile As String) As Boolean
        Dim oAccessDB As String = ""
        If DBFile = "" Then
            oAccessDB = My.Settings.DBFileName
        Else
            oAccessDB = Path.GetDirectoryName(My.Settings.DBFileName) & "\" & DBFile
        End If
        Dim bResult As Boolean = True
        Dim ConnectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & oAccessDB & "'; Persist Security Info=False;"
        Using connection As New System.Data.OleDb.OleDbConnection(ConnectionString)
            Try
                connection.Open()
                Dim Command As New System.Data.OleDb.OleDbDataAdapter(QueryString, connection)
                Command.SelectCommand.ExecuteNonQuery()
            Catch ex As Exception
                oLogFileGenerate.TextFileUpdate("FLETESONLINE", "Función:ExecuteAccessNonQuery / DataBase:" & oAccessDB & " / QueryString:" & QueryString & " / Error:" & ex.Message)
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
        'MailObject.Add(My.Settings.SupportMailAddress) 'Mail TO
        'MailObject.Add(My.Settings.CCMailAddress) 'Mail CC
        'MailObject.Add(My.Settings.BCCMailAddress) 'Mail BCC
        'mailItem.To = My.Settings.SupportMailAddress
        'mailItem.CC = ""
        'mailItem.BCC = ""
        Dim dtSchema As New DataTable
        Dim oAccessDB As String = ""
        If DBFile = "" Then
            oAccessDB = My.Settings.DBFileName
        Else
            oAccessDB = Path.GetDirectoryName(My.Settings.DBFileName) & "\" & DBFile
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
                oLogFileGenerate.TextFileUpdate("FLETESONLINE", "Función:InsertIntoAccess / DataBase:" & oAccessDB & " / Table:" & Table & " / Error:" & ex.Message)
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                'If Not mailItem Is Nothing Then
                '    sException.Add(sQuery)
                '    sException.Add(ex.Message)
                '    MailObject.Add(mailItem.Subject & " (PROCESS WITH ERROR)")
                '    MailObject.Add("Error: " & ex.Message & "<br><br>" & "Query: " & sQuery)
                '    Dim sFileName As String = ""
                '    If mailItem.Attachments.Count > 0 Then
                '        SendExceptionMessage(FileAttached, MailObject)
                '    Else
                '        SendExceptionMessage("", MailObject)
                '    End If
                'End If
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
            oAccessDB = My.Settings.DBFileName
        Else
            oAccessDB = Path.GetDirectoryName(My.Settings.DBFileName) & "\" & DBFile
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
                oLogFileGenerate.TextFileUpdate("FLETESONLINE", "Función:UpdateAccess / DataBase:" & oAccessDB & " / Table:" & Table & " / Error:" & ex.Message)
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                bResult = False
            Finally
                connection.Close()
            End Try
            Return bResult
        End Using
    End Function


    'Friend Sub CreateSendItem(Subject As String, Body As String, Origin As String, AttachFile As String)
    '    Dim Application As New Outlook.Application
    '    Dim mail As Outlook.MailItem = Nothing
    '    Dim mailRecipients As Outlook.Recipients = Nothing
    '    Dim mailRecipient As Outlook.Recipient = Nothing
    '    Try
    '        mail = Application.CreateItem(Outlook.OlItemType.olMailItem)
    '        mail.Subject = Subject
    '        mail.HTMLBody = Body
    '        If AttachFile <> "" Then
    '            mail.Attachments.Add(AttachFile)
    '        End If
    '        If Origin = "RS" Then
    '            'mail.To = "ferarell@hotmail.com;aremonfe@gmail.com" 'My.Settings.MailRecipients
    '            'mail.CC = "ferarell@yahoo.com" '"perudownload@hlag.com"
    '        ElseIf Origin = "CT" Then
    '            mailRecipients = mail.Recipients
    '            mailRecipient = mailRecipients.Add("Cesar.carranza@hlag.com")
    '            mailRecipient = mailRecipients.Add("Veronica.Portella@hlag.com")
    '            mailRecipient = mailRecipients.Add("Claudia.Pinillos@hlag.com")
    '            mail.CC = "perudownload@hlag.com"
    '            'mailRecipient.Resolve()
    '        End If
    '        mail.Display()
    '        'If (mailRecipient.Resolved) Then
    '        '    mail.Display()
    '        'Else
    '        '    System.Windows.Forms.MessageBox.Show(
    '        '        "There is no such record in your address book.")
    '        'End If
    '    Catch ex As Exception
    '        System.Windows.Forms.MessageBox.Show(ex.Message,
    '            "An exception is occured in the code of add-in.")
    '    Finally
    '        If Not IsNothing(mailRecipient) Then Marshal.ReleaseComObject(mailRecipient)
    '        If Not IsNothing(mailRecipients) Then Marshal.ReleaseComObject(mailRecipients)
    '        If Not IsNothing(mail) Then Marshal.ReleaseComObject(mail)
    '    End Try

    'End Sub

    'Friend Sub ExportToImage(sender As System.Object, ImageFile As String, Format As Imaging.ImageFormat)
    '    Dim oChartControl As New DevExpress.XtraCharts.ChartControl
    '    oChartControl = sender
    '    Dim sFile As String = Path.GetTempPath & ImageFile
    '    oChartControl.ExportToImage(sFile, Format)
    '    If IO.File.Exists(sFile) Then
    '        Process.Start(sFile)
    '    End If
    'End Sub

End Module
