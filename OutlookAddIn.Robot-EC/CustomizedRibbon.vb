Imports Microsoft.Office.Tools.Ribbon

Public Class CustomizedRibbon

    Private Sub CustomizedRibbon_Load(ByVal sender As System.Object, ByVal e As RibbonUIEventArgs) Handles MyBase.Load
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(My.Settings.LookAndFeel)
        Group3.Label = "ROBOT (" & My.Application.Info.Version.ToString & ")"
    End Sub

    Private Sub btSettings_Click(sender As Object, e As RibbonControlEventArgs) Handles btSettings.Click
        Dim oForm As New SettingsForm
        oForm.ShowDialog()
    End Sub

    Private Sub btStatistics_Click(sender As Object, e As RibbonControlEventArgs) Handles btStatistics.Click
        Dim oForm As New StatisticsForm
        oForm.ShowDialog()
    End Sub

    Private Sub btLayouts_Click(sender As Object, e As RibbonControlEventArgs) Handles btLayouts.Click
        Dim oForm As New LayoutForm
        oForm.Show()
    End Sub
End Class
