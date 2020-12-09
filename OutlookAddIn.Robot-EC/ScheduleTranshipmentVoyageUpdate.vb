Imports System
Imports System.Data
Imports System.IO

Public Class ScheduleTranshipmentVoyageUpdate

    Friend Function DataProcess(FileName As String) As Boolean
        Dim bResult As Boolean = True
        Dim dtSource, dtTextFile As New DataTable
        dtTextFile = LoadCSV(FileName, True)
        If dtTextFile.Rows.Count = 0 Then
            Return False
        End If
        dtSource = ExecuteAccessQuery("select * from ScheduleVoyage where [DPVOYAGE]=''", "dbColdTreatment.accdb").Tables(0)
        Try
            For r = 0 To dtTextFile.Rows.Count - 1
                Dim oRow As DataRow = dtTextFile.Rows(r)
                dtSource.Rows.Add()
                dtSource.Rows(r).Item(0) = oRow("LOCDE")
                dtSource.Rows(r).Item(1) = oRow("DPVOY")
                dtSource.Rows(r).Item(2) = oRow("VESSEL")
                dtSource.Rows(r).Item(3) = oRow("SCHED")
                dtSource.Rows(r).Item(4) = oRow("SSY")
                'dtSource.Rows(r).Item(5) = CDate(oRow(""))
                dtSource.Rows(r).Item(6) = CDate(Replace(Replace(oRow("ARR DATE") & Space(1) & oRow("ARR TIME"), "-", "/"), ".", ":"))
                dtSource.Rows(r).Item(7) = CDate(Replace(Replace(oRow("DEP DATE") & Space(1) & oRow("DEP TIME"), "-", "/"), ".", ":"))
                ScheduleVoyageUpdate(dtSource.Rows(r))
            Next
        Catch ex As Exception
            bResult = False
        End Try
        Return bResult
    End Function

End Class
