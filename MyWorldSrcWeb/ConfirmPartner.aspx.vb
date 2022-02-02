Partial Class ConfirmPartner
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
 '* Confirm Partner page provides the last step in establishing an inworld partnership between two people.
 '* 
 '* 

 '* Built from MyWorld Content Page template v. 1.0

 ' ********************************************
 '* This page is accessed from an email send by the ConfirmPartner.aspx page with the userID sent as a parameter.
 '* This page will create a new "secure" password for the designated Moonlight Grid account and send an email back to the 
 '* contact's email with the new password. Once they have that, they can logon to the website and replace it with a new one.
 ' ********************************************

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides MySQL data access methods and error handling
 Private PageCtl As New GridLib
 Private SQLCmd, SQLFields, SQLValues As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Start Page Load")

  Session("SSLStatus") = False
  ' Force SSL active if it is not and is required.
  If Request.ServerVariables("HTTPS") = "off" Then          ' Security is not active and is required
   If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Https is off")
   Dim drServer As MySqlDataReader
   SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='ServerLocation'"
   If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Get location SQLCmd: " + SQLCmd)
   drServer = MyDB.GetReader("MySite", SQLCmd)
   If drServer.Read() Then
    If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "ServerLocation = " + drServer("Parm2").ToString().Trim().ToUpper())
    If drServer("Parm2").ToString().Trim().ToUpper() = "LIVE" Then ' Security must be on if not testing
     drServer.Close()
     Session("SSLStatus") = True
     Response.Redirect(IIf(Session("SSLStatus"), "https://", "http://") + Request.ServerVariables("HTTP_HOST") + "/ConfirmPartner.aspx" + IIf(Request.QueryString().ToString().Length > 0, "?" + Request.QueryString().ToString(), ""))
    End If
   Else                                                     ' show error if not located
    If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Set https error: <br>Control value for 'Server Location' (ServerLocation) was not defined!")
    Session("ErrorMessage") = "Control value for 'Server Location' (ServerLocation) was not defined!"
    Response.Redirect("/Error.aspx")
   End If
   drServer.Close()
  End If

  If Not IsPostBack Then                                   ' First time page is called setup
   ' Define process unique objects here
   Dim tHtmlOut, tMsg As String
   tHtmlOut = ""
   tMsg = ""

   ' Define local objects here
   ' Setup general page controls
   ' Check if page is registered
   Dim drGetPage As MySqlDataReader
   SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
   If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Get Page Record Check: " + SQLCmd.ToString())
   drGetPage = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ConfirmPartner", "DB Error: " + MyDB.ErrMessage().ToString())
   If Not drGetPage.HasRows() Then
    drGetPage.Close()
    ' Create page entry
    SQLFields = "Name,Path"
    SQLValues = "'ConfirmPartner'," + MyDB.SQLStr(Request.ServerVariables("URL"))
    SQLCmd = "Insert into pagemaster (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
    If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Insert Page record: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ConfirmPartner", "DB Error: " + MyDB.ErrMessage().ToString())
    ' Reload PageID
    SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
    If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Reload Page RecordID: " + SQLCmd.ToString())
    drGetPage = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ConfirmPartner", "DB Error: " + MyDB.ErrMessage().ToString())
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
    If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Update Page AutoStart: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("ConfirmReset", "** DB Error: " + MyDB.ErrMessage().ToString())

    ' Check for Page display content
    Dim rsPage As MySqlDataReader
    SQLCmd = "Select Title,Content From pagedetail " +
             "Where Active=1 and PageID=" + MyDB.SQLNo(drGetPage("PageID"))
    If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Get Page Content: " + SQLCmd.ToString())
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

   tMsg = ""
   Dim UserUUID, UserUUID2 As String                        ' Prevent SQL injection content
   UserUUID = PageCtl.CleanStr(Request.QueryString("Part1").ToString(), 36) ' Requestor ID
   UserUUID2 = PageCtl.CleanStr(Request.QueryString("Part2"), 36)           ' Requestee ID

   ' Verify partnership is open and not expired / cancelled (Check on set parnership)
   Dim User As MySqlDataReader
   SQLCmd = "Select partner From users Where UUID=" + MyDB.SQLStr(UserUUID)
   If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Check users partner setting SQLCmd: " + SQLCmd.ToString())
   User = MyDB.GetReader("MyData", SQLCmd)
   User.Read()
   If User("partner").ToString().Trim() = "00000000-0000-0000-0000-000000000000" Then  ' OK to assign
    ' Update partner association
    SQLCmd = "Update users Set Partner=" + MyDB.SQLStr(UserUUID) +
             "Where UUID = " + MyDB.SQLStr(UserUUID2) + ";" +
             "Update users Set Partner=" + MyDB.SQLStr(UserUUID2) +
             "Where UUID = " + MyDB.SQLStr(UserUUID)
    If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Update users SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MyData", SQLCmd)

    ' Get Requestor Account record
    SQLCmd = "Select Concat(username,' ',lastname) as Name,email," +
             " (Select Concat(username,' ',lastname) as Name From users B Where UUID=users.partner) as PartnerName " +
             "From users " +
             "Where UUID=" + MyDB.SQLStr(UserUUID)
    User = MyDB.GetReader("MyData", SQLCmd)
    User.Read()

    Dim GetEmail As MySqlDataReader
    SQLCmd = "Select " +
             "(Select Parm2 From control Where Parm1='SMTPServer') as SMTPServer," +
             "Case When (Select Parm2 From control Where Parm1='SMTPPort') is null " +
             "Then '25' " +
             "Else (Select Parm2 From control Where Parm1='SMTPPort') " +
             "End as SMTPPort"
    If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Get SMTPServer SQLCmd: " + SQLCmd.ToString())
    GetEmail = MyDB.GetReader("MySite", SQLCmd)
    If GetEmail.HasRows() Then
     GetEmail.Read()
     Dim Domain As String
     Domain = ""
     Dim drDomain As MySqlDataReader
     SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='EmailDomain'"
     If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Get location SQLCmd: " + SQLCmd)
     drDomain = MyDB.GetReader("MySite", SQLCmd)
     If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ConfirmPartner", "DB Error: " + MyDB.ErrMessage().ToString())
     If drDomain.HasRows() Then
      drDomain.Read()
      Domain = drDomain("Parm2").ToString()
     End If
     drDomain.Close()
     ' Compose Confirmation Email to requestor
     Dim SendMail As New SendEmail
     SendMail.EmailServer = GetEmail("SMTPServer").ToString().Trim()
     If CInt(GetEmail("SMTPPort")) > 0 Then
      SendMail.Port = CInt(GetEmail("SMTPPort"))
     End If
     SendMail.FromAddress = "mailer@" + Domain.ToString()
     SendMail.ToAddress = User("email").ToString()
     SendMail.Subject = "Your My World Partnership Confirmation!"
     SendMail.Body = " " + vbCrLf +
             "Congratulations! " + User("PartnerName").ToString() + " has accepted your partnership offer!" + vbCrLf
     SendMail.IsHTML = False
     If Not SendMail.SendMail() Then
      If Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Email Failed: " + SendMail.ErrMessage.ToString())
      tMsg = SendMail.ErrMessage.ToString()
      SendMail.ToAddress = "support@" + Domain.ToString()
      SendMail.Subject = "Your My World Partnership Confirmation! - Failed to Send!"
      SendMail.Body = " " + vbCrLf + "Failed to send Partnership Confirmation email to member " + User("Email").ToString().Trim() + vbCrLf +
                      "Error Message: " + tMsg.ToString()
      SendMail.IsHTML = False
      SendMail.SendMail()
      If Not SendMail.SendMail() And Trace.IsEnabled Then Trace.Warn("ConfirmPartner", "Support Email Failed: " + SendMail.ErrMessage.ToString())
     Else
      ShowMsg.InnerText = "Congratulations! A Confirmation email has been sent to your partner!"
     End If
     SendMail.SendMail()
     SendMail.Close()
    End If
    GetEmail.Close()
   End If
   User.Close()
  End If

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub
End Class
