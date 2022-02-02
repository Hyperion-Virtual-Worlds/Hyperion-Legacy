
Partial Class Administration_SavePages
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
 '* This page is the core foundation template on which all other templates are derived and a good 
 '* start for creating a new template that has not already been made or unique one of a kind page.
 '* 
 '* Built from Website Basic Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Validate logon and session existance.
  If Not (Session.Count() = 0 Or Len(Session("UUID")) = 0) Then
   If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
    Response.Redirect("/Default.aspx")
   End If
   If Session("Access") <> 9 Then                           ' SysAdmin Only access
    Response.Redirect("Admin.aspx")
   End If

   Trace.IsEnabled = False
   If Trace.IsEnabled Then Trace.Warn("SavePages", "Start Page Load")

   Dim GetControl As MySqlClient.MySqlDataReader
   SQLCmd = "Select Name,Path " +
            "From pagemaster " +
            "Order by Name"
   If Trace.IsEnabled Then Trace.Warn("ControlForm", "Get display Values SQLCmd: " + SQLCmd.ToString())
   GetControl = MyDB.GetReader("MySite", SQLCmd)
   If GetControl.HasRows() Then
    ' Prepare data to send as a tab delimited file format
    Dim tOutLine As String
    tOutLine = ""
    Response.Clear()
    ' Set the filename to display in save dialog: Content Disposition forces the download.
    Response.AddHeader("Content-Disposition", "attachment; filename=" + Chr(34) + "PageList.txt" + Chr(34))
    tOutLine = ""
    While GetControl.Read()
     'Name, Path
     tOutLine = tOutLine +
                GetControl("Name").ToString().Trim() + Chr(9) + GetControl("Path").ToString().Trim() + Chr(9) + vbCrLf
    End While
    Response.AddHeader("Content-Length", tOutLine.Length)
    Response.ContentType = "text/plain"                     ' Force the browser initiate a download
    Response.Write(tOutLine)
   End If
   GetControl.Close()

  Else
   Response.Redirect("Admin.aspx")
  End If
 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
