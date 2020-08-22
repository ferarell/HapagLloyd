Imports DevExpress.LookAndFeel

Public Class LookAndFeelForm

    Private Sub frmEstilos_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        For Each cnt In DevExpress.Skins.SkinManager.Default.Skins
            lbcEstilos.Items.Add(cnt.SkinName)
        Next
        lbcEstilos.SelectedValue = DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveLookAndFeel.ActiveSkinName
    End Sub

    Private Sub lbcEstilos_Click(sender As Object, e As EventArgs) Handles lbcEstilos.Click
        Dim skinName As String
        skinName = lbcEstilos.Text
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("")
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(skinName)
        My.Settings.LookAndFeel = DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveLookAndFeel.ActiveSkinName
        My.Settings.Save()
    End Sub

End Class