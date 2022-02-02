
Partial Class Administration_UserSelect
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
 '* UserSelect shows the list of world accounts and access to change management settings on them.
 '* 
 '* 

 '* Built from MyWorld Select template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                                 ' Provides data access methods and error handling
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Validate logon and session existance.
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Response.Redirect("/Default.aspx")                         ' Return to logon page
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("/Default.aspx")
  End If
  If Session("Access") < 2 Then                               ' Webmaster and above access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("UserSelect", "Start Page Load")

  If Not IsPostBack Then                                      ' First time page is called setup
   ' Define process unique objects here

   ' Define local objects here
   If Session("FirstTime") <> "UserSelect" Then
    Session("FirstTime") = "UserSelect"
    Session("SelPage") = 0                                    ' Value is updated in the Page index change event
    Session("Search") = ""
    Session("SetSort") = "lastname,username"
   End If

   ' Setup general page controls

   ' Set up navigation options
   ' Sidebar Options control based on Clearance or Write Access
   Dim SBMenu As New TreeView
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                    ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                     ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Other Options")                   ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")        ' Program URL link entry
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   SBMenu.AddItem("B", "", "Blank Entry")
   SBMenu.AddItem("T", "", "Page Options")
   If Trace.IsEnabled Then Trace.Warn("UserSelect", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   SBMenu.Close()

   ' Gridview settings
   gvDisplay.PageIndex = Session("SelPage")
   gvDisplay.PageSize = 40

   Display()
  End If

 End Sub

 Private Sub Display()
  If Trace.IsEnabled Then Trace.Warn("UserSelect", "Display() Called")
  Dim tSearch As String
  tSearch = ""

  ' Set up dependent elements
  Search.Value = Session("Search")
  If Session("Search").ToString().Trim().Length > 0 Then
   tSearch = "and (username like " + MyDB.SQLLike(Session("Search")) + " or lastname like " + MyDB.SQLLike(Session("Search")) + ") "
  End If
  ShowDisabled.Checked = DisAccounts.Checked

  Dim tList As String
  tList = ""
  ' Get list of Grid accounts to exclude 
  Dim GridAccts As MySqlDataReader
  SQLCmd = "Select Parm2 From control " +
           "Where (Control='GridSysAccounts' or (Control='ECONOMY' and Parm1='WorldBankAcct')) " +
           "Order by Name"
  If Trace.IsEnabled Then Trace.Warn("UserSelect", "Get Grid Accounts List SQLCmd: " + SQLCmd.ToString())
  GridAccts = MyDB.GetReader("MySite", SQLCmd)
  If GridAccts.HasRows() Then
   While GridAccts.Read()
    If tList.ToString().Trim().Length > 0 Then
     tList = tList.ToString() + ","
    End If
    tList = tList.ToString() + MyDB.SQLStr(GridAccts("Parm2").ToString().Trim())
   End While
   tList = "Concat(username, ' ',lastname) not in (" + tList.ToString() + ") "
  End If

  ' Get Display list Items here
  Dim Accounts As MySqlDataReader
  SQLCmd = "Select UUID,username,lastname,email,created," +
           " Case When RTrim(email)='' " +
           " Then 'Reference' " +
           " When RTrim(email)<>'' and email=passwordSalt " +
           " Then 'Blocked' " +
           " Else '' " +
           " End as Access," +
           " (Select " +
           "   Case When agentOnline<>0 " +
           "   Then agentOnline " +
           "   Else " +
           "    Case When logoutTime=0 Then 0 Else logoutTime End " +
           "   End as Status " +
           "  From agents Where UUID=users.UUID) as Status " +
           "From users " +
           "Where " +
           tList.ToString() + IIf(tList.Length = 0, "", "and ") +
           tSearch.ToString() + IIf(tSearch.Length = 0, "", "and ") +
           IIf(DisAccounts.Checked, "passwordHash='NoPass' ", " passwordHash<>'NoPass' ") +
           "Order by " + Session("SetSort")
  Session("PriorSort") = Session("SetSort")
  If Trace.IsEnabled Then Trace.Warn("UserSelect", "Get SQLCmd: " + SQLCmd.ToString())
  Accounts = MyDB.GetReader("MyData", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then
   Trace.Warn("UserSelect", "DB Error: " + MyDB.ErrMessage().ToString())
  End If
  gvDisplay.DataSource = Accounts
  gvDisplay.DataBind()
  Accounts.Close()

 End Sub

 Protected Sub gvDisplay_PageIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvDisplay.PageIndexChanged
  ShowPages.InnerText = "Page " + (gvDisplay.PageIndex + 1).ToString() + " of " + gvDisplay.PageCount.ToString()
  Session("SelPage") = gvDisplay.PageIndex                    ' Remember current page displayed.
 End Sub

 ' Set Display Sort
 Private Sub SetSort_TextChanged(sender As Object, e As EventArgs) Handles SetSort.TextChanged
  If SetSort.Text.ToString().Trim() = "Name" Then
   Session("SetSort") = "lastname,username"
  ElseIf SetSort.Text.ToString().Trim() = "Email" Then
   If Session("SetSort").ToString() <> Session("PriorSort").ToString() Then
    Session("SetSort") = "email desc"
   Else
    If Session("SetSort").ToString().Contains(" asc") Then
     Session("SetSort") = "email desc"
    Else
     Session("SetSort") = "email asc"
    End If
   End If
  ElseIf SetSort.Text.ToString().Trim() = "Created" Then
   If Session("SetSort").ToString() <> Session("PriorSort").ToString() Then
    Session("SetSort") = "created desc"
   Else
    If Session("SetSort").ToString().Contains(" asc") Then
     Session("SetSort") = "created desc"
    Else
     Session("SetSort") = "created asc"
    End If
   End If
  ElseIf SetSort.Text.ToString().Trim() = "Last" Then
   If Session("SetSort").ToString() <> Session("PriorSort").ToString() Then
    Session("SetSort") = "lastLogin desc"
   Else
    If Session("SetSort").ToString().Contains(" asc") Then
     Session("SetSort") = "lastLogin desc"
    Else
     Session("SetSort") = "lastLogin asc"
    End If
   End If
  Else
   Session("SetSort") = "lastname,username"
  End If
  SetSort.Text = ""                                           ' Clear value so it may be used again
  Display()
 End Sub

 Public Function ShowStatus(ByVal aStatus As Integer) As String
  Dim tVal As String
  tVal = ""
  If aStatus = 0 Then                                         ' Has not logged in yet
   tVal = "Not logged in"
  Else
   If aStatus.ToString().Length > 4 Then                      ' Date of last logon or account created date as UnixTimeStamp
    tVal = FormatDateTime(DateAdd(DateInterval.Second, aStatus, Date.Parse("1/1/1970 00:00:00")), DateFormat.ShortDate)
    'tVal = FormatDateTime(aStatus, DateFormat.GeneralDate)
   Else                                                       ' Is currently online
    tVal = "<b>Online</b>"
   End If
  End If
  Return tVal.ToString()
 End Function

 'Search for Name
 Private Sub FindName_TextChanged(sender As Object, e As EventArgs) Handles FindName.TextChanged
  Session("Search") = FindName.Text.ToString().Trim()
  Display()
  'FindName.Text = ""                                         ' Allow process to be called again
 End Sub

 Private Sub DisAccounts_CheckedChanged(sender As Object, e As EventArgs) Handles DisAccounts.CheckedChanged
  Display()
 End Sub

 Public Function ShowUTF8(ByVal textIn As String) As String
  Return MyDB.Iso8859ToUTF8(textIn)
 End Function

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  SqlClient.SqlConnection.ClearAllPools()      ' Causes all pooled connection to be forced closed when finished, reducing pooled connection size.
 End Sub
End Class
