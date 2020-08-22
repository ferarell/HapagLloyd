Imports System.Data

Public Class LogProcessUpdate

    Friend Function GetIdLogProcess(CodeProcess As String) As Integer
        Dim dtQuery As New DataTable
        Dim iLogProcess As Integer = 0 'ExecuteAccessQuery("SELECT * FROM LastLogProcessQry", "").Tables(0).Rows(0)("IdLogProcess") + 1
        dtQuery = ExecuteAccessQuery("SELECT * FROM LastLogProcessQry", "").Tables(0)
        If dtQuery.Rows.Count = 0 Then
            iLogProcess = 1
        Else
            iLogProcess = ExecuteAccessQuery("SELECT * FROM LastLogProcessQry", "").Tables(0).Rows(0)("IdLogProcess") + 1
        End If
        'Dim dtLogPrc As New DataTable
        'dtLogPrc = dtQuery.Clone
        'dtLogPrc.Rows.Add()
        'dtLogPrc.Rows(0)(0) = iLogProcess
        'dtLogPrc.Rows(0)(1) = 1
        'dtLogPrc.Rows(0)(2) = CodeProcess
        'dtLogPrc.Rows(0)(4) = My.User.Name
        'dtLogPrc.Rows(0)(5) = Now.ToString
        'InsertIntoAccess("LogProcess", dtLogPrc.Rows(0), "", Nothing, Nothing)
        Return iLogProcess
    End Function

    Friend Function GetLogProcessItem(CodeProcess As String, IdLogProcess As Integer) As Integer
        Dim dtQuery As New DataTable
        Dim iLogProcessItem As Integer = 0
        dtQuery = ExecuteAccessQuery("SELECT * FROM LastLogProcessQry WHERE IdLogProcess=" & IdLogProcess.ToString, "").Tables(0)
        Dim dtLogPrc As New DataTable
        dtLogPrc = dtQuery.Clone
        If dtQuery.Rows.Count = 0 Then
            iLogProcessItem = 1
        Else
            iLogProcessItem = dtQuery.Rows(0)(1) + 1
        End If
        dtLogPrc.Rows.Add()
        dtLogPrc.Rows(0)(0) = IdLogProcess
        dtLogPrc.Rows(0)(1) = iLogProcessItem
        dtLogPrc.Rows(0)(2) = CodeProcess
        dtLogPrc.Rows(0)(4) = My.User.Name
        dtLogPrc.Rows(0)(5) = Now.ToString
        InsertIntoAccess("LogProcess", dtLogPrc.Rows(0), "", Nothing, Nothing)
        Return iLogProcessItem
    End Function

    Friend Function SetDescriptionLogProcess(iLogProcess As Integer, iLogProcessItem As Integer, sMessage As String) As Boolean
        Dim bResult As Boolean = True
        Dim sCondition As String = "IdLogProcess=" & iLogProcess.ToString & " AND LogProcessItem=" & iLogProcessItem.ToString
        Dim sValue As String = "Description='" & sMessage & "'"
        If Not UpdateAccess("LogProcess", sCondition, sValue, "") Then
            bResult = False
        End If
        Return bResult
    End Function
End Class
