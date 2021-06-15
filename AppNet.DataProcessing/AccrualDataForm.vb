Imports DevExpress.XtraSplashScreen
Imports BigStick.Http
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Grid

Public Class AccrualDataForm
    Dim oLogProcessUpdate As New LogProcessUpdate
    Dim oLogFileGenerate As New LogFileGenerate
    Dim oAppService As New AppService.HapagLloydServiceClient


    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
    End Sub

    Private Sub FletesOnLineForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub beDataSource_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beDataSource.Properties.ButtonClick
        OpenFileDialog1.Filter = "Excel Files (*.xls*)|*.xls*"
        OpenFileDialog1.FileName = ""
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beDataSource.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(gcFletesOnLine)
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiShowAll.ItemClick
        Dim dtQuery As New DataTable
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Getting all rows ")
        Try
            dtQuery = oAppService.ExecuteSQL("SELECT * FROM spl.AccrualReport").Tables(0)
            gcFletesOnLine.DataSource = dtQuery
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
        End Try
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub bbiDataTransfer_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiDataTransfer.ItemClick
        Dim dtSource As New DataTable
        Dim aResponse As New ArrayList
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Loading data source ")
            'dtSource = GetDistinctDataSource(LoadExcel(beDataSource.Text, "{0}").Tables(0))
            dtSource = LoadExcel(beDataSource.Text, "{0}").Tables(0)
            If dtSource.Rows.Count = 0 Then
                DevExpress.XtraEditors.XtraMessageBox.Show("The data from the source file was not loaded, please check the format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            SplashScreenManager.Default.SetWaitFormDescription("Inserting data source ")
            aResponse.AddRange(oAppService.UpdatingUsingTableAsParameter("spl.upAccrualReportByTable_Insert", Nothing, Nothing, dtSource))
            If aResponse(0) = 0 Then
                Throw New Exception(aResponse(1))
            End If
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show("The data was imported successfully. ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            bbiShowAll.PerformClick()
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show("An error occurred while inserting the data. " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Function GetDistinctDataSource(dtTemp As DataTable) As DataTable
        Dim dtResult As New DataTable
        dtResult = dtTemp.Clone
        For r = 0 To dtTemp.Rows.Count - 1
            Dim oRow As DataRow = dtTemp.Rows(r)
            If IsDBNull(oRow("Standard Location")) Then
                Continue For
            End If
            If dtResult.Select("[Ship# Number]='" & oRow("Ship# Number") & "' And [Standard Location]='" & oRow("Standard Location") & "'").Length > 0 Then
                Continue For
            End If
            dtResult.Rows.Add(oRow.ItemArray)
        Next
        Return dtResult
    End Function
End Class