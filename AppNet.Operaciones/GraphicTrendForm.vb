Imports DevExpress.XtraCharts

Public Class GraphicTrendForm
    Dim dsVendorData As New dsMain
    Friend pBooking, pContainer, pGap As String
    Friend pSetpoint As Double
    Friend dtEvents As New DataTable

    Private Sub GraphicTrendForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim dtResult As New DataTable
        Dim gvResult As New DevExpress.XtraGrid.GridControl
        Dim diagram As SwiftPlotDiagram = CType(ccTrends.Diagram, SwiftPlotDiagram)
        bar5.Visible = False
        Try
            dtResult = ExecuteAccessQuery("select CT_DATE AS C1, CT_TIME AS C2, CT_USDA1 AS C3, CT_USDA2 AS C4, CT_USDA3 AS C5 from ColdTreatmentReadings where [BOOKING]='" & pBooking & "' and [CONTAINER] = '" & pContainer & "'").Tables(0)
            If dtResult.Rows.Count > 0 Then
                gvResult.DataSource = dtResult
                ccTrends.DataSource = gvResult.DataSource
            End If
            If pGap = "Y" Then
                For r = 0 To dtEvents.Rows.Count - 1
                    Dim oRow As DataRow = dtEvents.Rows(r)
                    If oRow("DESCRIPTION").ToString.ToUpper.Contains("GAP") Then
                        NewLine(r, Mid(oRow("DESCRIPTION"), 36, 17))
                    End If
                Next
            End If
            diagram.AxisY.ConstantLines(2).AxisValue = pSetpoint
            diagram.AxisY.ConstantLines(2).Visible = True

        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub NewLine(LineNumber As Integer, LineText As String)
        Dim diagram As SwiftPlotDiagram = CType(ccTrends.Diagram, SwiftPlotDiagram)

        ' Create a constant line.
        Dim constantLine As New ConstantLine(LineNumber.ToString)
        diagram.AxisX.ConstantLines.Add(constantLine)

        ' Define its axis value.
        constantLine.AxisValue = LineText

        ' Customize the behavior of the constant line.
        'constantLine.Visible = True
        constantLine.ShowInLegend = False
        'constantLine.LegendText = "Some Threshold"
        'constantLine.ShowBehind = False

        ' Customize the constant line's title.
        constantLine.Title.Visible = True
        constantLine.Title.Text = "GAP " & LineText
        constantLine.Title.TextColor = Color.Red
        constantLine.Title.Antialiasing = False
        constantLine.Title.Font = New Font("Tahoma", 7.5)
        constantLine.Title.ShowBelowLine = True
        constantLine.Title.Alignment = ConstantLineTitleAlignment.Near

        ' Customize the appearance of the constant line.
        'constantLine.Color = Color.Red
        constantLine.LineStyle.DashStyle = DashStyle.DashDotDot
        constantLine.LineStyle.Thickness = 1
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportToImage(ccTrends, pBooking & "-" & pContainer & ".png", Imaging.ImageFormat.Png)
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub beiShowSetPoint_EditValueChanged(sender As Object, e As EventArgs) Handles beiShowSetPoint.EditValueChanged
    End Sub

    Private Sub beiShowSetPoint_ItemPress(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles beiShowSetPoint.ItemPress
    End Sub

    Private Sub beiShowSetPoint_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles beiShowSetPoint.ItemClick
        'Dim bShowSetpoint As Boolean = beiShowSetPoint.EditValue
        'Dim diagram As SwiftPlotDiagram = CType(ccTrends.Diagram, SwiftPlotDiagram)
        'diagram.AxisY.ConstantLines(2).Visible = bShowSetpoint
    End Sub
End Class