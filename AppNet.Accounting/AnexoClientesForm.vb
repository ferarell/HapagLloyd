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

Public Class AnexoClientesForm
    Dim dtAccountMapping As New DataTable

    Private Sub LibroDiarioForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GridView1.RestoreLayoutFromRegistry(Directory.GetCurrentDirectory)
        seEjercicio.Value = Today.Year
        sePeriodo.Value = Today.Month
        FillCompany()
        LoadInputValidations()

        SplitContainerControl2.Collapsed = True
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
        vpInputs.SetValidationRule(Me.lueSociedad, customValidationRule)
        vpInputs.SetValidationRule(Me.seEjercicio, customValidationRule)
        vpInputs.SetValidationRule(Me.seEjercicio, customValidationRule)
        vpInputs.SetValidationRule(Me.beArchivoOrigen, customValidationRule)
    End Sub

    Private Sub LoadAccountMapping()
        dtAccountMapping = FillDataTable("AccountMapping", "CompanyCode='" & lueSociedad.EditValue & "'", "ACC")
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiProcesar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesar.ItemClick
        If Not vpInputs.Validate Then
            Return
        End If
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Cargando datos externos...")
        Dim dtTxtRef As DataTable = ExecuteAccessQuery("SELECT * FROM AnexoConfiguracion").Tables(0)
        Dim dtSource1, dtSource2, dtSource3, dtResult As New DataTable
        Dim TextFile1, TextFile2 As String
        For f = 0 To XtraOpenFileDialog1.FileNames.Count - 1
            If XtraOpenFileDialog1.FileNames(f).ToString.ToUpper.Contains("XLS") Then
                dtSource3 = LoadExcel(XtraOpenFileDialog1.FileNames(f), "{0}").Tables(0)
            Else
                Dim sLine As New StreamReader(XtraOpenFileDialog1.FileNames(f))
                If sLine.ReadLine().Count > 51 Then
                    TextFile1 = XtraOpenFileDialog1.FileNames(f)
                Else
                    TextFile2 = XtraOpenFileDialog1.FileNames(f)
                End If
            End If
        Next
        Try
            dtResult = ExecuteAccessQuery("SELECT * FROM AnexoClientes WHERE CustomerCode='#'").Tables(0)
            'dtSource1 = ProcessTextFile1(TextFile1)
            'If dtSource1.Rows.Count = 0 Then
            '    DevExpress.XtraEditors.XtraMessageBox.Show("El archivo de texto 1 no contiene datos o no tiene el formato correcto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    Return
            'End If
            'dtSource2 = ProcessTextFile2(TextFile2)
            'If dtSource2.Rows.Count = 0 Then
            '    DevExpress.XtraEditors.XtraMessageBox.Show("El archivo de texto 2 no contiene datos o no tiene el formato correcto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    Return
            'End If
            Dim iPos As Integer = 0
            For r = 0 To dtSource3.Rows.Count - 1
                Dim oRow As DataRow = dtSource3.Rows(r)
                dtResult.Rows.Add()
                iPos = dtResult.Rows.Count - 1
                dtResult.Rows(iPos)("Company") = lueSociedad.EditValue
                dtResult.Rows(iPos)("Origin") = GetOriginByReference(dtTxtRef, oRow("Reference"))
                dtResult.Rows(iPos)("Period") = seEjercicio.EditValue.ToString & Format(sePeriodo.EditValue, "00")
                dtResult.Rows(iPos)("ReconciliationAccount") = oRow("Reconciliation acct") 'oRow("ReconAcct")
                dtResult.Rows(iPos)("CustomerCode") = oRow("Customer") 'oRow("Customer")
                dtResult.Rows(iPos)("CustomerTaxNumber") = oRow("Tax Number 1") 'oRow("TaxNumber1")
                dtResult.Rows(iPos)("CustomerName") = Replace(oRow("Name 1"), "'", "") 'Replace(oRow("Name1"), "'", "")
                dtResult.Rows(iPos)("PostingDate") = oRow("Posting Date") 'oRow("PstngDate")
                dtResult.Rows(iPos)("DocumentNumber") = oRow("Document Number") 'oRow("DocumentNo")
                dtResult.Rows(iPos)("DocumentDate") = oRow("Document Date") 'oRow("DocDate")
                dtResult.Rows(iPos)("Reference") = oRow("Reference") 'oRow("Reference")
                dtResult.Rows(iPos)("DocumentType") = oRow("Document type") 'oRow("Ty")
                dtResult.Rows(iPos)("ForeignCurrency") = oRow("For#currency") 'oRow("ForC")
                dtResult.Rows(iPos)("AmountFC") = oRow("Amount in foreign cur#") 'oRow("AmountInFC"
                dtResult.Rows(iPos)("LocalCurrency") = oRow("Currency") 'oRow("Crcy")
                dtResult.Rows(iPos)("AmountLC") = oRow("Amt#in loc#cur#") * IIf(oRow("Amount in foreign cur#") < 0, -1, 1) 'oRow("AmountInLC") * IIf(oRow("AmountInFC") < 0, -1, 1)
                dtResult.Rows(iPos)("ValuatedAmountFC") = oRow("Valuated amt loc#curr#2")
                'If dtSource2.Select("DocumentNo='" & oRow("DocumentNo") & "'").Length > 0 Then
                '    dtResult.Rows(iPos)("ValuatedAmountFC") = dtSource2.Select("DocumentNo='" & oRow("DocumentNo") & "'")(0)("ValuatedAmount")
                'End If
                'If dtSource3.Select("[Document Number]='" & oRow("DocumentNo") & "'").Length > 0 Then
                '    Dim drExcel3 As DataRow = dtSource3.Select("[Document Number]='" & oRow("DocumentNo") & "'")(0)
                dtResult.Rows(iPos)("AmountGroupCurrency") = oRow("Amount group currency") 'drExcel3("Amount group currency")
                dtResult.Rows(iPos)("ValuatedAmountLC") = oRow("Valuated amount") 'drExcel3("Valuated amt loc#curr#2")
                dtResult.Rows(iPos)("LineItem") = oRow("Line item") 'drExcel3("Line item")
                dtResult.Rows(iPos)("NetDueDate") = oRow("Net due date") 'drExcel3("Net due date")
                'End If
            Next
            gcAnexoClientes.DataSource = dtResult
            GridView1.ExpandAllGroups()
            GridView1.BestFitColumns()
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        PivotGridControl1.DataSource = gcAnexoClientes.DataSource
        PivotGridControl1.RefreshData()
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
        'If Mid(TextRef, 1, 2).Contains({"01", "03", "07"}) Then
        '    sResult = "LOCAL"
        'ElseIf Mid(TextRef, 1, 2).StartsWith({"BCP"}) Then
        '    sResult = "LOCAL"
        'Else
        '    sResult = "FOREING"
        'End If
        Return sResult
    End Function

    Function ProcessTextFile1(TextFile As String) As DataTable
        Dim dtSource As New DataTable
        Dim aPositions As New ArrayList
        dtSource.Columns.Add("ReconAcct", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("Customer", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("TaxNumber1", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("Name1", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("PstngDate", GetType(Date)).AllowDBNull = True
        dtSource.Columns.Add("DocumentNo", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("DocDate", GetType(Date)).AllowDBNull = True
        dtSource.Columns.Add("Reference", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("Ty", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("ForC", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("AmountInFC", GetType(Decimal)).AllowDBNull = True
        dtSource.Columns.Add("Crcy", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("AmountInLC", GetType(Decimal)).AllowDBNull = True
        Dim iPos As Integer = 0
        Using sr As New StreamReader(TextFile)
            Dim lines As List(Of String) = New List(Of String)
            Dim bExit As Boolean = False
            Do While Not sr.EndOfStream
                lines.Add(sr.ReadLine())
            Loop
            Dim TextLine As String = lines(3)
            For c = 1 To TextLine.Length
                If Mid(TextLine, c, 1) = "|" Then
                    aPositions.Add(c + 1)
                End If
            Next
            For l = 5 To lines.Count - 2
                Try
                    dtSource.Rows.Add()
                    iPos = dtSource.Rows.Count - 1
                    dtSource.Rows(iPos)("ReconAcct") = Mid(lines(l), aPositions(0), aPositions(1) - aPositions(0) - 1).Trim
                    dtSource.Rows(iPos)("Customer") = Mid(lines(l), aPositions(1), aPositions(2) - aPositions(1) - 1).Trim
                    dtSource.Rows(iPos)("TaxNumber1") = Mid(lines(l), aPositions(2), aPositions(3) - aPositions(2) - 1).Trim
                    dtSource.Rows(iPos)("Name1") = Mid(lines(l), aPositions(3), aPositions(4) - aPositions(3) - 1).Trim
                    dtSource.Rows(iPos)("PstngDate") = CDate(Mid(lines(l), aPositions(4), aPositions(5) - aPositions(4) - 1).Trim)
                    dtSource.Rows(iPos)("DocumentNo") = Mid(lines(l), aPositions(5), aPositions(6) - aPositions(5) - 1).Trim
                    dtSource.Rows(iPos)("DocDate") = CDate(Mid(lines(l), aPositions(6), aPositions(7) - aPositions(6) - 1).Trim)
                    dtSource.Rows(iPos)("Reference") = Mid(lines(l), aPositions(7), aPositions(8) - aPositions(7) - 1).Trim
                    dtSource.Rows(iPos)("Ty") = Mid(lines(l), aPositions(8), aPositions(9) - aPositions(8) - 1).Trim
                    dtSource.Rows(iPos)("ForC") = Mid(lines(l), aPositions(9), aPositions(10) - aPositions(9) - 1).Trim
                    dtSource.Rows(iPos)("AmountInFC") = CDec(Mid(lines(l), aPositions(10), aPositions(11) - aPositions(10) - 1).Trim)
                    dtSource.Rows(iPos)("Crcy") = Mid(lines(l), aPositions(11), aPositions(12) - aPositions(11) - 1).Trim
                    dtSource.Rows(iPos)("AmountInLC") = CDec(Mid(lines(l), aPositions(12), aPositions(13) - aPositions(12) - 1).Trim)
                Catch ex As Exception
                    SplashScreenManager.CloseForm(False)
                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

            Next
        End Using
        Return dtSource
    End Function

    Function ProcessTextFile2(TextFile As String) As DataTable
        Dim dtSource As New DataTable
        Dim aPositions As New ArrayList
        dtSource.Columns.Add("DocumentNo", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("Reference", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("ValuatedAmount", GetType(Decimal)).AllowDBNull = True
        Dim iPos As Integer = 0
        Using sr As New StreamReader(TextFile)
            Dim lines As List(Of String) = New List(Of String)
            Dim bExit As Boolean = False
            Do While Not sr.EndOfStream
                lines.Add(sr.ReadLine())
            Loop
            Dim TextLine As String = lines(8)
            For c = 1 To TextLine.Length
                If Mid(TextLine, c, 1) = "|" Then
                    aPositions.Add(c + 1)
                End If
            Next
            For l = 10 To lines.Count - 4
                Try
                    dtSource.Rows.Add()
                    iPos = dtSource.Rows.Count - 1
                    dtSource.Rows(iPos)("DocumentNo") = Mid(lines(l), aPositions(0), aPositions(1) - aPositions(0) - 1).Trim
                    dtSource.Rows(iPos)("Reference") = Mid(lines(l), aPositions(1), aPositions(2) - aPositions(1) - 1).Trim
                    dtSource.Rows(iPos)("ValuatedAmount") = CDec(Mid(lines(l), aPositions(2), aPositions(3) - aPositions(2) - 1).Trim)
                Catch ex As Exception
                    SplashScreenManager.CloseForm(False)
                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

            Next
        End Using
        Return dtSource
    End Function

    Function ProcessExcelFile3(ExcelFile As DataTable) As DataTable
        Dim dtSource As New DataTable

        Return dtSource
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
        ExportarExcel(gcAnexoClientes)
    End Sub

    Private Sub lueSociedad_EditValueChanged(sender As Object, e As EventArgs) Handles lueSociedad.EditValueChanged, seEjercicio.EditValueChanged, sePeriodo.EditValueChanged

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
            If ExecuteAccessQuery("SELECT * FROM AnexoClientes WHERE Company='" & lueSociedad.EditValue & "' AND Period='" & seEjercicio.EditValue.ToString & Format(sePeriodo.EditValue, "00") & "'").Tables(0).Rows.Count > 0 Then
                If DevExpress.XtraEditors.XtraMessageBox.Show("Ya existen datos para este periodo, desea reemplazarlos?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    Return
                End If
            End If
            If Not ExecuteAccessNonQuery("DELETE FROM AnexoClientes WHERE Company='" & lueSociedad.EditValue & "' AND Period='" & seEjercicio.EditValue.ToString & Format(sePeriodo.EditValue, "00") & "'") Then
                DevExpress.XtraEditors.XtraMessageBox.Show("Se generó un error al eliminar los datos del periodo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            Dim dtUpdate As DataTable = gcAnexoClientes.DataSource
            For r = 0 To dtUpdate.Rows.Count - 1
                If Not InsertIntoAccess("AnexoClientes", dtUpdate.Rows(r)) Then
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
        gcAnexoClientes.DataSource = Nothing
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQuery("SELECT * FROM AnexoClientes WHERE Company='" & lueSociedad.EditValue & "' AND Period='" & seEjercicio.EditValue.ToString & Format(sePeriodo.EditValue, "00") & "'").Tables(0)
        If dtQuery.Rows.Count = 0 Then
            DevExpress.XtraEditors.XtraMessageBox.Show("La consulta no retornó datos.", "Advartencia", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        gcAnexoClientes.DataSource = dtQuery
        GridView1.ExpandAllGroups()
        GridView1.BestFitColumns()
    End Sub

    Private Sub AnexoClientesForm_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        GridView1.SaveLayoutToRegistry(Directory.GetCurrentDirectory)
    End Sub
End Class