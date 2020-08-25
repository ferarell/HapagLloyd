Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.Text.RegularExpressions

Public Class AnexoClientesForm
    Dim RUC, SunatFileName1, SunatFileName2 As String
    Dim LibroSunat As String = "LibroDiario"
    Dim dsLibroSunat As New dsSunat
    Dim dsExcel As New DataSet
    Dim dtTypePaytDoc, dtAccountMapping, dtResult1, dtResult2 As New DataTable
    Dim dtBanks, dtCashBankMapping, dtSales, dtPurchases As New DataTable
    Dim bFlatFileGenerate As Boolean = True
    Dim bProcess As Boolean = True

    Private Sub LibroDiarioForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        seEjercicio.Value = Today.Year
        sePeriodo.Value = Today.Month
        FillCompany()
        LoadInputValidations()
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
        LoadAccountMapping()
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Cargando datos externos...")

        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            End Try
        PivotGridControl1.DataSource = gcLibroSunat.DataSource
        PivotGridControl1.RefreshData()
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub ProcessLedger()
        Dim DocSAP, TxtRef As String
        Dim FecDoc, FecCtb As Date
        Dim dtSource, dtAccounts As New DataTable
        dtSource.Columns.Add("DocSAP", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("FecDoc", GetType(Date)).AllowDBNull = True
        dtSource.Columns.Add("FecCtb", GetType(Date)).AllowDBNull = True
        dtSource.Columns.Add("NumItm", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("CtaCtb", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("ClaCtb", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("CodMon", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("ImpDeb", GetType(Double)).AllowDBNull = True
        dtSource.Columns.Add("ImpCre", GetType(Double)).AllowDBNull = True
        dtSource.Columns.Add("TxtRef", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("CtaDes", GetType(String)).AllowDBNull = True
        dtSource.Columns.Add("CtaOri", GetType(String)).AllowDBNull = True
        Dim iPosition As Integer = 0
        Using sr As New StreamReader(beArchivoOrigen.Text)
            Dim lines As List(Of String) = New List(Of String)
            Dim bExit As Boolean = False
            Do While Not sr.EndOfStream
                lines.Add(sr.ReadLine())
            Loop
            Dim bSkip As Boolean = True
            For i As Integer = 0 To lines.Count - 1
                If TextContain(Microsoft.VisualBasic.Left(lines(i), 8), "OnlyNumbers") Then
                    DocSAP = Mid(lines(i), 9, 11)
                    FecDoc = Mid(lines(i), 35, 2) & "/" & Mid(lines(i), 37, 2) & "/" & "20" & Mid(lines(i), 39, 2)
                    FecCtb = Mid(lines(i), 28, 2) & "/" & Mid(lines(i), 30, 2) & "/" & "20" & Mid(lines(i), 32, 2)
                    TxtRef = Mid(lines(i), 62, 50)
                    i = i + 1
                End If
                If TextContain(Mid(lines(i), 36, 3), "OnlyNumbers") And TextContain(Mid(lines(i), 61, 2), "OnlyNumbers") Then
                    dtSource.Rows.Add()
                    iPosition = dtSource.Rows.Count - 1
                    dtSource.Rows(iPosition).Item(0) = DocSAP
                    dtSource.Rows(iPosition).Item(1) = FecDoc
                    dtSource.Rows(iPosition).Item(2) = FecCtb
                    dtSource.Rows(iPosition).Item(3) = Mid(lines(i), 36, 3)
                    If Mid(lines(i), 66, 10).Trim = "" Then
                        dtSource.Rows(iPosition).Item(4) = "" 'GetLocalAccount(Mid(lines(i), 45, 10), "0")
                        dtSource.Rows(iPosition).Item(11) = Mid(lines(i), 45, 10)
                    Else
                        dtSource.Rows(iPosition).Item(4) = "" 'GetLocalAccount(Mid(lines(i), 66, 10), "0")
                        dtSource.Rows(iPosition).Item(11) = Mid(lines(i), 66, 10)
                    End If
                    dtSource.Rows(iPosition).Item(5) = Mid(lines(i), 61, 2)
                    dtSource.Rows(iPosition).Item(6) = Mid(lines(i), 97, 3)
                    dtSource.Rows(iPosition).Item(7) = IIf(Mid(lines(i), 100, 15).Trim = "", "0.00", Mid(lines(i), 100, 15))
                    dtSource.Rows(iPosition).Item(8) = IIf(Mid(lines(i), 116, 15).Trim = "", "0.00", Mid(lines(i), 116, 15))
                    dtSource.Rows(iPosition).Item(9) = TxtRef
                    dtSource.Rows(iPosition).Item(10) = ""
                    'If Mid(lines(i), 66, 10).Trim = "" Then
                    '    dtSource.Rows(iPosition).Item(10) = GetLocalAccount(Mid(lines(i), 45, 10), "1")
                    'Else
                    '    dtSource.Rows(iPosition).Item(10) = GetLocalAccount(Mid(lines(i), 66, 10), "1")
                    'End If
                End If
            Next
        End Using
        Try

        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoOrigen.Properties.ButtonClick
        XtraOpenFileDialog1.Filter = "Text files (*.txt)|*.txt"
        XtraOpenFileDialog1.FileName = ""
        'XtraOpenFileDialog1.InitialDirectory = ""
        If XtraOpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beArchivoOrigen.Text = XtraOpenFileDialog1.FileName
        End If
    End Sub

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
        ExportarExcel(gcLibroSunat)
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

    Friend Function DataValidation(column As String, value As String) As String
        Dim sResult As String = ""
        If column = "TipDoc" Then
            value = Strings.Right("00" & value.Trim, 2)
            If dtTypePaytDoc.Select("Código = '" & value & "'").Length > 0 Then
                sResult = value
            End If
        End If
        If sResult = "" Then
            bFlatFileGenerate = False
        End If
        Return sResult
    End Function

End Class