<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BankForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BankForm))
        Me.PanelControl2 = New DevExpress.XtraEditors.PanelControl()
        Me.lueSociedad = New DevExpress.XtraEditors.LookUpEdit()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.beArchivoSalida = New DevExpress.XtraEditors.ButtonEdit()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.beArchivoOrigen = New DevExpress.XtraEditors.ButtonEdit()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.sePeriodo = New DevExpress.XtraEditors.SpinEdit()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.seEjercicio = New DevExpress.XtraEditors.SpinEdit()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.lueCuenta = New DevExpress.XtraEditors.LookUpEdit()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.bmActions = New DevExpress.XtraBars.BarManager()
        Me.bar5 = New DevExpress.XtraBars.Bar()
        Me.brBarraAcciones = New DevExpress.XtraBars.Bar()
        Me.bbiProcesar = New DevExpress.XtraBars.BarButtonItem()
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
        Me.rpiProceso = New DevExpress.XtraEditors.Repository.RepositoryItemProgressBar()
        Me.RepositoryItemLookUpEdit1 = New DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit()
        Me.RepositoryItemImageComboBox1 = New DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox()
        Me.vpBank = New DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider()
        CType(Me.PanelControl2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl2.SuspendLayout()
        CType(Me.lueSociedad.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.beArchivoSalida.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.beArchivoOrigen.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sePeriodo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.seEjercicio.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lueCuenta.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bmActions, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rpiProceso, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemLookUpEdit1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemImageComboBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.vpBank, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelControl2
        '
        Me.PanelControl2.Controls.Add(Me.lueSociedad)
        Me.PanelControl2.Controls.Add(Me.Label1)
        Me.PanelControl2.Controls.Add(Me.beArchivoSalida)
        Me.PanelControl2.Controls.Add(Me.Label2)
        Me.PanelControl2.Controls.Add(Me.beArchivoOrigen)
        Me.PanelControl2.Controls.Add(Me.Label3)
        Me.PanelControl2.Controls.Add(Me.Label4)
        Me.PanelControl2.Controls.Add(Me.sePeriodo)
        Me.PanelControl2.Controls.Add(Me.Label5)
        Me.PanelControl2.Controls.Add(Me.seEjercicio)
        Me.PanelControl2.Controls.Add(Me.Label6)
        Me.PanelControl2.Controls.Add(Me.lueCuenta)
        Me.PanelControl2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelControl2.Location = New System.Drawing.Point(0, 47)
        Me.PanelControl2.Margin = New System.Windows.Forms.Padding(2)
        Me.PanelControl2.Name = "PanelControl2"
        Me.PanelControl2.Size = New System.Drawing.Size(615, 253)
        Me.PanelControl2.TabIndex = 1
        '
        'lueSociedad
        '
        Me.lueSociedad.Location = New System.Drawing.Point(130, 26)
        Me.lueSociedad.Margin = New System.Windows.Forms.Padding(2)
        Me.lueSociedad.Name = "lueSociedad"
        Me.lueSociedad.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.lueSociedad.Properties.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo("CompanyCode", "Código"), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("CompanyDescription", "Descripción", 60, DevExpress.Utils.FormatType.None, "", True, DevExpress.Utils.HorzAlignment.[Default], DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.[Default]), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("CompanyTaxCode", "RUC"), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("CompanyAddress", "Dirección", 20, DevExpress.Utils.FormatType.None, "", False, DevExpress.Utils.HorzAlignment.[Default], DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.[Default])})
        Me.lueSociedad.Properties.NullText = ""
        Me.lueSociedad.Size = New System.Drawing.Size(394, 20)
        Me.lueSociedad.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(65, 29)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 13)
        Me.Label1.TabIndex = 10
        Me.Label1.Text = "Sociedad"
        '
        'beArchivoSalida
        '
        Me.beArchivoSalida.Location = New System.Drawing.Point(130, 185)
        Me.beArchivoSalida.Margin = New System.Windows.Forms.Padding(2)
        Me.beArchivoSalida.Name = "beArchivoSalida"
        Me.beArchivoSalida.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beArchivoSalida.Size = New System.Drawing.Size(394, 20)
        Me.beArchivoSalida.TabIndex = 5
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(32, 61)
        Me.Label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(86, 13)
        Me.Label2.TabIndex = 11
        Me.Label2.Text = "Cuenta Bancaria"
        '
        'beArchivoOrigen
        '
        Me.beArchivoOrigen.Location = New System.Drawing.Point(130, 153)
        Me.beArchivoOrigen.Margin = New System.Windows.Forms.Padding(2)
        Me.beArchivoOrigen.Name = "beArchivoOrigen"
        Me.beArchivoOrigen.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.beArchivoOrigen.Size = New System.Drawing.Size(394, 20)
        Me.beArchivoOrigen.TabIndex = 4
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(68, 92)
        Me.Label3.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(46, 13)
        Me.Label3.TabIndex = 12
        Me.Label3.Text = "Ejercicio"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(71, 124)
        Me.Label4.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(43, 13)
        Me.Label4.TabIndex = 13
        Me.Label4.Text = "Periodo"
        '
        'sePeriodo
        '
        Me.sePeriodo.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.sePeriodo.Location = New System.Drawing.Point(130, 122)
        Me.sePeriodo.Margin = New System.Windows.Forms.Padding(2)
        Me.sePeriodo.Name = "sePeriodo"
        Me.sePeriodo.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.sePeriodo.Properties.Mask.EditMask = "f00"
        Me.sePeriodo.Size = New System.Drawing.Size(40, 20)
        Me.sePeriodo.TabIndex = 3
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(25, 155)
        Me.Label5.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(93, 13)
        Me.Label5.TabIndex = 14
        Me.Label5.Text = "Estado de Cuenta"
        '
        'seEjercicio
        '
        Me.seEjercicio.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.seEjercicio.Location = New System.Drawing.Point(130, 90)
        Me.seEjercicio.Margin = New System.Windows.Forms.Padding(2)
        Me.seEjercicio.Name = "seEjercicio"
        Me.seEjercicio.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.seEjercicio.Properties.Mask.EditMask = "f0000"
        Me.seEjercicio.Size = New System.Drawing.Size(54, 20)
        Me.seEjercicio.TabIndex = 2
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(25, 187)
        Me.Label6.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(89, 13)
        Me.Label6.TabIndex = 15
        Me.Label6.Text = "Archivo de Salida"
        '
        'lueCuenta
        '
        Me.lueCuenta.Location = New System.Drawing.Point(130, 59)
        Me.lueCuenta.Margin = New System.Windows.Forms.Padding(2)
        Me.lueCuenta.Name = "lueCuenta"
        Me.lueCuenta.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.[True]
        Me.lueCuenta.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup
        Me.lueCuenta.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.lueCuenta.Properties.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo("CompanyCode", "Sociedad", 20, DevExpress.Utils.FormatType.None, "", False, DevExpress.Utils.HorzAlignment.[Default], DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.[Default]), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("BankCode", "BancoCódigo", 20, DevExpress.Utils.FormatType.None, "", False, DevExpress.Utils.HorzAlignment.[Default], DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.[Default]), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("BankDescription", "BancoDescripción"), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("BankFormat", "Formato", 20, DevExpress.Utils.FormatType.None, "", False, DevExpress.Utils.HorzAlignment.[Default], DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.[Default]), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("BankCurrency", "Moneda"), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("AccountBankCode", "CuentaBancaria"), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("AccountCode1", "CuentaContable1"), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("AccountCode2", "CuentaContable2", 20, DevExpress.Utils.FormatType.None, "", False, DevExpress.Utils.HorzAlignment.[Default], DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.[Default])})
        Me.lueCuenta.Properties.NullText = ""
        Me.lueCuenta.Properties.PopupWidthMode = DevExpress.XtraEditors.PopupWidthMode.ContentWidth
        Me.lueCuenta.Size = New System.Drawing.Size(394, 20)
        Me.lueCuenta.TabIndex = 1
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
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
        Me.bmActions.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.bbiProcesar, Me.bbiCerrar, Me.BarButtonItem1, Me.bsiVistas, Me.bbiVistaGrilla, Me.bbiTarjeta, Me.bbiContrato, Me.bbiCronograma, Me.bbiCartaNotarial, Me.bbiLetras})
        Me.bmActions.MaxItemId = 20
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
        Me.brBarraAcciones.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.bbiProcesar, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph), New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.bbiCerrar, "", True, True, True, 0, Nothing, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)})
        Me.brBarraAcciones.OptionsBar.AllowQuickCustomization = False
        Me.brBarraAcciones.OptionsBar.UseWholeRow = True
        Me.brBarraAcciones.Text = "Custom 5"
        '
        'bbiProcesar
        '
        Me.bbiProcesar.Caption = "&Procesar"
        Me.bbiProcesar.Id = 33
        Me.bbiProcesar.ImageOptions.Image = CType(resources.GetObject("bbiProcesar.ImageOptions.Image"), System.Drawing.Image)
        Me.bbiProcesar.ImageOptions.ImageIndex = 26
        Me.bbiProcesar.ImageOptions.LargeImageIndex = 7
        Me.bbiProcesar.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.P))
        Me.bbiProcesar.Name = "bbiProcesar"
        '
        'bbiCerrar
        '
        Me.bbiCerrar.Caption = "&Cerrar"
        Me.bbiCerrar.Id = 41
        Me.bbiCerrar.ImageOptions.Image = CType(resources.GetObject("bbiCerrar.ImageOptions.Image"), System.Drawing.Image)
        Me.bbiCerrar.ImageOptions.ImageIndex = 27
        Me.bbiCerrar.ImageOptions.LargeImageIndex = 0
        Me.bbiCerrar.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.C))
        Me.bbiCerrar.Name = "bbiCerrar"
        Me.bbiCerrar.ShortcutKeyDisplayString = "Ctrl+C"
        '
        'BarDockControl1
        '
        Me.BarDockControl1.CausesValidation = False
        Me.BarDockControl1.Dock = System.Windows.Forms.DockStyle.Top
        Me.BarDockControl1.Location = New System.Drawing.Point(0, 0)
        Me.BarDockControl1.Manager = Me.bmActions
        Me.BarDockControl1.Size = New System.Drawing.Size(615, 47)
        '
        'BarDockControl2
        '
        Me.BarDockControl2.CausesValidation = False
        Me.BarDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.BarDockControl2.Location = New System.Drawing.Point(0, 300)
        Me.BarDockControl2.Manager = Me.bmActions
        Me.BarDockControl2.Size = New System.Drawing.Size(615, 29)
        '
        'BarDockControl3
        '
        Me.BarDockControl3.CausesValidation = False
        Me.BarDockControl3.Dock = System.Windows.Forms.DockStyle.Left
        Me.BarDockControl3.Location = New System.Drawing.Point(0, 47)
        Me.BarDockControl3.Manager = Me.bmActions
        Me.BarDockControl3.Size = New System.Drawing.Size(0, 253)
        '
        'BarDockControl4
        '
        Me.BarDockControl4.CausesValidation = False
        Me.BarDockControl4.Dock = System.Windows.Forms.DockStyle.Right
        Me.BarDockControl4.Location = New System.Drawing.Point(615, 47)
        Me.BarDockControl4.Manager = Me.bmActions
        Me.BarDockControl4.Size = New System.Drawing.Size(0, 253)
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
        Me.bsiVistas.ImageOptions.ImageIndex = 20
        Me.bsiVistas.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(Me.bbiVistaGrilla), New DevExpress.XtraBars.LinkPersistInfo(Me.bbiTarjeta)})
        Me.bsiVistas.Name = "bsiVistas"
        '
        'bbiVistaGrilla
        '
        Me.bbiVistaGrilla.Caption = "Grilla"
        Me.bbiVistaGrilla.Id = 7
        Me.bbiVistaGrilla.ImageOptions.ImageIndex = 23
        Me.bbiVistaGrilla.Name = "bbiVistaGrilla"
        '
        'bbiTarjeta
        '
        Me.bbiTarjeta.Caption = "Tarjeta"
        Me.bbiTarjeta.Id = 8
        Me.bbiTarjeta.ImageOptions.ImageIndex = 21
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
        'vpBank
        '
        Me.vpBank.ValidationMode = DevExpress.XtraEditors.DXErrorProvider.ValidationMode.Manual
        '
        'BankForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(615, 329)
        Me.Controls.Add(Me.PanelControl2)
        Me.Controls.Add(Me.BarDockControl3)
        Me.Controls.Add(Me.BarDockControl4)
        Me.Controls.Add(Me.BarDockControl2)
        Me.Controls.Add(Me.BarDockControl1)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "BankForm"
        Me.Text = "Genera Archivo Plano de Bancos (SAP)"
        CType(Me.PanelControl2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl2.ResumeLayout(False)
        Me.PanelControl2.PerformLayout()
        CType(Me.lueSociedad.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.beArchivoSalida.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.beArchivoOrigen.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sePeriodo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.seEjercicio.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lueCuenta.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bmActions, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rpiProceso, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemLookUpEdit1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemImageComboBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.vpBank, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PanelControl2 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents beArchivoSalida As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents beArchivoOrigen As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents sePeriodo As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents seEjercicio As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents lueCuenta As DevExpress.XtraEditors.LookUpEdit
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Private WithEvents bmActions As DevExpress.XtraBars.BarManager
    Private WithEvents bar5 As DevExpress.XtraBars.Bar
    Private WithEvents rpiProceso As DevExpress.XtraEditors.Repository.RepositoryItemProgressBar
    Private WithEvents brBarraAcciones As DevExpress.XtraBars.Bar
    Private WithEvents bbiProcesar As DevExpress.XtraBars.BarButtonItem
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
    Friend WithEvents vpBank As DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider
    Friend WithEvents lueSociedad As DevExpress.XtraEditors.LookUpEdit
End Class
