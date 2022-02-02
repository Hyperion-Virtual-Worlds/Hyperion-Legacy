Partial Class ViewerDownloads
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
 '* Viewer Downloads Content display page. Use the Administration Web Content Management page to provide any
 '* display content about your world management team etc. for this page.
 '* Contributed by Emperor Starfinder
 '* 

 '* Built from MyWorld Basic Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private SQLCmd, SQLFields, SQLValues As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("ViewerDownloads", "Start Page Load")

  If IsNothing(Session("SSLStatus")) Then
   Session("SSLStatus") = False
   ' Force SSL active if it is not and is required.
   If Request.ServerVariables("HTTPS") = "off" Then          ' Security is not active and is required
    If Trace.IsEnabled Then Trace.Warn("ViewerDownloads", "Https is off")
    Dim drServer As MySqlDataReader
    SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='ServerLocation'"
    If Trace.IsEnabled Then Trace.Warn("ViewerDownloads", "Get location SQLCmd: " + SQLCmd)
    drServer = MyDB.GetReader("MySite", SQLCmd)
    If drServer.Read() Then
     If Trace.IsEnabled Then Trace.Warn("ViewerDownloads", "ServerLocation = " + drServer("Parm2").ToString().Trim().ToUpper())
     If drServer("Parm2").ToString().Trim().ToUpper() = "LIVE" Then ' Security must be on if not testing
      drServer.Close()
      Session("SSLStatus") = True
      Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + "/ViewerDownloads.aspx")
     End If
    Else                                                     ' show error if not located
     If Trace.IsEnabled Then Trace.Warn("ViewerDownloads", "Set https error: <br>Control value for 'Server Location' (ServerLocation) was not defined!")
     Session("ErrorMessage") = "Control value for 'Server Location' (ServerLocation) was not defined!"
     Response.Redirect("/Error.aspx")
    End If
    drServer.Close()
   End If
  ElseIf Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then          ' Security is not active and is required
   Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + "/ViewerDownloads.aspx")
  End If

  If Not IsPostBack Then                                    ' First time page is called setup

   ' define local objects here
   Dim tHtmlOut As String
   tHtmlOut = ""

   ' Check if page is registered
   Dim drGetPage As MySqlDataReader
   SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
   If Trace.IsEnabled Then Trace.Warn("ViewerDownloads", "Get Page Record Check: " + SQLCmd)
   drGetPage = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ViewerDownloads", "DB Error: " + MyDB.ErrMessage().ToString())
   If Not drGetPage.HasRows() Then
    drGetPage.Close()
    ' Create page entry
    SQLFields = "Name,Path"
    SQLValues = "'Viewer Downloads'," + MyDB.SQLStr(Request.ServerVariables("URL"))
    SQLCmd = "Insert into pagemaster (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
    If Trace.IsEnabled Then Trace.Warn("Schools", "Insert Page record: " + SQLCmd)
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ViewerDownloads", "DB Error: " + MyDB.ErrMessage().ToString())
    ' Reload PageID
    SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
    If Trace.IsEnabled Then Trace.Warn("Schools", "Reload Page RecordID: " + SQLCmd)
    drGetPage = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("ViewerDownloads", "DB Error: " + MyDB.ErrMessage().ToString())
   End If

   If drGetPage.Read() Then
    SQLCmd = "Update pagedetail " +
             " Set Active= " +
             "  Case " +
             "   When AutoStart is not null and AutoExpire is null " +
             "   Then Case When AutoStart<=CurDate() Then 'true' Else Active End " +
             "   When AutoStart is null and AutoExpire is not null " +
             "   Then Case When AutoExpire<CurDate() Then 'false' Else Active End " +
             "   When AutoStart is not null and AutoExpire is not null " +
             "   Then Case When CurDate() between AutoStart and AutoExpire Then 'true' " +
             "   Else Case When AutoStart<=CurDate() Then 'false' Else Active End " +
             "   End " +
             "  End " +
             "Where (AutoStart is not null Or AutoExpire is not null) and PageID=" + MyDB.SQLNo(drGetPage("PageID"))
    If Trace.IsEnabled Then Trace.Warn("ViewerDownloads", "Update Page AutoStart: " + SQLCmd)
    MyDB.DBCmd("MySite", SQLCmd)

    ' Check for Page display content
    Dim rsPage As MySqlDataReader
    SQLCmd = "Select Title,Content From pagedetail " +
             "Where Active=1 and PageID=" + MyDB.SQLNo(drGetPage("PageID")) + " " +
             "Order by SortOrder"
    If Trace.IsEnabled Then Trace.Warn("ViewerDownloads", "Get Page Content: " + SQLCmd)
    rsPage = MyDB.GetReader("MySite", SQLCmd)
    If rsPage.HasRows() Then
     tHtmlOut = tHtmlOut +
               "     <table style=""width: 100%;"" cellpadding=""5"" cellspacing=""0""> " + vbCrLf
     While rsPage.Read()
      If rsPage("Title").ToString().Trim().Length > 0 Then
       tHtmlOut = tHtmlOut +
                  "      <tr>" + vbCrLf +
                  "       <td style=""height: 20px;"" class=""TopicTitle"">" + vbCrLf +
                  "        " + rsPage("Title").ToString() + vbCrLf +
                  "       </td>" + vbCrLf +
                  "      </tr>" + vbCrLf
      End If
      tHtmlOut = tHtmlOut +
                 "      <tr>" + vbCrLf +
                 "       <td class=""TopicContent"">" + vbCrLf +
                 "        " + rsPage("Content").ToString() + vbCrLf +
                 "       </td>" + vbCrLf +
                 "      </tr>" + vbCrLf
     End While
     tHtmlOut = tHtmlOut +
                "     </table>"
    End If
    rsPage.Close()
   End If
   drGetPage.Close()
   ShowContent.InnerHtml = tHtmlOut.ToString()

   ' Setup general page controls

   If Not (Session.Count() = 0 Or Len(Session("UUID")) = 0) Then  ' Allow menu access while logged in
    Dim SBMenu As New TreeView
    ' Sidebar Options control based on Clearance or Write Access
    ' Set up navigation options
    If Trace.IsEnabled Then SBMenu.SetTrace = True
    'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
    'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
    'SBMenu.AddItem("T", "", "Other Options")                ' Title entry
    'SBMenu.AddItem("L", "CallEdit(0,'TempAddEdit.aspx');", "New Entry")        ' Javascript activated entry
    'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
    SBMenu.AddItem("P", "Account.aspx", "My Account")
    SBMenu.AddItem("P", "Logout.aspx", "Logout")
    If Session("Access") > 1 Then
     SBMenu.AddItem("B", "", "Blank Entry")
     SBMenu.AddItem("T", "", "Page Options")
     SBMenu.AddItem("P", "/Administration/Admin.aspx", "Website Administration")
    End If
    If Trace.IsEnabled Then Trace.Warn("ViewerDownloads", "Show Menu")
    SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
    SBMenu.Close()
   End If
  End If

  ' Get Display list Items here

 End Sub

    Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
        ' Close open page objects
        MyDB.Close()
        MyDB = Nothing
    End Sub
End Class
