
Partial Class ResetPass
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
 '* The ResetPass page starts the process to change a users account password, by initialting the request
 '* to be confirmed by email to the account's email address. Only when the ConfirmReset page is opened 
 '* from the email link, is the new password created and applied to the account and is emailed back to the
 '* user. Then they may logon and change the password to something they can remember.

 '* Built from MyWorld Basic Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private GridLib As New GridLib
 Private SQLCmd, SQLFields, SQLValues As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  If Session.Count() = 0 Then
   Response.Redirect("/Default.aspx")                       ' Return to logon page
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("/Default.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("ResetPass", "Start Page Load")

  If Not IsPostBack Then                                    ' First time page is called setup
   ' Define process unique objects here
   Dim tHtmlOut As String
   tHtmlOut = ""

   ' Define local objects here

   ' Check if page is registered
   Dim drGetPage As MySqlDataReader
   SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
   If Trace.IsEnabled Then Trace.Warn("ResetPass", "Get Page Record Check: " + SQLCmd)
   drGetPage = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ResetPass", "DB Error: " + MyDB.ErrMessage().ToString())
   If Not drGetPage.HasRows() Then
    drGetPage.Close()
    ' Create page entry
    SQLFields = "Name,Path"
    SQLValues = "'ResetPass'," + MyDB.SQLStr(Request.ServerVariables("URL"))
    SQLCmd = "Insert into pagemaster (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
    If Trace.IsEnabled Then Trace.Warn("ResetPass", "Insert Page record: " + SQLCmd)
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ResetPass", "DB Error: " + MyDB.ErrMessage().ToString())
    ' Reload PageID
    SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
    If Trace.IsEnabled Then Trace.Warn("ResetPass", "Reload Page RecordID: " + SQLCmd)
    drGetPage = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ResetPass", "DB Error: " + MyDB.ErrMessage().ToString())
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
    If Trace.IsEnabled Then Trace.Warn("ResetPass", "Update Page AutoStart: " + SQLCmd)
    MyDB.DBCmd("MySite", SQLCmd)

    ' Check for Page display content
    Dim rsPage As MySqlDataReader
    SQLCmd = "Select Title,Content From pagedetail " +
             "Where Active=1 and PageID=" + MyDB.SQLNo(drGetPage("PageID")) + " and " +
             " Name='Reset Password Introduction' "
    If Trace.IsEnabled Then Trace.Warn("ResetPass", "Get Page Content: " + SQLCmd)
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
   ShowContent.InnerHtml = tHtmlOut.ToString()
   FirstName.Focus()                                        ' Set focus to the first field for entry

   ' Setup general page controls

   ' Set up navigation options
   Dim SBMenu As New TreeView
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                  ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                   ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Other Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")      ' Program URL link entry

   'SBMenu.AddItem("B", "", "Blank Entry")
   'SBMenu.AddItem("T", "", "Other Options")
   'SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/TempSelect.aspx", "Template Selection")
   If Trace.IsEnabled Then Trace.Warn("ResetPass", "Show Menu")
   'SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   SBMenu.Close()
  End If

  ' Get Display list Items here

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
  If Email.Value.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Email address!\r\n"
  End If
  If aMsg.ToString().Trim().Length = 0 Then
   ' See if account username is already in use
   Dim ChkUser As MySqlDataReader
   SQLCmd = "Select UUID " +
            "From users " +
            "Where username=" + MyDB.SQLStr(FirstName.Value) + " and lastname=" + MyDB.SQLStr(LastName.Value)
   If Trace.IsEnabled Then Trace.Warn("ResetPass", "Check User Exists, SQLCmd: " + SQLCmd.ToString())
   ChkUser = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    If Trace.IsEnabled Then Trace.Warn("ResetPass", "MyDB Error: " + MyDB.ErrMessage().ToString())
   Else
    If ChkUser.Read() Then
    Else
     aMsg = aMsg.ToString() + "User Name Was not found!\r\n"
    End If
   End If
   ChkUser.Close()
  End If
  'If FieldName.Text.ToString().Trim().Length = 0 Then
  ' aMsg = aMsg.ToString() + "Missing Field Name!\r\n"
  'End If
  Return aMsg
 End Function

 ' Button Click
 Private Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
  Dim tMsg As String
  tMsg = ValAddEdit(True)

  If tMsg.Length = 0 Then
   ' Check for User Account
   Dim Users As MySqlDataReader
   SQLCmd = "Select UUID,Concat(username,' ',lastname) as Name,email,passwordSalt " +
            "From users " +
            "Where username=" + MyDB.SQLStr(FirstName.Value) + " and lastname=" + MyDB.SQLStr(LastName.Value)
   If Trace.IsEnabled Then Trace.Warn("ResetPass", "Get User Record SQLCmd: " + SQLCmd.ToString())
   Users = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    tMsg = tMsg.ToString() + "Get users account MyDB. Error: " + MyDB.ErrMessage() + "\r\n" +
             "SQLCmd: " + SQLCmd.ToString() + "\r\n"
   End If
   If Users.HasRows() Then
    Users.Read()
    If Users("Email").ToString().Trim() <> Users("passwordSalt").ToString().Trim() Then ' Account is not blocked proceed
     If Email.Value.ToString().Trim().ToLower() = Users("Email").ToString().Trim().ToLower() Then
      ' Get system email content for a confirmation email.
      Dim GetEmail As MySqlDataReader
      SQLCmd = "Select " +
               "(Select Parm2 From control Where Parm1='SMTPServer') as SMTPServer," +
               "(Select Parm2 From control Where Parm1='SMTPPort') as SMTPPort"
      If Trace.IsEnabled Then Trace.Warn("ResetPass", "Get SMTPServer SQLCmd: " + SQLCmd.ToString())
      GetEmail = MyDB.GetReader("MySite", SQLCmd)
      If GetEmail.HasRows() Then
       GetEmail.Read()
       Dim Domain As String
       Domain = ""
       Dim drDomain As MySqlDataReader
       SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='EmailDomain'"
       If Trace.IsEnabled Then Trace.Warn("ResetPass", "Get location SQLCmd: " + SQLCmd)
       drDomain = MyDB.GetReader("MySite", SQLCmd)
       If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ResetPass", "DB Error: " + MyDB.ErrMessage().ToString())
       If drDomain.HasRows() Then
        drDomain.Read()
        Domain = drDomain("Parm2").ToString()
       End If
       drDomain.Close()
       Dim tBody As String
       tBody = "<p>A request was sent for a password reset on your My World account. " +
               "Please <a href=""" + IIf(Session("SSLStatus"), "https://", "http://") + Request.ServerVariables("HTTP_HOST") + "/ConfirmReset.aspx?id=[%Name%]"">confirm</a> " +
               "this request. Once confirmed, you will be emailed your new password.</p> " +
               "Thank You!"
       tBody = tBody.ToString().Replace("[%Name%]", Users("Name").ToString())
       If Trace.IsEnabled Then Trace.Warn("ResetPass", "Sending Email (My World Password Reset Email) to " + Email.Value.ToString().Trim() + " From mailer@" + Domain.ToString())
       Dim SendMail As New SendEmail
       SendMail.SetTrace = Trace.IsEnabled
       SendMail.EmailServer = GetEmail("SMTPServer").ToString().Trim()
       If GetEmail("SMTPPort").ToString().Trim().Length > 0 Then
        SendMail.Port = CInt(GetEmail("SMTPPort"))
       End If
       SendMail.FromAddress = "mailer@" + Domain.ToString()
       SendMail.ToAddress = Users("Email").ToString().Trim()
       SendMail.Subject = "My World Password Reset Email"
       SendMail.Body = " " + vbCrLf + tBody.ToString()
       SendMail.IsHTML = True
       If Not SendMail.SendMail() Then
        '' Temp fix for when email service does not work:
        'Response.Redirect("ConfirmReset.aspx?id=" + Users("Name").ToString())
        If Trace.IsEnabled Then Trace.Warn("ResetPass", "Email Failed: " + SendMail.ErrMessage.ToString())
        tMsg = SendMail.ErrMessage.ToString()
        SendMail.ToAddress = "Support@" + Domain.ToString()
        SendMail.Subject = "My World Password Reset Email - Failed to Send!"
        SendMail.Body = " " + vbCrLf + "Failed to send Confirmation email to member " + Users("Email").ToString().Trim() + vbCrLf +
                        "Error Message: " + tMsg.ToString()
        SendMail.IsHTML = False
        If Not SendMail.SendMail() And Trace.IsEnabled Then Trace.Warn("ResetPass", "Support Email Failed: " + SendMail.ErrMessage.ToString())
       End If
       SendMail.Close()
       SendMail = Nothing
      End If
      GetEmail.Close()
     Else
      tMsg = "Entered email address does not match the account email."
     End If
    Else                                                 ' Account is blocked, disallow password reset.
     tMsg = "Account was not found."
    End If
   Else
    tMsg = "No account was found for the First and Last names entered."
   End If
   Users.Close()
  End If

  If tMsg.Length > 0 Then
   tMsg = "Cannot process reset!:\r\n" + tMsg.ToString()
   BodyTag.Attributes.Add("onload", "ShowMsg();")           ' Activate onload option to show message
   GridLib.AlertMessage(Me, tMsg, "ErrMsg")                 ' Display Alert Message
  Else
   Dim tHtmlOut As String
   tHtmlOut = ""
   ' Change Screen Message
   FormDisp.Visible = False
   ' Show closing message
   Dim drGetPage As MySqlDataReader
   SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
   If Trace.IsEnabled Then Trace.Warn("ResetPass", "Get Page Record Check: " + SQLCmd)
   drGetPage = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ResetPass", "DB Error: " + MyDB.ErrMessage().ToString())
   If drGetPage.HasRows() Then
    drGetPage.Read()
    Dim rsPage As MySqlDataReader
    SQLCmd = "Select Title,Content From pagedetail " +
             "Where Active=1 and PageID=" + MyDB.SQLNo(drGetPage("PageID")) + " and " +
             " Name='Password Reset Confirmation' "
    If Trace.IsEnabled Then Trace.Warn("ResetPass", "Get Page Content: " + SQLCmd.ToString())
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
     ShowContent.InnerHtml = tHtmlOut.ToString()
    End If
    rsPage.Close()
   End If
   drGetPage.Close()
  End If

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  GridLib = Nothing
 End Sub
End Class
