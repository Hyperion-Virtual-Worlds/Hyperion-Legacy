Partial Class ConfirmReset
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
 '* Confirm Reset Password is the last step in the process of getting an account password changed so 
 '* the account can be logged into and the password changed there for the user's choice of password.
 '* 

 '* Built from MyWorld Content Page template v. 1.0

 ' ********************************************
 '* This page is accessed from an email send by the ConfirmReset.aspx page with the userID sent as a parameter.
 '* This page will create a new "secure" password for the designated My World account and send an email back to the 
 '* contact's email with the new password. Once they have that, they can logon to the website and replace it with a new one.
 ' ********************************************

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private SQLCmd, SQLFields, SQLValues As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Start Page Load")

  Session("SSLStatus") = False
  ' Force SSL active if it is not and is required.
  If Request.ServerVariables("HTTPS") = "off" Then          ' Security is not active and is required
   If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Https is off")
   Dim drServer As MySqlDataReader
   SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='ServerLocation'"
   If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Get location SQLCmd: " + SQLCmd)
   drServer = MyDB.GetReader("MySite", SQLCmd)
   If drServer.Read() Then
    If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "ServerLocation = " + drServer("Parm2").ToString().Trim().ToUpper())
    If drServer("Parm2").ToString().Trim().ToUpper() = "LIVE" Then ' Security must be on if not testing
     drServer.Close()
     Session("SSLStatus") = True
     Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + "/ConfirmReset.aspx" + IIf(Request.QueryString().ToString().Length > 0, "?" + Request.QueryString().ToString(), ""))
    End If
   Else                                                     ' show error if not located
    If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Set https error: <br>Control value for 'Server Location' (ServerLocation) was not defined!")
    Session("ErrorMessage") = "Control value for 'Server Location' (ServerLocation) was not defined!"
    Response.Redirect("/Error.aspx")
   End If
   drServer.Close()
  End If

  If Not IsPostBack Then                                    ' First time page is called setup
   ' Define process unique objects here
   Dim tHtmlOut, tMsg As String
   tHtmlOut = ""
   tMsg = ""

   ' Define local objects here
   ' Setup general page controls
   ' Check if page is registered
   Dim drGetPage As MySqlDataReader
   SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
   If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Get Page Record Check: " + SQLCmd.ToString())
   drGetPage = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ConfirmReset", "DB Error: " + MyDB.ErrMessage().ToString())
   If Not drGetPage.HasRows() Then
    drGetPage.Close()
    ' Create page entry
    SQLFields = "Name,Path"
    SQLValues = "'ConfirmReset'," + MyDB.SQLStr(Request.ServerVariables("URL"))
    SQLCmd = "Insert into pagemaster (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
    If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Insert Page record: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ConfirmReset", "DB Error: " + MyDB.ErrMessage().ToString())
    ' Reload PageID
    SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
    If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Reload Page RecordID: " + SQLCmd.ToString())
    drGetPage = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ConfirmReset", "DB Error: " + MyDB.ErrMessage().ToString())
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
    If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Update Page AutoStart: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)

    ' Check for Page display content
    Dim rsPage As MySqlDataReader
    SQLCmd = "Select Title,Content From pagedetail " +
             "Where Active=1 and PageID=" + MyDB.SQLNo(drGetPage("PageID"))
    If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Get Page Content: " + SQLCmd.ToString())
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
   EmailFail.Visible = False

   tMsg = ""
   Dim UserName, Name() As String
   UserName = Request.QueryString("id").ToString()
   If UserName.ToString().Trim().Length > 0 Then
    Name = UserName.ToString().Split(" ")
    ' Get User Account record
    If Name(0).ToString().Trim().Length > 0 And Name(1).ToString().Trim().Length > 0 Then
     Dim Users As MySqlDataReader
     SQLCmd = "Select UUID,username,lastname,email,passwordSalt " +
              "From users " +
              "Where username=" + MyDB.SQLStr(Name(0)) + " and lastname=" + MyDB.SQLStr(Name(1))
     If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Get user data SQLCmd: " + SQLCmd.ToString())
     Users = MyDB.GetReader("MyData", SQLCmd)
     Users.Read()
     If Users("email").ToString().Trim() <> Users("passwordSalt").ToString().Trim() Then  ' Account is not blocked
      ' Create new password and update account
      Dim Password As String
      Password = System.IO.Path.GetRandomFileName().Replace(".", "")
      If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Password is: " + Password.ToString() + ", Len=" + Password.ToString().Length.ToString())

      Dim tPass, tSalt As String
      tSalt = "" '+ UserUUID.Replace("-", "").ToString()
      Dim GridLib As New GridLib
      tPass = GridLib.CodePassword(Password.ToString(), tSalt.ToString())
      GridLib.Close()

      SQLCmd = "Update users Set passwordHash=" + MyDB.SQLStr(tPass) + ",passwordSalt=" + MyDB.SQLStr(tSalt) + " " +
               "Where UUID=" + MyDB.SQLStr(Users("UUID"))
      If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Insert users SQLCmd: " + SQLCmd.ToString())
      MyDB.DBCmd("MyData", SQLCmd)
      If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("ConfirmReset", "** DB Error: " + MyDB.ErrMessage().ToString())

      ' Get system email content for a confirmation email.
      Dim tBody As String
      Dim GetEmail As MySqlDataReader
      SQLCmd = "Select " +
               "(Select Parm2 From control Where Parm1='SMTPServer') as SMTPServer," +
               "(Select Parm2 From control Where Parm1='SMTPPort') as SMTPPort"
      If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Get SMTPServer SQLCmd: " + SQLCmd.ToString())
      GetEmail = MyDB.GetReader("MySite", SQLCmd)
      If GetEmail.HasRows() Then
       GetEmail.Read()
       Dim Domain As String
       Domain = ""
       Dim drDomain As MySqlDataReader
       SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='EmailDomain'"
       If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Get location SQLCmd: " + SQLCmd)
       drDomain = MyDB.GetReader("MySite", SQLCmd)
       If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ConfirmReset", "DB Error: " + MyDB.ErrMessage().ToString())
       If drDomain.HasRows() Then
        drDomain.Read()
        Domain = drDomain("Parm2").ToString()
       End If
       drDomain.Close()
       tBody = "Greetings [%FirstName%],<br>" +
               "<p>Your password was reset to: [%Password%] </p>" +
               "<p>We recommend that you logon at <a href=""" + IIf(Session("SSLStatus"), "https://", "http://") + Request.ServerVariables("HTTP_HOST") + "/logon.aspx"">My World Logon</a> and change your password. </p>" +
               "My World Administration"
       tBody = tBody.ToString().Replace("[%FirstName%]", Users("username").ToString().Trim())
       tBody = tBody.ToString().Replace("[%Password%]", Password.ToString())
       If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Sending Email (Your My World Password was Reset!) to " + Users("Email").ToString().Trim())
       Dim SendMail As New SendEmail
       SendMail.EmailServer = GetEmail("SMTPServer").ToString().Trim()
       If GetEmail("SMTPPort").ToString().Trim().Length > 0 Then
        SendMail.Port = CInt(GetEmail("SMTPPort"))
       End If
       SendMail.FromAddress = "mailer@" + Domain.ToString()
       SendMail.ToAddress = Users("Email").ToString().Trim()
       SendMail.Subject = "Your My World Password was Reset!"
       SendMail.Body = " " + vbCrLf + tBody.ToString()
       SendMail.IsHTML = True
       If Not SendMail.SendMail() Then
        'EmailFail.Visible = True                                       ' Used when email process is not working to show on page.
        'NewPass.InnerText = Password.ToString()
        If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Email Failed: " + SendMail.ErrMessage.ToString())
        tMsg = SendMail.ErrMessage.ToString()
        SendMail.ToAddress = "support@" + Domain.ToString()
        SendMail.Subject = "Your My World Password was Reset! - Failed to Send!"
        SendMail.Body = " " + vbCrLf + "Failed to send New Password email to member " + Users("Email").ToString().Trim() + vbCrLf +
                       "Error Message: " + tMsg.ToString()
        SendMail.IsHTML = False
        Dim Sent As Boolean
        Sent = SendMail.SendMail()
        EmailFail.Visible = Not Sent
        If Not Sent Then
         If Trace.IsEnabled Then Trace.Warn("ConfirmReset", "Support Email Failed: " + SendMail.ErrMessage.ToString())
         NewPass.InnerText = Password.ToString()
        End If
       End If
       SendMail.Close()
       SendMail = Nothing
      End If
      GetEmail.Close()
     End If
     Users.Close()
    End If
   End If
  End If

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
