Imports HLAG.OPE.NET
Imports DevExpress.XtraEditors

Public Class ucAuditPanel

    Public Property pnlAuditoria() As PanelControl
        Get
            Return Me.pnlAuditoriaUp
        End Get
        Set(ByVal value As PanelControl)
            Me.pnlAuditoriaUp = value
        End Set
    End Property

    Private _CreatedBy As String
    Public Property CreatedBy() As String
        Get
            Return _CreatedBy
        End Get
        Set(ByVal value As String)
            _CreatedBy = value
        End Set
    End Property

    Private _UpdatedBy As String
    Public Property UpdatedBy() As String
        Get
            Return _UpdatedBy
        End Get
        Set(ByVal value As String)
            _UpdatedBy = value
        End Set
    End Property


    Private _CreatedDate As DateTime
    Public Property CreatedDate() As DateTime
        Get
            Return _CreatedDate
        End Get
        Set(ByVal value As DateTime)
            _CreatedDate = value
        End Set
    End Property


    Private _UpdatedDate As DateTime
    Public Property UpdatedDate() As DateTime
        Get
            Return _UpdatedDate
        End Get
        Set(ByVal value As DateTime)
            _UpdatedDate = value
        End Set
    End Property

    Private Sub panelControl3_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles pnlAuditoriaUp.Paint
        Me.lblUsuarioC.Text = CreatedBy
        Me.lblUsuarioM.Text = UpdatedBy
        Me.lblFechaCreacion.Text = CreatedDate
        Me.lblFechaModificacion.Text = UpdatedDate
    End Sub
End Class
