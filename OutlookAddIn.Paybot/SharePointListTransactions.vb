Imports System
Imports System.Security
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading
Imports System.Configuration
Imports System.Globalization
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Client
Imports System.Collections
Imports System.Data
Imports System.Net

Public Class SharePointListTransactions
    Dim Password As New SecureString
    Friend SharePointUrl As String
    Friend SharePointList As String
    Friend ValuesList, FieldsList As New ArrayList

    Public Sub New()
        'Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        For Each c As Char In My.Settings.SharePoint_Password
            Password.AppendChar(c)
        Next
    End Sub

    Friend Sub InsertItem()
        Dim clienContext As New ClientContext(SharePointUrl)
        clienContext.Credentials = New SharePointOnlineCredentials(My.Settings.SharePoint_User, Password)
        Dim oList As List = clienContext.Web.Lists.GetByTitle(SharePointList)
        Dim listItemCreationInformation As New ListItemCreationInformation
        Dim oListItem As ListItem = oList.AddItem(listItemCreationInformation)

        For c = 0 To ValuesList.Count - 1
            oListItem(ValuesList(c)(0)) = ValuesList(c)(1)
        Next

        oListItem.Update()
        clienContext.ExecuteQueryRetry()

    End Sub

    Friend Sub UpdateItem(IdRow As Integer)
        Dim clienContext As New ClientContext(SharePointUrl)
        clienContext.Credentials = New SharePointOnlineCredentials(My.Settings.SharePoint_User, Password)
        Dim oList As List = clienContext.Web.Lists.GetByTitle(SharePointList)
        Dim oListItem As ListItem = oList.GetItemById(IdRow)

        For c = 0 To ValuesList.Count - 1
            oListItem(ValuesList(c)(0)) = ValuesList(c)(1)
        Next

        oListItem.Update()
        clienContext.ExecuteQueryRetry()
    End Sub

    Friend Sub DeleteItem(IdRows As Integer)

    End Sub

    Friend Sub SelectItem(dtItems As DataTable)
        Dim clienContext As New ClientContext(SharePointUrl)
        clienContext.Credentials = New SharePointOnlineCredentials(My.Settings.SharePoint_User, Password)
        Dim oList As List = clienContext.Web.Lists.GetByTitle(SharePointList)
        Dim oQuery As New CamlQuery
        oQuery = CamlQuery.CreateAllItemsQuery()
        oQuery.ViewXml = "<View/>"
        Dim oItemsList As ListItemCollection = oList.GetItems(oQuery)
        clienContext.Load(oList)
        clienContext.Load(oItemsList)
        clienContext.ExecuteQueryRetry()
        Dim listFields As FieldCollection = oList.Fields

        For Each Item As ListItem In oItemsList
            dtItems.Rows.Add()
            For c = 0 To dtItems.Columns.Count - 1
                dtItems.Rows(dtItems.Rows.Count - 1)(c) = Item(ValuesList(c)(0))
            Next
        Next

    End Sub

    Friend Function GetItems() As DataTable
        Dim clienContext As New ClientContext(SharePointUrl)
        clienContext.Credentials = New SharePointOnlineCredentials(My.Settings.SharePoint_User, Password)
        Dim oList As List = clienContext.Web.Lists.GetByTitle(SharePointList)
        Dim oQuery As New CamlQuery
        oQuery = CamlQuery.CreateAllItemsQuery()
        oQuery.ViewXml = "<View/>"
        Dim oItemsList As ListItemCollection = oList.GetItems(oQuery)
        clienContext.Load(oList)
        clienContext.Load(oItemsList)
        clienContext.ExecuteQueryRetry()
        Dim listFields As FieldCollection = oList.Fields

        Dim dtItems As New DataTable
        For c = 0 To FieldsList.Count - 1
            dtItems.Columns.Add(FieldsList(c)(0))
        Next

        For Each Item As ListItem In oItemsList
            dtItems.Rows.Add()
            For c = 0 To dtItems.Columns.Count - 1
                dtItems.Rows(dtItems.Rows.Count - 1)(c) = Item(FieldsList(c)(0))
            Next
        Next
        Return dtItems
    End Function

End Class
