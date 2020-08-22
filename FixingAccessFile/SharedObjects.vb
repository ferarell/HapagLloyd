Imports System.Data
Imports System.Data.SqlClient
Imports System
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Collections
Imports Microsoft.Office.Interop

Module SharedObjects

    <System.Runtime.CompilerServices.Extension> _
    Public Function Contains(ByVal str As String, ByVal ParamArray values As String()) As Boolean
        For Each value In values
            If str.Contains(value) Then
                Return True
            End If
        Next
        Return False
    End Function

    Friend Function ExecuteAccessQuery(QueryString As String, dbFile As String) As DataSet
        Dim dsResult As New DataSet
        Dim ConnectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source='" & dbFile & "'; Persist Security Info=False;"
        Using connection As New System.Data.OleDb.OleDbConnection(ConnectionString)
            Try
                connection.Open()
                Dim Command As New System.Data.OleDb.OleDbDataAdapter(QueryString, connection)
                Command.Fill(dsResult)
            Catch ex As Exception

            Finally
                connection.Close()
            End Try
            Return dsResult
        End Using
    End Function
End Module
