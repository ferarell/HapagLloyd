Imports System.Collections

Public Class PrintForm
    Friend dtPrint, dtPrint1, dtPrint2 As New DataTable
    Friend aParams As New ArrayList
    Friend RptFile As String
    Friend RptSegments As Integer = 1

    Private Sub PrintForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    Dim drPrint As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Try
            drPrint.FileName = IO.Directory.GetCurrentDirectory & "\Reports\" & RptFile
            drPrint.SetDataSource(dtPrint)
            If RptSegments = 2 Then
                drPrint.Subreports(0).SetDataSource(dtPrint1)
                drPrint.Subreports(1).SetDataSource(dtPrint2)
            End If
            For p = 0 To aParams.Count - 1
                drPrint.SetParameterValue(p, aParams(p))
            Next
            CrystalReportViewer.ReportSource = drPrint
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

End Class