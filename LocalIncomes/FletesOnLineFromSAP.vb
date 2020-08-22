Imports System.Collections
Imports System.Data
Imports System.Windows.Forms
Imports System.IO
Imports BigStick.Http
Imports System.Net
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Grid

Public Class FletesOnLineFromSAP

    Dim oLogProcessUpdate As New LogProcessUpdate
    Dim oLogFileGenerate As New LogFileGenerate
    Dim oSendMessage As New SendMessage
    Dim iLogProcess As Integer = 0
    Dim iLogProcessItem As Integer = 0
    Dim dtInvoiceIssued, dtOnLinePaymentAudit, dtOnLinePaymentQueue, dtOnLinePayment, dtErrorProcess As New DataTable
    Dim dDateTime1, dDateTime2 As DateTime

    Friend Sub StartProcess(Items As Object)
        Dim CodeProcess As String = "FOL"
        Dim oMailItems As Outlook.MailItem = Items
        Dim sFileName = FileIO.FileSystem.GetTempFileName
        Dim aResult As New ArrayList
        dDateTime1 = Now
        dtErrorProcess = ExecuteAccessQuery("SELECT * FROM ErrorProcess WHERE CodeProcess='" & CodeProcess & "'", "").Tables(0)
        iLogProcess = oLogProcessUpdate.GetIdLogProcess(CodeProcess)
        For a = 1 To oMailItems.Attachments.Count
            If oMailItems.Attachments(a).FileName.ToUpper.Contains("XLS") Then
                sFileName = My.Settings.AttachedFilePath & "\" & Format(Now, "ddMMyyyy HHmmss") & " - " & oMailItems.Attachments(a).FileName
                Try
                    oMailItems.Attachments(a).SaveAsFile(sFileName)
                Catch ex As Exception
                    oLogFileGenerate.TextFileUpdate("FLETES ONLINE", ex.Message)
                    oSendMessage.SendErrorMessage(oMailItems, "FLETES ONLINE", ex.Message, Nothing)
                End Try

                If Not IO.File.Exists(sFileName) Then
                    oLogFileGenerate.TextFileUpdate("FLETES ONLINE", "No se descargó el archivo adjunto.")
                    oSendMessage.SendErrorMessage(oMailItems, "FLETES ONLINE", "No se descargó el archivo adjunto.", Nothing)
                End If
            End If
        Next

        Dim dtSource As New DataTable
        Dim dtInvoiceType As New DataTable

        dtSource = LoadExcel(sFileName, "{0}").Tables(0)
        If dtSource.Rows.Count = 0 Then
            Return
        End If
        dtInvoiceType = ExecuteAccessQuery("SELECT CodeInvoiceType FROM InvoiceType WHERE Include=True", "").Tables(0)
        dtInvoiceIssued = ExecuteAccessQuery("SELECT * FROM InvoiceIssued WHERE IdLogProcess=0", "").Tables(0)

        For r = 0 To dtSource.Rows.Count - 1
            Dim drSource As DataRow = dtSource.Rows(r)
            'drSource(7) = drSource(7) * -1
            If dtInvoiceType.Select("CodeInvoiceType='" & drSource(4) & "'").Length = 0 Then
                Continue For
            End If
            If drSource(2).trim = "" Or drSource(9) = 0 Then
                Continue For
            End If
            If Mid(drSource(10).ToString, 1, 2) = "01" Then
                Continue For
            End If
            If drSource(11).ToString = "A5" Then
                Continue For
            End If
            If drSource(19).ToString = "V" Then
                Continue For
            End If
            If drSource(4).ToString = "I1" And Not drSource(14).ToString.Contains({"ZB00", "ZB57"}) Then
                Continue For
            End If
            If drSource(4).ToString = "I7" And Not drSource(15).ToString.Contains({"ZB00", "ZB57"}) Then
                Continue For
            End If
            'If DocumentExists(drSource) Then
            '    Continue For
            'End If
            iLogProcessItem = oLogProcessUpdate.GetLogProcessItem("FOL", iLogProcess)
            InsertRowProcess(drSource)
        Next
        dtOnLinePayment = ExecuteAccessQuery("SELECT * FROM OnLinePaymentQry WHERE IdLogProcess=" & iLogProcess.ToString, "").Tables(0)
        Dim dtPending As New DataTable
        dtPending = ExecuteAccessQuery("SELECT * FROM OnLinePaymentPendingQry", "").Tables(0)
        For r = 0 To dtPending.Rows.Count - 1
            dtOnLinePayment.ImportRow(dtPending.Rows(r))
        Next
        If dtOnLinePayment.Rows.Count = 0 Then
            oLogFileGenerate.TextFileUpdate("PAYBOT", "La consulta OnLinePaymentQry no retornó datos.")
            Return
        End If
        dtOnLinePaymentAudit = ExecuteAccessQuery("SELECT * FROM OnLinePaymentAudit WHERE IdLogProcess=0", "").Tables(0)
        dtOnLinePaymentQueue = ExecuteAccessQuery("SELECT * FROM OnLinePaymentQueue WHERE IdLogProcess=0", "").Tables(0)
        For r = 0 To dtOnLinePayment.Rows.Count - 1
            Dim drAudit As DataRow = dtOnLinePayment.Rows(r)
            If IsDBNull(drAudit(7)) Then
                Continue For
            End If
            Try
                aResult.AddRange(SendData(drAudit))
            Catch ex As Exception
                oLogProcessUpdate.SetDescriptionLogProcess(iLogProcess, iLogProcessItem, ex.Message)
            Finally
                InsertAudit(drAudit, oMailItems.Sender.Address, aResult)
            End Try
        Next
        Try
            dtQuery = ExecuteAccessQuery("SELECT * FROM OnLinePaymentAuditQry WHERE IdLogProcess=" & iLogProcess.ToString, "").Tables(0)
            Dim sMsgResponse As String = ""
            If dtQuery.Select("Error=True").Length = 0 Then
                dDateTime2 = Now
                sMsgResponse += "<br>Fecha/Hora Inicial: " & dDateTime1.ToString
                sMsgResponse += "<br>Fecha/Hora Final: " & dDateTime2.ToString
                sMsgResponse += "<br><br>El proceso finalizó satisfactoriamente."
                oSendMessage.SendNewMessage("PRC_OK", oMailItems, "FLETES ONLINE", sMsgResponse)
            Else
                Dim sAttachFileName = My.Settings.LogFilePath & "\" & "LOG" & Format(Today, "yyyyMMddHHmm") & ".xlsx"
                Dim oGridControl As New GridControl
                Dim oGridView As New GridView
                oGridControl.ViewCollection.Add(oGridView)
                oGridControl.MainView = oGridView
                oGridControl.BindingContext = New BindingContext()
                oGridControl.DataSource = dtQuery
                oGridView.PopulateColumns()
                oGridView.OptionsPrint.AutoWidth = True
                oGridView.BestFitColumns()
                oGridControl.ForceInitialize()
                oGridControl.MainView.ExportToXlsx(sAttachFileName)
                If IO.File.Exists(sAttachFileName) Then
                    dDateTime2 = Now
                    sMsgResponse += "<br>Fecha/Hora Inicial: " & dDateTime1.ToString
                    sMsgResponse += "<br>Fecha/Hora Final: " & dDateTime2.ToString
                    sMsgResponse += "<br><br>El proceso identificó varios errores, por favor revise el archivo adjunto."
                    oSendMessage.SendErrorMessage(oMailItems, "FLETES ONLINE", sMsgResponse, sAttachFileName)
                End If
            End If
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate("FLETES ONLINE", ex.Message)
            oSendMessage.SendErrorMessage(oMailItems, "FLETES ONLINE", ex.Message, Nothing)
        End Try

    End Sub

    Function DocumentExists(drSource As DataRow) As Boolean
        Dim bResult As Boolean = False
        Dim dtQuery As New DataTable
        Dim sQuery As String = "SELECT Blno FROM InvoiceIssued WHERE CompanyCode='" & drSource(0) & "' AND DocumentNumber='" & drSource(3) & "' AND Blno='HLCU" & drSource(10) & "'"
        If ExecuteAccessQuery(sQuery, "").Tables(0).Rows.Count > 0 Then
            bResult = True
        End If
        Return bResult
    End Function

    Private Sub InsertRowProcess(drSource As DataRow)
        Dim iPos As Integer = 0
        'Inserta datos en tabla InvoiceIssued
        dtInvoiceIssued.Rows.Add()
        iPos = dtInvoiceIssued.Rows.Count - 1
        dtInvoiceIssued.Rows(iPos)(0) = iLogProcess
        dtInvoiceIssued.Rows(iPos)(1) = iLogProcessItem
        dtInvoiceIssued.Rows(iPos)(2) = drSource(0)
        dtInvoiceIssued.Rows(iPos)(3) = drSource(12)
        dtInvoiceIssued.Rows(iPos)(4) = drSource(1)
        dtInvoiceIssued.Rows(iPos)(5) = Replace(drSource(2), "'", " ")
        dtInvoiceIssued.Rows(iPos)(6) = drSource(3)
        dtInvoiceIssued.Rows(iPos)(7) = drSource(4)
        dtInvoiceIssued.Rows(iPos)(8) = drSource(5)
        dtInvoiceIssued.Rows(iPos)(9) = drSource(6)
        dtInvoiceIssued.Rows(iPos)(10) = drSource(7)
        dtInvoiceIssued.Rows(iPos)(11) = "HLCU" & drSource(10)
        dtInvoiceIssued.Rows(iPos)(12) = drSource(9)
        dtInvoiceIssued.Rows(iPos)(13) = drSource(8)
        dtInvoiceIssued.Rows(iPos)(14) = 0
        dtInvoiceIssued.Rows(iPos)(15) = drSource(14)
        dtInvoiceIssued.Rows(iPos)(16) = drSource(15)
        dtInvoiceIssued.Rows(iPos)(17) = drSource(16)
        dtInvoiceIssued.Rows(iPos)(18) = drSource(17)
        dtInvoiceIssued.Rows(iPos)(19) = drSource(18)
        dtInvoiceIssued.Rows(iPos)(20) = drSource(19)
        dtInvoiceIssued.Rows(iPos)(21) = drSource(20)
        Try
            InsertIntoAccess("InvoiceIssued", dtInvoiceIssued.Rows(iPos), "", Nothing, Nothing)
        Catch ex As Exception
            oLogProcessUpdate.SetDescriptionLogProcess(iLogProcess, iLogProcessItem, ex.Message)
        End Try
    End Sub

    Private Sub InsertAudit(drSource As DataRow, MailSender As String, aWSResponse As ArrayList)
        Dim iPos As Integer = 0
        Dim sCondition As String = ""
        Dim sValue As String = ""
        'Inserta datos en tabla OnLinePaymentAudit
        dtOnLinePaymentAudit.Rows.Add()
        iPos = dtOnLinePaymentAudit.Rows.Count - 1
        dtOnLinePaymentAudit.Rows(iPos)(0) = drSource(0)
        dtOnLinePaymentAudit.Rows(iPos)(1) = drSource(9)
        dtOnLinePaymentAudit.Rows(iPos)(2) = MailSender
        dtOnLinePaymentAudit.Rows(iPos)(3) = Now.ToString
        dtOnLinePaymentAudit.Rows(iPos)(4) = aWSResponse(0)
        dtOnLinePaymentAudit.Rows(iPos)(5) = aWSResponse(1)
        dtOnLinePaymentAudit.Rows(iPos)(6) = aWSResponse(2)
        Try
            sCondition = "IdLogProcess=" & drSource(0).ToString & " AND Blno='" & drSource(9) & "'"
            If ExecuteAccessQuery("SELECT * FROM OnLinePaymentAudit WHERE " & sCondition, "").Tables(0).Rows.Count = 0 Then
                InsertIntoAccess("OnLinePaymentAudit", dtOnLinePaymentAudit.Rows(iPos), "", Nothing, Nothing)
            Else
                sValue = "SenderMail='" & MailSender & "',SendingDate='" & Now.ToString & "',Sent=" & aWSResponse(0).ToString & ",Error=" & aWSResponse(1) & ",WebServiceResponse='" & aWSResponse(2) & "'"
                UpdateAccess("OnLinePaymentAudit", sCondition, sValue, "")
            End If
        Catch ex As Exception
            oLogProcessUpdate.SetDescriptionLogProcess(iLogProcess, iLogProcessItem, ex.Message)
        End Try
    End Sub

    Private Sub InsertQueue(drSource As DataRow)
        Dim iPos As Integer = 0
        'Inserta datos en tabla OnLinePaymentAudit
        dtOnLinePaymentQueue.Rows.Add()
        iPos = dtOnLinePaymentQueue.Rows.Count - 1
        dtOnLinePaymentQueue.Rows(iPos)(0) = drSource(0)
        dtOnLinePaymentQueue.Rows(iPos)(1) = drSource(9)
        dtOnLinePaymentQueue.Rows(iPos)(2) = 0
        dtOnLinePaymentQueue.Rows(iPos)(3) = Now.ToString
        dtOnLinePaymentQueue.Rows(iPos)(4) = DateAdd(DateInterval.Day, 1, Now).ToString
        Try
            InsertIntoAccess("OnLinePaymentQueue", dtOnLinePaymentQueue.Rows(iPos), "", Nothing, Nothing)
        Catch ex As Exception
            oLogProcessUpdate.SetDescriptionLogProcess(iLogProcess, iLogProcessItem, ex.Message)
        End Try
    End Sub

    Function SendData(drSource As DataRow) As ArrayList
        Dim aResult As New ArrayList
        Dim request As New ImportarFletesRequest()
        Dim token As String = Guid.NewGuid.ToString.ToUpper
        Dim _status As Integer = 0
        If IsDBNull(drSource(11)) Then
            drSource(11) = ""
        End If
        If CDec(drSource(8)) = 0 Or drSource(11) <> "" Then
            drSource(8) = 1
            _status = 3
        End If
        Dim bSent, bError As Boolean
        Try
            request.Token = token
            request.FleteList.Add(New FleteDTO() With { _
                .BL = drSource(9), _
                .Booking = drSource(10), _
                .Identificacion = drSource(1), _
                .RazonSocial = drSource(3), _
                .Moneda = drSource(7), _
                .Monto = CDec(drSource(8)), _
                .CodigoCobro = "FR", _
                .Comprobante = drSource(9), _
                .FechaVencimiento = DateTime.Now, _
                .ExoneradoVencimiento = False, _
                .EstadoDocumento = _status _
            })

            bSent = True
            bError = False
            Dim bl, CodErr, sMessage As String
            Dim response = Importar(request)
            Dim result = response.Result.Success
            If Not result Then
                Throw New Exception(response.Result.Message)
                bSent = False
            End If
            sMessage = "Envío Satisfactorio"
            If response.ErrorList.Count > 0 Then
                bl = response.ErrorList(0).BL
                CodErr = response.ErrorList(0).CodigoError
                If Not CodErr.Contains({"", "00"}) Then
                    If CodErr.Contains({"21", "22", "23", "24"}) Then
                        bError = True
                        sMessage = "Error: " & CodErr & " - " & dtErrorProcess.Select("CodeError=" & CodErr)(0)("ErrorDescription")
                    Else
                        bError = True
                        sMessage = "Error: " & CodErr & " - " & dtErrorProcess.Select("CodeError=" & CodErr)(0)("ErrorDescription")
                        Throw New Exception(sMessage)
                    End If
                End If
            End If
            aResult.Add(bSent)
            aResult.Add(bError)
            aResult.Add(sMessage)
        Catch ex As Exception
            aResult.Add(bSent)
            aResult.Add(True)
            aResult.Add(ex.Message)
            oLogProcessUpdate.SetDescriptionLogProcess(iLogProcess, iLogProcessItem, ex.Message)
        End Try
        Return aResult
    End Function

    Public Shared Function Importar(request As ImportarFletesRequest) As ImportarFleteResponse
        'Dim url As String = My.Settings.TRM_UrlService
        Dim url As String = "http://104.45.136.32:1717/api/deuda/importar"
        Dim restDialer As New RestDialer()
        Dim response As ImportarFleteResponse = restDialer.PostJSON(Of ImportarFleteResponse, ImportarFletesRequest)(request, url, "")
        If response Is Nothing Then
            Throw New Exception("Formato no valido en respuesta de url: " & url)
        End If
        Return response
    End Function

    Public Class ImportarFletesRequest
        Inherits BaseRequest
        Public Sub New()
            Me.FleteList = New List(Of FleteDTO)()
        End Sub

        Public Property Token() As String
        Public Property FleteList() As List(Of FleteDTO)
    End Class

    Public Class FleteDTO
        'Public Property IdDocumentoComercial() As Integer
        'Public Property IdDeuda() As Integer
        Public Property BL() As String
        Public Property Booking() As String
        Public Property CodigoCobro() As String
        Public Property Identificacion() As String
        Public Property RazonSocial() As String
        Public Property Moneda() As String
        Public Property Monto() As Decimal
        Public Property Comprobante() As String
        Public Property FechaVencimiento() As System.Nullable(Of DateTime)
        Public Property ExoneradoVencimiento() As Boolean
        Public Property EstadoDocumento() As Integer
        '
        'Public Property EstadoImportacion() As String
    End Class

    Public Class BaseRequest
        Public Sub New()
            Me.Meta = New MetaRequest()
        End Sub

        Public Property Meta() As MetaRequest
    End Class

    Public Class MetaRequest
        Public Property Usuario() As String

        Public Property CurrentPage() As Integer
        Public Property Size() As Integer
    End Class

    Public Class ImportarFleteResponse
        Inherits BaseResponse
        Public Sub New()
            Me.ErrorList = New List(Of DeudaError)()
        End Sub

        Public Property ErrorList() As List(Of DeudaError)
    End Class

    Public Class DeudaError
        Public Property BL() As String
        Public Property Comprobante() As String
        Public Property CodigoCobro() As String
        Public Property CodigoError() As String
        Public Property [Error]() As String
    End Class

    Public Class BaseResponse
        Public Sub New()
            Me.Result = New Result()
            Me.Meta = New MetaResponse()
        End Sub

        Public Property Result() As Result
        Public Property Meta() As MetaResponse
    End Class

    Public Class Result
        Public Sub New()
            Me.Success = False
            Me.ErrCode = ""
            Me.Message = ""
            Me.Messages = New List(Of Result)()
        End Sub

        Public Property Success() As Boolean
        Public Property ErrCode() As String
        Public Property Message() As String
        Public Property IdError() As Guid
        Public Property Messages() As List(Of Result)
    End Class

    Public Class MetaResponse
        Public Property Total() As Integer
    End Class

End Class
