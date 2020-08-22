Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraBars
Imports DevExpress.XtraSplashScreen
Imports System.Threading

Public Class TiposCambioForm
    Dim sAction As String = ""
    Dim dtExchange As New DataTable

    Private Sub TiposCambioForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FillMainView()
        ControlsActive("Load")
        LoadCurrency()
        LoadValidations()
        SplitContainerControl2.Collapsed = True
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
        ExportarExcel(gcTiposCambio)
    End Sub

    Private Sub FillMainView()
        dtExchange.Rows.Clear()
        dtExchange = ExecuteAccessQuery("select * from TiposCambio").Tables(0)
        gcTiposCambio.DataSource = dtExchange
    End Sub

    Private Sub ControlsActive(Action As String)
        ButtonsActive(Action)
        InputsActive(Action)
    End Sub

    Private Sub InputsActive(Action As String)
        For Each ctrl As Control In SplitContainerControl1.Panel1.Controls
            For Each input As Control In ctrl.Controls
                input.Enabled = True
            Next
        Next
        For Each ctrl As Control In SplitContainerControl1.Panel1.Controls
            If DirectCast(ctrl, DevExpress.XtraEditors.GroupControl).Text.Contains({"Datos Generales", "Tipo Cambio Venta", "Tipo Cambio Compra", "Tipo de Registro"}) Then
                For Each input As Control In ctrl.Controls
                    If Action.Contains({"Load", "Save", "Delete", "Undo"}) Then
                        input.Enabled = False
                    End If
                    If Action.Contains({"Edit"}) Then
                        If input.Name.Contains({"lueMoneda", "deFecha", "rgTipoRegistro"}) Then
                            input.Enabled = False
                        End If
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub ButtonsActive(Action As String)
        For Each bbi As BarButtonItemLink In bAcciones.ItemLinks
            If bbi IsNot Nothing Then
                bbi.Item.Enabled = True
            End If
        Next
        For Each bbi As BarButtonItemLink In bAcciones.ItemLinks
            If bbi IsNot Nothing Then
                If Action.Contains({"Load", "Save", "Delete", "Undo"}) Then
                    If bbi.Caption.Contains({"Deshacer", "Grabar"}) Then
                        bbi.Item.Enabled = False
                    End If
                End If
                If Action.Contains({"Insert", "Edit"}) Then
                    If bbi.Caption.Contains({"Nuevo", "Editar", "Eliminar", "Exportar"}) Then
                        bbi.Item.Enabled = False
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub bbiNuevo_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiNuevo.ItemClick
        sAction = e.Item.Tag
        ControlsActive(sAction)
    End Sub

    Private Sub bbiEditar_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiEditar.ItemClick
        sAction = e.Item.Tag
        ControlsActive(sAction)
    End Sub

    Private Sub bbiGrabar_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiGrabar.ItemClick
        Me.Refresh()
        Dim bResult As Boolean = True
        If vpControls.Validate Then
            Try
                If sAction = "Insert" Then
                    Dim dtQuery As New DataTable
                    dtQuery = ExecuteAccessQuery("select * from TiposCambio where [CodigoMoneda] = '" & lueMoneda.EditValue & "' and [Fecha] = #" & Format(deFecha.EditValue, "MM/dd/yyyy") & "# and [TipoRegistro] = '" & rgTipoRegistro.EditValue & "'").Tables(0)
                    If dtQuery.Rows.Count = 0 Then
                        dtQuery.Rows.Add({lueMoneda.EditValue, deFecha.EditValue, rgTipoRegistro.EditValue, teTcLocalV.EditValue, teTcLocalC.EditValue, teTcDolarV.EditValue, teTcDolarC.EditValue})
                        If lueMoneda.EditValue = "USD" Then
                            dtQuery.Rows.Add({"PEN", deFecha.EditValue, rgTipoRegistro.EditValue, 1, 1, (1 / teTcLocalV.EditValue), (1 / teTcLocalC.EditValue)})
                        End If
                        For Each row As DataRow In dtQuery.Rows
                            If Not InsertIntoAccess("TiposCambio", dtQuery.Rows(dtQuery.Rows.IndexOf(row))) Then
                                bResult = False
                            End If
                        Next
                        FillMainView()
                        If bResult Then
                            DevExpress.XtraEditors.XtraMessageBox.Show("Los datos han sido actualizados satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Else
                            DevExpress.XtraEditors.XtraMessageBox.Show("Ocurrió un error al insertar los datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    End If
                Else
                    If UpdateAccessRow() Then
                        FillMainView()
                        DevExpress.XtraEditors.XtraMessageBox.Show("Los datos han sido actualizados satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End If
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show("Error al insertar nuevo registro." & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                ControlsActive("Save")
            End Try
        Else
        DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El tipo de cambio ya existe, por favor verifique.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        End If

    End Sub

    Private Sub bbiDeshacer_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiDeshacer.ItemClick
        sAction = e.Item.Tag
        ControlsActive(sAction)
    End Sub

    Private Sub LoadCurrency()
        lueMoneda.Properties.DataSource = FillDataTable("Currency", "")
        lueMoneda.Properties.DisplayMember = "CurrencyCode"
        lueMoneda.Properties.ValueMember = "CurrencyCode"
    End Sub

    Private Sub LoadValidations()
        Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

        containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
        containsValidationRule.ErrorText = "Asigne un valor."
        containsValidationRule.ErrorType = ErrorType.Critical

        Dim customValidationRule As New CustomValidationRule()
        customValidationRule.ErrorText = "Valor obligatorio."
        customValidationRule.ErrorType = ErrorType.Critical

        vpControls.SetValidationRule(lueMoneda, customValidationRule)
        vpControls.SetValidationRule(deFecha, customValidationRule)
        vpControls.SetValidationRule(teTcDolarC, customValidationRule)
        vpControls.SetValidationRule(teTcLocalC, customValidationRule)
        vpControls.SetValidationRule(teTcDolarV, customValidationRule)
        vpControls.SetValidationRule(teTcLocalV, customValidationRule)
    End Sub

    Private Sub CardView1_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles CardView1.FocusedRowChanged
        If e.FocusedRowHandle >= 0 Then
            Dim drvItem As DataRowView = CardView1.GetRow(e.FocusedRowHandle)
            lueMoneda.EditValue = drvItem("CurrencyCode")
            deFecha.EditValue = drvItem("Fecha")
            rgTipoRegistro.EditValue = drvItem("TipoRegistro")
            teTcLocalV.EditValue = drvItem("TcLocalV")
            teTcDolarV.EditValue = drvItem("TcDolarV")
            teTcLocalC.EditValue = drvItem("TcLocalC")
            teTcDolarC.EditValue = drvItem("TcDolarC")
        End If
    End Sub

    Private Sub bbiEliminar_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiEliminar.ItemClick
        Dim dtQuery As New DataTable
        If DevExpress.XtraEditors.XtraMessageBox.Show("Está seguro que desea eliminar el registro seleccionado? ", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
            If ExecuteAccessNonQuery("DELETE FROM TiposCambio where [CodigoMoneda] = '" & lueMoneda.EditValue & "' and [Fecha] = #" & Format(deFecha.EditValue, "MM/dd/yyyy") & "# and [TipoRegistro] = '" & rgTipoRegistro.EditValue & "'") Then
                CardView1.FocusedRowHandle = CardView1.FocusedRowHandle - 1
                FillMainView
                DevExpress.XtraEditors.XtraMessageBox.Show("El registro ha sido eliminado satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub

    Friend Function UpdateAccessRow() As Boolean
        Dim bResult As Boolean = True
        Dim SetValues, Condition As String
        Condition = "[CodigoMoneda] = '" & lueMoneda.EditValue & "' and [Fecha] = #" & Format(deFecha.EditValue, "MM/dd/yyyy") & "# and [TipoRegistro] = '" & rgTipoRegistro.EditValue & "'"
        SetValues = ""
        SetValues = SetValues & "[CodigoMoneda] = '" & lueMoneda.EditValue & "'"
        SetValues = SetValues & ", [Fecha] = " & "#" & Format(deFecha.EditValue, "MM/dd/yyyy") & "#"
        SetValues = SetValues & ", [TipoRegistro] = '" & rgTipoRegistro.EditValue & "'"
        SetValues = SetValues & ", [TcLocalV] = " & teTcLocalV.Text
        SetValues = SetValues & ", [TcLocalC] = " & teTcLocalC.Text
        SetValues = SetValues & ", [TcDolarV] = " & teTcDolarV.Text
        SetValues = SetValues & ", [TcDolarC] = " & teTcDolarC.Text
        bResult = UpdateAccess("TiposCambio", Condition, SetValues)
        Return bResult
    End Function

    Private Sub bbiImportar_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiImportar.ItemClick
        SplitContainerControl2.Collapsed = False
    End Sub

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoOrigen.Properties.ButtonClick
        OpenFileDialog1.Filter = "Excel Files (*.xls*)|*.xls*"
        OpenFileDialog1.FileName = ""
        'OpenFileDialog1.InitialDirectory = IIf(My.Settings.LedgerSourceDirectory5 <> "", My.Settings.LedgerSourceDirectory5, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beArchivoOrigen.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub sbLoadExchange_Click(sender As Object, e As EventArgs) Handles sbLoadExchange.Click
        Dim dtQuery As New DataTable
        Dim iPos As Integer = 0
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Cargando tipos de cambio...")
        dtExchange.Rows.Clear()
        dtExchange = ExecuteAccessQuery("select * from TiposCambio where CodigoMoneda = '#'").Tables(0)
        dtQuery = LoadExcel(OpenFileDialog1.FileName, "{0}").Tables(0)
        Try
            For Each row As DataRow In dtQuery.Rows
                If ExecuteAccessQuery("select * from TiposCambio where CodigoMoneda='PEN' and Fecha=#" & Format(CDate(Replace(row(0), ".", "/")), "MM/dd/yyyy") & "# and TipoRegistro='D'").Tables(0).Rows.Count = 0 Then
                    dtExchange.Rows.Add()
                    iPos = dtExchange.Rows.Count - 1
                    dtExchange.Rows(iPos).Item(0) = "PEN"
                    dtExchange.Rows(iPos).Item(1) = CDate(Replace(row(0), ".", "/"))
                    dtExchange.Rows(iPos).Item(2) = "D"
                    dtExchange.Rows(iPos).Item(3) = 1
                    dtExchange.Rows(iPos).Item(4) = 1
                    dtExchange.Rows(iPos).Item(5) = row(9)
                    dtExchange.Rows(iPos).Item(6) = row(9)
                    If Not InsertIntoAccess("TiposCambio", dtExchange.Rows(iPos)) Then
                        Exit For
                    End If
                End If
                If ExecuteAccessQuery("select * from TiposCambio where CodigoMoneda='USD' and Fecha=#" & Format(CDate(Replace(row(0), ".", "/")), "MM/dd/yyyy") & "# and TipoRegistro='D'").Tables(0).Rows.Count = 0 Then
                    dtExchange.Rows.Add()
                    iPos = dtExchange.Rows.Count - 1
                    dtExchange.Rows(iPos).Item(0) = "USD"
                    dtExchange.Rows(iPos).Item(1) = CDate(Replace(row(0), ".", "/"))
                    dtExchange.Rows(iPos).Item(2) = "D"
                    dtExchange.Rows(iPos).Item(3) = row(5)
                    dtExchange.Rows(iPos).Item(4) = row(5)
                    dtExchange.Rows(iPos).Item(5) = 1
                    dtExchange.Rows(iPos).Item(6) = 1
                    If Not InsertIntoAccess("TiposCambio", dtExchange.Rows(iPos)) Then
                        Exit For
                    End If
                End If
            Next
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
        SplitContainerControl2.Collapsed = True
        FillMainView()
    End Sub
End Class