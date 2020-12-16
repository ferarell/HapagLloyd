Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraSplashScreen

Public Class LocalBenefitsForm
    Dim oSharePointTransactions As New SharePointListTransactions
    Dim oAppService As New AppService.HapagLloydServiceClient
    Dim dtList, dtListDetail, dtConcept, dtCurrency, dtUserRole As New DataTable
    Dim oDataAcces As New DataAccess

    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        oSharePointTransactions.SharePointUrl = My.Settings.SharePoint_Url
        GridView2.OptionsView.NewItemRowPosition = NewItemRowPosition.Top
    End Sub

    Private Sub LocalBenefitsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SplitContainerControl1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1
        SplitContainerControl2.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1
        Try
            LoadCountry()
            LoadCurrency()
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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

    Private Sub LoadCurrency()
        dtCurrency = oAppService.ExecuteSQL("SELECT * FROM spl.Currency").Tables(0)
        RepositoryItemLookUpEdit3.DataSource = dtCurrency
        RepositoryItemLookUpEdit3.DisplayMember = "CurrencyName"
        RepositoryItemLookUpEdit3.ValueMember = "CurrencyCode"
        RepositoryItemLookUpEdit3.KeyMember = "ID"
    End Sub

    Private Sub LoadUserRole()
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get User Roles")
        oSharePointTransactions.SharePointList = "UserRoleByProcess"
        oSharePointTransactions.FieldsList.Clear()
        oSharePointTransactions.FieldsList.Add({"ID"})
        oSharePointTransactions.FieldsList.Add({"ProcessCode"})
        oSharePointTransactions.FieldsList.Add({"UserAccount"})
        oSharePointTransactions.FieldsList.Add({"UserName"})
        oSharePointTransactions.FieldsList.Add({"UserMail"})
        oSharePointTransactions.FieldsList.Add({"UserType"})

        dtUserRole = oSharePointTransactions.GetItems()
        If dtUserRole.Select("ProcessCode='LCI'").Length > 0 Then
            dtUserRole = dtUserRole.Select("ProcessCode='LCI'").CopyToDataTable
        End If
        lueSalesExecution.Properties.DataSource = dtUserRole.Select("UserType='Sales Execution'").CopyToDataTable
        lueSalesExecution.Properties.DisplayMember = "UserName"
        lueSalesExecution.Properties.ValueMember = "UserName"
        lueSalesCoordination.Properties.DataSource = dtUserRole.Select("UserType='Sales Coordination'").CopyToDataTable
        lueSalesCoordination.Properties.DisplayMember = lueSalesExecution.Properties.DisplayMember
        lueSalesCoordination.Properties.ValueMember = lueSalesExecution.Properties.ValueMember
        lueUserAuthorization.Properties.DataSource = dtUserRole.Select("UserType LIKE '%Authorization'").CopyToDataTable
        lueUserAuthorization.Properties.DisplayMember = lueSalesExecution.Properties.DisplayMember
        lueUserAuthorization.Properties.ValueMember = lueSalesExecution.Properties.ValueMember
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub LoadLocalBenefits()
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get Local Benefits")
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
        oSharePointTransactions.FieldsList.Add({"SalesExecution"})
        oSharePointTransactions.FieldsList.Add({"SalesCoordination"})
        oSharePointTransactions.FieldsList.Add({"TipoConcesion"})
        oSharePointTransactions.FieldsList.Add({"TipoConcesionEspecifica"})
        oSharePointTransactions.FieldsList.Add({"CondicionBL"})
        oSharePointTransactions.FieldsList.Add({"RateAgreement"})
        oSharePointTransactions.FieldsList.Add({"MBL_Rol"})
        oSharePointTransactions.FieldsList.Add({"MBL_RUC"})
        oSharePointTransactions.FieldsList.Add({"MBL_RazonSocial"})
        oSharePointTransactions.FieldsList.Add({"HBL_Rol"})
        oSharePointTransactions.FieldsList.Add({"HBL_RUC"})
        oSharePointTransactions.FieldsList.Add({"HBL_RazonSocial"})
        oSharePointTransactions.FieldsList.Add({"BillOfLading"})
        oSharePointTransactions.FieldsList.Add({"Booking"})
        oSharePointTransactions.FieldsList.Add({"PaisOrigen"})
        oSharePointTransactions.FieldsList.Add({"PuertoOrigen"})
        oSharePointTransactions.FieldsList.Add({"PaisCarga"})
        oSharePointTransactions.FieldsList.Add({"PuertoCarga"})
        oSharePointTransactions.FieldsList.Add({"PaisDescarga"})
        oSharePointTransactions.FieldsList.Add({"PuertoDescarga"})
        oSharePointTransactions.FieldsList.Add({"PaisFinal"})
        oSharePointTransactions.FieldsList.Add({"PuertoFinal"})
        oSharePointTransactions.FieldsList.Add({"PrecintoBASC"})
        oSharePointTransactions.FieldsList.Add({"Profit"})
        oSharePointTransactions.FieldsList.Add({"Volumen"})
        oSharePointTransactions.FieldsList.Add({"UsuarioAutorizador"})
        oSharePointTransactions.FieldsList.Add({"FechaAutorizacion"})
        'oSharePointTransactions.FieldsList.Add({"Booking"})
        'oSharePointTransactions.FieldsList.Add({"Booking"})
        'oSharePointTransactions.FieldsList.Add({"Booking"})
        'oSharePointTransactions.FieldsList.Add({"Booking"})
        'oSharePointTransactions.FieldsList.Add({"Booking"})
        oSharePointTransactions.FieldsList.Add({"Estado"})
        oSharePointTransactions.FieldsList.Add({"NumeroConcesion"})

        dtList = oSharePointTransactions.GetItems()
        GridControl1.DataSource = dtList
        FormatGrid(GridView1)
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub LoadLocalBenefitsDetail()
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get Local Benefits Detail")
        oSharePointTransactions.SharePointList = "LocalBenefitsDetail"
        oSharePointTransactions.FieldsList.Clear()
        oSharePointTransactions.FieldsList.Add({"ID"})
        oSharePointTransactions.FieldsList.Add({"IdParent"})
        oSharePointTransactions.FieldsList.Add({"ConceptCode"})
        oSharePointTransactions.FieldsList.Add({"ConceptValue"})
        oSharePointTransactions.FieldsList.Add({"ConceptCurrency"})

        dtListDetail = oSharePointTransactions.GetItems()
        GridControl2.DataSource = dtListDetail
        GridView2.BestFitColumns()
        SplashScreenManager.CloseForm(False)
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

    Private Sub LoadConcepts()
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get Concepts By Country")
        oSharePointTransactions.SharePointList = "ConceptByCountryList"
        oSharePointTransactions.FieldsList.Clear()
        oSharePointTransactions.FieldsList.Add({"ID"})
        oSharePointTransactions.FieldsList.Add({"CountryCode"})
        oSharePointTransactions.FieldsList.Add({"ConceptCode"})
        oSharePointTransactions.FieldsList.Add({"ConceptName"})
        dtConcept = oSharePointTransactions.GetItems()
        RepositoryItemLookUpEdit2.DataSource = dtConcept
        RepositoryItemLookUpEdit2.DisplayMember = "ConceptName"
        RepositoryItemLookUpEdit2.ValueMember = "ConceptCode"
        RepositoryItemLookUpEdit2.KeyMember = "ID"
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub LocalBenefitsForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        LoadLocalBenefits()
        LoadConcepts()
        LoadLocalBenefitsDetail()
        LoadUserRole()
    End Sub

    Private Sub FormatGrid(oGridView As GridView)
        For c = 0 To oGridView.Columns.Count - 1
            oGridView.Columns(c).OptionsColumn.ReadOnly = True
        Next
    End Sub

    Private Sub bbiRefresh_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiRefresh.ItemClick
        LoadLocalBenefits()
        LoadLocalBenefitsDetail()
        'Try
        '    SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        '    GridControl1.DataSource = Nothing
        '    dtList.Rows.Clear()
        '    dtList = oSharePointTransactions.GetItems()
        '    GridControl1.DataSource = dtList
        '    SplashScreenManager.CloseForm(False)
        'Catch ex As Exception
        '    SplashScreenManager.CloseForm(False)
        '    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        'End Try

    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(GridControl1)
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
        If GridView1.FocusedRowHandle < 0 Then
            Return
        End If
        Dim oControls As Control
        Dim oRow As DataRow = GridView1.GetFocusedDataRow
        Try
            For Each oControls In LayoutControl1.Controls
                If oControls.Tag Is Nothing Then
                    Continue For
                End If
                DirectCast(oControls, DevExpress.XtraEditors.BaseEdit).EditValue = Nothing
                If oRow.Table.Columns.Contains(oControls.Tag) Then
                    'If DirectCast(oControls.AccessibilityObject, DevExpress.Accessibility.BaseAccessibleObject).Role = "ComboBox" Then
                    '    MsgBox("hola")
                    'End If
                    If Not IsDBNull(oRow(oControls.Tag)) Then
                        DirectCast(oControls, DevExpress.XtraEditors.BaseEdit).EditValue = oRow(oControls.Tag)
                    End If
                End If
            Next

        Catch ex As Exception

        End Try

    End Sub

    Private Sub bbiSave_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSave.ItemClick
        Validate()
        If DevExpress.XtraEditors.XtraMessageBox.Show("Are you sure you want to save this record? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
            Return
        End If
        Dim drSource As DataRow = GridView1.GetFocusedDataRow
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Save Local Benefits")
            oSharePointTransactions.SharePointList = "Local Benefits"
            oSharePointTransactions.ValuesList.Clear()
            If drSource("CodigoPais").ToString <> lueCountry.EditValue Then
                oSharePointTransactions.ValuesList.Add({"CodigoPais", lueCountry.GetColumnValue("ID")})
            End If
            If drSource("TipoEmbarque").ToString <> cbeLoadingType.EditValue.ToString Then
                oSharePointTransactions.ValuesList.Add({"TipoEmbarque", cbeLoadingType.EditValue})
            End If
            If drSource("TipoBeneficio").ToString <> cbeBenefitType.Text Then
                oSharePointTransactions.ValuesList.Add({"TipoBeneficio", cbeBenefitType.EditValue})
            End If
            If drSource("RazonSocial").ToString <> teCompanyName.Text Then
                oSharePointTransactions.ValuesList.Add({"RazonSocial", teCompanyName.Text})
            End If
            If drSource("NumeroIdentificacionTributaria").ToString <> teTaxNumber.Text Then
                oSharePointTransactions.ValuesList.Add({"NumeroIdentificacionTributaria", teTaxNumber.Text})
            End If
            If drSource("Vigencia_Desde") <> deValidityFrom.Text Then
                oSharePointTransactions.ValuesList.Add({"Vigencia_Desde", deValidityFrom.DateTime.ToShortDateString})
            End If
            If drSource("Vigencia_Hasta") <> deValidityTo.Text Then
                oSharePointTransactions.ValuesList.Add({"Vigencia_Hasta", deValidityTo.DateTime.ToShortDateString})
            End If
            If drSource("SalesExecution").ToString <> lueSalesExecution.Text Then
                oSharePointTransactions.ValuesList.Add({"SalesExecution", lueSalesExecution.GetColumnValue("ID")})
            End If
            If drSource("SalesCoordination").ToString <> lueSalesCoordination.Text Then
                oSharePointTransactions.ValuesList.Add({"SalesCoordination", lueSalesCoordination.GetColumnValue("ID")})
            End If
            If drSource("TipoConcesion").ToString <> cbeConcessionType.Text Then
                oSharePointTransactions.ValuesList.Add({"TipoConcesion", cbeConcessionType.EditValue})
            End If
            If drSource("CondicionBL").ToString <> cbeBlContition.Text Then
                oSharePointTransactions.ValuesList.Add({"CondicionBL", cbeBlContition.EditValue})
            End If
            If drSource("RateAgreement").ToString <> teRateAgreement.Text Then
                oSharePointTransactions.ValuesList.Add({"RateAgreement", teRateAgreement.Text})
            End If
            If drSource("MBL_Rol").ToString <> cbeMblRol.Text Then
                oSharePointTransactions.ValuesList.Add({"MBL_Rol", cbeMblRol.EditValue})
            End If
            If drSource("MBL_RUC").ToString <> teMblNit.Text Then
                oSharePointTransactions.ValuesList.Add({"MBL_RUC", teMblNit.Text})
            End If
            If drSource("MBL_RazonSocial").ToString <> teMblCompanyName.Text Then
                oSharePointTransactions.ValuesList.Add({"MBL_RazonSocial", teMblCompanyName.Text})
            End If
            If drSource("HBL_Rol").ToString <> cbeHblRol.Text Then
                oSharePointTransactions.ValuesList.Add({"HBL_Rol", cbeHblRol.EditValue})
            End If
            If drSource("HBL_RUC").ToString <> teHblNit.Text Then
                oSharePointTransactions.ValuesList.Add({"HBL_RUC", teHblNit.Text})
            End If
            If drSource("HBL_RazonSocial").ToString <> teHblCompanyName.Text Then
                oSharePointTransactions.ValuesList.Add({"HBL_RazonSocial", teHblCompanyName.Text})
            End If
            If drSource("BillOfLading").ToString <> teBillOfLading.Text Then
                oSharePointTransactions.ValuesList.Add({"BillOfLading", teBillOfLading.Text})
            End If
            If drSource("Booking").ToString <> teBooking.Text Then
                oSharePointTransactions.ValuesList.Add({"Booking", teBooking.Text})
            End If
            If drSource("PaisOrigen").ToString <> lueOriginCountry.Text Then
                oSharePointTransactions.ValuesList.Add({"PaisOrigen", lueOriginCountry.EditValue})
            End If
            If drSource("PuertoOrigen").ToString <> lueOriginPort.Text Then
                oSharePointTransactions.ValuesList.Add({"PuertoOrigen", lueOriginPort.GetColumnValue("ID")})
            End If
            If drSource("PaisCarga").ToString <> lueLoadCountry.Text Then
                oSharePointTransactions.ValuesList.Add({"PaisCarga", lueLoadCountry.EditValue})
            End If
            If drSource("PuertoCarga").ToString <> lueLoadPort.Text Then
                oSharePointTransactions.ValuesList.Add({"PuertoCarga", lueLoadPort.GetColumnValue("ID")})
            End If
            If drSource("PaisDescarga").ToString <> lueDischargeCountry.Text Then
                oSharePointTransactions.ValuesList.Add({"PaisDescarga", lueDischargeCountry.EditValue})
            End If
            If drSource("PuertoDescarga").ToString <> lueDischargePort.Text Then
                oSharePointTransactions.ValuesList.Add({"PuertoDescarga", lueDischargePort.GetColumnValue("ID")})
            End If
            If drSource("PaisFinal").ToString <> lueFinalCountry.Text Then
                oSharePointTransactions.ValuesList.Add({"PaisFinal", lueFinalCountry.EditValue})
            End If
            If drSource("PuertoFinal").ToString <> lueFinalPort.Text Then
                oSharePointTransactions.ValuesList.Add({"PuertoFinal", lueFinalPort.GetColumnValue("ID")})
            End If
            If drSource("PrecintoBASC").ToString <> teSealBasc.Text Then
                oSharePointTransactions.ValuesList.Add({"PrecintoBASC", teSealBasc.Text})
            End If
            If drSource("Profit").ToString <> teProfit.Text Then
                oSharePointTransactions.ValuesList.Add({"Profit", teProfit.Text})
            End If
            If drSource("Volumen").ToString <> teVolumen.Text Then
                oSharePointTransactions.ValuesList.Add({"Volumen", teVolumen.Text})
            End If
            If drSource("UsuarioAutorizador").ToString <> lueUserAuthorization.Text Then
                'oSharePointTransactions.ValuesList.Add({"UsuarioAutorizador", lueUserAuthorization.GetColumnValue("ID")})
            End If
            'If drSource("FechaAutorizacion") <> deAuthorizationDate.Text Then
            '    oSharePointTransactions.ValuesList.Add({"FechaAutorizacion", deAuthorizationDate.DateTime.ToShortDateString})
            'End If
            'If drSource("Estado").ToString <> cbeStatus.Text Then
            '    oSharePointTransactions.ValuesList.Add({"Estado", cbeStatus.EditValue})
            'End If
            If drSource("NumeroConcesion").ToString <> teConcessionNumber.Text Then
                oSharePointTransactions.ValuesList.Add({"NumeroConcesion", teConcessionNumber.Text})
            End If
            If teID.Text = "" Then
                oSharePointTransactions.InsertItem()
            Else
                oSharePointTransactions.UpdateItem(teID.Text)
            End If
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Dim drSourceDetail As DataRow = GridView2.GetFocusedDataRow
        If drSourceDetail("IdParent") Is Nothing Then
            Return
        End If
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Save Local Benefits Detail")
            oSharePointTransactions.SharePointList = "LocalBenefitsDetail"
            oSharePointTransactions.ValuesList.Clear()
            oSharePointTransactions.ValuesList.Add({"IdParent", drSource("ID").ToString})
            oSharePointTransactions.ValuesList.Add({"ConceptCode", GetValueByField("Concept", drSourceDetail("ConceptCode"))})
            oSharePointTransactions.ValuesList.Add({"ConceptCurrency", GetValueByField("Currency", drSourceDetail("ConceptCurrency"))})
            oSharePointTransactions.ValuesList.Add({"ConceptValue", drSourceDetail("ConceptValue").ToString})

            If IsDBNull(drSourceDetail("ID")) Then
                oSharePointTransactions.InsertItem()
            Else
                oSharePointTransactions.UpdateItem(drSourceDetail("ID").ToString)
            End If
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Function GetValueByField(FieldName As String, FieldValue As String) As String
        Dim sResult As String = ""
        If FieldName = "Concept" Then
            sResult = dtConcept.Select("ConceptCode='" & FieldValue & "'")(0)("ID")
        End If
        If FieldName = "Currency" Then
            sResult = dtCurrency.Select("CurrencyCode='" & FieldValue & "'")(0)("ID")
        End If
        Return sResult
    End Function
    Private Sub bbiNew_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiNew.ItemClick
        GridView1.AddNewRow()
        SplitContainerControl1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Both
        Dim iPos As Integer = SplitContainerControl1.Size.Height - LayoutControl1.Size.Height
        SplitContainerControl1.SplitterPosition = iPos
    End Sub

    Private Sub lueOriginCountry_EditValueChanged(sender As Object, e As EventArgs) Handles lueOriginCountry.EditValueChanged
        LoadPort(lueOriginPort, lueOriginCountry.EditValue)
    End Sub

    Private Sub lueLoadCountry_EditValueChanged(sender As Object, e As EventArgs) Handles lueLoadCountry.EditValueChanged
        LoadPort(lueLoadPort, lueLoadCountry.EditValue)
    End Sub

    Private Sub lueDischargeCountry_EditValueChanged(sender As Object, e As EventArgs) Handles lueDischargeCountry.EditValueChanged
        LoadPort(lueDischargePort, lueDischargeCountry.EditValue)
    End Sub

    Private Sub lueFinalCountry_EditValueChanged(sender As Object, e As EventArgs) Handles lueFinalCountry.EditValueChanged
        LoadPort(lueFinalPort, lueFinalCountry.EditValue)
    End Sub

    Private Sub BarEditItem1_EditValueChanged(sender As Object, e As EventArgs) Handles BarEditItem1.EditValueChanged
        SplitContainerControl2.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1
        If BarEditItem1.EditValue = True Then
            SplitContainerControl2.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Both
        End If
    End Sub
End Class