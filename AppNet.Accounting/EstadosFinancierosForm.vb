Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing

Public Class EstadosFinancierosForm
    Dim RUC, SunatFileName As String
    Dim LibroSunat As String = "EstadosFinancieros"
    Dim dsLibroSunat As New dsSunat
    Dim dsExcel As New DataSet
    Dim dtResult, dtProcess, dtTypePaytDoc, dtPaytTerms As New DataTable
    Dim bFlatFileGenerate As Boolean = True

    Private Sub EstadosFinancierosForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        seEjercicio.Value = Today.Year
        sePeriodo.Value = Today.Month
        FillCompany()
        FolderBrowserDialog1.SelectedPath = IIf(My.Settings.LedgerTargetDirectory1 <> "", My.Settings.LedgerTargetDirectory1, "")
        LoadInputValidations()
        LoadPaytTerms()
        LoadTypePaytDoc()
    End Sub

    Private Sub beArchivoSalida_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoSalida.Properties.ButtonClick
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            beArchivoSalida.Text = FolderBrowserDialog1.SelectedPath & "\" & SunatFileName
        End If
    End Sub

    Private Sub FillCompany()
        lueSociedad.Properties.DataSource = FillDataTable("Company", "")
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
        vpLedger.SetValidationRule(Me.lueSociedad, customValidationRule)
        vpLedger.SetValidationRule(Me.seEjercicio, customValidationRule)
        vpLedger.SetValidationRule(Me.seEjercicio, customValidationRule)
        vpLedger.SetValidationRule(Me.beArchivoOrigen, customValidationRule)
        vpLedger.SetValidationRule(Me.beArchivoSalida, customValidationRule)
    End Sub

    Private Sub LoadPaytTerms()
        dtPaytTerms = FillDataTable("CondPago", "")
    End Sub

    Private Sub LoadTypePaytDoc()
        dtTypePaytDoc = FillDataTable("TipoComprobante", "")
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiProcesar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesar.ItemClick
        dsLibroSunat.Tables(LibroSunat).Rows.Clear()
        If vpLedger.Validate Then
            ProcessLedger()
        End If
        gcLibroSunat.DataSource = dsLibroSunat.Tables(LibroSunat)
        PivotGridControl1.DataSource = gcLibroSunat.DataSource
        PivotGridControl1.RefreshData()
        If CheckedComboBoxEdit1.SelectedIndex = 0 Then
            SunatFlatFileGenerate()
        End If
    End Sub

    Private Sub lueSociedad_EditValueChanged(sender As Object, e As EventArgs) Handles lueSociedad.EditValueChanged, seEjercicio.EditValueChanged, sePeriodo.EditValueChanged
        If lueSociedad.EditValue <> "" Then
            RUC = lueSociedad.GetColumnValue("CompanyTaxCode")
            SunatFileName = "LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "140100" & "00" & "1111" & ".TXT"
            If My.Settings.LedgerTargetDirectory1 <> "" Then
                beArchivoSalida.EditValue = FolderBrowserDialog1.SelectedPath & "\" & SunatFileName
            End If
        End If
    End Sub

    Private Sub ProcessLedger()
        Dim SourceFile As String = beArchivoOrigen.EditValue
        dsExcel = LoadExcel(SourceFile, "{0}")
        If dsExcel.Tables(0).Rows.Count > 0 Then
            Try
                dtResult = dsLibroSunat.Tables(LibroSunat)
                For Each row As DataRow In dsExcel.Tables(0).Rows
                    NewRowLedger(row)
                Next
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Friend Sub NewRowLedger(row As DataRow)
        Dim iPosition As Integer = 0
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = Format(row(1), "yyyyMM00")
            dtResult.Rows(iPosition).Item("C2") = row(0)
            dtResult.Rows(iPosition).Item("C3") = "M" & row(0)
            dtResult.Rows(iPosition).Item("C4") = Format(CDate(row(1)), "dd/MM/yyyy")
            dtResult.Rows(iPosition).Item("C5") = "" 'Format(DateAdd(DateInterval.Day, GetDueDays(row(2)), row.Item(1)), "dd/MM/yyyy")
            dtResult.Rows(iPosition).Item("C6") = IIf(row(6) > 0, "07", "01")
            If row(3).ToString.Trim <> "" Then
                dtResult.Rows(iPosition).Item("C7") = "00" & Microsoft.VisualBasic.Strings.Left(row(3).trim, 2)
                dtResult.Rows(iPosition).Item("C8") = Microsoft.VisualBasic.Strings.Right(row(3).trim, 7)
                dtResult.Rows(iPosition).Item("C9") = ""
            End If
            If row(5) <> "ANULADA" Then
                dtResult.Rows(iPosition).Item("C10") = IIf(row(4).ToString.Trim.Length = 11, "6", IIf(row(4).ToString.Trim.Length = 8, "1", "0"))
                If row(4).trim = "" Then
                    dtResult.Rows(iPosition).Item("C11") = GetRucByCia(row(5))
                Else
                    dtResult.Rows(iPosition).Item("C11") = row(4)
                End If
            Else
                dtResult.Rows(iPosition).Item("C10") = "6"
                dtResult.Rows(iPosition).Item("C11") = RUC
            End If
            dtResult.Rows(iPosition).Item("C12") = row(5)
            dtResult.Rows(iPosition).Item("C13") = "0"
            dtResult.Rows(iPosition).Item("C14") = Format(row(6) * -1, "###########0.00")
            dtResult.Rows(iPosition).Item("C15") = "0"
            dtResult.Rows(iPosition).Item("C16") = Format(row(8) * -1, "###########0.00")
            dtResult.Rows(iPosition).Item("C17") = "0"
            dtResult.Rows(iPosition).Item("C18") = "0"
            dtResult.Rows(iPosition).Item("C19") = Format(row(7) * -1, "###########0.00")
            dtResult.Rows(iPosition).Item("C20") = "0"
            dtResult.Rows(iPosition).Item("C21") = "0"
            dtResult.Rows(iPosition).Item("C22") = "0"
            dtResult.Rows(iPosition).Item("C23") = "0"
            dtResult.Rows(iPosition).Item("C24") = Format(row(9) * -1, "###########0.00")
            dtResult.Rows(iPosition).Item("C25") = row(14).trim
            If row(14).trim = "PEN" Then
                dtResult.Rows(iPosition).Item("C26") = "1.000"
            Else
                dtResult.Rows(iPosition).Item("C26") = Format(CDbl(row(12)), "0.000")
            End If
            If dtResult.Rows(iPosition).Item("C6") = "07" Then
                dtResult.Rows(iPosition).Item("C27") = Format(CDate(row(15)), "dd/MM/yyyy")
                dtResult.Rows(iPosition).Item("C28") = DataValidation("C6", row(16))
                dtResult.Rows(iPosition).Item("C29") = row(17)
                dtResult.Rows(iPosition).Item("C30") = row(18)
            End If
            dtResult.Rows(iPosition).Item("C31") = ""
            dtResult.Rows(iPosition).Item("C32") = ""
            dtResult.Rows(iPosition).Item("C33") = "1"
            dtResult.Rows(iPosition).Item("C34") = GetStatus("01/" & Format(sePeriodo.EditValue, "00/") & seEjercicio.Text, row(1), row(8), IIf(row(5) = "ANULADA", True, False))
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SunatFlatFileGenerate()
        If bFlatFileGenerate Then
            If CreateTextDelimiterFile(beArchivoSalida.EditValue, dsLibroSunat.Tables(LibroSunat), "|", False, False) Then
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El archivo plano ha sido generado satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generó el archivo plano, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If
    End Sub

    Friend Function GetRucByCia(CiaName As String) As String
        Dim CiaRUC As String = ""
        CiaRUC = dsExcel.Tables(0).Select("[Name 1] = '" & CiaName & "'")(0).ItemArray(4).ToString.Trim
        Return CiaRUC
    End Function

    Friend Function GetDueDays(PaytTerms As String) As Integer
        Dim iDays As Integer = 0
        iDays = DirectCast(dtPaytTerms.Select("Código = '" & PaytTerms & "'")(0).ItemArray(1), Double)
        Return iDays
    End Function

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoOrigen.Properties.ButtonClick
        OpenFileDialog1.Filter = "Excel Files (*.xls*)|*.xls*"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.InitialDirectory = IIf(My.Settings.LedgerSourceDirectory1 <> "", My.Settings.LedgerSourceDirectory1, "")
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

    Private Sub GridView1_RowCellStyle(ByVal sender As Object, ByVal e As RowCellStyleEventArgs) Handles GridView1.RowCellStyle
        Dim View As GridView = sender
        If (e.RowHandle >= 0) Then
            If e.Column.FieldName = "C1" Then 'Periodo
                Dim C1 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("C1"))
                If Microsoft.VisualBasic.Strings.Left(C1, 6) <> seEjercicio.EditValue & Format(sePeriodo.EditValue, "00") Then
                    e.Appearance.BackColor = Color.DeepSkyBlue
                    e.Appearance.BackColor2 = Color.LightCyan
                    bFlatFileGenerate = False
                End If
            End If
            If e.Column.FieldName = "C4" Then 'Fecha Comprobante de Pago
                If Format(CDate(View.GetRowCellDisplayText(e.RowHandle, View.Columns("C4"))), "yyyyMM") > seEjercicio.EditValue & Format(sePeriodo.EditValue, "00") Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                    bFlatFileGenerate = False
                End If
            End If
            If e.Column.FieldName = "C6" Then 'Tipo Comprobante de Pago
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "" Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                    bFlatFileGenerate = False
                End If
            End If
            If e.Column.FieldName = "C7" Then 'Serie Comprobante de Pago
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C7")) = "" Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                    bFlatFileGenerate = False
                End If
            End If
            If e.Column.FieldName = "C8" Then 'Número Comprobante de Pago
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C8")) = "" Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                    bFlatFileGenerate = False
                End If
            End If
            If e.Column.FieldName = "C10" Then 'Tipo Documento de Identidad
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C10")) = "" Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                    bFlatFileGenerate = False
                End If
            End If
            If e.Column.FieldName = "C11" Then 'Número Documento de Identidad
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C11")) = "" Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                    bFlatFileGenerate = False
                End If
            End If
            If e.Column.FieldName = "C27" Then
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C27")) = "" Then 'Fecha Comprobante de Pago que se modifica (NC)
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
            End If
            If e.Column.FieldName = "C28" Then
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C28")) = "" Then 'Tipo Comprobante de Pago que se modifica (NC)
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
            End If
            If e.Column.FieldName = "C29" Then
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C29")) = "" Then 'Serie Comprobante de Pago que se modifica (NC)
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
            End If
            If e.Column.FieldName = "C30" Then
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C30")) = "" Then 'Número Comprobante de Pago que se modifica (NC)
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
            End If
            If e.Column.FieldName = "C34" Then 'Estado
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C34")) = "" Then
                    e.Appearance.BackColor = Color.Peru
                    e.Appearance.BackColor2 = Color.LightYellow
                    bFlatFileGenerate = False
                End If
            End If
        End If
    End Sub

    Friend Function DataValidation(column As String, value As String) As String
        Dim sResult As String = ""
        If column = "C6" Then
            If dtTypePaytDoc.Select("Código = '" & value & "'").Length > 0 Then
                sResult = value
            End If
        End If
        If sResult = "" Then
            bFlatFileGenerate = False
        End If
        Return sResult
    End Function

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

End Class