Public Class EventosForm 

    Private Sub EventosForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub gcEventos_EmbeddedNavigator_ButtonClick(sender As Object, e As DevExpress.XtraEditors.NavigatorButtonClickEventArgs) Handles gcEventos.EmbeddedNavigator.ButtonClick
        If e.Button.Tag = "Excel" Then
            ExportarExcel(gcEventos)
        End If
    End Sub
End Class