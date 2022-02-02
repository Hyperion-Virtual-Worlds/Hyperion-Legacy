
Partial Class RegionForm
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
 '* This page is the entry form for user region update.
 '* 

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

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("RegionForm", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page accessed setup
   ' Define process unique objects here

   ' Get identity key
   If Len(Request("KeyID")) > 0 Then                       ' Update or Add Mode
    KeyID.Value = Request("KeyID")
   Else                                                    ' Critical error, cancel operation
    Response.Redirect("EstateForm.aspx")
   End If

   ' Setup general page controls
   Dim StatusImg As String
   StatusImg = "Offline"

   ' Display data fields
   Dim GetObjects As MySqlDataReader
   SQLCmd = "Select Count(UUID) as Objects From prims Where RegionUUID=" + MyDB.SQLStr(KeyID.Value)
   If Trace.IsEnabled Then Trace.Warn("RegionForm", "Get Object count SQLCmd: " + SQLCmd.ToString())
   GetObjects = MyDB.GetReader("MyData", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("RegionForm", "DB Error: " + MyDB.ErrMessage().ToString())
   GetObjects.Read()
   Objects.InnerText = GetObjects("Objects")
   GetObjects.Close()

   Dim drApp As MySqlDataReader
   SQLCmd = "Select regionName,status,locationX,locationY,ownerUUID,primMax,productType,CreateDate " +
            "From regionxml " +
            "Where UUID=" + MyDB.SQLStr(KeyID.Value)
   If Trace.IsEnabled Then Trace.Warn("RegionForm", "Get display Values SQLCmd: " + SQLCmd.ToString())
   drApp = MyDB.GetReader("MySite", SQLCmd)
   If drApp.Read() Then
    RegionName.Text = drApp("regionName").ToString().Trim()
    ShowUUID.InnerText = KeyID.Value
    MadeDate.InnerText = FormatDateTime(drApp("CreateDate"), DateFormat.ShortDate)
    LocationX.InnerText = drApp("locationX").ToString()
    LocationY.InnerText = drApp("locationY").ToString()
    PrimMax.InnerText = drApp("primMax").ToString()
    If drApp("productType") = 1 Then
     RegType.InnerText = "Full region"
    ElseIf drApp("productType") = 2 Then
     RegType.InnerText = "Landscape"
    Else
     RegType.InnerText = "Homestead"
    End If
    If drApp("status") = 0 Then
     StatusImg = "Offline"
    ElseIf drApp("status") = 1 Then
     StatusImg = "Starting"
    ElseIf drApp("status") = 2 Then
     StatusImg = "Online"
    Else
     StatusImg = "Closing"
    End If
   End If
   drApp.Close()
   ' Setup Edit Mode page display controls
   PageTitle.InnerHtml = "Edit Region Settings - Status: <img src=""/Images/Icons/" + StatusImg.ToString() + ".png"" alt=""" + StatusImg.ToString() + """ style=""vertical-align: middle;""/>"
   UpdDelBtn.Visible = True                                ' Allow Update to show
   RegionName.Focus()                                      ' Set focus to the first field for entry

   Dim SBMenu As New TreeView
   ' Set up navigation options
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
   SBMenu.AddItem("L", "CallEdit(" + Session("EstateID").ToString() + ",'EstateForm.aspx');", "Estate Edit")
   SBMenu.AddItem("P", "Estate.aspx", "Estate Management")
   SBMenu.AddItem("P", "Account.aspx", "Account")
   SBMenu.AddItem("P", "Logout.aspx", "Logout")
   If Session("Access") > 1 Then                            ' Access only for Web Admin or System Admin
    SBMenu.AddItem("B", "", "Blank Entry")
    SBMenu.AddItem("P", "/Administration/Admin.aspx", "Website Administration")
   End If
   If Trace.IsEnabled Then Trace.Warn("RegionForm", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   ' Close Sidebar Menu object
   SBMenu.Close()
  End If

 End Sub

 ' Process data validation checks
 Private Function ValAddEdit(ByVal tAdd As Boolean) As String
  ' Parameter tAdd allows selections of Add mode only testing
  Dim aMsg As String
  aMsg = ""
  ' Process error checking as required, place messages in tMsg.
  If RegionName.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Region Name!\r\n"
  Else
   Dim ChkRegion As MySqlDataReader
   SQLCmd = "Select locationX,LocationY " +
            "From regionxml " +
            "Where regionName=" + MyDB.SQLStr(RegionName.Text)
   If Not tAdd Then                                       ' Edit mode verification
    SQLCmd = SQLCmd.ToString() + " and " +
            " UUID<>" + MyDB.SQLStr(KeyID.Value)
   End If
   If Trace.IsEnabled Then Trace.Warn("RegionForm", "Validate Name SQLCmd: " + SQLCmd.ToString())
   ChkRegion = MyDB.GetReader("MySite", SQLCmd)
   If ChkRegion.HasRows() Then
    ChkRegion.Read()
    aMsg = aMsg.ToString() + "Region name is in use at " + ChkRegion("locationX").ToString().Trim() + "," + ChkRegion("locationY").ToString().Trim() + "!\r\n"
   End If
  End If
  Return aMsg
 End Function

 ' Update Button
 Private Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
  Dim tMsg As String
  tMsg = ValAddEdit(False)

  If tMsg.ToString().Trim().Length = 0 Then
   SQLCmd = "Update regionxml Set " +
            "regionName=" + MyDB.SQLStr(RegionName.Text) + " " +
            "Where UUID=" + MyDB.SQLStr(KeyID.Value)
   If Trace.IsEnabled Then Trace.Warn("RegionForm", "Update regionxml SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   End If
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot update Entry:\r\n" + tMsg
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
