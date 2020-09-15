Imports DevExpress.XtraEditors.DXErrorProvider
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports DevExpress.XtraSplashScreen
Imports System.Threading
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports Microsoft.Office.Interop
Imports System.Runtime.InteropServices
Imports System.ComponentModel.DataAnnotations

Public Class NotificationsWcfForm
    Dim eMailTo As String = ""
    Dim sRegime As String = ""
    Dim dtContacts, dtBookings, dtBlackList As New DataTable
    Dim oAppService As New AppService.HapagLloydServiceClient

    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()

        DevExpress.Skins.SkinManager.EnableFormSkins()
        DevExpress.UserSkins.BonusSkins.Register()

        SkinName = My.Settings.LookAndFeel

    End Sub

    Private Sub NotificationsForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Me.Dispose()
        End
    End Sub

    Private Sub NotificationsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False
        DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(SkinName)
        Me.Icon = My.Application.ApplicationContext.MainForm.Icon
        Dim iWidth As Integer = 400
        SplitContainerControl1.SplitterPosition = Me.Size.Height - 350
        SplitContainerControl2.SplitterPosition = iWidth
        SplitContainerControl3.SplitterPosition = Me.Size.Width - iWidth
        ImageListBoxControl1.Items.Clear()
        ImageListBoxControl1.MultiColumn = True
        bsiCountry.Caption = "Country: " & My.Settings.Country
        bsiUser.Caption = "User: " & My.User.Name
    End Sub

    Private Sub bbiOpenFile_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiOpenFile.ItemClick
        OpenFileDialog2.Filter = "Source Files (*.doc*;*.htm*)|*.doc*;*.htm*"
        If Not OpenFileDialog2.ShowDialog = Windows.Forms.DialogResult.OK Then
            Return
        End If
        If OpenFileDialog2.FileName.ToUpper.Contains(".HTM") Then
            richEditControl.LoadDocument(OpenFileDialog2.FileName, DevExpress.XtraRichEdit.DocumentFormat.Html)
        ElseIf OpenFileDialog2.FileName.ToUpper.Contains(".DOC") Then
            richEditControl.LoadDocument(OpenFileDialog2.FileName)
        End If
        XtraTabControl1.SelectedTabPageIndex = 1
    End Sub

    Private Sub bbiLoadContacts_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiLoadContacts.ItemClick
        If beiRegime.EditValue <> "" Then
            sRegime = Mid(beiRegime.EditValue, 1, 1)
        End If
        If sRegime = "" Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "You must select the regime", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        If My.Settings.Country Is Nothing Or My.Settings.Country = "" Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "You must select the Country in the Preference option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Load Contacts")
        dtContacts.Clear()
        'dtContacts = oAppService.ExecuteSQL("select distinct * from ntf.vwActiveContacts where CountryCode = '" & My.Settings.Country & "' and Regime='" & sRegime & "'").Tables(0)
        dtContacts = oAppService.ExecuteSQL("select * from ntf.vwActiveContacts where CountryCode = '" & My.Settings.Country & "' and Regime='" & sRegime & "'").Tables(0)
        If Not dtContacts.Columns.Contains("Checked") Then
            dtContacts.Columns.Add("Checked", GetType(Boolean)).DefaultValue = False
        End If
        SplashScreenManager.Default.SetWaitFormDescription("Load Black List")
        dtBlackList = oAppService.ExecuteSQL("select distinct * from ntf.BlackList where CountryCode = '" & My.Settings.Country & "' and Regime='" & sRegime & "'").Tables(0)
        SplashScreenManager.CloseForm(False)
        gcBlackList.DataSource = dtBlackList
        gcMainData.DataSource = dtContacts
    End Sub

    Private Sub bbiAttachFiles_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiAttachFiles.ItemClick
        OpenFileDialog1.ShowDialog()
        ImageListBoxControl1.Items.Clear()
        For r = 0 To OpenFileDialog1.FileNames.Count - 1
            ImageListBoxControl1.Items.Add(OpenFileDialog1.FileNames(r))
        Next

    End Sub

    Private Sub bbiMessagePreview_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiMessagePreview.ItemClick
        Validate()
        Dim aFiles As New ArrayList
        Dim oRow As DataRow
        For i = 0 To ImageListBoxControl1.Items.Count - 1
            aFiles.Add(ImageListBoxControl1.Items(i).Value)
        Next
        eMailTo = edtTO.Text.Trim
        If DevExpress.XtraEditors.XtraMessageBox.Show("Due to new recently applied policies, only the first " & My.Settings.EmailQuantityBySend.ToString & " emails will be included in the selected list, do you want to continue?", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning) = Windows.Forms.DialogResult.No Then
            Return
        End If
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Loading eMail Adresses List")
        Dim iEmailQuantityBySend As Integer = IIf(GridView1.RowCount > My.Settings.EmailQuantityBySend, My.Settings.EmailQuantityBySend, GridView1.RowCount)
        For r = 0 To iEmailQuantityBySend - 1 'GridView1.RowCount - 1
            oRow = GridView1.GetDataRow(r)
            If IsDBNull(oRow("Checked")) Then
                Continue For
            End If
            If oRow("Checked") Then
                eMailTo += IIf(eMailTo.Trim.Length > 0, ";", "") & oRow("eMail").ToString.Trim
            End If
        Next
        SplashScreenManager.CloseForm(False)
        If eMailTo = "" Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "You must select at least one email", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Creating a New Message")
        CreateSendItem(edtSubject.Text, richEditControl.HtmlText, aFiles, "Display")
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub bbiSendMessage_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSendByDocument.ItemClick
        If GridView2.RowCount = 0 Then
            DevExpress.XtraEditors.XtraMessageBox.Show("You must load bookings list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Dim dtBookingByMatchCode As New DataTable
        Dim eMail As String = ""
        Dim oMatchCode1 As String = ""
        Dim oMatchCode2 As String = ""
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Sending Messages")
        XtraTabControl1.SelectedTabPageIndex = 0
        For r = 0 To GridView1.RowCount - 1
            Try
                Dim oRow As DataRow = GridView1.GetDataRow(r)
                oMatchCode1 = oRow("MatchCode")
                If IsDBNull(oRow("Checked")) Then
                    Continue For
                End If
                If Not oRow("Checked") Then
                    Continue For
                End If
                eMail = ""
                oMatchCode2 = GridView1.GetDataRow(r)("MatchCode")
                While oMatchCode1 = oMatchCode2
                    If Not eMail.Contains(GridView1.GetDataRow(r)("eMail")) Then
                        eMail += GridView1.GetDataRow(r)("eMail") & ";"
                    End If
                    r += 1
                    If r >= GridView1.RowCount Then
                        oMatchCode2 = ""
                    Else
                        oMatchCode2 = GridView1.GetDataRow(r)("MatchCode")
                    End If
                End While
                r -= 1
                'oRow = GridView1.GetDataRow(r)
                dtBookingByMatchCode = dtBookings.Select("F2='" & oMatchCode1 & "'").CopyToDataTable
                SendBookingsByMail(dtBookingByMatchCode, eMail)
            Catch ex As Exception

            End Try
        Next
        SplashScreenManager.CloseForm(False)
    End Sub

    Private Sub SendBookingsByMail(dtSource As DataTable, eMail As String)
        Dim Application As New Outlook.Application
        Dim mail As Outlook.MailItem = Nothing
        Dim mailRecipients As Outlook.Recipients = Nothing
        Dim mailRecipient As Outlook.Recipient = Nothing
        Dim BookingsList As New RichTextBox
        Dim sMatchCode As String = ""
        Try
            For r = 0 To dtSource.Rows.Count - 1
                BookingsList.AppendText(dtSource.Rows(r)("F1") & "<br>")
                sMatchCode = dtSource.Rows(r)("F2")
            Next
            mail = Application.CreateItem(Outlook.OlItemType.olMailItem)
            mail.Subject = edtSubject.Text
            mail.HTMLBody = Replace(richEditControl.HtmlText, "[BookingsList]", BookingsList.Text)
            mail.HTMLBody = Replace(mail.HTMLBody, "[MatchCode]", sMatchCode)
            'mail.HTMLBody += "<br><br> " & eMail
            For i = 0 To ImageListBoxControl1.Items.Count - 1
                mail.Attachments.Add(ImageListBoxControl1.Items(i).Value)
            Next
            mail.To = eMail & ";" & edtTO.Text
            mail.CC = edtCC.Text
            mail.BCC = edtBCC.Text
            mail.Send()
        Catch ex As Exception
            System.Windows.Forms.MessageBox.Show(ex.Message,
                "An exception is occured in the code of add-in.")
        Finally
            If Not IsNothing(mailRecipient) Then Marshal.ReleaseComObject(mailRecipient)
            If Not IsNothing(mailRecipients) Then Marshal.ReleaseComObject(mailRecipients)
            If Not IsNothing(mail) Then Marshal.ReleaseComObject(mail)
        End Try
    End Sub

    Friend Sub CreateSendItem(Subject As String, Body As String, AttachFile As ArrayList, CreateType As String)
        Dim Application As New Outlook.Application
        Dim mail As Outlook.MailItem = Nothing
        Dim mailRecipients As Outlook.Recipients = Nothing
        Dim mailRecipient As Outlook.Recipient = Nothing
        Try
            mail = Application.CreateItem(Outlook.OlItemType.olMailItem)
            mail.Subject = Subject
            mail.HTMLBody = Body
            If AttachFile.Count > 0 Then
                For f = 0 To AttachFile.Count - 1
                    mail.Attachments.Add(AttachFile(f))
                Next
            End If
            mail.BCC = eMailTo
            If CreateType = "Display" Then
                mail.Display()
            End If
            If CreateType = "Send" Then
                mail.Send()
            End If
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            System.Windows.Forms.MessageBox.Show(ex.Message,
                "An exception is occured in the code of add-in.")
        Finally
            If Not IsNothing(mailRecipient) Then Marshal.ReleaseComObject(mailRecipient)
            If Not IsNothing(mailRecipients) Then Marshal.ReleaseComObject(mailRecipients)
            If Not IsNothing(mail) Then Marshal.ReleaseComObject(mail)
        End Try

    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

    Private Sub ImageListBoxControl1_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyValue = Keys.Delete Then
            ImageListBoxControl1.Items.RemoveAt(ImageListBoxControl1.SelectedIndex)
        End If
    End Sub

    Private Sub rgMatchCode_SelectedIndexChanged(sender As Object, e As EventArgs)
        FilterContactsByMatchCode()
    End Sub

    Private Sub FilterContactsByMatchCode()
        Dim iPos As Integer = 0
        If MemoEdit1.Text.Length = 0 Then
            Return
        End If
        Dim sFilter As String = ""
        If rgMatchCode.SelectedIndex = 0 Then
            GridView1.ActiveFilterString = ""
        End If
        If rgMatchCode.SelectedIndex = 1 Then
            GridView1.ActiveFilterString = ""
            For l = 0 To MemoEdit1.Lines.Count - 1
                If MemoEdit1.Lines(l).Length > 0 Then
                    iPos = InStr(1, MemoEdit1.Lines(l), " ") - 1
                    sFilter += IIf(l > 0, " OR ", "") & "[MatchCode] = '" & Mid(MemoEdit1.Lines(l), 1, iPos) & Space(1) & Trim(Mid(MemoEdit1.Lines(l), iPos + 1, Len(MemoEdit1.Lines(l)))) & "'"
                End If
            Next
            GridView1.ActiveFilterCriteria = DevExpress.Data.Filtering.CriteriaOperator.Or
            GridView1.ActiveFilterString = "(" & sFilter & ")"
        End If
    End Sub

    Private Sub SeleccionaFilas(caso As Integer)
        Dim i As Integer = 0
        For i = 0 To GridView1.RowCount - 1
            Dim row As DataRow = GridView1.GetDataRow(i)
            If Not IsValidEmail(row("eMail")) Then
                Continue For
            End If
            If caso = 0 Then
                row("Checked") = True
            End If
            If caso = 1 Then
                row("Checked") = False
            End If
            If caso = 2 Then
                If row("Checked") Then
                    row("Checked") = False
                Else
                    row("Checked") = True
                End If
            End If
        Next
    End Sub

    Private Sub SeleccionaTodosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SeleccionaTodosToolStripMenuItem.Click
        SeleccionaFilas(0)
    End Sub

    Private Sub DeseleccionaTodosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeseleccionaTodosToolStripMenuItem.Click
        SeleccionaFilas(1)
    End Sub

    Private Sub InvertirSelecciónToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InvertirSelecciónToolStripMenuItem.Click
        SeleccionaFilas(2)
    End Sub

    Private Sub RepositoryItemCheckEdit1_CheckedChanged(sender As Object, e As EventArgs) Handles RepositoryItemCheckEdit1.CheckedChanged
        If Not IsValidEmail(GridView1.GetFocusedRowCellValue("eMail")) Then
            GridView1.SetFocusedRowCellValue("Checked", False)
        End If
    End Sub

    Private Sub bbiConfiguration_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiConfiguration.ItemClick
        Dim oForm As New PreferencesForm
        oForm.ShowDialog()
    End Sub

    Private Sub NotificationsForm_TextChanged(sender As Object, e As EventArgs) Handles MyBase.TextChanged
        Me.Text = My.Application.Info.ProductName + " [" + My.Application.Info.Version.ToString + "]"
    End Sub

    Private Sub bbiImportContacts_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiImportContacts.ItemClick
        If beiRegime.EditValue <> "" Then
            sRegime = Mid(beiRegime.EditValue, 1, 1)
        End If
        If sRegime = "" Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "You must select the regime", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        If My.Settings.Country Is Nothing Or My.Settings.Country = "" Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "You must select the Country in the Preference option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        Dim dtSource As New DataTable
        OpenFileDialog3.Filter = "Import File (*.xls*)|*.xls*"
        If Not OpenFileDialog3.ShowDialog = Windows.Forms.DialogResult.OK Then
            Return
        End If
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        SplashScreenManager.Default.SetWaitFormDescription("Load Data Source of Contacts")
        dtSource = LoadExcelWC(OpenFileDialog3.FileName, "Data-MSG-Defaults per MC filter$", "F10='EMAIL'").Tables(0)
        If dtSource.Rows.Count = 0 Then
            Return
        End If
        'Dim dtResult As New DataTable
        Dim sMatchCode As String = ""
        'dtResult.Columns.Add("Regime", GetType(String)).DefaultValue = ""
        'dtResult.Columns.Add("MatchCode", GetType(String)).DefaultValue = ""
        'dtResult.Columns.Add("eMail", GetType(String)).DefaultValue = ""
        'dtResult.Columns.Add("Status", GetType(String)).DefaultValue = "A"
        'dtResult.Columns.Add("CreatedBy", GetType(String)).DefaultValue = ""
        'dtResult.Columns.Add("CreatedDate", GetType(Date)).DefaultValue = Now
        SplashScreenManager.Default.SetWaitFormDescription("Delete Current Contacts")
        oAppService.ExecuteSQLNonQuery("delete from ntf.Contacts where CountryCode = '" & My.Settings.Country & "' and Regime='" & sRegime & "'")
        For r = 0 To dtSource.Rows.Count - 1
            Dim oRow As DataRow = dtSource.Rows(r)
            sMatchCode = oRow("F1").ToString.Trim & Space(1) & CInt(oRow("F2")).ToString
            oRow("F11") = Replace(oRow("F11"), "'", "").Trim
            'dtResult.Rows.Add(sRegime, sMatchCode, oRow("F11"), "A", My.User.Name, Now)
            Dim aSource As New ArrayList
            aSource.AddRange({My.Settings.Country, sRegime, sMatchCode, oRow("F11"), "A", My.User.Name, Now})
            SplashScreenManager.Default.SetWaitFormDescription("Insert eMail Contacts (" & (r + 1).ToString & " of " & dtSource.Rows.Count.ToString & ")")
            'InsertIntoAccess("Contacts", dtResult.Rows(r))
            oAppService.InsertContacts(aSource.ToArray)

        Next
        SplashScreenManager.CloseForm(False)
        'bbiLoadContacts.PerformClick()
    End Sub

    Private Sub gcBookingFilter_EmbeddedNavigator_ButtonClick(sender As Object, e As DevExpress.XtraEditors.NavigatorButtonClickEventArgs) Handles gcBookingFilter.EmbeddedNavigator.ButtonClick
        If e.Button.Tag = "LoadExcel" Then
            If GridView1.RowCount = 0 Then
                bbiLoadContacts.PerformClick()
            End If
            OpenFileDialog3.Filter = "Import File (*.xls*)|*.xls*"
            If Not OpenFileDialog3.ShowDialog = Windows.Forms.DialogResult.OK Then
                Return
            End If
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Load Excel Data File")
            dtBookings.Rows.Clear()
            dtBookings = LoadExcelWH(OpenFileDialog3.FileName, "{0}").Tables(0)
            SplashScreenManager.CloseForm(False)
            If dtBookings.Rows.Count = 0 Then
                Return
            End If

            dtBookings = UpdateDataTable(SelectDistinct(dtBookings, "", "F1", "F2"))
            gcBookingFilter.DataSource = dtBookings
            FilterContactsByBookings(SelectDistinct(dtBookings, "", "F2"))
        End If
    End Sub

    Private Sub gcBlackList_EmbeddedNavigator_ButtonClick(sender As Object, e As DevExpress.XtraEditors.NavigatorButtonClickEventArgs) Handles gcBlackList.EmbeddedNavigator.ButtonClick
        If e.Button.Tag = "LoadText" Then
            OpenFileDialog3.Filter = "Import File (*.txt)|*.txt"
            If Not OpenFileDialog3.ShowDialog = Windows.Forms.DialogResult.OK Then
                Return
            End If
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Load Text File")
            dtBlackList.Rows.Clear()
            dtBlackList = LoadBlackListTextFile(OpenFileDialog3.FileName)
            SplashScreenManager.CloseForm(False)
            If dtBookings.Rows.Count = 0 Then
                Return
            End If
        End If
    End Sub

    Friend Function LoadBlackListTextFile(FileName As String) As DataTable
        Dim iPosition As Integer = 0
        Dim sEqpType, sMatchCode As String
        Dim dtResult As New DataTable
        dtResult.Columns.Add("Regime", GetType(String)).DefaultValue = ""
        dtResult.Columns.Add("MatchCode", GetType(String)).DefaultValue = ""
        Validate()
        If beiRegime.EditValue <> "" Then
            sRegime = Mid(beiRegime.EditValue, 1, 1)
        End If
        If sRegime = "" Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "You must select the regime", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return dtResult
        End If
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            SplashScreenManager.Default.SetWaitFormDescription("Clean Black List")
            oAppService.ExecuteSQLNonQuery("delete from ntf.BlackList where CountryCode = '" & My.Settings.Country & "' and Regime='" & sRegime & "'")
            SplashScreenManager.Default.SetWaitFormDescription("Update Black List")
            Using sr As New StreamReader(FileName)
                Dim lines As List(Of String) = New List(Of String)
                Dim line As String = ""
                Dim bExit As Boolean = False
                Do While Not sr.EndOfStream
                    line = sr.ReadLine()
                    If Mid(line, 105, 5) = "INACT" Or Mid(line, 105, 5) = "INVAL" Then
                        lines.Add(line)
                    End If
                Loop
                Dim bSkip As Boolean = True
                For i As Integer = 0 To lines.Count - 1
                    SplashScreenManager.Default.SetWaitFormDescription("Update Black List (Row: " & (i + 1).ToString & " of " & (lines.Count - 1).ToString & ")")
                    If Mid(lines(i), 23, 10).Trim = "" Then
                        Continue For
                    End If
                    'If Mid(lines(i), 105, 5).Trim = "ACTIV" Then
                    '    Continue For
                    'End If
                    sMatchCode = Mid(lines(i), 23, 7).Trim & Space(1) & CInt(Mid(lines(i), 30, 3)).ToString
                    If oAppService.ExecuteSQL("select * from ntf.Contacts where MatchCode = '" & sMatchCode & "' and Country = '" & My.Settings.Country & "' and Regime='" & sRegime & "'").Tables(0).Rows.Count = 0 Then
                        Continue For
                    End If
                    dtResult.Rows.Add()
                    iPosition = dtResult.Rows.Count - 1
                    dtResult.Rows(iPosition).Item(0) = sRegime
                    dtResult.Rows(iPosition).Item(1) = sMatchCode
                    Dim aSource As New ArrayList
                    aSource.AddRange({My.Settings.Country, sRegime, sMatchCode, My.User.Name, Now})
                    oAppService.InsertBlackList(aSource.ToArray)
                    'InsertIntoAccess("BlackList", dtResult.Rows(iPosition))
                Next
            End Using
            gcBlackList.DataSource = dtResult
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The process has been completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return dtResult
    End Function

    Friend Function UpdateDataTable(dtSource As DataTable) As DataTable
        Dim IPos As Integer = 0
        For r = 0 To dtSource.Rows.Count - 1
            Dim oRow As DataRow = dtSource.Rows(r)
            IPos = InStr(oRow("F2").ToString.Trim, " ") - 1
            oRow("F2") = Mid(oRow("F2"), 1, IPos) & Space(1) & Trim(CInt(Mid(oRow("F2"), IPos + 1, Len(oRow("F2")))).ToString)
        Next
        Return dtSource
    End Function

    Private Sub FilterContactsByBookings(dtBookings As DataTable)
        If GridView1.RowCount = 0 Then
            bbiLoadContacts.PerformClick()
        End If
        Dim sFilter As String = ""
        GridView1.ActiveFilterString = ""
        For r = 0 To dtBookings.Rows.Count - 1
            Dim oRow As DataRow = dtBookings.Rows(r)
            sFilter += IIf(r > 0, " OR ", "") & "[MatchCode] = '" & oRow("F2") & "'"
        Next
        GridView1.ActiveFilterCriteria = DevExpress.Data.Filtering.CriteriaOperator.Or
        GridView1.ActiveFilterString = "(" & sFilter & ")"
    End Sub

    Private Sub GridView1_RowStyle(sender As Object, e As RowStyleEventArgs) Handles GridView1.RowStyle
        Dim View As GridView = sender
        If (e.RowHandle >= 0) Then
            If Not View.Columns("eMail") Is Nothing Then
                Dim C1 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns("eMail"))
                If Not IsValidEmail(C1) Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If
            End If
        End If
    End Sub
    Private Sub GridView2_RowStyle(sender As Object, e As RowStyleEventArgs) Handles GridView2.RowStyle
        Dim View As GridView = sender
        If (e.RowHandle >= 0) Then
            If Not View.Columns(1) Is Nothing Then
                Dim C1 As String = View.GetRowCellDisplayText(e.RowHandle, View.Columns(1))
                If dtContacts.Select("MatchCode='" & C1 & "'").Length = 0 Then
                    e.Appearance.BackColor = Color.Salmon
                    e.Appearance.BackColor2 = Color.SeaShell
                End If
            End If
        End If
    End Sub

    Private Sub beiRegime_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles beiRegime.ItemClick
        Validate()
    End Sub

    Private Sub gcMainData_EmbeddedNavigator_ButtonClick(sender As Object, e As DevExpress.XtraEditors.NavigatorButtonClickEventArgs) Handles gcMainData.EmbeddedNavigator.ButtonClick
        If e.Button.Tag = "Excel" Then
            ExportarExcel(gcMainData)
        End If
    End Sub

    Private Sub bbiSendAllByGroups_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSendAllByGroups.ItemClick
        Validate()
        Dim aFiles As New ArrayList
        Dim oRow As DataRow
        For i = 0 To ImageListBoxControl1.Items.Count - 1
            aFiles.Add(ImageListBoxControl1.Items(i).Value)
        Next
        eMailTo = edtTO.Text.Trim
        Dim iSelected As Integer = GetSelectedQ()
        If iSelected = 0 Then
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "You must select at least one email", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If DevExpress.XtraEditors.XtraMessageBox.Show("Due to new recently implemented policies, messages will be created and sent every " & My.Settings.EmailQuantityBySend.ToString & " contacts from the selected list, do you want to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
            Return
        End If
        Dim iGroupsQ As Integer = Math.Ceiling(iSelected / My.Settings.EmailQuantityBySend)
        SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
        Dim iCurrentGroup As Integer = 1
        For r = 0 To GridView1.RowCount - 1
            oRow = GridView1.GetDataRow(r)
            If IsDBNull(oRow("Checked")) Then
                Continue For
            End If
            If (r / iCurrentGroup = My.Settings.EmailQuantityBySend) Or (r = GridView1.RowCount - 1 And iCurrentGroup = iGroupsQ) Then
                If r = GridView1.RowCount - 1 Then
                    If oRow("Checked") Then
                        eMailTo += IIf(eMailTo.Trim.Length > 0, ";", "") & oRow("eMail").ToString.Trim
                    End If
                End If
                SplashScreenManager.Default.SetWaitFormDescription("Sending Message " & iCurrentGroup.ToString & " of " & iGroupsQ.ToString)
                CreateSendItem(edtSubject.Text, richEditControl.HtmlText, aFiles, "Send")
                iCurrentGroup += 1
                eMailTo = ""
            End If
            If oRow("Checked") Then
                eMailTo += IIf(eMailTo.Trim.Length > 0, ";", "") & oRow("eMail").ToString.Trim
            End If
        Next
        SplashScreenManager.CloseForm(False)
    End Sub

    Function GetSelectedQ() As Integer
        Dim iResult As Integer = 0
        For r = 0 To GridView1.RowCount - 1
            If IsDBNull(GridView1.GetDataRow(r)("Checked")) Then
                Continue For
            End If
            If GridView1.GetDataRow(r)("Checked") Then
                iResult += 1
            End If
        Next
        Return iResult
    End Function
End Class