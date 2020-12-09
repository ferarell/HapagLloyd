﻿Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraSplashScreen

Public Class LocalBenefitsForm
    Dim oSharePointTransactions As New SharePointListTransactions
    Dim oAppService As New AppService.HapagLloydServiceClient
    Dim dtList As New DataTable
    Dim oDataAcces As New DataAccess

    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        oSharePointTransactions.SharePointUrl = My.Settings.SharePoint_Url
    End Sub

    Private Sub LocalBenefitsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SplitContainerControl1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1
        LoadCountry
    End Sub

    Private Sub LoadCountry()
        Dim dtQuery As New DataTable
        dtQuery = oAppService.ExecuteSQL("SELECT * FROM spl.Country").Tables(0)
        lueCountry.Properties.DataSource = dtQuery
        lueCountry.Properties.DisplayMember = "CountryName"
        lueCountry.Properties.ValueMember = "CountryCode"
        lueOriginCountry.Properties.DataSource = lueCountry.Properties.DataSource
        lueOriginCountry.Properties.DisplayMember = lueCountry.Properties.DisplayMember
        lueOriginCountry.Properties.ValueMember = lueCountry.Properties.ValueMember
        lueLoadCountry.Properties.DataSource = lueCountry.Properties.DataSource
        lueLoadCountry.Properties.DisplayMember = lueCountry.Properties.DisplayMember
        lueLoadCountry.Properties.ValueMember = lueCountry.Properties.ValueMember
        lueDischargeCountry.Properties.DataSource = lueCountry.Properties.DataSource
        lueDischargeCountry.Properties.DisplayMember = lueCountry.Properties.DisplayMember
        lueDischargeCountry.Properties.ValueMember = lueCountry.Properties.ValueMember
        lueFinalCountry.Properties.DataSource = lueCountry.Properties.DataSource
        lueFinalCountry.Properties.DisplayMember = lueCountry.Properties.DisplayMember
        lueFinalCountry.Properties.ValueMember = lueCountry.Properties.ValueMember
    End Sub

    Private Sub LoadPort(luePort As DevExpress.XtraEditors.LookUpEdit, CountryCode As String)
        If CountryCode Is Nothing Then
            luePort.Properties.DataSource = Nothing
            Return
        End If
        Dim dtQuery As New DataTable
        dtQuery = oAppService.ExecuteSQL("SELECT * FROM spl.Port WHERE CountryCode = '" & CountryCode & "'").Tables(0)
        luePort.Properties.DataSource = dtQuery
        luePort.Properties.DisplayMember = "PortName"
        luePort.Properties.ValueMember = "PortCode"
    End Sub
    Private Sub LocalBenefitsForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            oSharePointTransactions.SharePointList = "Local Benefits"
            oSharePointTransactions.FieldsList.Clear()
            oSharePointTransactions.FieldsList.Add({"ID"})
            oSharePointTransactions.FieldsList.Add({"CodigoPais"})
            oSharePointTransactions.FieldsList.Add({"TipoEmbarque"})
            oSharePointTransactions.FieldsList.Add({"TipoBeneficio"})
            oSharePointTransactions.FieldsList.Add({"RazonSocial"})
            oSharePointTransactions.FieldsList.Add({"NumeroIdentificacionTributaria"})
            oSharePointTransactions.FieldsList.Add({"Vigencia_Desde"})
            oSharePointTransactions.FieldsList.Add({"Vigencia_Hasta"})
            oSharePointTransactions.FieldsList.Add({"SalesCoordinator"})
            oSharePointTransactions.FieldsList.Add({"SalesExecution"})
            oSharePointTransactions.FieldsList.Add({"TipoConcesion"})
            oSharePointTransactions.FieldsList.Add({"CondicionBL"})
            oSharePointTransactions.FieldsList.Add({"MBL_Rol"})
            oSharePointTransactions.FieldsList.Add({"MBL_RUC"})
            oSharePointTransactions.FieldsList.Add({"MBL_RazonSocial"})
            oSharePointTransactions.FieldsList.Add({"HBL_Rol"})
            oSharePointTransactions.FieldsList.Add({"HBL_RUC"})
            oSharePointTransactions.FieldsList.Add({"HBL_RazonSocial"})
            oSharePointTransactions.FieldsList.Add({"BillOfLading"})
            oSharePointTransactions.FieldsList.Add({"Booking"})
            'oSharePointTransactions.FieldsList.Add({"Importe_TDE"})
            'oSharePointTransactions.FieldsList.Add({"Importe_TDI"})
            'oSharePointTransactions.FieldsList.Add({"Importe_GDCE"})
            'oSharePointTransactions.FieldsList.Add({"Importe_GDCI"})
            'oSharePointTransactions.FieldsList.Add({"Importe_SACE"})
            'oSharePointTransactions.FieldsList.Add({"Importe_SACI"})
            'oSharePointTransactions.FieldsList.Add({"Importe_SACCE"})
            'oSharePointTransactions.FieldsList.Add({"Importe_SACCI"})
            'oSharePointTransactions.FieldsList.Add({"Importe_GateIn"})
            'oSharePointTransactions.FieldsList.Add({"Importe_GateOut"})
            'oSharePointTransactions.FieldsList.Add({"Rebate_Gates"})
            'oSharePointTransactions.FieldsList.Add({"Rebate_VistoBueno"})
            oSharePointTransactions.FieldsList.Add({"NumeroConcesion"})

            SplashScreenManager.Default.SetWaitFormDescription("Get Local Benefits")
            dtList = oSharePointTransactions.GetItems()
            GridControl1.DataSource = dtList
            FormatGrid(GridView1)
            SplashScreenManager.CloseForm(False)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub FormatGrid(oGridView As GridView)
        For c = 0 To oGridView.Columns.Count - 1
            oGridView.Columns(c).OptionsColumn.ReadOnly = True
        Next
    End Sub

    'Private Sub Sincronize()
    '    Dim dtSource As New DataTable
    '    Try
    '        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
    '        dtSource = LoadExcelHDR(beDataSource.Text, "Data_Landscape_color$", "A3:N3000").Tables(0)
    '        For r = 0 To dtSource.Rows.Count - 1
    '            Dim sArrDateTime, sDepDateTime, sCloDateTime As String
    '            If IsDBNull(dtSource.Rows(r)(1)) Then
    '                Continue For
    '            End If
    '            For c = 0 To dtSource.Rows(r).ItemArray.Count - 1
    '                If IsDBNull(dtSource.Rows(r).Item(c)) Then
    '                    dtSource.Rows(r).Item(c) = ""
    '                End If
    '            Next
    '            If dtSource.Rows(r)(1) = "" Then
    '                Continue For
    '            End If
    '            If dtSource.Rows(r)("DP Voyage") = 0 Then
    '                Continue For
    '            End If
    '            If dtCoordinator.Select("Coordinator_Service='" & dtSource.Rows(r)("SSY") & "'").Length = 0 Then
    '                Continue For
    '            End If
    '            Dim oDPVoyage, oPol As String
    '            oDPVoyage = dtSource.Rows(r)("DP Voyage")
    '            oPol = dtSource.Rows(r)("Port Locode")
    '            Dim IdRow As Integer = 0
    '            If dtList.Select("DPVoyage = '" & oDPVoyage & "' AND Port_Locode = '" & oPol & "'").Length > 0 Then
    '                IdRow = dtList.Select("DPVoyage = '" & oDPVoyage & "' AND Port_Locode = '" & oPol & "'")(0)("ID")
    '            End If
    '            If dtSource.Rows(r)("Arr Date") <> "" Then
    '                sArrDateTime = Format(CDate(dtSource.Rows(r)("Arr Date") & Space(1) & IIf(dtSource.Rows(r)("Arr Time") = "", "00:00", dtSource.Rows(r)("Arr Time"))), "M/d/yyyy HH:mm")
    '            End If
    '            If dtSource.Rows(r)("Dep Date") <> "" Then
    '                sDepDateTime = Format(CDate(dtSource.Rows(r)("Dep Date") & Space(1) & IIf(dtSource.Rows(r)("Dep Time") = "", "00:00", dtSource.Rows(r)("Dep Time"))), "M/d/yyyy HH:mm")
    '            End If
    '            If dtSource.Rows(r)("Close Docu Date") <> "" Then
    '                sCloDateTime = Format(CDate(dtSource.Rows(r)("Close Docu Date") & Space(1) & IIf(dtSource.Rows(r)("Close Docu Time") = "", "00:00", dtSource.Rows(r)("Close Docu Time"))), "M/d/yyyy HH:mm")
    '            End If
    '            If IdRow = 0 Then
    '                oSharePointTransactions.ValuesList.Clear()
    '                oSharePointTransactions.ValuesList.Add({"SSY", dtSource.Rows(r)("SSY")})
    '                oSharePointTransactions.ValuesList.Add({"Port_Locode", oPol})
    '                oSharePointTransactions.ValuesList.Add({"TerminalCode", dtSource.Rows(r)("Terminal")})
    '                oSharePointTransactions.ValuesList.Add({"DPVoyage", oDPVoyage})
    '                oSharePointTransactions.ValuesList.Add({"VesselName", dtSource.Rows(r)("Vessel")})
    '                oSharePointTransactions.ValuesList.Add({"ScheduleVoyage", dtSource.Rows(r)("Schedule Voyage No#")})

    '                If IsDate(sArrDateTime) Then
    '                    oSharePointTransactions.ValuesList.Add({"Arrival_Date", sArrDateTime})
    '                End If
    '                If IsDate(sDepDateTime) Then
    '                    oSharePointTransactions.ValuesList.Add({"Departure_Date", sDepDateTime})
    '                End If
    '                If IsDate(sCloDateTime) Then
    '                    oSharePointTransactions.ValuesList.Add({"Close_Document_Date", sCloDateTime})
    '                End If
    '                If dtCoordinator.Select("Coordinator_Service='" & dtSource.Rows(r)("SSY") & "'").Length > 0 Then
    '                    oSharePointTransactions.ValuesList.Add({"Coordinator_Name", dtCoordinator.Select("Coordinator_Service='" & dtSource.Rows(r)("SSY") & "'")(0)("Coordinator_x0020_UserName")})
    '                    'oSharePointTransactions.ValuesList.Add({"Coordinator_UserAccount", dtCoordinator.Select("Coordinator_Service='" & dtSource.Rows(r)("SSY") & "'")(0)("Coordinator_x0020_UserAccount")})
    '                End If
    '                'oSharePointTransactions.FieldsList.Add({"Coordinator_x0020_UserName", dtSource.Rows(r)("Coordinator_x0020_UserName")})
    '                'oSharePointTransactions.FieldsList.Add({"Local_Transmition_Date", dtSource.Rows(r)("Local_Transmition_Date")})
    '                oSharePointTransactions.InsertItem()
    '            Else
    '                oSharePointTransactions.ValuesList.Clear()
    '                Dim drItem As DataRow = dtList.Select("ID=" & IdRow.ToString)(0)
    '                If IsDate(sArrDateTime) Then
    '                    If CDate(sArrDateTime) <> drItem("Arrival_Date") Then
    '                        oSharePointTransactions.ValuesList.Add({"Arrival_Date", sArrDateTime})
    '                    End If
    '                End If
    '                If IsDate(sDepDateTime) Then
    '                    If CDate(sDepDateTime) <> drItem("Departure_Date") Then
    '                        oSharePointTransactions.ValuesList.Add({"Departure_Date", sDepDateTime})
    '                    End If
    '                End If
    '                If IsDate(sCloDateTime) Then
    '                    If CDate(sCloDateTime) <> drItem("Close_Document_Date") Then
    '                        oSharePointTransactions.ValuesList.Add({"Close_Document_Date", sCloDateTime})
    '                    End If
    '                End If
    '                If oSharePointTransactions.ValuesList.Count > 0 Then
    '                    oSharePointTransactions.UpdateItem(IdRow)
    '                End If
    '            End If
    '        Next
    '        bbiShowAll.PerformClick()
    '        SplashScreenManager.CloseForm(False)
    '        DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The process has been completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
    '    Catch ex As Exception
    '        SplashScreenManager.CloseForm(False)
    '        DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    End Try
    'End Sub

    Private Sub bbiSincronize_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSincronize.ItemClick
        'Sincronize()
    End Sub

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiShowAll.ItemClick
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            GridControl1.DataSource = Nothing
            dtList.Rows.Clear()
            dtList = oSharePointTransactions.GetItems()
            GridControl1.DataSource = dtList
            SplashScreenManager.CloseForm(False)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick

    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub bbiEdit_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiEdit.ItemClick
        SplitContainerControl1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Both
        Dim iPos As Integer = SplitContainerControl1.Size.Height - LayoutControl1.Size.Height
        SplitContainerControl1.SplitterPosition = iPos
    End Sub

    Private Sub GridView1_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView1.FocusedRowChanged
        Dim oControls As Control
        Dim oRow As DataRow = GridView1.GetFocusedDataRow
        Try
            For Each oControls In LayoutControl1.Controls
                If oControls.Tag Is Nothing Then
                    Continue For
                End If
                If oRow.Table.Columns.Contains(oControls.Tag) Then
                    'If DirectCast(oControls.AccessibilityObject, DevExpress.Accessibility.BaseAccessibleObject).Role = "ComboBox" Then
                    '    MsgBox("hola")
                    'End If
                    DirectCast(oControls, DevExpress.XtraEditors.BaseEdit).EditValue = oRow(oControls.Tag)
                End If
            Next

        Catch ex As Exception

        End Try

    End Sub

    Private Sub bbiSave_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSave.ItemClick
        If DevExpress.XtraEditors.XtraMessageBox.Show("Are you sure you want to save this record? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
            Return
        End If
        Dim drSource As DataRow = GridView1.GetFocusedDataRow

        If teID.Text = "" Then
            oSharePointTransactions.InsertItem()
        Else
            oSharePointTransactions.UpdateItem(teID.Text)
        End If
    End Sub

    Private Sub bbiNew_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiNew.ItemClick
        GridView1.AddNewRow()
        SplitContainerControl1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Both
        Dim iPos As Integer = SplitContainerControl1.Size.Height - LayoutControl1.Size.Height
        SplitContainerControl1.SplitterPosition = iPos
    End Sub

    Private Sub lueOriginCountry_EditValueChanged(sender As Object, e As EventArgs) Handles lueOriginCountry.EditValueChanged
        LoadPort(lueOriginPort, lueOriginCountry.EditValue)
    End Sub

    Private Sub lueLoadPort_EditValueChanged(sender As Object, e As EventArgs) Handles lueLoadPort.EditValueChanged
        LoadPort(lueLoadPort, lueLoadCountry.EditValue)
    End Sub

    Private Sub lueDischargePort_EditValueChanged(sender As Object, e As EventArgs) Handles lueDischargePort.EditValueChanged
        LoadPort(lueDischargePort, lueDischargeCountry.EditValue)
    End Sub

    Private Sub lueFinalPort_EditValueChanged(sender As Object, e As EventArgs) Handles lueFinalPort.EditValueChanged
        LoadPort(lueFinalPort, lueFinalCountry.EditValue)
    End Sub
End Class