
Partial Class BuyThanks
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
 '* This page is primaily intended for content managed display by an administrative process as part 
 '* of the whole website.
 '*
 '* Built from MyWorld Content Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private PageCtl As New GridLib
 Private SQLCmd, SQLFields, SQLValues As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("BuyThanks", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page is called setup
   ' Define process unique objects here

   ' Define local objects here
   ' Setup general page controls

   ' Check if page is registered
   Dim drGetPage As MySqlDataReader
   SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
   If Trace.IsEnabled Then Trace.Warn("BuyThanks", "Get Page Record SQLCmd: " + SQLCmd.ToString())
   drGetPage = MyDB.GetReader("MySite", SQLCmd)
   If Not drGetPage.HasRows() Then
    ' Create page entry
    SQLFields = "Name,Path"
    SQLValues = "'BuyThanks'," + MyDB.SQLStr(Request.ServerVariables("URL"))
    SQLCmd = "Insert into pagemaster (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
    If Trace.IsEnabled Then Trace.Warn("BuyThanks", "Insert Page SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    drGetPage.Close()
   End If
   drGetPage.Close()


   '' Set up navigation options
   'Dim SBMenu As New TreeView
   'SBMenu.SetTrace = Trace.IsEnabled
   ''SBMenu.AddItem("M", "3", "Report List")                ' Sub Menu entry requires number of expected entries following to contain in it
   ''SBMenu.AddItem("B", "", "Blank Entry")                 ' Blank Line as item separator
   ''SBMenu.AddItem("T", "", "Page Options")                ' Title entry
   ''SBMenu.AddItem("L", "CallEdit(0,'TempForm.aspx');", "New Entry") ' Javascript activated entry
   ''SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")    ' Program URL link entry

   'SBMenu.AddItem("L", "CallEdit(0,'TempForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/TempSelect.aspx", "BuyThanks Selection")
   'If Trace.IsEnabled Then Trace.Warn("BuyThanks", "Show Menu")
   'SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   'SBMenu.Close()

   Display()
  End If

  ' Get Display list Items here

 End Sub

 Private Sub Display()
  Dim tHtmlOut As String
  tHtmlOut = ""

  ' Update any page settings for this page
  Dim drUpdPage As MySqlDataReader
  SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
  If Trace.IsEnabled Then Trace.Warn("BuyThanks", "Get Page ID SQLCmd: " + SQLCmd.ToString())
  drUpdPage = MyDB.GetReader("MySite", SQLCmd)
  If drUpdPage.Read() Then
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
            "Where (AutoStart is not null Or AutoExpire is not null) and PageID=" + MyDB.SQLNo(drUpdPage("PageID"))
   If Trace.IsEnabled Then Trace.Warn("BuyThanks", "Update Page AutoStart SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)

   ' Check for Page display content
   Dim rsPage As MySqlDataReader
   SQLCmd = "Select Title,Content From pagedetail " +
            "Where Active=1 and PageID=" + MyDB.SQLNo(drUpdPage("PageID")) + " " +
            "Order by SortOrder"
   If Trace.IsEnabled Then Trace.Warn("BuyThanks", "Get Page Content SQLCmd: " + SQLCmd.ToString())
   rsPage = MyDB.GetReader("MySite", SQLCmd)
   ShowContent.Visible = False
   If rsPage.HasRows() Then
    tHtmlOut = tHtmlOut + _
               "     <table style=""width: 100%;"" cellpadding=""5"" cellspacing=""0""> " + vbCrLf
    While rsPage.Read()
     If rsPage("Title").ToString().Trim().Length > 0 Then
      tHtmlOut = tHtmlOut + _
               "      <tr>" + vbCrLf + _
               "       <td style=""height: 40px;"" class=""TopicTitle"">" + vbCrLf + _
               "        " + rsPage("Title").ToString() + vbCrLf + _
               "       </td>" + vbCrLf + _
               "      </tr>" + vbCrLf
     End If
     tHtmlOut = tHtmlOut + _
               "      <tr>" + vbCrLf + _
               "       <td style=""height: 40px;"" class=""TopicContent"">" + vbCrLf + _
               "        " + rsPage("Content").ToString() + vbCrLf + _
               "       </td>" + vbCrLf + _
               "      </tr>" + vbCrLf
    End While
    tHtmlOut = tHtmlOut + _
               "     </table>"
   End If
   rsPage.Close()
  End If
  drUpdPage.Close()
  Content.InnerHtml = tHtmlOut.ToString()
  ShowContent.Visible = (tHtmlOut.ToString().Length > 0)

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub
End Class
