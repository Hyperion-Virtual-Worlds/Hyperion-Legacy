
Partial Class RegXML
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
 '* This page provides the region.xml data for any requested region for Halcyon region instance.
 '* It expects the parameter ?Region=<regionName> 
 '* 
 '* Built from Website Basic Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  Dim tLogOut, tFilePath, Port As String
  tLogOut = ""
  Port = ""
  Dim tLog As Boolean
  tLog = True

  ' Log file output setup
  Dim sw As System.IO.StreamWriter
  tFilePath = Server.MapPath("").ToString()
  sw = System.IO.File.AppendText(tFilePath.ToString() + "\RegXML.log")

  If tLog Then                                             ' Log file output
   sw.WriteLine("RegXML Start: " + FormatDateTime(Date.Now(), DateFormat.GeneralDate))
   sw.Flush()
  End If

  If CInt(Request("Port")) > 0 Then                         ' Get passed Port Number
   Port = Request("Port")
   If tLog Then                                            ' Log file output
    sw.WriteLine("Region Port given: " + Port.ToString())
    sw.Flush()
   End If
   ' Get Region Data by Name
   Dim GetRegion As MySqlDataReader
   SQLCmd = "Select UUID,regionName,locationX,locationY,port,externalIP,ownerUUID,lastmapUUID," +
            "lastmapRefresh,primMax,productType " +
            "From regionxml " +
            "Where port=" + MyDB.SQLStr(Port)
   If tLog Then                                            ' Log file output
    sw.WriteLine("Get regionxml values SQLCmd: " + SQLCmd.ToString())
    sw.Flush()
   End If
   GetRegion = MyDB.GetReader("MySite", SQLCmd)
   If MyDB.Error() And tLog Then                                           ' Log file output
    sw.WriteLine("DB Error: " + MyDB.ErrMessage().ToString())
    sw.Flush()
   End If
   If GetRegion.HasRows() Then
    GetRegion.Read()
    ' Setup output as xml
    Dim resp As String
    resp = "<Regions>" +
            "<Root>" +
             "<Config " +
              "sim_UUID=""" + GetRegion("UUID").ToString() + """ " +
              "sim_name=""" + GetRegion("regionName").ToString() + """ " +
              "sim_location_x=""" + GetRegion("locationX").ToString() + """ " +
              "sim_location_y=""" + GetRegion("locationY").ToString() + """ " +
              "internal_ip_address=""0.0.0.0"" " +
              "internal_ip_port=""" + GetRegion("port").ToString() + """ " +
              "allow_alternate_ports=""false"" " +
              "external_host_name=""" + GetRegion("externalIP").ToString() + """ " +
              "master_avatar_uuid=""" + GetRegion("ownerUUID").ToString() + """ " +
              "lastmap_uuid=""" + GetRegion("lastmapUUID").ToString() + """ " +
              "lastmap_refresh=""" + GetRegion("lastmapRefresh").ToString() + """ " +
              "nonphysical_prim_max=""" + GetRegion("primMax").ToString() + """ " +
              "physical_prim_max=""0"" " +
              "clamp_prim_size=""false"" " +
              "object_capacity=""0"" " +
              "region_product=""" + GetRegion("productType").ToString() + """ " +
              "region_access=""0"" " +
              "outside_ip="""" " +
             "/>" +
            "</Root>" +
           "</Regions>"
    Response.Clear()
    Response.ContentType = "text/xml"                      ' Set Mime type to xml
    Response.Write(resp)
    If tLog Then                                           ' Log file output
     sw.WriteLine("XML: " + resp)
     sw.Flush()
    End If
   End If
  Else
   If tLog Then                                            ' Log file output
    sw.WriteLine("No Port number was provided.")
    sw.Flush()
   End If
  End If

  If tLog Then                                             ' Log file output
   sw.WriteLine("********* Completed Processing *********" + vbCrLf)
   sw.Flush()
  End If
  sw.Close()

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
