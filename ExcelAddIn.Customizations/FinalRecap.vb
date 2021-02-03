Imports DevExpress.XtraSplashScreen
Imports System.Windows.Forms
Imports System.Collections
Imports System.Data

Public Class FinalRecap
    Dim dtBookings, dtInventario, dtEmptyReturn As New DataTable

    Friend Sub MainProcess()
        SplashScreenManager.ShowForm(GetType(WaitForm))
        Try
            ProcesaHojaBookings()
            'ProcesaHojaInventario()
            'ProcesaHojaEmptyReturn()
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
    End Sub

    Friend Function ProcesaHojaBookings() As Boolean
        Dim bResult As Boolean = True
        Dim SheetRng As Excel.Range = Globals.ThisAddIn.Application.Sheets("Bookings").Range("A1:AM5000")
        Dim sWeek As String = Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("C3").Value2
        Dim aConditions As New ArrayList
        'Dim currentFind As Excel.Range = Nothing
        'Dim firstFind As Excel.Range = Nothing

        'currentFind = SheetRng.Find(sWeek, , _
        'Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart, _
        'Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, False)
        'While Not currentFind Is Nothing
        '    ' Keep track of the first range you find.
        '    If firstFind Is Nothing Then
        '        firstFind = currentFind
        '        ' If you didn't move to a new range, you are done.
        '    ElseIf currentFind.Address = firstFind.Address Then
        '        Exit While
        '    End If
        '    With currentFind.Font
        '        .Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
        '        .Bold = True
        '    End With
        '    currentFind = SheetRng.FindNext(currentFind)
        'End While
        'Load Data Source
        SplashScreenManager.Default.SetWaitFormDescription("Load Bookings Data Source")
        dtBookings = QueryExcel(Globals.ThisAddIn.Application.ActiveWorkbook.FullName, "SELECT * FROM [Bookings$] ").Tables(0)
        'Paita
        SplashScreenManager.Default.SetWaitFormDescription("Update Paita Table...")
        aConditions.Clear()
        aConditions.AddRange({"PEPAI", sWeek, "BD", "Regular"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("B10").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PEPAI", sWeek, "BD", "ExtraFresh"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("C10").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PEPAI", sWeek, "BD", "ExtraFresh Plus"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("D10").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PEPAI", sWeek, "BPD", "Regular"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("B11").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PEPAI", sWeek, "BPD", "ExtraFresh"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("C11").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PEPAI", sWeek, "BPD", "ExtraFresh Plus"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("D11").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        'Callao
        SplashScreenManager.Default.SetWaitFormDescription("Update Callao Table...")
        aConditions.Clear()
        aConditions.AddRange({"PECLL", sWeek, "BD", "Regular"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("J11").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PECLL", sWeek, "BD", "Liventus"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("K11").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PECLL", sWeek, "BD", "CA 3rd Party"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("L11").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PECLL", sWeek, "BD", "Everfresh"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("M11").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PECLL", sWeek, "BD", "Maxtend"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("N11").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PECLL", sWeek, "BD", "ExtraFresh"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("O11").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PECLL", sWeek, "BD", "ExtraFresh Plus"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("P11").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PECLL", sWeek, "BPD", "Regular"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("J12").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PECLL", sWeek, "BPD", "Liventus"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("K12").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PECLL", sWeek, "BPD", "CA 3rd Party"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("L12").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PECLL", sWeek, "BPD", "Everfresh"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("M12").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PECLL", sWeek, "BPD", "Maxtend"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("N12").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PECLL", sWeek, "BPD", "ExtraFresh"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("O12").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PECLL", sWeek, "BPD", "ExtraFresh Plus"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("P12").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        'ILO
        SplashScreenManager.Default.SetWaitFormDescription("Update Ilo Table...")
        aConditions.Clear()
        aConditions.AddRange({"PEILQ", sWeek, "BD", "Regular"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("V9").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PEILQ", sWeek, "BPD", "Regular"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("V10").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        'MATARANI
        SplashScreenManager.Default.SetWaitFormDescription("Update Matarani Table...")
        aConditions.Clear()
        aConditions.AddRange({"PEMRI", sWeek, "BD", "Regular"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("AB9").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)
        aConditions.Clear()
        aConditions.AddRange({"PEMRI", sWeek, "BPD", "Regular"})
        Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("AB10").Value2 = GetValueBySheet(dtBookings, "Bookings", aConditions)

        DevExpress.XtraEditors.XtraMessageBox.Show("The process has finalized succesfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)

        'SheetRng.Cells(firstFind.Cells.Row + 1, firstFind.Cells.Column) = ""

        Return bResult
    End Function

    Friend Function GetValueBySheet(dtSource As DataTable, Sheet As String, aConditions As ArrayList) As Integer
        Dim iResult As Integer = 0
        Dim dtQuery As New DataTable
        If Sheet = "Bookings" Then
            If dtSource.Select("Locode='" & aConditions(0) & "' AND Week='" & aConditions(1) & "' AND [Dispatch Status]='" & aConditions(2) & "' AND Categoria='" & aConditions(3) & "'").Length > 0 Then
                dtQuery = dtSource.Select("Locode='" & aConditions(0) & "' AND Week='" & aConditions(1) & "' AND [Dispatch Status]='" & aConditions(2) & "' AND Categoria='" & aConditions(3) & "'").CopyToDataTable
                If dtQuery.Rows.Count > 0 Then
                    iResult = dtQuery.Compute("SUM([Amount Booked])", "")
                End If
            End If
        End If
        Return iResult
    End Function

End Class
