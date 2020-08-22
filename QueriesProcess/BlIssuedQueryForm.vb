Imports System.IO
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Grid

Public Class BlIssuedQueryForm
    Dim oCreateMailItem As New CreateMailItem
    Dim oLogFileGenerate As New LogFileGenerate
    Dim oDataAccess As New DataAccess
    Dim oGridControl As New GridControl
    Dim oGridView As New GridView
    Dim oProcessCode As String = "BIS"

    Private Sub BlIssuedQueryForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim oToday As Date = DateAdd(DateInterval.Day, 0, Now)
        Dim sPath As String = Path.GetTempPath
        Dim sFileName = "C:\TEMP\BLS EMITIDOS HASTA " & Replace(Format(oToday, "hh tt"), ".", "").ToUpper & " DEL " & Format(oToday, "dd.MM.yyyy") & ".xlsx"
        Dim dtSource As New DataTable
        Try
            dtSource = oDataAccess.ExecuteAccessQuery("SELECT blno AS BL, DateValue(date_up) AS FECHA, Cstr(TimeValue(date_up)) AS HORA FROM EmisionPeru WHERE date_up >= " & Format(oToday, "#yyyy-MM-dd 00:00#") & " AND date_up <=" & Format(oToday, "#yyyy-MM-dd 23:59#") & " ORDER BY 3").Tables(0)
            oGridControl.ViewCollection.Add(oGridView)
            oGridControl.MainView = oGridView
            oGridControl.BindingContext = New BindingContext()
            oGridControl.DataSource = dtSource
            oGridView.PopulateColumns()
            oGridView.OptionsPrint.AutoWidth = False
            oGridView.BestFitMaxRowCount = oGridView.RowCount
            oGridView.ExportToXlsx(sFileName)
            If Not IO.File.Exists(sFileName) Then
                'System.Diagnostics.Process.Start(sFileName)
                oLogFileGenerate.TextFileUpdate("EMISIONBLS", "No se encontró el archivo " & sFileName)
                Return
            End If
            Dim drMailProcess As DataRow = GetMailProcess(oProcessCode)
            oCreateMailItem.mailAttachment.Add(sFileName)
            If Not drMailProcess("MailTo") Is Nothing Then
                oCreateMailItem.mailTo = drMailProcess("MailTo")
            End If
            If Not drMailProcess("MailCC") Is Nothing Then
                oCreateMailItem.mailCc = drMailProcess("MailCC")
            End If
            If Not drMailProcess("MailBCC") Is Nothing Then
                oCreateMailItem.mailBcc = drMailProcess("MailBCC")
            End If
            oCreateMailItem.mailSubject = "BLS EMITIDOS HASTA " & Replace(Format(oToday, "hh tt"), ".", "").ToUpper & " DEL " & Format(oToday, "dd.MM.yyyy")
            oCreateMailItem.mailHtmlBody.AppendText("<html><body lang=ES style='tab-interval:35.4pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
            oCreateMailItem.mailHtmlBody.AppendText("Estimados Señores:<br><br>")
            oCreateMailItem.mailHtmlBody.AppendText("Adjunto podrán encontrar reporte de BLs emitidos hoy hasta las " & oToday.ToShortTimeString & "<br><br>")
            oCreateMailItem.mailHtmlBody.AppendText("Por favor confirmar si la información está conforme.<br><br>")
            oCreateMailItem.mailHtmlBody.AppendText("Saludos,<br><br>")
            oCreateMailItem.mailHtmlBody.AppendText("Customer Service Import<br>")
            oCreateMailItem.mailHtmlBody.AppendText("Hapag-Lloyd(Perú)")
            oCreateMailItem.mailHtmlBody.AppendText("</html></body>")
            oCreateMailItem.CreateCustomMessage("Send", True, False)
            oLogFileGenerate.TextFileUpdate("EMISIONBLS", "El procesó se ejecutó satisfactoriamente")
        Catch ex As Exception
            oLogFileGenerate.TextFileUpdate("EMISIONBLS", "El procesó no se ejecutó satisfactoriamente dibido al siguiente error: " & ex.Message)
            oCreateMailItem.SendErrorMessage(oCreateMailItem, "EMISIONBLS", "El procesó no se ejecutó satisfactoriamente dibido al siguiente error: " & ex.Message)
        End Try

        End
    End Sub

    Friend Function GetMailProcess(ProcessCode As String) As DataRow
        Dim drMail As DataRow
        drMail = oDataAccess.ExecuteAccessQuery("SELECT * FROM MailProcess WHERE ProcessCode='" & ProcessCode & "'").Tables(0).Rows(0)
            Return drMail
    End Function


End Class
