
Partial Class Administration_ListSelect
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
 '* Website Control Lists display for entry selection and navigation of the collections of content. 
 '* 
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
  If Session("Access") <> 9 Then                            ' SysAdmin Only access
   Response.Redirect("Admin.aspx")
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("ListSelect", "Start Page Load")

  If Not IsPostBack Then                                    ' First time page is called setup
   ' Define process unique objects here

   ' Define local objects here
   Session("DBConnection") = "MySite"                       ' Define Database connection for this program set
   Session("MaxLevel") = 0                                  ' Maximum number of allowed member levels. Zero = no limit
   If Session("FirstTime") = "" Then
    Session("FirstTime") = "ADMINLISTS"
    Session("SelPage") = 0                                  ' Value is updated in the Page index change event
    Session("ParentName") = ""
    Session("ParentCtl") = ""
    Session("ListControl") = "ADMINLISTS"                   ' Highest level parent
    Session("Level") = 0
   End If

   ' Setup general page controls

   ' Gridview settings
   gvDisplay.PageIndex = Session("SelPage")
   gvDisplay.PageSize = 40
  End If

  If Len(Request.Form("List")) > 0 Then                     ' New Member list selected
   If Request.Form("List") = Session("ParentCtl") Then
    Session("Level") = Session("Level") - 1                 ' Decrement Active Level
   Else
    Session("Level") = Session("Level") + 1                 ' Increment Active Level
   End If
   Session("FirstTime") = Request.Form("List")
   gvDisplay.PageIndex = 0                                  ' Reset paging index to first page
  End If

  If Session("FirstTime") = Session("ListControl") Then    ' Is at the top of the list chain
   If Trace.IsEnabled Then Trace.Warn("ListSelect", "Top level settings")
   Session("ParentName") = ""
   Session("ParentCtl") = ""
   PageTitle.InnerHtml = "Control List Select"
   Session("Level") = 0
  Else                                                      ' Show parent level
   If Trace.IsEnabled Then Trace.Warn("ListSelect", "Parent Level Parm1: " + Session("FirstTime").ToString())
   ' Get parent level Control and name
   Dim drGetName As MySqlDataReader
   SQLCmd = "Select Control,Name From control " +
            "Where Parm1=" + MyDB.SQLStr(Session("FirstTime"))
   If Trace.IsEnabled Then Trace.Warn("ListSelect", "Get parent level and name SQLCmd= " + SQLCmd)
   drGetName = MyDB.GetReader(Session("DBConnection"), SQLCmd)
   If drGetName.HasRows() Then
    drGetName.Read()
    Session("ParentName") = Trim(drGetName("Name"))         ' Start parent child levels with master parent info
    Session("ParentCtl") = Trim(drGetName("Control"))
   End If
   drGetName.Close()
   PageTitle.InnerHtml = "Members in " +
            "<span onclick=" + Chr(34) + "SetList('" + Session("ParentCtl") + "');" + Chr(34) + " title=" + Chr(34) + "Open Parent List" + Chr(34) +
            " onMouseOver=" + Chr(34) + "this.className='NOverLink'" + Chr(34) +
            " onMouseOut=" + Chr(34) + "this.className='NavLink';" + Chr(34) + " class=" + Chr(34) + "NavLink" + Chr(34) + ">" +
            Session("ParentName") + "</span>"
  End If

  Dim SBMenu As New TreeView
  ' Sidebar Options control based on Clearance or Write Access
  SBMenu.SetTrace = Trace.IsEnabled
  'SBMenu.AddItem("M", "3", "Report List")                   ' Sub Menu entry requires number of expected entries following to contain in it
  'SBMenu.AddItem("B", "", "Blank Entry")                    ' Blank Line as item separator
  'SBMenu.AddItem("T", "", "Other Options")                  ' Title entry
  'SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry")        ' Javascript activated entry
  'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")       ' Program URL link entry
  If Session("FirstTime") <> Session("ListControl") Then
   SBMenu.AddItem("L", "SetList('" + Session("ParentCtl") + "');", "Parent Level")
  End If
  SBMenu.AddItem("P", "Admin.aspx", "Website Administration")
  SBMenu.AddItem("P", "/Account.aspx", "Account")
  SBMenu.AddItem("P", "/Logout.aspx", "Logout")
  SBMenu.AddItem("B", "", "Blank Entry")
  SBMenu.AddItem("T", "", "Page Options")
  SBMenu.AddItem("L", "CallEdit('','ListForm.aspx');", "New Member")
  If Trace.IsEnabled Then Trace.Warn("ListSelect", "Show Menu")
  SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
  SBMenu.Close()

  ' Get Display list Items here
  Dim Display As MySqlDataReader
  SQLCmd = "Select Name,Parm1 " +
           "From control " +
           "Where Control=" + MyDB.SQLStr(Session("FirstTime")) + " " +
           "Order by Name"
  If Trace.IsEnabled Then Trace.Warn("ListSelect", "Get SQLCmd: " + SQLCmd.ToString())
  ' SqlDataSource1 is a screen data object placed on the page.
  Display = MyDB.GetReader(Session("DBConnection"), SQLCmd)
  gvDisplay.DataSource = Display
  gvDisplay.DataBind()
  If Not IsPostBack Then
   If gvDisplay.PageCount > 1 Then
    ShowPages.InnerText = "Page " + (gvDisplay.PageIndex + 1).ToString() + " of " + gvDisplay.PageCount.ToString()
   Else
    ShowPages.InnerText = " "
   End If
  End If

 End Sub

 Protected Sub gvDisplay_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvDisplay.RowDataBound
  'If Trace.IsEnabled Then Trace.Warn("ListSelect", "RowType= " + e.Row.RowType.ToString())
  If e.Row.RowType = DataControlRowType.DataRow Then        ' Process only for Data rows
   'If Trace.IsEnabled Then Trace.Warn("ListSelect", "Row processed ")
   If Len(Trim(DataBinder.Eval(e.Row.DataItem, "Parm1"))) = 0 Then ' Do not show Member option on blank Parm1
    e.Row.Cells(1).Visible = False
   Else                                                     ' May use next member level
    If Session("MaxLevel") > 0 Then                         ' Maximum levels is set
     If Session("Level") < Session("MaxLevel") Then         ' Not exceeded the Maximum level
      e.Row.Cells(1).Visible = True
     Else
      e.Row.Cells(1).Visible = False
     End If
    Else
     e.Row.Cells(1).Visible = True
    End If
   End If
  End If
  'If Trace.IsEnabled Then Trace.Warn("ListSelect", "Databound Level setting: " + Session("Level").ToString() + "/" + Session("MaxLevel").ToString())
  If Trace.IsEnabled Then Trace.Warn("ListSelect", "DB PageIndex=" + gvDisplay.PageIndex.ToString())
 End Sub

 Protected Sub gvDisplay_PageIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvDisplay.PageIndexChanged
  ShowPages.InnerText = "Page " + (gvDisplay.PageIndex + 1).ToString() + " of " + gvDisplay.PageCount.ToString()
  Session("SelPage") = gvDisplay.PageIndex                  ' Remember current page displayed.
 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  SqlClient.SqlConnection.ClearAllPools()      ' Causes all pooled connection to be forced closed when finished, reducing pooled connection size.
 End Sub

End Class
