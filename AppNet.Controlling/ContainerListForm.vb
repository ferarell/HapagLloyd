Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports Microsoft.Office.Interop
Imports System.Runtime.InteropServices

Public Class ContainerListForm
    Dim dtSource As New DataTable
    Dim dsApp As New dsMain

    Private Sub SDCForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub beSourceFile_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beSourceFile.Properties.ButtonClick
        Dim FileNames() As String
        OpenFileDialog1.Filter = "Source Files (*.xls*;*.txt;*.pdf)|*.xls*;*.txt;*.pdf"
        OpenFileDialog1.FileName = ""
        'OpenFileDialog1.InitialDirectory = My.Settings.SDCDataSourcePath
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            FileNames = OpenFileDialog1.FileNames
            beSourceFile.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub bbiProcesss_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesss.ItemClick
        dtSource.Rows.Clear()
        If vpInputs.Validate Then
            dtSource = LoadTXT(beSourceFile.Text)
        End If
        gcMasterData.DataSource = dtSource
    End Sub

    Friend Function LoadTXT(FileName As String) As DataTable
        Dim iPosition As Integer = 0
        Dim sEqpType As String = ""
        dtSource = dsApp.EQDE0201
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            Using sr As New StreamReader(FileName)
                Dim lines As List(Of String) = New List(Of String)
                Dim bExit As Boolean = False
                Do While Not sr.EndOfStream
                    lines.Add(sr.ReadLine())
                Loop
                Dim bSkip As Boolean = True
                For i As Integer = 0 To lines.Count - 1
                    If Mid(lines(i), 3, 4) = "----" Then
                        sEqpType = Mid(lines(i + 1), 3, 4)
                        'bSkip = False
                        'Continue For
                    End If
                    If Mid(lines(i), 8, 4) <> "----" And Mid(lines(i), 14, 7).Trim.Length = 7 And IsNumeric(Mid(lines(i), 14, 7)) Then
                        dtSource.Rows.Add()
                        iPosition = dtSource.Rows.Count - 1
                        dtSource.Rows(iPosition).Item(0) = sEqpType
                        dtSource.Rows(iPosition).Item(1) = Replace(Mid(lines(i), 8, 13), " ", "")
                        dtSource.Rows(iPosition).Item(2) = Mid(lines(i), 22, 4)
                        dtSource.Rows(iPosition).Item(3) = Mid(lines(i), 27, 3)
                        dtSource.Rows(iPosition).Item(4) = Mid(lines(i), 31, 1)
                        dtSource.Rows(iPosition).Item(5) = CDate(Replace(Mid(lines(i), 33, 10), "-", "/") & Space(1) & Replace(Mid(lines(i), 44, 5), ".", ":"))
                        dtSource.Rows(iPosition).Item(6) = Mid(lines(i), 50, 5)
                        dtSource.Rows(iPosition).Item(7) = Mid(lines(i), 56, 5)
                        dtSource.Rows(iPosition).Item(8) = Mid(lines(i), 62, 6)
                        dtSource.Rows(iPosition).Item(9) = Mid(lines(i), 69, 14)
                        dtSource.Rows(iPosition).Item(10) = Mid(lines(i), 84, 10)
                        dtSource.Rows(iPosition).Item(11) = Mid(lines(i), 96, 6)
                        'DBTableUpdate(dtSource.Rows(iPosition))
                        'If Mid(lines(i), 3, 4) = "PROG" Then
                        '    bSkip = True
                        'End If
                    End If
                Next
            End Using
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The process has been completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return dtSource
    End Function

    Private Sub LoadInputValidations()
        Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

        containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
        containsValidationRule.ErrorText = "Asigne un valor."
        containsValidationRule.ErrorType = ErrorType.Critical

        Dim customValidationRule As New CustomValidationRule()
        customValidationRule.ErrorText = "Valor obligatorio."
        customValidationRule.ErrorType = ErrorType.Critical

        vpInputs.SetValidationRule(Me.beSourceFile, customValidationRule)
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(gcMasterData)
    End Sub

End Class