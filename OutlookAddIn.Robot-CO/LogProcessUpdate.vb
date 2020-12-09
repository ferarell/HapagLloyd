Imports System.Data

Public Class LogProcessUpdate
    Dim oDataAccess As New DataAccess

    Friend Function GetIdLogProcess(ProcessCode As String) As Integer
        Dim dtQuery As New DataTable
        Dim iLogProcess As Integer = 0
        dtQuery = oDataAccess.ExecuteAccessQuery("SELECT * FROM LastLogProcessQry WHERE ProcessCode='" & ProcessCode & "'").Tables(0)
        If dtQuery.Rows.Count = 0 Then
            iLogProcess = 1
        Else
            iLogProcess = oDataAccess.ExecuteAccessQuery("SELECT * FROM LastLogProcessQry WHERE ProcessCode='" & ProcessCode & "'").Tables(0).Rows(0)(0) + 1
        End If
        Return iLogProcess
    End Function

    Friend Function SetLogProcessItem(IdLogProcess As Integer, ProcessCode As String, KeyValue1 As String, KeyValue2 As String, UserProcess As String) As Integer
        Dim dtQuery As New DataTable
        Dim iLogProcessItem As Integer = 0
        dtQuery = oDataAccess.ExecuteAccessQuery("SELECT * FROM LastLogProcessQry WHERE IdLogProcess=" & IdLogProcess.ToString).Tables(0)
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
        dtLogPrc.Rows(0)(2) = ProcessCode
        dtLogPrc.Rows(0)(3) = KeyValue1
        dtLogPrc.Rows(0)(4) = KeyValue2
        dtLogPrc.Rows(0)(7) = UserProcess
        dtLogPrc.Rows(0)(8) = Now.ToString
        oDataAccess.InsertIntoAccess("LogProcess", dtLogPrc.Rows(0))
        Return iLogProcessItem
    End Function

    Friend Function SetDescriptionLogProcess(iLogProcess As Integer, iLogProcessItem As Integer, UserProcess As String, sMessage As String) As Boolean
        Dim bResult As Boolean = True
        Dim sCondition As String = "IdLogProcess=" & iLogProcess.ToString & " AND LogProcessItem=" & iLogProcessItem.ToString
        Dim sValue As String = "Description='" & sMessage & "', UserCreate='" & UserProcess & "'"
        If Not oDataAccess.UpdateAccess("LogProcess", sCondition, sValue) Then
            bResult = False
        End If
        Return bResult
    End Function

    Public Sub New()
        dtLogProcess = oDataAccess.ExecuteAccessQuery("SELECT * FROM LogProcess WHERE IdLogProcess=0").Tables(0)
    End Sub
End Class
