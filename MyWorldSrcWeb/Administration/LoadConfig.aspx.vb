
Partial Class Administration_LoadConfig
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
 '* Provides a way to load Saved Website Configuration data. 
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
  If Trace.IsEnabled Then Trace.Warn("LoadConfig", "Start Page Load")

  If Not IsPostBack Then                                    ' First time page accessed setup
   ' Define process unique objects here

   ' Setup general page controls

   ' Display data fields based on edit or add mode
   UpdDelBtn.Visible = True                                 ' Allow Update and Delete button to show
   FileUpload1.Focus()                                      ' Set focus to the first field for entry

   Dim SBMenu As New TreeView
   ' Set up navigation options
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                  ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                   ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Other Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'LoadConfig.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")      ' Program URL link entry
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   If Trace.IsEnabled Then Trace.Warn("LoadConfig", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   ' Close Sidebar Menu object
   SBMenu.Close()
  End If

 End Sub

 ' Process Configuration data validation checks
 Private Function ValAddEdit(ByVal tAdd As Boolean) As String
  ' Parameter tAdd allows selections of Add mode only testing
  Dim aMsg, Fields() As String
  Fields = Session("TextLine").ToString().Split(Chr(9))
  aMsg = ""
  ' Process error checking as required, place messages in tMsg.
  If Fields(0).ToString().Trim().Length > 20 Then
   aMsg = aMsg.ToString() + "Control field content is too long! =" + Fields(0).ToString() + "\r\n"
  End If
  If Fields(1).ToString().Trim().Length > 30 Then
   aMsg = aMsg.ToString() + "Name field content is too long! =" + Fields(0).ToString() + "\r\n"
  End If
  If Fields(2).ToString().Trim().Length > 20 Then
   aMsg = aMsg.ToString() + "Parm1 field content is too long! =" + Fields(0).ToString() + "\r\n"
  End If
  If Fields(3).ToString().Trim().Length > 150 Then
   aMsg = aMsg.ToString() + "Parm2 field content is too long! =" + Fields(0).ToString() + "\r\n"
  End If
  If Not IsNumeric(Fields(4)) Then
   aMsg = aMsg.ToString() + "Nbr1 field is not numeric! =" + Fields(0).ToString() + "\r\n"
  Else
   If Fields(4).ToString().Contains(".") Then
    aMsg = aMsg.ToString() + "Nbr1 may not have decimal values! Integer only =" + Fields(0).ToString() + "\r\n"
   End If
  End If
  If Not IsNumeric(Fields(5)) Then
   aMsg = aMsg.ToString() + "Nbr2 field is not numeric! =" + Fields(0).ToString() + "\r\n"
  Else
   If Fields(5).ToString().Contains(".") Then
    aMsg = aMsg.ToString() + "Nbr2 may not have decimal values! Integer only =" + Fields(0).ToString() + "\r\n"
   End If
  End If
  'If FieldName.Text.ToString().Trim().Length = 0 Then
  ' aMsg = aMsg.ToString() + "Missing Field Name!\r\n"
  'End If
  Return aMsg
 End Function

 ' Load Button
 Private Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
  Dim tMsg, tLine, tSQL, tFields() As String
  tMsg = ""
  tSQL = ""
  If Not BodyTag.Attributes.Item("onload") Is Nothing Then  ' Remove onload error message display 
   BodyTag.Attributes.Remove("onload")
  End If

  If FileUpload1.FileName.ToString().Trim().Length > 0 Then
   If ClearFirst.Checked Then
    ClearFirst.Checked = False
    SQLCmd = "Delete From control"
    If Trace.IsEnabled Then Trace.Warn("ControlForm", "Delete control Table SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
   End If
   Dim FileExt As String
   FileExt = System.IO.Path.GetExtension(FileUpload1.FileName)
   If FileExt.ToString().ToLower() = ".txt" Then
    Dim FileReader As New System.IO.StreamReader(FileUpload1.PostedFile.InputStream)    ' Load selected file to FileReader
    tLine = FileReader.ReadLine()
    ' Test for header field names. If not match Control table fields or quantity it is invalid input
    ' Valid fields: Control,Name,Parm1,Parm2,Nbr1,Nbr2
    tFields = tLine.Split(Chr(9))
    If tFields(0).ToString().ToLower() = "control" And tFields(1).ToString().ToLower() = "name" And
       tFields(2).ToString().ToLower() = "parm1" And tFields(3).ToString().ToLower() = "parm2" And
       tFields(4).ToString().ToLower() = "nbr1" And tFields(5).ToString().ToLower() = "nbr2" Then
     While Not FileReader.EndOfStream()
      tLine = FileReader.ReadLine()
      Session("TextLine") = tLine.ToString()
      ' Validate field content lengths
      tMsg = ValAddEdit(False)
      If tMsg.ToString().Trim().Length = 0 Then
       PageCtl.LoadConfig(tLine, MyDB)                            ' Load configuration data
      End If
     End While
    Else
     ' invalid configuration file data
     tMsg = "Invalid configuration file!<br />"
    End If
   Else
    tMsg = "File selected was not a .txt file!<br />"
   End If
  Else
   tMsg = "No file was selected!<br />"
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot Iport data:<br />" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")           ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                 ' Display Alert Message
  Else
   MessageArea.InnerText = "Configuration File " + FileUpload1.FileName.ToString().Trim() + " was loaded! Check your settings in Control Values and Control List."
   UpdDelBtn.Visible = False
  End If

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub

End Class
