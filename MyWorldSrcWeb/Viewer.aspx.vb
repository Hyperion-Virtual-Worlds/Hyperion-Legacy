Partial Class Viewer
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
 '* Viewer splash page. Contents managed in the Administration Website Content Management program.
 '* Page may display pictures taken inworld and uploaded to the website images/site folder.
 '* 

 '* Built from Web Basic Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private SQLCmd, SQLFields, SQLValues As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("Viewer", "Start Page Load")

  Session("SSLStatus") = False
  ' Force SSL active if it is not and is required.
  If Request.ServerVariables("HTTPS") = "off" Then          ' Security is not active and is required
   If Trace.IsEnabled Then Trace.Warn("Viewer", "Https is off")
   Dim drServer As MySqlDataReader
   SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='ServerLocation'"
   If Trace.IsEnabled Then Trace.Warn("Viewer", "Get location SQLCmd: " + SQLCmd)
   drServer = MyDB.GetReader("MySite", SQLCmd)
   If drServer.Read() Then
    If Trace.IsEnabled Then Trace.Warn("Viewer", "ServerLocation = " + drServer("Parm2").ToString().Trim().ToUpper())
    If drServer("Parm2").ToString().Trim().ToUpper() = "LIVE" Then ' Security must be on if not testing
     drServer.Close()
     Session("SSLStatus") = True
     ' Disabled until viewers can support SSL
     'Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + "/Viewer.aspx")
    End If
   Else                                                     ' show error if not located
    If Trace.IsEnabled Then Trace.Warn("Viewer", "Set https error: <br>Control value for 'Server Location' (ServerLocation) was not defined!")
    Session("ErrorMessage") = "Control value for 'Server Location' (ServerLocation) was not defined!"
    Response.Redirect("/Error.aspx")
   End If
   drServer.Close()
  End If

  ' Viewer Page Override URL
  Dim RedirectURL As MySqlDataReader
  SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='ViewerURL'"
  If Trace.IsEnabled Then Trace.Warn("Viewer", "Get Control ViewerURL SQLCmd: " + SQLCmd)
  RedirectURL = MyDB.GetReader("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Viewer", "DB Error: " + MyDB.ErrMessage().ToString())
  If RedirectURL.HasRows() Then
   RedirectURL.Read()
   If Not RedirectURL("Parm2").ToString().Trim().Length = 0 Then
    Response.Redirect(RedirectURL("Parm2").ToString().Trim())
   End If
  Else                                                     ' Add Control record
   SQLCmd = "Insert into control (Control,Name,Parm1,Parm2,Nbr1,Nbr2) " +
            "Values ('ADMINSYSTEM','Viewer Redirect URL','ViewerURL','',0,0)"
   If Trace.IsEnabled Then Trace.Warn("Viewer", "Insert Control ViewerURL SQLCmd: " + SQLCmd)
   MyDB.DBCmd("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Viewer", "DB Error: " + MyDB.ErrMessage().ToString())
  End If
  RedirectURL.Close()

  If Not IsPostBack Then                                    ' First time page is called setup

   ' define local objects here
   Dim tHtmlOut As String
   tHtmlOut = ""

   ' Setup general page controls
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
   If Trace.IsEnabled Then Trace.Warn("Viewer", "Get user count SQLCmd: " + SQLCmd.ToString())
   GetCount = MyDB.GetReader("MyData", SQLCmd)
   If GetCount.HasRows() Then
    GetCount.Read()
    GridTUsers.InnerText = GetCount("Counted").ToString()
   End If
   GetCount.Close()
   ' How many are online
   SQLCmd = "Select Count(UUID) as Total " +
            "From agents " +
            "Where agentOnline>0"
   If Trace.IsEnabled Then Trace.Warn("Viewer", "Get users online count SQLCmd: " + SQLCmd.ToString())
   GetCount = MyDB.GetReader("MyData", SQLCmd)
   If GetCount.HasRows() Then
    GetCount.Read()
    GridOnline.InnerText = GetCount("Total").ToString()
   End If
   GetCount.Close()
   ' How many Regions
   SQLCmd = "Select Count(UUID) as Total " +
            "From regions "
   If Trace.IsEnabled Then Trace.Warn("Viewer", "Get total regions SQLCmd: " + SQLCmd.ToString())
   GetCount = MyDB.GetReader("MyData", SQLCmd)
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
    If Trace.IsEnabled Then Trace.Warn("Viewer", "Get Grid Owner Name SQLCmd: " + SQLCmd.ToString())
    GetAccount = MyDB.GetReader("MySite", SQLCmd)
    GetAccount.Read()
    Name = GetAccount("Parm2").ToString().Trim().Split(" ")
    GetAccount.Close()

    ' How many Govenor Owned Regions
    SQLCmd = "Select Count(UUID) as Total " +
             "From regions " +
             "Where owner_uuid=" +
             " (Select UUID From users Where username=" + MyDB.SQLStr(Name(0)) + " and lastname=" + MyDB.SQLStr(Name(1)) + ")"
    If Trace.IsEnabled Then Trace.Warn("Viewer", "Get total Public regions SQLCmd: " + SQLCmd.ToString())
    GetCount = MyDB.GetReader("MyData", SQLCmd)
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
    If Trace.IsEnabled Then Trace.Warn("Viewer", "Get total School regions SQLCmd: " + SQLCmd.ToString())
    GetCount = MyDB.GetReader("MyData", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("Viewer", "MyDB Error: " + MyDB.ErrMessage().ToString())
    If GetCount.HasRows() Then
     GetCount.Read()
     PrivRegions.InnerText = GetCount("Total").ToString()
    End If
    GetCount.Close()

    Dim GetSettings As MySqlDataReader
    SQLCmd = "Select Parm2 " +
            "From control " +
            "Where Control='Grid' and Parm1='Status'"
    If Trace.IsEnabled Then Trace.Warn("Viewer", "Get Control settings SQLCmd: " + SQLCmd.ToString())
    GetSettings = MyDB.GetReader("MySite", SQLCmd)
    If GetSettings.HasRows() Then
     GetSettings.Read()
     StatusURL = GetSettings("Parm2").ToString().Trim()
    End If
    If Trace.IsEnabled Then Trace.Warn("Viewer", "** Get Grid Status URL: ")
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
     If Trace.IsEnabled Then Trace.Warn("Viewer", "Website returned: " + ReturnInfo.ToString())
     If ReturnInfo.ToString() = "OK" Then
      GridStats.InnerHtml = "<span style=""color: #00EE00;"">online</span>"
     End If
    Catch ex As Exception
     ' dont care
    End Try
   End If

   ' Get background picture list: *.jpg,*.png
   Dim tURLPath, tPath, tFiles(), FileList() As String
   Dim I, Index As Integer
   tURLPath = "/Images/Site/Backgrounds/"
   tPath = Server.MapPath(tURLPath.ToString().Replace("/", "\")).ToString()
   I = 0
   Index = 0
   FileList = System.IO.Directory.GetFiles(tPath, "*.jpg")
   ReDim tFiles(FileList.Length - 1)
   For I = 0 To FileList.Length - 1
    tFiles(Index) = FileList(I)
    Index = Index + 1
   Next
   FileList = System.IO.Directory.GetFiles(tPath, "*.png")
   ReDim Preserve tFiles(tFiles.Length + FileList.Length - 1)
   For I = 0 To FileList.Length - 1
    tFiles(Index) = FileList(I)
    Index = Index + 1
   Next
   Randomize()
   If Trace.IsEnabled Then Trace.Warn("Viewer", "tFiles.Length: " + tFiles.Length.ToString())
   If tFiles.Length > 0 Then
    Index = CInt(Int(tFiles.Length * Rnd()))               ' Get a number between 0 and tFiles.Length
    Background.Style.Item("background-image") = "'" + tURLPath + System.IO.Path.GetFileName(tFiles(Index)).ToString() + "'"
   End If

   ' Check if page is registered
   Dim drGetPage As MySqlDataReader
   SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
   If Trace.IsEnabled Then Trace.Warn("Viewer", "Get Page Record Check: " + SQLCmd)
   drGetPage = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Viewer", "DB Error: " + MyDB.ErrMessage().ToString())
   If Not drGetPage.HasRows() Then
    drGetPage.Close()
    ' Create page entry
    SQLFields = "Name,Path"
    SQLValues = "'Viewer'," + MyDB.SQLStr(Request.ServerVariables("URL"))
    SQLCmd = "Insert into pagemaster (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
    If Trace.IsEnabled Then Trace.Warn("Viewer", "Insert Page record: " + SQLCmd)
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Viewer", "DB Error: " + MyDB.ErrMessage().ToString())
    ' Reload PageID
    SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
    If Trace.IsEnabled Then Trace.Warn("Viewer", "Reload Page RecordID: " + SQLCmd)
    drGetPage = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Viewer", "DB Error: " + MyDB.ErrMessage().ToString())
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
    If Trace.IsEnabled Then Trace.Warn("Viewer", "Update Page AutoStart: " + SQLCmd)
    MyDB.DBCmd("MySite", SQLCmd)

    ' Check for Page display content
    Dim rsPage As MySqlDataReader
    SQLCmd = "Select Title,Content From pagedetail " +
             "Where Active=1 and PageID=" + MyDB.SQLNo(drGetPage("PageID")) + " " +
             "Order by SortOrder"
    If Trace.IsEnabled Then Trace.Warn("Viewer", "Get Page Content: " + SQLCmd)
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
  End If

  ' Get Display list Items here

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
