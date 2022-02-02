Partial Class Register
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
 '* This page processes new world account creation and avatar selection, setting up the account in the 
 '* world database and copy of selected avatar items to the new account.
 '* 

 '* Built from MyWorld Basic Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private GridLib As New GridLib
 Private SQLCmd, SQLFields, SQLValues As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("Register", "Start Page Load")

  If IsNothing(Session("SSLStatus")) Then
   Session("SSLStatus") = False
   ' Force SSL active if it is not and is required.
   If Request.ServerVariables("HTTPS") = "off" Then         ' Security is not active and is required
    If Trace.IsEnabled Then Trace.Warn("Register", "Https is off")
    Dim drServer As MySqlDataReader
    SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='ServerLocation'"
    If Trace.IsEnabled Then Trace.Warn("Register", "Get location SQLCmd: " + SQLCmd)
    drServer = MyDB.GetReader("MySite", SQLCmd)
    If drServer.Read() Then
     If Trace.IsEnabled Then Trace.Warn("Register", "ServerLocation = " + drServer("Parm2").ToString().Trim().ToUpper())
     If drServer("Parm2").ToString().Trim().ToUpper() = "LIVE" Then ' Security must be on if not testing
      drServer.Close()
      Session("SSLStatus") = True
      Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + "/Register.aspx")
     End If
    Else                                                    ' show error if not located
     If Trace.IsEnabled Then Trace.Warn("Register", "Set https error: <br>Control value for 'Server Location' (ServerLocation) was not defined!")
     Session("ErrorMessage") = "Control value for 'Server Location' (ServerLocation) was not defined!"
     Response.Redirect("/Error.aspx")
    End If
    drServer.Close()
   End If
  ElseIf Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + "/Register.aspx")
  End If

  If Not IsPostBack Then                                    ' First time page is called setup
   Dim tHtmlOut As String
   tHtmlOut = ""

   ' Setup general page controls
   ' define local objects here

   ' Setup Avatar Selections
   Dim AvatarTypes As MySqlDataReader
   SQLCmd = "Select Name,Parm1 From control Where Control='GridAvatars' Order by Nbr1"
   If Trace.IsEnabled Then Trace.Warn("Register", "Avatar Selections: " + SQLCmd.ToString())
   AvatarTypes = MyDB.GetReader("MySite", SQLCmd)
   If AvatarTypes.HasRows() Then
    While AvatarTypes.Read()
     Gender.Items.Add(New ListItem(AvatarTypes("Name").ToString(), AvatarTypes("Parm1").ToString()))
    End While
   End If
   AvatarTypes.Close()

   ' Check if page is registered
   Dim drGetPage As MySqlDataReader
   SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
   If Trace.IsEnabled Then Trace.Warn("Register", "Get Page Record Check: " + SQLCmd)
   drGetPage = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Register", "DB Error: " + MyDB.ErrMessage().ToString())
   If Not drGetPage.HasRows() Then
    drGetPage.Close()
    ' Create page entry
    SQLFields = "Name,Path"
    SQLValues = "'Register'," + MyDB.SQLStr(Request.ServerVariables("URL"))
    SQLCmd = "Insert into pagemaster (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
    If Trace.IsEnabled Then Trace.Warn("Register", "Insert Page record: " + SQLCmd)
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Register", "DB Error: " + MyDB.ErrMessage().ToString())
    ' Reload PageID
    SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
    If Trace.IsEnabled Then Trace.Warn("Schools", "Reload Page RecordID: " + SQLCmd)
    drGetPage = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Register", "DB Error: " + MyDB.ErrMessage().ToString())
   End If

   If drGetPage.Read() Then
    SQLCmd = "Update pagedetail " +
             " Set Active= " +
             "  Case " +
             "   When AutoStart is not null and AutoExpire is null " +
             "   Then Case When AutoStart<=CurDate() Then 'true' Else Active End " +
             "   When AutoStart is null and AutoExpire is not null " +
             "   Then Case When AutoExpire<CurDate() Then 'false' Else Active End " +
             "   When AutoStart is not null and AutoExpire is not null " +
             "   Then Case When CurDate() between AutoStart and AutoExpire Then 'true' " +
             "   Else Case When AutoStart<=CurDate() Then 'false' Else Active End " +
             "   End " +
             "  End " +
             "Where (AutoStart is not null Or AutoExpire is not null) and PageID=" + MyDB.SQLNo(drGetPage("PageID"))
    If Trace.IsEnabled Then Trace.Warn("Register", "Update Page AutoStart: " + SQLCmd)
    MyDB.DBCmd("MySite", SQLCmd)

    ' Check for Page display content
    Dim rsPage As MySqlDataReader
    SQLCmd = "Select Title,Content From pagedetail " +
             "Where Active=1 and PageID=" + MyDB.SQLNo(drGetPage("PageID")) + " " +
             "Order by SortOrder"
    If Trace.IsEnabled Then Trace.Warn("Register", "Get Page Content: " + SQLCmd)
    rsPage = MyDB.GetReader("MySite", SQLCmd)
    If rsPage.HasRows() Then
     tHtmlOut = tHtmlOut +
                "     <table style=""width: 100%;"" cellpadding=""5"" cellspacing=""0""> " + vbCrLf
     While rsPage.Read()
      If rsPage("Title").ToString().Trim().Length > 0 Then
       tHtmlOut = tHtmlOut +
                "      <tr>" + vbCrLf +
                "       <td style=""height: 20px;"" class=""TopicTitle"">" + vbCrLf +
                "        " + rsPage("Title").ToString() + vbCrLf +
                "       </td>" + vbCrLf +
                "      </tr>" + vbCrLf
      End If
      tHtmlOut = tHtmlOut +
                "      <tr>" + vbCrLf +
                "       <td class=""TopicContent"">" + vbCrLf +
                "        " + rsPage("Content").ToString() + vbCrLf +
                "       </td>" + vbCrLf +
                "      </tr>" + vbCrLf
     End While
     tHtmlOut = tHtmlOut +
                "     </table>"
    End If
    rsPage.Close()
   End If
   drGetPage.Close()
   'tHtmlOut = Guid.NewGuid().ToString()

   'Button1.Visible = False                                  ' Disable registration option
   ShowContent.InnerHtml = tHtmlOut.ToString()
   ShowAvatars.Visible = False
   AvatarCnt.Value = "0"                                    ' Has no avatars by default
   Gender.Focus()                                           ' Set focus to the first field for entry

   Dim SBMenu As New TreeView
   ' Sidebar Options control based on Clearance or Write Access
   ' Set up navigation options
   If Trace.IsEnabled Then SBMenu.SetTrace = True
   'SBMenu.AddItem("M", "3", "Report List")                  ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                   ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Other Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry")        ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")      ' Program URL link entry
   SBMenu.AddItem("L", "ShowPOP('Banking');", "Banking Policy")
   SBMenu.AddItem("L", "ShowPOP('Standards');", "Community Standards")
   SBMenu.AddItem("L", "ShowPOP('DMCA');", "DMCA and EUCD Policies")
   SBMenu.AddItem("L", "ShowPOP('Gambling');", "Gambling Policy")
   SBMenu.AddItem("L", "ShowPOP('Privacy');", "Privacy Policy")
   SBMenu.AddItem("L", "ShowPOP('TOS');", "Terms of Service")
   SBMenu.AddItem("L", "ShowPOP('TPV);", "Third Party Viewer Policy")
   If Trace.IsEnabled Then Trace.Warn("Register", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   SBMenu.Close()
  End If

  ' Get Display list Items here

 End Sub

 ' Gender Selection made
 Protected Sub Gender_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Gender.SelectedIndexChanged
  ' NOTE: The second value in the Animator script function is the playback frame rate. This allows the animation to be tuned for best effect.
  Dim HTMLOut As String
  HTMLOut = ""
  Dim Counter As Integer
  Counter = 1
  AvatarCnt.Value = 0                                       ' Has no count if none are found.

  ' Display selected Avatar list
  Dim drAvatars As MySqlDataReader
  SQLCmd = "Select Name From control Where Control='" + Gender.SelectedValue.ToString() + "' Order by Name"
  If Trace.IsEnabled Then Trace.Warn("Register", "Avatar List: " + SQLCmd.ToString())
  drAvatars = MyDB.GetReader("MySite", SQLCmd)
  If drAvatars.HasRows() Then
   While drAvatars.Read()
    HTMLOut = HTMLOut.ToString() + "<div class=""avatar"" style=""background-image:url('/images/Avatars/" + drAvatars("Name").ToString().Trim() + ".jpg');"" title=""" + drAvatars("Name").ToString().Trim() + """ onclick=""ChangeAvatar(this);""></div>" + vbCrLf
    Counter = Counter + 1
   End While
   Avatars.InnerHtml = HTMLOut.ToString()
   AvatarCnt.Value = Counter
  End If
  drAvatars.Close()
  ShowAvatars.Visible = True

 End Sub

 ' Process data validation checks
 Private Function ValAddEdit(ByVal tAdd As Boolean) As String
  ' Parameter tAdd allows selections of Add mode only testing
  Dim aMsg As String
  aMsg = ""
  ' Process error checking as required, place messages in tMsg.
  If FirstName.Value.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing First Name!\r\n"
  End If
  If LastName.Value.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Last Name!\r\n"
  End If
  If Password.Value.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Password!\r\n"
  End If
  If Password2.Value.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Confirm Password!\r\n"
  End If
  If Password.Value.ToString() <> Password2.Value.ToString() Then
   aMsg = aMsg.ToString() + "Password entry mismatch!\r\n"
  End If
  If Email.Value.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Email address!\r\n"
  Else
   If Not GridLib.ValidEmail(Email.Value.ToString()) Then
    aMsg = aMsg.ToString() + "Invalid Email address!\r\n"
   End If
  End If
  If aMsg.ToString().Trim().Length = 0 Then
   ' See if account username is already in use
   Dim ChkUser As MySqlDataReader
   SQLCmd = "Select UUID " +
            "From users " +
            "Where username=" + MyDB.SQLStr(FirstName.Value) + " and lastname=" + MyDB.SQLStr(LastName.Value)
   If Trace.IsEnabled Then Trace.Warn("Register", "Check User Exists, SQLCmd: " + SQLCmd.ToString())
   ChkUser = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    If Trace.IsEnabled Then Trace.Warn("Register", "MyDB Error: " + MyDB.ErrMessage().ToString())
   Else
    If ChkUser.HasRows() Then
     aMsg = aMsg.ToString() + "User Name already exists! Please change it.\r\n"
    End If
   End If
   ChkUser.Close()
  End If
  If Gender.SelectedValue.ToString().Trim().Length > 0 Then ' Has some avatars set up...
   Dim drAvatars As MySqlDataReader
   SQLCmd = "Select Count(Name) as Count From control Where Control='" + Gender.SelectedValue.ToString() + "'"
   If Trace.IsEnabled Then Trace.Warn("Register", "Avatar List: " + SQLCmd.ToString())
   drAvatars = MyDB.GetReader("MySite", SQLCmd)
   If drAvatars.HasRows() Then
    drAvatars.Read()
    If drAvatars("Count") > 0 Then
     If Avatar.Value.ToString().Trim().Length = 0 Then
      aMsg = aMsg.ToString() + "Missing Avatar Selection!\r\n"
     End If
    End If
   End If
  End If
  'If FieldName.Text.ToString().Trim().Length = 0 Then
  ' aMsg = aMsg.ToString() + "Missing Field Name!\r\n"
  'End If
  Return aMsg
 End Function

 ' Add Button
 Private Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
  Dim SQLFields, SQLValues, tMsg, UserUUID, ClothingUUID, OutfitsUUID, CurrentUUID,
      HRegionUUID, SRegionUUID, GroupID, RoleID, StRegName, HRegName As String
  Dim HRegionHandle, SRegionHandle As Int64
  Dim HUserLocationX, HUserLocationY, HUserLocationZ, HUserLookAtX, HUserLookAtY, HUserLookAtZ,
      SUserLocationX, SUserLocationY, SUserLocationZ, SUserLookAtX, SUserLookAtY, SUserLookAtZ As Decimal
  tMsg = ""
  tMsg = ValAddEdit(True)
  If tMsg.Length = 0 Then

   StRegName = ""
   HRegName = ""
   ' Get Default home region and welcome regions from website configuration
   Dim GetSettings As MySqlDataReader
   SQLCmd = "Select " +
            " Case When " +
            "  (Select Parm2 " +
            "   From control " +
            "   Where Control='GRIDSETUP' and Parm1='WelcomeRegion') is null " +
            " Then '' " +
            " Else " +
            "  (Select Parm2 " +
            "   From control " +
            "   Where Control='GRIDSETUP' and Parm1='WelcomeRegion') " +
            " End as Welcome," +
            " Case When " +
            " (Select Parm2 " +
            "  From control " +
            "  Where Control='GRIDSETUP' and Parm1='HomeRegion') is null " +
            " Then '' " +
            " Else " +
            " (Select Parm2 " +
            "  From control " +
            "  Where Control='GRIDSETUP' and Parm1='HomeRegion') " +
            " End as Home"
   If Trace.IsEnabled Then Trace.Warn("Register", "Get Control settings SQLCmd: " + SQLCmd.ToString())
   GetSettings = MyDB.GetReader("MySite", SQLCmd)
   If GetSettings.HasRows() Then
    GetSettings.Read()
    StRegName = GetSettings("Welcome").ToString().Trim()
    HRegName = GetSettings("Home").ToString().Trim()
   End If
   GetSettings.Close()

   ' Get Home region Assignment if the region name is specified in the website configuration
   Dim HomeReg As MySqlDataReader
   SQLCmd = "Select A.UUID,A.regionHandle,B.UserLocationX,B.UserLocationY,B.UserLocationZ," +
            " B.UserLookAtX,B.UserLookAtY,B.UserLookAtZ " +
            "From regions A Inner Join land B on A.UUID=B.RegionUUID " +
            "Where A.regionName=" + MyDB.SQLStr(HRegName)
   If Trace.IsEnabled Then Trace.Warn("Register", "Get Home region SQLCmd: " + SQLCmd.ToString())
   HomeReg = MyDB.GetReader("MyData", SQLCmd)
   If HomeReg.HasRows() Then
    HomeReg.Read()
    HRegionHandle = HomeReg("regionHandle")
    If HomeReg("UserLocationX") = 0 And HomeReg("UserLocationY") = 0 Then ' Attempt default center landing
     HUserLocationX = 128
     HUserLocationY = 128
     HUserLocationZ = 20
    Else                                                    ' Use set landing point
     HUserLocationX = HomeReg("UserLocationX")
     HUserLocationY = HomeReg("UserLocationY")
     HUserLocationZ = HomeReg("UserLocationZ")
    End If
    HUserLookAtX = HomeReg("UserLookAtX")
    HUserLookAtY = HomeReg("UserLookAtY")
    HUserLookAtZ = HomeReg("UserLookAtZ")
    HRegionUUID = HomeReg("UUID")
   Else
    HRegionHandle = 0
    HUserLocationX = 0
    HUserLocationY = 0
    HUserLocationZ = 0
    HUserLookAtX = 0
    HUserLookAtY = 0
    HUserLookAtZ = 0
    HRegionUUID = "00000000-0000-0000-0000-000000000000"
   End If
   HomeReg.Close()

   ' Get Starting Region location if the region name is specified in the website configuration
   Dim StartReg As MySqlDataReader
   SQLCmd = "Select A.UUID,A.regionHandle,B.UserLocationX,B.UserLocationY,B.UserLocationZ," +
            " B.UserLookAtX,B.UserLookAtY,B.UserLookAtZ " +
            "From regions A Inner Join land B on A.UUID=B.RegionUUID " +
            "Where A.regionName=" + MyDB.SQLStr(StRegName)
   If Trace.IsEnabled Then Trace.Warn("Register", "Get Start region SQLCmd: " + SQLCmd.ToString())
   StartReg = MyDB.GetReader("MyData", SQLCmd)
   If StartReg.HasRows() Then
    StartReg.Read()
    SRegionHandle = StartReg("regionHandle")
    If StartReg("UserLocationX") = 0 And StartReg("UserLocationY") = 0 Then ' Attempt default center landing
     SUserLocationX = 128
     SUserLocationY = 128
     SUserLocationZ = 20
    Else                                                    ' Use set landing point
     SUserLocationX = StartReg("UserLocationX")
     SUserLocationY = StartReg("UserLocationY")
     SUserLocationZ = StartReg("UserLocationZ")
    End If
    SUserLookAtX = StartReg("UserLookAtX")
    SUserLookAtY = StartReg("UserLookAtY")
    SUserLookAtZ = StartReg("UserLookAtZ")
    SRegionUUID = StartReg("UUID")
   Else
    SRegionHandle = 0
    SUserLocationX = 0
    SUserLocationY = 0
    SUserLocationZ = 0
    SUserLookAtX = 0
    SUserLookAtY = 0
    SUserLookAtZ = 0
    SRegionUUID = "00000000-0000-0000-0000-000000000000"
   End If
   StartReg.Close()

   ' Get Group information for a default group assignment if one exists. 
   ' Definition may be null or empty if not set. Defined in Administration Default Group setup.
   GroupID = "00000000-0000-0000-0000-000000000000"    ' No group defined
   RoleID = "00000000-0000-0000-0000-000000000000"     ' Role is the Everyone role.
   Dim GetDefGroup As MySqlDataReader
   SQLCmd = "Select " +
            " Case When " +
            "  (Select RTrim(Parm2) From control Where Control='GRIDSETUP' and Parm1='StartGroup') is null " +
            " Then '' " +
            " Else " +
            "  (Select RTrim(Parm2) From control Where Control='GRIDSETUP' and Parm1='StartGroup') " +
            " End as GroupName "
   If Trace.IsEnabled Then Trace.Warn("Register", "Get Group SQLCmd: " + SQLCmd.ToString())
   GetDefGroup = MyDB.GetReader("MySite", SQLCmd)
   If GetDefGroup.HasRows() Then
    GetDefGroup.Read()
    If GetDefGroup("GroupName").ToString().Trim().Length > 0 Then
     Dim GetGroup As MySqlDataReader
     SQLCmd = "Select GroupID " +
              "From osgroup " +
              "Where Name=" + MyDB.SQLStr(GetDefGroup("GroupName"))
     If Trace.IsEnabled Then Trace.Warn("Register", "Get Group SQLCmd: " + SQLCmd.ToString())
     GetGroup = MyDB.GetReader("MyData", SQLCmd)
     If GetGroup.HasRows() Then
      GetGroup.Read()
      GroupID = GetGroup("GroupID").ToString()
     End If
     GetGroup.Close()
    End If
   End If
   GetDefGroup.Close()

   ' May Process registration input
   UserUUID = Guid.NewGuid().ToString()
   ' Password processing formula: md5(md5("password") + ":" + passwordSalt)
   ' MD5 Reference: http://www.aspnettutorials.com/tutorials/advanced/md5-secret-aspnet2-vb/
   ' OpenSim Docs: http://opensimulator.org/wiki/AuthIntegration
   Dim tPass, tSalt As String
   tSalt = "" '+ UserUUID.Replace("-", "").ToString()
   tPass = GridLib.CodePassword(Password.Value.ToString(), tSalt.ToString())
   ' Create User Account
   SQLFields = "UUID,username,lastname,passwordHash,passwordSalt,homeRegion," +
               "homeLocationX,homeLocationY,homeLocationZ,homeLookAtX,homeLookAtY,homeLookAtZ," +
               "created,lastLogin,profileImage,profileFirstImage,WebLoginKey,homeRegionID,iz_level,email," +
               "userInventoryURI,userAssetURI,profileAboutText,profileFirstText"
   SQLValues = MyDB.SQLStr(UserUUID) + ",N" + MyDB.SQLStr(FirstName.Value) + ",N" + MyDB.SQLStr(LastName.Value) + "," +
               MyDB.SQLStr(tPass) + "," + MyDB.SQLStr(tSalt) + "," +
               MyDB.SQLNo(HRegionHandle) + "," + MyDB.SQLNo(HUserLocationX) + "," +
               MyDB.SQLNo(HUserLocationY) + "," + MyDB.SQLNo(HUserLocationZ) + "," +
               MyDB.SQLNo(HUserLookAtX) + "," + MyDB.SQLNo(HUserLookAtY) + "," +
               MyDB.SQLNo(HUserLookAtZ) + ",UNIX_TIMESTAMP()," +
               "0,'00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000'," +
               MyDB.SQLStr(HRegionUUID) + ",0," + MyDB.SQLStr(Email.Value) + "," +
               "'','','',''"
   ' Check for grid owner account, defaults to Grid Owner
   Dim MAvatar As MySqlDataReader
   SQLCmd = "Select Parm2 " +
            "From control " +
            "Where Control='GridSysAccounts' and Parm1='GridOwnerAcct'"
   If Trace.IsEnabled Then Trace.Warn("Economy", "Check for Economy SQLCmd: " + SQLCmd.ToString())
   MAvatar = MyDB.GetReader("MySite", SQLCmd)
   If MAvatar.HasRows() Then
    MAvatar.Read()
    If FirstName.Value.ToString().Trim() + " " + LastName.Value.ToString().Trim() = MAvatar("Parm2").ToString().Trim() Then
     ' godlevel required for Grid Manager program access to services and region instances
     SQLFields = SQLFields.ToString() + ",godlevel"
     SQLValues = SQLValues.ToString() + ",250"
    End If
   End If
   MAvatar.Close()
   SQLCmd = "Insert Into users (" + SQLFields + ") Values (" + SQLValues + ")"
   If Trace.IsEnabled Then Trace.Warn("Register", "Insert users SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MyData", SQLCmd)
   If MyDB.Error() Then
    tMsg = "Insert Users DB Error: " + MyDB.ErrMessage() + "\r\n"
    If Trace.IsEnabled Then Trace.Warn("Register", "Insert Users DB Error: " + MyDB.ErrMessage().ToString())
   Else
    ' Create agent entry with starting location settings entry. Float values here were all OK.
    SQLFields = "UUID,agentIP,agentPort,agentOnline,loginTime,logoutTime,sessionID,secureSessionID,currentRegion,currentHandle,currentPos,currentLookAt"
    SQLValues = MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(Request.ServerVariables("remote_host")) + ",0,0,0,0," +
                MyDB.SQLStr(Guid.NewGuid()) + "," + MyDB.SQLStr(Guid.NewGuid()) + "," +
                MyDB.SQLStr(SRegionUUID) + "," + MyDB.SQLStr(SRegionHandle) + "," +
                MyDB.SQLStr("<" + MyDB.SQLNo(SUserLocationX) + "," + MyDB.SQLNo(SUserLocationY) + "," +
                MyDB.SQLNo(SUserLocationZ) + ">") + "," + MyDB.SQLStr("<" + MyDB.SQLNo(SUserLookAtX) + "," +
                MyDB.SQLNo(SUserLookAtY) + "," + MyDB.SQLNo(SUserLookAtZ) + ">")
    SQLCmd = "Insert Into agents (" + SQLFields + ") Values (" + SQLValues + ")"
    If Trace.IsEnabled Then Trace.Warn("Register", "Insert agents SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MyData", SQLCmd)
    If MyDB.Error() Then
     If Trace.IsEnabled Then Trace.Warn("Register", "MyDB Error: " + MyDB.ErrMessage().ToString())
     tMsg = tMsg.ToString() + "Insert Agents DB Error: " + MyDB.ErrMessage() + "\r\n"
    Else
     ' Preset User Preferences based on selection
     SQLFields = "user_id,recv_ims_via_email,listed_in_directory"
     SQLValues = MyDB.SQLStr(UserUUID) + "," + MyDB.SQLNo(IIf(ImsInEmail.Checked, 1, 0)) + "," +
                 MyDB.SQLNo(IIf(ListInDirect.Checked, 1, 0))
     SQLCmd = "Insert Into userpreferences (" + SQLFields + ") Values (" + SQLValues + ")"
     If Trace.IsEnabled Then Trace.Warn("Register", "Insert userpreferences SQLCmd: " + SQLCmd.ToString())
     MyDB.DBCmd("MyData", SQLCmd)
     If MyDB.Error() Then
      tMsg = tMsg.ToString() + "** Insert UserPreferences DB Error: " + MyDB.ErrMessage() + "\r\n"
     End If
     If GroupID.ToString() <> "00000000-0000-0000-0000-000000000000" Then
      ' Create Group membership entry 
      SQLFields = "GroupID,AgentID,SelectedRoleID,Contribution,ListInProfile,AcceptNotices"
      SQLValues = MyDB.SQLStr(GroupID) + "," + MyDB.SQLStr(UserUUID) + "," +
                  MyDB.SQLStr(RoleID) + ",0,1,1"
      SQLCmd = "Insert Into osgroupmembership (" + SQLFields + ") Values (" + SQLValues + ")"
      If Trace.IsEnabled Then Trace.Warn("Register", "Insert osgroupmembership SQLCmd: " + SQLCmd.ToString())
      MyDB.DBCmd("MyData", SQLCmd)
      If MyDB.Error() Then
       tMsg = tMsg.ToString() + "** Insert Groupmembership DB Error: " + MyDB.ErrMessage() + "\r\n"
      End If
      ' Set GroupRoleMembership
      SQLFields = "GroupID,RoleID,AgentID"
      SQLValues = MyDB.SQLStr(GroupID) + "," + MyDB.SQLStr(RoleID) + "," +
                  MyDB.SQLStr(UserUUID)
      SQLCmd = "Insert Into osgrouprolemembership (" + SQLFields + ") Values (" + SQLValues + ")"
      If Trace.IsEnabled Then Trace.Warn("Register", "Insert osgrouprolemembership SQLCmd: " + SQLCmd.ToString())
      MyDB.DBCmd("MyData", SQLCmd)
      If MyDB.Error() Then
       tMsg = tMsg.ToString() + "** Insert Groupmembership role DB Error: " + MyDB.ErrMessage() + "\r\n" +
              "SQLCmd: " + SQLCmd.ToString() + "\r\n"
      End If
      ' Set Active Group 
      SQLFields = "AgentID,ActiveGroupID"
      SQLValues = MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(GroupID)
      SQLCmd = "Insert Into osagent (" + SQLFields + ") Values (" + SQLValues + ")"
      If Trace.IsEnabled Then Trace.Warn("Register", "Insert osagent SQLCmd: " + SQLCmd.ToString())
      MyDB.DBCmd("MyData", SQLCmd)
      If MyDB.Error() Then
       tMsg = tMsg.ToString() + "** Insert agent DB Error: " + MyDB.ErrMessage() + "\r\n" +
              "SQLCmd: " + SQLCmd.ToString() + "\r\n"
      End If
     End If
     ' Create inventory folders default list for Avatar starting with the My Inventory as the root folder
     ' These are required in more than one place:
     ClothingUUID = Guid.NewGuid().ToString()
     CurrentUUID = Guid.NewGuid().ToString()
     OutfitsUUID = Guid.NewGuid().ToString()

     tMsg = tMsg.ToString() + GridLib.CreateInvFolders(UserUUID, ClothingUUID, CurrentUUID, OutfitsUUID, MyDB)

     If tMsg.ToString().Trim().Length = 0 And CInt(AvatarCnt.Value) > 0 Then
      ' Setup Avatar here, add componets to inventory in the Clothing/Avatar, My Outfits/Avatar and Current Outfit folders.
      ' System manages the items in avatarattachments and avatarappearance tables.

      Dim ClothingAvatarUUID, OutfitsAvatarUUID, InvUUID As String
      ' Create Avatar folder in the Clothing folder for this account
      ClothingAvatarUUID = Guid.NewGuid().ToString()
      SQLFields = "folderName,type,version,folderID,agentID,parentFolderID"
      SQLValues = MyDB.SQLStr(Avatar.Value) + ",-1,1," + MyDB.SQLStr(ClothingAvatarUUID) + "," +
                  MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(ClothingUUID)
      SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
      If Trace.IsEnabled Then Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
      MyDB.DBCmd("MyData", SQLCmd)
      If MyDB.Error() Then
       If Trace.IsEnabled Then Trace.Warn("Register", "Create Clothing, " + Avatar.Value.ToString() + " folder DB Error: " + MyDB.ErrMessage())
       tMsg = tMsg.ToString() + "** Create Clothing/" + Avatar.Value.ToString() + " folder DB Error: " + MyDB.ErrMessage() + "\r\n"
      End If

      ' Create Avatar folder in the My Outfits folder
      OutfitsAvatarUUID = Guid.NewGuid().ToString()
      SQLFields = "folderName,type,version,folderID,agentID,parentFolderID"
      SQLValues = MyDB.SQLStr(Avatar.Value) + ",47,1," + MyDB.SQLStr(OutfitsAvatarUUID) + "," +
                  MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(OutfitsUUID)
      SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
      If Trace.IsEnabled Then Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
      MyDB.DBCmd("MyData", SQLCmd)
      If MyDB.Error() Then
       If Trace.IsEnabled Then Trace.Warn("Register", "Create My Outfits, " + Avatar.Value.ToString() + " folder DB Error: " + MyDB.ErrMessage())
       tMsg = tMsg.ToString() + "** Create My Outfits/" + Avatar.Value.ToString() + " folder DB Error: " + MyDB.ErrMessage() + "\r\n"
      End If

      Dim Name() As String
      ReDim Name(1)
      Name(0) = ""
      Name(1) = ""
      Dim GetAccount As MySqlDataReader
      SQLCmd = "Select Parm2 From control Where Control='GridSysAccounts' and Parm1='GridLibAcct'"
      If Trace.IsEnabled Then Trace.Warn("Register", "Get World Library Name SQLCmd: " + SQLCmd.ToString())
      GetAccount = MyDB.GetReader("MySite", SQLCmd)
      If GetAccount.HasRows() Then
       GetAccount.Read()
       If GetAccount("Parm2").ToString().Trim().Length > 0 And GetAccount("Parm2").ToString().Trim().Contains(" ") Then
        If Trace.IsEnabled Then Trace.Warn("Register", "* Name has a space.")
        Name = GetAccount("Parm2").ToString().Trim().Split(" ")
       End If
      End If
      GetAccount.Close()

      If Name(0).ToString().Length > 0 And Name(1).ToString().Length > 0 Then ' Has Grid Library Account
       Dim GetAvatar As MySqlDataReader
       If tMsg.ToString().Trim().Length = 0 Then
        ' Get Avatar non-worn items from Grid Library's Inventory (avatar named Library)
        SQLCmd = "Select assetID,assetType,inventoryName,inventoryDescription,inventoryNextPermissions," +
                 "inventoryCurrentPermissions,invType,creatorID,inventoryBasePermissions," +
                 "inventoryEveryOnePermissions,salePrice,saleType,creationDate,groupID,groupOwned,flags," +
                 "inventoryID,avatarID,parentFolderID,inventoryGroupPermissions " +
                 "From inventoryitems " +
                 "Where parentFolderID=" +
                 " (Select folderID From inventoryfolders " +
                 "  Where agentID=" +
                 "   (Select UUID From users " +
                 "    Where username=" + MyDB.SQLStr(Name(0)) + " and lastname=" + MyDB.SQLStr(Name(1)) + ") and folderName=" + MyDB.SQLStr(Avatar.Value) + " and type=47)"
        If Trace.IsEnabled Then Trace.Warn("Register", "Get GetAvatar inventory SQLCmd: " + SQLCmd.ToString())
        GetAvatar = MyDB.GetReader("MyData", SQLCmd)
        If MyDB.Error() Then
         If Trace.IsEnabled Then Trace.Warn("Register", "Get Avatar Items DB Error: " + MyDB.ErrMessage())
         tMsg = tMsg.ToString() + "Get Avatar Items DB Error: " + MyDB.ErrMessage() + "\r\n"
        Else
         If GetAvatar.HasRows() Then
          SQLFields = "assetID,assetType,inventoryName,inventoryDescription,inventoryNextPermissions," +
                      "inventoryCurrentPermissions,invType,creatorID,inventoryBasePermissions," +
                      "inventoryEveryOnePermissions,salePrice,saleType,creationDate,groupID,groupOwned,flags," +
                      "inventoryID,avatarID,parentFolderID,inventoryGroupPermissions"
          While GetAvatar.Read()
           ' Place items into Clothing/Avatar folder - Assign the Next Permissions as Current Permissions
           InvUUID = Guid.NewGuid().ToString()
           SQLValues = MyDB.SQLStr(GetAvatar("assetID")) + "," + MyDB.SQLNo(GetAvatar("assetType")) + "," +
                       MyDB.SQLStr(GetAvatar("inventoryName")) + "," + MyDB.SQLStr(GetAvatar("inventoryDescription")) + "," +
                       MyDB.SQLNo(GetAvatar("inventoryNextPermissions")) + "," + MyDB.SQLNo(GetAvatar("inventoryNextPermissions")) + "," +
                       MyDB.SQLNo(GetAvatar("invType")) + "," + MyDB.SQLStr(GetAvatar("creatorID")) + "," +
                       MyDB.SQLNo(GetAvatar("inventoryBasePermissions")) + "," + MyDB.SQLNo(GetAvatar("inventoryEveryOnePermissions")) + "," +
                       MyDB.SQLNo(GetAvatar("salePrice")) + "," + MyDB.SQLNo(GetAvatar("saleType")) + "," +
                       MyDB.SQLStr(GetAvatar("creationDate")) + "," + MyDB.SQLStr(GetAvatar("groupID")) + "," +
                       MyDB.SQLNo(GetAvatar("groupOwned")) + "," + MyDB.SQLNo(GetAvatar("flags")) + "," +
                       MyDB.SQLStr(InvUUID) + "," + MyDB.SQLStr(UserUUID) + "," +
                       MyDB.SQLStr(ClothingAvatarUUID) + "," + MyDB.SQLNo(GetAvatar("inventoryGroupPermissions"))
           SQLCmd = "Insert Into inventoryitems (" + SQLFields + ") Values (" + SQLValues + ")"
           If Trace.IsEnabled Then Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
           MyDB.DBCmd("MyData", SQLCmd)
           If MyDB.Error() Then
            If Trace.IsEnabled Then Trace.Warn("Register", "Insert Clothing/Avatar item DB Error: " + MyDB.ErrMessage())
            tMsg = tMsg.ToString() + "Add Item to Clothing/" + Avatar.Value.ToString() + " folder DB Error: " + MyDB.ErrMessage() + "\r\n"
           End If
          End While
         End If
        End If
        GetAvatar.Close()
       End If
       If tMsg.ToString().Trim().Length = 0 Then
        'Load worn items
        SQLCmd = "Select assetID,assetType,inventoryName,inventoryDescription,inventoryNextPermissions," +
                 "inventoryCurrentPermissions,invType,creatorID,inventoryBasePermissions," +
                 "inventoryEveryOnePermissions,salePrice,saleType,creationDate,groupID,groupOwned,flags," +
                 "inventoryID,avatarID,parentFolderID,inventoryGroupPermissions " +
                 "From inventoryitems " +
                 "Where parentFolderID=" +
                 " (Select folderID From inventoryfolders " +
                 "  Where agentID=" +
                 "   (Select UUID From users " +
                 "    Where username=" + MyDB.SQLStr(Name(0)) + " and lastname=" + MyDB.SQLStr(Name(1)) + ") and folderName=" + MyDB.SQLStr(Avatar.Value + "Worn") + ")"
        If Trace.IsEnabled Then Trace.Warn("Register", "Get GetAvatar inventory SQLCmd: " + SQLCmd.ToString())
        GetAvatar = MyDB.GetReader("MyData", SQLCmd)
        If MyDB.Error() Then
         If Trace.IsEnabled Then Trace.Warn("Register", "Get Avatar Items DB Error: " + MyDB.ErrMessage())
         tMsg = tMsg.ToString() + "** Get Avatar Items DB Error: " + MyDB.ErrMessage() + "\r\n"
        Else

         Dim ItemCount As Integer
         ItemCount = 0
         SQLFields = "assetID,assetType,inventoryName,inventoryDescription,inventoryNextPermissions," +
                     "inventoryCurrentPermissions,invType,creatorID,inventoryBasePermissions," +
                     "inventoryEveryOnePermissions,salePrice,saleType,creationDate,groupID,groupOwned,flags," +
                     "inventoryID,avatarID,parentFolderID,inventoryGroupPermissions"
         While GetAvatar.Read()
          ItemCount = ItemCount + 1
          ' Place worn items into Clothing/Avatar folder - Assign the Next Permissions as Current Permissions
          InvUUID = Guid.NewGuid().ToString()
          SQLValues = MyDB.SQLStr(GetAvatar("assetID")) + "," + MyDB.SQLNo(GetAvatar("assetType")) + "," +
                      MyDB.SQLStr(GetAvatar("inventoryName")) + "," + MyDB.SQLStr(GetAvatar("inventoryDescription")) + "," +
                      MyDB.SQLNo(GetAvatar("inventoryNextPermissions")) + "," + MyDB.SQLNo(GetAvatar("inventoryNextPermissions")) + "," +
                      MyDB.SQLNo(GetAvatar("invType")) + "," + MyDB.SQLStr(GetAvatar("creatorID")) + "," +
                      MyDB.SQLNo(GetAvatar("inventoryBasePermissions")) + "," + MyDB.SQLNo(GetAvatar("inventoryEveryOnePermissions")) + "," +
                      MyDB.SQLNo(GetAvatar("salePrice")) + "," + MyDB.SQLNo(GetAvatar("saleType")) + "," +
                      MyDB.SQLStr(GetAvatar("creationDate")) + "," + MyDB.SQLStr(GetAvatar("groupID")) + "," +
                      MyDB.SQLNo(GetAvatar("groupOwned")) + "," + MyDB.SQLNo(GetAvatar("flags")) + "," +
                      MyDB.SQLStr(InvUUID) + "," + MyDB.SQLStr(UserUUID) + "," +
                      MyDB.SQLStr(ClothingAvatarUUID) + "," + MyDB.SQLNo(GetAvatar("inventoryGroupPermissions"))
          SQLCmd = "Insert Into inventoryitems (" + SQLFields + ") Values (" + SQLValues + ")"
          If Trace.IsEnabled Then Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
          MyDB.DBCmd("MyData", SQLCmd)
          If MyDB.Error() Then
           If Trace.IsEnabled Then Trace.Warn("Register", "Insert Clothing/Avatar worn item DB Error: " + MyDB.ErrMessage())
           tMsg = tMsg.ToString() + "** Add Item to Clothing/" + Avatar.Value.ToString() + " folder DB Error: " + MyDB.ErrMessage() + "\r\n"
          Else
           ' Place worn items into the My Outfits/Avatar folder
           SQLValues = MyDB.SQLStr(InvUUID) + ",24," +
                       MyDB.SQLStr(GetAvatar("inventoryName")) + "," + MyDB.SQLStr(GetAvatar("inventoryDescription")) + "," +
                       MyDB.SQLNo(GetAvatar("inventoryNextPermissions")) + "," + MyDB.SQLNo(GetAvatar("inventoryNextPermissions")) + "," +
                       MyDB.SQLNo(GetAvatar("invType")) + "," + MyDB.SQLStr(GetAvatar("creatorID")) + "," +
                       MyDB.SQLNo(GetAvatar("inventoryBasePermissions")) + "," + MyDB.SQLNo(GetAvatar("inventoryEveryOnePermissions")) + "," +
                       MyDB.SQLNo(GetAvatar("salePrice")) + "," + MyDB.SQLNo(GetAvatar("saleType")) + "," +
                       MyDB.SQLStr(GetAvatar("creationDate")) + "," + MyDB.SQLStr(GetAvatar("groupID")) + "," +
                       MyDB.SQLNo(GetAvatar("groupOwned")) + "," + MyDB.SQLNo(GetAvatar("flags")) + "," +
                       MyDB.SQLStr(Guid.NewGuid().ToString()) + "," + MyDB.SQLStr(UserUUID) + "," +
                       MyDB.SQLStr(OutfitsAvatarUUID) + "," + MyDB.SQLNo(GetAvatar("inventoryGroupPermissions"))
           SQLCmd = "Insert Into inventoryitems (" + SQLFields + ") Values (" + SQLValues + ")"
           If Trace.IsEnabled Then Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
           MyDB.DBCmd("MyData", SQLCmd)
           If MyDB.Error() Then
            If Trace.IsEnabled Then Trace.Warn("Register", "Insert My Outfits/Avatar worn item DB Error: " + MyDB.ErrMessage())
            tMsg = tMsg.ToString() + "** Add Item to My Outfits/" + Avatar.Value.ToString() + " folder DB Error: " + MyDB.ErrMessage() + "\r\n"
           Else
            ' Copy Avatar worn items into Current Outfits folder from entries created in the My Outfits folder
            SQLValues = MyDB.SQLStr(InvUUID) + ",24," +
                        MyDB.SQLStr(GetAvatar("inventoryName")) + "," + MyDB.SQLStr(GetAvatar("inventoryDescription")) + "," +
                        MyDB.SQLNo(GetAvatar("inventoryNextPermissions")) + "," + MyDB.SQLNo(GetAvatar("inventoryNextPermissions")) + "," +
                        MyDB.SQLNo(GetAvatar("invType")) + "," + MyDB.SQLStr(GetAvatar("creatorID")) + "," +
                        MyDB.SQLNo(GetAvatar("inventoryBasePermissions")) + "," + MyDB.SQLNo(GetAvatar("inventoryEveryOnePermissions")) + "," +
                        MyDB.SQLNo(GetAvatar("salePrice")) + "," + MyDB.SQLNo(GetAvatar("saleType")) + "," +
                        MyDB.SQLStr(GetAvatar("creationDate")) + "," + MyDB.SQLStr(GetAvatar("groupID")) + "," +
                        MyDB.SQLNo(GetAvatar("groupOwned")) + "," + MyDB.SQLNo(GetAvatar("flags")) + "," +
                        MyDB.SQLStr(Guid.NewGuid().ToString()) + "," + MyDB.SQLStr(UserUUID) + "," +
                        MyDB.SQLStr(CurrentUUID) + "," + MyDB.SQLNo(GetAvatar("inventoryGroupPermissions"))
            SQLCmd = "Insert Into inventoryitems (" + SQLFields + ") Values (" + SQLValues + ")"
            If Trace.IsEnabled Then Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
            MyDB.DBCmd("MyData", SQLCmd)
            If MyDB.Error() Then
             If Trace.IsEnabled Then Trace.Warn("Register", "Insert Current item DB Error: " + MyDB.ErrMessage())
             tMsg = tMsg.ToString() + "** Add Item to Current folder DB Error: " + MyDB.ErrMessage() + "\r\n"
            End If
           End If
          End If
         End While
         ' Update version to item count +1
         SQLCmd = "Update inventoryfolders Set version=version+" + MyDB.SQLNo(ItemCount) + " " +
                  "Where folderID=" + MyDB.SQLStr(OutfitsAvatarUUID)
         If Trace.IsEnabled Then Trace.Warn("Register", "Update inventoryfolders SQLCmd: " + SQLCmd.ToString())
         MyDB.DBCmd("MyData", SQLCmd)
         If MyDB.Error() Then
          tMsg = tMsg.ToString() + "** Update My Outfits/" + Avatar.Value.ToString() + " folder DB Error: " + MyDB.ErrMessage() + "\r\n"
         End If
         ' Create reference link in Current Outfit to the My Outfit/Avatar folder as an inventory item
         SQLFields = "assetID,assetType,inventoryName,inventoryDescription,inventoryNextPermissions," +
                     "inventoryCurrentPermissions,invType,creatorID,inventoryBasePermissions," +
                     "inventoryEveryOnePermissions,salePrice,saleType,creationDate,groupID,groupOwned,flags," +
                     "inventoryID,avatarID,parentFolderID,inventoryGroupPermissions"
         SQLValues = MyDB.SQLStr(OutfitsAvatarUUID) + ",25," +
                     MyDB.SQLStr(Avatar.Value) + ",'',581632,647168,8," + MyDB.SQLStr(UserUUID) + "," +
                     "647168,0,0,0," + MyDB.SQLNo(DateDiff(DateInterval.Second, Date.Parse("1/1/1970 00:00:00"), Date.UtcNow())) + "," +
                     "'00000000-0000-0000-0000-000000000000',0,0," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                     MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(CurrentUUID) + ",0"
         SQLCmd = "Insert Into inventoryitems (" + SQLFields + ") Values (" + SQLValues + ")"
         If Trace.IsEnabled Then Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
         MyDB.DBCmd("MyData", SQLCmd)
         If MyDB.Error() Then
          If Trace.IsEnabled Then Trace.Warn("Register", "Insert Current item DB Error: " + MyDB.ErrMessage())
          tMsg = tMsg.ToString() + "** Add Item to Current folder DB Error: " + MyDB.ErrMessage() + "\r\n"
         End If
         ' Avatar loads with all items worn on world entry
        End If
        GetAvatar.Close()
       End If
      End If
     End If
    End If
   End If

   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If

  If tMsg.Length > 0 Then
   tMsg = "Cannot add Entry:\r\n" + tMsg.ToString()
   BodyTag.Attributes.Add("onload", "ShowMsg();")           ' Activate onload option to show message
   GridLib.AlertMessage(Me, tMsg, "ErrMsg")                 ' Display Alert Message
  Else
   If Not Trace.IsEnabled Then
    Response.Redirect("Connect.aspx")                       ' To connect page to get connection URL info
   End If
  End If
 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  GridLib = Nothing
 End Sub
End Class
