Imports System
Imports System.Data

Public Class ReeferDataMasterUpdate
    Dim dtSource As New DataTable

    Friend Function DataProcess(oFileName As String) As Boolean
        Dim bResult As Boolean = True
        If LoadExcel(oFileName, "{0}").Tables(0).Columns.Count <= 33 Then
            dtSource = QueryExcel(oFileName, "SELECT F1 AS [POL],F4 AS [EqpType],F5 AS [Container],F12 AS [Booking],F13 AS [MainType],F16 AS [DPVoyage],Spec_Prod AS [SpecialProduct],F25 AS [TSP],F27 AS [ShipperMR_Name],F28 AS [ShipperMR_Code],F29 AS [POD],F30 AS [CommodityDescription] FROM [Stock Change Units$] WHERE F1 IS NOT NULL").Tables(0)
        End If
        bResult = DataProcess()
        Return bResult
    End Function

    Friend Function DataProcess() As Boolean
        Dim bResult As Boolean = True
        Dim ContainerNumber, Vessel, DPVoyage, sCondition, sValues, sTransaction As String
        Dim dtQuery, dtReeferDM, dtColdTreatment, dtScheduleVoyage1, dtTranshipment As New DataTable
        Dim iPos As Integer = 0
        Try
            dtReeferDM = ExecuteAccessQuery("SELECT * FROM ReeferDataMaster WHERE Booking IS NULL", "dbColdTreatment.accdb").Tables(0)
            For r = 0 To dtSource.Rows.Count - 1
                Dim oRow As DataRow = dtSource.Rows(r + 1)
                oRow("Booking") = IIf(IsDBNull(oRow("Booking")), 0, oRow("Booking"))
                oRow("Container") = IIf(IsDBNull(oRow("Container")), "", oRow("Container"))
                If oRow("Booking") = 0 Or oRow("Container") = "" Then
                    Continue For
                End If
                dtTranshipment.Rows.Clear()
                ContainerNumber = Replace(oRow("Container"), " ", "")
                DPVoyage = "000000"
                If Not IsDBNull(oRow("DPVoyage")) Then
                    DPVoyage = Format(CInt(oRow("DPVoyage")), "000000")
                End If
                dtColdTreatment = ExecuteAccessQuery("select * from ColdTreatment where Container = '" & ContainerNumber & "' and Booking = '" & oRow("Booking").ToString & "'", "dbColdTreatment.accdb").Tables(0)
                'If dtSourceFile2.Rows.Count > 0 Then
                '    If dtSourceFile2.Select("Container = '" & oRow("Container") & "' and Booking = '" & oRow("Booking").ToString & "'").Length > 0 Then
                '        dtTranshipment = dtSourceFile2.Select("Container = '" & oRow("Container") & "' and Booking = '" & oRow("Booking").ToString & "'").CopyToDataTable
                '    End If
                'End If
                dtScheduleVoyage1 = ExecuteAccessQuery("select * from ScheduleVoyage where POL = '" & oRow("POL") & "' and DPVOYAGE = '" & DPVoyage & "'", "dbColdTreatment.accdb").Tables(0)
                dtQuery = ExecuteAccessQuery("select * from ReeferDataMaster where Container = '" & ContainerNumber & "' and Booking = '" & oRow("Booking").ToString & "'", "dbColdTreatment.accdb").Tables(0)
                If dtQuery.Rows.Count > 0 Then
                    sTransaction = "Update"
                    sCondition = "Container = '" & ContainerNumber & "' and Booking = '" & oRow("Booking").ToString & "'"
                    sValues = ""
                    If Not IsDBNull(oRow("EqpType")) Then
                        sValues += IIf(sValues = "", "", ", ") & "EqpType='" & oRow("EqpType") & "'"
                    End If
                    If Not IsDBNull(oRow("MainType")) Then
                        sValues += IIf(sValues = "", "", ", ") & "MainType='" & oRow("MainType") & "'"
                    End If
                    If Not IsDBNull(oRow("SpecialProduct")) Then
                        sValues += IIf(sValues = "", "", ", ") & "SpecialProduct='" & oRow("SpecialProduct") & "'"
                    End If
                    If dtColdTreatment.Rows.Count > 0 Then
                        sValues += IIf(sValues = "", "", ", ") & "IsColdTreatment=1"
                    End If
                    If dtScheduleVoyage1.Rows.Count > 0 Then
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("POL")) Then
                            sValues += IIf(sValues = "", "", ", ") & "POL='" & dtScheduleVoyage1.Rows(0)("POL") & "'"
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("ETD")) Then
                            sValues += IIf(sValues = "", "", ", ") & "Departure1='" & dtScheduleVoyage1.Rows(0)("ETD") & "'"
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("DPVOYAGE")) Then
                            sValues += IIf(sValues = "", "", ", ") & "DPVoyage1='" & Format(CInt(dtScheduleVoyage1.Rows(0)("DPVOYAGE")), "000000") & "'"
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("VESSEL_NAME")) Then
                            sValues += IIf(sValues = "", "", ", ") & "VesselName1='" & dtScheduleVoyage1.Rows(0)("VESSEL_NAME") & "'"
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("SCHEDULE")) Then
                            sValues += IIf(sValues = "", "", ", ") & "VesselVoyage1='" & dtScheduleVoyage1.Rows(0)("SCHEDULE") & "'"
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("SERVICE")) Then
                            sValues += IIf(sValues = "", "", ", ") & "Service='" & dtScheduleVoyage1.Rows(0)("SERVICE") & "'"
                        End If
                    End If
                    If Not IsDBNull(oRow("TSP")) Then
                        sValues += IIf(sValues = "", "", ", ") & "TSP='" & oRow("TSP") & "'"
                    End If
                    If dtTranshipment.Rows.Count > 0 Then
                        If Not IsDBNull(dtTranshipment.Rows(0)("ArrivalTSP")) Then
                            sValues += IIf(sValues = "", "", ", ") & "ArrivalTSP='" & dtTranshipment.Rows(0)("ArrivalTSP") & "'"
                        End If
                        dtReeferDM.Rows(iPos).Item("Notify2") = 0
                        If Not IsDBNull(dtTranshipment.Rows(0)("Departure2")) Then
                            sValues += IIf(sValues = "", "", ", ") & "Departure2='" & dtTranshipment.Rows(0)("Departure2") & "'"
                        End If
                        If Not IsDBNull(dtTranshipment.Rows(0)("DPVoyage2")) Then
                            sValues += IIf(sValues = "", "", ", ") & "DPVoyage2='" & Format(CInt(dtTranshipment.Rows(0)("DPVoyage2")), "000000") & "'"
                        End If
                        If Not IsDBNull(dtTranshipment.Rows(0)("VesselName2")) Then
                            sValues += IIf(sValues = "", "", ", ") & "VesselName2='" & dtTranshipment.Rows(0)("VesselName2") & "'"
                        End If
                        If Not IsDBNull(dtTranshipment.Rows(0)("ArrivalPOD")) Then
                            sValues += IIf(sValues = "", "", ", ") & "ArrivalPOD='" & dtTranshipment.Rows(0)("ArrivalPOD") & "'"
                        End If
                    End If
                    If Not IsDBNull(oRow("POD")) Then
                        sValues += IIf(sValues = "", "", ", ") & "POD='" & oRow("POD") & "'"
                    End If
                    sValues += IIf(sValues = "", "", ", ") & "UpdatedBy='" & My.User.Name & "'"
                    sValues += IIf(sValues = "", "", ", ") & "UpdatedDate='" & Now.ToString & "'"

                    UpdateAccess("ReeferDataMaster", sCondition, sValues, "dbColdTreatment.accdb")
                Else
                    sTransaction = "Insert"
                    dtReeferDM.Rows.Add()
                    iPos = dtReeferDM.Rows.Count - 1
                    dtReeferDM.Rows(iPos).Item("Booking") = oRow("Booking")
                    dtReeferDM.Rows(iPos).Item("Container") = ContainerNumber
                    dtReeferDM.Rows(iPos).Item("EqpType") = oRow("EqpType")
                    dtReeferDM.Rows(iPos).Item("MainType") = oRow("MainType")
                    If Not IsDBNull(oRow("SpecialProduct")) Then
                        dtReeferDM.Rows(iPos).Item("SpecialProduct") = oRow("SpecialProduct")
                    End If
                    If dtColdTreatment.Rows.Count > 0 Then
                        dtReeferDM.Rows(iPos).Item("IsColdTreatment") = 1
                    End If
                    If Not IsDBNull(oRow("ShipperMR_Name")) And Not IsDBNull(oRow("ShipperMR_Code")) Then
                        dtReeferDM.Rows(iPos).Item("ShipperMR") = oRow("ShipperMR_Name").ToString.Trim & Space(1) & Format(CInt(oRow("ShipperMR_Code")), "000")
                    End If
                    If Not IsDBNull(oRow("CommodityDescription")) Then
                        dtReeferDM.Rows(iPos).Item("CommodityDescription") = Replace(oRow("CommodityDescription"), "'", " ")
                    End If
                    If dtScheduleVoyage1.Rows.Count > 0 Then
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("POL")) Then
                            dtReeferDM.Rows(iPos).Item("POL") = dtScheduleVoyage1.Rows(0)("POL")
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("ETD")) Then
                            dtReeferDM.Rows(iPos).Item("Departure1") = dtScheduleVoyage1.Rows(0)("ETD")
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("DPVOYAGE")) Then
                            dtReeferDM.Rows(iPos).Item("DPVoyage1") = Format(CInt(dtScheduleVoyage1.Rows(0)("DPVOYAGE")), "000000")
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("VESSEL_NAME")) Then
                            dtReeferDM.Rows(iPos).Item("VesselName1") = dtScheduleVoyage1.Rows(0)("VESSEL_NAME")
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("SCHEDULE")) Then
                            dtReeferDM.Rows(iPos).Item("VesselVoyage1") = dtScheduleVoyage1.Rows(0)("SCHEDULE")
                        End If
                        If Not IsDBNull(dtScheduleVoyage1.Rows(0)("SERVICE")) Then
                            dtReeferDM.Rows(iPos).Item("Service") = dtScheduleVoyage1.Rows(0)("SERVICE")
                        End If
                    End If
                    If Not IsDBNull(oRow("TSP")) Then
                        dtReeferDM.Rows(iPos).Item("TSP") = oRow("TSP")
                    End If
                    If dtTranshipment.Rows.Count > 0 Then
                        If Not IsDBNull(dtTranshipment.Rows(0)("ArrivalTSP")) Then
                            dtReeferDM.Rows(iPos).Item("ArrivalTSP") = dtTranshipment.Rows(0)("ArrivalTSP")
                        End If
                        dtReeferDM.Rows(iPos).Item("Notify2") = 0
                        If Not IsDBNull(dtTranshipment.Rows(0)("Departure2")) Then
                            dtReeferDM.Rows(iPos).Item("Departure2") = dtTranshipment.Rows(0)("Departure2")
                        End If
                        If Not IsDBNull(dtTranshipment.Rows(0)("DPVoyage2")) Then
                            dtReeferDM.Rows(iPos).Item("DPVoyage2") = Format(CInt(dtTranshipment.Rows(0)("DPVoyage2")), "000000")
                        End If
                        If Not IsDBNull(dtTranshipment.Rows(0)("VesselName2")) Then
                            dtReeferDM.Rows(iPos).Item("VesselName2") = dtTranshipment.Rows(0)("VesselName2")
                        End If
                        If Not IsDBNull(dtTranshipment.Rows(0)("ArrivalPOD")) Then
                            dtReeferDM.Rows(iPos).Item("ArrivalPOD") = dtTranshipment.Rows(0)("ArrivalPOD")
                        End If
                    End If
                    If Not IsDBNull(oRow("POD")) Then
                        dtReeferDM.Rows(iPos).Item("POD") = oRow("POD")
                    End If
                    dtReeferDM.Rows(iPos).Item("CreatedBy") = My.User.Name
                    dtReeferDM.Rows(iPos).Item("CreatedDate") = Now
                    'dtReeferDM.Rows(iPos).Item("TransitDays") = oRow("")
                    'dtReeferDM.Rows(iPos).Item("Comments") = "" 'oRow("")
                    InsertIntoAccess("ReeferDataMaster", dtReeferDM.Rows(iPos), "dbColdTreatment.accdb")
                End If
            Next

        Catch ex As Exception
            bResult = False
        End Try
        Return bResult
    End Function

End Class
