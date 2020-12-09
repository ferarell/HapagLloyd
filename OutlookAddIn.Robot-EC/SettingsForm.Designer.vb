<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SettingsForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SettingsForm))
        Me.tePassword = New DevExpress.XtraEditors.TextEdit()
        Me.teUserName = New DevExpress.XtraEditors.TextEdit()
        Me.beDatabase = New DevExpress.XtraEditors.ButtonEdit()
        Me.LabelControl5 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl4 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl1 = New DevExpress.XtraEditors.LabelControl()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.GroupControl1 = New DevExpress.XtraEditors.GroupControl()
        Me.lbcEstilos = New DevExpress.XtraEditors.ListBoxControl()
        Me.SplitContainerControl1 = New DevExpress.XtraEditors.SplitContainerControl()
        Me.GroupControl3 = New DevExpress.XtraEditors.GroupControl()
        Me.beProcessFilePath = New DevExpress.XtraEditors.ButtonEdit()
        Me.beLogFilePath = New DevExpress.XtraEditors.ButtonEdit()
        Me.LabelControl7 = New DevExpress.XtraEditors.LabelControl()
        Me.beAttachedFilePath = New DevExpress.XtraEditors.ButtonEdit()
        Me.LabelControl6 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl3 = New DevExpress.XtraEditors.LabelControl()
        Me.GroupControl2 = New DevExpress.XtraEditors.GroupControl()
        Me.teConfigTable = New DevExpress.XtraEditors.TextEdit()
        Me.lueConfigTable = New DevExpress.XtraEditors.LookUpEdit()
        Me.BarManager1 = New DevExpress.XtraBars.BarManager(Me.components)
        Me.Bar1 = New DevExpress.XtraBars.Bar()
        Me.bbiSave = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiRefresh = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiClose = New DevExpress.XtraBars.BarButtonItem()
        Me.barDockControlTop = New DevExpress.XtraBars.BarDockControl()
        Me.barDockControlBottom = New DevExpress.XtraBars.BarDockControl()
        Me.barDockControlLeft = New DevExpress.XtraBars.BarDockControl()
        Me.barDockControlRight = New DevExpress.XtraBars.BarDockControl()
        Me.vpInputs = New DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(Me.components)
        Me.XtraFolderBrowserDialog1 = New DevExpress.XtraEditors.XtraFolderBrowserDialog(Me.components)
        Me.bbiOpenDB = New DevExpress.XtraBars.BarButtonItem()
        CType(Me.tePassword.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.teUserName.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.beDatabase.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GroupControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupControl1.SuspendLayout()
        CType(Me.lbcEstilos, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainerControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerControl1.SuspendLayout()
        CType(Me.GroupControl3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupControl3.SuspendLayout()
        CType(Me.beProcessFilePath.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.beLogFilePath.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.beAttachedFilePath.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GroupControl2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupControl2.SuspendLayout()
        CType(Me.teConfigTable.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lueConfigTable.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BarManager1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.vpInputs, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tePassword
        '
        Me.tePassword.Location = New System.Drawing.Point(121, 117)
        Me.tePassword.Name = "tePassword"
        Me.tePassword.Properties.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.tePassword.Properties.UseSystemPasswordChar = True
        Me.tePassword.Size = New System.Drawing.Size(165, 20)
        Me.tePassword.TabIndex = 3
        '
        'teUserName
        '
        Me.teUserName.Location = New System.Drawing.Point(121, 91)
        Me.teUserName.Name = "teUserName"
        Me.teUserName.Size = New System.Drawing.Size(165, 20)
        Me.teUserName.TabIndex = 2
        '
        'beDatabase
        '
        Me.beDatabase.Location = New System.Drawing.Point(121, 38)
        Me.beDatabase.Name = "beDatabase"
        Me.beDatabase.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beDatabase.Size = New System.Drawing.Size(367, 20)
        Me.beDatabase.TabIndex = 0
        '
        'LabelControl5
        '
        Me.LabelControl5.Location = New System.Drawing.Point(69, 120)
        Me.LabelControl5.Name = "LabelControl5"
        Me.LabelControl5.Size = New System.Drawing.Size(46, 13)
        Me.LabelControl5.TabIndex = 4
        Me.LabelControl5.Text = "Password"
        '
        'LabelControl4
        '
        Me.LabelControl4.Location = New System.Drawing.Point(93, 94)
        Me.LabelControl4.Name = "LabelControl4"
        Me.LabelControl4.Size = New System.Drawing.Size(22, 13)
        Me.LabelControl4.TabIndex = 5
        Me.LabelControl4.Text = "User"
        '
        'LabelControl2
        '
        Me.LabelControl2.Location = New System.Drawing.Point(89, 68)
        Me.LabelControl2.Name = "LabelControl2"
        Me.LabelControl2.Size = New System.Drawing.Size(26, 13)
        Me.LabelControl2.TabIndex = 7
        Me.LabelControl2.Text = "Table"
        '
        'LabelControl1
        '
        Me.LabelControl1.Location = New System.Drawing.Point(69, 41)
        Me.LabelControl1.Name = "LabelControl1"
        Me.LabelControl1.Size = New System.Drawing.Size(46, 13)
        Me.LabelControl1.TabIndex = 8
        Me.LabelControl1.Text = "Database"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'GroupControl1
        '
        Me.GroupControl1.Controls.Add(Me.lbcEstilos)
        Me.GroupControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupControl1.Location = New System.Drawing.Point(0, 0)
        Me.GroupControl1.Name = "GroupControl1"
        Me.GroupControl1.Size = New System.Drawing.Size(247, 380)
        Me.GroupControl1.TabIndex = 5
        Me.GroupControl1.Text = "Look And Feel Style"
        '
        'lbcEstilos
        '
        Me.lbcEstilos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbcEstilos.Location = New System.Drawing.Point(2, 20)
        Me.lbcEstilos.Name = "lbcEstilos"
        Me.lbcEstilos.Size = New System.Drawing.Size(243, 358)
        Me.lbcEstilos.TabIndex = 0
        '
        'SplitContainerControl1
        '
        Me.SplitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainerControl1.Location = New System.Drawing.Point(5, 48)
        Me.SplitContainerControl1.Name = "SplitContainerControl1"
        Me.SplitContainerControl1.Padding = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.SplitContainerControl1.Panel1.Controls.Add(Me.GroupControl1)
        Me.SplitContainerControl1.Panel1.Text = "Panel1"
        Me.SplitContainerControl1.Panel2.Controls.Add(Me.GroupControl3)
        Me.SplitContainerControl1.Panel2.Controls.Add(Me.GroupControl2)
        Me.SplitContainerControl1.Panel2.Text = "Panel2"
        Me.SplitContainerControl1.Size = New System.Drawing.Size(812, 390)
        Me.SplitContainerControl1.SplitterPosition = 247
        Me.SplitContainerControl1.TabIndex = 6
        '
        'GroupControl3
        '
        Me.GroupControl3.Controls.Add(Me.beProcessFilePath)
        Me.GroupControl3.Controls.Add(Me.beLogFilePath)
        Me.GroupControl3.Controls.Add(Me.LabelControl7)
        Me.GroupControl3.Controls.Add(Me.beAttachedFilePath)
        Me.GroupControl3.Controls.Add(Me.LabelControl6)
        Me.GroupControl3.Controls.Add(Me.LabelControl3)
        Me.GroupControl3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupControl3.Location = New System.Drawing.Point(0, 160)
        Me.GroupControl3.Name = "GroupControl3"
        Me.GroupControl3.Size = New System.Drawing.Size(560, 220)
        Me.GroupControl3.TabIndex = 1
        Me.GroupControl3.Text = "General"
        '
        'beProcessFilePath
        '
        Me.beProcessFilePath.Location = New System.Drawing.Point(121, 96)
        Me.beProcessFilePath.Name = "beProcessFilePath"
        Me.beProcessFilePath.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beProcessFilePath.Size = New System.Drawing.Size(367, 20)
        Me.beProcessFilePath.TabIndex = 2
        '
        'beLogFilePath
        '
        Me.beLogFilePath.Location = New System.Drawing.Point(121, 70)
        Me.beLogFilePath.Name = "beLogFilePath"
        Me.beLogFilePath.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beLogFilePath.Size = New System.Drawing.Size(367, 20)
        Me.beLogFilePath.TabIndex = 1
        '
        'LabelControl7
        '
        Me.LabelControl7.Location = New System.Drawing.Point(34, 99)
        Me.LabelControl7.Name = "LabelControl7"
        Me.LabelControl7.Size = New System.Drawing.Size(81, 13)
        Me.LabelControl7.TabIndex = 8
        Me.LabelControl7.Text = "Process File Path"
        '
        'beAttachedFilePath
        '
        Me.beAttachedFilePath.Location = New System.Drawing.Point(121, 44)
        Me.beAttachedFilePath.Name = "beAttachedFilePath"
        Me.beAttachedFilePath.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beAttachedFilePath.Size = New System.Drawing.Size(367, 20)
        Me.beAttachedFilePath.TabIndex = 0
        '
        'LabelControl6
        '
        Me.LabelControl6.Location = New System.Drawing.Point(54, 73)
        Me.LabelControl6.Name = "LabelControl6"
        Me.LabelControl6.Size = New System.Drawing.Size(61, 13)
        Me.LabelControl6.TabIndex = 8
        Me.LabelControl6.Text = "Log File Path"
        '
        'LabelControl3
        '
        Me.LabelControl3.Location = New System.Drawing.Point(27, 47)
        Me.LabelControl3.Name = "LabelControl3"
        Me.LabelControl3.Size = New System.Drawing.Size(88, 13)
        Me.LabelControl3.TabIndex = 8
        Me.LabelControl3.Text = "Attached File Path"
        '
        'GroupControl2
        '
        Me.GroupControl2.Controls.Add(Me.teConfigTable)
        Me.GroupControl2.Controls.Add(Me.lueConfigTable)
        Me.GroupControl2.Controls.Add(Me.tePassword)
        Me.GroupControl2.Controls.Add(Me.LabelControl1)
        Me.GroupControl2.Controls.Add(Me.LabelControl2)
        Me.GroupControl2.Controls.Add(Me.teUserName)
        Me.GroupControl2.Controls.Add(Me.LabelControl4)
        Me.GroupControl2.Controls.Add(Me.beDatabase)
        Me.GroupControl2.Controls.Add(Me.LabelControl5)
        Me.GroupControl2.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupControl2.Location = New System.Drawing.Point(0, 0)
        Me.GroupControl2.Name = "GroupControl2"
        Me.GroupControl2.Size = New System.Drawing.Size(560, 160)
        Me.GroupControl2.TabIndex = 0
        Me.GroupControl2.Text = "Database"
        '
        'teConfigTable
        '
        Me.teConfigTable.Location = New System.Drawing.Point(121, 64)
        Me.teConfigTable.Name = "teConfigTable"
        Me.teConfigTable.Size = New System.Drawing.Size(252, 20)
        Me.teConfigTable.TabIndex = 1
        '
        'lueConfigTable
        '
        Me.lueConfigTable.Location = New System.Drawing.Point(415, 117)
        Me.lueConfigTable.Name = "lueConfigTable"
        Me.lueConfigTable.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.lueConfigTable.Properties.NullText = ""
        Me.lueConfigTable.Size = New System.Drawing.Size(73, 20)
        Me.lueConfigTable.TabIndex = 4
        Me.lueConfigTable.Visible = False
        '
        'BarManager1
        '
        Me.BarManager1.Bars.AddRange(New DevExpress.XtraBars.Bar() {Me.Bar1})
        Me.BarManager1.DockControls.Add(Me.barDockControlTop)
        Me.BarManager1.DockControls.Add(Me.barDockControlBottom)
        Me.BarManager1.DockControls.Add(Me.barDockControlLeft)
        Me.BarManager1.DockControls.Add(Me.barDockControlRight)
        Me.BarManager1.Form = Me
        Me.BarManager1.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.bbiSave, Me.bbiRefresh, Me.bbiClose, Me.bbiOpenDB})
        Me.BarManager1.MaxItemId = 4
        '
        'Bar1
        '
        Me.Bar1.BarName = "Herramientas"
        Me.Bar1.DockCol = 0
        Me.Bar1.DockRow = 0
        Me.Bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top
        Me.Bar1.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.bbiSave, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph), New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.bbiRefresh, "", True, True, True, 0, Nothing, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph), New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.bbiOpenDB, "", True, True, True, 0, Nothing, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph), New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.bbiClose, "", True, True, True, 0, Nothing, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)})
        Me.Bar1.OptionsBar.AllowQuickCustomization = False
        Me.Bar1.OptionsBar.MultiLine = True
        Me.Bar1.OptionsBar.UseWholeRow = True
        Me.Bar1.Text = "Herramientas"
        '
        'bbiSave
        '
        Me.bbiSave.Caption = "&Save"
        Me.bbiSave.Id = 0
        Me.bbiSave.ImageOptions.Image = CType(resources.GetObject("bbiSave.ImageOptions.Image"), System.Drawing.Image)
        Me.bbiSave.ImageOptions.LargeImage = CType(resources.GetObject("bbiSave.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.bbiSave.Name = "bbiSave"
        '
        'bbiRefresh
        '
        Me.bbiRefresh.Caption = "&Refresh"
        Me.bbiRefresh.Id = 1
        Me.bbiRefresh.ImageOptions.Image = CType(resources.GetObject("bbiRefresh.ImageOptions.Image"), System.Drawing.Image)
        Me.bbiRefresh.Name = "bbiRefresh"
        '
        'bbiClose
        '
        Me.bbiClose.Caption = "&Close"
        Me.bbiClose.Id = 2
        Me.bbiClose.ImageOptions.Image = CType(resources.GetObject("bbiClose.ImageOptions.Image"), System.Drawing.Image)
        Me.bbiClose.Name = "bbiClose"
        '
        'barDockControlTop
        '
        Me.barDockControlTop.CausesValidation = False
        Me.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top
        Me.barDockControlTop.Location = New System.Drawing.Point(5, 1)
        Me.barDockControlTop.Manager = Me.BarManager1
        Me.barDockControlTop.Size = New System.Drawing.Size(812, 47)
        '
        'barDockControlBottom
        '
        Me.barDockControlBottom.CausesValidation = False
        Me.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.barDockControlBottom.Location = New System.Drawing.Point(5, 438)
        Me.barDockControlBottom.Manager = Me.BarManager1
        Me.barDockControlBottom.Size = New System.Drawing.Size(812, 0)
        '
        'barDockControlLeft
        '
        Me.barDockControlLeft.CausesValidation = False
        Me.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left
        Me.barDockControlLeft.Location = New System.Drawing.Point(5, 48)
        Me.barDockControlLeft.Manager = Me.BarManager1
        Me.barDockControlLeft.Size = New System.Drawing.Size(0, 390)
        '
        'barDockControlRight
        '
        Me.barDockControlRight.CausesValidation = False
        Me.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right
        Me.barDockControlRight.Location = New System.Drawing.Point(817, 48)
        Me.barDockControlRight.Manager = Me.BarManager1
        Me.barDockControlRight.Size = New System.Drawing.Size(0, 390)
        '
        'vpInputs
        '
        Me.vpInputs.ValidationMode = DevExpress.XtraEditors.DXErrorProvider.ValidationMode.Manual
        '
        'bbiOpenDB
        '
        Me.bbiOpenDB.Caption = "&Open DB"
        Me.bbiOpenDB.Id = 3
        Me.bbiOpenDB.ImageOptions.Image = CType(resources.GetObject("bbiOpenDB.ImageOptions.Image"), System.Drawing.Image)
        Me.bbiOpenDB.Name = "bbiOpenDB"
        '
        'SettingsForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(822, 439)
        Me.Controls.Add(Me.SplitContainerControl1)
        Me.Controls.Add(Me.barDockControlLeft)
        Me.Controls.Add(Me.barDockControlRight)
        Me.Controls.Add(Me.barDockControlBottom)
        Me.Controls.Add(Me.barDockControlTop)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "SettingsForm"
        Me.Padding = New System.Windows.Forms.Padding(5, 1, 5, 1)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Settings"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        CType(Me.tePassword.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.teUserName.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.beDatabase.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GroupControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupControl1.ResumeLayout(False)
        CType(Me.lbcEstilos, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SplitContainerControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerControl1.ResumeLayout(False)
        CType(Me.GroupControl3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupControl3.ResumeLayout(False)
        Me.GroupControl3.PerformLayout()
        CType(Me.beProcessFilePath.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.beLogFilePath.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.beAttachedFilePath.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GroupControl2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupControl2.ResumeLayout(False)
        Me.GroupControl2.PerformLayout()
        CType(Me.teConfigTable.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lueConfigTable.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BarManager1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.vpInputs, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tePassword As DevExpress.XtraEditors.TextEdit
    Friend WithEvents teUserName As DevExpress.XtraEditors.TextEdit
    Friend WithEvents beDatabase As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents LabelControl5 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl4 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents GroupControl1 As DevExpress.XtraEditors.GroupControl
    Friend WithEvents SplitContainerControl1 As DevExpress.XtraEditors.SplitContainerControl
    Friend WithEvents GroupControl2 As DevExpress.XtraEditors.GroupControl
    Friend WithEvents lueConfigTable As DevExpress.XtraEditors.LookUpEdit
    Friend WithEvents BarManager1 As DevExpress.XtraBars.BarManager
    Friend WithEvents Bar1 As DevExpress.XtraBars.Bar
    Friend WithEvents bbiSave As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents barDockControlTop As DevExpress.XtraBars.BarDockControl
    Friend WithEvents barDockControlBottom As DevExpress.XtraBars.BarDockControl
    Friend WithEvents barDockControlLeft As DevExpress.XtraBars.BarDockControl
    Friend WithEvents barDockControlRight As DevExpress.XtraBars.BarDockControl
    Friend WithEvents bbiRefresh As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bbiClose As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents lbcEstilos As DevExpress.XtraEditors.ListBoxControl
    Friend WithEvents teConfigTable As DevExpress.XtraEditors.TextEdit
    Friend WithEvents vpInputs As DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider
    Friend WithEvents GroupControl3 As DevExpress.XtraEditors.GroupControl
    Friend WithEvents beLogFilePath As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents beAttachedFilePath As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents LabelControl6 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl3 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents XtraFolderBrowserDialog1 As DevExpress.XtraEditors.XtraFolderBrowserDialog
    Friend WithEvents beProcessFilePath As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents LabelControl7 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents bbiOpenDB As DevExpress.XtraBars.BarButtonItem
End Class
