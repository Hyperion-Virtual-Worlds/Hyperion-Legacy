Public Class LandTool
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
 '* The LandTool page provide world simulator support for account verification for land purchase or 
 '* transfer. This page requires website support to redirect any reference to landtool.php to redirect 
 '* to this page. Since viewers as of 2/27/2017 still have the page hardcoded to landtool.php.

 '* Built from MyWorld Basic Page template v. 1.0
 '*************
 '* This page handles the HTTP function calls from the viewer to verify and complete land purchases for the InWorldz Halcyon server.
 '* It provides the xml response to the viewer when called for the two functions:
 '* buy_land_prep(method_name, params, app_data)
 '* buy_land(method_name, params, app_data)
 '*************
 '* Viewer First Pass sent:
 '*<?xml version="1.0"?>
 '*<methodCall>
 '* <methodName>preflightBuyLandPrep</methodName>
 '* <params>
 '*  <param>
 '*   <value>
 '*    <struct>
 '*     <member>
 '*      <name>agentId</name>
 '*      <value>
 '*       <string>17c99029-bb97-4efd-8b71-4f4715321d49</string>
 '*      </value>
 '*     </member>
 '*     <member>
 '*      <name>secureSessionId</name>
 '*      <value>
 '*       <string>9a035d61-1414-b19c-3c16-7cd9f56bd9e1</string>
 '*      </value>
 '*     </member>
 '*     <member>
 '*      <name>language</name>
 '*      <value>
 '*       <string>en</string>
 '*      </value>
 '*     </member>
 '*     <member>
 '*      <name>billableArea</name>
 '*      <value>
 '*       <int>0</int>
 '*      </value>
 '*     </member>
 '*     <member>
 '*      <name>currencyBuy</name>
 '*      <value>
 '*       <int>0</int>
 '*      </value>
 '*     </member>
 '*    </struct>
 '*   </value>
 '*  </param>
 '* </params>
 '*</methodCall> 


 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private SQLCmd As String
 Private Logging As Boolean

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("Landtool", "Start Page Load")

  Dim RawInput, MsgStr, Message, ErrorMsg As String
  Dim doc As New System.Xml.XmlDocument()
  Dim root, methodName As System.Xml.XmlNode
  Dim params As System.Xml.XmlNode

  Dim tMsg, tLogOut, tFilePath, resp As String
  Logging = True                                             ' Activate or disable action logging to file
  tLogOut = ""
  tMsg = ""

  '* NOTE: Headers, Form and QueryString collection do not contain any helpful contents for this process.
  RawInput = ""
  Message = ""
  MsgStr = ""
  ErrorMsg = ""
  resp = "<?xml version=""1.0""?>" +
   "<methodResponse>" +
    "<params>" +
     "<param>" +
      "<value>" +
       "<struct>" +
        "<member><name>success</name><value><boolean>0</boolean></value></member>" +
        "<member><name>errorMessage</name><value><string>Some error...</string></value></member>" +
        "<member><name>errorURI</name><value><string>" + IIf(Request.ServerVariables("HTTPS") = "on", "https://", "http://") + Request.ServerVariables("HTTP_HOST") + "</string></value></member>" +
       "</struct>" +
      "</value>" +
     "</param>" +
    "</params>" +
   "</methodResponse>"

  If Request.InputStream.Length > 0 Then
   Dim FileBytes(Request.InputStream.Length) As Byte        ' Holds uploaded binary data
   Request.InputStream.Read(FileBytes, 0, Request.InputStream.Length()) ' Fills the FileBytes with the content
   MsgStr = Encoding.UTF8.GetString(FileBytes)              ' Contains xml document content Convert to XML DOM
   MsgStr = MsgStr.Substring(MsgStr.IndexOf("<?"))          ' Remove any leading content
   MsgStr = MsgStr.Substring(0, MsgStr.Length - 1)          ' Remove null delimiter
   tLogOut = tLogOut.ToString() + "XML Received: " + vbCrLf + MsgStr.ToString() + vbCrLf

   ' Load XML data into document
   Try
    doc.LoadXml(MsgStr)
   Catch ex As Exception
    tMsg = "LandTool had an exception parsing the incoming XML"
    tLogOut = tLogOut.ToString() + tMsg.ToString() + "!" + vbCrLf + vbCrLf
   End Try

   ' XML RPC handler
   If doc.HasChildNodes Then
    root = doc("methodCall")
    methodName = root("methodName")
    params = root("params")

    Select Case methodName.InnerText
     Case Is = "preflightBuyLandPrep"
      'tMsg = "preflightBuyLandPrep: exec buy_land_prep(" +
      '       params.SelectNodes("./param/value/struct/member[name='agentId']/value/string")(0).InnerText + "," +
      '       params.SelectNodes("./param/value/struct/member[name='secureSessionId']/value/string")(0).InnerText + "," +
      '       Convert.ToDouble(params.SelectNodes("./param/value/struct/member[name='billableArea']/value/int")(0).InnerText).ToString() + "," +
      '       Convert.ToInt32(params.SelectNodes("./param/value/struct/member[name='currencyBuy']/value/int")(0).InnerText).ToString() + ")"
      'tLogOut = tLogOut.ToString() + tMsg.ToString() + vbCrLf + vbCrLf
      resp = buy_land_prep(
       params.SelectNodes("./param/value/struct/member[name='agentId']/value/string")(0).InnerText,
       params.SelectNodes("./param/value/struct/member[name='secureSessionId']/value/string")(0).InnerText,
       Convert.ToDouble(params.SelectNodes("./param/value/struct/member[name='billableArea']/value/int")(0).InnerText),
       Convert.ToInt32(params.SelectNodes("./param/value/struct/member[name='currencyBuy']/value/int")(0).InnerText)
      )
      'tLogOut = tLogOut.ToString() + "Reponse: " + resp.ToString() + vbCrLf + vbCrLf

     Case Else
      ' Dunno what that call is, report to admin.
      tMsg = "LandTool page was sent:" + vbCrLf + vbCrLf + MsgStr + vbCrLf + "Method: " + methodName.InnerText.ToString()
      tLogOut = tLogOut.ToString() + tMsg.ToString() + vbCrLf
    End Select
   Else
    tMsg = "LandTool got XML, but it had no content."
    tLogOut = tLogOut.ToString() + tMsg.ToString() + vbCrLf
   End If
  Else
   tMsg = "LandTool was not sent any content."
   tLogOut = tLogOut.ToString() + tMsg.ToString() + vbCrLf
  End If
  tMsg = "LandTool process complete."
  tLogOut = tLogOut.ToString() + tMsg.ToString() + vbCrLf

  If Logging Then                                           ' Set true to turn on process logging
   If Trace.IsEnabled Then Trace.Warn("LandTool", "Logging is active.")
   Dim sw As System.IO.StreamWriter
   tFilePath = Server.MapPath("").ToString()
   If Trace.IsEnabled Then Trace.Warn("LandTool", "File Path: " + tFilePath.ToString())
   ' Trace Actions to Log file
   sw = System.IO.File.AppendText(tFilePath.ToString() + "\LandToolLog.txt")
   sw.WriteLine(tLogOut)
   sw.Flush()
   sw.Close()
  End If

  If Not Trace.IsEnabled Then
   Response.Clear()
   Response.ContentType = "text/xml"                        ' Set Mime type to xml
   Response.Write(resp)
   'Response.Close()
  End If

 End Sub

 ' Process if done as a XMLRPC web service
 Public Function buy_land_prep(ByVal agentID As String, ByVal sessionID As String, ByVal Amount As Double, ByVal billableArea As Int32) As String

  Dim tMsg, tFilePath, tLogOut, tOut As String
  Dim ipAddress, ErrorMsg As String
  ipAddress = Request.ServerVariables("REMOTE_ADDR")
  ErrorMsg = ""
  tLogOut = ""
  tMsg = ""

  ' Verify User UUID
  Dim GetUUID As MySqlDataReader
  SQLCmd = "Select UUID " +
           "From agents " +
           "Where UUID=" + MyDB.SQLStr(agentID)
  If Trace.IsEnabled Then Trace.Warn("Landtool", "Agent SQLCmd: " + SQLCmd.ToString())

  'Message = Message.ToString() + "GetCount SQL: " + SQLCmd.ToString() + "<br>"
  GetUUID = MyDB.GetReader("MyData", SQLCmd)
  If MyDB.Error() Then
   ErrorMsg = "Get UUID DB Error: " + MyDB.ErrMessage.ToString()
   If Trace.IsEnabled Then Trace.Warn("Landtool", "Error message return XML")
   tOut = "<?xml version=""1.0"" encoding=""utf-8""?>" +
          "<params>" +
          " <param>" +
          "  <value>" +
          "   <struct>" +
          "    <member>" +
          "     <name>success</name>" +
          "     <value>" +
          "      <boolean>0</boolean>" +
          "     </value>" +
          "    </member>" +
          "    <member>" +
          "     <name>errorMessage</name>" +
          "     <value>" +
          "      <string>&#10;&#10;Unable to authenticate&#10;Bummer!&#10;&#10;Click URL for more info.</string>" +
          "     </value>" +
          "    </member>" +
          "    <member>" +
          "     <name>errorURI</name>" +
          "     <value>" +
          "      <string>" + IIf(Request.ServerVariables("HTTPS") = "on", "https://", "http://") + Request.ServerVariables("HTTP_HOST") + "/Default.aspx</string>" +
          "     </value>" +
          "    </member>" +
          "   </struct>" +
          "  </value>" +
          " </param>" +
          "</params>"


  Else                                                      ' Format validation response
   ErrorMsg = "No error, Agent UUID validated."
   tOut = "<?xml version=""1.0"" encoding=""utf-8""?>" +
          "<params>" +
          " <param>" +
          "  <value>" +
          "   <struct>" +
          "    <member>" +
          "     <name>success</name>" +
          "     <value>" +
          "      <boolean>1</boolean>" +
          "     </value>" +
          "    </member>" +
          "    <member>" +
          "     <name>currency</name>" +
          "     <value>" +
          "      <struct>" +
          "       <member>" +
          "        <name>estimatedCost</name>" +
          "        <value>" +
          "         <double>" + Amount.ToString() + "</double>" +
          "        </value>" +
          "       </member>" +
          "      </struct>" +
          "     </value>" +
          "    </member>" +
          "    <member>" +
          "     <name>membership</name>" +
          "     <value>" +
          "      <struct>" +
          "       <member>" +
          "        <name>upgrade</name>" +
          "        <value>" +
          "         <boolean>0</boolean>" +
          "        </value>" +
          "       </member>" +
          "       <member>" +
          "        <name>action</name>" +
          "        <value>" +
          "         <string>Not affect your account.</string>" +
          "        </value>" +
          "       </member>" +
          "       <member>" +
          "        <name>levels</name>" +
          "        <value>" +
          "         <array>" +
          "          <data>" +
          "           <value>" +
          "            <struct>" +
          "             <member>" +
          "              <name>id</name>" +
          "              <value>" +
          "               <string>00000000-0000-0000-0000-000000000000</string>" +
          "              </value>" +
          "             </member>" +
          "             <member>" +
          "              <name>description</name>" +
          "              <value>" +
          "               <string>some level</string>" +
          "              </value>" +
          "             </member>" +
          "            </struct>" +
          "           </value>" +
          "          </data>" +
          "         </array>" +
          "        </value>" +
          "       </member>" +
          "      </struct>" +
          "     </value>" +
          "    </member>" +
          "    <member>" +
          "     <name>landUse</name>" +
          "     <value>" +
          "      <struct>" +
          "       <member>" +
          "        <name>upgrade</name>" +
          "        <value>" +
          "         <boolean>0</boolean>" +
          "        </value>" +
          "       </member>" +
          "       <member>" +
          "        <name>action</name>" +
          "        <value>" +
          "         <string>Not affect any My World fees.</string>" +
          "        </value>" +
          "       </member>" +
          "      </struct>" +
          "     </value>" +
          "    </member>" +
          "    <member>" +
          "     <name>confirm</name>" +
          "     <value>" +
          "      <string></string>" +
          "     </value>" +
          "    </member>" +
          "   </struct>" +
          "  </value>" +
          " </param>" +
          "</params>"
  End If
  tMsg = "* Parms: agentID: " + agentID.ToString() + ", sessionID: " + sessionID.ToString() + ", Amount:" + Amount.ToString() + ", billableArea: " + billableArea.ToString() + vbCrLf +
         "* Error: " + ErrorMsg.ToString() + vbCrLf +
         "* Reponse: " + tOut.ToString().Replace(" ", "")
  tLogOut = tLogOut.ToString() + "buy_land_prep parms sent and results: " + vbCrLf + tMsg.ToString() + vbCrLf

  If Logging Then                                           ' Set true to turn on process logging
   If Trace.IsEnabled Then Trace.Warn("LandTool", "Logging is active.")
   Dim sw As System.IO.StreamWriter
   tFilePath = Server.MapPath("").ToString()
   If Trace.IsEnabled Then Trace.Warn("LandTool", "File Path: " + tFilePath.ToString())
   ' Trace Actions to Log file
   sw = System.IO.File.AppendText(tFilePath.ToString() + "\LandToolLog.txt")
   sw.WriteLine(tLogOut)
   sw.Flush()
   sw.Close()
  End If

  Return tOut.ToString()
 End Function

 ' Apparently this function is never called by a viewer. Not sure what it was intended to do!
 Public Function buy_land(ByVal method_name As String, ByVal params As Array, ByVal app_data As String) As String

  Dim tMsg, tFilePath, tLogOut, tOut As String
  Dim agentID, sessionID, ipAddress, ErrorMsg As String
  Dim Amount, Real As Double
  Dim billableArea As Integer
  ipAddress = Request.ServerVariables("REMOTE_ADDR")
  ErrorMsg = ""
  tLogOut = ""
  tMsg = ""
  agentID = params(0)
  sessionID = params(1)
  Amount = params(2)
  Real = params(3)
  billableArea = params(4)
  ipAddress = params(5)

  tOut = "<?xml version=""1.0"" encoding=""utf-8""?>"
  tLogOut = tLogOut.ToString() + "Function buy_land(" + method_name.ToString() + ", [" + agentID.ToString() + "," + sessionID.ToString() + "," +
            Amount.ToString() + "," + Real.ToString() + "," + billableArea.ToString() + "," + ipAddress.ToString() + "], " +
            app_data.ToString() + ")" + vbCrLf

  If Logging Then                                           ' Set true to turn on process logging
   If Trace.IsEnabled Then Trace.Warn("LandTool", "Logging is active.")
   Dim sw As System.IO.StreamWriter
   tFilePath = Server.MapPath("").ToString()
   If Trace.IsEnabled Then Trace.Warn("LandTool", "File Path: " + tFilePath.ToString())
   ' Trace Actions to Log file
   sw = System.IO.File.AppendText(tFilePath.ToString() + "\LandToolLog.txt")
   sw.WriteLine(tLogOut)
   sw.Flush()
   sw.Close()
  End If

  Return tOut.ToString()
 End Function

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
