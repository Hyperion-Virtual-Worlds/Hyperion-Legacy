
Partial Class Administration_RegionConfig
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
 '* update records in a table. It may also be modified for any sort of data entry form process.
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
  If Session("Access") < 6 Then                            ' Grid Administrator or SysAdmin access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("RegionConfig", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page accessed setup
   ' Define process unique objects here

   ' Get identity key
   If Len(Request("KeyID")) > 0 Then                       ' Update or Add Mode
    KeyID.Value = Request("KeyID")
   Else                                                    ' Critical error, cancel operation
    Response.Redirect("GridManager.aspx")
   End If

   ' Setup general page controls
   Dim DBName As String
   DBName = "MyData"
   Dim Control As MySqlDataReader
   SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='MyDataDBName'"
   If Trace.IsEnabled Then Trace.Warn("RegionConfig", "Get Control DB Name SQLCmd: " + SQLCmd.ToString())
   Control = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("RegionConfig", "DB Error: " + MyDB.ErrMessage().ToString())
   If Control.HasRows() Then
    Control.Read()
    DBName = Control("Parm2").Trim().ToString()
   End If
   Control.Close()

   ' Fill Port selection based on server open ports. This will not contain any assigned ports.
   Dim drServers As MySqlDataReader
   SQLCmd = "Select port " +
            "From serverports " +
            "Where port not in " +
            " (Select port From regionxml " +
            "  Where UUID<>" + MyDB.SQLStr(KeyID.Value) + ")"
   If Trace.IsEnabled Then Trace.Warn("RegionConfig", "Get display Values SQLCmd: " + SQLCmd.ToString())
   drServers = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("RegionConfig", "DB Error: " + MyDB.ErrMessage().ToString())
   If Port.Items.Count > 0 Then Port.Items.Clear()
   Port.Items.Add(New ListItem("Set Port", ""))
   While drServers.Read()
    Port.Items.Add(New ListItem(drServers("port").ToString().Trim(), drServers("port").ToString().Trim()))
   End While
   drServers.Close()

   ' Display data fields based on edit or add mode
   Dim StatusImg As String
   StatusImg = "Offline"
   Dim drApp As MySqlDataReader
   SQLCmd = "Select regionName,status,locationX,locationY,port,internalIP,externalIP," +
            " Case When Length(Trim(externalIP))>0 " +
            " Then (Select ServerName From serverhosts Where externalIP=regionxml.externalIP) " +
            " Else 'No selection' " +
            " End as ServerName," +
            " (Select Concat(username,' ',lastname) as Name " +
            "  From " + DBName.ToString() + ".users Where UUID=regionxml.ownerUUID) as Owner " +
            "From regionxml " +
            "Where UUID=" + MyDB.SQLStr(KeyID.Value)
   If Trace.IsEnabled Then Trace.Warn("RegionConfig", "Get display Values SQLCmd: " + SQLCmd.ToString())
   drApp = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("RegionConfig", "DB Error: " + MyDB.ErrMessage().ToString())
   If drApp.Read() Then
    ExtIP.Value = drApp("externalIP").ToString()
    regionName.InnerText = drApp("regionName").ToString().Trim()
    ShowUUID.InnerText = KeyID.Value.ToString()
    LocationX.InnerText = drApp("locationX").ToString()
    LocationY.InnerText = drApp("locationY").ToString()
    ServerName.InnerText = drApp("ServerName").ToString() + ": " + drApp("externalIP").ToString() + ", " + drApp("internalIP").ToString()
    ' Add assigned port to list if its not zero and not in the list.
    If drApp("port") > 0 And Not Port.Items.Contains(New ListItem(drApp("port").ToString().Trim(), drApp("port").ToString().Trim())) Then
     Port.Items.Add(New ListItem(drApp("port").ToString().Trim(), drApp("port").ToString().Trim()))
    End If
    Port.SelectedValue = drApp("port").ToString()
    Port.Enabled = (drApp("status") = 0)
    Owner.InnerText = drApp("Owner").ToString().Trim()
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
   ' Setup Edit Mode page display controls
   PageTitle.InnerHtml = "Edit Region Settings - Status: <img src=""/Images/Icons/" + StatusImg.ToString() + ".png"" alt=""" + StatusImg.ToString() + """ style=""vertical-align: middle;""/>"
   drApp.Close()
   UpdDelBtn.Visible = True                                ' Allow Update and Delete button to show
   Port.Focus()                                            ' Set focus to the first field for entry

   Dim SBMenu As New TreeView
   ' Set up navigation options
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'RegionConfig.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
   SBMenu.AddItem("P", "GridManager.aspx", "Grid Manager")
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   If Trace.IsEnabled Then Trace.Warn("RegionConfig", "Show Menu")
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
  ' Process error checking as required, place messages in tMsg. Port.SelectedValue
  If Port.SelectedValue.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Port assignment!\r\n"
  Else
   If ExtIP.Value.ToString().Trim().Length > 0 Then
    Dim ChkRegion As MySqlDataReader
    SQLCmd = "Select regionName " +
             "From regionxml " +
             "Where externalIP=" + MyDB.SQLStr(ExtIP.Value) + " and port=" + MyDB.SQLNo(Port.SelectedValue)
    If Not tAdd Then                                        ' Edit mode verification
     SQLCmd = SQLCmd.ToString() + " and " +
              " UUID<>" + MyDB.SQLStr(KeyID.Value)
    End If
    If Trace.IsEnabled Then Trace.Warn("RegionConfig", "Validate Port SQLCmd: " + SQLCmd.ToString())
    ChkRegion = MyDB.GetReader("MySite", SQLCmd)
    If ChkRegion.HasRows() Then
     ChkRegion.Read()
     aMsg = aMsg.ToString() + "Port is in use by " + ChkRegion("regionName").ToString().Trim() + "!\r\n"
    End If
    ChkRegion.Close()
   End If
  End If
  'If FieldName.Text.ToString().Trim().Length = 0 Then
  ' aMsg = aMsg.ToString() + "Missing Field Name!\r\n"
  'End If
  Return aMsg
 End Function

 ' Update Button
 Private Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
  Dim tMsg As String
  tMsg = ValAddEdit(False)
  If tMsg.ToString().Trim().Length = 0 Then
   Dim GetServer As MySqlDataReader
   SQLCmd = "Select ServerName,externalIP,internalIP " +
            "From serverhosts " +
            "Where externalIP=(Select externalIP From serverports Where port=" + MyDB.SQLNo(Port.SelectedValue) + ")"
   If Trace.IsEnabled Then Trace.Warn("RegionConfig", "Get serverhost IPs SQLCmd: " + SQLCmd.ToString())
   GetServer = MyDB.GetReader("MySite", SQLCmd)
   GetServer.Read()
   ServerName.InnerText = GetServer("ServerName").ToString()
   ExtIP.Value = GetServer("externalIP").ToString()

   SQLCmd = "Update regionxml Set " +
            "externalIP=" + MyDB.SQLStr(GetServer("externalIP")) + "," + "internalIP=" + MyDB.SQLStr(GetServer("internalIP")) + "," +
            "port=" + MyDB.SQLNo(Port.SelectedValue) + " " +
            "Where UUID=" + MyDB.SQLStr(KeyID.Value)
   If Trace.IsEnabled Then Trace.Warn("RegionConfig", "Update regionxml SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   End If
   GetServer.Close()
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
