Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraBars
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.IO

Public Class ProveedoresForm
    Dim sAction As String = "Browser"
    Dim dtVendor, dtAgenRet, dtBueCont As New DataTable

    Private Sub TiposCambioForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            FillMainView()
            ControlsActive("Load")
            LoadValidations()
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        SplitContainerControl2.Collapsed = True
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
        ExportarExcel(gcProveedores)
    End Sub

    Private Sub FillMainView()
        Try
            dtVendor.Rows.Clear()
            dtVendor = ExecuteAccessQuery("select * from Proveedores").Tables(0)
            gcProveedores.DataSource = dtVendor
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ControlsActive(Action As String)
        ButtonsActive(Action)
        InputsActive(Action)
    End Sub

    Private Sub InputsActive(Action As String)
        For Each ctrl As Control In SplitContainerControl1.Panel1.Controls
            For Each input As Control In ctrl.Controls
                If input.Name <> "teRUC" Then
                    input.Enabled = True
                End If
            Next
        Next
        For Each ctrl As Control In SplitContainerControl1.Panel1.Controls
            'If DirectCast(ctrl, DevExpress.XtraEditors.GroupControl).Text.Contains({"Datos Generales", "Tipo Cambio Venta", "Tipo Cambio Compra", "Tipo de Registro"}) Then
            For Each input As Control In ctrl.Controls
                If Action.Contains({"Load", "Save", "Delete", "Undo"}) Then
                    input.Enabled = False
                End If
                'If Action.Contains({"Edit"}) Then
                '    If input.Name.Contains({"teRUC"}) Then
                '        input.Enabled = True
                '    End If
                'End If
                If Action.Contains({"Insert"}) Then
                    input.Text = ""
                End If
            Next
            'End If
        Next
    End Sub

    Private Sub ButtonsActive(Action As String)
        For Each bbi As BarButtonItemLink In bAcciones.ItemLinks
            If bbi IsNot Nothing Then
                bbi.Item.Enabled = True
            End If
        Next
        For Each bbi As BarButtonItemLink In bAcciones.ItemLinks
            If bbi IsNot Nothing Then
                If Action.Contains({"Load", "Save", "Delete", "Undo"}) Then
                    If bbi.Caption.Contains({"Deshacer", "Grabar"}) Then
                        bbi.Item.Enabled = False
                    End If
                End If
                If Action.Contains({"Insert", "Edit"}) Then
                    If bbi.Caption.Contains({"Nuevo", "Editar", "Eliminar", "Exportar"}) Then
                        bbi.Item.Enabled = False
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub bbiNuevo_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiNuevo.ItemClick
        sAction = e.Item.Tag
        ControlsActive(sAction)
        teRUC.Enabled = True
        teRUC.Focus()
    End Sub

    Private Sub bbiEditar_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiEditar.ItemClick
        sAction = e.Item.Tag
        ControlsActive(sAction)
        teRUC.Enabled = False
    End Sub

    Private Sub bbiGrabar_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiGrabar.ItemClick
        Me.Refresh()
        If vpControls.Validate Then
            Try
                If sAction = "Insert" Then
                    Dim dtQuery As New DataTable
                    dtQuery = ExecuteAccessQuery("select * from Proveedores where [NoRUC] = '" & teRUC.Text & "'").Tables(0)
                    If dtQuery.Rows.Count = 0 Then
                        dtQuery.Rows.Add({teRUC.Text, teRazonSocial.Text, teCtaBN.Text})
                        If InsertIntoAccess("Proveedores", dtQuery.Rows(0)) Then
                            FillMainView()
                            DevExpress.XtraEditors.XtraMessageBox.Show("Los datos han sido actualizados satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                    End If
                Else
                    If UpdateAccessRow(teCodSAP.Text, teRUC.Text, teRazonSocial.Text, teCtaBN.Text) Then
                        FillMainView()
                        DevExpress.XtraEditors.XtraMessageBox.Show("Los datos han sido actualizados satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End If
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show("Error al insertar nuevo registro." & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                ControlsActive("Save")
            End Try
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El proveedor ya existe, por favor verifique.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
        sAction = "Browser"
    End Sub

    Private Sub bbiDeshacer_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiDeshacer.ItemClick
        sAction = e.Item.Tag
        ControlsActive(sAction)
        sAction = "Browser"
    End Sub

    Private Sub LoadValidations()
        Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

        containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
        containsValidationRule.ErrorText = "Asigne un valor."
        containsValidationRule.ErrorType = ErrorType.Critical

        Dim customValidationRule As New CustomValidationRule()
        customValidationRule.ErrorText = "Valor obligatorio."
        customValidationRule.ErrorType = ErrorType.Critical

        vpControls.SetValidationRule(teRUC, customValidationRule)
        vpControls.SetValidationRule(teRazonSocial, customValidationRule)
    End Sub

    Private Sub CardView1_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles CardView1.FocusedRowChanged
        If e.FocusedRowHandle >= 0 Then
            Dim drvItem As DataRowView = CardView1.GetRow(e.FocusedRowHandle)
            teCodSAP.EditValue = drvItem("CodigoSAP")
            teRUC.EditValue = drvItem("NoRUC")
            teRazonSocial.EditValue = drvItem("Nombre")
            teCtaBN.EditValue = drvItem("CuentaBN")
        End If
    End Sub

    Private Sub GridView1_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView1.FocusedRowChanged
        If e.FocusedRowHandle >= 0 And sAction = "Browser" Then
            Dim drvItem As DataRowView = GridView1.GetRow(e.FocusedRowHandle)
            teCodSAP.EditValue = drvItem("CodigoSAP")
            teRUC.EditValue = drvItem("NoRUC")
            teRazonSocial.EditValue = drvItem("Nombre")
            teCtaBN.EditValue = drvItem("CuentaBN")
        End If
    End Sub

    Private Sub bbiEliminar_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiEliminar.ItemClick
        Dim dtQuery As New DataTable
        If DevExpress.XtraEditors.XtraMessageBox.Show("Está seguro que desea eliminar el registro seleccionado? ", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
            If ExecuteAccessNonQuery("DELETE FROM Proveedores where [NoRUC] = '" & teRUC.Text & "'") Then
                GridView1.FocusedRowHandle = GridView1.FocusedRowHandle - 1
                FillMainView()
                DevExpress.XtraEditors.XtraMessageBox.Show("El registro ha sido eliminado satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
        sAction = "Browser"
    End Sub

    Friend Function UpdateAccessRow(codsap As String, ruc As String, nombre As String, cuenta As String) As Boolean
        Dim bResult As Boolean = True
        Dim SetValues, Condition As String
        SetValues = ""
        If sAction = "Load" Then
            Condition = "[NoRUC] = '" & ruc & "'"
            If codsap <> "" Then
                SetValues = SetValues & IIf(SetValues <> "", ", ", "") & codsap & "'"
            End If
            If nombre <> "" Then
                SetValues = SetValues & IIf(SetValues <> "", ", ", "") & "[Nombre] = '" & nombre & "'"
            End If
            If cuenta <> "" Then
                SetValues = SetValues & IIf(SetValues <> "", ", ", "") & "[CuentaBN] = '" & cuenta & "'"
            End If
        Else
            Condition = "[NoRUC] = '" & teRUC.Text & "'"
            SetValues = SetValues & IIf(SetValues <> "", ", ", "") & "[Nombre] = '" & teRazonSocial.Text & "'"
            SetValues = SetValues & IIf(SetValues <> "", ", ", "") & "[CuentaBN] = '" & teCtaBN.Text & "'"
        End If
        bResult = UpdateAccess("Proveedores", Condition, SetValues)
        Return bResult
    End Function

    Private Sub bbiImportar_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiImportar.ItemClick
        SplitContainerControl2.Collapsed = False
        sAction = "Load"
    End Sub

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoOrigen.Properties.ButtonClick
        OpenFileDialog1.Filter = "Source Files (*.xls*;*.txt;*.zip)|*.xls*;*.txt;*.zip"
        OpenFileDialog1.FileName = ""
        'OpenFileDialog1.InitialDirectory = IIf(My.Settings.LedgerSourceDirectory5 <> "", My.Settings.LedgerSourceDirectory5, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beArchivoOrigen.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub sbLoadVendor_Click(sender As Object, e As EventArgs) Handles sbLoadVendor.Click
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        Try
            For i = 0 To OpenFileDialog1.FileNames.Count - 1
                If OpenFileDialog1.FileNames(i).ToUpper.Contains(".XLS") Then
                    LoadVendors(OpenFileDialog1.FileNames(i))
                End If
            Next
            For i = 0 To OpenFileDialog1.FileNames.Count - 1
                If OpenFileDialog1.FileNames(i).ToUpper.EndsWith(".ZIP") Then
                    Decompress(OpenFileDialog1.FileNames(i), Path.GetDirectoryName(OpenFileDialog1.FileNames(i)))
                    LoadSunatFiles(OpenFileDialog1.FileNames(i).ToUpper.Replace(".ZIP", ".TXT"))
                End If
            Next
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
        SplitContainerControl2.Collapsed = True
        FillMainView()
    End Sub

    Private Sub LoadVendors(sFileName As String)
        SplashScreenManager.Default.SetWaitFormDescription("Cargando proveedores...")
        Dim dtQuery As New DataTable
        Dim iPos As Integer = 0
        Dim sCodSAP, sRUC, sCta As String
        sCodSAP = ""
        sRUC = ""
        sCta = ""
        dtVendor.Rows.Clear()
        dtVendor = ExecuteAccessQuery("select * from Proveedores").Tables(0)
        dtQuery = LoadExcel(sFileName, "{0}").Tables(0)
        For Each row As DataRow In dtQuery.Rows
            If Not IsDBNull(row(0)) Then
                If row(0) = "" Then
                    Continue For
                End If
            End If
            sRUC = IIf(row(9) <> "" And Len(Replace(row(9), "RUC", "")) = 11, Replace(row(9), "RUC", ""), IIf(Len(row(10)) = 11, row(10), ""))
            sCodSAP = row(3).ToString
            If row(13) = "018" Then 'And row(13).ToString.Trim <> "" Then
                sCta = row(14).ToString.Trim
            End If
            If Not ValidaRUC(sRUC) Then
                Continue For
            End If
            If ExecuteAccessQuery("select * from Proveedores where [NoRUC]='" & sRUC & "' and [CodigoSAP]='" & sCodSAP & "'").Tables(0).Rows.Count = 0 Then
                dtVendor.Rows.Add()
                iPos = dtVendor.Rows.Count - 1
                dtVendor.Rows(iPos).Item(0) = sCodSAP
                dtVendor.Rows(iPos).Item(1) = sRUC
                dtVendor.Rows(iPos).Item(2) = row(5)
                dtVendor.Rows(iPos).Item(3) = ""
                If Not InsertIntoAccess("Proveedores", dtVendor.Rows(iPos)) Then
                    Exit For
                End If
            Else
                If sCta <> "" Then
                    UpdateAccessRow(sCodSAP, sRUC, row(5), sCta)
                End If
            End If
        Next
    End Sub

    Private Sub LoadSunatFiles(sFileName As String)
        SplashScreenManager.Default.SetWaitFormDescription("Cargando archivo SUNAT...")
        Dim dtQuery As New DataTable
        Dim iPos As Integer = 0
        Dim sColumn As String = IIf(sFileName.ToUpper.Contains("AGENRET_TXT"), "AgenteRetenedor", "BuenContribuyente")
        Dim SetValues, Condition As String
        dtQuery = LoadTXT(sFileName, True, "|")
        SplashScreenManager.Default.SetWaitFormDescription("Actualizando Contribuyente SUNAT (" & sColumn & ")")
        For Each row As DataRow In dtVendor.Rows
            If dtQuery.Select("Ruc = '" & row(1) & "'").Length > 0 Then
                Condition = "[NoRUC] = '" & row(1) & "'"
                SetValues = sColumn & "='S'"
                If Not UpdateAccess("Proveedores", Condition, SetValues) Then
                    DevExpress.XtraEditors.XtraMessageBox.Show("Error al actualizar proveedor:" & row(2) & ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        Next
    End Sub

    'Private Sub LoadDataSources()
    '    For i = 0 To OpenFileDialog1.FileNames.Count - 1
    '        Try
    '            If OpenFileDialog1.FileNames(i).ToUpper.EndsWith(".ZIP") Then
    '                SplashScreenManager.Default.SetWaitFormDescription("(" & (i + 1).ToString & " of 5) Loading Invoice Data Source...")
    '                dtSource1 = LoadTXT(OpenFileDialog1.FileNames(i), True)
    '            End If
    '        Catch ex As Exception
    '            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "There was an error loading the Invoice Report. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        End Try
    '        If OpenFileDialog1.FileNames(i).ToUpper.EndsWith(".XLS") Or OpenFileDialog1.FileNames(i).ToUpper.EndsWith(".XLSX") Or OpenFileDialog1.FileNames(i).ToUpper.EndsWith(".XLSB") Then
    '            Try
    '                SplashScreenManager.Default.SetWaitFormDescription("(" & (i + 1).ToString & " of 5) Loading Main Data Source 1...")
    '                If LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Columns.Count = 16 Then
    '                    dtSource2 = LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0)
    '                End If
    '            Catch ex As Exception
    '                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "There was an error loading the Main Data Source (15). " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '            End Try
    '            Try
    '                SplashScreenManager.Default.SetWaitFormDescription("(" & (i + 1).ToString & " of 5) Loading Main Data Source 2...")
    '                If LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Columns.Count = 20 Then
    '                    dtSource5 = LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0)
    '                End If
    '            Catch ex As Exception
    '                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "There was an error loading the Main Data Source (19). " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '            End Try
    '            Try
    '                If LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0).Columns.Count < 6 Then
    '                    SplashScreenManager.Default.SetWaitFormDescription("(" & (i + 1).ToString & " of 5) Loading Dictionary (GENSET) Data Source...")
    '                    dtSource3 = LoadExcel(OpenFileDialog1.FileNames(i), "GENSET$").Tables(0)
    '                    dtSource3.Columns(0).ColumnName = "C1"
    '                    SplashScreenManager.Default.SetWaitFormDescription("(" & (i + 1).ToString & " of 5) Loading Dictionary (COMMODITY) Data Source...")
    '                    dtSource4 = LoadExcel(OpenFileDialog1.FileNames(i), "commodity$").Tables(0)
    '                    dtSource4.Columns(0).ColumnName = "C1"
    '                End If
    '            Catch ex As Exception
    '                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "There was an error loading the dictionary (COMMODITY). " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '            End Try

    '        End If
    '    Next
    'End Sub

    Private Sub teRUC_Leave(sender As Object, e As EventArgs) Handles teRUC.Leave, teCodSAP.Leave
        If sAction = "Browser" Then
            Return
        End If
        If sender.Text.Length = 11 Then
            If Not ValidaRUC(sender.Text) Then
                DevExpress.XtraEditors.XtraMessageBox.Show("El RUC ingresado no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                sender.Focus()
            End If
            If ExecuteAccessQuery("select * from Proveedores where [NoRUC] = '" & teRUC.Text & "'").Tables(0).Rows.Count > 0 Then
                DevExpress.XtraEditors.XtraMessageBox.Show("El RUC ingresado ya existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                sender.Focus()
            End If
        ElseIf sender.Text.Length > 1 And sender.Text.Length < 11 Then
            DevExpress.XtraEditors.XtraMessageBox.Show("La longitud del RUC es incorrecta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            sender.Focus()
        End If
    End Sub

    Private Sub bbiSunatPadrones_ItemClick(sender As Object, e As ItemClickEventArgs) Handles bbiSunatPadrones.ItemClick
        System.Diagnostics.Process.Start("http://www.sunat.gob.pe/padronesnotificaciones/")
    End Sub

    Friend Function LoadTXT(FileName As String, Header As Boolean, ListSeparator As String) As DataTable
        Dim dtReading As New DataTable
        Dim sColumn As String = ""
        Dim txtpos As String = ""
        Dim iPosCol As Integer = 0
        Dim line As New StreamReader(FileName, False)
        Dim sFila As String = line.ReadLine
        For i = 1 To sFila.Count + 1
            txtpos = Mid(sFila, i, 1)
            If txtpos = ListSeparator Or i = sFila.Count + 1 Then
                If Header Then
                    dtReading.Columns.Add(sColumn).AllowDBNull = True
                Else
                    dtReading.Columns.Add("C" & (dtReading.Columns.Count + 1).ToString).AllowDBNull = True
                End If
                sColumn = ""
            Else
                sColumn = sColumn & txtpos
            End If
        Next
        Using sr As New StreamReader(FileName)
            Dim lines As List(Of String) = New List(Of String)
            Dim bExit As Boolean = False
            Dim sColumnValue As String = ""
            Do While Not sr.EndOfStream
                lines.Add(sr.ReadLine())
            Loop
            For i As Integer = 1 To lines.Count - 1
                iPosCol = 0
                txtpos = ""
                dtReading.Rows.Add()
                For c = 1 To lines.Item(i).Length + 1
                    txtpos = Mid(lines(i), c, 1)
                    If txtpos = ListSeparator Or c = lines.Item(i).Length + 1 Then
                        dtReading.Rows(i - 1).Item(iPosCol) = sColumnValue
                        iPosCol = iPosCol + 1
                        sColumnValue = ""
                    Else
                        sColumnValue = sColumnValue + txtpos.Replace("'", "")
                    End If
                Next
            Next
        End Using
        Return dtReading
    End Function

End Class