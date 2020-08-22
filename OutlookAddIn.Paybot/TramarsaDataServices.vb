Imports System.Collections
Imports System.Data
Imports System.Windows.Forms
Imports System.IO
Imports BigStick.Http
Imports System.Net

Public Class Tramarsa
    Dim oLogProcessUpdate As New LogProcessUpdate

    Friend Function SendData(drSource As DataRow) As ArrayList
        Dim aResult As New ArrayList
        Dim bResult As Boolean = True
        Dim oRestDialer As New BigStick.Http.RestDialer
        Dim sUri As String = "http://plataformadepagosweb.tramarsa.com.pe/api/deuda/importar"

        Dim oRequest As WebRequest
        Dim oResponse As WebResponse

        Dim aFleteList As New ArrayList
        aFleteList.Add(drSource(9))
        aFleteList.Add(drSource(10))
        aFleteList.Add(drSource(1))
        aFleteList.Add(drSource(3))
        aFleteList.Add(drSource(7))
        aFleteList.Add(drSource(8))
        aFleteList.Add(drSource(6))
        aFleteList.Add(drSource(4))
        aFleteList.Add(drSource(6))
        aFleteList.Add(1)
        aFleteList.Add(0)
        Dim oParams As New System.Collections.Generic.Dictionary(Of String, Object)
        oParams.Add("Token", Guid.NewGuid)
        oParams.Add("FleteList", aFleteList)

        Try
            'oRestDialer.PostJSON(Of oResponse, oRequest)(oParams, sUri, "")
        Catch ex As Exception

        End Try
        Return aResult
    End Function
End Class
