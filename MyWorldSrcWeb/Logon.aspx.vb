
Partial Class Logon
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
 '* User website logon page. Provides access to any website tools the user account is authorized to 
 '* use in the website. Sets up access rules and settings per account settings.
 '* 

 '* Built from MyWorld Form template v. 1.0

 ' Define common Page class properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private PageCtl As New GridLib
 Private SQLCmd, SQLFields, SQLValues As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("Logon", "Start Page Load")

  If IsNothing(Session("SSLStatus")) Then
   Session("SSLStatus") = False
   ' Force SSL active if it is not and is required.
   If Request.ServerVariables("HTTPS") = "off" Then         ' Security is not active and is required
    If Trace.IsEnabled Then Trace.Warn("Logon", "Https is off")
    Dim drServer As MySqlDataReader
    SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='ServerLocation'"
    If Trace.IsEnabled Then Trace.Warn("Logon", "Get location SQLCmd: " + SQLCmd)
    drServer = MyDB.GetReader("MySite", SQLCmd)
    If drServer.Read() Then
     If Trace.IsEnabled Then Trace.Warn("Logon", "ServerLocation = " + drServer("Parm2").ToString().Trim().ToUpper())
     If drServer("Parm2").ToString().Trim().ToUpper() = "LIVE" Then ' Security must be on if not testing
      drServer.Close()
      Session("SSLStatus") = True
      Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + "/Logon.aspx")
     End If
    Else                                                    ' show error if not located
     If Trace.IsEnabled Then Trace.Warn("Logon", "Set https error: <br>Control value for 'Server Location' (ServerLocation) was not defined!")
     Session("ErrorMessage") = "Control value for 'Server Location' (ServerLocation) was not defined!"
     Response.Redirect("/Error.aspx")
    End If
    drServer.Close()
   End If
  ElseIf Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + "/Logon.aspx")
  End If

  If Not IsPostBack Then                                    ' First time page accessed setup
   ' Define process unique objects here
   Dim tHtmlOut As String
   tHtmlOut = ""

   ' Setup general page controls
   Dim Domain As String
   Domain = ""
   Dim drDomain As MySqlDataReader
   SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='EmailDomain'"
   If Trace.IsEnabled Then Trace.Warn("Logon", "Get location SQLCmd: " + SQLCmd)
   drDomain = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Logon", "DB Error: " + MyDB.ErrMessage().ToString())
   If drDomain.HasRows() Then
    drDomain.Read()
    Domain = drDomain("Parm2").ToString()
   Else
    Session("ErrorMessage") = "Control value for 'Email Domain Name' (EmailDomain) was not defined!"
    Response.Redirect("/Error.aspx")
   End If
   drDomain.Close()
   Session("Domain") = Domain.ToString()                    ' Domain for email addresses

   ' Check if page is registered
   Dim drGetPage As MySqlDataReader
   SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
   If Trace.IsEnabled Then Trace.Warn("Logon", "Get Page Record Check: " + SQLCmd.ToString())
   drGetPage = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Logon", "DB Error: " + MyDB.ErrMessage().ToString())
   If Not drGetPage.HasRows() Then
    drGetPage.Close()
    ' Create page entry
    SQLFields = "Name,Path"
    SQLValues = "'Logon'," + MyDB.SQLStr(Request.ServerVariables("URL"))
    SQLCmd = "Insert into pagemaster (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
    If Trace.IsEnabled Then Trace.Warn("Logon", "Insert Page record: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Logon", "DB Error: " + MyDB.ErrMessage().ToString())
    ' Reload PageID
    SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
    If Trace.IsEnabled Then Trace.Warn("Logon", "Reload Page RecordID: " + SQLCmd.ToString())
    drGetPage = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Logon", "DB Error: " + MyDB.ErrMessage().ToString())
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
    If Trace.IsEnabled Then Trace.Warn("Logon", "Update Page AutoStart: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)

    ' Check for Page display content
    Dim rsPage As MySqlDataReader
    SQLCmd = "Select Title,Content From pagedetail " +
             "Where Active=1 and PageID=" + MyDB.SQLNo(drGetPage("PageID")) + " " +
             "Order by SortOrder"
    If Trace.IsEnabled Then Trace.Warn("Logon", "Get Page Content: " + SQLCmd.ToString())
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
   'tHtmlOut = Guid.NewGuid().ToString()
   If tHtmlOut.ToString().Trim().Length > 0 Then
    ShowContent.InnerHtml = tHtmlOut.ToString()
   End If

   ' Display logon data fields
   FirstName.Value = ""
   LastName.Value = ""
   Password.Value = ""
   ' Setup Add Mode page display controls
   UpdDelBtn.Visible = True                                 ' Allow the Logon button to show
   FirstName.Focus()                                        ' Set focus to the first field for entry

  End If

 End Sub

 ' Process data validation checks
 Private Function ValAddEdit(ByVal tAdd As Boolean) As String
  ' Parameter tAdd allows selections of Add mode only testing
  Dim aMsg As String
  aMsg = ""
  ' Process error checking as required, place messages in tMsg.
  If FirstName.Value.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Field First Name!\r\n"
  End If
  If LastName.Value.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Field Last Name!\r\n"
  End If
  If Password.Value.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Field Password!\r\n"
  End If
  'If FieldName.Text.ToString().Trim().Length = 0 Then
  ' aMsg = aMsg.ToString() + "Missing Field Name!\r\n"
  'End If
  Return aMsg
 End Function

 ' Logon Button
 Private Sub Button1_ServerClick(sender As Object, e As EventArgs) Handles Button1.ServerClick
  Dim tMsg As String
  tMsg = ValAddEdit(True)
  If Not BodyTag.Attributes.Item("onload") Is Nothing Then  ' Remove onload error message display 
   BodyTag.Attributes.Remove("onload")
  End If
  If tMsg.Length = 0 Then
   Dim tPass As String
   ' Verify Account access
   Dim users As MySqlDataReader
   SQLCmd = "Select UUID,username,lastname,passwordSalt,passwordHash " +
            "From users " +
            "Where username=" + MyDB.SQLStr(FirstName.Value) + " and lastname=" + MyDB.SQLStr(LastName.Value)
   If Trace.IsEnabled Then Trace.Warn("Logon", "Get User SQLCmd: " + SQLCmd.ToString())
   users = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   End If
   If users.HasRows() Then
    users.Read()
    tPass = PageCtl.CodePassword(Password.Value.ToString(), users("passwordSalt"))
    If Trace.IsEnabled Then Trace.Warn("Logon", "PasswordHash: " + users("passwordHash").ToString() + ", input: " + tPass.ToString())
    If tPass = users("passwordHash") Then
     Session("UUID") = users("UUID").ToString().Trim()
     Session("FirstName") = users("username").ToString().Trim()
     Session("SSLStatus") = (Request.ServerVariables("HTTPS") = "on")
     Session("Access") = 1                                  ' Anyone logon access
     Session("ELevel") = 1                                  ' Default to no economy
     ' Check if Economy control records exists
     Dim Economy As MySqlDataReader
     SQLCmd = "Select Nbr2 " +
              "From control " +
              "Where Control='ECONOMY' and Parm1='HasEconomy'"
     If Trace.IsEnabled Then Trace.Warn("Economy", "Check for Economy SQLCmd: " + SQLCmd.ToString())
     Economy = MyDB.GetReader("MySite", SQLCmd)
     If Economy.HasRows() Then
      Economy.Read()
      Session("ELevel") = Economy("Nbr2")
     End If
     Economy.Close()
     ' Get list of web access accounts
     Dim SysAccts As MySqlDataReader
     SQLCmd = "Select Nbr2 From control " +
              "Where Control in ('AccessList','GridSysAccounts','WorldBankAcct') and (Parm2=" +
              MyDB.SQLStr(users("username").ToString().Trim() + " " + users("lastname").ToString().Trim()) + " Or Name=" +
              MyDB.SQLStr(users("username").ToString().Trim() + " " + users("lastname").ToString().Trim()) + ")"
     If Trace.IsEnabled Then Trace.Warn("Logon", "Get WebAccess SQLCmd: " + SQLCmd.ToString())
     SysAccts = MyDB.GetReader("MySite", SQLCmd)
     If MyDB.Error() Then
      If Trace.IsEnabled Then Trace.Warn("Logon", "DB Error: " + MyDB.ErrMessage().ToString())
      tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
     End If
     If SysAccts.HasRows() Then
      SysAccts.Read()
      Session("Access") = SysAccts("Nbr2")
     Else                                                   ' Website initial installation setup account for Grid Owner
      ' Get account name for Grid Owner in Control.
      Dim MAvatar As MySqlDataReader
      SQLCmd = "Select Parm2 " +
               "From control " +
               "Where Control='GridSysAccounts' and Parm1='GridOwnerAcct'"
      If Trace.IsEnabled Then Trace.Warn("Economy", "Check for Economy SQLCmd: " + SQLCmd.ToString())
      MAvatar = MyDB.GetReader("MySite", SQLCmd)
      If MAvatar.HasRows() Then
       MAvatar.Read()
       If users("username").ToString().Trim() + " " + users("lastname").ToString().Trim() = MAvatar("Parm2").ToString().Trim() Then
        Session("Access") = 9                               ' SysAdmin Logon access
       End If
      End If
      MAvatar.Close()
     End If
     SysAccts.Close()

     ' Presume no economy settings
     Session("MaxBuy") = 0
     Session("MaxSell") = 0
     Session("Hours") = 0

     If Session("ELevel") = 3 Then                          ' Economy is active
      ' Get Exchange rate and fee
      Dim GetSettings As MySqlDataReader
      SQLCmd = "Select " +
               " (Select Nbr2 From control " +
               "  Where Control='ECONOMY' and Parm1='MaxBuy') as MaxBuy," +
               " (Select Nbr2 From control " +
               "  Where Control='ECONOMY' and Parm1='MaxSell') as MaxSell," +
               " (Select Nbr2 From control " +
               "  Where Control='ECONOMY' and Parm1='BuySellTime') as BuySellTime"
      If Trace.IsEnabled Then Trace.Warn("Logon", "Get Control settings SQLCmd: " + SQLCmd.ToString())
      GetSettings = MyDB.GetReader("MySite", SQLCmd)
      If GetSettings.Read() Then
       Session("MaxBuy") = GetSettings("MaxBuy")
       Session("MaxSell") = GetSettings("MaxSell")
       Session("Hours") = GetSettings("BuySellTime")
       If Trace.IsEnabled Then Trace.Warn("Logon", "** Default Settings: MaxBuy=" + Session("MaxBuy").ToString() + ", Hours=" + Session("Hours").ToString())
      End If
      If Trace.IsEnabled Then Trace.Warn("Logon", "** Fee: " + Session("MaxBuy").ToString())
      GetSettings.Close()

      ' Get User account override limits
      Dim GetLimits As MySqlDataReader
      SQLCmd = "Select MaxBuy,MaxSell,Hours " +
               "From usereconomy " +
               "Where UUID=" + MyDB.SQLStr(Session("UUID"))
      If Trace.IsEnabled Then Trace.Warn("Logon", "Get usereconomy SQLCmd: " + SQLCmd.ToString())
      GetLimits = MyDB.GetReader("MySite", SQLCmd)
      If GetLimits.HasRows() Then                           ' Override Economy Default limits with user settings
       GetLimits.Read()
       Session("MaxBuy") = GetLimits("MaxBuy")
       Session("MaxSell") = GetLimits("MaxSell")
       Session("Hours") = GetLimits("Hours")
       If Trace.IsEnabled Then Trace.Warn("Logon", "**Account override Settings: MaxBuy=" + Session("MaxBuy").ToString() + ", Hours=" + Session("Hours").ToString())
      Else                                                  ' Record was not found, create it
       SQLCmd = "Insert into usereconomy (UUID,MaxBuy,MaxSell,Hours) " +
                "Values (" + MyDB.SQLStr(Session("UUID")) + "," + MyDB.SQLNo(Session("MaxBuy")) + "," +
                MyDB.SQLNo(Session("MaxSell")) + "," + MyDB.SQLNo(Session("Hours")) + ")"
       If Trace.IsEnabled Then Trace.Warn("Logon", "Insert usereconomy SQLCmd: " + SQLCmd.ToString())
       MyDB.DBCmd("MySite", SQLCmd)
      End If
      GetLimits.Close()
     End If
    Else                                                    ' password hash fail
     tMsg = "Logon failed! "
    End If
   Else
    tMsg = "User account was not found! "
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot logon:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")           ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                 ' Display Alert Message
  Else
   If Trace.IsEnabled Then Trace.Warn("Logon", "** Go to Account page")
   If Not Trace.IsEnabled Then
    Response.Redirect("Account.aspx")                       ' Continue to Selection page
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
