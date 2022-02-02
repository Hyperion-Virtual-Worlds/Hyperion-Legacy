
Partial Class TransHist
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
 '* User Account inworld money Transaction History display page. Shows all transactions in a users account
 '* by month and year. 
 '* 

 '* Built from MyWorld Select template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Validate logon and session existance.
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Response.Redirect("/Default.aspx")                       ' Return to logon page
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("/Default.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("TransHist", "Start Page Load")

  If Not IsPostBack Then                                    ' First time page is called setup
   ' Define process unique objects here

   ' Define local objects here
   If Session("FirstTime") <> "TransHist" Then
    Session("FirstTime") = "TransHist"
   End If

   ' Setup general page controls

   ' Display data fields based on edit or add mode
   Dim drApp As MySqlDataReader
   SQLCmd = "Select Total " +
            "From economy_totals " +
            "Where user_id=" + MyDB.SQLStr(Session("UUID"))
   If Trace.IsEnabled Then Trace.Warn("TransHist", "Get economy_totals SQLCmd: " + SQLCmd.ToString())
   drApp = MyDB.GetReader("MyData", SQLCmd)
   If drApp.Read() Then
    MyTotal.InnerText = drApp("Total").ToString().Trim()
   Else
    MyTotal.InnerText = "0"
   End If
   drApp.Close()

   ' Get list of month/year from economy_transaction 
   Dim GetMYList As MySqlDataReader
   SQLCmd = "Select Distinct Month,Year " +
            "From " +
            " (Select MONTH(FROM_UNIXTIME(timeOccurred)) as Month,YEAR(FROM_UNIXTIME(timeOccurred)) as Year " +
            "  From economy_transaction " +
            "  Where TransactionAmount>0 and " +
            "   (sourceAvatarID=" + MyDB.SQLStr(Session("UUID")) + " Or destAvatarID=" + MyDB.SQLStr(Session("UUID")) + ")) as RawData " +
            "Order by Year DESC, Month DESC"
   If Trace.IsEnabled Then Trace.Warn("TransHist", "GetMYList SQLCmd: " + SQLCmd.ToString())
   GetMYList = MyDB.GetReader("MyData", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("TransHist", "DB Error: " + MyDB.ErrMessage().ToString())
   If GetMYList.HasRows() Then
    While GetMYList.Read()
     TransDate.Items.Add(GetMYList("Month").ToString().Trim() + "/" + GetMYList("Year").ToString().Trim())
    End While
   End If
   GetMYList.Close()
   If TransDate.Items.Count() > 0 Then
    TransDate.SelectedValue = TransDate.Items.Item(0).Value ' Select the first entry
   Else
    ShowDates.InnerHtml = "<br />&nbsp;<b>No transaction history was found.</b>"
   End If

   ' Set up navigation options
   ' Sidebar Options control based on Clearance or Write Access
   Dim SBMenu As New TreeView
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                  ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                   ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Other Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")      ' Program URL link entry
   SBMenu.AddItem("P", "Account.aspx", "My Account")
   SBMenu.AddItem("P", "Logout.aspx", "Logout")
   SBMenu.AddItem("B", "", "Blank Entry")
   SBMenu.AddItem("T", "", "Page Options")
   If Session("ELevel") = 3 And Not Session("TLimited") Then ' Economy Level 3 only
    ' PayPal Email address and PayPal Merchant ID is required before going here.
    Dim GetPayPal As MySqlDataReader
    SQLCmd = "Select " +
             " (Select Parm2 From control Where Control='ECONOMY' and Parm1='PayPalEmail') as PPEmail," +
             " (Select Parm2 From control Where Control='ECONOMY' and Parm1='PPMerchantID') as PPID"
    If Trace.IsEnabled Then Trace.Warn("Account", "Get usereconomy Limit SQLCmd: " + SQLCmd.ToString())
    GetPayPal = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("Account", "DB Error: " + MyDB.ErrMessage().ToString())
    GetPayPal.Read()
    If GetPayPal("PPEmail").ToString().Trim().Length > 0 And GetPayPal("PPID").ToString().Trim().Length > 0 Then
     SBMenu.AddItem("P", "Buy.aspx", "Buy M$")
     'SBMenu.AddItem("P", "Sell.aspx", "Sell M$")             ' Comment out if not used!
    End If
    GetPayPal.Close()
   End If
   If Trace.IsEnabled Then Trace.Warn("TransHist", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   SBMenu.Close()

   Display()
  End If

 End Sub

 Private Sub Display()
  If Trace.IsEnabled Then Trace.Warn("TransHist", "Display() Called")
  Dim Month, Year As Integer
  Dim MYAry() As String

  If TransDate.Items.Count() > 0 Then
   MYAry = TransDate.SelectedValue.ToString().Split("/")
   Month = CInt(MYAry(0))
   Year = CInt(MYAry(1))
  Else
   Month = Date.Today().Month()
   Year = Date.Today().Year
  End If
  ' Get Display list Items here
  Dim Display As MySqlDataReader
  SQLCmd = "Select FROM_UNIXTIME(timeOccurred) as DateTime," +
           " Case When sourceAvatarID=" + MyDB.SQLStr(Session("UUID")) + " " +
           " Then " +
           "  Case When RTrim(transactionDescription) = 'Gift' " +
           "  Then CONCAT('To: ',(Select CONCAT(username,' ',lastname) as Name From users " +
           "   Where UUID=economy_transaction.DestAvatarID),' - ',transactionDescription) " +
           "  Else transactionDescription " +
           "  End " +
           " Else CONCAT('From: ',(Select CONCAT(username,' ',lastname) as Name From users " +
           "  Where UUID=economy_transaction.sourceAvatarID),' - ',transactionDescription) " +
           " End as Description," +
           " Case When sourceAvatarID=" + MyDB.SQLStr(Session("UUID")) + " Then TransactionAmount Else 0 End As Debit," +
           " Case When DestAvatarID=" + MyDB.SQLStr(Session("UUID")) + " Then TransactionAmount Else 0 End As Credit," +
           " (Select Sum(Case When sourceAvatarID=" + MyDB.SQLStr(Session("UUID")) + " " +
           "   Then -TransactionAmount Else TransactionAmount End) as Bal " +
           "  From economy_transaction B " +
           "  Where ID<=economy_transaction.id and TransactionAmount>0 and " +
           "   (sourceAvatarID=" + MyDB.SQLStr(Session("UUID")) + " Or destAvatarID=" + MyDB.SQLStr(Session("UUID")) + ")) as Balance " +
           "From economy_transaction " +
           "Where TransactionAmount>0 and (sourceAvatarID=" + MyDB.SQLStr(Session("UUID")) + " Or " +
           " destAvatarID=" + MyDB.SQLStr(Session("UUID")) + ") and " +
           " Date(FROM_UNIXTIME(timeOccurred)) between " +
           " DATE(" + MyDB.SQLStr(Year.ToString() + "-" + Month.ToString() + "-1") + ") and " +
           " DATE(LAST_DAY(" + MyDB.SQLStr(Year.ToString() + "-" + Month.ToString() + "-1") + ")) " +
           "Order by timeOccurred Desc"
  If Trace.IsEnabled Then Trace.Warn("TransHist", "Get SQLCmd: " + SQLCmd.ToString())
  Display = MyDB.GetReader("MyData", SQLCmd)
  gvDisplay.DataSource = Display
  gvDisplay.DataBind()

 End Sub

 Private Sub TransDate_TextChanged(sender As Object, e As EventArgs) Handles TransDate.TextChanged
  If Trace.IsEnabled Then Trace.Warn("TransHist", "TransDate() Called")
  Display()
 End Sub

 Public Function ShowUTF8(ByVal textIn As String) As String
  Return MyDB.Iso8859ToUTF8(textIn)
 End Function

 Public Function FmtMoney(aVal As Object, ShowZero As Boolean) As String
  Dim tOut As String
  tOut = ""
  If ShowZero Then
   tOut = "$0"
  End If
  If aVal > 0 Then
   tOut = "$" + aVal.ToString()
  ElseIf aVal < 0 Then
   tOut = "-$" + aVal.ToString()
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
