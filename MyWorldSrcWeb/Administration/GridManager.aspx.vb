
Partial Class Administration_GridManager
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
 '* This is the template for content selection display and access to the Form template page to add,
 '* edit and remove records in a table. These pages may be used in hierarchial structure with use of
 '* the Session() values to retain state for each level.

 '* Built from MyWorld Select template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private SQLCmd, SQLFields, SQLValues As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Validate logon and session existance.
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Response.Redirect("/Default.aspx")                      ' Return to logon page
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("/Default.aspx")
  End If
  If Session("Access") < 6 Then                            ' Grid Administrator or SysAdmin access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("GridManager", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page is called setup
   ' Define process unique objects here

   ' Define local objects here
   If Session("FirstTime") <> "GridManager" Then
    Session("FirstTime") = "GridManager"                   ' Allows session data to be maintained with lower level pages.
    Session("SelField") = " regionName"
   End If
   Session("FirstTimeB") = ""
   Session("SelPageB") = ""

   ' Setup general page controls

   ' Set up navigation options
   ' Sidebar Options control based on Clearance or Write Access
   Dim SBMenu As New TreeView
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   SBMenu.AddItem("B", "", "Blank Entry")
   SBMenu.AddItem("T", "", "Page Options")
   If Session("Access") = 9 Then                            ' SysAdmin Only
    SBMenu.AddItem("P", "ServerSelect.aspx", "Server Management")
   End If
   SBMenu.AddItem("B", "", "Blank Entry")
   SBMenu.AddItem("L", "OpenMsg(0,'Enter message to send to all regions');", "Grid Message")       ' send message to all region users
   SBMenu.AddItem("P", "RegionRpt.aspx", "Region Report")
   If Trace.IsEnabled Then Trace.Warn("GridManager", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   SBMenu.Close()

   UpdateData()
   Display()
  End If

 End Sub

 Private Sub Display()
  If Trace.IsEnabled Then Trace.Warn("GridManager", "Display() Called")
  ' Get Display list Items here
  Dim DBName As String
  DBName = "MyData"
  Dim Control As MySqlDataReader
  SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='MyDataDBName'"
  If Trace.IsEnabled Then Trace.Warn("RegionConfig", "Get Control DB Name SQLCmd: " + SQLCmd.ToString())
  Control = MyDB.GetReader("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridManager", "DB Error: " + MyDB.ErrMessage().ToString())
  If Control.HasRows() Then
   Control.Read()
   DBName = Control("Parm2").Trim().ToString()
  End If

  Dim Display As MySqlDataReader
  SQLCmd = "Select regionName,UUID,status,Concat(Cast(locationX as char),',',Cast(locationY as char)) as Map,externalIP,port,ownerUUID," +
           " (Select Count(UUID) As Total From " + DBName.ToString() + ".agents " +
           "  Where agentOnline>0 and currentRegion=regionxml.UUID) as UserCnt," +
           " (Select Concat(username,' ',lastname) as Name From " + DBName.ToString() + ".users Where UUID=regionxml.OwnerUUID) as OwnerName,ProcessHandle " +
           "From regionxml " +
           "Order by " + Session("SelField").ToString()
  If Trace.IsEnabled Then Trace.Warn("GridManager", "Get SQLCmd: " + SQLCmd.ToString())
  ' gvDisplay is a gridview data object placed on the page.
  Display = MyDB.GetReader("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridManager", "Get GridList DB error: " + MyDB.ErrMessage())
  gvDisplay.DataSource = Display
  gvDisplay.DataBind()

 End Sub

 ' Validate regions status
 Protected Sub UpdateData()
  If Trace.IsEnabled Then Trace.Warn("GridManager", "** UpdateData() Called **")
  Dim tUUIDList As String
  tUUIDList = ""
  Dim ReturnInfo, IP As String
  Dim GetEstate As MySqlDataReader
  Dim GetRegion As MySqlDataReader
  Dim GridList As MySqlDataReader
  SQLCmd = "Select regionName,UUID,status,locationX,locationY,externalIP,internalIP,port,ownerUUID,ProcessHandle " +
           "From regionxml " +
           "Order by regionName"
  If Trace.IsEnabled Then Trace.Warn("GridManager", "Get Regions list SQLCmd: " + SQLCmd.ToString())
  GridList = MyDB.GetReader("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridManager", "Get GridList DB error: " + MyDB.ErrMessage())
  If GridList.HasRows() Then
   While GridList.Read()
    If Trace.IsEnabled Then Trace.Warn("GridManager", "** Processing region: " + GridList("UUID").ToString().Trim())
    ' Check estate_map for UUID: Establishes the region as an existing region, not a new one.
    SQLCmd = "Select RegionID " +
             "From estate_map " +
             "Where RegionID=" + MyDB.SQLStr(GridList("UUID"))
    If Trace.IsEnabled Then Trace.Warn("GridManager", "Get Region SQLCmd: " + SQLCmd.ToString())
    GetEstate = MyDB.GetReader("MyData", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridMonkey", "Get GridList DB error: " + MyDB.ErrMessage())
    If GetEstate.HasRows() Then                            ' Process region
     If tUUIDList.ToString().Trim().Length > 0 Then
      tUUIDList = tUUIDList.ToString() + ","
     End If
     tUUIDList = tUUIDList.ToString() + "'" + GridList("UUID").ToString() + "'"
     SQLCmd = "Select regionName,serverIP,serverPort,locX,locY " +
              "From regions " +
              "Where UUID=" + MyDB.SQLStr(GridList("UUID"))
     If Trace.IsEnabled Then Trace.Warn("GridManager", "Get Region SQLCmd: " + SQLCmd.ToString())
     GetRegion = MyDB.GetReader("MyData", SQLCmd)
     If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridMonkey", "Get GridList DB error: " + MyDB.ErrMessage())
     ' NOTE: Just because the DB says it is online, does not mean it really is! 
     SQLCmd = ""
     If GetRegion.HasRows() Then                           ' Region is supposed to be online
      GetRegion.Read()
      ReturnInfo = ""
      If GridList("locationX").ToString() <> GetRegion("locX").ToString() Then ' Update Location X
       SQLCmd = SQLCmd.ToString() +
                 "locationX=" + MyDB.SQLNo(GetRegion("locX"))
      End If
      If GridList("locationY").ToString() <> GetRegion("locY").ToString() Then ' Update Location X
       If SQLCmd.ToString().Trim().Length <> 0 Then       ' Prior set data to write exists, extend SQL
        SQLCmd = SQLCmd.ToString() + ","
       End If
       SQLCmd = SQLCmd.ToString() +
                 "locationY=" + MyDB.SQLNo(GetRegion("locY"))
      End If
      If GridList("externalIP").ToString() <> GetRegion("serverIP").ToString() Then ' Update externalIP
       If SQLCmd.ToString().Trim().Length <> 0 Then       ' Prior set data to write exists, extend SQL
        SQLCmd = SQLCmd.ToString() + ","
       End If
       SQLCmd = SQLCmd.ToString() +
                 "externalIP=" + MyDB.SQLStr(GetRegion("serverIP"))
      End If
      If GridList("port").ToString() <> GetRegion("serverPort").ToString() Then ' Update port
       If SQLCmd.ToString().Trim().Length <> 0 Then       ' Prior set data to write exists, extend SQL
        SQLCmd = SQLCmd.ToString() + ","
       End If
       SQLCmd = SQLCmd.ToString() +
                 "port=" + MyDB.SQLNo(GetRegion("serverPort"))
      End If
      If GridList("regionName").ToString() <> GetRegion("regionName").ToString() Then ' Region name changed?
       If SQLCmd.ToString().Trim().Length <> 0 Then       ' Prior set data to write exists, extend SQL
        SQLCmd = SQLCmd.ToString() + ","
       End If
       SQLCmd = SQLCmd.ToString() +
                "regionName=" + MyDB.SQLStr(GetRegion("regionName"))
      End If
      ' Ping server region for status
      ' Can use internal network address: http://10.0.0.30:9518/simstatus/ for communication with regions.
      If GridList("internalIP").ToString().Trim().Length > 0 Then ' Use the internalIP if it has one
       IP = GridList("internalIP").ToString()
      Else
       IP = GetRegion("serverIP").ToString()
      End If
      Dim response As System.Net.WebResponse = Nothing
      Dim reader As System.IO.StreamReader = Nothing
      Dim requestWeb As System.Net.WebRequest
      Dim responseStream As System.IO.Stream
      If Trace.IsEnabled Then Trace.Warn("GridManager", "Ask Message: " + "http://" + IP.ToString() + ":" + GetRegion("serverPort").ToString() + "/simstatus/")
      requestWeb = System.Net.WebRequest.Create("http://" + IP.ToString() + ":" + GetRegion("serverPort").ToString() + "/simstatus/")
      requestWeb.Timeout = 2000                           ' Two second response timeout
      Try
       response = requestWeb.GetResponse()
       responseStream = response.GetResponseStream()
       reader = New System.IO.StreamReader(responseStream)
       ReturnInfo = reader.ReadToEnd()                    ' Get response text
       response.Close()
       reader.Close()
      Catch ex As Exception
       ReturnInfo = ""
      End Try
      requestWeb = Nothing
      If Trace.IsEnabled Then Trace.Warn("GridManager", "-- Region Response: " + ReturnInfo.ToString())
      If SQLCmd.ToString().Trim().Length <> 0 Then        ' Prior set data to write exists, extend SQL
       SQLCmd = SQLCmd.ToString() + ","
      End If
      If ReturnInfo.ToString() = "OK" Then                ' Region responded correctly, its online
       If Trace.IsEnabled Then Trace.Warn("GridManager", "** Region Status: " + GridList("Status").ToString())
       If GridList("Status") <> 2 Then

        If Not IsDBNull(Session("Counter" + GridList("UUID").ToString())) Or CInt(Session("Counter" + GridList("UUID").ToString())) = 0 Then
         If Trace.IsEnabled Then Trace.Warn("GridManager", "** Process Counter: " + Session("Counter" + GridList("UUID").ToString()).ToString())
         If CInt(Session("Counter" + GridList("UUID").ToString())) > 0 Then ' Delay status change until region has restarted
          SQLCmd = SQLCmd.ToString() +
                    "Status=" + MyDB.SQLStr(GridList("Status"))
          Session("Counter" + GridList("UUID").ToString()) = Session("Counter" + GridList("UUID").ToString()) - 1
         Else
          SQLCmd = SQLCmd.ToString() + "Status=2"
         End If
        Else                                              ' Not in command processing
         SQLCmd = SQLCmd.ToString() + "Status=2"
        End If
       End If
      Else
       If GridList("Status") = 2 Then                     ' It is offline.
        SQLCmd = SQLCmd.ToString() + "Status=0"
       Else                                               ' Check Status
        If GridList("Status") <> 0 Then                   ' Retain prior Status set
         SQLCmd = SQLCmd.ToString() + "Status=" + MyDB.SQLStr(GridList("Status"))
        End If
       End If
      End If
     Else                                                  ' Region is offline, not in DB
      ' Status Values: 0=Offline, 1=Starting, 2=Online, 3=Closing
      If Not IsDBNull(Session("Counter" + GridList("UUID").ToString())) Or CInt(Session("Counter" + GridList("UUID").ToString())) = 0 Then
       Session("Counter" + GridList("UUID").ToString()) = 0 ' Counter may be cleared
      End If
      ' If starting up,
      If GridList("Status") = 1 Then                        ' Retain prior status it is in change
       SQLCmd = SQLCmd.ToString() + "Status=" + MyDB.SQLStr(GridList("Status"))
      Else                                                 ' Its offline
       If GridList("Status") <> 0 Then                     ' Clear the status
        SQLCmd = SQLCmd.ToString() + "Status=0"
       End If
      End If
     End If
     GetRegion.Close()
     If SQLCmd.ToString().Trim().Length <> 0 Then          ' Write data only if there is any set
      SQLCmd = "Update regionxml Set " + SQLCmd.ToString() + " Where UUID=" + MyDB.SQLStr(GridList("UUID"))
      If Trace.IsEnabled Then Trace.Warn("GridManager", "Update Region SQLCmd: " + SQLCmd.ToString())
      MyDB.DBCmd("MySite", SQLCmd)                         ' Update records
      If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridManager", "Update DB error: " + MyDB.ErrMessage())
     End If
    Else                                                   ' Remove from regionxml, It has no estate entry, user removed.
     SQLCmd = "Delete From regionxml Where UUID=" + MyDB.SQLStr(GridList("UUID"))
     If Trace.IsEnabled Then Trace.Warn("GridManager", "Remove Region SQLCmd: " + SQLCmd.ToString())
     MyDB.DBCmd("MySite", SQLCmd)
     If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridManager", "Insert DB error: " + MyDB.ErrMessage())
    End If
    GetEstate.Close()
   End While
  End If
  GridList.Close()
  ' Check for any orphan regions not in the GridList
  SQLCmd = "Select UUID " +
           "From regions "
  If tUUIDList.ToString().Trim().Length > 0 Then           ' Do not include already listed regions
   SQLCmd = SQLCmd.ToString() +
            "Where UUID Not in (" + tUUIDList.ToString() + ") "
  End If
  SQLCmd = SQLCmd.ToString() +
           "Order by regionName"
  If Trace.IsEnabled Then Trace.Warn("GridManager", "Get New Regions SQLCmd: " + SQLCmd.ToString())
  GetRegion = MyDB.GetReader("MyData", SQLCmd)
  If GetRegion.HasRows() Then                              ' Got Regions!
   If Trace.IsEnabled Then Trace.Warn("GridManager", "** Add Regions **")
   While GetRegion.Read()
    SQLCmd = "Delete from regions Where UUID=" + MyDB.SQLStr(GetRegion("UUID"))
    If Trace.IsEnabled Then Trace.Warn("GridManager", "Delete Region SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MyData", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridManager", "Insert DB error: " + MyDB.ErrMessage())
   End While
   GetRegion.Close()
  End If
  Display()
 End Sub

 ' Send message to regions
 Private Sub Send_CheckedChanged(sender As Object, e As EventArgs) Handles Send.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("GridManager", "** Send Message to Region Process **")
  Dim IP, SessionID As String
  SessionID = ""

  Dim GridList As MySqlDataReader
  If RegID.Value.ToString().Length = 1 Then               ' Process message for all running regions in all servers
   SQLCmd = "Select externalIP,internalIP,port, " +
            " (Select AccountName From serverhosts Where externalIP=regionxml.externalIP) as AccountName," +
            " (Select AcctPassword From serverhosts Where externalIP=regionxml.externalIP) as AcctPassword " +
            "From regionxml " +
            "Where Status=2 " +
            "Order by port"
   If Trace.IsEnabled Then Trace.Warn("GridManager", "All Regions SQLCmd: " + SQLCmd.ToString())
   GridList = MyDB.GetReader("MySite", SQLCmd)
   While GridList.Read()
    ' Can use internal network address: http://10.0.0.30:9518/simstatus/ for communication with regions.
    If GridList("internalIP").ToString().Trim().Length > 0 Then ' Use the internalIP if it has one
     IP = GridList("internalIP").ToString().Trim()
    Else                                                  ' Else use external IP
     IP = GridList("externalIP").ToString().Trim()
    End If
    ProcessMsg(IP, GridList("port"), GridList("AccountName").ToString(), GridList("AcctPassword").ToString())

   End While

  Else                                                    ' Presume a region UUID was sent
   SQLCmd = "Select externalIP,internalIP,port, " +
            " (Select AccountName From serverhosts Where externalIP=regionxml.externalIP) as AccountName," +
            " (Select AcctPassword From serverhosts Where externalIP=regionxml.externalIP) as AcctPassword " +
            "From regionxml " +
            "Where UUID=" + MyDB.SQLStr(RegID.Value)
   If Trace.IsEnabled Then Trace.Warn("GridManager", "Get Region SQLCmd: " + SQLCmd.ToString())
   GridList = MyDB.GetReader("MySite", SQLCmd)
   GridList.Read()
   ' Can use internal network address: http://10.0.0.30:9518/simstatus/ for communication with regions.
   If GridList("internalIP").ToString().Trim().Length > 0 Then ' Use the internalIP if it has one
    IP = GridList("internalIP").ToString().Trim()
   Else                                                   ' Else use external IP
    IP = GridList("externalIP").ToString().Trim()
   End If
   ProcessMsg(IP, GridList("port"), GridList("AccountName").ToString(), GridList("AcctPassword").ToString())
  End If

  GridList.Close()
  GridList = Nothing
  System.Threading.Thread.Sleep(5000)                     ' Cause 5 second delay before proceeding, give region time to close

  Send.Checked = False                                    ' Allow another command to process
  Display()

 End Sub

 Protected Sub ProcessMsg(tIP As String, tPort As Integer, tAcct As String, tPass As String)
  If Trace.IsEnabled Then Trace.Warn("GridManager", "** ProcessMsg Called **")
  If Trace.IsEnabled Then Trace.Warn("GridManager", "Parms- tIP: " + tIP.ToString() + ", tPort: " +
                                     tPort.ToString() + ", tAcct: " + tAcct.ToString())
  Dim ReturnInfo, OutParms, SessionID As String

  If tIP.ToString().Trim().Length > 0 Then
   ReturnInfo = ""
   SessionID = ""
   Dim doc As New System.Xml.XmlDocument()
   Dim root, params As System.Xml.XmlNode
   Dim requestStream As System.IO.Stream
   Dim responseStream As System.IO.Stream
   Dim response As System.Net.WebResponse = Nothing
   Dim reader As System.IO.StreamReader = Nothing
   Dim requestWeb As System.Net.WebRequest
   Dim OutBytes() As Byte
   OutParms = "<?xml version=""1.0""?>" +
              " <methodCall>" +
              "  <methodName>session.login_with_password</methodName>" +
              "  <params>" +
              "   <param>" +
              "   <value><string>" + tAcct.ToString() + "</string></value>" +
              "   </param>" +
              "   <param>" +
              "   <value><string>" + tPass.ToString() + "</string></value>" +
              "   </param>" +
              "  </params>" +
              " </methodCall>"
   OutBytes = System.Text.Encoding.UTF8.GetBytes(OutParms.ToString())
   If Trace.IsEnabled Then Trace.Warn("GridManager", "StartSession Message: " + "http://" + tIP.ToString() + ":" + tPort.ToString() + "/xmlrpc/RemoteAdmin")
   If Trace.IsEnabled Then Trace.Warn("GridManager", "Sending: " + OutParms.ToString())
   requestWeb = System.Net.WebRequest.Create("http://" + tIP.ToString() + ":" + tPort.ToString() + "/xmlrpc/RemoteAdmin")
   requestWeb.Method = "post"
   requestWeb.ContentType = "text/xml"
   requestWeb.ContentLength = OutBytes.Length
   requestStream = requestWeb.GetRequestStream()
   requestStream.Write(OutBytes, 0, OutBytes.Length)
   requestStream.Flush()                                   ' Send outgoing request, clear buffers
   requestStream.Close()
   requestWeb.Timeout = 2000                               ' Two second response timeout
   Try
    response = requestWeb.GetResponse()
    responseStream = response.GetResponseStream()
    reader = New System.IO.StreamReader(responseStream)
    ReturnInfo = reader.ReadToEnd()                        ' Get response text
    response.Close()
    reader.Close()
    If Trace.IsEnabled Then Trace.Warn("GridManager", "Response returned: " + ReturnInfo.ToString())
   Catch ex As Exception
    ReturnInfo = ""
    If Trace.IsEnabled Then Trace.Warn("GridManager", "Message Failed: " + ex.Message().ToString())
   End Try
   requestWeb = Nothing
   If ReturnInfo.ToString().Trim().Length > 0 Then
    doc.LoadXml(ReturnInfo.ToString())
    ' Extract the sessionID from the response xml
    If doc.HasChildNodes Then                              ' For Debug processing to send in email
     root = doc.Item("methodResponse")
     params = root("params")
     SessionID = params.SelectNodes("./param/value/struct/member[name='Value']/value/string")(0).InnerText()
    End If
   End If

   If SessionID.ToString().Trim().Length > 0 Then
    OutParms = "<?xml version=""1.0""?>" +
               "<methodCall>" +
               " <methodName>Console.Command</methodName>" +
               " <params>" +
               "  <param>" +
               "   <value><string>" + SessionID.ToString() + "</string></value>" +
               "  </param>" +
               "  <param>" +
               "   <value><string>alert general " + MsgOut.Value.ToString() + "</string></value>" +
               "  </param>" +
               " </params>" +
               "</methodCall>"
    OutBytes = System.Text.Encoding.UTF8.GetBytes(OutParms.ToString())
    If Trace.IsEnabled Then Trace.Warn("GridManager", "Send a Message: " + "http://" + tIP.ToString() + ":" + tPort.ToString() + "/xmlrpc/RemoteAdmin")
    If Trace.IsEnabled Then Trace.Warn("GridManager", "Sending: " + OutParms.ToString())
    requestWeb = System.Net.WebRequest.Create("http://" + tIP.ToString() + ":" + tPort.ToString() + "/xmlrpc/RemoteAdmin")
    requestWeb.Method = "post"
    requestWeb.ContentType = "text/xml"
    requestWeb.ContentLength = OutBytes.Length
    requestStream = requestWeb.GetRequestStream()
    requestStream.Write(OutBytes, 0, OutBytes.Length)
    requestStream.Flush()                                  ' Send outgoing request
    requestStream.Close()
    requestWeb.Timeout = 2000                              ' Two second response timeout
    Try
     response = requestWeb.GetResponse()
     responseStream = response.GetResponseStream()
     reader = New System.IO.StreamReader(responseStream)
     ReturnInfo = reader.ReadToEnd()                       ' Get response text
     response.Close()
     reader.Close()
     If Trace.IsEnabled Then Trace.Warn("GridManager", "Response returned: " + ReturnInfo.ToString())
    Catch ex As Exception
     ReturnInfo = ""
     If Trace.IsEnabled Then Trace.Warn("GridManager", "Message Failed: " + ex.Message().ToString())
    End Try
   End If
   requestWeb = Nothing
   requestStream = Nothing
   responseStream = Nothing
   reader = Nothing
   doc = Nothing
  End If

 End Sub

 ' Process Region Restart
 Protected Sub ReStart_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ReStart.TextChanged
  If Trace.IsEnabled Then Trace.Warn("GridManager", "** ReStart Region Process **")
  Dim IP, ReturnInfo, OutParms, SessionID As String
  Dim GridList As MySqlDataReader
  SQLCmd = "Select externalIP,internalIP,port, " +
           " (Select AccountName From serverhosts Where externalIP=regionxml.externalIP) as AccountName," +
           " (Select AcctPassword From serverhosts Where externalIP=regionxml.externalIP) as AcctPassword " +
           "From regionxml " +
           "Where UUID=" + MyDB.SQLStr(ReStart.Text)
  If Trace.IsEnabled Then Trace.Warn("GridManager", "Get Region SQLCmd: " + SQLCmd.ToString())
  GridList = MyDB.GetReader("MySite", SQLCmd)
  GridList.Read()
  ' Can use internal network address: http://10.0.0.30:9518/simstatus/ for communication with regions.
  If GridList("internalIP").ToString().Trim().Length > 0 Then ' Use the internalIP if it has one
   IP = GridList("internalIP").ToString().Trim()
  Else                                                     ' Else use external IP
   IP = GridList("externalIP").ToString().Trim()
  End If
  If IP.ToString().Trim().Length > 0 Then
   ReturnInfo = ""
   SessionID = ""
   Dim doc As New System.Xml.XmlDocument()
   Dim root, params As System.Xml.XmlNode
   Dim requestStream As System.IO.Stream
   Dim responseStream As System.IO.Stream
   Dim response As System.Net.WebResponse = Nothing
   Dim reader As System.IO.StreamReader = Nothing
   Dim requestWeb As System.Net.WebRequest
   Dim OutBytes() As Byte
   OutParms = "<?xml version=""1.0""?>" +
              " <methodCall>" +
              "  <methodName>session.login_with_password</methodName>" +
              "  <params>" +
              "   <param>" +
              "   <value><string>" + GridList("AccountName").ToString() + "</string></value>" +
              "   </param>" +
              "   <param>" +
              "   <value><string>" + GridList("AcctPassword").ToString() + "</string></value>" +
              "   </param>" +
              "  </params>" +
              " </methodCall>"
   OutBytes = System.Text.Encoding.UTF8.GetBytes(OutParms.ToString())
   If Trace.IsEnabled Then Trace.Warn("GridManager", "StartSession Message: " + "http://" + IP.ToString() + ":" + GridList("Port").ToString() + "/xmlrpc/RemoteAdmin")
   requestWeb = System.Net.WebRequest.Create("http://" + IP.ToString() + ":" + GridList("Port").ToString() + "/xmlrpc/RemoteAdmin")
   requestWeb.Method = "post"
   requestWeb.ContentType = "text/xml"
   requestWeb.ContentLength = OutBytes.Length
   requestStream = requestWeb.GetRequestStream()
   requestStream.Write(OutBytes, 0, OutBytes.Length)
   requestStream.Flush()                                   ' Send outgoing request, clear buffers
   requestStream.Close()
   requestWeb.Timeout = 2000                               ' Two second response timeout
   Try
    response = requestWeb.GetResponse()
    responseStream = response.GetResponseStream()
    reader = New System.IO.StreamReader(responseStream)
    ReturnInfo = reader.ReadToEnd()                        ' Get response text
    response.Close()
    reader.Close()
    If Trace.IsEnabled Then Trace.Warn("GridManager", "Response returned: " + ReturnInfo.ToString())
   Catch ex As Exception
    ReturnInfo = ""
    If Trace.IsEnabled Then Trace.Warn("GridManager", "Message Failed: " + ex.Message().ToString())
   End Try
   requestWeb = Nothing
   If ReturnInfo.ToString().Trim().Length > 0 Then
    doc.LoadXml(ReturnInfo.ToString())
    ' Extract the sessionID from the response xml
    If doc.HasChildNodes Then                              ' For Debug processing to send in email
     root = doc.Item("methodResponse")
     params = root("params")
     SessionID = params.SelectNodes("./param/value/struct/member[name='Value']/value/string")(0).InnerText()
    End If
   End If

   If SessionID.ToString().Trim().Length > 0 Then
    If Trace.IsEnabled Then Trace.Warn("GridManager", "Restart Message: " + "http://" + IP.ToString() + ":" + GridList("Port").ToString() + "/xmlrpc/RemoteAdmin")
    requestWeb = System.Net.WebRequest.Create("http://" + IP.ToString() + ":" + GridList("Port").ToString() + "/xmlrpc/RemoteAdmin")
    requestWeb.Method = "post"
    requestWeb.ContentType = "text/xml"
    OutParms = "<?xml version=""1.0""?>" +
               "<methodCall>" +
               " <methodName>Console.Command</methodName>" +
               " <params>" +
               "  <param>" +
               "   <value><string>" + SessionID.ToString() + "</string></value>" +
               "  </param>" +
               "  <param>" +
               "   <value><string>restart</string></value>" +
               "  </param>" +
               " </params>" +
               "</methodCall>"
    OutBytes = System.Text.Encoding.UTF8.GetBytes(OutParms.ToString())
    requestWeb.ContentLength = OutBytes.Length
    requestStream = requestWeb.GetRequestStream()
    requestStream.Write(OutBytes, 0, OutBytes.Length)
    requestStream.Flush()                                  ' Send outgoing request
    requestStream.Close()
    requestWeb.Timeout = 2000                              ' Two second response timeout
    Try
     response = requestWeb.GetResponse()
     responseStream = response.GetResponseStream()
     reader = New System.IO.StreamReader(responseStream)
     ReturnInfo = reader.ReadToEnd()                       ' Get response text
     response.Close()
     reader.Close()
     If Trace.IsEnabled Then Trace.Warn("GridManager", "Response returned: " + ReturnInfo.ToString())
    Catch ex As Exception
     ReturnInfo = ""
     If Trace.IsEnabled Then Trace.Warn("GridManager", "Message Failed: " + ex.Message().ToString())
    End Try
    ' NOTE: No response is actually given. The region simply shuts down and restart processed.
    If Trace.IsEnabled Then Trace.Warn("GridManager", "-- Region Response: " + ReturnInfo.ToString())
    SQLCmd = "Update regionxml Set " +
             "status=3 " +
             "Where UUID=" + MyDB.SQLStr(ReStart.Text)
    If Trace.IsEnabled Then Trace.Warn("GridManager", "Set Region Status: shutting down SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)                           ' Update record
    Session("Counter" + ReStart.Text.ToString()) = 1       ' Set processing counter
   End If
   requestWeb = Nothing
   requestStream = Nothing
   responseStream = Nothing
   reader = Nothing
   doc = Nothing
  End If
  GridList.Close()
  GridList = Nothing
  System.Threading.Thread.Sleep(5000)                      ' Cause 5 second delay before proceeding, give region time to close

  ReStart.Text = ""                                        ' Allow another command to process
  Display()
 End Sub

 ' Process Region Shutdown
 Protected Sub Quit_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Quit.TextChanged
  If Trace.IsEnabled Then Trace.Warn("GridManager", "** Shutdown Region Process **")
  Dim IP, ReturnInfo, OutParms, SessionID As String
  Dim GridList As MySqlDataReader   ' Used this way to not have the DB in Lock mode while using a datareader
  SQLCmd = "Select externalIP,internalIP,port, " +
           " (Select AccountName From serverhosts Where externalIP=regionxml.externalIP) as AccountName," +
           " (Select AcctPassword From serverhosts Where externalIP=regionxml.externalIP) as AcctPassword " +
           "From regionxml " +
           "Where UUID=" + MyDB.SQLStr(Quit.Text)
  If Trace.IsEnabled Then Trace.Warn("GridManager", "Get Region SQLCmd: " + SQLCmd.ToString())
  GridList = MyDB.GetReader("MySite", SQLCmd)
  GridList.Read()
  ' Can use internal network address: http://10.0.0.30:9518/simstatus/ for communication with regions.
  If GridList("InternalIP").ToString().Trim().Length > 0 Then ' Use the internalIP if it has one
   IP = GridList("InternalIP").ToString().Trim()
  Else                                                     ' Else use external IP
   IP = GridList("externalIP").ToString().Trim()
  End If
  If IP.ToString().Trim().Length > 0 Then
   ReturnInfo = ""
   SessionID = ""
   Dim doc As New System.Xml.XmlDocument()
   Dim root, params As System.Xml.XmlNode
   Dim requestStream As System.IO.Stream
   Dim responseStream As System.IO.Stream
   Dim response As System.Net.WebResponse = Nothing
   Dim reader As System.IO.StreamReader = Nothing
   Dim requestWeb As System.Net.WebRequest
   Dim OutBytes() As Byte
   OutParms = "<?xml version=""1.0""?>" +
              " <methodCall>" +
              "  <methodName>session.login_with_password</methodName>" +
              "  <params>" +
              "   <param>" +
              "   <value><string>" + GridList("AccountName").ToString() + "</string></value>" +
              "   </param>" +
              "   <param>" +
              "   <value><string>" + GridList("AcctPassword").ToString() + "</string></value>" +
              "   </param>" +
              "  </params>" +
              " </methodCall>"
   OutBytes = System.Text.Encoding.UTF8.GetBytes(OutParms.ToString())
   If Trace.IsEnabled Then Trace.Warn("GridManager", "StartSession Message: " + "http://" + IP.ToString() + ":" + GridList("Port").ToString() + "/xmlrpc/RemoteAdmin")
   requestWeb = System.Net.WebRequest.Create("http://" + IP.ToString() + ":" + GridList("Port").ToString() + "/xmlrpc/RemoteAdmin")
   requestWeb.Method = "post"
   requestWeb.ContentType = "text/xml"
   requestWeb.ContentLength = OutBytes.Length
   requestStream = requestWeb.GetRequestStream()
   requestStream.Write(OutBytes, 0, OutBytes.Length)
   requestStream.Flush()                                   ' Send outgoing request, clear buffers
   requestStream.Close()
   requestWeb.Timeout = 2000                               ' Two second response timeout
   Try
    response = requestWeb.GetResponse()
    responseStream = response.GetResponseStream()
    reader = New System.IO.StreamReader(responseStream)
    ReturnInfo = reader.ReadToEnd()                        ' Get response text
    response.Close()
    reader.Close()
    If Trace.IsEnabled Then Trace.Warn("GridManager", "Response returned: " + ReturnInfo.ToString())
   Catch ex As Exception
    ReturnInfo = ""
    If Trace.IsEnabled Then Trace.Warn("GridManager", "Message Failed: " + ex.Message().ToString())
   End Try
   requestWeb = Nothing
   If ReturnInfo.ToString().Trim().Length > 0 Then
    doc.LoadXml(ReturnInfo.ToString())
    ' Extract the sessionID from the response xml
    If doc.HasChildNodes Then                              ' For Debug processing to send in email
     root = doc.Item("methodResponse")
     params = root("params")
     SessionID = params.SelectNodes("./param/value/struct/member[name='Value']/value/string")(0).InnerText()
    End If
   End If

   If SessionID.ToString().Trim().Length > 0 Then
    If Trace.IsEnabled Then Trace.Warn("GridManager", "Quit Message: " + "http://" + IP.ToString() + ":" + GridList("Port").ToString() + "/xmlrpc/RemoteAdmin")
    requestWeb = System.Net.WebRequest.Create("http://" + IP.ToString() + ":" + GridList("Port").ToString() + "/xmlrpc/RemoteAdmin")
    requestWeb.Method = "post"
    requestWeb.ContentType = "text/xml"
    OutParms = "<?xml version=""1.0""?>" +
               "<methodCall>" +
               " <methodName>Console.Command</methodName>" +
               " <params>" +
               "  <param>" +
               "   <value><string>" + SessionID.ToString() + "</string></value>" +
               "  </param>" +
               "  <param>" +
               "   <value><string>shutdown</string></value>" +
               "  </param>" +
               " </params>" +
               "</methodCall>"
    OutBytes = System.Text.Encoding.UTF8.GetBytes(OutParms.ToString())
    requestWeb.ContentLength = OutBytes.Length
    requestStream = requestWeb.GetRequestStream()
    requestStream.Write(OutBytes, 0, OutBytes.Length)
    requestStream.Flush()                                  ' Send outgoing request
    requestStream.Close()
    requestWeb.Timeout = 2000                              ' Two second response timeout
    Try
     response = requestWeb.GetResponse()
     responseStream = response.GetResponseStream()
     reader = New System.IO.StreamReader(responseStream)
     ReturnInfo = reader.ReadToEnd()                       ' Get response text
     response.Close()
     reader.Close()
     If Trace.IsEnabled Then Trace.Warn("GridManager", "Response returned: " + ReturnInfo.ToString())
    Catch ex As Exception
     ReturnInfo = ""
     If Trace.IsEnabled Then Trace.Warn("GridManager", "Message Failed: " + ex.Message().ToString())
    End Try
    If Trace.IsEnabled Then Trace.Warn("GridManager", "-- Region Response: " + ReturnInfo.ToString())
    ' Note: ReturnInfo is empty at this point. The reponse does not appear to happen.
    SQLCmd = "Update regionxml Set status=3 " +
             "Where UUID=" + MyDB.SQLStr(Quit.Text)
    If Trace.IsEnabled Then Trace.Warn("GridManager", "Set Region shutting down SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)                           ' Update record
    Session("Counter" + Quit.Text.ToString()) = 1          ' Set process Counter
   End If
   requestWeb = Nothing
   requestStream = Nothing
   responseStream = Nothing
   reader = Nothing
   doc = Nothing
  End If
  GridList.Close()
  GridList = Nothing
  System.Threading.Thread.Sleep(5000)                      ' Cause 5 second delay before proceeding, give region time to close

  Quit.Text = ""                                           ' Allow another command to process
  UpdateData()
 End Sub

 ' Process Region Startup
 Protected Sub Start_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Start.TextChanged
  If Trace.IsEnabled Then Trace.Warn("GridManager", "** Start Region Process **")
  ' Testing process
  SQLCmd = "Update regionxml Set status=1 " +
           "Where UUID=" + MyDB.SQLStr(Start.Text)
  If Trace.IsEnabled Then Trace.Warn("GridManager", "Set Region Starting SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MySite", SQLCmd)                             ' Update record

  ' Get data to start the process
  Dim StartUp As MySqlDataReader   ' 
  SQLCmd = "Select regionName,Port,internalIP,ServerName " +
           "From regionxml " +
           "Where UUID=" + MyDB.SQLStr(Start.Text)
  If Trace.IsEnabled Then Trace.Warn("GridManager", "Set Region Starting SQLCmd: " + SQLCmd.ToString())
  StartUp = MyDB.GetReader("MySite", SQLCmd)
  StartUp.Read()

  '' Process creation in targeted Machine:
  'Dim MyProcess As New System.Diagnostics.Process()
  'Dim IPByName As System.Diagnostics.Process() = System.Diagnostics.Process.GetProcessesByName("Halcyon.exe", StartUp("internalIP"))
  ''MyProcess.StartInfo = IPByName(0).StartInfo
  'If IPByName.Length = 0 Then                               ' Process does not exist, OK to create
  ' MyProcess = IPByName(0)
  ' ' Fatal Flaw: No way to specify process name or machineName in Process. Would not work in a linux OS.
  ' MyProcess.StartInfo.FileName = "\\" + StartUp("ServerName") + "\C$\IMATGrid\Halcyon.bat" 'Filename To start up.
  ' MyProcess.StartInfo.Arguments = "--inimaster=..\Halcyon.ini --inifile=" + StartUp("regionName").ToString().Trim() + ".ini" ' Commandline arguments To use.
  ' MyProcess.StartInfo.WorkingDirectory = "C:\Grid\" + StartUp("regionName") ' Folder For starting the process In.
  ' MyProcess.StartInfo.CreateNoWindow = True ' Do Not open a window for the process.
  ' MyProcess.Start() ' Start Process using info Set above.
  'End If
  ''MyProcess.ProcessName = StartUp("regionName")

  'MyProcess.Close()  ' Close this Object.
  'MyProcess.Dispose() ' Release Object resources.

  StartUp.Close()
  Start.Text = ""                                          ' Allow another command to process
  Display()
 End Sub

 ' Timer activated refresh process
 Protected Sub Refresh_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Refresh.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("GridManager", "** Timer Refresh Process **")
  Refresh.Checked = False
  UpdateData()
 End Sub

 ' Changed Display Sort Order 
 Protected Sub Order_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Order.TextChanged
  If Order.Text.ToString() = "Name" Then
   If Session("SelField") = " regionName" Then             ' Toggle StudentID Order
    Session("SelField") = " regionName Desc"
   Else
    Session("SelField") = " regionName"
   End If
  ElseIf Order.Text.ToString() = "Map" Then
   If Session("SelField") = " Map" Then                    ' Toggle Map Order
    Session("SelField") = " Map Desc"
   Else
    Session("SelField") = " Map"
   End If
  ElseIf Order.Text.ToString() = "Port" Then
   If Session("SelField") = " Port" Then                    ' Toggle Port Order
    Session("SelField") = " Port Desc"
   Else
    Session("SelField") = " Port"
   End If
  ElseIf Order.Text.ToString() = "Owner" Then              ' Toggle Transcript Next Lesson Order
   If Session("SelField") = " OwnerName" Then
    Session("SelField") = " OwnerName Desc"
   Else
    Session("SelField") = " OwnerName"
   End If
  ElseIf Order.Text.ToString() = "Status" Then             ' Toggle EnrlDate Order
   If Session("SelField") = " Status,regionName" Then
    Session("SelField") = " Status Desc,regionName"
   Else
    Session("SelField") = " Status,regionName"
   End If
  End If
  Order.Text = ""                                          ' Clear Selection
  Display()
 End Sub

 Public Function ShowStatus(ByVal UUID As String, ByVal Status As Integer) As String
  Dim tOut As String
  If Status = 0 Then
   tOut = "<img src=""/Images/Icons/Offline.png"" id=""" + UUID.ToString() + "STA"" alt=""Offline"" /> "
  ElseIf Status = 1 Then
   tOut = "<img src=""/Images/Icons/Starting.png"" id=""" + UUID.ToString() + "STA"" alt=""Starting"" /> "
  ElseIf Status = 2 Then
   tOut = "<img src=""/Images/Icons/Online.png"" id=""" + UUID.ToString() + "STA"" alt=""Online"" /> "
  Else
   tOut = "<img src=""/Images/Icons/Closing.png"" id=""" + UUID.ToString() + "STA"" alt=""Closing"" /> "
  End If
  Return tOut.ToString()
 End Function

 ' Process send Message Icon
 Public Function SetMsg(ByVal UUID As String, ByVal Status As Integer, tServer As String) As String
  Dim tOut As String
  If Status = 2 Then                                       ' Online: Allow Region Restart
   tOut = "<img src=""/Images/Icons/MessageIcon.png"" id=""" + UUID.ToString() + "MSG"" alt=""Message"" " +
          "onclick=""OpenMsg('" + UUID.ToString() + "','Enter message to send to region " + tServer.ToString() + ":');"" " +
          "style=""cursor: pointer;"" />"
  Else                                                     ' Disable function
   tOut = ""
  End If
  Return tOut.ToString()
 End Function

 ' Process region restart
 Public Function SetRestart(ByVal UUID As String, ByVal Status As Integer) As String
  Dim tOut As String
  If Status = 2 Then                                       ' Online: Allow Region Restart
   tOut = "<img src=""/Images/Icons/RestartIcon.png"" id=""" + UUID.ToString() + "RES"" alt=""Restart"" onclick=""DoRestart('" + UUID.ToString() + "');"" style=""cursor: pointer;"" />"
  Else                                                     ' Disable function
   tOut = "<img src=""/Images/Icons/RestartIconDis.png"" id=""" + UUID.ToString() + "RES"" alt=""Disabled"" />"
  End If
  Return tOut.ToString()
 End Function

 ' Process Region Start / Shutdown
 Public Function SetAction(ByVal UUID As String, ByVal Status As Integer) As String
  Dim tOut As String
  If Status = 0 Then                                       ' Set for startup Action
   tOut = "<img src=""/Images/Icons/StartIcon.png"" id=""" + UUID.ToString() + "ACT"" alt=""Startup"" onclick=""DoStart('" + UUID.ToString() + "');"" style=""cursor: pointer;"" />"
  ElseIf Status = 1 Then                                   ' Set for starting up in process
   tOut = "<img src=""/Images/Icons/StartIconDis.png"" id=""" + UUID.ToString() + "ACT"" alt=""Startup"" style=""cursor: none;"" />"
  ElseIf Status = 2 Then                                   ' Set for shutdown Action
   tOut = "<img src=""/Images/Icons/StopIcon.png"" id=""" + UUID.ToString() + "ACT"" alt=""Shutdown"" onclick=""DoShutdown('" + UUID.ToString() + "');"" style=""cursor: pointer;"" />"
  Else                                                     ' Set shutdown in process
   tOut = "<img src=""/Images/Icons/StopIconDis.png"" id=""" + UUID.ToString() + "ACT"" alt=""Shutdown"" style=""cursor: none;"" />"
  End If
  Return tOut.ToString()
 End Function

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  SqlClient.SqlConnection.ClearAllPools()      ' Causes all pooled connection to be forced closed when finished, reducing pooled connection size.
 End Sub
End Class
