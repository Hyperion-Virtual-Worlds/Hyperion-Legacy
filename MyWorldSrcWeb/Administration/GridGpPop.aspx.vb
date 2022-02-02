
Partial Class Administration_GridGpPop
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
 '* This page is the general template for any called popup pages needed from any other website pages.
 '* 
 '* 

 '* Built from MyWorld Popup Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private PageCtl As New GridLib
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Validate logon and session existance.
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Session("Message") = "Session timeout occured! Please close this window and logon again."
   Response.Redirect("/PopupError.aspx")                   ' Display Error message
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Session("Message") = "Secure Access (https:) is not active! Please close this window and logon again."
   Response.Redirect("/PopupError.aspx")                   ' Display Error message
  End If
  If Session("Access") <> 9 Then                           ' SysAdmin Only access
   BodyTag.Attributes.Add("onload", "window.close();")     ' Activate onload option to close window
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("GridGpPop", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page is called setup
   ' define local objects here
   PageTitle.InnerText = "Group Setup"

   ' Populate Grid Owner Group selection list Either as Group Founder or Owner role.
   Dim GroupList As MySqlDataReader
   SQLCmd = "Select Name From osgroup " +
            "Where FounderID=" + MyDB.SQLStr(Session("UUID")) + " Or GroupID in " +
            " (Select GroupID From osgrouprolemembership " +
            "  Where RoleID in (Select RoleID From osroles Where Name='Owners') and AgentID=" + MyDB.SQLStr(Session("UUID")) + ") " +
            "Order by Name"
   If Trace.IsEnabled Then Trace.Warn("GridGpPop", "Group Selections: " + SQLCmd.ToString())
   GroupList = MyDB.GetReader("MyData", SQLCmd)
   Groups.Items.Add("New Group")
   If GroupList.HasRows() Then
    While GroupList.Read()
     Groups.Items.Add(GroupList("Name").ToString())
    End While
   End If
   GroupList.Close()

   ' Replace the oAcctWin with the actual window identifier in the calling page.
   BodyTag.Attributes.Item("onload") = "opener.oGroupWin=window;"
   BodyTag.Attributes.Item("onunload") = "opener.oGroupWin=opener;"

   Display()
  End If
 End Sub

 ' Show Regions listing for Estate
 Private Sub Display()
  If Trace.IsEnabled Then Trace.Warn("GridGpPop", "** Display() Called")
  ' Get Display list Items here
  Dim GetGroup As MySqlDataReader
  SQLCmd = "Select Parm2 " +
           "From control " +
           "Where Control='GRIDSETUP' and Parm1='StartGroup'"
  If Trace.IsEnabled Then Trace.Warn("GridGpPop", "Get Assigned Group SQLCmd: " + SQLCmd.ToString())
  GetGroup = MyDB.GetReader("MySite", SQLCmd)
  GetGroup.Read()
  If Groups.Items.Count() = 1 Then                         ' Add Group mode
   ShowSelect.Visible = False
   UpdDelBtn.Visible = False
  Else                                                     ' Select Group Update
   If GetGroup("Parm2").ToString().Trim().Length > 0 Then
    Groups.SelectedValue = GetGroup("Parm2").ToString().Trim()
   End If
   ShowForm.Visible = False
   AddBtn.Visible = False
  End If
  GetGroup.Close()

 End Sub

 ' Process data validation checks
 Private Function ValAddEdit(ByVal tAdd As Boolean) As String
  ' Parameter tAdd allows selections of Add mode only testing
  Dim aMsg As String
  aMsg = ""
  If tAdd Then
   ' Process error checking as required, place messages in tMsg.
   If GroupName.Text.ToString().Trim().Length = 0 Then
    aMsg = aMsg.ToString() + "Missing Group Name!\r\n"
   Else
    Dim GroupChk As MySqlDataReader
    SQLCmd = "Select GroupID From osgroup Where Name=" + MyDB.SQLStr(GroupName.Text)
    If Trace.IsEnabled Then Trace.Warn("GridGpPop", "Get Assigned Group SQLCmd: " + SQLCmd.ToString())
    GroupChk = MyDB.GetReader("MyData", SQLCmd)
    If GroupChk.HasRows() Then
     aMsg = aMsg.ToString() + "Group Name is already in use!\r\n"
    End If
    GroupChk.Close()
   End If
  End If
  'If FieldName.Text.ToString().Trim().Length = 0 Then
  ' aMsg = aMsg.ToString() + "Missing Field Name!\r\n"
  'End If
  Return aMsg
 End Function

 ' Update Button
 Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
  Dim tMsg As String
  tMsg = ValAddEdit(False)

  If tMsg.ToString().Trim().Length = 0 Then
   SQLCmd = "Update control Set Parm2=" + MyDB.SQLStr(Groups.SelectedValue) + " " +
            "Where Control='GRIDSETUP' and Parm1='StartGroup'"
   If Trace.IsEnabled Then Trace.Warn("GridGpPop", "Update Start Group SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridGpPop", "DB Error: " + MyDB.ErrMessage().ToString())
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot set Group:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")          ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                ' Display Alert Message
  Else
   BodyTag.Attributes.Add("onload", "opener.location.reload(true); window.close();")     ' Activate onload option to close window
  End If
 End Sub

 ' Remove Button
 Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
  If Trace.IsEnabled Then Trace.Warn("GridGpPop", "Delete process Event")
  ' Action is triggered by a JavaScript process, not a button click.
  SQLCmd = "Update control Set Parm2='' " +
           "Where Control='GRIDSETUP' and Parm1='StartGroup'"
  If Trace.IsEnabled Then Trace.Warn("GridGpPop", "Remove Start Group SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridGpPop", "DB Error: " + MyDB.ErrMessage().ToString())
  If Not Trace.IsEnabled Then
   BodyTag.Attributes.Add("onload", "opener.location.reload(true); window.close();")     ' Activate onload option to close window
  End If
 End Sub

 ' Add Button
 Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
  Dim tMsg As String
  tMsg = ValAddEdit(True)

  If tMsg.Length = 0 Then
   Dim GroupID, OwnerRoleID As String
   GroupID = Guid.NewGuid().ToString()
   OwnerRoleID = Guid.NewGuid().ToString()
   SQLCmd = "Insert into osgroup " +
            "(GroupID, Name, Charter, InsigniaID, FounderID, MembershipFee, OpenEnrollment," +
            " ShowInList, AllowPublish, MaturePublish, OwnerRoleID) " +
            "Values " +
            "(" + MyDB.SQLStr(GroupID) + ", " + MyDB.SQLStr(GroupName.Text) + ", '', " +
            "'00000000-0000-0000-0000-000000000000', " + MyDB.SQLStr(Session("UUID")) + ", " +
            "'0', '0', '1', '0', '1', " + MyDB.SQLStr(OwnerRoleID) + ")"
   If Trace.IsEnabled Then Trace.Warn("GridGpPop", "Create osgroup SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MyData", SQLCmd)
   If MyDB.Error() Then
    If Trace.IsEnabled Then Trace.Warn("GridGpPop", "DB Error: " + MyDB.ErrMessage().ToString())
   Else
    SQLCmd = "Insert into osrole " +
             "(GroupID, RoleID, Name, Description, Title, Powers) " +
             "Values " +
             "(" + MyDB.SQLStr(GroupID) + ", '00000000-0000-0000-0000-000000000000', 'Everyone', 'Everyone in the group is in the everyone role.', 'Member of group', '8796495740928');" +
             "Insert into osrole " +
             "(GroupID, RoleID, Name, Description, Title, Powers)" +
             "Values " +
             "(" + MyDB.SQLStr(GroupID) + ", " + MyDB.SQLStr(OwnerRoleID) + ", 'Owners', 'Owners of Group', 'Owner of group', '18446744073709551615')"
    If Trace.IsEnabled Then Trace.Warn("GridGpPop", "Create osrole SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MyData", SQLCmd)
    If MyDB.Error() Then
     If Trace.IsEnabled Then Trace.Warn("GridGpPop", "DB Error: " + MyDB.ErrMessage().ToString())
    Else
     SQLCmd = "Insert into osgrouprolemembership " +
              "(GroupID, RoleID, AgentID) " +
              "Values " +
              "(" + MyDB.SQLStr(GroupID) + ", '00000000-0000-0000-0000-000000000000', " + MyDB.SQLStr(Session("UUID")) + ");" +
              "Insert into osgrouprolemembership " +
              "(GroupID, RoleID, AgentID)" +
              "Values " +
              "(" + MyDB.SQLStr(GroupID) + ", " + MyDB.SQLStr(OwnerRoleID) + ", " + MyDB.SQLStr(Session("UUID")) + ")"
     If Trace.IsEnabled Then Trace.Warn("GridGpPop", "Create osgrouprolemembership SQLCmd: " + SQLCmd.ToString())
     MyDB.DBCmd("MyData", SQLCmd)
     If MyDB.Error() Then
      If Trace.IsEnabled Then Trace.Warn("GridGpPop", "DB Error: " + MyDB.ErrMessage().ToString())
     Else
      SQLCmd = "Insert into osgroupmembership " +
               "(GroupID, AgentID, SelectedRoleID, Contribution, ListInProfile, AcceptNotices) " +
               "Values " +
               "(" + MyDB.SQLStr(GroupID) + ", " + MyDB.SQLStr(Session("UUID")) + ", " + MyDB.SQLStr(OwnerRoleID) + ", '0', '1', '1')"
      If Trace.IsEnabled Then Trace.Warn("GridGpPop", "Create osgroupmembership SQLCmd: " + SQLCmd.ToString())
      MyDB.DBCmd("MyData", SQLCmd)
      If MyDB.Error() Then
       If Trace.IsEnabled Then Trace.Warn("GridGpPop", "DB Error: " + MyDB.ErrMessage().ToString())
      Else
       SQLCmd = "Update control Set Parm2=" + MyDB.SQLStr(GroupName.Text) + " " +
                "Where Control='GRIDSETUP' and Parm1='StartGroup'"
       If Trace.IsEnabled Then Trace.Warn("GridGpPop", "Assign Start Group SQLCmd: " + SQLCmd.ToString())
       MyDB.DBCmd("MySite", SQLCmd)
      End If
     End If
    End If
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot create Group:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")          ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                ' Display Alert Message
  Else
   BodyTag.Attributes.Add("onload", "opener.location.reload(true); window.close();")     ' Activate onload option to close window
  End If
 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub

End Class
