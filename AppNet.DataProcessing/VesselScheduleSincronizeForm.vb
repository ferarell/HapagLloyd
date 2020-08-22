Imports DevExpress.XtraSplashScreen

Public Class VesselScheduleSincronizeForm
    Dim oSharePointTransactions As New SharePointListTransactions
    Dim oAppService As New AppService.HapagLloydServiceClient
    Dim dtList As New DataTable

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub VesselScheduleSincronizeForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            oSharePointTransactions.SharePointUrl = My.Settings.SharePoint_Url
            oSharePointTransactions.SharePointList = "ScheduleVoyageList"
            oSharePointTransactions.FieldsList.Clear()
            oSharePointTransactions.FieldsList.Add({"POL"})
            oSharePointTransactions.FieldsList.Add({"DPVOYAGE"})
            oSharePointTransactions.FieldsList.Add({"VESSEL_NAME"})
            oSharePointTransactions.FieldsList.Add({"SCHEDULE"})
            oSharePointTransactions.FieldsList.Add({"SERVICE"})
            oSharePointTransactions.FieldsList.Add({"DOC_CLOSE"})
            oSharePointTransactions.FieldsList.Add({"ETA"})
            oSharePointTransactions.FieldsList.Add({"ETD"})
            dtList = oSharePointTransactions.GetItems()
            GridControl1.DataSource = dtList
            SplashScreenManager.CloseForm(False)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Sincronize()
        Dim dtSource As New DataTable
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            dtSource = oAppService.ExecuteSQL("SELECT * FROM tck.ScheduleVoyage WHERE LEFT(POL,2)='PE' AND ETA >= DATEADD(day,-180, GETDATE())").Tables(0)
            For r = 0 To dtSource.Rows.Count - 1
                Dim oDPVoyage, oPol As String
                oDPVoyage = dtSource.Rows(r)("DPVOYAGE")
                oPol = dtSource.Rows(r)("POL")
                If dtList.Select("DPVOYAGE = '" & oDPVoyage & "' AND POL = '" & oPol & "'").Length = 0 Then
                    oSharePointTransactions.ValuesList.Clear()
                    oSharePointTransactions.ValuesList.Add({"POL", dtSource.Rows(r)("POL")})
                    oSharePointTransactions.ValuesList.Add({"DPVOYAGE", dtSource.Rows(r)("DPVOYAGE")})
                    oSharePointTransactions.ValuesList.Add({"VESSEL_NAME", dtSource.Rows(r)("VESSEL_NAME")})
                    oSharePointTransactions.ValuesList.Add({"SCHEDULE", dtSource.Rows(r)("SCHEDULE")})
                    oSharePointTransactions.ValuesList.Add({"SERVICE", dtSource.Rows(r)("SERVICE")})
                    If dtSource.Rows(r)("DOC_CLOSE").ToString <> "" Then
                        oSharePointTransactions.ValuesList.Add({"DOC_CLOSE", dtSource.Rows(r)("DOC_CLOSE")})
                    End If
                    oSharePointTransactions.ValuesList.Add({"ETA", dtSource.Rows(r)("ETA")})
                    oSharePointTransactions.ValuesList.Add({"ETD", dtSource.Rows(r)("ETD")})
                    oSharePointTransactions.InsertItem()
                End If
            Next
            bbiShowAll.PerformClick()
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The process has been completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bbiSincronize_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSincronize.ItemClick
        Sincronize()
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


End Class