Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.Collections
Imports Microsoft.Office.Interop
Imports System.Runtime.InteropServices

Public Class ColdTreatmentWcfForm
    Dim beDataSource As New DevExpress.XtraEditors.ButtonEdit
    Dim dsVendorData As New dsMain
    Dim dtBreak, dtResult, dtEvents, dtProtocolSP As New DataTable
    Dim ContainerNumber, Booking, Customer, Vessel, Voyage, Sensor, TSP, POD, Service As String
    Dim MailSubject, MailBody As String
    Dim Deadline, FailDate, EtaDate As Date
    Dim CTInitialDate, InitialDate, FinalDate, DateFrom, DateTo As DateTime
    Dim sCTDaysInterval, sDaysInterval, CTInitialTime, InitialTime, FinalTime As String
    Dim DataMaxTemp, MaxTemp, MinTemp As Decimal
    Dim bProcessError As Boolean
    Dim iDays As Integer = 0
    Dim iBrokes As Integer = 0
    Dim iDaysInterval As Integer = 0
    Dim iCTDaysInterval As Integer = 0
    Dim ContainerNumberTmp As String = ""
    Dim oAppService As New AppService.HapagLloydServiceClient
    Dim oSharePointTransactions As New SharePointListTransactions

    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        LoadProtocolFromSP()
    End Sub

    Private Sub ColdTreatmentForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'beDataSource.Text = My.Settings.DataSourcePath & "\" & My.Settings.DBFileName
        OpenFileDialog1.InitialDirectory = My.Settings.VendorSourcePath
        'OpenFileDialog2.InitialDirectory = My.Settings.DataSourcePath       
        dtEvents.Columns.Add("Events")
        bbiMessage.Enabled = False
        bbiSave.Enabled = False
    End Sub

    Private Sub beVendorData_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beVendorData.Properties.ButtonClick
        teContainer.Text = ""
        tePOL.Text = ""
        teTSP.Text = ""
        tePOD.Text = ""
        teClient.Text = ""
        lueVoyage.Properties.DataSource = Nothing
        OpenFileDialog1.Filter = "Source Files (*.xls*;*.txt)|*.xls*;*.txt"
        OpenFileDialog1.FileName = ""
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            beVendorData.Text = OpenFileDialog1.FileName
            'If beVendorData.Text <> "" Then
            '    OpenFileDialog1.FileName = beVendorData.Text
            'End If
            ContainerNumberTmp = Replace(IO.Path.GetFileName(OpenFileDialog1.FileName), IO.Path.GetExtension(OpenFileDialog1.FileName), "")
            ContainerNumberTmp = Strings.Left(Replace(ContainerNumberTmp.ToUpper, "TEMP", "").Trim, 11)
        End If
    End Sub

    Private Sub bbiProcesss_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcesss.ItemClick
        LoadValidations()
        If Not vpInputs.Validate Then
            Return
        End If
        If Not File.Exists(beVendorData.Text) Then
            DevExpress.XtraEditors.XtraMessageBox.Show("The file doesn't exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        bProcessError = False
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            bsiRemarks1.Caption = ""
            bsiRemarks2.Caption = ""
            ContainerNumber = ""
            teContainer.EditValue = Nothing
            tePOL.EditValue = Nothing
            teTSP.EditValue = Nothing
            tePOD.EditValue = Nothing
            teClient.EditValue = Nothing
            lueVoyage.Properties.DataSource = Nothing
            gcVendorReadings.DataSource = Nothing
            ccTrends.DataSource = Nothing
            gcProtocol.DataSource = Nothing
            gcEvents.DataSource = Nothing
            CTInitialDate = Nothing
            CTInitialTime = Nothing
            InitialDate = Nothing
            InitialTime = Nothing
            FinalDate = Nothing
            FinalTime = Nothing
            dsVendorData.Tables(0).Rows.Clear()
            dtResult.Rows.Clear()
            dtEvents.Rows.Clear()
            dtResult = dsVendorData.Tables(0)
            If beVendorData.Text.ToUpper.Contains(".TXT") Then
                Using sr As New StreamReader(beVendorData.Text)
                    Dim lines As New List(Of String) ' = New List(Of String)
                    Dim bExit As Boolean = False
                    Do While Not sr.EndOfStream
                        lines.Add(sr.ReadLine())
                    Loop
                    If lines(0) = "" Then
                        TextFileV1(lines)
                    ElseIf Microsoft.VisualBasic.Left(lines(0), 9) = "CONTAINER" Then
                        TextFileV2(lines)
                    ElseIf IsFileV3(lines) Then
                        TextFileV3(lines)
                    Else
                        Try
                            TextFileV4(lines)
                        Catch ex As Exception
                            SplashScreenManager.CloseForm(False)
                            DevExpress.XtraEditors.XtraMessageBox.Show("Unknow file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        End Try
                    End If
                End Using
            Else
                ProcessesVendorExcelData()
            End If
            If ContainerNumber = "" Then
                SplashScreenManager.CloseForm(False)
                DevExpress.XtraEditors.XtraMessageBox.Show("Unknow file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            teContainer.Text = ContainerNumber
            LoadVoyageByContainerNumber()
            DataMaxTemp = GetDataMaxTemp(dtResult)
            LoadProtocol()
            DataValidation()
            GridView3.MoveLast()
            gcVendorReadings.DataSource = dtResult
            ccTrends.DataSource = gcVendorReadings.DataSource
            ccTrends.RefreshData()
            'InsertGapLines()
            'If Not LoadProtocol() Then
            If GridView2.RowCount = 0 Then
                SplashScreenManager.CloseForm(False)
                bbiMessage.Enabled = False
                DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "This container doesn't have cold treatment protocol and will not updated in data master.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                'If rgMode.SelectedIndex > 0 And ExecuteAccessQuery("select * from ColdTreatment where BOOKING='" & Booking & "' and CONTAINER='" & ContainerNumber & "' and REMARKS='CT PASSED'").Tables(0).Rows.Count > 0 Then
                '    DevExpress.XtraEditors.XtraMessageBox.Show("Cold treatment finalized, will not update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                '    bbiSave.Enabled = False
                '    Return
                'End If
                'VoyageValidate()
                bbiMessage.Enabled = True
            End If
            'bbiSave.Enabled = False
            'If rgMode.SelectedIndex = 1 Then
            '    bbiSave.Enabled = True
            'End If
        Catch ex As System.Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            SplashScreenManager.CloseForm(False)
        End Try

    End Sub

    Friend Function IsFileV3(lines As List(Of String)) As Boolean
        Dim bResult As Boolean = False
        For l = 0 To 5
            If lines(l).Contains("Airflow in CFM") Then
                bResult = True
            End If
        Next
        Return bResult
    End Function

    Friend Function GetDataMaxTemp(dtSource As DataTable) As Double
        Dim iValue As Double = 0
        For Each row As DataRow In dtSource.Rows
            If row(2) > iValue Then
                iValue = row(2)
            End If
            If row(3) > iValue Then
                iValue = row(3)
            End If
            If row(4) > iValue Then
                iValue = row(4)
            End If
        Next
        Return iValue
    End Function

    Private Sub TextFileV1(lines As List(Of String))
        Dim iLine As Integer = 0
        Dim C1 As String = ""
        Dim C2 As String = ""
        Dim C3 As String = ""
        Dim C4 As String = ""
        Dim C5 As String = ""
        Dim iTime, iUSDA1, iUSDA2, iUSDA3 As Integer
        iTime = 0
        iUSDA1 = 0
        iUSDA2 = 0
        iUSDA3 = 0
        Dim bSkip As Boolean
        Try
            For i As Integer = 0 To lines.Count - 1
                iLine = i
                bSkip = False
                If lines(i).Contains("Setpoint:") Then
                    ContainerNumber = lines(i).Substring(InStrRev(lines(i), ",") - 12, 11)
                End If
                If lines(i).Trim.Length = 0 Then
                    Continue For
                End If
                If TextContain(Replace(Mid(lines(i), 1, 4), ".", " "), "MonthOfYear") Then
                    C1 = Format(CDate(GetReadingDate(lines(i).Substring(0, 12))), "yyyy-MM-dd")
                End If
                If lines(i).ToUpper.Contains("TIME") And lines(i).Contains("USDA") Then
                    iTime = IIf(iTime = 0, InStr(lines(i), "TIME") + 1, iTime)
                    If lines(i).ToUpper.Contains("USDA1") Then
                        iUSDA1 = IIf(iUSDA1 = 0, InStr(lines(i), "USDA1") - 3, iUSDA1)
                        iUSDA2 = IIf(iUSDA2 = 0, InStr(lines(i), "USDA2") - 3, iUSDA2)
                        iUSDA3 = IIf(iUSDA3 = 0, InStr(lines(i), "USDA3") - 3, iUSDA3)
                    ElseIf lines(i).ToUpper.Contains("USDA 1") Then
                        iUSDA1 = IIf(iUSDA1 = 0, InStr(lines(i), "USDA 1") - 1, iUSDA1)
                        iUSDA2 = IIf(iUSDA2 = 0, InStr(lines(i), "USDA 2") - 1, iUSDA2)
                        iUSDA3 = IIf(iUSDA3 = 0, InStr(lines(i), "USDA 3") - 1, iUSDA3)
                    End If
                End If
                If iTime = 0 Or Not IsDate(C1) Then
                    Continue For
                End If
                If Mid(lines(i), iTime, 5).Contains(":") Then
                    C2 = lines(i).Substring(0, 6)
                    If lines(i).Length < iUSDA3 + 7 Then
                        Continue For
                    End If
                    If TextContain(lines(i).Substring(iUSDA1, 7).Trim, "OnlyNumbers") And TextContain(lines(i).Substring(iUSDA2, 7).Trim, "OnlyNumbers") And TextContain(lines(i).Substring(iUSDA3, 7).Trim, "OnlyNumbers") Then
                        C3 = Replace(lines(i).Substring(iUSDA1, 7), ",", ".")
                        C4 = Replace(lines(i).Substring(iUSDA2, 7), ",", ".")
                        C5 = Replace(lines(i).Substring(iUSDA3, 7), ",", ".")
                    Else
                        bSkip = True
                    End If
                    If Not bSkip Then
                        Dim iPosition As Integer = 0
                        dtResult.Rows.Add()
                        iPosition = dtResult.Rows.Count - 1
                        dtResult.Rows(iPosition).Item("C1") = CDate(C1)
                        dtResult.Rows(iPosition).Item("C2") = C2
                        dtResult.Rows(iPosition).Item("C3") = C3
                        dtResult.Rows(iPosition).Item("C4") = C4
                        dtResult.Rows(iPosition).Item("C5") = C5
                    End If
                End If
            Next
            'TimeAssign()
        Catch ex As System.Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show("Línea: " & iLine.ToString & " " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    'Friend Function GetDataResult(odate As Object, otime As Object, dtResultTmp As DataTable) As DataTable
    '    Dim dtQuery As New DataTable
    '    Dim sTime As String = ""
    '    dtQuery = dtResultTmp.Clone
    '    If otime.ToString.Trim.Contains("24:00") Then
    '        otime = " 23:59"
    '    End If
    '    For Each row As DataRow In dtResultTmp.Rows
    '        sTime = " " & row("C2")
    '        If row("C2").ToString.Trim.Contains("24:00") Then
    '            sTime = " 23:59"
    '        End If
    '        If CDate(Format(row("C1"), "dd/MM/yyyy") & sTime) >= CDate(Format(odate, "dd/MM/yyyy") & " " & otime) Then
    '            dtQuery.ImportRow(row)
    '        End If
    '    Next
    '    Return dtQuery
    'End Function

    Private Sub TextFileV2(lines As List(Of String))
        Dim C1, C2, C3, C4, C5 As String
        Dim bSkip As Boolean
        Dim iTime, iUSDA1, iUSDA2, iUSDA3 As Integer
        iTime = 0
        iUSDA1 = 0
        iUSDA2 = 0
        iUSDA3 = 0
        For i As Integer = 1 To lines.Count - 1
            bSkip = False
            If i = 1 Then
                ContainerNumber = Microsoft.VisualBasic.Left(lines(i), 11)
            End If
            If lines(0).ToUpper.Contains("TIME") And lines(0).Contains("USDA") Then
                iTime = IIf(iTime = 0, InStr(lines(0), "TIME") - 1, iTime)
                iUSDA1 = IIf(iUSDA1 = 0, InStr(lines(0), "USDA1") - 3, iUSDA1)
                iUSDA2 = IIf(iUSDA2 = 0, InStr(lines(0), "USDA2") - 3, iUSDA2)
                iUSDA3 = IIf(iUSDA3 = 0, InStr(lines(0), "USDA3") - 3, iUSDA3)
            End If
            If IsDate(lines(i).Substring(12, 10)) Then
                C1 = Format(CDate(lines(i).Substring(12, 10)), "yyyy-MM-dd")
            ElseIf IsDate(Replace(lines(i).Substring(12, 10), "/", "-")).ToString.Substring(0, 10) Then
                C1 = CDate(Replace(lines(i).Substring(12, 10), "/", "-")).ToString.Substring(0, 10)
            End If
            C2 = lines(i).Substring(iTime, 5)
            If lines(i).Substring(iUSDA1, 7).Trim = "" Or lines(i).Substring(iUSDA2, 7) = "" Or lines(i).Substring(iUSDA3, 7) = "" Then
                Continue For
            End If
            If TextContain(lines(i).Substring(iUSDA1, 7).Trim, "OnlyNumbers") And TextContain(lines(i).Substring(iUSDA2, 7).Trim, "OnlyNumbers") And TextContain(lines(i).Substring(iUSDA3, 7).Trim, "OnlyNumbers") Then
                C3 = Replace(lines(i).Substring(iUSDA1, 7), ",", ".")
                C4 = Replace(lines(i).Substring(iUSDA2, 7), ",", ".")
                C5 = Replace(lines(i).Substring(iUSDA3, 7), ",", ".")
            Else
                bSkip = True
            End If
            If Not bSkip Then
                Dim iPosition As Integer = 0
                dtResult.Rows.Add()
                iPosition = dtResult.Rows.Count - 1
                dtResult.Rows(iPosition).Item("C1") = CDate(C1)
                dtResult.Rows(iPosition).Item("C2") = C2
                dtResult.Rows(iPosition).Item("C3") = C3
                dtResult.Rows(iPosition).Item("C4") = C4
                dtResult.Rows(iPosition).Item("C5") = C5
            End If
        Next
        'TimeAssign()
        'dtResult = GetDataResult(aDateTime(0), aDateTime(1), dtResult)
    End Sub

    Private Sub TextFileV3(lines As List(Of String))
        Dim C1, C2, C3, C4, C5 As String
        Dim bSkip As Boolean
        Dim iTime, iUSDA1, iUSDA2, iUSDA3 As Integer
        iTime = 1
        iUSDA1 = 1
        iUSDA2 = 1
        iUSDA3 = 1
        For i As Integer = 1 To lines.Count - 1
            'If lines(i).Substring(39, 7).Trim = "" Or lines(i).Substring(46, 7).Trim = "" Or lines(i).Substring(53, 7).Trim = "" Then
            '    Continue For
            'End If
            If i = 1 Then
                ContainerNumber = ContainerNumberTmp
            End If
            bSkip = False
            If lines(i).ToUpper.Contains("TIME") And lines(i).Contains("USDA") Then
                iTime = IIf(iTime = 1, InStr(lines(i), "TIME") - 1, iTime)
                If lines(i).ToUpper.Contains("USDA1") Then
                    iUSDA1 = IIf(iUSDA1 = 1, InStr(lines(i), "USDA1") - 3, iUSDA1)
                    iUSDA2 = IIf(iUSDA2 = 1, InStr(lines(i), "USDA2") - 3, iUSDA2)
                    iUSDA3 = IIf(iUSDA3 = 1, InStr(lines(i), "USDA3") - 3, iUSDA3)
                ElseIf lines(i).ToUpper.Contains("USDA 1") Then
                    iUSDA1 = IIf(iUSDA1 = 1, InStr(lines(i), "USDA 1") - 1, iUSDA1)
                    iUSDA2 = IIf(iUSDA2 = 1, InStr(lines(i), "USDA 2") - 1, iUSDA2)
                    iUSDA3 = IIf(iUSDA3 = 1, InStr(lines(i), "USDA 3") - 1, iUSDA3)
                End If
                Continue For
            End If
            If Not IsDate(Mid(lines(i), 1, 10)) Then
                Continue For
            End If
            If lines(i).Substring(iUSDA1, 7).Trim = "" Or lines(i).Substring(iUSDA2, 7).Trim = "" Or lines(i).Substring(iUSDA3, 7).Trim = "" Then
                Continue For
            End If
            If TextContain(lines(i).Substring(iUSDA1, 7).Trim, "OnlyNumbers") And TextContain(lines(i).Substring(iUSDA2, 7).Trim, "OnlyNumbers") And TextContain(lines(i).Substring(iUSDA3, 7).Trim, "OnlyNumbers") Then
                C1 = Format(CDate(Mid(lines(i), 1, 10)), "yyyy-MM-dd")
                C2 = lines(i).Substring(iTime, 5)
                C3 = Replace(lines(i).Substring(iUSDA1, 7), ",", ".")
                C4 = Replace(lines(i).Substring(iUSDA2, 7), ",", ".")
                C5 = Replace(lines(i).Substring(iUSDA3, 7), ",", ".")
            Else
                bSkip = True
            End If
            If Not bSkip Then
                Dim iPosition As Integer = 0
                dtResult.Rows.Add()
                iPosition = dtResult.Rows.Count - 1
                dtResult.Rows(iPosition).Item("C1") = CDate(C1)
                dtResult.Rows(iPosition).Item("C2") = C2
                dtResult.Rows(iPosition).Item("C3") = C3
                dtResult.Rows(iPosition).Item("C4") = C4
                dtResult.Rows(iPosition).Item("C5") = C5
            End If
        Next
        'TimeAssign()
        'dtResult = GetDataResult(aDateTime(0), aDateTime(1), dtResult)
    End Sub

    Private Sub TextFileV4(lines As List(Of String))
        Dim iLine As Integer = 0
        Dim C1, C2, C3, C4, C5 As String
        Dim bSkip As Boolean
        Dim iTime, iUSDA1, iUSDA2, iUSDA3 As Integer
        iTime = 1
        iUSDA1 = 1
        iUSDA2 = 1
        iUSDA3 = 1
        Try
            For i As Integer = 0 To lines.Count - 1
                iLine = i
                bSkip = False
                lines(i) = lines(i).Trim + "  "
                If lines(i).ToUpper.Contains("CONTAINER") Then
                    If ContainerNumber = "" Then
                        ContainerNumber = GetContainerNumber(lines(i)) 'Microsoft.VisualBasic.Left(lines(i), 11)
                    End If
                End If
                If lines(i) = "" Or ContainerNumber = "" Then
                    Continue For
                End If
                If lines(i).ToUpper.Contains("TIME") And lines(i).Contains("USDA") Then
                    iTime = IIf(iTime = 1, InStr(lines(i), "TIME") - 1, iTime)
                    If lines(i).ToUpper.Contains("USDA1") Then
                        iUSDA1 = IIf(iUSDA1 = 1, InStr(lines(i), "USDA1") - 3, iUSDA1)
                        iUSDA2 = IIf(iUSDA2 = 1, InStr(lines(i), "USDA2") - 3, iUSDA2)
                        iUSDA3 = IIf(iUSDA3 = 1, InStr(lines(i), "USDA3") - 3, iUSDA3)
                    ElseIf lines(i).ToUpper.Contains("USDA 1") Then
                        iUSDA1 = IIf(iUSDA1 = 1, InStr(lines(i), "USDA 1") - 1, iUSDA1)
                        iUSDA2 = IIf(iUSDA2 = 1, InStr(lines(i), "USDA 2") - 1, iUSDA2)
                        iUSDA3 = IIf(iUSDA3 = 1, InStr(lines(i), "USDA 3") - 1, iUSDA3)
                    End If
                End If
                If Mid(lines(i), iUSDA1, 7).Trim = "" Or Mid(lines(i), iUSDA2, 7) = "" Or Mid(lines(i), iUSDA3, 7) = "" Then
                    Continue For
                End If
                If Not IsDate(Mid(lines(i), 1, 10)) Then
                    Continue For
                End If
                If TextContain(lines(i).Substring(iUSDA1, 7).Trim, "OnlyNumbers") And TextContain(lines(i).Substring(iUSDA2, 7).Trim, "OnlyNumbers") And TextContain(lines(i).Substring(iUSDA3, 7).Trim, "OnlyNumbers") Then
                    C1 = Format(CDate(Mid(lines(i), 1, 10)), "yyyy-MM-dd")
                    C2 = lines(i).Substring(iTime, 5)
                    C3 = Replace(lines(i).Substring(iUSDA1, 7), ",", ".")
                    C4 = Replace(lines(i).Substring(iUSDA2, 7), ",", ".")
                    C5 = Replace(lines(i).Substring(iUSDA3, 7), ",", ".")
                Else
                    bSkip = True
                End If
                If Not bSkip Then
                    Dim iPosition As Integer = 0
                    dtResult.Rows.Add()
                    iPosition = dtResult.Rows.Count - 1
                    dtResult.Rows(iPosition).Item("C1") = CDate(C1)
                    dtResult.Rows(iPosition).Item("C2") = C2
                    dtResult.Rows(iPosition).Item("C3") = C3
                    dtResult.Rows(iPosition).Item("C4") = C4
                    dtResult.Rows(iPosition).Item("C5") = C5
                End If
            Next
            'TimeAssign()
            'dtResult = GetDataResult(aDateTime(0), aDateTime(1), dtResult)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show("Línea: " & iLine.ToString & " " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub TimeAssign()
        InitialDate = dtResult.Rows(0)(0)
        InitialTime = Replace(dtResult.Rows(0)(1).trim, "24:00", "23:59")
        CTInitialDate = dtResult.Rows(0)(0) & Space(1) & Replace(dtResult.Rows(0)(1).trim, "24:00", "23:59")
        CTInitialTime = Replace(dtResult.Rows(0)(1).trim, "24:00", "23:59")
        dtBreak = GetFirsTime(dtResult)
        If dtBreak.Rows.Count > 0 Then
            If Not IsDBNull(dtBreak.Rows(dtBreak.Rows.Count - 1)(2)) Then
                CTInitialDate = dtBreak.Rows(dtBreak.Rows.Count - 1)(2) & Space(1) & Replace(dtBreak.Rows(dtBreak.Rows.Count - 1)(3).trim, "24:00", "23:59")
                CTInitialTime = Replace(dtBreak.Rows(dtBreak.Rows.Count - 1)(3).trim, "24:00", "23:59")
            End If
        End If
    End Sub

    Friend Function GetContainerNumber(ctn As String) As String
        Dim sResult As String = ""
        ctn = ctn.ToUpper.Trim
        Replace(ctn, "CONTAINER", "")
        For i = 1 To ctn.Length - 1
            sResult = sResult & Mid(ctn, i, 1)
            If Mid(ctn, i, 1) = " " Then
                sResult = ""
            End If
            If sResult.Length = 11 Then
                Exit For
            End If
        Next
        Return sResult
    End Function

    Private Sub UpdateVendorReadings()
        Dim dtQuery As New DataTable
        SplashScreenManager.Default.SetWaitFormDescription("Update Data Readings of Cold Treatment")
        dtQuery = oAppService.ExecuteSQL("select * from tck.ColdTreatmentReadings where [BOOKING]='" & Booking & "' and [CONTAINER] = '" & ContainerNumber & "'").Tables(0)
        If dtQuery.Rows.Count > 0 Then
            Dim aSource As New ArrayList
            aSource.AddRange({ContainerNumber, Booking})
            SplashScreenManager.Default.SetWaitFormDescription("Delete Data Readings of Cold Treatment")
            'oAppService.ExecuteSQLNonQuery("delete from ColdTreatmentReadings where [BOOKING]='" & Booking & "' and [CONTAINER] = '" & ContainerNumber & "'")
            oAppService.DeleteColdTreatmentReadings(aSource.ToArray)
        End If
        dtQuery.Rows.Clear()
        For r = 0 To dtResult.Rows.Count - 1
            Dim ctrow As DataRow = dtResult.Rows(r)
            SplashScreenManager.Default.SetWaitFormDescription("Insert Data Readings (" & dtResult.Rows.IndexOf(ctrow).ToString & " of " & (dtResult.Rows.Count - 1).ToString & ")")
            dtQuery.Rows.Add(Booking, ContainerNumber, ctrow(0), ctrow(1), ctrow(2), ctrow(3), ctrow(4))
            'If ctrow(0) & Space(1) & Replace(ctrow(1), "24:00", "23:59") >= CTInitialDate Then
            'InsertIntoAccess("ColdTreatmentReadings", dtQuery.Rows(dtQuery.Rows.Count - 1))
            'Dim dtSource As New DataTable
            'dtSource = ctrow.Table.Clone
            'dtSource.ImportRow(ctrow)
            oAppService.InsertColdTreatmentReadings(dtQuery.Rows(r).ItemArray.ToArray())
            'End If
        Next
    End Sub

    Private Sub UpdateVendorEvents()
        Dim dtQuery As New DataTable
        SplashScreenManager.Default.SetWaitFormDescription("Update Data Events of Cold Treatment")
        dtQuery = oAppService.ExecuteSQL("select * from tck.ColdTreatmentEvents where [BOOKING]='" & Booking & "' and [CONTAINER] = '" & ContainerNumber & "'").Tables(0)
        If dtQuery.Rows.Count > 0 Then
            Dim aSource As New ArrayList
            aSource.AddRange({ContainerNumber, Booking})
            SplashScreenManager.Default.SetWaitFormDescription("Delete Data Events of Cold Treatment")
            'ExecuteAccessNonQuery("delete from ColdTreatmentEvents where [BOOKING]='" & Booking & "' and [CONTAINER] = '" & ContainerNumber & "'")
            oAppService.DeleteColdTreatmentEvents(aSource.ToArray)
        End If
        dtQuery.Rows.Clear()
        For r = 0 To dtEvents.Rows.Count - 1
            Dim ctrow As DataRow = dtEvents.Rows(r)
            SplashScreenManager.Default.SetWaitFormDescription("Insert Data Events (" & dtEvents.Rows.IndexOf(ctrow).ToString & " of " & (dtEvents.Rows.Count - 1).ToString & ")")
            dtQuery.Rows.Add(Booking, ContainerNumber, ctrow(0))
            'InsertIntoAccess("ColdTreatmentEvents", dtQuery.Rows(dtQuery.Rows.Count - 1))
            oAppService.InsertColdTreatmentEvents(dtQuery.Rows(r).ItemArray.ToArray())
        Next
    End Sub

    'Private Sub InsertExcelRow(FileName As String)
    '    Try
    '        Dim oXls As New Excel.Application 'Crea el objeto excel 
    '        oXls.Workbooks.Open(FileName, , False) 'El true es para abrir el archivo en modo Solo lectura (False si lo quieres de otro modo)
    '        oXls.Range("a1:a1").EntireRow.Insert()
    '        oXls.Cells(1, 1) = Today
    '        oXls.Workbooks.Close()
    '    Catch ex As System.Exception
    '        MessageBox.Show(ex.Message)
    '    End Try
    'End Sub

    Private Sub ProcessesVendorExcelData()
        Dim dtVendorData, dtTemp As New DataTable
        dtVendorData = LoadExcelWH(beVendorData.Text, "{0}").Tables(0)
        Dim iPosition As Integer = 0
        Try
            For Each row As DataRow In dtVendorData.Rows
                If dtVendorData.Rows.IndexOf(row) = 0 Then
                    ContainerNumber = Microsoft.VisualBasic.Right(row(1).Trim, 11)
                    teContainer.Text = ContainerNumber
                End If
                If dtVendorData.Rows.IndexOf(row) > 11 Then
                    If row(1).ToString.ToUpper = "DATA" Then
                        dtResult.Rows.Add()
                        iPosition = dtResult.Rows.Count - 1
                        dtResult.Rows(iPosition).Item("C1") = Format(row(0), "yyyy-MM-dd")
                        dtResult.Rows(iPosition).Item("C2") = Format(row(0), "hh:mm tt")
                        dtResult.Rows(iPosition).Item("C3") = row(11)
                        dtResult.Rows(iPosition).Item("C4") = row(12)
                        dtResult.Rows(iPosition).Item("C5") = row(13)
                    End If
                End If
            Next
        Catch ex As System.Exception
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        If gcVendorReadings.FocusedView.IsFocusedView Then
            ExportarExcel(gcVendorReadings)
        ElseIf ccTrends.Focused Then
            ExportGraphToExcel(ccTrends)
        ElseIf gcEvents.FocusedView.IsFocusedView Then
            ExportarExcel(gcEvents)
        End If

    End Sub

    Private Sub GridView1_RowCellStyle(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs) Handles GridView1.RowCellStyle
        Dim View As GridView = sender
        If (e.RowHandle >= 0) Then
            Dim C1 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("C1"))
            Dim C2 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("C2"))

            If C1 = "" Or C2 = "" Then 'Or C3 = "" Or C4 = "" Or C5 = "" Then
                e.Appearance.BackColor = Color.Salmon
                e.Appearance.BackColor2 = Color.SeaShell
            End If
            If e.Column.FieldName = "C3" Then 'USDA1
                Dim C3 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("C3"))
                If C3 = "" Or C3 > MaxTemp Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If
            End If
            If e.Column.FieldName = "C4" Then 'USDA2
                Dim C4 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("C4"))
                If C4 = "" Or C4 > MaxTemp Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If
            End If
            If e.Column.FieldName = "C5" Then 'USDA3
                Dim C5 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("C5"))
                If C5 = "" Or C5 > MaxTemp Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If
            End If
        End If
    End Sub

    Private Sub LoadProtocolFromSP()
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Get Protocols from SharePoint List")
        Try
            oSharePointTransactions.SharePointUrl = My.Settings.SharePoint_Url
            oSharePointTransactions.SharePointList = "ColdTreatmentProtocol"
            oSharePointTransactions.FieldsList.Clear()
            oSharePointTransactions.FieldsList.Add({"Service"})
            oSharePointTransactions.FieldsList.Add({"Port1"})
            oSharePointTransactions.FieldsList.Add({"Port2"})
            oSharePointTransactions.FieldsList.Add({"Port3"})
            oSharePointTransactions.FieldsList.Add({"Port4"})
            oSharePointTransactions.FieldsList.Add({"Temp"})
            oSharePointTransactions.FieldsList.Add({"Days"})
            oSharePointTransactions.FieldsList.Add({"TTime1"})
            oSharePointTransactions.FieldsList.Add({"STime1"})
            oSharePointTransactions.FieldsList.Add({"TTime2"})
            oSharePointTransactions.FieldsList.Add({"STime2"})
            oSharePointTransactions.FieldsList.Add({"TTime3"})
            oSharePointTransactions.FieldsList.Add({"Total"})
            dtProtocolSP = oSharePointTransactions.GetItems()
            SplashScreenManager.CloseForm(False)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadVoyageByContainerNumber()
        Dim dtVoyage As New DataTable
        'ContainerNumber = Microsoft.VisualBasic.Left(ContainerNumber, 4) & Space(1) & Microsoft.VisualBasic.Right(ContainerNumber, 7)
        dtVoyage = oAppService.ExecuteSQL("select * from tck.ColdTreatment where [CONTAINER]='" & ContainerNumber & "'").Tables(0)
        If dtVoyage.Rows.Count > 0 Then
            lueVoyage.Properties.DataSource = dtVoyage
            lueVoyage.Properties.DisplayMember = "VESSEL"
            lueVoyage.Properties.ValueMember = "VESSEL"
            lueVoyage.ItemIndex = 0
            lueVoyage.EditValue = dtVoyage.Rows(0).Item("VESSEL")
            Service = lueVoyage.GetColumnValue("SERVICE")
        End If
        LoadPorts()
    End Sub

    Private Sub LoadPorts()
        If lueVoyage.ItemIndex >= 0 Then
            tePOL.EditValue = lueVoyage.GetColumnValue("POL")
            teTSP.EditValue = lueVoyage.GetColumnValue("TSP")
            tePOD.EditValue = lueVoyage.GetColumnValue("FDP")
            teClient.EditValue = lueVoyage.GetColumnValue("EXPORT_PARTY")
        End If
    End Sub

    Friend Function LoadProtocol() As Boolean
        Dim bResult As Boolean = True
        Dim dtProtocol As New DataTable
        bsiRemarks1.Caption = "The maximum temperature in this file is: " & DataMaxTemp.ToString
        'If dtProtocolSP.Select("Service = '" & Service & "' AND Port2 = '" & teTSP.Text & "' AND " & " Port4 = '" & tePOD.Text & "'").Length > 0 Then

        'End If
        If dtProtocolSP.Select("Service = '" & Service & "' AND Port2 = '" & teTSP.Text & "' AND " & " Port4 = '" & tePOD.Text & "'").Length > 0 Then
            dtProtocol = dtProtocolSP.Select("Service = '" & Service & "' AND Port2 = '" & teTSP.Text & "' AND " & " Port4 = '" & tePOD.Text & "'").CopyToDataTable
        End If
        gcProtocol.MainView = GridView2
        gcProtocol.DataSource = dtProtocol
        If dtProtocol.Rows.Count = 0 Then
            'MaxTemp = My.Settings.MaxTemp
            bResult = False
        Else
            Booking = lueVoyage.GetColumnValue("BOOKING")
            Voyage = GetVoyageFromVessel(lueVoyage.Text)
            Vessel = lueVoyage.Text.Replace(Voyage, "")
            Customer = lueVoyage.GetColumnValue("EXPORT_PARTY")
            POD = lueVoyage.GetColumnValue("POD")
            TSP = lueVoyage.GetColumnValue("TSP")
        End If
        Return bResult
    End Function

    Friend Function GetProtocolFromSP(SRV As String, TSP As String, POD As String) As DataTable
        Dim dtQuery As New DataTable

        Return dtQuery
    End Function

    Friend Function GetVoyageFromVessel(vessel As String) As String
        Dim sResult As String = ""
        Dim iPos As Integer = 0
        iPos = InStrRev(vessel, " ")
        sResult = Mid(vessel, iPos, vessel.Length - iPos + 1).Trim
        Return sResult
    End Function

    Private Sub DataValidation()
        bProcessError = False
        iBrokes = 0
        dtEvents.Rows.Clear()
        Dim date1, date2 As Date
        Dim iPos, iLastPos, iDateDiff, iMaxReadDays As Integer
        Dim sTime As String = ""
        Dim MaxTemp1 As Double = 1.11
        Dim MaxTemp2 As Double = My.Settings.MaxTemp
        MaxTemp = MaxTemp2
        iMaxReadDays = My.Settings.MaxReadingDays
        DateFrom = Nothing
        iLastPos = dtResult.Rows.Count - 1
        Dim dtResultTmp As New DataTable
        dtResultTmp = dtResult.Clone
        For Each row2 As DataRow In dtResult.Rows
            If DateDiff(DateInterval.Day, row2(0) & Space(1) & Replace(row2(1), "24:00", "23:59"), dtResult.Rows(iLastPos)(0) & Space(1) & Replace(dtResult.Rows(iLastPos)(1), "24:00", "23:59")) < iMaxReadDays Then
                dtResultTmp.ImportRow(row2)
            End If
        Next
        TimeAssign()
        dtResult.Rows.Clear()
        dtResult = dtResultTmp.Select("").CopyToDataTable
        For Each row As DataRow In dtResult.Rows
            iPos = dtResult.Rows.IndexOf(row)
            sTime = Replace(row(1), "24:00", "23:59")
            If row(2) <= MaxTemp2 And row(3) <= MaxTemp2 And row(4) <= MaxTemp2 Then
                If DateFrom = Nothing Then
                    DateFrom = row(0) & Space(1) & Replace(row(1), "24:00", "23:59")
                End If
            End If
            date1 = CDate(row(0) & " " & sTime)
            If row(2) <= MaxTemp1 And row(3) <= MaxTemp1 And row(4) <= MaxTemp1 And DateDiff(DateInterval.Day, date1, dtResult.Rows(dtResult.Rows.Count - 1)(0) & Space(1) & Replace(dtResult.Rows(dtResult.Rows.Count - 1)(1), "24:00", "23:59")) >= 15 Then
                MaxTemp = MaxTemp1
            ElseIf row(2) > MaxTemp1 Or row(3) > MaxTemp1 Or row(4) > MaxTemp1 Then
                MaxTemp = MaxTemp2
            End If
            If iPos > 0 Then
                sTime = Replace(dtResult.Rows(iPos - 1).Item(1), "24:00", "23:59")
                'If dtResult.Rows(iPos - 1).Item(1).trim = "24:00" Then
                '    sTime = "23:59:59"
                'End If
                date2 = CDate(dtResult.Rows(iPos - 1).Item(0) & " " & sTime)
                iDateDiff = DateDiff(DateInterval.Hour, date2, date1)
            End If
            If iDateDiff > 1 Then
                dtEvents.Rows.Add()
                dtEvents.Rows(dtEvents.Rows.Count - 1).Item(0) = "Exist a gap in the readings between " & Format(date2, "dd/MM/yyyy HH:mm") & " and " & Format(date1, "dd/MM/yyyy HH:mm")
                If date1 > CTInitialDate Then
                    CTInitialDate = date1
                End If
                FailDate = row(0)
                bProcessError = True
            End If
            If row(2) > MaxTemp2 Then
                dtEvents.Rows.Add()
                dtEvents.Rows(dtEvents.Rows.Count - 1).Item(0) = "The maximum allowable temperature on USDA1 is exceeded on " & Format(row(0), "dd/MM/yyyy") & " " & row(1)
                Sensor = IIf(Sensor = "", "USDA1", Sensor)
                FailDate = row(0)
                bProcessError = True
            End If
            If row(3) > MaxTemp2 Then
                dtEvents.Rows.Add()
                dtEvents.Rows(dtEvents.Rows.Count - 1).Item(0) = "The maximum allowable temperature on USDA2 is exceeded on " & Format(row(0), "dd/MM/yyyy") & " " & row(1)
                Sensor = IIf(Sensor = "", "USDA2", Sensor)
                FailDate = row(0)
                bProcessError = True
            End If
            If row(4) > MaxTemp2 Then
                dtEvents.Rows.Add()
                dtEvents.Rows(dtEvents.Rows.Count - 1).Item(0) = "The maximum allowable temperature on USDA3 is exceeded on " & Format(row(0), "dd/MM/yyyy") & " " & row(1)
                Sensor = IIf(Sensor = "", "USDA3", Sensor)
                FailDate = row(0)
                bProcessError = True
            End If
            FinalDate = row(0)
            FinalTime = Replace(row(1).trim, "24:00", "23:59")
        Next
        'GetFirsTime(dtResult)
        For Each brow As DataRow In dtBreak.Rows
            iBrokes += 1
            If Not IsDBNull(brow(2)) Then
                dtEvents.Rows.Add("Broke at " & Format(CDate(brow(0)), "dd/MM/yyyy") & Space(1) & brow(1) & " and restart at " & Format(CDate(brow(2)), "dd/MM/yyyy") & Space(1) & brow(3))
            Else
                dtEvents.Rows.Add("Broke at " & Format(CDate(brow(0)), "dd/MM/yyyy") & Space(1) & brow(1) & " and not restarted ")
            End If
        Next
        FinalDate = FinalDate & Space(1) & FinalTime
        iDaysInterval = DateDiff(DateInterval.Day, InitialDate, FinalDate).ToString
        sDaysInterval = sTiempo(InitialDate & Space(1) & InitialTime, FinalDate)
        iCTDaysInterval = DateDiff(DateInterval.Day, CTInitialDate, FinalDate).ToString
        sCTDaysInterval = sTiempo(CTInitialDate, FinalDate)
        dtEvents.Rows.Add()
        dtEvents.Rows(dtEvents.Rows.Count - 1).Item(0) = "Full reading from " & Format(InitialDate, "dd/MM/yyyy") & Space(1) & InitialTime & " to " & Format(FinalDate, "dd/MM/yyyy") & Space(1) & FinalTime & " (" & sDaysInterval & ")"
        dtEvents.Rows.Add()
        dtEvents.Rows(dtEvents.Rows.Count - 1).Item(0) = "CT reading from " & Format(CTInitialDate, "dd/MM/yyyy HH:mm") & " to " & Format(FinalDate, "dd/MM/yyyy") & Space(1) & FinalTime & " (" & sCTDaysInterval & ")"
        bsiRemarks2.Caption = "The maximum temperature for this container is: " & GetFocusedMaxTemp(DataMaxTemp).ToString
        Refresh()
        DateTo = DateAdd(DateInterval.Day, iDays, InitialDate)
        Deadline = DateAdd(DateInterval.Day, iDays + 2, InitialDate)
        gcEvents.DataSource = dtEvents
        gcVendorReadings.DataSource = dtResult
    End Sub

    Function GetFocusedMaxTemp(iMaxTempRead As Decimal)
        Dim dResult As Decimal = 0
        For r = 0 To GridView2.RowCount - 1
            Dim oRow As DataRow = GridView2.GetDataRow(r)
            dResult = oRow("TEMP")
            iDays = oRow("DAYS")
            If iMaxTempRead <= oRow("TEMP") Then
                Exit For
            End If
        Next
        Return dResult
    End Function

    Function sTiempo(dInicio As DateTime, dFin As DateTime) As String
        sTiempo = Str((DateDiff("s", dInicio, dFin) \ 86400) Mod 365).Trim & " days, "
        sTiempo = sTiempo & Str((DateDiff("s", dInicio, dFin) \ 3600) Mod 24).Trim & " hours"
        'sTiempo = sTiempo & Str((DateDiff("s", dInicio, dFin) \ 60) Mod 60) & " minutes, "
        'sTiempo = sTiempo & Str(DateDiff("s", dInicio, dFin) Mod 60) & " seconds"
    End Function

    Friend Function GetFirsTime(dtReadings As DataTable) As DataTable
        Dim dtDataTmp As New DataTable
        Dim iPos As Integer = 0
        Dim bSwitch As Boolean = False
        dtDataTmp.Columns.Add("date_ini", GetType(String)).AllowDBNull = True
        dtDataTmp.Columns.Add("time_ini", GetType(String)).AllowDBNull = True
        dtDataTmp.Columns.Add("date_end", GetType(String)).AllowDBNull = True
        dtDataTmp.Columns.Add("time_end", GetType(String)).AllowDBNull = True
        Try
            For Each row As DataRow In dtReadings.Rows
                If Not bSwitch Then
                    If (row(2) > MaxTemp Or row(3) > MaxTemp Or row(4) > MaxTemp) Then
                        dtDataTmp.Rows.Add()
                        dtDataTmp.Rows(dtDataTmp.Rows.Count - 1)("date_ini") = Format(row(0), "yyyy-MM-dd")
                        dtDataTmp.Rows(dtDataTmp.Rows.Count - 1)("time_ini") = row(1)
                        bSwitch = True
                    End If
                Else
                    If (row(2) <= MaxTemp And row(3) <= MaxTemp And row(4) <= MaxTemp) Then
                        dtDataTmp.Rows(dtDataTmp.Rows.Count - 1)("date_end") = Format(row(0), "yyyy-MM-dd")
                        dtDataTmp.Rows(dtDataTmp.Rows.Count - 1)("time_end") = row(1)
                        bSwitch = False
                    End If
                End If
            Next
        Catch ex As Exception
            'DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return dtDataTmp
    End Function

    Friend Function GetFirsTimeOld(dtReadings As DataTable) As ArrayList
        Dim aResult As New ArrayList
        Dim dResult As Date
        Dim row As DataRow
        Dim bBreak As Boolean = False
        aResult.AddRange({"", ""})
        Try
            For iPosition = 0 To dtReadings.Rows.Count - 1
                row = dtReadings.Rows(iPosition)
                dResult = row(0)
                'HoursQty = 1
                Do While dResult = row(0) And iPosition <= dtReadings.Rows.Count - 1
                    If bBreak Then
                        aResult(0) = Nothing
                        aResult(1) = Nothing
                    End If
                    If row(2) <= MaxTemp And row(3) <= MaxTemp And row(4) <= MaxTemp Then
                        'HoursQty = HoursQty + 1
                        If aResult(0) = Nothing And aResult(1) = Nothing Then
                            aResult(0) = row(0)
                            aResult(1) = row(1)
                            bBreak = False
                        End If
                    Else
                        bBreak = True
                    End If
                    iPosition = iPosition + 1
                    row = dtReadings.Rows(iPosition)
                Loop
                'If HoursQty = 24 Then
                '    Exit For
                'End If
            Next
        Catch ex As Exception
            'DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return aResult
    End Function

    Private Sub lueVoyage_Properties_EditValueChanged(sender As Object, e As EventArgs) Handles lueVoyage.Properties.EditValueChanged
        LoadPorts()
        VoyageValidate()
    End Sub

    Private Sub beDataSource_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs)
        OpenFileDialog2.Filter = "Excel Files (*.xls*;*.txt)|*.xls*"
        OpenFileDialog2.FileName = ""
        If OpenFileDialog2.ShowDialog() = DialogResult.OK Then
            beDataSource.Text = My.Settings.DataSourcePath & "\" & My.Settings.DBFileName 'OpenFileDialog2.FileName
        End If
    End Sub

    Private Sub GetMessageToSend()
        Dim drSource As DataRow
        Dim sCase As String = 0
        beDataSource.Text = My.Settings.DataSourcePath & "\" & My.Settings.DBFileName
        DateFrom = Format(CTInitialDate, "yyyy-MM-dd HH:mm")
        DateTo = Format(DateAdd(DateInterval.Day, 15, CTInitialDate), "yyyy-MM-dd HH:mm")
        Deadline = Format(DateAdd(DateInterval.Day, 2, DateTo), "yyyy-MM-dd HH:mm")
        If rgPort.SelectedIndex = 0 Then
            If dtBreak.Rows.Count > 0 Then
                sCase = "1"
            End If
        Else
            If dtBreak.Rows.Count > 0 Then
                sCase = "4"
            End If
            If dtBreak.Rows.Count = 0 Then
                sCase = "6"
            End If
        End If
        If LoadExcel(beDataSource.EditValue, "CT_WORDING$").Tables(0).Select("[Case] = '" & sCase & "'").Length > 0 Then
            drSource = LoadExcel(beDataSource.EditValue, "CT_WORDING$").Tables(0).Select("[Case] = '" & sCase & "'")(0)
            MailSubject = drSource.Item(1)
            MailBody = drSource.Item(2)
            If MailBody.Contains("{container}") Then
                MailBody = MailBody.Replace("{container}", ContainerNumber.ToUpper)
            End If
            If MailBody.Contains("{booking}") Then
                MailBody = MailBody.Replace("{booking}", Booking.ToUpper)
            End If
            If MailBody.Contains("{customer}") Then
                MailBody = MailBody.Replace("{customer}", Customer.ToUpper)
            End If
            If MailBody.Contains("{vessel}") Then
                MailBody = MailBody.Replace("{vessel}", Vessel.ToUpper.Trim)
            End If
            If MailBody.Contains("{voyage}") Then
                MailBody = MailBody.Replace("{voyage}", Voyage.ToUpper)
            End If
            If MailBody.Contains("{sensor}") Then
                MailBody = MailBody.Replace("{sensor}", Sensor)
            End If
            If MailBody.Contains("{DateFrom}") Then
                MailBody = MailBody.Replace("{DateFrom}", Format(DateFrom, "dd/MM/yyyy"))
            End If
            If MailBody.Contains("{DateTo}") Then
                MailBody = MailBody.Replace("{DateTo}", Format(DateTo, "dd/MM/yyyy"))
            End If
            If MailBody.Contains("{deadline}") Then
                MailBody = MailBody.Replace("{deadline}", Format(Deadline, "dd/MM/yyyy"))
            End If
            If MailBody.Contains("{tsp}") Then
                MailBody = MailBody.Replace("{tsp}", TSP.ToUpper)
            End If
            If MailBody.Contains("{pod}") Then
                MailBody = MailBody.Replace("{pod}", POD.ToUpper)
            End If
            If MailBody.Contains("{eta}") Then
                MailBody = MailBody.Replace("{eta}", Format(EtaDate, "dd/MM/yyyy"))
            End If
            If MailBody.Contains("{FailDate}") Then
                MailBody = MailBody.Replace("{FailDate}", Format(FailDate, "dd/MM/yyyy"))
            End If
            If MailBody.Contains("{Days}") Then
                MailBody = MailBody.Replace("{Days}", iDays.ToString)
            End If
        End If
    End Sub

    Private Sub LoadValidations()
        Validate()
        Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

        containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
        containsValidationRule.ErrorText = "Assign value."
        containsValidationRule.ErrorType = ErrorType.Critical

        Dim customValidationRule As New CustomValidationRule()
        customValidationRule.ErrorText = "Required value."
        customValidationRule.ErrorType = ErrorType.Critical

        'vpInputs.SetValidationRule(Me.beDataSource, customValidationRule)
        vpInputs.SetValidationRule(Me.beVendorData, customValidationRule)

    End Sub

    Private Sub bbiMessage_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiMessage.ItemClick
        MailBody = ""
        GetMessageToSend()
        If MailBody <> "" Then
            CreateSendItem(MailSubject, MailBody, "CT", "")
        End If
    End Sub

    'Sub CustomMailMessage()

    '    Dim OutApp As New Outlook.Application
    '    Dim objOutlookMsg As Outlook.MailItem
    '    Dim objOutlookRecip As Outlook.Recipient
    '    Dim Recipients As Outlook.Recipients

    '    'OutApp = CreateObject("Outlook.Application")
    '    objOutlookMsg = OutApp.CreateItem(Outlook.OlItemType.olMailItem)

    '    Recipients = objOutlookMsg.Recipients
    '    objOutlookRecip = Recipients.Add("aremonfe@gmail.com")
    '    objOutlookRecip.Type = 1

    '    objOutlookMsg.SentOnBehalfOfName = "ferarell@hotmail.com"

    '    objOutlookMsg.Subject = MailSubject

    '    objOutlookMsg.HTMLBody = MailBody & vbCrLf & vbCrLf

    '    'Resolve each Recipient's name.
    '    For Each objOutlookRecip In objOutlookMsg.Recipients
    '        objOutlookRecip.Resolve()
    '    Next

    '    'objOutlookMsg.Send
    '    objOutlookMsg.Display()

    '    OutApp = Nothing

    'End Sub

    'Private Sub CreateSendItem()
    '    Dim Application As New Outlook.Application '= Nothing
    '    Dim mail As Outlook.MailItem = Nothing
    '    Dim mailRecipients As Outlook.Recipients = Nothing
    '    Dim mailRecipient As Outlook.Recipient = Nothing
    '    Try
    '        mail = Application.CreateItem(Outlook.OlItemType.olMailItem)
    '        mail.Subject = MailSubject
    '        mail.Body = MailBody
    '        'mail.To = "ferarell@hotmail.com;aremonfe@gmail.com" 'My.Settings.MailRecipients
    '        'mail.CC = "ferarell@yahoo.com" '"perudownload@hlag.com"
    '        mailRecipients = mail.Recipients
    '        mailRecipient = mailRecipients.Add("Cesar.carranza@hlag.com")
    '        mailRecipient = mailRecipients.Add("Veronica.Portella@hlag.com")
    '        mailRecipient = mailRecipients.Add("Claudia.Pinillos@hlag.com")
    '        mail.CC = "perudownload@hlag.com"
    '        mailRecipient.Resolve()
    '        If (mailRecipient.Resolved) Then
    '            mail.Display()
    '        Else
    '            System.Windows.Forms.MessageBox.Show(
    '                "There is no such record in your address book.")
    '        End If
    '    Catch ex As Exception
    '        System.Windows.Forms.MessageBox.Show(ex.Message,
    '            "An exception is occured in the code of add-in.")
    '    Finally
    '        If Not IsNothing(mailRecipient) Then Marshal.ReleaseComObject(mailRecipient)
    '        If Not IsNothing(mailRecipients) Then Marshal.ReleaseComObject(mailRecipients)
    '        If Not IsNothing(mail) Then Marshal.ReleaseComObject(mail)
    '    End Try
    'End Sub

    Private Sub DataMasterUpdate()
        Dim SetValues, Condition As String
        Dim dtQuery As New DataTable
        Dim chkdlstat, tschkdlstat As String
        SplashScreenManager.Default.SetWaitFormDescription("Update Data Master of Cold Treatment")
        Condition = "[CONTAINER] = '" & ContainerNumber & "' AND [BOOKING]='" & Booking & "'"
        dtQuery = oAppService.ExecuteSQL("select * from tck.ColdTreatment where " & Condition).Tables(0).Select(Condition).CopyToDataTable
        SetValues = ""
        chkdlstat = IIf(bProcessError, "INTERRUPTION", "OK")
        tschkdlstat = IIf(DateDiff(DateInterval.Day, DateFrom, FinalDate) >= 15, "OK", "INCOMPLETE")
        If rgPort.SelectedIndex = 0 Then
            SetValues = SetValues & IIf(SetValues <> "", ", ", "") & "[CHKDL]='" & chkdlstat & "'"
            SetValues = SetValues & IIf(SetValues <> "", ", ", "") & "[INIDATE]='" & Format(DateFrom, "yyyyMMdd HH:mm") & "'"
        Else
            dtQuery.Rows(0)("CHKDL") = IIf(IsDBNull(dtQuery.Rows(0)("CHKDL")), "", dtQuery.Rows(0)("CHKDL"))
            If dtQuery.Rows(0)("CHKDL") = "" Then
                SetValues = SetValues & IIf(SetValues <> "", ", ", "") & "[CHKDL]='" & chkdlstat & "'"
            End If
            SetValues = SetValues & IIf(SetValues <> "", ", ", "") & "[INIDATE]='" & Format(DateFrom, "yyyyMMdd HH:mm") & "'"
            SetValues = SetValues & IIf(SetValues <> "", ", ", "") & "[TSCHKDL]='" & tschkdlstat & "'"
        End If
        If IsDBNull(dtQuery.Rows(0)("TSCHKDL")) Then
            dtQuery.Rows(0)("TSCHKDL") = ""
        End If
        If (dtQuery.Rows(0)("TSCHKDL") = "OK" Or SetValues.Contains("[TSCHKDL]='OK'")) And iCTDaysInterval >= 15 Then 'And Not SetValues.Contains("INTERRUPTION") Then
            SetValues = SetValues & ", [REMARKS]='CT PASSED'"
        ElseIf iCTDaysInterval < 15 Then
            SetValues = SetValues & ", [REMARKS]='INCOMPLETE'"
        ElseIf bProcessError Then
            Dim sErrors As New RichTextBox
            For Each row As DataRow In dtEvents.Rows
                If dtEvents.Rows(0)(0).ToString.Contains("Broke") Then
                    sErrors.Lines.SetValue(dtEvents.Rows(0)(0), sErrors.Lines.Count)
                End If
            Next
            SetValues = SetValues & ", [REMARKS]='" & sErrors.Text & "'"
        End If
        'If iDays = 0 Then
        '    iDays = 15
        '    If MaxTemp > 1.11 And iBrokes > 1 Then
        '        iDays = 17
        '    End If
        'End If
        SetValues = SetValues & ", [FINDATE]='" & Format(DateAdd(DateInterval.Day, iDays, CTInitialDate), "yyyyMMdd HH:mm") & "'"
        SetValues = SetValues & ", [CTDAYS]=" & iDays.ToString
        'Update
        Dim aResult As New ArrayList
        aResult.AddRange(oAppService.ExecuteSQLNonQuery("update tck.ColdTreatment SET " & SetValues & " WHERE " & Condition))
        If aResult(0) = True Then
            UpdateVendorReadings()
            UpdateVendorEvents()
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show("The update was successfuly.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show("The update was not successfuly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

    End Sub

    Private Sub rgMode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rgMode.SelectedIndexChanged
        bbiSave.Enabled = False
        'If rgMode.SelectedIndex = 1 Then
        '    bbiSave.Enabled = True
        'End If
        VoyageValidate()
    End Sub

    Private Sub bbiSave_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSave.ItemClick
        If rgMode.SelectedIndex > 0 Then
            If DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "This process will update the data master, do you want continue?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                Me.Refresh()
                SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
                WaitForm.CheckForIllegalCrossThreadCalls = True
                DataMasterUpdate()
            End If
        End If
        If My.Settings.MailEnabled Then
            If dtEvents.Rows.Count > 0 Then
                GetMessageToSend()
                SendMail(MailSubject, MailBody, True)
            End If
        End If
    End Sub

    Private Sub lueVoyage_TextChanged(sender As Object, e As EventArgs) Handles lueVoyage.TextChanged
        Booking = lueVoyage.GetColumnValue("BOOKING")
        Voyage = GetVoyageFromVessel(lueVoyage.Text)
        Vessel = lueVoyage.Text.Replace(Voyage, "")
        Customer = lueVoyage.GetColumnValue("EXPORT_PARTY")
        POD = lueVoyage.GetColumnValue("POD")
        TSP = lueVoyage.GetColumnValue("TSP")
    End Sub

    Private Sub VoyageValidate()
        Validate()
        'If bbiSave.Enabled = False Then
        If rgMode.SelectedIndex > 0 And oAppService.ExecuteSQL("select * from tck.ColdTreatment where BOOKING='" & lueVoyage.GetColumnValue("BOOKING") & "' and CONTAINER='" & ContainerNumber & "' and REMARKS='CT PASSED'").Tables(0).Rows.Count > 0 Then
            DevExpress.XtraEditors.XtraMessageBox.Show("Cold treatment finalized, will not update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            bbiSave.Enabled = False
            Return
        End If
        bbiSave.Enabled = True
        'End If
    End Sub


End Class