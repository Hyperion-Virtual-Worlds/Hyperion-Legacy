
Partial Class Administration_EstateForm
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
 '* This page handles creating a new Estate or editing the selected Estate. New Regions are created in an 
 '* Estate or may be reassigned to another Estate. Region specific settings may be accessed by opening
 '* the region Edit from here.

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
  If Trace.IsEnabled Then Trace.Warn("EstateForm", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page accessed setup
   ' Define process unique objects here

   ' Get identity key
   If Len(Request("KeyID")) > 0 Then                       ' Get passed ID
    KeyID.Value = Request("KeyID")
    Session("EstateID") = KeyID.Value.ToString()
   Else
    If Session("EstateID") <> "" Then                      ' Does the Session have it?
     KeyID.Value = Session("EstateID")
    Else                                                   ' Critical error, cancel operation
     Response.Redirect("EstateSelect.aspx")
    End If
   End If

   ' Setup general page controls
   GetEstates()

   Dim HasRegions As Boolean
   HasRegions = False

   ' Fill Users selection list
   Dim GetUsers As MySqlDataReader
   SQLCmd = "Select UUID, Concat(username,' ',lastname) as Name " +
            "From users " +
            "Order by lastname,username"
   If Trace.IsEnabled Then Trace.Warn("EstateForm", "User Selection SQLCmd: " + SQLCmd.ToString())
   GetUsers = MyDB.GetReader("MyData", SQLCmd)
   Dim utf8 As New System.Text.UTF8Encoding
   While GetUsers.Read()
    If Trace.IsEnabled Then Trace.Warn("EstateForm", "User Name: " + GetUsers("Name").ToString())
    SelOwner.Items.Add(New ListItem(GetUsers("Name").ToString(), GetUsers("UUID").ToString().Trim()))
   End While
   GetUsers.Close()

   ' Display data fields based on edit or add mode
   If CDbl(KeyID.Value) > 0 Then                           ' Edit Mode, show database values
    Dim drApp As MySqlDataReader
    SQLCmd = "Select EstateID,EstateName,AbuseEmailToEstateOwner,AbuseEmail,EstateOwner," +
             " (Select Count(EstateID) as Count From estate_map Where EstateID=estate_settings.EstateID) as RegCount " +
             "From estate_settings " +
             "Where EstateID=" + MyDB.SQLNo(KeyID.Value)
    If Trace.IsEnabled Then Trace.Warn("EstateForm", "Get display Values SQLCmd: " + SQLCmd.ToString())
    drApp = MyDB.GetReader("MyData", SQLCmd)
    If drApp.Read() Then
     EstateID.InnerText = drApp("EstateID").ToString()
     EstateName.Text = drApp("EstateName").ToString().Trim()
     SelOwner.SelectedValue = drApp("EstateOwner").ToString().Trim()
     Session("OwnerUUID") = drApp("EstateOwner").ToString().Trim()
     AbuseEmail.Checked = drApp("AbuseEmailToEstateOwner")
     HasRegions = (drApp("RegCount") > 0)
     ShowEmail.Visible = AbuseEmail.Checked
     If AbuseEmail.Checked Then
      EmailAddr.Text = drApp("AbuseEmail").ToString().Trim()
     End If
     DelTitle.InnerText = "Entry: " + drApp("EstateName").ToString().Trim()
    End If
    drApp.Close()
    ' Setup Edit Mode page display controls
    PageTitle.InnerText = "Edit Estate Settings"
    UpdDelBtn.Visible = True                               ' Allow Update and Delete button to show
    Button2.Visible = Not HasRegions                       ' Delete only if no regions listed
    AddBtn.Visible = False                                 ' Disable the Add button
   Else                                                    ' Add Mode, show blank fields
    ShowID.Visible = False
    EstateName.Text = ""
    If SelOwner.Items.Count() > 0 Then
     ' Get Grid Owner account for default assignment
     Dim GetAccount As MySqlDataReader
     SQLCmd = "Select Parm2 From control Where Control='GridSysAccounts' and Parm1='GridOwnerAcct'"
     If Trace.IsEnabled Then Trace.Warn("EstateForm", "Get Grid Owner Name SQLCmd: " + SQLCmd.ToString())
     GetAccount = MyDB.GetReader("MySite", SQLCmd)
     If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("EstateForm", "DB Error: " + MyDB.ErrMessage().ToString())
     GetAccount.Read()
     SelOwner.SelectedValue = SelOwner.Items.FindByText(GetAccount("Parm2").ToString().Trim()).Value ' Set to grid owner by default
     GetAccount.Close()
    End If
    Session("OwnerUUID") = SelOwner.SelectedValue
    AbuseEmail.Checked = False
    ShowSubTitle.Visible = False
    ShowRegions.Visible = False
    ShowEmail.Visible = False
    EmailAddr.Text = ""
    ' Setup Add Mode page display controls
    PageTitle.InnerText = "New Estate Settings"
    UpdDelBtn.Visible = False                              ' Disable Update and Delete button
    AddBtn.Visible = True                                  ' Allow the Add button to show
   End If
   EstateName.Focus()                                      ' Set focus to the first field for entry
   ShowSubTitle.Visible = HasRegions
   ShowRegions.Visible = HasRegions
   If HasRegions Then
    Display()
   End If

   Dim SBMenu As New TreeView
   ' Set up navigation options
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
   SBMenu.AddItem("P", "EstateSelect.aspx", "Estate Management")
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   SBMenu.AddItem("B", "", "Blank Entry")
   SBMenu.AddItem("T", "", "Page Options")
   SBMenu.AddItem("L", "CallEdit('','RegionForm.aspx');", "New Region")
   If Trace.IsEnabled Then Trace.Warn("EstateForm", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   ' Close Sidebar Menu object
   SBMenu.Close()
  End If

 End Sub

 ' Show Regions listing for Estate
 Private Sub Display()
  If Trace.IsEnabled Then Trace.Warn("EstateForm", "** Display() Called")
  ' Get Display list Items here
  Dim DBName As String
  DBName = "MySite"
  Dim Control As MySqlDataReader
  SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='MySiteDBName'"
  If Trace.IsEnabled Then Trace.Warn("EstateForm", "Get Control DB Name SQLCmd: " + SQLCmd.ToString())
  Control = MyDB.GetReader("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("EstateForm", "DB Error: " + MyDB.ErrMessage().ToString())
  If Control.HasRows() Then
   Control.Read()
   DBName = Control("Parm2").Trim().ToString()
  End If
  Control.Close()

  ' Corrected to provide region name and owner when it is offline.
  Dim Display As MySqlDataReader
  SQLCmd = "Select A.EstateID,A.RegionID," +
           " Case When B.regionName is null Then " +
           "  Case When (Select regionName From " + DBName.ToString() + ".regionxml Where UUID=A.RegionID) is null " +
           "  Then 'Not Found' " +
           "  Else " +
           "   (Select regionName From " + DBName.ToString() + ".regionxml Where UUID=A.RegionID) " +
           "  End " +
           " Else B.regionName End as regionName," +
           " (Select Concat(Rtrim(username),' ',Rtrim(lastname)) as Name From users " +
           "  Where UUID=(Select ownerUUID From " + DBName.ToString() + ".regionxml Where UUID=A.RegionID)) as Owner," +
           " Case When B.regionName is null Then " +
           "  Case When (Select status From " + DBName.ToString() + ".regionxml Where UUID=A.RegionID) is null " +
           "  Then 0 " +
           "  Else " +
           "  (Select status From " + DBName.ToString() + ".regionxml Where UUID=A.RegionID) " +
           "  End " +
           " Else 2 End as Status " +
           "From estate_map A Left Join regions B on A.RegionID=B.UUID  " +
           "Where A.EstateID=" + MyDB.SQLNo(KeyID.Value) + " " +
           "Order by RegionName"
  If Trace.IsEnabled Then Trace.Warn("EstateForm", "Get SQLCmd: " + SQLCmd.ToString())
  ' gvDisplay is a gridview data object placed on the page.
  Display = MyDB.GetReader("MyData", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("EstateForm", "DB Error: " + MyDB.ErrMessage().ToString())
  gvDisplay.DataSource = Display
  gvDisplay.DataBind()
  If gvDisplay.PageCount = 0 Then
   Button2.Visible = True
  End If

 End Sub

 Public Sub GetEstates()
  Dim Estates As String
  Estates = ""
  'Build list of Owner's Estates for region selection
  Dim GetEstates As MySqlDataReader
  SQLCmd = "Select EstateID,EstateName " +
           "From estate_settings " +
           "Order by EstateName"
  If Trace.IsEnabled Then Trace.Warn("EstateForm", "Get Estate List SQLCmd: " + SQLCmd.ToString())
  GetEstates = MyDB.GetReader("MyData", SQLCmd)
  While GetEstates.Read()
   Estates = Estates.ToString() +
             "<option" + IIf(GetEstates("EstateID").ToString() = KeyID.Value, " selected", "") + " value=""" + GetEstates("EstateID").ToString() + """>" + GetEstates("EstateName").ToString().Trim() + "</option>" + vbCrLf
  End While
  Session("EstateList") = Estates.ToString()
  GetEstates.Close()

 End Sub

 ' Abuse email used check changed
 Private Sub AbuseEmail_CheckedChanged(sender As Object, e As EventArgs) Handles AbuseEmail.CheckedChanged
  ShowEmail.Visible = AbuseEmail.Checked
  If Not ShowEmail.Visible Then
   EmailAddr.Text = ""
  End If
 End Sub

 ' Estate Changed for Region
 Private Sub Estate_TextChanged(sender As Object, e As EventArgs) Handles Estate.TextChanged
  If Trace.IsEnabled Then Trace.Warn("EstateForm", "** Estate.TextChanged Called")
  SQLCmd = "Update estate_map Set EstateID=" + MyDB.SQLStr(Estate.Text) + " Where RegionID=" + MyDB.SQLStr(Region.Value)
  If Trace.IsEnabled Then Trace.Warn("EstateForm", "Update estate_map SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("EstateForm", "DB Error: " + MyDB.ErrMessage())
  Estate.Text = ""
  Region.Value = ""
  Display()
 End Sub

 ' Process data validation checks
 Private Function ValAddEdit(ByVal tAdd As Boolean) As String
  ' Parameter tAdd allows selections of Add mode only testing
  Dim aMsg As String
  aMsg = ""
  ' Process error checking as required, place messages in tMsg.
  If EstateName.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Estate Name!\r\n"
  Else
   Dim CheckName As MySqlDataReader
   SQLCmd = "Select EstateID " +
            "From estate_settings " +
            "Where EstateName=" + MyDB.SQLStr(EstateName.Text) + " and EstateID<>" + MyDB.SQLNo(KeyID.Value)
   If Trace.IsEnabled Then Trace.Warn("EstateForm", "CheckName SQLCmd: " + SQLCmd.ToString())
   CheckName = MyDB.GetReader("MyData", SQLCmd)
   If CheckName.HasRows() Then
    aMsg = aMsg.ToString() + "Estate Name has already been used! May not duplicate estate names.\r\n"
   End If
   CheckName.Close()
  End If
  If AbuseEmail.Checked Then
   If EmailAddr.Text.ToString().Trim().Length = 0 Then
    aMsg = aMsg.ToString() + "Missing Missing email address!\r\n"
   ElseIf Not PageCtl.ValidEmail(EmailAddr.Text) Then
    aMsg = aMsg.ToString() + "Invalid email address! Please check the email address.\r\n"
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
   Dim Name As String
   Name = ""
   Dim CheckName As MySqlDataReader
   SQLCmd = "Select EstateName " +
            "From estate_settings " +
            "Where EstateID=" + MyDB.SQLNo(KeyID.Value)
   If Trace.IsEnabled Then Trace.Warn("EstateForm", "CheckName SQLCmd: " + SQLCmd.ToString())
   CheckName = MyDB.GetReader("MyData", SQLCmd)
   If CheckName.Read() Then
    Name = CheckName("EstateName").ToString().Trim()
   End If
   CheckName.Close()
   'Update estate options
   SQLCmd = "Update estate_settings Set " +
            "EstateName=" + MyDB.SQLStr(EstateName.Text) + "," + "AbuseEmailToEstateOwner=" + MyDB.SQLNo(IIf(AbuseEmail.Checked, 1, 0)) + "," +
            "AbuseEmail=" + MyDB.SQLStr(EmailAddr.Text) + "," + "EstateOwner=" + MyDB.SQLStr(SelOwner.SelectedValue) + " " +
            "Where EstateID=" + MyDB.SQLNo(KeyID.Value)
   MyDB.DBCmd("MyData", SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   Else
    Session("OwnerUUID") = SelOwner.SelectedValue
    If Name.ToString() <> EstateName.Text.ToString() And Not Button2.Visible Then ' Estate name changed and hasregions, update region display
     GetEstates()
     Display()
    End If
   End If
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot update Estate:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")          ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                ' Display Alert Message
  End If

 End Sub

 ' Delete Button
 Private Sub SetDel_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SetDel.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("EstateForm", "Delete process Event")
  ' Action is triggered by a JavaScript process, not a button click.
  SQLCmd = "Delete From estate_settings Where EstateID=" + MyDB.SQLNo(KeyID.Value)
  If Trace.IsEnabled Then Trace.Warn("EstateForm", "Delete estate_settings SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If Not Trace.IsEnabled Then
   Response.Redirect("EstateSelect.aspx")                  ' Return to Selection page
  End If
 End Sub

 ' Add Button
 Private Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
  Dim SQLFields, SQLValues, tMsg As String
  tMsg = ValAddEdit(True)

  If tMsg.Length = 0 Then
   Dim EstateID As Integer
   Dim GetEstate As MySqlDataReader
   SQLCmd = "Select EstateID From estate_settings Order by EstateID Desc Limit 1"
   If Trace.IsEnabled Then Trace.Warn("EstateForm", "CheckName SQLCmd: " + SQLCmd.ToString())
   GetEstate = MyDB.GetReader("MyData", SQLCmd)
   If GetEstate.HasRows() Then
    GetEstate.Read()
    EstateID = (GetEstate("EstateID") + 1)
   Else
    EstateID = 1
   End If
   GetEstate.Close()
   ' Create New Estate with default settings.
   SQLFields = "EstateID,EstateName,AbuseEmailToEstateOwner,DenyAnonymous,ResetHomeOnTeleport,FixedSun,DenyTransacted,BlockDwell," +
               "DenyIdentified,AllowVoice,UseGlobalTime,PricePerMeter,TaxFree,AllowDirectTeleport,RedirectGridX,RedirectGridY," +
               "ParentEstateID,SunPosition,EstateSkipScripts,BillableFactor,PublicAccess,AbuseEmail,EstateOwner,DenyMinors"
   SQLValues = MyDB.SQLNo(EstateID) + "," + MyDB.SQLStr(EstateName.Text) + "," + MyDB.SQLNo(IIf(AbuseEmail.Checked, 1, 0)) + ",0,0,0,0,0," +
               "0,1,1,1,0,1,0,0," +
               MyDB.SQLNo(EstateID) + ",0,0,0,1," + MyDB.SQLStr(EmailAddr.Text) + "," + MyDB.SQLStr(SelOwner.SelectedValue) + ",0"
   SQLCmd = "Insert Into estate_settings (" + SQLFields + ") Values (" + SQLValues + ")"
   If Trace.IsEnabled Then Trace.Warn("EstateForm", "Insert estate_settings SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MyData", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("EstateForm", "DB Error: " + MyDB.ErrMessage().ToString())
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   Else
    Session("OwnerUUID") = SelOwner.SelectedValue
   End If
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot add Entry:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")          ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                ' Display Alert Message
  Else
   If Not Trace.IsEnabled Then
    Response.Redirect("EstateSelect.aspx")                 ' Return to Selection page
   End If
  End If
 End Sub

 ' Provide Estate selection list per region displayed by RegionID
 Public Function EstateList(aRegID As String) As String
  Dim tOut As String
  tOut = Session("EstateList")
  Return tOut.ToString()
 End Function

 Public Function ShowStatus(ByVal Status As Integer) As String
  Dim tOut As String
  If Status = 0 Then
   tOut = "<img src=""/Images/Icons/Offline.png"" alt=""Offline"" /> "
  ElseIf Status = 1 Then
   tOut = "<img src=""/Images/Icons/Starting.png"" alt=""Starting"" /> "
  ElseIf Status = 2 Then
   tOut = "<img src=""/Images/Icons/Online.png"" alt=""Online"" /> "
  Else
   tOut = "<img src=""/Images/Icons/Closing.png"" alt=""Closing"" /> "
  End If
  Return tOut.ToString()
 End Function

 ' Timer activated refresh process
 Protected Sub Refresh_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Refresh.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("EstateForm", "** Timer Refresh Process **")
  Refresh.Checked = False
  Display()
 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub

End Class
