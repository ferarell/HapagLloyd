Imports DevExpress.XtraSplashScreen

Public Class LocalChargesInvoicingForm
    Dim oAppService As New AppService.HapagLloydServiceClient

    Private Sub LocalChargesInvoicingFormvb_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        deDateFrom.EditValue = Now.AddDays(-30)
        deDateTo.EditValue = Now
        LoadCountry
    End Sub

    Private Sub LoadCountry()
        Dim dtCountry As New DataTable
        dtCountry = oAppService.ExecuteSQL("SELECT * FROM dbo.Country").Tables(0)
        lueCountry.Properties.DataSource = dtCountry
        lueCountry.Properties.DisplayMember = "CountryDescription"
        lueCountry.Properties.ValueMember = "CountryCode"
    End Sub

    Private Sub bbiSearch_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSearch.ItemClick
        Dim dtQuery As New DataTable
        Dim DateFrom As String = ""
        Dim DateTo As String = ""
        Dim Country As String = ""
        Validate()
        If deDateFrom.EditValue IsNot Nothing Then
            DateFrom = "'" & Format(deDateFrom.EditValue, "yyyy-MM-dd") & "'"
        End If
        If deDateTo.EditValue IsNot Nothing Then
            DateTo = ",'" & Format(deDateTo.EditValue, "yyyy-MM-dd") & "'"
        End If
        If lueCountry.EditValue IsNot Nothing Then
            If lueCountry.EditValue.ToString <> "" Then
                Country = ",'" & lueCountry.EditValue & "'"
            End If
        End If
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Getting data rows ")
            'dtQuery = oAppService.ExecuteSQL("EXEC ctr.upGetLocalChargeInvoicing " & DateFrom & DateTo & Country).Tables(0)
            dtQuery = oAppService.GetLocalChargeInvoicing(deDateFrom.EditValue, deDateTo.EditValue, lueCountry.EditValue)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
        End Try
        SplashScreenManager.CloseForm(False)
        GridControl1.DataSource = dtQuery
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(GridControl1)
    End Sub
End Class