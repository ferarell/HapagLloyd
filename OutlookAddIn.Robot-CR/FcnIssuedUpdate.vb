Imports Microsoft.Office.Interop
Imports System.Data

Public Class FcnIssuedUpdate
    Dim oLogFileGenerate As New LogFileGenerate
    Dim oDataAccess As New DataAccess
    Dim oCreateMailItem As New CreateMailItem
    Friend oMailItem As Outlook.MailItem
    Friend drConfiguration As DataRow

    Friend Sub StartProcess()
        Dim sFileName = FileIO.FileSystem.GetTempFileName
        Dim dtSource, dtResult As New DataTable
        Dim sVoyage As String = Replace(oMailItem.Subject, drConfiguration("Identifier"), "").Trim

        For a = 1 To oMailItem.Attachments.Count
            If oMailItem.Attachments(a).FileName.ToUpper.Contains("XLS") Then
                sFileName = My.Settings.ProcessFilePath & "\" & Format(Now, "ddMMyyyy HHmmss") & " - " & oMailItem.Attachments(a).FileName
                oMailItem.Attachments(a).SaveAsFile(sFileName)
                If Not IO.File.Exists(sFileName) Then
                    oLogFileGenerate.TextFileUpdate(drConfiguration("Identifier"), "No se descargó el archivo adjunto.")
                    oCreateMailItem.SendErrorMessage(oMailItem, drConfiguration("Identifier"), "No se descargó el archivo adjunto.")
                    Return
                End If
            End If
        Next
        dtResult = oDataAccess.ExecuteAccessQuery("SELECT * FROM " & drConfiguration("TableSQL") & " WHERE Blno='#'").Tables(0)
        dtSource = LoadExcel(sFileName, "DOCUMENTOS$").Tables(0)
        If dtSource.Rows.Count = 0 Then
            Return
        End If
        For r = 0 To dtSource.Rows.Count - 1
            Dim oRow As DataRow = dtSource.Rows(r)
            If IsDBNull(oRow("NUMERO_DOC_VIAJE")) Then
                Continue For
            End If
            If oRow("NUMERO_DOC_VIAJE") = "" Then
                Continue For
            End If
            dtResult.Rows.Add(oRow("NUMERO_DOC_VIAJE"), oRow("FECHA_DOCUMENTO"), sVoyage, oMailItem.SenderEmailAddress, oMailItem.To, oMailItem.Subject, Environment.UserName, Now)
        Next
        For r = 0 To dtResult.Rows.Count - 1
            If oDataAccess.ExecuteAccessQuery("SELECT * FROM " & drConfiguration("TableSQL") & " WHERE blno='" & dtResult.Rows(r)("blno") & "'").Tables(0).Rows.Count = 0 Then
                If Not oDataAccess.InsertIntoAccess(drConfiguration("TableSQL"), dtResult.Rows(r)) Then
                    oLogFileGenerate.TextFileUpdate(drConfiguration("Identifier"), "Error al insertar el BL: " & dtResult.Rows(r)("blno"))
                End If
            End If
        Next
        oLogFileGenerate.TextFileUpdate(drConfiguration("Identifier"), "El proceso terminó satisfacotoriamente")

    End Sub

End Class
