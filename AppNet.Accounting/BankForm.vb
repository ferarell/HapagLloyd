Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.Collections

Public Class BankForm
    Dim Cuenta1, Cuenta2, Moneda, Formato As String
    Dim dtAccStat As New DataTable
    Dim dtProcess As DataTable = CreateFormatTable()

    Private Sub BankForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        seEjercicio.Value = Today.Year
        sePeriodo.Value = Today.Month
        FillCompany()
        FolderBrowserDialog1.SelectedPath = IIf(My.Settings.BankTargetDirectory <> "", My.Settings.BankTargetDirectory, "")
        LoadValidations()
        dtAccStat = ExecuteAccessQuery("select * from EstadoCuentaBanco where Sociedad='####'").Tables(0)
    End Sub

    Private Sub FillCompany()
        lueSociedad.Properties.DataSource = FillDataTable("Company", "")
        lueSociedad.Properties.DisplayMember = "CompanyDescription"
        lueSociedad.Properties.ValueMember = "CompanyCode"
    End Sub

    Private Sub FillAccountBank()
        lueCuenta.Properties.DataSource = FillDataTable("AccountBank", "CompanyCode='" & lueSociedad.EditValue & "'")
        lueCuenta.Properties.DisplayMember = "AccountBankCode"
        lueCuenta.Properties.ValueMember = "AccountBankCode"
    End Sub

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoOrigen.Properties.ButtonClick
        OpenFileDialog1.Filter = "Excel Files (*.xls*)|*.xls*"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.InitialDirectory = IIf(My.Settings.BankSourceDirectory <> "", My.Settings.BankSourceDirectory, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beArchivoOrigen.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub beArchivoSalida_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoSalida.Properties.ButtonClick
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            beArchivoSalida.Text = FolderBrowserDialog1.SelectedPath & "\" & Cuenta1 & ".txt"
        End If
    End Sub

    Private Sub ProcessBankStatement()
        Dim SourceFile As String = beArchivoOrigen.EditValue
        Dim dtExcel As New DataTable
        dtExcel = LoadExcelWH(SourceFile, "{0}").Tables(0)
        If dtExcel.Rows.Count > 0 Then
            dtAccStat.Rows.Clear()
            Try
                If Not ValidAccount(dtExcel, Formato) Then
                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El estado de cuenta seleccionado no es válido para esta cuenta bancaria.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
                If ProcessDataSource(dtExcel, Formato) Then
                    If lueSociedad.EditValue = "" Then '"4040" Then
                        If UpdateBankStatement() Then
                            TargetFileGenerate(Formato)
                        End If
                    Else
                        TargetFileGenerate(Formato)
                    End If
                    
                End If
            Catch ex As Exception
                SplashScreenManager.CloseForm(False)
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Se generó un error al procesar el estado de cuenta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
    
            'If Formato = "02.1.0" Then
            '    TargetFileGenerate0210(dsExcel.Tables(0))
            'ElseIf Formato = "02.2.0" Then
            '    TargetFileGenerate0220(dsExcel.Tables(0))
            'ElseIf Formato = "07.1.0" Then
            '    TargetFileGenerate0710(dsExcel.Tables(0))
            'ElseIf Formato = "18.1.0" Then
            '    TargetFileGenerate1810(dsExcel.Tables(0))
            'End If
        End If
    End Sub

    Friend Function ValidAccount(dtSource As DataTable, FormatBank As String) As Boolean
        Dim bResult As Boolean = True
        If FormatBank = "02.1.0" Then
            If Not dtSource.Rows(0)(1).ToString.Contains(lueCuenta.GetColumnValue("CuentaBancaria")) Then
                bResult = False
            End If
        ElseIf FormatBank = "02.2.0" Then
            If Not dtSource.Rows(1)(0).ToString.Contains(Replace(lueCuenta.GetColumnValue("CuentaBancaria"), "-", "")) Then
                bResult = False
            End If
        ElseIf FormatBank = "07.1.0" Then
            If Not dtSource.Rows(9)(0).ToString.Contains(lueCuenta.GetColumnValue("CuentaBancaria")) Then
                bResult = False
            End If
        ElseIf FormatBank = "18.1.0" Then
            'If Not Replace(dtSource.Rows(0)(2).ToString, " ", "").Contains(lueCuenta.GetColumnValue("CuentaBancaria")) Then
            '    bResult = False
            'End If
        End If
        Return bResult
    End Function

    Friend Function ProcessDataSource(dtSource As DataTable, FormatBank As String) As Boolean
        Dim bResult As Boolean = True
        Dim aFields As New ArrayList
        Dim PosKey1, PosKey2 As String
        Dim Descri, Opera, Glosa As String
        Dim Monto As Double
        Dim Fecha As Date
        Dim iPos1 As Integer = 0
        Dim iPos2 As Integer = IIf(FormatBank = "02.1.0", 5, IIf(FormatBank = "02.2.0", 1, IIf(FormatBank = "07.1.0", 9, IIf(FormatBank = "18.1.0", 1, 0))))
        Dim iPos3 As Integer = 0
        dtProcess.Rows.Clear()
        dtProcess.Rows.Add()
        dtProcess.Rows(iPos1).Item(0) = "(02) Company Code"
        dtProcess.Rows(iPos1).Item(1) = "(08) Posting Key"
        dtProcess.Rows(iPos1).Item(2) = "(03) Account Number"
        dtProcess.Rows(iPos1).Item(3) = "(10) Amount in Document Currency"
        dtProcess.Rows(iPos1).Item(4) = "(09) Currency Key"
        dtProcess.Rows(iPos1).Item(5) = "(13) Text"
        dtProcess.Rows(iPos1).Item(6) = "(22) Reference Document Number"
        dtProcess.Rows(iPos1).Item(7) = "(28) Value Date"
        dtProcess.Rows(iPos1).Item(8) = "(23) Assignment Number"
        dtProcess.Rows(iPos1).Item(9) = "(06) Posting Date"
        dtProcess.Rows(iPos1).Item(10) = "(07) Document Date"
        dtProcess.Rows(iPos1).Item(11) = "(05) Document Type"
        Fecha = GetFieldsByFormat(dtSource.Rows(iPos2), FormatBank)(0)
        For i = iPos2 To dtSource.Rows.Count - 1
            If FormatBank = "07.1.0" And dtSource.Rows(i)(0) <> Replace(lueCuenta.EditValue, "-", "") Then
                Continue For
            End If
            iPos3 += 1
            aFields = GetFieldsByFormat(dtSource.Rows(i), FormatBank)
            PosKey1 = "40"
            PosKey2 = "50"
            Descri = aFields(1) 'dtSource.Rows(i).Item(2)
            Glosa = iPos3.ToString & "-EC " & aFields(1) '(i - 3).ToString & "-EC " & dtSource.Rows(i).Item(2)
            If IsDBNull(aFields(2)) Then
                Monto = 0
            Else
                Monto = aFields(2) 'dtSource.Rows(i).Item(3)
            End If

            Opera = aFields(3) 'dtSource.Rows(i).Item(6)
            If ExecuteAccessQuery("select * from EstadoCuentaBanco where Sociedad='" & lueSociedad.EditValue & "' and Periodo='" & seEjercicio.Text & Format(sePeriodo.EditValue, "00") & "' and CuentaBancaria='" & lueCuenta.GetColumnValue("CuentaBancaria") & "' and CuentaContable='" & lueCuenta.GetColumnValue("CuentaContable1") & "' and Fecha=" & Format(CDate(aFields(0)), "#MM/dd/yyyy#") & " and Descripcion='" & Descri & "' and Importe=" & Monto.ToString & " and Operacion='" & Opera & "' and Hora='" & aFields(4) & "' and Referencia='" & aFields(5) & "'").Tables(0).Rows.Count > 0 Then
                Continue For
            End If
            If Fecha <> aFields(0) Then
                InsertSaveText()
                Fecha = aFields(0)
            End If
            If Monto < 0 Then
                PosKey1 = "50"
                PosKey2 = "40"
            End If
            dtProcess.Rows.Add()
            iPos1 = dtProcess.Rows.Count - 1
            dtProcess.Rows(iPos1).Item(0) = lueSociedad.EditValue
            dtProcess.Rows(iPos1).Item(1) = PosKey1
            dtProcess.Rows(iPos1).Item(2) = Cuenta1
            dtProcess.Rows(iPos1).Item(3) = Format(Math.Abs(Math.Round(Monto, 2)), "#0.00")
            dtProcess.Rows(iPos1).Item(4) = Moneda
            dtProcess.Rows(iPos1).Item(5) = Glosa
            dtProcess.Rows(iPos1).Item(6) = Format(sePeriodo.EditValue, "00")
            dtProcess.Rows(iPos1).Item(7) = Format(Fecha, "dd.MM.yyyy")
            dtProcess.Rows(iPos1).Item(8) = Opera
            dtProcess.Rows(iPos1).Item(9) = Format(Fecha, "dd.MM.yyyy")
            dtProcess.Rows(iPos1).Item(10) = Format(Fecha, "dd.MM.yyyy")
            dtProcess.Rows(iPos1).Item(11) = "SB"
            dtProcess.Rows.Add()
            iPos1 = dtProcess.Rows.Count - 1
            dtProcess.Rows(iPos1).Item(0) = lueSociedad.EditValue
            dtProcess.Rows(iPos1).Item(1) = PosKey2
            dtProcess.Rows(iPos1).Item(2) = Cuenta2
            dtProcess.Rows(iPos1).Item(3) = Format(Math.Abs(Math.Round(Monto, 2)), "#0.00")
            dtProcess.Rows(iPos1).Item(4) = Moneda
            dtProcess.Rows(iPos1).Item(5) = Glosa
            dtProcess.Rows(iPos1).Item(6) = Format(sePeriodo.EditValue, "00")
            dtProcess.Rows(iPos1).Item(7) = Format(Fecha, "dd.MM.yyyy")
            dtProcess.Rows(iPos1).Item(8) = Opera
            dtProcess.Rows(iPos1).Item(9) = Format(Fecha, "dd.MM.yyyy")
            dtProcess.Rows(iPos1).Item(10) = Format(Fecha, "dd.MM.yyyy")
            dtProcess.Rows(iPos1).Item(11) = "SB"

            dtAccStat.Rows.Add(lueSociedad.EditValue, seEjercicio.Text & Format(sePeriodo.EditValue, "00"), lueCuenta.GetColumnValue("CuentaBancaria"), lueCuenta.GetColumnValue("CuentaContable1"), Fecha, Descri, Monto.ToString, Opera, aFields(4), aFields(5), UserApp, Now)
        Next
        InsertSaveText()
        Return bResult
    End Function

    Private Sub InsertSaveText()
        dtProcess.Rows.Add()
        dtProcess.Rows(dtProcess.Rows.Count - 1).Item(0) = "(99) Save"
    End Sub

    Friend Function GetFieldsByFormat(dtRow As DataRow, FormatBank As String) As ArrayList
        Dim aReturn As New ArrayList
        For i = 0 To dtRow.ItemArray.Count - 1
            If IsDBNull(dtRow(i)) Then
                If dtRow.Table.Columns(i).DataType = GetType(Double) Or IsNumeric(dtRow(i)) Then
                    dtRow(i) = 0
                    Continue For
                End If
                If dtRow.Table.Columns(i).DataType = GetType(String) Then
                    dtRow(i) = ""
                End If
            End If
        Next
        If FormatBank = "02.1.0" Then
            aReturn.Add(dtRow(0)) 'Fecha
            aReturn.Add(RemoveCharacter(dtRow(2))) 'Descripción/Glosa
            aReturn.Add(dtRow(3)) 'Importe
            aReturn.Add(dtRow(6)) 'Operación    
            aReturn.Add(dtRow(7)) 'Hora
            aReturn.Add(RemoveCharacter(dtRow(10))) 'Referencia
        ElseIf FormatBank = "02.2.0" Then
            aReturn.Add(dtRow(1))
            aReturn.Add(RemoveCharacter(dtRow(3)))
            aReturn.Add(dtRow(5).trim & dtRow(6))
            aReturn.Add(dtRow(4))
            aReturn.Add("")
            aReturn.Add(RemoveCharacter(dtRow(8)) & "-" & RemoveCharacter(dtRow(9)))
        ElseIf FormatBank = "07.1.0" Then
            aReturn.Add(Mid(dtRow(12),4,3) & Mid(dtRow(12),1,3) & Mid(dtRow(12),7,4))
            aReturn.Add(RemoveCharacter(dtRow(16)))
            aReturn.Add(Replace(dtRow(10), "'", ""))
            aReturn.Add(dtRow(13))
            aReturn.Add("")
            aReturn.Add(RemoveCharacter(dtRow(14)))
        ElseIf FormatBank = "18.1.0" Then
            aReturn.Add(dtRow(1))
            aReturn.Add(RemoveCharacter(dtRow(2)))
            If Not IsNumeric(dtRow(6)) Then
                dtRow(6) = Math.Abs(CDbl(IIf(dtRow(6) = "", 0, dtRow(6))))
            End If
            If Not IsNumeric(dtRow(7)) Then
                dtRow(7) = Math.Abs(CDbl(IIf(dtRow(7) = "", 0, dtRow(7))))
            End If
            aReturn.Add(CDbl(dtRow(7)) - CDbl(dtRow(6)))
            aReturn.Add(dtRow(3))
            aReturn.Add("")
            aReturn.Add("")
            End If
            Return aReturn
    End Function

    Friend Function UpdateBankStatement() As Boolean
        Dim bResult As Boolean = True
        For Each row As DataRow In dtAccStat.Rows
            If Not InsertIntoAccess("EstadoCuentaBanco", row) Then
                bResult = False
            End If
        Next
        Return bResult
    End Function

    Friend Function TargetFileGenerate(format As String) As Boolean
        Dim bResult As Boolean = True
        If CreateTextDelimiterFile(beArchivoSalida.EditValue, dtProcess, Convert.ToChar(Keys.Tab), False, False) Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El archivo plano ha sido generado satisfactoriamente.", "INformación", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generó el archivo plano, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
        Return bResult
    End Function

    Private Sub TargetFileGenerate0210(ByRef dtBank As Data.DataTable)
        Dim dtProcess As Data.DataTable = CreateFormatTable()
        Dim Descri, Opera, Glosa, PosKey1, PosKey2 As String
        Dim DocType As String = "SB"
        Dim Monto As Double
        Dim Fecha As Date
        'Dim iColumns As Integer = dtProcess.Columns.Count
        Dim iRow, iPosition As Integer
        dtProcess.Rows.Add()
        dtProcess.Rows(iPosition).Item(0) = "(02) Company Code"
        dtProcess.Rows(iPosition).Item(1) = "(08) Posting Key"
        dtProcess.Rows(iPosition).Item(2) = "(03) Account Number"
        dtProcess.Rows(iPosition).Item(3) = "(10) Amount in Document Currency"
        dtProcess.Rows(iPosition).Item(4) = "(09) Currency Key"
        dtProcess.Rows(iPosition).Item(5) = "(13) Text"
        dtProcess.Rows(iPosition).Item(6) = "(22) Reference Document Number"
        dtProcess.Rows(iPosition).Item(7) = "(28) Value Date"
        dtProcess.Rows(iPosition).Item(8) = "(23) Assignment Number"
        dtProcess.Rows(iPosition).Item(9) = "(06) Posting Date"
        dtProcess.Rows(iPosition).Item(10) = "(07) Document Date"
        dtProcess.Rows(iPosition).Item(11) = "(05) Document Type"
        iRow = 4
        iPosition = 0
        dtBank.Rows.Add()
        Do While dtBank.Rows(iRow).IsNull(0) = False
            Fecha = dtBank.Rows(iRow).Item(0)
            Do While Fecha = dtBank.Rows(iRow).Item(0)
                Descri = dtBank.Rows(iRow).Item(2)
                Opera = dtBank.Rows(iRow).Item(6)
                Monto = dtBank.Rows(iRow).Item(3)
                Glosa = (iRow - 3).ToString & "-EC " & dtBank.Rows(iRow).Item(2)
                If Monto < 0 Then
                    PosKey1 = "50"
                    PosKey2 = "40"
                Else
                    PosKey1 = "40"
                    PosKey2 = "50"
                End If
                dtProcess.Rows.Add()
                iPosition = dtProcess.Rows.Count - 1
                dtProcess.Rows(iPosition).Item(0) = lueSociedad.EditValue
                dtProcess.Rows(iPosition).Item(1) = PosKey1
                dtProcess.Rows(iPosition).Item(2) = Cuenta1
                dtProcess.Rows(iPosition).Item(3) = Format(Math.Abs(Math.Round(Monto, 2)), "#0.00")
                dtProcess.Rows(iPosition).Item(4) = Moneda
                dtProcess.Rows(iPosition).Item(5) = Glosa
                dtProcess.Rows(iPosition).Item(6) = Format(sePeriodo.EditValue, "00")
                dtProcess.Rows(iPosition).Item(7) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(8) = Opera
                dtProcess.Rows(iPosition).Item(9) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(10) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(11) = DocType
                dtProcess.Rows.Add()
                iPosition = dtProcess.Rows.Count - 1
                dtProcess.Rows(iPosition).Item(0) = lueSociedad.EditValue
                dtProcess.Rows(iPosition).Item(1) = PosKey2
                dtProcess.Rows(iPosition).Item(2) = Cuenta2
                dtProcess.Rows(iPosition).Item(3) = Format(Math.Abs(Math.Round(Monto, 2)), "#0.00")
                dtProcess.Rows(iPosition).Item(4) = Moneda
                dtProcess.Rows(iPosition).Item(5) = Glosa
                dtProcess.Rows(iPosition).Item(6) = Format(sePeriodo.EditValue, "00")
                dtProcess.Rows(iPosition).Item(7) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(8) = Opera
                dtProcess.Rows(iPosition).Item(9) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(10) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(11) = DocType
                iRow = iRow + 1
                If iRow >= dtBank.Rows.Count - 1 Then
                    Exit Do
                End If
            Loop
            dtProcess.Rows.Add()
            iPosition = dtProcess.Rows.Count - 1
            dtProcess.Rows(iPosition).Item(0) = "(99) Save"
        Loop
        If CreateTextDelimiterFile(beArchivoSalida.EditValue, dtProcess, Convert.ToChar(Keys.Tab), False, False) Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El archivo plano ha sido generado satisfactoriamente.", "INformación", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generó el archivo plano, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub TargetFileGenerate0220(ByRef dtBank As Data.DataTable)
        Dim dtProcess As Data.DataTable = CreateFormatTable()
        Dim Descri, Operador, Glosa, PosKey1, PosKey2 As String
        Dim DocType As String = "SB"
        Dim Monto As Double
        Dim Fecha As Date
        Dim iColumns As Integer = dtProcess.Columns.Count
        Dim iRow, iPosition As Integer
        dtProcess.Rows.Add()
        dtProcess.Rows(iPosition).Item(0) = "(02) Company Code"
        dtProcess.Rows(iPosition).Item(1) = "(08) Posting Key"
        dtProcess.Rows(iPosition).Item(2) = "(03) Account Number"
        dtProcess.Rows(iPosition).Item(3) = "(10) Amount in Document Currency"
        dtProcess.Rows(iPosition).Item(4) = "(09) Currency Key"
        dtProcess.Rows(iPosition).Item(5) = "(13) Text"
        dtProcess.Rows(iPosition).Item(6) = "(22) Reference Document Number"
        dtProcess.Rows(iPosition).Item(7) = "(28) Value Date"
        dtProcess.Rows(iPosition).Item(8) = "(23) Assignment Number"
        dtProcess.Rows(iPosition).Item(9) = "(06) Posting Date"
        dtProcess.Rows(iPosition).Item(10) = "(07) Document Date"
        dtProcess.Rows(iPosition).Item(11) = "(05) Document Type"
        iRow = 0
        iPosition = 0
        dtBank.Rows.Add()
        Do While dtBank.Rows(iRow).IsNull(0) = False
            Fecha = dtBank.Rows(iRow).Item(1)
            Do While Fecha = dtBank.Rows(iRow).Item(1)
                Descri = dtBank.Rows(iRow).Item(3)
                Operador = Trim(dtBank.Rows(iRow).Item(5))
                Monto = dtBank.Rows(iRow).Item(6)
                Glosa = (iRow + 1).ToString & "-EC " & dtBank.Rows(iRow).Item(3)
                If Operador = "-" Then
                    PosKey1 = "50"
                    PosKey2 = "40"
                Else
                    PosKey1 = "40"
                    PosKey2 = "50"
                End If
                dtProcess.Rows.Add()
                iPosition = dtProcess.Rows.Count - 1
                dtProcess.Rows(iPosition).Item(0) = lueSociedad.EditValue
                dtProcess.Rows(iPosition).Item(1) = PosKey1
                dtProcess.Rows(iPosition).Item(2) = Cuenta1
                dtProcess.Rows(iPosition).Item(3) = Format(Math.Abs(Math.Round(Monto, 2)), "#0.00")
                dtProcess.Rows(iPosition).Item(4) = Moneda
                dtProcess.Rows(iPosition).Item(5) = Glosa
                dtProcess.Rows(iPosition).Item(6) = Format(sePeriodo.EditValue, "00")
                dtProcess.Rows(iPosition).Item(7) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(8) = "" 'Opera
                dtProcess.Rows(iPosition).Item(9) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(10) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(11) = DocType
                dtProcess.Rows.Add()
                iPosition = dtProcess.Rows.Count - 1
                dtProcess.Rows(iPosition).Item(0) = lueSociedad.EditValue
                dtProcess.Rows(iPosition).Item(1) = PosKey2
                dtProcess.Rows(iPosition).Item(2) = Cuenta2
                dtProcess.Rows(iPosition).Item(3) = Format(Math.Abs(Math.Round(Monto, 2)), "#0.00")
                dtProcess.Rows(iPosition).Item(4) = Moneda
                dtProcess.Rows(iPosition).Item(5) = Glosa
                dtProcess.Rows(iPosition).Item(6) = Format(sePeriodo.EditValue, "00")
                dtProcess.Rows(iPosition).Item(7) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(8) = "" 'Opera
                dtProcess.Rows(iPosition).Item(9) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(10) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(11) = DocType
                iRow = iRow + 1
                If iRow >= dtBank.Rows.Count - 1 Then
                    Exit Do
                End If
            Loop
            dtProcess.Rows.Add()
            iPosition = dtProcess.Rows.Count - 1
            dtProcess.Rows(iPosition).Item(0) = "(99) Save"
        Loop
        If CreateTextDelimiterFile(beArchivoSalida.EditValue, dtProcess, Convert.ToChar(Keys.Tab), False, False) Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El archivo plano ha sido generado satisfactoriamente.", "INformación", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generó el archivo plano, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub TargetFileGenerate0710(ByRef dtBank As Data.DataTable)
        Dim dtProcess As Data.DataTable = CreateFormatTable()
        Dim Descri, Opera, Glosa, PosKey1, PosKey2 As String
        Dim DocType As String = "SB"
        Dim Monto As Double
        Dim Fecha As Date
        Dim iColumns As Integer = dtProcess.Columns.Count
        Dim iRow, iPosition As Integer
        dtProcess.Rows.Add()
        dtProcess.Rows(iPosition).Item(0) = "(02) Company Code"
        dtProcess.Rows(iPosition).Item(1) = "(08) Posting Key"
        dtProcess.Rows(iPosition).Item(2) = "(03) Account Number"
        dtProcess.Rows(iPosition).Item(3) = "(10) Amount in Document Currency"
        dtProcess.Rows(iPosition).Item(4) = "(09) Currency Key"
        dtProcess.Rows(iPosition).Item(5) = "(13) Text"
        dtProcess.Rows(iPosition).Item(6) = "(22) Reference Document Number"
        dtProcess.Rows(iPosition).Item(7) = "(28) Value Date"
        dtProcess.Rows(iPosition).Item(8) = "(23) Assignment Number"
        dtProcess.Rows(iPosition).Item(9) = "(06) Posting Date"
        dtProcess.Rows(iPosition).Item(10) = "(07) Document Date"
        dtProcess.Rows(iPosition).Item(11) = "(05) Document Type"
        iRow = 8
        iPosition = 0
        dtBank.Rows.Add()
        Do While dtBank.Rows(iRow).IsNull(0) = False
            Fecha = dtBank.Rows(iRow).Item(0)
            Do While Fecha = dtBank.Rows(iRow).Item(0)
                Descri = dtBank.Rows(iRow).Item(1)
                Opera = IIf(dtBank.Rows(iRow).IsNull(5), "", dtBank.Rows(iRow).Item(5))
                Monto = dtBank.Rows(iRow).Item(3)
                Glosa = (iRow - 7).ToString & "-EC " & dtBank.Rows(iRow).Item(2)
                If Monto < 0 Then
                    PosKey1 = "50"
                    PosKey2 = "40"
                Else
                    PosKey1 = "40"
                    PosKey2 = "50"
                End If
                dtProcess.Rows.Add()
                iPosition = dtProcess.Rows.Count - 1
                dtProcess.Rows(iPosition).Item(0) = lueSociedad.EditValue
                dtProcess.Rows(iPosition).Item(1) = PosKey1
                dtProcess.Rows(iPosition).Item(2) = Cuenta1
                dtProcess.Rows(iPosition).Item(3) = Format(Math.Abs(Math.Round(Monto, 2)), "#0.00")
                dtProcess.Rows(iPosition).Item(4) = Moneda
                dtProcess.Rows(iPosition).Item(5) = Glosa
                dtProcess.Rows(iPosition).Item(6) = Format(sePeriodo.EditValue, "00")
                dtProcess.Rows(iPosition).Item(7) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(8) = Opera
                dtProcess.Rows(iPosition).Item(9) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(10) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(11) = DocType
                dtProcess.Rows.Add()
                iPosition = dtProcess.Rows.Count - 1
                dtProcess.Rows(iPosition).Item(0) = lueSociedad.EditValue
                dtProcess.Rows(iPosition).Item(1) = PosKey2
                dtProcess.Rows(iPosition).Item(2) = Cuenta2
                dtProcess.Rows(iPosition).Item(3) = Format(Math.Abs(Math.Round(Monto, 2)), "#0.00")
                dtProcess.Rows(iPosition).Item(4) = Moneda
                dtProcess.Rows(iPosition).Item(5) = Glosa
                dtProcess.Rows(iPosition).Item(6) = Format(sePeriodo.EditValue, "00")
                dtProcess.Rows(iPosition).Item(7) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(8) = Opera
                dtProcess.Rows(iPosition).Item(9) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(10) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(11) = DocType
                iRow = iRow + 1
                If iRow >= dtBank.Rows.Count - 1 Then
                    Exit Do
                End If
            Loop
            dtProcess.Rows.Add()
            iPosition = dtProcess.Rows.Count - 1
            dtProcess.Rows(iPosition).Item(0) = "(99) Save"
        Loop
        If CreateTextDelimiterFile(beArchivoSalida.EditValue, dtProcess, Convert.ToChar(Keys.Tab), False, False) Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El archivo plano ha sido generado satisfactoriamente.", "INformación", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generó el archivo plano, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub TargetFileGenerate1810(ByRef dtBank As Data.DataTable)
        Dim dtProcess As Data.DataTable = CreateFormatTable()
        Dim Descri, Opera, Glosa, PosKey1, PosKey2 As String
        Dim DocType As String = "SB"
        Dim Monto As Double
        Dim Fecha As Date
        Dim iColumns As Integer = dtProcess.Columns.Count
        Dim iRow, iPosition As Integer
        dtProcess.Rows.Add()
        dtProcess.Rows(iPosition).Item(0) = "(02) Company Code"
        dtProcess.Rows(iPosition).Item(1) = "(08) Posting Key"
        dtProcess.Rows(iPosition).Item(2) = "(03) Account Number"
        dtProcess.Rows(iPosition).Item(3) = "(10) Amount in Document Currency"
        dtProcess.Rows(iPosition).Item(4) = "(09) Currency Key"
        dtProcess.Rows(iPosition).Item(5) = "(13) Text"
        dtProcess.Rows(iPosition).Item(6) = "(22) Reference Document Number"
        dtProcess.Rows(iPosition).Item(7) = "(28) Value Date"
        dtProcess.Rows(iPosition).Item(8) = "(23) Assignment Number"
        dtProcess.Rows(iPosition).Item(9) = "(06) Posting Date"
        dtProcess.Rows(iPosition).Item(10) = "(07) Document Date"
        dtProcess.Rows(iPosition).Item(11) = "(05) Document Type"
        iRow = 0
        iPosition = 0
        dtBank.Rows.Add()
        Do While dtBank.Rows(iRow).IsNull(0) = False
            Fecha = dtBank.Rows(iRow).Item(1)
            Do While Fecha = dtBank.Rows(iRow).Item(1)
                Descri = dtBank.Rows(iRow).Item(2)
                Opera = dtBank.Rows(iRow).Item(3)
                Monto = dtBank.Rows(iRow).Item(7) - dtBank.Rows(iRow).Item(6)
                Glosa = (iRow + 1).ToString & "-EC " & dtBank.Rows(iRow).Item(2)
                If Monto < 0 Then
                    PosKey1 = "50"
                    PosKey2 = "40"
                Else
                    PosKey1 = "40"
                    PosKey2 = "50"
                End If
                dtProcess.Rows.Add()
                iPosition = dtProcess.Rows.Count - 1
                dtProcess.Rows(iPosition).Item(0) = lueSociedad.EditValue
                dtProcess.Rows(iPosition).Item(1) = PosKey1
                dtProcess.Rows(iPosition).Item(2) = Cuenta1
                dtProcess.Rows(iPosition).Item(3) = Format(Math.Abs(Math.Round(Monto, 2)), "#0.00")
                dtProcess.Rows(iPosition).Item(4) = Moneda
                dtProcess.Rows(iPosition).Item(5) = Glosa
                dtProcess.Rows(iPosition).Item(6) = Format(sePeriodo.EditValue, "00")
                dtProcess.Rows(iPosition).Item(7) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(8) = Opera
                dtProcess.Rows(iPosition).Item(9) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(10) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(11) = DocType
                dtProcess.Rows.Add()
                iPosition = dtProcess.Rows.Count - 1
                dtProcess.Rows(iPosition).Item(0) = lueSociedad.EditValue
                dtProcess.Rows(iPosition).Item(1) = PosKey2
                dtProcess.Rows(iPosition).Item(2) = Cuenta2
                dtProcess.Rows(iPosition).Item(3) = Format(Math.Abs(Math.Round(Monto, 2)), "#0.00")
                dtProcess.Rows(iPosition).Item(4) = Moneda
                dtProcess.Rows(iPosition).Item(5) = Glosa
                dtProcess.Rows(iPosition).Item(6) = Format(sePeriodo.EditValue, "00")
                dtProcess.Rows(iPosition).Item(7) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(8) = Opera
                dtProcess.Rows(iPosition).Item(9) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(10) = Format(Fecha, "dd.MM.yyyy")
                dtProcess.Rows(iPosition).Item(11) = DocType
                iRow = iRow + 1
                If iRow >= dtBank.Rows.Count - 1 Then
                    Exit Do
                End If
            Loop
            dtProcess.Rows.Add()
            iPosition = dtProcess.Rows.Count - 1
            dtProcess.Rows(iPosition).Item(0) = "(99) Save"
        Loop
        If CreateTextDelimiterFile(beArchivoSalida.EditValue, dtProcess, Convert.ToChar(Keys.Tab), False, False) Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El archivo plano ha sido generado satisfactoriamente.", "INformación", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generó el archivo plano, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub lueSociedad_Properties_EditValueChanged(sender As Object, e As EventArgs) Handles lueSociedad.EditValueChanged
        FillAccountBank()
    End Sub

    Private Sub lueCuenta_Properties_EditValueChanged(sender As Object, e As EventArgs) Handles lueCuenta.Properties.EditValueChanged
        Cuenta1 = lueCuenta.GetColumnValue("AccountCode1")
        Cuenta2 = lueCuenta.GetColumnValue("AccountCode2")
        Moneda = lueCuenta.GetColumnValue("BankCurrency")
        Formato = lueCuenta.GetColumnValue("BankCode") & "." & lueCuenta.GetColumnValue("BankFormat")
        If My.Settings.BankTargetDirectory <> "" Then
            beArchivoSalida.EditValue = FolderBrowserDialog1.SelectedPath & "\" & Cuenta1 & ".txt"
        End If
    End Sub

    Private Sub bbiProcesar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesar.ItemClick
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        If vpBank.Validate Then
            ProcessBankStatement()
        End If
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub LoadValidations()
        Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

        containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
        containsValidationRule.ErrorText = "Asigne un valor."
        containsValidationRule.ErrorType = ErrorType.Critical

        Dim customValidationRule As New CustomValidationRule()
        customValidationRule.ErrorText = "Valor obligatorio."
        customValidationRule.ErrorType = ErrorType.Critical

        vpBank.SetValidationRule(Me.lueSociedad, customValidationRule)
        vpBank.SetValidationRule(Me.lueCuenta, customValidationRule)
        vpBank.SetValidationRule(Me.seEjercicio, customValidationRule)
        vpBank.SetValidationRule(Me.seEjercicio, customValidationRule)
        vpBank.SetValidationRule(Me.beArchivoOrigen, customValidationRule)
        vpBank.SetValidationRule(Me.beArchivoSalida, customValidationRule)
    End Sub

    Friend Function CreateSAPFormatTable() As DataTable
        Dim dtProcess As New DataTable
        dtProcess.Columns.Add("CompanyCode").AllowDBNull = True
        dtProcess.Columns.Add("PostingKey").AllowDBNull = True
        dtProcess.Columns.Add("AccountNumber").AllowDBNull = True
        dtProcess.Columns.Add("AmountDocumentCurrency").AllowDBNull = True
        dtProcess.Columns.Add("CurrencyKey").AllowDBNull = True
        dtProcess.Columns.Add("Text").AllowDBNull = True
        dtProcess.Columns.Add("ReferenceDocumentNumber").AllowDBNull = True
        dtProcess.Columns.Add("ValueDate").AllowDBNull = True
        dtProcess.Columns.Add("AssignmentNumber").AllowDBNull = True
        dtProcess.Columns.Add("PostingDate").AllowDBNull = True
        dtProcess.Columns.Add("DocumentDate").AllowDBNull = True
        dtProcess.Columns.Add("DocumentType").AllowDBNull = True
        Return dtProcess
    End Function

End Class