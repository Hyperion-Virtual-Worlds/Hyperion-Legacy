
Partial Class Administration_TransYTDRpt
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
  If Trace.IsEnabled Then Trace.Warn("TransYTDRpt", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page is called setup
   ' Define process unique objects here

   ' Setup general page controls
   Dim aDate As DateTime
   aDate = Date.Today()

   ' Get list of month/year from economy_transaction 
   Dim GetMYList As MySqlDataReader
   SQLCmd = "Select Distinct Year " +
            "From " +
            " (Select YEAR(FROM_UNIXTIME(timeOccurred)) as Year " +
            "  From economy_transaction " +
            "  Where TransactionAmount>0 and " +
            "   (sourceAvatarID='00000000-0000-0000-0000-000000000000' Or destAvatarID='00000000-0000-0000-0000-000000000000')) as RawData " +
            "Order by Year DESC"
   If Trace.IsEnabled Then Trace.Warn("TransYTDRpt", "GetMYList SQLCmd: " + SQLCmd.ToString())
   GetMYList = MyDB.GetReader("MyData", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("TransHist", "DB Error: " + MyDB.ErrMessage().ToString())
   If GetMYList.HasRows() Then
    While GetMYList.Read()
     StartYear.Items.Add(GetMYList("Year").ToString().Trim())
    End While
   Else
    StartYear.Items.Add(Year(aDate))
   End If
   GetMYList.Close()
   If StartYear.Items.Count() > 0 Then
    StartYear.SelectedValue = StartYear.Items.Item(0).Value ' Select the first entry
   End If
   ReportDate.InnerText = FormatDateTime(aDate, DateFormat.ShortDate)

   ' Set up navigation options
   ' Sidebar Options control based on Clearance or Write Access
   Dim SBMenu As New TreeView
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
   SBMenu.AddItem("P", "ReportSelect.aspx", "Accounting Reports")
   SBMenu.AddItem("P", "Economy.aspx", "Economy Management")
   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   SBMenu.AddItem("B", "", "Blank Entry")
   SBMenu.AddItem("T", "", "Page Options")
   SBMenu.AddItem("P", "Accounting.aspx", "Accounting")
   If Trace.IsEnabled Then Trace.Warn("TempSelect", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   SBMenu.Close()

   Display()
  End If

 End Sub

 Private Sub Display()
  If Trace.IsEnabled Then Trace.Warn("TransYTDRpt", "Display() Called")
  Dim Year, I, tTotal, tMonth, LineCnt, RptMonth(12) As Integer ' Month Selection options
  Dim tHTMLOut As String
  Dim HeaderSet As Boolean

  Year = 0

  ' Get Display list Items here
  SQLCmd = "Select MONTH(FROM_UNIXTIME(timeOccurred)) as Month,YEAR(FROM_UNIXTIME(timeOccurred)) as Year,Sum(TransactionAmount) as MTotal " +
           "From economy_transaction " +
           "Where TransactionAmount>0 and " +
           " (sourceAvatarID='00000000-0000-0000-0000-000000000000' Or destAvatarID='00000000-0000-0000-0000-000000000000') and " +
           " FROM_UNIXTIME(timeOccurred) between " +
           " DATE(" + MyDB.SQLStr(StartYear.SelectedValue.ToString() + "-1-1") + ") and CURDATE() " +
           "Group by YEAR(FROM_UNIXTIME(timeOccurred)),MONTH(FROM_UNIXTIME(timeOccurred)) " +
           "Order by timeOccurred"
  If Trace.IsEnabled Then Trace.Warn("TransYTDRpt", "Get SQLCmd: " + SQLCmd.ToString())
  Dim RptData As MySqlDataReader
  RptData = MyDB.GetReader("MyData", SQLCmd, 180)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("TransYTDRpt", "DB Error= " + MyDB.ErrMessage().ToString())
  tHTMLOut = ""
  tTotal = 0                                               ' Total for year
  tMonth = 0                                               ' Current month to display counter
  HeaderSet = False
  If RptData.HasRows() Then
   ' Set up column Titles
   tHTMLOut = "<table width=""100%"" cellpadding=""0"" cellspacing=""1"">" + vbCrLf +
              " <tr class=""SubTitle"">" + vbCrLf +
              "  <td width=""5%"">Year</td>" + vbCrLf
   ' List number of months between start and end dates
   For I = 1 To 12
    tHTMLOut = tHTMLOut +
               "  <td width=""8%"">" + MonthName(I, True) + "</td>" + vbCrLf
   Next
   tHTMLOut = tHTMLOut +
              "  <td width=""8%"">Total</td>" + vbCrLf +
              " </tr>"
   While RptData.Read()

    If Year.ToString() <> RptData("Year").ToString().Trim() Then
     Year = RptData("Year")
     If HeaderSet Then                                     ' Starting a new row, finish current row
      If Trace.IsEnabled Then Trace.Warn("TransYTDRpt", "Print Month Data")
      For I = 1 To 12
       tHTMLOut = tHTMLOut +
                  "  <td width=""7%"">" + IIf(RptMonth(I) > 0, RptMonth(I).ToString(), " ") + "</td>" + vbCrLf
       tTotal = tTotal + RptMonth(I)                       ' Sum months for total in year
       RptMonth(I) = 0                                     ' Zero month value
      Next
      tHTMLOut = tHTMLOut +
                 "  <td width=""7%"" class=""Text3Bold"" style=""text-align: right;"">" + tTotal.ToString() + "</td>" + vbCrLf +
                 " </tr>" + vbCrLf
      tTotal = 0
     End If

     If Trace.IsEnabled Then Trace.Warn("TransYTDRpt", "Print Header Titles")
     LineCnt = LineCnt + 1
     If LineCnt > 1 Then
      LineCnt = 0
     End If
     tHTMLOut = tHTMLOut +
                " <tr" + IIf(LineCnt = 0, " class=""AltLine""", "") + ">" + vbCrLf
     tHTMLOut = tHTMLOut +
                 "  <td width=""8%"" height=""20"" class=""Text3Bold"">" + Year.ToString() + "</td>" + vbCrLf
     ' Update changes in data
     HeaderSet = True
    End If

    ' Fill array with counts
    RptMonth(RptData("Month")) = RptData("MTotal")
    tMonth = RptData("Month")

   End While
   ' Print last row of data
   If Trace.IsEnabled Then Trace.Warn("TransYTDRpt", "Print last record Month Data")
   For I = 1 To 12
    tHTMLOut = tHTMLOut +
               "  <td width=""7%"">" + IIf(RptMonth(I) > 0, RptMonth(I).ToString(), " ") + "</td>" + vbCrLf
    tTotal = tTotal + RptMonth(I)                          ' Sum months for total in year
   Next
   tHTMLOut = tHTMLOut +
              "  <td width=""8%"" class=""Text3Bold"" style=""text-align: right;"">" + tTotal.ToString() + "</td>" + vbCrLf +
              " </tr>" +
              "</table>" + vbCrLf
  End If
  RptData.Close()
  ShowReport.InnerHtml = tHTMLOut.ToString()

 End Sub

 Public Function ShowUTF8(ByVal textIn As String) As String
  Return MyDB.Iso8859ToUTF8(textIn)
 End Function

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  SqlClient.SqlConnection.ClearAllPools()      ' Causes all pooled connection to be forced closed when finished, reducing pooled connection size.
 End Sub

End Class
