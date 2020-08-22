Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.Text.RegularExpressions

Public Class LibroDiarioForm
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
        FolderBrowserDialog1.SelectedPath = IIf(My.Settings.LedgerTargetDirectory5 <> "", My.Settings.LedgerTargetDirectory5, "")
        LoadInputValidations()
        'LoadPaytTerms()
        LoadTypePaytDoc()
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
        vpLedger.SetValidationRule(Me.beArchivoSalida1, customValidationRule)
    End Sub

    'Private Sub LoadPaytTerms()
    '    dtPaytTerms = FillDataTable("CondPago$")
    'End Sub

    Private Sub LoadTypePaytDoc()
        dtTypePaytDoc = FillDataTable("TipoComprobante", "")
    End Sub

    Private Sub LoadAccountMapping()
        dtAccountMapping = FillDataTable("AccountMapping", "CompanyCode='" & lueSociedad.EditValue & "'")
    End Sub

    Private Sub LoadSalesFile(SalesFile As String)
        If SalesFile IsNot Nothing Then
            dtSales.Rows.Clear()
            dtSales = LoadExcel(SalesFile, "{0}").Tables(0)
        End If
    End Sub

    Private Sub LoadPurchasesFile(PurchasesFile As String)
        If PurchasesFile IsNot Nothing Then
            dtPurchases.Rows.Clear()
            dtPurchases = LoadExcel(PurchasesFile, "{0}").Tables(0)
        End If
    End Sub

    Private Sub LoadBanksFile(BanksFile As String)
        If BanksFile IsNot Nothing Then
            dtBanks.Rows.Clear()
            dtBanks = LoadExcel(BanksFile, "{0}").Tables(0)
        End If
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiProcesar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesar.ItemClick
        Me.Refresh()
        LoadAccountMapping()
        dtCashBankMapping = FillDataTable("AccountMapping", "AccountType='B'")
        'dtAccountMapping.Select("AccountType] = 'B'").CopyToDataTable()
        bFlatFileGenerate = True
        bProcess = True
        If vpLedger.Validate Then
            Try
                SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
                SplashScreenManager.Default.SetWaitFormDescription("Cargando datos externos...")
                LoadSalesFile(beArchivoVentas.Text)
                LoadPurchasesFile(beArchivoCompras.Text)
                LoadBanksFile(beArchivoBancos.Text)
                dsLibroSunat.Tables(LibroSunat & "1").Rows.Clear()
                dsLibroSunat.Tables(LibroSunat & "2").Rows.Clear()
                ProcessLedger()
            Catch ex As Exception
                SplashScreenManager.CloseForm(False)
            End Try
        Else
            Return
        End If
        gcLibroSunat.DataSource = dsLibroSunat.Tables(LibroSunat & "1")
        'For i = 18 To 19
        '    GridView1.Columns("C" & i.ToString).SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
        '    GridView1.Columns("C" & i.ToString).DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        '    GridView1.Columns("C" & i.ToString).DisplayFormat.FormatString = "n2"
        'Next
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
            dtResult1 = dsLibroSunat.Tables(LibroSunat & "1") 'Movimiento Contable
            dtResult2 = dsLibroSunat.Tables(LibroSunat & "2") 'Plan Contable
            For r = 0 To dtSource.Rows.Count - 1
                Dim row As DataRow = dtSource.Rows(r)
                If Not IsDBNull(row(0)) Then
                    If bProcess Then
                        SplashScreenManager.Default.SetWaitFormDescription("Procesando Libro 5.1 - Fila: " & (r + 1).ToString & " de " & dtSource.Rows.Count.ToString)
                        NewRowLedger1(row)
                    Else
                        Exit For
                    End If
                End If
            Next
            dtAccounts = SelectDistinct(dtResult1, "C4")
            For r = 0 To dtAccounts.Rows.Count - 1
                Dim row As DataRow = dtAccounts.Rows(r)
                If Not IsDBNull(row(0)) Then
                    If bProcess Then
                        SplashScreenManager.Default.SetWaitFormDescription("Procesando Libro 5.3 - Fila: " & (r + 1).ToString & " de " & dtAccounts.Rows.Count.ToString)
                        NewRowLedger2(row)
                    Else
                        Exit For
                    End If
                End If
            Next
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SunatFlatFileGenerate()
        If bFlatFileGenerate Then
            beArchivoSalida1.EditValue = FolderBrowserDialog1.SelectedPath & "\LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "050100" & "00" & "1" & IIf(dtResult1.Rows.Count = 0, "0", "1") & "11" & ".TXT"
            beArchivoSalida2.EditValue = FolderBrowserDialog1.SelectedPath & "\LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "050300" & "00" & "1" & IIf(dtResult2.Rows.Count = 0, "0", "1") & "11" & ".TXT"
            If CreateTextDelimiterFile(beArchivoSalida1.EditValue, dtResult1.Select("C4<>''").CopyToDataTable, "|", False, False) And CreateTextDelimiterFile(beArchivoSalida2.EditValue, dtResult2.Select("C2<>''").CopyToDataTable, "|", False, False) Then
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Los archivos planos han sido generados satisfactoriamente.", "INformación", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generaron los archivos planos, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Se identificaron algunos errores en el proceso, no es posible generar el archivo PLE.  .", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Friend Sub NewRowLedger1(row As DataRow)
        Dim iPosition As Integer = 0
        Dim aSales As List(Of String) = ExistsDocSAP("Sales", row)
        Dim aPurchases As List(Of String) = ExistsDocSAP("Purchases", row)
        Dim aBanks As List(Of String) = ExistsDocSAP("Banks", row)
        Dim caractEspecial As New Regex("[^a-zA-Z0-9]")
        For icol = 0 To row.Table.Columns.Count - 1
            If IsDBNull(row(icol)) Then
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "La columna " & icol.ToString & " es nula, verifique el origen de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Next
        Try
            Dim drAccount As DataRow = GetLocalAccount(row(11))
            dtResult1.Rows.Add()
            iPosition = dtResult1.Rows.Count - 1
            dtResult1.Rows(iPosition).Item("C1") = Format(row(2), "yyyyMM00")
            dtResult1.Rows(iPosition).Item("C2") = row(0).ToString.Trim & "-" & row(3)
            dtResult1.Rows(iPosition).Item("C3") = rgTipoAsiento.EditValue & row(3)
            dtResult1.Rows(iPosition).Item("C4") = ""
            If Not drAccount Is Nothing Then
                dtResult1.Rows(iPosition).Item("C4") = drAccount(3)
            End If
            dtResult1.Rows(iPosition).Item("C5") = ""
            dtResult1.Rows(iPosition).Item("C6") = ""
            dtResult1.Rows(iPosition).Item("C7") = row(6)
            dtResult1.Rows(iPosition).Item("C8") = ""
            dtResult1.Rows(iPosition).Item("C9") = ""
            dtResult1.Rows(iPosition).Item("C10") = "00"
            dtResult1.Rows(iPosition).Item("C11") = ""
            dtResult1.Rows(iPosition).Item("C12") = row(0).ToString.Trim
            dtResult1.Rows(iPosition).Item("C20") = ""
            If Not aSales Is Nothing Then
                If aSales.Count > 0 Then
                    dtResult1.Rows(iPosition).Item("C8") = aSales(1)
                    dtResult1.Rows(iPosition).Item("C9") = aSales(2)
                    dtResult1.Rows(iPosition).Item("C10") = aSales(3)
                    dtResult1.Rows(iPosition).Item("C11") = aSales(4)
                    dtResult1.Rows(iPosition).Item("C12") = aSales(5)
                    dtResult1.Rows(iPosition).Item("C20") = aSales(6)
                End If
            End If
            If Not aPurchases Is Nothing Then
                If aPurchases.Count > 0 Then
                    dtResult1.Rows(iPosition).Item("C8") = aPurchases(1)
                    dtResult1.Rows(iPosition).Item("C9") = Strings.Right(aPurchases(2), 15)
                    dtResult1.Rows(iPosition).Item("C10") = aPurchases(3)
                    dtResult1.Rows(iPosition).Item("C11") = aPurchases(4)
                    dtResult1.Rows(iPosition).Item("C12") = aPurchases(5)
                    dtResult1.Rows(iPosition).Item("C20") = aPurchases(6)
                End If
            End If
            dtResult1.Rows(iPosition).Item("C13") = Format(row(2), "dd/MM/yyyy")
            dtResult1.Rows(iPosition).Item("C14") = ""
            dtResult1.Rows(iPosition).Item("C15") = IIf(row(1) > row(2), Format(row(2), "dd/MM/yyyy"), Format(row(1), "dd/MM/yyyy"))
            dtResult1.Rows(iPosition).Item("C16") = row(9)
            dtResult1.Rows(iPosition).Item("C17") = ""
            dtResult1.Rows(iPosition).Item("C18") = Format(row(7), "###########0.00")
            dtResult1.Rows(iPosition).Item("C19") = Format(row(8), "###########0.00")
            'If Not aSales Is Nothing Then
            '    dtResult1.Rows(iPosition).Item("C20") = aSales(6)
            'ElseIf Not aPurchases Is Nothing Then
            '    dtResult1.Rows(iPosition).Item("C20") = aPurchases(6)
            'Else
            '    dtResult1.Rows(iPosition).Item("C20") = ""
            'End If
            dtResult1.Rows(iPosition).Item("C21") = "1"
            If Not aBanks Is Nothing Then
                If aBanks.Count > 0 Then
                    dtResult1.Rows(iPosition).Item("C22") = aBanks(1)
                    dtResult1.Rows(iPosition).Item("C23") = aBanks(2)
                    dtResult1.Rows(iPosition).Item("C24") = aBanks(3)
                    dtResult1.Rows(iPosition).Item("C25") = aBanks(4)
                    If Not aSales Is Nothing Then
                        dtResult1.Rows(iPosition).Item("C26") = aSales(1) 'Tipo de documento de identidad del beneficiario
                        dtResult1.Rows(iPosition).Item("C27") = aSales(7) 'Número de documento de identidad del beneficiario
                        dtResult1.Rows(iPosition).Item("C28") = aSales(8) 'Razón Social o Nombres
                    ElseIf Not aPurchases Is Nothing Then
                        dtResult1.Rows(iPosition).Item("C26") = aPurchases(1)
                        dtResult1.Rows(iPosition).Item("C27") = aPurchases(7)
                        dtResult1.Rows(iPosition).Item("C28") = aPurchases(8)
                    Else
                        dtResult1.Rows(iPosition).Item("C26") = aBanks(5)
                        dtResult1.Rows(iPosition).Item("C27") = aBanks(6)
                        dtResult1.Rows(iPosition).Item("C28") = aBanks(7)
                    End If
                    dtResult1.Rows(iPosition).Item("C29") = aBanks(8)
                    If Mid(row(4), 3) >= "104" And Mid(row(4), 3) <= "109" And aBanks.Count = 0 Then
                        bFlatFileGenerate = False
                        dtResult1.Rows(iPosition).Item("ERR") = "No existen datos asociados a la cuenta de banco. "
                    End If
                End If
            End If
            If dtResult1.Rows(iPosition).Item("C4") = "" Then
                bFlatFileGenerate = False
                dtResult1.Rows(iPosition).Item("ERR") = "No existe equivalencia para la cuenta: " & row(11)
            End If
            If IsDBNull(dtResult1.Rows(iPosition).Item("ERR")) Then
                If Not IsDBNull(dtResult1.Rows(iPosition).Item("C10")) Then
                    If dtResult1.Rows(iPosition).Item("C10").ToString.Contains({"01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "12", "20", "22", "23", "25", "31", "33", "34", "35", "36", "40", "41", "46", "48", "50", "51", "52", "53", "54", "55", "56", "89"}) And dtResult1.Rows(iPosition).Item("C11").ToString = "" Then
                        bFlatFileGenerate = False
                        dtResult1.Rows(iPosition).Item("ERR") = "De acuerdo al tipo de comprobante asignado, la serie es obligatoria. "
                    End If
                    If caractEspecial.Matches(dtResult1.Rows(iPosition).Item("C11")).Count > 0 Then
                        bFlatFileGenerate = False
                        dtResult1.Rows(iPosition).Item("ERR") = "La columna 11 debe contener sólo valores alfanuméricos. "
                    End If
                    If dtResult1.Rows(iPosition).Item("C12").ToString = "" Then
                        bFlatFileGenerate = False
                        dtResult1.Rows(iPosition).Item("ERR") = "El número de comprobante es obligatorio. "
                    End If
                    If caractEspecial.Matches(dtResult1.Rows(iPosition).Item("C12")).Count > 0 Then
                        bFlatFileGenerate = False
                        dtResult1.Rows(iPosition).Item("ERR") = "La columna 12 debe contener sólo valores alfanuméricos. "
                    End If
                Else
                    bFlatFileGenerate = False
                    dtResult1.Rows(iPosition).Item("ERR") = "El asiento contable " & row(0) & " no contiene información asociada al comprobante de pago o documento. "
                End If
            End If
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Function ExistsDocSAP(type As String, SourceRow As DataRow) As List(Of String)
        Dim sResult As New List(Of String)
        Dim iDocLength As Integer = 0
        Dim row As DataRow
        Try
            If SourceRow(0).ToString.Trim = "" Then
                Return Nothing
            End If
            If type = "Sales" Then
                If dtSales.Select("[Document Number] = '" & Convert.ToInt64(SourceRow(0)).ToString & "'").Length > 0 Then
                    row = dtSales.Select("[Document Number] = '" & Convert.ToInt64(SourceRow(0)).ToString & "'")(0)
                    iDocLength = Len(row(4).ToString)
                    sResult.AddRange({"", "", "", "", "", "", "", "", "", ""})
                    If iDocLength = 11 Then
                        sResult(1) = "6"
                    ElseIf iDocLength = 8 Then
                        sResult(1) = "1"
                    Else
                        sResult(1) = "0"
                    End If
                    sResult(2) = IIf(row(4).ToString = "", "0", row(4))
                    'If iDocLength = 11 Then
                    '    sResult(3) = "01"
                    'Else
                    '    sResult(3) = "03"
                    'End If
                    sResult(3) = DataValidation("TipDoc", Microsoft.VisualBasic.Left(row(3), 2))
                    sResult(4) = GetTextFormatValue(sResult(3), "NroSer", Mid(row(3), 4, Len(row(3)) - 3)) '"00" & Microsoft.VisualBasic.Strings.Left(row(3).trim, 2)
                    sResult(5) = GetTextFormatValue(sResult(3), "NroDoc", Mid(row(3), 4, Len(row(3)) - 3)) 'Microsoft.VisualBasic.Strings.Right(row(3).trim, 7)
                    sResult(6) = "140100&" & Format(row(1), "yyyyMM00") & "&" & Format(Convert.ToInt64(row(0)), "0000000000") & "&" & SourceRow(3)
                    sResult(7) = row(4).ToString
                    sResult(8) = row(5).ToString
                Else
                    sResult = Nothing
                End If
            End If
            If type = "Purchases" Then
                If dtPurchases.Select("[Document Number] = '" & Convert.ToInt64(SourceRow(0)).ToString & "'").Length > 0 Then
                    row = dtPurchases.Select("[Document Number] = '" & Convert.ToInt64(SourceRow(0)).ToString & "'")(0)
                    iDocLength = Len(row(3))
                    sResult.AddRange({"", "", "", "", "", "", "", "", "", ""})
                    sResult(2) = IIf(row(3) = "", "0", row(3))
                    sResult(3) = DataValidation("TipDoc", Microsoft.VisualBasic.Left(row(2), 2))
                    sResult(4) = IIf(Mid(sResult(3), 1, 1) = "9", "", GetTextFormatValue(sResult(3), "NroSer", Mid(row(2), 4, Len(row(2)) - 3)))
                    sResult(5) = IIf(Mid(sResult(3), 1, 1) = "9", ExtractOnlyNumbers(GetTextFormatValue(sResult(3), "NroDoc", Mid(row(2), 4, Len(row(2)) - 3))), GetTextFormatValue(sResult(3), "NroDoc", Mid(row(2), 4, Len(row(2)) - 3)))
                    sResult(6) = IIf(Mid(sResult(3), 1, 1) = "9", "080200&", "080100&") & Format(row(17), "yyyyMM00") & "&" & Format(Convert.ToInt64(row(0)), "0000000000") & "&" & SourceRow(3)
                    sResult(7) = row(3)
                    sResult(8) = row(4)
                    sResult(1) = "0"
                    If Mid(sResult(3), 1, 1) <> "9" Then
                        If iDocLength = 11 Then
                            sResult(1) = "6"
                        ElseIf iDocLength = 8 Then
                            sResult(1) = "1"
                        End If
                    End If
                Else
                    sResult = Nothing
                End If
            End If
            If type = "Banks" Then
                If dtBanks.Select("[Document Number] = '" & Convert.ToInt64(SourceRow(0)).ToString & "'").Length > 0 Then
                    row = dtBanks.Select("[Document Number] = '" & Convert.ToInt64(SourceRow(0)).ToString & "'")(0)
                    If dtCashBankMapping.Select("Account LIKE '%" & CInt(row(1).ToString) & "' AND AccountType = 'B'").Length > 0 Then
                        Dim drCashBank As DataRow = dtCashBankMapping.Select("Account LIKE '%" & CInt(row(1).ToString) & "' AND AccountType = 'B'")(0)
                        sResult.AddRange({"", "", "", "", "", "", "", "", "", ""})
                        sResult(1) = drCashBank(6)
                        sResult(2) = drCashBank(7)
                        sResult(3) = GetPaymentType(row("Text"))
                        sResult(4) = RemoveCharacter(IIf(row(18).trim <> "", row(18), IIf(row(3) <> "", row(3), row(2))))
                        If row(28) <> "" Then
                            Dim aVendor As List(Of String) = GetDataVendor(Strings.Left(row(28), 10), Strings.Right(row(1).ToString.Trim, 1))
                            If Not aVendor Is Nothing Then
                                sResult(5) = aVendor(1)
                                sResult(6) = aVendor(2)
                                sResult(7) = aVendor(3)
                            End If
                        End If
                        sResult(8) = row(6)
                    Else
                        sResult = Nothing
                    End If
                End If
            End If
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error en la función ExistsDocSAP (" & type & ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return sResult
    End Function

    Friend Sub NewRowLedger2(row As DataRow)
        Dim iPosition As Integer = 0
        Try
            If dtAccountMapping.Select("LocalAccount='" & row(0) & "'").Length > 0 Then
                If dtResult2.Select("C2='" & row(0) & "'").Length = 0 Then
                    dtResult2.Rows.Add()
                    iPosition = dtResult2.Rows.Count - 1
                    dtResult2.Rows(iPosition).Item("C1") = seEjercicio.Text & Format(sePeriodo.EditValue, "00") & "01"
                    dtResult2.Rows(iPosition).Item("C2") = row(0)
                    dtResult2.Rows(iPosition).Item("C3") = dtAccountMapping.Select("LocalAccount='" & row(0) & "'")(0)(4)
                    dtResult2.Rows(iPosition).Item("C4") = "01"
                    dtResult2.Rows(iPosition).Item("C5") = "0"
                    dtResult2.Rows(iPosition).Item("C6") = "" 'StrDup(15, "0")
                    dtResult2.Rows(iPosition).Item("C7") = "" '"-"
                    dtResult2.Rows(iPosition).Item("C8") = "1"
                End If
            End If
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    'Private Sub LoadBankAccountMapping()
    '    dtCashBankMapping = LoadExcel(DBFileName, "MapeoCtas" & lueSociedad.EditValue & "$").Tables(0).Select("[G/L Account Type] = 'B'").CopyToDataTable
    'End Sub

    Friend Function ValueExists(dtResult As DataTable, condition As String) As Boolean
        Dim bResult As Boolean = False
        If dtResult.Rows.Count > 0 Then
            If dtResult.Select(condition).Length > 0 Then
                bResult = True
            End If
        End If
        Return bResult
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

    'Friend Function GetDueDays(PaytTerms As String) As Integer
    '    Dim iDays As Integer = 0
    '    iDays = DirectCast(dtPaytTerms.Select("Código = '" & PaytTerms & "'")(0).ItemArray(1), Double)
    '    Return iDays
    'End Function

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoOrigen.Properties.ButtonClick
        OpenFileDialog1.Filter = "Text files (*.txt)|*.txt|Excel Files (*.xls*)|*.xls*"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.InitialDirectory = IIf(My.Settings.LedgerSourceDirectory5 <> "", My.Settings.LedgerSourceDirectory5, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beArchivoOrigen.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
        ExportarExcel(gcLibroSunat)
    End Sub

    Private Sub BarButtonItem3_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem3.ItemClick, BarButtonItem4.ItemClick
        gcLibroSunat.DataSource = dsLibroSunat.Tables(LibroSunat & e.Item.Tag)
        gcLibroSunat.MainView.PopulateColumns()
        PivotGridControl1.DataSource = gcLibroSunat.DataSource
    End Sub

    Private Sub lueSociedad_EditValueChanged(sender As Object, e As EventArgs) Handles lueSociedad.EditValueChanged, seEjercicio.EditValueChanged, sePeriodo.EditValueChanged
        If lueSociedad.EditValue <> "" Then
            RUC = lueSociedad.GetColumnValue("CompanyTaxCode")
            SunatFileName1 = "LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "050100" & "00" & "1111" & ".TXT"
            SunatFileName2 = "LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "050300" & "00" & "1111" & ".TXT"
            If My.Settings.LedgerTargetDirectory5 <> "" Then
                beArchivoSalida1.EditValue = FolderBrowserDialog1.SelectedPath & "\" & SunatFileName1
                beArchivoSalida2.EditValue = FolderBrowserDialog1.SelectedPath & "\" & SunatFileName2
            End If
        End If
    End Sub

    Private Sub beArchivoSalida_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoSalida1.Properties.ButtonClick
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            beArchivoSalida1.EditValue = FolderBrowserDialog1.SelectedPath & "\" & SunatFileName1
            beArchivoSalida2.EditValue = FolderBrowserDialog1.SelectedPath & "\" & SunatFileName2
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
            If e.Column.FieldName = "C4" Then 'Cuenta Contable
                If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C4")) = "" Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                    bFlatFileGenerate = False
                End If
            End If
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
            'If e.Column.FieldName = "C8" Then 'Año de la DUA
            '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "50" And ((View.GetRowCellDisplayText(e.RowHandle, View.Columns("C8")) = "") Or (View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) < "1981" Or View.GetRowCellDisplayText(e.RowHandle, View.Columns("C8")) > Year(Now).ToString)) Then
            '        e.Appearance.BackColor = Color.Green
            '        e.Appearance.BackColor2 = Color.LightGreen
            '        bFlatFileGenerate = False
            '    End If
            'End If
            'If e.Column.FieldName = "C9" Then 'Número Comprobante de Pago
            '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C9")) = "" Then
            '        e.Appearance.BackColor = Color.Salmon
            '        e.Appearance.BackColor2 = Color.SeaShell
            '        bFlatFileGenerate = False
            '    End If
            'End If
            'If e.Column.FieldName = "C11" Then 'Tipo Documento de Identidad
            '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C11")) = "" Then
            '        e.Appearance.BackColor = Color.Salmon
            '        e.Appearance.BackColor2 = Color.SeaShell
            '        bFlatFileGenerate = False
            '    End If
            'End If
            'If e.Column.FieldName = "C12" Then 'Número Documento de Identidad
            '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")) = "" Then
            '        e.Appearance.BackColor = Color.Salmon
            '        e.Appearance.BackColor2 = Color.SeaShell
            '        bFlatFileGenerate = False
            '    End If
            'End If
            'If e.Column.FieldName = "C26" Then
            '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C26")) = "" Then 'Fecha Comprobante de Pago que se modifica (NC)
            '            e.Appearance.BackColor = Color.Salmon
            '            e.Appearance.BackColor2 = Color.SeaShell
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            'End If
            'If e.Column.FieldName = "C27" Then
            '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C27")) = "" Then 'Tipo Comprobante de Pago que se modifica (NC)
            '            e.Appearance.BackColor = Color.Salmon
            '            e.Appearance.BackColor2 = Color.SeaShell
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            'End If
            'If e.Column.FieldName = "C28" Then
            '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C28")) = "" Then 'Serie Comprobante de Pago que se modifica (NC)
            '            e.Appearance.BackColor = Color.Salmon
            '            e.Appearance.BackColor2 = Color.SeaShell
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            'End If
            'If e.Column.FieldName = "C30" Then
            '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C30")) = "" Then 'Número Comprobante de Pago que se modifica (NC)
            '            e.Appearance.BackColor = Color.Salmon
            '            e.Appearance.BackColor2 = Color.SeaShell
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            'End If
            'If e.Column.FieldName = "C27" Then
            '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C27")) = "50" Then 'Tipo Comprobante de Pago que se modifica (NC)
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C29")) = "" Then
            '            e.Appearance.BackColor = Color.Salmon
            '            e.Appearance.BackColor2 = Color.SeaShell
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            'End If

            'If e.Column.FieldName = "C41" Then 'Estado
            '    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C41")) = "" Then
            '        e.Appearance.BackColor = Color.Peru
            '        e.Appearance.BackColor2 = Color.LightYellow
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

    Private Sub beArchivoVentas_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoVentas.Properties.ButtonClick
        OpenFileDialog1.Filter = "Excel Files (*.xls*)|*.xls*|Text files (*.txt)|*.txt"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.InitialDirectory = IIf(My.Settings.LedgerSourceDirectory14 <> "", My.Settings.LedgerSourceDirectory14, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            sender.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub beArchivoCompras_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoCompras.Properties.ButtonClick
        OpenFileDialog1.Filter = "Excel Files (*.xls*)|*.xls*|Text files (*.txt)|*.txt"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.InitialDirectory = IIf(My.Settings.LedgerSourceDirectory8 <> "", My.Settings.LedgerSourceDirectory8, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            sender.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub beArchivoBancos_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoBancos.Properties.ButtonClick
        OpenFileDialog1.Filter = "Excel Files (*.xls*)|*.xls*|Text files (*.txt)|*.txt"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.InitialDirectory = IIf(My.Settings.LedgerSourceDirectory1 <> "", My.Settings.LedgerSourceDirectory1, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            sender.Text = OpenFileDialog1.FileName
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

    Friend Function GetTextFormatValue(DocType As String, Group As String, Value As String) As String
        Dim sResult As String = ""
        Dim NewValue As String = ""
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
                    If InStr(Value, "-") > 0 Then
                        Value = Strings.Left(Value, InStr(Value, "-") - IIf(Value.Contains("-"), 1, 0))
                        sResult = Strings.Right(StrDup(iPositions, "0") & Value.ToString.Trim, iPositions)
                    Else
                        Value = ""
                    End If
                End If
            ElseIf Group = "NroDoc" Then
                If InStr(Value, "-") > 0 Then
                    Value = Mid(Value, InStr(Value, "-") + 1, iPositions)
                End If
                For i = 1 To Value.Length
                    If TextContain(Mid(Value, i, 1), "OnlyNumbers") Then
                        NewValue = Mid(Value, i, 1)
                    Else
                        NewValue = ""
                    End If
                    sResult = sResult & NewValue
                Next
                sResult = Strings.Right(StrDup(iPositions, "0") & sResult.ToString.Trim, iPositions)
            End If
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error en la función GetTextFormatValue. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error en la función GetPositionsByDocType. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return iResult
    End Function

    Friend Function GetPaymentType(text As String) As String
        Dim sResult As String = ""
        If text.Contains("CHEQUE", "CHQ") Then
            sResult = "007"
        ElseIf text.Contains("TLC", " A ", "TRANS") Then
            sResult = "003"
        Else
            sResult = "999"
        End If

        Return sResult
    End Function

    Friend Function GetDataVendor(name As String, type As String) As List(Of String)
        Dim sResult As New List(Of String)
        Dim dtQuery As New DataTable
        Dim row As DataRow
        Try
            sResult.AddRange({"", "", "", ""})
            sResult(1) = "0"
            sResult(2) = StrDup(15, "0")
            sResult(3) = "-"
            If type <> "0" And name <> "" Then
                dtQuery = ExecuteAccessQuery("select * from Proveedores where left([Nombre],10) = '" & Replace(name, "'", "") & "' and NoRUC <> '' order by NoRUC desc").Tables(0)
                If dtQuery.Rows.Count > 0 Then
                    row = dtQuery.Rows(0)
                    sResult(1) = "6"
                    sResult(2) = row(1)
                    sResult(3) = row(2)
                End If
            Else
                sResult(1) = "0"
                sResult(2) = StrDup(15, "0")
                sResult(3) = "-"
            End If

        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return sResult
    End Function

    Private Sub bbiSunatPle_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSunatPle.ItemClick
        SunatFlatFileGenerate()
    End Sub
End Class