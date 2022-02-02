
Partial Class Administration_GridPgPop
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
  If Session("Access") <> 9 Then                           ' SysAdmin Only access
   BodyTag.Attributes.Add("onload", "window.close();")     ' Activate onload option to close window
  End If

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("GridPgPop", "Start Page Load")

  If Not IsPostBack Then                                   ' First time page is called setup
   ' define local objects here

   ' Get identity key
   If Len(Request("KeyID")) > 0 Then                       ' Update or Add Mode
    KeyID.Value = Request("KeyID")
   Else
    BodyTag.Attributes.Add("onload", "opener.location.reload(true); window.close();")     ' Activate onload option to close window
   End If
   If KeyID.Value.ToString() = "WelcomeRegion" Then
    TagName.InnerText = "Welcome Region Name:"
    PageTitle.InnerText = "Welcome Region Setup"
   Else
    TagName.InnerText = "Home Region Name:"
    PageTitle.InnerText = "Home Region Setup"
   End If
   ' Populate Region selection list
   Dim RegionList As MySqlDataReader
   SQLCmd = "Select regionName From regionxml Order by regionName"
   If Trace.IsEnabled Then Trace.Warn("GridPgPop", "Region Selections: " + SQLCmd.ToString())
   RegionList = MyDB.GetReader("MySite", SQLCmd)
   If RegionList.HasRows() Then
    While RegionList.Read()
     Regions.Items.Add(RegionList("regionName").ToString())
    End While
   End If
   RegionList.Close()
   ' Get existing assigned region name
   Dim drControl As MySqlDataReader
   SQLCmd = "Select Parm2 " +
            "From control " +
            "Where Control='GRIDSETUP' and Parm1=" + IIf(KeyID.Value.ToString() = "WelcomeRegion", "'WelcomeRegion'", "'HomeRegion'")
   If Trace.IsEnabled Then Trace.Warn("GridPgPop", "Get display Values SQLCmd: " + SQLCmd.ToString())
   drControl = MyDB.GetReader("MySite", SQLCmd)
   drControl.Read()
   If drControl("Parm2").ToString().Trim().Length > 0 Then
    Regions.SelectedValue = drControl("Parm2").ToString().Trim()
   End If
   drControl.Close()

   ' Replace the oAcctWin with the actual window identifier in the calling page.
   BodyTag.Attributes.Item("onload") = "opener.oPageWin=window;"
    BodyTag.Attributes.Item("onunload") = "opener.oPageWin=opener;"
   End If

 End Sub

 ' Update Button clicked
 Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
  If KeyID.Value.ToString() = "WelcomeRegion" Then
   SQLCmd = "Update control Set Parm2=" + MyDB.SQLStr(Regions.SelectedValue) + " " +
            "Where Control='GRIDSETUP' and Parm1='WelcomeRegion'"
  Else
   SQLCmd = "Update control Set Parm2=" + MyDB.SQLStr(Regions.SelectedValue) + " " +
            "Where Control='GRIDSETUP' and Parm1='HomeRegion'"
  End If
  If Trace.IsEnabled Then Trace.Warn("GridPgPop", "Get display Values SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error() Then Trace.Warn("GridPgPop", "DB Error: " + MyDB.ErrMessage().ToString())
  BodyTag.Attributes.Add("onload", "opener.location.reload(true); window.close();")     ' Activate onload option to close window
 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
