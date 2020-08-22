Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading

Public Class LibroCajaBancosForm
    Dim RUC, SunatFileName1, SunatFileName2 As String
    Dim LibroSunat As String = "LibroCajaBanco"
    Dim dsLibroSunat As New dsSunat
    Dim dsExcel As New DataSet
    Dim dtSource, dtTypePaytDoc, dtAccountMapping, dtResult1, dtResult2, dtSales, dtPurchases As New DataTable
    Dim bFlatFileGenerate As Boolean = True
    Dim bProcess As Boolean = True

    Private Sub LibroDiarioForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        seEjercicio.Value = Today.Year
        sePeriodo.Value = Today.Month
        FillCompany()
        FolderBrowserDialog1.SelectedPath = IIf(My.Settings.LedgerTargetDirectory1 <> "", My.Settings.LedgerTargetDirectory1, "")
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
                LoadSalesFile(beArchivoVentas.Text)
                LoadPurchasesFile(beArchivoCompras.Text)
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
        gcLibroSunat.Refresh()
        PivotGridControl1.DataSource = gcLibroSunat.DataSource
        PivotGridControl1.RefreshData()
        SplashScreenManager.CloseForm(False)
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

    'Private Sub ProcessLedger()
    '    Dim DocSAP, TxtRef As String
    '    Dim FecDoc, FecCtb As Date
    '    Dim dtSource, dtAccounts As New DataTable
    '    LoadCashBankAccountMapping()
    '    dtSource.Columns.Add("DocSAP", GetType(String)).AllowDBNull = True
    '    dtSource.Columns.Add("FecDoc", GetType(Date)).AllowDBNull = True
    '    dtSource.Columns.Add("FecCtb", GetType(Date)).AllowDBNull = True
    '    dtSource.Columns.Add("NumItm", GetType(String)).AllowDBNull = True
    '    dtSource.Columns.Add("CtaCtb", GetType(String)).AllowDBNull = True
    '    dtSource.Columns.Add("ClaCtb", GetType(String)).AllowDBNull = True
    '    dtSource.Columns.Add("CodMon", GetType(String)).AllowDBNull = True
    '    dtSource.Columns.Add("ImpDeb", GetType(Double)).AllowDBNull = True
    '    dtSource.Columns.Add("ImpCre", GetType(Double)).AllowDBNull = True
    '    dtSource.Columns.Add("TxtRef", GetType(String)).AllowDBNull = True
    '    dtSource.Columns.Add("CtaDes", GetType(String)).AllowDBNull = True
    '    dtSource.Columns.Add("CtaOri", GetType(String)).AllowDBNull = True
    '    Dim iPosition As Integer = 0
    '    Using sr As New StreamReader(beArchivoOrigen.Text)
    '        Dim lines As List(Of String) = New List(Of String)
    '        Dim bExit As Boolean = False
    '        Do While Not sr.EndOfStream
    '            lines.Add(sr.ReadLine())
    '        Loop
    '        Dim bSkip As Boolean = True
    '        For i As Integer = 0 To lines.Count - 1
    '            If TextContain(Microsoft.VisualBasic.Left(lines(i), 8), "OnlyNumbers") Then
    '                DocSAP = Mid(lines(i), 9, 11)
    '                FecDoc = Mid(lines(i), 35, 2) & "/" & Mid(lines(i), 37, 2) & "/" & "20" & Mid(lines(i), 39, 2)
    '                FecCtb = Mid(lines(i), 28, 2) & "/" & Mid(lines(i), 30, 2) & "/" & "20" & Mid(lines(i), 32, 2)
    '                TxtRef = Mid(lines(i), 62, 50)
    '                i = i + 1
    '            End If
    '            If TextContain(Mid(lines(i), 36, 3), "OnlyNumbers") And TextContain(Mid(lines(i), 61, 2), "OnlyNumbers") Then
    '                dtSource.Rows.Add()
    '                iPosition = dtSource.Rows.Count - 1
    '                dtSource.Rows(iPosition).Item(0) = DocSAP
    '                dtSource.Rows(iPosition).Item(1) = FecDoc
    '                dtSource.Rows(iPosition).Item(2) = FecCtb
    '                dtSource.Rows(iPosition).Item(3) = Mid(lines(i), 36, 3)
    '                If Mid(lines(i), 66, 10).Trim = "" Then
    '                    dtSource.Rows(iPosition).Item(4) = GetLocalAccount(Mid(lines(i), 45, 10), "0")
    '                    dtSource.Rows(iPosition).Item(11) = Mid(lines(i), 45, 10)
    '                Else
    '                    dtSource.Rows(iPosition).Item(4) = GetLocalAccount(Mid(lines(i), 66, 10), "0")
    '                    dtSource.Rows(iPosition).Item(11) = Mid(lines(i), 66, 10)
    '                End If
    '                dtSource.Rows(iPosition).Item(5) = Mid(lines(i), 61, 2)
    '                dtSource.Rows(iPosition).Item(6) = Mid(lines(i), 97, 3)
    '                dtSource.Rows(iPosition).Item(7) = IIf(Mid(lines(i), 100, 15).Trim = "", "0.00", Mid(lines(i), 100, 15))
    '                dtSource.Rows(iPosition).Item(8) = IIf(Mid(lines(i), 116, 15).Trim = "", "0.00", Mid(lines(i), 116, 15))
    '                dtSource.Rows(iPosition).Item(9) = TxtRef
    '                If Mid(lines(i), 66, 10).Trim = "" Then
    '                    dtSource.Rows(iPosition).Item(10) = GetLocalAccount(Mid(lines(i), 45, 10), "1")
    '                Else
    '                    dtSource.Rows(iPosition).Item(10) = GetLocalAccount(Mid(lines(i), 66, 10), "1")
    '                End If
    '            End If
    '        Next
    '    End Using
    '    Try
    '        dtResult1 = dsLibroSunat.Tables(LibroSunat & "1") 'Movimiento Efectivo
    '        dtResult2 = dsLibroSunat.Tables(LibroSunat & "2") 'Movimiento Bancario
    '        Dim drCashBank As DataRow
    '        For Each row As DataRow In dtSource.Rows
    '            If Not IsDBNull(row(0)) Then
    '                If bProcess Then
    '                    If dtCashBankMapping.Select("[G/L Local Account] = '" & row(4) & "' and [G/L Local Account Type] = 'C'").Length > 0 Then
    '                        NewRowLedger1(row)
    '                    End If
    '                    If dtCashBankMapping.Select("[G/L Local Account] = '" & row(4) & "' and [G/L Local Account Type] = 'B'").Length > 0 Then
    '                        drCashBank = dtCashBankMapping.Select("[G/L Local Account] = '" & row(4) & "' and [G/L Local Account Type] = 'B'")(0)
    '                        NewRowLedger2(row, drCashBank)
    '                    End If
    '                Else
    '                    Exit For
    '                End If
    '            End If
    '        Next
    '    Catch ex As Exception
    '        DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    End Try
    'End Sub

    Private Sub ProcessLedger()
        'Dim DocSAP, TxtRef As String
        'Dim FecDoc, FecCtb As Date
        Dim dtAccounts As New DataTable
        LoadCashBankAccountMapping()
        SplashScreenManager.Default.SetWaitFormDescription("Cargando datos externos...")
        dtSource = LoadExcel(beArchivoOrigen.Text, "{0}").Tables(0)
        UpdateItemSource()
        Try
            dtResult1 = dsLibroSunat.Tables(LibroSunat & "1") 'Movimiento Efectivo
            dtResult2 = dsLibroSunat.Tables(LibroSunat & "2") 'Movimiento Bancario
            For Each row As DataRow In dtSource.Rows
                If Not IsDBNull(row(0)) Then
                    If bProcess Then
                        If dtAccountMapping.Select("Account LIKE '%" & CInt(row(1).ToString) & "' AND AccountType = 'C'").Length > 0 Then
                            SplashScreenManager.Default.SetWaitFormDescription("Procesando Libro 1.1 - Fila: " & (dtSource.Rows.IndexOf(row) + 1).ToString & " de " & dtSource.Rows.Count.ToString)
                            NewRowLedger1(row)
                        End If
                        If dtAccountMapping.Select("Account LIKE '%" & CInt(row(1).ToString) & "' AND AccountType = 'B'").Length > 0 Then
                            SplashScreenManager.Default.SetWaitFormDescription("Procesando Libro 1.2 - Fila: " & (dtSource.Rows.IndexOf(row) + 1).ToString & " de " & dtSource.Rows.Count.ToString)
                            NewRowLedger2(row, dtAccountMapping.Select("Account LIKE '%" & CInt(row(1).ToString) & "' AND AccountType = 'B'")(0))
                        End If
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

    Private Sub UpdateItemSource()
        If Not IsDBNull(dtSource.Rows(0).Item(4)) Then
            Dim DocNum As String = dtSource.Rows(0).Item(4)
            Dim item As Integer = 0
            dtSource = dtSource.Select("[Amt#in loc#cur#]<>0", "[Document Number]").CopyToDataTable()
            'dtSource = dtSource.Select("[Amount in local currency' ]<>0", "[Document Number]").CopyToDataTable()
            For Each row As DataRow In dtSource.Rows
                If row(4) = DocNum Then
                    item = item + 1
                Else
                    item = 1
                End If
                row(5) = item
                DocNum = row(4)
            Next
        End If
    End Sub

    Private Sub SunatFlatFileGenerate()
        If bFlatFileGenerate Then
            beArchivoSalida1.EditValue = FolderBrowserDialog1.SelectedPath & "\LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "010100" & "00" & "1" & IIf(dtResult1.Rows.Count = 0, "0", "1") & "11" & ".TXT"
            beArchivoSalida2.EditValue = FolderBrowserDialog1.SelectedPath & "\LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "010200" & "00" & "1" & IIf(dtResult2.Rows.Count = 0, "0", "1") & "11" & ".TXT"
            If CreateTextDelimiterFile(beArchivoSalida1.EditValue, dtResult1, "|", False, False) And CreateTextDelimiterFile(beArchivoSalida2.EditValue, dtResult2, "|", False, False) Then
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
        Try
            dtResult1.Rows.Add()
            iPosition = dtResult1.Rows.Count - 1
            dtResult1.Rows(iPosition).Item("C1") = seEjercicio.Text & Format(sePeriodo.EditValue, "00") & "00"
            dtResult1.Rows(iPosition).Item("C2") = row(4).ToString & "-" & Format(CInt(row(5)), "000")
            dtResult1.Rows(iPosition).Item("C3") = Format(CInt(row(5)), "M000")
            dtResult1.Rows(iPosition).Item("C4") = GetLocalAccount(Format(CInt(row(1).ToString), "0000000000"), "0")
            dtResult1.Rows(iPosition).Item("C5") = ""
            dtResult1.Rows(iPosition).Item("C6") = ""
            dtResult1.Rows(iPosition).Item("C7") = row(14)
            If Not aSales Is Nothing Then
                dtResult1.Rows(iPosition).Item("C8") = aSales(3)
                dtResult1.Rows(iPosition).Item("C9") = aSales(4)
                dtResult1.Rows(iPosition).Item("C10") = aSales(5)
            ElseIf Not aPurchases Is Nothing Then
                dtResult1.Rows(iPosition).Item("C8") = aPurchases(3)
                dtResult1.Rows(iPosition).Item("C9") = aPurchases(4)
                dtResult1.Rows(iPosition).Item("C10") = aPurchases(5)
            Else
                dtResult1.Rows(iPosition).Item("C8") = "00"
                dtResult1.Rows(iPosition).Item("C9") = ""
                dtResult1.Rows(iPosition).Item("C10") = Format(CInt(row(4).ToString), "00000000")
            End If
            dtResult1.Rows(iPosition).Item("C11") = Format(row(8), "dd/MM/yyyy")
            dtResult1.Rows(iPosition).Item("C12") = ""
            dtResult1.Rows(iPosition).Item("C13") = Format(row(7), "dd/MM/yyyy")
            dtResult1.Rows(iPosition).Item("C14") = IIf(row(18) = "", row(3), row(18))
            dtResult1.Rows(iPosition).Item("C15") = ""
            dtResult1.Rows(iPosition).Item("C16") = Format(IIf(row(10) = "40", row(26), 0), "###########0.00")
            dtResult1.Rows(iPosition).Item("C17") = Format(IIf(row(10) = "50", row(26), 0), "###########0.00")
            If Not aSales Is Nothing Then
                dtResult1.Rows(iPosition).Item("C18") = aSales(6)
            ElseIf Not aPurchases Is Nothing Then
                dtResult1.Rows(iPosition).Item("C18") = aPurchases(6)
            Else
                dtResult1.Rows(iPosition).Item("C18") = ""
            End If
            dtResult1.Rows(iPosition).Item("C19") = "1" 'GetStatus(row(8), row(7))
            If dtResult1.Rows(iPosition).Item("C4") = "" Then
                dtResult1.Rows(iPosition).Item("C20") = "No existe equivalencia para la cuenta: " & row(1)
            End If
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger2(row As DataRow, drCashBank As DataRow)
        Dim iPosition As Integer = 0
        'Dim item As Integer = dtResult2.Select("[Document Number]='" & row(4).ToString & "'").Length + 1
        Dim aSales As List(Of String) = ExistsDocSAP("Sales", row)
        Dim aPurchases As List(Of String) = ExistsDocSAP("Purchases", row)
        Try
            Dim aVendor As List(Of String) = GetDataVendor(Strings.Left(row(28), 10), Strings.Right(row(1).ToString.Trim, 1))
            dtResult2.Rows.Add()
            iPosition = dtResult2.Rows.Count - 1
            dtResult2.Rows(iPosition).Item("C1") = Format(row(7), "yyyyMM00")
            dtResult2.Rows(iPosition).Item("C2") = row(4).ToString & "-" & Format(CInt(row(5)), "000")
            dtResult2.Rows(iPosition).Item("C3") = "M" & Format(CInt(row(5)), "000")
            dtResult2.Rows(iPosition).Item("C4") = drCashBank(5) 'Código Entidad Financiera
            dtResult2.Rows(iPosition).Item("C5") = drCashBank(6) 'Cuenta Bancaria Propia
            dtResult2.Rows(iPosition).Item("C6") = Format(row(7), "dd/MM/yyyy") 'Fecha de la operación
            dtResult2.Rows(iPosition).Item("C7") = GetPaymentType(row(18)) 'Medio de Pago de la operación
            dtResult2.Rows(iPosition).Item("C8") = IIf(row(18).trim <> "", row(18), IIf(row(3) <> "", row(3), row(2))) 'Descripción de la operación 
            If Not aSales Is Nothing Then
                dtResult2.Rows(iPosition).Item("C9") = aSales(3) 'Tipo de documento de identidad del beneficiario
                dtResult2.Rows(iPosition).Item("C10") = aSales(7) 'Número de documento de identidad del beneficiario
                dtResult2.Rows(iPosition).Item("C11") = aSales(8) 'Razón Social o Nombres
            ElseIf Not aPurchases Is Nothing Then
                dtResult2.Rows(iPosition).Item("C9") = aPurchases(3)
                dtResult2.Rows(iPosition).Item("C10") = aPurchases(7)
                dtResult2.Rows(iPosition).Item("C11") = aPurchases(8)
            Else
                dtResult2.Rows(iPosition).Item("C9") = aVendor(1)
                dtResult2.Rows(iPosition).Item("C10") = aVendor(2)
                dtResult2.Rows(iPosition).Item("C11") = aVendor(3)
            End If
            dtResult2.Rows(iPosition).Item("C12") = row(4) 'Número de operación
            dtResult2.Rows(iPosition).Item("C13") = Format(IIf(row(10) = "40", row(26), 0), "###########0.00")
            dtResult2.Rows(iPosition).Item("C14") = Format(IIf(row(10) = "50", row(26), 0), "###########0.00")
            dtResult2.Rows(iPosition).Item("C15") = GetStatus(row(8), row(7))
            If row(4).trim = "" Then
                dtResult2.Rows(iPosition).Item("C16") = "No existe equivalencia para la cuenta: " & row(1)
            End If
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Function GetDataVendor(name As String, type As String) As List(Of String)
        Dim sResult As New List(Of String)
        Dim row As DataRow
        Try
            sResult.AddRange({"", "", "", ""})
            sResult(1) = "0"
            sResult(2) = StrDup(15, "0")
            sResult(3) = "-"
            If type <> "0" And name <> "" Then
                If ExecuteAccessQuery("select * from Proveedores where left([Nombre],10) = '" & Replace(name, "'", "") & "' and NoRUC <> '' order by NoRUC desc").Tables(0).Rows.Count > 0 Then
                    row = ExecuteAccessQuery("select * from Proveedores where left([Nombre],10) = '" & Replace(name, "'", "") & "' and NoRUC <> '' order by NoRUC desc").Tables(0).Rows(0)
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
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return sResult
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

    Friend Function ExistsDocSAP(type As String, SourceRow As DataRow) As List(Of String)
        Dim sResult As New List(Of String)
        Dim iDocLength As Integer = 0
        Dim row As DataRow
        If type = "Sales" Then
            If dtSales.Select("[Document Number] = '" & Convert.ToInt64(SourceRow(4)).ToString & "'").Length > 0 Then
                row = dtSales.Select("[Document Number] = '" & Convert.ToInt64(SourceRow(4)).ToString & "'")(0)
                iDocLength = Len(row(4))
                sResult.AddRange({"", "", "", "", "", "", "", "", ""})
                If iDocLength = 11 Then
                    sResult(1) = "6"
                ElseIf iDocLength = 8 Then
                    sResult(1) = "1"
                Else
                    sResult(1) = "0"
                End If
                sResult(2) = row(4)
                If iDocLength = 11 Then
                    sResult(3) = "01"
                Else
                    sResult(3) = "03"
                End If
                sResult(4) = "00" & Microsoft.VisualBasic.Strings.Left(row(3).trim, 2)
                sResult(5) = Microsoft.VisualBasic.Strings.Right(row(3).trim, 7)
                sResult(6) = "140100&" & Format(row(1), "yyyyMM00") & "&" & Format(Convert.ToInt64(row(0)), "0000000000") & "&" & SourceRow(3)
                sResult(7) = row(4)
                sResult(8) = row(5)
            Else
                sResult = Nothing
            End If
        End If
        If type = "Purchases" Then
            If dtPurchases.Select("[Document Number] = '" & Convert.ToInt64(SourceRow(0)).ToString & "'").Length > 0 Then
                row = dtPurchases.Select("[Document Number] = '" & Convert.ToInt64(SourceRow(0)).ToString & "'")(0)
                iDocLength = Len(row(3))
                sResult.AddRange({"", "", "", "", "", "", "", "", ""})
                If iDocLength = 11 Then
                    sResult(1) = "6"
                ElseIf iDocLength = 8 Then
                    sResult(1) = "1"
                Else
                    sResult(1) = "0"
                End If
                sResult(2) = row(3)
                sResult(3) = DataValidation("TipDoc", Microsoft.VisualBasic.Left(row(2), 2))
                sResult(4) = Microsoft.VisualBasic.Mid(row(2), 3, 3)
                sResult(5) = Microsoft.VisualBasic.Mid(row(2), 7, 12)
                sResult(6) = IIf(sResult(3) = "91" Or sResult(3) = "97", "080200&", "080100&") & Format(row(17), "yyyyMM00") & "&" & Format(Convert.ToInt64(row(0)), "0000000000") & "&" & SourceRow(3)
                sResult(7) = row(3)
                sResult(8) = row(4)
            Else
                sResult = Nothing
            End If
        End If
        Return sResult
    End Function

    Friend Function ValueExists(dtResult As DataTable, condition As String) As Boolean
        Dim bResult As Boolean = False
        If dtResult.Rows.Count > 0 Then
            If dtResult.Select(condition).Length > 0 Then
                bResult = True
            End If
        End If
        Return bResult
    End Function

    Friend Function GetStatus(RefDate As Date, DocDate As Date) As String
        Dim status As String = ""
        If Format(RefDate, "yyyyMM") = Format(DocDate, "yyyyMM") Then
            status = "1"
        End If
        If Format(DocDate, "yyyyMM") < Format(RefDate, "yyyyMM") Then 'And DateDiff(DateInterval.Month, DocDate, RefDate) <= 12 Then
            status = "8"
        End If
        Return status
    End Function

    'Friend Function GetDueDays(PaytTerms As String) As Integer
    '    Dim iDays As Integer = 0
    '    iDays = DirectCast(dtPaytTerms.Select("Código = '" & PaytTerms & "'")(0).ItemArray(1), Double)
    '    Return iDays
    'End Function

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

    Private Sub BarButtonItem3_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem3.ItemClick, BarButtonItem4.ItemClick
        If e.Item.Tag = "1" Then
            PivotGridControl1.Fields.Item(1).FieldName = "C16"
            PivotGridControl1.Fields.Item(2).FieldName = "C17"
        Else
            PivotGridControl1.Fields.Item(1).FieldName = "C13"
            PivotGridControl1.Fields.Item(2).FieldName = "C14"
        End If
        gcLibroSunat.DataSource = dsLibroSunat.Tables(LibroSunat & e.Item.Tag)
        gcLibroSunat.MainView.PopulateColumns()
        PivotGridControl1.DataSource = gcLibroSunat.DataSource
        PivotGridControl1.RefreshData()
    End Sub

    Private Sub lueSociedad_EditValueChanged(sender As Object, e As EventArgs) Handles lueSociedad.EditValueChanged, seEjercicio.EditValueChanged, sePeriodo.EditValueChanged
        If lueSociedad.EditValue <> "" Then
            RUC = lueSociedad.GetColumnValue("CompanyTaxCode")
            SunatFileName1 = "LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "010100" & "00" & "1111" & ".TXT"
            SunatFileName2 = "LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "010200" & "00" & "1111" & ".TXT"
            If My.Settings.LedgerTargetDirectory1 <> "" Then
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
                    'bFlatFileGenerate = False
                End If
            End If
            '    If e.Column.FieldName = "C4" Then 'Fecha Comprobante de Pago
            '        If Format(CDate(View.GetRowCellDisplayText(e.RowHandle, View.Columns("C4"))), "yyyyMM") > seEjercicio.EditValue & Format(sePeriodo.EditValue, "00") Then
            '            e.Appearance.BackColor = Color.Salmon
            '            e.Appearance.BackColor2 = Color.SeaShell
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            '    If e.Column.FieldName = "C6" Then 'Tipo Comprobante de Pago
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "" Then
            '            e.Appearance.BackColor = Color.Salmon
            '            e.Appearance.BackColor2 = Color.SeaShell
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            '    If e.Column.FieldName = "C7" Then 'Serie Comprobante de Pago
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C7")) = "" Then
            '            e.Appearance.BackColor = Color.Salmon
            '            e.Appearance.BackColor2 = Color.SeaShell
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            '    If e.Column.FieldName = "C8" Then 'Año de la DUA
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "50" And ((View.GetRowCellDisplayText(e.RowHandle, View.Columns("C8")) = "") Or (View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) < "1981" Or View.GetRowCellDisplayText(e.RowHandle, View.Columns("C8")) > Year(Now).ToString)) Then
            '            e.Appearance.BackColor = Color.Green
            '            e.Appearance.BackColor2 = Color.LightGreen
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            '    If e.Column.FieldName = "C9" Then 'Número Comprobante de Pago
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C9")) = "" Then
            '            e.Appearance.BackColor = Color.Salmon
            '            e.Appearance.BackColor2 = Color.SeaShell
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            '    If e.Column.FieldName = "C11" Then 'Tipo Documento de Identidad
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C11")) = "" Then
            '            e.Appearance.BackColor = Color.Salmon
            '            e.Appearance.BackColor2 = Color.SeaShell
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            '    If e.Column.FieldName = "C12" Then 'Número Documento de Identidad
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")) = "" Then
            '            e.Appearance.BackColor = Color.Salmon
            '            e.Appearance.BackColor2 = Color.SeaShell
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            '    If e.Column.FieldName = "C26" Then
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
            '            If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C26")) = "" Then 'Fecha Comprobante de Pago que se modifica (NC)
            '                e.Appearance.BackColor = Color.Salmon
            '                e.Appearance.BackColor2 = Color.SeaShell
            '                bFlatFileGenerate = False
            '            End If
            '        End If
            '    End If
            '    If e.Column.FieldName = "C27" Then
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
            '            If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C27")) = "" Then 'Tipo Comprobante de Pago que se modifica (NC)
            '                e.Appearance.BackColor = Color.Salmon
            '                e.Appearance.BackColor2 = Color.SeaShell
            '                bFlatFileGenerate = False
            '            End If
            '        End If
            '    End If
            '    If e.Column.FieldName = "C28" Then
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
            '            If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C28")) = "" Then 'Serie Comprobante de Pago que se modifica (NC)
            '                e.Appearance.BackColor = Color.Salmon
            '                e.Appearance.BackColor2 = Color.SeaShell
            '                bFlatFileGenerate = False
            '            End If
            '        End If
            '    End If
            '    If e.Column.FieldName = "C30" Then
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
            '            If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C30")) = "" Then 'Número Comprobante de Pago que se modifica (NC)
            '                e.Appearance.BackColor = Color.Salmon
            '                e.Appearance.BackColor2 = Color.SeaShell
            '                bFlatFileGenerate = False
            '            End If
            '        End If
            '    End If
            '    If e.Column.FieldName = "C27" Then
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C27")) = "50" Then 'Tipo Comprobante de Pago que se modifica (NC)
            '            If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C29")) = "" Then
            '                e.Appearance.BackColor = Color.Salmon
            '                e.Appearance.BackColor2 = Color.SeaShell
            '                bFlatFileGenerate = False
            '            End If
            '        End If
            '    End If

            '    If e.Column.FieldName = "C41" Then 'Estado
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C41")) = "" Then
            '            e.Appearance.BackColor = Color.Peru
            '            e.Appearance.BackColor2 = Color.LightYellow
            '            bFlatFileGenerate = False
            '        End If
            '    End If
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

    'Friend Function GetLocalAccount(chart As String) As DataRow
    '    Dim drResult As DataRow = Nothing
    '    Dim dtResult As New DataTable
    '    dtResult = dtAccountMapping.Select("Account LIKE '%" & CInt(chart).ToString & "'").CopyToDataTable
    '    If dtResult.Rows.Count > 0 Then
    '        drResult = dtResult.Rows(0)
    '    End If
    '    Return drResult
    'End Function

    Friend Function GetLocalAccount(chart As String, type As String) As String
        Dim sResult As String = ""
        Dim drTemp As DataRow = dtAccountMapping.Select("Account LIKE '%" & chart & "'")(0)
        If drTemp.ItemArray.Count > 0 Then
            If type = "0" Then
                If Not IsDBNull(drTemp(2)) Then
                    sResult = drTemp(2)
                End If
            Else
                If Not IsDBNull(drTemp(3)) Then
                    sResult = drTemp(3)
                End If
            End If
        End If
        Return sResult
    End Function

    Friend Function DataValidation(column As String, value As String) As String
        Dim sResult As String = ""
        If column = "TipDoc" Then
            If dtTypePaytDoc.Select("Código = '" & value & "'").Length > 0 Then
                sResult = value
            End If
        End If
        If sResult = "" Then
            bFlatFileGenerate = False
        End If
        Return sResult
    End Function

    Private Sub LoadCashBankAccountMapping()
        dtAccountMapping = FillDataTable("AccountMapping", "CompanyCode='" & lueSociedad.EditValue & "' AND AccountType IN ('C','B')")
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


    Private Sub bbiSunatPle_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSunatPle.ItemClick
        SunatFlatFileGenerate()
    End Sub
End Class