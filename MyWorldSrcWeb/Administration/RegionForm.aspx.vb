
Partial Class Administration_RegionForm
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
 '* This page is the entry form for region creation and update.
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
  If Session("Access") <> 9 Then                           ' SysAdmin Only access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("RegionForm", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page accessed setup
   ' Define process unique objects here

   ' Get identity key
   If Len(Request("KeyID")) > 0 Then                       ' Update or Add Mode
    KeyID.Value = Request("KeyID")
   End If
   Session("UpdAdd") = False

   ' Setup general page controls

   ' Fill Users selection list
   Dim GetUsers As MySqlDataReader
   SQLCmd = "Select UUID, Concat(username,' ',lastname) as Name " +
            "From users " +
            "Order by lastname,username"
   If Trace.IsEnabled Then Trace.Warn("RegionForm", "Language SQLCmd=" + SQLCmd.ToString())
   GetUsers = MyDB.GetReader("MyData", SQLCmd)
   Dim utf8 As New System.Text.UTF8Encoding
   While GetUsers.Read()
    If Trace.IsEnabled Then Trace.Warn("RegionForm", "User Name: " + GetUsers("Name").ToString())
    SelOwner.Items.Add(New ListItem(GetUsers("Name").ToString(), GetUsers("UUID").ToString().Trim()))
   End While
   GetUsers.Close()

   ' Define list of Region Types
   RegionTypes.Items.Add(New ListItem("Full", 1))
   RegionTypes.Items.Add(New ListItem("Ocean / Strait", 2))
   RegionTypes.Items.Add(New ListItem("Scenic", 3))

   ' Display data fields based on edit or add mode
   If KeyID.Value.ToString().Trim().Length > 0 Then                           ' Edit Mode, show database values
    ShowUUID.InnerText = KeyID.Value

    Dim GetObjects As MySqlDataReader
    SQLCmd = "Select Count(UUID) as Objects From prims Where RegionUUID=" + MyDB.SQLStr(KeyID.Value)
    If Trace.IsEnabled Then Trace.Warn("RegionForm", "Get Object count SQLCmd: " + SQLCmd.ToString())
    GetObjects = MyDB.GetReader("MyData", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("EstateForm", "DB Error: " + MyDB.ErrMessage().ToString())
    GetObjects.Read()
    Objects.InnerText = GetObjects("Objects")
    GetObjects.Close()

    Dim StatusImg As String
    StatusImg = "Offline"
    Dim drApp As MySqlDataReader
    SQLCmd = "Select regionName,status,locationX,locationY,ownerUUID,primMax,productType,CreateDate " +
             "From regionxml " +
             "Where UUID=" + MyDB.SQLStr(KeyID.Value)
    If Trace.IsEnabled Then Trace.Warn("RegionForm", "Get display Values SQLCmd: " + SQLCmd.ToString())
    drApp = MyDB.GetReader("MySite", SQLCmd)
    If drApp.HasRows() Then
     drApp.Read()
     regionName.Text = drApp("regionName").ToString().Trim()
     MadeDate.InnerText = FormatDateTime(drApp("CreateDate"), DateFormat.ShortDate)
     LocationX.Text = drApp("locationX").ToString()
     'LocationX.Enabled = (drApp("status") = 0)
     LocationY.Text = drApp("locationY").ToString()
     'LocationY.Enabled = (drApp("status") = 0)
     SelOwner.SelectedValue = drApp("ownerUUID").ToString().Trim()
     primMax.Text = drApp("primMax").ToString()
     RegionTypes.SelectedValue = drApp("productType").ToString().Trim()
     DelTitle.InnerText = "Entry: " + drApp("regionName").ToString().Trim()
     If drApp("status") = 0 Then
      StatusImg = "Offline"
     ElseIf drApp("status") = 1 Then
      StatusImg = "Starting"
     ElseIf drApp("status") = 2 Then
      StatusImg = "Online"
     Else
      StatusImg = "Closing"
     End If
    Else
     ShowDate.Visible = False
     regionName.Text = ""
     LocationX.Text = ""
     LocationY.Text = ""
     SelOwner.SelectedValue = Session("OwnerUUID").ToString()
     If CInt(Objects.InnerText) > 30000 Then
      primMax.Text = "45000"
     Else
      primMax.Text = "30000"
     End If
     Session("UpdAdd") = True
    End If
    ' Setup Edit Mode page display controls
    PageTitle.InnerHtml = "Edit Region Settings - Status: <img src=""/Images/Icons/" + StatusImg.ToString() + ".png"" alt=""" + StatusImg.ToString() + """ style=""vertical-align: middle;""/>"
    UpdDelBtn.Visible = True                               ' Allow Update and Delete button to show
    Button2.Visible = (StatusImg = "Offline" And CInt(Objects.InnerText) = 0) ' Delete only if offline and no objects
    drApp.Close()
    AddBtn.Visible = False                                 ' Disable the Add button
   Else                                                    ' Add Mode, show blank fields
    ShowDate.Visible = False
    ShowItems.Visible = False
    ShowMsg.Visible = False
    ShowUUID.Visible = False
    regionName.Text = ""
    LocationX.Text = ""
    LocationY.Text = ""
    SelOwner.SelectedValue = Session("OwnerUUID")
    RegionTypes.SelectedValue = 1                          ' Default to full region
    primMax.Text = "30000"
    ' Setup Add Mode page display controls
    PageTitle.InnerText = "New Region"
    UpdDelBtn.Visible = False                              ' Disable Update and Delete button
    AddBtn.Visible = True                                  ' Allow the Add button to show
   End If
   regionName.Focus()                                       ' Set focus to the first field for entry

   Dim SBMenu As New TreeView
   ' Set up navigation options
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'RegionForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
   SBMenu.AddItem("L", "CallEdit(" + Session("EstateID").ToString() + ",'EstateForm.aspx');", "Estate Edit")
   SBMenu.AddItem("P", "EstateSelect.aspx", "Estate Management")
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
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
  If regionName.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Region Name!\r\n"
  End If
  If primMax.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Prim Maximum!\r\n"
  ElseIf Not IsNumeric(primMax.Text.ToString()) Then
   aMsg = aMsg.ToString() + "Prim Maximum must be an integer value!\r\n"
  ElseIf RegionTypes.SelectedValue = 1 And CInt(primMax.Text) > 45000 Then ' Full region
   aMsg = aMsg.ToString() + "Prim Maximum may not be more than 45000!\r\n"
  ElseIf RegionTypes.SelectedValue = 2 And CInt(primMax.Text) > 7500 Then  ' Ocean / Strait
   aMsg = aMsg.ToString() + "Prim Maximum may not be more than 7500!\r\n"
  ElseIf RegionTypes.SelectedValue = 3 And CInt(primMax.Text) > 5000 Then  ' Scenic
   aMsg = aMsg.ToString() + "Prim Maximum may not be more than 5000!\r\n"
  End If
  If LocationX.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Location X!\r\n"
  ElseIf Not IsNumeric(LocationX.Text.ToString()) Then
   aMsg = aMsg.ToString() + "Location X must be an integer value!\r\n"
  End If
  If LocationY.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Location Y!\r\n"
  ElseIf Not IsNumeric(LocationY.Text.ToString()) Then
   aMsg = aMsg.ToString() + "Location Y must be an integer value!\r\n"
  End If
  If LocationX.Text.ToString().Trim().Length > 0 And
     LocationY.Text.ToString().Trim().Length > 0 Then
   Dim ChkRegion As MySqlDataReader
   SQLCmd = "Select regionName " +
            "From regionxml " +
            "Where locationX=" + MyDB.SQLNo(LocationX.Text) + " and LocationY=" + MyDB.SQLNo(LocationY.Text)
   If Not tAdd Then                                       ' Edit mode verification
    SQLCmd = SQLCmd.ToString() + " and " +
            " UUID<>" + MyDB.SQLStr(KeyID.Value)
   End If
   If Trace.IsEnabled Then Trace.Warn("RegionForm", "Validate Location SQLCmd: " + SQLCmd.ToString())
   ChkRegion = MyDB.GetReader("MySite", SQLCmd)
   If ChkRegion.HasRows() Then
    ChkRegion.Read()
    aMsg = aMsg.ToString() + "Location X, Location Y are in use by " + ChkRegion("regionName").ToString().Trim() + "!\r\n"
   End If
   ChkRegion.Close()
   SQLCmd = "Select locationX,LocationY " +
            "From regionxml " +
            "Where regionName=" + MyDB.SQLStr(regionName.Text)
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
  'If FieldName.Text.ToString().Trim().Length = 0 Then
  ' aMsg = aMsg.ToString() + "Missing Field Name!\r\n"
  'End If
  Return aMsg
 End Function

 ' Update Button
 Private Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
  Dim SQLFields, SQLValues, tMsg As String
  tMsg = ValAddEdit(False)
  If tMsg.ToString().Trim().Length = 0 Then
   If Session("UpdAdd") Then
    SQLFields = "UUID,regionName,locationX,locationY,internalIP,port,externalIP,ownerUUID,lastmapUUID," +
                "lastmapRefresh,primMax,physicalMax,productType"
    SQLValues = MyDB.SQLStr(KeyID.Value) + "," + MyDB.SQLStr(regionName.Text) + "," +
                MyDB.SQLStr(LocationX.Text) + "," + MyDB.SQLStr(LocationY.Text) + "," +
                "'',0,''," +
                MyDB.SQLStr(SelOwner.SelectedValue) + "," + "'00000000-0000-0000-0000-000000000000',0," +
                MyDB.SQLStr(primMax.Text) + ",0," + MyDB.SQLStr(RegionTypes.SelectedValue)
    SQLCmd = "Insert Into regionxml (" + SQLFields + ") Values (" + SQLValues + ")"
    If Trace.IsEnabled Then Trace.Warn("RegionAddEdit", "Insert regionxml SQLCmd: " + SQLCmd.ToString())
   Else
    SQLCmd = "Update regionxml Set " +
             "regionName=" + MyDB.SQLStr(regionName.Text) + "," + "locationX=" + MyDB.SQLStr(LocationX.Text) + "," +
             "locationY=" + MyDB.SQLStr(LocationY.Text) + "," + "ownerUUID=" + MyDB.SQLStr(SelOwner.SelectedValue) + "," +
             "primMax=" + MyDB.SQLStr(primMax.Text) + "," + "productType=" + MyDB.SQLStr(RegionTypes.SelectedValue) + " " +
             "Where UUID=" + MyDB.SQLStr(KeyID.Value)
    If Trace.IsEnabled Then Trace.Warn("RegionAddEdit", "Update regionxml SQLCmd: " + SQLCmd.ToString())
   End If
   MyDB.DBCmd("MySite", SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   Else
    If Not Session("UpdAdd") Then                          ' Land record must exist first
     ' Update Land record with owner changes and remove any prior group assignment.
     SQLCmd = "Update land Set OwnerUUID=" + MyDB.SQLStr(SelOwner.SelectedValue) + "," +
              "GroupUUID='00000000-0000-0000-0000-000000000000' " +
              "Where UUID=" + MyDB.SQLStr(KeyID.Value)
     If Trace.IsEnabled Then Trace.Warn("RegionAddEdit", "Update land SQLCmd: " + SQLCmd.ToString())
     MyDB.DBCmd("MyData", SQLCmd)
     If MyDB.Error() Then
      tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
     End If
    End If
   End If
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot update Region:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")          ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                ' Display Alert Message
  End If

 End Sub

 ' Delete Button
 Private Sub SetDel_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SetDel.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("RegionForm", "Delete process Event")
  ' Action is triggered by a JavaScript process, not a button click.
  ' Remove entry from Estate
  SQLCmd = "Delete From estate_map Where RegionID=" + MyDB.SQLStr(KeyID.Value)
  If Trace.IsEnabled Then Trace.Warn("RegionForm", "Delete estate_map SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  ' Remove entry from regionsettings
  SQLCmd = "Delete From regionsettings Where RegionUUID=" + MyDB.SQLStr(KeyID.Value)
  If Trace.IsEnabled Then Trace.Warn("RegionForm", "Delete regionsettings SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  ' Remove entry record
  SQLCmd = "Delete From regionxml Where UUID=" + MyDB.SQLStr(KeyID.Value)
  If Trace.IsEnabled Then Trace.Warn("RegionForm", "Delete regionxml SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MySite", SQLCmd)
  If Not Trace.IsEnabled Then
   Response.Redirect("EstateForm.aspx")                    ' Return to Selection page
  End If
 End Sub

 ' Add Button
 Private Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Response.Redirect("/Default.aspx")                      ' Return to logon page
  End If
  Dim SQLFields, SQLValues, tMsg As String
  tMsg = ValAddEdit(True)

  If tMsg.Length = 0 Then
   If CInt(Session("EstateID")) = 0 Then
    Response.Redirect("EstateSelect.aspx")                   ' Return to Selection page
   End If
   Dim UUID As String
   UUID = Guid.NewGuid().ToString()
   SQLFields = "UUID,regionName,locationX,locationY,internalIP,port,externalIP,ownerUUID,lastmapUUID," +
               "lastmapRefresh,primMax,physicalMax,productType"
   SQLValues = MyDB.SQLStr(UUID) + "," + MyDB.SQLStr(regionName.Text) + "," +
               MyDB.SQLStr(LocationX.Text) + "," + MyDB.SQLStr(LocationY.Text) + "," +
               "'',0,''," +
               MyDB.SQLStr(SelOwner.SelectedValue) + "," + "'00000000-0000-0000-0000-000000000000',0," +
               MyDB.SQLStr(primMax.Text) + ",0," + MyDB.SQLStr(RegionTypes.SelectedValue)
   SQLCmd = "Insert Into regionxml (" + SQLFields + ") Values (" + SQLValues + ")"
   If Trace.IsEnabled Then Trace.Warn("RegionForm", "Insert regionxml SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   Else
    ' Add to Estate
    SQLCmd = "Insert into estate_map Set RegionID=" + MyDB.SQLStr(UUID) + ",EstateID=" + MyDB.SQLNo(Session("EstateID"))
    If Trace.IsEnabled Then Trace.Warn("RegionForm", "Insert estate_map SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MyData", SQLCmd)
    If MyDB.Error() Then
     tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
    End If
   End If
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot add Region:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")          ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                ' Display Alert Message
  Else
   If Not Trace.IsEnabled Then
    Response.Redirect("EstateForm.aspx")                   ' Return to Selection page
   End If
  End If
 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub

End Class
