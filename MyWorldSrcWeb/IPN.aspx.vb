Partial Class _IPN
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
 '* The IPN page processes secured communication between PayPal and the website to handle cash 
 '* transactions and apply the results to the world account.
 '* 

 '* Built from MyWorld Basic Page Default v. 1.0
 '*******************************************************************************
 '* This page is controlled by the /Administration/Economy page settings.
 '* It will only process completed PayPal transaction notifications.
 '*******************************************************************************

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                                 ' Provides data access methods and error handling
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  ' Debugging verification of operation sent via email.
  Dim tFilePath, tEmailOut, tSMTP As String
  Dim tPort As Integer
  Dim tLog As Boolean
  tEmailOut = ""
  tSMTP = ""
  tPort = 0
  tLog = True                                                 ' Set to false to turn off file logging
  If Trace.IsEnabled And tLog Then Trace.Warn("IPN", "Map Logging is active.")

  ' Log file output setup
  Dim sw As System.IO.StreamWriter
  tFilePath = Server.MapPath("").ToString()
  If Trace.IsEnabled Then Trace.Warn("IPN", "File Path: " + tFilePath.ToString())
  sw = System.IO.File.AppendText(tFilePath.ToString() + "\IPNLog.txt")

  Trace.IsEnabled = False
  ' NOTE: this page cannot be debugged directly. So some alternate options have been added to send debugging info to an email or a text file.
  If Trace.IsEnabled Then Trace.Warn("IPN", "IPN Page Load")
  If Trace.IsEnabled Then Response.ContentType = "text/xml"
  If Trace.IsEnabled Then Response.Write("Start Page Load<br>")
  tEmailOut = tEmailOut.ToString() + "IPN Page Load" + vbCrLf + vbCrLf

  If tLog Then                                                ' Log file output
   ' Trace Actions to Log file
   sw.WriteLine("IPN Page Load " + FormatDateTime(Date.Now(), DateFormat.GeneralDate))
   sw.Flush()
  End If

  ' Setup a Request.Form object so it can be read through by values sent as I have no way to know the full list.
  If Request.Form.Count() > 0 Then

   '' *** Global reporting on any operation done, (activate for email trace of operations) ***
   'Dim drHost As MySqlDataReader
   '' Get the SMTP Address and Port for email processing.
   'SQLCmd = "Select " +
   '         "(Select Parm2 From control Where Parm1='SMTPServer') as SMTPServer," +
   '         "(Select Parm2 From control Where Parm1='SMTPPort') as SMTPPort"
   'drHost = MyDB.GetReader("MySite", SQLCmd)
   'If drHost.HasRows() Then
   ' drHost.Read()
   ' tSMTP = drHost("SMTPServer").ToString().Trim()
   ' tPort = CInt(drHost("SMTPPort"))
   'End If
   'drHost.Close()

   ' Send response back to PayPal site for verification:
   Dim requestStream As System.IO.Stream = Nothing
   Dim tResponse As System.Net.WebResponse = Nothing
   Dim WebPost As System.Net.WebRequest
   Dim reader As System.IO.StreamReader
   Dim OutParms, ReturnInfo As String
   Dim OutBytes() As Byte

   '*********************
   'Set values for the request back (PayPal Example)
   Dim Param() As Byte = Request.BinaryRead(HttpContext.Current.Request.ContentLength)
   OutParms = "cmd=_notify-validate&" + Encoding.UTF8.GetString(Param)
   '*********************

   If Trace.IsEnabled Then Trace.Warn("IPN", "Form Values Received: " + Encoding.UTF8.GetString(Param).ToString())
   If Trace.IsEnabled Then Response.Write("Form Values Received: " + Encoding.UTF8.GetString(Param).ToString() + "<br>")
   tEmailOut = "Form Values Received: " + Encoding.UTF8.GetString(Param).ToString() + vbCrLf + vbCrLf +
               "Received ContentType: " + Request.ContentType.ToString() + vbCrLf +
               "Received Encoding: " + Request.ContentEncoding.ToString() + vbCrLf + vbCrLf
   If tLog Then                                               ' Log file output
    ' Trace Actions to Log file
    sw.WriteLine(tEmailOut)
    sw.Flush()
   End If

   ' Encode output parameters: Send back what was received from PayPal.
   If OutParms.ToString().Contains(vbCrLf) Then
    ' Do nothing
   ElseIf OutParms.ToString().Contains(vbCr) Then
    OutParms = OutParms.ToString().Replace(vbCr, vbCrLf).ToString()
   End If

   If Trace.IsEnabled Then Trace.Warn("IPN", "Values Sending Out: " + OutParms.ToString())
   If Trace.IsEnabled Then Response.Write("Values Sending Out: " + OutParms.ToString() + "<br><br>")
   tEmailOut = tEmailOut.ToString() + "Values Sending Out: " + OutParms.ToString() + vbCrLf + vbCrLf
   If tLog Then                                               ' Log file output
    ' Trace Actions to Log file
    sw.WriteLine("Values Sending Out: " + OutParms.ToString() + vbCrLf)
    sw.Flush()
   End If

   'OutBytes = System.Text.Encoding.UTF8.GetBytes(OutParms.ToString())
   OutBytes = System.Text.Encoding.ASCII.GetBytes(OutParms.ToString())
   tEmailOut = tEmailOut.ToString() + "UTF8 Bytes: " + OutBytes.Length.ToString() + vbCrLf
   tEmailOut = tEmailOut.ToString() + "ASCII Bytes: " + System.Text.Encoding.ASCII.GetBytes(OutParms.ToString()).Length.ToString() + vbCrLf + vbCrLf
   If tLog Then                                               ' Log file output
    ' Trace Actions to Log file
    sw.WriteLine("UTF8 Bytes: " + OutBytes.Length.ToString() + vbCrLf +
                 "ASCII Bytes: " + System.Text.Encoding.ASCII.GetBytes(OutParms.ToString()).Length.ToString() + vbCrLf)
    sw.Flush()
   End If

   WebPost = System.Net.WebRequest.Create("https://www.PayPal.com/cgi-bin/webscr") ' Live PayPal validation. 
   'WebPost = System.Net.WebRequest.Create("https://www.Sandbox.PayPal.com/cgi-bin/webscr") ' PayPal sandbox testing. 
   If Trace.IsEnabled Then Trace.Warn("IPN", "Sent to: " + WebPost.RequestUri().ToString())
   If Trace.IsEnabled Then Response.Write("Sent to: " + WebPost.RequestUri().ToString() + "<br><br>")
   tEmailOut = tEmailOut.ToString() + "Sent to: " + WebPost.RequestUri().ToString() + vbCrLf + vbCrLf
   'WebPost.Credentials = System.Net.CredentialCache.DefaultCredentials
   'WebPost.Accept = "text/html"
   WebPost.Method = "Post"
   WebPost.ContentType = Request.ContentType '"application/x-www-form-urlencoded"
   WebPost.ContentLength = OutBytes.Length
   requestStream = WebPost.GetRequestStream()
   requestStream.Write(OutBytes, 0, OutBytes.Length)          ' Send outgoing request
   requestStream.Close()
   tResponse = WebPost.GetResponse()
   reader = New System.IO.StreamReader(tResponse.GetResponseStream())
   ReturnInfo = reader.ReadToEnd()                            ' Get response text
   reader.Close()

   If Trace.IsEnabled Then Trace.Warn("IPN", "Response Returned:" + ReturnInfo.ToString())
   If Trace.IsEnabled Then Response.Write("<br>Response Returned:" + ReturnInfo.ToString())
   tEmailOut = tEmailOut.ToString() + "Response Returned:" + ReturnInfo.ToString() + vbCrLf + vbCrLf
   If tLog Then                                               ' Log file output
    ' Trace Actions to Log file
    sw.WriteLine("Response Returned:" + ReturnInfo.ToString() + vbCrLf)
    sw.Flush()
   End If

   If ReturnInfo.ToString() = "VERIFIED" Then                 ' Process valid response from PayPal
    'Process data
    Dim PPEmail As String
    ' Get Economy PayPal Account email address
    Dim GetEmail As MySqlDataReader
    SQLCmd = "Select Parm2 From control " +
             "Where Control='ECONOMY' and Parm1='PayPalEmail'"
    If Trace.IsEnabled Then Trace.Warn("IPN", "Get PayPal Account email SQLCmd: " + SQLCmd.ToString())
    GetEmail = MyDB.GetReader("MySite", SQLCmd)
    GetEmail.Read()
    PPEmail = GetEmail("Parm2").ToString().Trim()
    GetEmail.Close()

    ' Check PayPal Account Email in info is correct. Field = receiver_email = PayPal Account email!
    If Request.Form("receiver_email").ToString().ToLower() = PPEmail.ToString().ToLower() Then
     If Trace.IsEnabled Then Trace.Warn("IPN", "Passed Email validation.")
     tEmailOut = tEmailOut.ToString() + "Passed Email validation." + vbCrLf
     If tLog Then                                             ' Log file output
      ' Trace Actions to Log file
      sw.WriteLine("Passed Email validation." + vbCrLf)
      sw.Flush()
     End If
     ' Verify payment status as completed Field= payment_status=Completed
     If Request.Form("payment_status").ToString().ToLower() = "completed" Then ' Only completed transactions are recorded
      If Trace.IsEnabled Then Trace.Warn("IPN", "Passed completed validation.")
      tEmailOut = tEmailOut.ToString() + "Passed completed validation." + vbCrLf
      If tLog Then                                            ' Log file output
       ' Trace Actions to Log file
       sw.WriteLine("Passed completed validation." + vbCrLf)
       sw.Flush()
      End If
      ' If item_name=Donation OR recurring_payment, create donation record entry.
      Dim TransType, Name(), WBName() As String
      TransType = ""

      ' Valid page transaction will send grid account names in the custom field
      If Len(Request.Form("custom")) > 0 And Request.Form("custom").ToString().Contains(":") Then
       Name = Request.Form("custom").ToString().Split(":")
      Else                                                    ' If no custom content, fall back to the first_name and last_name fields
       ReDim Name(2)                                          ' Match the two expected fields
       Name(0) = Request.Form("first_name").ToString().Trim()
       Name(1) = Request.Form("last_name").ToString().Trim()
      End If
      If Len(Request.Form("item_name")) > 0 Then              ' What was processed by the sending page
       TransType = Request.Form("item_name").ToString().Trim().ToLower()
      Else                                                    ' Fall back to the transaction type
       TransType = Request.Form("txn_type").ToString().Trim().ToLower()
      End If

      If tLog Then                                            ' Log file output
       ' Trace Actions to Log file
       sw.WriteLine("TransType: " + TransType.ToString() + ", Custom: " + Request.Form("custom") +
                    "last_name: " + Name(0).ToString() + ", first_name: " + Name(0).ToString() + vbCrLf)
       sw.Flush()
      End If
      ' Process each Transtype the website passed through PayPal
      If TransType.ToString() = "buy$" Then                   ' Order payment processing
       '' *** Debugging process tracer for Buy$ processing only ***
       'Dim drHost As MySqlDataReader
       '' Get the SMTP Address and Port for email processing.
       'SQLCmd = "Select " +
       '         "(Select Parm2 From control Where Parm1='SMTPServer') as SMTPServer," +
       '         "(Select Parm2 From control Where Parm1='SMTPPort') as SMTPPort"
       'drHost = MyDB.GetReader("MySite", SQLCmd)
       'If drHost.HasRows() Then
       ' drHost.Read()
       ' tSMTP = drHost("SMTPServer").ToString().Trim()
       ' tPort = CInt(drHost("SMTPPort"))
       'End If
       'drHost.Close()
       If Trace.IsEnabled Then Trace.Warn("IPN", "Is a World Cash Purchase validation.")
       If Trace.IsEnabled Then Trace.Warn("IPN", "<br>Fields to load: " + Request.Form("first_name").ToString() + ", " +
                   Request.Form("last_name").ToString() + ", " + Request.Form("txn_id").ToString())
       tEmailOut = tEmailOut.ToString() + "Is a World Cash Purchase validation. Amount= " + Request.Form("mc_gross").ToString() + vbCrLf +
                   "Fields to load: " + Request.Form("first_name").ToString() + ", " +
                   Request.Form("last_name").ToString() + ", " + Request.Form("txn_ID").ToString() + ", " +
                   TransType.ToString().Trim() + vbCrLf + vbCrLf
       If tLog Then                                           ' Log file output
        ' Trace Actions to Log file
        sw.WriteLine("Is a World Cash Purchase validation. Amount= " + Request.Form("mc_gross").ToString() + vbCrLf +
                     "Fields to load: " + Request.Form("first_name").ToString() + ", " +
                     Request.Form("last_name").ToString() + ", " + Request.Form("txn_ID").ToString() + ", " +
                     TransType.ToString().Trim() + vbCrLf)
        sw.Flush()
       End If

       Dim tRate As Integer
       tRate = 250                                            ' Assume exchange rate of $250 Grid money to $1.00 USD
       Dim tFRate As Double
       tFRate = 0
       Dim UserUUID, WBUUID As String
       UserUUID = ""
       WBUUID = ""
       ' Get Exchange rate and World Bank Name
       Dim GetSettings As MySqlDataReader
       SQLCmd = "Select " +
                " (Select Nbr2 From control Where Control='ECONOMY' and Parm1='ExchangeRate') as Rate, " +
                " (Select Nbr2 From control Where Control='ECONOMY' and Parm1='ExchangeFee') as XRate, " +
                " (Select Parm2 From control Where Control='ECONOMY' and Parm1='WorldBankAcct') as WBName"
       If Trace.IsEnabled Then Trace.Warn("IPN", "Get ExchangeRate Setting: " + SQLCmd.ToString())
       If Trace.IsEnabled Then Response.Write("<br>Get ExchangeRate Setting: " + SQLCmd.ToString())
       tEmailOut = tEmailOut.ToString() + "Get ExchangeRate Setting: " + SQLCmd.ToString() + vbCrLf + vbCrLf
       If tLog Then                                           ' Log file output
        ' Trace Actions to Log file
        sw.WriteLine("Get ExchangeRate Setting: " + SQLCmd.ToString() + vbCrLf)
        sw.Flush()
       End If
       GetSettings = MyDB.GetReader("MySite", SQLCmd)
       If GetSettings.HasRows() Then
        GetSettings.Read()
        tRate = GetSettings("Rate")
        tFRate = GetSettings("XRate") / 100
        WBName = GetSettings("WBName").ToString().Split(" ") ' Defines first and last account names
       Else
        ReDim WBName(1)                                      ' No name or rate was found!
        WBName(0) = ""
       End If
       GetSettings.Close()

       ' Get user's and World Bank UUID
       Dim GetUUID As MySqlDataReader
       SQLCmd = "Select " +
                " (Select UUID From users " +
                "  Where username=" + MyDB.SQLStr(Name(0)) + " and lastname=" + MyDB.SQLStr(Name(1)) + ") as UserUUID," +
                " (Select UUID From users " +
                "  Where username=" + MyDB.SQLStr(WBName(0)) + " and lastname=" + MyDB.SQLStr(WBName(1)) + ") as WBUUID"
       If Trace.IsEnabled Then Trace.Warn("IPN", "Get users Record: " + SQLCmd.ToString())
       If Trace.IsEnabled Then Response.Write("<br>Get users Record: " + SQLCmd.ToString())
       tEmailOut = tEmailOut.ToString() + "Get users Record: " + SQLCmd.ToString() + vbCrLf + vbCrLf
       If tLog Then                                           ' Log file output
        ' Trace Actions to Log file
        sw.WriteLine("Get users Record: " + SQLCmd.ToString() + vbCrLf)
        sw.Flush()
       End If
       GetUUID = MyDB.GetReader("MyData", SQLCmd)
       If GetUUID.Read() Then                                 ' Record exists, Ok to process it
        UserUUID = GetUUID("UserUUID")
        WBUUID = GetUUID("WBUUID")
       End If
       GetUUID.Close()

       Dim tTot, tFee, tXFee, tNet, tTax As Double
       tTax = IIf(Len(Request.Form("tax")) = 0, 0, CDbl(Request.Form("tax")))
       tNet = CDbl(Request.Form("mc_gross")) - CDbl(Request.Form("mc_fee")) - tTax  ' Extract the Net amount
       tXFee = Math.Round(tNet * tFRate, 2) ' Get the Exchange fee amount = Round(tNet * tFRate),2)
       ' Recalculate the inworld $amount to place in the account. Prevents any user alteration on what they paid to get.
       tTot = (tNet - tXFee) * tRate   ' Tot = (Net - exchange fee) * tRate to apply to buyers account
       tFee = tXFee * tRate  ' $ Amount applied to World Bank
       If tLog Then                                           ' Log file output
        ' Trace Actions to Log file
        sw.WriteLine("tNet: " + tNet.ToString() + ", tXFee: " + tXFee.ToString() + ", tTax: " + tTax.ToString() +
                     ", tTot: " + tTot.ToString() + ", tFee: " + tFee.ToString() + vbCrLf)
        sw.Flush()
       End If

       ' When the World Bank and anyone else buys $ it adds to the world total funds. 
       ' When the World Bank and anyone else cashes out its removing $ from the world fund total.
       ' Total world $ must be backed by the $USD at the nominal exchange rate.
       ' Fees charged are paid to the World Bank. Any other fees are also paid to the world bank. 
       ' The fees ensure that the funds tend to flow to the world bank. The exchange is intended to have what was put in equals what goes out.
       ' NOTE: a trigger on economy_transaction auto updates the totals table.
       ' Update Member transactions:
       SQLCmd = "Insert economy_transaction " +
                "(sourceAvatarID,destAvatarID,transactionAmount,transactionType,transactionDescription,timeOccurred) " +
                "Values ('00000000-0000-0000-0000-000000000000'," + MyDB.SQLStr(UserUUID) + "," + MyDB.SQLNo(Math.Round(tTot, 0)) + "," +
                "5001," + MyDB.SQLStr("Buy$ Exchange : " + Request.Form("txn_id")) + ",UNIX_TIMESTAMP(NOW()))"
       If WBName.Count() > 1 Then                             ' Apply the Exchange fee-PayPal fee to the World Bank - Identifies fee amounts for possible cashout
        SQLCmd = SQLCmd +
                 ";Insert economy_transaction " +
                 "(sourceAvatarID,destAvatarID,transactionAmount,transactionType,transactionDescription,timeOccurred) " +
                 "Values ('00000000-0000-0000-0000-000000000000'," + MyDB.SQLStr(WBUUID) + "," + MyDB.SQLNo(Math.Round(tFee, 0)) + "," +
                 "5001," + MyDB.SQLStr("Buy$ Fee Paid : " + Request.Form("txn_id")) + ",UNIX_TIMESTAMP(NOW()))"
       End If
       ' " + MyDB.SQLNo(DateDiff(DateInterval.Second, Date.Parse("1/1/1970 00:00:00"), Date.UtcNow())) + "
       If Trace.IsEnabled Then Trace.Warn("IPN", "Insert economy_transaction: " + SQLCmd.ToString())
       If Trace.IsEnabled Then Response.Write("<br>Insert economy_transaction: " + SQLCmd.ToString())
       tEmailOut = tEmailOut.ToString() + "Insert economy_transaction: " + SQLCmd.ToString() + vbCrLf + vbCrLf
       'Write cash transaction to the PayPal Balance tracking table
       If tLog Then                                           ' Log file output
        ' Trace Actions to Log file
        sw.WriteLine("Insert economy_transaction: " + SQLCmd.ToString() + vbCrLf)
        sw.Flush()
       End If
       MyDB.DBCmd("MyData", SQLCmd)
       If tLog And MyDB.Error() Then                          ' Log file output
        ' Trace Actions to Log file
        sw.WriteLine("Insert DB Error: " + MyDB.ErrMessage().ToString() + vbCrLf)
        sw.Flush()
       End If

       ' Update Accountbal Table
       tTot = CDbl(Request.Form("mc_gross")) - CDbl(Request.Form("mc_fee"))
       SQLCmd = "Insert into accountbal (UUID,Name,Action,TransDate,Amount,Actual,TransFee,ExchangeRate,TxnID) " +
                "Values (" + MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(Name(0).ToString() + " " + Name(1).ToString()) + "," +
                MyDB.SQLStr(TransType) + "," + "Now()," +
                MyDB.SQLNo(Request.Form("mc_gross")) + "," + MyDB.SQLNo(tTot) + "," +
                MyDB.SQLNo(Request.Form("mc_fee")) + "," + MyDB.SQLNo(tRate) + "," +
                MyDB.SQLStr(Request.Form("txn_id")) + ")"
       If Trace.IsEnabled Then Trace.Warn("IPN", "Insert accountbal: " + SQLCmd.ToString())
       If Trace.IsEnabled Then Response.Write("<br>Insert accountbal: " + SQLCmd.ToString())
       tEmailOut = tEmailOut.ToString() + "Insert accountbal: " + SQLCmd.ToString() + vbCrLf + vbCrLf
       'Write cash transaction to the PayPal Balance tracking table
       If tLog Then                                           ' Log file output
        ' Trace Actions to Log file
        sw.WriteLine("Insert accountbal: " + SQLCmd.ToString() + vbCrLf)
        sw.Flush()
       End If
       MyDB.DBCmd("MySite", SQLCmd)
       If tLog And MyDB.Error() Then                          ' Log file output
        ' Trace Actions to Log file
        sw.WriteLine("Insert DB Error: " + MyDB.ErrMessage().ToString() + vbCrLf)
        sw.Flush()
       End If

      ElseIf TransType.ToString() = "sell$" Then              ' Another transaction type
       ' ******************************************************
       ' * Sell$ PROCESS IS NOT BEING USED! It will be radically changed when that process has been completed.
       ' ******************************************************

       '' *** Debugging process tracer for Sell$ only ***
       'Dim drHost As MySqlDataReader
       '' Get the SMTP Address and Port for email processing.
       'SQLCmd = "Select " +
       '         "(Select Parm2 From control Where Parm1='SMTPServer') as SMTPServer," +
       '         "(Select Parm2 From control Where Parm1='SMTPPort') as SMTPPort"
       'drHost = MyDB.GetReader("MySite", SQLCmd)
       'If drHost.HasRows() Then
       ' drHost.Read()
       ' tSMTP = drHost("SMTPServer").ToString().Trim()
       ' tPort = CInt(drHost("SMTPPort"))
       'End If
       'drHost.Close()
       If Trace.IsEnabled Then Trace.Warn("IPN", "Is a World Cash Out validation. Amount= " + Request.Form("mc_gross").ToString())
       If Trace.IsEnabled Then Trace.Warn("IPN", "<br>Fields to load: " + Request.Form("first_name").ToString() + ", " +
                   Request.Form("last_name").ToString() + ", " + Request.Form("txn_id").ToString())
       tEmailOut = tEmailOut.ToString() + "Is a World Cash Out validation. Amount= " + Request.Form("mc_gross").ToString() + vbCrLf +
                  "Fields to load: " + Request.Form("first_name").ToString() + ", " +
                  Request.Form("last_name").ToString() + ", " + Request.Form("txn_ID").ToString() + ", " +
                  TransType.ToString().Trim() + vbCrLf + vbCrLf
       If tLog Then                                           ' Log file output
        ' Trace Actions to Log file
        sw.WriteLine("Is a World Cash Out validation. Amount= " + Request.Form("mc_gross").ToString() + vbCrLf +
                     "Fields to load: " + Request.Form("first_name").ToString() + ", " +
                     Request.Form("last_name").ToString() + ", " + Request.Form("txn_ID").ToString() + ", " +
                     TransType.ToString().Trim() + vbCrLf)
        sw.Flush()
       End If

       ' Get world bank account UUID
       Dim GetBank As MySqlDataReader
       SQLCmd = "Select UUID From users Where username='World' and lastname='Bank'"
       If Trace.IsEnabled Then Trace.Warn("IPN", "Get world bank UUID: " + SQLCmd.ToString())
       If Trace.IsEnabled Then Response.Write("<br>Get world bank UUID: " + SQLCmd.ToString())
       tEmailOut = tEmailOut.ToString() + "Get world bank UUID: " + SQLCmd.ToString() + vbCrLf + vbCrLf
       GetBank = MyDB.GetReader("MyData", SQLCmd)
       GetBank.Read()

       ' Get the user UUID. THIS IS FOR SALES PROCESSING ONLY!
       Dim GetUUID As MySqlDataReader
       SQLCmd = "Select UUID From users " +
                "Where username=" + MyDB.SQLStr(Name(0)) + " and " +
                " lastname=" + MyDB.SQLStr(Name(1))
       If Trace.IsEnabled Then Trace.Warn("IPN", "Get users Record: " + SQLCmd.ToString())
       If Trace.IsEnabled Then Response.Write("<br>Get users Record: " + SQLCmd.ToString())
       tEmailOut = tEmailOut.ToString() + "Get users Record: " + SQLCmd.ToString() + vbCrLf + vbCrLf
       GetUUID = MyDB.GetReader("MyData", SQLCmd)
       If GetUUID.Read() Then                                 ' Record exists, Ok to process it
        Dim tTot As Double
        ' Create record with current status
        If CDbl(Request.Form("mc_fee")) > 0 Then
         tTot = CDbl(Request.Form("mc_gross")) - CDbl(Request.Form("mc_fee"))
        Else
         tTot = CDbl(Request.Form("mc_gross"))
        End If
        'Policy point here!! Amount of grid money purchased depends on the exchange rate and if there was any exchange fee charged.
        ' tTot at this point is the Gross - PayPal fee. So the math needs to be adjusted if you want the PayPal fee deducted or not from the purchase amount.
        'Presume straight exchange rate
        tTot = tTot * 250 ' Assume exchange rate of $250 Grid money to $1.00 USD

        ' NOTE: a trigger on economy_transaction auto updates the totals table.
        ' Update Member transactions
        SQLCmd = "Insert economy_transaction " +
                 "(sourceAvatarID,destAvatarID,transactionAmount,transactionType,transactionDescription,timeOccurred) " +
                 "Values (" + MyDB.SQLStr(GetBank("UUID")) + "," + MyDB.SQLStr(GetUUID("UUID")) + "," + MyDB.SQLNo(-tTot) + "," +
                 "5001,'$M Sale'," + MyDB.SQLNo(DateDiff(DateInterval.Second, Date.Parse("1/1/1970 00:00:00"), Date.UtcNow())) + ");" +
                 "Insert economy_transaction " +
                 "(sourceAvatarID,destAvatarID,transactionAmount,transactionType,transactionDescription,timeOccurred) " +
                 "Values (" + MyDB.SQLStr(GetUUID("UUID")) + "," + MyDB.SQLStr(GetBank("UUID")) + "," + MyDB.SQLNo(tTot) + "," +
                 "5001,'$M Sale'," + MyDB.SQLNo(DateDiff(DateInterval.Second, Date.Parse("1/1/1970 00:00:00"), Date.UtcNow())) + ")"
        If Trace.IsEnabled Then Trace.Warn("IPN", "Insert economy_transaction: " + SQLCmd.ToString())
        If Trace.IsEnabled Then Response.Write("<br>Insert economy_transaction: " + SQLCmd.ToString())
        tEmailOut = tEmailOut.ToString() + "Insert economy_transaction: " + SQLCmd.ToString() + vbCrLf + vbCrLf
        MyDB.DBCmd("MyData", SQLCmd)

       End If
       GetUUID.Close()
       GetBank.Close()

       ' Disabled membership process until all that is required can be worked out
      ElseIf TransType.ToString() = "membership" And False Then ' Another transaction type
       ' ******************************************************
       ' * membership PROCESS IS NOT BEING USED! It will be radically changed when that process has been completed.
       ' ******************************************************

       '' *** Debugging process tracer for Membership only ***
       'Dim drHost As MySqlDataReader
       '' Get the SMTP Address and Port for email processing.
       'SQLCmd = "Select " +
       '         "(Select Parm2 From control Where Parm1='SMTPServer') as SMTPServer," +
       '         "(Select Parm2 From control Where Parm1='SMTPPort') as SMTPPort"
       'drHost = MyDB.GetReader("MySite", SQLCmd)
       'If drHost.HasRows() Then
       ' drHost.Read()
       ' tSMTP = drHost("SMTPServer").ToString().Trim()
       ' tPort = CInt(drHost("SMTPPort"))
       'End If
       'drHost.Close()

       ' Otherwise Verify the total matches pending transaction for valid account for SponsorID in field item_name
       ' Check that the trxID has not been set already Field= txn_id= up to 20 character value
       Dim tAmount, tActual As Double
       tAmount = CDbl(Request.Form("mc_gross"))               ' Total amount I was paid, not counting fees taken
       If CDbl(Request.Form("mc_fee")) > 0 Then
        tActual = CDbl(Request.Form("mc_gross")) - CDbl(Request.Form("mc_fee"))
       Else
        tActual = CDbl(Request.Form("mc_gross"))
       End If
       If Trace.IsEnabled Then Trace.Warn("IPN", "Is a Membership fee payment. Amount processed= " + tAmount.ToString())
       If Trace.IsEnabled Then Trace.Warn("IPN", "<br>Fields to load: " + Request.Form("first_name").ToString() + ", " +
                   Request.Form("last_name").ToString() + ", " + Request.Form("txn_ID").ToString())
       tEmailOut = tEmailOut.ToString() + "Is a Membership fee payment. Amount processed= " + tAmount.ToString() + vbCrLf +
                   "Fields to load: " + Request.Form("first_name").ToString() + ", " +
                   Request.Form("last_name").ToString() + ", " + Request.Form("txn_ID").ToString() + vbCrLf + vbCrLf
       If tLog Then                                           ' Log file output
        ' Trace Actions to Log file
        sw.WriteLine("Is a Membership fee payment. Amount processed= " + tAmount.ToString() + vbCrLf +
                     "Fields to load: " + Request.Form("first_name").ToString() + ", " +
                     Request.Form("last_name").ToString() + ", " + Request.Form("txn_ID").ToString() + vbCrLf)
        sw.Flush()
       End If
       ' Get Admin settings for membership that may apply

       ' Update MySite.UserMembership. Membership options are not supported in the MyData.user DB structure. Must be done in the MySite DB.
       ' Check if record exists
       Dim ChkAcct As MySqlDataReader
       SQLCmd = "Select UUID " +
                "From users " +
                "Where username=" + MyDB.SQLStr(Request.Form("first_name")) + " and " +
                " lastname=" + MyDB.SQLStr(Request.Form("last_name"))
       If Trace.IsEnabled Then Trace.Warn("Payment", "Get Account info SQLCmd: " + SQLCmd.ToString())
       ChkAcct = MyDB.GetReader("MyData", SQLCmd)
       ChkAcct.Read()
       If Trace.IsEnabled Then Trace.Warn("IPN", "Check for Users: " + SQLCmd.ToString())
       If Trace.IsEnabled Then Response.Write("<br>Check for Users: " + SQLCmd.ToString())
       tEmailOut = tEmailOut.ToString() + "Check for Users: " + SQLCmd.ToString() + vbCrLf + vbCrLf
       ChkAcct = MyDB.GetReader("MySite", SQLCmd)
       If ChkAcct.HasRows() Then
        SQLCmd = "Update UserMembership Set " +
                 "StipendAmt=0,Expire=" + MyDB.SQLDate(Date.Today().AddYears(1)) + " " +
                 "Where UserID=" + MyDB.SQLNo(Request.Form("item_number"))
        If Trace.IsEnabled Then Trace.Warn("IPN", "Set UserMembership: " + SQLCmd.ToString())
        If Trace.IsEnabled Then Response.Write("<br>Set UserMembership: " + SQLCmd.ToString())
        tEmailOut = tEmailOut.ToString() + "Set UserMembership: " + SQLCmd.ToString() + vbCrLf + vbCrLf
        MyDB.DBCmd("MySite", SQLCmd)
       Else
        SQLCmd = "Insert UserMembership (UserID,StipendAmt,Expire) Values(" +
                 MyDB.SQLNo(Request.Form("item_number")) + "," +
                 "0" + "," +
                 MyDB.SQLDate(Date.Today().AddYears(1)) + ")"
        If Trace.IsEnabled Then Trace.Warn("IPN", "Insert UserMembership: " + SQLCmd.ToString())
        If Trace.IsEnabled Then Response.Write("<br>Insert UserMembership: " + SQLCmd.ToString())
        tEmailOut = tEmailOut.ToString() + "Insert UserMembership: " + SQLCmd.ToString() + vbCrLf + vbCrLf
        MyDB.DBCmd("MySite", SQLCmd)
       End If
       ChkAcct.Close()
      Else
       If Trace.IsEnabled Then Trace.Warn("IPN", "Unidentified Transaction was sent!")
       If Trace.IsEnabled Then Response.Write("<br>Unidentified Transaction was sent!<br>" + Request.Form().ToString())
       tEmailOut = tEmailOut.ToString() + "Unidentified Transaction was sent!" + vbCrLf + Request.Form().ToString() + vbCrLf
       If tLog Then                                             ' Log file output
        ' Trace Actions to Log file
        sw.WriteLine("Unidentified Transaction was sent!<br>" + Request.Form().ToString() + vbCrLf)
        sw.Flush()
       End If
      End If
     End If
    Else
     If Trace.IsEnabled Then Trace.Warn("IPN", "Bad Email: " + Request.Form("receiver_email") + " != " + PPEmail.ToString())
     If Trace.IsEnabled Then Response.Write("<br>Bad Email: " + Request.Form("receiver_email") + " != " + PPEmail.ToString())
     tEmailOut = tEmailOut.ToString() + "Bad Email: " + Request.Form("receiver_email").ToString() + " != " + PPEmail.ToString() + vbCrLf
     If tLog Then                                             ' Log file output
      ' Trace Actions to Log file
      sw.WriteLine("Bad Email: " + Request.Form("receiver_email").ToString().ToLower() + " != " + PPEmail.ToString().ToLower() + vbCrLf)
      sw.Flush()
     End If
    End If
   Else
    If Trace.IsEnabled Then Trace.Warn("IPN", "Invalid response content returned.")
    If Trace.IsEnabled Then Response.Write("<br>Invalid response content returned.")
    tEmailOut = tEmailOut.ToString() + "Invalid response content returned." + vbCrLf
    If tLog Then                                              ' Log file output
     ' Trace Actions to Log file
     sw.WriteLine("Invalid response content returned." + vbCrLf)
     sw.Flush()
    End If
   End If
   If tSMTP.ToString().Trim().Length > 0 Then
    Dim Domain As String
    Domain = ""
    Dim drDomain As MySqlDataReader
    SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='EmailDomain'"
    If Trace.IsEnabled Then Trace.Warn("IPN", "Get location SQLCmd: " + SQLCmd)
    drDomain = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("IPN", "DB Error: " + MyDB.ErrMessage().ToString())
    If drDomain.HasRows() Then
     drDomain.Read()
     Domain = drDomain("Parm2").ToString()
    End If
    drDomain.Close()
    Dim SendMail As New SendEmail
    SendMail.SetTrace = Trace.IsEnabled
    SendMail.IsHTML = False
    SendMail.EmailServer = tSMTP.ToString()
    If tPort > 0 Then
     SendMail.Port = tPort
    End If
    SendMail.Subject = "IPN Transaction Report"
    SendMail.ToAddress = "director@" + Domain.ToString()
    SendMail.FromAddress = "mailer@" + Domain.ToString()
    SendMail.Body = tEmailOut.ToString()
    If SendMail.SendMail() Then
     If Trace.IsEnabled Then Trace.Warn("IPN", "Email sent!")
     If Trace.IsEnabled Then Response.Write("<br>Email sent!")
    Else
     If Trace.IsEnabled Then Trace.Warn("IPN", "Email Failed! Error: " + SendMail.ErrMessage().ToString())
     If Trace.IsEnabled Then Response.Write("<br>Email Failed! Error: " + SendMail.ErrMessage().ToString())
    End If
    SendMail.Close()
    SendMail = Nothing
   End If
  Else
   If Trace.IsEnabled Then Trace.Warn("IPN", "No Form data sent!")
   If Trace.IsEnabled Then Response.Write("<br>No Form data sent!")
   If tLog Then                                                ' Log file output
    ' Trace Actions to Log file
    sw.WriteLine("No Form data sent!" + vbCrLf)
    sw.Flush()
   End If
  End If
  If tLog Then                                                ' Log file output
   ' Trace Actions to Log file
   sw.WriteLine("********* Completed Processing *********" + vbCrLf)
   sw.Flush()
  End If
  sw.Close()

 End Sub

 ' This calls removes certificate validation for Mono implmentation
 Private Class CerPol
  Implements System.Net.ICertificatePolicy

  Public Function CheckValidationResult(ByVal srvPoint As System.Net.ServicePoint, ByVal certificate As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal request As System.Net.WebRequest, ByVal certificateProblem As Integer) As Boolean Implements System.Net.ICertificatePolicy.CheckValidationResult
   Return True
  End Function
 End Class

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
