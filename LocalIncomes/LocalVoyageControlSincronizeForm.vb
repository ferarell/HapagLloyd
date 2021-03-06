﻿Imports DevExpress.XtraSplashScreen

Public Class LocalVoyageControlSincronizeForm
    Dim oSharePointTransactions As New SharePointListTransactions
    Dim dtList, dtCoordinator As New DataTable

    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        oSharePointTransactions.SharePointUrl = My.Settings.SharePoint_Url
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub VesselScheduleSincronizeForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            oSharePointTransactions.SharePointList = "CoordinatorByServiceList"
            oSharePointTransactions.FieldsList.Clear()
            oSharePointTransactions.FieldsList.Add({"Coordinator_Area"})
            oSharePointTransactions.FieldsList.Add({"Coordinator_Service"})
            oSharePointTransactions.FieldsList.Add({"Coordinator_x0020_UserAccount"})
            oSharePointTransactions.FieldsList.Add({"Coordinator_x0020_UserName"})
            SplashScreenManager.Default.SetWaitFormDescription("Get Coordinator List")
            dtCoordinator = oSharePointTransactions.GetItems()

            SplashScreenManager.Default.SetWaitFormDescription("Get Local Voyage Control List")
            'oSharePointTransactions.SharePointUrl = My.Settings.SharePoint_Url
            oSharePointTransactions.SharePointList = "Local Voyage Control"
            oSharePointTransactions.FieldsList.Clear()
            oSharePointTransactions.FieldsList.Add({"ID"})
            oSharePointTransactions.FieldsList.Add({"SSY"})
            oSharePointTransactions.FieldsList.Add({"Port_Locode"})
            oSharePointTransactions.FieldsList.Add({"TerminalCode"})
            oSharePointTransactions.FieldsList.Add({"DPVoyage"})
            oSharePointTransactions.FieldsList.Add({"VesselName"})
            oSharePointTransactions.FieldsList.Add({"ScheduleVoyage"})
            oSharePointTransactions.FieldsList.Add({"Arrival_Date"})
            oSharePointTransactions.FieldsList.Add({"Departure_Date"})
            oSharePointTransactions.FieldsList.Add({"Close_Document_Date"})
            oSharePointTransactions.FieldsList.Add({"Coordinator_Name"})
            oSharePointTransactions.FieldsList.Add({"Coordinator_UserAccount"})
            oSharePointTransactions.FieldsList.Add({"Coordinator_x0020_UserName"})
            oSharePointTransactions.FieldsList.Add({"Local_Transmition_Date"})
            oSharePointTransactions.FieldsList.Add({"Manifest_Number"})

            dtList = oSharePointTransactions.GetItems()
            GridControl1.DataSource = dtList
            SplashScreenManager.CloseForm(False)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Sincronize()
        Dim dtSource As New DataTable
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            dtSource = LoadExcelHDR(beDataSource.Text, "Data_Landscape_color$", "A3:N3000").Tables(0)
            For r = 0 To dtSource.Rows.Count - 1
                Dim sArrDateTime, sDepDateTime, sCloDateTime As String
                If IsDBNull(dtSource.Rows(r)(1)) Then
                    Continue For
                End If
                For c = 0 To dtSource.Rows(r).ItemArray.Count - 1
                    If IsDBNull(dtSource.Rows(r).Item(c)) Then
                        dtSource.Rows(r).Item(c) = ""
                    End If
                Next
                If dtSource.Rows(r)(1) = "" Then
                    Continue For
                End If
                If dtSource.Rows(r)("DP Voyage") = 0 Then
                    Continue For
                End If
                If dtCoordinator.Select("Coordinator_Service='" & dtSource.Rows(r)("SSY") & "'").Length = 0 Then
                    Continue For
                End If
                Dim oDPVoyage, oPol As String
                oDPVoyage = dtSource.Rows(r)("DP Voyage")
                oPol = dtSource.Rows(r)("Port Locode")
                Dim IdRow As Integer = 0
                If dtList.Select("DPVoyage = '" & oDPVoyage & "' AND Port_Locode = '" & oPol & "'").Length > 0 Then
                    IdRow = dtList.Select("DPVoyage = '" & oDPVoyage & "' AND Port_Locode = '" & oPol & "'")(0)("ID")
                End If
                If dtSource.Rows(r)("Arr Date") <> "" Then
                    sArrDateTime = Format(CDate(dtSource.Rows(r)("Arr Date") & Space(1) & IIf(dtSource.Rows(r)("Arr Time") = "", "00:00", dtSource.Rows(r)("Arr Time"))), "M/d/yyyy HH:mm")
                End If
                If dtSource.Rows(r)("Dep Date") <> "" Then
                    sDepDateTime = Format(CDate(dtSource.Rows(r)("Dep Date") & Space(1) & IIf(dtSource.Rows(r)("Dep Time") = "", "00:00", dtSource.Rows(r)("Dep Time"))), "M/d/yyyy HH:mm")
                End If
                If dtSource.Rows(r)("Close Docu Date") <> "" Then
                    sCloDateTime = Format(CDate(dtSource.Rows(r)("Close Docu Date") & Space(1) & IIf(dtSource.Rows(r)("Close Docu Time") = "", "00:00", dtSource.Rows(r)("Close Docu Time"))), "M/d/yyyy HH:mm")
                End If
                If IdRow = 0 Then
                    oSharePointTransactions.ValuesList.Clear()
                    oSharePointTransactions.ValuesList.Add({"SSY", dtSource.Rows(r)("SSY")})
                    oSharePointTransactions.ValuesList.Add({"Port_Locode", oPol})
                    oSharePointTransactions.ValuesList.Add({"TerminalCode", dtSource.Rows(r)("Terminal")})
                    oSharePointTransactions.ValuesList.Add({"DPVoyage", oDPVoyage})
                    oSharePointTransactions.ValuesList.Add({"VesselName", dtSource.Rows(r)("Vessel")})
                    oSharePointTransactions.ValuesList.Add({"ScheduleVoyage", dtSource.Rows(r)("Schedule Voyage No#")})
 
                    If IsDate(sArrDateTime) Then
                        oSharePointTransactions.ValuesList.Add({"Arrival_Date", sArrDateTime})
                    End If
                    If IsDate(sDepDateTime) Then
                        oSharePointTransactions.ValuesList.Add({"Departure_Date", sDepDateTime})
                    End If
                    If IsDate(sCloDateTime) Then
                        oSharePointTransactions.ValuesList.Add({"Close_Document_Date", sCloDateTime})
                    End If
                    If dtCoordinator.Select("Coordinator_Service='" & dtSource.Rows(r)("SSY") & "'").Length > 0 Then
                        oSharePointTransactions.ValuesList.Add({"Coordinator_Name", dtCoordinator.Select("Coordinator_Service='" & dtSource.Rows(r)("SSY") & "'")(0)("Coordinator_x0020_UserName")})
                        'oSharePointTransactions.ValuesList.Add({"Coordinator_UserAccount", dtCoordinator.Select("Coordinator_Service='" & dtSource.Rows(r)("SSY") & "'")(0)("Coordinator_x0020_UserAccount")})
                    End If
                    'oSharePointTransactions.FieldsList.Add({"Coordinator_x0020_UserName", dtSource.Rows(r)("Coordinator_x0020_UserName")})
                    'oSharePointTransactions.FieldsList.Add({"Local_Transmition_Date", dtSource.Rows(r)("Local_Transmition_Date")})
                    oSharePointTransactions.InsertItem()
                Else
                    oSharePointTransactions.ValuesList.Clear()
                    Dim drItem As DataRow = dtList.Select("ID=" & IdRow.ToString)(0)
                    If IsDate(sArrDateTime) Then
                        If CDate(sArrDateTime) <> drItem("Arrival_Date") Then
                            oSharePointTransactions.ValuesList.Add({"Arrival_Date", sArrDateTime})
                        End If
                    End If
                    If IsDate(sDepDateTime) Then
                        If CDate(sDepDateTime) <> drItem("Departure_Date") Then
                            oSharePointTransactions.ValuesList.Add({"Departure_Date", sDepDateTime})
                        End If
                    End If
                    If IsDate(sCloDateTime) Then
                        If CDate(sCloDateTime) <> drItem("Close_Document_Date") Then
                            oSharePointTransactions.ValuesList.Add({"Close_Document_Date", sCloDateTime})
                        End If
                    End If
                    If oSharePointTransactions.ValuesList.Count > 0 Then
                        oSharePointTransactions.UpdateItem(IdRow)
                    End If
                End If
            Next
            bbiShowAll.PerformClick()
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, "The process has been completed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bbiSincronize_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiSincronize.ItemClick
        Sincronize()
    End Sub

    Private Sub bbiShowAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiShowAll.ItemClick
        Try
            SplashScreenManager.ShowForm(Me, GetType(WaitForm), True, True, False)
            GridControl1.DataSource = Nothing
            dtList.Rows.Clear()
            dtList = oSharePointTransactions.GetItems()
            GridControl1.DataSource = dtList
            SplashScreenManager.CloseForm(False)
        Catch ex As Exception
            SplashScreenManager.CloseForm(False)
            DevExpress.XtraEditors.XtraMessageBox.Show(Me.LookAndFeel, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub bbiExport_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiExport.ItemClick
        ExportarExcel(GridControl1)
    End Sub

    Private Sub bbiClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbiClose.ItemClick
        Close()
    End Sub

End Class