
Partial Class Administration_ContactSelect
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
 '* Contact Us Mananagement Entry Selection page.
 '* 
 '* 

 '* Built from MyWorld Select template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private PageCtl As New GridLib
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Validate logon and session existance.
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Response.Redirect("/Default.aspx")                       ' Return to logon page
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("/Default.aspx")
  End If
  If Session("Access") < 2 Then                             ' Webadmin or SysAdmin access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("ContactSelect", "Start Page Load")

  If Not IsPostBack Then                                    ' First time page is called setup
   ' Define process unique objects here

   ' Define local objects here
   If Session("FirstTime") = "" Then
    Session("FirstTime") = "No"
    Session("SelPage") = 0                                  ' Value is updated in the Page index change event
   End If

   ' Setup general page controls

   ' Set up navigation options
   ' Sidebar Options control based on Clearance or Write Access
   Dim SBMenu As New TreeView
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                  ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                   ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Other Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")      ' Program URL link entry
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   SBMenu.AddItem("B", "", "Blank Entry")
   SBMenu.AddItem("T", "", "Page Options")
   SBMenu.AddItem("L", "CallEdit(0,'ContactForm.aspx');", "New Entry")
   If Trace.IsEnabled Then Trace.Warn("ContactSelect", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   SBMenu.Close()

   Display()
  End If

 End Sub

 Protected Sub Display()
  If Trace.IsEnabled Then Trace.Warn("ContactSelect", "Display() Called")

  ' Get Display list Items here
  Dim DisplayList As MySqlDataReader
  SQLCmd = "Select ContactID,Title,Active " +
           "From contactus " +
           "Order by SortOrder"
  If Trace.IsEnabled Then Trace.Warn("ContactSelect", "Get email list SQLCmd: " + SQLCmd.ToString())
  ' SqlDataSource1 is a screen data object placed on the page.
  DisplayList = MyDB.GetReader("MySite", SQLCmd)
  gvDisplay.DataSource = DisplayList
  gvDisplay.DataBind()

 End Sub

 Public Function ShowActive(ByVal aActive As Boolean) As String
  Dim tOut As String
  tOut = ""
  If aActive Then
   tOut = "<span class=""Errors"">*</span>"
  End If
  Return tOut
 End Function

 ' Contact Entry Sort Order processing
 Protected Sub KeyID_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles KeyID.TextChanged

  Dim tMsg As String
  tMsg = ""
  If Trace.IsEnabled Then Trace.Warn("ContactSelect", "Process entry relocation")
  ' Check if entry still exist
  Dim NewSort As Double
  NewSort = 0
  Dim drResort As MySqlDataReader
  SQLCmd = "Select SortOrder " +
           "From contactus " +
           "Where ContactID=" + MyDB.SQLNo(KeyID.Text)
  If Trace.IsEnabled Then Trace.Warn("ContactSelect", "Get Get Current SortOrder SQLCmd: " + SQLCmd.ToString())
  drResort = MyDB.GetReader("MySite", SQLCmd)
  If drResort.Read() Then                                   ' Process resort command
   If SortAction.Value.ToString().Trim() = "T" Then         ' Move to top
    NewSort = 0.5
   ElseIf SortAction.Value.ToString().Trim() = "U" Then     ' Move up
    NewSort = drResort("SortOrder") - 1.5
   ElseIf SortAction.Value.ToString().Trim() = "D" Then     ' Move down
    NewSort = drResort("SortOrder") + 1.5
   ElseIf SortAction.Value.ToString().Trim() = "B" Then     ' Move to bottom
    Dim drLast As MySqlDataReader
    SQLCmd = "Select Top 1 SortOrder From contactus " +
             "Order by SortOrder DESC"
    If Trace.IsEnabled Then Trace.Warn("ContactSelect", "Get Last in list SQLCmd: " + SQLCmd.ToString())
    drLast = MyDB.GetReader("MySite", SQLCmd)
    drLast.Read()
    NewSort = drLast("SortOrder") + 0.5
    drLast.Close()
   End If
   ' Update entry
   SQLCmd = "Update contactus Set SortOrder=" + MyDB.SQLNo(NewSort) + " " +
            "Where ContactID=" + MyDB.SQLNo(KeyID.Text)
   If Trace.IsEnabled Then Trace.Warn("ContactSelect", "Update to new SortOrder SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)
   If MyDB.Error Then
    tMsg = "Failed to update entry! DB Error: " + MyDB.ErrMessage.ToString()
   Else
    ' Resort entries
    'NOTE: MySQL 4.* cannot process single command update from a select.
    SQLCmd = "Set @Newsort=0;" +
             "Update contactus Set SortOrder=B.NewSort " +
             "From " +
             " (Select ContactID,@Newsort := @Newsort+1 as NewSort " +
             "  From contactus) as B INNER JOIN " +
             "   contactus ON B.ContactID = contactus.ContactID"
    'If Trace.IsEnabled Then Trace.Warn("ContactSelect", "Get Resort portal entries SQLCmd: " + SQLCmd.ToString())
    'MyDB.DBCmd("MySite", SQLCmd)
    'If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("ContactSelect", "DB Error: " + MyDB.ErrMessage().ToString())

    ' Reverting to the old, slow and working process
    NewSort = 0
    Dim GetPRows As MySqlDataReader
    SQLCmd = "Select ContactID " +
             "From contactus " +
             "Order by SortOrder"
    If Trace.IsEnabled Then Trace.Warn("ContactSelect", "Get Page records SQLCmd: " + SQLCmd.ToString())
    GetPRows = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("ContactSelect", "DB Error: " + MyDB.ErrMessage().ToString())
    If GetPRows.HasRows() Then
     While GetPRows.Read()
      NewSort = NewSort + 1
      SQLCmd = "Update contactus Set SortOrder=" + MyDB.SQLNo(NewSort) + " " +
               "Where ContactID=" + MyDB.SQLNo(GetPRows("ContactID"))
      If Trace.IsEnabled Then Trace.Warn("ContactSelect", "Resort Entry SQLCmd: " + SQLCmd.ToString())
      MyDB.DBCmd("MySite", SQLCmd)
      If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("ContactSelect", "DB Error: " + MyDB.ErrMessage().ToString())
     End While
    End If
    GetPRows.Close()
   End If
  End If
  drResort.Close()
  KeyID.Text = ""                                           ' Clear value to allow trigger to work again

  If tMsg.ToString.Trim.Length > 0 Then
   BodyTag.Attributes.Add("onload", "ShowMsg();")           ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                 ' Display Alert Message
  End If
  Display()

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  SqlClient.SqlConnection.ClearAllPools()      ' Causes all pooled connection to be forced closed when finished, reducing pooled connection size.
  PageCtl = Nothing
 End Sub

End Class
