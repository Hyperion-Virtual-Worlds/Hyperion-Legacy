
Partial Class Places
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
 '* This page provices world map display access and SLURL connection to clicked locations.
 '* 
 '* Built from Website Basic Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private SQLCmd As String
 Public RTitle, RImg, RMsg As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("Places", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page is called setup
   ' Define process unique objects here

   ' Define local objects here
   ' Setup general page controls

   ' Process if a Map location is given in URL:
   ' https://<domain>/Places.aspx/outpost/128/128/25/?title=Homestead&img=/Images/Site/Outpost.png&msg=Outpost%20in%20a%20small%20island.
   ' https://<domain>/Places.aspx/ima%20outpost/128/128/25/?title=IMA%20Outpost

   RTitle = ""                                             ' These three fields set JavaScript values on the page.
   RImg = ""
   RMsg = ""
   If Request("Title") <> "" Then
    RTitle = Request("Title")
    If Trace.IsEnabled Then Trace.Warn("Places", "Passed Title: " + Request("Title").ToString())
   End If
   If Request("Img") <> "" Then
    RImg = Request("Img")
    If Trace.IsEnabled Then Trace.Warn("Places", "Passed Img: " + Request("Img").ToString())
   End If
   If Request("Msg") <> "" Then
    RMsg = Request("Msg")
    If Trace.IsEnabled Then Trace.Warn("Places", "Passed Msg: " + Request("Msg").ToString())
   End If

   Dim path() As String = HttpContext.Current.Request.Url.AbsolutePath.Split("/"c)
   If path.Length > 2 Then                                 ' We have a region request.
    If Trace.IsEnabled Then Trace.Warn("Places", "Passed path(1): " + path(1).ToString())
    If Trace.IsEnabled Then Trace.Warn("Places", "Passed path(2): " + Server.UrlDecode(path(2)).ToString())
    ' Do lookup of the region name to get region grid coordinates.
    Dim GetRegion As MySqlDataReader
    SQLCmd = "Select locationX,locationY " +
             "From regionxml " +
             "Where regionName=" + MyDB.SQLStr(Server.UrlDecode(path(2)))
    If Trace.IsEnabled Then Trace.Warn("Places", "Get Region map location SQLCmd: " + SQLCmd.ToString())
    GetRegion = MyDB.GetReader("MySite", SQLCmd)
    If GetRegion.HasRows() Then
     GetRegion.Read()
     Body.Attributes.Add("data-region-coords-x", GetRegion("locationX").ToString())
     Body.Attributes.Add("data-region-coords-y", GetRegion("locationY").ToString())
    Else
     Body.Attributes.Add("data-region-coords-error", "true")
    End If
    GetRegion.Close()
   End If
   ShowSearch.Visible = False                               ' Search options disabled until completed

   '' Set up navigation options
   'Dim SBMenu As New TreeView
   'SBMenu.SetTrace = Trace.IsEnabled
   ''SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   ''SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   ''SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   ''SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry")        ' Javascript activated entry
   ''SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry

   'SBMenu.AddItem("B", "", "Blank Entry")
   ''SBMenu.AddItem("T", "", "Search Options")
   ''SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry") ' Javascript activated entry
   ''SBMenu.AddItem("P", "/TempSelect.aspx", "Template Selection")
   'If Trace.IsEnabled Then Trace.Warn("Places", "Show Menu")
   'SidebarMenu.InnerHtml = SBMenu.BuildMenu("Search Options", 14) ' Build and display Menu options
   'SBMenu.Close()
  End If

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
