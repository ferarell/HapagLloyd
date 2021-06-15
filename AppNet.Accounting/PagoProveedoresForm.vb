Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports DevExpress.XtraSplashScreen
Imports System.Threading

Public Class PagoProveedoresForm
    Dim RUC, SunatFileName As String
    Dim LibroSunat As String = "RegistroVentas"
    Dim dsLibroSunat As New dsSunat
    Dim dsExcel As New DataSet
    Dim dtResult, dtProcess, dtTypePaytDoc, dtPaytTerms As New DataTable
    Dim bFlatFileGenerate As Boolean = True
    Dim bProcess As Boolean = True

    Private Sub PagoProveedoresForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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
        vpLedger.SetValidationRule(Me.lueSociedad, customValidationRule)
        vpLedger.SetValidationRule(Me.beArchivoOrigen, customValidationRule)
        vpLedger.SetValidationRule(Me.beArchivoSalida, customValidationRule)
    End Sub

    Private Sub LoadPaytTerms()
        dtPaytTerms = FillDataTable("CondPago", "", "ACC")
    End Sub

    Private Sub LoadTypePaytDoc()
        dtTypePaytDoc = FillDataTable("TipoComprobante", "", "ACC")
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiProcesar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesar.ItemClick
        Me.Refresh()
        bFlatFileGenerate = True
        bProcess = True
        If vpLedger.Validate Then
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            dsLibroSunat.Tables(LibroSunat).Rows.Clear()
            ProcessLedger()
        Else
            Return
        End If
        gcListaPagos.DataSource = dsLibroSunat.Tables(LibroSunat)
        PivotGridControl1.DataSource = gcListaPagos.DataSource
        PivotGridControl1.RefreshData()
        gcListaPagos.Refresh()
        SplashScreenManager.CloseForm(False)

    End Sub

    Private Sub lueSociedad_EditValueChanged(sender As Object, e As EventArgs) Handles lueSociedad.EditValueChanged
        If lueSociedad.EditValue <> "" Then
            RUC = lueSociedad.GetColumnValue("CompanyTaxCode")
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
                    If bProcess Then
                        NewRowLedger(row)
                    End If
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
            dtResult.Rows(iPosition).Item("C34") = ""
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.OK Then
                bProcess = False
            End If
        End Try
    End Sub

    Private Sub SunatFlatFileGenerate()
        If bFlatFileGenerate Then
            If CreateTextDelimiterFile(beArchivoSalida.EditValue, dsLibroSunat.Tables(LibroSunat), "|", False, False) Then
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El archivo plano ha sido generado satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generó el archivo plano, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Se identificaron algunos errores en el proceso, no es posible generar el archivo PLE.  .", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
        ExportarExcel(gcListaPagos)
    End Sub

    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().

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