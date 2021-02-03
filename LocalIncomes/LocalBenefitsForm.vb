Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.XtraRichEdit
Imports DevExpress.XtraSplashScreen

Public Class LocalBenefitsForm
    Dim oSharePointTransactions As New SharePointListTransactions
    Dim oAppService As New AppService.HapagLloydServiceClient
    Dim dtLocBenHeader, dtLocBenConcept, dtLocBenCommodity, dtLocBenEqpType, dtConceptList, dtCurrencyList, dtCommodityList, dtContainerTypeList, dtUserRoleList As New DataTable
    Dim oDataAcces As New DataAccess

    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        oSharePointTransactions.SharePointUrl = My.Settings.SharePoint_Url
        GridView2.OptionsView.NewItemRowPosition = NewItemRowPosition.Top
        GridView4.OptionsView.NewItemRowPosition = NewItemRowPosition.Top
    End Sub

    Private Sub LocalBenefitsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SplitContainerControl1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1
        SplitContainerControl2.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1
        'Try
        '    SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        '    SplashScreenManager.Default.SetWaitFormDescription("Get Data Master Tables")

        '    SplashScreenManager.CloseForm(False)
        'Catch ex As Exception
        '    SplashScreenManager.CloseForm(False)
        '    DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        'End Try
        XtraTabControl1.SelectedTabPageIndex = 0
    End Sub

    Private Sub LoadPartner()
        Dim dtQuery As New DataTable
        dtQuery = oAppService.ExecuteSQL("EXEC ntf.upGetAllPartnersByFilters 'C'").Tables(0)
        lueMblNit.Properties.DataSource = dtQuery
        lueMblNit.Properties.DisplayMember = "TaxNumber"
        lueMblNit.Properties.ValueMember = "TaxNumber"
        lueHblNit.Properties.DataSource = dtQuery
        lueHblNit.Properties.DisplayMember = "TaxNumber"
        lueHblNit.Properties.ValueMember = "TaxNumber"
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
        dtCurrencyList = oAppService.ExecuteSQL("SELECT * FROM spl.Currency").Tables(0)
        RepositoryItemLookUpEdit3.DataSource = dtCurrencyList
        RepositoryItemLookUpEdit3.DisplayMember = "CurrencyName"
        RepositoryItemLookUpEdit3.ValueMember = "CurrencyCode"
        RepositoryItemLookUpEdit3.KeyMember = "ID"
    End Sub

    Private Sub LoadCommodity()
        dtCommodityList = oAppService.ExecuteSQL("SELECT * FROM spl.Commodity").Tables(0)
        RepositoryItemLookUpEdit10.DataSource = dtCommodityList
        RepositoryItemLookUpEdit10.DisplayMember = "CommodityName"
        RepositoryItemLookUpEdit10.ValueMember = "CommodityCode"
        RepositoryItemLookUpEdit10.KeyMember = "ID"
    End Sub

    Private Sub LoadUserRoleList()
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get User Roles")
        'oSharePointTransactions.SharePointList = "UserRoleByProcess"
        'oSharePointTransactions.FieldsList.Clear()
        'oSharePointTransactions.FieldsList.Add({"ID"})
        'oSharePointTransactions.FieldsList.Add({"ProcessCode"})
        'oSharePointTransactions.FieldsList.Add({"UserAccount"})
        'oSharePointTransactions.FieldsList.Add({"UserName"})
        'oSharePointTransactions.FieldsList.Add({"UserMail"})
        'oSharePointTransactions.FieldsList.Add({"UserType"})

        'dtUserRole = oSharePointTransactions.GetItems()
        dtUserRoleList = oDataAcces.ExecuteAccessQuery("SELECT * FROM UserRoleByProcess WHERE [Código Proceso:Process Code]='LCI'").Tables(0)
        'If dtUserRole.Select("[Código Proceso:Process Code]='LCI'").Length > 0 Then
        '    dtUserRole = dtUserRole.Select("[Código Proceso:Process Code]='LCI'").CopyToDataTable
        'End If
        lueSalesExecution.Properties.DataSource = dtUserRoleList.Select("UserType='Sales Execution'").CopyToDataTable
        lueSalesExecution.Properties.DisplayMember = "UserName"
        lueSalesExecution.Properties.ValueMember = "UserName"
        lueSalesCoordination.Properties.DataSource = dtUserRoleList.Select("UserType='Sales Coordination'").CopyToDataTable
        lueSalesCoordination.Properties.DisplayMember = lueSalesExecution.Properties.DisplayMember
        lueSalesCoordination.Properties.ValueMember = lueSalesExecution.Properties.ValueMember
        lueUserAuthorization.Properties.DataSource = dtUserRoleList.Select("UserType LIKE '%Authorization'").CopyToDataTable
        lueUserAuthorization.Properties.DisplayMember = lueSalesExecution.Properties.DisplayMember
        lueUserAuthorization.Properties.ValueMember = lueSalesExecution.Properties.ValueMember
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub LoadDepotList()
        Dim dtDepot As New DataTable
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get Depots")
        'oSharePointTransactions.SharePointList = "DepotList"
        'oSharePointTransactions.FieldsList.Clear()
        'oSharePointTransactions.FieldsList.Add({"ID"})
        'oSharePointTransactions.FieldsList.Add({"PortCode"})
        'oSharePointTransactions.FieldsList.Add({"PortCode_x003a_PortName"})
        'oSharePointTransactions.FieldsList.Add({"DepotName"})
        'dtDepot = oSharePointTransactions.GetItems()
        dtDepot = oDataAcces.ExecuteAccessQuery("SELECT * FROM DepotList").Tables(0)
        gcDepots.DataSource = dtDepot
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub LoadContainerTypeList()
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get Equipment Types")
        'oSharePointTransactions.SharePointList = "ContainerTypeList"
        'oSharePointTransactions.FieldsList.Clear()
        'oSharePointTransactions.FieldsList.Add({"ID"})
        'oSharePointTransactions.FieldsList.Add({"ContainerCode"})
        'oSharePointTransactions.FieldsList.Add({"ContainerDescription"})
        'oSharePointTransactions.FieldsList.Add({"ContainerSize"})
        'dtContainerType = oSharePointTransactions.GetItems()
        dtContainerTypeList = oDataAcces.ExecuteAccessQuery("SELECT ID, ContainerCode, ContainerDescription, ContainerSize FROM ContainerTypeList").Tables(0)
        dtContainerTypeList.Columns.Add("Checked", GetType(Boolean))
        gcEquipmentTypes.DataSource = dtContainerTypeList
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub LoadLocalBenefits()
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get Local Benefits")
        'oSharePointTransactions.SharePointList = "Local Benefits"
        'oSharePointTransactions.FieldsList.Clear()
        'oSharePointTransactions.FieldsList.Add({"ID"})
        'oSharePointTransactions.FieldsList.Add({"CodigoPais"})
        'oSharePointTransactions.FieldsList.Add({"TipoEmbarque"})
        'oSharePointTransactions.FieldsList.Add({"TipoBeneficio"})
        'oSharePointTransactions.FieldsList.Add({"RazonSocial"})
        'oSharePointTransactions.FieldsList.Add({"NumeroIdentificacionTributaria"})
        'oSharePointTransactions.FieldsList.Add({"Vigencia_Desde"})
        'oSharePointTransactions.FieldsList.Add({"Vigencia_Hasta"})
        'oSharePointTransactions.FieldsList.Add({"SalesExecution"})
        'oSharePointTransactions.FieldsList.Add({"SalesCoordination"})
        'oSharePointTransactions.FieldsList.Add({"TipoConcesion"})
        'oSharePointTransactions.FieldsList.Add({"CondicionBL"})
        'oSharePointTransactions.FieldsList.Add({"TipoBL"})
        'oSharePointTransactions.FieldsList.Add({"RateAgreement"})
        'oSharePointTransactions.FieldsList.Add({"MBL_Rol"})
        'oSharePointTransactions.FieldsList.Add({"MBL_RUC"})
        'oSharePointTransactions.FieldsList.Add({"MBL_RazonSocial"})
        'oSharePointTransactions.FieldsList.Add({"HBL_Rol"})
        'oSharePointTransactions.FieldsList.Add({"HBL_RUC"})
        'oSharePointTransactions.FieldsList.Add({"HBL_RazonSocial"})
        'oSharePointTransactions.FieldsList.Add({"BillOfLading"})
        'oSharePointTransactions.FieldsList.Add({"Booking"})
        'oSharePointTransactions.FieldsList.Add({"PaisOrigen"})
        'oSharePointTransactions.FieldsList.Add({"PuertoOrigen"})
        'oSharePointTransactions.FieldsList.Add({"PaisCarga"})
        'oSharePointTransactions.FieldsList.Add({"PuertoCarga"})
        'oSharePointTransactions.FieldsList.Add({"PaisDescarga"})
        'oSharePointTransactions.FieldsList.Add({"PuertoDescarga"})
        'oSharePointTransactions.FieldsList.Add({"PaisFinal"})
        'oSharePointTransactions.FieldsList.Add({"PuertoFinal"})
        'oSharePointTransactions.FieldsList.Add({"Profit"})
        'oSharePointTransactions.FieldsList.Add({"Volumen"})
        'oSharePointTransactions.FieldsList.Add({"UsuarioAutorizador"})
        'oSharePointTransactions.FieldsList.Add({"FechaAutorizacion"})
        ''oSharePointTransactions.FieldsList.Add({"Booking"})
        ''oSharePointTransactions.FieldsList.Add({"Booking"})
        ''oSharePointTransactions.FieldsList.Add({"Booking"})
        ''oSharePointTransactions.FieldsList.Add({"Booking"})
        ''oSharePointTransactions.FieldsList.Add({"Booking"})
        'oSharePointTransactions.FieldsList.Add({"Estado"})
        'oSharePointTransactions.FieldsList.Add({"NumeroConcesion"})
        'oSharePointTransactions.FieldsList.Add({"WarningLog"})

        'dtList = oSharePointTransactions.GetItems()
        dtLocBenHeader = oDataAcces.ExecuteAccessQuery("SELECT * FROM LocalBenefits").Tables(0)
        GridControl1.DataSource = dtLocBenHeader
        FormatGrid(GridView1)
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

    Private Sub LoadConceptList(IdParent As Integer, LoadingType As String)
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get Concepts By Country")
        'oSharePointTransactions.SharePointList = "ConceptByCountryList"
        'oSharePointTransactions.FieldsList.Clear()
        'oSharePointTransactions.FieldsList.Add({"ID"})
        'oSharePointTransactions.FieldsList.Add({"CountryCode"})
        'oSharePointTransactions.FieldsList.Add({"ConceptCode"})
        'oSharePointTransactions.FieldsList.Add({"ConceptName"})
        'dtConcept = oSharePointTransactions.GetItems()
        'If CountryCode Is Nothing Then
        '    DevExpress.XtraEditors.XtraMessageBox.Show("Debe asignar el País Responsable", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    XtraTabControl1.SelectedTabPage = General
        '    Return
        'End If
        'dtConceptList = oDataAcces.ExecuteAccessQuery("SELECT * FROM ConceptByCountryList WHERE [SourceCountry:CountryCode] = '" & CountryCode & "'").Tables(0)
        If IdParent > 0 And LoadingType = "" Then
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show("Debe indicar el tipo de embarque.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            XtraTabControl1.SelectedTabPageIndex = 0
        End If
        dtConceptList = oDataAcces.ExecuteAccessQuery("SELECT * FROM ConceptByCountryList WHERE ShipmentType IS NULL OR LEFT(ShipmentType,1) = '" & Mid(LoadingType, 1, 1) & "'").Tables(0)
        RepositoryItemLookUpEdit2.DataSource = dtConceptList
        RepositoryItemLookUpEdit2.DisplayMember = "ConceptName"
        RepositoryItemLookUpEdit2.ValueMember = "ConceptCode"
        RepositoryItemLookUpEdit2.KeyMember = "ID"
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub LoadLocalBenefitsConcept(IdParent As Integer)
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get Local Benefits Concept")
        LoadConceptList(IdParent, cbeLoadingType.EditValue)
        'oSharePointTransactions.SharePointList = "LocalBenefitsConcept"
        'oSharePointTransactions.FieldsList.Clear()
        'oSharePointTransactions.FieldsList.Add({"ID"})
        'oSharePointTransactions.FieldsList.Add({"IdParent"})
        'oSharePointTransactions.FieldsList.Add({"ConceptCode"})
        'oSharePointTransactions.FieldsList.Add({"ConceptValue"})
        'oSharePointTransactions.FieldsList.Add({"ConceptCurrency"})
        'dtListDetail = oSharePointTransactions.GetItems()
        If IdParent = 0 Then
            SplashScreenManager.CloseForm(False)
            Return
        End If
        dtLocBenConcept = oDataAcces.ExecuteAccessQuery("SELECT * FROM LocalBenefitsConcept WHERE IdParent=" & IdParent.ToString).Tables(0)
            gcConcepts.DataSource = dtLocBenConcept
        GridView2.BestFitColumns()
        'ShowDetailSelected()
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub LoadLocalBenefitsCommodity(IdParent As Integer)
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get Local Benefits Commodities")
        'oSharePointTransactions.SharePointList = "LocalBenefitsCommodity"
        'oSharePointTransactions.FieldsList.Clear()
        'oSharePointTransactions.FieldsList.Add({"ID"})
        'oSharePointTransactions.FieldsList.Add({"IdParent"})
        'oSharePointTransactions.FieldsList.Add({"CommodityCode"})
        'oSharePointTransactions.FieldsList.Add({"CommodityName"})
        'dtLocBenCommodities = oSharePointTransactions.GetItems()
        If IdParent = 0 Then
            SplashScreenManager.CloseForm(False)
            Return
        End If
        dtLocBenCommodity = oDataAcces.ExecuteAccessQuery("SELECT * FROM LocalBenefitsCommodity WHERE IdParent = " & IdParent.ToString).Tables(0)
        gcCommodities.DataSource = dtLocBenCommodity
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub LoadLocalBenefitsContainer(IdParent As Integer)
        LoadContainerTypeList()
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get Local Benefits Container Types")
        'oSharePointTransactions.SharePointList = "LocalBenefitsContainer"
        'oSharePointTransactions.FieldsList.Clear()
        'oSharePointTransactions.FieldsList.Add({"ID"})
        'oSharePointTransactions.FieldsList.Add({"ContainerCode"})
        'oSharePointTransactions.FieldsList.Add({"ContainerDescription"})
        'oSharePointTransactions.FieldsList.Add({"ContainerSize"})
        'dtLocBenContainer = oSharePointTransactions.GetItems()
        If IdParent = 0 Then
            SplashScreenManager.CloseForm(False)
            Return
        End If
        dtLocBenEqpType = oDataAcces.ExecuteAccessQuery("SELECT IdParent, ContainerCode FROM LocalBenefitsContainer WHERE IdParent = " & IdParent.ToString).Tables(0)
        For r = 0 To dtContainerTypeList.Rows.Count - 1
            Dim oRow As DataRow = dtContainerTypeList.Rows(r)
            If dtLocBenEqpType.Select("ContainerCode='" & oRow("ContainerCode") & "'").Length > 0 Then
                oRow("Checked") = True
            End If
        Next
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub LocalBenefitsForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        'LoadConcepts()
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get Data Master Tables")
        LoadCountry()
        LoadCurrency()
        LoadCommodity()
        LoadPartner()
        SplashScreenManager.CloseForm(False)
        LoadUserRoleList()
        LoadDepotList()
        LoadContainerTypeList()
        'LoadConceptList()
        LoadLocalBenefits()
        'LoadLocalBenefitsDetail()

    End Sub

    Private Sub FormatGrid(oGridView As GridView)
        For c = 0 To oGridView.Columns.Count - 1
            oGridView.Columns(c).OptionsColumn.ReadOnly = True
        Next
    End Sub

    Private Sub bbiRefresh_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiRefresh.ItemClick
        Dim info As GridViewInfo = TryCast(GridView1.GetViewInfo(), GridViewInfo)
        Dim GridRowInfo As GridRowInfo = info.GetGridRowInfo(GridView1.FocusedRowHandle)
        LoadInputValidations("Refresh")
        LoadLocalBenefits()
        GridView1.MoveBy(GridRowInfo.RowHandle)
        ShowPageDataTable()

        'LoadLocalBenefitsDetail()
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
        Dim iPos As Integer = SplitContainerControl1.Size.Height - lcConcessionDetail.Size.Height
        SplitContainerControl1.SplitterPosition = iPos
    End Sub

    Private Sub GridView1_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView1.FocusedRowChanged
        'If IsDBNull(GridView1.GetFocusedRowCellValue("ID")) Then
        '    Return
        'End If
        EnableButtons(GridView1)
        'ShowDetailSelected()
        ShowPageDataTable()
    End Sub

    Private Sub SetItems(IdParent As Integer)
        'If GridView1.FocusedRowHandle < 0 Then
        '    Return
        'End If
        'RepositoryItemMemoEdit1
        Dim oControls As Control
        Dim oRow As DataRow = GridView1.GetFocusedDataRow
        Try
            'General Information
            For Each oControls In lcGeneralInfo.Controls
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
            'Concession Detail
            For Each oControls In lcConcessionDetail.Controls
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
            'If teID.Text = "" Then
            '    cbeStatus.EditValue = "Borrador"
            'End If
            'Concepts
            'Dim dtLocBenConceptTemp As New DataTable
            'dtLocBenConceptTemp = dtLocBenConcept.Clone
            'If dtLocBenConcept.Rows.Count > 0 Then
            '    If dtLocBenConcept.Select("IdParent='" & GridView1.GetFocusedRowCellValue("ID") & "'").Length > 0 Then
            '        dtLocBenConceptTemp = dtLocBenConcept.Select("IdParent='" & GridView1.GetFocusedRowCellValue("ID") & "'").CopyToDataTable
            '    End If
            'End If
            'gcConcepts.DataSource = dtLocBenConceptTemp
            ''Depots
            ''Commodities
            ''Equipment Types

        Catch ex As Exception

        End Try
    End Sub

    Private Sub ClearDataControls()
        Dim oControls As Control
        Dim oRow As DataRow = GridView1.GetFocusedDataRow
        For Each oControls In lcGeneralInfo.Controls
            If oControls.Tag Is Nothing Then
                Continue For
            End If
            DirectCast(oControls, DevExpress.XtraEditors.BaseEdit).EditValue = Nothing
        Next
        'Concession Detail
        For Each oControls In lcConcessionDetail.Controls
            If oControls.Tag Is Nothing Then
                Continue For
            End If
            DirectCast(oControls, DevExpress.XtraEditors.BaseEdit).EditValue = Nothing
        Next
        dtLocBenConcept.Rows.Clear()
        dtLocBenCommodity.Rows.Clear()
    End Sub
    Private Sub bbiSave_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSave.ItemClick
        cbeStatus.EditValue = "Ingresada"
        LoadInputValidations("Save")
        Dim drSource As DataRow = GridView1.GetFocusedDataRow
        Dim WarningText As New List(Of String)
        Dim bError As Boolean = False
        If GridView2.RowCount = 0 Then
            bError = True
            WarningText.Add("Debe asignar al menos un concepto.")
        End If
        If Not vpInputs.Validate Then
            bError = True
            For i = 0 To vpInputs.GetInvalidControls.Count - 1
                WarningText.Add(vpInputs.GetInvalidControls(i).Tag)
            Next
        End If
        If bError Then
            cbeStatus.EditValue = "Borrador"
            If DevExpress.XtraEditors.XtraMessageBox.Show("Some bugs were identified, you want to save it as a draft?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = Windows.Forms.DialogResult.No Then
                Return
            End If
        Else
            If DevExpress.XtraEditors.XtraMessageBox.Show("Are you sure you want to save this record? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                Return
            End If
        End If
        If Not IsDBNull(GridView1.GetFocusedRowCellValue("CloneFrom")) Then
            cbeStatus.EditValue = "Borrador"
        End If
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Save Local Benefits")
            oSharePointTransactions.SharePointList = "Local Benefits"
            oSharePointTransactions.ValuesList.Clear()
            If teID.Text = "" Or drSource("CodigoPais").ToString <> lueCountry.EditValue Then
                oSharePointTransactions.ValuesList.Add({"CodigoPais", lueCountry.GetColumnValue("ID")})
            End If
            If teID.Text = "" Or drSource("Tipo de Embarque").ToString <> cbeLoadingType.EditValue Then
                oSharePointTransactions.ValuesList.Add({"TipoEmbarque", cbeLoadingType.EditValue})
            End If
            'If drSource("TipoBeneficio").ToString <> cbeBenefitType.Text Then
            '    oSharePointTransactions.ValuesList.Add({"TipoBeneficio", cbeBenefitType.EditValue})
            'End If
            If teID.Text = "" Or drSource("Razón Social").ToString <> teCompanyName.Text Then
                oSharePointTransactions.ValuesList.Add({"RazonSocial", teCompanyName.Text})
            End If
            If teID.Text = "" Or drSource("NumeroIdentificacionTributaria").ToString <> teTaxNumber.Text Then
                oSharePointTransactions.ValuesList.Add({"NumeroIdentificacionTributaria", teTaxNumber.Text})
            End If
            If teID.Text = "" Or drSource("Vigencia Desde").ToString <> deValidityFrom.DateTime.ToString Then
                oSharePointTransactions.ValuesList.Add({"Vigencia_Desde", deValidityFrom.DateTime.ToShortDateString})
            End If
            If teID.Text = "" Or drSource("Vigencia Hasta").ToString <> deValidityTo.DateTime.ToString Then
                oSharePointTransactions.ValuesList.Add({"Vigencia_Hasta", deValidityTo.DateTime.ToShortDateString})
            End If
            If Not lueSalesExecution.GetColumnValue("ID") Is Nothing Then
                If teID.Text = "" Or drSource("SalesExecution").ToString <> lueSalesExecution.GetColumnValue("ID").ToString Then
                    oSharePointTransactions.ValuesList.Add({"SalesExecution", lueSalesExecution.GetColumnValue("ID").ToString})
                End If
            End If
            If Not lueSalesCoordination.GetColumnValue("ID") Is Nothing Then
                If teID.Text = "" Or drSource("SalesCoordination").ToString <> lueSalesCoordination.Text Then
                    oSharePointTransactions.ValuesList.Add({"SalesCoordination", lueSalesCoordination.GetColumnValue("ID")})
                End If
            End If
            If Not cbeConcessionType.EditValue Is Nothing Then
                If teID.Text = "" Or drSource("Tipo de Concesión").ToString <> cbeConcessionType.Text Then
                    oSharePointTransactions.ValuesList.Add({"TipoConcesion", cbeConcessionType.EditValue})
                End If
            End If
            If Not cbeBlType.EditValue Is Nothing Then
                If teID.Text = "" Or drSource("TipoBL").ToString <> cbeBlType.Text Then
                    oSharePointTransactions.ValuesList.Add({"TipoBL", cbeBlType.EditValue})
                End If
            End If
            If Not cbeBlContition.EditValue Is Nothing Then
                If teID.Text = "" Or drSource("Condición de BL").ToString <> cbeBlContition.Text Then
                    oSharePointTransactions.ValuesList.Add({"CondicionBL", cbeBlContition.EditValue})
                End If
            End If
            If teID.Text = "" Or drSource("Rate Agreement").ToString <> teRateAgreement.Text Then
                oSharePointTransactions.ValuesList.Add({"RateAgreement", teRateAgreement.Text})
            End If
            If Not cbeMblRol.EditValue Is Nothing Then
                If teID.Text = "" Or drSource("MBL - Rol").ToString <> cbeMblRol.Text Then
                    oSharePointTransactions.ValuesList.Add({"MBL_Rol", cbeMblRol.EditValue})
                End If
            End If
            If teID.Text = "" Or drSource("MBL - RUC").ToString <> lueMblNit.Text Then
                oSharePointTransactions.ValuesList.Add({"MBL_RUC", lueMblNit.Text})
            End If
            If teID.Text = "" Or drSource("MBL - Razón Social").ToString <> teMblCompanyName.Text Then
                oSharePointTransactions.ValuesList.Add({"MBL_RazonSocial", teMblCompanyName.Text})
            End If
            If teID.Text = "" Or drSource("HBL - Rol").ToString <> cbeHblRol.Text Then
                oSharePointTransactions.ValuesList.Add({"HBL_Rol", cbeHblRol.EditValue})
            End If
            If Not cbeHblRol.EditValue Is Nothing Then
                If teID.Text = "" Or drSource("HBL - RUC").ToString <> lueHblNit.Text Then
                    oSharePointTransactions.ValuesList.Add({"HBL_RUC", lueHblNit.Text})
                End If
            End If
            If teID.Text = "" Or drSource("HBL - Razón Social").ToString <> teHblCompanyName.Text Then
                oSharePointTransactions.ValuesList.Add({"HBL_RazonSocial", teHblCompanyName.Text})
            End If
            If teID.Text = "" Or drSource("BillOfLading").ToString <> teBillOfLading.Text Then
                oSharePointTransactions.ValuesList.Add({"BillOfLading", teBillOfLading.Text})
            End If
            If teID.Text = "" Or drSource("Booking").ToString <> teBooking.Text Then
                oSharePointTransactions.ValuesList.Add({"Booking", teBooking.Text})
            End If
            If teID.Text = "" Or drSource("PaisOrigen").ToString <> lueOriginCountry.EditValue Then
                oSharePointTransactions.ValuesList.Add({"PaisOrigen", lueOriginCountry.EditValue})
            End If
            If Not lueOriginPort.GetColumnValue("ID") Is Nothing Then
                If teID.Text = "" Or drSource("PuertoOrigen").ToString <> lueOriginPort.Text Then
                    oSharePointTransactions.ValuesList.Add({"PuertoOrigen", lueOriginPort.GetColumnValue("ID")})
                End If
            End If
            If teID.Text = "" Or drSource("PaisCarga").ToString <> lueLoadCountry.EditValue Then
                oSharePointTransactions.ValuesList.Add({"PaisCarga", lueLoadCountry.EditValue})
            End If
            If Not lueLoadPort.GetColumnValue("ID") Is Nothing Then
                If teID.Text = "" Or drSource("PuertoCarga").ToString <> lueLoadPort.Text Then
                    oSharePointTransactions.ValuesList.Add({"PuertoCarga", lueLoadPort.GetColumnValue("ID")})
                End If
            End If
            If teID.Text = "" Or drSource("PaisDescarga").ToString <> lueDischargeCountry.EditValue Then
                oSharePointTransactions.ValuesList.Add({"PaisDescarga", lueDischargeCountry.EditValue})
            End If
            If Not lueDischargePort.GetColumnValue("ID") Is Nothing Then
                If teID.Text = "" Or drSource("PuertoDescarga").ToString <> lueDischargePort.Text Then
                    oSharePointTransactions.ValuesList.Add({"PuertoDescarga", lueDischargePort.GetColumnValue("ID")})
                End If
            End If
            If teID.Text = "" Or drSource("PaisFinal").ToString <> lueFinalCountry.EditValue Then
                oSharePointTransactions.ValuesList.Add({"PaisFinal", lueFinalCountry.EditValue})
            End If
            If Not lueFinalPort.GetColumnValue("ID") Is Nothing Then
                If teID.Text = "" Or drSource("PuertoFinal").ToString <> lueFinalPort.Text Then
                    oSharePointTransactions.ValuesList.Add({"PuertoFinal", lueFinalPort.GetColumnValue("ID")})
                End If
            End If
            'If drSource("PrecintoBASC").ToString <> teSealBasc.Text Then
            '    oSharePointTransactions.ValuesList.Add({"PrecintoBASC", teSealBasc.Text})
            'End If
            If teID.Text = "" Or drSource("Profit").ToString <> teProfit.Text Then
                oSharePointTransactions.ValuesList.Add({"Profit", teProfit.Text})
            End If
            If teID.Text = "" Or drSource("Volumen").ToString <> teVolumen.Text Then
                oSharePointTransactions.ValuesList.Add({"Volumen", teVolumen.Text})
            End If
            If teID.Text = "" Or drSource("UsuarioAutorizador").ToString <> lueUserAuthorization.Text Then
                oSharePointTransactions.ValuesList.Add({"UsuarioAutorizador", lueUserAuthorization.GetColumnValue("ID")})
            End If
            'If drSource("FechaAutorizacion") <> deAuthorizationDate.Text Then
            '    oSharePointTransactions.ValuesList.Add({"FechaAutorizacion", deAuthorizationDate.DateTime.ToShortDateString})
            'End If
            If teID.Text = "" Or drSource("Estado").ToString <> cbeStatus.Text Then
                oSharePointTransactions.ValuesList.Add({"Estado", cbeStatus.EditValue})
            End If
            If teID.Text = "" Or drSource("NumeroConcesion").ToString <> teConcessionNumber.Text Then
                oSharePointTransactions.ValuesList.Add({"NumeroConcesion", teConcessionNumber.Text})
            End If
            If teID.Text = "" Or Not IsDBNull(GridView1.GetFocusedRowCellValue("CloneFrom")) Then
                oSharePointTransactions.ValuesList.Add({"CloneFrom", GridView1.GetFocusedRowCellValue("CloneFrom")})
            End If
            If WarningText.Count > 0 Then
                'oSharePointTransactions.ValuesList.Add({"WarningLog", WarningText.ToArray})
            End If
            'Save Local Benefits (Header)
            If oSharePointTransactions.ValuesList.Count > 0 Then
                If teID.Text = "" Then
                    oSharePointTransactions.InsertItem()
                Else
                    oSharePointTransactions.UpdateItem(teID.Text)
                    Dim sValues As String = ""
                    For v = 0 To oSharePointTransactions.ValuesList.Count - 1
                        sValues += IIf(v = 0, "[", ",[") & oSharePointTransactions.ValuesList(v)(0) & "]='" & oSharePointTransactions.ValuesList(v)(1) & "'"
                    Next
                    'oDataAcces.UpdateAccess("LocalBenefits", "ID=" & teID.Text, sValues)
                End If
            End If
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        ' Save Concepts
        If GridView2.RowCount > 0 Then
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Save Local Benefits Concepts")
            oSharePointTransactions.SharePointList = "LocalBenefitsConcept"
            For r = 0 To dtLocBenConcept.Rows.Count - 1
                If dtLocBenConcept.Rows(r).RowState = DataRowState.Deleted Then
                    dtLocBenConcept.Rows(r).RejectChanges()
                    oSharePointTransactions.DeleteItem(dtLocBenConcept.Rows(r)("ID").ToString)
                    Continue For
                End If
                oSharePointTransactions.ValuesList.Clear()
                Dim drSourceConcept As DataRow = GridView2.GetDataRow(r)
                Try
                    oSharePointTransactions.ValuesList.Add({"IdParent", drSource("ID").ToString})
                    oSharePointTransactions.ValuesList.Add({"ConceptCode", GetValueByField("Concept", drSourceConcept("ConceptCode:Código Concepto"))})
                    oSharePointTransactions.ValuesList.Add({"ConceptCurrency", GetValueByField("Currency", drSourceConcept("ConceptCurrency:Código Moneda"))})
                    oSharePointTransactions.ValuesList.Add({"ConceptValue", drSourceConcept("ConceptValue").ToString})

                    If dtLocBenConcept.Rows(r).RowState = DataRowState.Added Then 'IsDBNull(drSourceConcept("ID")) Then
                        oSharePointTransactions.InsertItem()
                        'oDataAcces.InsertIntoAccess("LocalBenefitsConcept", drSourceConcept)
                    ElseIf Not dtLocBenConcept.Rows(r).RowState = DataRowState.Unchanged Then
                        oSharePointTransactions.UpdateItem(drSourceConcept("ID").ToString)
                        'Dim sValues As String = ""
                        'For v = 0 To oSharePointTransactions.ValuesList.Count - 1
                        '    sValues += IIf(v = 0, "[", ",[") & oSharePointTransactions.ValuesList(v)(0) & "]='" & oSharePointTransactions.ValuesList(v)(1) & "'"
                        'Next
                        'oDataAcces.UpdateAccess("LocalBenefitsConcept", "ID=" & drSourceConcept("ID").ToString, sValues)
                    End If
                    'If oSharePointTransactions.ValuesList.Count > 0 Then
                    '    LoadLocalBenefitsDetail()
                    '    GridView2.MoveLast()
                    'End If
                Catch ex As Exception
                    SplashScreenManager.CloseForm(False)
                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Next
        End If

        ' Save Commodities
        If GridView4.RowCount > 0 Then
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Save Local Benefits Commodities")
            oSharePointTransactions.SharePointList = "LocalBenefitsCommodity"
            For r = 0 To dtLocBenCommodity.Rows.Count - 1
                If dtLocBenCommodity.Rows(r).RowState = DataRowState.Deleted Then
                    dtLocBenCommodity.Rows(r).RejectChanges()
                    'oSharePointTransactions.DeleteItem(dtLocBenCommodity.Rows(r)("ID").ToString)
                    oDataAcces.DeleteAccess("LocalBenefitsCommodity", "ID=" & dtLocBenCommodity.Rows(r)("ID").ToString)
                    Continue For
                End If
                oSharePointTransactions.ValuesList.Clear()
                Dim drSourceCommodity As DataRow = GridView4.GetDataRow(r)
                Try
                    oSharePointTransactions.ValuesList.Add({"IdParent", drSource("ID").ToString})
                    oSharePointTransactions.ValuesList.Add({"CommodityCode", drSourceCommodity("CommodityCode")})
                    oSharePointTransactions.ValuesList.Add({"CommodityName", drSourceCommodity("CommodityName")})
                    If dtLocBenCommodity.Rows(r).RowState = DataRowState.Added Then 'IsDBNull(drSourceConcept("ID")) Then
                        'oSharePointTransactions.InsertItem()
                        oDataAcces.InsertIntoAccess("LocalBenefitsCommodity", drSourceCommodity)
                    ElseIf Not dtLocBenCommodity.Rows(r).RowState = DataRowState.Unchanged Then
                        'oSharePointTransactions.UpdateItem(drSourceCommodity("ID").ToString)
                        Dim sValues As String = ""
                        For v = 0 To oSharePointTransactions.ValuesList.Count - 1
                            sValues += IIf(v = 0, "[", ",[") & oSharePointTransactions.ValuesList(v)(0) & "]='" & oSharePointTransactions.ValuesList(v)(1) & "'"
                        Next
                        oDataAcces.UpdateAccess("LocalBenefitsCommodity", "ID=" & drSourceCommodity("ID").ToString, sValues)
                    End If
                Catch ex As Exception
                    SplashScreenManager.CloseForm(False)
                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Next
        End If

        ' Save Equipment Types
        If GridView5.RowCount > 0 Then
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Save Local Benefits Equipment Types")
            oSharePointTransactions.SharePointList = "LocalBenefitsContainer"
            For r = 0 To GridView5.RowCount - 1
                'If dtLocBenEqpType.Rows(r).RowState = DataRowState.Modified Then
                '    dtLocBenEqpType.Rows(r).RejectChanges()
                '    'oSharePointTransactions.DeleteItem(dtLocBenCommodity.Rows(r)("ID").ToString)
                '    oDataAcces.DeleteAccess("LocalBenefitsContainer", "ID=" & dtLocBenEqpType.Rows(r)("ID").ToString)
                '    Continue For
                'End If
                oSharePointTransactions.ValuesList.Clear()
                Dim drSourceEqpType As DataRow = GridView5.GetDataRow(r)
                Try
                    oSharePointTransactions.ValuesList.Add({"IdParent", drSource("ID").ToString})
                    oSharePointTransactions.ValuesList.Add({"ContainerCode", drSourceEqpType("ContainerCode")})
                    If dtContainerTypeList.Rows(r).RowState = DataRowState.Modified Then 'IsDBNull(drSourceConcept("ID")) Then
                        '    'oSharePointTransactions.InsertItem()
                        '    oDataAcces.InsertIntoAccess("LocalBenefitsContainer", drSourceEqpType)
                        'Else
                        '    'oSharePointTransactions.UpdateItem(drSourceCommodity("ID").ToString)
                        '    Dim sValues As String = ""
                        '    For v = 0 To oSharePointTransactions.ValuesList.Count - 1
                        '        sValues += IIf(v = 0, "[", ",[") & oSharePointTransactions.ValuesList(v)(0) & "]='" & oSharePointTransactions.ValuesList(v)(1) & "'"
                        '    Next
                        '    oDataAcces.UpdateAccess("LocalBenefitsContainer", "ID=" & drSourceEqpType("ID").ToString, sValues)
                        If drSourceEqpType("Checked") Then
                            If oDataAcces.ExecuteAccessQuery("SELECT * FROM LocalBenefitsContainer WHERE IdParent=" & drSource("ID").ToString & " AND [ContainerCode]='" & drSourceEqpType("ContainerCode") & "'").Tables(0).Rows.Count = 0 Then
                                Dim sFields As String = Nothing
                                Dim sValues As String = Nothing
                                For d = 0 To oSharePointTransactions.ValuesList.Count - 1
                                    sFields += IIf(d = 0, "", ",") & oSharePointTransactions.ValuesList(d)(0)
                                    sValues += IIf(d = 0, "'", ",'") & oSharePointTransactions.ValuesList(d)(1) & "'"
                                Next
                                oDataAcces.InsertAccess("LocalBenefitsContainer", sFields, sValues)
                            End If
                        Else
                            If oDataAcces.ExecuteAccessQuery("SELECT * FROM LocalBenefitsContainer WHERE IdParent=" & drSource("ID").ToString & " AND [ContainerCode]='" & drSourceEqpType("ContainerCode") & "'").Tables(0).Rows.Count > 0 Then
                                oDataAcces.DeleteAccess("LocalBenefitsContainer", "IdParent = " & drSource("ID").ToString & " And [ContainerCode]='" & drSourceEqpType("ContainerCode") & "'")
                            End If
                        End If
                    End If
                Catch ex As Exception
                    SplashScreenManager.CloseForm(False)
                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Next
        End If
        SplashScreenManager.CloseForm(False)
        bbiRefresh.PerformClick()
    End Sub

    Private Function GetValueByField(FieldName As String, FieldValue As String) As String
        Dim sResult As String = ""
        If FieldName = "Concept" Then
            sResult = dtConceptList.Select("ConceptCode='" & FieldValue & "'")(0)("ID")
        End If
        If FieldName = "Currency" Then
            sResult = dtCurrencyList.Select("CurrencyCode='" & FieldValue & "'")(0)("ID")
        End If
        Return sResult
    End Function
    Private Sub bbiNew_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiNew.ItemClick
        XtraTabControl1.SelectedTabPageIndex = 0
        ClearDataControls()
        GridView1.AddNewRow()
        SplitContainerControl1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Both
        Dim iPos As Integer = SplitContainerControl1.Size.Height - lcConcessionDetail.Size.Height
        SplitContainerControl1.SplitterPosition = iPos
    End Sub

    Private Sub lueOriginCountry_EditValueChanged(sender As Object, e As EventArgs) Handles lueOriginCountry.EditValueChanged
        LoadPort(lueOriginPort, lueOriginCountry.EditValue)
    End Sub

    Private Sub GridView2_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView2.FocusedRowChanged
        GridView2.SetFocusedRowCellValue("IdParent", GridView1.GetFocusedRowCellValue("ID"))
    End Sub

    Private Sub EnableButtons(oGridview As GridView)
        bbiClone.Enabled = False
        If IsDBNull(oGridview.GetFocusedRowCellValue("Estado")) Then
            Return
        End If
        If oGridview.GetFocusedRowCellValue("Estado") <> "Borrador" Then
            bbiClone.Enabled = True
        End If
    End Sub
    Private Sub GridView4_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView4.FocusedRowChanged
        GridView4.SetFocusedRowCellValue("IdParent", GridView1.GetFocusedRowCellValue("ID"))
    End Sub

    Private Sub bbiClone_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClone.ItemClick
        If GridView1.FocusedRowHandle < 0 Then
            Return
        End If
        bbiClone.Enabled = False
        Dim iPos As Integer = GridView1.FocusedRowHandle
        Dim CloneFrom As Integer = GridView1.GetFocusedRowCellValue("ID")
        GridView1.AddNewRow()
        For c = 0 To GridView1.Columns.Count - 1
            GridView1.SetFocusedRowCellValue(GridView1.Columns(c).FieldName, GridView1.GetRowCellValue(iPos, GridView1.Columns(c)))
        Next
        GridView1.SetFocusedRowCellValue("ID", "")
        GridView1.SetFocusedRowCellValue("Estado", "Borrador")
        GridView1.SetFocusedRowCellValue("CloneFrom", CloneFrom)
        GridView1.OptionsNavigation.EndUpdate()
        teID.EditValue = ""
        cbeStatus.EditValue = "Borrador"
        Validate()
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

    Private Sub lueMblNit_EditValueChanged(sender As Object, e As EventArgs) Handles lueMblNit.EditValueChanged
        teMblCompanyName.EditValue = lueMblNit.GetColumnValue("PartnerName")
    End Sub

    Private Sub lueHblNit_EditValueChanged(sender As Object, e As EventArgs) Handles lueHblNit.EditValueChanged
        teHblCompanyName.EditValue = lueHblNit.GetColumnValue("PartnerName")
    End Sub

    Private Sub XtraTabControl1_SelectedPageChanged(sender As Object, e As DevExpress.XtraTab.TabPageChangedEventArgs) Handles XtraTabControl1.SelectedPageChanged
        ShowPageDataTable()
    End Sub

    Private Sub ShowPageDataTable()
        Dim IdParent As Integer = 0
        XtraTabControl1.TabPages(2).PageEnabled = True
        XtraTabControl1.TabPages(4).PageEnabled = True
        XtraTabControl1.TabPages(5).PageEnabled = True
        If IsDBNull(GridView1.GetFocusedRowCellValue("ID")) Then
            XtraTabControl1.TabPages(2).PageEnabled = False
            XtraTabControl1.TabPages(4).PageEnabled = False
            XtraTabControl1.TabPages(5).PageEnabled = False
            Return
        End If
        IdParent = GridView1.GetFocusedRowCellValue("ID")
        If XtraTabControl1.SelectedTabPage.Name.Contains({"General", "Detail"}) Then
            SetItems(IdParent)
            LoadLocalBenefitsConcept(IdParent)
        End If
        If XtraTabControl1.SelectedTabPage.Name = "Concept" Then
            LoadLocalBenefitsConcept(IdParent)
        End If
        If XtraTabControl1.SelectedTabPage.Name = "Depot" Then
            'LoadDepotList(IdParent)
        End If
        If XtraTabControl1.SelectedTabPage.Name = "Commodity" Then
            LoadLocalBenefitsCommodity(IdParent)
        End If
        If XtraTabControl1.SelectedTabPage.Name = "EquipmentType" Then
            LoadLocalBenefitsContainer(IdParent)
        End If
    End Sub
    Friend Function RowSelectedCount(oGridView As GridView) As Integer
        Dim iChecked As Integer = 0
        For i = 0 To oGridView.RowCount - 1
            If IsDBNull(oGridView.GetRowCellValue(i, "Checked")) Then
                Continue For
            End If
            If oGridView.GetRowCellValue(i, "Checked") Then
                iChecked += 1
            End If
        Next
        Return iChecked
    End Function

    Private Sub SelectRowsByType(oGridView As GridView, SelectType As Integer)
        For i = 0 To oGridView.RowCount - 1
            Dim row As DataRow = oGridView.GetDataRow(i)
            If IsDBNull(row("Checked")) Then
                row("Checked") = False
            End If
            If SelectType = 0 Then
                row("Checked") = True
            Else
                row("Checked") = True
            End If
            If SelectType = 1 Then
                row("Checked") = False
            End If
            If SelectType = 2 Then
                If row("Checked") Then
                    row("Checked") = False
                Else
                    row("Checked") = True
                End If
            End If
            Validate()
        Next
    End Sub

    Private Sub SeleccionaTodosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SeleccionaTodosToolStripMenuItem.Click
        SelectRowsByType(GridView5, 0)
    End Sub

    Private Sub DeseleccionaTodosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeseleccionaTodosToolStripMenuItem.Click
        SelectRowsByType(GridView5, 1)
    End Sub

    Private Sub InvertirSelecciónToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InvertirSelecciónToolStripMenuItem.Click
        SelectRowsByType(GridView5, 2)
    End Sub

    Private Sub LoadInputValidations(ValType As String)
        Validate()
        Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

        containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
        containsValidationRule.ErrorText = "Assign value."
        containsValidationRule.ErrorType = ErrorType.Critical

        Dim customValidationRule As New CustomValidationRule()
        customValidationRule.ErrorText = "Required value."
        customValidationRule.ErrorType = ErrorType.Critical

        vpInputs.SetValidationRule(Me.teRateAgreement, Nothing)
        vpInputs.SetValidationRule(Me.lueCountry, Nothing)
        vpInputs.SetValidationRule(Me.cbeLoadingType, Nothing)
        vpInputs.SetValidationRule(Me.deValidityFrom, Nothing)
        vpInputs.SetValidationRule(Me.deValidityTo, Nothing)
        vpInputs.SetValidationRule(Me.cbeConcessionType, Nothing)
        vpInputs.SetValidationRule(Me.teProfit, Nothing)
        vpInputs.SetValidationRule(Me.teVolumen, Nothing)
        vpInputs.SetValidationRule(Me.lueSalesExecution, Nothing)
        vpInputs.SetValidationRule(Me.lueSalesCoordination, Nothing)

        If ValType = "Save" Then
            If cbeConcessionType.Text = "Por RA" Then
                vpInputs.SetValidationRule(Me.teRateAgreement, customValidationRule)
            End If
            vpInputs.SetValidationRule(Me.lueCountry, customValidationRule)
            vpInputs.SetValidationRule(Me.cbeLoadingType, customValidationRule)
            vpInputs.SetValidationRule(Me.deValidityFrom, customValidationRule)
            vpInputs.SetValidationRule(Me.deValidityTo, customValidationRule)
            vpInputs.SetValidationRule(Me.cbeConcessionType, customValidationRule)
            vpInputs.SetValidationRule(Me.teProfit, customValidationRule)
            vpInputs.SetValidationRule(Me.teVolumen, customValidationRule)
            vpInputs.SetValidationRule(Me.lueSalesExecution, customValidationRule)
            vpInputs.SetValidationRule(Me.lueSalesCoordination, customValidationRule)
        End If

    End Sub

End Class