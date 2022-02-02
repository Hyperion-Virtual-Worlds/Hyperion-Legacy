Partial Class _Default
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
 '* This is the website default / home page. Content to display is managed by the Website Page Content 
 '* Management program in the Administration pages.
 '* 

 '* Built from MyWorld Basic Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private PageCtl As New GridLib
 Private SQLCmd, SQLFields, SQLValues As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  'Response.Redirect("Info.aspx")
  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("Default", "Start Page Load")

  ' Check for startup configuration file, if it exists, load it, then remove it.
  Dim FileName, tLine, tMsg, tFields() As String
  FileName = Server.MapPath("").ToString() + "\FirstConfig.txt"
  If Trace.IsEnabled Then Trace.Warn("Default", "Checking for Configuration: " + FileName.ToString())
  If System.IO.File.Exists(FileName) Then              ' load file if it exists
   If Trace.IsEnabled Then Trace.Warn("Default", "Loading Configuration: " + FileName.ToString())
   Dim FileReader As New System.IO.StreamReader(FileName) ' Create Style sheet file access
   tLine = FileReader.ReadLine()
   ' Test for header field names. If not match Control table fields or quantity it is invalid input
   ' Valid fields: Control,Name,Parm1,Parm2,Nbr1,Nbr2
   tFields = tLine.Split(Chr(9))
   If tFields(0).ToString().ToLower() = "control" And tFields(1).ToString().ToLower() = "name" And
      tFields(2).ToString().ToLower() = "parm1" And tFields(3).ToString().ToLower() = "parm2" And
      tFields(4).ToString().ToLower() = "nbr1" And tFields(5).ToString().ToLower() = "nbr2" Then
    tMsg = ""
    While Not FileReader.EndOfStream()
     tLine = FileReader.ReadLine()
     Session("TextLine") = tLine.ToString()
     ' Validate field content lengths
     tMsg = tMsg + ValAddEdit(False)
     If tMsg.ToString().Trim().Length = 0 Then
      PageCtl.LoadConfig(tLine, MyDB)                  ' Load configuration data
     End If
    End While
    FileReader.Close()
    System.IO.File.Delete(FileName)                    ' Remove first time configuration file
   Else
    ' invalid configuration file data
    tMsg = "Invalid configuration file! FirstConfig.txt <br />"
   End If
   If tMsg.ToString().Trim().Length > 0 Then
    Session("ErrorMessage") = tMsg.ToString()
    Response.Redirect("/Error.aspx")
   End If
  End If

  Session("SSLStatus") = False
  ' Force SSL active if it is not and is required.
  If Request.ServerVariables("HTTPS") = "off" Then          ' Security is not active and is required
   If Trace.IsEnabled Then Trace.Warn("Default", "Https is off")
   Dim drServer As MySqlDataReader
   SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='ServerLocation'"
   If Trace.IsEnabled Then Trace.Warn("Default", "Get location SQLCmd: " + SQLCmd)
   drServer = MyDB.GetReader("MySite", SQLCmd)
   If drServer.Read() Then
    If Trace.IsEnabled Then Trace.Warn("Default", "ServerLocation = " + drServer("Parm2").ToString().Trim().ToUpper())
    If drServer("Parm2").ToString().Trim().ToUpper() = "LIVE" Then ' Security must be on if not testing
     drServer.Close()
     Session("SSLStatus") = True
     Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + "/Default.aspx")
    End If
   Else                                                     ' show error if not located
    If Trace.IsEnabled Then Trace.Warn("Default", "Set https error: <br>Control value for 'Server Location' (ServerLocation) was not defined!")
    Session("ErrorMessage") = "Control value for 'Server Location' (ServerLocation) was not defined!"
    Response.Redirect("/Error.aspx")
   End If
   drServer.Close()
  End If

  If Not IsPostBack Then                                    ' First time page is called setup
   Dim tHtmlOut As String
   tHtmlOut = ""

   ' Setup general page controls

   ' define local objects here
   GridStats.InnerHtml = "<span class=""Errors"">offline</span>"
   GridTUsers.InnerText = "0"
   GridOnline.InnerText = "0"
   GridRegions.InnerText = "0"
   PubRegions.InnerText = "0"
   PrivRegions.InnerText = "0"

   ' Get Grid Status Controls
   Dim StatusURL As String
   Dim RegCount As Integer
   StatusURL = ""                                           ' Set in website Control List
   RegCount = 0

   ' Get Grid Stats for display
   Dim GetCount As MySqlDataReader
   SQLCmd = "Select Count(lastname) as Counted " +
            "From users " +
            "Where Length(PasswordHash)>31"
   If Trace.IsEnabled Then Trace.Warn("Default", "Get user count SQLCmd: " + SQLCmd.ToString())
   GetCount = MyDB.GetReader("MyData", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("Default", "Get user count DB Error: " + MyDB.ErrMessage())
   If GetCount.HasRows() Then
    GetCount.Read()
    GridTUsers.InnerText = GetCount("Counted").ToString()
   End If
   GetCount.Close()
   ' How many are online
   SQLCmd = "Select Count(UUID) as Total " +
            "From agents " +
            "Where agentOnline>0"
   If Trace.IsEnabled Then Trace.Warn("Default", "Get users online count SQLCmd: " + SQLCmd.ToString())
   GetCount = MyDB.GetReader("MyData", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("Default", "Get agent online count DB Error: " + MyDB.ErrMessage())
   If GetCount.HasRows() Then
    GetCount.Read()
    GridOnline.InnerText = GetCount("Total").ToString()
   End If
   GetCount.Close()
   ' How many Regions
   SQLCmd = "Select Count(UUID) as Total " +
            "From regions "
   If Trace.IsEnabled Then Trace.Warn("Default", "Get total regions SQLCmd: " + SQLCmd.ToString())
   GetCount = MyDB.GetReader("MyData", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("Default", "Get region count DB Error: " + MyDB.ErrMessage())
   If GetCount.HasRows() Then
    GetCount.Read()
    RegCount = GetCount("Total")
    GridRegions.InnerText = RegCount.ToString()
   End If
   GetCount.Close()

   If RegCount > 0 Then
    ' Get Grid Owner Account Name
    Dim Name() As String
    Dim GetAccount As MySqlDataReader
    SQLCmd = "Select Parm2 From control Where Control='GridSysAccounts' and Parm1='GridOwnerAcct'"
    If Trace.IsEnabled Then Trace.Warn("Default", "Get Grid Owner Name SQLCmd: " + SQLCmd.ToString())
    GetAccount = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("Default", "Get Grid Owner DB Error: " + MyDB.ErrMessage())
    If GetAccount.HasRows() Then
     GetAccount.Read()
     Name = GetAccount("Parm2").ToString().Trim().Split(" ")
    End If
    GetAccount.Close()

    ' How many Govenor Owned Regions
    SQLCmd = "Select Count(UUID) as Total " +
             "From regions " +
             "Where owner_uuid=" +
             " (Select UUID From users Where username=" + MyDB.SQLStr(Name(0)) + " and lastname=" + MyDB.SQLStr(Name(1)) + ")"
    If Trace.IsEnabled Then Trace.Warn("Default", "Get total Public regions SQLCmd: " + SQLCmd.ToString())
    GetCount = MyDB.GetReader("MyData", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("Default", "Get Governor region count DB Error: " + MyDB.ErrMessage())
    If GetCount.HasRows() Then
     GetCount.Read()
     PubRegions.InnerText = GetCount("Total").ToString()
    End If
    GetCount.Close()
    ' How many private regions
    SQLCmd = "Select Count(UUID) as Total " +
             "From regions " +
             "Where owner_uuid<>" +
             " (Select UUID From users Where username=" + MyDB.SQLStr(Name(0)) + " and lastname=" + MyDB.SQLStr(Name(1)) + ")"
    If Trace.IsEnabled Then Trace.Warn("Default", "Get total School regions SQLCmd: " + SQLCmd.ToString())
    GetCount = MyDB.GetReader("MyData", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("Default", "Get Private region count DB Error: " + MyDB.ErrMessage())
    If GetCount.HasRows() Then
     GetCount.Read()
     PrivRegions.InnerText = GetCount("Total").ToString()
    End If
    GetCount.Close()

    Dim GetSettings As MySqlDataReader
    SQLCmd = "Select Parm2 " +
             "From control " +
             "Where Control='Grid' and Parm1='Status'"
    If Trace.IsEnabled Then Trace.Warn("Default", "Get Control settings SQLCmd: " + SQLCmd.ToString())
    GetSettings = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("Default", "Get Grid Status DB Error: " + MyDB.ErrMessage())
    If GetSettings.HasRows() Then
     GetSettings.Read()
     StatusURL = GetSettings("Parm2").ToString().Trim()
    End If
    If Trace.IsEnabled Then Trace.Warn("Default", "** Get Grid Status URL: ")
    GetSettings.Close()

    ' Get grid status
    Dim ReturnInfo As String
    Dim response As System.Net.WebResponse = Nothing
    Dim reader As System.IO.StreamReader = Nothing
    Dim requestWeb As System.Net.WebRequest
    Dim responseStream As System.IO.Stream
    Try
     requestWeb = System.Net.WebRequest.Create(StatusURL)
     response = requestWeb.GetResponse()
     responseStream = response.GetResponseStream()
     reader = New System.IO.StreamReader(responseStream)
     ReturnInfo = reader.ReadToEnd()                         ' Get response text
     requestWeb = Nothing
     response.Close()
     reader.Close()
     If Trace.IsEnabled Then Trace.Warn("Default", "Website returned: " + ReturnInfo.ToString())
     If ReturnInfo.ToString() = "OK" Then
      GridStats.InnerHtml = "<span style=""color: #00EE00;"">online</span>"
     End If
    Catch ex As Exception
     ' dont care
    End Try
   End If

   'If exists, load the PageList.txt for the starting website content pages.
   FileName = Server.MapPath("").ToString() + "\PageList.txt"
   If System.IO.File.Exists(FileName) Then              ' load file if it exists
    If Trace.IsEnabled Then Trace.Warn("Default", "Loading Configuration: " + FileName.ToString())
    Dim FileReader As New System.IO.StreamReader(FileName) ' Create file access
    SQLCmd = "Delete From pagemaster " +
             "Where PageID Not in (Select PageID From pagedetail)"
    If Trace.IsEnabled Then Trace.Warn("Default", "Clear unlinked Pages SQLCmd: " + SQLCmd)
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Default", "DB Error: " + MyDB.ErrMessage().ToString())
    SQLFields = "Name,Path"
    Dim ChkPage As MySqlDataReader
    While Not FileReader.EndOfStream()
     tLine = FileReader.ReadLine()
     tFields = tLine.Split(Chr(9))
     SQLValues = MyDB.SQLStr(tFields(0)) + "," + MyDB.SQLStr(tFields(1))
     ' Verify the page does not already exist!
     SQLCmd = "Select name From pagemaster Where Path=" + MyDB.SQLStr(tFields(1))
     If Trace.IsEnabled Then Trace.Warn("Default", "Page Record Check: " + SQLCmd)
     ChkPage = MyDB.GetReader("MySite", SQLCmd)
     If Not ChkPage.HasRows() Then
      SQLCmd = "Insert into pagemaster (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
      If Trace.IsEnabled Then Trace.Warn("Default", "Insert Page record: " + SQLCmd)
      MyDB.DBCmd("MySite", SQLCmd)
     End If
     ChkPage.Close()
    End While
    FileReader.Close()
    System.IO.File.Delete(FileName)                    ' Remove PageList.txt configuration file
   End If

   ' Check if page is registered
   Dim drGetPage As MySqlDataReader
   SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
   If Trace.IsEnabled Then Trace.Warn("Default", "Get Page Record Check: " + SQLCmd)
   drGetPage = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Default", "DB Error: " + MyDB.ErrMessage().ToString())
   If Not drGetPage.HasRows() Then
    drGetPage.Close()
    ' Create page entry
    SQLFields = "Name,Path"
    SQLValues = "'Home'," + MyDB.SQLStr(Request.ServerVariables("URL"))
    SQLCmd = "Insert into pagemaster (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
    If Trace.IsEnabled Then Trace.Warn("Default", "Insert Page record: " + SQLCmd)
    MyDB.DBCmd("MySite", SQLCmd)
    ' Reload PageID
    SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
    drGetPage = MyDB.GetReader("MySite", SQLCmd)
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
    If Trace.IsEnabled Then Trace.Warn("Default", "Update Page AutoStart: " + SQLCmd)
    MyDB.DBCmd("MySite", SQLCmd)

    ' Check for Page display content
    Dim rsPage As MySqlDataReader
    SQLCmd = "Select Title,Content From pagedetail " +
             "Where Active=1 and PageID=" + MyDB.SQLNo(drGetPage("PageID")) + " " +
             "Order by SortOrder"
    If Trace.IsEnabled Then Trace.Warn("Default", "Get Page Content: " + SQLCmd)
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

   Dim SBMenu As New TreeView
   ' Sidebar Options control based on Clearance or Write Access
   ' Set up navigation options
   If Trace.IsEnabled Then SBMenu.SetTrace = True
   'SBMenu.AddItem("M", "3", "Report List")                  ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                   ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Other Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry")        ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")      ' Program URL link entry
   If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then  ' Allow menu access while not logged in
    SBMenu.AddItem("B", "", "Blank Entry")
    SBMenu.AddItem("T", "", "Page Options")
    SBMenu.AddItem("P", "/Register.aspx", "New Account")
    SBMenu.AddItem("P", "/Logon.aspx", "Account Logon")
    SBMenu.AddItem("P", "/ResetPass.aspx", "Reset Password") ' Only needed for accounts that cannot logon
   Else                                                     ' Logged in menu options
    SBMenu.AddItem("P", "Account.aspx", "My Account")
    SBMenu.AddItem("P", "Logout.aspx", "Logout")
    If Session("Access") > 1 Then
     SBMenu.AddItem("B", "", "Blank Entry")
     SBMenu.AddItem("T", "", "Page Options")
     SBMenu.AddItem("P", "/Administration/Admin.aspx", "Website Administration")
    End If
   End If
   If Trace.IsEnabled Then Trace.Warn("Default", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   SBMenu.Close()
  End If

  ' Get Display list Items here

 End Sub

 ' Process Configuration data validation checks
 Private Function ValAddEdit(ByVal tAdd As Boolean) As String
  ' Parameter tAdd allows selections of Add mode only testing
  Dim aMsg, Fields() As String
  Fields = Session("TextLine").ToString().Split(Chr(9))
  aMsg = ""
  ' Process error checking as required, place messages in tMsg.
  If Fields(0).ToString().Trim().Length > 20 Then
   aMsg = aMsg.ToString() + "Control field content is too long! =" + Fields(0).ToString() + "<br />"
  End If
  If Fields(1).ToString().Trim().Length > 30 Then
   aMsg = aMsg.ToString() + "Name field content is too long! =" + Fields(0).ToString() + "<br />"
  End If
  If Fields(2).ToString().Trim().Length > 20 Then
   aMsg = aMsg.ToString() + "Parm1 field content is too long! =" + Fields(0).ToString() + "<br />"
  End If
  If Fields(3).ToString().Trim().Length > 150 Then
   aMsg = aMsg.ToString() + "Parm2 field content is too long! =" + Fields(0).ToString() + "<br />"
  End If
  If Not IsNumeric(Fields(4)) Then
   aMsg = aMsg.ToString() + "Nbr1 field is not numeric! =" + Fields(0).ToString() + "<br />"
  Else
   If Fields(4).ToString().Contains(".") Then
    aMsg = aMsg.ToString() + "Nbr1 may not have decimal values! Integer only =" + Fields(0).ToString() + "<br />"
   End If
  End If
  If Not IsNumeric(Fields(5)) Then
   aMsg = aMsg.ToString() + "Nbr2 field is not numeric! =" + Fields(0).ToString() + "<br />"
  Else
   If Fields(5).ToString().Contains(".") Then
    aMsg = aMsg.ToString() + "Nbr2 may not have decimal values! Integer only =" + Fields(0).ToString() + "<br />"
   End If
  End If
  'If FieldName.Text.ToString().Trim().Length = 0 Then
  ' aMsg = aMsg.ToString() + "Missing Field Name!<br />"
  'End If
  Return aMsg
 End Function

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub
End Class
