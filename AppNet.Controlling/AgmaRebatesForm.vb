Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.Collections

Public Class AgmaRebatesForm
    Dim dtSource As New DataTable
    Dim oAppService As New AppService.HapagLloydServiceClient

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

    Private Sub AgmaRebatesForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        deEffectiveDate.EditValue = Now
    End Sub

    Private Sub bbiProcesss_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesss.ItemClick
        Dim aResult As New ArrayList
        LoadInputValidations()
        If Not vpInputs.Validate Then
            Return
        End If
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            If LoadDataSourceV2() Then
                'ImportDataSource()
                If dtSource.Rows.Count > 0 Then
                    Dim oParams, oValues As New ArrayList
                    oParams.Add("@User")
                    oValues.Add(My.User.Name)
                    SplashScreenManager.Default.SetWaitFormDescription("Updating Master Table of Agma Rebates (Tramarsa)")
                    aResult.AddRange(oAppService.UpdatingUsingTableAsParameter("ctr.spAgmaRebatesUpdate", oParams.ToArray, oValues.ToArray, dtSource))
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

    Function CreateDataSourceTable() As DataTable
        Dim dtFile As New DataTable
        dtFile.Columns.Add("MRShipper", GetType(String)).AllowDBNull = True
        dtFile.Columns.Add("SXC", GetType(String)).AllowDBNull = True
        dtFile.Columns.Add("RA", GetType(String)).AllowDBNull = True
        dtFile.Columns.Add("LocalForeign", GetType(String)).AllowDBNull = True
        dtFile.Columns.Add("BLType", GetType(String)).AllowDBNull = True
        dtFile.Columns.Add("Shipper", GetType(String)).AllowDBNull = True
        dtFile.Columns.Add("Consignee", GetType(String)).AllowDBNull = True
        dtFile.Columns.Add("Commodity", GetType(String)).AllowDBNull = True
        dtFile.Columns.Add("ValidFrom", GetType(String)).AllowDBNull = True
        dtFile.Columns.Add("ValidTo", GetType(String)).AllowDBNull = True
        dtFile.Columns.Add("VBTariff", GetType(Decimal)).AllowDBNull = True
        dtFile.Columns.Add("CommissionCustomer", GetType(Decimal)).AllowDBNull = True
        dtFile.Columns.Add("Unit", GetType(String)).AllowDBNull = True
        dtFile.Columns.Add("Comments", GetType(String)).AllowDBNull = True
        Return dtFile
    End Function

    Friend Function LoadDataSourceV2() As Boolean
        Dim bResult As Boolean = True
        Dim dtFile As New DataTable
        Dim sFecha1, sFecha2 As String
        dtSource.Columns.Clear()
        If dtSource.Columns.Count = 0 Then
            dtSource = CreateDataSourceTable.Clone
        End If
        dtSource.TableName = "AgmaRebates"
        For i = 0 To OpenFileDialog1.FileNames.Count - 1
            If OpenFileDialog1.FileNames(i).ToUpper.Contains(".XLS") Then
                SplashScreenManager.Default.SetWaitFormDescription("Loading Data Sources...")
                'Acuerdos Comerciales (TRAMARSA)
                SplashScreenManager.Default.SetWaitFormDescription("Loading Data of Agma Rebates")
                dtFile = LoadExcelWH(OpenFileDialog1.FileNames(i), "{0}", "").Tables(0)
                dtFile = dtFile.Select("F1 <> 'TYPE CUSTOMER 1' AND F1 <> '' AND F21 <> ''").CopyToDataTable
                Continue For
            End If
        Next
        If dtFile.Rows.Count = 0 Then
            bResult = False
        End If
        Dim Shipper, Consignee As String
        For r = 0 To dtFile.Rows.Count - 1
            Dim drFile As DataRow = dtFile.Rows(r)
            If IsDBNull(drFile("F5")) Then
                drFile("F5") = ""
            End If
            If IsDBNull(drFile("F1")) Then
                drFile("F1") = ""
            End If
            Shipper = IIf(drFile("F5") = "SHIPPER", drFile("F6"), "")
            Consignee = IIf(drFile("F1") = "CONSIGNEE", drFile("F2"), "")
            sFecha1 = ToDate(drFile("F12"))
            sFecha2 = ToDate(drFile("F13"))
            dtSource.Rows.Add(drFile("F2"), "", drFile("F14"), "", drFile("F19"), Shipper, Consignee, drFile("F18"), sFecha1, sFecha2, drFile("F21"), drFile("F21"), "", drFile("F34"))
        Next
        Return bResult
    End Function

    Function ToDate(sFecha As String) As String
        Dim sResult As String = ""
        Try
            If IsDate(sFecha) Then
                sResult = Format(CDate(sFecha), "dd/MM/yyyy")
            Else
                sResult = Mid(sFecha, 1, 2) & "/" & ConvertShortMonthAsNumber(Mid(sFecha, 4, 3)) & "/20" & Strings.Right(sFecha, 2)
            End If

        Catch ex As Exception

        End Try
        Return sResult
    End Function

    Friend Function LoadDataSource() As Boolean
        Dim bResult As Boolean = True
        dtSource.Rows.Clear()
        For i = 0 To OpenFileDialog1.FileNames.Count - 1
            If OpenFileDialog1.FileNames(i).ToUpper.Contains(".XLS") Then
                SplashScreenManager.Default.SetWaitFormDescription("Loading Data Sources...")
                'Acuerdos Comerciales (TRAMARSA)
                SplashScreenManager.Default.SetWaitFormDescription("Loading Data of Agma Rebates")
                dtSource = LoadExcelWH(OpenFileDialog1.FileNames(i), "{0}", "").Tables(0)
                dtSource = dtSource.Select("F1 <> 'MRShipper'").CopyToDataTable
                dtSource.TableName = "AgmaRebates"
                Continue For
            End If
        Next
        If dtSource.Rows.Count = 0 Then
            bResult = False
        End If
        Return bResult
    End Function

    Private Sub ImportDataSource()

    End Sub

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiShowAll.ItemClick
        Dim dtQuery As New DataTable
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get data table rows")
        Validate()
        dtQuery = oAppService.ExecuteSQL("EXEC ctr.spGetAgmaRebates '" & Format(deEffectiveDate.EditValue, "yyyyMMdd") & "','" & rgBlType.EditValue & "'").Tables(0)
        gcMainData.DataSource = dtQuery
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub bbiUpdate_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiUpdate.ItemClick
        If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "Are you sure to update?", "Confirmation", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.No Then
            Return
        End If
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            oAppService.ExecuteSQLNonQuery("EXEC ctr.spAgmaRebatesUpdate")
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(gcMainData)
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

    Private Sub bbiDelete_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiDelete.ItemClick
        If gcMainData.FocusedView.Name <> "GridView1" Or gcMainData.FocusedView.RowCount = 0 Then
            Return
        End If
        Dim aReturn As New ArrayList
        If DevExpress.XtraEditors.XtraMessageBox.Show("Are you sure you want to delete the selected record? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
            Dim oGridView As New GridView
            If GridView1.IsFocusedView Then
                oGridView = GridView1
            Else
                oGridView = GridView1
            End If
            oGridView.ActiveFilterEnabled = False
            Try
                oGridView.DeleteRow(oGridView.FocusedRowHandle)
                aReturn.AddRange(ExecuteSQLNonQuery("DELETE FROM  WHERE = '" & oGridView.GetFocusedRowCellValue("BLNO") & "'"))
            Catch ex As Exception
                DevExpress.XtraEditors.XtraMessageBox.Show("Ocurrió un error al eliminar el registro.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
            oGridView.RefreshData()
            oGridView.ActiveFilterEnabled = True
            DevExpress.XtraEditors.XtraMessageBox.Show("El registro ha sido eliminado satisfactoriamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
End Class