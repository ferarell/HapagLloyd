Imports System
Imports System.Data
Imports System.IO

Public Class ScheduleLocalVoyageUpdate

    Friend Function DataProcess(FileName As String) As Boolean
        Dim bResult As Boolean = True
        Dim dtSource As New DataTable
        Dim iPosition As Integer = 0
        dtSource = ExecuteAccessQuery("select * from ScheduleVoyage where [DPVOYAGE]=''", "dbColdTreatment.accdb").Tables(0)
        Try
            Using sr As New StreamReader(FileName)
                Dim lines As List(Of String) = New List(Of String)
                Dim bExit As Boolean = False
                Do While Not sr.EndOfStream
                    lines.Add(sr.ReadLine())
                Loop
                Dim bSkip As Boolean = True
                For i As Integer = 0 To lines.Count - 1
                    If Mid(lines(i), 1, 5).Trim = "-----" Then
                        i = i + 1
                    End If
                    If Mid(lines(i), 1, 6).Trim.Length = 5 Then
                        dtSource.Rows.Add()
                        iPosition = dtSource.Rows.Count - 1
                        dtSource.Rows(iPosition).Item(0) = Mid(lines(i), 1, 5)
                        dtSource.Rows(iPosition).Item(1) = Mid(lines(i), 7, 6)
                        dtSource.Rows(iPosition).Item(2) = Mid(lines(i), 14, 14)
                        dtSource.Rows(iPosition).Item(3) = Mid(lines(i), 29, 8)
                        dtSource.Rows(iPosition).Item(4) = Mid(lines(i), 38, 3)
                        dtSource.Rows(iPosition).Item(5) = CDate(Replace(Replace(Mid(lines(i), 44, 16), "-", "/"), ".", ":"))
                        dtSource.Rows(iPosition).Item(6) = CDate(Replace(Replace(Mid(lines(i), 83, 16), "-", "/"), ".", ":"))
                        dtSource.Rows(iPosition).Item(7) = CDate(Replace(Replace(Mid(lines(i), 102, 16), "-", "/"), ".", ":"))
                        ScheduleVoyageUpdate(dtSource.Rows(iPosition))
                    End If
                Next
            End Using
        Catch ex As Exception
            bResult = False
        End Try
        Return bResult
    End Function

End Class
