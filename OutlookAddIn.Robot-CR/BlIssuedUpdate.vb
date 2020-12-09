Imports System.Data
Imports System.Data.SqlClient
Imports System
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Collections

Public Class BlIssuedUpdate
    Dim fecha_release As Date
    Dim oDataAccess As New DataAccess
    Dim oLogFileGenerate As New LogFileGenerate
    Friend oMailItem As Outlook.MailItem
    Friend drConfiguration As DataRow

    Friend Sub StartProcess()
        Dim sBlno, sDateR, sMailFrom, sMailTo, sMailSubject, sUser As String
        Dim dToday As DateTime
        sBlno = Replace(Mid(oMailItem.Subject, InStr(oMailItem.Subject, "HLCU"), 16), "'", "")
        sDateR = oMailItem.ReceivedTime.ToShortDateString
        sMailFrom = oMailItem.SenderName & " <" & oMailItem.Sender.Address & ">"
        sMailTo = oMailItem.To
        sMailSubject = Replace(oMailItem.Subject, "'", "")
        sUser = Environment.UserName
        dToday = DateTime.Now
        Try
            DataProcess(sBlno, sDateR, sMailFrom, sMailTo, sMailSubject, sUser, dToday)
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate(drConfiguration("Identifier"), ex.Message)
        End Try

    End Sub

    Private Sub DataProcess(pBlno As String, pDateR As String, pMailFrom As String, pMailTo As String, pMailSubject As String, pUser As String, pToday As DateTime)
        Dim sCol As String = ""
        Dim dtQuery As New DataTable
        dtQuery = oDataAccess.ExecuteAccessQuery("SELECT * FROM " & drConfiguration("TableSQL") & " WHERE blno='" & pBlno & "'").Tables(0)
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
                If Not oDataAccess.InsertIntoAccess(drConfiguration("TableSQL"), dtQuery.Rows(0)) Then
                    oLogFileGenerate.TextFileUpdate(drConfiguration("Identifier"), "Error al insertar el BL: " & pBlno)
                    Return
                End If
                oLogFileGenerate.TextFileUpdate(drConfiguration("Identifier"), "El BL " & pBlno & " se insertó satisfactoriamente")
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
                If Not oDataAccess.UpdateAccess(drConfiguration("TableSQL"), "blno='" & pBlno & "'", sCol & "=" & Format(fecha_release, "#MM/dd/yyyy#")) Then
                    oLogFileGenerate.TextFileUpdate(drConfiguration("Identifier"), "Error al actualizar el BL: " & pBlno)
                    Return
                End If
                oLogFileGenerate.TextFileUpdate(drConfiguration("Identifier"), "El BL " & pBlno & " se actualizó satisfactoriamente")
            End If
        End If
    End Sub

End Class
