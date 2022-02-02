
Partial Class Administration_ServerForm
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
 '* This page is an entry form page asscociated with records displayed by the Select page, to add or 
 '* update records in a table. It may also be modified for any sort of data entry form process.
 '* 

 '* Built from MyWorld Form template v. 1.0

 ' Define common Page class properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private PageCtl As New GridLib
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Validate logon and session existance.
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Response.Redirect("/Default.aspx")                      ' Return to logon page
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("/Default.aspx")
  End If
  If Session("Access") < 9 Then                            ' SysAdmin Only
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("ServerForm", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page accessed setup
   ' Define process unique objects here

   ' Get identity key
   If Len(Request("KeyID")) > 0 Then                       ' Update or Add Mode
    KeyID.Value = Request("KeyID")
   Else                                                    ' Critical error, cancel operation
    Response.Redirect("ServerSelect.aspx")
   End If

   ' Setup general page controls

   ' Display data fields based on edit or add mode
   If CDbl(KeyID.Value) > 0 Then                           ' Edit Mode, show database values
    Dim drApp As MySqlDataReader
    SQLCmd = "Select ServerName,AccountName,AcctPassword,externalIP,internalIP,PortList," +
             " (Select count(externalIP) as Connects From regionxml Where externalIP=serverhosts.externalIP) as Used " +
             "From serverhosts " +
             "Where ID=" + MyDB.SQLNo(KeyID.Value)
    If Trace.IsEnabled Then Trace.Warn("ServerForm", "Get display Values SQLCmd: " + SQLCmd.ToString())
    drApp = MyDB.GetReader("MySite", SQLCmd)
    If drApp.Read() Then
     ServerName.Text = drApp("ServerName").ToString()
     AccountName.Text = drApp("AccountName").ToString()
     AcctPassword.Text = drApp("AcctPassword").ToString()
     externalIP.Text = drApp("externalIP").ToString()
     PriorExtIP.Value = drApp("externalIP").ToString()
     internalIP.Text = drApp("internalIP").ToString()
     PortList.Text = drApp("PortList").ToString()
     PriorPorts.Value = drApp("PortList").ToString()
     DelTitle.InnerText = "Entry: " + drApp("ServerName").ToString().Trim()
     RegCnt.InnerText = drApp("Used").ToString()
    End If
    ' Setup Edit Mode page display controls
    PageTitle.InnerText = "Edit Server"
    UpdDelBtn.Visible = True                               ' Allow Update and Delete button to show
    Button2.Visible = (drApp("Used") = 0)                  ' Allow Delete button to show
    AddBtn.Visible = False                                 ' Disable the Add button
    drApp.Close()
   Else                                                    ' Add Mode, show blank fields
    ServerName.Text = ""
    AccountName.Text = ""
    AcctPassword.Text = ""
    externalIP.Text = ""
    internalIP.Text = ""
    PortList.Text = ""
    ' Setup Add Mode page display controls
    PageTitle.InnerText = "New Server"
    UpdDelBtn.Visible = False                              ' Disable Update and Delete button
    AddBtn.Visible = True                                  ' Allow the Add button to show
   End If
   FieldName.Focus()                                       ' Set focus to the first field for entry

   Dim SBMenu As New TreeView
   ' Set up navigation options
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'ServerForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
   SBMenu.AddItem("P", "ServerSelect.aspx", "Server Manager")
   SBMenu.AddItem("P", "GridManager.aspx", "Grid Manager")
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   If Trace.IsEnabled Then Trace.Warn("ServerForm", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   ' Close Sidebar Menu object
   SBMenu.Close()
  End If

 End Sub

 ' Process data validation checks
 Private Function ValAddEdit(ByVal tAdd As Boolean) As String
  ' Parameter tAdd allows selections of Add mode only testing
  Dim aMsg, PortStr As String
  aMsg = ""
  ' Process error checking as required, place messages in tMsg.
  ' ServerName,AccountName,AcctPassword,externalIP,internalIP,PortList
  If ServerName.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Field Server Name!\r\n"
  End If
  If AccountName.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Field Account Name!\r\n"
  End If
  If AcctPassword.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Field Password!\r\n"
  End If
  If externalIP.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Field external IP!\r\n"
  Else
   ' External IP assigned to another server?
   Dim ChkExtIP As MySqlDataReader
   SQLCmd = "Select serverName " +
            "From serverhosts " +
            "Where externalIP=" + MyDB.SQLStr(externalIP.Text) + " " +
            IIf(Not tAdd, " and ID<>" + MyDB.SQLNo(KeyID.Value), "")
   If Trace.IsEnabled Then Trace.Warn("ServerForm", "Check ExtIP assigned SQLCmd: " + SQLCmd.ToString())
   ChkExtIP = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridManager", "DB Error: " + MyDB.ErrMessage().ToString())
   If ChkExtIP.HasRows() Then
    ChkExtIP.Read()
    aMsg = aMsg.ToString() + ChkExtIP("serverName").ToString() + " has external IP already assigned!\r\n"
   End If
   ChkExtIP.Close()
  End If
  If internalIP.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Field internal IP!\r\n"
  Else
   ' Internal IP assigned to another server?
   Dim ChkIntIP As MySqlDataReader
   SQLCmd = "Select serverName " +
            "From serverhosts " +
            "Where internalIP=" + MyDB.SQLStr(internalIP.Text) + " " +
            IIf(Not tAdd, " and ID<>" + MyDB.SQLNo(KeyID.Value), "")
   If Trace.IsEnabled Then Trace.Warn("ServerForm", "Check IntIP assigned SQLCmd: " + SQLCmd.ToString())
   ChkIntIP = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridManager", "DB Error: " + MyDB.ErrMessage().ToString())
   If ChkIntIP.HasRows() Then
    ChkIntIP.Read()
    aMsg = aMsg.ToString() + ChkIntIP("serverName").ToString() + " has internal IP already assigned!\r\n"
   End If
   ChkIntIP.Close()
  End If
  ' Validate port range list against ports assigned in serverports table
  If PortList.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Field Port List!\r\n"
  Else
   ' invalid characters test
   Dim I As Integer
   Dim Test, BadC As String
   BadC = ""
   Test = PortList.Text.ToString().Replace(" ", "") ' Remove spaces
   For I = 0 To (Test.ToString().Length - 1)
    If Not ("0123456789-,").ToString().Contains(Test.ToString().Substring(I, 1)) Then
     BadC = BadC.ToString() + Test.ToString().Substring(I, 1)
    End If
   Next
   If BadC.ToString().Length > 0 Then
    aMsg = aMsg.ToString() + "Port List contains invalid characters: (" + BadC.ToString() + ")!\r\n"
   Else
    PortStr = Convert(PortList.Text)
    ' Any ports assigned to another server?
    Dim ChkPorts As MySqlDataReader
    SQLCmd = "Select Count(port) as Used " +
             "From serverports " +
             "Where " + IIf(Not tAdd, "externalIP<>" + MyDB.SQLStr(PriorExtIP.Value) + " and ", "") +
             "port in (" + PortStr.ToString() + ")"
    If Trace.IsEnabled Then Trace.Warn("ServerForm", "Check Port overlap SQLCmd: " + SQLCmd.ToString())
    ChkPorts = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridManager", "DB Error: " + MyDB.ErrMessage().ToString())
    ChkPorts.Read()
    If ChkPorts("Used") > 0 Then
     aMsg = aMsg.ToString() + ChkPorts("Used").ToString() + " ports already assigned!\r\n"
    End If
    ChkPorts.Close()
    If Not tAdd Then                                        ' Update only test
     If PriorPorts.Value.ToString().Replace(" ", "").ToString() <> PortList.Text.ToString().Replace(" ", "").ToString() Then
      ' Any region assigned ports missing in the list for this server?
      SQLCmd = "Select Count(port) as Used " +
               "From regionxml " +
               "Where externalIP=" + MyDB.SQLStr(PriorExtIP.Value) + " and port not in (" + PortStr.ToString() + ")"
      If Trace.IsEnabled Then Trace.Warn("ServerForm", "Check missing ports assigned SQLCmd: " + SQLCmd.ToString())
      ChkPorts = MyDB.GetReader("MySite", SQLCmd)
      ChkPorts.Read()
      If ChkPorts("Used") > 0 Then
       aMsg = aMsg.ToString() + ChkPorts("Used").ToString() + " ports already assigned not in port list!\r\n"
      End If
      ChkPorts.Close()
     End If
    End If
    I = Nothing
    Test = Nothing
    BadC = Nothing
   End If
  End If
  Return aMsg
 End Function

 ' Update Button
 Private Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
  Dim tMsg, PortStr As String
  tMsg = ValAddEdit(False)

  If tMsg.ToString().Trim().Length = 0 Then
   'ServerName,AccountName,AcctPassword,externalIP,internalIP,PortList
   SQLCmd = "Update serverhosts Set " +
            "ServerName=" + MyDB.SQLStr(ServerName.Text) + "," + "AccountName=" + MyDB.SQLStr(AccountName.Text) + "," +
            "AcctPassword=" + MyDB.SQLStr(AcctPassword.Text) + "," + "externalIP=" + MyDB.SQLStr(externalIP.Text) + "," +
            "internalIP=" + MyDB.SQLStr(internalIP.Text) + "," + "PortList=" + MyDB.SQLStr(PortList.Text) + " " +
            "Where ID=" + MyDB.SQLNo(KeyID.Value)
   If Trace.IsEnabled Then Trace.Warn("ServerForm", "Update serverhosts SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   Else
    ' Did the port list change?
    If PriorPorts.Value.ToString().Replace(" ", "").ToString() <> PortList.Text.ToString().Replace(" ", "").ToString() Then
     ' remove then reassign ports.
     PortStr = Convert(PortList.Text)
     SQLCmd = "Delete From serverports Where externalIP=" + MyDB.SQLStr(PriorExtIP.Value)
     If Trace.IsEnabled Then Trace.Warn("ServerForm", "Delete serverports SQLCmd: " + SQLCmd.ToString())
     MyDB.DBCmd("MySite", SQLCmd)
     MakePorts(PortStr)
     PriorPorts.Value = PortList.Text.ToString()
     ' Check for any regions assigned to prior IP
     Dim ChkReg As MySqlDataReader
     SQLCmd = "Select Count(externalIP) as Used From regionxml Where externalIP=" + MyDB.SQLStr(PriorExtIP.Value)
     If Trace.IsEnabled Then Trace.Warn("ServerForm", "Check regions assigned SQLCmd: " + SQLCmd.ToString())
     ChkReg = MyDB.GetReader("MySite", SQLCmd)
     ChkReg.Read()
     If ChkReg("Used") > 0 Then
      ' Reassign the new server IP addresses
      SQLCmd = "Update regionxml Set externalIP=" + MyDB.SQLStr(externalIP.Text) + "," +
               "internalIP=" + MyDB.SQLStr(internalIP.Text) + " " +
               "Where externalIP=" + MyDB.SQLStr(PriorExtIP.Value)
      If Trace.IsEnabled Then Trace.Warn("ServerForm", "Update regionxml SQLCmd: " + SQLCmd.ToString())
      MyDB.DBCmd("MySite", SQLCmd)
     End If
    End If
   End If
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot update Entry:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")          ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                ' Display Alert Message
  End If

 End Sub

 ' Delete Button
 Private Sub SetDel_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SetDel.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("ServerForm", "Delete process Event")
  ' Action is triggered by a JavaScript process, not a button click.
  'Remove all related entries in all affected tables.
  SQLCmd = "Delete From serverports " +
           "Where externalIP=(Select externalIP From serverhosts Where ID=" + MyDB.SQLNo(KeyID.Value) + ")"
  If Trace.IsEnabled Then Trace.Warn("ServerForm", "Delete Dependencies SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MySite", SQLCmd)
  'Remove entry record
  SQLCmd = "Delete From serverhosts Where ID=" + MyDB.SQLNo(KeyID.Value)
  If Trace.IsEnabled Then Trace.Warn("ServerForm", "Delete serverhosts SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MySite", SQLCmd)
  If Not Trace.IsEnabled Then
   Response.Redirect("ServerSelect.aspx")                  ' Return to Selection page
  End If
 End Sub

 ' Add Button
 Private Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
  Dim SQLFields, SQLValues, tMsg, PortStr As String
  tMsg = ValAddEdit(True)

  If tMsg.Length = 0 Then
   SQLFields = "ServerName,AccountName,AcctPassword,externalIP,internalIP,PortList"
   SQLValues = MyDB.SQLStr(ServerName.Text) + "," + MyDB.SQLStr(AccountName.Text) + "," +
               MyDB.SQLStr(AcctPassword.Text) + "," + MyDB.SQLStr(externalIP.Text) + "," +
               MyDB.SQLStr(internalIP.Text) + "," + MyDB.SQLStr(PortList.Text)
   SQLCmd = "Insert Into serverhosts (" + SQLFields + ") Values (" + SQLValues + ")"
   If Trace.IsEnabled Then Trace.Warn("ServerForm", "Insert serverhosts SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   Else
    ' Create serverport list of assigned ports
    PortStr = Convert(PortList.Text)
    MakePorts(PortStr)
   End If
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot add Entry:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")          ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                ' Display Alert Message
  Else
   If Not Trace.IsEnabled Then
    Response.Redirect("ServerSelect.aspx")                 ' Return to Selection page
   End If
  End If
 End Sub

 Private Sub MakePorts(tPorts As String)
  If Trace.IsEnabled Then Trace.Warn("ServerForm", "** MakePorts Called **")
  Dim SQLFields, SQLValues, Ports() As String
  Ports = tPorts.ToString().Split(",")
  SQLFields = "externalIP,port"
  For I = 0 To Ports.Length - 1
   SQLValues = MyDB.SQLStr(externalIP.Text) + "," + MyDB.SQLStr(Ports(I).ToString())
   SQLCmd = "Insert into serverports (" + SQLFields.ToString() + ") values (" + SQLValues.ToString() + ")"
   If Trace.IsEnabled Then Trace.Warn("ServerForm", "Add serverports SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("ServerForm", "DB Error: " + MyDB.ErrMessage().ToString())
  Next
 End Sub

 Private Function Convert(tPortList As String) As String
  ' Process ports as list by item or range
  Dim PortItems(), PortList(), Ports As String
  Ports = ""
  If Trace.IsEnabled Then Trace.Warn("ServerForm", "tPortList: " + tPortList.ToString())
  If tPortList.ToString().Contains(",") Then
   PortItems = tPortList.ToString().Replace(" ", "").Split(",") ' Remove spaces
  Else                                                     ' Range of port numbers or single port
   ReDim PortItems(0)
   PortItems(0) = tPortList.ToString().Replace(" ", "")    ' Remove spaces
  End If
  Dim I, J As Integer
  I = 0
  J = 0
  For I = 0 To PortItems.Length - 1                        ' Expand list of port ranges into Port list
   If PortItems(I).ToString().Contains("-") Then           ' Expand range
    PortList = PortItems(I).ToString().Split("-")
    For J = CInt(PortList(0)) To CInt(PortList(1))         ' Expand port range
     Ports = Ports.ToString() + IIf(Ports.ToString().Trim().Length > 0, ",", "") + J.ToString()
    Next
   Else                                                    ' Add port number to list
    Ports = Ports.ToString() + IIf(Ports.ToString().Trim().Length > 0, ",", "") + PortItems(I).ToString()
   End If
  Next

  Return Ports.ToString()
 End Function

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub

End Class
