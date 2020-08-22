Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo

Public Class BaseRatesForm
    Dim oAppService As New AppService.HapagLloydServiceClient

    Private Sub BaseRatesForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'LoadMainData()
        LoadBlTypes()
        LoadUnits()
        LoadRates()
    End Sub

    Private Sub LoadBlTypes()
        Dim dtQuery As New DataTable
        dtQuery = oAppService.ExecuteSQL("select * from ctr.Regime").Tables(0)
        RepositoryItemLookUpEdit2.DataSource = dtQuery
        RepositoryItemLookUpEdit2.DisplayMember = "Description"
        RepositoryItemLookUpEdit2.ValueMember = "Code"
    End Sub

    Private Sub LoadUnits()
        Dim dtQuery As New DataTable
        dtQuery = oAppService.ExecuteSQL("select * from ctr.Unit").Tables(0)
        RepositoryItemLookUpEdit3.DataSource = dtQuery
        RepositoryItemLookUpEdit3.DisplayMember = "Description"
        RepositoryItemLookUpEdit3.ValueMember = "Description"
    End Sub

    Private Sub LoadRates()
        Dim dtQuery As New DataTable
        dtQuery = oAppService.ExecuteSQL("select * from ctr.Rate").Tables(0)
        RepositoryItemLookUpEdit4.DataSource = dtQuery
        RepositoryItemLookUpEdit4.DisplayMember = "Code"
        RepositoryItemLookUpEdit4.ValueMember = "Code"
    End Sub

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiShowAll.ItemClick
        LoadMainData()
    End Sub

    Private Sub bbiUpdate_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiUpdate.ItemClick
        Dim dtComparer As DataTable = oAppService.ExecuteSQL("SELECT * FROM ctr.BookRates").Tables(0)
        Validate()
        Dim sConditions, sValues As String
        GridView1.OptionsLayout.StoreAllOptions = True
        GridView1.ActiveFilterEnabled = False
        GridView1.ClearSorting()
        Dim info As GridViewInfo = TryCast(GridView1.GetViewInfo(), GridViewInfo)
        Dim GridRowInfo As GridRowInfo = info.GetGridRowInfo(GridView1.FocusedRowHandle)
        For r = 0 To GridView1.RowCount - 1
            Dim dtRowR As DataRow = dtComparer.Rows(r)
            Dim dtRowQ As DataRow = GridView1.GetDataRow(r)
            Dim comparer As IEqualityComparer(Of DataRow) = DataRowComparer.Default
            Dim bEqual = comparer.Equals(dtRowR, dtRowQ)
            If bEqual Then
                Continue For
            End If
            'sConditions = "Container='" & GridView1.GetRowCellValue(r, "Container") & "' AND Booking='" & GridView1.GetRowCellValue(r, "Booking") & "'"
            sConditions = ""
            sValues = ""
            For c = 0 To GridView1.Columns.Count - 1
                If Not GridView1.Columns(c).OptionsColumn.ReadOnly Then
                    If IsDBNull(GridView1.GetRowCellValue(r, GridView1.Columns(c).FieldName)) Then
                        sValues = sValues & IIf(sValues = "", "", ", ") & GridView1.Columns(c).FieldName & "=NULL"
                    Else
                        If GridView1.Columns(c).ColumnType = GetType(Boolean) Then
                            sValues = sValues & IIf(sValues = "", "", ", ") & GridView1.Columns(c).FieldName & "=" & GridView1.GetRowCellValue(r, GridView1.Columns(c).FieldName)
                        Else
                            sValues = sValues & IIf(sValues = "", "", ", ") & GridView1.Columns(c).FieldName & "='" & GridView1.GetRowCellValue(r, GridView1.Columns(c).FieldName) & "'"
                        End If
                    End If
                End If
            Next
            'UpdateAccess("BookRates", sConditions, sValues)
        Next
        GridView1.ActiveFilterEnabled = True
        bbiShowAll.PerformClick()
        GridView1.MoveBy(GridRowInfo.RowHandle)
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(gcMainData)
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub LoadMainData()
        Dim dtQuery As New DataTable
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get all data table rows")
        dtQuery = oAppService.ExecuteSQL("select * from ctr.BookRates order by BL_TYPE, RATE_CODE, FECHA_VB_INI, FECHA_VB_FIN").Tables(0)
        gcMainData.DataSource = dtQuery
        SplashScreenManager.CloseForm(False)

    End Sub

    Private Sub rgCargoType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rgCargoType.SelectedIndexChanged
        If rgCargoType.SelectedIndex = 0 Then
            GridView1.ActiveFilterString = ""
        ElseIf rgCargoType.SelectedIndex = 1 Then
            GridView1.ActiveFilterString = "BL_TYPE='I'"
        ElseIf rgCargoType.SelectedIndex = 2 Then
            GridView1.ActiveFilterString = "BL_TYPE='E'"
        End If
        GridView1.ActiveFilterEnabled = True
    End Sub
End Class