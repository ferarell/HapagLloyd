Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.Collections

Public Class RegistroVentasForm
    Dim RUC, SunatFileName As String
    Dim LibroSunat As String = "RegistroVentas"
    Dim dsLibroSunat As New dsSunat
    Dim dsExcel As New DataSet
    Dim dtResult, dtProcess, dtTypePaytDoc, dtPaytTerms As New DataTable
    Dim bFlatFileGenerate As Boolean = True
    Dim bProcess As Boolean = True

    Private Sub RegistroVentasForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        seEjercicio.Value = Today.Year
        sePeriodo.Value = Today.Month
        FillCompany()
        FolderBrowserDialog1.SelectedPath = IIf(My.Settings.LedgerTargetDirectory14 <> "", My.Settings.LedgerTargetDirectory14, "")
        LoadInputValidations()
        LoadPaytTerms()
        LoadTypePaytDoc()
    End Sub

    Private Sub beArchivoSalida_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoSalida.Properties.ButtonClick
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            beArchivoSalida.EditValue = FolderBrowserDialog1.SelectedPath & "\" & SunatFileName
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
        dtPaytTerms = Nothing 'FillDataTable("CondPago", "")
    End Sub

    Private Sub LoadTypePaytDoc()
        dtTypePaytDoc = FillDataTable("TipoComprobante", "")
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiProcesar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesar.ItemClick
        Me.Refresh()
        bFlatFileGenerate = True
        bProcess = True
        If vpLedger.Validate Then
            Try
                SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
                dsLibroSunat.Tables(LibroSunat).Rows.Clear()
                ProcessLedger()
                ValidationSequence()
            Catch ex As Exception
                bProcess = False
                SplashScreenManager.CloseForm(False)
            Finally
                SplashScreenManager.CloseForm(False)
            End Try
        Else
            Return
        End If
        gcLibroSunat.DataSource = dsLibroSunat.Tables(LibroSunat)
        PivotGridControl1.DataSource = gcLibroSunat.DataSource
        PivotGridControl1.RefreshData()
        gcLibroSunat.RefreshDataSource()
    End Sub

    Private Sub ValidationSequence()
        Try
            Dim dtSequence As DataTable = dtResult.Select("", "C6, C7, C8").CopyToDataTable
            Dim ErrorMsg As String
            Dim TipDoc1, TipDoc2, NumSer1, NumSer2, NumDoc1, NumDoc2, iPos, iDiff As String
            For Each row As DataRow In dtSequence.Rows
                If Not IsDBNull(row(5)) And Not IsDBNull(row(6)) And Not IsDBNull(row(7)) Then
                    iPos = dtSequence.Rows.IndexOf(row)
                    TipDoc1 = row(5)
                    NumSer1 = row(6)
                    NumDoc1 = row(7)
                    If Not IsDBNull(dtSequence.Rows(iPos).Item(5)) And Not IsDBNull(dtSequence.Rows(iPos).Item(6)) And Not IsDBNull(dtSequence.Rows(iPos).Item(7)) Then
                        If iPos > 0 Then
                            TipDoc2 = dtSequence.Rows(iPos - 1).Item(5)
                            NumSer2 = dtSequence.Rows(iPos - 1).Item(6)
                            NumDoc2 = dtSequence.Rows(iPos - 1).Item(7)
                            iDiff = NumDoc1 - NumDoc2
                        End If
                        If TipDoc1 = TipDoc2 And NumSer1 = NumSer2 Then
                            If iDiff > 1 Then
                                bFlatFileGenerate = False
                                ErrorMsg = "Existe un salto de correlatividad entre los documentos " & NumDoc2.ToString & " y " & NumDoc1.ToString & " de la serie " & NumSer1.ToString & " del tipo de documento " & Format(TipDoc1, "00")
                                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ErrorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End If
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub lueSociedad_EditValueChanged(sender As Object, e As EventArgs) Handles lueSociedad.EditValueChanged, seEjercicio.EditValueChanged, sePeriodo.EditValueChanged
        If lueSociedad.EditValue <> "" Then
            RUC = lueSociedad.GetColumnValue("CompanyTaxCode")
            SunatFileName = "LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "140100" & "00" & "1111" & ".TXT"
            If My.Settings.LedgerTargetDirectory14 <> "" Then
                beArchivoSalida.EditValue = FolderBrowserDialog1.SelectedPath & "\" & SunatFileName
            End If
        End If
    End Sub

    Private Sub ProcessLedger()
        Dim SourceFile As String = beArchivoOrigen.Text
        dsExcel = LoadExcel(SourceFile, "{0}")
        If dsExcel.Tables(0).Rows.Count > 0 Then
            Try
                dtResult = dsLibroSunat.Tables(LibroSunat)
                For Each row As DataRow In dsExcel.Tables(0).Rows
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
        For i = 0 To 14
            If IsDBNull(row(i)) Then
                If row.Table.Columns(i).DataType Is System.Type.GetType("System.String") Then
                    row(i) = ""
                ElseIf row.Table.Columns(i).DataType Is System.Type.GetType("System.Double") Then
                    row(i) = 0
                End If
            End If
        Next
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = Format(row(1), "yyyyMM00")
            dtResult.Rows(iPosition).Item("C2") = row(0)
            dtResult.Rows(iPosition).Item("C3") = "M" & IIf(row(0).ToString.Length > 9, Strings.Right(row(0).ToString, 9), row(0).ToString)
            dtResult.Rows(iPosition).Item("C4") = Format(CDate(row(1)), "dd/MM/yyyy")
            dtResult.Rows(iPosition).Item("C5") = "" 'Format(DateAdd(DateInterval.Day, GetDueDays(row(2)), row.Item(1)), "dd/MM/yyyy")
            If lueSociedad.EditValue = "0098" Then
                If seEjercicio.Text & Format(sePeriodo.EditValue, "00") < "201706" Then
                    sTipDoc = GetDocType(row(5), row(6) + row(7), row(3).ToString.Trim)
                Else
                    sTipDoc = IIf(Mid(row(3).ToString.Trim, 1, 1) = "B", "03", IIf(Mid(row(3).ToString.Trim, 1, 2) = "FC", "07", "01"))
                End If
            Else
                sTipDoc = DataValidation("TipDoc", GetDocType(row(5), row(6), row(3).ToString.Trim))
            End If
            If sTipDoc <> "" Then
                dtResult.Rows(iPosition).Item("C6") = sTipDoc
                If lueSociedad.EditValue = "0098" Then
                    If seEjercicio.Text & Format(sePeriodo.EditValue, "00") < "201706" Then
                        dtResult.Rows(iPosition).Item("C7") = "00" & Mid(row(3), 1, 2)
                    Else
                        dtResult.Rows(iPosition).Item("C7") = Mid(row(3), 1, 4)
                    End If
                    dtResult.Rows(iPosition).Item("C8") = Strings.Right(row(3), 8)
                Else
                    dtResult.Rows(iPosition).Item("C7") = GetTextFormatValue(sTipDoc, "NroSer", Mid(row(3), 4, Len(row(3)) - 3)) 'GetNroSer(row(3))
                    dtResult.Rows(iPosition).Item("C8") = GetTextFormatValue(sTipDoc, "NroDoc", Mid(row(3), 4, Len(row(3)) - 3)) 'Strings.Right(row(3).ToString.Trim, 7)
                End If
                dtResult.Rows(iPosition).Item("C9") = ""
            Else
                dtResult.Rows(iPosition).Item("C36") = "El tipo de documento es incorrecto, verifique la estructura del número de documento de origen (" & row(3) & ")."
            End If
            If Not row(5).ToString.Contains("ANULAD") Then
                dtResult.Rows(iPosition).Item("C10") = IIf(row(4).ToString.Trim.Length = 11, "6", IIf(row(4).ToString.Trim.Length = 8, "1", "0"))
                If row(4).ToString.Trim = "" Then
                    dtResult.Rows(iPosition).Item("C11") = GetRucByCia(row(5))
                Else
                    dtResult.Rows(iPosition).Item("C11") = Replace(row(4), "-", "")
                End If
            Else
                dtResult.Rows(iPosition).Item("C10") = "6"
                dtResult.Rows(iPosition).Item("C11") = RUC
            End If
            dtResult.Rows(iPosition).Item("C12") = row(5)
            dtResult.Rows(iPosition).Item("C13") = IIf(lueSociedad.EditValue = "0098", 0, Format(row(7) * -1, "###########0.00"))
            dtResult.Rows(iPosition).Item("C14") = Format(row(6) * -1, "###########0.00")
            dtResult.Rows(iPosition).Item("C15") = "0"
            dtResult.Rows(iPosition).Item("C16") = Format(row(8) * -1, "###########0.00")
            dtResult.Rows(iPosition).Item("C17") = "0"
            dtResult.Rows(iPosition).Item("C18") = "0"
            dtResult.Rows(iPosition).Item("C19") = IIf(lueSociedad.EditValue = "0098", Format(row(7) * -1, "###########0.00"), 0)
            dtResult.Rows(iPosition).Item("C20") = "0"
            dtResult.Rows(iPosition).Item("C21") = "0"
            dtResult.Rows(iPosition).Item("C22") = "0"
            dtResult.Rows(iPosition).Item("C23") = "0.00"
            dtResult.Rows(iPosition).Item("C24") = "0"
            dtResult.Rows(iPosition).Item("C25") = Format(row(9) * -1, "###########0.00")
            dtResult.Rows(iPosition).Item("C26") = row(14).ToString.Trim
            If row(14).ToString.Trim = "PEN" Then
                dtResult.Rows(iPosition).Item("C27") = "1.000"
            Else
                dtResult.Rows(iPosition).Item("C27") = Format(CDbl(row(12)), "0.000")
            End If
            If dtResult.Rows(iPosition).Item("C6") = "07" And Not row(5).ToString.Contains("ANULAD") Then
                Dim aDatos As New ArrayList
                Dim dtRow As DataRow = Nothing
                aDatos.Add("NC")
                aDatos.Add(lueSociedad.EditValue)
                aDatos.Add(seEjercicio.Text & Format(sePeriodo.EditValue, "00"))
                aDatos.Add(row(0).ToString.Trim)
                aDatos.Add(row(3))
                dtRow = InsertaDatosAsociados(aDatos)
                If Not dtRow Is Nothing Then
                    dtRow(4) = IIf(IsDBNull(dtRow(4)), "", dtRow(4))
                    If dtRow.ItemArray.Count > 4 And dtRow(4).ToString.Trim <> "" Then
                        dtResult.Rows(iPosition).Item("C28") = Format(dtRow(7), "dd/MM/yyyy")
                        dtResult.Rows(iPosition).Item("C29") = dtRow(4)
                        dtResult.Rows(iPosition).Item("C30") = dtRow(5)
                        dtResult.Rows(iPosition).Item("C31") = dtRow(6)
                    End If
                End If
                If Not IsDBNull(row(15)) And Not IsDBNull(row(16)) And Not IsDBNull(row(17)) And Not IsDBNull(row(18)) Then
                    dtResult.Rows(iPosition).Item("C28") = Format(CDate(row(15)), "dd/MM/yyyy")
                    dtResult.Rows(iPosition).Item("C29") = DataValidation("TipDoc", Strings.Left(row(16).ToString.Trim, 2))
                    dtResult.Rows(iPosition).Item("C30") = GetTextFormatValue(dtResult.Rows(iPosition).Item("C29"), "NroSer", row(17).ToString.Trim)
                    dtResult.Rows(iPosition).Item("C31") = GetTextFormatValue(dtResult.Rows(iPosition).Item("C29"), "NroDoc", row(18).ToString.Trim)
                End If
            End If
            dtResult.Rows(iPosition).Item("C32") = ""
            dtResult.Rows(iPosition).Item("C33") = ""
            dtResult.Rows(iPosition).Item("C34") = "1"
            dtResult.Rows(iPosition).Item("C35") = GetStatus("01/" & Format(sePeriodo.Value, "00/") & seEjercicio.Value, row(1), row(8), IIf(row(5).ToString.Contains("ANULAD"), True, False))
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Function GetNroSer(numdoc As String) As String
        Dim sResult As String = ""
        If lueSociedad.ItemIndex = 0 Then
            sResult = "00" & Strings.Left(numdoc.Trim, 2)
        Else
            sResult = Strings.Right("0000" & Mid(numdoc, 4, InStr(numdoc, "-") - 4), 4)
        End If
        Return sResult
    End Function

    Friend Function GetDocType(desc As String, value As Double, numdoc As String) As String
        Dim sResult As String = ""
        If lueSociedad.ItemIndex = 0 Then
            If desc.Contains("ANULAD") Then
                sResult = SeekNear(Strings.Right(numdoc, 8))
            Else
                sResult = IIf(value > 0, "07", "01")
            End If
        Else
            sResult = Strings.Left(numdoc, 2)
        End If
        Return sResult
    End Function

    Friend Function SeekNear(numdoc As Integer) As String
        Dim tipdoc, ndoc1, ndoc2 As String
        ndoc1 = Strings.Right("00000000" & (numdoc - 1).ToString.Trim, 8)
        ndoc2 = Strings.Right("00000000" & (numdoc + 1).ToString.Trim, 8)
        If dtResult.Select("C8 = '" & ndoc1 & "'").Length > 0 Then
            tipdoc = dtResult.Select("C8 = '" & ndoc1 & "'")(0).ItemArray(5)
        ElseIf dtResult.Select("C8 = '" & ndoc2 & "'").Length > 0 Then
            tipdoc = dtResult.Select("C8 = '" & ndoc2 & "'")(0).ItemArray(5)
        Else
            tipdoc = ""
        End If
        Return tipdoc
    End Function

    Private Sub SunatFlatFileGenerate()
        If bFlatFileGenerate Then
            beArchivoSalida.EditValue = FolderBrowserDialog1.SelectedPath & "\LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "140100" & "00" & "1" & IIf(dtResult.Rows.Count = 0, "0", "1") & "11" & ".TXT"
            If CreateTextDelimiterFile(beArchivoSalida.EditValue, dsLibroSunat.Tables(LibroSunat), "|", False, False) Then
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El archivo plano ha sido generado satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generó el archivo plano, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Se identificaron algunos errores en el proceso, no es posible generar el archivo PLE.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Friend Function GetRucByCia(CiaName As String) As String
        Dim CiaRUC As String = ""
        CiaName = Replace(CiaName, "'", "")
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
            If e.Column.FieldName = "C28" Then
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C28")) = "" And Not View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")).Contains("ANULAD") Then 'Fecha Comprobante de Pago que se modifica (NC)
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
            End If
            If e.Column.FieldName = "C29" Then
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C29")) = "" And Not View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")).Contains("ANULAD") Then 'Tipo Comprobante de Pago que se modifica (NC)
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
            End If
            If e.Column.FieldName = "C30" Then
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C30")) = "" And Not View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")).Contains("ANULAD") Then 'Serie Comprobante de Pago que se modifica (NC)
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
            End If
            If e.Column.FieldName = "C31" Then
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C31")) = "" And Not View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")).Contains("ANULAD") Then 'Número Comprobante de Pago que se modifica (NC)
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
            End If
            If e.Column.FieldName = "C35" Then 'Estado
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C35")) = "" Then
                    e.Appearance.BackColor = Color.Peru
                    e.Appearance.BackColor2 = Color.LightYellow
                    bFlatFileGenerate = False
                End If
            End If
        End If
    End Sub

    'Friend Function DataValidation(column As String, value As String) As String
    '    Dim sResult As String = ""
    '    If column = "C6" Then
    '        If dtTypePaytDoc.Select("Código = '" & value & "'").Length > 0 Then
    '            sResult = value
    '        End If
    '    End If
    '    If sResult = "" Then
    '        bFlatFileGenerate = False
    '    End If
    '    Return sResult
    'End Function

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

    Friend Function GetTextFormatValue(DocType As String, Group As String, Value As String) As String
        Dim sResult As String = ""
        Dim iPositions As Integer = GetPositionsByDocType(DocType, Group)
        Try
            If Group = "NroSer" Then
                If InStr(Value, "-") > 0 Then
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


    Private Sub bbiSunatPle_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSunatPle.ItemClick
        SunatFlatFileGenerate()
    End Sub
End Class