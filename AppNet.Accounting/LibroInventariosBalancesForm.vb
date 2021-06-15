Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.Collections

Public Class LibroInventariosBalancesForm
    Dim SunatFileName As String
    Dim LibroSunat As String = "" 'LibroInventariosBalances
    Dim dsLibroSunat As New dsSunat
    'Dim dsExcel As New DataSet
    'Dim dtResult, dtProcess, dtTypePaytDoc, dtPaytTerms As New DataTable
    Dim dtResult As New DataTable
    Dim bFlatFileGenerate As Boolean = True
    Dim bProcess As Boolean = True

    Private Sub LibroInventariosBalancesForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        seEjercicio.Value = Today.Year - 1
        sePeriodo.Value = 12
        FillCompany()
        FillLedgerList()
        FolderBrowserDialog1.SelectedPath = IIf(My.Settings.LedgerTargetDirectory3 <> "", My.Settings.LedgerTargetDirectory3, "")

    End Sub

    Private Sub beArchivoSalida_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoSalida.Properties.ButtonClick
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            beArchivoSalida.EditValue = GetSunatFileName(FolderBrowserDialog1.SelectedPath)
        End If
    End Sub

    Private Sub FillCompany()
        lueSociedad.Properties.DataSource = FillDataTable("Company", "", "ACC")
        lueSociedad.Properties.DisplayMember = "CompanyDescription"
        lueSociedad.Properties.ValueMember = "CompanyCode"
    End Sub

    Private Sub FillLedgerList()
        lueReport.Properties.DataSource = FillDataTable("LibrosRegistrosSunatQry", "CodigoLibro=3", "ACC")
        lueReport.Properties.DisplayMember = "NombreLibro"
        lueReport.Properties.ValueMember = "CodigoEstructura"
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

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiProcesar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesar.ItemClick
        LoadInputValidations()
        If Not vpLedger.Validate Then
            Return
        End If
        bFlatFileGenerate = True
        bProcess = True
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            dsLibroSunat.Tables(LibroSunat).Rows.Clear()
            ProcessLedger()
        Catch ex As Exception
            bProcess = False
            SplashScreenManager.CloseForm(False)
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
        GridView1.PopulateColumns()
        gcLibroSunat.DataSource = dtResult 'dsLibroSunat.Tables(LibroSunat)
        'gcLibroSunat.RefreshDataSource()
    End Sub

    Private Sub lueSociedad_EditValueChanged(sender As Object, e As EventArgs) Handles lueSociedad.EditValueChanged, seEjercicio.EditValueChanged, sePeriodo.EditValueChanged, lueReport.EditValueChanged
        bbiProcesar.Enabled = True
        If lueReport.GetColumnValue("IndicadorContenido") = 0 Then
            bbiProcesar.Enabled = False
        End If
        If lueSociedad.EditValue <> "" Then
            GetSunatFileName(My.Settings.LedgerTargetDirectory3)
        End If
        LibroSunat = "InventariosBalances" & Replace(lueReport.GetColumnValue("SubLibro"), ".", "")
    End Sub

    Function GetSunatFileName(sPath As String) As String
        Dim sFileName As String = ""
        If sPath = "" Then
            If FolderBrowserDialog1.SelectedPath <> "" Then
                sPath = FolderBrowserDialog1.SelectedPath
            Else
                sPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            End If
        End If
        'LE2049218508720181231031601011111 -- Validado
        'LE20492185087201812310301011011.TXT
        sFileName = sPath & "\LE" & lueSociedad.GetColumnValue("CompanyTaxCode") & seEjercicio.Text & Format(sePeriodo.Value, "00") & "310" & Replace(lueReport.GetColumnValue("CodigoEstructura"), ".", "") & "011" & lueReport.GetColumnValue("IndicadorContenido") & "11." & lueReport.GetColumnValue("TipoArchivo")
        beArchivoSalida.EditValue = sFileName
        Return sFileName
    End Function

    Private Sub ProcessLedger()
        Validate()
        Dim SourceFile As String = beArchivoOrigen.Text
        Dim dtSource As New DataTable
        dtSource = LoadExcel(SourceFile, "{0}").Tables(0)
        If dtSource.Rows.Count > 0 Then
            Try
                dtResult = dsLibroSunat.Tables(LibroSunat)
                For Each row As DataRow In dtSource.Rows
                    If bProcess Then
                        If Not IsDBNull(row(0)) Then
                            Select Case lueReport.GetColumnValue("CodigoEstructura")
                                Case "3.01.00"
                                    NewRowLedger1(row)
                                Case "3.02.00"
                                    NewRowLedger2(row)
                                Case "3.03.00"
                                    NewRowLedger3(row)
                                Case "3.04.00"
                                    NewRowLedger4(row)
                                Case "3.05.00"
                                    NewRowLedger5(row)
                                    'Case "3.6"
                                    '    NewRowLedger6(row)
                                    'Case "3.7"
                                    '    NewRowLedger7(row)
                                    'Case "3.8"
                                    '    NewRowLedger8(row)
                                    'Case "3.9"
                                    '    NewRowLedger9(row)
                                Case "3.11.00"
                                    NewRowLedger11(row)
                                Case "3.12.00"
                                    NewRowLedger12(row)
                                Case "3.13.00"
                                    NewRowLedger13(row)
                                Case "3.14.00"
                                    NewRowLedger14(row)
                                Case "3.15.00"
                                    NewRowLedger15(row)
                                Case "3.16.01"
                                    NewRowLedger161(row)
                                Case "3.16.02"
                                    NewRowLedger162(row)
                                Case "3.17.00"
                                    NewRowLedger17(row)
                                    'Case "3.18.00"
                                    '    NewRowLedger18(row)
                                Case "3.19.00"
                                    NewRowLedger19(row)
                                Case "3.20.00"
                                    NewRowLedger20(row)
                                    'Case "3.23"
                                    '    NewRowLedger23(row)
                                    'Case "3.24"
                                    '    NewRowLedger24(row)
                                Case "3.25.00"
                                    NewRowLedger25(row)
                            End Select
                        End If
                    End If
                Next
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Function SetDefaultValues(row As DataRow) As DataRow
        For i = 0 To row.ItemArray.Count - 1
            If IsDBNull(row(i)) Then
                If row.Table.Columns(i).DataType Is System.Type.GetType("System.String") Then
                    row(i) = ""
                ElseIf row.Table.Columns(i).DataType Is System.Type.GetType("System.Double") Or row.Table.Columns(i).DataType Is System.Type.GetType("System.Decimal") Then
                    row(i) = 0
                End If
            End If
        Next
        row.AcceptChanges()
        Return row
    End Function

    Friend Sub NewRowLedger1(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = "09"
            dtResult.Rows(iPosition).Item("C3") = row(2)
            dtResult.Rows(iPosition).Item("C4") = Format(row(3), "########0.00")
            dtResult.Rows(iPosition).Item("C5") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger2(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = row(2)
            dtResult.Rows(iPosition).Item("C4") = row(3)
            dtResult.Rows(iPosition).Item("C5") = row(4)
            dtResult.Rows(iPosition).Item("C6") = Format(row(5), "#########0.00")
            dtResult.Rows(iPosition).Item("C7") = Format(row(6), "#########0.00")
            dtResult.Rows(iPosition).Item("C8") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger3(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = "M" & Format(CInt(row(2)), "000")
            dtResult.Rows(iPosition).Item("C4") = row(3)
            dtResult.Rows(iPosition).Item("C5") = row(4)
            dtResult.Rows(iPosition).Item("C6") = row(5)
            dtResult.Rows(iPosition).Item("C7") = Format(row(6), "dd/MM/yyyy")
            dtResult.Rows(iPosition).Item("C8") = Format(row(7), "#########0.00")
            dtResult.Rows(iPosition).Item("C9") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger4(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = row(2)
            dtResult.Rows(iPosition).Item("C4") = row(3)
            dtResult.Rows(iPosition).Item("C5") = row(4)
            dtResult.Rows(iPosition).Item("C6") = row(5)
            dtResult.Rows(iPosition).Item("C7") = row(6)
            dtResult.Rows(iPosition).Item("C8") = row(7)
            dtResult.Rows(iPosition).Item("C9") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger5(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = "M" & Format(CInt(row(2)), "000")
            dtResult.Rows(iPosition).Item("C4") = row(3)
            dtResult.Rows(iPosition).Item("C5") = row(4)
            dtResult.Rows(iPosition).Item("C6") = row(5)
            dtResult.Rows(iPosition).Item("C7") = Format(row(6), "dd/MM/yyyy")
            dtResult.Rows(iPosition).Item("C8") = Format(row(7), "#########0.00")
            dtResult.Rows(iPosition).Item("C9") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger11(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = "M" & Format(CInt(row(2)), "000")
            dtResult.Rows(iPosition).Item("C4") = row(3)
            dtResult.Rows(iPosition).Item("C5") = row(4)
            dtResult.Rows(iPosition).Item("C6") = row(5)
            dtResult.Rows(iPosition).Item("C7") = row(6)
            dtResult.Rows(iPosition).Item("C8") = row(7)
            dtResult.Rows(iPosition).Item("C9") = Format(row(8), "#########0.00")
            dtResult.Rows(iPosition).Item("C10") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger12(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = "M" & Format(CInt(row(2)), "000")
            dtResult.Rows(iPosition).Item("C4") = row(3)
            dtResult.Rows(iPosition).Item("C5") = row(4)
            dtResult.Rows(iPosition).Item("C6") = Format(row(5), "dd/MM/yyyy")
            dtResult.Rows(iPosition).Item("C7") = row(6)
            dtResult.Rows(iPosition).Item("C8") = Format(row(7), "#########0.00")
            dtResult.Rows(iPosition).Item("C9") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger13(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = "M" & Format(CInt(row(2)), "000")
            dtResult.Rows(iPosition).Item("C4") = row(3)
            dtResult.Rows(iPosition).Item("C5") = row(4)
            dtResult.Rows(iPosition).Item("C6") = Format(row(5), "dd/MM/yyyy")
            dtResult.Rows(iPosition).Item("C7") = row(6)
            dtResult.Rows(iPosition).Item("C8") = row(7)
            dtResult.Rows(iPosition).Item("C9") = Format(row(8), "#########0.00")
            dtResult.Rows(iPosition).Item("C10") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger14(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = row(2)
            dtResult.Rows(iPosition).Item("C4") = row(3)
            dtResult.Rows(iPosition).Item("C5") = row(4)
            dtResult.Rows(iPosition).Item("C6") = row(5)
            dtResult.Rows(iPosition).Item("C7") = row(6)
            dtResult.Rows(iPosition).Item("C8") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger15(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = row(2)
            dtResult.Rows(iPosition).Item("C4") = row(3)
            dtResult.Rows(iPosition).Item("C5") = row(4)
            dtResult.Rows(iPosition).Item("C6") = row(5)
            dtResult.Rows(iPosition).Item("C7") = row(6)
            dtResult.Rows(iPosition).Item("C8") = row(7)
            dtResult.Rows(iPosition).Item("C9") = row(8)
            dtResult.Rows(iPosition).Item("C10") = row(9)
            dtResult.Rows(iPosition).Item("C11") = row(10)
            dtResult.Rows(iPosition).Item("C12") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger161(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = row(2)
            dtResult.Rows(iPosition).Item("C4") = row(3)
            dtResult.Rows(iPosition).Item("C5") = row(4)
            dtResult.Rows(iPosition).Item("C6") = row(5)
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger162(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = row(2)
            dtResult.Rows(iPosition).Item("C4") = row(3)
            dtResult.Rows(iPosition).Item("C5") = row(4)
            dtResult.Rows(iPosition).Item("C6") = row(5)
            dtResult.Rows(iPosition).Item("C7") = Format(row(6), "000.00000000")
            dtResult.Rows(iPosition).Item("C8") = row(7)
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger17(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = Format(row(2), "#########0.00")
            dtResult.Rows(iPosition).Item("C4") = Format(row(3), "#########0.00")
            dtResult.Rows(iPosition).Item("C5") = Format(row(4), "#########0.00")
            dtResult.Rows(iPosition).Item("C6") = Format(row(5), "#########0.00")
            dtResult.Rows(iPosition).Item("C7") = Format(row(6), "#########0.00")
            dtResult.Rows(iPosition).Item("C8") = Format(row(7), "#########0.00")
            dtResult.Rows(iPosition).Item("C9") = Format(row(8), "#########0.00")
            dtResult.Rows(iPosition).Item("C10") = Format(row(9), "#########0.00")
            dtResult.Rows(iPosition).Item("C11") = Format(row(10), "#########0.00")
            dtResult.Rows(iPosition).Item("C12") = Format(row(11), "#########0.00")
            dtResult.Rows(iPosition).Item("C13") = Format(row(12), "#########0.00")
            dtResult.Rows(iPosition).Item("C14") = Format(row(13), "#########0.00")
            dtResult.Rows(iPosition).Item("C15") = Format(row(14), "#########0.00")
            dtResult.Rows(iPosition).Item("C16") = Format(row(15), "#########0.00")
            dtResult.Rows(iPosition).Item("C17") = Format(row(16), "#########0.00")
            dtResult.Rows(iPosition).Item("C18") = Format(row(17), "#########0.00")
            dtResult.Rows(iPosition).Item("C19") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger18(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = row(2)
            dtResult.Rows(iPosition).Item("C4") = row(3)
            dtResult.Rows(iPosition).Item("C5") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger19(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = row(2)
            dtResult.Rows(iPosition).Item("C4") = Format(row(3), "#########0.00")
            dtResult.Rows(iPosition).Item("C5") = Format(row(4), "#########0.00")
            dtResult.Rows(iPosition).Item("C6") = Format(row(5), "#########0.00")
            dtResult.Rows(iPosition).Item("C7") = Format(row(6), "#########0.00")
            dtResult.Rows(iPosition).Item("C8") = Format(row(7), "#########0.00")
            dtResult.Rows(iPosition).Item("C9") = Format(row(8), "#########0.00")
            dtResult.Rows(iPosition).Item("C10") = Format(row(9), "#########0.00")
            dtResult.Rows(iPosition).Item("C11") = Format(row(10), "#########0.00")
            dtResult.Rows(iPosition).Item("C12") = Format(row(11), "#########0.00")
            dtResult.Rows(iPosition).Item("C13") = Format(row(12), "#########0.00")
            dtResult.Rows(iPosition).Item("C14") = Format(row(13), "#########0.00")
            dtResult.Rows(iPosition).Item("C15") = Format(row(14), "#########0.00")
            dtResult.Rows(iPosition).Item("C16") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger20(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = row(2)
            dtResult.Rows(iPosition).Item("C4") = Format(row(3), "#########0.00")
            dtResult.Rows(iPosition).Item("C5") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Friend Sub NewRowLedger25(row As DataRow)
        Dim iPosition As Integer = 0
        Dim sTipDoc As String = ""
        row = SetDefaultValues(row)
        Try
            dtResult.Rows.Add()
            iPosition = dtResult.Rows.Count - 1
            dtResult.Rows(iPosition).Item("C1") = row(0)
            dtResult.Rows(iPosition).Item("C2") = row(1)
            dtResult.Rows(iPosition).Item("C3") = row(2)
            dtResult.Rows(iPosition).Item("C4") = Format(row(3), "#########0.00")
            dtResult.Rows(iPosition).Item("C5") = "1"
        Catch ex As Exception
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
        End Try
    End Sub

    Private Sub SunatFlatFileGenerate()
        GetSunatFileName(FolderBrowserDialog1.SelectedPath)
        If lueReport.GetColumnValue("TipoArchivo") = "PDF" Then
            IO.File.Copy(beArchivoOrigen.Text, beArchivoSalida.Text, True)
            Return
        End If
        If lueReport.GetColumnValue("IndicadorContenido") = 0 Then
            If CreateEmptyTextFile(beArchivoSalida.Text) Then
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El archivo plano ha sido generado satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
            Return
        End If
        If bFlatFileGenerate Then
            'beArchivoSalida.EditValue = FolderBrowserDialog1.SelectedPath & "\LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "140100" & "00" & "1" & IIf(dtResult.Rows.Count = 0, "0", "1") & "11" & ".TXT"
            If CreateTextDelimiterFile(beArchivoSalida.EditValue, dsLibroSunat.Tables(LibroSunat), "|", False, False) Then
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El archivo plano ha sido generado satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generó el archivo plano, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Se identificaron algunos errores en el proceso, no es posible generar el archivo PLE.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Function CreateEmptyTextFile(sFile As String) As Boolean
        Dim sw As System.IO.StreamWriter
        sw = New IO.StreamWriter(beArchivoSalida.Text, False, System.Text.Encoding.Default)
        sw.Close()
        Return IO.File.Exists(beArchivoSalida.Text)
    End Function

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoOrigen.Properties.ButtonClick
        OpenFileDialog1.Filter = "Excel Files (*.xls*)|*.xls*|PDF Files (*.pdf)|*.pdf"
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
            '    If e.Column.FieldName = "C1" Then 'Periodo
            '        Dim C1 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("C1"))
            '        If Microsoft.VisualBasic.Strings.Left(C1, 6) <> seEjercicio.EditValue & Format(sePeriodo.EditValue, "00") Then
            '            e.Appearance.BackColor = Color.DeepSkyBlue
            '            e.Appearance.BackColor2 = Color.LightCyan
            '            bFlatFileGenerate = False
            '        End If
            '    End If
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
            '    If e.Column.FieldName = "C8" Then 'Número Comprobante de Pago
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C8")) = "" Then
            '            e.Appearance.BackColor = Color.Salmon
            '            e.Appearance.BackColor2 = Color.SeaShell
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            '    If e.Column.FieldName = "C10" Then 'Tipo Documento de Identidad
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C10")) = "" Then
            '            e.Appearance.BackColor = Color.Salmon
            '            e.Appearance.BackColor2 = Color.SeaShell
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            '    If e.Column.FieldName = "C11" Then 'Número Documento de Identidad
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C11")) = "" Then
            '            e.Appearance.BackColor = Color.Salmon
            '            e.Appearance.BackColor2 = Color.SeaShell
            '            bFlatFileGenerate = False
            '        End If
            '    End If
            '    If e.Column.FieldName = "C27" Then
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
            '            If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C27")) = "" And Not View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")).Contains("ANULAD") Then 'Fecha Comprobante de Pago que se modifica (NC)
            '                e.Appearance.BackColor = Color.Salmon
            '                e.Appearance.BackColor2 = Color.SeaShell
            '                bFlatFileGenerate = False
            '            End If
            '        End If
            '    End If
            '    If e.Column.FieldName = "C28" Then
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
            '            If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C28")) = "" And Not View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")).Contains("ANULAD") Then 'Tipo Comprobante de Pago que se modifica (NC)
            '                e.Appearance.BackColor = Color.Salmon
            '                e.Appearance.BackColor2 = Color.SeaShell
            '                bFlatFileGenerate = False
            '            End If
            '        End If
            '    End If
            '    If e.Column.FieldName = "C29" Then
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
            '            If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C29")) = "" And Not View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")).Contains("ANULAD") Then 'Serie Comprobante de Pago que se modifica (NC)
            '                e.Appearance.BackColor = Color.Salmon
            '                e.Appearance.BackColor2 = Color.SeaShell
            '                bFlatFileGenerate = False
            '            End If
            '        End If
            '    End If
            '    If e.Column.FieldName = "C30" Then
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Then
            '            If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C30")) = "" And Not View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")).Contains("ANULAD") Then 'Número Comprobante de Pago que se modifica (NC)
            '                e.Appearance.BackColor = Color.Salmon
            '                e.Appearance.BackColor2 = Color.SeaShell
            '                bFlatFileGenerate = False
            '            End If
            '        End If
            '    End If
            '    If e.Column.FieldName = "C34" Then 'Estado
            '        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C34")) = "" Then
            '            e.Appearance.BackColor = Color.Peru
            '            e.Appearance.BackColor2 = Color.LightYellow
            '            bFlatFileGenerate = False
            '        End If
            '    End If
        End If
    End Sub

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

    Private Sub bbiSunatPle_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSunatPle.ItemClick
        SunatFlatFileGenerate()
    End Sub

End Class