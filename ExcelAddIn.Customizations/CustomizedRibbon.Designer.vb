Partial Class CustomizedRibbon
    Inherits Microsoft.Office.Tools.Ribbon.RibbonBase

    <System.Diagnostics.DebuggerNonUserCode()> _
    Public Sub New(ByVal container As System.ComponentModel.IContainer)
        MyClass.New()

        'Requerido para la compatibilidad con el Diseñador de composiciones de clases Windows.Forms
        If (container IsNot Nothing) Then
            container.Add(Me)
        End If

    End Sub

    <System.Diagnostics.DebuggerNonUserCode()> _
    Public Sub New()
        MyBase.New(Globals.Factory.GetRibbonFactory())

        'El Diseñador de componentes requiere esta llamada.
        InitializeComponent()

    End Sub

    'Component invalida a Dispose para limpiar la lista de componentes.
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

    'Requerido por el Diseñador de componentes
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de componentes requiere el siguiente procedimiento
    'Se puede modificar usando el Diseñador de componentes.
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CustomizedRibbon))
        Me.Tab1 = Me.Factory.CreateRibbonTab
        Me.Group1 = Me.Factory.CreateRibbonGroup
        Me.Tab2 = Me.Factory.CreateRibbonTab
        Me.Group2 = Me.Factory.CreateRibbonGroup
        Me.btResultadoOperativo = Me.Factory.CreateRibbonButton
        Me.Group3 = Me.Factory.CreateRibbonGroup
        Me.btIncomeReconciliation = Me.Factory.CreateRibbonButton
        Me.Tab1.SuspendLayout()
        Me.Tab2.SuspendLayout()
        Me.Group2.SuspendLayout()
        Me.Group3.SuspendLayout()
        '
        'Tab1
        '
        Me.Tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office
        Me.Tab1.Groups.Add(Me.Group1)
        Me.Tab1.Label = "TabAddIns"
        Me.Tab1.Name = "Tab1"
        '
        'Group1
        '
        Me.Group1.Label = "Group1"
        Me.Group1.Name = "Group1"
        '
        'Tab2
        '
        Me.Tab2.Groups.Add(Me.Group2)
        Me.Tab2.Groups.Add(Me.Group3)
        Me.Tab2.Label = "HAPAG LLOYD PERÚ"
        Me.Tab2.Name = "Tab2"
        '
        'Group2
        '
        Me.Group2.Items.Add(Me.btResultadoOperativo)
        Me.Group2.Label = "Reportes"
        Me.Group2.Name = "Group2"
        Me.Group2.Visible = False
        '
        'btResultadoOperativo
        '
        Me.btResultadoOperativo.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge
        Me.btResultadoOperativo.Image = CType(resources.GetObject("btResultadoOperativo.Image"), System.Drawing.Image)
        Me.btResultadoOperativo.Label = "FINAL RECAP"
        Me.btResultadoOperativo.Name = "btResultadoOperativo"
        Me.btResultadoOperativo.ShowImage = True
        '
        'Group3
        '
        Me.Group3.Items.Add(Me.btIncomeReconciliation)
        Me.Group3.Label = "Business Administration"
        Me.Group3.Name = "Group3"
        '
        'btIncomeReconciliation
        '
        Me.btIncomeReconciliation.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge
        Me.btIncomeReconciliation.Image = CType(resources.GetObject("btIncomeReconciliation.Image"), System.Drawing.Image)
        Me.btIncomeReconciliation.Label = "Incomes Reconciliation"
        Me.btIncomeReconciliation.Name = "btIncomeReconciliation"
        Me.btIncomeReconciliation.ShowImage = True
        '
        'CustomizedRibbon
        '
        Me.Name = "CustomizedRibbon"
        Me.RibbonType = "Microsoft.Excel.Workbook"
        Me.Tabs.Add(Me.Tab1)
        Me.Tabs.Add(Me.Tab2)
        Me.Tab1.ResumeLayout(False)
        Me.Tab1.PerformLayout()
        Me.Tab2.ResumeLayout(False)
        Me.Tab2.PerformLayout()
        Me.Group2.ResumeLayout(False)
        Me.Group2.PerformLayout()
        Me.Group3.ResumeLayout(False)
        Me.Group3.PerformLayout()

    End Sub

    Friend WithEvents Tab1 As Microsoft.Office.Tools.Ribbon.RibbonTab
    Friend WithEvents Group1 As Microsoft.Office.Tools.Ribbon.RibbonGroup
    Friend WithEvents Tab2 As Microsoft.Office.Tools.Ribbon.RibbonTab
    Friend WithEvents Group2 As Microsoft.Office.Tools.Ribbon.RibbonGroup
    Friend WithEvents btResultadoOperativo As Microsoft.Office.Tools.Ribbon.RibbonButton
    Friend WithEvents Group3 As Microsoft.Office.Tools.Ribbon.RibbonGroup
    Friend WithEvents btIncomeReconciliation As Microsoft.Office.Tools.Ribbon.RibbonButton
End Class

Partial Class ThisRibbonCollection

    <System.Diagnostics.DebuggerNonUserCode()> _
    Friend ReadOnly Property Ribbon1() As CustomizedRibbon
        Get
            Return Me.GetRibbon(Of CustomizedRibbon)()
        End Get
    End Property
End Class
