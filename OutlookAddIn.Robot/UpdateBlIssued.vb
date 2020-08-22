Imports System.Data
Imports System.Data.SqlClient
Imports System
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Collections

Public Class UpdateBlIssued
    Dim fecha_release As Date

    Friend Sub DataProcess(pBlno As String, pDateR As String, pMailFrom As String, pMailTo As String, pMailSubject As String, pUser As String, pToday As DateTime)
        MDBFileName = My.Settings.DBFileName
        Dim sCol As String = ""
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("select * from " & drConfig("Tabla") & " where blno='" & pBlno & "'", "").Tables(0)
        sException.Clear()
        If dtQuery.Rows.Count = 0 Then
            If pBlno <> "" Then
                fecha_release = CDate(pDateR)
                dtQuery.Rows.Add()
                dtQuery.Rows(0).Item("blno") = pBlno
                dtQuery.Rows(0).Item("fecha_release1") = fecha_release
                dtQuery.Rows(0).Item("fecha_release2") = DBNull.Value
                dtQuery.Rows(0).Item("fecha_release3") = DBNull.Value
                dtQuery.Rows(0).Item("fecha_release4") = DBNull.Value
                dtQuery.Rows(0).Item("remitente") = pMailFrom
                dtQuery.Rows(0).Item("destinatarios") = pMailTo.Trim
                dtQuery.Rows(0).Item("asunto") = pMailSubject.Trim
                dtQuery.Rows(0).Item("user_up") = pUser
                dtQuery.Rows(0).Item("date_up") = pToday
                If Not InsertIntoAccess(drConfig("Tabla"), dtQuery.Rows(0), "") Then
                    LogFileGenerate("Error al insertar el BL: " & pBlno)
                End If
            End If
        Else
            For i = 2 To 4
                If IsDBNull(dtQuery.Rows(0).Item(i)) Or (Not IsDBNull(dtQuery.Rows(0).Item(i)) And i = 4) Then
                    If Not IsDBNull(dtQuery.Rows(0).Item("fecha_release" & (i - 1).ToString)) Then
                        If dtQuery.Rows(0).Item("fecha_release" & (i - 1).ToString) <> CDate(pDateR) Then
                            sCol = "fecha_release" & i.ToString
                            Exit For
                        End If
                    End If
                End If
            Next
            If sCol <> "" Then
                fecha_release = CDate(pDateR)
                If Not UpdateAccess(drConfig("Tabla"), "blno='" & pBlno & "'", sCol & "=" & Format(fecha_release, "#MM/dd/yyyy#"), "") Then
                    LogFileGenerate("Error al actualizar el BL: " & pBlno)
                End If
            End If
        End If
    End Sub

    Private Sub LogFileGenerate(TextLog As String)
        Dim sFile As System.IO.StreamWriter
        sFile = My.Computer.FileSystem.OpenTextFileWriter("C:\Robot\" & drConfig("Tabla") & ".Log", True)
        sFile.WriteLine("[" & DateTime.Now.ToString & "]" & Space(2) & TextLog & Space(2))
        For i = 0 To sException.Count - 1
            sFile.WriteLine(sException(i))
        Next
        sFile.Close()
    End Sub

End Class
