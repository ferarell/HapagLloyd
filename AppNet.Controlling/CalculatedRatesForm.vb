Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.Collections

Public Class CalculatedRatesForm
    Dim dtSource As New DataTable
    Dim iPrc As Integer = 0
    Dim oAppService As New AppService.HapagLloydServiceClient

    Private Sub bbiProcesss_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesss.ItemClick
        Dim aResult As New ArrayList
        LoadInputValidations()
        If Not vpInputs.Validate Then
            Return
        End If
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            If LoadDataSources() Then
                If dtSource.Rows.Count > 0 Then
                    Dim oParams, oValues As New ArrayList
                    oParams.Add("@User")
                    oValues.Add(My.User.Name)
                    SplashScreenManager.Default.SetWaitFormDescription("Updating Master Table of Calculated Rates")
                    aResult.AddRange(oAppService.UpdatingUsingTableAsParameter("ctr.spCalculatedRatesUpdate", oParams.ToArray, oValues.ToArray, dtSource))
                    'gcMainData.DataSource = dtResult
                End If
            End If
            SplashScreenManager.CloseForm(False)
            If Convert.ToInt32(aResult(0)) = 1 Then
                bbiShowAll.PerformClick()
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The process has been completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, aResult(1), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
        Finally
            SplashScreenManager.CloseForm(False)
        End Try
    End Sub

    Friend Function LoadDataSources() As Boolean
        Dim bResult As Boolean = True
        Dim dtBridge As New DataTable
        dtSource.Rows.Clear()
        Dim dtMainName As String = ""
        For i = 0 To OpenFileDialog1.FileNames.Count - 1
            If OpenFileDialog1.FileNames(i).ToUpper.Contains(".XLS") Then
                SplashScreenManager.Default.SetWaitFormDescription("Loading Data Sources...")
                dtBridge = LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0)
                If dtBridge.Rows.Count = 0 Then
                    Continue For
                End If
                If dtBridge.Rows(0)(5).ToString.Contains({"I", "E"}) Then
                    'Acuerdos Comerciales (TRAMARSA)
                    SplashScreenManager.Default.SetWaitFormDescription("Loading Data of Calculated Rates (Tramarsa)")
                    dtSource = LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0)
                    Continue For
                End If
            End If
        Next
        Return bResult
    End Function

    Private Function GetBookingByBL(BlNo As String) As String
        Dim sResult As String = ""
        Dim dtQuery As New DataTable
        dtQuery = oAppService.ExecuteSQL("select Booking from WebFocus" & rgCargoType.EditValue & " where BLNO='" & BlNo & "'").Tables(0)
        If dtQuery.Rows.Count > 0 Then
            sResult = dtQuery.Rows(0)("Booking")
        End If
        Return sResult
    End Function

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(gcMainData)
    End Sub

    Private Sub beDataSource_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beDataSource.Properties.ButtonClick
        Dim FileNames() As String
        OpenFileDialog1.Filter = "Source File (*.xls*)|*.xls*"
        OpenFileDialog1.FileName = ""
        'OpenFileDialog1.InitialDirectory = My.Settings.SDRDataSourcePath
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            FileNames = OpenFileDialog1.FileNames
            beDataSource.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub LoadInputValidations()
        Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

        containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
        containsValidationRule.ErrorText = "Assign value."
        containsValidationRule.ErrorType = ErrorType.Critical

        Dim customValidationRule As New CustomValidationRule()
        customValidationRule.ErrorText = "Required value."
        customValidationRule.ErrorType = ErrorType.Critical

        vpInputs.SetValidationRule(Me.beDataSource, Nothing)
        vpInputs.SetValidationRule(Me.beDataSource, customValidationRule)

    End Sub

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiShowAll.ItemClick
        LoadMainData()
    End Sub

    Private Sub rgCargoType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rgCargoType.SelectedIndexChanged
        LoadMainData()
    End Sub

    Private Sub LoadMainData()
        Dim dtQuery As New DataTable
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get all data table rows")
        dtQuery = oAppService.ExecuteSQL("EXEC ctr.GetCalculatedRates '" & Format(deDateFrom.EditValue, "yyyyMMdd") & "','" & Format(deDateTo.EditValue, "yyyyMMdd") & "','" & Mid(rgCargoType.EditValue, 1, 1) & "'").Tables(0)
        gcMainData.DataSource = dtQuery
        GridView1.PopulateColumns()
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub CalculatedRatesForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GridView1.RestoreLayoutFromRegistry(Directory.GetCurrentDirectory)
        deDateFrom.EditValue = Now.AddDays(-30)
        deDateTo.EditValue = Now

    End Sub

    Private Sub CalculatedRatesForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        GridView1.ActiveFilter.Clear()
        GridView1.SaveLayoutToRegistry(Directory.GetCurrentDirectory)
    End Sub

    Private Sub bbiUpdate_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiUpdate.ItemClick

    End Sub


End Class