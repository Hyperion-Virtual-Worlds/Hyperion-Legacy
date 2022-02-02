
Partial Class Buy
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
 '* Buy page is only accessed by world users if there is level 3 full economy setup. It is where the 
 '* process of purchasing in-world money is done starts. The process is completed in PayPal and applied 
 '* to the user account in the IPN page process.

 '* Built from MyWorld Form template v. 1.0
 '***************************************************
 '* NOTE: This page does not write any data to the user world account. 
 '* Not until the transaction has been completed and the IPN page does the update will the funds be put in the user account transactions.
 '* The page is protected from attempts to "backup" and redo the purchase to get around any limits imposed.
 '***************************************************
 ' Define common Page class properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private PageCtl As New GridLib
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Validate logon and session existance.
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Response.Redirect("/Default.aspx")                       ' Return to logon page
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("/Default.aspx")
  End If
  If Session("ELevel") <> 3 Or Session("TLimited") Then     ' Economy Level 3 only
   Response.Redirect("/Account.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("Buy", "Start Page Load")

  If Not IsPostBack Then                                    ' First time page accessed setup
   ' Define process unique objects here

   ' Setup general page controls
   PayPal.Enabled = False                                   ' Disable PayPal button unless there is a valid amount entered to buy
   MyTotal.InnerText = "0"
   MAmt.Text = 0
   USDAmt.Text = 0
   Purchase.InnerText = "M$ 0"
   USDCost.InnerText = "US $0"
   Fee.InnerText = "US $0"
   USDTot.InnerText = "US $0"

   ' Display data fields based on edit or add mode
   Dim drApp As MySqlDataReader
   SQLCmd = "Select Total " +
            "From economy_totals " +
            "Where user_id=" + MyDB.SQLStr(Session("UUID"))
   If Trace.IsEnabled Then Trace.Warn("Buy", "Get economy_totals SQLCmd: " + SQLCmd.ToString())
   drApp = MyDB.GetReader("MyData", SQLCmd)
   If drApp.Read() Then
    MyTotal.InnerText = drApp("Total").ToString().Trim()
   End If
   drApp.Close()
   MAmt.Focus()                                             ' Set focus to the first field for entry

   ' Check User account limits
   Dim GetLimits As MySqlDataReader
   SQLCmd = "Select MaxBuy,Hours," +
            " (Select Nbr2 From control Where Control='ECONOMY' and Parm1='ExchangeFee') as XRate " +
            "From usereconomy " +
            "Where UUID=" + MyDB.SQLStr(Session("UUID"))
   If Trace.IsEnabled Then Trace.Warn("Buy", "Get usereconomy settings SQLCmd: " + SQLCmd.ToString())
   GetLimits = MyDB.GetReader("MySite", SQLCmd)
   If GetLimits.Read() Then
    Session("MaxBuy") = GetLimits("MaxBuy")
    Session("Hours") = GetLimits("Hours")
    Session("Fee") = GetLimits("XRate") / 100
    If Trace.IsEnabled Then Trace.Warn("Buy", "** Economy settings: MaxBuy=" + Session("MaxBuy").ToString() + ", Hours=" + Session("Hours").ToString())
   Else
    Session("MaxBuy") = 0
    Session("Hours") = 0
    Session("Fee") = 0.04
   End If
   GetLimits.Close()
   ShowLimit.Visible = False
   If Session("MaxBuy") > 0 Then
    ShowLimit.Visible = True
    MyLimit.InnerText = Session("MaxBuy").ToString()
   End If
   ShowHours.Visible = False
   If Session("Hours") > 0 Then
    ShowHours.Visible = True
    MyHours.InnerText = Session("Hours").ToString()
   End If

   If Session("Hours") > 0 Then                             ' Has Limit Test for time, Prevents use of back arrow to repeat transaction
    ' Check Time since last transaction
    Dim GetHours As MySqlDataReader
    SQLCmd = "Select TIMESTAMPDIFF(HOUR,TransDate,Now()) as Hours " +
             "From accountbal " +
             "Where UUID=" + MyDB.SQLStr(Session("UUID")) + " " +
             "Order by TransDate Desc " +
             "Limit 0,1"
    If Trace.IsEnabled Then Trace.Warn("Buy", "Get accountbal Last Transaction SQLCmd: " + SQLCmd.ToString())
    GetHours = MyDB.GetReader("MySite", SQLCmd)
    If GetHours.HasRows() Then
     GetHours.Read()
     If GetHours("Hours") < Session("Hours") Then           ' Time limited, disallow page access.
      GetHours.Close()
      Response.Redirect("Account.aspx")
     End If
    End If
    GetHours.Close()
   End If

   Dim SBMenu As New TreeView
   ' Set up navigation options
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                  ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                   ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Other Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'Buy.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")      ' Program URL link entry
   SBMenu.AddItem("P", "Account.aspx", "My Account")
   SBMenu.AddItem("P", "Logout.aspx", "Logout")
   SBMenu.AddItem("B", "", "Blank Entry")
   SBMenu.AddItem("T", "", "Page Options")
   SBMenu.AddItem("P", "TransHist.aspx", "$ Transaction History")
   'SBMenu.AddItem("P", "Sell.aspx", "Sell $M")              ' Comment out if not used!
   If Trace.IsEnabled Then Trace.Warn("Buy", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   ' Close Sidebar Menu object
   SBMenu.Close()
  End If

 End Sub

 ' MAmt to buy was changed
 Private Sub MAmt_TextChanged(sender As Object, e As EventArgs) Handles MAmt.TextChanged
  If Trace.IsEnabled Then Trace.Warn("Buy", "* MAmount changed called!")
  Dim tMsg As String
  tMsg = ""
  PayPal.Enabled = False

  If Session("Hours") > 0 Then                              ' Has Limit Test for time, Prevents use of back arrow to repeat transaction
   ' Check Time since last transaction
   Dim GetHours As MySqlDataReader
   SQLCmd = "Select TIMESTAMPDIFF(HOUR,TransDate,Now()) as Hours " +
            "From accountbal " +
            "Where UUID=" + MyDB.SQLStr(Session("UUID")) + " " +
            "Order by TransDate Desc " +
            "Limit 0,1"
   If Trace.IsEnabled Then Trace.Warn("Buy", "Get usereconomy settings SQLCmd: " + SQLCmd.ToString())
   GetHours = MyDB.GetReader("MySite", SQLCmd)
   If GetHours.HasRows() Then
    GetHours.Read()
    If GetHours("Hours") < Session("Hours") Then            ' Time limited, disallow page access.
     GetHours.Close()
     Response.Redirect("Account.aspx")
    End If
   End If
   GetHours.Close()
  End If

  Dim tRate As Integer
  tRate = 250                                            ' Assume exchange rate of $250 Grid money to $1.00 USD
  Dim tFRate As Double
  tFRate = 0

  Dim GetSettings As MySqlDataReader
  SQLCmd = "Select " +
           " (Select Nbr2 From control Where Control='ECONOMY' and Parm1='ExchangeRate') as Rate, " +
           " (Select Nbr2 From control Where Control='ECONOMY' and Parm1='ExchangeFee') as XRate"
  If Trace.IsEnabled Then Trace.Warn("Buy", "Get Rates SQLCmd: " + SQLCmd.ToString())
  GetSettings = MyDB.GetReader("MySite", SQLCmd)
  If GetSettings.HasRows() Then
   GetSettings.Read()
   tRate = GetSettings("Rate")
   tFRate = GetSettings("XRate") / 100
   If Trace.IsEnabled Then Trace.Warn("Buy", "** Rate: " + tRate.ToString())
   If Trace.IsEnabled Then Trace.Warn("Buy", "** XRate: " + tFRate.ToString())
  End If
  GetSettings.Close()

  If MAmt.Text.ToString().Trim().Length = 0 Then
   tMsg = tMsg.ToString() + "My Dollars may not be empty!\r\n"
  ElseIf Not IsNumeric(MAmt.Text) Then
   tMsg = tMsg.ToString() + "My Dollars must be a positive integer!\r\n"
  ElseIf CInt(MAmt.Text) < 0 Then
   tMsg = tMsg.ToString() + "My Dollars must be a positive integer!\r\n"
  End If

  If tMsg.ToString().Trim().Length = 0 Then
   ' Update display with new values based on this entry amount

   USDAmt.Text = CInt(MAmt.Text) / tRate                    ' Net Amount
   If Session("MaxBuy") > 0 Then                            ' Has a MaxBuy Limit
    If CInt(USDAmt.Text) > Session("MaxBuy") Then           ' Buy Amount Limit exceeded
     tMsg = "$USD Limit Exceeded! Maximum limit is $" + Session("MaxBuy").ToString() + "."
     USDAmt.Text = Session("MaxBuy")
    End If
   End If
   ' PayPal transaction fee = $0.30
   USDCost.InnerText = "US " + FormatCurrency(USDAmt.Text)  ' Net Amount
   Fee.InnerText = "US " + FormatCurrency((((CDbl(USDAmt.Text) * (1 + tFRate)) + 0.3) / (1 - 0.029)) - CDbl(USDAmt.Text), 2) ' Fee = Math.Round((((PNet +.30) / (1-.029))) - CDbl(USDAmt.Text),2)
   If Trace.IsEnabled Then Trace.Warn("Buy", "** Fee: " + (CDbl(USDAmt.Text) * tFRate).ToString()) ' Debug output
   If Trace.IsEnabled Then Trace.Warn("Buy", "** PFee: " + ((((CDbl(USDAmt.Text) * (1 + tFRate)) + 0.3) / (1 - 0.029)) - CDbl(USDAmt.Text)).ToString()) ' Debug output
   USDTot.InnerText = "US " + FormatCurrency(((CDbl(USDAmt.Text) * (1 + tFRate)) + 0.3) / (1 - 0.029), 2) ' Gross = (PNet +.30) / (1-.029) 
   MAmt.Text = Math.Round((CDbl(USDAmt.Text) * tRate), 0)    ' M$ Amount = Round(Net * Xrate)
   Purchase.InnerText = "M$ " + FormatNumber(CInt(MAmt.Text), 0,,, TriState.True).ToString().Replace(".00", "")

   PayPal.Enabled = True
   If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
    BodyTag.Attributes.Remove("onload")
   End If
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   BodyTag.Attributes.Add("onload", "ShowMsg();")           ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                 ' Display Alert Message
  End If

 End Sub

 ' $USD Amount changed
 Private Sub USDAmt_TextChanged(sender As Object, e As EventArgs) Handles USDAmt.TextChanged
  If Trace.IsEnabled Then Trace.Warn("Buy", "* USD Amount changed called!")
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then   ' Ok to process incomming data
   Response.Redirect("/Default.aspx")                       ' Return to logon page
  Else
   Dim tMsg As String
   tMsg = ""
   PayPal.Enabled = False

   If Session("Hours") > 0 Then                             ' Has Limit Test for time, Prevents use of back arrow to repeat transaction
    ' Check Time since last transaction
    Dim GetHours As MySqlDataReader
    SQLCmd = "Select TIMESTAMPDIFF(HOUR,TransDate,Now()) as Hours " +
             "From accountbal " +
             "Where UUID=" + MyDB.SQLStr(Session("UUID")) + " " +
             "Order by TransDate Desc " +
             "Limit 0,1"
    If Trace.IsEnabled Then Trace.Warn("Buy", "Get usereconomy settings SQLCmd: " + SQLCmd.ToString())
    GetHours = MyDB.GetReader("MySite", SQLCmd)
    If GetHours.HasRows() Then
     GetHours.Read()
     If GetHours("Hours") < Session("Hours") Then           ' Time limited, disallow page access.
      GetHours.Close()
      Response.Redirect("Account.aspx")
     End If
    End If
    GetHours.Close()
   End If

   Dim tRate As Integer
   tRate = 250                                              ' Assume exchange rate of $250 Grid money to $1.00 USD
   Dim tFRate As Double
   tFRate = 0

   Dim GetSettings As MySqlDataReader
   SQLCmd = "Select " +
            " (Select Nbr2 From control Where Control='ECONOMY' and Parm1='ExchangeRate') as Rate, " +
            " (Select Nbr2 From control Where Control='ECONOMY' and Parm1='ExchangeFee') as XRate"
   If Trace.IsEnabled Then Trace.Warn("Buy", "Get Rates SQLCmd: " + SQLCmd.ToString())
   GetSettings = MyDB.GetReader("MySite", SQLCmd)
   If GetSettings.HasRows() Then
    GetSettings.Read()
    tRate = GetSettings("Rate")
    tFRate = GetSettings("XRate") / 100
    If Trace.IsEnabled Then Trace.Warn("Buy", "** Rate: " + tRate.ToString())
    If Trace.IsEnabled Then Trace.Warn("Buy", "** XRate: " + tFRate.ToString())
   End If
   GetSettings.Close()

   If USDAmt.Text.ToString().Trim().Length = 0 Then
    tMsg = tMsg.ToString() + "Missing Field Name!\r\n"
    tMsg = tMsg.ToString() + "US Dollars may not be empty!\r\n"
   ElseIf Not IsNumeric(USDAmt.Text) Then
    tMsg = tMsg.ToString() + "US Dollars must be a positive value!\r\n"
   ElseIf CInt(USDAmt.Text) < 0 Then
    tMsg = tMsg.ToString() + "US Dollars must be a positive value!\r\n"
   End If

   If tMsg.ToString().Trim().Length = 0 Then
    ' Update display with new values based on this entry amount
    If Session("MaxBuy") > 0 Then                            ' Has a MaxBuy Limit
     If CInt(USDAmt.Text) > Session("MaxBuy") Then           ' Buy Amount Limit exceeded
      tMsg = "$USD Limit Exceeded! Maximum limit is $" + Session("MaxBuy").ToString() + "."
      USDAmt.Text = Session("MaxBuy")
     End If
    End If
    USDCost.InnerText = "US " + FormatCurrency(USDAmt.Text)  ' Net Amount
    Fee.InnerText = "US " + FormatCurrency((((CDbl(USDAmt.Text) * (1 + tFRate)) + 0.3) / (1 - 0.029)) - CDbl(USDAmt.Text), 2) ' Fee = Math.Round((((PNet +.30) / (1-.029))) - CDbl(USDAmt.Text),2)
    If Trace.IsEnabled Then Trace.Warn("Buy", "** Fee: " + (CDbl(USDAmt.Text) * tFRate).ToString()) ' Debug output
    If Trace.IsEnabled Then Trace.Warn("Buy", "** PFee: " + ((((CDbl(USDAmt.Text) * (1 + tFRate)) + 0.3) / (1 - 0.029)) - CDbl(USDAmt.Text)).ToString()) ' Debug output
    USDTot.InnerText = "US " + FormatCurrency(((CDbl(USDAmt.Text) * (1 + tFRate)) + 0.3) / (1 - 0.029), 2) ' Gross = (PNet +.30) / (1-.029) 
    MAmt.Text = Math.Round((CDbl(USDAmt.Text) * tRate), 0)    ' M$ Amount = Round(Net * Xrate)
    Purchase.InnerText = "M$ " + FormatNumber(MAmt.Text,,, TriState.True).ToString().Replace(".00", "")
    PayPal.Enabled = True
    If Not BodyTag.Attributes.Item("onload") Is Nothing Then ' Remove onload error message display 
     BodyTag.Attributes.Remove("onload")
    End If
   End If
   If tMsg.ToString().Trim().Length > 0 Then
    BodyTag.Attributes.Add("onload", "ShowMsg();")          ' Activate onload option to show message
    PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                ' Display Alert Message
   End If
  End If
 End Sub

 ' PayPal button Click 
 Protected Sub PayPal_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
  If Trace.IsEnabled Then Trace.Warn("Buy", "* PayPal Button clicked called!")
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then   ' Ok to process incoming data
   Response.Redirect("/Default.aspx")                       ' Return to logon page
  Else
   Dim GetSettings As MySqlDataReader
   SQLCmd = "Select Parm2 From control " +
            "Where Control='ECONOMY' and Parm1='PPMerchantID'"
   If Trace.IsEnabled Then Trace.Warn("Buy", "Get Control settings SQLCmd: " + SQLCmd.ToString())
   GetSettings = MyDB.GetReader("MySite", SQLCmd)
   GetSettings.Read()
   ' Get User information
   Dim Users As MySqlDataReader
   SQLCmd = "Select username,lastname,email " +
            "From users " +
            "Where UUID=" + MyDB.SQLStr(Session("UUID"))
   If Trace.IsEnabled Then Trace.Warn("Payment", "Get Account info SQLCmd: " + SQLCmd.ToString())
   Users = MyDB.GetReader("MyData", SQLCmd)
   Users.Read()
   business.Value = GetSettings("Parm2")
   custom.Value = Users("username").ToString().Trim() + ":" + Users("lastname").ToString().Trim()
   first_name.Value = Users("username").ToString().Trim()   ' NOTE: gets replaces with real first and last names
   last_name.Value = Users("lastname").ToString().Trim()
   email.Value = Users("email").ToString().Trim()
   email.Visible = (Users("email").ToString().Trim().Length > 0)
   item_number.Value = 150                                  ' Item number for display. Means nothing.
   amount.Value = (USDTot.InnerText).ToString().Replace("US $", "")
   If Trace.IsEnabled Then Trace.Warn("Payment", "** USDTot.InnerText: " + USDTot.InnerText.ToString())
   If Trace.IsEnabled Then Trace.Warn("Payment", "** amount.Value: " + amount.Value.ToString())
   Users.Close()
   GetSettings.Close()
   If Not Trace.IsEnabled Then
    BodyTag.Attributes.Add("onload", "SendPayPal();")       ' Activate PayPal form post
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
