Partial Class map
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
 '* Map tile processing page support to the viewer world map view and any website map pages.
 '* 
 '* 

 '* Built from MyWorld Basic Page template v. 1.0

 '* It is intended for use with the Anaximander https://github.com/kf6kjg/Anaximander2 map tile processing 
 '* program running in Windows or linux server for use with Halcyon based grids.
 '* This page is set up in the Halcyon.ini using the [SimulatorFeatures], MapImageServerURI = 
 '* "http://domain/map.aspx". Put in your own domain and path to the file.
 '* This page presumes it is in the same location as the /maptiles folder, but that may be changed as long as there is 
 '* a path to that folder accsessible to this page.
 '**************************************
 '* At end of this page is an option to activate writing process results to a log file: MapLog.txt
 '*

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("Map", "Start Page Load")

  ' Define process unique objects here

  ' Get command line content to parse the image or coordinates sent to this page:
  ' Requests appear to be /map-z-x-y-objects.jpg where z is the zoom level and x,y are the grid coordinates.
  ' Or x=,y=,z= as get parms, where x, y are grid coordinates and z is the zoom level.
  Dim tRequest, InParm(), x, y, z, tLogOut, tURLPath, tFilePath, tFile As String
  tRequest = ""
  x = ""
  y = ""
  z = ""
  tLogOut = ""
  tURLPath = ""                      ' Place your URL path to where this page is placed relative to the root.

  If Len(Request.QueryString("x")) = 0 And Len(Request.QueryString("y")) = 0 And Len(Request.QueryString("z")) = 0 Then
   tRequest = Request.ServerVariables("PATH_INFO").ToString().Replace(tURLPath.ToString() + "map.aspx/", "")
   If Trace.IsEnabled Then Trace.Warn("Map", "Passed by URL: " + tRequest.ToString())
   tLogOut = "Request: " + tRequest.ToString()
   ' Get image name parsed
   InParm = tRequest.ToString().Split("-")
   z = InParm(1)
   x = InParm(2)
   y = InParm(3)
  Else
   If Trace.IsEnabled Then Trace.Warn("Map", "Passed values: " + Request.QueryString().ToString())
   x = Request.QueryString("x")
   y = Request.QueryString("y")
   z = Request.QueryString("z")
  End If
  ' After parsing out the request, locate the actual image in the maptiles folder returning it else send ocean.jpg.
  If Trace.IsEnabled Then Trace.Warn("Map", "Values: x=" + x.ToString() + ", y=" + y.ToString() + ", z=" + z.ToString())
  ' Check if file exists
  tFilePath = Server.MapPath(tURLPath.ToString().Replace("/", "\") + "maptiles\").ToString()
  tFile = "map-" + z.ToString() + "-" + x.ToString() + "-" + y.ToString() + "-objects.jpg"
  If Trace.IsEnabled Then Trace.Warn("Map", "Selected file: " + tFilePath.ToString() + tFile.ToString())
  If System.IO.File.Exists(tFilePath + tFile) Then
   tLogOut = tLogOut.ToString() + ", sent back " + tFilePath.ToString() + tFile.ToString()
   If Not Trace.IsEnabled Then
    Response.Clear()
    Response.ContentType = "image/jpg"
    Response.WriteFile(tFilePath + tFile)
   End If
  Else                                                ' Return 307 redirect to the ocean map tile so the page can use cache loaded entry
   tLogOut = tLogOut.ToString() + ", Redirect Location: /maptiles/ocean.jpg"
   If Trace.IsEnabled Then Trace.Warn("Map", "File not found: Sent ocean.jpg")
   If Not Trace.IsEnabled Then
    Response.Clear()
    Response.StatusCode = 307
    Response.AddHeader("Location", "/maptiles/ocean.jpg")
   End If
  End If

  If False Then                                       ' Set true to turn on process logging
   If Trace.IsEnabled Then Trace.Warn("Map", "Map Logging is active.")
   Dim sw As System.IO.StreamWriter
   tFilePath = Server.MapPath(tURLPath.ToString().Replace("/", "\")).ToString()
   If Trace.IsEnabled Then Trace.Warn("Map", "File Path: " + tFilePath.ToString())
   ' Trace Actions to Log file
   sw = System.IO.File.AppendText(tFilePath.ToString() + "\MapLog.txt")
   sw.WriteLine(tLogOut)
   sw.Flush()
   sw.Close()
  End If
  Response.End()

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
 End Sub
End Class
