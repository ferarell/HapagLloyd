Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Base

Public Class DatosAsociadosForm
    Dim dtResult As New DataTable

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoOrigen.Properties.ButtonClick

    End Sub

    Private Sub DatosAsociadosForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SplitContainerControl2.Collapsed = True
        seEjercicio.Value = Today.Year
        sePeriodo.Value = Today.Month
        FillCompany()
        dtResult = ExecuteAccessQuery("select * from DatosAsociados where Sociedad='####'").Tables(0)
    End Sub

    Private Sub bbiDeshacer_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiDeshacer.ItemClick
        gcDatosAsociados.RefreshDataSource()
    End Sub

    Private Sub bbiConsultar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiConsultar.ItemClick
        dtResult.Rows.Clear()
        dtResult = ExecuteAccessQuery("select * from DatosAsociados where Sociedad='" & lueSociedad.EditValue & "' and Periodo='" & seEjercicio.Text & Format(sePeriodo.EditValue, "00") & "'").Tables(0)
        gcDatosAsociados.DataSource = dtResult
    End Sub

    Private Sub FillCompany()
        lueSociedad.Properties.DataSource = FillDataTable("Company", "", "ACC")
        lueSociedad.Properties.DisplayMember = "CompanyDescription"
        lueSociedad.Properties.ValueMember = "CompanyCode"
    End Sub

    Private Sub bbiGrabar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiGrabar.ItemClick
        Dim sCondition As String = ""
        Dim sValues As String = ""
        Validate()
        Try
            For i As Integer = 0 To GridView1.DataRowCount - 1
                sValues = ""
                sCondition = "Sociedad='" & lueSociedad.EditValue & "' and Periodo='" & seEjercicio.Text & Format(sePeriodo.EditValue, "00") & "' and AsientoContable='" & GridView1.GetRowCellValue(i, GridColumn1) & "' and NumDocRef='" & GridView1.GetRowCellValue(i, GridColumn2) & "'"
                If Not IsDBNull(GridView1.GetRowCellValue(i, "TipDocEmisor")) Then
                    If GridView1.GetRowCellValue(i, "TipDocEmisor") <> "" Then
                        sValues = sValues & "TipDocEmisor='" & Microsoft.VisualBasic.Strings.Left(GridView1.GetRowCellValue(i, "TipDocEmisor"), 2) & "'"
                    End If
                End If
                If Not IsDBNull(GridView1.GetRowCellValue(i, "SerDocEmisor")) Then
                    If GridView1.GetRowCellValue(i, "SerDocEmisor") <> "" Then
                        sValues = sValues & IIf(sValues = "", "", ", ") & "SerDocEmisor='" & GridView1.GetRowCellValue(i, "SerDocEmisor") & "'"
                    End If
                End If
                If Not IsDBNull(GridView1.GetRowCellValue(i, "NumDocEmisor")) Then
                    If GridView1.GetRowCellValue(i, "NumDocEmisor") <> "" Then
                        sValues = sValues & IIf(sValues = "", "", ", ") & "NumDocEmisor='" & GridView1.GetRowCellValue(i, "NumDocEmisor") & "'"
                    End If
                End If
                If Not IsDBNull(GridView1.GetRowCellValue(i, "FecDocEmisor")) Then
                    sValues = sValues & IIf(sValues = "", "", ", ") & "FecDocEmisor='" & Format(GridView1.GetRowCellValue(i, "FecDocEmisor"), "dd/MM/yyy") & "'"
                End If
                If sValues <> "" Then
                    UpdateAccess("DatosAsociados", sCondition, sValues)
                End If
            Next
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            bbiConsultar.PerformClick()
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Los datos fueron guardados satisfactoriamente.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
        ExportarExcel(gcDatosAsociados)
    End Sub

    Private Sub GridView1_CellValueChanged(sender As Object, e As CellValueChangedEventArgs) Handles GridView1.CellValueChanged

    End Sub

    Private Sub GridView1_ValidatingEditor(sender As Object, e As DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs) Handles GridView1.ValidatingEditor

    End Sub

    Private Sub RepositoryItemTextEdit1_Leave(sender As Object, e As EventArgs) Handles RepositoryItemTextEdit1.Leave
        Dim sValue As String = Strings.Right("0000" & GridView1.GetFocusedRowCellValue("SerDocEmisor"), 4)
        GridView1.SetFocusedRowCellValue("SerDocEmisor", sValue)
    End Sub

    Private Sub RepositoryItemTextEdit2_Leave(sender As Object, e As EventArgs) Handles RepositoryItemTextEdit2.Leave
        Dim sValue As String = Strings.Right("00000000" & GridView1.GetFocusedRowCellValue("NumDocEmisor"), 8)
        GridView1.SetFocusedRowCellValue("NumDocEmisor", sValue)
    End Sub
End Class