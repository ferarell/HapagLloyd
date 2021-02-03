Imports Microsoft.Office.Tools.Ribbon
Imports Microsoft.Office.Interop.Excel
Imports System.Windows.Forms


Public Class CustomizedRibbon

    Private Sub Ribbon1_Load(ByVal sender As System.Object, ByVal e As RibbonUIEventArgs) Handles MyBase.Load

    End Sub

    Private Sub btResultadoOperativo_Click(sender As Object, e As RibbonControlEventArgs) Handles btResultadoOperativo.Click
        Dim TittleRng As Excel.Range = Globals.ThisAddIn.Application.Sheets("FINAL RECAP").Range("A5")
        If Not TittleRng.Text.ToUpper.Contains("INVENTORY") Then
            DevExpress.XtraEditors.XtraMessageBox.Show("Este archivo no es válido para esta opción, verifique por favor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        Dim oFinalRecap As New FinalRecap
        oFinalRecap.MainProcess()
    End Sub

    Private Sub btIncomeReconciliation_Click(sender As Object, e As RibbonControlEventArgs) Handles btIncomeReconciliation.Click
        Dim Tittle1Rng As Excel.Range = Globals.ThisAddIn.Application.Sheets("GATE IN").Range("E2")
        Dim Tittle2Rng As Excel.Range = Globals.ThisAddIn.Application.Sheets("GATE OUT").Range("E2")
        If Not (Tittle1Rng.Text.ToUpper.Contains("GATE IN") And Tittle2Rng.Text.ToUpper.Contains("GATE OUT")) Then
            DevExpress.XtraEditors.XtraMessageBox.Show("Este archivo no es válido para esta opción, verifique por favor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        Dim oForm As New IncomeReconciliationForm
        oForm.ShowDialog()
    End Sub
End Class
