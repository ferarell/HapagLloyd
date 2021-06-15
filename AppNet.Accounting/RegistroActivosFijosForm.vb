Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.Collections

Public Class RegistroActivosFijosForm
    Dim RUC, SunatFileName, SunatLedger As String
    Dim LibroSunat As String = ""
    Dim dsLibroSunat As New dsSunat
    'Dim dsExcel As New DataSet
    'Dim dtResult, dtProcess, dtTypePaytDoc, dtPaytTerms As New DataTable
    Dim dtAccountMapping, dtResult As New DataTable
    Dim bFlatFileGenerate As Boolean = True
    Dim bProcess As Boolean = True

    Private Sub LibroInventariosBalancesForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        seEjercicio.Value = Today.Year
        'sePeriodo.Value = Today.Month
        FillCompany()
        FillLedgerList()
        FolderBrowserDialog1.SelectedPath = IIf(My.Settings.LedgerTargetDirectory7 <> "", My.Settings.LedgerTargetDirectory7, "")
    End Sub

    Private Sub beArchivoSalida_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoSalida.Properties.ButtonClick
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            beArchivoSalida.EditValue = FolderBrowserDialog1.SelectedPath & "\" & SunatFileName
        End If
    End Sub

    Private Sub FillCompany()
        lueSociedad.Properties.DataSource = FillDataTable("Company", "", "ACC")
        lueSociedad.Properties.DisplayMember = "CompanyDescription"
        lueSociedad.Properties.ValueMember = "CompanyCode"
    End Sub

    Private Sub FillLedgerList()
        lueReport.Properties.DataSource = FillDataTable("LibrosRegistrosSunat", "CodigoLibro=7", "ACC")
        lueReport.Properties.DisplayMember = "NombreLibro"
        lueReport.Properties.ValueMember = "CodigoEstructura"
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
        vpLedger.SetValidationRule(Me.lueSociedad, customValidationRule)
        vpLedger.SetValidationRule(Me.seEjercicio, customValidationRule)
        vpLedger.SetValidationRule(Me.seEjercicio, customValidationRule)
        vpLedger.SetValidationRule(Me.beArchivoOrigen, customValidationRule)
        vpLedger.SetValidationRule(Me.beArchivoSalida, customValidationRule)
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiProcesar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesar.ItemClick
        LoadInputValidations()
        If Not vpLedger.Validate Then
            Return
        End If
        dtAccountMapping.Rows.Clear()
        dtAccountMapping = ExecuteAccessQuery("SELECT Account, LocalAccount FROM AccountMapping WHERE CompanyCode='" & lueSociedad.EditValue & "'").Tables(0)
        bFlatFileGenerate = True
        bProcess = True
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            dsLibroSunat.Tables(LibroSunat).Rows.Clear()
            ProcessLedger()
        Catch ex As Exception
            bProcess = False
            SplashScreenManager.CloseForm(False)
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
        gcLibroSunat.DataSource = dtResult 'dsLibroSunat.Tables(LibroSunat)
        gcLibroSunat.RefreshDataSource()
    End Sub

    Private Sub lueSociedad_EditValueChanged(sender As Object, e As EventArgs) Handles lueSociedad.EditValueChanged, seEjercicio.EditValueChanged, lueReport.EditValueChanged
        If lueSociedad.EditValue <> "" Then
            GetSunatFileName(My.Settings.LedgerTargetDirectory7)
        End If
        LibroSunat = "RegistroActivosFijos" & lueReport.GetColumnValue("SubLibro")
    End Sub

    Private Sub GetSunatFileName(sPath As String)
        Dim sFileName As String = ""
        If sPath = "" Then
            If OpenFileDialog1.FileName <> "" Then
                sPath = IO.Path.GetDirectoryName(OpenFileDialog1.FileName)
            Else
                sPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            End If
        End If
        sFileName = sPath & "\LE" & lueSociedad.GetColumnValue("CompanyTaxCode") & seEjercicio.Text & "0000" & "070" & lueReport.GetColumnValue("SubLibro") & "00" & "00" & "1" & lueReport.GetColumnValue("IndicadorContenido") & "11" & ".TXT"
        beArchivoSalida.EditValue = sFileName
    End Sub

    Private Sub ProcessLedger()
        Dim SourceFile As String = beArchivoOrigen.Text
        Dim dtSource As New DataTable
        dtSource = LoadExcel(SourceFile, "{0}").Tables(0)
        If dtSource.Rows.Count > 0 Then
            Try
                dtResult = dsLibroSunat.Tables(LibroSunat)
                For Each row As DataRow In dtSource.Rows
                    If bProcess Then
                        If Not IsDBNull(row(0)) Then
                            NewRowLedger(row)
                        End If
                    End If
                Next
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Friend Sub NewRowLedger(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        For i = 0 To 16
            If IsDBNull(row(i)) Then
                If row.Table.Columns(i).DataType Is System.Type.GetType("System.String") Then
                    row(i) = ""
                ElseIf row.Table.Columns(i).DataType Is System.Type.GetType("System.Double") Or row.Table.Columns(i).DataType Is System.Type.GetType("System.Double") Then
                    row(i) = 0
                End If
            End If
        Next
        If Not IsNumeric(row(1)) Then
            Return
        End If
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = seEjercicio.Text & "0000" 'Format(sePeriodo.EditValue, "00") & "00"
            dtResult.Rows(iPosition).Item("C2") = GetDocumentoSAP(row(0).ToString)
            dtResult.Rows(iPosition).Item("C3") = "M" & dtResult.Select("C2='" & dtResult.Rows(iPosition).Item("C2") & "'").Length
            dtResult.Rows(iPosition).Item("C4") = "9"
            dtResult.Rows(iPosition).Item("C5") = row(0)
            dtResult.Rows(iPosition).Item("C6") = ""
            dtResult.Rows(iPosition).Item("C7") = GetCodigoExistenciaSunat()
            dtResult.Rows(iPosition).Item("C8") = "1"
            dtResult.Rows(iPosition).Item("C9") = GetCuentaContable(row(17).ToString)
            dtResult.Rows(iPosition).Item("C10") = "9"
            dtResult.Rows(iPosition).Item("C11") = Mid(row(5).ToString, 1, 40)
            dtResult.Rows(iPosition).Item("C12") = "-"
            dtResult.Rows(iPosition).Item("C13") = "-"
            dtResult.Rows(iPosition).Item("C14") = "-"
            dtResult.Rows(iPosition).Item("C15") = Format(row(8), "0.00")
            dtResult.Rows(iPosition).Item("C16") = Format(row(9), "0.00")
            dtResult.Rows(iPosition).Item("C17") = "0.00"
            dtResult.Rows(iPosition).Item("C18") = Format(row(12), "0.00")
            dtResult.Rows(iPosition).Item("C19") = "0.00"
            dtResult.Rows(iPosition).Item("C20") = "0.00"
            dtResult.Rows(iPosition).Item("C21") = "0.00"
            dtResult.Rows(iPosition).Item("C22") = "0.00"
            dtResult.Rows(iPosition).Item("C23") = "0.00"
            If IsDate(row(4)) Then
                dtResult.Rows(iPosition).Item("C24") = Format(CDate(row(4)), "dd/MM/yyyy")
                dtResult.Rows(iPosition).Item("C25") = Format(CDate(row(4)), "dd/MM/yyyy")
            End If
            dtResult.Rows(iPosition).Item("C26") = "1"
            dtResult.Rows(iPosition).Item("C27") = "0"
            If Not (IsDBNull(row(6)) Or row(6).ToString = "") Then
                If IsNumeric(Mid(row(6), 1, 3)) Then
                    dtResult.Rows(iPosition).Item("C28") = Format(CDec(Mid(row(6), 1, 3)), "000.00")
                End If
            End If
            dtResult.Rows(iPosition).Item("C29") = Format(row(15), "0.00")
            dtResult.Rows(iPosition).Item("C30") = Format(row(14), "0.00")
            dtResult.Rows(iPosition).Item("C31") = Format(row(13), "0.00")
            dtResult.Rows(iPosition).Item("C32") = "0.00"
            dtResult.Rows(iPosition).Item("C33") = "0.00"
            dtResult.Rows(iPosition).Item("C34") = "0.00"
            dtResult.Rows(iPosition).Item("C35") = "0.00"
            dtResult.Rows(iPosition).Item("C36") = "0.00"
            dtResult.Rows(iPosition).Item("C37") = "1"
            dtResult.Rows(iPosition).Item("ERR") = ""
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Function GetDocumentoSAP(AssetNo As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("SELECT Document FROM AssetDocumentMapping WHERE Asset='" & AssetNo & "'").Tables(0)
        If dtQuery.Rows.Count > 0 Then
            sResult = dtQuery.Rows(0)(0).ToString
        End If
        Return IIf(sResult = "", "16999999", sResult)
    End Function

    Friend Function GetCuentaContable(account As String) As String
        Dim sResult As String = ""
        If dtAccountMapping.Select("Account LIKE '%" & account & "'").Length > 0 Then
            sResult = dtAccountMapping.Select("Account LIKE '%" & account & "'")(0)("LocalAccount")
        End If
        Return sResult
    End Function

    Friend Function GetCodigoExistenciaSunat() As String
        Dim sResult As String = ""

        Return sResult
    End Function

    Private Sub SunatFlatFileGenerate()
        Validate()
        If bFlatFileGenerate Then
            'beArchivoSalida.EditValue = FolderBrowserDialog1.SelectedPath & "\LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "140100" & "00" & "1" & IIf(dtResult.Rows.Count = 0, "0", "1") & "11" & ".TXT"
            GetSunatFileName(My.Settings.LedgerTargetDirectory7)
            If CreateTextDelimiterFile(beArchivoSalida.EditValue, dsLibroSunat.Tables(LibroSunat), "|", False, False) Then
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El archivo plano ha sido generado satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generó el archivo plano, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Se identificaron algunos errores en el proceso, no es posible generar el archivo PLE.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoOrigen.Properties.ButtonClick
            OpenFileDialog1.Filter = "Excel Files (*.xls*)|*.xls*"
            OpenFileDialog1.FileName = ""
            OpenFileDialog1.InitialDirectory = IIf(My.Settings.LedgerSourceDirectory14 <> "", My.Settings.LedgerSourceDirectory14, "")
            If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
                beArchivoOrigen.Text = OpenFileDialog1.FileName
            End If
    End Sub

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
            ExportarExcel(gcLibroSunat)
    End Sub

    Public Sub New()

            ' Llamada necesaria para el diseñador.
            InitializeComponent()

            ' Agregue cualquier inicialización después de la llamada a InitializeComponent().

    End Sub

    Private Sub seEjercicio_Leave(sender As Object, e As EventArgs) Handles seEjercicio.Leave
        If seEjercicio.Text > Year(Today).ToString Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El ejercicio no puede ser mayor al año en curso.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            sender.focus()
        End If
        'If seEjercicio.Text & Format(sePeriodo.EditValue, "00") > Year(Today).ToString & Format(Month(Today), "00") Then
        '    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El periodo no puede ser mayor al mes en curso.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    sender.focus()
        'End If
    End Sub

    Private Sub GridView1_RowCellStyle(ByVal sender As Object, ByVal e As RowCellStyleEventArgs) Handles GridView1.RowCellStyle
            Dim View As GridView = sender
            If (e.RowHandle >= 0) Then
            'If e.Column.FieldName = "C1" Then 'Periodo
            '    Dim C1 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("C1"))
            '    If Microsoft.VisualBasic.Strings.Left(C1, 6) <> seEjercicio.EditValue & Format(sePeriodo.EditValue, "00") Then
            '        e.Appearance.BackColor = Color.DeepSkyBlue
            '        e.Appearance.BackColor2 = Color.LightCyan
            '        bFlatFileGenerate = False
            '    End If
            'End If
                'If e.Column.FieldName = "C4" Then 'Fecha Comprobante de Pago
                '    If Format(CDate(View.GetRowCellDisplayText(e.RowHandle, View.Columns("C4"))), "yyyyMM") > seEjercicio.EditValue & Format(sePeriodo.EditValue, "00") Then
                '        e.Appearance.BackColor = Color.Salmon
                '        e.Appearance.BackColor2 = Color.SeaShell
                '        bFlatFileGenerate = False
                '    End If
                'End If
                'If e.Column.FieldName = "C6" Then 'Tipo Comprobante de Pago
                '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "" Then
                '        e.Appearance.BackColor = Color.Salmon
                '        e.Appearance.BackColor2 = Color.SeaShell
                '        bFlatFileGenerate = False
                '    End If
                'End If
                'If e.Column.FieldName = "C7" Then 'Serie Comprobante de Pago
                '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C7")) = "" Then
                '        e.Appearance.BackColor = Color.Salmon
                '        e.Appearance.BackColor2 = Color.SeaShell
                '        bFlatFileGenerate = False
                '    End If
                'End If
                'If e.Column.FieldName = "C8" Then 'Número Comprobante de Pago
                '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C8")) = "" Then
                '        e.Appearance.BackColor = Color.Salmon
                '        e.Appearance.BackColor2 = Color.SeaShell
                '        bFlatFileGenerate = False
                '    End If
                'End If
                'If e.Column.FieldName = "C10" Then 'Tipo Documento de Identidad
                '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C10")) = "" Then
                '        e.Appearance.BackColor = Color.Salmon
                '        e.Appearance.BackColor2 = Color.SeaShell
                '        bFlatFileGenerate = False
                '    End If
                'End If
                'If e.Column.FieldName = "C11" Then 'Número Documento de Identidad
                '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C11")) = "" Then
                '        e.Appearance.BackColor = Color.Salmon
                '        e.Appearance.BackColor2 = Color.SeaShell
                '        bFlatFileGenerate = False
                '    End If
                'End If
                'If e.Column.FieldName = "C27" Then
                '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
                '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C27")) = "" And Not View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")).Contains("ANULAD") Then 'Fecha Comprobante de Pago que se modifica (NC)
                '            e.Appearance.BackColor = Color.Salmon
                '            e.Appearance.BackColor2 = Color.SeaShell
                '            bFlatFileGenerate = False
                '        End If
                '    End If
                'End If
                'If e.Column.FieldName = "C28" Then
                '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
                '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C28")) = "" And Not View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")).Contains("ANULAD") Then 'Tipo Comprobante de Pago que se modifica (NC)
                '            e.Appearance.BackColor = Color.Salmon
                '            e.Appearance.BackColor2 = Color.SeaShell
                '            bFlatFileGenerate = False
                '        End If
                '    End If
                'End If
                'If e.Column.FieldName = "C29" Then
                '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
                '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C29")) = "" And Not View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")).Contains("ANULAD") Then 'Serie Comprobante de Pago que se modifica (NC)
                '            e.Appearance.BackColor = Color.Salmon
                '            e.Appearance.BackColor2 = Color.SeaShell
                '            bFlatFileGenerate = False
                '        End If
                '    End If
                'End If
                'If e.Column.FieldName = "C30" Then
                '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
                '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C30")) = "" And Not View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")).Contains("ANULAD") Then 'Número Comprobante de Pago que se modifica (NC)
                '            e.Appearance.BackColor = Color.Salmon
                '            e.Appearance.BackColor2 = Color.SeaShell
                '            bFlatFileGenerate = False
                '        End If
                '    End If
                'End If
                'If e.Column.FieldName = "C34" Then 'Estado
                '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C34")) = "" Then
                '        e.Appearance.BackColor = Color.Peru
                '        e.Appearance.BackColor2 = Color.LightYellow
                '        bFlatFileGenerate = False
                '    End If
                'End If
            End If
    End Sub

    Friend Function GetStatus(RefDate As Date, DocDate As Date, IGV As Double, IsVoided As Boolean) As String
            Dim status As String = ""
            If IsVoided Then
                status = "2"
            Else
                If Format(RefDate, "yyyyMM") = Format(DocDate, "yyyyMM") Then
                    status = "1"
                End If
            End If
            Return status
    End Function

    Private Sub bbiSunatPle_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSunatPle.ItemClick
            SunatFlatFileGenerate()
    End Sub

End Class