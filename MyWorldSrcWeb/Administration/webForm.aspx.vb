
Partial Class WebForm
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
 '* WebForm page provides access to add, edit or remove website display contents for all content enabled pages. 
 '* Entries may contain one or more paragraphs or content topics per site page or just provide some 
 '* friendly help, news or blogs.

 '* Built from MyWorld Form template v. 1.0

 ' Define common Page class properties and objects here for the page
 Private MyDB As New MySQLLib                                 ' Provides data access methods and error handling
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
  If Session("Access") < 2 Then                            ' Webadmin or SysAdmin access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("WebForm", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page accessed setup
   ' Define process unique objects here

   ' Get identity key
   If Len(Request("KeyID")) > 0 Then                       ' Update or Add Mode
    KeyID.Value = Request("KeyID")
   Else                                                    ' Critical error, cancel operation
    Response.Redirect("WebSelect.aspx")
   End If

   ' Setup general page controls

   ' Display data fields based on edit or add mode
   If CDbl(KeyID.Value) > 0 Then                           ' Edit Mode, show database values
    Dim drApp As MySqlDataReader
    SQLCmd = "Select Name,Title,Content,Active,AutoStart,AutoExpire " +
             "From pagedetail " +
             "Where EntryID=" + MyDB.SQLNo(KeyID.Value)
    If Trace.IsEnabled Then Trace.Warn("WebForm", "Get display Values SQLCmd: " + SQLCmd.ToString())
    drApp = MyDB.GetReader("MySite", SQLCmd)
    If drApp.Read() Then
     Name.Value = drApp("Name").ToString().Trim()
     Title.Value = drApp("Title").ToString().Trim()
     Content.Value = drApp("Content").ToString().Trim()
     Active.Checked = drApp("Active")
     If drApp("AutoStart").ToString().Trim().Length > 0 Then
      AutoStart.Value = FormatDateTime(drApp("AutoStart"), DateFormat.ShortDate)
     Else
      AutoStart.Value = ""
     End If
     If drApp("AutoExpire").ToString().Trim().Length > 0 Then
      AutoExpire.Value = FormatDateTime(drApp("AutoExpire"), DateFormat.ShortDate)
     Else
      AutoExpire.Value = ""
     End If
     DelTitle.InnerText = "Entry: " + drApp("Name").ToString().Trim()
    End If
    drApp.Close()
    ' Setup Edit Mode page display controls
    PageTitle.InnerText = "Edit Record"
    UpdDelBtn.Visible = True                               ' Allow Update and Delete button to show
    AddBtn.Visible = False                                 ' Disable the Add button
   Else                                                    ' Add Mode, show blank fields
    Name.Value = ""
    Title.Value = ""
    Content.Value = ""
    Active.Checked = False
    AutoStart.Value = ""
    AutoExpire.Value = ""
    ' Setup Add Mode page display controls
    PageTitle.InnerText = "New Record"
    UpdDelBtn.Visible = False                              ' Disable Update and Delete button
    AddBtn.Visible = True                                  ' Allow the Add button to show
   End If
   Name.Focus()                                            ' Set focus to the first field for entry

   Dim SBMenu As New TreeView
   ' Set up navigation options
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Other Options")                ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'WebForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
   SBMenu.AddItem("P", "WebSelect.aspx", "Page Select")
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   SBMenu.AddItem("B", "", "Blank Entry")
   If KeyID.Value > 0 Then                                  ' Edit Mode only option
    SBMenu.AddItem("T", "", "Page Options")
    SBMenu.AddItem("L", "ShowPreview();", "Preview Entry")
   End If
   If Trace.IsEnabled Then Trace.Warn("WebForm", "Show Menu")
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
  If Content.Value.ToString().Trim().Length > 2048 Then
   aMsg = aMsg.ToString() + "Content entry is too large! Max size is 2048 characters. Current size is " + Content.Value.ToString().Trim().Length.ToString() + ".\r\n"
  End If
  If Name.Value.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Entry Name!\r\n"
  End If
  If AutoStart.Value.ToString().Trim().Length > 0 Then
   If Not IsDate(AutoStart.Value) Then
    aMsg = aMsg.ToString() + "Auto start date is not a date value!\r\n"
   End If
  End If
  If AutoExpire.Value.ToString().Trim().Length > 0 Then
   If Not IsDate(AutoExpire.Value) Then
    aMsg = aMsg.ToString() + "Auto expire date is not a date value!\r\n"
   End If
  End If
  If AutoStart.Value.ToString().Trim().Length > 0 And AutoExpire.Value.ToString().Trim().Length > 0 Then
   If CDate(AutoStart.Value) >= CDate(AutoExpire.Value) Then
    aMsg = aMsg.ToString() + "Auto expire date must be greater than the auto Start date!\r\n"
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
   SQLCmd = "Update pagedetail Set " +
            "Name=" + MyDB.SQLStr(Name.Value) + "," + "Title=" + MyDB.SQLStr(Title.Value) + "," +
            "Content=" + MyDB.SQLStr(Content.Value) + "," + "Active=" + MyDB.SQLTF(Active.Checked) + "," +
            "AutoStart=" + MyDB.SQLDate(AutoStart.Value) + "," + "AutoExpire=" + MyDB.SQLDate(AutoExpire.Value) + " " +
            "Where EntryID=" + MyDB.SQLNo(KeyID.Value)
   If Trace.IsEnabled Then Trace.Warn("WebForm", "Update pagedetail SQLCmd: " + SQLCmd.ToString())
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

 ' Delete Button
 Private Sub SetDel_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SetDel.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("UserMaint", "Delete process Event")
  ' Action is triggered by a JavaScript process, not a button click.
  'Remove entry record
  SQLCmd = "Delete From pagedetail Where EntryID=" + MyDB.SQLNo(KeyID.Value)
  If Trace.IsEnabled Then Trace.Warn("WebForm", "Delete pagedetail SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MySite", SQLCmd)
  If Not Trace.IsEnabled Then
   Response.Redirect("WebSelect.aspx")                    ' Return to Selection page
  End If
 End Sub

 ' Add Button
 Private Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
  Dim SQLFields, SQLValues, tMsg As String
  tMsg = ValAddEdit(True)

  If tMsg.Length = 0 Then
   SQLFields = "PageID,SortOrder,Name,Title,Content,Active,AutoStart,AutoExpire"
   SQLValues = MyDB.SQLNo(Session("PageID")) + ",9999," +
               MyDB.SQLStr(Name.Value) + "," + MyDB.SQLStr(Title.Value) + "," +
               MyDB.SQLStr(Content.Value) + "," + MyDB.SQLTF(Active.Checked) + "," +
               MyDB.SQLDate(AutoStart.Value) + "," + MyDB.SQLDate(AutoExpire.Value)
   SQLCmd = "Insert Into pagedetail (" + SQLFields + ") Values (" + SQLValues + ")"
   If Trace.IsEnabled Then Trace.Warn("WebForm", "Insert pagedetail SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   Else
    ResortMenu()
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
    Response.Redirect("WebSelect.aspx")                   ' Return to Selection page
   End If
  End If
 End Sub

 ' Common Menu resort process
 Private Sub ResortMenu()
  'NOTE: MySQL 4.* cannot process single command update from a select.
  SQLCmd = "Set @Newsort=0;" +
           "Update pagedetail Set SortOrder=B.NewSort " +
           "From " +
           "(Select EntryID,@Newsort := @Newsort+1 as NewSort " +
           " From pagedetail " +
           " Where PageID=" + MyDB.SQLNo(Session("PageID")) + ") as B INNER JOIN " +
           "pagedetail ON pagedetail.EntryID = B.EntryID "
  'If Trace.IsEnabled Then Trace.Warn("PageSelect", "Resort Menu SQLCmd: " + SQLCmd.ToString())
  'MyDB.DBCmd("MySite", SQLCmd)
  'If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("WebForm", "DB Error: " + MyDB.ErrMessage().ToString())

  ' Reverting to the old, slow and working process
  Dim NewSort As Integer
  NewSort = 0
  Dim GetPRows As MySqlDataReader
  SQLCmd = "Select EntryID " +
           "From pagedetail " +
           "Where PageID=" + MyDB.SQLNo(Session("PageID")) + " " +
           "Order by SortOrder"
  If Trace.IsEnabled Then Trace.Warn("WebSelect", "Get Page records SQLCmd: " + SQLCmd.ToString())
  GetPRows = MyDB.GetReader("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("WebForm", "DB Error: " + MyDB.ErrMessage().ToString())
  If GetPRows.HasRows() Then
   While GetPRows.Read()
    NewSort = NewSort + 1
    SQLCmd = "Update pagedetail Set SortOrder=" + MyDB.SQLNo(NewSort) + " " +
             "Where EntryID=" + MyDB.SQLNo(GetPRows("EntryID"))
    If Trace.IsEnabled Then Trace.Warn("WebForm", "Resort Entry SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("WebForm", "DB Error: " + MyDB.ErrMessage().ToString())
   End While
  End If
  GetPRows.Close()

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub

End Class
