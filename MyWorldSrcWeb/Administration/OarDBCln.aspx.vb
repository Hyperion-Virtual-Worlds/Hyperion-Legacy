
Partial Class Administration_OarDBCln
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
  If Session("Access") < 6 Then                            ' Grid Administrator or SysAdmin access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page accessed setup
   ' Define process unique objects here

   ' Setup general page controls
   PageTitle.InnerText = "Database Correction for Region OAR Loaded Contents"
   StepNum.Value = 1                                       ' Process step to complete!
   Session("RegUUID") = ""
   Session("RegName") = ""
   ' OldAcctTbl = List of all old accounts from OAR file
   ' OldOwnersTbl = List of Old Owner accounts found in region
   ' OwnersMatTbl = List of matched old and new Owner accounts
   ' OldCreatorsTbl = List of Old Creator accounts found in region
   ' CreatorMatTbl = List of matched old and new Creator accounts
   ' RefAccounts = List of all creators found in region not reassigned, to be added as reference accounts.
   Dim ATable As New DataTable
   ATable.Columns.Add("UUID")
   ATable.Columns.Add("username")
   ATable.Columns.Add("lastname")
   ATable.AcceptChanges()
   Dim Key(1) As DataColumn
   Key(0) = ATable.Columns("UUID")
   ATable.PrimaryKey = Key
   ATable.AcceptChanges()
   Session("OldAcctTbl") = ATable.Clone()
   Session("OldOwnersTbl") = ATable.Clone()
   Session("OldCreatorsTbl") = ATable.Clone()
   Session("RefAccounts") = ATable.Clone()
   ATable.Dispose()

   Dim BTable As New DataTable
   BTable.Columns.Add("OldUUID")
   BTable.Columns.Add("OldName")
   BTable.Columns.Add("NewUUID")
   BTable.Columns.Add("NewName")
   BTable.AcceptChanges()
   Session("OwnersMatTbl") = BTable.Clone()
   Session("CreatorsMatTbl") = BTable.Clone()
   BTable.Dispose()
   OldAccounts.Value = ""

   Dim SBMenu As New TreeView
   ' Set up navigation options
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'OarDBCln.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   ' Close Sidebar Menu object
   SBMenu.Close()
   Display()
  End If

 End Sub

 ' Show Regions listing for Estate
 Private Sub Display()
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** Display() Called")
  ' Get Display list Items here
  Step1.Visible = (StepNum.Value = 1)
  Step2.Visible = (StepNum.Value = 2)
  Step3.Visible = (StepNum.Value = 3)
  Step4.Visible = (StepNum.Value = 4)
  Step5.Visible = (StepNum.Value = 5)
  Step6.Visible = (StepNum.Value = 6)
  Dim Row As DataRow
  Dim tHtml As String

  If Session("RegName") <> "" Then
   ShowRegName.InnerHtml = "<b>Processing for:</b> " + Session("RegName").ToString()
  End If
  ' Display info for each block to be processed
  If CInt(StepNum.Value) = 1 Then                          ' Display offline Regions list
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "*Step 1 Display")
   Dim GetReg As MySqlDataReader
   SQLCmd = "Select UUID,regionName " +
            "From regionxml " +
            "Where Status=0 " +
            "Order by regionName"
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Get region list SQLCmd: " + SQLCmd.ToString())
   GetReg = MyDB.GetReader("MySite", SQLCmd)
   If GetRegion.Items.Count > 0 Then GetRegion.Items.Clear()
   While GetReg.Read()
    GetRegion.Items.Add(New ListItem(GetReg("regionName").ToString(), GetReg("UUID").ToString()))
   End While
   GetReg.Close()
   If GetRegion.Items.Count() = 0 Then
    BodyTag.Attributes.Add("onload", "ShowMsg();")         ' Activate onload option to show message
    PageCtl.AlertMessage(Me, "No regions are offline to list! Please close region first.", "ErrMsg") ' Display Alert Message
   End If

  ElseIf CInt(StepNum.Value) = 2 Then                      ' Old Accounts Entry
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "*Step 2 Display")
   ' Show data entry for old accounts data entry

  ElseIf CInt(StepNum.Value) = 3 Then                      ' Display Matched Owner Accounts and options
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "*Step 3 Display")
   ' Display Old Owner and New Owner Account match selections best guess
   Dim OldOwnersTbl As New DataTable                       ' UUID, username, lastname
   OldOwnersTbl = Session("OldOwnersTbl")                  ' Restore table for use here
   Dim OwnersMatTbl As New DataTable
   OwnersMatTbl = Session("OwnersMatTbl")                  ' Restore table for use here
   Dim RowNum As Integer

   If OwnersMatTbl.Rows.Count() = 0 Then
    tHtml = "<p>No Owner Account Name Matches were found.</p>" + vbCrLf
   Else
    ' Show Match list 
    tHtml = "<table style=""width:100%; border: solid 1px #000000;"">" + vbCrLf +
           " <tr>" + vbCrLf +
           "  <td style=""width:45%;""><b>Old Account</b></td>" + vbCrLf +
           "  <td style=""width:45%;""><b>New Account</b></td>" + vbCrLf +
           "  <td style=""width:10%;""><b>Remove</b></td>" + vbCrLf +
           " </tr>" + vbCrLf
    RowNum = 0
    For Each Row In OwnersMatTbl.Rows()
     tHtml = tHtml.ToString() +
           " <tr>" + vbCrLf +
           "  <td style=""width:45%;"">" + Row("OldName").ToString() + "</td>" + vbCrLf +
           "  <td style=""width:45%;"">" + Row("NewName").ToString() + "</td>" + vbCrLf +
           "  <td style=""width:10%;"">" +
           "<span class=""Errors"" onclick=""RemOEntry(" + RowNum.ToString() + ")"" style=""cursor:pointer;color:#FF1111;"" title=""Remove Match"">X</span>" +
           "</td>" + vbCrLf +
           " </tr>" + vbCrLf
     RowNum = RowNum + 1
    Next
    tHtml = tHtml.ToString() +
           " </table>" + vbCrLf
   End If
   OwnersList.InnerHtml = tHtml.ToString()                 ' Place html contents into page 
   OwnersMatTbl.Dispose()

   If OldOwnersTbl.Rows.Count() = 0 Then
    ShowOMatching.Visible = False
   Else
    ' Create match drop lists to create / new matches
    Dim SortRows As DataRow()
    SortRows = OldOwnersTbl.Select("UUID<>''", "lastname,username") ' UUID, username, lastname
    If OldOwners.Items.Count() > 0 Then OldOwners.Items.Clear() ' Remove prior drop list entries
    OldOwners.Items.Add(New ListItem("Select", ""))
    For Each Row In SortRows
     OldOwners.Items.Add(New ListItem(Row("username").ToString() + " " + Row("lastname").ToString(), Row("UUID").ToString()))
    Next
    ' List of current grid active accounts
    Dim GridUsers As MySqlDataReader
    SQLCmd = "Select UUID, username, lastname " +
             "From users " +
             "Where passwordHash<>'NoPass' " +
             "Order by lastname,username"
    If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Get users list SQLCmd: " + SQLCmd.ToString())
    GridUsers = MyDB.GetReader("MyData", SQLCmd)
    If GridAccts1.Items.Count() > 0 Then GridAccts1.Items.Clear()
    GridAccts1.Items.Add(New ListItem("Select", ""))
    While GridUsers.Read()
     GridAccts1.Items.Add(New ListItem(GridUsers("username").ToString() + " " + GridUsers("lastname").ToString(), GridUsers("UUID").ToString()))
    End While
    GridUsers.Close()
   End If
   OldOwnersTbl.Dispose()

  ElseIf CInt(StepNum.Value) = 4 Then                      ' Match Creator Accounts
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "*Step 4 Display")
   ' Display Old Creator and New Creator Account match selections best guess
   Dim OldCreatorsTbl As New DataTable                     ' UUID, username, lastname
   OldCreatorsTbl = Session("OldCreatorsTbl")              ' Restore table for use here
   Dim CreatorsMatTbl As New DataTable
   CreatorsMatTbl = Session("CreatorsMatTbl")              ' Restore table for use here
   If CreatorsMatTbl.Rows.Count() = 0 Then
    tHtml = "<p>No Creator Account Name Matches were found.</p>" + vbCrLf
   Else
    Dim RowNum As Integer
    ' Show Match list 
    tHtml = "<table style=""width:100%; border: solid 1px #000000;"">" + vbCrLf +
           " <tr>" + vbCrLf +
           "  <td style=""width:45%;""><b>Old Account</b></td>" + vbCrLf +
           "  <td style=""width:45%;""><b>New Account</b></td>" + vbCrLf +
           "  <td style=""width:10%;""><b>Remove</b></td>" + vbCrLf +
           " </tr>" + vbCrLf
    RowNum = 0
    For Each Row In CreatorsMatTbl.Rows()
     tHtml = tHtml.ToString() +
           " <tr>" + vbCrLf +
           "  <td style=""width:45%;"">" + Row("OldName").ToString() + "</td>" + vbCrLf +
           "  <td style=""width:45%;"">" + Row("NewName").ToString() + "</td>" + vbCrLf +
           "  <td style=""width:10%;"">" +
           "<span class=""Errors"" onclick=""RemCEntry(" + RowNum.ToString() + ")"" style=""cursor:pointer;color:#FF1111;"" title=""Remove Match"">X</span>" +
           "</td>" + vbCrLf +
           " </tr>" + vbCrLf
     RowNum = RowNum + 1
    Next
    tHtml = tHtml.ToString() +
           " </table>" + vbCrLf
   End If
   CreatorsList.InnerHtml = tHtml.ToString()               ' Place html contents into page 
   CreatorsMatTbl.Dispose()

   If OldCreatorsTbl.Rows.Count() = 0 Then
    ShowCMatching.Visible = False
   Else
    ' Create match drop lists to create / new matches
    Dim SortRows As DataRow()
    SortRows = OldCreatorsTbl.Select("UUID<>''", "lastname,username") ' UUID, username, lastname
    If OldCreators.Items.Count() > 0 Then OldCreators.Items.Clear() ' Remove prior drop list entries
    OldCreators.Items.Add(New ListItem("Select", ""))
    For Each Row In SortRows
     OldCreators.Items.Add(New ListItem(Row("username").ToString() + " " + Row("lastname").ToString(), Row("UUID").ToString()))
    Next
    ' List of current grid active accounts
    Dim GridUsers As MySqlDataReader
    SQLCmd = "Select UUID, username, lastname " +
             "From users " +
             "Where passwordHash<>'NoPass' " +
             "Order by lastname,username"
    If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Get users list SQLCmd: " + SQLCmd.ToString())
    GridUsers = MyDB.GetReader("MyData", SQLCmd)
    If GridAccts2.Items.Count() > 0 Then GridAccts2.Items.Clear()
    GridAccts2.Items.Add(New ListItem("Select", ""))
    While GridUsers.Read()
     GridAccts2.Items.Add(New ListItem(GridUsers("username").ToString() + " " + GridUsers("lastname").ToString(), GridUsers("UUID").ToString()))
    End While
    GridUsers.Close()
   End If
   OldCreatorsTbl.Dispose()

  ElseIf CInt(StepNum.Value) = 5 Then                      ' DB Changes Summary
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "*Step 5 Display")
   ' Display Changes Summary to be applied

   ' OldAcctTbl = List of all old accounts from OAR file
   ' OldOwnersTbl = List of Old Owner accounts found in region
   ' OwnersMatTbl = List of matched old and new Owner accounts
   ' OldCreatorsTbl = List of Old Creator accounts found in region
   ' CreatorMatTbl = List of matched old and new Creator accounts
   Dim OwnersMatTbl As New DataTable
   OwnersMatTbl = Session("OwnersMatTbl")                  ' Restore table for use here
   Dim CreatorsMatTbl As New DataTable
   CreatorsMatTbl = Session("CreatorsMatTbl")              ' Restore table for use here
   tHtml = ""

   If OwnersMatTbl.Rows.Count() = 0 Then
    tHtml = tHtml.ToString() +
            "<p>No Owner Account Name Matches were found.</p>" + vbCrLf
   Else
    ' Show Owners Match list 
    tHtml = tHtml.ToString() +
            "<h1>Process Owner / Creator Account Matching Summary</h1>" + vbCrLf +
            "<h2>Owner Match List to Apply</h2>" + vbCrLf +
            "<table style=""width:100%; border: solid 1px #000000;"">" + vbCrLf +
            " <tr>" + vbCrLf +
            "  <td style=""width:50%;""><b>Old Account</b></td>" + vbCrLf +
            "  <td style=""width:50%;""><b>New Account</b></td>" + vbCrLf +
            " </tr>" + vbCrLf
    For Each Row In OwnersMatTbl.Rows()
     tHtml = tHtml.ToString() +
             " <tr>" + vbCrLf +
             "  <td style=""width:50%;"">" + Row("OldName").ToString() + "</td>" + vbCrLf +
             "  <td style=""width:50%;"">" + Row("NewName").ToString() + "</td>" + vbCrLf +
             " </tr>" + vbCrLf
    Next
    tHtml = tHtml.ToString() +
            "</table>" + vbCrLf
   End If
   OwnersMatTbl.Dispose()

   If CreatorsMatTbl.Rows.Count() = 0 Then
    tHtml = tHtml.ToString() +
            "<p>No Creator Account Name Matches were found.</p>" + vbCrLf
   Else
    ' Show Creators Match list 
    tHtml = tHtml.ToString() +
            "<h2>Creator Match List to Apply</h2>" + vbCrLf +
            "<table style=""width:100%; border: solid 1px #000000;"">" + vbCrLf +
            " <tr>" + vbCrLf +
            "  <td style=""width:50%;""><b>Old Account</b></td>" + vbCrLf +
            "  <td style=""width:50%;""><b>New Account</b></td>" + vbCrLf +
            " </tr>" + vbCrLf
    For Each Row In CreatorsMatTbl.Rows()
     tHtml = tHtml.ToString() +
             " <tr>" + vbCrLf +
             "  <td style=""width:50%;"">" + Row("OldName").ToString() + "</td>" + vbCrLf +
             "  <td style=""width:50%;"">" + Row("NewName").ToString() + "</td>" + vbCrLf +
             " </tr>" + vbCrLf
    Next
    tHtml = tHtml.ToString() +
            "</table>" + vbCrLf
   End If
   tHtml = tHtml.ToString() +
            "<p>Any Unassigned old creators will be added as reference accounts.</p>" + vbCrLf
   ShowSummary.InnerHtml = tHtml.ToString()                ' Place html contents into page 
   CreatorsMatTbl.Dispose()

  Else                                                     ' Step 6. Process completed! Start up region.
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "*Step 6 Display")
   Dim OldAcctTbl As New DataTable                         ' UUID, username, lastname
   OldAcctTbl = Session("OldAcctTbl")                      ' Restore table for use here
   tHtml = ""
   Dim Temp As String
   Temp = ""

   ' Get all old Owners still in Region to be Displayed in Close Report
   Dim GetOldOwners As MySqlDataReader
   SQLCmd = "Select OwnerID " +
            "From " +
            " ((Select Distinct OwnerID From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + ") " +
            "  Union " +
            "  (Select Distinct OwnerID From primitems" +
            "   Where primID in (Select UUID From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + "))) as RawData " +
            "Where OwnerID not in (Select UUID From users) " +
            "Order by OwnerID"
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Get Unmatched OwnerIDs SQLCmd: " + SQLCmd.ToString())
   GetOldOwners = MyDB.GetReader("MyData", SQLCmd)
   If GetOldOwners.HasRows() Then
    While GetOldOwners.Read()
     ' Find match UUID in OldAccounts table to get the name elements
     Row = OldAcctTbl.Rows.Find(GetOldOwners("OwnerID"))
     If Row Is Nothing Then
      If Temp.ToString().Trim().Length > 0 Then
       Temp = Temp.ToString() + ", "
      End If
      Temp = Temp.ToString() + GetOldOwners("OwnerID").ToString()
     End If
    End While
    If Temp.ToString().Trim().Length > 0 Then
     tHtml = tHtml.ToString() +
             "<h2>Old OAR Owners Not Matched to Current Accounts!</h2>" +
             "<p>" + Temp.ToString() + "</p>"
    End If
   End If
   GetOldOwners.Close()

   Temp = ""
   ' Get all old Creators still in Region to be Displayed in Close Report
   Dim GetOldCreators As MySqlDataReader
   SQLCmd = "Select CreatorID " +
            "From " +
            " ((Select Distinct CreatorID From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + ") " +
            "  Union " +
            "  (Select Distinct CreatorID From primitems" +
            "   Where primID in (Select UUID From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + "))) as RawData " +
            "Where CreatorID not in (Select UUID From users) " +
            "Order by CreatorID"
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Get Unmatched CreatorIDs SQLCmd: " + SQLCmd.ToString())
   GetOldCreators = MyDB.GetReader("MyData", SQLCmd)
   If GetOldCreators.HasRows() Then
    While GetOldCreators.Read()
     ' Find match UUID in OldAccounts table to get the name elements
     Row = OldAcctTbl.Rows.Find(GetOldCreators("CreatorID"))
     If Row Is Nothing Then
      If Temp.ToString().Trim().Length > 0 Then
       Temp = Temp.ToString() + ", "
      End If
      Temp = Temp.ToString() + GetOldCreators("CreatorID").ToString()
     End If
    End While
    If Temp.ToString().Trim().Length > 0 Then
     tHtml = tHtml.ToString() +
             "<h2>Old OAR Creators Not Matched to Current Accounts!</h2>" +
             "<p>" + Temp.ToString() + "</p>"
    End If
   End If
   GetOldCreators.Close()

   ' Show Creator accounts added as reference accounts.
   Temp = ""
   Dim RefAccounts As New DataTable                        ' UUID, username, lastname
   RefAccounts = Session("RefAccounts")
   If RefAccounts.Rows.Count() > 0 Then
    For Each Row In RefAccounts.Rows()
     If Temp.ToString().Trim().Length > 0 Then
      Temp = Temp.ToString() + ", "
     End If
     Temp = Temp.ToString() +
            Row("username").ToString() + " " + Row("lastname").ToString()
    Next
    If Temp.ToString().Trim().Length > 0 Then
     tHtml = tHtml.ToString() +
             "<h2>Old Creator Accounts Added as Reference Accounts.</h2>" +
             "<p>" + Temp.ToString() + "</p>"
    End If
   End If
   RefAccounts.Dispose()
   OldAcctTbl.Dispose()
   ShowRefAccts.InnerHtml = tHtml.ToString() +
                            "<p>Region " + Session("RegName").ToString() + " may now be started up.</p>"
  End If

 End Sub

 ' Process data validation checks
 Private Function ValAddEdit() As String
  ' Process for active StepNum
  Dim aMsg As String
  aMsg = ""
  ' Process error checking as required, place messages in aMsg.
  If CInt(StepNum.Value) = 2 Then ' Old Accounts Entry Validation
   If OldAccounts.Value.ToString().Trim().Length = 0 Then
    aMsg = aMsg.ToString() + "Missing Old Accounts List!"
   End If
   'ElseIf CInt(StepNum.Value) = 3 Then
   'ElseIf CInt(StepNum.Value) = 4 Then
  End If
  'If FieldName.Text.ToString().Trim().Length = 0 Then
  ' aMsg = aMsg.ToString() + "Missing Field Name!\r\n"
  'End If
  Return aMsg
 End Function

 ' Step 1 Button - Region Selection Validation Process, Setup for OAR Old Accounts Entry
 Private Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** Step 1 Button Clicked")
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then Response.Redirect("Admin.aspx") ' Session loss check

  ' Check if selected region is online - It must not be on.
  Dim ChkReg As MySqlDataReader
  SQLCmd = "Select regionName " +
           "From regions " +
           "Where uuid=" + MyDB.SQLStr(GetRegion.SelectedItem.Value)
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Get region list SQLCmd: " + SQLCmd.ToString())
  ChkReg = MyDB.GetReader("MyData", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("OarDBCln", "Get regions DB error: " + MyDB.ErrMessage())
  If ChkReg.HasRows() Then
   ' Fail - region is online or was crashed out.
   DispError.InnerText = GetRegion.SelectedItem.Text.ToString() + " region is currently online. Please close the region first then return here."
   BodyTag.Attributes.Add("onload", "ShowDelWin();")       ' Activate display to show error message
  Else
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
   Session("RegUUID") = GetRegion.SelectedItem.Value.ToString()
   Session("RegName") = GetRegion.SelectedItem.Text.ToString()
   StepNum.Value = 2                                       ' Display for next Step
   Display()
  End If

 End Sub

 ' Step 2 Button - Old Accounts Entered, Setup for Matched Owner Display
 Private Sub Button2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button2.Click
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** Step 2 Button Clicked")
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then Response.Redirect("Admin.aspx") ' Session loss check

  Dim tMsg As String
  tMsg = ValAddEdit()

  If tMsg.ToString().Trim().Length = 0 Then
   ' Check data entered for validity
   Dim TxtRows() As String
   TxtRows = OldAccounts.Value.ToString().Trim().Split(vbCrLf)
   If TxtRows.Length() > 0 Then
    Dim TxtCols() As String
    If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Text Rows length: " + TxtRows.Length.ToString())
    For Each Line In TxtRows
     If Trace.IsEnabled Then Trace.Warn("OarDBCln", "*Text Line: " + Line.ToString())
     If Line.ToString().Length > 0 Then                    ' Ignore rows with no contents = blank line or end of list
      TxtCols = Line.ToString().Replace(vbLf, "").Split(" ")
      If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Text loaded: 0= " + TxtCols(0).ToString() + ", 1= " + TxtCols(1).ToString() + ", 2=" + TxtCols(2).ToString())
      If TxtCols.Length() <> 3 Then                        ' Each entry must have UUID,UserName,LastName
       ' Invalid row data
       tMsg = "Data has invalid format for line '" + Line.Count().ToString() + ". " + Line.ToString() + "'."
       Exit For
      End If
     End If
    Next
    If tMsg.ToString().Trim().Length = 0 Then
     If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
      BodyTag.Attributes.Remove("onload")
     End If
     ' Convert text entry into datatable for processing.
     Dim Row, NewRow, CheckRow As DataRow
     Dim OldAcctTbl As New DataTable
     OldAcctTbl = Session("OldAcctTbl")
     For Each Line In TxtRows
      If Line.ToString().Length > 0 Then                   ' Ignore rows with no contents = blank line or end of list
       TxtCols = Line.ToString().Replace(vbLf, "").Split(" ")
       Row = OldAcctTbl.NewRow()
       Row.Item("UUID") = TxtCols(0).ToString()
       Row.Item("username") = TxtCols(1).ToString()
       Row.Item("lastname") = TxtCols(2).ToString()
       OldAcctTbl.Rows.Add(Row)
       OldAcctTbl.AcceptChanges()
      End If
     Next
     If Trace.IsEnabled Then Trace.Warn("OarDBCln", "OldAcctTbl Rows: " + OldAcctTbl.Rows.Count().ToString())

     ' Create Old Owners and Owners match lists from the old accounts
     Dim OldOwnersTbl As New DataTable                     ' UUID, username, lastname
     OldOwnersTbl = Session("OldOwnersTbl")
     Dim OwnersMatTbl As New DataTable                     ' OldUUID, OldName, NewUUID, NewName
     OwnersMatTbl = Session("OwnersMatTbl")
     Dim GridUsers As MySqlDataReader

     ' Get list of Old OwnerIDs from region contents
     Dim RegOldOwners As MySqlDataReader
     SQLCmd = "Select OwnerID " +
              "From " +
              "((Select Distinct OwnerID From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + ") " +
              " Union " +
              " (Select Distinct OwnerID From primitems " +
              "  Where primID In (Select UUID From prims Where RegionUUID= " + MyDB.SQLStr(Session("RegUUID")) + "))) as RawData " +
              "Where OwnerID not in (Select UUID From users) " +
              "Order by OwnerID"
     If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Get Old OwnerID SQLCmd: " + SQLCmd.ToString())
     RegOldOwners = MyDB.GetReader("MyData", SQLCmd)
     If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("OarDBCln", "DB Error: " + MyDB.ErrMessage().ToString())
     If RegOldOwners.HasRows() Then
      While RegOldOwners.Read()
       If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Processing Old OwnerID: " + RegOldOwners("OwnerID").ToString())
       ' Find match UUID in OldAccounts table to get the name elements
       Row = OldAcctTbl.Rows.Find(RegOldOwners("OwnerID"))
       If Row IsNot Nothing Then
        If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Found Row UUID: " + Row("UUID").ToString() + ", " + Row("username").ToString() + " " + Row("lastname").ToString())
        ' See if the grid has a matching name in the users
        SQLCmd = "Select UUID, username, lastname " +
                 "From users " +
                 "Where username=" + MyDB.SQLStr(Row("username")) + " and lastname=" + MyDB.SQLStr(Row("lastname"))
        If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Get users values SQLCmd: " + SQLCmd.ToString())
        GridUsers = MyDB.GetReader("MyData", SQLCmd)
        If GridUsers.HasRows() Then
         GridUsers.Read()
         If Trace.IsEnabled Then Trace.Warn("OarDBCln", "User Found: " + GridUsers("UUID").ToString() + ", " + GridUsers("username").ToString() + " " + GridUsers("lastname").ToString())
         If GridUsers("UUID").ToString() <> Row("UUID").ToString() Then
          If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Adding Match Old: " + Row("UUID").ToString() + ", " + Row("username").ToString() + " " + Row("lastname").ToString())
          If Trace.IsEnabled Then Trace.Warn("OarDBCln", "To New: " + GridUsers("UUID").ToString() + ", " + GridUsers("username").ToString() + " " + GridUsers("lastname").ToString())
          ' Add both entries to the match list for each OwnerID found in region
          NewRow = OwnersMatTbl.NewRow()
          NewRow.Item("OldUUID") = Row("UUID").ToString()
          NewRow.Item("OldName") = Row("username").ToString() + " " + Row("lastname").ToString()
          NewRow.Item("NewUUID") = GridUsers("UUID").ToString()
          NewRow.Item("NewName") = GridUsers("username").ToString() + " " + GridUsers("lastname").ToString()
          OwnersMatTbl.Rows.Add(NewRow)
          OwnersMatTbl.AcceptChanges()
         End If
        Else
         CheckRow = OldOwnersTbl.Rows.Find(Row("UUID"))
         If CheckRow Is Nothing Then                       ' Add Old Owner to OldOwnersTbl
          If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Adding to Old Owners List: " + Row("UUID").ToString() + ", " + Row("username").ToString() + " " + Row("lastname").ToString())
          NewRow = OldOwnersTbl.NewRow()
          NewRow.Item("UUID") = Row("UUID").ToString()
          NewRow.Item("username") = Row("username").ToString()
          NewRow.Item("lastname") = Row("lastname").ToString()
          OldOwnersTbl.Rows.Add(NewRow)
          OldOwnersTbl.AcceptChanges()
         End If
        End If
        GridUsers.Close()
       End If
      End While
     Else
      If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** No Old Owners or Matches Found!")
     End If
     RegOldOwners.Close()
     Session("OldOwnersTbl") = OldOwnersTbl                ' Save table for use later
     Session("OwnersMatTbl") = OwnersMatTbl                ' Save table for use later
     Session("OldAcctTbl") = OldAcctTbl                    ' Save table for use later
     'If Trace.IsEnabled Then Trace.Warn("OarDBCln", "OldAcctTbl Rows: " + OldAcctTbl.Rows.Count().ToString())
     'If Trace.IsEnabled Then Trace.Warn("OarDBCln", "OwnersMatTbl Rows: " + OwnersMatTbl.Rows.Count().ToString())
     OldAcctTbl.Dispose()
     OldOwnersTbl.Dispose()
     OwnersMatTbl.Dispose()
     StepNum.Value = 3                                     ' Display for next Step
     Display()
    End If
   Else
    tMsg = "No old account records were entered!"
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   DispError.InnerText = tMsg.ToString()
   BodyTag.Attributes.Add("onload", "ShowDelWin();")       ' Activate display to show error message
  End If

 End Sub

 ' Step 3 Button - Setup for Matched Creator Accounts
 Private Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** Step 3 Button Clicked")
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then Response.Redirect("Admin.aspx") ' Session loss check

  Dim tMsg As String
  tMsg = ""

  If tMsg.ToString().Trim().Length = 0 Then
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
   ' Save Creator matches into a processing table
   Dim OldAcctTbl As New DataTable                         ' UUID, username, lastname
   OldAcctTbl = Session("OldAcctTbl")                      ' Restore table for use here NOTE: somehow all records are lost here.

   ' Create Old Creators and Creators match lists from the old accounts
   Dim OldCreatorsTbl As New DataTable                     ' UUID, username, lastname
   OldCreatorsTbl = Session("OldCreatorsTbl")
   Dim CreatorsMatTbl As New DataTable                     ' OldUUID, OldName, NewUUID, NewName
   CreatorsMatTbl = Session("CreatorsMatTbl")              ' Restore table for use here
   Dim Row, NewRow, CheckRow As DataRow
   Dim GridUsers As MySqlDataReader

   ' Get list of Old CreatorIDs from region contents
   Dim RegOldCreators As MySqlDataReader
   SQLCmd = "Select CreatorID " +
            "From " +
            "((Select Distinct CreatorID From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + ") " +
            " Union " +
            " (Select Distinct CreatorID From primitems" +
            "  Where primID in (Select UUID From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + "))) as RawData " +
            "Where CreatorID not in (Select UUID From users) " +
            "Order by CreatorID"
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Get Old CreatorID SQLCmd: " + SQLCmd.ToString())
   RegOldCreators = MyDB.GetReader("MyData", SQLCmd)
   If RegOldCreators.HasRows() Then
    While RegOldCreators.Read()
     If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Processing Old CreatorID: " + RegOldCreators("CreatorID").ToString())
     ' Find match UUID in OldAccounts table to get the name elements
     Row = OldAcctTbl.Rows.Find(RegOldCreators("CreatorID"))
     If Row IsNot Nothing Then
      If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Found Row UUID: " + Row("UUID").ToString() + ", RegOldCreatorID= " + RegOldCreators("CreatorID").ToString())
      ' See if the grid has a matching name in the users
      SQLCmd = "Select UUID, username, lastname " +
               "From users " +
               "Where username=" + MyDB.SQLStr(Row("username")) + " and lastname=" + MyDB.SQLStr(Row("lastname"))
      If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Get users SQLCmd: " + SQLCmd.ToString())
      GridUsers = MyDB.GetReader("MyData", SQLCmd)
      If GridUsers.HasRows() Then
       GridUsers.Read()
       If GridUsers("UUID").ToString() <> Row("UUID").ToString() Then
        If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Adding Match Old: " + Row("UUID").ToString() + ", " + Row("username").ToString() + " " + Row("lastname").ToString())
        If Trace.IsEnabled Then Trace.Warn("OarDBCln", "To New: " + GridUsers("UUID").ToString() + ", " + GridUsers("username").ToString() + " " + GridUsers("lastname").ToString())
        ' Add both entries to the match list for each CreatorID found in region
        NewRow = CreatorsMatTbl.NewRow()
        NewRow.Item("OldUUID") = Row("UUID").ToString()
        NewRow.Item("OldName") = Row("username").ToString() + " " + Row("lastname").ToString()
        NewRow.Item("NewUUID") = GridUsers("UUID").ToString()
        NewRow.Item("NewName") = GridUsers("username").ToString() + " " + GridUsers("lastname").ToString()
        CreatorsMatTbl.Rows.Add(NewRow)
        CreatorsMatTbl.AcceptChanges()
       End If
      Else
       CheckRow = OldCreatorsTbl.Rows.Find(Row("UUID"))
       If CheckRow Is Nothing Then                         ' Add Old Creator to OldCreatorsTbl
        If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Adding to Old Creators List: " + Row("UUID").ToString() + ", " + Row("username").ToString() + " " + Row("lastname").ToString())
        NewRow = OldCreatorsTbl.NewRow()
        NewRow.Item("UUID") = Row("UUID").ToString()
        NewRow.Item("username") = Row("username").ToString()
        NewRow.Item("lastname") = Row("lastname").ToString()
        OldCreatorsTbl.Rows.Add(NewRow)
        OldCreatorsTbl.AcceptChanges()
       End If
      End If
      GridUsers.Close()
     End If
    End While
   End If
   Session("OldCreatorsTbl") = OldCreatorsTbl              ' Save table for use later
   Session("CreatorsMatTbl") = CreatorsMatTbl              ' Save table for use later
   RegOldCreators.Close()
   OldAcctTbl.Dispose()
   OldCreatorsTbl.Dispose()
   CreatorsMatTbl.Dispose()

   StepNum.Value = 4                                       ' Display for next Step
   Display()
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   DispError.InnerText = tMsg.ToString()
   BodyTag.Attributes.Add("onload", "ShowDelWin();")       ' Activate display to show error message
  End If

 End Sub

 ' Step 4 Button - Process Summary Display to Show
 Private Sub Button4_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button4.Click
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** Step 4 Button Clicked")
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then Response.Redirect("Admin.aspx") ' Session loss check

  If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
   BodyTag.Attributes.Remove("onload")
  End If

  StepNum.Value = 5                                        ' Display for next Step
  Display()

 End Sub

 ' Step 5 Button - Process Database Updates
 Private Sub Button5_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button5.Click
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** Step 5 Button Clicked")
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then Response.Redirect("Admin.aspx") ' Session loss check

  Dim tMsg, SQLFields, SQLValues As String
  tMsg = ""
  ' OldAcctTbl = List of all old accounts from OAR file
  ' OldOwnersTbl = List of Old Owner accounts found in region
  ' OwnersMatTbl = List of matched old and new Owner accounts
  ' OldCreatorsTbl = List of Old Creator accounts found in region
  ' CreatorMatTbl = List of matched old and new Creator accounts

  If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
   BodyTag.Attributes.Remove("onload")
  End If
  ' Setup working table data to be used
  Dim OwnersMatTbl As New DataTable
  OwnersMatTbl = Session("OwnersMatTbl")                  ' Restore table for use here
  Dim CreatorsMatTbl As New DataTable
  CreatorsMatTbl = Session("CreatorsMatTbl")              ' Restore table for use here
  Dim OldAcctTbl As New DataTable                         ' UUID, username, lastname
  OldAcctTbl = Session("OldAcctTbl")                      ' Restore table for use here
  Dim RefAccounts As New DataTable                        ' UUID, username, lastname
  RefAccounts = Session("RefAccounts")
  Dim Row, NewRow, CheckRow As DataRow

  ' Apply New Owner Account for each old owner account.
  If OwnersMatTbl.Rows.Count() > 0 Then
   For Each Row In OwnersMatTbl.Rows
    ' Set Old Owner to New Owner in prims table
    If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Update old Owner: " + Row("OldName").ToString() + " to " + Row("NewName").ToString())
    SQLCmd = "Update prims Set OwnerID=" + MyDB.SQLStr(Row("NewUUID")) + " " +
             "Where OwnerID=" + MyDB.SQLStr(Row("OldUUID")) + " and RegionUUID=" + MyDB.SQLStr(Session("RegUUID"))
    If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Update prims SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MyData", SQLCmd)
    If MyDB.Error() Then
     tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
     Exit For
    Else
     ' Set Old Owner to New Owner in primitems table
     If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Update old Owner: " + Row("OldName").ToString() + " to " + Row("NewName").ToString())
     SQLCmd = "Update primitems Set ownerID=" + MyDB.SQLStr(Row("NewUUID")) + " " +
              "Where ownerID=" + MyDB.SQLStr(Row("OldUUID")) + " and primID in " +
              " (Select UUID From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + ")"
     If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Update primitems SQLCmd: " + SQLCmd.ToString())
     MyDB.DBCmd("MyData", SQLCmd)
     If MyDB.Error() Then
      tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
      Exit For
     End If
    End If
   Next
  End If

  If tMsg.ToString().Trim().Length = 0 Then
   ' Apply New Creator account for each old creator account
   If CreatorsMatTbl.Rows.Count() > 0 Then
    For Each Row In CreatorsMatTbl.Rows
     ' Set Old Creator to New Creator in prims table
     If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Update old Creator: " + Row("OldName").ToString() + " to " + Row("NewName").ToString())
     SQLCmd = "Update prims Set " +
              " CreatorID=" + MyDB.SQLStr(Row("NewUUID")) + "," + "LastOwnerID=" + MyDB.SQLStr(Row("NewUUID")) + " " +
              "Where CreatorID=" + MyDB.SQLStr(Row("OldUUID")) + " and RegionUUID=" + MyDB.SQLStr(Session("RegUUID"))
     If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Update prims SQLCmd: " + SQLCmd.ToString())
     MyDB.DBCmd("MyData", SQLCmd)
     If MyDB.Error() Then
      tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
      Exit For
     Else
      ' Change prims to full permissions anything that is not, where Creator and OwnerID are the same.
      SQLCmd = "Update prims Set OwnerMask=647168, BaseMask=647168 " +
               "Where OwnerMask<>647168 and CreatorID=" + MyDB.SQLStr(Row("NewUUID")) + " and " +
               " OwnerID=" + MyDB.SQLStr(Row("NewUUID")) + " and RegionUUID=" + MyDB.SQLStr(Session("RegUUID"))
      If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Update prims SQLCmd: " + SQLCmd.ToString())
      MyDB.DBCmd("MyData", SQLCmd)
      If MyDB.Error() Then
       tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
       Exit For
      Else
       ' Set Old Creator to New Creator in primitems table
       If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Update old Creator: " + Row("OldName").ToString() + " to " + Row("NewName").ToString())
       SQLCmd = "Update primitems Set " +
                " creatorID=" + MyDB.SQLStr(Row("NewUUID")) + "," + "lastOwnerID=" + MyDB.SQLStr(Row("NewUUID")) + " " +
                "Where creatorID=" + MyDB.SQLStr(Row("OldUUID")) + " and primID in " +
                " (Select UUID From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + ")"
       If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Update primitems SQLCmd: " + SQLCmd.ToString())
       MyDB.DBCmd("MyData", SQLCmd)
       If MyDB.Error() Then
        tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
        Exit For
       Else
        ' Change primitems to full permissions anything that is not, where Creator and OwnerID are the same.
        SQLCmd = "Update primitems Set currentPermissions=647168, basePermissions=647168 " +
                 "Where currentPermissions<>647168 and " + " creatorID=" + MyDB.SQLStr(Row("NewUUID")) + " and " +
                 " ownerID=" + MyDB.SQLStr(Row("NewUUID")) + " and primID in " +
                 " (Select UUID From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + ")"
        If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Update primitems SQLCmd: " + SQLCmd.ToString())
        MyDB.DBCmd("MyData", SQLCmd)
        If MyDB.Error() Then
         tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
         Exit For
        End If
       End If
      End If
     End If
    Next
   End If
   If tMsg.ToString().Trim().Length = 0 Then
    ' Get all old Creators still in Region to be added as Reference acccounts
    Dim GetOldCreators As MySqlDataReader
    SQLCmd = "Select CreatorID " +
             "From " +
             " ((Select Distinct CreatorID From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + ") " +
             "  Union " +
             "  (Select Distinct CreatorID From primitems" +
             "   Where primID in (Select UUID From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + "))) as RawData " +
             "Where CreatorID not in (Select UUID From users) " +
             "Order by CreatorID"
    ' While Read DB Records
    If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Get old CreatorIDs SQLCmd: " + SQLCmd.ToString())
    GetOldCreators = MyDB.GetReader("MyData", SQLCmd)
    If GetOldCreators.HasRows() Then
     While GetOldCreators.Read()
      ' Find match UUID in OldAccounts table to get the name elements
      Row = OldAcctTbl.Rows.Find(GetOldCreators("CreatorID"))
      If Row IsNot Nothing Then
       If Row("UUID").ToString() = GetOldCreators("CreatorID").ToString() Then
        CheckRow = RefAccounts.Rows.Find(Row("UUID"))
        If CheckRow Is Nothing Then                       ' Add Old Creator to RefAccounts
         If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Adding to RefAccounts: " + Row("UUID").ToString() + ", " + Row("username").ToString() + " " + Row("lastname").ToString())
         NewRow = RefAccounts.NewRow()
         NewRow("UUID") = Row("UUID")
         NewRow.Item("username") = Row("username").ToString()
         NewRow.Item("lastname") = Row("lastname").ToString()
         RefAccounts.Rows.Add(NewRow)                     ' Add to Reference Accounts
         RefAccounts.AcceptChanges()
        End If
       End If
      End If
     End While
    End If
    GetOldCreators.Close()
   End If
  End If
  ' If RefAccounts has rows = There were no errors at this point...
  If RefAccounts.Rows().Count() > 0 Then
   ' Insert Reference Accounts into users table
   SQLFields = "`UUID`, `username`, `lastname`, `passwordHash`, `passwordSalt`, `homeRegion`, `homeLocationX`, " +
               "`homeLocationY`, `homeLocationZ`, `homeLookAtX`, `homeLookAtY`, `homeLookAtZ`, `created`, " +
               "`lastLogin`, `userInventoryURI`, `userAssetURI`, `profileAboutText`, `profileFirstText`, " +
               "`profileImage`, `profileFirstImage`, `webLoginKey`, `homeRegionID`, `userFlags`, `godLevel`, " +
               "`iz_level`, `customType`, `partner`, `email`, `profileURL`, `skillsMask`, `skillsText`, " +
               "`wantToMask`, `wantToText`, `languagesText`"
   SQLValues = ""
   For Each Row In RefAccounts.Rows()
    ' Get data to add to users and agents tables
    If SQLValues.ToString().Trim().Length > 0 Then
     SQLValues = SQLValues.ToString() + "," + vbCrLf
    End If
    SQLValues = SQLValues.ToString() +
                "(" + MyDB.SQLStr(Row("UUID")) + "," + MyDB.SQLStr(Row("username")) + "," + MyDB.SQLStr(Row("lastname")) + "," +
                "'NoPass','',0,0,0,0,0,0,0,UNIX_TIMESTAMP(),0,'','','',''," +
                "'00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000'," +
                "'00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000',0,0,0,''," +
                "'00000000-0000-0000-0000-000000000000','','',0,'None',0,'None','English')"
   Next
   SQLCmd = "Insert into users (" + SQLFields.ToString() + ") " + vbCrLf +
            "Values " + vbCrLf +
            SQLValues.ToString() + ";"
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Insert users SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MyData", SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   Else
    ' Insert Reference Accounts into agents table
    SQLFields = "`UUID`,`sessionID`,`secureSessionID`,`agentIP`,`agentPort`,`agentOnline`,`loginTime`,`logoutTime`," +
                "`currentRegion`,`currentHandle`,`currentPos`,`currentLookAt`"
    SQLValues = ""
    For Each Row In RefAccounts.Rows()
     If SQLValues.ToString().Trim().Length > 0 Then
      SQLValues = SQLValues.ToString() + "," + vbCrLf
     End If
     SQLValues = SQLValues.ToString() +
                 "(" + MyDB.SQLStr(Row("UUID")) + ",UUID(),UUID(),'127.0.0.1',0,0,0,0," +
                 "'00000000-0000-0000-0000-000000000000',0,'<0,0,0>','<0,0,0>')"
    Next
    SQLCmd = "Insert into agents (" + SQLFields.ToString() + ") " + vbCrLf +
             "Values " + vbCrLf +
             SQLValues.ToString() + ";"
    If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Insert agents SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MyData", SQLCmd)
    If MyDB.Error() Then
     tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
    Else
     ' Insert Reference Accounts into userpreferences table
     SQLFields = "`user_ID`,`recv_ims_via_email`,`listed_in_directory`"
     SQLValues = ""
     For Each Row In RefAccounts.Rows()
      If SQLValues.ToString().Trim().Length > 0 Then
       SQLValues = SQLValues.ToString() + "," + vbCrLf
      End If
      SQLValues = SQLValues.ToString() +
                  "(" + MyDB.SQLStr(Row("UUID")) + ",1,0)"
     Next
     SQLCmd = "Insert into userpreferences (" + SQLFields.ToString() + ") " + vbCrLf +
              "Values " + vbCrLf +
              SQLValues.ToString() + ";"
     If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Insert userpreferences SQLCmd: " + SQLCmd.ToString())
     MyDB.DBCmd("MyData", SQLCmd)
     If MyDB.Error() Then
      tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
     Else
      ' Insert Reference Accounts into inventoryfolders table
      SQLFields = "`folderName`,`type`,`version`,`folderID`,`agentID`,`parentFolderID`"
      SQLValues = ""
      For Each Row In RefAccounts.Rows()
       If SQLValues.ToString().Trim().Length > 0 Then
        SQLValues = SQLValues.ToString() + "," + vbCrLf
       End If
       SQLValues = SQLValues.ToString() +
                   "('My Inventory',8,13,UUID()," + MyDB.SQLStr(Row("UUID")) + ",'00000000-0000-0000-0000-000000000000')"
      Next
      SQLCmd = "Insert into inventoryfolders (" + SQLFields.ToString() + ") " + vbCrLf +
               "Values " + vbCrLf +
               SQLValues.ToString() + ";"
      If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
      MyDB.DBCmd("MyData", SQLCmd)
      If MyDB.Error() Then
       tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
      End If
     End If
    End If
   End If
  End If

  OldAcctTbl.Dispose()
  Session("RefAccounts") = RefAccounts
  RefAccounts.Dispose()
  If tMsg.ToString().Trim().Length > 0 Then
   DispError.InnerText = tMsg.ToString()
   BodyTag.Attributes.Add("onload", "ShowDelWin();")       ' Activate display to show error message
  Else
   StepNum.Value = 6                                       ' Display for next Step
   Display()
  End If

 End Sub

 ' Get Old accounts from DB
 Private Sub GetAccounts_CheckedChanged(sender As Object, e As EventArgs) Handles GetAccounts.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** GetAccounts_Checked() Called")
  Dim AcctList As String
  Dim Counter As Integer
  AcctList = ""
  Counter = 0

  Dim GetIncrement As MySqlDataReader
  SQLCmd = "Select lastname From users Where lastname like 'Creator%' Order by lastname Desc Limit 1"
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Get old CreatorIDs SQLCmd: " + SQLCmd.ToString())
  GetIncrement = MyDB.GetReader("MyData", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("OarDBCln", "Get regions DB error: " + MyDB.ErrMessage())
  If GetIncrement.HasRows() Then
   GetIncrement.Read()
   Counter = CInt(GetIncrement("lastname").ToString().Replace("Creator", "")) + 1
  End If
  GetIncrement.Close()

  ' Extract old account UUID from region tables
  Dim GetOldAccounts As MySqlDataReader
  SQLCmd = "Select UUID " +
           "From " +
           " ((Select Distinct OwnerID as UUID " +
           "   From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + ") " +
           "  Union " +
           "  (Select Distinct OwnerID as UUID From primitems" +
           "   Where primID in (Select UUID From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + ")) " +
           "  Union " +
           "  (Select Distinct CreatorID as UUID " +
           "   From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + ") " +
           "  Union " +
           "  (Select Distinct CreatorID as UUID From primitems" +
           "   Where primID in (Select UUID From prims Where RegionUUID=" + MyDB.SQLStr(Session("RegUUID")) + "))) as RawData " +
           "Where UUID not in (Select UUID From users) " +
           "Order by UUID"
  ' While Read DB Records
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "Get old CreatorIDs SQLCmd: " + SQLCmd.ToString())
  GetOldAccounts = MyDB.GetReader("MyData", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("OarDBCln", "Get regions DB error: " + MyDB.ErrMessage())
  If GetOldAccounts.HasRows() Then
   While GetOldAccounts.Read()
    If AcctList.ToString().Trim().Length > 0 Then
     AcctList = AcctList.ToString() + vbCrLf
    End If
    AcctList = AcctList.ToString() + GetOldAccounts("UUID").ToString() + " Unknown Creator" + IIf(Counter > 0, Counter.ToString().Trim(), "")
    Counter = Counter + 1
   End While
  End If
  GetOldAccounts.Close()
  OldAccounts.InnerText = AcctList.ToString()
  GetAccounts.Checked = False
  Display()

 End Sub

 ' Cancel Button
 Private Sub Button6_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button6.Click
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** Cancel Button Clicked")
  StepNum.Value = 9                                        ' Abort all operation!
  Cancel()
 End Sub

 ' Error display cancel activated
 Private Sub SetCan_CheckedChanged(sender As Object, e As EventArgs) Handles SetCan.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** Error Message Cancel Clicked")
  SetCan.Checked = False
  Cancel()
 End Sub

 Private Sub Cancel()
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** Cancel() Called")
  If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
   BodyTag.Attributes.Remove("onload")
  End If

  Dim ATable As New DataTable
  ATable.Columns.Add("UUID")
  ATable.Columns.Add("username")
  ATable.Columns.Add("lastname")
  ATable.AcceptChanges()
  Dim Key(1) As DataColumn
  Key(0) = ATable.Columns("UUID")
  ATable.PrimaryKey = Key
  ATable.AcceptChanges()
  Dim BTable As New DataTable
  BTable.Columns.Add("OldUUID")
  BTable.Columns.Add("OldName")
  BTable.Columns.Add("NewUUID")
  BTable.Columns.Add("NewName")
  BTable.AcceptChanges()

  ' Clear values set based on StepNum.value
  If CInt(StepNum.Value) = 2 Then                          ' Old Accounts Entry.
   Session("RegUUID") = ""
   Session("RegName") = ""
   StepNum.Value = 1                                       ' Return to Step 1 - Region Selection

  ElseIf CInt(StepNum.Value) = 3 Then                      ' Match Owner Accounts.
   Session("OldAcctTbl") = ATable.Clone()
   StepNum.Value = 2                                       ' Return to Step 2 - Old Accounts Entry

  ElseIf CInt(StepNum.Value) = 4 Then                      ' Match Creator Accounts.
   Session("OldOwnersTbl") = ATable.Clone()
   Session("OwnersMatTbl") = BTable.Clone()
   StepNum.Value = 3                                       ' Return to Step 3 - Match Owner Accounts.

  ElseIf CInt(StepNum.Value) = 5 Then                      ' Process Summary.
   Session("OldCreatorsTbl") = ATable.Clone()
   Session("CreatorsMatTbl") = BTable.Clone()
   StepNum.Value = 4                                       ' Return to Step 4 - Match Creator Accounts.

  ElseIf CInt(StepNum.Value) = 9 Then                      ' Abort all operations, start over!
   Session("RegUUID") = ""
   Session("RegName") = ""
   Session("OldAcctTbl") = ATable.Clone()
   Session("OldOwnersTbl") = ATable.Clone()
   Session("OwnersMatTbl") = BTable.Clone()
   Session("OldCreatorsTbl") = ATable.Clone()
   Session("CreatorsMatTbl") = BTable.Clone()
   Session("RefAccounts") = ATable.Clone()
   StepNum.Value = 1
  End If
  ATable.Dispose()
  BTable.Dispose()
  Display()
 End Sub

 ' Add Owner Match Entry
 Private Sub AddOwnMat_CheckedChanged(sender As Object, e As EventArgs) Handles AddOwnMat.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** Add Owner Match Called ")
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then Response.Redirect("Admin.aspx") ' Session loss check

  Dim tMsg As String
  tMsg = ""
  ' Validate selections
  If OldOwners.SelectedValue.ToString() <> "" And GridAccts1.SelectedValue.ToString() <> "" Then
   Dim Row As DataRow
   Dim OldOwnersTbl As New DataTable                       ' UUID, username, lastname
   OldOwnersTbl = Session("OldOwnersTbl")                  ' Restore table for use here
   Dim OwnersMatTbl As New DataTable
   OwnersMatTbl = Session("OwnersMatTbl")                  ' Restore table for use here
   ' Add both entries to the match list assigned by User
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "* Adding Owner Matches: " +
                                      OldOwners.SelectedItem.ToString() + " To " +
                                      GridAccts1.SelectedItem.ToString())
   Row = OwnersMatTbl.NewRow()
   Row.Item("OldUUID") = OldOwners.SelectedValue.ToString()
   Row.Item("OldName") = OldOwners.SelectedItem.ToString()
   Row.Item("NewUUID") = GridAccts1.SelectedValue.ToString()
   Row.Item("NewName") = GridAccts1.SelectedItem.ToString()
   OwnersMatTbl.Rows.Add(Row)
   OwnersMatTbl.AcceptChanges()
   Session("OwnersMatTbl") = OwnersMatTbl                  ' Restore table for use here
   ' Remove entry from OldOwnersTbl
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "* Remove From Old Owners List: " + OldOwners.Items.Item(OldOwners.SelectedIndex).Text.ToString())
   Row = OldOwnersTbl.Rows.Find(OldOwners.SelectedValue)
   OldOwnersTbl.Rows.Remove(Row)
   OldOwnersTbl.AcceptChanges()
   Session("OldOwnersTbl") = OldOwnersTbl                  ' Save table for use in Display()
   OldOwnersTbl.Dispose()
   OwnersMatTbl.Dispose()
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
   AddOwnMat.Checked = False                               ' Allow option to be called again
   Display()
  Else
   If OldOwners.SelectedValue.ToString() <> "" Then
    tMsg = tMsg.ToString() + "Missing Old Account selection!"
   End If
   If GridAccts1.SelectedValue.ToString() <> "" Then
    If tMsg.ToString().Trim().Length > 0 Then tMsg = tMsg.ToString() + "\n\r"
    tMsg = tMsg.ToString() + "Missing Grid Account selection!"
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   DispError.InnerText = tMsg.ToString()
   BodyTag.Attributes.Add("onload", "ShowDelWin();")       ' Activate display to show error message
  End If

 End Sub

 ' Remove Owner Match Entry
 Private Sub RemOwnEnt_TextChanged(sender As Object, e As EventArgs) Handles RemOwnEnt.TextChanged
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** Remove Owner Match Called ")
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then Response.Redirect("Admin.aspx") ' Session loss check

  Dim Row, NewRow, FoundRow As DataRow
  Dim Name() As String
  Dim OwnersMatTbl As New DataTable
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "*Remove Owner Match:" + RemOwnEnt.Text.ToString())
  OwnersMatTbl = Session("OwnersMatTbl")
  Row = OwnersMatTbl.Rows.Item(CInt(RemOwnEnt.Text))       ' Sent the row index number
  Name = Row("OldName").ToString().Split(" ")

  Dim OldOwnersTbl As New DataTable                        ' UUID, username, lastname
  OldOwnersTbl = Session("OldOwnersTbl")                   ' Restore table for use here
  FoundRow = OldOwnersTbl.Rows.Find(Row("OldUUID"))
  If FoundRow Is Nothing Then                              ' Entry does not exist,
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "*Add to Owner Match List:" + RemOwnEnt.Text.ToString())
   ' Add Old Owner to OldOwnersTbl
   NewRow = OldOwnersTbl.NewRow()
   NewRow.Item("UUID") = Row("OldUUID").ToString()
   NewRow.Item("username") = Name(0).ToString()
   NewRow.Item("lastname") = Name(1).ToString()
   OldOwnersTbl.Rows.Add(NewRow)
   OldOwnersTbl.AcceptChanges()
   Session("OldOwnersTbl") = OldOwnersTbl                  ' Save table for use in Display()
  End If
  OldOwnersTbl.Dispose()

  OwnersMatTbl.Rows.Remove(Row)
  OwnersMatTbl.AcceptChanges()
  Session("OwnersMatTbl") = OwnersMatTbl

  RemOwnEnt.Text = ""                                      ' Allow process to be called again
  Display()

 End Sub

 ' Add Creator Match Entry
 Private Sub AddCrtMat_CheckedChanged(sender As Object, e As EventArgs) Handles AddCrtMat.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** Add Creator Match Called ")
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then Response.Redirect("Admin.aspx") ' Session loss check

  Dim tMsg As String
  tMsg = ""

  ' Validate selections
  If OldCreators.SelectedValue.ToString() <> "" And GridAccts2.SelectedValue.ToString() <> "" Then
   Dim Row As DataRow
   Dim OldCreatorsTbl As New DataTable                     ' UUID, username, lastname
   OldCreatorsTbl = Session("OldCreatorsTbl")              ' Restore table for use here
   Dim CreatorsMatTbl As New DataTable
   CreatorsMatTbl = Session("CreatorsMatTbl")              ' Restore table for use here
   ' Add both entries to the match list for each OwnerID found in prims
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "* Adding Creator Matches: " +
                                      OldCreators.SelectedItem.ToString() + " To " +
                                      GridAccts2.SelectedItem.ToString())
   Row = CreatorsMatTbl.NewRow()
   Row.Item("OldUUID") = OldCreators.SelectedValue.ToString()
   Row.Item("OldName") = OldCreators.SelectedItem.ToString()
   Row.Item("NewUUID") = GridAccts2.SelectedValue.ToString()
   Row.Item("NewName") = GridAccts2.SelectedItem.ToString()
   CreatorsMatTbl.Rows.Add(Row)
   CreatorsMatTbl.AcceptChanges()
   Session("CreatorsMatTbl") = CreatorsMatTbl              ' Restore table for use here
   ' Remove entry from OldCreatorsTbl
   If Trace.IsEnabled Then Trace.Warn("OarDBCln", "* Remove From Old Creators List: " + OldCreators.SelectedItem.ToString())
   Row = OldCreatorsTbl.Rows.Find(OldCreators.SelectedValue)
   OldCreatorsTbl.Rows.Remove(Row)
   OldCreatorsTbl.AcceptChanges()
   Session("OldCreatorsTbl") = OldCreatorsTbl              ' Save table for use in Display()
   OldCreatorsTbl.Dispose()
   CreatorsMatTbl.Dispose()
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
   AddCrtMat.Checked = False                               ' Allow option to be called again
   Display()
  Else
   If OldCreators.SelectedValue.ToString() <> "" Then
    tMsg = tMsg.ToString() + "Missing Old Account selection!"
   End If
   If GridAccts2.SelectedValue.ToString() <> "" Then
    If tMsg.ToString().Trim().Length > 0 Then tMsg = tMsg.ToString() + "\n\r"
    tMsg = tMsg.ToString() + "Missing Grid Account selection!"
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   DispError.InnerText = tMsg.ToString()
   BodyTag.Attributes.Add("onload", "ShowDelWin();")       ' Activate display to show error message
  End If

 End Sub

 ' Remove Creator Match Entry
 Private Sub RemCrtEnt_TextChanged(sender As Object, e As EventArgs) Handles RemCrtEnt.TextChanged
  If Trace.IsEnabled Then Trace.Warn("OarDBCln", "** Remove Creator Match Called ")
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then Response.Redirect("Admin.aspx") ' Session loss check

  Dim Row, NewRow, FoundRow As DataRow
  Dim Name() As String
  Dim CreatorsMatTbl As New DataTable
  CreatorsMatTbl = Session("CreatorsMatTbl")
  Row = CreatorsMatTbl.Rows.Item(CInt(RemOwnEnt.Text))
  Name = Row("OldName").ToString().Split(" ")
  ' Add Old Creator to OldCreatorsTbl
  Dim OldCreatorsTbl As New DataTable                      ' UUID, username, lastname
  OldCreatorsTbl = Session("OldCreatorsTbl")               ' Restore table for use here
  FoundRow = OldCreatorsTbl.Rows.Find(Row("OldUUID"))
  If FoundRow Is Nothing Then                              ' Entry does not exist,
   NewRow = OldCreatorsTbl.NewRow()
   NewRow.Item("UUID") = Row("OldUUID").ToString()
   NewRow.Item("username") = Name(0).ToString()
   NewRow.Item("lastname") = Name(1).ToString()
   OldCreatorsTbl.Rows.Add(NewRow)
   OldCreatorsTbl.AcceptChanges()
   Session("OldCreatorsTbl") = OldCreatorsTbl              ' Save table for use in Display()
  End If
  OldCreatorsTbl.Dispose()
  CreatorsMatTbl.Rows.Remove(CreatorsMatTbl.Rows.Item(CInt(RemOwnEnt.Text)))
  Session("CreatorsMatTbl") = CreatorsMatTbl
  RemCrtEnt.Text = ""                                      ' Allow process to be called again
  Display()

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub
End Class
