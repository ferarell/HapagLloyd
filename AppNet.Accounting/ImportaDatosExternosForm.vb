Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports System.Collections
Imports DevExpress.XtraSplashScreen
Imports System.Threading

Public Class ImportaDatosExternosForm
    Dim RUC, SunatFileName As String
    Dim LibroSunat As String = "BalanceComprobacion"
    Dim dsLibroSunat As New dsSunat
    Dim dsExcel As New DataSet
    Dim dtSource1, dtSource2, dtExchange, dtTypePaytDoc, dtAccountMapping, dtResult As New DataTable
    Dim bFlatFileGenerate As Boolean = True
    Dim bProcess As Boolean = True
    Dim LastButtonClick, WaitText As String
    Dim iPrc As Integer = 0
    Dim sTable As String = ""

    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().

    End Sub

    Private Sub BalanceComprobacionForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        seEjercicio.Value = Today.Year
        sePeriodo.Value = Today.Month
        FillCompany()
        FolderBrowserDialog1.SelectedPath = IIf(My.Settings.LedgerTargetDirectory3 <> "", My.Settings.LedgerTargetDirectory3, "")
        'DataSourceTableCreate()
    End Sub

     Private Sub FillCompany()
        lueSociedad.Properties.DataSource = FillDataTable("Company", "", "ACC")
        lueSociedad.Properties.DisplayMember = "CompanyDescription"
        lueSociedad.Properties.ValueMember = "CompanyCode"
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
        vpInputs.SetValidationRule(Me.beArchivoOrigen, Nothing)
        vpInputs.SetValidationRule(Me.lueSociedad, customValidationRule)
        vpInputs.SetValidationRule(Me.seEjercicio, customValidationRule)
        vpInputs.SetValidationRule(Me.seEjercicio, customValidationRule)
        If LastButtonClick = "bbiProcesar" Then
            vpInputs.SetValidationRule(Me.beArchivoOrigen, customValidationRule)
        End If
    End Sub

    Private Sub LoadTypePaytDoc()
        dtTypePaytDoc = FillDataTable("TipoComprobante", "", "ACC")
    End Sub

    Private Sub LoadAccountMapping()
        dtAccountMapping = FillDataTable("AccountMapping", "CompanyCode='" & lueSociedad.EditValue & "'", "ACC")
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiProcesar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesar.ItemClick
        LastButtonClick = e.Item.Name
        LoadInputValidations()
        LoadAccountMapping()
        If Not vpInputs.Validate Then
            Return
        End If
        sTable = "DetalleContable" & rgOrigen.EditValue
        Try
            bProcess = True
            dtSource1.Rows.Clear()
            dtSource2.Rows.Clear()
            dtResult.Rows.Clear()
            If Not RequirementsValidate() Then
                Return
            End If
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El proceso eliminará los datos existentes del periodo: " & seEjercicio.Text & Format(sePeriodo.EditValue, "00") & ", está seguro que desea continuar?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                Return
            End If
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            If Not DeleteDataFromDB() Then
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No fue posible eliminar los datos existentes, el proceso no continuará.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            If LoadDataSources() Then
                ExternalDataProcess()
            End If
            If bProcess Then
                SplashScreenManager.CloseForm(False)
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El proceso terminó satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
            gcExternalData.DataSource = dtResult
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
    End Sub

    Friend Function LoadDataSources() As Boolean
        'Dim bResult As Boolean = True
        Dim iStep As Integer = 1
        iPrc = OpenFileDialog1.FileNames.Count + 1
        For i = 0 To OpenFileDialog1.FileNames.Count - 1
            If OpenFileDialog1.FileNames(i).ToUpper.Contains(".XLS") Then
                Try
                    Dim dtBridge As New DataTable
                    If LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Select("[Posting Period] = '" & sePeriodo.Text & "'").Length > 0 Then
                        dtBridge = LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Select("[Posting Period] = '" & sePeriodo.Text & "'").CopyToDataTable
                        InsertDataFile(dtBridge, WaitText)
                    End If
                Catch ex As Exception
                    bProcess = False
                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error al cargar archivo " & OpenFileDialog1.FileNames(i) & ". " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit For
                End Try
            End If
        Next
        Return bProcess
    End Function

    Private Sub InsertDataFile(dtFile As DataTable, WaitText As String)
        Dim IniCta As String = Strings.Left(dtFile.Rows(0)(0), 2)
        Dim TipoCuenta As String = IIf(IniCta = "16", "Cuentas por Pagar", IIf(IniCta = "14", "Cuentas por Cobrar", "Cuentas No Asociadas"))
        WaitText = "Cargando " & TipoCuenta
        dtFile.Columns.Add("LocalAccount", GetType(String)).AllowDBNull = True
        dtFile.Columns.Add("ValuedAccount", GetType(String)).AllowDBNull = True
        If dtSource1.Columns.Count = 0 Then
            dtSource1 = dtFile.Clone
        End If
        Try
            For Each row As DataRow In dtFile.Rows
                SplashScreenManager.Default.SetWaitFormDescription(WaitText & " (Fila: " & (dtFile.Rows.IndexOf(row) + 1).ToString & " de " & dtFile.Rows.Count.ToString & ")")
                If row("Document Number").ToString.Contains({"94001234", "94001869", "10000425", "10000576"}) Then
                    Continue For
                End If
                Dim drAccount As DataRow = GetLocalAccount(CInt(row("G/L Account")))
                row("LocalAccount") = drAccount(0)
                row("ValuedAccount") = drAccount(2)
                If seEjercicio.Text & Format(sePeriodo.EditValue, "00") < "201605" Then
                    dtSource1.ImportRow(row)
                Else
                    If Month(row(9)) = sePeriodo.Text And drAccount(0) <> "599999999" Then 'And Mid(row(16), 1, 9).ToUpper <> "MIGRATION"
                        dtSource1.ImportRow(row)
                    End If
                End If
            Next
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Friend Function RequirementsValidate() As Boolean
        Dim bResult As Boolean = True
        'Dim dtQuery As New DataTable
        ''If seEjercicio.Text & Format(sePeriodo.EditValue, "00") < "201605" Then
        ''    Return bResult
        ''End If
        'Dim dExchangeDate As Date = LastDayOfMonth("01/" & Format(sePeriodo.EditValue, "00/") & seEjercicio.Text)
        'dtExchange.Rows.Clear()
        'dtExchange = ExecuteAccessQuery("select * from TiposCambio unlock where Fecha=#" & dExchangeDate.ToShortDateString & "# and TipoRegistro='M'").Tables(0)
        'dtQuery = ExecuteAccessQuery("select distinct Moneda from DetalleContable where Sociedad='" & lueSociedad.EditValue & "' and Periodo='" & seEjercicio.Text & Format(sePeriodo.EditValue, "00") & "'").Tables(0)
        'For Each row As DataRow In dtQuery.Rows
        '    If dtExchange.Select("CodigoMoneda='" & row(0) & "'").Length = 0 Then
        '        DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No existe tipo de cambio para aplicar la valoración, por favor ingrese el tipo de cambio mensual de la moneda " & row(0) & ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        '        bResult = False
        '        Exit For
        '    End If
        'Next
        Return bResult
    End Function

    Private Sub lueSociedad_EditValueChanged(sender As Object, e As EventArgs) Handles seEjercicio.EditValueChanged, sePeriodo.EditValueChanged
        If lueSociedad.EditValue <> "" Then
            LoadAccountMapping()
        End If
        OriginAssign()
    End Sub

    Private Sub ExternalDataProcess()
        Dim dtInitialBalance As New DataTable
        bProcess = True
        Try
            dtResult = ExecuteAccessQuery("select * from " & sTable & " unlock where sociedad='#'").Tables(0) 'Detalle Contable
            If dtSource1.Rows.Count > 0 Then
                dtSource1 = dtSource1.Select("", "LocalAccount").CopyToDataTable
                'LoadInitialBalance(dtInitialBalance)
                For Each row As DataRow In dtSource1.Rows
                    If bProcess And row(0).ToString <> "" Then
                        SplashScreenManager.Default.SetWaitFormDescription("Procesando Movimiento Contable (" & (dtSource1.Rows.IndexOf(row) + 1).ToString & " de " & dtSource1.Rows.Count.ToString & ")")
                        NewDataRow(row)
                    Else
                        Exit For
                    End If
                Next
                'If bProcess Then
                '    UpdateFinalBalance()
                'End If
            End If
            'gcResumen.DataSource = dtResult1
            'gcExternalData.DataSource = dtResult
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    'Private Sub LoadInitialBalance(dtIniBal)
    '    Dim iPos As Integer = 0
    '    If dtIniBal.rows.count > 0 Then
    '        For Each row As DataRow In dtIniBal.rows
    '            SplashScreenManager.Default.SetWaitFormDescription("Cargando Saldo Inicial Contable (" & (dtIniBal.Rows.IndexOf(row) + 1).ToString & " de " & dtIniBal.Rows.Count.ToString & ")")
    '            If dtResult1.Select("CuentaLocal='" & row("CuentaLocal") & "' and Moneda='" & rgMoneda.EditValue & "'").Length = 0 Then
    '                dtResult1.Rows.Add()
    '                iPos = dtResult1.Rows.Count - 1
    '                dtResult1.Rows(iPos).Item("Sociedad") = row("Sociedad")
    '                dtResult1.Rows(iPos).Item("Periodo") = seEjercicio.Text & Format(sePeriodo.EditValue, "00")
    '                dtResult1.Rows(iPos).Item("Moneda") = row("Moneda")
    '                dtResult1.Rows(iPos).Item("CuentaLocal") = row("CuentaLocal")
    '                dtResult1.Rows(iPos).Item("CuentaExterna") = row("CuentaExterna")
    '                dtResult1.Rows(iPos).Item("debe_ini") = row("debe_fin")
    '                dtResult1.Rows(iPos).Item("haber_ini") = row("haber_fin")
    '                dtResult1.Rows(iPos).Item("debe_mov") = "0.00"
    '                dtResult1.Rows(iPos).Item("haber_mov") = "0.00"
    '                dtResult1.Rows(iPos).Item("debe_fin") = "0.00"
    '                dtResult1.Rows(iPos).Item("haber_fin") = "0.00"
    '            End If
    '        Next
    '    End If
    'End Sub

    'Private Sub UpdateFinalBalance()
    '    Dim bSaldo As Double = 0
    '    If seEjercicio.Text & Format(sePeriodo.EditValue, "00") < "201605" Then
    '        Return
    '    End If
    '    For Each row As DataRow In dtResult1.Rows
    '        SplashScreenManager.Default.SetWaitFormDescription("Actualizando Saldo Final Contable (" & (dtResult1.Rows.IndexOf(row) + 1).ToString & " de " & dtResult1.Rows.Count.ToString & ")")
    '        row("debe_fin") = "0.00"
    '        row("haber_fin") = "0.00"
    '        bSaldo = (row("debe_ini") - row("haber_ini")) + (row("debe_mov") - row("haber_mov"))
    '        If bSaldo >= 0 Then
    '            row("debe_fin") = bSaldo
    '        Else
    '            row("haber_fin") = bSaldo * -1
    '        End If
    '        row.AcceptChanges()
    '        If bSaldo <> 0 Then
    '            If Not InsertIntoAccess("SaldoContableGR", row) Then
    '                bProcess = False
    '            End If
    '        End If
    '    Next
    'End Sub

    Friend Function DeleteDataFromDB() As Boolean
        SplashScreenManager.Default.SetWaitFormDescription("Eliminando datos del periodo...")
        Dim bResult As Boolean = True
        Try
            If Not ExecuteAccessNonQuery("delete from " & sTable & " where Sociedad='" & lueSociedad.EditValue & "' and Periodo='" & seEjercicio.Text & Format(sePeriodo.EditValue, "00") & "'") Then
                bResult = False
            End If
            'If seEjercicio.Text & Format(sePeriodo.EditValue, "00") > "201604" Then
            '    If Not ExecuteAccessNonQuery("delete from SaldoContableGR where Sociedad='" & lueSociedad.EditValue & "' and Periodo='" & seEjercicio.Text & Format(sePeriodo.EditValue, "00") & "' and Moneda='" & rgMoneda.EditValue & "'") Then
            '        bResult = False
            '        sTable = "SaldoContable"
            '    End If
            'End If
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No fue posible eliminar los datos existentes de la tabla: " & sTable, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return bResult
    End Function

    Friend Sub NewDataRow(row As DataRow)
        Dim iPos1 As Integer = 0
        Dim iPos2 As Integer = 0
        Dim drAccount As DataRow = GetLocalAccount(row("G/L Account"))
        Dim ClCtb As String = GetAmountPosition(row(10))
        Try
            'Detail
            dtResult.Rows.Add()
            iPos2 = dtResult.Rows.Count - 1
            For c = 0 To row.ItemArray.Count - 1
                If row.Table.Columns(c).DataType.Name = "String" Then
                    If IsDBNull(row(c)) Then
                        row(c) = ""
                    End If
                End If
            Next
            dtResult.Rows(iPos2).Item("Sociedad") = lueSociedad.EditValue
            dtResult.Rows(iPos2).Item("Periodo") = seEjercicio.Text & Format(sePeriodo.EditValue, "00")
            dtResult.Rows(iPos2).Item("CuentaLocal") = drAccount(0)
            dtResult.Rows(iPos2).Item("CuentaExterna") = row(0)
            dtResult.Rows(iPos2).Item("Moneda") = row(11)
            dtResult.Rows(iPos2).Item("AsientoContable") = row(2)
            dtResult.Rows(iPos2).Item("Posicion") = row(3)
            dtResult.Rows(iPos2).Item("FechaContable") = row(9)
            dtResult.Rows(iPos2).Item("FechaDocumento") = row(8)
            dtResult.Rows(iPos2).Item("TipoDocumento") = row(7)
            dtResult.Rows(iPos2).Item("TextoCabecera") = Replace(row(16), "'", "")
            dtResult.Rows(iPos2).Item("ClaveContable") = row(10)
            dtResult.Rows(iPos2).Item("CuentaAsociada") = row("Account")
            dtResult.Rows(iPos2).Item("Referencia") = row(5)
            dtResult.Rows(iPos2).Item("IndicadorImpuesto") = row(24)
            dtResult.Rows(iPos2).Item("DocComp") = row(25)
            dtResult.Rows(iPos2).Item("ImporteMD") = row(12)
            dtResult.Rows(iPos2).Item("ImporteDebeML") = "0.00"
            dtResult.Rows(iPos2).Item("ImporteDebeMLV") = "0.00"
            If ClCtb = "D" Then
                dtResult.Rows(iPos2).Item("ImporteDebeML") = row(14)
                dtResult.Rows(iPos2).Item("ImporteDebeMLV") = row(14)
                'If aAccount(2) = "Y" Then
                '    dtResult.Rows(iPos2).Item("ImporteDebeMLV") = GetExchangeRate(row(11), row(12), "L", row(14))
                'End If
            End If
            dtResult.Rows(iPos2).Item("ImporteHaberML") = "0.00"
            dtResult.Rows(iPos2).Item("ImporteHaberMLV") = "0.00"
            If ClCtb = "H" Then
                dtResult.Rows(iPos2).Item("ImporteHaberML") = row(14) * -1
                dtResult.Rows(iPos2).Item("ImporteHaberMLV") = row(14) * -1
                'If aAccount(2) = "Y" Then
                '    dtResult.Rows(iPos2).Item("ImporteHaberMLV") = GetExchangeRate(row(11), row(12), "L", row(14)) * -1
                'End If
            End If
            dtResult.Rows(iPos2).Item("ImporteDebeME") = "0.00"
            dtResult.Rows(iPos2).Item("ImporteDebeMEV") = "0.00"
            If ClCtb = "D" Then
                dtResult.Rows(iPos2).Item("ImporteDebeME") = row(13)
                dtResult.Rows(iPos2).Item("ImporteDebeMEV") = row(13)
                'If aAccount(2) = "Y" Then
                '    dtResult.Rows(iPos2).Item("ImporteDebeMEV") = GetExchangeRate(row(11), row(12), "E", row(13))
                'End If
            End If
            dtResult.Rows(iPos2).Item("ImporteHaberME") = "0.00"
            dtResult.Rows(iPos2).Item("ImporteHaberMEV") = "0.00"
            If ClCtb = "H" Then
                dtResult.Rows(iPos2).Item("ImporteHaberME") = row(13) * -1
                dtResult.Rows(iPos2).Item("ImporteHaberMEV") = row(13) * -1
                'If aAccount(2) = "Y" Then
                '    dtResult.Rows(iPos2).Item("ImporteHaberMEV") = GetExchangeRate(row(11), row(12), "E", row(13)) * -1
                'End If
            End If
            'row(26) = IIf(IsDBNull(row(26)), "", row(26))
            If IsDate(row(26)) Then
                dtResult.Rows(iPos2).Item("FecComp") = CDate(row(26))
            End If
            dtResult.Rows(iPos2).AcceptChanges()
            If ExecuteAccessQuery("select * from " & sTable & " where AsientoContable='" & row(2) & "' and Posicion=" & row(3).ToString).Tables(0).Rows.Count = 0 Then
                If Not InsertIntoAccess(sTable, dtResult.Rows(iPos2)) Then
                    bProcess = False
                End If
            End If
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPos2.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Function GetExchangeRate(currency As String, amount As Double, curcol As String, amount_e As Double) As Double
        Dim dNewAmount As Double = 1
        Dim dtQuery As New DataTable
        Try
            dtQuery = dtExchange.Select("CodigoMoneda='" & currency & "'").CopyToDataTable
            If dtQuery.Rows.Count > 0 Then
                If curcol = "L" Then
                    dNewAmount = amount * dtQuery.Rows(0)("TcLocalV")
                Else
                    dNewAmount = amount * dtQuery.Rows(0)("TcDolarV")
                End If
            End If
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error en la función GetExchangeRate. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return dNewAmount
    End Function

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoOrigen.Properties.ButtonClick
        OpenFileDialog1.Filter = "Archivos de Origen (*.xls*;*.txt;*.csv)|*.xls*;*.txt;*.csv"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.InitialDirectory = IIf(My.Settings.LedgerSourceDirectory3 <> "", My.Settings.LedgerSourceDirectory3, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beArchivoOrigen.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
        ExportarExcel(gcExternalData)
    End Sub

    Private Sub seEjercicio_Leave(sender As Object, e As EventArgs) Handles seEjercicio.Leave, sePeriodo.Leave
        If seEjercicio.EditValue > Year(Today).ToString Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El ejercicio no puede ser mayor al año en curso.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            sender.focus()
        End If
        If seEjercicio.EditValue & Format(sePeriodo.EditValue, "00") > Year(Today).ToString & Format(Month(Today), "00") Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El periodo no puede ser mayor al mes en curso.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            sender.focus()
        End If
    End Sub

    'Private Sub GridView1_RowCellStyle(ByVal sender As Object, ByVal e As RowCellStyleEventArgs)
    '    Dim View As GridView = sender
    '    If (e.RowHandle >= 0) Then
    '        If e.Column.FieldName = "C1" Then 'Periodo
    '            Dim C1 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("C1"))
    '            If Microsoft.VisualBasic.Strings.Left(C1, 6) <> seEjercicio.EditValue & Format(sePeriodo.EditValue, "00") Then
    '                e.Appearance.BackColor = Color.DeepSkyBlue
    '                e.Appearance.BackColor2 = Color.LightCyan
    '                bFlatFileGenerate = False
    '            End If
    '        End If
    '    End If
    'End Sub

    Friend Function GetStatus(RefDate As Date, DocDate As Date, IGV As Double, IsReversed As Boolean) As String
        Dim status As String = ""
        If IGV = 0 Then
            status = "0"
        Else
            If Format(RefDate, "yyyyMM") = Format(DocDate, "yyyyMM") Then
                status = "1"
            End If
            If Format(DocDate, "yyyyMM") < Format(RefDate, "yyyyMM") And DateDiff(DateInterval.Month, DocDate, RefDate) <= 12 Then
                status = "6"
            End If
            If Format(DocDate, "yyyyMM") < Format(RefDate, "yyyyMM") And DateDiff(DateInterval.Month, DocDate, RefDate) > 12 Then
                status = "7"
            End If
            If IsReversed Then
                status = "9"
            End If
        End If
        Return status
    End Function

    Friend Function GetLocalAccount(chart As String) As DataRow
        Dim drResult As DataRow = Nothing
        Dim dtResult As New DataTable
        If dtAccountMapping.Select("Account LIKE '%" & CInt(chart).ToString & "'").Length > 0 Then
            dtResult = dtAccountMapping.Select("Account LIKE '%" & CInt(chart).ToString & "'").CopyToDataTable
        End If
        If dtResult.Rows.Count > 0 Then
            drResult = dtResult.Rows(0)
        End If
        Return drResult
    End Function

    'Friend Function GetAccountData(chart As String) As ArrayList
    '    Dim aResult As New ArrayList
    '    Try
    '        aResult.AddRange({"", "", ""})
    '        If rgOrigen.EditValue = "GR" Then
    '            aResult(0) = CInt(chart).ToString
    '            If dtAccountMapping.Select("[G/L Local Account] = '" & aResult(0) & "'").Length > 0 Then
    '                If Not IsDBNull(dtAccountMapping.Select("[G/L Local Account] = '" & aResult(0) & "'")(0).Item(3)) Then
    '                    aResult(1) = dtAccountMapping.Select("[G/L Local Account] = '" & aResult(0) & "'")(0).Item(3)
    '                    If Not IsDBNull(dtAccountMapping.Select("[G/L Local Account] = '" & aResult(0) & "'")(0).Item(7)) Then
    '                        aResult(2) = dtAccountMapping.Select("[G/L Local Account] = '" & aResult(0) & "'")(0).Item(7)
    '                    End If
    '                End If
    '            End If
    '        Else
    '            If dtAccountMapping.Select("[G/L Account] LIKE '%" & chart & "'").Length > 0 Then
    '                If Not IsDBNull(dtAccountMapping.Select("[G/L Account] LIKE '%" & chart & "'")(0).Item(2)) Then
    '                    aResult(0) = dtAccountMapping.Select("[G/L Account] LIKE '%" & chart & "'")(0).Item(2)
    '                End If
    '                If dtAccountMapping.Select("[G/L Local Account] = '" & aResult(0) & "'").Length > 0 Then
    '                    If Not IsDBNull(dtAccountMapping.Select("[G/L Local Account] = '" & aResult(0) & "'")(0).Item(3)) Then
    '                        aResult(1) = dtAccountMapping.Select("[G/L Local Account] = '" & aResult(0) & "'")(0).Item(3)
    '                        If Not IsDBNull(dtAccountMapping.Select("[G/L Local Account] = '" & aResult(0) & "'")(0).Item(7)) Then
    '                            aResult(2) = dtAccountMapping.Select("[G/L Local Account] = '" & aResult(0) & "'")(0).Item(7)
    '                        End If
    '                    End If
    '                End If
    '            End If
    '        End If
    '    Catch ex As Exception
    '        DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error en la función GetAccountData. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    End Try
    '    Return aResult
    'End Function

    Private Sub bbiConsultar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiConsultar.ItemClick
        Dim dtQuery As New DataTable
        sTable = "DetalleContable" & rgOrigen.EditValue
        dtQuery = ExecuteAccessQuery("select * from " & sTable & " unlock where sociedad='" & lueSociedad.EditValue & "' and periodo='" & seEjercicio.Text & Format(sePeriodo.EditValue, "00") & "' order by CuentaLocal, Periodo").Tables(0)
        gcExternalData.DataSource = dtQuery
        'LastButtonClick = e.Item.Name
        'LoadInputValidations()
        'If Not vpInputs.Validate Then
        '    Return
        'End If
        'Dim dtQuery, dtSummaryView As New DataTable
        'Dim sCurrPeriod As String = seEjercicio.Text & Format(sePeriodo.EditValue, "00")
        'Dim sPrevPeriod As String = PreviousPeriod(sCurrPeriod)
        'Dim sMoneda As String = IIf(rgMoneda.SelectedIndex = 0, "ML", "ME")
        'Dim iPosition As Integer = 0
        ''dtQuery = ExecuteAccessQuery("select * from SaldoContableQry1 unlock where sociedad='" & lueSociedad.EditValue & "' and periodo='" & sCurrPeriod & "' and moneda='" & rgMoneda.EditValue & "' order by CuentaLocal, Periodo").Tables(0)
        'dtQuery = ExecuteAccessQueryWP("select * from BalanceComprobacionQry2 unlock where sociedad='" & lueSociedad.EditValue & "'", "sPeriodo", sCurrPeriod).Tables(0)
        'dtSummaryView = dsLibroSunat.Tables("BalanceComprobacion")
        'dtSummaryView.Rows.Clear()
        'For Each row As DataRow In dtQuery.Rows
        '    If dtSummaryView.Select("C1=" & row(3)).Length = 0 Then
        '        dtSummaryView.Rows.Add()
        '    End If
        '    iPosition = dtSummaryView.Rows.Count - 1
        '    dtSummaryView.Rows(iPosition).Item("C1") = row("Cuenta")
        '    dtSummaryView.Rows(iPosition).Item("C2") = row("Descripcion")
        '    dtSummaryView.Rows(iPosition).Item("C3") = row("SaldoIniDebe" & sMoneda)
        '    dtSummaryView.Rows(iPosition).Item("C4") = row("SaldoIniHaber" & sMoneda) * -1
        '    dtSummaryView.Rows(iPosition).Item("C5") = row("MovDebe" & sMoneda)
        '    dtSummaryView.Rows(iPosition).Item("C6") = row("MovHaber" & sMoneda) * -1
        '    dtSummaryView.Rows(iPosition).Item("C7") = row("SaldoFinDebe" & sMoneda)
        '    dtSummaryView.Rows(iPosition).Item("C8") = row("SaldoFinHaber" & sMoneda) * -1
        '    If Strings.Left(row("Cuenta"), 1) >= 1 And Strings.Left(row("Cuenta"), 1) <= 5 Then
        '        dtSummaryView.Rows(iPosition).Item("C9") = dtSummaryView.Rows(iPosition).Item("C7")
        '        dtSummaryView.Rows(iPosition).Item("C10") = dtSummaryView.Rows(iPosition).Item("C8")
        '    End If
        '    If Strings.Left(row("Cuenta"), 1) >= 6 Then
        '        dtSummaryView.Rows(iPosition).Item("C11") = dtSummaryView.Rows(iPosition).Item("C7")
        '        dtSummaryView.Rows(iPosition).Item("C12") = dtSummaryView.Rows(iPosition).Item("C8")
        '    End If
        '    dtSummaryView.Rows(iPosition).Item("C13") = ""
        'Next
        'gcResumen.DataSource = dtSummaryView
        ''GridView1.PopulateColumns()
        'For i = 3 To 12
        '    GridView1.Columns("C" & i.ToString).SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
        '    GridView1.Columns("C" & i.ToString).DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        '    GridView1.Columns("C" & i.ToString).DisplayFormat.FormatString = "n2"
        'Next
        'GetAccountDetail(GridView1.GetFocusedRowCellValue("C1"))
        ''GridColumnsHide()
    End Sub

    Friend Function PreviousPeriod(period As String) As String
        Dim sResult As String = ""
        sResult = Format(DateAdd(DateInterval.Day, -1, CDate("01/" & Format(sePeriodo.Value, "00/") & seEjercicio.Value)), "yyyyMM")
        Return sResult
    End Function

    'Private Sub GridView1_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs)
    '    If LastButtonClick = "bbiConsultar" Then
    '        GetAccountDetail(GridView1.GetFocusedRowCellValue("C1"))
    '    Else
    '        GetAccountDetail(GridView1.GetFocusedRowCellValue("CuentaLocal"))
    '    End If
    'End Sub

    'Private Sub GetAccountDetail(chart As String)
    '    Dim dtQuery, dtSummaryView As New DataTable
    '    Dim sCurrPeriod As String = seEjercicio.Text & Format(sePeriodo.EditValue, "00")
    '    Dim iPosition As Integer = 0
    '    dtQuery = ExecuteAccessQuery("select * from DetalleContableGRQry1 unlock where Sociedad='" & lueSociedad.EditValue & "' and Periodo<='" & seEjercicio.Text & Format(sePeriodo.EditValue, "00") & "' and CuentaLocal='" & chart & "'").Tables(0)
    '    gcExternalData.DataSource = dtQuery
    '    'GridView2.PopulateColumns()
    '    For i = 9 To GridView1.Columns.Count - 1
    '        If i > 9 Then
    '            GridView1.Columns(i).SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
    '        End If
    '        GridView1.Columns(i).DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
    '        GridView1.Columns(i).DisplayFormat.FormatString = "n2"
    '    Next
    'End Sub

    Private Sub bbiGenerarArchivoPDT_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiGenerarArchivoPDT.ItemClick
        LastButtonClick = e.Item.Name
        LoadInputValidations()
    End Sub

    Friend Function GetAmountPosition(pk As Integer) As String
        Dim PosCol As String = "H"
        If (pk >= 1 And pk <= 10) Or (pk >= 21 And pk <= 30) Or pk = 40 Or pk = 70 Or (pk >= 80 And pk <= 89) Then
            PosCol = "D"
        End If
        Return PosCol
    End Function

    'Private Sub SunatFlatFileGenerate()
    '    If bFlatFileGenerate Then
    '        If CreateTextDelimiterFile(beArchivoSalida.EditValue, dsLibroSunat.Tables(LibroSunat), "|", False, False) Then
    '            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El archivo plano ha sido generado satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
    '        Else
    '            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generó el archivo plano, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        End If
    '    End If
    'End Sub

    Private Sub sePeriodo_ValueChanged(sender As Object, e As EventArgs) Handles sePeriodo.ValueChanged
        OriginAssign()
    End Sub

    Private Sub OriginAssign()
        If seEjercicio.Text = "2016" And sePeriodo.EditValue < 5 Then
            rgOrigen.SelectedIndex = 0
        Else
            rgOrigen.SelectedIndex = 1
        End If
    End Sub

End Class