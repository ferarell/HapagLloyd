<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucAuditPanel
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.pnlAuditoriaUp = New DevExpress.XtraEditors.PanelControl()
        Me.lblUsuarioM = New DevExpress.XtraEditors.LabelControl()
        Me.lblFechaModificacion = New DevExpress.XtraEditors.LabelControl()
        Me.lblFechaCreacion = New DevExpress.XtraEditors.LabelControl()
        Me.lblUsuarioC = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl10 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl11 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl9 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl7 = New DevExpress.XtraEditors.LabelControl()
        CType(Me.pnlAuditoriaUp, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlAuditoriaUp.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlAuditoriaUp
        '
        Me.pnlAuditoriaUp.Controls.Add(Me.lblUsuarioM)
        Me.pnlAuditoriaUp.Controls.Add(Me.lblFechaModificacion)
        Me.pnlAuditoriaUp.Controls.Add(Me.lblFechaCreacion)
        Me.pnlAuditoriaUp.Controls.Add(Me.lblUsuarioC)
        Me.pnlAuditoriaUp.Controls.Add(Me.labelControl10)
        Me.pnlAuditoriaUp.Controls.Add(Me.labelControl11)
        Me.pnlAuditoriaUp.Controls.Add(Me.labelControl9)
        Me.pnlAuditoriaUp.Controls.Add(Me.labelControl7)
        Me.pnlAuditoriaUp.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlAuditoriaUp.Location = New System.Drawing.Point(0, 2)
        Me.pnlAuditoriaUp.Name = "pnlAuditoriaUp"
        Me.pnlAuditoriaUp.Size = New System.Drawing.Size(670, 35)
        Me.pnlAuditoriaUp.TabIndex = 156
        '
        'lblUsuarioM
        '
        Me.lblUsuarioM.Location = New System.Drawing.Point(560, 11)
        Me.lblUsuarioM.Name = "lblUsuarioM"
        Me.lblUsuarioM.Size = New System.Drawing.Size(105, 13)
        Me.lblUsuarioM.TabIndex = 45
        Me.lblUsuarioM.Text = "Usuario Administrador"
        '
        'lblFechaModificacion
        '
        Me.lblFechaModificacion.Location = New System.Drawing.Point(801, 11)
        Me.lblFechaModificacion.Name = "lblFechaModificacion"
        Me.lblFechaModificacion.Size = New System.Drawing.Size(103, 13)
        Me.lblFechaModificacion.TabIndex = 45
        Me.lblFechaModificacion.Text = "06/06/2011 09:31:00"
        '
        'lblFechaCreacion
        '
        Me.lblFechaCreacion.Location = New System.Drawing.Point(299, 11)
        Me.lblFechaCreacion.Name = "lblFechaCreacion"
        Me.lblFechaCreacion.Size = New System.Drawing.Size(103, 13)
        Me.lblFechaCreacion.TabIndex = 45
        Me.lblFechaCreacion.Text = "06/06/2011 09:31:00"
        '
        'lblUsuarioC
        '
        Me.lblUsuarioC.Location = New System.Drawing.Point(88, 11)
        Me.lblUsuarioC.Name = "lblUsuarioC"
        Me.lblUsuarioC.Size = New System.Drawing.Size(105, 13)
        Me.lblUsuarioC.TabIndex = 45
        Me.lblUsuarioC.Text = "Usuario Administrador"
        '
        'labelControl10
        '
        Me.labelControl10.Location = New System.Drawing.Point(714, 11)
        Me.labelControl10.Name = "labelControl10"
        Me.labelControl10.Size = New System.Drawing.Size(55, 13)
        Me.labelControl10.TabIndex = 44
        Me.labelControl10.Text = "Date / Time"
        '
        'labelControl11
        '
        Me.labelControl11.Location = New System.Drawing.Point(440, 11)
        Me.labelControl11.Name = "labelControl11"
        Me.labelControl11.Size = New System.Drawing.Size(60, 13)
        Me.labelControl11.TabIndex = 43
        Me.labelControl11.Text = "Updated by:"
        '
        'labelControl9
        '
        Me.labelControl9.Location = New System.Drawing.Point(215, 11)
        Me.labelControl9.Name = "labelControl9"
        Me.labelControl9.Size = New System.Drawing.Size(55, 13)
        Me.labelControl9.TabIndex = 42
        Me.labelControl9.Text = "Date / Time"
        '
        'labelControl7
        '
        Me.labelControl7.Location = New System.Drawing.Point(7, 11)
        Me.labelControl7.Name = "labelControl7"
        Me.labelControl7.Size = New System.Drawing.Size(58, 13)
        Me.labelControl7.TabIndex = 40
        Me.labelControl7.Text = "Created by:"
        '
        'ucAuditPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.pnlAuditoriaUp)
        Me.Name = "ucAuditPanel"
        Me.Size = New System.Drawing.Size(670, 37)
        CType(Me.pnlAuditoriaUp, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlAuditoriaUp.ResumeLayout(False)
        Me.pnlAuditoriaUp.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents pnlAuditoriaUp As DevExpress.XtraEditors.PanelControl
    Private WithEvents lblUsuarioM As DevExpress.XtraEditors.LabelControl
    Private WithEvents lblFechaModificacion As DevExpress.XtraEditors.LabelControl
    Private WithEvents lblFechaCreacion As DevExpress.XtraEditors.LabelControl
    Private WithEvents lblUsuarioC As DevExpress.XtraEditors.LabelControl
    Private WithEvents labelControl10 As DevExpress.XtraEditors.LabelControl
    Private WithEvents labelControl11 As DevExpress.XtraEditors.LabelControl
    Private WithEvents labelControl9 As DevExpress.XtraEditors.LabelControl
    Private WithEvents labelControl7 As DevExpress.XtraEditors.LabelControl

End Class
