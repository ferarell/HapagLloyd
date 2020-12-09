Imports System.Windows.Forms
Imports Microsoft.Office.Interop
Imports System.Data
Imports System.Collections

Public Class SilStatusQuery
    Dim oLogProcessUpdate As New LogProcessUpdate
    Dim oLogFileGenerate As New LogFileGenerate
    Dim iLogProcess As Integer = 0
    Dim oMailItems As Outlook.MailItem = Nothing

    Friend Sub StartProcess(oItems As Object, sIdentifier As String)
        oMailItems = oItems
        Dim ProcessCode As String = "SIL"
        Dim QueryList As New ArrayList
        Dim sSubject(), sBody() As String
        sSubject = Split(Replace(oMailItems.Subject.ToUpper, sIdentifier, "").TrimStart, vbNewLine)
        sBody = Split(oMailItems.Body, vbNewLine)
        'Replace(oMailItems.Subject.ToUpper, sIdentifier, "").TrimEnd.TrimStart
        Try
            Dim dtResponse As New DataTable
            QueryList = GetTextFromMessage(sSubject, sBody)
            If QueryList.Count = 0 Then
                SendErrorMessageResponse(dtResponse)
                Return
            End If
            dtResponse = GetDataResult(QueryList)
            SendMessageResponse(dtResponse)
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate("SIL STATUS", "StartProcess - " & ex.Message)
        End Try

    End Sub

    Function GetDataResult(QueryList As ArrayList) As DataTable
        Dim dtResult As New DataTable
        Dim iPos As Integer = 0
        Dim sStatus As String = ""
        dtResult = ExecuteAccessQuery(drConfig("ConsultaSQL") & " WHERE BL='#'", "").Tables(0)
        Try
            For r = 0 To QueryList.Count - 1
                If r = 100 Then
                    Exit For
                End If
                Dim dtSilQuery As New DataTable
                If QueryList(r).ToString.Trim = "" Then
                    Continue For
                End If
                dtResult.Rows.Add()
                iPos = dtResult.Rows.Count - 1
                dtResult.Rows(iPos)(0) = QueryList(r).ToString.Trim
                If IsNumeric(QueryList(r).ToString) Then
                    'Busca por Booking
                    dtSilQuery = ExecuteAccessQuery("SELECT * FROM " & drConfig("Tabla") & " WHERE Booking=" & QueryList(r).ToString, "").Tables(0)
                End If
                'Busca por BL
                If dtSilQuery.Rows.Count = 0 Then
                    dtSilQuery = ExecuteAccessQuery("SELECT * FROM " & drConfig("Tabla") & " WHERE BL='" & QueryList(r) & "'", "").Tables(0)
                End If
                If dtSilQuery.Rows.Count = 0 Then
                    'Busca por BL (concatenando HLCU)
                    dtSilQuery = ExecuteAccessQuery("SELECT * FROM " & drConfig("Tabla") & " WHERE BL='HLCU" & QueryList(r) & "'", "").Tables(0)
                End If
                If dtSilQuery.Rows.Count = 0 Then
                    sStatus = "No tenemos registro para este embarque"
                    dtResult.Rows(iPos)(7) = sStatus 'Estado
                    Continue For
                End If
                'Asignación de Texto Status
                Dim SinDocCobranza As String = ""
                If IsDBNull(dtSilQuery.Rows(0)("Nro de Documento de Cobranza")) Then
                    dtSilQuery.Rows(0)("Nro de Documento de Cobranza") = ""
                End If
                If dtSilQuery.Rows(0)("Nro de Documento de Cobranza") = "" Then
                    SinDocCobranza = ". Importante indicar que aún no contamos con el documento de cobranza y copia fletada del bl,  de no contar con esta información no se podrá continuar con el proceso de pago"
                End If
                'Escenario 1
                If Not IsDBNull(dtSilQuery.Rows(0)("Status")) Then
                    If dtSilQuery.Rows(0)("Status").ToString.ToUpper = "SIL NO SOLICITADO PARA BL" Then
                        sStatus = "No tenemos registro para este embarque"
                    End If
                End If
                'Escenario 2
                If Not IsDBNull(dtSilQuery.Rows(0)("Fecha de llegada POD")) Then
                    If Today < dtSilQuery.Rows(0)("Fecha de llegada POD") Then
                        sStatus = "Carga en travesía , se le recuerda que el pago del SIL se hará efectivo cuando la carga llegue a destino y el consignatario haya realizado el pago correspondiente"
                    End If
                End If
                'Escenario 3
                If Not IsDBNull(dtSilQuery.Rows(0)("Fecha de llegada POD")) And Not IsDBNull(dtSilQuery.Rows(0)("Pago Confirmado por Agencia Colectora?")) Then
                    If dtSilQuery.Rows(0)("Fecha de llegada POD") <= Today And dtSilQuery.Rows(0)("Pago Confirmado por Agencia Colectora?") = "NO" Then
                        sStatus = "Su carga arribó a destino el " & Format(dtSilQuery.Rows(0)("Fecha de llegada POD"), "dd/MM/yyyy") & ", sin embargo aún no contamos con la confirmación de pago del recargo logístico (SIL) de parte del consignatario" & SinDocCobranza
                    End If
                End If
                'Escenario 4
                If Not IsDBNull(dtSilQuery.Rows(0)("Fecha de llegada POD")) And Not IsDBNull(dtSilQuery.Rows(0)("Pago Confirmado por Agencia Colectora?")) Then
                    If dtSilQuery.Rows(0)("Fecha de llegada POD") <= Today And dtSilQuery.Rows(0)("Pago Confirmado por Agencia Colectora?") = "YES" Then
                        sStatus = "Su carga arribó a destino el " & Format(dtSilQuery.Rows(0)("Fecha de llegada POD"), "dd/MM/yyyy") & ", y validamos que con fecha " & Format(dtSilQuery.Rows(0)("Fecha confirmación de pago oficina collectora"), "dd/MM/yyyy") & " hemos recibido la confirmación de pago del consignatario. Dentro de los siguientes 15 días útiles , el equipo de Finanzas programará el reembolso" & SinDocCobranza
                    End If
                End If
                'Escenario 5
                If Not IsDBNull(dtSilQuery.Rows(0)("Fecha programada deposito a operador")) Then
                    If dtSilQuery.Rows(0)("Fecha programada deposito a operador").ToString <> "" Then
                        sStatus = "Informamos que se cumplió con el procedimiento, el pago del recargo logístico (SIL) está programado para el " & Format(dtSilQuery.Rows(0)("Fecha programada deposito a operador"), "dd/MM/yyyy") & SinDocCobranza
                    End If
                End If
                'Escenario 6
                If Not IsDBNull(dtSilQuery.Rows(0)("Fecha de deposito a operador")) Then
                    If dtSilQuery.Rows(0)("Fecha de deposito a operador").ToString <> "" Then
                        sStatus = "Informamos que el SIL fue reembolsado el " & Format(dtSilQuery.Rows(0)("Fecha de deposito a operador"), "dd/MM/yyyy") & " al operador " & dtSilQuery.Rows(0)("Operador logistico")
                    End If
                End If
                'Escenario 7
                If Not IsDBNull(dtSilQuery.Rows(0)("Status")) Then
                    If dtSilQuery.Rows(0)("Status").ToString.ToUpper.Contains("PAGO RECHAZADO POR CNEE") Then
                        sStatus = "Informamos que el consignatario en destino ha rechazado el pago del SIL por lo tanto no se procederá con el reembolso del recargo logístico (SIL)"
                    End If
                End If
                'Escenario 8
                If Not IsDBNull(dtSilQuery.Rows(0)("Status")) Then
                    If dtSilQuery.Rows(0)("Status").ToString.ToUpper.Contains("SIL CON DISCREPANCIA") Then
                        sStatus = "Informamos que se encontraron  inconsistencias en los documentos presentados por Uds. , agradeceremos regularizar las observaciones de acuerdo a las instrucciones informadas por el equipo de Documentación."
                    End If
                End If
                'Asignación de datos en tabla de resultado
                dtResult.Rows(iPos)(1) = dtSilQuery.Rows(0)(1) 'BL
                dtResult.Rows(iPos)(2) = dtSilQuery.Rows(0)(0) 'Booking
                dtResult.Rows(iPos)(3) = dtSilQuery.Rows(0)(5) 'POL
                dtResult.Rows(iPos)(4) = dtSilQuery.Rows(0)(6) 'POD
                dtResult.Rows(iPos)(5) = dtSilQuery.Rows(0)(11) 'Importe
                dtResult.Rows(iPos)(6) = dtSilQuery.Rows(0)(3) 'Operador Logistico
                dtResult.Rows(iPos)(7) = sStatus 'Estado
            Next

        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate("SIL STATUS", "GetDataResult - " & ex.Message)
        End Try
        Return dtResult
    End Function

    Function GetTextFromMessage(sSubject() As String, sBody() As String) As ArrayList
        Dim aContent, aResult As New ArrayList
        Dim sValueText As String = ""
        Dim bInsert As Boolean = False
        For l = 0 To sSubject.Length - 1
            If sSubject(l) = "" Then
                Continue For
            End If
            If aContent.IndexOf(sSubject(l)) < 0 Then
                aContent.Add(sSubject(l))
            End If
        Next
        For l = 0 To sBody.Length - 1
            If sBody(l) = "" Then
                Continue For
            End If
            If aContent.IndexOf(sBody(l)) < 0 Then
                aContent.Add(sBody(l))
            End If
        Next
        'sSubject.
        'For p = 1 To sSubject.Length + 1
        '    If Mid(sSubject, p, 1).Contains({" ", ",", ";"}) Or p = sSubject.Length + 1 Then
        '        If aContent.IndexOf(sValueText) < 0 Then
        '            aContent.Add(sValueText)
        '            sValueText = ""
        '        End If
        '    End If
        '    sValueText += Mid(sSubject, p, 1)
        'Next
        'For p = 1 To sBody.Length + 1
        '    If Mid(sBody, p, 1).Contains({oSpace, ",", ";"}) Or p = sBody.Length + 1 Then
        '        If aContent.IndexOf(sValueText) < 0 Then
        '            aContent.Add(sValueText)
        '            sValueText = ""
        '        End If
        '    End If
        '    sValueText += Mid(sBody, p, 1)
        'Next
        For c = 0 To aContent.Count - 1
            bInsert = False
            If IsNumeric(aContent(c)) And aContent(c).ToString.Length = 8 Then
                bInsert = True
            End If
            If aContent(c).ToString.Contains("HLCU") Then 'and sFoundText.Length = 16
                bInsert = True
            End If
            'If sFoundText.Length = 12 And IsNumeric(Mid(sFoundText, 7, 10)) Then
            '    bInsert = True
            'End If
            If bInsert Then
                If aResult.IndexOf(aContent(c)) < 0 Then
                    aResult.Add(aContent(c))
                End If
            End If
        Next
        Return aResult
    End Function

    Private Sub SendMessageResponse(dtResponse As DataTable)
        Dim oMessage As New SendMessage
        Try
            oMessage.dtSourceHtml = dtResponse
            oMessage.Response(oMailItems, Nothing)
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate("SIL STATUS", "SendMessageResponse - " & ex.Message)
        End Try
    End Sub

    Private Sub SendErrorMessageResponse(dtResponse As DataTable)
        Dim oMessage As New SendMessage
    End Sub


End Class
