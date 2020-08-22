<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SpecialRatesForm
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SpecialRatesForm))
        Me.PanelControl1 = New DevExpress.XtraEditors.PanelControl()
        Me.RadioGroup1 = New DevExpress.XtraEditors.RadioGroup()
        Me.bbiProcesss = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiShowAll = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiExport = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiClose = New DevExpress.XtraBars.BarButtonItem()
        Me.BarDockControl4 = New DevExpress.XtraBars.BarDockControl()
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadioGroup1.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelControl1
        '
        Me.PanelControl1.Location = New System.Drawing.Point(0, 0)
        Me.PanelControl1.Name = "PanelControl1"
        Me.PanelControl1.Size = New System.Drawing.Size(200, 100)
        Me.PanelControl1.TabIndex = 0
        '
        'RadioGroup1
        '
        Me.RadioGroup1.EditValue = "A"
        Me.RadioGroup1.Location = New System.Drawing.Point(123, 9)
        Me.RadioGroup1.Name = "RadioGroup1"
        Me.RadioGroup1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.RadioGroup1.Properties.Appearance.Options.UseBackColor = True
        Me.RadioGroup1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.RadioGroup1.Size = New System.Drawing.Size(100, 96)
        Me.RadioGroup1.TabIndex = 0
        '
        'bbiProcesss
        '
        Me.bbiProcesss.Caption = "&Process"
        Me.bbiProcesss.Glyph = CType(resources.GetObject("bbiProcesss.Glyph"), System.Drawing.Image)
        Me.bbiProcesss.Id = 33
        Me.bbiProcesss.ImageIndex = 26
        Me.bbiProcesss.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.P))
        Me.bbiProcesss.LargeImageIndex = 7
        Me.bbiProcesss.Name = "bbiProcesss"
        '
        'bbiShowAll
        '
        Me.bbiShowAll.Caption = "&Show All"
        Me.bbiShowAll.Glyph = CType(resources.GetObject("bbiShowAll.Glyph"), System.Drawing.Image)
        Me.bbiShowAll.Id = 31
        Me.bbiShowAll.Name = "bbiShowAll"
        '
        'bbiExport
        '
        Me.bbiExport.Caption = "&Export"
        Me.bbiExport.Id = 21
        Me.bbiExport.ImageIndex = 29
        Me.bbiExport.Name = "bbiExport"
        Me.bbiExport.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph
        '
        'bbiClose
        '
        Me.bbiClose.Caption = "&Close"
        Me.bbiClose.Glyph = CType(resources.GetObject("bbiClose.Glyph"), System.Drawing.Image)
        Me.bbiClose.Id = 41
        Me.bbiClose.ImageIndex = 27
        Me.bbiClose.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.C))
        Me.bbiClose.LargeImageIndex = 0
        Me.bbiClose.Name = "bbiClose"
        Me.bbiClose.ShortcutKeyDisplayString = "Alt+C"
        '
        'BarDockControl4
        '
        Me.BarDockControl4.CausesValidation = False
        Me.BarDockControl4.Dock = System.Windows.Forms.DockStyle.Right
        Me.BarDockControl4.Location = New System.Drawing.Point(740, 0)
        Me.BarDockControl4.Size = New System.Drawing.Size(0, 422)
        '
        'SpecialRatesForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(740, 422)
        Me.Controls.Add(Me.BarDockControl4)
        Me.Name = "SpecialRatesForm"
        Me.Text = "Special Rates"
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadioGroup1.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelControl1 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents RadioGroup1 As DevExpress.XtraEditors.RadioGroup
    Private WithEvents bbiProcesss As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bbiShowAll As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bbiExport As DevExpress.XtraBars.BarButtonItem
    Private WithEvents bbiClose As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BarDockControl4 As DevExpress.XtraBars.BarDockControl
End Class
