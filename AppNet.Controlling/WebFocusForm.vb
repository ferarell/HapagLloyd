Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.Collections

Public Class WebFocusForm
    Dim MasterTable As String = ""
    Dim dtResult, dtSourceWebFocus As New DataTable
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

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiShowAll.ItemClick
        MasterTable = "ctr.WebFocus" & IIf(rgCargoType.SelectedIndex = 0, "Import", "Export")
        Dim dtQuery As New DataTable
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get all data table rows")
        dtQuery = oAppService.ExecuteSQL("EXEC ctr.GetWebFocus '" & Format(deDateFrom.EditValue, "yyyyMMdd") & "','" & Format(deDateTo.EditValue, "yyyyMMdd") & "','" & rgCargoType.EditValue & "'").Tables(0)
        gcMainData.DataSource = dtQuery
        GridView1.PopulateColumns()
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(gcMainData)
    End Sub

    Private Sub bbiProcesss_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesss.ItemClick
        Dim aResult As New ArrayList
        LoadInputValidations()
        If Not vpInputs.Validate Then
            Return
        End If
        Try
            MasterTable = "WebFocus" & IIf(rgCargoType.SelectedIndex = 0, "Import", "Export")
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            If LoadDataSources() Then
                'ImportDataSources()
                If dtSourceWebFocus.Rows.Count > 0 Then
                    Dim oParams, oValues As New ArrayList
                    oParams.Add("@User")
                    oValues.Add(My.User.Name)
                    SplashScreenManager.Default.SetWaitFormDescription("Updating Master Table of WebFocus")
                    aResult.AddRange(oAppService.UpdatingUsingTableAsParameter("ctr.spWebFocusUpdate", oParams.ToArray, oValues.ToArray, dtSourceWebFocus))
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
        dtSourceWebFocus.Rows.Clear()
        Dim dtMainName As String = ""
        For i = 0 To OpenFileDialog1.FileNames.Count - 1
            If OpenFileDialog1.FileNames(i).ToUpper.Contains(".XLS") Then
                'WEB FOCUS (HAPAG LLOYD)
                SplashScreenManager.Default.SetWaitFormDescription("Loading Data Source of Web Focus (Hapag Lloyd)")
                dtSourceWebFocus = LoadExcel(OpenFileDialog1.FileNames(i), "{0}").Tables(0)
                Continue For
            End If
        Next
        Return bResult
    End Function

    'Private Sub ImportDataSources()
    '    Dim iPos As Integer = 0
    '    Dim sWhere As String = ""
    '    dtResult.Rows.Clear()
    '    dtResult = oAppService.ExecuteSQL("SELECT * FROM " & MasterTable & " WHERE BLNO=''").Tables(0)
    '    SplashScreenManager.Default.SetWaitFormDescription("Importing files selected...")
    '    For r = 1 To dtSourceWebFocus.Rows.Count - 1
    '        Try
    '            Dim oRow As DataRow = dtSourceWebFocus.Rows(r)
    '            SplashScreenManager.Default.SetWaitFormDescription("Processing Row " & r.ToString & " de " & (dtSourceWebFocus.Rows.Count - 1).ToString)
    '            If IsDBNull(dtSourceWebFocus.Rows(r)(0)) Or dtSourceWebFocus.Rows(r)(0) = "" Then
    '                Continue For
    '            End If
    '            If (Mid(oRow("F22"), 1, 2) = "PE" Or Mid(oRow("F24"), 1, 2) = "PE") And rgCargoType.SelectedIndex = 0 Then
    '                Continue For
    '            End If
    '            If (Mid(oRow("F26"), 1, 2) = "PE" Or Mid(oRow("F29"), 1, 2) = "PE") And rgCargoType.SelectedIndex = 1 Then
    '                Continue For
    '            End If
    '            If Not IsDate(oRow("F20")) Or Not IsDate(oRow("F21")) Then
    '                Continue For
    '            End If
    '            'If Not BlTypeValidate(oRow("BLNO"), rgCargoType.EditValue) Then
    '            '    Continue For
    '            'End If
    '            sWhere = "BLNO='" & oRow("F1") & "' AND Booking='" & oRow("F2") & "' AND Commodity_HS_Code='" & oRow("F4") & "' AND Geo_From_Std_Loc_Code='" & oRow("F22") & "' AND Geo_Start_Std_Loc_Code='" & oRow("F24") & "'"
    '            If ExecuteAccessQuery("SELECT * FROM " & MasterTable & " WHERE " & sWhere).Tables(0).Rows.Count > 0 Then
    '                If Not ExecuteAccessNonQuery("DELETE FROM " & MasterTable & " WHERE " & sWhere) Then
    '                    DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "There was an error while delete the existing BL: " & oRow("F1"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '                    Exit For
    '                End If
    '            End If
    '            iPos = dtResult.Rows.Count
    '            For c = 0 To oRow.ItemArray.Count - 1
    '                If oRow(c).ToString.Contains("'") Then
    '                    oRow(c) = Replace(oRow(c), "'", " ")
    '                End If
    '            Next
    '            dtResult.Rows.Add()
    '            dtResult.Rows(iPos).ItemArray = oRow.ItemArray
    '            dtResult.Rows(iPos)("CreatedBy") = My.User.Name
    '            dtResult.Rows(iPos)("CreatedDate") = Now
    '            InsertIntoAccess(MasterTable, dtResult.Rows(iPos))
    '        Catch ex As Exception
    '            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "There was an error into data process. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        End Try
    '    Next
    'End Sub

    Friend Function BlTypeValidate(BLNO As String, BlType As String) As Boolean
        Dim bResult As Boolean = True
        If BlType = "I" Then
            If Not (Mid(BLNO, 1, 6) = "HLCULI" And IsNumeric(Mid(BLNO, 7, 1))) Then
                bResult = False
            End If
        ElseIf BlType = "E" Then
            If Mid(BLNO, 1, 6) = "HLCULI" And IsNumeric(Mid(BLNO, 7, 1)) Then
                bResult = False
            End If
        End If
        Return bResult
    End Function

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

    Private Sub WebFocusForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GridView1.RestoreLayoutFromRegistry(Directory.GetCurrentDirectory)
        deDateFrom.EditValue = Now.AddDays(-30)
        deDateTo.EditValue = Now
    End Sub

    Private Sub WebFocusForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        GridView1.ActiveFilter.Clear()
        GridView1.SaveLayoutToRegistry(Directory.GetCurrentDirectory)
    End Sub
End Class