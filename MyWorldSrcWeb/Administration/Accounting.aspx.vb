
Partial Class Administration_Accounting
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
 '* Accounting totals display and World Bank adjustment.
 '* 
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
  If Session("Access") <> 9 Then                           ' SysAdmin Only access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("Accounting", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page accessed setup
   ' Define process unique objects here

   ' Setup general page controls
   Dim WBName() As String
   Session("WBUUID") = ""
   ' Get World Bank Name
   Dim GetSettings As MySqlDataReader
   SQLCmd = "Select Parm2 From control Where Control='ECONOMY' and Parm1='WorldBankAcct'"
   If Trace.IsEnabled Then Trace.Warn("Accounting", "Get ExchangeRate Setting: " + SQLCmd.ToString())
   GetSettings = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("Accounting", "DB Error: " + MyDB.ErrMessage().ToString())
   If GetSettings.HasRows() Then
    GetSettings.Read()
    WBName = GetSettings("Parm2").ToString().Split(" ") ' Defines first and last account names
   Else
    ReDim WBName(1)                                      ' No name or rate was found! Keeps compiler happy.
    WBName(0) = ""
   End If
   GetSettings.Close()

   ' Get user's and World Bank UUID
   Dim GetUUID As MySqlDataReader
   SQLCmd = "Select UUID as WBUUID From users " +
            "Where username=" + MyDB.SQLStr(WBName(0)) + " and lastname=" + MyDB.SQLStr(WBName(1))
   If Trace.IsEnabled Then Trace.Warn("Accounting", "Get users Record: " + SQLCmd.ToString())
   GetUUID = MyDB.GetReader("MyData", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("Accounting", "DB Error: " + MyDB.ErrMessage().ToString())
   If GetUUID.Read() Then                                 ' Record exists, Ok to process it
    Session("WBUUID") = GetUUID("WBUUID")
   End If
   GetUUID.Close()

   Dim SBMenu As New TreeView
   ' Set up navigation options
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'Accounting.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
   SBMenu.AddItem("P", "Economy.aspx", "Economy Management")
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   SBMenu.AddItem("B", "", "Blank Entry")
   SBMenu.AddItem("T", "", "Page Options")
   SBMenu.AddItem("P", "ReportSelect.aspx", "Accounting Reports")
   If Trace.IsEnabled Then Trace.Warn("Accounting", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   ' Close Sidebar Menu object
   SBMenu.Close()

   Display()
  End If

 End Sub

 Private Sub Display()
  If Trace.IsEnabled Then Trace.Warn("Economy", "Display() Called")
  Dim AcctBal, AccountTot As Double
  ' Display World $ Account Balance
  Dim drApp As MySqlDataReader
  SQLCmd = "Select " +
           " (Select Sum(total) as Bal From economy_totals) as WorldTotal," +
           " (Select total From economy_totals Where user_id=" + MyDB.SQLStr(Session("WBUUID")) + ") as WBBal"
  If Trace.IsEnabled Then Trace.Warn("Accounting", "Get World $ Total SQLCmd: " + SQLCmd.ToString())
  drApp = MyDB.GetReader("MyData", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("Accounting", "DB Error: " + MyDB.ErrMessage().ToString())
  If drApp.Read() Then
   If drApp("WorldTotal").ToString().Trim().Length = 0 Then
    ShowWorldBal.InnerText = "$0.00"
   Else
    ShowWorldBal.InnerText = FormatCurrency(drApp("WorldTotal"), 2)
   End If
   AccountTot = IIf(drApp("WorldTotal").ToString().Trim().Length = 0, 0, drApp("WorldTotal"))
   WBBalance.InnerText = "$" + drApp("WBBal").ToString()
  End If
  drApp.Close()

  ' Display PayPal tracking Account Balance
  SQLCmd = "Select Sum(Actual) as Bal From accountbal"
  If Trace.IsEnabled Then Trace.Warn("Accounting", "Get PayPal $ Total SQLCmd: " + SQLCmd.ToString())
  drApp = MyDB.GetReader("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("Accounting", "DB Error: " + MyDB.ErrMessage().ToString())
  If drApp.Read() Then
   If drApp("Bal").ToString().Trim().Length = 0 Then
    ShowPPBal.InnerText = "$0.00"
    AcctBal = 0
   Else
    ShowPPBal.InnerText = FormatCurrency(drApp("Bal"), 2)
    AcctBal = drApp("Bal")
   End If
  End If
  drApp.Close()

  If AcctBal > 0 Then
   ShowRatio.InnerText = (AccountTot / AcctBal).ToString()
  Else
   ShowRatio.InnerText = AccountTot.ToString() + " / 0 "
  End If
  SQLCmd = "Select Nbr2 From control Where Control='ECONOMY' and Parm1='ExchangeRate'"
  drApp = MyDB.GetReader("MySite", SQLCmd)
  If drApp.Read() Then
   ShowRatio.InnerText = "Exchange Rate: " + ShowRatio.InnerText + ", Target = " + drApp("Nbr2").ToString()
  End If
  drApp.Close()
  ' Setup Edit Mode page display controls
  UpdDelBtn.Visible = True                                 ' Allow Update and Delete button to show
  Adjusment.Focus()                                        ' Set focus to the first field for entry

 End Sub

 ' Process data validation checks
 Private Function ValAddEdit(ByVal tAdd As Boolean) As String
  ' Parameter tAdd allows selections of Add mode only testing
  Dim aMsg As String
  aMsg = ""
  ' Process error checking as required, place messages in tMsg.
  If Adjusment.Text.ToString().Trim().Length = 0 Then
   aMsg = aMsg.ToString() + "Missing Adjustment amount!\r\n"
  Else
   If Not IsNumeric(Adjusment.Text) Then
    aMsg = aMsg.ToString() + "Adjustment amount must be a +/- whole number!\r\n"
   ElseIf Adjusment.Text.ToString().Contains(".") Then
    aMsg = aMsg.ToString() + "Adjustment amount must be a +/- whole number!\r\n"
   ElseIf CInt(Adjusment.Text) = 0 Then
    aMsg = aMsg.ToString() + "Adjustment amount must not be zero!\r\n"
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
  tMsg = ValAddEdit(False)

  If tMsg.ToString().Trim().Length = 0 Then

   ' Make journal entry adjustment.
   SQLCmd = "Insert economy_transaction " +
            "(sourceAvatarID,destAvatarID,transactionAmount,transactionType,transactionDescription,timeOccurred) " +
            "Values ('00000000-0000-0000-0000-000000000000'," + MyDB.SQLStr(Session("WBUUID")) + "," + MyDB.SQLNo(Adjusment.Text) + "," +
            "5001,'$ Journal Entry',UNIX_TIMESTAMP(NOW()))"
   If Trace.IsEnabled Then Trace.Warn("Accounting", "Update economy_transaction SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MyData", SQLCmd)
   If MyDB.Error() Then
    If Trace.IsEnabled Then Trace.Warn("Accounting", "DB Error: " + MyDB.ErrMessage().ToString())
    tMsg = "DB Error: " + MyDB.ErrMessage() + "\r\n"
   End If
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

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub

End Class
