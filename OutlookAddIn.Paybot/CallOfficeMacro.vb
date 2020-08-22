Imports Microsoft.Office.Interop
''' <summary>
''' clase definida para el manejo de excel
''' </summary>
''' <remarks></remarks>
Public Class CallOfficeMacro
    ''' <summary>
    ''' ejecuta una macros de excel
    ''' </summary>
    ''' <param name="carpetaObjetivo">carpeta donde se guarda el archivo</param>
    ''' <param name="Titulo">titulo con el que se guarda el archivo</param>
    ''' <param name="macros">archivo xlsm (excel que soporta macros)</param>
    ''' <returns>retorna un entero segun exito o falla de la ejecucion</returns>
    ''' <remarks></remarks>
    Public Function macro(ByVal carpetaObjetivo As String, ByVal Titulo As String, ByVal ArchivoMacro As String, NombreMacro As String) As Integer
        Try
            Dim oExcel As Excel.ApplicationClass
            Dim oBook As Excel.WorkbookClass
            Dim oBooks As Excel.Workbooks

            'inicia excel y el libro de trabajo
            oExcel = CreateObject("Excel.Application")
            oExcel.Visible = True
            oBooks = oExcel.Workbooks
            oBook = oBooks.Open(ArchivoMacro)
            'Corre la macro
            oExcel.Run(NombreMacro, "4", "5", "6")
            'gaurda el archivo con nombre y fecha
            Dim fecha As Date
            fecha = Date.Now
            Dim dia As String
            Dim nombre As String
            dia = fecha.Day & "-" & fecha.Month & "-" & fecha.Year
            nombre = carpetaObjetivo & "\" & Titulo & dia & ".xlsm"
            oBook.SaveAs(nombre)
            'cierra excel
            'oBook.Close(False)
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook)
            oBook = Nothing
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks)
            oBooks = Nothing
            oExcel.Quit()
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel)
            oExcel = Nothing
            Return 0
        Catch ex As Exception
            Return 1
        End Try
    End Function
End Class
