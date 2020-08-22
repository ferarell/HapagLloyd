Imports Microsoft.SharePoint.Client
Public Class FormSPList
    Private Sub FormSPList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Width = Screen.PrimaryScreen.Bounds.Width - 50
        Height = Screen.PrimaryScreen.Bounds.Height - 100
        CenterToScreen()

        Dim path As String = "http://intranet"
        Dim clientContext = New ClientContext(path)
        Dim oWebsite = clientContext.Web
        Dim result As WebCollection = oWebsite.GetSubwebsForCurrentUser(New SubwebQuery())
        clientContext.Load(result, Function(n) n.Include(Function(o) o.Title, Function(o) o.ServerRelativeUrl))
        clientContext.ExecuteQuery()

        For Each orWebsite In result
            ComboBoxSites.Items.Add(path & orWebsite.ServerRelativeUrl)
        Next
    End Sub

    Private Sub ComboBoxSites_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxSites.SelectedIndexChanged
        ComboBoxLists.Items.Clear()

        Dim context As ClientContext = New ClientContext(ComboBoxSites.Text)
        Dim web As Web = context.Web
        context.Load(web.Lists, Function(lists) lists.Include(Function(list) list.Title, Function(list) list.Id))
        context.ExecuteQuery()

        For Each list As List In web.Lists
            ComboBoxLists.Items.Add(list.Title)
        Next

    End Sub

    Private Sub ComboBoxLists_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxLists.SelectedIndexChanged
        Dim Dtable As New DataTable

        Dim context As ClientContext = New ClientContext(ComboBoxSites.Text)
        Dim web As Web = context.Web
        Dim list = web.Lists.GetByTitle(ComboBoxLists.Text)
        context.Load(list.Fields)
        Dim query As CamlQuery = CamlQuery.CreateAllItemsQuery()
        Dim AllItems As ListItemCollection = list.GetItems(query)
        context.Load(AllItems)
        context.ExecuteQuery()

        For Each f As Field In list.Fields
            If Not f.ReadOnlyField Then
                Dtable.Columns.Add(f.InternalName, GetType(System.String))
            End If
        Next

        For Each RowItem As ListItem In AllItems
            If RowItem IsNot Nothing Then
                Dim dr As DataRow = Dtable.NewRow
                For Each ColName As DataColumn In Dtable.Columns
                    If RowItem.FieldValues.ContainsKey(ColName.ColumnName) Then
                        If RowItem.FieldValues(ColName.ColumnName) IsNot Nothing Then
                            dr(ColName.ColumnName) = RowItem.FieldValues(ColName.ColumnName).ToString

                        End If
                    End If
                Next
                Dtable.Rows.Add(dr)
            End If

        Next

        DGVList.DataSource = Dtable
        For Each DgvCol As DataGridViewColumn In DGVList.Columns
            DgvCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
        Next
    End Sub
End Class