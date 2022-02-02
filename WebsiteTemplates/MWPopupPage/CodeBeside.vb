
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
 '* This page is the general template for any called popup pages needed from any other website pages.
 '* 
 '* 

 '* Built from MyWorld Popup Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Validate logon and session existance.
  If Session.Count() = 0 Or Len(Session("UUID")) = 0 Then
   Session("Message") = "Session timeout occured! Please close this window and logon again."
   Response.Redirect("/PopupError.aspx")                   ' Display Error message
  End If
  If Request.ServerVariables("HTTPS") = "off" And Session("SSLStatus") Then ' Security is not active and is required
   Session("Message") = "Secure Access (https:) is not active! Please close this window and logon again."
   Response.Redirect("/PopupError.aspx")                   ' Display Error message
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("TempPopup", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page is called setup
   ' define local objects here

   ' Replace the oAcctWin with the actual window identifier in the calling page.
   BodyTag.Attributes.Item("onload") = "opener.oAcctWin=window;"
   BodyTag.Attributes.Item("onunload") = "opener.oAcctWin=opener;"
  End If

  ' Get Display list Items here

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub

End Class
