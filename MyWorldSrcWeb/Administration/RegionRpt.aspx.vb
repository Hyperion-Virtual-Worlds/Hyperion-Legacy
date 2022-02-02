
Partial Class Administration_RegionRpt
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
 '* This is the template for report display.

 '* Built from MyWorld Report template v. 1.0

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
  If Session("Access") < 6 Then                            ' Grid Administrator or SysAdmin access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("RegionRpt", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page is called setup
   ' Define process unique objects here

   ' Define local objects here

   ' Setup general page controls
   ReportDate.InnerText = FormatDateTime(Date.Today(), DateFormat.ShortDate)

   ' Set up navigation options
   ' Sidebar Options control based on Clearance or Write Access
   Dim SBMenu As New TreeView
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
   'SBMenu.AddItem("B", "", "Blank Entry")
   'SBMenu.AddItem("T", "", "Page Options")
   SBMenu.AddItem("P", "GridManager.aspx", "Grid Manager")
   If Trace.IsEnabled Then Trace.Warn("RegionRpt", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   SBMenu.Close()

  End If

  Dim ServerName, HTMLOut As String
  ' Get Display list Items here
  Dim Display As MySqlDataReader
  SQLCmd = "Select A.ServerName,A.externalIP,A.internalIP,B.regionName,B.port " +
           "From serverhosts A Inner Join regionxml B on A.externalIP=B.externalIP " +
           "Order by ServerName,port"
  If Trace.IsEnabled Then Trace.Warn("RegionRpt", "Get Report SQLCmd: " + SQLCmd.ToString())
  Display = MyDB.GetReader("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridManager", "DB Error: " + MyDB.ErrMessage().ToString())
  If Display.HasRows() Then
   ServerName = ""
   HTMLOut = "<table style=""width:100%;"">" + vbCrLf +
             " <tr>" + vbCrLf +
             "  <td style=""width: 90%;"">" + vbCrLf +
             "   <b>Server / Region</b> " + vbCrLf +
             "  </td>" + vbCrLf +
             "  <td style=""width: 10%;"">" + vbCrLf +
             "   <b>Port</b>" + vbCrLf +
             "  </td>" + vbCrLf +
             " </tr>" + vbCrLf
   While Display.Read()
    If Trace.IsEnabled Then Trace.Warn("RegionRpt", "Processing : " + Display("ServerName").ToString() +
       " and Region: " + Display("regionName").ToString())
    If ServerName.ToString() <> Display("ServerName").ToString() Then
     HTMLOut = HTMLOut.ToString() +
               " <tr style=""height: 25px; background-color: #eeeeee;"">" + vbCrLf +
               "  <td colspan=""2"">" + vbCrLf +
               "  <b>" + Display("ServerName").ToString() + "</b> " +
               Display("externalIP").ToString() + " " + Display("internalIP").ToString() + vbCrLf +
               "  </td>" + vbCrLf +
               " </tr>" + vbCrLf
     ServerName = Display("ServerName").ToString()
    End If
    HTMLOut = HTMLOut.ToString() +
              " <tr>" + vbCrLf +
              "  <td style=""width: 90%;"">" + vbCrLf +
              "   " + Display("regionName").ToString() + vbCrLf +
              "  </td>" + vbCrLf +
              "  <td style=""width: 10%;"">" + vbCrLf +
              "   " + Display("port").ToString() + vbCrLf +
              "  </td>" + vbCrLf +
              " </tr>" + vbCrLf
   End While
   HTMLOut = HTMLOut.ToString() +
             "</table>" + vbCrLf
   ShowReport.InnerHtml = HTMLOut.ToString()
  End If
  Display.Close()

 End Sub

 Public Function ShowUTF8(ByVal textIn As String) As String
  Return MyDB.Iso8859ToUTF8(textIn)
 End Function

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub

End Class
