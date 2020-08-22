Imports DevExpress.XtraEditors.DXErrorProvider

Public Class CustomValidationRule
    Inherits ValidationRule

    Public Overrides Function Validate(ByVal control As Control, ByVal value As Object) As Boolean
        Dim res As Boolean = False
        If IsDBNull(value) Then
            Return res
        End If
        Dim str As String = CStr(value)
        If Not String.IsNullOrEmpty(str) Then
            If Not String.IsNullOrEmpty(str.Trim()) Then
                res = True
            End If
        End If
        Return res
    End Function
End Class


