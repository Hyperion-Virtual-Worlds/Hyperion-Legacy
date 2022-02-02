
Partial Class Administration_Admin
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
 '* Administration access to website management tools. Options controlled by menu entries by session 
 '* access levels.
 '* 

 '* Built from MyWorld Basic Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Validate logon and session existance.
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Response.Redirect("/Default.aspx")                      ' Return to logon page
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("/Default.aspx")
  End If
  If Session("Access") < 2 Then                            ' Webadmin or SysAdmin access
   Response.Redirect("/Default.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("Admin", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page is called setup
   ' Define process unique objects here

   ' Define local objects here
   ' Setup general page controls
   Session("FirstTime") = ""                               ' Clear / Set session control flag for Admin pages.

   ' Set up navigation options
   Dim SBMenu As New TreeView
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry")        ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   SBMenu.AddItem("B", "", "Blank Entry")
   SBMenu.AddItem("T", "", "Admin Options")
   SBMenu.AddItem("P", "WebSelect.aspx", "Webpage Management")
   Dim drControl As MySqlDataReader
   ' ** World Promo Uploads **
   SQLCmd = "Select Nbr2 From control Where Control='GRIDSETUP' and Parm1='WorldPromo'"
   If Trace.IsEnabled Then Trace.Warn("Admin", "Get display Values SQLCmd: " + SQLCmd.ToString())
   drControl = MyDB.GetReader("MySite", SQLCmd)
   If drControl.HasRows() Then
    drControl.Read()
    If drControl("Nbr2") = 1 Then
     SBMenu.AddItem("P", "WorldPromoMgt.aspx", "World Promo Management")
    End If
   End If
   drControl.Close()
   If Session("Access") > 5 Then                           ' Grid Administrator or Grid Manager
    SBMenu.AddItem("P", "GridManager.aspx", "Grid Manager")
    SBMenu.AddItem("P", "OarDBCln.aspx", "Oar DB Cleanup")
   End If
   If Session("Access") = 9 Then                           ' Grid owner access only
    SBMenu.AddItem("P", "ContactSelect.aspx", "Contact Management")
    SBMenu.AddItem("P", "ControlSelect.aspx", "Control Value Management")
    SBMenu.AddItem("P", "ListSelect.aspx", "Control List Management")
    SBMenu.AddItem("P", "Economy.aspx", "Economy Management")
   End If
   If Session("Access") > 5 Then                           ' Grid owner or Grid Manager
    SBMenu.AddItem("P", "EstateSelect.aspx", "Estate Management")
   End If
   If Session("Access") = 9 Then                           ' Grid owner access only
    SBMenu.AddItem("P", "GridSetup.aspx", "World Setup")
    SBMenu.AddItem("P", "MMSetup.aspx", "Mundos Market Setup")
    SBMenu.AddItem("P", "SaveConfig.aspx", "Download Config")
    SBMenu.AddItem("P", "LoadConfig.aspx", "Load Config")
   End If
   SBMenu.AddItem("B", "", "Blank Entry")
   SBMenu.AddItem("P", "UserSelect.aspx", "User Management")
   SBMenu.AddItem("P", "ErrorItems.aspx", "Error Item Search")
   If Trace.IsEnabled Then Trace.Warn("Admin", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   SBMenu.Close()
  End If

  ' Get Display list Items here

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
