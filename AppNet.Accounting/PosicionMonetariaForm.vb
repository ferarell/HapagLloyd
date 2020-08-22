Public Class PosicionMonetariaForm 

    Private Sub bbiConsultar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiConsultar.ItemClick
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("select * from PosicionMonetariaQry1 a where Sociedad='" & lueSociedad.EditValue & "' and ejercicio='" & seEjercicio.Text & "'").Tables(0)
        gcSummary.DataSource = dtQuery
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
        ExportarExcel(gcSummary)
    End Sub

    Private Sub PosicionMonetariaForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        seEjercicio.Text = Year(Now) - 1
        FillCompany()
    End Sub

    Private Sub FillCompany()
        lueSociedad.Properties.DataSource = FillDataTable("Company", "")
        lueSociedad.Properties.DisplayMember = "CompanyDescription"
        lueSociedad.Properties.ValueMember = "CompanyCode"
    End Sub

    Private Sub GetAccountDetail(chart As String, moneda As String)
        Dim dtQuery, dtSummaryView As New DataTable
        'dtQuery = ExecuteAccessQuery("select Periodo, CuentaLocal, CuentaExterna, Moneda, AsientoContable, Posicion, FechaContable, FechaDocumento, TextoCabecera, ImporteMD, (ImporteDebeML-ImporteHaberML) as ImporteML, (ImporteDebeME-ImporteHaberME) as ImporteME from DetalleContable unlock where Sociedad='" & lueSociedad.EditValue & "' and left(Periodo,4)='" & seEjercicio.Text & "' and Moneda='" & moneda & "' and CuentaLocal='" & chart & "' order by Periodo, CuentaLocal, AsientoContable, Posicion").Tables(0)
        dtQuery = ExecuteAccessQuery("select Periodo, CuentaLocal, CuentaExterna, Moneda, AsientoContable, Posicion, FechaContable, FechaDocumento, TextoCabecera, ImporteMD, ImporteML, ImporteME, CuentaAsociada, Nombre from DetalleContableQry1 unlock where Sociedad='" & lueSociedad.EditValue & "' and left(Periodo,4)='" & seEjercicio.Text & "' and Moneda='" & moneda & "' and CuentaLocal='" & chart & "' order by Periodo, CuentaLocal, AsientoContable, Posicion").Tables(0)
        gcDetalle.DataSource = dtQuery
        GridView2.PopulateColumns()
        For i = 9 To 11
            If i > 9 Then
                GridView2.Columns(i).SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
            End If
            GridView2.Columns(i).DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
            GridView2.Columns(i).DisplayFormat.FormatString = "n2"
        Next
    End Sub

    Private Sub GridView1_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView1.FocusedRowChanged
        If Not IsDBNull(GridView1.GetFocusedRowCellValue("Cuenta")) And Not IsDBNull(GridView1.GetFocusedRowCellValue("Moneda")) Then
            GetAccountDetail(GridView1.GetFocusedRowCellValue("Cuenta"), GridView1.GetFocusedRowCellValue("Moneda"))
        End If

    End Sub
End Class