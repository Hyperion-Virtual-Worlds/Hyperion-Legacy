
Partial Class Administration_ErrorItems
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
 '* This page is an entry form page asscociated with records displayed by the Select page, to add or 
 '* update records in a table. It amy also be modified for any sort of data entry form process.
 '* Display Item information from region.log error report for objects.

 '* Built from MyWorld Form template v. 1.0

 ' Define common Page class properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private PageCtl As New GridLib
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Validate logon and session existance.
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Response.Redirect("/Default.aspx")                      ' Return to logon page
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("/Default.aspx")
  End If
  If Session("Access") < 6 Then                            ' Grid Administrator or SysAdmin access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("ErrorItems", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page accessed setup
   ' Define process unique objects here

   ' Setup general page controls
   ShowItem.Visible = False
   ShowMessage.Visible = False

   Dim SBMenu As New TreeView
   ' Set up navigation options
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'ErrorItems.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   If Trace.IsEnabled Then Trace.Warn("ErrorItems", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   ' Close Sidebar Menu object
   SBMenu.Close()
  End If

 End Sub

 Private Sub Display()

  ' Display data fields
  Dim drApp As MySqlDataReader
  SQLCmd = "Select A.name,A.description,B.Name,B.PositionX,B.PositionY,B.PositionZ,B.GroupPositionX,B.GroupPositionY,B.GroupPositionZ," +
           " B.RotationX,B.RotationY,B.RotationZ,B.RotationW," +
           " (Select Concat(username,' ',lastname) as Name from users Where UUID=B.OwnerID) as OwnerName," +
           " (Select regionName From Regions Where uuid=B.RegionUUID) as RegName " +
           "From primitems A inner join prims B on B.UUID=A.itemID " +
           "Where A.itemID=" + MyDB.SQLStr(ItemUUID.Text)
  If Trace.IsEnabled Then Trace.Warn("ErrorItems", "Get display Values SQLCmd: " + SQLCmd.ToString())
  drApp = MyDB.GetReader("MyData", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("ErrorItems", "DB Error: " + MyDB.ErrMessage().ToString())
  If drApp.Read() Then
   ItemName.InnerText = drApp("name").ToString().Trim() + " - " + drApp("description").ToString().Trim()
   PrimName.InnerText = drApp("Name").ToString().Trim()
   Location.InnerText = "<" + (drApp("GroupPositionX") - drApp("PositionX")).ToString() + "," +
                              (drApp("GroupPositionY") - drApp("PositionY")).ToString() + "," +
                              (drApp("GroupPositionZ") - drApp("PositionZ")).ToString() + "> * " +
                              "<" + drApp("RotationX").ToString() + "," + drApp("RotationY").ToString() + "," +
                              drApp("RotationZ").ToString() + "," + drApp("RotationW").ToString() + ">"
   IsLinkset.InnerText = (drApp("PositionX") <> 0 And drApp("PositionY") <> 0 And drApp("PositionZ") <> 0)
   ShowMessage.Visible = False
   ShowItem.Visible = True
  Else
   ShowMessage.Visible = True
   ShowItem.Visible = False
   Message.InnerText = "Item was not found."
  End If
  drApp.Close()
  ItemUUID.Focus()                                        ' Set focus to the first field for entry

 End Sub

 ' Process data validation checks
 Private Function ValAddEdit(ByVal tAdd As Boolean) As String
  ' Parameter tAdd allows selections of Add mode only testing
  Dim aMsg As String
  aMsg = ""
  ' Process error checking as required, place messages in tMsg.
  If ItemUUID.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Item UUID!\r\n"
  ElseIf Not ItemUUID.Text.ToString().Contains("-") Then
   aMsg = aMsg.ToString() + "Entry is not a UUID!\r\n"
  End If
  Return aMsg
 End Function

 ' Update Button
 Private Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
  Dim tMsg As String
  tMsg = ValAddEdit(False)

  If tMsg.ToString().Trim().Length = 0 Then
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
   Display()
  Else
   tMsg = "Entry Error:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")          ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                ' Display Alert Message
  End If

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub

End Class
