Partial Class Connect
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
 '* After User account creation, this page provides how to set up a viewer to access the world. 
 '* Information is managed in the Website Content Management Admin pages.
 '* 
 '* 

 '* Built from MyWorld Content Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private SQLCmd, SQLFields, SQLValues As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("Connect", "Start Page Load")

  If Not IsPostBack Then                                    ' First time page is called setup
   Dim tHtmlOut As String
   tHtmlOut = ""

   ' Setup general page controls
   ' define local objects here

   ' Check if page is registered
   Dim drGetPage As MySqlDataReader
   SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
   If Trace.IsEnabled Then Trace.Warn("Connect", "Get Page Record Check: " + SQLCmd)
   drGetPage = MyDB.GetReader("MySite", SQLCmd)
   If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Connect", "DB Error: " + MyDB.ErrMessage().ToString())
   If Not drGetPage.HasRows() Then
    drGetPage.Close()
    ' Create page entry
    SQLFields = "Name,Path"
    SQLValues = "'Connect'," + MyDB.SQLStr(Request.ServerVariables("URL"))
    SQLCmd = "Insert into pagemaster (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
    If Trace.IsEnabled Then Trace.Warn("Connect", "Insert Page record: " + SQLCmd)
    MyDB.DBCmd("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Connect", "DB Error: " + MyDB.ErrMessage().ToString())
    ' Reload PageID
    SQLCmd = "Select PageID From pagemaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
    If Trace.IsEnabled Then Trace.Warn("Schools", "Reload Page RecordID: " + SQLCmd)
    drGetPage = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("Connect", "DB Error: " + MyDB.ErrMessage().ToString())
   End If

   If drGetPage.Read() Then
    SQLCmd = "Update pagedetail " +
             " Set Active= Case " +
             "  When AutoStart is not null and AutoExpire is not null " +
             "  Then Case When CurDate() between AutoStart and AutoExpire Then 1 Else 0 End " +
             "  When AutoStart is not null and AutoExpire is null " +
             "  Then Case When AutoStart<=CurDate() Then 1 Else Active End " +
             "  When AutoStart is null and AutoExpire is not null " +
             "  Then Case When AutoExpire<CurDate() Then 0 Else 1 End " +
             "  End " +
             "Where (AutoStart is not null Or AutoExpire is not null) and PageID=" + MyDB.SQLNo(drGetPage("PageID"))
    If Trace.IsEnabled Then Trace.Warn("Connect", "Update Page AutoStart: " + SQLCmd)
    MyDB.DBCmd("MySite", SQLCmd)

    ' Check for Page display content
    Dim rsPage As MySqlDataReader
    SQLCmd = "Select Title,Content From pagedetail " +
             "Where Active=1 and PageID=" + MyDB.SQLNo(drGetPage("PageID")) + " " +
             "Order by SortOrder"
    If Trace.IsEnabled Then Trace.Warn("Connect", "Get Page Content: " + SQLCmd)
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
  End If
 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
