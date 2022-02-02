
Partial Class Administration_TransMDRpt
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
  If Trace.IsEnabled Then Trace.Warn("TransMDRpt", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page is called setup
   ' Define process unique objects here

   ' Define local objects here

   ' Setup general page controls
   Dim aDate As DateTime
   aDate = Date.Today()

   ' Get list of month/year from economy_transaction 
   Dim GetMYList As MySqlDataReader
   SQLCmd = "Select Distinct Month,Year " +
            "From " +
            " (Select MONTH(FROM_UNIXTIME(timeOccurred)) as Month,YEAR(FROM_UNIXTIME(timeOccurred)) as Year " +
            "  From economy_transaction " +
            "  Where TransactionAmount>0 and " +
            "   ('00000000-0000-0000-0000-000000000000' Or destAvatarID='00000000-0000-0000-0000-000000000000')) as RawData " +
            "Order by Year DESC, Month DESC"
   If Trace.IsEnabled Then Trace.Warn("TransMDRpt", "GetMYList SQLCmd: " + SQLCmd.ToString())
   GetMYList = MyDB.GetReader("MyData", SQLCmd)
   If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("TransMDRpt", "DB Error: " + MyDB.ErrMessage().ToString())
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
   If Trace.IsEnabled Then Trace.Warn("TransMDRpt", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   SBMenu.Close()

   ' Gridview settings
   'gvDisplay.PageSize = 0

   Display()
  End If

 End Sub

 Private Sub Display()
  If Trace.IsEnabled Then Trace.Warn("TransMDRpt", "Display() Called")
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
  ' Month's total
  Dim Display As MySqlDataReader
  SQLCmd = "Select Sum(TransactionAmount) as Total " +
           "From economy_transaction " +
           "Where TransactionAmount>0 and " +
           " (sourceAvatarID='00000000-0000-0000-0000-000000000000' Or destAvatarID='00000000-0000-0000-0000-000000000000') and " +
           " FROM_UNIXTIME(timeOccurred) between " +
           " DATE(" + MyDB.SQLStr(Year.ToString() + "-" + Month.ToString() + "-1") + ") and " +
           " DATE(LAST_DAY(" + MyDB.SQLStr(Year.ToString() + "-" + Month.ToString() + "-1") + ")) " +
           "Order by timeOccurred Desc"
  If Trace.IsEnabled Then Trace.Warn("TransMDRpt", "Get SQLCmd: " + SQLCmd.ToString())
  Display = MyDB.GetReader("MyData", SQLCmd)
  If Display.HasRows() Then
   Display.Read()
   If Display("Total").ToString().Trim().Length > 0 Then
    ShowTotal.InnerText = "No" + FormatCurrency(Display("Total"), 0,,, True)
   End If
  End If
  Display.Close()

  ' Get Display list Items here
  SQLCmd = "Select FROM_UNIXTIME(timeOccurred) as DateTime,TransactionAmount,transactionDescription " +
           "From economy_transaction " +
           "Where TransactionAmount>0 and (sourceAvatarID='00000000-0000-0000-0000-000000000000' Or destAvatarID='00000000-0000-0000-0000-000000000000') and " +
           " FROM_UNIXTIME(timeOccurred) between " +
           " DATE(" + MyDB.SQLStr(Year.ToString() + "-" + Month.ToString() + "-1") + ") and " +
           " DATE(LAST_DAY(" + MyDB.SQLStr(Year.ToString() + "-" + Month.ToString() + "-1") + ")) " +
           "Order by timeOccurred Desc"
  If Trace.IsEnabled Then Trace.Warn("TransMDRpt", "Get SQLCmd: " + SQLCmd.ToString())
  Display = MyDB.GetReader("MyData", SQLCmd)
  gvDisplay.DataSource = Display
  gvDisplay.DataBind()

 End Sub

 Private Sub TransDate_TextChanged(sender As Object, e As EventArgs) Handles TransDate.TextChanged
  If Trace.IsEnabled Then Trace.Warn("TransMDRpt", "TransDate() Called")
  Display()
 End Sub

 Public Function FmtMoney(aVal As Object) As String
  Dim tOut As String
  If aVal >= 0 Then
   tOut = "No$" + aVal.ToString()
  Else
   tOut = "-No$" + aVal.ToString()
  End If
  Return tOut.ToString()
 End Function

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
