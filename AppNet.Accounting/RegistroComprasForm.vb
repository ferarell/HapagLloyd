Imports System.Threading
Imports System.Drawing
Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraSplashScreen
Imports System.Collections

Public Class RegistroComprasForm
    Dim RUC, SunatFileName1, SunatFileName2, sTipDoc As String
    Dim LibroSunat As String = "RegistroCompras"
    Dim dsLibroSunat As New dsSunat
    Dim dsExcel As New DataSet
    Dim dtTypePaytDoc, dtPaytTerms, dtResult1, dtResult2 As New DataTable
    Dim bFlatFileGenerate As Boolean = True
    Dim bProcess As Boolean = True

    Private Sub RegistroComprasForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        seEjercicio.Value = Today.Year
        sePeriodo.Value = Today.Month
        FillCompany()
        FolderBrowserDialog1.SelectedPath = IIf(My.Settings.LedgerTargetDirectory8 <> "", My.Settings.LedgerTargetDirectory8, "")
        LoadInputValidations()
        LoadPaytTerms()
        LoadTypePaytDoc()
        bbiSunatPle.Enabled = False
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
                dsLibroSunat.Tables(LibroSunat & "1").Rows.Clear()
                dsLibroSunat.Tables(LibroSunat & "2").Rows.Clear()
                ProcessLedger()
                ApplyLayout()
            Catch ex As Exception
                bProcess = False
                SplashScreenManager.CloseForm(False)
            Finally
                SplashScreenManager.CloseForm(False)
            End Try
        Else
            Return
        End If
        gcLibroSunat.DataSource = dsLibroSunat.Tables(LibroSunat & "1")
        PivotGridControl1.DataSource = gcLibroSunat.DataSource
        PivotGridControl1.RefreshData()
        gcLibroSunat.Refresh()
    End Sub

    Private Sub ProcessLedger()
        Dim SourceFile As String = beArchivoOrigen.Text
        dsExcel = LoadExcel(SourceFile, "{0}")
        dsExcel.Tables(0).Select("", "[Document Number],[Tax code]").CopyToDataTable()
        If dsExcel.Tables(0).Rows.Count > 0 Then
            Try
                dtResult1 = dsLibroSunat.Tables(LibroSunat & "1") 'Proveedores Locales
                dtResult2 = dsLibroSunat.Tables(LibroSunat & "2") 'Proveedores del Exterior
                SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
                For r = 0 To dsExcel.Tables(0).Rows.Count - 1
                    SplashScreenManager.Default.SetWaitFormDescription("Procesando Fila " & r.ToString & " de " & (dsExcel.Tables(0).Rows.Count - 1).ToString)
                    Dim oRow As DataRow = dsExcel.Tables(0).Rows(r)
                    If Not IsDBNull(oRow(0)) Then
                        sTipDoc = Microsoft.VisualBasic.Strings.Left(oRow(2), 2)
                        If sTipDoc = "91" Or sTipDoc = "97" Or sTipDoc = "98" Then
                            If bProcess Then
                                NewRowLedger2(oRow, dsExcel.Tables(0))
                            End If
                        Else
                            If bProcess Then
                                NewRowLedger1(oRow, dsExcel.Tables(0))
                            End If
                        End If
                    End If
                Next
                SplashScreenManager.CloseForm(False)
            Catch ex As Exception
                SplashScreenManager.CloseForm(False)
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
        bbiSunatPle.Enabled = bProcess
    End Sub

    Private Sub ApplyLayout()
        For r = 0 To GridView1.Columns.Count - 1
            GridView1.Columns(r).OptionsColumn.ReadOnly = False
            If r <= 41 Then
                GridView1.Columns(r).OptionsColumn.ReadOnly = True
            End If
        Next
    End Sub

    Private Sub SunatFlatFileGenerate()
        If bFlatFileGenerate Then
            beArchivoSalida.EditValue = FolderBrowserDialog1.SelectedPath & "\LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "080100" & "00" & "1" & IIf(dtResult1.Rows.Count = 0, "0", "1") & "11" & ".TXT"
            beArchivoSalida1.EditValue = FolderBrowserDialog1.SelectedPath & "\LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "080200" & "00" & "1" & IIf(dtResult2.Rows.Count = 0, "0", "1") & "11" & ".TXT"
            If CreateTextDelimiterFile(beArchivoSalida.EditValue, dtResult1, "|", False, False) And CreateTextDelimiterFile(beArchivoSalida1.EditValue, dtResult2, "|", False, False) Then
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Los archivos planos han sido generados satisfactoriamente.", "INformación", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generaron los archivos planos, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Se identificaron algunos errores en el proceso, no es posible generar el archivo PLE.  .", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Friend Function DataValidation(column As String, value As String) As String
        Dim sResult As String = ""
        Dim NewValue As String = ""
        If column = "TipDoc" Then
            value = Strings.Right("00" & value.Trim, 2)
            If dtTypePaytDoc.Select("Código = '" & value & "'").Length > 0 Then
                sResult = value
            End If
        End If
        If column = "NroDoc" Then
            value = value.Trim
            For i = 1 To value.Length
                If TextContain(Mid(value, i, 1), "OnlyNumbers") Then
                    NewValue = Mid(value, i, 1)
                Else
                    NewValue = ""
                End If
                sResult = sResult & NewValue
            Next
        End If
        If sResult = "" Then
            bFlatFileGenerate = False
        End If
        Return sResult
    End Function

    Private Sub NewRowLedger1(row As DataRow, dtSource As DataTable)
        Dim iPosition, iNumSer, iNumDoc As Integer
        Dim dtDetracciones As New DataTable
        'For i = 0 To row.ItemArray.Count - 1
        '    If IsDBNull(row(i)) Then
        '        If row.Table.Columns(i).DataType Is System.Type.GetType("System.String") Then
        '            row(i) = ""
        '        End If
        '        If row.Table.Columns(i).DataType Is System.Type.GetType("System.Double") Then
        '            row(i) = 0
        '        End If
        '    End If
        'Next
        Try
            If dtResult1.Select("C2 = " & CInt(row(0)).ToString.Trim).Length = 0 Then
                dtResult1.Rows.Add()
            End If
            iPosition = dtResult1.Rows.Count - 1
            dtResult1.Rows(iPosition).Item("C1") = seEjercicio.Text & Format(Month(row(17)), "00") & "00"
            dtResult1.Rows(iPosition).Item("C2") = row(0).ToString.Trim
            dtResult1.Rows(iPosition).Item("C3") = "M" & row(0).ToString.Trim
            dtResult1.Rows(iPosition).Item("C4") = Format(CDate(row(1)), "dd/MM/yyyy")
            sTipDoc = DataValidation("TipDoc", Strings.Left(row(2).ToString.Trim, 2))
            dtResult1.Rows(iPosition).Item("C5") = IIf(sTipDoc = "14", Format(DateAdd(DateInterval.Day, 15, CDate(row(1))), "dd/MM/yyyy"), "")
            If sTipDoc <> "" Then
                dtResult1.Rows(iPosition).Item("C6") = sTipDoc
                dtResult1.Rows(iPosition).Item("C7") = GetTextFormatValue(sTipDoc, "NroSer", Mid(row(2), 4, Len(row(2)) - 3))
                dtResult1.Rows(iPosition).Item("C9") = GetTextFormatValue(sTipDoc, "NroDoc", Mid(row(2), 4, Len(row(2)) - 3))
                If sTipDoc.Contains({"50", "52"}) Then
                    dtResult1.Rows(iPosition).Item("C8") = Year(row(1)).ToString
                End If
            Else
                dtResult1.Rows(iPosition).Item("C42") = "El tipo de documento es incorrecto, verifique la estructura del número de documento de origen (" & row(2) & ")."
            End If
            dtResult1.Rows(iPosition).Item("C10") = ""
            dtResult1.Rows(iPosition).Item("C11") = IIf(row(3).ToString.Trim.Length = 11, "6", IIf(row(4).ToString.Trim.Length = 8, "1", "0"))
            dtResult1.Rows(iPosition).Item("C12") = IIf(row(3).ToString.Trim = "", GetRucByCia(row(4)), row(3).ToString.Trim)
            dtResult1.Rows(iPosition).Item("C13") = row(4)
            If row(5) <> 0 And row(10).trim = "V5" Then
                dtResult1.Rows(iPosition).Item("C14") = Format(row(5), "###########0.00")
                If lueSociedad.EditValue = "4040" Then
                    If Not IsDBNull(row(6)) Then
                        If row(6) <> 0 Then
                            dtResult1.Rows(iPosition).Item("C20") = Format(row(6), "###########0.00")
                        End If
                    End If
                End If
            End If
            If row(7) <> 0 And row(10).trim = "V5" Then
                dtResult1.Rows(iPosition).Item("C15") = Format(row(7), "###########0.00")
            End If
            dtResult1.Rows(iPosition).Item("C16") = "0"
            dtResult1.Rows(iPosition).Item("C17") = "0"
            dtResult1.Rows(iPosition).Item("C18") = "0"
            dtResult1.Rows(iPosition).Item("C19") = "0"
            If row(10) = "V0" Then
                dtResult1.Rows(iPosition).Item("C20") = Format(row(6), "###########0.00")
            End If
            dtResult1.Rows(iPosition).Item("C21") = "0"
            dtResult1.Rows(iPosition).Item("C22") = "0.00"
            dtResult1.Rows(iPosition).Item("C23") = "0"
            dtResult1.Rows(iPosition).Item("C24") = Format(dtResult1.Rows(iPosition).Item("C14") + dtResult1.Rows(iPosition).Item("C15") + dtResult1.Rows(iPosition).Item("C20"), "###########0.00")
            dtResult1.Rows(iPosition).Item("C25") = RTrim(row(13))
            If row(13).trim = "PEN" Then
                dtResult1.Rows(iPosition).Item("C26") = "1.000"
            Else
                dtResult1.Rows(iPosition).Item("C26") = Format(CDbl(row(14)), "0.000")
            End If
            If sTipDoc <> "" Then
                If dtResult1.Rows(iPosition).Item("C6").ToString.Contains({"07", "08", "87", "88"}) Then
                    Dim aDatos As New ArrayList
                    Dim dtRow As DataRow = Nothing
                    aDatos.Add("NC")
                    aDatos.Add(lueSociedad.EditValue)
                    aDatos.Add(seEjercicio.Text & Format(sePeriodo.EditValue, "00"))
                    aDatos.Add(row(0).ToString.Trim)
                    aDatos.Add(row(2))
                    dtRow = InsertaDatosAsociados(aDatos)
                    If Not dtRow Is Nothing Then
                        If dtRow.ItemArray.Count > 4 And dtRow(4).ToString.Trim <> "" Then
                            dtResult1.Rows(iPosition).Item("C27") = Format(dtRow(7), "dd/MM/yyyy")
                            dtResult1.Rows(iPosition).Item("C28") = dtRow(4)
                            dtResult1.Rows(iPosition).Item("C29") = dtRow(5)
                            dtResult1.Rows(iPosition).Item("C31") = dtRow(6)
                        End If
                    End If
                    If Not IsDBNull(row(19)) And Not IsDBNull(row(20)) And Not IsDBNull(row(21)) And Not IsDBNull(row(22)) Then
                        dtResult1.Rows(iPosition).Item("C27") = Format(CDate(row(19)), "dd/MM/yyyy")
                        dtResult1.Rows(iPosition).Item("C28") = DataValidation("TipDoc", Strings.Left(row(20).ToString.Trim, 2))
                        dtResult1.Rows(iPosition).Item("C29") = GetTextFormatValue(dtResult1.Rows(iPosition).Item("C27"), "NroSer", row(21).ToString.Trim)
                        dtResult1.Rows(iPosition).Item("C31") = GetTextFormatValue(dtResult1.Rows(iPosition).Item("C27"), "NroDoc", row(22).ToString.Trim)
                    End If
                End If
            End If
            If sTipDoc <> "" Then
                dtResult1.Rows(iPosition).Item("C30") = IIf(dtResult1.Rows(iPosition).Item("C6") = "50", "244", "")
                If dtResult1.Rows(iPosition).Item("C6") = "01" Or dtResult1.Rows(iPosition).Item("C6") = "07" Then
                    dtDetracciones = ExecuteAccessQuery("select [Fecha Pago], [Numero Constancia] from ConstanciasDetracciones where [RUC Proveedor] = '" & row(3).ToString.Trim & "' and [Serie de Comprobante] = '" & dtResult1.Rows(iPosition).Item("C7") & "' and [Numero de Comprobante] = '" & dtResult1.Rows(iPosition).Item("C9") & "'").Tables(0)
                    If dtDetracciones.Rows.Count > 0 Then
                        dtResult1.Rows(iPosition).Item("C32") = Format(dtDetracciones.Rows(0).Item(0), "dd/MM/yyyy")
                        dtResult1.Rows(iPosition).Item("C33") = dtDetracciones.Rows(0).Item(1)
                    End If
                End If
            End If
            dtResult1.Rows(iPosition).Item("C34") = "" 'Sujeto a retención = 1
            dtResult1.Rows(iPosition).Item("C35") = "" 'Clasificación de los bienes y servicios adquiridos (Tabla 30) 
            dtResult1.Rows(iPosition).Item("C36") = ""
            dtResult1.Rows(iPosition).Item("C37") = ""
            dtResult1.Rows(iPosition).Item("C38") = ""
            dtResult1.Rows(iPosition).Item("C39") = ""
            dtResult1.Rows(iPosition).Item("C40") = ""
            dtResult1.Rows(iPosition).Item("C41") = ""
            'If IsDBNull(dtResult1.Rows(iPosition).Item("C41")) Then
            dtResult1.Rows(iPosition).Item("C42") = GetStatus("01/" & Format(sePeriodo.EditValue, "00/") & seEjercicio.Text, row(1).ToString, dtSource.Compute("SUM([LC tax amount])", "[Document Number]='" & row(0).ToString & "'").ToString, IIf(row(18).ToString = "", False, True))
            'End If
            If dtResult1.Rows(iPosition).Item("C6").ToString.Contains({"07", "08", "87", "88"}) Then
                If IsDBNull(dtResult1.Rows(iPosition).Item("C27")) Or IsDBNull(dtResult1.Rows(iPosition).Item("C29")) Or IsDBNull(dtResult1.Rows(iPosition).Item("C31")) Then
                    bFlatFileGenerate = False
                    dtResult1.Rows(iPosition).Item("ERR") = "El documento de referencia es obligatorio. "
                End If
            End If
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila: " & iPosition.ToString & " (" & row(2).ToString.Trim & "). " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.Yes Then
                bProcess = False
            End If
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        End Try
    End Sub

    Friend Function GetSerieDoc(NroSerie As String) As String
        Dim sREsult As String = ""
        Try
            If TextContain(Microsoft.VisualBasic.Left(NroSerie, 1), "OnlyNumbers") Then
                sREsult = CInt(NroSerie).ToString
            Else
                sREsult = NroSerie
            End If
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error a generar número de serie. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return sREsult
    End Function

    Private Sub NewRowLedger2(row As DataRow, dtSource As DataTable)
        Dim iPosition As Integer = 0
        'For i = 0 To row.ItemArray.Count - 1
        '    If IsDBNull(row(i)) Then
        '        If row.Table.Columns(i).DataType Is System.Type.GetType("System.String") Then
        '            row(i) = ""
        '        End If
        '        If row.Table.Columns(i).DataType Is System.Type.GetType("System.Double") Then
        '            row(i) = 0
        '        End If
        '    End If
        'Next
        Try
            dtResult2.Rows.Add()
            iPosition = dtResult2.Rows.Count - 1
            dtResult2.Rows(iPosition).Item("C1") = seEjercicio.Text & Format(sePeriodo.EditValue, "00") & "00"
            dtResult2.Rows(iPosition).Item("C2") = row(0).ToString
            dtResult2.Rows(iPosition).Item("C3") = "M" & row(0).ToString
            dtResult2.Rows(iPosition).Item("C4") = Format(CDate(row(1)), "dd/MM/yyyy")
            dtResult2.Rows(iPosition).Item("C5") = DataValidation("TipDoc", Microsoft.VisualBasic.Left(row(2).trim, 2))
            dtResult2.Rows(iPosition).Item("C6") = ""
            dtResult2.Rows(iPosition).Item("C7") = DataValidation("NroDoc", Mid(row(2), 3, Len(row(2)) - 2))
            dtResult2.Rows(iPosition).Item("C8") = "0.00"
            dtResult2.Rows(iPosition).Item("C9") = "0.00"
            dtResult2.Rows(iPosition).Item("C10") = Format(row(8), "0.00")
            dtResult2.Rows(iPosition).Item("C11") = ""
            dtResult2.Rows(iPosition).Item("C12") = ""
            dtResult2.Rows(iPosition).Item("C13") = ""
            dtResult2.Rows(iPosition).Item("C14") = ""
            dtResult2.Rows(iPosition).Item("C15") = "0.00"
            dtResult2.Rows(iPosition).Item("C16") = row(13)
            If row(13) = "PEN" Then
                dtResult2.Rows(iPosition).Item("C17") = "1.000"
            Else
                dtResult2.Rows(iPosition).Item("C17") = Format(CDbl(row(14)), "0.000")
            End If
            dtResult2.Rows(iPosition).Item("C18") = "9011"
            dtResult2.Rows(iPosition).Item("C19") = row(4)
            dtResult2.Rows(iPosition).Item("C20") = ""
            dtResult2.Rows(iPosition).Item("C21") = IIf(row(3).ToString.Trim = "", "-", Replace(row(3).ToString.Trim, "RUC", ""))
            dtResult2.Rows(iPosition).Item("C22") = ""
            dtResult2.Rows(iPosition).Item("C23") = ""
            dtResult2.Rows(iPosition).Item("C24") = ""
            dtResult2.Rows(iPosition).Item("C25") = ""
            dtResult2.Rows(iPosition).Item("C26") = "0.00"
            dtResult2.Rows(iPosition).Item("C27") = "0.00"
            dtResult2.Rows(iPosition).Item("C28") = "0.00"
            dtResult2.Rows(iPosition).Item("C29") = "0.00"
            dtResult2.Rows(iPosition).Item("C30") = "0.00"
            dtResult2.Rows(iPosition).Item("C31") = "09"
            dtResult2.Rows(iPosition).Item("C32") = ""
            dtResult2.Rows(iPosition).Item("C33") = "" 'Tipo de Renta (Consultar)
            dtResult2.Rows(iPosition).Item("C34") = ""
            dtResult2.Rows(iPosition).Item("C35") = ""
            dtResult2.Rows(iPosition).Item("C36") = GetStatus("01/" & Format(sePeriodo.EditValue, "00/") & seEjercicio.Text, row(1), dtSource.Compute("SUM([LC tax amount])", "[Document Number]='" & row(0).ToString & "'").ToString, IIf(row(18).ToString = "", False, True))
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Fila " & iPosition.ToString & ". " & ex.Message & ". Desea cancelar el proceso?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.OK Then
                bProcess = False
            End If
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        End Try
    End Sub

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
            'If IsReversed Then
            '    status = "9"
            'End If
        End If
        Return status
    End Function

    Friend Function GetRucByCia(CiaName As String) As String
        If IsDBNull(CiaName) Then
            Return ""
        End If
        Dim CiaRUC As String = ""
        CiaName = Replace(CiaName, "'", "")
        Try
            If Not IsDBNull(dsExcel.Tables(0).Select("[Name 1] = '" & CiaName & "'")(0).ItemArray(3)) Then
                CiaRUC = dsExcel.Tables(0).Select("[Name 1] = '" & CiaName & "'")(0).ItemArray(3)
            End If
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error al obtener ruc por razón social. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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
        OpenFileDialog1.InitialDirectory = IIf(My.Settings.LedgerSourceDirectory8 <> "", My.Settings.LedgerSourceDirectory8, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beArchivoOrigen.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
        ExportarExcel(gcLibroSunat)
    End Sub

    Private Sub BarButtonItem3_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem3.ItemClick, BarButtonItem4.ItemClick
        If e.Item.Tag = "1" Then
            PivotGridControl1.Fields.Item(1).Caption = "Base Imponible"
            PivotGridControl1.Fields.Item(1).FieldName = "C14"
            PivotGridControl1.Fields.Item(2).FieldName = "C15"
            PivotGridControl1.Fields.Item(3).FieldName = "C20"
            PivotGridControl1.Fields.Item(4).FieldName = "C23"
            PivotGridControl1.Fields.Item(2).Visible = True
            PivotGridControl1.Fields.Item(3).Visible = True
            PivotGridControl1.Fields.Item(4).Visible = True
        Else
            PivotGridControl1.Fields.Item(1).Caption = "Total Adquisiciones"
            PivotGridControl1.Fields.Item(1).FieldName = "C10"
            PivotGridControl1.Fields.Item(2).Visible = False
            PivotGridControl1.Fields.Item(3).Visible = False
            PivotGridControl1.Fields.Item(4).Visible = False
        End If
        gcLibroSunat.DataSource = dsLibroSunat.Tables(LibroSunat & e.Item.Tag)
        gcLibroSunat.MainView.PopulateColumns()
        PivotGridControl1.DataSource = gcLibroSunat.DataSource
        PivotGridControl1.RefreshData()
    End Sub

    Private Sub lueSociedad_EditValueChanged(sender As Object, e As EventArgs) Handles lueSociedad.EditValueChanged, seEjercicio.EditValueChanged, sePeriodo.EditValueChanged
        If lueSociedad.EditValue <> "" Then
            RUC = lueSociedad.GetColumnValue("CompanyTaxCode")
            SunatFileName1 = "LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "080100" & "00" & "1111" & ".TXT"
            SunatFileName2 = "LE" & RUC & seEjercicio.Text & Format(sePeriodo.Value, "00") & "00" & "080200" & "00" & "1111" & ".TXT"
            If My.Settings.LedgerTargetDirectory8 <> "" Then
                beArchivoSalida.EditValue = FolderBrowserDialog1.SelectedPath & "\" & SunatFileName1
                beArchivoSalida1.EditValue = FolderBrowserDialog1.SelectedPath & "\" & SunatFileName2
            End If
        End If
    End Sub

    Private Sub beArchivoSalida_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoSalida.Properties.ButtonClick
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            beArchivoSalida.EditValue = FolderBrowserDialog1.SelectedPath & "\" & SunatFileName1
            beArchivoSalida1.EditValue = FolderBrowserDialog1.SelectedPath & "\" & SunatFileName2
        End If
    End Sub

    Private Sub GridView1_RowCellStyle(ByVal sender As Object, ByVal e As RowCellStyleEventArgs) Handles GridView1.RowCellStyle
        Dim View As GridView = sender
        If (e.RowHandle >= 0) Then
            'Validaciones Nacionales
            If Microsoft.VisualBasic.Right(gcLibroSunat.DataSource.ToString, 1) = "1" Then
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
                If e.Column.FieldName = "C8" Then 'Año de la DUA
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "50" And ((View.GetRowCellDisplayText(e.RowHandle, View.Columns("C8")) = "") Or (View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) < "1981" Or View.GetRowCellDisplayText(e.RowHandle, View.Columns("C8")) > Year(Now).ToString)) Then
                        e.Appearance.BackColor = Color.Green
                        e.Appearance.BackColor2 = Color.LightGreen
                        bFlatFileGenerate = False
                    End If
                End If
                If e.Column.FieldName = "C9" Then 'Número Comprobante de Pago
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C9")) = "" Then
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
                If e.Column.FieldName = "C11" Then 'Tipo Documento de Identidad
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C11")) = "" Then
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
                If e.Column.FieldName = "C12" Then 'Número Documento de Identidad
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C12")) = "" Then
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
                If e.Column.FieldName = "C26" Then
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Or View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "08" Then
                        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C26")) = "" Then 'Fecha Comprobante de Pago que se modifica (NC)
                            e.Appearance.BackColor = Color.Salmon
                            e.Appearance.BackColor2 = Color.SeaShell
                            bFlatFileGenerate = False
                        End If
                    End If
                End If
                If e.Column.FieldName = "C27" Then
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Or View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "08" Then
                        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C27")) = "" Then 'Tipo Comprobante de Pago que se modifica (NC)
                            e.Appearance.BackColor = Color.Salmon
                            e.Appearance.BackColor2 = Color.SeaShell
                            bFlatFileGenerate = False
                        End If
                    End If
                End If
                If e.Column.FieldName = "C28" Then
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Or View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "08" Then
                        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C28")) = "" Then 'Serie Comprobante de Pago que se modifica (NC)
                            e.Appearance.BackColor = Color.Salmon
                            e.Appearance.BackColor2 = Color.SeaShell
                            bFlatFileGenerate = False
                        End If
                    End If
                End If
                If e.Column.FieldName = "C30" Then
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "07" Or View.GetRowCellDisplayText(e.RowHandle, View.Columns("C6")) = "08" Then
                        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C30")) = "" Then 'Número Comprobante de Pago que se modifica (NC)
                            e.Appearance.BackColor = Color.Salmon
                            e.Appearance.BackColor2 = Color.SeaShell
                            bFlatFileGenerate = False
                        End If
                    End If
                End If
                If e.Column.FieldName = "C27" Then
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C27")) = "50" Then 'Tipo Comprobante de Pago que se modifica (NC)
                        If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C29")) = "" Then
                            e.Appearance.BackColor = Color.Salmon
                            e.Appearance.BackColor2 = Color.SeaShell
                            bFlatFileGenerate = False
                        End If
                    End If
                End If

                If e.Column.FieldName = "C41" Then 'Estado
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C41")) = "" Then
                        e.Appearance.BackColor = Color.Peru
                        e.Appearance.BackColor2 = Color.LightYellow
                        bFlatFileGenerate = False
                    End If
                End If
            Else
                'Validaciones No Domiciliados
                If e.Column.FieldName = "C5" Then 'Tipo Comprobante de Pago
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C5")) = "" Then
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
                If e.Column.FieldName = "C7" Then 'Número Comprobante de Pago
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C7")) = "" Then
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
                If e.Column.FieldName = "C19" Then 'Razón Social del No Domiciliado
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C19")) = "" Then
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
                If e.Column.FieldName = "C21" Then 'Número Identificación del No Domiciliado
                    If View.GetRowCellDisplayText(e.RowHandle, View.Columns("C21")) = "" Then
                        e.Appearance.BackColor = Color.Salmon
                        e.Appearance.BackColor2 = Color.SeaShell
                        bFlatFileGenerate = False
                    End If
                End If
            End If
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

    Public Function AllContains(ByVal str As String, ByVal ParamArray values As String()) As Boolean
        For Each value In values
            If str.Contains(value) Then
                Return True
            End If
        Next
        Return False
    End Function

    'Friend Function GetTextFormatValue(DocType As String, Group As String, Value As String) As String
    '    Dim sResult As String = ""
    '    Dim iPositions As Integer = GetPositionsByDocType(DocType, Group)
    '    Try
    '        If Group = "NroSer" Then
    '            If DocType = "05" Then
    '                Return "3"
    '            ElseIf DocType = "10" Then
    '                Return "1683"
    '            ElseIf DocType = "22" Then
    '                Return "0820"
    '            Else
    '                If InStr(Value, "-") > 0 Then
    '                    Value = Strings.Left(Value, InStr(Value, "-") - IIf(Value.Contains("-"), 1, 0))
    '                    sResult = Strings.Right(StrDup(iPositions, "0") & Value.ToString.Trim, iPositions)
    '                Else
    '                    Value = ""
    '                End If
    '            End If
    '        ElseIf Group = "NroDoc" Then
    '            If InStr(Value, "-") > 0 Then
    '                Value = Mid(Value, InStr(Value, "-") + 1, iPositions)
    '            End If
    '        End If
    '    Catch ex As Exception
    '        DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error en la función GetTextFormatValue. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    End Try
    '    sResult = Strings.Right(StrDup(iPositions, "0") & Value.ToString.Trim, iPositions)
    '    Return sResult
    'End Function

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
                    End If
                    sResult = Strings.Right(StrDup(iPositions, "0") & Value.ToString.Trim, iPositions)
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
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Error en la función GetPositionsByDocType. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return iResult
    End Function

    Private Sub bbiSunatPle_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSunatPle.ItemClick
        SunatFlatFileGenerate()
    End Sub

    Private Sub GridView1_CellValueChanging(sender As Object, e As DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs) Handles GridView1.CellValueChanging
        Dim iPos As Integer = GridView1.GetFocusedDataSourceRowIndex

        If e.Column.Name = "colC43" Then
            If e.Value = True Then
                DevExpress.XtraEditors.XtraMessageBox.Show("No es posible retornar al estado anterior, debe procesar nuevamente. ", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            dtResult1.Rows(iPos).Item("C5") = ""
            dtResult1.Rows(iPos).Item("C8") = ""
            dtResult1.Rows(iPos).Item("C10") = ""
            dtResult1.Rows(iPos).Item("C12") = RUC
            dtResult1.Rows(iPos).Item("C13") = "Comprobante eliminado"
            For r = 14 To 22
                dtResult1.Rows(iPos).Item("C" & r.ToString) = DBNull.Value
            Next
            dtResult1.Rows(iPos).Item("C23") = "0.00"
            For r = 24 To 40
                dtResult1.Rows(iPos).Item("C" & r.ToString) = ""
            Next
            dtResult1.Rows(iPos).Item("C41") = "9"
        End If
        If e.Column.Name = "colC44" Then
            If e.Value = True Then
                dtResult1.Rows(iPos).Item("C41") = "9"
            Else
                dtResult1.Rows(iPos).Item("C41") = GetStatus("01/" & Mid(GridView1.GetRowCellValue(iPos, "C1"), 5, 2) & "/" & seEjercicio.Text, GridView1.GetRowCellValue(iPos, "C4").ToString, GridView1.GetRowCellValue(iPos, "C15").ToString, False)
            End If
        End If
        GridView1.RefreshRow(GridView1.GetFocusedDataSourceRowIndex)
    End Sub

End Class