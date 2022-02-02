
Partial Class Administration_Economy
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
 '* Economy Management Page. Sets the Economy levels and input options needed for any world operation.
 '* Provides access to the Accounting Reports and DB fund management tools.
 '* 

 '* Built from MyWorld Form template v. 1.0

 ' Define common Page class properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private GridLib As New GridLib
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
  If Session("Access") <> 9 Then                           ' SysAdmin Only access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("Economy", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page accessed setup
   ' Define process unique objects here

   ' Setup general page controls

   Display()
  End If

 End Sub

 Private Sub Display()
  If Trace.IsEnabled Then Trace.Warn("Economy", "Display() Called")

  Dim HasEconomy As Boolean
  Dim ELevel As Integer
  ELevel = 1                                             ' Default Economy Level = none
  ' Check if Economy control records exists
  Dim Economy As MySqlDataReader
  SQLCmd = "Select IF(Nbr1=0,FALSE,TRUE) as Nbr1,Nbr2 " +
           "From control " +
           "Where Control='ECONOMY' and Parm1='HasEconomy'"
  If Trace.IsEnabled Then Trace.Warn("Economy", "Check for Economy SQLCmd: " + SQLCmd.ToString())
  Economy = MyDB.GetReader("MySite", SQLCmd)
  If Economy.HasRows() Then
   Economy.Read()
   HasEconomy = Economy("Nbr1")
   ELevel = Economy("Nbr2")
  End If
  Economy.Close()

  ' Display data fields based on edit or add mode
  If HasEconomy Then                                       ' Edit Mode, show database values
   Dim drApp As MySqlDataReader
   ' Get all the Economy Control values
   SQLCmd = "Select Name,Parm2,Nbr2 From control Where Control='ECONOMY' and Nbr1>9 Order by Nbr1"
   If Trace.IsEnabled Then Trace.Warn("Economy", "Get display Values SQLCmd: " + SQLCmd.ToString())
   drApp = MyDB.GetReader("MySite", SQLCmd)
   If drApp.Read() Then
    EmailName.InnerText = drApp("Name").ToString().Trim()
    PayPalEmail.Text = drApp("Parm2").ToString().Trim()
    If Trace.IsEnabled Then Trace.Warn("Economy", "Record 1 Name: " + drApp("Name").ToString() + ", " + drApp("Parm2").ToString())
    drApp.Read()
    PPMerchName.InnerText = drApp("Name").ToString().Trim()
    PPMerchantID.Text = drApp("Parm2").ToString().Trim()
    If Trace.IsEnabled Then Trace.Warn("Economy", "Record 2 Name: " + drApp("Name").ToString() + ", " + drApp("Parm2").ToString())
    drApp.Read()
    MaxBuyName.InnerText = drApp("Name").ToString().Trim()
    MaxBuyText.InnerText = drApp("Parm2").ToString().Trim()
    MaxBuy.Text = drApp("Nbr2").ToString()
    If Trace.IsEnabled Then Trace.Warn("Economy", "Record 3 Name: " + drApp("Name").ToString() + ", " + drApp("Parm2").ToString() + ", " + drApp("Nbr2").ToString())
    drApp.Read()
    MaxSellName.InnerText = drApp("Name").ToString().Trim()
    MaxSellText.InnerText = drApp("Parm2").ToString().Trim()
    MaxSell.Text = drApp("Nbr2").ToString()
    If Trace.IsEnabled Then Trace.Warn("Economy", "Record 4 Name: " + drApp("Name").ToString() + ", " + drApp("Parm2").ToString() + ", " + drApp("Nbr2").ToString())
    drApp.Read()
    BuySellName.InnerText = drApp("Name").ToString().Trim()
    BuySellText.InnerText = drApp("Parm2").ToString().Trim()
    BuySellTime.Text = drApp("Nbr2").ToString()
    If Trace.IsEnabled Then Trace.Warn("Economy", "Record 5 Name: " + drApp("Name").ToString() + ", " + drApp("Parm2").ToString() + ", " + drApp("Nbr2").ToString())
    drApp.Read()
    ExFeeName.InnerText = drApp("Name").ToString().Trim()
    ExFeeText.InnerText = drApp("Parm2").ToString().Trim()
    ExchangeFee.Text = drApp("Nbr2").ToString()
    If Trace.IsEnabled Then Trace.Warn("Economy", "Record 6 Name: " + drApp("Name").ToString() + ", " + drApp("Parm2").ToString() + ", " + drApp("Nbr2").ToString())
    drApp.Read()
    ExRateName.InnerText = drApp("Name").ToString().Trim()
    ExRateText.InnerText = drApp("Parm2").ToString().Trim()
    ExchangeRate.Text = drApp("Nbr2").ToString()
    If Trace.IsEnabled Then Trace.Warn("Economy", "Record 7 Name: " + drApp("Name").ToString() + ", " + drApp("Parm2").ToString() + ", " + drApp("Nbr2").ToString())
    drApp.Read()
    Dim Name() As String
    Name = drApp("Parm2").ToString().Trim().Split(" ")
    WBUserName.Text = Name(0).ToString().Trim()
    WBLastName.Text = Name(1).ToString().Trim()
    If Trace.IsEnabled Then Trace.Warn("Economy", "Record 8 Name: " + drApp("Name").ToString() + ", " + drApp("Parm2").ToString() + ", " + drApp("Nbr2").ToString())
    If Trace.IsEnabled Then Trace.Warn("Economy", "Extracted Name: " + Name(0).ToString() + ", " + Name(1).ToString())
    If drApp("Parm2").ToString().Trim().Length > 0 Then
     drApp.Close()
     'Get World Bank Account UUID
     SQLCmd = "Select UUID, " +
              " Case When " +
              "  (Select Total From economy_totals Where user_id=users.UUID) is null " +
              " Then 0 " +
              " Else" +
              "  (Select Total From economy_totals Where user_id=users.UUID) " +
              " End as Balance " +
              "From users " +
              "Where username=" + MyDB.SQLStr(Name(0)) + " and lastname=" + MyDB.SQLStr(Name(1))
     If Trace.IsEnabled Then Trace.Warn("Economy", "Get World Bank Account UUID SQLCmd: " + SQLCmd.ToString())
     drApp = MyDB.GetReader("MyData", SQLCmd)
     If MyDB.Error() Then
      If Trace.IsEnabled Then Trace.Warn("Economy", "DB Error: " + MyDB.ErrMessage())
     End If
     If drApp.HasRows() Then
      drApp.Read()
      WBUUID.InnerText = drApp("UUID").ToString().Trim()
      Balance.InnerText = drApp("Balance").ToString().Trim()
     Else                                                  ' Content does not exist yet
      WBUUID.InnerText = "ERROR: Not found!"
     End If
    End If
   End If
   UpdateGrid()

   DelTitle.InnerText = "Disable Economy"
   drApp.Close()

   ' Setup Edit Mode page display controls
   PageTitle.InnerText = "Edit Settings"
   ShowUpdate.Visible = True                               ' Show Update Display
   SetLevel.SelectedValue = ELevel
   ShowLvl2.Visible = (SetLevel.SelectedValue = 2)
   ShowLvl3A.Visible = (SetLevel.SelectedValue > 2)
   ShowLvl3B.Visible = (SetLevel.SelectedValue > 2)
   ShowLvl3C.Visible = (SetLevel.SelectedValue > 2)
   ShowLvl3D.Visible = (SetLevel.SelectedValue > 2)
   ShowLvl3E.Visible = (SetLevel.SelectedValue > 2)
   ShowLvl3F.Visible = (SetLevel.SelectedValue > 2)
   ShowLvl3G.Visible = (SetLevel.SelectedValue > 2)
   ShowLvl3H.Visible = (SetLevel.SelectedValue > 2)
   ShowLvl3I.Visible = (SetLevel.SelectedValue > 2)
   ShowLvl3J.Visible = (SetLevel.SelectedValue > 2)
   ShowLvl3K.Visible = (SetLevel.SelectedValue > 2)
   ShowSetup.Visible = False                               ' Disable the Setup display
  Else                                                     ' Add Mode, show blank fields
   ' Set the hidden fields with the Bank Name
   Dim drApp As MySqlDataReader
   ' Get all the Economy Control values
   SQLCmd = "Select Name,Parm2,Nbr2 From control Where Control='ECONOMY' and Parm1='WorldBankAcct'; "
   If Trace.IsEnabled Then Trace.Warn("Economy", "Get display Values SQLCmd: " + SQLCmd.ToString())
   drApp = MyDB.GetReader("MySite", SQLCmd)
   If drApp.HasRows() Then                                 ' Record was found
    drApp.Read()
    If drApp("Name").ToString().Trim().Length > 0 Then     ' There was a name
     If drApp("Name").ToString().Trim().Contains(" ") Then ' Correct Name format
      Dim Name() As String
      Name = drApp("Name").ToString().Trim().Split(" ")
      WBUserName1.Text = Name(0).ToString().Trim()
      WBLastName1.Text = Name(1).ToString().Trim()
     Else                                                  ' Invalid Name, set to default
      WBUserName1.Text = "World"
      WBLastName1.Text = "Bank"
     End If
    Else                                                   ' Empty name, set to default
     WBUserName1.Text = "World"
     WBLastName1.Text = "Bank"
    End If
   Else                                                    ' No record, set to default
    WBUserName1.Text = "World"
    WBLastName1.Text = "Bank"
   End If
   drApp.Close()
   ' Setup Add Mode page display controls
   Level.SelectedValue = 2
   PageTitle.InnerText = "Install Economy"
   ShowUpdate.Visible = False                              ' Hide Update display
   ShowSetup.Visible = True                                ' Show Setup Display
   If Trace.IsEnabled Then Trace.Warn("Economy", "BodyTag has onload: " + (Not BodyTag.Attributes.Item("onload") Is Nothing).ToString())
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    If Trace.IsEnabled Then Trace.Warn("Economy", "** Remove onload attribute")
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  PayPalEmail.Focus()                                      ' Set focus to the first field for entry

  Dim SBMenu As New TreeView
  ' Set up navigation options
  SBMenu.SetTrace = Trace.IsEnabled
  'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
  'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
  'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
  'SBMenu.AddItem("L", "CallEdit(0,'Economy.aspx');", "New Entry") ' Javascript activated entry
  'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
  SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
  SBMenu.AddItem("P", "/Account.aspx", "Account")
  SBMenu.AddItem("P", "/Logout.aspx", "Logout")
  If Session("ELevel") = 3 Then                           ' Accounting and reports only work when full accounting is in operation
   SBMenu.AddItem("B", "", "Blank Entry")
   SBMenu.AddItem("T", "", "Page Options")
   SBMenu.AddItem("P", "Accounting.aspx", "Accounting")
   SBMenu.AddItem("P", "ReportSelect.aspx", "Accounting Reports")
  End If
  If Trace.IsEnabled Then Trace.Warn("Economy", "Show Menu")
  SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
  ' Close Sidebar Menu object
  SBMenu.Close()

 End Sub

 Private Sub UpdateGrid()
  ' Get all the Region Type settings
  Dim GetRegType As MySqlDataReader
  SQLCmd = "Select Name,Parm2,Nbr1,Nbr2 From control Where Control='RegionTypes' Order by Nbr1"
  If Trace.IsEnabled Then Trace.Warn("Economy", "Get display Values SQLCmd: " + SQLCmd.ToString())
  GetRegType = MyDB.GetReader("MySite", SQLCmd)
  If GetRegType.HasRows() Then
   gvDisplay.DataSource = GetRegType
   gvDisplay.DataBind()
  Else
   gvDisplay.Visible = False
  End If
 End Sub

 ' Process data validation checks
 Private Function ValAddEdit(ByVal tVal As Integer) As String
  ' Parameter tAdd allows selections of Add mode only testing
  Dim aMsg As String
  aMsg = ""
  ' Process error checking as required, place messages in tMsg.
  If WBUserName.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing World Bank User Name!\r\n"
  End If
  If WBLastName.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing World Bank Last Name!\r\n"
  End If
  If tVal = 3 Then                                         ' Applies only when active level is 3
   If PayPalEmail.Text.ToString().Trim().Length = 0 Then
    aMsg = aMsg.ToString() + "Missing PayPal email address entered!\r\n"
   Else
    If Not PageCtl.ValidEmail(PayPalEmail.Text) Then
     aMsg = aMsg.ToString() + "Invalid PayPal email address entered!\r\n"
    End If
   End If
   If PPMerchantID.Text.ToString().Trim().Length = 0 Then
    aMsg = aMsg.ToString() + "Missing PayPal Merchant ID entered!\r\n"
   End If
   If ExchangeFee.Text.ToString().Trim().Length = 0 Then
    aMsg = aMsg.ToString() + "Missing Exchange Fee Amount!\r\n"
   Else
    If Not IsNumeric(ExchangeFee.Text) Then
     aMsg = aMsg.ToString() + "Exchange Fee must be an integer value!\r\n"
    End If
   End If
   If ExchangeRate.Text.ToString().Trim().Length = 0 Then
    aMsg = aMsg.ToString() + "Missing Exchange Rate!\r\n"
   Else
    If Not IsNumeric(ExchangeRate.Text) Then
     aMsg = aMsg.ToString() + "Exchange Rate must be an integer value!\r\n"
    End If
   End If
  End If
  'If FieldName.Text.ToString().Trim().Length = 0 Then
  ' aMsg = aMsg.ToString() + "Missing Field Name!\r\n"
  'End If
  Return aMsg
 End Function

 ' Update Button
 Private Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
  Dim tMsg As String
  tMsg = ""

  If SetLevel.SelectedValue = 1 Then                      ' Option 1. Remove Economy settings
   BodyTag.Attributes.Add("onload", "ShowDelWin();")      ' Activate onload option to show message validate Economy removal
  Else
   Dim ELevel As Integer
   ' Get Current Level
   Dim Economy As MySqlDataReader
   SQLCmd = "Select Nbr2 From control Where Control='ECONOMY' and Parm1='HasEconomy'"
   If Trace.IsEnabled Then Trace.Warn("Economy", "Get Economy Level SQLCmd: " + SQLCmd.ToString())
   Economy = MyDB.GetReader("MySite", SQLCmd)
   If Economy.HasRows() Then
    Economy.Read()
    ELevel = Economy("Nbr2")
   End If
   Economy.Close()
   If Trace.IsEnabled Then Trace.Warn("Economy", "Economy Level: " + ELevel.ToString())
   If ELevel <> SetLevel.SelectedValue Then               ' Economy Level change overrides any other updates
    SQLCmd = "Update control Set Nbr2=" + MyDB.SQLNo(SetLevel.SelectedValue) + " " +
             "Where Control='ECONOMY' and Parm1='HasEconomy';"
    If Trace.IsEnabled Then Trace.Warn("Economy", "Update control SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    If MyDB.Error() Then
     tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
    End If
    Session("ELevel") = SetLevel.SelectedValue             ' Update Session level 
   Else
    If SetLevel.SelectedValue = 3 Then                     ' Option 3. Full Economy
     tMsg = ValAddEdit(3)
     If tMsg.ToString().Trim().Length = 0 Then
      SQLCmd = "Update control Set Parm2=" + MyDB.SQLStr(PayPalEmail.Text) + " " +
               "Where Control='ECONOMY' and Parm1='PayPalEmail';" +
               "Update control Set Parm2=" + MyDB.SQLStr(PPMerchantID.Text) + " " +
               "Where Control='ECONOMY' and Parm1='PPMerchantID';" +
               "Update control Set Nbr2=" + MyDB.SQLStr(MaxBuy.Text) + " " +
               "Where Control='ECONOMY' and Parm1='MaxBuy';" +
               "Update control Set Nbr2=" + MyDB.SQLStr(MaxSell.Text) + " " +
               "Where Control='ECONOMY' and Parm1='MaxSell';" +
               "Update control Set Nbr2=" + MyDB.SQLStr(BuySellTime.Text) + " " +
               "Where Control='ECONOMY' and Parm1='BuySellTime';" +
               "Update control Set Nbr2=" + MyDB.SQLStr(ExchangeFee.Text) + " " +
               "Where Control='ECONOMY' and Parm1='ExchangeFee';" +
               "Update control Set Nbr2=" + MyDB.SQLStr(ExchangeRate.Text) + " " +
               "Where Control='ECONOMY' and Parm1='ExchangeRate';" +
               "Update control Set Nbr2=" + MyDB.SQLNo(SetLevel.SelectedValue) + " " +
               "Where Control='ECONOMY' and Parm1='HasEconomy';"
      If Trace.IsEnabled Then Trace.Warn("Economy", "Update control SQLCmd: " + SQLCmd.ToString())
      MyDB.DBCmd("MySite", SQLCmd)
      If MyDB.Error() Then
       tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
      End If
     End If
    Else                                                   ' Test for Level 2 validation
     tMsg = ValAddEdit(2)
    End If
   End If

   ' Check for World Bank user account name change, update the account. Account UUID is never to change, only the UserName, LastName.
   Dim users As MySqlDataReader
   SQLCmd = "Select UUID,username,lastname " +
            "From users " +
            "Where UUID=" + MyDB.SQLStr(WBUUID.InnerText)
   If Trace.IsEnabled Then Trace.Warn("Economy", "Get User SQLCmd: " + SQLCmd.ToString())
   users = MyDB.GetReader("MyData", SQLCmd)
   If users.HasRows() Then
    users.Read()                                           ' Record is required to exist as it has already been shown.
    If SetLevel.SelectedValue = 2 Then                     ' Option 2. Access Control Setup
     If WorldFunds.Text.ToString().Trim().Length > 0 Then  ' Starting funds amount is more than zero
      SQLCmd = "Insert economy_transaction " +
               "(sourceAvatarID,destAvatarID,transactionAmount,transactionType,transactionDescription,timeOccurred) " +
               "Values ('00000000-0000-0000-0000-000000000000'," + MyDB.SQLStr(users("UUID")) + "," + MyDB.SQLNo(WorldFunds.Text) + "," +
               "5001,'Owner Add $',UNIX_TIMESTAMP(NOW()))"
      If Trace.IsEnabled Then Trace.Warn("Economy", "Insert economy_transaction: " + SQLCmd.ToString())
      MyDB.DBCmd("MyData", SQLCmd)
      If MyDB.Error() Then
       tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
      Else
       WorldFunds.Text = ""
      End If
     End If
    End If

    If tMsg.ToString().Trim().Length = 0 Then
     If Trace.IsEnabled Then Trace.Warn("Economy", "** New Name: " + WBUserName.Text.ToString() + " " + WBLastName.Text.ToString() +
                                       " DB Name: " + users("username").ToString().Trim() + " " + users("lastname").ToString().Trim())
     If WBUserName.Text.ToString().Trim() <> users("username").ToString().Trim() Or
       WBLastName.Text.ToString().Trim() <> users("lastname").ToString().Trim() Then
      Dim userChk As MySqlDataReader
      SQLCmd = "Select UUID " +
               "From users " +
               "Where username=" + MyDB.SQLStr(WBUserName.Text) + " and lastname=" + MyDB.SQLStr(WBLastName.Text)
      If Trace.IsEnabled Then Trace.Warn("Economy", "Check User SQLCmd: " + SQLCmd.ToString())
      userChk = MyDB.GetReader("MyData", SQLCmd)
      If userChk.HasRows() Then                            ' May not use that name
       tMsg = "Account name is already in use! You may not assign the bank to another existing account."
      Else
       ' Update Name change
       SQLCmd = "Update users Set " +
                " username=" + MyDB.SQLStr(WBUserName.Text) + ", lastname=" + MyDB.SQLStr(WBLastName.Text) + " " +
                "Where UUID=" + MyDB.SQLStr(users("UUID"))
       If Trace.IsEnabled Then Trace.Warn("Economy", "Update Users SQLCmd: " + SQLCmd.ToString())
       MyDB.DBCmd("MyData", SQLCmd)
       If MyDB.Error() Then
        tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
       Else
        ' Update control entry
        SQLCmd = "Update control Set Parm2=" + MyDB.SQLStr(WBUserName.Text.ToString().Trim() + " " + WBLastName.Text.ToString().Trim()) + " " +
                 "Where Control='ECONOMY' and Parm1='WorldBankAcct';"
        If Trace.IsEnabled Then Trace.Warn("Economy", "Update control SQLCmd: " + SQLCmd.ToString())
        MyDB.DBCmd("MySite", SQLCmd)
        If MyDB.Error() Then
         tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
        End If
       End If
      End If
      userChk.Close()
     End If
    End If
   Else
    If Trace.IsEnabled Then Trace.Warn("Economy", "Failed to locate World Bank account for UUID='" + WBUUID.InnerText.ToString() + "'!")
    tMsg = "Failed to locate World Bank account for UUID='" + WBUUID.InnerText.ToString() + "'!"
   End If
   users.Close()
   ' Update only if there is a name change
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot update Entry:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")          ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                ' Display Alert Message
  Else
   Display()
  End If

 End Sub

 ' Called by verification 
 Private Sub SetDel_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SetDel.CheckedChanged
  If Trace.IsEnabled Then Trace.Warn("Economy", "Delete process Event")
  ' Action is triggered by a JavaScript process, not a button click.
  ' NOTE: The World Bank account and money processing tables are NOT cleared out. That would irreparably wipe out all transaction records 
  '  if the economy was accidently removed. If all of that is needed to be cleared, it can be done in the DB tables manually.
  'Remove all related entries in all affected tables.
  SQLCmd = "Delete From control Where Control='ECONOMY' and Parm1='PayPalEmail';" +
           "Delete From control Where Control='ECONOMY' and Parm1='PPMerchantID';" +
           "Delete From control Where Control='ECONOMY' and Parm1='MaxBuy';" +
           "Delete From control Where Control='ECONOMY' and Parm1='MaxSell';" +
           "Delete From control Where Control='ECONOMY' and Parm1='BuySellTime';" +
           "Delete From control Where Control='ECONOMY' and Parm1='ExchangeFee';" +
           "Delete From control Where Control='ECONOMY' and Parm1='ExchangeRate';" +
           "Update control Set Nbr1=0,Nbr2=0 " +
           "Where Control='ECONOMY' and Parm1='HasEconomy';"
  If Trace.IsEnabled Then Trace.Warn("Economy", "Delete Dependencies SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MySite", SQLCmd)
  ''Remove entry record
  'SQLCmd = "Delete From Table Where KeyID=" + MyDB.SQLNo(KeyID.Value)
  'If Trace.IsEnabled Then Trace.Warn("Economy", "Delete Table SQLCmd: " + SQLCmd.ToString())
  'MyDB.DBCmd("MySite", SQLCmd)
  SetDel.Checked = False
  Display()
 End Sub

 ' Install Button
 Private Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
  Dim SQLFields, SQLValues, tMsg As String
  tMsg = ""

  If tMsg.Length = 0 Then                                  ' Set up starting Economy controls
   Dim Economy As MySqlDataReader
   ' Check Region type Control records
   SQLCmd = "Select Name From control Where Control='ADMINLISTS' and Parm1='RegionTypes'"
   If Trace.IsEnabled Then Trace.Warn("Economy", "Check for RegionTypes SQLCmd: " + SQLCmd.ToString())
   Economy = MyDB.GetReader("MySite", SQLCmd)
   If Not Economy.HasRows() Then
    SQLCmd = "Insert Into control (Control,Name,Parm1,Parm2,Nbr1,Nbr2)" +
             "Values ('ADMINLISTS','Grid Region Types','RegionTypes','Region Types List and Prices for Full Economy',0,0); "
    If Trace.IsEnabled Then Trace.Warn("Economy", "Insert control SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    If MyDB.Error() Then
     tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
    Else
     'Install default collection of Region Types
     SQLCmd = "Insert Into control (Control,Name,Parm1,Parm2,Nbr1,Nbr2)" +
              "Values ('RegionTypes','Full Region','','Sort Value = Type number, Numeric value = Region Price',1,0);" +
              "Insert Into control (Control,Name,Parm1,Parm2,Nbr1,Nbr2)" +
              "Values ('RegionTypes','Landscape Region','','Sort Value = Type number, Numeric value = Region Price',1,0);" +
              "Insert Into control (Control,Name,Parm1,Parm2,Nbr1,Nbr2)" +
              "Values ('RegionTypes','Homestead Region','','Sort Value = Type number, Numeric value = Region Price',1,0);"
     If Trace.IsEnabled Then Trace.Warn("Economy", "Insert control SQLCmd: " + SQLCmd.ToString())
     MyDB.DBCmd("MySite", SQLCmd)
     If MyDB.Error() Then
      tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
     End If
    End If
   End If
   Economy.Close()

   If Level.SelectedValue >= 1 Then                        ' Option 2. Access Control Setup
    SQLCmd = "Select Nbr1 " +
             "From control " +
             "Where Control='ECONOMY' and Parm1='WorldBankAcct'"
    If Trace.IsEnabled Then Trace.Warn("Economy", "Check for Economy SQLCmd: " + SQLCmd.ToString())
    Economy = MyDB.GetReader("MySite", SQLCmd)
    If Economy.HasRows() Then
     SQLCmd = "Update control Set Parm2=" + MyDB.SQLStr(WBUserName1.Text + " " + WBLastName1.Text) + " " +
              "Where Control='ECONOMY' and Parm1='WorldBankAcct'; "
    Else                                                   ' Create World Bank account with SysAdmin Access
     SQLCmd = "Insert Into control (Control,Name,Parm1,Parm2,Nbr1,Nbr2)" +
              "Values ('ECONOMY','World Bank Account','WorldBankAcct'," +
              MyDB.SQLStr(WBUserName1.Text + " " + WBLastName1.Text) + ",99,3); "
    End If
    If Trace.IsEnabled Then Trace.Warn("Economy", "Insert control SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    Economy.Close()
    ' Check if Economy control records exists
    SQLCmd = "Select Nbr1 " +
             "From control " +
             "Where Control='ECONOMY' and Parm1='HasEconomy'"
    If Trace.IsEnabled Then Trace.Warn("Economy", "Check for Economy SQLCmd: " + SQLCmd.ToString())
    Economy = MyDB.GetReader("MySite", SQLCmd)
    If Economy.HasRows() Then
     SQLCmd = "Update control Set Nbr1=1,Nbr2=" + MyDB.SQLNo(Level.SelectedValue) + " " +
              "Where Control='ECONOMY' and Parm1='HasEconomy';"
    Else
     SQLCmd = "Insert Into control (Control,Name,Parm1,Parm2,Nbr1,Nbr2)" +
              "Values ('ECONOMY','Economy Setup','HasEconomy','Economy Management Installed',1," + MyDB.SQLNo(Level.SelectedValue) + "); "
    End If
    If Trace.IsEnabled Then Trace.Warn("Economy", "Insert control SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    Economy.Close()
   End If
   If Level.SelectedValue > 1 Then                         ' Options 2&3
    ' Create initial control table records. 
    SQLCmd = SQLCmd.ToString() +
             "Insert Into control (Control,Name,Parm1,Parm2,Nbr1,Nbr2)" +
             "Values ('ECONOMY','PayPal Email Address','PayPalEmail','',10,0); " +
             "Insert Into control (Control,Name,Parm1,Parm2,Nbr1,Nbr2)" +
             "Values ('ECONOMY','PayPal MerchantID','PPMerchantID','',11,0); " +
             "Insert Into control (Control,Name,Parm1,Parm2,Nbr1,Nbr2)" +
             "Values ('ECONOMY','Max Buy Exchange','MaxBuy','Most amount of any Buy$. Zero=no limit',12,50); " +
             "Insert Into control (Control,Name,Parm1,Parm2,Nbr1,Nbr2)" +
             "Values ('ECONOMY','Max Sell Exchange','MaxSell','Most amount of any Sell$. Zero=no limit',13,50); " +
             "Insert Into control (Control,Name,Parm1,Parm2,Nbr1,Nbr2)" +
             "Values ('ECONOMY','Buy/Sell Time Limit','BuySellTime','How many hours between Transactions. Zero=no limit.',14,24); " +
             "Insert Into control (Control,Name,Parm1,Parm2,Nbr1,Nbr2)" +
             "Values ('ECONOMY','Exchange Fee','ExchangeFee','Integer value for exchange % fee on all transactions',15,4); " +
             "Insert Into control (Control,Name,Parm1,Parm2,Nbr1,Nbr2)" +
             "Values ('ECONOMY','Exchange Rate','ExchangeRate','Amount of in world $ to one $USD',16,250);"
    If Trace.IsEnabled Then Trace.Warn("Economy", "Insert control SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    If MyDB.Error() Then
     tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
    End If
   End If

   ' Check for World Bank user account, if not exist, create the account.
   Dim users As MySqlDataReader
   SQLCmd = "Select UUID " +
            "From users " +
            "Where username=" + MyDB.SQLStr(WBUserName1.Text) + " and lastname=" + MyDB.SQLStr(WBLastName1.Text)
   If Trace.IsEnabled Then Trace.Warn("Economy", "Get User SQLCmd: " + SQLCmd.ToString())
   users = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   End If
   Dim UserUUID, tPass, tSalt As String
   If Not users.HasRows() Then                             ' Create World Bank user account
    UserUUID = Guid.NewGuid().ToString()

    tSalt = "" '+ UserUUID.Replace("-", "").ToString()
    tPass = PageCtl.CodePassword("MyMoneyBag2", tSalt.ToString())
    SQLFields = "UUID,username,lastname,passwordHash,passwordSalt,homeRegion," +
                "homeLocationX,homeLocationY,homeLocationZ,homeLookAtX,homeLookAtY,homeLookAtZ," +
                "created,lastLogin,profileImage,profileFirstImage,WebLoginKey,homeRegionID,iz_level,email," +
                "userInventoryURI,userAssetURI,profileAboutText,profileFirstText"
    SQLValues = MyDB.SQLStr(UserUUID) + ",N" + MyDB.SQLStr(WBUserName1.Text) + ",N" + MyDB.SQLStr(WBLastName1.Text) + "," +
                MyDB.SQLStr(tPass) + "," + MyDB.SQLStr(tSalt) + "," +
                "0,0,0,0,0,0,0,UNIX_TIMESTAMP(),0," +
                "'00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000'," +
                "'00000000-0000-0000-0000-000000000000',0,'','','','',''"
    SQLCmd = "Insert Into users (" + SQLFields + ") Values (" + SQLValues + ")"
    If Trace.IsEnabled Then Trace.Warn("Economy", "Insert users SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MyData", SQLCmd)
    If MyDB.Error() Then
     tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
    Else
     ' Create agent entry with starting location settings entry. Without this record user cannot logon.
     SQLFields = "UUID,agentIP,agentPort,agentOnline,loginTime,logoutTime,sessionID,secureSessionID,currentRegion,currentHandle,currentPos,currentLookAt"
     SQLValues = MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(Request.ServerVariables("remote_host")) + ",0,0,0,0," +
                 MyDB.SQLStr(Guid.NewGuid()) + "," + MyDB.SQLStr(Guid.NewGuid()) + "," +
                 "'00000000-0000-0000-0000-000000000000',0,<0,0,0>,<0,0,0>,"
     SQLCmd = "Insert Into agents (" + SQLFields + ") Values (" + SQLValues + ")"
     If Trace.IsEnabled Then Trace.Warn("Register", "Insert agents SQLCmd: " + SQLCmd.ToString())
     MyDB.DBCmd("MyData", SQLCmd)
     ' Preset User Preferences based on selection
     SQLFields = "user_id,recv_ims_via_email,listed_in_directory"
     SQLValues = MyDB.SQLStr(UserUUID) + ",0,0"
     SQLCmd = "Insert Into userpreferences (" + SQLFields + ") Values (" + SQLValues + ")"
     If Trace.IsEnabled Then Trace.Warn("Register", "Insert userpreferences SQLCmd: " + SQLCmd.ToString())
     MyDB.DBCmd("MyData", SQLCmd)

     ' Create inventory folders default list for Avatar starting with the My Inventory as the root folder
     tMsg = tMsg.ToString() + GridLib.CreateInvFolders(UserUUID, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), MyDB)
    End If
   Else
    users.Read()
    UserUUID = users("UUID").ToString()
   End If
   users.Close()
   WBUUID.InnerText = UserUUID.ToString()                  ' This must be displayed!

   ' Check for usereconomy table in MySite DB
   Dim usereconomy As MySqlDataReader
   SQLCmd = "Select * From usereconomy"
   If Trace.IsEnabled Then Trace.Warn("Economy", "Check for usereconomy SQLCmd: " + SQLCmd.ToString())
   usereconomy = MyDB.GetReader("MySite", SQLCmd)
   If MyDB.Error() Then                                    ' Error Code: 1146. Table 'mysite.usereconomy' doesn't exist
    If Trace.IsEnabled Then Trace.Warn("Economy", "DB Error: " + MyDB.ErrMessage().ToString())
    If MyDB.ErrMessage().Contains("Table 'mysite.usereconomy' doesn't exist") Then
     SQLCmd = "CREATE TABLE `usereconomy` (" + vbCrLf +
              " `UUID` varchar(36) NOT NULL," + vbCrLf +
              " `MaxBuy` int(11) NOT NULL DEFAULT '0'," + vbCrLf +
              " `MaxSell` int(11) NOT NULL DEFAULT '0'," + vbCrLf +
              " `Hours` int(11) NOT NULL DEFAULT '0'," + vbCrLf +
              " PRIMARY KEY (`UUID`)" + vbCrLf +
              ") ENGINE=InnoDB DEFAULT CHARSET=utf8;" + vbCrLf +
              "CREATE TABLE `accountbal` (" + vbCrLf +
              " `UUID` varchar(36) NOT NULL," + vbCrLf +
              " `Name` varchar(30) NOT NULL," + vbCrLf +
              " `Action` varchar(50) NOT NULL," + vbCrLf +
              " `TransDate` datetime NOT NULL," + vbCrLf +
              " `Amount` decimal(10,4) NOT NULL," + vbCrLf +
              " `Actual` decimal(10,4) NOT NULL," + vbCrLf +
              " `TransFee` decimal(10,4) NOT NULL," + vbCrLf +
              " `txnID` varchar(50) NOT NULL," + vbCrLf +
              " KEY `UUID` (`UUID`)," + vbCrLf +
              " INDEX `TransDate` (`TransDate` ASC)" + vbCrLf +
              ") ENGINE=InnoDB DEFAULT CHARSET=utf8;" + vbCrLf
     ' Add table for PayPal account balance tracking and cash evaluation
     If Trace.IsEnabled Then Trace.Warn("Economy", "Create usereconomy SQLCmd: " + SQLCmd.ToString())
     MyDB.DBCmd("MySite", SQLCmd)
     If MyDB.Error() Then
      tMsg = "DB Create Error:\r\n For: " +
             SQLCmd.ToString() + "\r\n Please manually create these tables!"
     End If
    End If
   Else                                                    ' Close only if there was no error
    usereconomy.Close()
   End If
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot Create Economy:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")          ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                ' Display Alert Message
  End If
  Display()

 End Sub

 ' Region price changed
 Private Sub SetType_TextChanged(sender As Object, e As EventArgs) Handles SetType.TextChanged
  If Trace.IsEnabled Then Trace.Warn("Economy", "RegionType Price process Event")
  Dim tMsg As String
  tMsg = ""
  Dim Type, Price As Integer
  Type = CInt(SetType.Text)
  Price = CInt(SetPrice.Text)

  If Price < 0 Then
   tMsg = tMsg.ToString() + "Price may not be negative.\r\n"
  End If
  If SetPrice.Text.ToString().Trim().Contains(".") Then
   tMsg = tMsg.ToString() + "Price must be a whole number!\r\n"
  End If
  'If True Then
  ' tMsg = tMsg.ToString() + "\r\n"
  'End If
  If tMsg.Length = 0 Then                                  ' Set up starting Economy controls
   SQLCmd = "Update control Set Nbr2=" + MyDB.SQLNo(Price) + " " +
            "Where Control='RegionTypes' and Nbr1=" + MyDB.SQLNo(Type)
   If Trace.IsEnabled Then Trace.Warn("Economy", "Update control SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)
   If MyDB.Error() Then
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   End If
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   tMsg = "Cannot Update Region Type:\r\n" + tMsg
   BodyTag.Attributes.Add("onload", "ShowMsg();")          ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                ' Display Alert Message
  End If
  SetType.Text = ""
  UpdateGrid()
 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  GridLib = Nothing
  PageCtl = Nothing
 End Sub

End Class
