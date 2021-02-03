Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports System.Data

Public Class IncomeReconciliationForm
    Dim dtInWebFocus As New DataTable
    Dim dtOutWebFocus As New DataTable
    Dim dtInFis As New DataTable
    Dim dtOutFis As New DataTable

    Private Sub beSourceFile_Properties_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles beInFis.Properties.ButtonClick, beOutFis.Properties.ButtonClick, beConcessions.Properties.ButtonClick, beInWebFocus.Properties.ButtonClick, beOutWebFocus.Properties.ButtonClick
        OpenFileDialog1.Filter = "Excel Files (*.xls*)|*.xls*"
        OpenFileDialog1.FileName = ""
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            sender.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub bbiProcess_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiProcess.ItemClick
        Validate()
        'If Not vpInputs.Validate Then
        '    Return
        'End If
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Copy FIS files")
        Dim wsGateIn As Excel.Worksheet = CType(Globals.ThisAddIn.Application.ActiveWorkbook.Worksheets(4), Excel.Worksheet)
        Dim GateInRange As Excel.Range = wsGateIn.UsedRange

        Dim wsGateOut As Excel.Worksheet = CType(Globals.ThisAddIn.Application.ActiveWorkbook.Worksheets(5), Excel.Worksheet)
        Dim GateOutRange As Excel.Range = wsGateOut.UsedRange

        Dim oInFis As Excel.Worksheet = Globals.ThisAddIn.Application.Workbooks.Open(beInFis.Text, , True).Worksheets(2)
        Dim oOutFis As Excel.Worksheet = Globals.ThisAddIn.Application.Workbooks.Open(beOutFis.Text, , True).Worksheets(2)

        Try
            oInFis.Name = "IN FIS"
            oInFis.Copy(After:=wsGateIn)
            oInFis.Application.ActiveWorkbook.Close(False)
            oOutFis.Name = "OUT FIS"
            oOutFis.Copy(After:=wsGateOut)
            oOutFis.Application.ActiveWorkbook.Close(False)

            'GATE IN
            Globals.ThisAddIn.Application.Sheets("GATE IN").Select()
            If beInFis.Text.Trim <> "" Then
                SplashScreenManager.Default.SetWaitFormDescription("Loading IN FIS file")
                dtInFis = LoadExcelWithConditions(beInFis.Text, "Data_Landscape_blackwhite$", "").Tables(0)
            End If
            If beInWebFocus.Text.Trim <> "" Then
                SplashScreenManager.Default.SetWaitFormDescription("Loading IN Web Focus file")
                dtInWebFocus = LoadExcelWithConditions(beInWebFocus.Text, "AdhocRequest$", "[F2] <> '' and [F2] <> 'RR SSY Relation'").Tables(0)
            End If
            SplashScreenManager.Default.SetWaitFormDescription("Updating GATE IN sheet")
            Dim wsInFis As Excel.Worksheet = CType(Globals.ThisAddIn.Application.ActiveWorkbook.Sheets("IN FIS"), Excel.Worksheet)
            Dim InFisRange As Excel.Range = wsInFis.Range("J1")
            InFisRange.EntireColumn.Insert(Excel.XlInsertShiftDirection.xlShiftToRight, Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow)
            InFisRange = wsInFis.UsedRange
            InFisRange.Cells(3, 10) = "COMMENT"
            Dim drInFis As DataRow = Nothing
            For n = 4 To InFisRange.Rows.CountLarge
                If InFisRange.Cells(n, 1).Value = "" Then
                    Continue For
                End If
                Dim sCondition As String = "NoName8='" & InFisRange.Cells(n, 9).Value & "'"
                If dtInFis.Select(sCondition).Length > 0 Then
                    drInFis = dtInFis.Select(sCondition)(0)
                    InFisRange.Cells(n, 10) = GetInComment(InFisRange.Cells(n, 9).Value)
                End If
            Next
            wsGateIn.EnableAutoFilter = False
            GateInRange = GateInRange.Range("N1", "R1")
            GateInRange.EntireColumn.Insert(Excel.XlInsertShiftDirection.xlShiftToRight, Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow)
            GateInRange = wsGateIn.UsedRange
            GateInRange.Cells(8, 14) = "BK"
            GateInRange.Cells(8, 15) = "RL"
            GateInRange.Cells(8, 16) = "RA"
            GateInRange.Cells(8, 17) = "CNEE"
            GateInRange.Cells(8, 18) = "COMMENT"
            Dim drGateIn As DataRow = Nothing
            For n = 9 To GateInRange.Rows.CountLarge
                Dim sCondition As String = "ReportTitle='" & GateInRange.Cells(n, 3).Value & "'"
                If dtInWebFocus.Select(sCondition).Length > 0 Then
                    drGateIn = dtInWebFocus.Select(sCondition)(0)
                    GateInRange.Cells(n, 14) = drGateIn(3)
                    GateInRange.Cells(n, 15) = drGateIn(1)
                    GateInRange.Cells(n, 16) = drGateIn(6)
                    GateInRange.Cells(n, 17) = drGateIn(5)
                    'GateInRange.Cells(n, 18) = GetInComment(dtInFis, GateInRange.Cells(n, 3).Value)
                End If
            Next

            'GATE OUT
            Globals.ThisAddIn.Application.Sheets("GATE OUT").Select()
            If beOutFis.Text.Trim <> "" Then
                SplashScreenManager.Default.SetWaitFormDescription("Loading OUT FIS file")
                dtOutFis = LoadExcelWithConditions(beOutFis.Text, "Data_Landscape_blackwhite$", "").Tables(0)
            End If
            If beOutWebFocus.Text.Trim <> "" Then
                SplashScreenManager.Default.SetWaitFormDescription("Loading OUT Web Focus file")
                dtOutWebFocus = LoadExcelWithConditions(beOutWebFocus.Text, "AdhocRequest$", "[F2] <> '' and [F2] <> 'RR SSY Relation'").Tables(0)
            End If
            SplashScreenManager.Default.SetWaitFormDescription("Updating GATE OUT sheet")
            Dim wsOutFis As Excel.Worksheet = CType(Globals.ThisAddIn.Application.ActiveWorkbook.Sheets("OUT FIS"), Excel.Worksheet)
            Dim OutFisRange As Excel.Range = wsOutFis.Range("J1")
            OutFisRange.EntireColumn.Insert(Excel.XlInsertShiftDirection.xlShiftToRight, Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow)
            OutFisRange = wsOutFis.UsedRange
            OutFisRange.Cells(3, 10) = "COMMENT"
            Dim drOutFis As DataRow = Nothing
            For n = 4 To OutFisRange.Rows.CountLarge
                If OutFisRange.Cells(n, 1).Value = "" Then
                    Continue For
                End If
                Dim sCondition As String = "NoName8='" & OutFisRange.Cells(n, 9).Value & "'"
                If dtOutFis.Select(sCondition).Length > 0 Then
                    'drOutFis = dtOutFis.Select(sCondition)(0)
                    OutFisRange.Cells(n, 10) = GetOutComment(OutFisRange.Cells(n, 9).Value, OutFisRange.Cells(n, 1).Value)
                End If
            Next
            wsGateOut.EnableAutoFilter = False
            GateOutRange = GateOutRange.Range("M1", "P1")
            GateOutRange.EntireColumn.Insert(Excel.XlInsertShiftDirection.xlShiftToRight, Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove)
            GateOutRange = wsGateOut.UsedRange
            GateOutRange.Columns("N").NumberFormat = "General"
            GateOutRange.Cells(9, 13) = "RL"
            GateOutRange.Cells(9, 14) = "RA"
            GateOutRange.Cells(9, 15) = "SHIPPER"
            GateOutRange.Cells(9, 16) = "COMMENT"
            Dim drGateOut As DataRow = Nothing
            'GateOutRange.Cells(n, 14).NumberFormat = "Text"
            For n = 10 To GateOutRange.Rows.CountLarge
                Dim sCondition As String = "ReportTitle='" & Replace(Replace(GateOutRange.Cells(n, 3).Value, " ", ""), "-", "") & "'"
                If dtOutWebFocus.Select(sCondition).Length > 0 Then
                    drGateOut = dtOutWebFocus.Select(sCondition)(0)
                    GateOutRange.Cells(n, 13) = drGateOut(2)
                    GateOutRange.Cells(n, 14) = drGateOut(3)
                    GateOutRange.Cells(n, 15) = drGateOut(5)
                End If
            Next

        Catch ex As Exception

        End Try
        SplashScreenManager.CloseForm(False)
        Close()
    End Sub

    Function GetInComment(sContainer As String) As String
        Dim sResult As String = ""
        sContainer = Replace(sContainer, " ", "")
        Dim sCondition As String = "NoName8='" & Mid(sContainer, 1, 4) & Space(2) & Mid(sContainer, 5, 7) & "'"
        If dtInWebFocus.Select("ReportTitle='" & sContainer & "'").Length = 0 Then
            sResult = "VERIFICAR"
        ElseIf dtInFis.Select(sCondition).Length > 0 Then
            Dim oRow As DataRow = dtInFis.Select(sCondition)(0)
            If oRow(0) = "INMR" Then
                sResult = "REPARACIONES"
            ElseIf oRow(0) = "GOSF" Then
                sResult = "STOCK FEEDING"
            ElseIf oRow(0) = "GIMT" Then
                If oRow(20) = "UMT" Then
                    sResult = "NO APLICA GATE"
                ElseIf oRow(20) = "AVA" And oRow(11) = "CU" Then
                    sResult = "APLICA GATE"
                End If
            End If
        End If
        Return sResult
    End Function

    Function GetOutComment(sContainer As String, sEventCode As String) As String
        Dim sResult As String = ""
        sContainer = Replace(Replace(sContainer, " ", ""), "-", "")
        Dim sCondition As String = "NoName8='" & Mid(sContainer, 1, 4) & Space(2) & Mid(sContainer, 5, 7) & "' and NoName='" & sEventCode & "'"
        If dtOutWebFocus.Select("ReportTitle='" & sContainer & "'").Length = 0 Then
            sResult = ""
        End If
        If dtOutFis.Select(sCondition).Length > 0 Then
            Dim oRow As DataRow = dtOutFis.Select(sCondition)(0)
            If oRow(0) = "EXMR" Then
                sResult = "REPARACIONES"
            ElseIf oRow(0) = "GOSF" Then
                sResult = "STOCK FEEDING"
            ElseIf oRow(0) = "GOMT" Then
                If oRow(20) = "UMT" Then
                    sResult = "NO APLICA GATE"
                ElseIf oRow(20) = "USE" And oRow(11) = "CU" Then
                    sResult = "APLICA GATE"
                End If
            End If
        End If
        Return sResult
    End Function

    Private Sub LoadInputValidations()
        Dim containsValidationRule As New DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule()

        containsValidationRule.ConditionOperator = ConditionOperator.IsNotBlank
        containsValidationRule.ErrorText = "Assign value."
        containsValidationRule.ErrorType = ErrorType.Critical

        Dim customValidationRule As New CustomValidationRule()
        customValidationRule.ErrorText = "Required value."
        customValidationRule.ErrorType = ErrorType.Critical

        vpInputs.SetValidationRule(beInFis, customValidationRule)
        vpInputs.SetValidationRule(beOutFis, customValidationRule)
        vpInputs.SetValidationRule(beConcessions, customValidationRule)
        vpInputs.SetValidationRule(beInWebFocus, customValidationRule)

    End Sub

    Private Sub IncomeReconciliationForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadInputValidations()
    End Sub
End Class