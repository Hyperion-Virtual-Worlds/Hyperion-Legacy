
Partial Class Administration_ListForm
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
 '* Website Control Lists Add, Edit and remove options for any entry.
 '* 
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
  If Trace.IsEnabled Then Trace.Warn("ListForm", "Start Page Load")

  If Not IsPostBack Then                                    ' First time page accessed setup
   ' Define process unique objects here

   ' Get identity key
   If Len(Request("KeyID")) > 0 Then                        ' Update or Add Mode
    KeyID.Value = Request("KeyID")
   End If

   ' Setup general page controls

   ' Display data fields based on edit or add mode
   If KeyID.Value.Length() > 0 Then                         ' Edit Mode, show database values
    Dim drList As MySqlDataReader
    SQLCmd = "Select Name,Parm1,Parm2,Nbr1,Nbr2 From control " +
             "Where Control=" + MyDB.SQLStr(Session("FirstTime")) + " and " +
             " Name=" + MyDB.SQLStr(KeyID.Value)
    If Trace.IsEnabled Then Trace.Warn("ListForm", "Get display Values SQLCmd: " + SQLCmd.ToString())
    drList = MyDB.GetReader(Session("DBConnection"), SQLCmd)
    If drList.Read() Then
     NName.Text = drList("Name").ToString().Trim()
     Parm1.Text = drList("Parm1").ToString().Trim()
     Parm2.Text = drList("Parm2").ToString().Trim()
     Nbr1.Text = drList("Nbr1").ToString()
     Nbr2.Text = drList("Nbr2").ToString()
     DelTitle.InnerText = "Entry: " + drList("Name").ToString().Trim()
    End If
    drList.Close()
    ' Setup Edit Mode page display controls
    PageTitle.InnerText = "List Member in " + IIf(Session("ParentName").ToString().Length = 0, "Control Lists", Session("ParentName"))
    UpdDelBtn.Visible = True                                ' Allow Update and Delete button to show
    AddBtn.Visible = False                                  ' Disable the Add button
    Dim drCount As MySqlDataReader
    SQLCmd = "Select Count(Control) as Count From control " +
             "Where Control=" +
             " (Select Parm1 From control " +
             "  Where Control=" + MyDB.SQLStr(Session("FirstTime")) + " and " +
             "   Name=" + MyDB.SQLStr(KeyID.Value) + ")"
    drCount = MyDB.GetReader(Session("DBConnection"), SQLCmd)
    If Trace.IsEnabled Then Trace.Warn("ListForm", "Get Control Count SQLCmd: " + SQLCmd.ToString())
    drCount.Read()
    If drCount("Count") = 0 Then
     Button2.Visible = True                                 ' Delete only if List member count=zero
    Else
     Button2.Visible = False                                ' Disallow Delete
    End If
    drCount.Close()
   Else                                                     ' Add Mode, show blank fields
    NName.Text = ""
    Parm1.Text = ""
    Parm2.Text = ""
    Nbr1.Text = "0"
    Nbr2.Text = "0"
    ' Setup Add Mode page display controls
    PageTitle.InnerText = "New Member in " + Session("ParentName")
    UpdDelBtn.Visible = False                               ' Disable Update and Delete button
    AddBtn.Visible = True                                   ' Allow the Add button to show
   End If
   NName.Focus()                                            ' Set focus to the first field for entry

   Dim SBMenu As New TreeView
   ' Set up navigation options
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                  ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                   ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Other Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'ListForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")      ' Program URL link entry
   SBMenu.AddItem("P", "ListSelect.aspx", "Control List Select")
   If Session("FirstTime") <> Session("ListControl") Then
    SBMenu.AddItem("L", "SetList('" + Session("ParentCtl") + "');", "Parent Level")
   End If
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   If Trace.IsEnabled Then Trace.Warn("ListForm", "Show Menu")
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
  Dim drCheck As MySqlDataReader
  If NName.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg + " Missing Member Name!\r\n"
  End If
  If Nbr1.Text.ToString().Trim().Length = 0 Then
   If Not IsNumeric(Nbr1.Text.ToString()) Then
    aMsg = aMsg + " Numeric Sort Value is not a number!\r\n"
   End If
  End If
  If (Nbr2.Text.Trim()).Length = 0 Then
   If Not IsNumeric(Nbr2.Text.ToString()) Then
    aMsg = aMsg + " Numeric Value is not a number!\r\n"
   End If
  End If
  If tAdd Then
   SQLCmd = "Select Name From control " +
            "Where Control=" + MyDB.SQLStr(Session("FirstTime")) + " and " +
            " Name=" + MyDB.SQLStr(NName.Text.ToString())
   If Trace.IsEnabled Then Trace.Warn("ListForm", "Check for name used SQLCmd: " + SQLCmd.ToString())
   drCheck = MyDB.GetReader(Session("DBConnection"), SQLCmd)
   If drCheck.Read() Then
    aMsg = aMsg + "Member Name has already been used!\r\n"
   End If
   drCheck.Close()
   If Parm1.Text.ToString().Trim().Length > 0 Then
    SQLCmd = "Select Name From control " +
             "Where Parm1=" + MyDB.SQLStr(Parm1.Text.ToString())
    If Trace.IsEnabled Then Trace.Warn("ListForm", "Check for Parm1 used SQLCmd: " + SQLCmd.ToString())
    drCheck = MyDB.GetReader(Session("DBConnection"), SQLCmd)
    If drCheck.Read() Then                                   ' Set Error Message
     aMsg = aMsg + "Key or List Link has already been used in " + drCheck("Name").ToString() + "!\r\n"
    End If
    drCheck.Close()
   End If
  Else                                                       ' Edit Checks
   Dim drVals As MySqlDataReader                    ' Get original record data to compare
   SQLCmd = "Select Name,Parm1 From control " +
            "Where Control=" + MyDB.SQLStr(Session("FirstTime")) + " and " +
            " Name=" + MyDB.SQLStr(KeyID.Value.ToString())
   If Trace.IsEnabled Then Trace.Warn("ListForm", "Check for name used SQLCmd: " + SQLCmd.ToString())
   drVals = MyDB.GetReader(Session("DBConnection"), SQLCmd)
   drVals.Read()
   If KeyID.Value.ToString() <> NName.Text.Trim() Then       ' Check if changed name is in use
    SQLCmd = "Select Name From control " +
             "Where Name=" + MyDB.SQLStr(NName.Text.ToString())
    If Trace.IsEnabled Then Trace.Warn("ListForm", "Check for new name used SQLCmd: " + SQLCmd.ToString())
    drCheck = MyDB.GetReader(Session("DBConnection"), SQLCmd)
    If drCheck.Read() Then
     aMsg = aMsg + "Member Name has already been used!\r\n"
    End If
   End If
   If Parm1.Text.ToString().Trim() <> drVals("Parm1").ToString().Trim() Then     ' List link was changed
    If Parm1.Text.ToString().Trim().Length > 0 Then
     SQLCmd = "Select Name From control " +
              "Where Parm1=" + MyDB.SQLStr(Parm1.Text.ToString())
     If Trace.IsEnabled Then Trace.Warn("ListForm", "Check for Parm1 used SQLCmd: " + SQLCmd.ToString())
     drCheck = MyDB.GetReader(Session("DBConnection"), SQLCmd)
     If drCheck.Read() Then                                  ' Set Error Message
      If Trace.IsEnabled Then Trace.Warn("ListForm", "Parm1: [" + Parm1.Text.ToString().Trim() + "] <> [" + drVals("Parm1").ToString().Trim() + "]")
      aMsg = aMsg + "Key or List Link has already been used in " + drCheck("Name").Trim() + "!\r\n"
     End If
     drCheck.Close()
    End If
   End If
   drVals.Close()
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
   ' Get original record values
   Dim drVals As MySqlDataReader
   SQLCmd = "Select Name,Parm1 From control " +
            "Where Control=" + MyDB.SQLStr(Session("FirstTime")) + " and " +
            " Name=" + MyDB.SQLStr(KeyID.Value.ToString())
   If Trace.IsEnabled Then Trace.Warn("ListForm", "Get prior Control SQLCmd: " + SQLCmd.ToString())
   drVals = MyDB.GetReader(Session("DBConnection"), SQLCmd)
   drVals.Read()
   ' Update fields
   SQLCmd = "Update control Set " +
            "Name=" + MyDB.SQLStr(NName.Text) + "," + "Parm1=" + MyDB.SQLStr(Parm1.Text) + "," +
            "Parm2=" + MyDB.SQLStr(Parm2.Text) + "," + "Nbr1=" + MyDB.SQLNo(Nbr1.Text) + "," +
            "Nbr2=" + MyDB.SQLNo(Nbr2.Text) + " " +
            "Where Control=" + MyDB.SQLStr(Session("FirstTime")) + " And " +
            " Name=" + MyDB.SQLStr(KeyID.Value)
   If Trace.IsEnabled Then Trace.Warn("ListForm", "Update control SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd(Session("DBConnection"), SQLCmd)
   If Parm1.ToString().Trim().Length > 0 Then                ' Do not allow a Blank member link update
    ' Update any Parm1 List Member Links in use
    SQLCmd = "Update control Set " +
             "Control=" + MyDB.SQLStr(Parm1.Text) + " " +
             "Where Control=" + MyDB.SQLStr(drVals("Parm1").ToString().Trim())
    If Trace.IsEnabled Then Trace.Warn("ListForm", "Update control SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd(Session("DBConnection"), SQLCmd)
   End If
   KeyID.Value = NName.Text.Trim()                           ' Update key value if changed
   drVals.Close()
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then  ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot update Entry:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")            ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                  ' Display Alert Message
  End If

 End Sub

 ' Delete Button
 Private Sub SetDel_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SetDel.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("UserMaint", "Delete process Event")
  ' Action is triggered by a JavaScript process, not a button click.
  'Remove entry record
  SQLCmd = "Delete From control Where Name=" + MyDB.SQLStr(KeyID.Value)
  If Trace.IsEnabled Then Trace.Warn("ListForm", "Delete control SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd(Session("DBConnection"), SQLCmd)
  If Not Trace.IsEnabled Then
   Response.Redirect("ListSelect.aspx")                      ' Return to Selection page
  End If
 End Sub

 ' Add Button
 Private Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
  Dim SQLFields, SQLValues, tMsg As String
  tMsg = ValAddEdit(True)

  If tMsg.Length = 0 Then
   SQLFields = "Control, Name, Parm1, Parm2, Nbr1, Nbr2"
   SQLValues = MyDB.SQLStr(Session("FirstTime")) + "," +
               MyDB.SQLStr(NName.Text) + "," + MyDB.SQLStr(Parm1.Text) + "," +
               MyDB.SQLStr(Parm2.Text) + "," + MyDB.SQLNo(Nbr1.Text) + "," +
               MyDB.SQLNo(Nbr2.Text)
   SQLCmd = "Insert Into control (" + SQLFields + ") Values (" + SQLValues + ")"
   If Trace.IsEnabled Then Trace.Warn("ListForm", "Insert control SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd(Session("DBConnection"), SQLCmd)
   If MyDB.Error() Then
    If Trace.IsEnabled Then Trace.Warn("ListForm", "DB Error: " + MyDB.ErrMessage().ToString())
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   End If
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then  ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot add Entry:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")            ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                  ' Display Alert Message
  Else
   If Not Trace.IsEnabled Then
    Response.Redirect("ListSelect.aspx")                     ' Return to Selection page
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
