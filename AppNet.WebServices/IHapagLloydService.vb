Imports System.ServiceModel

' NOTA: puede usar el comando "Cambiar nombre" del menú contextual para cambiar el nombre de interfaz "IHapagLloydService" en el código y en el archivo de configuración a la vez.
<ServiceContract()>
Public Interface IHapagLloydService

    <OperationContract()>
    Function ExecuteSQL(ByVal QueryString As String) As DataSet

    <OperationContract()>
    Function ExecuteSQLNonQuery(ByVal QueryString As String) As ArrayList

    <OperationContract()>
    Function UpdateTableWithBulkCopy(ByVal Table As String, ByVal dtSource As DataTable, ByVal ProcessType As String) As ArrayList

    <OperationContract()>
    Function UpdatingUsingTableAsParameter(ByVal StoreProcedure As String, ByVal Params As ArrayList, ByVal Values As ArrayList, ByVal dtSource As DataTable) As ArrayList

    <OperationContract()>
    Function InsertContacts(ByVal aSource As ArrayList) As Boolean

    <OperationContract()>
    Function InsertBlackList(ByVal aSource As ArrayList) As Boolean

    <OperationContract()>
    Function InsertColdTreatment(ByVal dtSource As DataTable) As Boolean

    <OperationContract()>
    Function UpdateColdTreatment(ByVal dtSource As DataTable) As Boolean

    <OperationContract()>
    Function DeleteColdTreatment(ByVal aSource As ArrayList) As Boolean

    <OperationContract()>
    Function InsertColdTreatmentEvents(ByVal aSource As ArrayList) As Boolean

    <OperationContract()>
    Function DeleteColdTreatmentEvents(ByVal aSource As ArrayList) As Boolean

    <OperationContract()>
    Function InsertColdTreatmentReadings(ByVal aSource As ArrayList) As Boolean

    <OperationContract()>
    Function CustomStoredProcedureExecution(ByVal StoreProcedure As String, ByVal ValueList As ArrayList, ByVal dtSource As DataTable) As ArrayList

    <OperationContract()>
    Function DeleteColdTreatmentReadings(ByVal aSource As ArrayList) As Boolean

    <OperationContract()>
    Function InsertScheduleVoyage(ByVal dtSource As DataTable) As Boolean

    <OperationContract()>
    Function InsertReeferDataMaster(ByVal dtSource As DataTable) As Boolean

    <OperationContract()>
    Function InsertPartners(ByVal dtSource As DataTable) As Boolean

    <OperationContract()>
    Function GetLocalChargeInvoicing(DateFrom As DateTime, DateTo As DateTime, Country As String) As DataTable

End Interface
