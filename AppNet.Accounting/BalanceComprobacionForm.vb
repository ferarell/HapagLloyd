Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports System.Collections
Imports DevExpress.XtraSplashScreen
Imports System.Threading

Public Class BalanceComprobacionForm
    Dim RUC, SunatFileName As String
    Dim LibroSunat As String = "BalanceComprobacion"
    Dim dsLibroSunat As New dsSunat
    Dim dsExcel As New DataSet
    Dim dtPrint, dtSource1, dtSource2, dtExchange, dtTypePaytDoc, dtAccountMapping, dtResult1, dtResult2 As New DataTable
    Dim bFlatFileGenerate As Boolean = True
    Dim bProcess As Boolean = True
    Dim LastButtonClick, WaitText As String
    Dim iPrc As Integer = 0

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
        vpInputs.SetValidationRule(Me.lueSociedad, customValidationRule)
        vpInputs.SetValidationRule(Me.seEjercicio, customValidationRule)
        vpInputs.SetValidationRule(Me.seEjercicio, customValidationRule)
    End Sub

    Private Sub LoadTypePaytDoc()
        dtTypePaytDoc = FillDataTable("TipoComprobante", "")
    End Sub

    Private Sub LoadAccountMapping()
        dtAccountMapping = FillDataTable("Accountmapping", "CompanyCode='" & lueSociedad.EditValue & "'")
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub GridColumnsHide()
        Dim iCols As Integer = GridView2.Columns.Count
        Dim sColHide As String = "ME"
        If rgMoneda.SelectedIndex = 1 Then
            sColHide = "ML"
        End If
        For i = 0 To iCols - 1
            GridView2.Columns(i).Visible = True
            If GridView2.Columns(i).FieldName.Contains(sColHide) Then
                GridView2.Columns(i).Visible = False
            End If
        Next
    End Sub

    Private Sub lueSociedad_EditValueChanged(sender As Object, e As EventArgs) Handles lueSociedad.EditValueChanged, seEjercicio.EditValueChanged, sePeriodo.EditValueChanged
        If lueSociedad.EditValue <> "" Then
            LoadAccountMapping()
        End If
    End Sub

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs)
        If gcDetalle.Focused Then
            ExportarExcel(gcDetalle)
        ElseIf gcResumen.Focused Then
            ExportarExcel(gcResumen)
        End If
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

    Private Sub bbiConsultar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiConsultar.ItemClick
        LastButtonClick = e.Item.Name
        LoadInputValidations()
        If Not vpInputs.Validate Then
            Return
        End If
        Dim dtQuery, dtSummaryView As New DataTable
        Dim sCurrPeriod As String = seEjercicio.Text & Format(sePeriodo.EditValue, "00")
        Dim sPrevPeriod As String = PreviousPeriod(sCurrPeriod)
        Dim sMoneda As String = IIf(rgMoneda.SelectedIndex = 0, "ML", "ME")
        Dim iPosition As Integer = 0
        'dtQuery = ExecuteAccessQuery("select * from SaldoContableQry1 unlock where sociedad='" & lueSociedad.EditValue & "' and periodo='" & sCurrPeriod & "' and moneda='" & rgMoneda.EditValue & "' order by CuentaLocal, Periodo").Tables(0)
        dtQuery = ExecuteAccessQueryWP("select * from BalanceComprobacionQry2 unlock where sociedad='" & lueSociedad.EditValue & "'", "sPeriodo", sCurrPeriod).Tables(0)
        dtSummaryView = dsLibroSunat.Tables("BalanceComprobacion")
        dtSummaryView.Rows.Clear()
        For Each row As DataRow In dtQuery.Rows
            If dtSummaryView.Select("C1=" & row(3)).Length = 0 Then
                dtSummaryView.Rows.Add()
            End If
            iPosition = dtSummaryView.Rows.Count - 1
            dtSummaryView.Rows(iPosition).Item("C1") = row("Cuenta")
            dtSummaryView.Rows(iPosition).Item("C2") = row("Descripcion")
            dtSummaryView.Rows(iPosition).Item("C3") = row("SaldoIniDebe" & sMoneda)
            dtSummaryView.Rows(iPosition).Item("C4") = row("SaldoIniHaber" & sMoneda) * -1
            dtSummaryView.Rows(iPosition).Item("C5") = row("MovDebe" & sMoneda)
            dtSummaryView.Rows(iPosition).Item("C6") = row("MovHaber" & sMoneda) * -1
            dtSummaryView.Rows(iPosition).Item("C7") = row("SaldoFinDebe" & sMoneda)
            dtSummaryView.Rows(iPosition).Item("C8") = row("SaldoFinHaber" & sMoneda) * -1
            If Strings.Left(row("Cuenta"), 1) >= 1 And Strings.Left(row("Cuenta"), 1) <= 5 Then
                dtSummaryView.Rows(iPosition).Item("C9") = dtSummaryView.Rows(iPosition).Item("C7")
                dtSummaryView.Rows(iPosition).Item("C10") = dtSummaryView.Rows(iPosition).Item("C8")
            End If
            If Strings.Left(row("Cuenta"), 1) >= 6 Then
                dtSummaryView.Rows(iPosition).Item("C11") = dtSummaryView.Rows(iPosition).Item("C7")
                dtSummaryView.Rows(iPosition).Item("C12") = dtSummaryView.Rows(iPosition).Item("C8")
            End If
            dtSummaryView.Rows(iPosition).Item("C13") = ""
        Next
        dtPrint = dtSummaryView
        gcResumen.DataSource = dtSummaryView
        'GridView1.PopulateColumns()
        For i = 3 To 12
            GridView1.Columns("C" & i.ToString).SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
            GridView1.Columns("C" & i.ToString).DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
            GridView1.Columns("C" & i.ToString).DisplayFormat.FormatString = "n2"
        Next
        GetAccountDetail(GridView1.GetFocusedRowCellValue("C1"))
        'GridColumnsHide()
    End Sub

    Friend Function PreviousPeriod(period As String) As String
        Dim sResult As String = ""
        sResult = Format(DateAdd(DateInterval.Day, -1, CDate("01/" & Format(sePeriodo.Value, "00/") & seEjercicio.Value)), "yyyyMM")
        Return sResult
    End Function

    Private Sub bbiExportar_ItemClick_1(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
        If gcResumen.FocusedView.IsFocusedView Then
            ExportarExcel(gcResumen)
        ElseIf gcDetalle.FocusedView.IsFocusedView Then
            ExportarExcel(gcDetalle)
        End If
    End Sub

    Private Sub GridView1_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView1.FocusedRowChanged
        If LastButtonClick = "bbiConsultar" Then
            GetAccountDetail(GridView1.GetFocusedRowCellValue("C1"))
        Else
            GetAccountDetail(GridView1.GetFocusedRowCellValue("CuentaLocal"))
        End If
    End Sub

    Private Sub GetAccountDetail(chart As String)
        Dim dtQuery, dtSummaryView As New DataTable
        Dim sCurrPeriod As String = seEjercicio.Text & Format(sePeriodo.EditValue, "00")
        Dim iPosition As Integer = 0
        dtQuery = ExecuteAccessQuery("select * from DetalleContableQry1 unlock where Sociedad='" & lueSociedad.EditValue & "' and Periodo<='" & seEjercicio.Text & Format(sePeriodo.EditValue, "00") & "' and CuentaLocal='" & chart & "'").Tables(0)
        gcDetalle.DataSource = dtQuery
        'GridView2.PopulateColumns()
        For i = 12 To GridView2.Columns.Count - 1
            If i > 12 Then
                GridView2.Columns(i).SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
            End If
            GridView2.Columns(i).DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
            GridView2.Columns(i).DisplayFormat.FormatString = "n2"
        Next
    End Sub

    Private Sub bbiGenerarArchivoPDT_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiGenerarArchivoPDT.ItemClick
        LastButtonClick = e.Item.Name
        LoadInputValidations()
    End Sub

    Private Sub bbiImprimir_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiImprimir.ItemClick
        Dim pForm As New PrintForm
        pForm.dtPrint = dtPrint
        pForm.aParams = GetParamValues()
        pForm.RptFile = "BalanceComprobacion.rpt"
        pForm.ShowDialog()
    End Sub

    Friend Function GetParamValues() As ArrayList
        Dim aParams As New ArrayList
        aParams.Add("BALANCE DE COMPROBACIÓN")
        aParams.Add(GetConditions())
        aParams.Add(lueSociedad.GetColumnValue("CompanyTaxCode"))
        aParams.Add(lueSociedad.GetColumnValue("CompanyDescription"))
        aParams.Add(lueSociedad.GetColumnValue("CompanyAddress"))
        Return aParams
    End Function

    Friend Function GetConditions() As String
        Dim RptCnd As String = "Condiciones: "
        RptCnd += "Periodo = " & seEjercicio.Text & "-" & Format(sePeriodo.EditValue, "00")
        RptCnd += ", Moneda = " & rgMoneda.EditValue
        Return RptCnd
    End Function

End Class