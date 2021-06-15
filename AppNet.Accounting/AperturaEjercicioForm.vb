Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports System.Collections
Imports DevExpress.XtraSplashScreen
Imports System.Threading

Public Class AperturaEjercicioForm
    Dim RUC, SunatFileName As String
    Dim LibroSunat As String = "BalanceComprobacion"
    Dim dsLibroSunat As New dsSunat
    Dim sEjercicioAnterior As String
    Dim dtCierre, dtApertura As New DataTable

    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().

    End Sub

    Private Sub AperturaEjercicioForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        seEjercicio.Value = Today.Year
        sEjercicioAnterior = seEjercicio.EditValue - 1
        FillCompany()
    End Sub

    Private Sub FillCompany()
        lueSociedad.Properties.DataSource = FillDataTable("Company", "", "ACC")
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
    End Sub

    Private Sub bbiCerrar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiCerrar.ItemClick
        Close()
    End Sub

    Private Sub bbiProcesar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesar.ItemClick
        Dim dtQuery As New DataTable
        dtQuery = ExecuteAccessQueryWP("select * from SaldosDetalleContableQry", "sPeriodo", seEjercicio.Text & "01").Tables(0)
        dtCierre = GeneraCierre(dtQuery.Select("Cuenta>'599999999'").CopyToDataTable)
        dtApertura = GeneraApertura(dtQuery.Select("Cuenta<='599999999'").CopyToDataTable)
    End Sub

    Friend Function GeneraCierre(dtSource As DataTable) As DataTable
        Dim dtQuery, dtResult As New DataTable
        Dim PstKey As String = ""
        Dim sReference As String = "CIERRE" & sEjercicioAnterior
        dtResult = ExecuteAccessQuery("select * from DetalleContable where sociedad=''").Tables(0)
        dtQuery = ExecuteAccessQueryWP("select * from SaldoResultadoEjercicioQry where sociedad='" & lueSociedad.EditValue & "'", "sPeriodo", sEjercicioAnterior).Tables(0)
        For Each row As DataRow In dtSource.Rows
            PstKey = IIf(row("importe_md") > 0, "50", "40")
            dtResult.Rows.Add(lueSociedad.EditValue, sEjercicioAnterior & "12", row("cuenta"), "", row("moneda"), sReference, dtResult.Rows.Count + 1, "31/12/" & sEjercicioAnterior, "31/12/" & sEjercicioAnterior, "CI", "", PstKey, "", sReference, "", "", row("importe_md"), IIf(PstKey = "40", row("importe_ml"), 0), IIf(PstKey = "50", row("importe_ml"), 0), IIf(PstKey = "40", row("importe_ml"), 0), IIf(PstKey = "50", row("importe_ml"), 0), IIf(PstKey = "40", row("importe_me"), 0), IIf(PstKey = "50", row("importe_me"), 0), IIf(PstKey = "40", row("importe_me"), 0), IIf(PstKey = "50", row("importe_me"), 0))
        Next
        For Each dtRow As DataRow In dtQuery.Rows
            dtRow("Periodo") = sEjercicioAnterior & "12"
            dtRow("CuentaLocal") = "592310001"
            dtRow("AsientoContable") = sReference
            dtRow("Posicion") = dtResult.Rows.Count + 1
            dtRow("FechaContable") = "31/12/" & sEjercicioAnterior
            dtRow("FechaDocumento") = dtRow("FechaContable")
            dtRow("TipoDocumento") = "CI"
            dtRow("Referencia") = sReference
            dtResult.Rows.Add(dtRow.ItemArray)
        Next
        gcCierre.DataSource = dtResult
        Return dtResult
    End Function

    Friend Function GeneraApertura(dtSource As DataTable) As DataTable
        Dim dtQuery, dtResult As New DataTable
        Dim PstKey As String = ""
        Dim sReference As String = "APERTURA" & seEjercicio.Text
        dtResult = ExecuteAccessQuery("select * from DetalleContable where sociedad=''").Tables(0)
        dtQuery = ExecuteAccessQueryWP("select * from SaldoResultadoEjercicioQry where sociedad='" & lueSociedad.EditValue & "'", "sPeriodo", sEjercicioAnterior).Tables(0)
        For Each row As DataRow In dtSource.Rows
            PstKey = IIf(row("importe_md") > 0, "40", "50")
            dtResult.Rows.Add(lueSociedad.EditValue, seEjercicio.Text & "01", row("cuenta"), "", row("moneda"), "APERTURA" & seEjercicio.Text, dtResult.Rows.Count + 1, "01/01/" & seEjercicio.Text, "01/01/" & seEjercicio.Text, "IN", "", PstKey, "", "APERTURA" & seEjercicio.Text, "", "", row("importe_md"), IIf(PstKey = "40", row("importe_ml"), 0), IIf(PstKey = "50", row("importe_ml"), 0), IIf(PstKey = "40", row("importe_ml"), 0), IIf(PstKey = "50", row("importe_ml"), 0), IIf(PstKey = "40", row("importe_me"), 0), IIf(PstKey = "50", row("importe_me"), 0), IIf(PstKey = "40", row("importe_me"), 0), IIf(PstKey = "50", row("importe_me"), 0))
        Next
        For Each dtRow As DataRow In dtQuery.Rows
            dtRow("Periodo") = seEjercicio.Text & "01"
            dtRow("CuentaLocal") = "592310001"
            dtRow("AsientoContable") = sReference
            dtRow("Posicion") = dtResult.Rows.Count + 1
            dtRow("FechaContable") = "01/01/" & seEjercicio.Text
            dtRow("FechaDocumento") = dtRow("FechaContable")
            dtRow("TipoDocumento") = "IN"
            dtRow("Referencia") = sReference
            dtResult.Rows.Add(dtRow.ItemArray)
        Next
        gcApertura.DataSource = dtResult
        Return dtResult
    End Function

    Friend Function DeleteDataFromDB() As Boolean
        Dim bResult As Boolean = True
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        Try
            SplashScreenManager.Default.SetWaitFormDescription("Eliminando el asiento de cierre")
            ExecuteAccessNonQuery("delete from DetalleContable where Sociedad='" & lueSociedad.EditValue & "' and Periodo='" & sEjercicioAnterior & "12' and TipoDocumento='CI'")
            SplashScreenManager.Default.SetWaitFormDescription("Eliminando el asiento de apertura")
            ExecuteAccessNonQuery("delete from DetalleContable where Sociedad='" & lueSociedad.EditValue & "' and Periodo='" & seEjercicio.Text & "01' and TipoDocumento='IN'")
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "No fue posible eliminar los datos existentes de la tabla.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
        Return bResult
    End Function

    Private Sub bbiExportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExportar.ItemClick
        If gcApertura.DefaultView.IsFocusedView Then
            ExportarExcel(gcApertura)
        Else
            ExportarExcel(gcCierre)
        End If

    End Sub

    Private Sub seEjercicio_Leave(sender As Object, e As EventArgs) Handles seEjercicio.Leave
        If seEjercicio.EditValue > Year(Today).ToString Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El ejercicio no puede ser mayor al año en curso.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            sender.focus()
        End If
    End Sub

    Private Sub bbiConsultar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiConsultar.ItemClick
        dtCierre.Rows.Clear()
        dtApertura.Rows.Clear()
        'dtQuery = ExecuteAccessQueryWP("select * from SaldosDetalleContableQry", "sPeriodo", seEjercicio.Text & "01").Tables(0)
        dtCierre = ExecuteAccessQuery("select * from DetalleContable where Sociedad='" & lueSociedad.EditValue & "' and Periodo='" & sEjercicioAnterior & "12' and TipoDocumento='CI'").Tables(0)
        gcCierre.DataSource = dtCierre
        dtApertura = ExecuteAccessQuery("select * from DetalleContable where Sociedad='" & lueSociedad.EditValue & "' and Periodo='" & seEjercicio.Text & "01' and TipoDocumento='IN'").Tables(0)
        gcApertura.DataSource = dtApertura
    End Sub

    Friend Function GetAmountPosition(pk As Integer) As String
        Dim PosCol As String = "H"
        If (pk >= 1 And pk <= 10) Or (pk >= 21 And pk <= 30) Or pk = 40 Or pk = 70 Or (pk >= 80 And pk <= 89) Then
            PosCol = "D"
        End If
        Return PosCol
    End Function

    Private Sub bbiActualizar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiActualizar.ItemClick
        If ExecuteAccessQuery("select * from DetalleContable where Sociedad='" & lueSociedad.EditValue & "' and Periodo='" & sEjercicioAnterior & "12' and TipoDocumento='CI'").Tables(0).Rows.Count > 0 Then
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "El proceso eliminará los datos existentes, está seguro que desea continuar?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                DeleteDataFromDB()
            End If
        End If
        InsertData()
    End Sub

    Private Sub InsertData()
        For Each row1 As DataRow In dtCierre.Rows
            InsertIntoAccess1("DetalleContable", row1)
        Next
        For Each row2 As DataRow In dtApertura.Rows
            InsertIntoAccess1("DetalleContable", row2)
        Next
    End Sub

End Class