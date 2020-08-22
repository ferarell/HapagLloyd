Imports System.Windows.Forms
Imports System.IO
Imports System.Data
Imports System.Collections
Imports Microsoft.Office.Interop
Imports System.Runtime.InteropServices
Imports DevExpress.XtraGrid.Views.Grid

Module SharedModule
    Friend MDBFileName As String = My.Settings.DBFileName
    Friend dtConfig, dtQuery, dtSubjects, dtCnfgLayout As New DataTable
    Friend sException As New ArrayList
    Friend Msg As New RichTextBox
    Friend drConfig As DataRow
    Dim bIssued As Boolean = False

    Friend Function ExecuteAccessQuery(QueryString As String, DBFile As String) As DataSet
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
                'sException.Add(ex.Message)
                'DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

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
                'DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

    <System.Runtime.CompilerServices.Extension> _
    Public Function Contains(ByVal str As String, ByVal ParamArray values As String()) As Boolean
        For Each value In values
            If str.Contains(value) Then
                Return True
            End If
        Next
        Return False
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
                        'SendExceptionMessage(FileAttached, MailObject)
                    Else
                        'SendExceptionMessage("", MailObject)
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

End Module
