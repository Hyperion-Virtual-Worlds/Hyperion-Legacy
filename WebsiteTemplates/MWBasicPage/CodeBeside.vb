
Partial Class $codebesideclassname$
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

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("Template", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page is called setup
   ' Define process unique objects here

   ' Define local objects here
   ' Setup general page controls

   ' Set up navigation options
   Dim SBMenu As New TreeView
   SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry")        ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry

   'SBMenu.AddItem("B", "", "Blank Entry")
   'SBMenu.AddItem("T", "", "Page Options")
   'SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/TempSelect.aspx", "Template Selection")
   If Trace.IsEnabled Then Trace.Warn("Template", "Show Menu")
   'SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   SBMenu.Close()
  End If

  ' Get Display list Items here

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
