' NOTA: puede usar el comando "Cambiar nombre" del menú contextual para cambiar el nombre de clase "HapagLloydService" en el código, en svc y en el archivo de configuración a la vez.
' NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione HapagLloydService.svc o HapagLloydService.svc.vb en el Explorador de soluciones e inicie la depuración.
Imports System.Data
Imports System.Data.SqlClient
Imports System.Net

Public Class HapagLloydService
    Implements IHapagLloydService

    Public Function ExecuteSQL(ByVal QueryString As String) As DataSet Implements IHapagLloydService.ExecuteSQL
        Dim dsResult As New DataSet
        Dim oDataAccess As New DataAccess
        dsResult = oDataAccess.ExecuteSQL(QueryString)
        Return dsResult
    End Function

    Public Function ExecuteSQLNonQuery(ByVal QueryString As String) As ArrayList Implements IHapagLloydService.ExecuteSQLNonQuery
        Dim aResult As New ArrayList
        Try
            Dim oDataAccess As New DataAccess
            aResult.AddRange(oDataAccess.ExecuteSQLNonQuery(QueryString))
        Catch ex As Exception
            aResult(1) = ex.Message
        End Try
        Return aResult
    End Function

    Public Function UpdateTableWithBulkCopy(ByVal Table As String, ByVal dtSource As DataTable, ByVal ProcessType As String) As ArrayList Implements IHapagLloydService.UpdateTableWithBulkCopy
        Dim aResult As New ArrayList
        Dim sNroVoucher As String = ""
        aResult.AddRange({1, ""})
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction("UpdateTableWithBulkCopy")
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                If ProcessType = "R" Then 'Replace All Data
                    Command.CommandText = "TRUNCATE TABLE " & Table
                    Command.ExecuteNonQuery()
                End If
                Dim Bulk As New SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction)
                Bulk.DestinationTableName = Table
                For Each col As DataColumn In dtSource.Columns
                    Bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName)
                Next
                Bulk.WriteToServer(dtSource)
                transaction.Commit()
            Catch ex As Exception
                aResult(0) = 0
                aResult(1) = ex.Message
                transaction.Rollback()
            Finally
                connection.Close()
            End Try
            Return aResult
        End Using
        Return aResult
    End Function

    Public Function UpdatingUsingTableAsParameter(ByVal StoreProcedure As String, ByVal Params As ArrayList, ByVal Values As ArrayList, ByVal dtSource As DataTable) As ArrayList Implements IHapagLloydService.UpdatingUsingTableAsParameter
        Dim aResult As New ArrayList
        Dim sNroVoucher As String = ""
        aResult.AddRange({1, ""})
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction("UpdatingUsingTableAsParameter")
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                Command.CommandType = CommandType.StoredProcedure
                Command.CommandText = StoreProcedure
                With Command.Parameters
                    .Clear()
                    .AddWithValue("@TableVar", dtSource)
                    If Params IsNot Nothing Then
                        If Params.Count > 0 Then
                            For p = 0 To Params.Count - 1
                                .AddWithValue(Params.Item(p), Values.Item(p))
                            Next
                        End If
                    End If
                End With
                Command.CommandTimeout = 60000
                If Command.ExecuteNonQuery() <= 0 Then
                    transaction.Commit()
                    aResult(0) = 1
                Else
                    transaction.Rollback()
                End If
            Catch ex As Exception
                aResult(0) = 0
                aResult(1) = ex.Message
                transaction.Rollback()
            Finally
                connection.Close()
            End Try
            Return aResult
        End Using
        Return aResult
    End Function

    Public Function CustomStoredProcedureExecution(ByVal StoreProcedure As String, ByVal ValueList As ArrayList, ByVal dtSource As DataTable) As ArrayList Implements IHapagLloydService.CustomStoredProcedureExecution
        Dim aResult As New ArrayList
        Dim sNroVoucher As String = ""
        aResult.AddRange({1, ""})
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction("UpdatingUsingTableAsParameter")
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                Command.CommandType = CommandType.StoredProcedure
                Command.CommandText = StoreProcedure
                With Command.Parameters
                    .Clear()
                    .AddWithValue("@TableVar", dtSource)
                    If ValueList.Count > 0 Then
                        For p = 0 To ValueList.Count - 1
                            .AddWithValue(ValueList.Item(p)(0), ValueList.Item(p)(1))
                        Next
                    End If
                End With
                Command.CommandTimeout = 60000
                If Command.ExecuteNonQuery() <= 0 Then
                    transaction.Commit()
                    aResult(0) = 1
                Else
                    transaction.Rollback()
                End If
            Catch ex As Exception
                aResult(0) = 0
                aResult(1) = ex.Message
                transaction.Rollback()
            Finally
                connection.Close()
            End Try
            Return aResult
        End Using
        Return aResult
    End Function

#Region "Notificador"

    Public Function InsertContacts(ByVal aSource As ArrayList) As Boolean Implements IHapagLloydService.InsertContacts
        Dim bResult As Boolean = True
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                Command.CommandType = CommandType.StoredProcedure
                Command.CommandText = "ntf.upContacts_Insert"
                With Command.Parameters
                    .Clear()
                    .Add("@CountryCode", SqlDbType.Char, 2).Value = aSource(0)
                    .Add("@Regime", SqlDbType.VarChar, 1).Value = aSource(1)
                    .Add("@MatchCode", SqlDbType.VarChar, 50).Value = aSource(2)
                    .Add("@eMail", SqlDbType.VarChar, 255).Value = aSource(3)
                    .Add("@Status", SqlDbType.Char, 1).Value = aSource(4)
                    .Add("@CreatedBy", SqlDbType.VarChar, 100).Value = aSource(5)
                    .Add("@CreatedDate", SqlDbType.DateTime).Value = aSource(6)
                End With
                Command.CommandTimeout = 60000
                If Command.ExecuteNonQuery() <= 0 Then
                    transaction.Commit()
                Else
                    bResult = False
                    transaction.Rollback()
                End If
            Catch ex As Exception
                bResult = False
                transaction.Rollback()
            Finally
                Command.Connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    Public Function InsertBlackList(ByVal aSource As ArrayList) As Boolean Implements IHapagLloydService.InsertBlackList
        Dim bResult As Boolean = True
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                Command.CommandType = CommandType.StoredProcedure
                Command.CommandText = "ntf.upBlackList_Insert"
                With Command.Parameters
                    .Clear()
                    .Add("@CountryCode", SqlDbType.Char, 2).Value = aSource(0)
                    .Add("@Regime", SqlDbType.VarChar, 1).Value = aSource(1)
                    .Add("@MatchCode", SqlDbType.VarChar, 50).Value = aSource(2)
                    .Add("@CreatedBy", SqlDbType.VarChar, 100).Value = aSource(3)
                    .Add("@CreatedDate", SqlDbType.DateTime).Value = aSource(4)
                End With
                Command.CommandTimeout = 60000
                If Command.ExecuteNonQuery() <= 0 Then
                    transaction.Commit()
                Else
                    bResult = False
                    transaction.Rollback()
                End If
            Catch ex As Exception
                bResult = False
                transaction.Rollback()
            Finally
                Command.Connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    Public Function InsertPartners(ByVal dtSource As DataTable) As Boolean Implements IHapagLloydService.InsertPartners
        Dim bResult As Boolean = True
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                Command.CommandType = CommandType.StoredProcedure
                Command.CommandText = "ntf.upPartners_Insert"
                With Command.Parameters
                    .Clear()
                    .Add("@PartnerType", SqlDbType.Char, 1).Value = dtSource.Rows(0)("PartnerType")
                    .Add("@PartnerCode", SqlDbType.VarChar, 50).Value = dtSource.Rows(0)("PartnerCode")
                    .Add("@PartnerName", SqlDbType.VarChar, 150).Value = dtSource.Rows(0)("PartnerName")
                    .Add("@MatchCode", SqlDbType.VarChar, 50).Value = dtSource.Rows(0)("MatchCode")
                    .Add("@TaxNumber", SqlDbType.VarChar, 20).Value = dtSource.Rows(0)("TaxNumber")
                    .Add("@CreatedBy", SqlDbType.VarChar, 100).Value = dtSource.Rows(0)("CreatedBy")
                    .Add("@CreatedDate", SqlDbType.DateTime).Value = dtSource.Rows(0)("CreatedDate")
                End With
                Command.CommandTimeout = 60000
                If Command.ExecuteNonQuery() <= 0 Then
                    transaction.Commit()
                Else
                    bResult = False
                    transaction.Rollback()
                End If
            Catch ex As Exception
                bResult = False
                transaction.Rollback()
            Finally
                Command.Connection.Close()
            End Try
            Return bResult
        End Using
    End Function

#End Region

#Region "T-CHECK"

    Public Function InserColdTreatment(ByVal dtSource As DataTable) As Boolean Implements IHapagLloydService.InsertColdTreatment
        Dim bResult As Boolean = True
        Dim aSource As DataRow = dtSource.Rows(0)
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                Command.CommandType = CommandType.StoredProcedure
                Command.CommandText = "tck.upColdTreatment_Insert"
                With Command.Parameters
                    .Clear()
                    .Add("@CONTAINER", SqlDbType.VarChar, 20).Value = aSource(0)
                    .Add("@BOOKING", SqlDbType.VarChar, 20).Value = aSource(1)
                    .Add("@CGODESC", SqlDbType.NVarChar, 255).Value = aSource(2)
                    .Add("@TEMPERATURE", SqlDbType.Decimal, 18, 2).Value = aSource(3)
                    .Add("@POL", SqlDbType.VarChar, 5).Value = aSource(4)
                    .Add("@CHKDL", SqlDbType.VarChar, 30).Value = aSource(5)
                    'If aSource(6) <> DBNull.Value Then
                    '    .Add("@INIDATE", SqlDbType.DateTime).Value = aSource(6)
                    'End If
                    .Add("@POD", SqlDbType.VarChar, 5).Value = aSource(7)
                    .Add("@FDP", SqlDbType.VarChar, 5).Value = aSource(8)
                    .Add("@EXPORT_PARTY", SqlDbType.VarChar, 20).Value = aSource(9)
                    .Add("@ROUTING_PARTY", SqlDbType.VarChar, 20).Value = aSource(10)
                    .Add("@DEPOT", SqlDbType.VarChar, 20).Value = aSource(11)
                    .Add("@VESSEL", SqlDbType.VarChar, 100).Value = aSource(12)
                    .Add("@SERVICE", SqlDbType.VarChar, 10).Value = aSource(13)
                    .Add("@ETA1", SqlDbType.DateTime).Value = aSource(14)
                    .Add("@TSP", SqlDbType.VarChar, 5).Value = aSource(15)
                    'If aSource(16) <> DBNull.Value Then
                    '    .Add("@ETA2", SqlDbType.DateTime).Value = aSource(16)
                    '    .Add("@ETD2", SqlDbType.DateTime).Value = aSource(17)
                    'End If
                    .Add("@TSCHKDL", SqlDbType.VarChar, 30).Value = aSource(18)
                    'If aSource(19) <> DBNull.Value Then
                    '    .Add("@FINDATE", SqlDbType.DateTime).Value = aSource(19)
                    'End If
                    'If aSource(20) <> DBNull.Value Then
                    '    .Add("@CTDAYS", SqlDbType.SmallInt).Value = aSource(20)
                    'End If
                    .Add("@REMARKS", SqlDbType.NText).Value = aSource(21)
                    .Add("@SENASA", SqlDbType.VarChar, 30).Value = aSource(22)
                    .Add("@SHARED", SqlDbType.VarChar, 1).Value = aSource(23)
                    '.Add("@COMMENTS", SqlDbType.NText).Value = aSource(24)
                    '.Add("@OPS_CODE", SqlDbType.VarChar, 10).Value = aSource(25)
                    .Add("@CreatedBy", SqlDbType.VarChar, 120).Value = aSource(26)
                    .Add("@CreatedDate", SqlDbType.DateTime).Value = aSource(27)

                End With
                Command.CommandTimeout = 60000
                If Command.ExecuteNonQuery() <= 0 Then
                    transaction.Commit()
                Else
                    bResult = False
                    transaction.Rollback()
                End If
            Catch ex As Exception
                bResult = False
                transaction.Rollback()
            Finally
                Command.Connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    Public Function UpdateColdTreatment(ByVal dtSource As DataTable) As Boolean Implements IHapagLloydService.UpdateColdTreatment
        Dim bResult As Boolean = True
        Dim aSource As DataRow = dtSource.Rows(0)
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                Command.CommandType = CommandType.StoredProcedure
                Command.CommandText = "tck.upColdTreatment_Update"
                With Command.Parameters
                    .Clear()
                    .Add("@CONTAINER", SqlDbType.VarChar, 20).Value = aSource(0)
                    .Add("@BOOKING", SqlDbType.VarChar, 20).Value = aSource(1)
                    .Add("@CGODESC", SqlDbType.NVarChar, 255).Value = aSource(2)
                    .Add("@TEMPERATURE", SqlDbType.Decimal, 18, 2).Value = aSource(3)
                    .Add("@POL", SqlDbType.VarChar, 5).Value = aSource(4)
                    .Add("@CHKDL", SqlDbType.VarChar, 30).Value = aSource(5)
                    .Add("@INIDATE", SqlDbType.DateTime).Value = aSource(6)
                    .Add("@POD", SqlDbType.VarChar, 5).Value = aSource(7)
                    .Add("@FDP", SqlDbType.VarChar, 5).Value = aSource(8)
                    .Add("@EXPORT_PARTY", SqlDbType.VarChar, 20).Value = aSource(9)
                    .Add("@ROUTING_PARTY", SqlDbType.VarChar, 20).Value = aSource(10)
                    .Add("@DEPOT", SqlDbType.VarChar, 20).Value = aSource(11)
                    .Add("@VESSEL", SqlDbType.VarChar, 100).Value = aSource(12)
                    .Add("@SERVICE", SqlDbType.VarChar, 10).Value = aSource(13)
                    .Add("@ETA1", SqlDbType.DateTime).Value = aSource(14)
                    .Add("@TSP", SqlDbType.VarChar, 5).Value = aSource(15)
                    .Add("@ETA2", SqlDbType.DateTime).Value = aSource(16)
                    .Add("@ETD2", SqlDbType.DateTime).Value = aSource(17)
                    .Add("@TSCHKDL", SqlDbType.VarChar, 30).Value = aSource(18)
                    .Add("@FINDATE", SqlDbType.DateTime).Value = aSource(19)
                    .Add("@CTDAYS", SqlDbType.SmallInt).Value = aSource(20)
                    .Add("@REMARKS", SqlDbType.NText).Value = aSource(21)
                    .Add("@SENASA", SqlDbType.VarChar, 30).Value = aSource(22)
                    .Add("@SHARED", SqlDbType.VarChar, 1).Value = aSource(23)
                    .Add("@COMMENTS", SqlDbType.NText).Value = aSource(24)
                    .Add("@OPS_CODE", SqlDbType.VarChar, 10).Value = aSource(25)
                    .Add("@UpdatedBy", SqlDbType.VarChar, 120).Value = aSource(26)
                    .Add("@UpdatedDate", SqlDbType.DateTime).Value = aSource(27)

                End With
                Command.CommandTimeout = 60000
                If Command.ExecuteNonQuery() <= 0 Then
                    transaction.Commit()
                Else
                    bResult = False
                    transaction.Rollback()
                End If
            Catch ex As Exception
                bResult = False
                transaction.Rollback()
            Finally
                Command.Connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    Public Function DeleteColdTreatment(ByVal aSource As ArrayList) As Boolean Implements IHapagLloydService.DeleteColdTreatment
        Dim bResult As Boolean = True
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                Command.CommandType = CommandType.StoredProcedure
                Command.CommandText = "tck.upColdTreatment_Delete"
                With Command.Parameters
                    .Clear()
                    .Add("@CONTAINER", SqlDbType.VarChar, 20).Value = aSource(0)
                    .Add("@BOOKING", SqlDbType.VarChar, 20).Value = aSource(1)
                End With
                Command.CommandTimeout = 60000
                If Command.ExecuteNonQuery() <= 0 Then
                    transaction.Commit()
                Else
                    bResult = False
                    transaction.Rollback()
                End If
            Catch ex As Exception
                bResult = False
                transaction.Rollback()
            Finally
                Command.Connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    Public Function InsertColdTreatmentEvents(ByVal aSource As ArrayList) As Boolean Implements IHapagLloydService.InsertColdTreatmentEvents
        Dim bResult As Boolean = True
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                Command.CommandType = CommandType.StoredProcedure
                Command.CommandText = "tck.upColdTreatmentEvents_Insert"
                With Command.Parameters
                    .Clear()
                    .Add("@BOOKING", SqlDbType.VarChar, 20).Value = aSource(0)
                    .Add("@CONTAINER", SqlDbType.VarChar, 20).Value = aSource(1)
                    .Add("@DESCRIPTION", SqlDbType.NText).Value = aSource(2)
                End With
                Command.CommandTimeout = 60000
                If Command.ExecuteNonQuery() <= 0 Then
                    transaction.Commit()
                Else
                    bResult = False
                    transaction.Rollback()
                End If
            Catch ex As Exception
                bResult = False
                transaction.Rollback()
            Finally
                Command.Connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    Public Function DeleteColdTreatmentEvents(ByVal aSource As ArrayList) As Boolean Implements IHapagLloydService.DeleteColdTreatmentEvents
        Dim bResult As Boolean = True
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                Command.CommandType = CommandType.StoredProcedure
                Command.CommandText = "tck.upColdTreatmentEvents_Delete"
                With Command.Parameters
                    .Clear()
                    .Add("@CONTAINER", SqlDbType.VarChar, 20).Value = aSource(0)
                    .Add("@BOOKING", SqlDbType.VarChar, 20).Value = aSource(1)
                End With
                Command.CommandTimeout = 60000
                If Command.ExecuteNonQuery() <= 0 Then
                    transaction.Commit()
                Else
                    bResult = False
                    transaction.Rollback()
                End If
            Catch ex As Exception
                bResult = False
                transaction.Rollback()
            Finally
                Command.Connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    Public Function InsertColdTreatmentReadings(ByVal aSource As ArrayList) As Boolean Implements IHapagLloydService.InsertColdTreatmentReadings
        Dim bResult As Boolean = True
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                Command.CommandType = CommandType.StoredProcedure
                Command.CommandText = "tck.upColdTreatmentReadings_Insert"
                With Command.Parameters
                    .Clear()
                    .Add("@BOOKING", SqlDbType.VarChar, 20).Value = aSource(0)
                    .Add("@CONTAINER", SqlDbType.VarChar, 20).Value = aSource(1)
                    .Add("@CT_DATE", SqlDbType.DateTime).Value = aSource(2)
                    .Add("@CT_TIME", SqlDbType.VarChar, 10).Value = aSource(3)
                    .Add("@CT_USDA1", SqlDbType.Float).Value = aSource(4)
                    .Add("@CT_USDA2", SqlDbType.Float).Value = aSource(5)
                    .Add("@CT_USDA3", SqlDbType.Float).Value = aSource(6)
                End With
                Command.CommandTimeout = 60000
                If Command.ExecuteNonQuery() <= 0 Then
                    transaction.Commit()
                Else
                    bResult = False
                    transaction.Rollback()
                End If
            Catch ex As Exception
                bResult = False
                transaction.Rollback()
            Finally
                Command.Connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    'Public Function InsertColdTreatmentReadingsByTable(ByVal dtSource As DataTable) As Boolean Implements IHapagLloydService.InsertColdTreatmentReadingsByTable
    '    Dim bResult As Boolean = True
    '    Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
    '        connection.Open()
    '        Dim Command As New SqlCommand
    '        Dim transaction As SqlTransaction
    '        transaction = connection.BeginTransaction
    '        Command.Connection = connection
    '        Command.Transaction = transaction
    '        Try
    '            Command.CommandType = CommandType.StoredProcedure
    '            Command.CommandText = "tck.upColdTreatmentReadings_Insert"
    '            With Command.Parameters
    '                .Clear()
    '                .Add("@BOOKING", SqlDbType.VarChar, 20).Value = aSource(0)
    '                .Add("@CONTAINER", SqlDbType.VarChar, 20).Value = aSource(1)
    '                .Add("@CT_DATE", SqlDbType.DateTime).Value = aSource(2)
    '                .Add("@CT_TIME", SqlDbType.VarChar, 10).Value = aSource(3)
    '                .Add("@CT_USDA1", SqlDbType.Float).Value = aSource(4)
    '                .Add("@CT_USDA2", SqlDbType.Float).Value = aSource(5)
    '                .Add("@CT_USDA3", SqlDbType.Float).Value = aSource(6)
    '            End With
    '            Command.CommandTimeout = 60000
    '            If Command.ExecuteNonQuery() <= 0 Then
    '                transaction.Commit()
    '            Else
    '                bResult = False
    '                transaction.Rollback()
    '            End If
    '        Catch ex As Exception
    '            bResult = False
    '            transaction.Rollback()
    '        Finally
    '            Command.Connection.Close()
    '        End Try
    '        Return bResult
    '    End Using
    'End Function

    Public Function DeleteColdTreatmentReadings(ByVal aSource As ArrayList) As Boolean Implements IHapagLloydService.DeleteColdTreatmentReadings
        Dim bResult As Boolean = True
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                Command.CommandType = CommandType.StoredProcedure
                Command.CommandText = "tck.upColdTreatmentReadings_Delete"
                With Command.Parameters
                    .Clear()
                    .Add("@CONTAINER", SqlDbType.VarChar, 20).Value = aSource(0)
                    .Add("@BOOKING", SqlDbType.VarChar, 20).Value = aSource(1)
                End With
                Command.CommandTimeout = 60000
                If Command.ExecuteNonQuery() <= 0 Then
                    transaction.Commit()
                Else
                    bResult = False
                    transaction.Rollback()
                End If
            Catch ex As Exception
                bResult = False
                transaction.Rollback()
            Finally
                Command.Connection.Close()
            End Try
            Return bResult
        End Using
    End Function

    Public Function InsertScheduleVoyage(ByVal dtSource As DataTable) As Boolean Implements IHapagLloydService.InsertScheduleVoyage
        Dim bResult As Boolean = True
        Dim aSource As DataRow = dtSource.Rows(0)
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                Command.CommandType = CommandType.StoredProcedure
                Command.CommandText = "tck.upScheduleVoyage_Insert"
                With Command.Parameters
                    .Clear()
                    .Add("@POL", SqlDbType.VarChar, 5).Value = aSource(0)
                    .Add("@DPVOYAGE", SqlDbType.VarChar, 10).Value = aSource(1)
                    .Add("@VESSEL_NAME", SqlDbType.NVarChar, 100).Value = aSource(2)
                    .Add("@SCHEDULE", SqlDbType.VarChar, 10).Value = aSource(3)
                    .Add("@SERVICE", SqlDbType.VarChar, 10).Value = aSource(4)
                    .Add("@DOC_CLOSE", SqlDbType.DateTime).Value = aSource(5)
                    .Add("@ETA", SqlDbType.DateTime).Value = aSource(6)
                    .Add("@ETD", SqlDbType.DateTime).Value = aSource(7)
                    .Add("@CreatedBy", SqlDbType.VarChar, 120).Value = aSource(8)
                    .Add("@CreatedDate", SqlDbType.DateTime).Value = aSource(9)

                End With
                Command.CommandTimeout = 60000
                If Command.ExecuteNonQuery() <= 0 Then
                    transaction.Commit()
                Else
                    bResult = False
                    transaction.Rollback()
                End If
            Catch ex As Exception
                bResult = False
                transaction.Rollback()
            Finally
                Command.Connection.Close()
            End Try
            Return bResult
        End Using
    End Function

#End Region

    Public Function GetLocalChargeInvoicing(DateFrom As DateTime, DateTo As DateTime, Country As String) As DataTable Implements IHapagLloydService.GetLocalChargeInvoicing
        Dim dtResult As New DataTable
        Try
            dtResult = ExecuteSQL("ctr.upGetLocalChargeInvoicing '" & Format(DateFrom, "yyyy-MM-dd") & "', '" & Format(DateTo, "yyyy-MM-dd") & "', '" & Country & "'").Tables(0)
        Catch ex As Exception
            Throw New System.Exception(ex.Message)
        End Try

        Return dtResult
    End Function

#Region "REEFER SALES"
    Public Function InsertReeferDataMaster(ByVal dtSource As DataTable) As Boolean Implements IHapagLloydService.InsertReeferDataMaster
        Dim bResult As Boolean = True
        Dim aSource As DataRow = dtSource.Rows(0)
        Using connection As New SqlConnection(ConfigurationManager.AppSettings("dbSolution"))
            connection.Open()
            Dim Command As New SqlCommand
            Dim transaction As SqlTransaction
            transaction = connection.BeginTransaction
            Command.Connection = connection
            Command.Transaction = transaction
            Try
                Command.CommandType = CommandType.StoredProcedure
                Command.CommandText = "tck.upReeferDataMaster_Insert"
                With Command.Parameters
                    .Clear()
                    .Add("@Booking", SqlDbType.VarChar, 20).Value = aSource(0)
                    .Add("@Container", SqlDbType.VarChar, 20).Value = aSource(1)
                    .Add("@EqpType", SqlDbType.VarChar, 10).Value = aSource(2)
                    .Add("@MainType", SqlDbType.VarChar, 2).Value = aSource(3)
                    .Add("@SpecialProduct", SqlDbType.VarChar, 20).Value = aSource(4)
                    .Add("@IsColdTreatment", SqlDbType.Bit).Value = aSource(5)
                    .Add("@ShipperMR", SqlDbType.VarChar, 10).Value = aSource(6)
                    .Add("@ShipperBL", SqlDbType.VarChar, 20).Value = aSource(7)
                    .Add("@CommodityDescription", SqlDbType.NVarChar, 255).Value = aSource(8)
                    .Add("@GPS", SqlDbType.VarChar, 10).Value = aSource(9)
                    .Add("@POL", SqlDbType.VarChar, 5).Value = aSource(10)
                    .Add("@Departure1", SqlDbType.DateTime).Value = aSource(11)
                    .Add("@Notify1", SqlDbType.Bit).Value = aSource(12)
                    .Add("@DPVoyage1", SqlDbType.VarChar, 10).Value = aSource(13)
                    .Add("@VesselName1", SqlDbType.VarChar, 100).Value = aSource(14)
                    .Add("@VesselVoyage1", SqlDbType.VarChar, 10).Value = aSource(15)
                    .Add("@Service", SqlDbType.VarChar, 10).Value = aSource(16)
                    .Add("@ArrivalTSP", SqlDbType.DateTime).Value = aSource(17)
                    .Add("@Notify2", SqlDbType.Bit).Value = aSource(18)
                    .Add("@TSP", SqlDbType.VarChar, 5).Value = aSource(19)
                    .Add("@Departure2", SqlDbType.DateTime).Value = aSource(20)
                    .Add("@DPVoyage2", SqlDbType.VarChar, 10).Value = aSource(21)
                    .Add("@VesselName2", SqlDbType.VarChar, 100).Value = aSource(22)
                    .Add("@VesselVoyage2", SqlDbType.VarChar, 10).Value = aSource(23)
                    .Add("@ArrivalPOD", SqlDbType.DateTime).Value = aSource(24)
                    .Add("@Notify3", SqlDbType.Bit).Value = aSource(25)
                    .Add("@POD", SqlDbType.VarChar, 5).Value = aSource(25)
                    .Add("@TransitDays", SqlDbType.SmallInt).Value = aSource(25)
                    .Add("@Comments", SqlDbType.NVarChar, 255).Value = aSource(25)
                    .Add("@ElapsedTime", SqlDbType.SmallInt).Value = aSource(25)
                    .Add("CreatedBy", SqlDbType.VarChar, 120).Value = aSource(26)
                    .Add("CreatedDate", SqlDbType.DateTime).Value = aSource(27)
                End With
                Command.CommandTimeout = 60000
                If Command.ExecuteNonQuery() <= 0 Then
                    transaction.Commit()
                Else
                    bResult = False
                    transaction.Rollback()
                End If
            Catch ex As Exception
                bResult = False
                transaction.Rollback()
            Finally
                Command.Connection.Close()
            End Try
            Return bResult
        End Using
    End Function
#End Region
End Class
