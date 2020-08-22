Public Class TiposCambioBE

    Private _CodigoMoneda As String
    Public Property CodigoMoneda() As String
        Get
            Return _CodigoMoneda
        End Get

        Set(ByVal value As String)
            _CodigoMoneda = value
        End Set
    End Property

    Private _Fecha As DateTime
    Public Property Fecha() As DateTime
        Get
            Return _Fecha
        End Get

        Set(ByVal value As DateTime)
            _Fecha = value
        End Set
    End Property

    Private _TipoRegistro As String
    Public Property TipoRegistro() As String
        Get
            Return _TipoRegistro
        End Get

        Set(ByVal value As String)
            _TipoRegistro = value
        End Set
    End Property

    Private _TcLocalV As Decimal
    Public Property TcLocalV() As Decimal
        Get
            Return _TcLocalV
        End Get

        Set(ByVal value As Decimal)
            _TcLocalV = value
        End Set
    End Property

    Private _TcLocalC As Decimal
    Public Property TcLocalC() As Decimal
        Get
            Return _TcLocalC
        End Get

        Set(ByVal value As Decimal)
            _TcLocalC = value
        End Set
    End Property

    Private _TcDolarV As Decimal
    Public Property TcDolarV() As Decimal
        Get
            Return _TcDolarV
        End Get

        Set(ByVal value As Decimal)
            _TcDolarV = value
        End Set
    End Property

    Private _TcDolarC As Decimal
    Public Property TcDolarC() As Decimal
        Get
            Return _TcDolarC
        End Get

        Set(ByVal value As Decimal)
            _TcDolarC = value
        End Set
    End Property

End Class
