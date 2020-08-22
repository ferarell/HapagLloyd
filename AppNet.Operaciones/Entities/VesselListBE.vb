Imports System.Collections

Public Class VesselListBE
    Public Property Title() As String

    'Private _Title As String

    Public Sub New()
        Me.VesselList = New List(Of VesselListBE)()
    End Sub

    Public Property VesselList() As List(Of VesselListBE)

    'Public Class VesselDTO
    '    Public Property Title() As String
    'End Class

    'Public Property Title() As String
    '    Get
    '        Return _Title
    '    End Get

    '    Set(ByVal value As String)
    '        _Title = value
    '    End Set
    'End Property

End Class
