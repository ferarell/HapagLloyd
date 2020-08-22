Imports System.Data
Imports System.Windows.Forms

Public Class GetHtmlTable

    Friend Function GenerateTable(dtSource As DataTable) As String
        Dim Msg As New RichTextBox
        'Inicio de Tabla
        Msg.AppendText("<table class=MsoTableGrid border=1 cellspacing=0 cellpadding=0")
        'Columns
        Msg.AppendText("<tr style='mso-yfti-irow:0;mso-yfti-firstrow:yes'>")
        For col = 0 To dtSource.Columns.Count - 1
            Msg.AppendText("<td width=auto valign=top style='width:134.45pt;border:solid windowtext 1.0pt;")
            Msg.AppendText("mso-border-alt:solid windowtext .5pt;background:#FFC000;padding:0cm 5.4pt 0cm 5.4pt'>")
            Msg.AppendText("<p class=MsoNormal align=center style='margin-bottom:0cm;margin-bottom:.0001pt;")
            Msg.AppendText("text-align:center;line-height:normal;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>" & dtSource.Columns(col).ColumnName & "</p></td>")
        Next
        Msg.AppendText("</tr>")
        'DataRows
        Msg.AppendText("<tr style='mso-yfti-irow:1;mso-yfti-lastrow:yes'>")
        For r = 0 To dtSource.Rows.Count - 1
            For c = 0 To dtSource.Columns.Count - 1
                If dtSource.Columns(c).DataType.Name = "String" Then
                    If IsDBNull(dtSource.Rows(r)(c)) Then
                        dtSource.Rows(r)(c) = ""
                    End If
                End If
                'DataColumn
                Msg.AppendText("<td align=center width=auto valign=top style='width:134.45pt;font-size:10.0pt;font-family:""Tahoma"",sans-serif'>")
                Msg.AppendText("<p>" & dtSource.Rows(r)(c).ToString.Trim & "</p></td>")
            Next
            Msg.AppendText("</tr>")
        Next
        Msg.AppendText("</table><br>")
        'Fin de Tabla
        Return Msg.Text
    End Function
End Class
