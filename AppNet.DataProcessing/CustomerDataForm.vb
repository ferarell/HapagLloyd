Imports DevExpress.XtraSplashScreen
Imports Microsoft.Office.Interop
Imports System.Runtime.InteropServices
Imports System.Reflection

Public Class CustomerDataForm
    'Dim oSharePointTransactions As New SharePointListTransactions
    Dim dtList, dtCoordinator As New DataTable
    Dim oAppService As New AppService.HapagLloydServiceClient

    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        'oSharePointTransactions.SharePointUrl = My.Settings.SharePoint_Url
    End Sub

    Private Sub beDataSource_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beDataSource.Properties.ButtonClick
        Dim FileNames() As String
        OpenFileDialog1.Filter = "SAP Source File (*.xls*)|*.xls*"
        OpenFileDialog1.FileName = ""
        'OpenFileDialog1.InitialDirectory = IIf(My.Settings.DataTargetPath <> "", My.Settings.DataTargetPath, "")
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            FileNames = OpenFileDialog1.FileNames
            beDataSource.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub ShowAllData()
        GridControl1.DataSource = Nothing
        dtList = oAppService.ExecuteSQL("EXEC ntf.upGetAllContactsByFilters 'PE', 'I'").Tables(0)
        If dtList.Rows.Count = 0 Then
            Return
        End If
        GridControl1.DataSource = dtList
        GridView1.BestFitColumns()
    End Sub

    Private Sub DataSourceImport()
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        Try
            Dim dtExcel As New DataTable
            dtExcel = LoadExcel(beDataSource.Text, "{0}").Tables(0)
            If dtExcel.Rows.Count > 0 Then
                UpdatePartners(dtExcel)
            End If
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub UpdatePartners(dtSource As DataTable)
        Dim dtResult As New DataTable
        dtResult = oAppService.ExecuteSQL("SELECT TOP 0 * FROM ntf.Partners").Tables(0)
        For r = 0 To dtSource.Rows.Count - 1
            Dim oRow As DataRow = dtSource.Rows(r)
            If Len(oRow("Group key").trim) = 0 Then
                Continue For
            End If
            Try
                dtResult.Rows.Clear()
                dtResult.Rows.Add()
                Dim iPos As Integer = dtResult.Rows.Count - 1
                dtResult.Rows(iPos)("PartnerType") = "C"
                dtResult.Rows(iPos)("PartnerCode") = oRow("Customer")
                dtResult.Rows(iPos)("PartnerName") = oRow("Name 1")
                dtResult.Rows(iPos)("MatchCode") = GetMatchCode(oRow("Group key"))
                dtResult.Rows(iPos)("TaxNumber") = oRow("Tax Number 1")
                dtResult.Rows(iPos)("SendingStatus") = DBNull.Value
                dtResult.Rows(iPos)("SendingDate") = DBNull.Value
                dtResult.Rows(iPos)("CreatedBy") = My.User.Name
                dtResult.Rows(iPos)("CreatedDate") = Today
                dtResult.Rows(iPos)("UpdatedBy") = DBNull.Value
                dtResult.Rows(iPos)("UpdatedDate") = DBNull.Value
                oAppService.InsertPartners(dtResult)
            Catch ex As Exception

            End Try
        Next

    End Sub

    Private Sub bbiImport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiImport.ItemClick
        DataSourceImport()
    End Sub

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiShowAll.ItemClick
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Get All Partners")
            ShowAllData()
            SplashScreenManager.CloseForm(False)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(GridControl1)
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

End Class