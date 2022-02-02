
Partial Class Administration_ControlForm
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
 '* Website Control Values Selection display page. Provides entry maintenance and removal of system 
 '* control values.
 '* 

 '* Built from MyWorld Form template v. 1.0

 ' Define common Page class properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private PageCtl As New GridLib
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Validate logon and session existance.
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Response.Redirect("/Default.aspx")                       ' Return to logon page
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("/Default.aspx")
  End If
  If Session("Access") <> 9 Then                            ' SysAdmin Only access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("ControlForm", "Start Page Load")

  If Not IsPostBack Then                                    ' First time page accessed setup
   ' Define process unique objects here

   ' Get identity key
   If Len(Request("KeyID")) > 0 Then                        ' Update or Add Mode
    KeyID.Value = Request("KeyID")
   End If

   ' Setup general page controls

   ' Display data fields based on edit or add mode
   If KeyID.Value.Length() > 0 Then                         ' Edit Mode, show database values
    Dim drControl As MySqlDataReader
    SQLCmd = "Select Name,Parm1,Parm2 " +
             "From control " +
             "Where Control='ADMINSYSTEM' and Name=" + MyDB.SQLStr(KeyID.Value)
    If Trace.IsEnabled Then Trace.Warn("ControlForm", "Get display Values SQLCmd: " + SQLCmd.ToString())
    drControl = MyDB.GetReader(Session("DBConnection"), SQLCmd)
    If drControl.Read() Then
     NNameCtl.InnerText = drControl("Name").ToString().Trim()
     'ReferenceCtl.InnerHtml = ""
     ReferenceCtl.InnerText = drControl("Parm1").ToString().Trim()
     NName.Text = drControl("Name").ToString().Trim()
     Value.Text = drControl("Parm2").ToString().Trim()
     Value.Focus()
     DelTitle.InnerText = "Entry: " + drControl("Name").ToString().Trim()
    End If
    drControl.Close()
    ' Setup Edit Mode page display controls
    PageTitle.InnerText = "Edit Entry"
    UpdDelBtn.Visible = True                                ' Allow Update and Delete button to show
    AddBtn.Visible = False                                  ' Disable the Add button
   Else                                                     ' Add Mode, show blank fields
    NName.Text = ""
    Parm1.Text = ""
    Value.Text = ""
    ' Setup Add Mode page display controls
    PageTitle.InnerText = "New Entry"
    UpdDelBtn.Visible = False                               ' Disable Update and Delete button
    AddBtn.Visible = True                                   ' Allow the Add button to show
    NName.Focus()
   End If

   Dim SBMenu As New TreeView
   ' Set up navigation options
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                  ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                   ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Other Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'ControlForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")      ' Program URL link entry
   SBMenu.AddItem("P", "ControlSelect.aspx", "Control Values Select")
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   If Trace.IsEnabled Then Trace.Warn("ControlForm", "Show Menu")
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
  If tAdd Then
   If (NName.Text.Trim()).Length = 0 Then
    aMsg = aMsg.ToString() + "Missing Name!\r\n"
   Else
    SQLCmd = "Select Name " +
             "From control " +
             "Where Control='ADMINSYSTEM' and Name=" + MyDB.SQLStr(NName.Text)
    Dim drCheck = MyDB.GetReader(Session("DBConnection"), SQLCmd)
    If drCheck.HasRows Then
     aMsg = aMsg + "List Name '" + NName.Text.ToString() + "' is already in use!\r\n"
    End If
    drCheck.Close()
   End If
   If (Parm1.Text.Trim()).Length = 0 Then
    aMsg = aMsg.ToString() + "Missing Control Entry Name!\r\n"
   Else
    ' Check if Control Entry Name already has been entered in the Control list
    SQLCmd = "Select Name " +
             "From control " +
             "Where Control=" + MyDB.SQLStr(Parm1.Text)
    Dim drCheck = MyDB.GetReader(Session("DBConnection"), SQLCmd)
    If drCheck.HasRows Then
     aMsg = aMsg + "List Control Entry Name '" + Parm1.Text.ToString() + "' is already in use!\r\n"
    End If
    drCheck.Close()
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
   SQLCmd = "Update control Set " +
            "Parm2=" + MyDB.SQLStr(Value.Text) + " " +
            "Where Control='ADMINSYSTEM' and Name=" + MyDB.SQLStr(KeyID.Value)
   If Trace.IsEnabled Then Trace.Warn("ControlAddEdit", "Update control SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd(Session("DBConnection"), SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   End If
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot update Entry:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")           ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                 ' Display Alert Message
  End If

 End Sub

 ' Delete Button
 Private Sub SetDel_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SetDel.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("UserMaint", "Delete process Event")
  ' Action is triggered by a JavaScript process, not a button click.
  'Remove entry record
  SQLCmd = "Delete From control " +
           "Where Control='ADMINSYSTEM' and Name=" + MyDB.SQLStr(KeyID.Value)
  If Trace.IsEnabled Then Trace.Warn("ControlAddEdit", "Delete control SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd(Session("DBConnection"), SQLCmd)
  If Not Trace.IsEnabled Then
   Response.Redirect("ControlSelect.aspx")                  ' Return to Selection page
  End If
 End Sub

 ' Add Button
 Private Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
  Dim SQLFields, SQLValues, tMsg As String
  tMsg = ValAddEdit(True)

  If tMsg.Length = 0 Then
   SQLFields = "Control,Name,Parm1,Parm2"
   SQLValues = "'ADMINSYSTEM'," + MyDB.SQLStr(NName.Text) + "," +
               MyDB.SQLStr(Parm1.Text) + "," + MyDB.SQLStr(Value.Text)
   SQLCmd = "Insert Into control (" + SQLFields + ") Values (" + SQLValues + ")"
   If Trace.IsEnabled Then Trace.Warn("ControlAddEdit", "Insert control SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd(Session("DBConnection"), SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   End If
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot add Entry:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")           ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                 ' Display Alert Message
  Else
   If Not Trace.IsEnabled Then
    Response.Redirect("ControlSelect.aspx")                 ' Return to Selection page
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
