
Partial Class Administration_ContactForm
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
 '* Contact Us page email management Add, Edit and Delete page.
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
  If Session("Access") < 2 Then                             ' Webadmin or SysAdmin access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("ContactForm", "Start Page Load")

  If Not IsPostBack Then                                    ' First time page accessed setup
   ' Define process unique objects here

   ' Get identity key
   If Len(Request("KeyID")) > 0 Then                        ' Update or Add Mode
    KeyID.Value = Request("KeyID")
   Else                                                     ' Critical error, cancel operation
    Response.Redirect("ContactSelect.aspx")
   End If

   ' Setup general page controls

   ' Fill Email Selection List
   Dim drEmails As MySqlDataReader
   SQLCmd = "Select Name,Parm2 " +
            "From control " +
            "Where Control='EmailAddr'"
   If Trace.IsEnabled Then Trace.Warn("ContactForm", "Get Emails List SQLCmd: " + SQLCmd.ToString())
   drEmails = MyDB.GetReader("MySite", SQLCmd)
   If drEmails.HasRows() Then
    While drEmails.Read()
     SendEmail.Items.Add(New ListItem(drEmails("Name").ToString().Trim(), drEmails("Parm2").ToString().Trim()))
    End While
   Else
    Session("ErrorMessage") = "Missing Website Control List, 'Email Addresses' (EmailAddr) email address list!"
    Response.Redirect("/Error.aspx")
   End If
   drEmails.Close()

   ' Display data fields based on edit or add mode
   If CDbl(KeyID.Value) > 0 Then                            ' Edit Mode, show database values
    Dim drApp As MySqlDataReader
    SQLCmd = "Select SendEmail,Title,Subject,AutoResponse,Active " +
             "From contactus " +
             "Where ContactID=" + MyDB.SQLNo(KeyID.Value)
    If Trace.IsEnabled Then Trace.Warn("ContactForm", "Get display Values SQLCmd: " + SQLCmd.ToString())
    drApp = MyDB.GetReader("MySite", SQLCmd)
    If drApp.Read() Then
     SendEmail.SelectedValue = drApp("SendEmail").ToString().Trim()
     Title.Text = drApp("Title").ToString().Trim()
     Subject.Text = drApp("Subject").ToString().Trim()
     AutoResponse.Value = drApp("AutoResponse").ToString().Trim()
     Active.Checked = drApp("Active")
     ShowActive.Visible = False
     Button4.Visible = (Not Active.Checked)                 ' Show Button
     Button5.Visible = (Active.Checked)                     ' Hide Button
     DelTitle.InnerText = "Entry: " + drApp("Title").ToString().Trim()
    End If
    drApp.Close()
    ' Setup Edit Mode page display controls
    PageTitle.InnerText = "Edit Contact"
    UpdDelBtn.Visible = True                                ' Allow Update and Delete button to show
    AddBtn.Visible = False                                  ' Disable the Add button
   Else                                                     ' Add Mode, show blank fields
    SendEmail.SelectedValue = ""
    Title.Text = ""
    Subject.Text = ""
    AutoResponse.Value = ""
    Active.Checked = False
    ' Setup Add Mode page display controls
    PageTitle.InnerText = "New Contact"
    UpdDelBtn.Visible = False                               ' Disable Update and Delete button
    AddBtn.Visible = True                                   ' Allow the Add button to show
   End If
   Title.Focus()                                            ' Set focus to the first field for entry

   Dim SBMenu As New TreeView
   ' Set up navigation options
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                  ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                   ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Other Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'ContactForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")      ' Program URL link entry
   SBMenu.AddItem("P", "ContactSelect.aspx", "Contact Select")
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   SBMenu.AddItem("B", "", "Blank Entry")
   If Trace.IsEnabled Then Trace.Warn("ContactForm", "Show Menu")
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
  If Title.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Contact Title!\r\n"
  End If
  If Subject.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Email Subject!\r\n"
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
   SQLCmd = "Update contactus Set " +
            "SendEmail=" + MyDB.SQLStr(SendEmail.SelectedValue) + "," + "Title=" + MyDB.SQLStr(Title.Text) + "," +
            "Subject=" + MyDB.SQLStr(Subject.Text) + "," + "AutoResponse=" + MyDB.SQLStr(AutoResponse.Value) + " " +
            "Where ContactID=" + MyDB.SQLNo(KeyID.Value)
   If Trace.IsEnabled Then Trace.Warn("ContactForm", "Update contactus SQLCmd: " + SQLCmd.ToString())
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
   BodyTag.Attributes.Add("onload", "ShowMsg();")           ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                 ' Display Alert Message
  End If

 End Sub

 ' Delete Button
 Private Sub SetDel_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SetDel.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("ContactForm", "Delete process Event")
  ' Action is triggered by a JavaScript process, not a button click.
  'Remove entry record
  SQLCmd = "Delete From contactus Where ContactID=" + MyDB.SQLNo(KeyID.Value)
  If Trace.IsEnabled Then Trace.Warn("ContactForm", "Delete contactus SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MySite", SQLCmd)
  If Not Trace.IsEnabled Then
   Response.Redirect("ContactSelect.aspx")                  ' Return to Selection page
  End If
 End Sub

 ' Add Button
 Private Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
  Dim SQLFields, SQLValues, tMsg As String
  tMsg = ValAddEdit(True)

  If tMsg.Length = 0 Then
   SQLFields = "SendEmail,Title,Subject,AutoResponse,Active"
   SQLValues = MyDB.SQLStr(SendEmail.SelectedValue) + "," + MyDB.SQLStr(Title.Text) + "," +
               MyDB.SQLStr(Subject.Text) + "," + MyDB.SQLStr(AutoResponse.Value) + "," +
               MyDB.SQLTF(Active.Checked)
   SQLCmd = "Insert Into contactus (" + SQLFields + ") Values (" + SQLValues + ")"
   If Trace.IsEnabled Then Trace.Warn("ContactForm", "Insert contactus SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   Else
    ' Resort entries
    'NOTE: MySQL 4.* cannot process single command update from a select.
    SQLCmd = "Set @Newsort=0;" +
             "Update contactus Set SortOrder=B.NewSort " +
             "From " +
             " (Select ContactID,@Newsort := @Newsort+1 as NewSort " +
             "  From contactus) as B INNER JOIN " +
             "   contactus ON B.ContactID = contactus.ContactID"
    'If Trace.IsEnabled Then Trace.Warn("ContactForm", "Get Resort portal entries SQLCmd: " + SQLCmd.ToString())
    'MyDB.DBCmd("MySite", SQLCmd)
    'If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("ContactForm", "DB Error: " + MyDB.ErrMessage().ToString())

    ' Reverting to the old, slow and working process
    Dim NewSort As Integer
    NewSort = 0
    Dim GetPRows As MySqlDataReader
    SQLCmd = "Select ContactID " +
             "From contactus " +
             "Order by SortOrder"
    If Trace.IsEnabled Then Trace.Warn("ContactSelect", "Get Page records SQLCmd: " + SQLCmd.ToString())
    GetPRows = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("ContactForm", "DB Error: " + MyDB.ErrMessage().ToString())
    If GetPRows.HasRows() Then
     While GetPRows.Read()
      NewSort = NewSort + 1
      SQLCmd = "Update contactus Set SortOrder=" + MyDB.SQLNo(NewSort) + " " +
               "Where ContactID=" + MyDB.SQLNo(GetPRows("ContactID"))
      If Trace.IsEnabled Then Trace.Warn("ContactForm", "Resort Entry SQLCmd: " + SQLCmd.ToString())
      MyDB.DBCmd("MySite", SQLCmd)
      If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("ContactForm", "DB Error: " + MyDB.ErrMessage().ToString())
     End While
    End If
    GetPRows.Close()
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
    Response.Redirect("ContactSelect.aspx")                 ' Return to Selection page
   End If
  End If
 End Sub

 ' Show entry
 Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

  SQLCmd = "Update contactus Set Active='true' " +
           "Where ContactID=" + MyDB.SQLNo(KeyID.Value)
  If Trace.IsEnabled Then Trace.Warn("ContactForm", "Update contactus SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("ContactForm", "DB error: " + MyDB.ErrMessage().ToString())
  Active.Checked = True
  Button4.Visible = (Not Active.Checked)                 ' Show Button
  Button5.Visible = (Active.Checked)                     ' Hide Button

 End Sub

 ' Hide entry
 Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click

  SQLCmd = "Update contactus Set Active='false' " +
           "Where ContactID=" + MyDB.SQLNo(KeyID.Value)
  If Trace.IsEnabled Then Trace.Warn("ContactForm", "Update contactus SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("ContactForm", "DB error: " + MyDB.ErrMessage().ToString())
  Active.Checked = False
  Button4.Visible = (Not Active.Checked)                 ' Show Button
  Button5.Visible = (Active.Checked)                     ' Hide Button

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub
End Class
