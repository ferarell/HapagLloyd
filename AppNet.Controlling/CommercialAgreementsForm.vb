Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.Collections

Public Class CommercialAgreementsForm
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

    Private Sub CommercialAgreementsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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
            If LoadDataSources() Then
                'ImportDataSources()
                If dtSource.Rows.Count > 0 Then
                    Dim oParams, oValues As New ArrayList
                    oParams.Add("@User")
                    oValues.Add(My.User.Name)
                    SplashScreenManager.Default.SetWaitFormDescription("Updating Master Table of Commercial Agreements (Tramarsa)")
                    aResult.AddRange(oAppService.UpdatingUsingTableAsParameter("ctr.spCommercialAgreementsUpdate", oParams.ToArray, oValues.ToArray, dtSource))
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
        dtSource.Rows.Clear()
        For i = 0 To OpenFileDialog1.FileNames.Count - 1
            If OpenFileDialog1.FileNames(i).ToUpper.Contains(".XLS") Then
                SplashScreenManager.Default.SetWaitFormDescription("Loading Data Sources...")
                'Acuerdos Comerciales (TRAMARSA)
                SplashScreenManager.Default.SetWaitFormDescription("Loading Data of Tramarsa Commercial Agreements ")
                dtSource = LoadExcelWH(OpenFileDialog1.FileNames(i), "{0}", "F31 IS NOT NULL").Tables(0)
                dtSource = dtSource.Select("F1 <> 'Número Conseción'").CopyToDataTable
                dtSource.TableName = "Concesiones"
                'InsertDataFile(dtSource, "Importing File " & (i + 1).ToString & " of " & OpenFileDialog1.FileNames.Count.ToString)
                Continue For
            End If
        Next
        If dtSource.Rows.Count = 0 Then
            bResult = False
        End If
        Return bResult
    End Function

    'Private Sub InsertDataFile(dtFile As DataTable, WaitText As String)
    '    For Each row As DataRow In dtFile.Rows
    '        SplashScreenManager.Default.SetWaitFormDescription(WaitText & " (Row: " & (dtFile.Rows.IndexOf(row) + 1).ToString & " of " & dtFile.Rows.Count.ToString & ")")
    '        ImportDataSources()
    '    Next
    'End Sub

    'Private Sub InsertDataFile(dtFile As DataTable, WaitText As String)
    '    Dim iPos, iConcesion As Integer
    '    Dim sWhere, sTarifa As String
    '    Dim dtResult As DataTable = ExecuteAccessQuery("SELECT * FROM CommercialAgreements WHERE NumeroConcesion=0").Tables(0)
    '    For r = 5 To dtSource.Rows.Count - 1
    '        Try
    '            Dim oRow As DataRow = dtSource.Rows(r)
    '            SplashScreenManager.Default.SetWaitFormDescription(WaitText & " (Row: " & (r - 4).ToString & " of " & (dtFile.Rows.Count - 4).ToString & ")")
    '            If IsDBNull(dtSource.Rows(r)(0)) Then
    '                Continue For
    '            End If
    '            iConcesion = CInt(oRow("F1"))
    '            sTarifa = Strings.Left(oRow("F32"), InStr(1, oRow("F32").ToString, " ") - 1)
    '            sWhere = "NumeroConcesion=" & iConcesion & " AND RateAgreement='" & oRow("F4") & "' AND Tarifa='" & sTarifa & "'"
    '            If ExecuteAccessQuery("SELECT * FROM CommercialAgreements WHERE " & sWhere).Tables(0).Rows.Count > 0 Then
    '                If Not ExecuteAccessNonQuery("DELETE FROM CommercialAgreements WHERE " & sWhere) Then
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
    '            oRow("F4") = IIf(IsDBNull(oRow("F4")), "", oRow("F4"))
    '            dtResult.Rows.Add()
    '            dtResult.Rows(iPos)("NumeroConcesion") = iConcesion
    '            dtResult.Rows(iPos)("Tarifa") = sTarifa
    '            dtResult.Rows(iPos)("RateAgreement") = oRow("F4")
    '            dtResult.Rows(iPos)("Cliente") = oRow("F13")
    '            dtResult.Rows(iPos)("TipoCliente") = oRow("F14")
    '            dtResult.Rows(iPos)("Agente") = oRow("F15")
    '            dtResult.Rows(iPos)("TipoAgente") = oRow("F16")
    '            dtResult.Rows(iPos)("ServicioNave") = oRow("F17")
    '            dtResult.Rows(iPos)("ServicioBL") = oRow("F18")
    '            dtResult.Rows(iPos)("Mercancia") = oRow("F19")
    '            dtResult.Rows(iPos)("TipoContenedor") = oRow("F20")
    '            dtResult.Rows(iPos)("BLMaster") = oRow("F21")
    '            dtResult.Rows(iPos)("Contenedor") = oRow("F22")
    '            dtResult.Rows(iPos)("BLHouse") = oRow("F23")
    '            dtResult.Rows(iPos)("ClienteHijo") = oRow("F24")
    '            dtResult.Rows(iPos)("TipoClienteHijo") = oRow("F25")
    '            dtResult.Rows(iPos)("Booking") = oRow("F26")
    '            dtResult.Rows(iPos)("FechaCreacionOriginal") = oRow("F29")
    '            dtResult.Rows(iPos)("VigenciaDesde") = oRow("F30")
    '            dtResult.Rows(iPos)("VigenciaHasta") = oRow("F31")
    '            dtResult.Rows(iPos)("Monto") = CDbl(oRow("F34"))
    '            dtResult.Rows(iPos)("CreatedBy") = My.User.Name
    '            dtResult.Rows(iPos)("CreatedDate") = Now
    '            InsertIntoAccess("CommercialAgreements", dtResult.Rows(iPos))
    '        Catch ex As Exception
    '            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "There was an error into data process. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        End Try
    '    Next
    'End Sub

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiShowAll.ItemClick
        Dim dtQuery As New DataTable
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get all data table rows")
        Validate()
        If rgType.SelectedIndex = 0 Then
            dtQuery = oAppService.ExecuteSQL("EXEC ctr.spGetCommercialAgreements NULL, 0").Tables(0)
        Else
            dtQuery = oAppService.ExecuteSQL("EXEC ctr.spGetCommercialAgreements '" & Format(deEffectiveDate.EditValue, "yyyyMMdd") & "', 1").Tables(0)
        End If
        gcMainData.DataSource = dtQuery
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub bbiUpdate_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiUpdate.ItemClick

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

End Class