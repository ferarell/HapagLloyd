Imports Microsoft.Office.Tools.Ribbon

Public Class CustomizedRibbon

    Private Sub CustomizedRibbon_Load(ByVal sender As System.Object, ByVal e As RibbonUIEventArgs) Handles MyBase.Load

    End Sub

    Private Sub btSettings_Click(sender As Object, e As RibbonControlEventArgs) Handles btSettings.Click
        Dim oForm As New SettingsForm
        oForm.ShowDialog()
    End Sub

End Class
