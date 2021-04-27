<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PreferencesForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PreferencesForm))
        Me.bmActions = New DevExpress.XtraBars.BarManager()
        Me.bar5 = New DevExpress.XtraBars.Bar()
        Me.brBarraAcciones = New DevExpress.XtraBars.Bar()
        Me.bbiGuardar = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiReset = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiCerrar = New DevExpress.XtraBars.BarButtonItem()
        Me.BarDockControl1 = New DevExpress.XtraBars.BarDockControl()
        Me.BarDockControl2 = New DevExpress.XtraBars.BarDockControl()
        Me.BarDockControl3 = New DevExpress.XtraBars.BarDockControl()
        Me.BarDockControl4 = New DevExpress.XtraBars.BarDockControl()
        Me.imActionsBar24x24 = New System.Windows.Forms.ImageList()
        Me.rpiProceso = New DevExpress.XtraEditors.Repository.RepositoryItemProgressBar()
        Me.RepositoryItemLookUpEdit1 = New DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit()
        Me.RepositoryItemImageComboBox1 = New DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox()
        Me.GroupControl2 = New DevExpress.XtraEditors.GroupControl()
        Me.seWaitTime = New DevExpress.XtraEditors.SpinEdit()
        Me.seEQuantity = New DevExpress.XtraEditors.SpinEdit()
        Me.LabelControl4 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl3 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.lueCountry = New DevExpress.XtraEditors.LookUpEdit()
        Me.LabelControl1 = New DevExpress.XtraEditors.LabelControl()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.SplitContainerControl1 = New DevExpress.XtraEditors.SplitContainerControl()
        Me.gcEstilos = New DevExpress.XtraEditors.GroupControl()
        Me.lbcEstilos = New DevExpress.XtraEditors.ListBoxControl()
        Me.rgPaintStyle = New DevExpress.XtraEditors.RadioGroup()
        CType(Me.bmActions, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rpiProceso, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemLookUpEdit1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemImageComboBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GroupControl2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupControl2.SuspendLayout()
        CType(Me.seWaitTime.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.seEQuantity.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lueCountry.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainerControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerControl1.SuspendLayout()
        CType(Me.gcEstilos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gcEstilos.SuspendLayout()
        CType(Me.lbcEstilos, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rgPaintStyle.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bmActions
        '
        Me.bmActions.Bars.AddRange(New DevExpress.XtraBars.Bar() {Me.bar5, Me.brBarraAcciones})
        Me.bmActions.DockControls.Add(Me.BarDockControl1)
        Me.bmActions.DockControls.Add(Me.BarDockControl2)
        Me.bmActions.DockControls.Add(Me.BarDockControl3)
        Me.bmActions.DockControls.Add(Me.BarDockControl4)
        Me.bmActions.Form = Me
        Me.bmActions.Images = Me.imActionsBar24x24
        Me.bmActions.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.bbiGuardar, Me.bbiCerrar, Me.bbiReset})
        Me.bmActions.MaxItemId = 21
        Me.bmActions.RepositoryItems.AddRange(New DevExpress.XtraEditors.Repository.RepositoryItem() {Me.rpiProceso, Me.RepositoryItemLookUpEdit1, Me.RepositoryItemImageComboBox1})
        '
        'bar5
        '
        Me.bar5.BarName = "Custom 3"
        Me.bar5.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom
        Me.bar5.DockCol = 0
        Me.bar5.DockRow = 0
        Me.bar5.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom
        Me.bar5.OptionsBar.AllowQuickCustomization = False
        Me.bar5.OptionsBar.DrawDragBorder = False
        Me.bar5.OptionsBar.MultiLine = True
        Me.bar5.OptionsBar.UseWholeRow = True
        Me.bar5.Text = "Custom 3"
        '
        'brBarraAcciones
        '
        Me.brBarraAcciones.BarName = "Custom 5"
        Me.brBarraAcciones.DockCol = 0
        Me.brBarraAcciones.DockRow = 0
        Me.brBarraAcciones.DockStyle = DevExpress.XtraBars.BarDockStyle.Top
        Me.brBarraAcciones.FloatLocation = New System.Drawing.Point(279, 188)
        Me.brBarraAcciones.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.bbiGuardar, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph), New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.bbiReset, "", True, True, True, 0, Nothing, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph), New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.bbiCerrar, "", True, True, True, 0, Nothing, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)})
        Me.brBarraAcciones.OptionsBar.AllowQuickCustomization = False
        Me.brBarraAcciones.OptionsBar.UseWholeRow = True
        Me.brBarraAcciones.Text = "Custom 5"
        '
        'bbiGuardar
        '
        Me.bbiGuardar.Caption = "&Save"
        Me.bbiGuardar.Glyph = CType(resources.GetObject("bbiGuardar.Glyph"), System.Drawing.Image)
        Me.bbiGuardar.Id = 33
        Me.bbiGuardar.ImageIndex = 28
        Me.bbiGuardar.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.G))
        Me.bbiGuardar.LargeImageIndex = 7
        Me.bbiGuardar.Name = "bbiGuardar"
        '
        'bbiReset
        '
        Me.bbiReset.Caption = "Reset"
        Me.bbiReset.Glyph = CType(resources.GetObject("bbiReset.Glyph"), System.Drawing.Image)
        Me.bbiReset.Id = 20
        Me.bbiReset.Name = "bbiReset"
        '
        'bbiCerrar
        '
        Me.bbiCerrar.Caption = "&Close"
        Me.bbiCerrar.Glyph = CType(resources.GetObject("bbiCerrar.Glyph"), System.Drawing.Image)
        Me.bbiCerrar.Id = 41
        Me.bbiCerrar.ImageIndex = 27
        Me.bbiCerrar.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.C))
        Me.bbiCerrar.LargeImageIndex = 0
        Me.bbiCerrar.Name = "bbiCerrar"
        Me.bbiCerrar.ShortcutKeyDisplayString = "Ctrl+C"
        '
        'BarDockControl1
        '
        Me.BarDockControl1.CausesValidation = False
        Me.BarDockControl1.Dock = System.Windows.Forms.DockStyle.Top
        Me.BarDockControl1.Location = New System.Drawing.Point(0, 0)
        Me.BarDockControl1.Size = New System.Drawing.Size(901, 41)
        '
        'BarDockControl2
        '
        Me.BarDockControl2.CausesValidation = False
        Me.BarDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.BarDockControl2.Location = New System.Drawing.Point(0, 479)
        Me.BarDockControl2.Size = New System.Drawing.Size(901, 23)
        '
        'BarDockControl3
        '
        Me.BarDockControl3.CausesValidation = False
        Me.BarDockControl3.Dock = System.Windows.Forms.DockStyle.Left
        Me.BarDockControl3.Location = New System.Drawing.Point(0, 41)
        Me.BarDockControl3.Size = New System.Drawing.Size(0, 438)
        '
        'BarDockControl4
        '
        Me.BarDockControl4.CausesValidation = False
        Me.BarDockControl4.Dock = System.Windows.Forms.DockStyle.Right
        Me.BarDockControl4.Location = New System.Drawing.Point(901, 41)
        Me.BarDockControl4.Size = New System.Drawing.Size(0, 438)
        '
        'imActionsBar24x24
        '
        Me.imActionsBar24x24.ImageStream = CType(resources.GetObject("imActionsBar24x24.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imActionsBar24x24.TransparentColor = System.Drawing.Color.Transparent
        Me.imActionsBar24x24.Images.SetKeyName(0, "ic_save.png")
        Me.imActionsBar24x24.Images.SetKeyName(1, "ic_edit.png")
        Me.imActionsBar24x24.Images.SetKeyName(2, "ic_new.png")
        Me.imActionsBar24x24.Images.SetKeyName(3, "ic_copy.png")
        Me.imActionsBar24x24.Images.SetKeyName(4, "ic_delete2.png")
        Me.imActionsBar24x24.Images.SetKeyName(5, "ic_print.png")
        Me.imActionsBar24x24.Images.SetKeyName(6, "ic_search2.png")
        Me.imActionsBar24x24.Images.SetKeyName(7, "ic_search3.png")
        Me.imActionsBar24x24.Images.SetKeyName(8, "ic_undo.png")
        Me.imActionsBar24x24.Images.SetKeyName(9, "ic_close.png")
        Me.imActionsBar24x24.Images.SetKeyName(10, "ic_save.png")
        Me.imActionsBar24x24.Images.SetKeyName(11, "ic_search2.png")
        Me.imActionsBar24x24.Images.SetKeyName(12, "ic_print.png")
        Me.imActionsBar24x24.Images.SetKeyName(13, "ic_search3.png")
        Me.imActionsBar24x24.Images.SetKeyName(14, "ic_mntTablas16x16.png")
        Me.imActionsBar24x24.Images.SetKeyName(15, "ic_first16x16.png")
        Me.imActionsBar24x24.Images.SetKeyName(16, "ic_previus16x16.png")
        Me.imActionsBar24x24.Images.SetKeyName(17, "ic_next16x16.png")
        Me.imActionsBar24x24.Images.SetKeyName(18, "ic_last16x16.png")
        Me.imActionsBar24x24.Images.SetKeyName(19, "ic_excel16x16.png")
        Me.imActionsBar24x24.Images.SetKeyName(20, "ci_views16x16.png")
        Me.imActionsBar24x24.Images.SetKeyName(21, "ic_cardview16x16.png")
        Me.imActionsBar24x24.Images.SetKeyName(22, "ic_carouselview16x16.png")
        Me.imActionsBar24x24.Images.SetKeyName(23, "ic_gridview16x16.png")
        Me.imActionsBar24x24.Images.SetKeyName(24, "Excel.ico")
        Me.imActionsBar24x24.Images.SetKeyName(25, "previmg.png")
        Me.imActionsBar24x24.Images.SetKeyName(26, "Pinion.png")
        Me.imActionsBar24x24.Images.SetKeyName(27, "Close.png")
        Me.imActionsBar24x24.Images.SetKeyName(28, "Save.png")
        '
        'rpiProceso
        '
        Me.rpiProceso.Name = "rpiProceso"
        Me.rpiProceso.ShowTitle = True
        '
        'RepositoryItemLookUpEdit1
        '
        Me.RepositoryItemLookUpEdit1.AutoHeight = False
        Me.RepositoryItemLookUpEdit1.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.RepositoryItemLookUpEdit1.Name = "RepositoryItemLookUpEdit1"
        '
        'RepositoryItemImageComboBox1
        '
        Me.RepositoryItemImageComboBox1.AutoHeight = False
        Me.RepositoryItemImageComboBox1.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo), New DevExpress.XtraEditors.Controls.EditorButton(), New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.RepositoryItemImageComboBox1.Name = "RepositoryItemImageComboBox1"
        '
        'GroupControl2
        '
        Me.GroupControl2.Controls.Add(Me.seWaitTime)
        Me.GroupControl2.Controls.Add(Me.seEQuantity)
        Me.GroupControl2.Controls.Add(Me.LabelControl4)
        Me.GroupControl2.Controls.Add(Me.LabelControl3)
        Me.GroupControl2.Controls.Add(Me.LabelControl2)
        Me.GroupControl2.Controls.Add(Me.lueCountry)
        Me.GroupControl2.Controls.Add(Me.LabelControl1)
        Me.GroupControl2.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupControl2.Location = New System.Drawing.Point(0, 0)
        Me.GroupControl2.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupControl2.Name = "GroupControl2"
        Me.GroupControl2.Size = New System.Drawing.Size(622, 131)
        Me.GroupControl2.TabIndex = 0
        Me.GroupControl2.Text = "General"
        '
        'seWaitTime
        '
        Me.seWaitTime.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.seWaitTime.Location = New System.Drawing.Point(162, 92)
        Me.seWaitTime.Name = "seWaitTime"
        Me.seWaitTime.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.seWaitTime.Size = New System.Drawing.Size(50, 20)
        Me.seWaitTime.TabIndex = 2
        '
        'seEQuantity
        '
        Me.seEQuantity.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.seEQuantity.Location = New System.Drawing.Point(162, 66)
        Me.seEQuantity.MenuManager = Me.bmActions
        Me.seEQuantity.Name = "seEQuantity"
        Me.seEQuantity.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.seEQuantity.Size = New System.Drawing.Size(73, 20)
        Me.seEQuantity.TabIndex = 1
        '
        'LabelControl4
        '
        Me.LabelControl4.Location = New System.Drawing.Point(218, 95)
        Me.LabelControl4.Name = "LabelControl4"
        Me.LabelControl4.Size = New System.Drawing.Size(40, 13)
        Me.LabelControl4.TabIndex = 22
        Me.LabelControl4.Text = "Seconds"
        '
        'LabelControl3
        '
        Me.LabelControl3.Location = New System.Drawing.Point(63, 95)
        Me.LabelControl3.Name = "LabelControl3"
        Me.LabelControl3.Size = New System.Drawing.Size(93, 13)
        Me.LabelControl3.TabIndex = 22
        Me.LabelControl3.Text = "Time Between Mails"
        '
        'LabelControl2
        '
        Me.LabelControl2.Location = New System.Drawing.Point(45, 69)
        Me.LabelControl2.Name = "LabelControl2"
        Me.LabelControl2.Size = New System.Drawing.Size(111, 13)
        Me.LabelControl2.TabIndex = 22
        Me.LabelControl2.Text = "Email Quantity By Send"
        '
        'lueCountry
        '
        Me.lueCountry.EditValue = ""
        Me.lueCountry.Location = New System.Drawing.Point(162, 39)
        Me.lueCountry.MenuManager = Me.bmActions
        Me.lueCountry.Name = "lueCountry"
        Me.lueCountry.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.lueCountry.Properties.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo("CountryCode", "Code", 20, DevExpress.Utils.FormatType.None, "", False, DevExpress.Utils.HorzAlignment.[Default]), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("CountryDescription", "Description")})
        Me.lueCountry.Properties.NullText = ""
        Me.lueCountry.Size = New System.Drawing.Size(126, 20)
        Me.lueCountry.TabIndex = 0
        '
        'LabelControl1
        '
        Me.LabelControl1.Location = New System.Drawing.Point(117, 42)
        Me.LabelControl1.Margin = New System.Windows.Forms.Padding(2)
        Me.LabelControl1.Name = "LabelControl1"
        Me.LabelControl1.Size = New System.Drawing.Size(39, 13)
        Me.LabelControl1.TabIndex = 21
        Me.LabelControl1.Text = "Country"
        '
        'SplitContainerControl1
        '
        Me.SplitContainerControl1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel1
        Me.SplitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainerControl1.Location = New System.Drawing.Point(0, 41)
        Me.SplitContainerControl1.Name = "SplitContainerControl1"
        Me.SplitContainerControl1.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.SplitContainerControl1.Panel1.Controls.Add(Me.gcEstilos)
        Me.SplitContainerControl1.Panel1.Text = "Panel1"
        Me.SplitContainerControl1.Panel2.Controls.Add(Me.GroupControl2)
        Me.SplitContainerControl1.Panel2.Text = "Panel2"
        Me.SplitContainerControl1.Size = New System.Drawing.Size(901, 438)
        Me.SplitContainerControl1.SplitterPosition = 275
        Me.SplitContainerControl1.TabIndex = 5
        Me.SplitContainerControl1.Text = "SplitContainerControl1"
        '
        'gcEstilos
        '
        Me.gcEstilos.Controls.Add(Me.lbcEstilos)
        Me.gcEstilos.Controls.Add(Me.rgPaintStyle)
        Me.gcEstilos.Dock = System.Windows.Forms.DockStyle.Left
        Me.gcEstilos.Location = New System.Drawing.Point(0, 0)
        Me.gcEstilos.Name = "gcEstilos"
        Me.gcEstilos.Size = New System.Drawing.Size(268, 434)
        Me.gcEstilos.TabIndex = 1
        Me.gcEstilos.Text = "Seleccione la nueva apariencia del sistema."
        '
        'lbcEstilos
        '
        Me.lbcEstilos.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.lbcEstilos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbcEstilos.Location = New System.Drawing.Point(2, 57)
        Me.lbcEstilos.Name = "lbcEstilos"
        Me.lbcEstilos.Padding = New System.Windows.Forms.Padding(1)
        Me.lbcEstilos.Size = New System.Drawing.Size(264, 375)
        Me.lbcEstilos.TabIndex = 2
        '
        'rgPaintStyle
        '
        Me.rgPaintStyle.Dock = System.Windows.Forms.DockStyle.Top
        Me.rgPaintStyle.EditValue = "ExplorerBar"
        Me.rgPaintStyle.Location = New System.Drawing.Point(2, 21)
        Me.rgPaintStyle.Margin = New System.Windows.Forms.Padding(2)
        Me.rgPaintStyle.Name = "rgPaintStyle"
        Me.rgPaintStyle.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.rgPaintStyle.Properties.Columns = 2
        Me.rgPaintStyle.Properties.Items.AddRange(New DevExpress.XtraEditors.Controls.RadioGroupItem() {New DevExpress.XtraEditors.Controls.RadioGroupItem("ExplorerBar", "Explorador"), New DevExpress.XtraEditors.Controls.RadioGroupItem("NavigationPane", "Navegador")})
        Me.rgPaintStyle.Size = New System.Drawing.Size(264, 36)
        Me.rgPaintStyle.TabIndex = 1
        '
        'PreferencesForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(901, 502)
        Me.Controls.Add(Me.SplitContainerControl1)
        Me.Controls.Add(Me.BarDockControl3)
        Me.Controls.Add(Me.BarDockControl4)
        Me.Controls.Add(Me.BarDockControl2)
        Me.Controls.Add(Me.BarDockControl1)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "PreferencesForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Preferences"
        CType(Me.bmActions, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rpiProceso, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemLookUpEdit1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemImageComboBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GroupControl2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupControl2.ResumeLayout(False)
        Me.GroupControl2.PerformLayout()
        CType(Me.seWaitTime.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.seEQuantity.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lueCountry.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SplitContainerControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerControl1.ResumeLayout(False)
        CType(Me.gcEstilos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gcEstilos.ResumeLayout(False)
        CType(Me.lbcEstilos, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rgPaintStyle.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents bmActions As DevExpress.XtraBars.BarManager
    Private WithEvents bar5 As DevExpress.XtraBars.Bar
    Private WithEvents rpiProceso As DevExpress.XtraEditors.Repository.RepositoryItemProgressBar
    Private WithEvents brBarraAcciones As DevExpress.XtraBars.Bar
    Private WithEvents bbiGuardar As DevExpress.XtraBars.BarButtonItem
    Private WithEvents bbiCerrar As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BarDockControl1 As DevExpress.XtraBars.BarDockControl
    Private WithEvents BarDockControl2 As DevExpress.XtraBars.BarDockControl
    Private WithEvents BarDockControl3 As DevExpress.XtraBars.BarDockControl
    Private WithEvents BarDockControl4 As DevExpress.XtraBars.BarDockControl
    Private WithEvents imActionsBar24x24 As System.Windows.Forms.ImageList
    Friend WithEvents RepositoryItemLookUpEdit1 As DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit
    Friend WithEvents RepositoryItemImageComboBox1 As DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox
    Friend WithEvents GroupControl2 As DevExpress.XtraEditors.GroupControl
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents bbiReset As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents SplitContainerControl1 As DevExpress.XtraEditors.SplitContainerControl
    Friend WithEvents gcEstilos As DevExpress.XtraEditors.GroupControl
    Friend WithEvents lbcEstilos As DevExpress.XtraEditors.ListBoxControl
    Friend WithEvents rgPaintStyle As DevExpress.XtraEditors.RadioGroup
    Friend WithEvents lueCountry As DevExpress.XtraEditors.LookUpEdit
    Friend WithEvents LabelControl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl3 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents seWaitTime As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents seEQuantity As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents LabelControl4 As DevExpress.XtraEditors.LabelControl
End Class
