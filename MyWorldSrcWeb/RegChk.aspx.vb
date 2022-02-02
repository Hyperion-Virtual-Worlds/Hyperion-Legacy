
Partial Class RegChk
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

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("RegChk", "Start Page Load")

  ' Page takes in a port number. If assigned to a region all is well ErrorLevel = 0
  ' If port is not assigned, ErrorLevel=1
  ' If no DB access ErrorLevel = 2

  Environment.ExitCode = 0
  Dim PortNum As Integer
  ' Get Port number
  If Len(Request("Port")) > 0 Then                         ' Update or Add Mode
   PortNum = CInt(Request("Port"))
  End If
  If PortNum > 0 Then
   Dim GetReg As MySqlDataReader
   SQLCmd = "Select regionName " +
            "From regionxml " +
            "Where port=" + MyDB.SQLNo(PortNum)
   If Trace.IsEnabled Then Trace.Warn("RegChk", "Reload Page RecordID: " + SQLCmd)
   Try
    GetReg = MyDB.GetReader("MySite", SQLCmd)
    If Trace.IsEnabled And MyDB.Error Then Trace.Warn("RegChk", "DB Error: " + MyDB.ErrMessage().ToString())
    If GetReg.HasRows() Then
     GetReg.Read()
     If Trace.IsEnabled Then Trace.Warn("RegChk", "Region Assigned: " + GetReg("regionName").ToString().Trim())
     Response.Write("0")
    Else
     If Trace.IsEnabled Then Trace.Warn("RegChk", "Port was not assigned!")
     Response.Write("1")                              ' Port was not assigned!
    End If

   Catch ex As Exception
    If Trace.IsEnabled Then Trace.Warn("RegChk", "No DB Access!")
    Response.Write("2")                               ' No DB Access!

   End Try
  Else
   If Trace.IsEnabled Then Trace.Warn("RegChk", "No port number was sent!")
   Response.Write("9")                                    ' No port number sent!
  End If

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
