Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.Text.RegularExpressions
Imports DevExpress.DataProcessing.InMemoryDataProcessor
Imports DevExpress.XtraEditors.ColorPick.Picker
Imports DevExpress.XtraRichEdit.Commands
Imports DevExpress.Data.XtraReports.Wizard.Presenters

Public Class AnexoProveedoresForm
    'Dim RUC, SunatFileName1, SunatFileName2 As String
    'Dim LibroSunat As String = "LibroDiario"
    'Dim dsLibroSunat As New dsSunat
    'Dim dsExcel As New DataSet
    Dim dtAccountMapping As New DataTable
    'Dim dtBanks, dtCashBankMapping, dtSales, dtPurchases As New DataTable
    'Dim bFlatFileGenerate As Boolean = True
    'Dim bProcess As Boolean = True

    Private Sub LibroDiarioForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GridView1.RestoreLayoutFromRegistry(Directory.GetCurrentDirectory)
        seEjercicio.Value = Today.Year
        sePeriodo.Value = Today.Month
        FillCompany()
        LoadInputValidations()

        SplitContainerControl2.Collapsed = True
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
        vpInputs.SetValidationRule(Me.beArchivoOrigen, customValidationRule)
    End Sub

    Private Sub LoadAccountMapping()
        dtAccountMapping = FillDataTable("AccountMapping", "CompanyCode='" & lueSociedad.EditValue & "'")
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiProcesar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesar.ItemClick
        If Not vpInputs.Validate Then
            Return
        End If
        'LoadAccountMapping()
        Dim dtSource1, dtSource2, dtResult As New DataTable
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Cargando datos externos...")
        Dim dtTxtRef As DataTable = ExecuteAccessQuery("SELECT * FROM AnexoConfiguracion ORDER BY 1").Tables(0)
        For f = 0 To XtraOpenFileDialog1.FileNames.Count - 1
            If LoadExcel(XtraOpenFileDialog1.FileNames(f), "{0}").Tables(0).Columns(0).ColumnName.ToUpper.Contains("ACCOUNT") Then
                dtSource2 = LoadExcel(XtraOpenFileDialog1.FileNames(f), "{0}").Tables(0)
            Else
                dtSource1 = LoadExcel(XtraOpenFileDialog1.FileNames(f), "{0}").Tables(0)
            End If
        Next
        Try
            dtResult = ExecuteAccessQuery("SELECT * FROM AnexoProveedores WHERE VendorCode='#'").Tables(0)
            If dtSource1.Rows.Count = 0 Then 'Or dtSource2.Rows.Count = 0 Then
                DevExpress.XtraEditors.XtraMessageBox.Show("Alguno de los archivos seleccionados no contiene datos o no tiene el formato correcto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            Dim iPos As Integer = 0
            For r = 0 To dtSource1.Rows.Count - 1
                Try
                    Dim oRow1 As DataRow = dtSource1.Rows(r)
                    If oRow1(0) Is Nothing Then
                        Continue For
                    End If
                    If oRow1(0) = "" Then
                        Continue For
                    End If
                    'Dim oRow2 As DataRow = Nothing
                    'If dtSource2.Select("[Document Number] = '" & oRow1("Document Number").ToString & "'").Length > 0 Then
                    '    oRow2 = dtSource2.Select("[Document Number] = '" & oRow1("Document Number") & "'")(0)
                    '    'dtResult.Rows(iPos)("ValuatedAmountFC") = dtSource2.Select("DocumentNo='" & oRow("DocumentNo") & "'")(0)("ValuatedAmount")
                    'End If
                    dtResult.Rows.Add()
                    iPos = dtResult.Rows.Count - 1
                    dtResult.Rows(iPos)("Company") = lueSociedad.EditValue
                    dtResult.Rows(iPos)("Origin") = GetOriginByReference(dtTxtRef, oRow1("Reference"))
                    dtResult.Rows(iPos)("Period") = seEjercicio.EditValue.ToString & Format(sePeriodo.EditValue, "00")
                    'dtResult.Rows(iPos)("GLAccount") = oRow1("G/L Account")
                    dtResult.Rows(iPos)("Account") = oRow1("Reconciliation acct")
                    dtResult.Rows(iPos)("VendorCode") = oRow1("Vendor")
                    dtResult.Rows(iPos)("VendorTaxNumber") = oRow1("Tax Number 1") 'IIf(Not IsDBNull(oRow1("Tax Number 1")), oRow1("Tax Number 1"), IIf(IsDBNull(oRow1("Tax Number 2")), "", oRow1("Tax Number 2")))
                    dtResult.Rows(iPos)("VendorName") = Replace(oRow1("Name"), "'", "")
                    'dtResult.Rows(iPos)("Assignment") = oRow1("Assignment")
                    'dtResult.Rows(iPos)("Text") = oRow1("Text")
                    dtResult.Rows(iPos)("PostingDate") = oRow1("Posting Date")
                    dtResult.Rows(iPos)("DocumentNumber") = oRow1("Document Number")
                    dtResult.Rows(iPos)("DocumentDate") = oRow1("Document Date")
                    dtResult.Rows(iPos)("Reference") = oRow1("Reference")
                    dtResult.Rows(iPos)("DocumentType") = oRow1("Document type")
                    dtResult.Rows(iPos)("ForeignCurrency") = oRow1("Doc#currency")
                    dtResult.Rows(iPos)("AmountFC") = oRow1("Amount in foreign cur#")
                    dtResult.Rows(iPos)("LocalCurrency") = oRow1("Local currency")
                    dtResult.Rows(iPos)("AmountLC") = oRow1("Amt#in loc#cur#") * IIf(oRow1("Amount in foreign cur#") < 0, -1, 1)
                    dtResult.Rows(iPos)("ValuatedAmountLC") = oRow1("Valuated amount")
                    dtResult.Rows(iPos)("ValuatedAmountFC") = oRow1("Valuated amt loc#curr#2")
                    dtResult.Rows(iPos)("AmountGroupCurrency") = oRow1("Amount group currency")
                    dtResult.Rows(iPos)("NetDueDate") = oRow1("Net due date")

                Catch ex As Exception
                    SplashScreenManager.CloseForm(False)
                    DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Next
            gcAnexoProveedores.DataSource = dtResult
            GridView1.ExpandAllGroups()
            GridView1.BestFitColumns()
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
        End Try
        'PivotGridControl1.DataSource = gcAnexoProveedores.DataSource
        'PivotGridControl1.RefreshData()
        SplashScreenManager.CloseForm(False)
    End Sub

    Function GetOriginByReference(dtTxtRef As DataTable, TextRef As String) As String
        Dim sResult As String = "FOREING"
        For r = 0 To dtTxtRef.Rows.Count - 1
            Dim oRow As DataRow = dtTxtRef.Rows(r)
            If TextRef.StartsWith(oRow("TextReference")) Then
                sResult = oRow("Origin")
                Exit For
            End If
        Next
        Return sResult
    End Function

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoOrigen.Properties.ButtonClick
        XtraOpenFileDialog1.Filter = "Source Files (*.xls*;*.txt)|*.xls*;*.txt"
        XtraOpenFileDialog1.FileName = ""
        'XtraOpenFileDialog1.InitialDirectory = ""
        If XtraOpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beArchivoOrigen.Text = XtraOpenFileDialog1.FileName
        End If
    End Sub

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
        ExportarExcel(gcAnexoProveedores)
    End Sub

    Private Sub lueSociedad_EditValueChanged(sender As Object, e As EventArgs) Handles lueSociedad.EditValueChanged, seEjercicio.EditValueChanged, sePeriodo.EditValueChanged

    End Sub

    Private Sub GridView1_RowCellStyle(ByVal sender As Object, ByVal e As RowCellStyleEventArgs)
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
        End If
    End Sub

    Private Sub seEjercicio_Leave(sender As Object, e As EventArgs) Handles seEjercicio.Leave, sePeriodo.Leave
        If seEjercicio.Text > Year(Today).ToString Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El ejercicio no puede ser mayor al año en curso.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            sender.focus()
        End If
        If seEjercicio.Text & Format(sePeriodo.EditValue, "00") > Year(Today).ToString & Format(Month(Today), "00") Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El periodo no puede ser mayor al mes en curso.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            sender.focus()
        End If
    End Sub

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

    Private Sub bbiGuardar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiGuardar.ItemClick
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Guardando datos...")
            If ExecuteAccessQuery("SELECT * FROM AnexoProveedores WHERE Company='" & lueSociedad.EditValue & "' AND Period='" & seEjercicio.EditValue.ToString & Format(sePeriodo.EditValue, "00") & "'").Tables(0).Rows.Count > 0 Then
                If DevExpress.XtraEditors.XtraMessageBox.Show("Ya existen datos para este periodo, desea reemplazarlos?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    Return
                End If
            End If
            If Not ExecuteAccessNonQuery("DELETE FROM AnexoProveedores WHERE Company='" & lueSociedad.EditValue & "' AND Period='" & seEjercicio.EditValue.ToString & Format(sePeriodo.EditValue, "00") & "'") Then
                DevExpress.XtraEditors.XtraMessageBox.Show("Se generó un error al eliminar los datos del periodo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            Dim dtUpdate As DataTable = gcAnexoProveedores.DataSource
            For r = 0 To dtUpdate.Rows.Count - 1
                If Not InsertIntoAccess("AnexoProveedores", dtUpdate.Rows(r)) Then
                    DevExpress.XtraEditors.XtraMessageBox.Show("Error al actualizar los datos de la fila " & r.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            Next
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show("Los datos se actualizaron satisfactoriamente.", "information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bbiBuscar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiBuscar.ItemClick
        gcAnexoProveedores.DataSource = Nothing
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("SELECT * FROM AnexoProveedores WHERE Company='" & lueSociedad.EditValue & "' AND Period='" & seEjercicio.EditValue.ToString & Format(sePeriodo.EditValue, "00") & "'").Tables(0)
        If dtQuery.Rows.Count = 0 Then
            DevExpress.XtraEditors.XtraMessageBox.Show("La consulta no retornó datos.", "Advartencia", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        gcAnexoProveedores.DataSource = dtQuery
        GridView1.ExpandAllGroups()
        GridView1.BestFitColumns()
    End Sub

    Private Sub AnexoProveedoresForm_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        GridView1.SaveLayoutToRegistry(Directory.GetCurrentDirectory)
    End Sub
End Class