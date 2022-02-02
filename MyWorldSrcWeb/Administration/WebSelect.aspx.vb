
Partial Class Administration_WebSelect
 Inherits System.Web.UI.Page

 '*************************************************************************************************
 '* Open Source Project Notice:
 '* The "MyWorld" website is a community supported open source project intended for use with the 
 '* Halcyon Simulator project posted at https://github.com/HalcyonGrid and compatible derivatives of 
 '* that work. 
 '* Contributions to the MyWorld website project are to be original works contributed by the authors
 '* or other open source projects. Only the works that are directly contributed to this project are
 '* considered to be part of the project, included in it as community open source content. This does 
 '* not include separate projects or sources used and owned by the respective contributors that may 
 '* contain similar code used in their other works. Each contribution to the MyWorld project is to 
 '* include in a header like this what its sources and contributor are and any applicable exclusions 
 '* from this project. 
 '* The MyWorld website is released as public domain content is intended for Halcyon Simulator 
 '* virtual world support. It is provided as is and for use in customizing a website access and 
 '* support for the intended application and may not be suitable for any other use. Any derivatives 
 '* of this project may not reverse claim exclusive rights or profiting from this work. 
 '*************************************************************************************************
 '* Website Content Selection page. Provides a list of all the pages and the display contents in them. 
 '* Each page's content may be sort arranged here.
 '* 

 '* Built from MyWorld Select template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Validate logon and session existance.
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Response.Redirect("/Default.aspx")                       ' Return to logon page
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("/Default.aspx")
  End If
  If Session("Access") < 2 Then                             ' Webmaster and above access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("WebSelect", "Start Page Load")

  If Not IsPostBack Then                                    ' First time page is called setup
   ' Define process unique objects here

   ' Define local objects here
   If Session("FirstTime") = "" Then
    Session("FirstTime") = "No"
    Session("PageID") = 0
    Session("SelPage") = 0                                  ' Value is updated in the Page index change event
   End If

   ' Setup general page controls

   ' Get dropbox list of pages assigned to user, Admin sees all pages
   Dim rsPages As MySqlDataReader
   SQLCmd = "Select PageID,Name " +
            "From pagemaster " +
            "Order by Name"
   If Trace.IsEnabled Then Trace.Warn("WebSelect", "Get Page List SQLCmd: " + SQLCmd.ToString())
   rsPages = MyDB.GetReader("MySite", SQLCmd)
   SelPageID.Items.Add(New WebControls.ListItem("Select a Page", "0"))
   While rsPages.Read()
    If Trace.IsEnabled Then Trace.Warn("WebSelect", "Loading: " + rsPages("Name").ToString().Trim() + ", " + rsPages("PageID").ToString().Trim())
    SelPageID.Items.Add(New WebControls.ListItem(rsPages("Name").ToString().Trim(), rsPages("PageID").ToString().Trim()))
   End While
   rsPages.Close()
   SelPageID.SelectedIndex = 0

   Display()
  End If

 End Sub

 Private Sub Display()
  Dim SBMenu As New TreeView
  ' Sidebar Options control based on Clearance or Write Access
  SBMenu.SetTrace = Trace.IsEnabled
  'SBMenu.AddItem("M", "3", "Report List")                   ' Sub Menu entry requires number of expected entries following to contain in it
  'SBMenu.AddItem("B", "", "Blank Entry")                    ' Blank Line as item separator
  'SBMenu.AddItem("T", "", "Other Options")                  ' Title entry
  'SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry")        ' Javascript activated entry
  'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")       ' Program URL link entry
  SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
  SBMenu.AddItem("P", "/Account.aspx", "Account")
  SBMenu.AddItem("P", "/Logout.aspx", "Logout")
  SBMenu.AddItem("B", "", "Blank Entry")
  If Session("PageID") > 0 Then
   SBMenu.AddItem("T", "", "Page Options")
   SBMenu.AddItem("L", "CallEdit(0,'WebForm.aspx');", "New Entry")
  End If
  If Trace.IsEnabled Then Trace.Warn("WebSelect", "Show Menu")
  SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
  SBMenu.Close()

  If Session("PageID") > 0 Then
   SelPageID.Value = Session("PageID")
  End If

  ' Get Display list Items here
  Dim Display As MySqlDataReader
  SQLCmd = "Select EntryID,Name,Active " +
           "From pagedetail " +
           "Where PageID=" + MyDB.SQLNo(Session("PageID")) + " " +
           "Order by SortOrder"
  If Trace.IsEnabled Then Trace.Warn("WebSelect", "Get SQLCmd: " + SQLCmd.ToString())
  Display = MyDB.GetReader("MySite", SQLCmd)
  gvDisplay.DataSource = Display
  gvDisplay.DataBind()
  Display.Close()

 End Sub

 ' Resort Action triggered
 Protected Sub EID_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles EID.TextChanged
  If Trace.IsEnabled Then Trace.Warn("WebSelect", "EID Text Changed")
  ' Process sort order move selection
  If Trace.IsEnabled Then Trace.Warn("WebSelect", "Process entry relocation")
  Dim tNewOrd As Double
  ' Check to see if current selection still exists...
  Dim drEntry As MySqlDataReader
  SQLCmd = "Select SortOrder From pagedetail " +
           "Where EntryID=" + MyDB.SQLNo(EID.Text)
  If Trace.IsEnabled Then Trace.Warn("WebSelect", "Current record SQLCmd: " + SQLCmd.ToString())
  drEntry = MyDB.GetReader("MySite", SQLCmd)
  If drEntry.Read() Then                                   ' Current record still exists,
   If Action.Text.ToString() = "T" Then                    ' Process Move to Top
    If Trace.IsEnabled Then Trace.Warn("WebSelect", "Move to Top")
    tNewOrd = 0.1
   ElseIf Action.Text.ToString() = "U" Then                ' Process Move Up
    If Trace.IsEnabled Then Trace.Warn("WebSelect", "Move Up from " + CDbl(drEntry("SortOrder")).ToString())
    tNewOrd = CDbl(drEntry("SortOrder")) - 1.9
   ElseIf Action.Text.ToString() = "D" Then                ' Process Move Down
    If Trace.IsEnabled Then Trace.Warn("WebSelect", "Move Down from " + CDbl(drEntry("SortOrder")).ToString())
    tNewOrd = CDbl(drEntry("SortOrder")) + 1.1
   ElseIf Action.Text.ToString() = "B" Then                ' Process Move to Bottom
    If Trace.IsEnabled Then Trace.Warn("WebSelect", "Move to Bottom")
    tNewOrd = 9999.9
   End If
   ' Update record with new sort order
   SQLCmd = "Update pagedetail Set SortOrder=" + MyDB.SQLNo(tNewOrd) + " " +
            "Where EntryID=" + MyDB.SQLNo(EID.Text)
   If Trace.IsEnabled Then Trace.Warn("WebSelect", "Update Entry SortOrder SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)
  End If
  drEntry.Close()
  EID.Text = ""                                            ' Clear for next input
  ResortMenu()
  Display()

 End Sub

 ' Common Menu resort process
 Private Sub ResortMenu()
  'NOTE: MySQL 4.* cannot process single command update from a select.
  SQLCmd = "Set @Newsort=0;" +
           "Update pagedetail Set SortOrder=B.NewSort " +
           "From " +
           "(Select EntryID,@Newsort := @Newsort+1 as NewSort " +
           " From pagedetail " +
           " Where PageID=" + MyDB.SQLNo(Session("PageID")) + ") as B INNER JOIN " +
           "pagedetail ON pagedetail.EntryID = B.EntryID "
  'If Trace.IsEnabled Then Trace.Warn("WebSelect", "Resort Entry SQLCmd: " + SQLCmd.ToString())
  'MyDB.DBCmd("MySite", SQLCmd)
  'If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("WebSelect", "DB Error: " + MyDB.ErrMessage().ToString())

  ' Reverting to the old, slow and working process
  Dim NewSort As Integer
  NewSort = 0
  Dim GetPRows As MySqlDataReader
  SQLCmd = "Select EntryID " +
           "From pagedetail " +
           "Where PageID=" + MyDB.SQLNo(Session("PageID")) + " " +
           "Order by SortOrder"
  If Trace.IsEnabled Then Trace.Warn("WebSelect", "Get Page records SQLCmd: " + SQLCmd.ToString())
  GetPRows = MyDB.GetReader("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("WebSelect", "DB Error: " + MyDB.ErrMessage().ToString())
  If GetPRows.HasRows() Then
   While GetPRows.Read()
    NewSort = NewSort + 1
    SQLCmd = "Update pagedetail Set SortOrder=" + MyDB.SQLNo(NewSort) + " " +
             "Where EntryID=" + MyDB.SQLNo(GetPRows("EntryID"))
    If Trace.IsEnabled Then Trace.Warn("WebSelect", "Resort Entry SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("WebSelect", "DB Error: " + MyDB.ErrMessage().ToString())
   End While
  End If
  GetPRows.Close()
 End Sub

 Public Function ShowUTF8(ByVal textIn As String) As String
  Return MyDB.Iso8859ToUTF8(textIn)
 End Function

 Protected Sub PageID_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles PageID.TextChanged
  ' Process event action
  If Trace.IsEnabled Then Trace.Warn("WebSelect", "Changed Page Selection")
  SelPageID.Value = PageID.Text.ToString()                  ' Reset active selection in drop box
  Session("PageID") = CInt(PageID.Text)
  Display()
 End Sub

 Public Function SetActive(ByVal aActive As String) As String
  Dim tOut As String = ""
  If aActive = "true" Then
   tOut = "<span class=""Errors"">*</span>"
  End If
  Return tOut
 End Function

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  SqlClient.SqlConnection.ClearAllPools()      ' Causes all pooled connection to be forced closed when finished, reducing pooled connection size.
 End Sub

End Class
