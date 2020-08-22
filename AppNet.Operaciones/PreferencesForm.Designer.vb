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
        Me.brsDescripcion = New DevExpress.XtraBars.BarStaticItem()
        Me.barStaticItem3 = New DevExpress.XtraBars.BarStaticItem()
        Me.barStaticItem4 = New DevExpress.XtraBars.BarStaticItem()
        Me.brsEstado = New DevExpress.XtraBars.BarStaticItem()
        Me.beiProceso = New DevExpress.XtraBars.BarEditItem()
        Me.rpiProceso = New DevExpress.XtraEditors.Repository.RepositoryItemProgressBar()
        Me.brBarraAcciones = New DevExpress.XtraBars.Bar()
        Me.bbiGuardar = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiReset = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiCerrar = New DevExpress.XtraBars.BarButtonItem()
        Me.BarDockControl1 = New DevExpress.XtraBars.BarDockControl()
        Me.BarDockControl2 = New DevExpress.XtraBars.BarDockControl()
        Me.BarDockControl3 = New DevExpress.XtraBars.BarDockControl()
        Me.BarDockControl4 = New DevExpress.XtraBars.BarDockControl()
        Me.imActionsBar24x24 = New System.Windows.Forms.ImageList()
        Me.BarButtonItem1 = New DevExpress.XtraBars.BarButtonItem()
        Me.bsiVistas = New DevExpress.XtraBars.BarSubItem()
        Me.bbiVistaGrilla = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiTarjeta = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiContrato = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiCronograma = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiCartaNotarial = New DevExpress.XtraBars.BarButtonItem()
        Me.bbiLetras = New DevExpress.XtraBars.BarButtonItem()
        Me.RepositoryItemLookUpEdit1 = New DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit()
        Me.RepositoryItemImageComboBox1 = New DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox()
        Me.GroupControl2 = New DevExpress.XtraEditors.GroupControl()
        Me.teMaxTemp = New DevExpress.XtraEditors.TextEdit()
        Me.LabelControl15 = New DevExpress.XtraEditors.LabelControl()
        Me.teDBFileName = New DevExpress.XtraEditors.TextEdit()
        Me.teBDFileName = New DevExpress.XtraEditors.TextEdit()
        Me.LabelControl20 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl4 = New DevExpress.XtraEditors.LabelControl()
        Me.beVendorSourcePath = New DevExpress.XtraEditors.ButtonEdit()
        Me.LabelControl18 = New DevExpress.XtraEditors.LabelControl()
        Me.beDataTargetPath = New DevExpress.XtraEditors.ButtonEdit()
        Me.LabelControl17 = New DevExpress.XtraEditors.LabelControl()
        Me.beDatabasePath = New DevExpress.XtraEditors.ButtonEdit()
        Me.LabelControl19 = New DevExpress.XtraEditors.LabelControl()
        Me.beDataSourcePath = New DevExpress.XtraEditors.ButtonEdit()
        Me.LabelControl3 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl16 = New DevExpress.XtraEditors.LabelControl()
        Me.GroupControl1 = New DevExpress.XtraEditors.GroupControl()
        Me.CheckEdit1 = New DevExpress.XtraEditors.CheckEdit()
        Me.ButtonEdit1 = New DevExpress.XtraEditors.ButtonEdit()
        Me.LabelControl10 = New DevExpress.XtraEditors.LabelControl()
        Me.TextEdit4 = New DevExpress.XtraEditors.TextEdit()
        Me.TextEdit3 = New DevExpress.XtraEditors.TextEdit()
        Me.TextEdit2 = New DevExpress.XtraEditors.TextEdit()
        Me.TextEdit1 = New DevExpress.XtraEditors.TextEdit()
        Me.LabelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl9 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl8 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl7 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl1 = New DevExpress.XtraEditors.LabelControl()
        Me.TextEdit5 = New DevExpress.XtraEditors.MemoEdit()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.GroupControl3 = New DevExpress.XtraEditors.GroupControl()
        Me.sbMailTest = New DevExpress.XtraEditors.SimpleButton()
        Me.ceMailEnabled = New DevExpress.XtraEditors.CheckEdit()
        Me.ceSMTPSsl = New DevExpress.XtraEditors.CheckEdit()
        Me.teSMTPPassword = New DevExpress.XtraEditors.TextEdit()
        Me.LabelControl5 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl14 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl21 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl13 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl6 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl11 = New DevExpress.XtraEditors.LabelControl()
        Me.teSMTPSender = New DevExpress.XtraEditors.TextEdit()
        Me.teMailCC = New DevExpress.XtraEditors.TextEdit()
        Me.teMailTo = New DevExpress.XtraEditors.TextEdit()
        Me.teSMTPUser = New DevExpress.XtraEditors.TextEdit()
        Me.LabelControl12 = New DevExpress.XtraEditors.LabelControl()
        Me.teSMTPPort = New DevExpress.XtraEditors.TextEdit()
        Me.teSMTPServer = New DevExpress.XtraEditors.TextEdit()
        CType(Me.bmActions, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rpiProceso, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemLookUpEdit1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemImageComboBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GroupControl2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupControl2.SuspendLayout()
        CType(Me.teMaxTemp.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.teDBFileName.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.teBDFileName.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.beVendorSourcePath.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.beDataTargetPath.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.beDatabasePath.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.beDataSourcePath.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GroupControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupControl1.SuspendLayout()
        CType(Me.CheckEdit1.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ButtonEdit1.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TextEdit4.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TextEdit3.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TextEdit2.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TextEdit1.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TextEdit5.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GroupControl3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupControl3.SuspendLayout()
        CType(Me.ceMailEnabled.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ceSMTPSsl.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.teSMTPPassword.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.teSMTPSender.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.teMailCC.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.teMailTo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.teSMTPUser.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.teSMTPPort.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.teSMTPServer.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.bmActions.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.brsDescripcion, Me.barStaticItem3, Me.barStaticItem4, Me.brsEstado, Me.bbiGuardar, Me.bbiCerrar, Me.beiProceso, Me.BarButtonItem1, Me.bsiVistas, Me.bbiVistaGrilla, Me.bbiTarjeta, Me.bbiContrato, Me.bbiCronograma, Me.bbiCartaNotarial, Me.bbiLetras, Me.bbiReset})
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
        Me.bar5.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(Me.brsDescripcion), New DevExpress.XtraBars.LinkPersistInfo(Me.barStaticItem3), New DevExpress.XtraBars.LinkPersistInfo(Me.barStaticItem4), New DevExpress.XtraBars.LinkPersistInfo(Me.brsEstado), New DevExpress.XtraBars.LinkPersistInfo(Me.beiProceso)})
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
        'barStaticItem3
        '
        Me.barStaticItem3.Caption = "0 / 0"
        Me.barStaticItem3.Id = 31
        Me.barStaticItem3.Name = "barStaticItem3"
        Me.barStaticItem3.TextAlignment = System.Drawing.StringAlignment.Near
        '
        'barStaticItem4
        '
        Me.barStaticItem4.Caption = "Estado"
        Me.barStaticItem4.Id = 46
        Me.barStaticItem4.Name = "barStaticItem4"
        Me.barStaticItem4.TextAlignment = System.Drawing.StringAlignment.Near
        '
        'brsEstado
        '
        Me.brsEstado.Caption = "Lectura"
        Me.brsEstado.Id = 47
        Me.brsEstado.Name = "brsEstado"
        Me.brsEstado.TextAlignment = System.Drawing.StringAlignment.Near
        '
        'beiProceso
        '
        Me.beiProceso.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right
        Me.beiProceso.Edit = Me.rpiProceso
        Me.beiProceso.Id = 0
        Me.beiProceso.Name = "beiProceso"
        Me.beiProceso.Width = 150
        '
        'rpiProceso
        '
        Me.rpiProceso.Name = "rpiProceso"
        Me.rpiProceso.ShowTitle = True
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
        Me.bbiReset.Caption = "&Reset"
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
        Me.BarDockControl1.Size = New System.Drawing.Size(708, 47)
        '
        'BarDockControl2
        '
        Me.BarDockControl2.CausesValidation = False
        Me.BarDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.BarDockControl2.Location = New System.Drawing.Point(0, 478)
        Me.BarDockControl2.Size = New System.Drawing.Size(708, 29)
        '
        'BarDockControl3
        '
        Me.BarDockControl3.CausesValidation = False
        Me.BarDockControl3.Dock = System.Windows.Forms.DockStyle.Left
        Me.BarDockControl3.Location = New System.Drawing.Point(0, 47)
        Me.BarDockControl3.Size = New System.Drawing.Size(0, 431)
        '
        'BarDockControl4
        '
        Me.BarDockControl4.CausesValidation = False
        Me.BarDockControl4.Dock = System.Windows.Forms.DockStyle.Right
        Me.BarDockControl4.Location = New System.Drawing.Point(708, 47)
        Me.BarDockControl4.Size = New System.Drawing.Size(0, 431)
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
        Me.GroupControl2.Controls.Add(Me.teMaxTemp)
        Me.GroupControl2.Controls.Add(Me.LabelControl15)
        Me.GroupControl2.Controls.Add(Me.teDBFileName)
        Me.GroupControl2.Controls.Add(Me.teBDFileName)
        Me.GroupControl2.Controls.Add(Me.LabelControl20)
        Me.GroupControl2.Controls.Add(Me.LabelControl4)
        Me.GroupControl2.Controls.Add(Me.beVendorSourcePath)
        Me.GroupControl2.Controls.Add(Me.LabelControl18)
        Me.GroupControl2.Controls.Add(Me.beDataTargetPath)
        Me.GroupControl2.Controls.Add(Me.LabelControl17)
        Me.GroupControl2.Controls.Add(Me.beDatabasePath)
        Me.GroupControl2.Controls.Add(Me.LabelControl19)
        Me.GroupControl2.Controls.Add(Me.beDataSourcePath)
        Me.GroupControl2.Controls.Add(Me.LabelControl3)
        Me.GroupControl2.Controls.Add(Me.LabelControl16)
        Me.GroupControl2.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupControl2.Location = New System.Drawing.Point(0, 47)
        Me.GroupControl2.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.GroupControl2.Name = "GroupControl2"
        Me.GroupControl2.Size = New System.Drawing.Size(708, 186)
        Me.GroupControl2.TabIndex = 6
        Me.GroupControl2.Text = "General"
        '
        'teMaxTemp
        '
        Me.teMaxTemp.EditValue = ""
        Me.teMaxTemp.EnterMoveNextControl = True
        Me.teMaxTemp.Location = New System.Drawing.Point(165, 159)
        Me.teMaxTemp.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.teMaxTemp.Name = "teMaxTemp"
        Me.teMaxTemp.Size = New System.Drawing.Size(51, 20)
        Me.teMaxTemp.TabIndex = 6
        '
        'LabelControl15
        '
        Me.LabelControl15.Location = New System.Drawing.Point(47, 161)
        Me.LabelControl15.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl15.Name = "LabelControl15"
        Me.LabelControl15.Size = New System.Drawing.Size(109, 13)
        Me.LabelControl15.TabIndex = 21
        Me.LabelControl15.Text = "Maximum Temperature"
        '
        'teDBFileName
        '
        Me.teDBFileName.EditValue = "dbColdTreatment.accdb"
        Me.teDBFileName.EnterMoveNextControl = True
        Me.teDBFileName.Location = New System.Drawing.Point(165, 71)
        Me.teDBFileName.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.teDBFileName.Name = "teDBFileName"
        Me.teDBFileName.Size = New System.Drawing.Size(126, 20)
        Me.teDBFileName.TabIndex = 2
        '
        'teBDFileName
        '
        Me.teBDFileName.EditValue = "DBColdTreatment.xlsx"
        Me.teBDFileName.Enabled = False
        Me.teBDFileName.EnterMoveNextControl = True
        Me.teBDFileName.Location = New System.Drawing.Point(165, 27)
        Me.teBDFileName.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.teBDFileName.MenuManager = Me.bmActions
        Me.teBDFileName.Name = "teBDFileName"
        Me.teBDFileName.Size = New System.Drawing.Size(126, 20)
        Me.teBDFileName.TabIndex = 0
        '
        'LabelControl20
        '
        Me.LabelControl20.Location = New System.Drawing.Point(95, 73)
        Me.LabelControl20.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl20.Name = "LabelControl20"
        Me.LabelControl20.Size = New System.Drawing.Size(65, 13)
        Me.LabelControl20.TabIndex = 21
        Me.LabelControl20.Text = "Database File"
        '
        'LabelControl4
        '
        Me.LabelControl4.Location = New System.Drawing.Point(81, 29)
        Me.LabelControl4.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl4.Name = "LabelControl4"
        Me.LabelControl4.Size = New System.Drawing.Size(78, 13)
        Me.LabelControl4.TabIndex = 21
        Me.LabelControl4.Text = "Data Source File"
        '
        'beVendorSourcePath
        '
        Me.beVendorSourcePath.Location = New System.Drawing.Point(165, 137)
        Me.beVendorSourcePath.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.beVendorSourcePath.Name = "beVendorSourcePath"
        Me.beVendorSourcePath.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beVendorSourcePath.Size = New System.Drawing.Size(490, 20)
        Me.beVendorSourcePath.TabIndex = 5
        '
        'LabelControl18
        '
        Me.LabelControl18.Location = New System.Drawing.Point(63, 139)
        Me.LabelControl18.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl18.Name = "LabelControl18"
        Me.LabelControl18.Size = New System.Drawing.Size(95, 13)
        Me.LabelControl18.TabIndex = 21
        Me.LabelControl18.Text = "Vendor Source Path"
        '
        'beDataTargetPath
        '
        Me.beDataTargetPath.Location = New System.Drawing.Point(165, 115)
        Me.beDataTargetPath.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.beDataTargetPath.Name = "beDataTargetPath"
        Me.beDataTargetPath.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beDataTargetPath.Size = New System.Drawing.Size(490, 20)
        Me.beDataTargetPath.TabIndex = 4
        '
        'LabelControl17
        '
        Me.LabelControl17.Location = New System.Drawing.Point(77, 117)
        Me.LabelControl17.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl17.Name = "LabelControl17"
        Me.LabelControl17.Size = New System.Drawing.Size(83, 13)
        Me.LabelControl17.TabIndex = 21
        Me.LabelControl17.Text = "Data Target Path"
        '
        'beDatabasePath
        '
        Me.beDatabasePath.Location = New System.Drawing.Point(165, 93)
        Me.beDatabasePath.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.beDatabasePath.Name = "beDatabasePath"
        Me.beDatabasePath.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beDatabasePath.Size = New System.Drawing.Size(490, 20)
        Me.beDatabasePath.TabIndex = 3
        '
        'LabelControl19
        '
        Me.LabelControl19.Location = New System.Drawing.Point(90, 95)
        Me.LabelControl19.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl19.Name = "LabelControl19"
        Me.LabelControl19.Size = New System.Drawing.Size(71, 13)
        Me.LabelControl19.TabIndex = 21
        Me.LabelControl19.Text = "Database Path"
        '
        'beDataSourcePath
        '
        Me.beDataSourcePath.Location = New System.Drawing.Point(165, 49)
        Me.beDataSourcePath.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.beDataSourcePath.Name = "beDataSourcePath"
        Me.beDataSourcePath.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beDataSourcePath.Size = New System.Drawing.Size(490, 20)
        Me.beDataSourcePath.TabIndex = 1
        '
        'LabelControl3
        '
        Me.LabelControl3.Location = New System.Drawing.Point(76, 51)
        Me.LabelControl3.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl3.Name = "LabelControl3"
        Me.LabelControl3.Size = New System.Drawing.Size(84, 13)
        Me.LabelControl3.TabIndex = 21
        Me.LabelControl3.Text = "Data Source Path"
        '
        'LabelControl16
        '
        Me.LabelControl16.Location = New System.Drawing.Point(219, 161)
        Me.LabelControl16.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl16.Name = "LabelControl16"
        Me.LabelControl16.Size = New System.Drawing.Size(12, 13)
        Me.LabelControl16.TabIndex = 4
        Me.LabelControl16.Text = "ºC"
        '
        'GroupControl1
        '
        Me.GroupControl1.Controls.Add(Me.CheckEdit1)
        Me.GroupControl1.Controls.Add(Me.ButtonEdit1)
        Me.GroupControl1.Controls.Add(Me.LabelControl10)
        Me.GroupControl1.Controls.Add(Me.TextEdit4)
        Me.GroupControl1.Controls.Add(Me.TextEdit3)
        Me.GroupControl1.Controls.Add(Me.TextEdit2)
        Me.GroupControl1.Controls.Add(Me.TextEdit1)
        Me.GroupControl1.Controls.Add(Me.LabelControl2)
        Me.GroupControl1.Controls.Add(Me.LabelControl9)
        Me.GroupControl1.Controls.Add(Me.LabelControl8)
        Me.GroupControl1.Controls.Add(Me.LabelControl7)
        Me.GroupControl1.Controls.Add(Me.LabelControl1)
        Me.GroupControl1.Controls.Add(Me.TextEdit5)
        Me.GroupControl1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupControl1.Location = New System.Drawing.Point(0, 233)
        Me.GroupControl1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.GroupControl1.Name = "GroupControl1"
        Me.GroupControl1.Size = New System.Drawing.Size(708, 205)
        Me.GroupControl1.TabIndex = 7
        Me.GroupControl1.Text = "Mail Processing Service"
        Me.GroupControl1.Visible = False
        '
        'CheckEdit1
        '
        Me.CheckEdit1.Location = New System.Drawing.Point(163, 68)
        Me.CheckEdit1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.CheckEdit1.MenuManager = Me.bmActions
        Me.CheckEdit1.Name = "CheckEdit1"
        Me.CheckEdit1.Properties.Caption = "Need Authentication"
        Me.CheckEdit1.Size = New System.Drawing.Size(127, 19)
        Me.CheckEdit1.TabIndex = 2
        '
        'ButtonEdit1
        '
        Me.ButtonEdit1.Location = New System.Drawing.Point(165, 181)
        Me.ButtonEdit1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.ButtonEdit1.Name = "ButtonEdit1"
        Me.ButtonEdit1.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.ButtonEdit1.Size = New System.Drawing.Size(490, 20)
        Me.ButtonEdit1.TabIndex = 6
        '
        'LabelControl10
        '
        Me.LabelControl10.Location = New System.Drawing.Point(102, 183)
        Me.LabelControl10.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl10.Name = "LabelControl10"
        Me.LabelControl10.Size = New System.Drawing.Size(57, 13)
        Me.LabelControl10.TabIndex = 21
        Me.LabelControl10.Text = "Target Path"
        '
        'TextEdit4
        '
        Me.TextEdit4.EditValue = ""
        Me.TextEdit4.EnterMoveNextControl = True
        Me.TextEdit4.Location = New System.Drawing.Point(165, 109)
        Me.TextEdit4.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.TextEdit4.Name = "TextEdit4"
        Me.TextEdit4.Properties.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TextEdit4.Size = New System.Drawing.Size(176, 20)
        Me.TextEdit4.TabIndex = 4
        '
        'TextEdit3
        '
        Me.TextEdit3.EditValue = ""
        Me.TextEdit3.EnterMoveNextControl = True
        Me.TextEdit3.Location = New System.Drawing.Point(165, 88)
        Me.TextEdit3.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.TextEdit3.Name = "TextEdit3"
        Me.TextEdit3.Size = New System.Drawing.Size(235, 20)
        Me.TextEdit3.TabIndex = 3
        '
        'TextEdit2
        '
        Me.TextEdit2.EditValue = ""
        Me.TextEdit2.EnterMoveNextControl = True
        Me.TextEdit2.Location = New System.Drawing.Point(165, 46)
        Me.TextEdit2.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.TextEdit2.Name = "TextEdit2"
        Me.TextEdit2.Size = New System.Drawing.Size(64, 20)
        Me.TextEdit2.TabIndex = 1
        '
        'TextEdit1
        '
        Me.TextEdit1.EditValue = ""
        Me.TextEdit1.EnterMoveNextControl = True
        Me.TextEdit1.Location = New System.Drawing.Point(165, 24)
        Me.TextEdit1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.TextEdit1.Name = "TextEdit1"
        Me.TextEdit1.Size = New System.Drawing.Size(235, 20)
        Me.TextEdit1.TabIndex = 0
        '
        'LabelControl2
        '
        Me.LabelControl2.Location = New System.Drawing.Point(137, 48)
        Me.LabelControl2.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl2.Name = "LabelControl2"
        Me.LabelControl2.Size = New System.Drawing.Size(20, 13)
        Me.LabelControl2.TabIndex = 21
        Me.LabelControl2.Text = "Port"
        '
        'LabelControl9
        '
        Me.LabelControl9.Location = New System.Drawing.Point(101, 133)
        Me.LabelControl9.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl9.Name = "LabelControl9"
        Me.LabelControl9.Size = New System.Drawing.Size(58, 13)
        Me.LabelControl9.TabIndex = 0
        Me.LabelControl9.Text = "Text Search"
        '
        'LabelControl8
        '
        Me.LabelControl8.Location = New System.Drawing.Point(112, 112)
        Me.LabelControl8.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl8.Name = "LabelControl8"
        Me.LabelControl8.Size = New System.Drawing.Size(46, 13)
        Me.LabelControl8.TabIndex = 0
        Me.LabelControl8.Text = "Password"
        '
        'LabelControl7
        '
        Me.LabelControl7.Location = New System.Drawing.Point(135, 90)
        Me.LabelControl7.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl7.Name = "LabelControl7"
        Me.LabelControl7.Size = New System.Drawing.Size(22, 13)
        Me.LabelControl7.TabIndex = 0
        Me.LabelControl7.Text = "User"
        '
        'LabelControl1
        '
        Me.LabelControl1.Location = New System.Drawing.Point(102, 26)
        Me.LabelControl1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl1.Name = "LabelControl1"
        Me.LabelControl1.Size = New System.Drawing.Size(57, 13)
        Me.LabelControl1.TabIndex = 0
        Me.LabelControl1.Text = "Host Server"
        '
        'TextEdit5
        '
        Me.TextEdit5.EditValue = ""
        Me.TextEdit5.Location = New System.Drawing.Point(165, 131)
        Me.TextEdit5.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.TextEdit5.Name = "TextEdit5"
        Me.TextEdit5.Size = New System.Drawing.Size(329, 46)
        Me.TextEdit5.TabIndex = 5
        Me.TextEdit5.UseOptimizedRendering = True
        '
        'GroupControl3
        '
        Me.GroupControl3.Controls.Add(Me.sbMailTest)
        Me.GroupControl3.Controls.Add(Me.ceMailEnabled)
        Me.GroupControl3.Controls.Add(Me.ceSMTPSsl)
        Me.GroupControl3.Controls.Add(Me.teSMTPPassword)
        Me.GroupControl3.Controls.Add(Me.LabelControl5)
        Me.GroupControl3.Controls.Add(Me.LabelControl14)
        Me.GroupControl3.Controls.Add(Me.LabelControl21)
        Me.GroupControl3.Controls.Add(Me.LabelControl13)
        Me.GroupControl3.Controls.Add(Me.LabelControl6)
        Me.GroupControl3.Controls.Add(Me.LabelControl11)
        Me.GroupControl3.Controls.Add(Me.teSMTPSender)
        Me.GroupControl3.Controls.Add(Me.teMailCC)
        Me.GroupControl3.Controls.Add(Me.teMailTo)
        Me.GroupControl3.Controls.Add(Me.teSMTPUser)
        Me.GroupControl3.Controls.Add(Me.LabelControl12)
        Me.GroupControl3.Controls.Add(Me.teSMTPPort)
        Me.GroupControl3.Controls.Add(Me.teSMTPServer)
        Me.GroupControl3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupControl3.Location = New System.Drawing.Point(0, 438)
        Me.GroupControl3.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.GroupControl3.Name = "GroupControl3"
        Me.GroupControl3.Size = New System.Drawing.Size(708, 40)
        Me.GroupControl3.TabIndex = 12
        Me.GroupControl3.Text = "Mail SMTP Service"
        '
        'sbMailTest
        '
        Me.sbMailTest.Location = New System.Drawing.Point(165, 221)
        Me.sbMailTest.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.sbMailTest.Name = "sbMailTest"
        Me.sbMailTest.Size = New System.Drawing.Size(51, 19)
        Me.sbMailTest.TabIndex = 9
        Me.sbMailTest.Text = "Test"
        '
        'ceMailEnabled
        '
        Me.ceMailEnabled.Location = New System.Drawing.Point(163, 25)
        Me.ceMailEnabled.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.ceMailEnabled.Name = "ceMailEnabled"
        Me.ceMailEnabled.Properties.Caption = "Enabled"
        Me.ceMailEnabled.Size = New System.Drawing.Size(127, 19)
        Me.ceMailEnabled.TabIndex = 0
        '
        'ceSMTPSsl
        '
        Me.ceSMTPSsl.Location = New System.Drawing.Point(163, 88)
        Me.ceSMTPSsl.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.ceSMTPSsl.Name = "ceSMTPSsl"
        Me.ceSMTPSsl.Properties.Caption = "Need Authentication"
        Me.ceSMTPSsl.Size = New System.Drawing.Size(127, 19)
        Me.ceSMTPSsl.TabIndex = 3
        '
        'teSMTPPassword
        '
        Me.teSMTPPassword.EditValue = ""
        Me.teSMTPPassword.EnterMoveNextControl = True
        Me.teSMTPPassword.Location = New System.Drawing.Point(165, 130)
        Me.teSMTPPassword.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.teSMTPPassword.Name = "teSMTPPassword"
        Me.teSMTPPassword.Properties.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.teSMTPPassword.Size = New System.Drawing.Size(176, 20)
        Me.teSMTPPassword.TabIndex = 5
        '
        'LabelControl5
        '
        Me.LabelControl5.Location = New System.Drawing.Point(102, 47)
        Me.LabelControl5.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl5.Name = "LabelControl5"
        Me.LabelControl5.Size = New System.Drawing.Size(57, 13)
        Me.LabelControl5.TabIndex = 0
        Me.LabelControl5.Text = "Host Server"
        '
        'LabelControl14
        '
        Me.LabelControl14.Location = New System.Drawing.Point(124, 198)
        Me.LabelControl14.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl14.Name = "LabelControl14"
        Me.LabelControl14.Size = New System.Drawing.Size(34, 13)
        Me.LabelControl14.TabIndex = 0
        Me.LabelControl14.Text = "Sender"
        '
        'LabelControl21
        '
        Me.LabelControl21.Location = New System.Drawing.Point(121, 176)
        Me.LabelControl21.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl21.Name = "LabelControl21"
        Me.LabelControl21.Size = New System.Drawing.Size(35, 13)
        Me.LabelControl21.TabIndex = 0
        Me.LabelControl21.Text = "Mail CC"
        '
        'LabelControl13
        '
        Me.LabelControl13.Location = New System.Drawing.Point(122, 154)
        Me.LabelControl13.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl13.Name = "LabelControl13"
        Me.LabelControl13.Size = New System.Drawing.Size(33, 13)
        Me.LabelControl13.TabIndex = 0
        Me.LabelControl13.Text = "Mail To"
        '
        'LabelControl6
        '
        Me.LabelControl6.Location = New System.Drawing.Point(135, 110)
        Me.LabelControl6.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl6.Name = "LabelControl6"
        Me.LabelControl6.Size = New System.Drawing.Size(22, 13)
        Me.LabelControl6.TabIndex = 0
        Me.LabelControl6.Text = "User"
        '
        'LabelControl11
        '
        Me.LabelControl11.Location = New System.Drawing.Point(112, 132)
        Me.LabelControl11.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl11.Name = "LabelControl11"
        Me.LabelControl11.Size = New System.Drawing.Size(46, 13)
        Me.LabelControl11.TabIndex = 0
        Me.LabelControl11.Text = "Password"
        '
        'teSMTPSender
        '
        Me.teSMTPSender.EditValue = ""
        Me.teSMTPSender.EnterMoveNextControl = True
        Me.teSMTPSender.Location = New System.Drawing.Point(165, 196)
        Me.teSMTPSender.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.teSMTPSender.Name = "teSMTPSender"
        Me.teSMTPSender.Size = New System.Drawing.Size(329, 20)
        Me.teSMTPSender.TabIndex = 8
        '
        'teMailCC
        '
        Me.teMailCC.EditValue = ""
        Me.teMailCC.EnterMoveNextControl = True
        Me.teMailCC.Location = New System.Drawing.Point(165, 174)
        Me.teMailCC.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.teMailCC.Name = "teMailCC"
        Me.teMailCC.Size = New System.Drawing.Size(490, 20)
        Me.teMailCC.TabIndex = 7
        '
        'teMailTo
        '
        Me.teMailTo.EditValue = ""
        Me.teMailTo.EnterMoveNextControl = True
        Me.teMailTo.Location = New System.Drawing.Point(165, 152)
        Me.teMailTo.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.teMailTo.Name = "teMailTo"
        Me.teMailTo.Size = New System.Drawing.Size(490, 20)
        Me.teMailTo.TabIndex = 6
        '
        'teSMTPUser
        '
        Me.teSMTPUser.EditValue = ""
        Me.teSMTPUser.EnterMoveNextControl = True
        Me.teSMTPUser.Location = New System.Drawing.Point(165, 108)
        Me.teSMTPUser.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.teSMTPUser.Name = "teSMTPUser"
        Me.teSMTPUser.Size = New System.Drawing.Size(235, 20)
        Me.teSMTPUser.TabIndex = 4
        '
        'LabelControl12
        '
        Me.LabelControl12.Location = New System.Drawing.Point(137, 68)
        Me.LabelControl12.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.LabelControl12.Name = "LabelControl12"
        Me.LabelControl12.Size = New System.Drawing.Size(20, 13)
        Me.LabelControl12.TabIndex = 21
        Me.LabelControl12.Text = "Port"
        '
        'teSMTPPort
        '
        Me.teSMTPPort.EditValue = ""
        Me.teSMTPPort.EnterMoveNextControl = True
        Me.teSMTPPort.Location = New System.Drawing.Point(165, 66)
        Me.teSMTPPort.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.teSMTPPort.Name = "teSMTPPort"
        Me.teSMTPPort.Size = New System.Drawing.Size(64, 20)
        Me.teSMTPPort.TabIndex = 2
        '
        'teSMTPServer
        '
        Me.teSMTPServer.EditValue = ""
        Me.teSMTPServer.EnterMoveNextControl = True
        Me.teSMTPServer.Location = New System.Drawing.Point(165, 44)
        Me.teSMTPServer.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.teSMTPServer.Name = "teSMTPServer"
        Me.teSMTPServer.Size = New System.Drawing.Size(235, 20)
        Me.teSMTPServer.TabIndex = 1
        '
        'PreferencesForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(708, 507)
        Me.Controls.Add(Me.GroupControl3)
        Me.Controls.Add(Me.GroupControl1)
        Me.Controls.Add(Me.GroupControl2)
        Me.Controls.Add(Me.BarDockControl3)
        Me.Controls.Add(Me.BarDockControl4)
        Me.Controls.Add(Me.BarDockControl2)
        Me.Controls.Add(Me.BarDockControl1)
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Name = "PreferencesForm"
        Me.Text = "Preferences"
        CType(Me.bmActions, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rpiProceso, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemLookUpEdit1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemImageComboBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GroupControl2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupControl2.ResumeLayout(False)
        Me.GroupControl2.PerformLayout()
        CType(Me.teMaxTemp.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.teDBFileName.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.teBDFileName.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.beVendorSourcePath.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.beDataTargetPath.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.beDatabasePath.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.beDataSourcePath.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GroupControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupControl1.ResumeLayout(False)
        Me.GroupControl1.PerformLayout()
        CType(Me.CheckEdit1.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ButtonEdit1.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TextEdit4.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TextEdit3.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TextEdit2.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TextEdit1.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TextEdit5.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GroupControl3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupControl3.ResumeLayout(False)
        Me.GroupControl3.PerformLayout()
        CType(Me.ceMailEnabled.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ceSMTPSsl.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.teSMTPPassword.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.teSMTPSender.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.teMailCC.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.teMailTo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.teSMTPUser.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.teSMTPPort.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.teSMTPServer.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents bmActions As DevExpress.XtraBars.BarManager
    Private WithEvents bar5 As DevExpress.XtraBars.Bar
    Private WithEvents brsDescripcion As DevExpress.XtraBars.BarStaticItem
    Private WithEvents barStaticItem3 As DevExpress.XtraBars.BarStaticItem
    Private WithEvents barStaticItem4 As DevExpress.XtraBars.BarStaticItem
    Private WithEvents brsEstado As DevExpress.XtraBars.BarStaticItem
    Private WithEvents beiProceso As DevExpress.XtraBars.BarEditItem
    Private WithEvents rpiProceso As DevExpress.XtraEditors.Repository.RepositoryItemProgressBar
    Private WithEvents brBarraAcciones As DevExpress.XtraBars.Bar
    Private WithEvents bbiGuardar As DevExpress.XtraBars.BarButtonItem
    Private WithEvents bbiCerrar As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BarDockControl1 As DevExpress.XtraBars.BarDockControl
    Private WithEvents BarDockControl2 As DevExpress.XtraBars.BarDockControl
    Private WithEvents BarDockControl3 As DevExpress.XtraBars.BarDockControl
    Private WithEvents BarDockControl4 As DevExpress.XtraBars.BarDockControl
    Private WithEvents imActionsBar24x24 As System.Windows.Forms.ImageList
    Friend WithEvents BarButtonItem1 As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bsiVistas As DevExpress.XtraBars.BarSubItem
    Friend WithEvents bbiVistaGrilla As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bbiTarjeta As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bbiContrato As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bbiCronograma As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bbiCartaNotarial As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents bbiLetras As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents RepositoryItemLookUpEdit1 As DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit
    Friend WithEvents RepositoryItemImageComboBox1 As DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox
    Friend WithEvents GroupControl1 As DevExpress.XtraEditors.GroupControl
    Friend WithEvents LabelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents GroupControl2 As DevExpress.XtraEditors.GroupControl
    Friend WithEvents teBDFileName As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LabelControl4 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents beDataSourcePath As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents LabelControl3 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents GroupControl3 As DevExpress.XtraEditors.GroupControl
    Friend WithEvents CheckEdit1 As DevExpress.XtraEditors.CheckEdit
    Friend WithEvents TextEdit4 As DevExpress.XtraEditors.TextEdit
    Friend WithEvents TextEdit3 As DevExpress.XtraEditors.TextEdit
    Friend WithEvents TextEdit2 As DevExpress.XtraEditors.TextEdit
    Friend WithEvents TextEdit1 As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LabelControl8 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl7 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl9 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents ButtonEdit1 As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents LabelControl10 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TextEdit5 As DevExpress.XtraEditors.MemoEdit
    Friend WithEvents ceSMTPSsl As DevExpress.XtraEditors.CheckEdit
    Friend WithEvents teSMTPPassword As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LabelControl5 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl14 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl13 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl6 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl11 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents teSMTPSender As DevExpress.XtraEditors.TextEdit
    Friend WithEvents teMailTo As DevExpress.XtraEditors.TextEdit
    Friend WithEvents teSMTPUser As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LabelControl12 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents teSMTPPort As DevExpress.XtraEditors.TextEdit
    Friend WithEvents teSMTPServer As DevExpress.XtraEditors.TextEdit
    Friend WithEvents teMaxTemp As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LabelControl15 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl16 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents beDataTargetPath As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents LabelControl17 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents sbMailTest As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents ceMailEnabled As DevExpress.XtraEditors.CheckEdit
    Friend WithEvents beVendorSourcePath As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents LabelControl18 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents teDBFileName As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LabelControl20 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents beDatabasePath As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents LabelControl19 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents bbiReset As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents LabelControl21 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents teMailCC As DevExpress.XtraEditors.TextEdit
End Class
