﻿Imports System.Data

Public Class LogProcessUpdate

    Friend Function GetIdLogProcess(ProcessCode As String) As Integer
        Dim dtQuery As New DataTable
        Dim iLogProcess As Integer = 0
        dtQuery = ExecuteAccessQuery("SELECT * FROM LastLogProcessQry WHERE ProcessCode='" & ProcessCode & "'", "").Tables(0)
        If dtQuery.Rows.Count = 0 Then
            iLogProcess = 1
        Else
            iLogProcess = ExecuteAccessQuery("SELECT * FROM LastLogProcessQry WHERE ProcessCode='" & ProcessCode & "'", "").Tables(0).Rows(0)(0) + 1
        End If
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
        InsertIntoAccess("LogProcess", dtLogPrc.Rows(0), "")
        Return iLogProcessItem
    End Function

    Friend Function SetLogProcessItem(IdLogProcess As Integer, ProcessCode As String, KeyValue1 As String, KeyValue2 As String, UserProcess As String) As Integer
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
        dtLogPrc.Rows(0)(2) = ProcessCode
        dtLogPrc.Rows(0)(3) = KeyValue1
        dtLogPrc.Rows(0)(4) = KeyValue2
        dtLogPrc.Rows(0)(7) = UserProcess
        dtLogPrc.Rows(0)(8) = Now.ToString
        InsertIntoAccess("LogProcess", dtLogPrc.Rows(0), "")
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
