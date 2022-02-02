
Partial Class Administration_WorldPromoMgt
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
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Response.Redirect("/Default.aspx")                      ' Return to logon page
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Response.Redirect("/Default.aspx")
  End If
  If Session("Access") < 2 Then                            ' Webmaster and above access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("WorldPromoMgt", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page is called setup
   ' Define process unique objects here

   ' Define local objects here
   ' Setup general page controls

   Dim tPath As String
   tPath = Request.MapPath("\Images\Site\WorldPromo")
   If Trace.IsEnabled Then Trace.Warn("WorldPromoMgt", "Session Path = " + tPath.ToString())
   If System.IO.Directory.GetCurrentDirectory() <> tPath Then
    If Not System.IO.Directory.Exists(tPath) Then
     System.IO.Directory.CreateDirectory(tPath)
    End If
    System.IO.Directory.SetCurrentDirectory(tPath)
   End If

   ' Set up navigation options
   Dim SBMenu As New TreeView
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry")        ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry

   SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
   SBMenu.AddItem("P", "/Account.aspx", "Account")
   SBMenu.AddItem("P", "/Logout.aspx", "Logout")
   'SBMenu.AddItem("B", "", "Blank Entry")
   'SBMenu.AddItem("T", "", "Page Options")
   'SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/TempSelect.aspx", "Template Selection")
   If Trace.IsEnabled Then Trace.Warn("WorldPromoMgt", "Show Menu")
   SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   SBMenu.Close()

   Display()
  End If
 End Sub

 Private Sub Display()
  If Trace.IsEnabled Then Trace.Warn("WorldPromoMgt", "* Display Called")
  Dim tOut, tPath, aFolder(), Filename As String
  tOut = ""
  tPath = System.IO.Directory.GetCurrentDirectory()

  For Each tFolder In System.IO.Directory.GetDirectories(tPath)
   If System.IO.Directory.GetFiles(tFolder).Count() > 0 Then
    aFolder = tFolder.Split("\")
    tOut = tOut.ToString() +
           "<h2>" + aFolder.Last().ToString() + "</h2><br />" + vbCrLf +
           "<div class=""PicFolder"">" + vbCrLf
    For Each tFile In System.IO.Directory.GetFiles(tFolder)
     Filename = System.IO.Path.GetFileName(tFile).ToString()
     tOut = tOut.ToString() +
            " <div style=""position: relative; display: flex;"">" + vbCrLf +
            "  <img src=""/Images/Site/WorldPromo/" + aFolder.Last().ToString() + "/" + Filename.ToString() + """ height=""100"" " +
            "alt=""" + Filename.ToString() + """ style=""cursor: pointer;"" " +
            "onclick=""ShowImg('/Images/Site/WorldPromo/" + aFolder.Last().ToString().Trim() + "/" + Filename.ToString().Trim() + "');"" " +
            "title=""Click to see larger view""/>" + vbCrLf +
            " </div>"
    Next
    tOut = tOut.ToString() +
           "</div>" + vbCrLf
   End If
  Next
  ShowList.InnerHtml = tOut.ToString()

 End Sub

 ' Move / Remove Image
 Protected Sub Action_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Action.TextChanged
  If Trace.IsEnabled Then Trace.Warn("WorldPromoMgt", "* Delete Image Called")
  Dim tFile, aDestination, Filename As String
  ' Remove Image
  If Trace.IsEnabled Then Trace.Warn("WorldPromoMgt", "* Filename: " + File.Value.ToString())
  Filename = System.IO.Path.GetFileName(File.Value).ToString()
  tFile = Request.MapPath(File.Value)
  If Trace.IsEnabled Then Trace.Warn("WorldPromoMgt", "* tFile: " + tFile.ToString())
  If Action.Text = "DEL" Then                             ' Delete File
   If Trace.IsEnabled Then Trace.Warn("WorldPromoMgt", "* Delete File: " + Filename.ToString())
   If System.IO.File.Exists(tFile) Then
    If Trace.IsEnabled Then Trace.Warn("WorldPromoMgt", "* tFile exists: " + Filename.ToString())
    System.IO.File.Delete(tFile)
   End If
  ElseIf Action.Text = "MOV" Then                         ' Move File to Backgrounds
   If Trace.IsEnabled Then Trace.Warn("WorldPromoMgt", "* Move File: " + tFile.ToString())
   If System.IO.File.Exists(tFile) Then
    aDestination = Request.MapPath("\Images\Site\Backgrounds\" + System.IO.Path.GetFileName(tFile).ToString())
    If Trace.IsEnabled Then Trace.Warn("WorldPromoMgt", "* Move File: " + tFile.ToString() + " to " + aDestination.ToString())
    System.IO.File.Move(tFile, aDestination)
   End If
  End If
  Action.Text = ""                                        ' Clear trigger
  Display()
 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
