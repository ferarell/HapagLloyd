Imports DevExpress.XtraEditors.DXErrorProvider
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.ReportSource
Imports CrystalDecisions.Shared
Imports CrystalDecisions.Windows.Forms
Imports DevExpress.XtraSplashScreen
Imports System.Threading

Public Class RetencionesForm

    Dim dtTypePaytDoc, dtPaytTerms, dtResult1, dtResult2 As New DataTable
    Dim dtResult, dtListaCR, dtFlatFile, dtEventos As New DataTable
    Dim dsMain As New dsSunat

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoOrigen.Properties.ButtonClick
        OpenFileDialog1.Filter = "Excel Files (*.xls*)|*.xls*"
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.InitialDirectory = My.Settings.RetenSourceDirectory
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beArchivoOrigen.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub beArchivoSalida_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoSalida.Properties.ButtonClick

    End Sub

    Private Sub bbiImprimir_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiImprimir.ItemClick
        If GridView2.RowCount = 0 Then
            Return
        End If
        If CRSeleccionados = 0 Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Debe seleccionar al menos 1 comprobante de retención.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        'Dim Report As New ComprobanteRetencion
        Dim CRSerie, CRNumero, sFileName As String
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            Dim i As Integer = 0
            Do While i < GridView2.RowCount
                Dim row As DataRow = GridView2.GetDataRow(i)
                Dim aParams As New ArrayList
                Dim dtPrint As New DataTable
                If row(0) = True Then
                    CRSerie = row(1)
                    CRNumero = row(2)
                    dtPrint = ExecuteAccessQuery("select * from RetencionesQry1 where sociedad='" & lueSociedad.EditValue & "' and serie_comprobante='" & CRSerie & "' and numero_comprobante='" & CRNumero & "'").Tables(0)
                    sFileName = My.Settings.RetenTargetDirectory & "\" & "CR" & CRSerie & CRNumero & ".pdf"
                    'Report.FileName = IO.Directory.GetCurrentDirectory & "\Reports\" & "ComprobanteRetencion.rpt"
                    'Report.SetParameterValue(0, CRNumero)
                    'Report.SetParameterValue(1, lueSociedad.Text)
                    'Report.SetParameterValue(2, lueSociedad.GetColumnValue("CompanyTaxCode"))
                    'Report.SetParameterValue(3, lueSociedad.GetColumnValue("CompanyAddress"))
                    'Report.SetParameterValue(4, lueSociedad.GetColumnValue("CompanyTelephone"))
                    'Report.SetParameterValue(5, "2248251011")
                    'Report.SetParameterValue(6, "001-00002001")
                    'Report.SetParameterValue(7, "001-00003000")
                    aParams.Add(CRNumero)
                    aParams.Add(lueSociedad.Text)
                    aParams.Add(lueSociedad.GetColumnValue("CompanyTaxCode"))
                    aParams.Add(lueSociedad.GetColumnValue("CompanyAddress"))
                    aParams.Add(lueSociedad.GetColumnValue("CompanyTelephone"))
                    aParams.Add("2248251011")
                    aParams.Add("001-00002001")
                    aParams.Add("001-00003000")
                    LoadReport(dtPrint, aParams, sFileName)
                    'Report.ExportToDisk(ExportFormatType.PortableDocFormat, sFileName)
                    'Report.Close()
                End If
                i += 1
            Loop
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Los comprobantes de retención fueron generados satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            'Report.Close()
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
    End Sub

    Private Sub LoadReport(dtPrint As DataTable, aParams As ArrayList, sFileName As String)
        'Dim rvPrint As New CrystalReportViewer
        Dim rdPrint As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Try
            rdPrint.FileName = IO.Directory.GetCurrentDirectory & "\Reports\" & "ComprobanteRetencion.rpt"
            rdPrint.SetDataSource(dtPrint)
            For p = 0 To aParams.Count - 1
                rdPrint.SetParameterValue(p, aParams(p))
            Next
            'rvPrint.ReportSource = rdPrint
            rdPrint.ExportToDisk(ExportFormatType.PortableDocFormat, sFileName)
            rdPrint.Close()
        Catch ex As Exception
            rdPrint.Close()
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Friend Function CRSeleccionados() As Boolean
        Dim iFilas As Integer = 0
        Dim i As Integer = 0
        Do While i < GridView2.RowCount
            Dim row As DataRow = GridView2.GetDataRow(i)
            If row(0) Then
                iFilas += 1
            End If
            i += 1
        Loop
        Return iFilas
    End Function

    Private Sub GeneraArchivoPlano()
        If CreateTextDelimiterFile(beArchivoSalida.EditValue, dtFlatFile, "|", False, False) Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El archivo plano ha sido generado satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No se generó el archivo plano, consulte con soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

    End Sub

    Private Sub CreaTablaArchivoPlano()
        dtFlatFile.Columns.Add("C1", GetType(String))
        dtFlatFile.Columns.Add("C2", GetType(String))
        dtFlatFile.Columns.Add("C3", GetType(String))
        dtFlatFile.Columns.Add("C4", GetType(String))
        dtFlatFile.Columns.Add("C5", GetType(String))
        dtFlatFile.Columns.Add("C6", GetType(String))
        dtFlatFile.Columns.Add("C7", GetType(String))
        dtFlatFile.Columns.Add("C8", GetType(String))
        dtFlatFile.Columns.Add("C9", GetType(String))
        dtFlatFile.Columns.Add("C10", GetType(String))
        dtFlatFile.Columns.Add("C11", GetType(String))
        dtFlatFile.Columns.Add("C12", GetType(String))
        dtFlatFile.Columns.Add("C13", GetType(String))
        dtFlatFile.Columns.Add("C14", GetType(String))
        dtFlatFile.Columns.Add("C15", GetType(String)).AllowDBNull = True
    End Sub

    Private Sub ObtieneComprobantes()
        Dim dtQuery As New DataTable
        Dim iPos As Integer = 0
        dsMain.Tables("Retenciones").Rows.Clear()
        dtFlatFile.Rows.Clear()
        dtResult.Rows.Clear()
        dtQuery = ExecuteAccessQuery("select * from retenciones  where sociedad='" & lueSociedad.EditValue & "' and periodo='" & tePeriodo.Text & "' order by numero_comprobante").Tables(0)
        For Each row As DataRow In dtQuery.Rows
            dtResult.Rows.Add()
            dtFlatFile.Rows.Add()
            iPos = dtResult.Rows.Count - 1
            dtResult.Rows(iPos).Item("C1") = row(0)
            dtResult.Rows(iPos).Item("C2") = row(1)
            dtResult.Rows(iPos).Item("C3") = row(2)
            dtResult.Rows(iPos).Item("C4") = row(3)
            dtResult.Rows(iPos).Item("C5") = row(4)
            dtFlatFile.Rows(iPos).Item(0) = row(4)
            dtResult.Rows(iPos).Item("C6") = row(5)
            If Not IsDBNull(row(5)) Then
                dtFlatFile.Rows(iPos).Item(1) = Strings.Left(row(5), 40)
            End If
            dtResult.Rows(iPos).Item("C7") = row(6)
            If Not IsDBNull(row(6)) Then
                dtFlatFile.Rows(iPos).Item(2) = Strings.Left(row(6), 20)
            End If
            dtResult.Rows(iPos).Item("C8") = row(7)
            If Not IsDBNull(row(7)) Then
                dtFlatFile.Rows(iPos).Item(3) = Strings.Left(row(7), 20)
            End If
            dtResult.Rows(iPos).Item("C9") = row(8)
            If Not IsDBNull(row(8)) Then
                dtFlatFile.Rows(iPos).Item(4) = Strings.Left(row(8), 20)
            End If
            dtResult.Rows(iPos).Item("C10") = row(9)
            dtFlatFile.Rows(iPos).Item(9) = row(9)
            dtResult.Rows(iPos).Item("C11") = row(10)
            dtFlatFile.Rows(iPos).Item(10) = row(10)
            dtResult.Rows(iPos).Item("C12") = row(11)
            dtFlatFile.Rows(iPos).Item(11) = row(11)
            dtResult.Rows(iPos).Item("C13") = Format(row(12), "dd/MM/yyyy")
            dtFlatFile.Rows(iPos).Item(12) = Format(row(12), "dd/MM/yyyy")
            dtResult.Rows(iPos).Item("C14") = row(13)
            dtResult.Rows(iPos).Item("C15") = Format(row(14), "#.00")
            dtResult.Rows(iPos).Item("C16") = Format(row(15), "#.00000")
            dtResult.Rows(iPos).Item("C17") = Format(row(16), "#.00")
            dtFlatFile.Rows(iPos).Item(13) = Format(row(16), "#.00")
            dtResult.Rows(iPos).Item("C18") = Format(row(17), "#.00")
            dtResult.Rows(iPos).Item("C19") = row(18)
            dtFlatFile.Rows(iPos).Item(5) = row(18)
            dtResult.Rows(iPos).Item("C20") = row(19)
            dtFlatFile.Rows(iPos).Item(6) = row(19)
            dtResult.Rows(iPos).Item("C21") = Format(row(20), "#.00")
            dtFlatFile.Rows(iPos).Item(8) = Format(row(20), "#.00")
            dtResult.Rows(iPos).Item("C22") = Format(row(21), "#.00")
            dtResult.Rows(iPos).Item("C23") = Format(row(22), "dd/MM/yyyy")
            dtFlatFile.Rows(iPos).Item(7) = Format(row(22), "dd/MM/yyyy")
        Next
        dtListaCR = SelectDistinct(dtResult, "C0", "C19", "C20", "C23", "C21", "C22")
        
    End Sub

    Private Sub CreaTablaListaCR()
        dtListaCR.Columns.Add("C0", GetType(Boolean))
        dtListaCR.Columns.Add("C19", GetType(String))
        dtListaCR.Columns.Add("C20", GetType(String))
        dtListaCR.Columns.Add("C23", GetType(String))
        dtListaCR.Columns.Add("C21", GetType(String))
        dtListaCR.Columns.Add("C22", GetType(String))
    End Sub

    Private Sub RetencionesForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        tePeriodo.Text = Format(Now, "yyyyMM")
        FillCompany()
        LoadTypePaytDoc()
        CreaTablaArchivoPlano()
        CreaTablaListaCR()
        dtResult = dsMain.Tables("Retenciones")
        dtResult.Columns.Add("C0", GetType(Boolean)).DefaultValue = False
        dtEventos.Columns.Add("Evento", GetType(String))
        dtEventos.Columns.Add("Tipo", GetType(Integer))
    End Sub

    Private Sub FillCompany()
        lueSociedad.Properties.DataSource = FillDataTable("Company", "")
        lueSociedad.Properties.DisplayMember = "CompanyDescription"
        lueSociedad.Properties.ValueMember = "CompanyCode"
    End Sub

    Private Sub LoadTypePaytDoc()
        dtTypePaytDoc = FillDataTable("TipoComprobante", "")
    End Sub

    Private Sub bbiProcesar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesar.ItemClick
        LoadInputValidations(DirectCast(e.Item, DevExpress.XtraBars.BarButtonItem).Name)
        If vpInputs.Validate Then
            Try
                SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
                If ProcesaArchivoExterno(beArchivoOrigen.Text) Then
                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El proceso finalizó satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                SplashScreenManager.CloseForm(False)
            End Try
        End If
    End Sub

    Friend Function ProcesaArchivoExterno(Archivo As String) As Boolean
        Dim bResult As Boolean = True
        Dim dtExcel, dtQuery As New DataTable
        Dim TipEvt As Integer
        Dim msg As String
        dtExcel = LoadExcel(Archivo, "{0}").Tables(0).Select("", "[Fecha de Pago],[RUC]").CopyToDataTable
        dtEventos.Rows.Clear()
        dtResult.Rows.Clear()
        For Each row As DataRow In dtExcel.Rows
            If IsDBNull(row(0)) Then
                Continue For
            End If
            If row(0).ToString <> 0 Then
                TipEvt = 0
                dtQuery.Rows.Clear()
                dtQuery = ExecuteAccessQuery("select * from Retenciones where numdoc_proveedor = '" & row(1).ToString.Trim & "' and tipdoc+numser+numdoc = '" & Strings.Right("00" & row(6).ToString, 2) & Strings.Right("0000" & row(7).ToString, 4) & Strings.Right("00000000" & row(8).ToString, 8) & "'").Tables(0)
                msg = "El documento " & Format(row(6), "00") & "-" & Format(row(7), "0000") & "-" & Format(row(8), "00000000") & " del proveedor " & row(2) & " se insertó correctamente."
                If dtQuery.Rows.Count > 0 Then
                    Dim QryRow As DataRow = dtQuery.Rows(0)
                    TipEvt = 1
                    msg = "El documento " & Format(row(6), "00") & "-" & Format(row(7), "0000") & "-" & Format(row(8), "00000000") & " del proveedor " & QryRow(5) & " ya existe y está asociado al comprobante de retención " & QryRow(18).ToString & "-" & QryRow(19).ToString & " del periodo " & QryRow(1).ToString
                    If tePeriodo.Text <> QryRow(1).ToString Then
                        bResult = False
                        msg = "El documento " & Format(row(6), "00") & "-" & Format(row(7), "0000") & "-" & Format(row(8), "00000000") & " del proveedor " & QryRow(5) & " ya existe y está asociado al comprobante de retención " & QryRow(18).ToString & "-" & QryRow(19).ToString & " del periodo " & QryRow(1).ToString
                        TipEvt = 2
                        DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                    dtExcel.Rows(dtExcel.Rows.IndexOf(row)).Delete()
                End If
                dtEventos.Rows.Add()
                dtEventos.Rows(dtEventos.Rows.Count - 1).Item(0) = msg
                dtEventos.Rows(dtEventos.Rows.Count - 1).Item(1) = TipEvt
            End If
        Next
        If bResult Then
            dtExcel.AcceptChanges()
            If dtExcel.Rows.Count > 0 Then
                AsignaDatos(dtExcel)
                For Each cr_fila As DataRow In dtResult.Rows
                    InsertIntoAccess1("Retenciones", cr_fila)
                Next
            End If
            ObtieneComprobantes()
            Dim ImpBasCR, ImpRetCR As Integer
            For Each updrow As DataRow In dtListaCR.Rows
                ImpBasCR = dtResult.Compute("Sum(C17)", "C20='" & updrow(2) & "'")
                ImpRetCR = dtResult.Compute("Sum(C18)", "C20='" & updrow(2) & "'")
                ExecuteAccessNonQuery("update retenciones set importe_base_comprobante = " & ImpBasCR.ToString & ", importe_retenido_comprobante = " & ImpRetCR.ToString & " where numero_comprobante = '" & updrow(2).ToString & "';")
            Next
            'gcRetenciones.DataSource = dtResult
            bbiConsultar.PerformClick()
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El proceso encontró varios errores que deben corregirse antes de procesar nuevamente el archivo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
        Return bResult
    End Function

    Private Sub AsignaDatos(dtExcel As DataTable)
        Dim dtQuery As New DataTable
        Dim MaxNumCR As Integer = CInt(ExecuteAccessQuery("select iif(isnull(max(numero_comprobante)), 0, max(numero_comprobante)) as MaxNumCR from Retenciones where sociedad = '" & lueSociedad.EditValue & "'").Tables(0).Rows(0)(0)) + 1
        Dim NumCR As String = ""
        Dim row As DataRow = dtExcel.Rows(0)
        Dim key As String = row(1) & Format(row(10), "yyyyMMdd")
        For iRow = 0 To dtExcel.Rows.Count - 1
            row = dtExcel.Rows(iRow)
            dtQuery.Rows.Clear()
            dtQuery = ExecuteAccessQuery("select numero_comprobante from retenciones  where sociedad='" & lueSociedad.EditValue & "' and numdoc_proveedor='" & row(1).ToString.Trim & "' and fecha_comprobante=" & Format(row(10), "#dd/MM/yyyy#")).Tables(0)
            If key <> row(1) & Format(row(10), "yyyyMMdd") Then
                key = row(1) & Format(row(10), "yyyyMMdd")
                MaxNumCR = MaxNumCR + 1
            End If
            If dtQuery.Rows.Count = 0 Then
                NumCR = Format(MaxNumCR, "00000000")
            Else
                NumCR = dtQuery.Rows(0)(0)
            End If
            dtResult.Rows.Add()
            dtResult.Rows(iRow).Item("C1") = lueSociedad.EditValue
            dtResult.Rows(iRow).Item("C2") = tePeriodo.Text
            dtResult.Rows(iRow).Item("C3") = row(0)
            dtResult.Rows(iRow).Item("C4") = IIf(row(1).ToString.Length = 8, "1", "6")
            dtResult.Rows(iRow).Item("C5") = row(1).ToString.Trim
            dtResult.Rows(iRow).Item("C6") = row(2)
            dtResult.Rows(iRow).Item("C7") = row(3)
            dtResult.Rows(iRow).Item("C8") = row(4)
            dtResult.Rows(iRow).Item("C9") = row(5)
            dtResult.Rows(iRow).Item("C10") = DataValidation("TipDoc", Microsoft.VisualBasic.Left(row(6).trim, 2))
            dtResult.Rows(iRow).Item("C11") = GetTextFormatValue(dtResult.Rows(iRow).Item("C10"), "NroSer", row(7).ToString.Trim)
            dtResult.Rows(iRow).Item("C12") = GetTextFormatValue(dtResult.Rows(iRow).Item("C10"), "NroDoc", row(8).ToString.Trim)
            dtResult.Rows(iRow).Item("C13") = Format(row(9), "dd/MM/yyyy")
            dtResult.Rows(iRow).Item("C14") = row(11)
            dtResult.Rows(iRow).Item("C15") = row(12)
            dtResult.Rows(iRow).Item("C16") = row(13)
            dtResult.Rows(iRow).Item("C17") = row(14)
            dtResult.Rows(iRow).Item("C18") = row(15)
            dtResult.Rows(iRow).Item("C19") = "0001"
            dtResult.Rows(iRow).Item("C20") = NumCR
            dtResult.Rows(iRow).Item("C21") = 0
            dtResult.Rows(iRow).Item("C22") = 0
            dtResult.Rows(iRow).Item("C23") = Format(row(10), "dd/MM/yyyy")
        Next
    End Sub

    Private Sub lueSociedad_Properties_Leave(sender As Object, e As EventArgs)
        beArchivoSalida.Text = My.Settings.RetenTargetDirectory & "\0626" & lueSociedad.GetColumnValue("CompanyTaxCode") & tePeriodo.Text & ".TXT"
    End Sub

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
        If GridView1.RowCount > 0 Then
            ExportarExcel(gcRetenciones)
        End If
    End Sub

    Private Sub bbiConsultar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiConsultar.ItemClick
        LoadInputValidations(DirectCast(e.Item, DevExpress.XtraBars.BarButtonItem).Name)
        If vpInputs.Validate Then
            ObtieneComprobantes()
            gcListaComprobantes.DataSource = dtListaCR
            For i = 21 To 22
                GridView2.Columns("C" & i.ToString).SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
                GridView2.Columns("C" & i.ToString).DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                GridView2.Columns("C" & i.ToString).DisplayFormat.FormatString = "n2"
            Next
            gcRetenciones.DataSource = dtResult
            For i = 15 To 18
                GridView1.Columns("C" & i.ToString).DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                If i = 16 Then
                    GridView1.Columns("C" & i.ToString).DisplayFormat.FormatString = "n5"
                ElseIf i <> 15 Then
                    GridView1.Columns("C" & i.ToString).SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
                Else
                    GridView1.Columns("C" & i.ToString).DisplayFormat.FormatString = "n2"
                End If
            Next
        End If
    End Sub

    Private Sub bbiGenerarArchivoPDT_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiGenerarArchivoPDT.ItemClick
        If GridView1.RowCount > 0 Then
            GeneraArchivoPlano()
        End If
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub LoadInputValidations(ObjectName As String)
        Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

        containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
        containsValidationRule.ErrorText = "Asigne un valor."
        containsValidationRule.ErrorType = ErrorType.Critical

        Dim customValidationRule As New CustomValidationRule()
        customValidationRule.ErrorText = "Valor obligatorio."
        customValidationRule.ErrorType = ErrorType.Critical
        Validate()
        vpInputs.SetValidationRule(lueSociedad, Nothing)
        vpInputs.SetValidationRule(tePeriodo, Nothing)
        vpInputs.SetValidationRule(beArchivoOrigen, Nothing)

        If ObjectName = "bbiConsultar" Then
            vpInputs.SetValidationRule(lueSociedad, customValidationRule)
            vpInputs.SetValidationRule(tePeriodo, customValidationRule)
        End If
        If ObjectName = "bbiProcesar" Then
            vpInputs.SetValidationRule(beArchivoOrigen, customValidationRule)
        End If
    End Sub

    Private Sub tePeriodo_EditValueChanged(sender As Object, e As EventArgs) Handles tePeriodo.EditValueChanged
        beArchivoSalida.Text = My.Settings.RetenTargetDirectory & "\0626" & lueSociedad.GetColumnValue("CompanyTaxCode") & tePeriodo.Text & ".TXT"
    End Sub

    Private Sub GridView2_CellValueChanged(sender As System.Object, e As DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs) Handles GridView1.CellValueChanged
        If e.Column.Caption = "Seleccionar" Then
            If RepositoryItemCheckEdit3.ValueChecked Then

            End If
        End If
    End Sub

    Private Sub RepositoryItemCheckEdit3_CheckStateChanged(sender As System.Object, e As System.EventArgs) Handles RepositoryItemCheckEdit3.CheckStateChanged
        GridView2.CloseEditor()
    End Sub

    Private Sub bbiVerEventos_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiVerEventos.ItemClick
        Dim popup As New EventosForm
        popup.gcEventos.DataSource = dtEventos
        popup.ShowDialog()
    End Sub

    Private Sub SeleccionaTodosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SeleccionaTodosToolStripMenuItem.Click
        SeleccionaFilas(0)
    End Sub

    Private Sub SeleccionaFilas(caso As Integer)
        Dim i As Integer = 0
        Do While i < GridView2.RowCount
            Dim row As DataRow = GridView2.GetDataRow(i)
            If caso = 0 Then
                row(0) = True
            End If
            If caso = 1 Then
                row(0) = False
            End If
            If caso = 2 Then
                If row(0) Then
                    row(0) = False
                Else
                    row(0) = True
                End If
            End If
            i += 1
        Loop
    End Sub

    Private Sub DeseleccionaTodosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeseleccionaTodosToolStripMenuItem.Click
        SeleccionaFilas(1)
    End Sub

    Private Sub InvertirSelecciónToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InvertirSelecciónToolStripMenuItem.Click
        SeleccionaFilas(2)
    End Sub

    Friend Function GetTextFormatValue(DocType As String, Group As String, Value As String) As String
        Dim sResult As String = ""
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

    Friend Function DataValidation(column As String, value As String) As String
        Dim sResult As String = ""
        value = Strings.Right("00" & value.Trim, 2)
        If column = "TipDoc" Then
            If dtTypePaytDoc.Select("Código = '" & value & "'").Length > 0 Then
                sResult = value
            End If
        End If
        Return sResult
    End Function

End Class