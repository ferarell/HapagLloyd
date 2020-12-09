Imports DevExpress.XtraRichEdit
Imports DevExpress.XtraRichEdit.Export
Imports DevExpress.XtraRichEdit.Services
Imports System.Windows.Forms
Imports System.Data
Imports System.IO

Public Class SettingsForm
    Dim dtConfiguration, dtOriginConfig, dtChild As New System.Data.DataTable
    Dim FileName As String = ""

    Private Sub SettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        bsiVersion.Caption = "Versión : " & My.Application.Info.Version.ToString
        rgResponse.SelectedIndex = 0
        rgFieldName.SelectedIndex = 0
        SplitContainerControl2.Panel2.Visible = False
        If IO.File.Exists(My.Settings.DBFileName) Then
            beDatabase.Text = My.Settings.DBFileName
            'teQueryTable.Text = My.Settings.QueryTableName
            teConfigTable.Text = My.Settings.ConfigTableName
            GetDataConfig()
        End If
    End Sub

    Private Sub FillRichTextControl()
        Dim oRow As DataRow = dtConfiguration.Rows(GridView1.FocusedRowHandle)
        dtChild.Clear()
        dtChild = ExecuteAccessQuery("SELECT Etiqueta, Resultado1, Resultado2 from RobotConfigurationLayout where Posicion=" & oRow("Posicion") & " and TipoColumna='R'").Tables(0)
        recText.RtfText = Nothing
        If Not IsDBNull(oRow(rgFieldName.EditValue)) Then
            recText.HtmlText = oRow(rgFieldName.EditValue)
        End If
        SplitContainerControl2.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1
        If oRow("Identificador").ToString.Contains("OBLI") Then
            SplitContainerControl2.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Both
            gcChildData.DataSource = dtChild
        End If
    End Sub

    Private Sub beDatabase_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beDatabase.Properties.ButtonClick
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beDatabase.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub GetDataConfig()
        If teConfigTable.Text <> "" Then
            dtConfiguration.Rows.Clear()
            dtConfiguration = ExecuteAccessQuery("select * from " & teConfigTable.Text & " where TipoRespuesta in (" & IIf(rgResponse.SelectedIndex = 2, "4,", "") & rgResponse.SelectedIndex.ToString & ")").Tables(0)
            If dtConfiguration.Rows.Count > 0 Then
                gcMainData.DataSource = dtConfiguration
                dtOriginConfig = dtConfiguration.Copy
                FillRichTextControl()
            End If
        End If
    End Sub

    Private Sub bbiSave_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSave.ItemClick
        If beDatabase.Text.Trim = "" Or teConfigTable.Text.Trim = "" Then 'Or teQueryTable.Text.Trim = "" Then
            MsgBox("Por favor ingrese los datos solicitados.", MsgBoxStyle.Exclamation, "Advertencia")
            Return
        End If
        Try
            My.Settings.DBFileName = beDatabase.Text
            'My.Settings.QueryTableName = teQueryTable.Text
            My.Settings.ConfigTableName = teConfigTable.Text
            My.Settings.Save()
            MDBFileName = My.Settings.DBFileName
            If dtConfiguration.Rows.Count Then
                UpdateConfiguration(GridView1.FocusedRowHandle)
            End If
        Catch ex As Exception
            MsgBox("Ocurrió un error al guardar la configuración. " & ex.Message, MsgBoxStyle.Critical, "Error")
        Finally
            MsgBox("Los cambios se guardaron satisfacotiamente.", MsgBoxStyle.Information, "Información")
        End Try
    End Sub

    Friend Function UpdateConfiguration(r As Integer) As Boolean
        Dim bResult As Boolean = True
        Dim comparer As IEqualityComparer(Of DataRow) = DataRowComparer.Default
        Dim oCondition, oValues, oCondition2, oValues2 As String
        Try
            Validate()
            'For r = 0 To dtConfiguration.Rows.Count
            Dim oRow As DataRow = dtConfiguration.Rows(GridView1.FocusedRowHandle)
            'Dim bEqual = comparer.Equals(oRow, dtOriginConfig(r))
            'If Not bEqual Then
            oCondition = "Posicion=" & oRow("Posicion").ToString & " AND TipoRespuesta=" & oRow("TipoRespuesta").ToString
            If rgFieldName.SelectedIndex = 4 Then
                oValues = rgFieldName.EditValue & "='" & recText.Text & "'"
            Else
                oValues = rgFieldName.EditValue & "='" & recText.HtmlText & "'"
            End If
            If Not IsDBNull(GridView1.GetRowCellValue(r, "Descripcion")) Then
                oValues += ", " & "Descripcion='" & GridView1.GetRowCellValue(r, "Descripcion") & "'"
            End If
            If IsDBNull(GridView1.GetRowCellValue(r, "NoticiaVigenteDesde")) Then
                oValues += ", " & "NoticiaVigenteDesde=NULL"
            Else
                oValues += ", " & "NoticiaVigenteDesde='" & Format(GridView1.GetRowCellValue(r, "NoticiaVigenteDesde"), "yyyy-MM-dd") & "'"
            End If
            If IsDBNull(GridView1.GetRowCellValue(r, "NoticiaVigenteHasta")) Then
                oValues += ", " & "NoticiaVigenteHasta=NULL"
            Else
                oValues += ", " & "NoticiaVigenteHasta='" & Format(GridView1.GetRowCellValue(r, "NoticiaVigenteHasta"), "yyyy-MM-dd") & "'"
            End If
            UpdateAccess(My.Settings.ConfigTableName, oCondition, oValues, "")
            oCondition2 = ""
            oValues2 = ""
            For f = 0 To GridView2.RowCount - 1
                Dim drChild As DataRow = dtChild.Rows(f)
                oCondition2 = "Posicion=" & oRow("Posicion").ToString & " AND TipoColumna='R' and Etiqueta='" & drChild("Etiqueta") & "'"
                oValues2 = "Resultado1='" & drChild("Resultado1") & "', Resultado2='" & drChild("Resultado2") & "'"
                UpdateAccess("RobotConfigurationLayout", oCondition2, oValues2, "")
            Next
            If rgFieldName.SelectedIndex = 4 Then
                oRow(rgFieldName.EditValue) = recText.Text
            Else
                oRow(rgFieldName.EditValue) = recText.HtmlText
            End If
            'End If
            'Next
        Catch ex As Exception
            bResult = False
        End Try
        'Dim dtRowR As DataRow = dtResult.Rows(iPosition)
        'Dim dtRowQ As DataRow = dtQuery.Rows(0)
        Return bResult
    End Function

    Private Sub bbiRefresh_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiRefresh.ItemClick
        GetDataConfig()
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    'Private Sub lueConfigTable_Enter(sender As Object, e As EventArgs)
    '    Dim dtQuery As New DataTable
    '    dtQuery = ExecuteAccessQuery("SELECT MSysObjects.Name AS table_name FROM MSysObjects WHERE (((Left([Name],1))<>'~') AND ((Left([Name],4))<>'MSys') AND ((MSysObjects.Type) In (1,4,6))  AND ((MSysObjects.Flags)=0)) order by MSysObjects.Name").Tables(0)
    '    lueConfigTable.Properties.DataSource = dtQuery
    '    lueConfigTable.Properties.DisplayMember = "table_name"
    '    lueConfigTable.Properties.ValueMember = "table_name"
    'End Sub

    Private Sub rgResponse_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rgResponse.SelectedIndexChanged
        rgFieldName.SelectedIndex = 0
        rgFieldName.Enabled = True
        recText.Enabled = True
        rgFieldName.Properties.Items(4).Enabled = False

        If rgResponse.SelectedIndex = 2 Then
            'rgFieldName.Enabled = False
            'recText.Enabled = False
        End If
        If rgResponse.SelectedIndex = 3 Then
            rgFieldName.Properties.Items(4).Enabled = True
            rgFieldName.SelectedIndex = 4
        End If
        GetDataConfig()
    End Sub

    Private Sub rgOleFieldName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rgFieldName.SelectedIndexChanged
        GridView1.Columns("NoticiaVigenteDesde").Visible = False
        GridView1.Columns("NoticiaVigenteHasta").Visible = False
        If dtConfiguration.Rows.Count > 0 Then
            FillRichTextControl()
        End If
        If rgFieldName.SelectedIndex = 3 Then
            GridView1.Columns("NoticiaVigenteDesde").Visible = True
            GridView1.Columns("NoticiaVigenteHasta").Visible = True
        End If
    End Sub

    Private Sub GridView1_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView1.FocusedRowChanged
        If dtConfiguration.Rows.Count > 0 Then
            FillRichTextControl()
        End If
    End Sub

    Private Sub SettingsForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not IO.File.Exists(beDatabase.Text) Then
            DialogResult = Windows.Forms.DialogResult.No
        End If
    End Sub
End Class