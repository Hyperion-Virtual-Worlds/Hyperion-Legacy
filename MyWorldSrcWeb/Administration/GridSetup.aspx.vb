
Partial Class Administration_GridSetup
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
 '* This page is the core foundation GridSetup on which all other GridSetups are derived and a good 
 '* start for creating a new GridSetup that has not already been made or unique one of a kind page.
 '* 
 '* Built from Website Basic Page GridSetup v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private SQLCmd, SQLFields, SQLValues As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Validate logon and session existance.
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Response.Redirect("/Default.aspx")                      ' Return to logon page
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("/Default.aspx")
  End If
  If Session("Access") <> 9 Then                           ' SysAdmin Only access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("GridSetup", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page is called setup
   ' Define process unique objects here

   ' Define local objects here
   ' Setup general page controls
   ErrorMsg.Visible = False

   ' Check for Setup entries, if not exist create them.
   ' Populate display entries with values that exist.
   ' ** Welcome Region **
   Dim drControl As MySqlDataReader
   SQLCmd = "Select Name,Parm2 " +
            "From control " +
            "Where Control='GRIDSETUP' and Parm1='WelcomeRegion'"
   If Trace.IsEnabled Then Trace.Warn("GridSetup", "Get display Values SQLCmd: " + SQLCmd.ToString())
   drControl = MyDB.GetReader("MySite", SQLCmd)
   If drControl.HasRows() Then
    drControl.Read()
    Welcome.InnerText = drControl("Parm2").ToString().Trim()
   Else
    SQLFields = "Control,Name,Parm1,Parm2,Nbr1"
    SQLValues = "'GRIDSETUP','Welcome Region','WelcomeRegion','',1"
    SQLCmd = "Insert Into control (" + SQLFields + ") Values (" + SQLValues + ")"
    If Trace.IsEnabled Then Trace.Warn("GridSetup", "Insert control SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridSetup", "DB Error: " + MyDB.ErrMessage().ToString())
   End If
   ' ** Home Region **
   SQLCmd = "Select Name,Parm2 " +
            "From control " +
            "Where Control='GRIDSETUP' and Parm1='HomeRegion'"
   If Trace.IsEnabled Then Trace.Warn("GridSetup", "Get display Values SQLCmd: " + SQLCmd.ToString())
   drControl = MyDB.GetReader("MySite", SQLCmd)
   If drControl.HasRows() Then
    drControl.Read()
    Home.InnerText = drControl("Parm2").ToString().Trim()
   Else
    SQLFields = "Control,Name,Parm1,Parm2,Nbr1"
    SQLValues = "'GRIDSETUP','Home Region','HomeRegion','',2"
    SQLCmd = "Insert Into control (" + SQLFields + ") Values (" + SQLValues + ")"
    If Trace.IsEnabled Then Trace.Warn("GridSetup", "Insert control SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridSetup", "DB Error: " + MyDB.ErrMessage().ToString())
   End If
   ' ** Start Group **
   SQLCmd = "Select Name,Parm2 " +
            "From control " +
            "Where Control='GRIDSETUP' and Parm1='StartGroup'"
   If Trace.IsEnabled Then Trace.Warn("GridSetup", "Get display Values SQLCmd: " + SQLCmd.ToString())
   drControl = MyDB.GetReader("MySite", SQLCmd)
   If drControl.HasRows() Then
    drControl.Read()
    Group.InnerText = drControl("Parm2").ToString().Trim()
   Else
    SQLFields = "Control,Name,Parm1,Parm2,Nbr1"
    SQLValues = "'GRIDSETUP','Start Group','StartGroup','',3"
    SQLCmd = "Insert Into control (" + SQLFields + ") Values (" + SQLValues + ")"
    If Trace.IsEnabled Then Trace.Warn("GridSetup", "Insert control SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridSetup", "DB Error: " + MyDB.ErrMessage().ToString())
   End If
   ' ** World Promo Uploads **
   SQLCmd = "Select Nbr2 " +
            "From control " +
            "Where Control='GRIDSETUP' and Parm1='WorldPromo'"
   If Trace.IsEnabled Then Trace.Warn("GridSetup", "Get display Values SQLCmd: " + SQLCmd.ToString())
   drControl = MyDB.GetReader("MySite", SQLCmd)
   If drControl.HasRows() Then
    drControl.Read()
    WPUpload.Checked = (drControl("Nbr2") = 1)
   Else
    ' Default is not allowed to upload, option does not show.
    SQLFields = "Control,Name,Parm1,Parm2,Nbr1,Nbr2"
    SQLValues = "'GRIDSETUP','World Promo Uploads','WorldPromo','',0,0"
    SQLCmd = "Insert Into control (" + SQLFields + ") Values (" + SQLValues + ")"
    If Trace.IsEnabled Then Trace.Warn("GridSetup", "Insert control SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridSetup", "DB Error: " + MyDB.ErrMessage().ToString())
   End If
   drControl.Close()

   Dim RegionCount As MySqlDataReader
   SQLCmd = "Select Count(regionName) as Counter From regionxml"
   If Trace.IsEnabled Then Trace.Warn("GridSetup", "Get Region Count: " + SQLCmd.ToString())
   RegionCount = MyDB.GetReader("MySite", SQLCmd)
   RegionCount.Read()
   If RegionCount("Counter") = 0 Then
    ' Error: must create regions in Estate Management first!
    ShowItems.Visible = False
    ErrorMsg.Visible = True
    ErrorMsg.InnerHtml = "Welcome and Home Regions must be created in Estate Management first!"
   End If
   RegionCount.Close()

   ' Set up navigation options
   Dim SBMenu As New TreeView
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry")        ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry

   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   If Trace.IsEnabled Then Trace.Warn("GridSetup", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   SBMenu.Close()
  End If

  ' Get Display list Items here

 End Sub

 ' World Promo Check Changed
 Private Sub WPUpload_CheckedChanged(sender As Object, e As EventArgs) Handles WPUpload.CheckedChanged

  If WPUpload.Checked Then
   SQLCmd = "Update control Set Nbr2=1 " +
            "Where Control='GRIDSETUP' and Parm1='WorldPromo'"
  Else
   SQLCmd = "Update control Set Nbr2=0 " +
            "Where Control='GRIDSETUP' and Parm1='WorldPromo'"
  End If
  If Trace.IsEnabled Then Trace.Warn("GridSetup", "Update control SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridSetup", "DB Error: " + MyDB.ErrMessage().ToString())

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
