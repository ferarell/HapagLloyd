Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports DevExpress.XtraSplashScreen
Imports System.Threading

Public Class DetraccionesConstanciasForm
    Dim dtSource As New DataTable
    Dim LastButton As String = ""

    Private Sub DetraccionesForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FiltersEnabled()
    End Sub

    Private Sub bbiProcesar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesar.ItemClick
        LastButton = e.Link.Item.Name
        LoadInputValidations()
        If vpFilters.Validate Then
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            Try
                LoadDataSource()
            Catch ex As Exception
            Finally
                SplashScreenManager.CloseForm(False)
            End Try
        End If
    End Sub

    Private Sub LoadDataSource()
        Dim dtTarget As New DataTable
        Dim bProcess As Boolean = True
        Dim iFiles As Integer = OpenFileDialog1.FileNames.Count
        Dim WaitText As String = ""
        For i = 0 To iFiles - 1
            Try
                WaitText = "Cargando archivos Sunat (Archivo: " & (i + 1).ToString & " de " & iFiles.ToString & ")"
                If OpenFileDialog1.FileNames(i).ToUpper.EndsWith(".CSV") Then
                    Dim dtMasterTmp As New DataTable
                    SplashScreenManager.Default.SetWaitFormDescription(WaitText)
                    dtMasterTmp = LoadCSV(OpenFileDialog1.FileNames(i), True, ",")
                    InsertDataFile(dtMasterTmp, WaitText)
                End If
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Ocurrió un error durante la carga de datos del archivo " & OpenFileDialog1.FileNames(i) & ". " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                SplashScreenManager.CloseForm(False)
                bProcess = False
            End Try
        Next
        SplashScreenManager.CloseForm(False)
        If bProcess Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Los datos fueron cargados satisfactoriamente.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Ocurrió un error durante la carga de datos. ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

    End Sub

    Private Sub InsertDataFile(dtFile As DataTable, WaitText As String)
        Try
            gcDetracciones.DataSource = dtSource
            For Each row As DataRow In dtFile.Rows
                If row(0) = "" Then
                    Continue For
                End If
                gcDetracciones.RefreshDataSource()
                SplashScreenManager.Default.SetWaitFormDescription(WaitText & " (Fila: " & (dtFile.Rows.IndexOf(row) + 1).ToString & " de " & dtFile.Rows.Count.ToString & ")")
                If ExecuteAccessQuery("select * from ConstanciasDetracciones where [Numero de Documento Adquiriente]='" & row(7).ToString.Trim & "' and [Numero Constancia] = '" & row(2).ToString & "'").Tables(0).Rows.Count = 0 Then
                    InsertIntoAccess("ConstanciasDetracciones", row)
                    dtSource.ImportRow(row)
                End If
            Next
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        'dtTarget = ExecuteAccessQuery("select * from ConstanciasDetracciones").Tables(0)
        'gcDetracciones.DataSource = dtSource
        GridView1.PopulateColumns()
    End Sub

    Private Sub SearchData()
        Dim dtTarget As New DataTable
        Dim dDate1, dDate2 As String
        dDate1 = Format(deDateFrom.EditValue, "#MM/dd/yyyy#")
        dDate2 = Format(deDateTo.EditValue, "#MM/dd/yyyy#")
        dtTarget = ExecuteAccessQuery("select * from ConstanciasDetracciones where [Fecha Pago] >= " & dDate1 & " and [Fecha Pago] <= " & dDate2).Tables(0)
        gcDetracciones.DataSource = dtTarget
    End Sub

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
        ExportarExcel(gcDetracciones)
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub beArchivoOrigen_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beArchivoOrigen.Properties.ButtonClick
        OpenFileDialog1.Filter = "Archivos de Origen (*.csv)|*.csv"
        OpenFileDialog1.FileName = ""
        'OpenFileDialog1.InitialDirectory = IIf(My.Settings.LedgerSourceDirectory8 <> "", My.Settings.LedgerSourceDirectory8, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beArchivoOrigen.Text = OpenFileDialog1.FileName
        End If
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
        vpFilters.SetValidationRule(beArchivoOrigen, Nothing)
        vpFilters.SetValidationRule(deDateFrom, Nothing)
        vpFilters.SetValidationRule(deDateTo, Nothing)

        If LastButton = "bbiProcesar" Then
            vpFilters.SetValidationRule(beArchivoOrigen, customValidationRule)
        Else
            vpFilters.SetValidationRule(deDateFrom, customValidationRule)
            vpFilters.SetValidationRule(deDateTo, customValidationRule)
        End If

    End Sub

    Private Sub FiltersEnabled()
        'If rbProcessType.SelectedIndex = 0 Then
        '    beArchivoOrigen.Enabled = True
        '    deDateFrom.Enabled = False
        '    deDateTo.Enabled = False
        'Else
        '    beArchivoOrigen.Enabled = False
        '    deDateFrom.Enabled = True
        '    deDateTo.Enabled = True
        'End If
    End Sub

    Private Sub bbiConsultar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiConsultar.ItemClick
        LastButton = e.Link.Item.Name
        LoadInputValidations()
        If Not vpFilters.Validate Then
            Return
        End If
        SearchData()
    End Sub
End Class