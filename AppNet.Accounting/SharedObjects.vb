Imports System.Data
Imports System.Data.SqlClient
Imports System
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Collections
Imports Microsoft.Office.Interop

Module SharedObjects
    Dim oAppService As New AppService.HapagLloydServiceClient
    Friend DBFileName As String = ""
    Friend MDBFileName As String = ""
    Friend SkinName As String = ""
    Friend UserApp As String = ""

    Friend Function ExecuteSQL(ByVal QueryString As String) As DataSet
        Dim dsResult As New DataSet
        Using connection As New SqlConnection(My.Settings.DBConnectionString)
            Dim Command As New SqlCommand(QueryString, connection)
            Try
                Command.Connection.Open()
                Dim reader As SqlDataReader = Command.ExecuteReader()
                dsResult.Tables.Add()
                dsResult.Tables(0).Load(reader)
            Catch ex As Exception
                Throw
            Finally
                Command.Connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

    Friend Function LoadExcel(ByVal FileName As String, ByRef Hoja As String) As DataSet
        Dim dsResult As New DataSet
        Dim ExcelConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & FileName & "'; Extended Properties='Excel 12.0 Xml; IMEX=1'"
        Using connection As New System.Data.OleDb.OleDbConnection(ExcelConnectionString)
            Try
                connection.Open()
                If Hoja = "{0}" Then
                    For r = 0 To connection.GetSchema("Tables").Rows.Count - 1
                        If Not connection.GetSchema("Tables").Rows(r)("TABLE_NAME").ToString.ToUpper.Contains({"FILTER", "PRINT"}) Then
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

    Friend Function ExecuteAccessQuery(QueryString As String, DataReturn As Boolean) As DataSet
        MDBFileName = My.Settings.MDBDirectory & "\" & My.Settings.MDBFileName
        Dim dsResult As New DataSet
        Dim CnxStr As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" & MDBFileName & "';Jet OLEDB:Database Password=" & My.Settings.MDBPassword & ";"
        Using connection As New SqlConnection(CnxStr)
            Dim Command As New SqlCommand(QueryString, connection)
            Try
                Command.Connection.Open()
                If DataReturn Then
                    Dim reader As SqlDataReader = Command.ExecuteReader()
                    dsResult.Tables.Add()
                    dsResult.Tables(0).Load(reader)
                Else
                    Command.ExecuteNonQuery()
                End If
            Catch ex As Exception
                Throw
            Finally
                Command.Connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

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
                    If dc.DataType = GetType(Boolean) Then
                        Continue For
                    End If

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
                    If dc.DataType = GetType(Boolean) Then
                        Continue For
                    End If

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

    Friend Function FillDataTable(Table As String, Condition As String, Origin As String) As DataTable
        If Origin = "SQL" Then
            Return oAppService.ExecuteSQL("select * from " & Table & IIf(Condition = "", "", " where ") & Condition).Tables(0)
        Else
            Return ExecuteAccessQuery("select * from " & Table & IIf(Condition = "", "", " where ") & Condition).Tables(0)
        End If

    End Function

    Friend Sub ExportarExcel(sender As System.Object)
        Dim sPath As String = Path.GetTempPath
        Dim sFileName = (FileIO.FileSystem.GetTempFileName).Replace(".tmp", ".xlsx")
        sender.MainView.ExportToXlsx(sFileName)
        If IO.File.Exists(sFileName) Then
            Dim oXls As New Excel.Application 'Crea el objeto excel 
            oXls.Workbooks.Open(sFileName, , False) 'El true es para abrir el archivo en modo Solo lectura (False si lo quieres de otro modo)
            oXls.Visible = True
            oXls.WindowState = Excel.XlWindowState.xlMaximized 'Para que la ventana aparezca maximizada.
        End If
    End Sub

    Friend Function TextContain(text As String, type As String) As Boolean
        Dim bResult As Boolean = False
        If type = "MonthOfYear" Then
            If text.ToUpper.Contains("ENE ", "FEB ", "MAR ", "ABR ", "MAY ", "JUN ", "JUL ", "AGO ", "SET ", "OCT ", "NOV ", "DIC ") Then
                bResult = True
            End If
            If text.ToUpper.Contains("JAN ", "FEB ", "MAR ", "APR ", "MAY ", "JUN ", "JUL ", "AUG ", "SEP ", "OCT ", "NOV ", "DEC ") Then
                bResult = True
            End If
        End If
        If type = "OnlyNumbers" Then
            For i As Integer = 1 To text.Length
                If Mid(text, i, 1).Contains("0", "1", "2", "3", "4", "5", "6", "7", "8", "9") Then
                    bResult = True
                End If
                i = i + 1
            Next
        End If
        Return bResult
    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Function Contains(ByVal str As String, ByVal ParamArray values As String()) As Boolean
        For Each value In values
            If str.Contains(value) Then
                Return True
            End If
        Next
        Return False
    End Function

    Friend Function SelectDistinct(ByVal SourceTable As DataTable, ByVal ParamArray FieldNames() As String) As DataTable
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

        For Each Row As DataRow In SourceTable.Select("", String.Join(", ", FieldNames))
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

    Friend Function ExecuteAccessQuery(QueryString As String) As DataSet
        MDBFileName = My.Settings.MDBDirectory & "\" & My.Settings.MDBFileName
        Dim dsResult As New DataSet
        Dim ConnectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & MDBFileName & "'; Persist Security Info=False;"
        Using connection As New System.Data.OleDb.OleDbConnection(ConnectionString)
            Try
                connection.Open()
                Dim Command As New System.Data.OleDb.OleDbDataAdapter(QueryString, connection)
                Command.Fill(dsResult)
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

    Friend Function ExecuteAccessQueryWP(QueryString As String, ParamName As String, ParamValue As String) As DataSet
        MDBFileName = My.Settings.MDBDirectory & "\" & My.Settings.MDBFileName
        Dim dsResult As New DataSet
        Dim ConnectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & MDBFileName & "'; Persist Security Info=False;"
        Using connection As New System.Data.OleDb.OleDbConnection(ConnectionString)
            Try
                connection.Open()
                Dim Command As New System.Data.OleDb.OleDbDataAdapter(QueryString, connection)
                Command.SelectCommand.Parameters.AddWithValue(ParamName, ParamValue)
                Command.Fill(dsResult)
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

    Friend Function ExecuteAccessNonQuery(QueryString As String) As Boolean
        MDBFileName = My.Settings.MDBDirectory & "\" & My.Settings.MDBFileName
        Dim bResult As Boolean = True
        Dim ConnectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & MDBFileName & "'; Persist Security Info=False;"
        Using connection As New System.Data.OleDb.OleDbConnection(ConnectionString)
            Try
                connection.Open()
                Dim Command As New System.Data.OleDb.OleDbDataAdapter(QueryString, connection)
                Command.SelectCommand.ExecuteNonQuery()
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    Friend Function InsertIntoAccess(ByRef Table As String, ByVal drValues As DataRow) As Boolean
        MDBFileName = My.Settings.MDBDirectory & "\" & My.Settings.MDBFileName
        Dim drColumns As OleDb.OleDbDataReader
        Dim bResult As Boolean = True
        Dim sQuery, sColumns, sValues As String
        'Dim col As DataColumn
        Dim dtSchema As New DataTable
        Dim AccessConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & MDBFileName & "';"
        Using connection As New System.Data.OleDb.OleDbConnection(AccessConnectionString)
            Try
                connection.Open()
                Dim Command As New System.Data.OleDb.OleDbDataAdapter("select * from [" & Table & "]", connection)
                drColumns = Command.SelectCommand.ExecuteReader()
                dtSchema = drColumns.GetSchemaTable
                For Each row As DataRow In dtSchema.Rows
                    If drValues.Table.Columns.Contains(row.ItemArray(0)) Then
                        If Not IsDBNull(drValues.Item(dtSchema.Rows.IndexOf(row))) Then
                            sColumns = sColumns + IIf(dtSchema.Rows.IndexOf(row) = 0, "", ", ") & "[" & row.ItemArray(0) & "]"
                            sValues = sValues + IIf(dtSchema.Rows.IndexOf(row) = 0, "'", ", '") & drValues.Item(dtSchema.Rows.IndexOf(row)) & "'"
                        End If
                    End If
                Next
                sQuery = "insert into [" & Table & "] (" & sColumns & ") values (" & sValues & ")"
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

    Friend Function InsertIntoAccess1(ByRef Table As String, ByVal drValues As DataRow) As Boolean
        MDBFileName = My.Settings.MDBDirectory & "\" & My.Settings.MDBFileName
        Dim drColumns As OleDb.OleDbDataReader
        Dim bResult As Boolean = True
        Dim sQuery, sColumns, sValues As String
        'Dim col As DataColumn
        Dim dtSchema As New DataTable
        Dim AccessConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & MDBFileName & "';"
        Using connection As New System.Data.OleDb.OleDbConnection(AccessConnectionString)
            Try
                connection.Open()
                Dim Command As New System.Data.OleDb.OleDbDataAdapter("select * from [" & Table & "]", connection)
                drColumns = Command.SelectCommand.ExecuteReader()
                dtSchema = drColumns.GetSchemaTable
                For Each row As DataRow In dtSchema.Rows
                    sColumns = sColumns + IIf(dtSchema.Rows.IndexOf(row) = 0, "", ", ") & "[" & row.ItemArray(0) & "]"
                    sValues = sValues + IIf(dtSchema.Rows.IndexOf(row) = 0, "'", ", '") & drValues.Item(dtSchema.Rows.IndexOf(row)) & "'"
                Next
                sQuery = "insert into [" & Table & "] (" & sColumns & ") values (" & sValues & ")"
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

    Friend Function UpdateAccess(Table As String, Condition As String, SetValues As String) As Boolean
        MDBFileName = My.Settings.MDBDirectory & "\" & My.Settings.MDBFileName
        'Friend Function UpdateExcel(ByVal FileName As String, ByRef Hoja As String, Condition As String, SetValues As String) As Boolean
        Dim bResult As Boolean = True
        Dim sQuery As String
        Dim AccessConnectionString As String = "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & MDBFileName & "';"
        Using connection As New System.Data.OleDb.OleDbConnection(AccessConnectionString)
            Try
                connection.Open()
                sQuery = "UPDATE [" & Table & "] SET " & SetValues & " WHERE " & Condition
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

    Friend Function CreateTextFileWithHeaderAndDetail(ByVal fileName As String,
                                         ByVal dtHeader As DataTable,
                                         ByVal dtDetail As DataTable,
                                         ByVal hdr As Boolean,
                                         ByVal textDelimiter As Boolean) As Boolean
        If (fileName = String.Empty) OrElse
       (dtDetail Is Nothing) Then Throw New System.ArgumentException("Argumentos no válidos.")
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
            sw = New IO.StreamWriter(fileName, False, System.Text.Encoding.Default)
            If (hdr) Then
                For Each dr As DataRow In dtHeader.Rows
                    For Each dc As DataColumn In dtHeader.Columns
                        If ((dc.DataType Is System.Type.GetType("System.String")) And
                       (textDelimiter = True)) Then
                            value &= """" & dr.Item(col).ToString & """"
                        Else
                            value &= dr.Item(col).ToString
                        End If
                        col += 1
                    Next
                    sw.WriteLine(value)
                    value = String.Empty
                    col = 0
                Next
            End If

            For Each dr As DataRow In dtDetail.Rows
                For Each dc As DataColumn In dtDetail.Columns
                    If ((dc.DataType Is System.Type.GetType("System.String")) And
                   (textDelimiter = True)) Then
                        value &= """" & dr.Item(col).ToString & """"
                    Else
                        If dc.ColumnName = "C7" Then
                            value &= Strings.Right(Strings.StrDup(15, "0") & dr.Item(col).ToString & "00", 15)
                            'ElseIf dc.ColumnName = "C3" Then
                            '    value &= Space(35)
                        Else
                            value &= dr.Item(col).ToString
                        End If

                    End If
                    col += 1
                Next
                sw.WriteLine(value)
                value = String.Empty
                col = 0
            Next
            sw.Close()
            Return True

        Catch ex As Exception
            Return False

        Finally
            sw = Nothing

        End Try
    End Function

    Friend Function ValidaRUC(ByRef sRUC As String) As Boolean
        Dim bResult As Boolean = False
        Dim i001, i002, i003, i004 As Integer
        Dim s005 As String
        If sRUC.Trim.Length = 11 Then
            i001 = 5 * sRUC.Substring(0, 1) + 4 * sRUC.Substring(1, 1) + 3 * sRUC.Substring(2, 1) + 2 * sRUC.Substring(3, 1) + 7 * sRUC.Substring(4, 1) + 6 * sRUC.Substring(5, 1) + 5 * sRUC.Substring(6, 1) + 4 * sRUC.Substring(7, 1) + 3 * sRUC.Substring(8, 1) + 2 * sRUC.Substring(9, 1)
            i002 = Int(i001 / 11)
            i003 = Int(i001 - i002 * 11)
            i004 = Int(11 - i003)
            If i004 = 10 Then
                s005 = "0"
            ElseIf i004 = 11 Then
                s005 = "1"
            Else
                s005 = i004.ToString()
            End If
            If s005 = sRUC.Substring(10, 1) Then
                bResult = True
            End If
        End If
        If sRUC.Trim.Length <> 11 Then
            bResult = True
        End If
        Return bResult
    End Function

    Friend Sub TextToSpeak(sText As String)
        If My.Settings.AudioEnabled Then
            Dim t As New System.Threading.Thread(AddressOf SpeechThread)
            t.Start(sText)
        End If
    End Sub

    Private Sub SpeechThread(sText As String)
        Try
            Dim sapi
            sapi = CreateObject("sapi.spvoice")
            sapi.speak(sText)
        Catch ex As Exception
            My.Settings.AudioEnabled = False
            My.Settings.Save()
        End Try
    End Sub

    Function LastDayOfMonth(ByVal RefDate As Date) As Date
        LastDayOfMonth = DateSerial(Year(RefDate), Month(RefDate) + 1, 0)
    End Function

    Friend Function Decompress(zipPath As String, extractPath As String) As Boolean
        Dim bResult As Boolean = True
        Try
            ZipFile.ExtractToDirectory(zipPath, extractPath)
        Catch ex As Exception
            bResult = False
        End Try
        Return bResult
    End Function

    Friend Function LoadCSV(FileName As String, Header As Boolean, LstSpr As String) As DataTable
        Dim dtReading As New DataTable
        Dim sColumn As String = ""
        Dim txtpos As String = ""
        Dim iPosCol As Integer = 0
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
                For c = 1 To lines.Item(i).Length + 1
                    If iPosCol >= dtReading.Columns.Count Then
                        Continue For
                    End If
                    txtpos = Mid(lines(i), c, 1)
                    If (txtpos = LstSpr Or i = sFila.Count + 1) Then
                        dtReading.Rows(i - 1).Item(iPosCol) = sColumnValue.TrimEnd
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

    Friend Function InsertaDatosAsociados(aDatos As ArrayList) As DataRow
        Dim iPos As Integer = 0
        Dim dtQuery, dtDatosAsociados As New DataTable
        Dim dtRow As DataRow = Nothing
        Dim sColumns As String = ""
        dtQuery = ExecuteAccessQuery("select * from DatosAsociados where Sociedad='" & aDatos(1) & "' and Periodo='" & aDatos(2) & "' and AsientoContable='" & aDatos(3) & "' and NumDocRef='" & aDatos(4) & "'").Tables(0)
        sColumns = IIf(dtQuery.Rows.Count > 0, "*", "Sociedad, Periodo, AsientoContable, NumDocRef")
        dtDatosAsociados = ExecuteAccessQuery("select " & sColumns & " from DatosAsociados where Sociedad='####'").Tables(0)
        If aDatos(0) = "NC" Then
            Try
                If dtQuery.Rows.Count > 0 Then
                    dtRow = dtQuery.Rows(0)
                Else
                    dtDatosAsociados.Rows.Add()
                    iPos = dtDatosAsociados.Rows.Count - 1
                    dtDatosAsociados.Rows(iPos).Item("Sociedad") = aDatos(1)
                    dtDatosAsociados.Rows(iPos).Item("Periodo") = aDatos(2)
                    dtDatosAsociados.Rows(iPos).Item("AsientoContable") = aDatos(3)
                    dtDatosAsociados.Rows(iPos).Item("NumDocRef") = aDatos(4)
                    'dtDatosAsociados.Rows(iPos).Item("TipDocEmisor") = ""
                    'dtDatosAsociados.Rows(iPos).Item("SerDocEmisor") = ""
                    'dtDatosAsociados.Rows(iPos).Item("NumDocEmisor") = ""
                    'dtDatosAsociados.Rows(iPos).Item("FecDocEmisor") = DBNull.Value
                    InsertIntoAccess("DatosAsociados", dtDatosAsociados.Rows(iPos))
                End If
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error al insertar documento de referencia. " & ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
        Return dtRow

    End Function

    Function RemoveCharacter(ByVal stringToCleanUp)
        Dim characterToRemove As String = ""
        characterToRemove = Chr(34) + "#$%&'()*+,-./\~"
        Dim firstThree As Char() = characterToRemove.Take(16).ToArray()
        For index = 1 To firstThree.Length - 1
            stringToCleanUp = stringToCleanUp.ToString.Replace(firstThree(index), "")
        Next
        Return stringToCleanUp
    End Function

    Function ExtractOnlyNumbers(TextToExtract As String)
        Dim sResult As String = ""
        For index = 1 To TextToExtract.Length - 1
            If IsNumeric(Mid(TextToExtract, index, 1)) Then
                sResult += Mid(TextToExtract, index, 1)
            End If
        Next
        Return sResult
    End Function

    Function GetDBFileName() As String
        Dim sResult As String = ""
        If My.Settings.MDBDirectory = "" Then
            sResult = IO.Directory.GetCurrentDirectory & "\" & My.Settings.MDBFileName
        Else
            sResult = My.Settings.MDBDirectory & "\" & My.Settings.MDBFileName
        End If
        Return sResult
    End Function

End Module
