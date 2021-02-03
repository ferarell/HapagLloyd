<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IncomeReconciliationForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IncomeReconciliationForm))
        Me.bmActions = New DevExpress.XtraBars.BarManager()
        Me.bar5 = New DevExpress.XtraBars.Bar()
        Me.brsDescripcion = New DevExpress.XtraBars.BarStaticItem()
        Me.brBarraAcciones = New DevExpress.XtraBars.Bar()
        Me.bbiProcess = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiClose = New DevExpress.XtraBars.BarButtonItem()
        Me.BarDockControl1 = New DevExpress.XtraBars.BarDockControl()
        Me.BarDockControl2 = New DevExpress.XtraBars.BarDockControl()
        Me.BarDockControl3 = New DevExpress.XtraBars.BarDockControl()
        Me.BarDockControl4 = New DevExpress.XtraBars.BarDockControl()
        Me.BarButtonItem1 = New DevExpress.XtraBars.BarButtonItem()
        Me.bsiVistas = New DevExpress.XtraBars.BarSubItem()
        Me.bbiVistaGrilla = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiTarjeta = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiContrato = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiCronograma = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiCartaNotarial = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiLetras = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonItem3 = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonItem4 = New DevExpress.XtraBars.BarButtonItem()
        Me.rpiProceso = New DevExpress.XtraEditors.Repository.RepositoryItemProgressBar()
        Me.RepositoryItemLookUpEdit1 = New DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit()
        Me.RepositoryItemImageComboBox1 = New DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox()
        Me.RepositoryItemRadioGroup1 = New DevExpress.XtraEditors.Repository.RepositoryItemRadioGroup()
        Me.RepositoryItemComboBox1 = New DevExpress.XtraEditors.Repository.RepositoryItemComboBox()
        Me.vpInputs = New DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.PanelControl1 = New DevExpress.XtraEditors.PanelControl()
        Me.LabelControl4 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl3 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl1 = New DevExpress.XtraEditors.LabelControl()
        Me.beInWebFocus = New DevExpress.XtraEditors.ButtonEdit()
        Me.beConcessions = New DevExpress.XtraEditors.ButtonEdit()
        Me.beOutFis = New DevExpress.XtraEditors.ButtonEdit()
        Me.beInFis = New DevExpress.XtraEditors.ButtonEdit()
        Me.DefaultLookAndFeel1 = New DevExpress.LookAndFeel.DefaultLookAndFeel()
        Me.beOutWebFocus = New DevExpress.XtraEditors.ButtonEdit()
        Me.LabelControl5 = New DevExpress.XtraEditors.LabelControl()
        CType(Me.bmActions, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rpiProceso, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemLookUpEdit1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemImageComboBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemRadioGroup1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemComboBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.vpInputs, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl1.SuspendLayout()
        CType(Me.beInWebFocus.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.beConcessions.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.beOutFis.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.beInFis.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.beOutWebFocus.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.bmActions.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.brsDescripcion, Me.bbiProcess, Me.bbiClose, Me.BarButtonItem1, Me.bsiVistas, Me.bbiVistaGrilla, Me.bbiTarjeta, Me.bbiContrato, Me.bbiCronograma, Me.bbiCartaNotarial, Me.bbiLetras, Me.BarButtonItem3, Me.BarButtonItem4})
        Me.bmActions.MaxItemId = 27
        Me.bmActions.RepositoryItems.AddRange(New DevExpress.XtraEditors.Repository.RepositoryItem() {Me.rpiProceso, Me.RepositoryItemLookUpEdit1, Me.RepositoryItemImageComboBox1, Me.RepositoryItemRadioGroup1, Me.RepositoryItemComboBox1})
        '
        'bar5
        '
        Me.bar5.BarName = "Custom 3"
        Me.bar5.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom
        Me.bar5.DockCol = 0
        Me.bar5.DockRow = 0
        Me.bar5.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom
        Me.bar5.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(Me.brsDescripcion)})
        Me.bar5.OptionsBar.AllowQuickCustomization = False
        Me.bar5.OptionsBar.DrawDragBorder = False
        Me.bar5.OptionsBar.MultiLine = True
        Me.bar5.OptionsBar.UseWholeRow = True
        Me.bar5.Text = "Custom 3"
        '
        'brsDescripcion
        '
        Me.brsDescripcion.Id = 30
        Me.brsDescripcion.Name = "brsDescripcion"
        Me.brsDescripcion.TextAlignment = System.Drawing.StringAlignment.Near
        '
        'brBarraAcciones
        '
        Me.brBarraAcciones.BarName = "Custom 5"
        Me.brBarraAcciones.DockCol = 0
        Me.brBarraAcciones.DockRow = 0
        Me.brBarraAcciones.DockStyle = DevExpress.XtraBars.BarDockStyle.Top
        Me.brBarraAcciones.FloatLocation = New System.Drawing.Point(279, 188)
        Me.brBarraAcciones.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.bbiProcess, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph), New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.bbiClose, "", True, True, True, 0, Nothing, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)})
        Me.brBarraAcciones.OptionsBar.AllowQuickCustomization = False
        Me.brBarraAcciones.OptionsBar.UseWholeRow = True
        Me.brBarraAcciones.Text = "Custom 5"
        '
        'bbiProcess
        '
        Me.bbiProcess.Caption = "&Process"
        Me.bbiProcess.Glyph = CType(resources.GetObject("bbiProcess.Glyph"), System.Drawing.Image)
        Me.bbiProcess.Id = 33
        Me.bbiProcess.ImageIndex = 26
        Me.bbiProcess.LargeImageIndex = 7
        Me.bbiProcess.Name = "bbiProcess"
        '
        'bbiClose
        '
        Me.bbiClose.Caption = "&Close"
        Me.bbiClose.Glyph = CType(resources.GetObject("bbiClose.Glyph"), System.Drawing.Image)
        Me.bbiClose.Id = 41
        Me.bbiClose.ImageIndex = 27
        Me.bbiClose.LargeImageIndex = 0
        Me.bbiClose.Name = "bbiClose"
        Me.bbiClose.ShortcutKeyDisplayString = "Alt+C"
        '
        'BarDockControl1
        '
        Me.BarDockControl1.CausesValidation = False
        Me.BarDockControl1.Dock = System.Windows.Forms.DockStyle.Top
        Me.BarDockControl1.Location = New System.Drawing.Point(0, 0)
        Me.BarDockControl1.Size = New System.Drawing.Size(642, 41)
        '
        'BarDockControl2
        '
        Me.BarDockControl2.CausesValidation = False
        Me.BarDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.BarDockControl2.Location = New System.Drawing.Point(0, 199)
        Me.BarDockControl2.Size = New System.Drawing.Size(642, 27)
        '
        'BarDockControl3
        '
        Me.BarDockControl3.CausesValidation = False
        Me.BarDockControl3.Dock = System.Windows.Forms.DockStyle.Left
        Me.BarDockControl3.Location = New System.Drawing.Point(0, 41)
        Me.BarDockControl3.Size = New System.Drawing.Size(0, 158)
        '
        'BarDockControl4
        '
        Me.BarDockControl4.CausesValidation = False
        Me.BarDockControl4.Dock = System.Windows.Forms.DockStyle.Right
        Me.BarDockControl4.Location = New System.Drawing.Point(642, 41)
        Me.BarDockControl4.Size = New System.Drawing.Size(0, 158)
        '
        'BarButtonItem1
        '
        Me.BarButtonItem1.Caption = "Vista"
        Me.BarButtonItem1.Id = 3
        Me.BarButtonItem1.Name = "BarButtonItem1"
        '
        'bsiVistas
        '
        Me.bsiVistas.Caption = "Vistas"
        Me.bsiVistas.Id = 6
        Me.bsiVistas.ImageIndex = 20
        Me.bsiVistas.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(Me.bbiVistaGrilla), New DevExpress.XtraBars.LinkPersistInfo(Me.bbiTarjeta)})
        Me.bsiVistas.Name = "bsiVistas"
        '
        'bbiVistaGrilla
        '
        Me.bbiVistaGrilla.Caption = "Grilla"
        Me.bbiVistaGrilla.Id = 7
        Me.bbiVistaGrilla.ImageIndex = 23
        Me.bbiVistaGrilla.Name = "bbiVistaGrilla"
        '
        'bbiTarjeta
        '
        Me.bbiTarjeta.Caption = "Tarjeta"
        Me.bbiTarjeta.Id = 8
        Me.bbiTarjeta.ImageIndex = 21
        Me.bbiTarjeta.Name = "bbiTarjeta"
        '
        'bbiContrato
        '
        Me.bbiContrato.Caption = "Contrato Compra-Venta"
        Me.bbiContrato.Id = 13
        Me.bbiContrato.Name = "bbiContrato"
        '
        'bbiCronograma
        '
        Me.bbiCronograma.Caption = "Cronograma de Pagos"
        Me.bbiCronograma.Id = 14
        Me.bbiCronograma.Name = "bbiCronograma"
        '
        'bbiCartaNotarial
        '
        Me.bbiCartaNotarial.Caption = "Carta Notarial"
        Me.bbiCartaNotarial.Id = 15
        Me.bbiCartaNotarial.Name = "bbiCartaNotarial"
        '
        'bbiLetras
        '
        Me.bbiLetras.Caption = "Letras de Cambio"
        Me.bbiLetras.Id = 16
        Me.bbiLetras.Name = "bbiLetras"
        '
        'BarButtonItem3
        '
        Me.BarButtonItem3.Caption = "5.1 Movimiento Contable"
        Me.BarButtonItem3.Id = 25
        Me.BarButtonItem3.Name = "BarButtonItem3"
        Me.BarButtonItem3.Tag = "1"
        '
        'BarButtonItem4
        '
        Me.BarButtonItem4.Caption = "5.3 Plan Contable"
        Me.BarButtonItem4.Id = 26
        Me.BarButtonItem4.Name = "BarButtonItem4"
        Me.BarButtonItem4.Tag = "2"
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
        'RepositoryItemRadioGroup1
        '
        Me.RepositoryItemRadioGroup1.Items.AddRange(New DevExpress.XtraEditors.Controls.RadioGroupItem() {New DevExpress.XtraEditors.Controls.RadioGroupItem(Nothing, "Nacionales"), New DevExpress.XtraEditors.Controls.RadioGroupItem(Nothing, "No Domiciliados")})
        Me.RepositoryItemRadioGroup1.Name = "RepositoryItemRadioGroup1"
        '
        'RepositoryItemComboBox1
        '
        Me.RepositoryItemComboBox1.AutoHeight = False
        Me.RepositoryItemComboBox1.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.RepositoryItemComboBox1.Items.AddRange(New Object() {"Nacionales", "No Domiciliados"})
        Me.RepositoryItemComboBox1.Name = "RepositoryItemComboBox1"
        '
        'vpInputs
        '
        Me.vpInputs.ValidationMode = DevExpress.XtraEditors.DXErrorProvider.ValidationMode.Manual
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'PanelControl1
        '
        Me.PanelControl1.Controls.Add(Me.LabelControl5)
        Me.PanelControl1.Controls.Add(Me.LabelControl4)
        Me.PanelControl1.Controls.Add(Me.LabelControl3)
        Me.PanelControl1.Controls.Add(Me.LabelControl2)
        Me.PanelControl1.Controls.Add(Me.LabelControl1)
        Me.PanelControl1.Controls.Add(Me.beOutWebFocus)
        Me.PanelControl1.Controls.Add(Me.beInWebFocus)
        Me.PanelControl1.Controls.Add(Me.beConcessions)
        Me.PanelControl1.Controls.Add(Me.beOutFis)
        Me.PanelControl1.Controls.Add(Me.beInFis)
        Me.PanelControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelControl1.Location = New System.Drawing.Point(0, 41)
        Me.PanelControl1.Name = "PanelControl1"
        Me.PanelControl1.Size = New System.Drawing.Size(642, 158)
        Me.PanelControl1.TabIndex = 4
        '
        'LabelControl4
        '
        Me.LabelControl4.Location = New System.Drawing.Point(43, 96)
        Me.LabelControl4.Name = "LabelControl4"
        Me.LabelControl4.Size = New System.Drawing.Size(67, 13)
        Me.LabelControl4.TabIndex = 11
        Me.LabelControl4.Text = "IN Web Focus"
        '
        'LabelControl3
        '
        Me.LabelControl3.Location = New System.Drawing.Point(51, 72)
        Me.LabelControl3.Name = "LabelControl3"
        Me.LabelControl3.Size = New System.Drawing.Size(59, 13)
        Me.LabelControl3.TabIndex = 11
        Me.LabelControl3.Text = "Concessions"
        '
        'LabelControl2
        '
        Me.LabelControl2.Location = New System.Drawing.Point(70, 48)
        Me.LabelControl2.Name = "LabelControl2"
        Me.LabelControl2.Size = New System.Drawing.Size(40, 13)
        Me.LabelControl2.TabIndex = 11
        Me.LabelControl2.Text = "OUT FIS"
        '
        'LabelControl1
        '
        Me.LabelControl1.Location = New System.Drawing.Point(80, 24)
        Me.LabelControl1.Name = "LabelControl1"
        Me.LabelControl1.Size = New System.Drawing.Size(30, 13)
        Me.LabelControl1.TabIndex = 11
        Me.LabelControl1.Text = "IN FIS"
        '
        'beInWebFocus
        '
        Me.beInWebFocus.Location = New System.Drawing.Point(115, 93)
        Me.beInWebFocus.Margin = New System.Windows.Forms.Padding(2)
        Me.beInWebFocus.Name = "beInWebFocus"
        Me.beInWebFocus.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beInWebFocus.Size = New System.Drawing.Size(456, 20)
        Me.beInWebFocus.TabIndex = 3
        '
        'beConcessions
        '
        Me.beConcessions.Location = New System.Drawing.Point(115, 69)
        Me.beConcessions.Margin = New System.Windows.Forms.Padding(2)
        Me.beConcessions.Name = "beConcessions"
        Me.beConcessions.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beConcessions.Size = New System.Drawing.Size(456, 20)
        Me.beConcessions.TabIndex = 2
        '
        'beOutFis
        '
        Me.beOutFis.Location = New System.Drawing.Point(115, 45)
        Me.beOutFis.Margin = New System.Windows.Forms.Padding(2)
        Me.beOutFis.Name = "beOutFis"
        Me.beOutFis.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beOutFis.Size = New System.Drawing.Size(456, 20)
        Me.beOutFis.TabIndex = 1
        '
        'beInFis
        '
        Me.beInFis.Location = New System.Drawing.Point(115, 21)
        Me.beInFis.Margin = New System.Windows.Forms.Padding(2)
        Me.beInFis.Name = "beInFis"
        Me.beInFis.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beInFis.Size = New System.Drawing.Size(456, 20)
        Me.beInFis.TabIndex = 0
        '
        'DefaultLookAndFeel1
        '
        Me.DefaultLookAndFeel1.LookAndFeel.SkinName = "Office 2007 Silver"
        '
        'beOutWebFocus
        '
        Me.beOutWebFocus.Location = New System.Drawing.Point(115, 117)
        Me.beOutWebFocus.Margin = New System.Windows.Forms.Padding(2)
        Me.beOutWebFocus.Name = "beOutWebFocus"
        Me.beOutWebFocus.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beOutWebFocus.Size = New System.Drawing.Size(456, 20)
        Me.beOutWebFocus.TabIndex = 4
        '
        'LabelControl5
        '
        Me.LabelControl5.Location = New System.Drawing.Point(33, 120)
        Me.LabelControl5.Name = "LabelControl5"
        Me.LabelControl5.Size = New System.Drawing.Size(77, 13)
        Me.LabelControl5.TabIndex = 11
        Me.LabelControl5.Text = "OUT Web Focus"
        '
        'IncomeReconciliationForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(642, 226)
        Me.Controls.Add(Me.PanelControl1)
        Me.Controls.Add(Me.BarDockControl3)
        Me.Controls.Add(Me.BarDockControl4)
        Me.Controls.Add(Me.BarDockControl2)
        Me.Controls.Add(Me.BarDockControl1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "IncomeReconciliationForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Income Reconciliation"
        CType(Me.bmActions, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rpiProceso, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemLookUpEdit1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemImageComboBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemRadioGroup1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemComboBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.vpInputs, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl1.ResumeLayout(False)
        Me.PanelControl1.PerformLayout()
        CType(Me.beInWebFocus.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.beConcessions.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.beOutFis.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.beInFis.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.beOutWebFocus.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents bmActions As DevExpress.XtraBars.BarManager
    Private WithEvents bar5 As DevExpress.XtraBars.Bar
    Private WithEvents brsDescripcion As DevExpress.XtraBars.BarStaticItem
    Private WithEvents brBarraAcciones As DevExpress.XtraBars.Bar
    Private WithEvents bbiProcess As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonItem3 As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonItem4 As DevExpress.XtraBars.BarButtonItem
    Private WithEvents bbiClose As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BarDockControl1 As DevExpress.XtraBars.BarDockControl
    Private WithEvents BarDockControl2 As DevExpress.XtraBars.BarDockControl
    Private WithEvents BarDockControl3 As DevExpress.XtraBars.BarDockControl
    Private WithEvents BarDockControl4 As DevExpress.XtraBars.BarDockControl
    Friend WithEvents BarButtonItem1 As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bsiVistas As DevExpress.XtraBars.BarSubItem
    Friend WithEvents bbiVistaGrilla As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bbiTarjeta As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bbiContrato As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bbiCronograma As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bbiCartaNotarial As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bbiLetras As DevExpress.XtraBars.BarButtonItem
    Private WithEvents rpiProceso As DevExpress.XtraEditors.Repository.RepositoryItemProgressBar
    Friend WithEvents RepositoryItemLookUpEdit1 As DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit
    Friend WithEvents RepositoryItemImageComboBox1 As DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox
    Friend WithEvents RepositoryItemRadioGroup1 As DevExpress.XtraEditors.Repository.RepositoryItemRadioGroup
    Friend WithEvents RepositoryItemComboBox1 As DevExpress.XtraEditors.Repository.RepositoryItemComboBox
    Friend WithEvents vpInputs As DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents PanelControl1 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents beInWebFocus As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents beConcessions As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents beOutFis As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents beInFis As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents LabelControl3 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl4 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents DefaultLookAndFeel1 As DevExpress.LookAndFeel.DefaultLookAndFeel
    Friend WithEvents LabelControl5 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents beOutWebFocus As DevExpress.XtraEditors.ButtonEdit
End Class
