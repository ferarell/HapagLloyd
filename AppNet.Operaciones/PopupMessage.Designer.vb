<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PopupMessage
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
        Me.recMessage = New DevExpress.XtraRichEdit.RichEditControl()
        Me.SuspendLayout()
        '
        'recMessage
        '
        Me.recMessage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.recMessage.EnableToolTips = True
        Me.recMessage.Location = New System.Drawing.Point(0, 0)
        Me.recMessage.Name = "recMessage"
        Me.recMessage.Options.Fields.UseCurrentCultureDateTimeFormat = False
        Me.recMessage.Options.MailMerge.KeepLastParagraph = False
        Me.recMessage.Size = New System.Drawing.Size(1127, 593)
        Me.recMessage.TabIndex = 0
        '
        'PopupMessage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 19.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1127, 593)
        Me.Controls.Add(Me.recMessage)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Name = "PopupMessage"
        Me.Text = "Mail Message"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents recMessage As DevExpress.XtraRichEdit.RichEditControl
End Class
