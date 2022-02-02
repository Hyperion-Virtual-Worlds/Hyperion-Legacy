Partial Class ContactUs
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
 '* Contact Us email page provides a way for website visitors to send an email to their selected entry,
 '* and not expose email addresses or use identity capcha systems. The best secured communication process
 '* for a website. Selections are set up in the Administration Contact Us Management program.

 '* Built from MyWorld Basic Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private PageCtl As New GridLib
 Private SQLCmd, SQLFields, SQLValues As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("ContactUs", "Start Page Load")

  If IsNothing(Session("SSLStatus")) Then
   Session("SSLStatus") = False
   ' Force SSL active if it is not and is required.
   If Request.ServerVariables("HTTPS") = "off" Then          ' Security is not active and is required
    If Trace.IsEnabled Then Trace.Warn("ContactUs", "Https is off")
    Dim drServer As MySqlDataReader
    SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='ServerLocation'"
    If Trace.IsEnabled Then Trace.Warn("ContactUs", "Get location SQLCmd: " + SQLCmd)
    drServer = MyDB.GetReader("MySite", SQLCmd)
    If drServer.Read() Then
     If Trace.IsEnabled Then Trace.Warn("ContactUs", "ServerLocation = " + drServer("Parm2").ToString().Trim().ToUpper())
     If drServer("Parm2").ToString().Trim().ToUpper() = "LIVE" Then ' Security must be on if not testing
      drServer.Close()
      Session("SSLStatus") = True
      Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + "/ContactUs.aspx")
     End If
    Else                                                     ' show error if not located
     If Trace.IsEnabled Then Trace.Warn("ContactUs", "Set https error: <br>Control value for 'Server Location' (ServerLocation) was not defined!")
     Session("ErrorMessage") = "Control value for 'Server Location' (ServerLocation) was not defined!"
     Response.Redirect("/Error.aspx")
    End If
    drServer.Close()
   End If
  ElseIf Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then          ' Security is not active and is required
   Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + "/ContactUs.aspx")
  End If

  If Not IsPostBack Then                                    ' First time page is called setup
   ' Setup general page controls
   ' define local objects here

   ' Check if page is registered
   Dim drGetPage As MySqlDataReader
   SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
   If Trace.IsEnabled Then Trace.Warn("ContactUs", "Get Page Record Check: " + SQLCmd)
   drGetPage = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ContactUs", "DB Error: " + MyDB.ErrMessage().ToString())
   If Not drGetPage.HasRows() Then
    ' Create page entry
    SQLFields = "Name,Path"
    SQLValues = "'Contact Us'," + MyDB.SQLStr(Request.ServerVariables("URL"))
    SQLCmd = "Insert into pagemaster (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
    If Trace.IsEnabled Then Trace.Warn("ContactUs", "Insert Page record: " + SQLCmd)
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ContactUs", "DB Error: " + MyDB.ErrMessage().ToString())
   End If
   drGetPage.Close()

   Call Display()

   If Not (Session.Count() = 0 Or Len(Session("UUID")) = 0) Then  ' Allow menu access while logged in
    Dim SBMenu As New TreeView
    ' Sidebar Options control based on Clearance or Write Access
    ' Set up navigation options
    If Trace.IsEnabled Then SBMenu.SetTrace = True
    'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
    'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
    'SBMenu.AddItem("T", "", "Other Options")                ' Title entry
    'SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry")        ' Javascript activated entry
    'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
    SBMenu.AddItem("P", "Account.aspx", "My Account")
    SBMenu.AddItem("P", "Logout.aspx", "Logout")
    SBMenu.AddItem("B", "", "Blank Entry")
    SBMenu.AddItem("T", "", "Page Options")
    If Session("Access") > 1 Then
     SBMenu.AddItem("P", "/Administration/Admin.aspx", "Website Administration")
    End If
    If Trace.IsEnabled Then Trace.Warn("ContactUs", "Show Menu")
    SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
    SBMenu.Close()
   End If
  End If

  ' Get Display list Items here

 End Sub

 Private Sub Display()
  Dim tHtmlOut As String
  tHtmlOut = ""

  ' Update any page settings for this page
  Dim drGetPage As MySqlDataReader
  SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
  drGetPage = MyDB.GetReader("MySite", SQLCmd)
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
   If Trace.IsEnabled Then Trace.Warn("ContactUs", "Update Page AutoStart: " + SQLCmd)
   MyDB.DBCmd("MySite", SQLCmd)

   ' Check for Page display content
   Dim rsPage As MySqlDataReader
   SQLCmd = "Select Title,Content From pagedetail " +
            "Where Active=1 and PageID=" + MyDB.SQLNo(drGetPage("PageID")) + " " +
            "Order by SortOrder"
   If Trace.IsEnabled Then Trace.Warn("ContactUs", "Get Page Content: " + SQLCmd)
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
  ContentDisp.Visible = (tHtmlOut.ToString().Trim().Length > 0)
  ShowContent.InnerHtml = tHtmlOut.ToString()

  ShowForm.Visible = False                                  ' Presume no addresses
  eID.Items.Clear()
  eID.Items.Add(New ListItem("Select Contact", "0"))
  ' Display Email Options if any
  Dim drGetEmails As MySqlDataReader
  SQLCmd = "Select ContactID,Title " +
           "From contactus " +
           "Where Active=1 " +
           "Order by SortOrder"
  If Trace.IsEnabled Then Trace.Warn("ContactUs", "Get Email Options: " + SQLCmd)
  drGetEmails = MyDB.GetReader("MySite", SQLCmd)
  If MyDB.Error Then
   If Trace.IsEnabled Then Trace.Warn("ContactUs", "DB Error: " + MyDB.ErrMessage().ToString())
  End If
  If drGetEmails.HasRows() Then
   ShowForm.Visible = True
   While drGetEmails.Read()
    eID.Items.Add(New ListItem(drGetEmails("Title"), drGetEmails("ContactID")))
   End While
  End If
  drGetEmails.Close()

 End Sub

 ' Send Email
 Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs)
  Dim tEmailOut, tMsg As String
  tEmailOut = ""
  tMsg = ""

  If eID.Value = 0 Then
   tMsg = "Contact was not Selected!"
  End If
  If Email.Value.ToString().Trim().Length = 0 Then
   tMsg = "Missing your email address!"
  Else
   If Not PageCtl.ValidEmail(Email.Value) Then
    tMsg = "Email is not valid!\r\n"
   End If
  End If
  If Comment.Value.ToString().Trim().Length = 0 Then
   tMsg = "Comment is missing!"
  End If
  If tMsg.ToString().Trim().Length = 0 Then                 ' Process email if no errors
   Dim drGetEmail As MySqlDataReader
   ' Get target email and response
   SQLCmd = "Select SendEmail,Subject," +
            " Case When AutoResponse is null Then '' Else Cast(AutoResponse as char) End as AutoResponse " +
            "From contactus " +
            "Where ContactID=" + MyDB.SQLNo(eID.Value)
   If Trace.IsEnabled Then Trace.Warn("ContactUs", "Get Target Email: " + SQLCmd.ToString())
   drGetEmail = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("ContactUs", "DB Error: " + MyDB.ErrMessage().ToString())
   drGetEmail.Read()

   Dim drServer As MySqlDataReader
   SQLCmd = "Select " +
            "(Select Parm2 From control Where Parm1='SMTPServer') as Server," +
            "(Select Parm2 From control Where Parm1='SMTPPort') as Port"
   If Trace.IsEnabled Then Trace.Warn("ContactUs", "Get SMTPServer SQLCmd: " + SQLCmd.ToString())
   drServer = MyDB.GetReader("MySite", SQLCmd)
   drServer.Read()

   Dim SendMail As New SendEmail
   SendMail.SetTrace = Trace.IsEnabled
   SendMail.IsHTML = False
   SendMail.EmailServer = drServer("Server").ToString().Trim()
   If drServer("Port") > 0 Then
    SendMail.Port = CInt(drServer("Port"))
   End If
   SendMail.Subject = drGetEmail("Subject").ToString().Trim()
   SendMail.ToAddress = drGetEmail("SendEmail").ToString().Trim()
   SendMail.FromAddress = Email.Value.ToString().Trim()
   SendMail.Body = Comment.Value.ToString()
   If Trace.IsEnabled Then Trace.Warn("ContactUs", "Email sent to " + drGetEmail("SendEmail").ToString().Trim() +
         " from " + Email.Value.ToString().Trim())
   If SendMail.SendMail() Then                              ' Send was successful
    tMsg = "Email was sent!"
    If Not drGetEmail("AutoResponse") Is DBNull.Value Then
     If drGetEmail("AutoResponse").ToString().Trim().Length > 0 Then
      SendMail.ToAddress = Email.Value.ToString().Trim()
      SendMail.FromAddress = drGetEmail("SendEmail").ToString()
      SendMail.Body = drGetEmail("AutoResponse").ToString()
      SendMail.SendMail()
     End If
    End If
    Comment.Value = ""                                      ' Clear message if sent
   Else
    If Trace.IsEnabled Then Trace.Warn("ContactUs", "Email Failed! Error: " + SendMail.ErrMessage().ToString())
    tMsg = "Send Email Failed! Error: " + SendMail.ErrMessage().ToString()
   End If
   SendMail.Close()
   SendMail = Nothing
   drServer.Close()
   drGetEmail.Close()
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   BodyTag.Attributes.Add("onload", "ShowMsg();")           ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                 ' Display Alert Message
  End If

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub
End Class
