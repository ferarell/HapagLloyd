<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LookAndFeelForm
    Inherits DevExpress.XtraEditors.XtraForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LookAndFeelForm))
        Me.gcEstilos = New DevExpress.XtraEditors.GroupControl()
        Me.lbcEstilos = New DevExpress.XtraEditors.ListBoxControl()
        Me.SplitterControl1 = New DevExpress.XtraEditors.SplitterControl()
        CType(Me.gcEstilos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gcEstilos.SuspendLayout()
        CType(Me.lbcEstilos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'gcEstilos
        '
        Me.gcEstilos.Controls.Add(Me.lbcEstilos)
        Me.gcEstilos.Dock = System.Windows.Forms.DockStyle.Left
        Me.gcEstilos.Location = New System.Drawing.Point(0, 0)
        Me.gcEstilos.Margin = New System.Windows.Forms.Padding(4)
        Me.gcEstilos.Name = "gcEstilos"
        Me.gcEstilos.Size = New System.Drawing.Size(402, 574)
        Me.gcEstilos.TabIndex = 0
        Me.gcEstilos.Text = "Seleccione la nueva apariencia del sistema."
        '
        'lbcEstilos
        '
        Me.lbcEstilos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbcEstilos.Location = New System.Drawing.Point(2, 27)
        Me.lbcEstilos.Margin = New System.Windows.Forms.Padding(4)
        Me.lbcEstilos.Name = "lbcEstilos"
        Me.lbcEstilos.Size = New System.Drawing.Size(398, 545)
        Me.lbcEstilos.TabIndex = 0
        '
        'SplitterControl1
        '
        Me.SplitterControl1.Location = New System.Drawing.Point(402, 0)
        Me.SplitterControl1.Margin = New System.Windows.Forms.Padding(4)
        Me.SplitterControl1.Name = "SplitterControl1"
        Me.SplitterControl1.Size = New System.Drawing.Size(5, 574)
        Me.SplitterControl1.TabIndex = 1
        Me.SplitterControl1.TabStop = False
        '
        'LookAndFeelForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 19.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1092, 574)
        Me.Controls.Add(Me.SplitterControl1)
        Me.Controls.Add(Me.gcEstilos)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "LookAndFeelForm"
        Me.Text = "Paint Styles"
        CType(Me.gcEstilos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gcEstilos.ResumeLayout(False)
        CType(Me.lbcEstilos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gcEstilos As DevExpress.XtraEditors.GroupControl
    Friend WithEvents SplitterControl1 As DevExpress.XtraEditors.SplitterControl
    Friend WithEvents lbcEstilos As DevExpress.XtraEditors.ListBoxControl
End Class
