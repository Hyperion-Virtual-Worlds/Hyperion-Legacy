
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
 '* This page is primarily for the ContactUs page or any application like it dealing with email contacts.
 '* 

 '* Built from MyWorld Contact Email Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private PageCtl As New GridLib
 Private SQLCmd, SQLFields, SQLValues As String

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

   ' Check if page is registered
   Dim drGetPage As MySql.Data.MySqlClient.MySqlDataReader
   SQLCmd = "Select PageID From PageMaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
   If Trace.IsEnabled Then Trace.Warn("Template", "Get Page Record SQLCmd: " + SQLCmd.ToString())
   drGetPage = MyDB.GetReader("MySite", SQLCmd)
   If Not drGetPage.HasRows() Then
    ' Create page entry
    SQLFields = "Name,Path"
    SQLValues = "'Template'," + MyDB.SQLStr(Request.ServerVariables("URL"))
    SQLCmd = "Insert into PageMaster (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
    If Trace.IsEnabled Then Trace.Warn("Template", "Insert Page SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    drGetPage.Close()
   End If
   drGetPage.Close()

   '' Set up navigation options
   'Dim SBMenu As New TreeView
   'SBMenu.SetTrace = Trace.IsEnabled
   'SBMenu.AddItem("M", "3", "Report List")                 ' Sub Menu entry requires number of expected entries following to contain in it
   'SBMenu.AddItem("B", "", "Blank Entry")                  ' Blank Line as item separator
   'SBMenu.AddItem("T", "", "Page Options")                 ' Title entry
   'SBMenu.AddItem("L", "CallEdit(0,'TempForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/Path/page.aspx", "Link Name")     ' Program URL link entry
   'SBMenu.AddItem("B", "", "Blank Entry")
   'SBMenu.AddItem("T", "", "Page Options")
   'SBMenu.AddItem("L", "CallEdit(0,'TempForm.aspx');", "New Entry") ' Javascript activated entry
   'SBMenu.AddItem("P", "/TempSelect.aspx", "Template Selection")
   'If Trace.IsEnabled Then Trace.Warn("Template", "Show Menu")
   'SidebarMenu.InnerHtml = SBMenu.BuildMenu("Menu Selections", 14) ' Build and display Menu options
   'SBMenu.Close()
  End If

  ' Get Display list Items here

 End Sub

 Private Sub Display()
  Dim tHtmlOut As String
  tHtmlOut = ""

  ' Update any page settings for this page
  Dim drUpdPage As MySql.Data.MySqlClient.MySqlDataReader
  SQLCmd = "Select PageID From PageMaster Where Path=" + MyDB.SQLStr(Request.ServerVariables("URL"))
  If Trace.IsEnabled Then Trace.Warn("Template", "Get Page ID SQLCmd: " + SQLCmd.ToString())
  drUpdPage = MyDB.GetReader("MySite", SQLCmd)
  If drUpdPage.Read() Then
   SQLCmd = "Update PageDetail " +
            " Set Active= Case " +
            "   When AutoStart is not null and AutoExpire is not null " +
            "   Then Case When CurDate() between AutoStart and AutoExpire Then 1 Else 0 End " +
            "   When AutoStart is not null and AutoExpire is null " +
            "   Then Case When AutoStart<=CurDate() Then 1 Else Active End " +
            "   When AutoStart is null and AutoExpire is not null " +
            "   Then Case When AutoExpire<CurDate() Then 0 Else 1 End " +
            "   End " +
            "Where (AutoStart is not null Or AutoExpire is not null) and PageID=" + MyDB.SQLNo(drUpdPage("PageID"))
   If Trace.IsEnabled Then Trace.Warn("Template", "Update Page AutoStart SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)

   ' Check for Page display content
   Dim rsPage As MySql.Data.MySqlClient.MySqlDataReader
   SQLCmd = "Select Title,Content From PageDetail " +
            "Where Active=1 and PageID=" + MyDB.SQLNo(drUpdPage("PageID")) + " " +
            "Order by SortOrder"
   If Trace.IsEnabled Then Trace.Warn("Template", "Get Page Content SQLCmd: " + SQLCmd.ToString())
   rsPage = MyDB.GetReader("MySite", SQLCmd)
   ShowContent.Visible = False
   If rsPage.HasRows() Then
    tHtmlOut = tHtmlOut +
               "     <table style=""width: 100%;"" cellpadding=""5"" cellspacing=""0""> " + vbCrLf
    While rsPage.Read()
     If rsPage("Topic").ToString().Trim().Length > 0 Then
      tHtmlOut = tHtmlOut +
               "      <tr>" + vbCrLf +
               "       <td style=""height: 40px;"" class=""TopicTitle"">" + vbCrLf +
               "        " + rsPage("Topic").ToString() + vbCrLf +
               "       </td>" + vbCrLf +
               "      </tr>" + vbCrLf
     End If
     tHtmlOut = tHtmlOut +
               "      <tr>" + vbCrLf +
               "       <td style=""height: 40px;"" class=""TopicContent"">" + vbCrLf +
               "        " + rsPage("Content").ToString() + vbCrLf +
               "       </td>" + vbCrLf +
               "      </tr>" + vbCrLf
    End While
    tHtmlOut = tHtmlOut +
               "     </table>"
   End If
   rsPage.Close()
  End If
  drUpdPage.Close()
  Content.InnerHtml = tHtmlOut.ToString()
  ShowContent.Visible = (tHtmlOut.ToString().Length > 0)

  ShowForm.Visible = False                                 ' Presume no addresses
  eID.Items.Clear()
  eID.Items.Add(New ListItem("Select Contact", "0"))
  ' Display Email Options if any
  Dim drGetEmails As MySql.Data.MySqlClient.MySqlDataReader
  SQLCmd = "Select ContactID,Title,Member " +
           "From Template " +
           "Where Active=1 " +
           "Order by Member, SortOrder"
  If Trace.IsEnabled Then Trace.Warn("Template", "Get Email Options SQLCmd: " + SQLCmd.ToString())
  drGetEmails = MyDB.GetReader("MySite", SQLCmd)
  If MyDB.Error Then
   If Trace.IsEnabled Then Trace.Warn("Template", "DB Error: " + MyDB.ErrMessage().ToString())
  End If
  If drGetEmails.HasRows() Then
   ShowForm.Visible = True
   While drGetEmails.Read()
    eID.Items.Add(New ListItem(drGetEmails("Title"), drGetEmails("ContactID")))
   End While
  End If
  drGetEmails.Close()

 End Sub

 ' Send Email
 Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs)
  Dim tEmailOut, tSMTP, tMsg As String
  tEmailOut = ""
  tSMTP = ""
  tMsg = ""

  If eID.Value = 0 Then
   tMsg = "Contact was not Selected!"
  End If
  If Email.Value.ToString().Trim().Length = 0 Then
   tMsg = "Missing your email address!"
  Else
   If Not PageCtl.ValidEmail(Email.Value) Then
    tMsg = "Email is not valid!\r\n"
   End If
  End If
  If Comment.Value.ToString().Trim().Length = 0 Then
   tMsg = "Comment is missing!"
  End If
  If tMsg.ToString().Trim().Length = 0 Then                ' Process email if no errors
   Dim drGetEmail As MySql.Data.MySqlClient.MySqlDataReader
   ' Get target email and response
   SQLCmd = "Select SendEmail,Subject," +
            " Case When AutoResponse is null Then '' Else Cast(AutoResponse as char) End as AutoResponse " +
            "From Template " +
            "Where ContactID=" + MyDB.SQLNo(eID.Value)
   If Trace.IsEnabled Then Trace.Warn("Template", "Get Target Email SQLCmd: " + SQLCmd.ToString())
   drGetEmail = MyDB.GetReader("MySite", SQLCmd)
   drGetEmail.Read()

   Dim drHost As MySql.Data.MySqlClient.MySqlDataReader
   SQLCmd = "Select Parm2 From Control Where Parm1='SMTPServer'"
   If Trace.IsEnabled Then Trace.Warn("Template", "Get SMTP Server SQLCmd: " + SQLCmd.ToString())
   drHost = MyDB.GetReader("MySite", SQLCmd)
   If drHost.HasRows() Then
    drHost.Read()
    tSMTP = drHost("Parm2").ToString().Trim()
   End If
   drHost.Close()

   If tSMTP.ToString().Trim().Length > 0 Then
    Dim SendMail As New SendEmail
    SendMail.SetTrace = Trace.IsEnabled
    SendMail.IsHTML = False
    SendMail.EmailServer = tSMTP.ToString()
    SendMail.Subject = drGetEmail("Subject").ToString().Trim()
    SendMail.ToAddress = drGetEmail("SendEmail").ToString().Trim()
    SendMail.FromAddress = Email.Value.ToString().Trim()
    SendMail.Body = Comment.Value.ToString()
    If SendMail.SendMail() Then                            ' Send was successful
     If Trace.IsEnabled Then Trace.Warn("Template", "Email sent!")
     tMsg = "Email was sent!"
     If Not drGetEmail("AutoResponse") Is DBNull.Value Then
      If drGetEmail("AutoResponse").ToString().Trim().Length > 0 Then
       SendMail.ToAddress = Email.Value.ToString().Trim()
       SendMail.FromAddress = drGetEmail("SendEmail").ToString()
       SendMail.Body = drGetEmail("AutoResponse").ToString()
       SendMail.SendMail()
      End If
     End If
     Comment.Value = ""                                    ' Clear message if sent
    Else
     If Trace.IsEnabled Then Trace.Warn("Template", "Email Failed! Error: " + SendMail.ErrMessage().ToString())
     'tMsg = "Email send failed! Please check your email address."
     tMsg = "Send Email Failed! Error: " + SendMail.ErrMessage().ToString()
    End If
    SendMail.Close()
    SendMail = Nothing
   End If
   drGetEmail.Close()
  End If
  If tMsg.ToString().Trim().Length > 0 Then
   BodyTag.Attributes.Add("onload", "ShowMsg();")          ' Activate onload option to show message
   PageCtl.AlertMessage(Me, tMsg, "ErrMsg")                ' Display Alert Message
  End If

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  PageCtl = Nothing
 End Sub
End Class
