Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Collections.Generic

Public Class DataAccessTest

    Public Function ExecuteSQL(ByVal QueryString As String) As DataSet
        Dim dsResult As New DataSet
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolutionTest"))
            Dim Command As New SqlCommand(QueryString, connection)
            Dim Adapter As New SqlDataAdapter
            Try
                Command.CommandText = QueryString
                Command.CommandTimeout = 60000
                Adapter.SelectCommand = Command
                Command.Connection.Open()
                Adapter.Fill(dsResult)
            Catch ex As Exception
                dsResult.Tables.Add("Error").Columns.Add("Exception", GetType(String)).DefaultValue = ""
                dsResult.Tables("Error").Rows.Add(ex.Message)
            Finally
                Command.Connection.Close()
            End Try
            Return dsResult
        End Using
    End Function

    Public Function ExecuteSQLNonQuery(ByVal QueryString As String) As ArrayList
        Dim aResult As New ArrayList
        aResult.AddRange({True, ""})
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolutionTest"))
            Dim Command As New SqlCommand(QueryString, connection)
            Try
                Command.Connection.Open()
                Command.ExecuteNonQuery()
            Catch ex As Exception
                aResult(0) = False
                aResult(1) = ex.Message
            Finally
                Command.Connection.Close()
            End Try
            Return aResult
        End Using
    End Function


End Class
