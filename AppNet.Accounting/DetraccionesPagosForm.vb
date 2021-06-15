Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports DevExpress.XtraEditors

Public Class DetraccionesPagosForm

    Dim oAppService As New AppService.HapagLloydServiceClient
    Dim dsMain As New dsSunat
    Dim dtTypePaytDoc, dtHeader, dtDetail As New DataTable
    Dim DetraFileName As String = ""
    Dim bExisteLote As Boolean = False
    Dim bFlatFileGenerate As Boolean = True
    Dim bProcess As Boolean = True
    Dim LastButton As String = ""

    Private Sub DetraccionesForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FolderBrowserDialog1.SelectedPath = IIf(My.Settings.DetraTargetDirectory <> "", My.Settings.DetraTargetDirectory, "")
        dtTypePaytDoc = FillDataTable("TipoComprobante", "", "ACC")
        EnableButtons(False)
        FiltersEnabled()
        FillCompany()
    End Sub

    Private Sub bbiProcesar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesar.ItemClick
        LastButton = e.Item.Name
        LoadInputValidations()
        If vpInputs.Validate Then
            Try
                If ExisteLote() Then
                    If DevExpress.XtraEditors.XtraMessageBox.Show("El lote indicado ya existe, desea reemplazarlo?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                        Return
                    End If
                End If
                TransactionData()
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error al procesar solicitud. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                SplashScreenManager.CloseForm(False)
            End Try

        End If
        EnableButtons(True)
    End Sub

    Friend Function ExisteLote() As Boolean
        bExisteLote = False
        Dim sQuery As String = "select * from PagosDetracciones where [Numero RUC Empresa] = '" & lueSociedad.GetColumnValue("CompanyTaxCode") & "' and [Numero Lote] = '" & seLote.Text & "'"
        If oAppService.ExecuteSQL(sQuery).Tables(0).Rows.Count > 0 Then
            bExisteLote = True
        End If
        Return bExisteLote
    End Function

    Private Sub TransactionData()
        bFlatFileGenerate = True
        bProcess = True
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            dtHeader.Rows.Clear()
            dtDetail.Rows.Clear()
            dtHeader = dsMain.Tables("DetraccionesPMC")
            dtDetail = dsMain.Tables("DetraccionesPMD")
            If DataProcess(LoadExcel(beArchivoOrigen.Text, "{0}").Tables(0)) Then
                gcDetracciones.DataSource = dtDetail
                GridView1.PopulateColumns()
                gcDetracciones.RefreshDataSource()
                GridView1.Columns("C7").SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
            End If
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error al procesar solicitud. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
    End Sub

    Friend Function EliminaLote() As Boolean
        'Dim bResult As Boolean = True
        Dim aResult As New ArrayList
        aResult.AddRange(oAppService.ExecuteSQLNonQuery("DELETE FROM PagosDetracciones where [Numero RUC Empresa] = '" & lueSociedad.GetColumnValue("CompanyTaxCode") & "' and [Numero Lote] = '" & seLote.Text & "'"))
        'If aResult(0) = 0 Then
        '    bResult = False
        'End If
        Return aResult(0)
    End Function

    Friend Function ActualizaLote() As Boolean
        Dim bResult As Boolean = True
        Dim dtPagos As New DataTable
        Dim iPos As Integer = 0
        dtPagos = oAppService.ExecuteSQL("select * from PagosDetracciones where [Numero RUC Empresa] = ''").Tables(0)
        For Each row As DataRow In dtDetail.Rows
            dtPagos.Rows.Add()
            iPos = dtPagos.Rows.Count - 1
            dtPagos.Rows(iPos).Item(0) = seLote.Text
            dtPagos.Rows(iPos).Item(1) = lueSociedad.GetColumnValue("CompanyTaxCode")
            dtPagos.Rows(iPos).Item(2) = row(0)
            dtPagos.Rows(iPos).Item(3) = row(1)
            dtPagos.Rows(iPos).Item(4) = row(2)
            dtPagos.Rows(iPos).Item(5) = row(3)
            dtPagos.Rows(iPos).Item(6) = row(4)
            dtPagos.Rows(iPos).Item(7) = row(5)
            dtPagos.Rows(iPos).Item(8) = row(6)
            dtPagos.Rows(iPos).Item(9) = row(7)
            dtPagos.Rows(iPos).Item(10) = row(8)
            dtPagos.Rows(iPos).Item(11) = row(9)
            dtPagos.Rows(iPos).Item(12) = row(10)
            dtPagos.Rows(iPos).Item(13) = row(11)
            If Not InsertIntoAccess("PagosDetracciones", dtPagos.Rows(iPos)) Then
                bResult = False
            End If
        Next
        Return bResult
    End Function

    Friend Function DataProcess(dtSource As DataTable) As Boolean
        Dim bResult As Boolean = True
        Dim ImpTot, ImpDoc As Integer
        Dim iPosition As Integer = 0
        'Dim iPosSep As Integer = 0
        Dim sTipDoc As String = ""
        bProcess = True
        If dtSource.Rows.Count = 0 Then
            bResult = False
        End If
        Try
            ImpTot = 0
            'Detalle
            For Each row As DataRow In dtSource.Rows
                If Not IsDBNull(row(0)) Then
                    ImpDoc = CDbl(row(9)) ' GetAmountByDoc(IIf(IsDBNull(row(4)), 0, row(4)), IIf(IsDBNull(row(5)), 0, row(5)), IIf(IsDBNull(row(7)), 0, row(7)), 10)
                    ImpTot = ImpTot + ImpDoc
                    'iPosSep = InStr(row(2), "-")
                    dtDetail.Rows.Add()
                    iPosition = dtDetail.Rows.Count - 1
                    dtDetail.Rows(iPosition).Item("C1") = "6"
                    If Not IsDBNull(row(1)) Then
                        dtDetail.Rows(iPosition).Item("C2") = Mid(row(1).ToString.Trim & Space(11), 1, 11)
                    End If
                    If Not IsDBNull(row(0)) Then
                        dtDetail.Rows(iPosition).Item("C3") = Mid(row(0).ToString.Trim & Space(35), 1, 35)
                    End If
                    dtDetail.Rows(iPosition).Item("C4") = "000000000"
                    dtDetail.Rows(iPosition).Item("C5") = "037"
                    dtDetail.Rows(iPosition).Item("C6") = Mid(GetAccVendor(row(1).ToString.Trim) & Space(11), 1, 11)
                    dtDetail.Rows(iPosition).Item("C7") = ImpDoc.ToString
                    dtDetail.Rows(iPosition).Item("C8") = "01"
                    dtDetail.Rows(iPosition).Item("C9") = Format(row(3), "yyyyMM")
                    sTipDoc = DataValidation("TipDoc", Strings.Left(row(2).ToString.Trim, 2))
                    If sTipDoc <> "" And InStr(row(2), " ") Then
                        dtDetail.Rows(iPosition).Item("C10") = sTipDoc
                        dtDetail.Rows(iPosition).Item("C11") = GetTextFormatValue(sTipDoc, "NroSer", Mid(row(2), 4, Len(row(2)) - 3)) 'Strings.Right("0000" & Strings.Left(row(2), iPosSep - 1), 4)
                        dtDetail.Rows(iPosition).Item("C12") = GetTextFormatValue(sTipDoc, "NroDoc", Mid(row(2), 4, Len(row(2)) - 3)) 'Strings.Right("00000000" & Mid(row(2), iPosSep + 1, row(2).Length - iPosSep), 8)
                    Else
                        dtDetail.Rows(iPosition).Item("C13") = "El formato de la columna Factura del archivo de origen es incorrecto, el formato correcto es (XX XXXX-XXXXXXXX)."
                        'bFlatFileGenerate = False
                        bProcess = False
                    End If
                    Dim sLoteDoc As String = GetLoteByDoc(dtDetail.Rows(iPosition).Item("C2"), dtDetail.Rows(iPosition).Item("C10"), dtDetail.Rows(iPosition).Item("C11"), dtDetail.Rows(iPosition).Item("C12"))
                    If sLoteDoc <> "" Then
                        Dim sDocDup As String = sTipDoc & " " & dtDetail.Rows(iPosition).Item("C11") & "-" & dtDetail.Rows(iPosition).Item("C12")
                        dtDetail.Rows(iPosition).Item("C13") = "El documento " & sDocDup & " ya fue pagado en el lote " & sLoteDoc & ", por favor verifique."
                        'bFlatFileGenerate = False
                        bProcess = False
                    End If
                End If
            Next
            'Cabecera
            dtHeader.Rows.Add()
            dtHeader.Rows(0).Item("C1") = rgTipo.EditValue
            dtHeader.Rows(0).Item("C2") = lueSociedad.GetColumnValue("CompanyTaxCode")
            dtHeader.Rows(0).Item("C3") = Strings.Left(lueSociedad.Text & Space(35), 35)
            dtHeader.Rows(0).Item("C4") = Format(seLote.EditValue, "000000")
            dtHeader.Rows(0).Item("C5") = Strings.Right(Strings.StrDup(15, "0") & ImpTot.ToString & "00", 15)
            dtHeader.Rows(0).Item("C6") = ""
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                'bProcess = False
            End If
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
        Return bResult
    End Function

    Friend Function GetLoteByDoc(ruc As String, tipdoc As String, serdoc As String, numdoc As String) As String
        Dim sLote As String = ""
        Dim dtQuery As New DataTable
        dtQuery = oAppService.ExecuteSQL("select [Numero Lote] from PagosDetracciones where [Numero Documento Identidad]='" & ruc & "' and [Tipo Comprobante]='" & tipdoc & "' and [Serie Comprobante]='" & serdoc & "' and [Numero Comprobante]='" & numdoc & "'").Tables(0)
        If dtQuery.Rows.Count > 0 Then
            sLote = dtQuery.Rows(0)(0)
        End If
        Return sLote
    End Function

    Friend Function GetAmountByDoc(ImpLoc As Decimal, ImpDol As Decimal, tc As Double, porcentaje As Integer) As Integer
        Dim dResult As Decimal = 0
        Dim factor As Double = porcentaje / 100
        Try
            If ImpLoc > 0 Then
                dResult = ImpLoc * factor
            Else
                dResult = Math.Round(ImpDol * tc, 2) * factor
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        Return Redondeo(dResult, 0)
    End Function

    Private Function Redondeo(ByVal Numero, ByVal Decimales)
        Redondeo = Int(Numero * 10 ^ Decimales + 1 / 2) / 10 ^ Decimales
    End Function

    Friend Function GetAccVendor(ruc As String) As String
        Dim sResult As String = ""
        Try
            If oAppService.ExecuteSQL("select [CuentaBN] from Proveedores where [NoRUC] = '" & ruc & "'").Tables(0).Rows.Count > 0 Then
                sResult = oAppService.ExecuteSQL("select [CuentaBN] from Proveedores where [NoRUC] = '" & ruc & "'").Tables(0).Rows(0)(0)
            End If
        Catch ex As Exception
            'MessageBox.Show(ex.Message)
        End Try
        Return sResult
    End Function

    'Private Sub LoadData()
    '    Try
    '        Dim dtSource As New DataTable
    '        Dim dtTarget As New DataTable
    '        dtSource = LoadExcel(beArchivoOrigen.Text, "{0}").Tables(0)
    '        For Each row As DataRow In dtSource.Rows
    '            If ExecuteAccessQuery("select * from PagosDetracciones where [Numero Constancia] = '" & row(2).ToString & "'").Tables(0).Rows.Count = 0 Then
    '                InsertIntoAccess(My.Settings.DBDirectory & "\" & My.Settings.MDBFileName, "PagosDetracciones", row)
    '            End If
    '        Next
    '        dtTarget = ExecuteAccessQuery("select * from PagosDetracciones").Tables(0)
    '        gcDetracciones.DataSource = dtTarget
    '        DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Los datos fueron cargados satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
    '    Catch ex As Exception
    '        DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Ocurrió un error durante la carga de datos, consulte con soporte. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    End Try
    'End Sub

    Private Sub SearchData()
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        Dim dtTarget As New DataTable
        Dim sCondition As String = ""
        If teRUC.Text.Trim <> "" Then
            sCondition &= IIf(sCondition = "", " where ", " and ") & "[Numero RUC Empresa]='" & lueSociedad.GetColumnValue("CompanyTaxCode") & "'"
        End If
        If tePeriodo.Text.Trim <> "" Then
            sCondition &= IIf(sCondition = "", " where ", " and ") & "[Periodo]='" & tePeriodo.Text & "'"
        End If
        If teRUC.Text.Trim <> "" Then
            sCondition &= IIf(sCondition = "", " where ", " and ") & "[Numero Documento Identidad]='" & teRUC.Text & "'"
        End If
        If seLote.EditValue > 0 Then
            sCondition &= IIf(sCondition = "", " where ", " and ") & "[Numero Lote]='" & seLote.Text & "'"
        End If
        dtTarget = oAppService.ExecuteSQL("select * from acc.DetraccionesPagos" & sCondition).Tables(0)
        gcDetracciones.DataSource = dtTarget
        GridView1.PopulateColumns()
        GridView1.Columns("Importe Deposito").SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
        GridView1.Columns("Importe Deposito").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        GridView1.Columns("Importe Deposito").DisplayFormat.FormatString = "n2"
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
        ExportarExcel(gcDetracciones)
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoOrigen.Properties.ButtonClick
        OpenFileDialog1.Filter = "Excel Files (*.xls*)|*.xls*"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.InitialDirectory = IIf(My.Settings.DetraSourceDirectory <> "", My.Settings.DetraSourceDirectory, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beArchivoOrigen.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub LoadInputValidations()
        Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

        containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
        containsValidationRule.ErrorText = "Asigne un valor."
        containsValidationRule.ErrorType = ErrorType.Critical

        Dim customValidationRule As New CustomValidationRule()
        customValidationRule.ErrorText = "Valor obligatorio."
        customValidationRule.ErrorType = ErrorType.Critical
        Validate()
        vpInputs.SetValidationRule(lueSociedad, Nothing)
        vpInputs.SetValidationRule(beArchivoOrigen, Nothing)
        vpInputs.SetValidationRule(seLote, Nothing)
        vpInputs.SetValidationRule(beArchivoSalida, Nothing)
        vpInputs.SetValidationRule(tePeriodo, Nothing)
        vpInputs.SetValidationRule(teRUC, Nothing)
        If LastButton = "bbiProcesar" Then
            vpInputs.SetValidationRule(lueSociedad, customValidationRule)
            vpInputs.SetValidationRule(beArchivoOrigen, customValidationRule)
            vpInputs.SetValidationRule(seLote, customValidationRule)
        ElseIf LastButton = "bbiGuardar" Then
            vpInputs.SetValidationRule(beArchivoSalida, customValidationRule)
        End If


    End Sub

    Private Sub FiltersEnabled()
        'If rbProcessType.SelectedIndex = 0 Then
        '    beArchivoOrigen.Enabled = True
        '    tePeriodo.Enabled = False
        '    teRUC.Enabled = False
        'Else
        '    beArchivoOrigen.Enabled = False
        '    tePeriodo.Enabled = True
        '    teRUC.Enabled = True
        'End If
    End Sub

    Private Sub rbProcessType_SelectedIndexChanged(sender As Object, e As EventArgs)
        LoadInputValidations()
        FiltersEnabled()
        seLote.EditValue = 0
    End Sub

    Private Sub FillCompany()
        Dim dtQuery As New DataTable
        dtQuery = oAppService.ExecuteSQL(" SELECT * FROM acc.Company").Tables(0)
        lueSociedad.Properties.DataSource = dtQuery
        lueSociedad.Properties.DisplayMember = "CompanyDescription"
        lueSociedad.Properties.ValueMember = "CompanyCode"
    End Sub

    Private Sub lueSociedad_EditValueChanged(sender As Object, e As EventArgs) Handles lueSociedad.EditValueChanged
        seLote.EditValue = oAppService.ExecuteSQL("select iif(IsNull(max([Numero Lote])),  0,  max([Numero Lote])) + 1 as Lote from PagosDetracciones where [Numero RUC Empresa] = '" & lueSociedad.GetColumnValue("CompanyTaxCode") & "'").Tables(0).Rows(0)(0)
        DetraFileName = "D" & lueSociedad.GetColumnValue("CompanyTaxCode") & seLote.Text & ".TXT"
        If My.Settings.DetraTargetDirectory <> "" Then
            beArchivoSalida.EditValue = FolderBrowserDialog1.SelectedPath & "\" & DetraFileName
        End If
    End Sub

    Private Sub beArchivoSalida_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoSalida.Properties.ButtonClick
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            beArchivoSalida.Text = FolderBrowserDialog1.SelectedPath & "\" & DetraFileName
        End If
    End Sub

    Private Sub seLote_Properties_EditValueChanged(sender As Object, e As EventArgs) Handles seLote.Properties.EditValueChanged
        DetraFileName = "D" & lueSociedad.GetColumnValue("RUC") & seLote.Text & ".TXT"
        beArchivoSalida.Text = FolderBrowserDialog1.SelectedPath & "\" & DetraFileName
    End Sub

    Private Sub GridView1_RowCellStyle(ByVal sender As Object, ByVal e As RowCellStyleEventArgs) Handles GridView1.RowCellStyle
        Dim View As GridView = sender
        If (e.RowHandle >= 0) Then
            If e.Column.FieldName = "C6" Then 'Cuenta Corriente Proveedor
                Dim C6 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6"))
                If C6.Trim = "" Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If
            End If
            If e.Column.FieldName = "C10" Or e.Column.FieldName = "C11" Or e.Column.FieldName = "C12" Then 'Documento Proveedor
                Dim C10 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("C10"))
                Dim C11 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("C11"))
                Dim C12 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12"))
                If C10.Trim = "" Or C11.Trim = "" Or C12.Trim = "" Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If
            End If
        End If
    End Sub

    Friend Function GetTextFormatValue(DocType As String, Group As String, Value As String) As String
        Dim sResult As String = ""
        Dim iPositions As Integer = GetPositionsByDocType(DocType, Group)
        Try
            If Group = "NroSer" Then
                If DocType = "05" Then
                    Return "3"
                ElseIf DocType = "10" Then
                    Return "1683"
                ElseIf DocType = "22" Then
                    Return "0820"
                Else
                    Value = Strings.Left(Value, InStr(Value, "-") - IIf(Value.Contains("-"), 1, 0))
                End If
            ElseIf Group = "NroDoc" Then
                If InStr(Value, "-") > 0 Then
                    Value = Mid(Value, InStr(Value, "-") + 1, iPositions)
                End If
            End If
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error en la función GetTextFormatValue. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        sResult = Strings.Right(StrDup(iPositions, "0") & Value.ToString.Trim, iPositions)
        Return sResult
    End Function

    Friend Function GetPositionsByDocType(DocType As String, Group As String) As Integer
        Dim iResult As Integer = 0
        Try
            If Group = "NroSer" Then
                iResult = dtTypePaytDoc.Select("Código = '" & DocType & "'")(0).ItemArray(2)
            ElseIf Group = "NroDoc" Then
                iResult = dtTypePaytDoc.Select("Código = '" & DocType & "'")(0).ItemArray(3)
            End If
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error en la función GetPositionsByDocType. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return iResult
    End Function

    Friend Function DataValidation(column As String, value As String) As String
        Dim sResult As String = ""
        If column = "TipDoc" Then
            If dtTypePaytDoc.Select("Código = '" & value & "'").Length > 0 Then
                sResult = value
            End If
        End If
        'If sResult = "" Then
        '    bFlatFileGenerate = False
        'End If
        Return sResult

    End Function

    Private Sub bbiGuardar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiGuardar.ItemClick
        LastButton = e.Item.Name
        LoadInputValidations()
        If Not vpInputs.Validate Then
            Return
        End If
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        If bProcess Then
            If EliminaLote() Then
                If Not ActualizaLote() Then
                    SplashScreenManager.CloseForm(False)
                    DevExpress.XtraEditors.XtraMessageBox.Show("Se generó un error al actualizar el lote.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        Else
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El proceso identificó varios errores, por favor verifique la columna errores.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
        SplashScreenManager.CloseForm(False)
        DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Los datos se guardaron satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
        bbiConsultar.PerformClick()
    End Sub

    Private Sub bbiTxtSunat_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiTxtSunat.ItemClick
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        If GridView1.RowCount > 0 Then
            bProcess = True
        End If
        If bProcess Then
            If CreateTextFileWithHeaderAndDetail(beArchivoSalida.Text, dtHeader, dtDetail, True, False) Then
                SplashScreenManager.CloseForm(False)
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El archivo plano ha sido generado satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                SplashScreenManager.CloseForm(False)
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generó el archivo plano, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Se identificaron algunos errores en el proceso, no es posible generar el archivo plano.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub bbiConsultar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiConsultar.ItemClick
        LastButton = e.Item.Name
        LoadInputValidations()
        If Not vpInputs.Validate Then
            Return
        End If
        SearchData()
        EnableButtons(False)
    End Sub

    Private Sub EnableButtons(bEnable As Boolean)
        bbiGuardar.Enabled = bEnable
        bbiEliminar.Enabled = bEnable
        If GridView1.RowCount > 0 Then
            bbiEliminar.Enabled = True
        End If
        'bbiTxtSunat.Enabled = bEnable
    End Sub

    Private Sub bbiEliminar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiEliminar.ItemClick
        Validate()
        Dim aResult As New ArrayList
        If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Esta seguro de eliminar el lote?", "Salir", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            aResult.AddRange(oAppService.ExecuteSQLNonQuery("DELETE FROM PagosDetracciones where [Numero RUC Empresa] = '" & lueSociedad.GetColumnValue("CompanyTaxCode") & "' and [Numero Lote] = '" & seLote.Text & "'"))
            If aResult(0) = 0 Then
                XtraMessageBox.Show(aResult(1), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        End If
        bbiConsultar.PerformClick()
    End Sub
End Class