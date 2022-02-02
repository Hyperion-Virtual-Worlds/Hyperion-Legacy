Partial Class offline
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
 '* Offline IM processing page with email support. 
 '* Page handles Offline IM storage and email for a Halcyon based grid. 
 '* This page address is set up in the Halcyon.ini for the message processing address.
 '* OfflineMessageURL = https://MyWorld.MyWorldSrc.com/offline.aspx
 '* 

 '* Built from MyWorld Basic Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                               ' Provides data access methods and error handling
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Trace.IsEnabled = False
  If Trace.IsEnabled Then Trace.Warn("offline", "Start Page Load")

  Dim Method, RawInput, MsgStr, Message, tFilePath, ErrorMsg, tSMTPServer As String
  Dim tSMTPPort As Integer
  Dim doc As New System.Xml.XmlDocument()
  Dim root, position As System.Xml.XmlNode
  Dim tLog As Boolean

  tSMTPServer = ""
  tSMTPPort = 0
  tLog = True                                               ' Set to false to turn off file logging
  If Trace.IsEnabled And tLog Then Trace.Warn("offline", "Map Logging is active.")

  ' Get email server address
  Dim drHost As MySqlDataReader
  SQLCmd = "Select " +
           "(Select Parm2 From control Where Parm1='SMTPServer') as SMTPServer," +
           "Case When (Select Parm2 From control Where Parm1='SMTPPort') is null " +
           "Then '25' " +
           "Else (Select Parm2 From control Where Parm1='SMTPPort') " +
           "End as SMTPPort"
  ' Get the SMTPServer and SMTPPort Address for email processing.
  drHost = MyDB.GetReader("MySite", SQLCmd)
  If drHost.HasRows() Then
   drHost.Read()
   tSMTPServer = drHost("SMTPServer").ToString().Trim()
   tSMTPPort = CInt(drHost("SMTPPort"))
  End If
  drHost.Close()

  ' Log file output setup
  Dim sw As System.IO.StreamWriter
  tFilePath = Server.MapPath("").ToString()
  If Trace.IsEnabled Then Trace.Warn("offline", "File Path: " + tFilePath.ToString())
  sw = System.IO.File.AppendText(tFilePath.ToString() + "\OfflineIMLog.txt")

  If tLog Then                                              ' Log file output
   ' Trace Actions to Log file
   sw.WriteLine("Offline Page Load " + FormatDateTime(Date.Now(), DateFormat.GeneralDate))
   sw.Flush()
  End If

  '* NOTE: Headers, Form and QueryString collection do not contain any helpful contents for this process.
  Method = Request.RawUrl.ToString()                        ' Contains the command to process
  RawInput = ""
  Message = ""
  MsgStr = ""
  ErrorMsg = ""

  Dim Domain As String
  Domain = ""
  Dim drDomain As MySqlDataReader
  SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='EmailDomain'"
  If Trace.IsEnabled Then Trace.Warn("offline", "Get location SQLCmd: " + SQLCmd)
  drDomain = MyDB.GetReader("MySite", SQLCmd)
  If Trace.IsEnabled And MyDB.Error Then Trace.Warn("offline", "DB Error: " + MyDB.ErrMessage().ToString())
  If drDomain.HasRows() Then
   drDomain.Read()
   Domain = drDomain("Parm2").ToString()
  End If
  drDomain.Close()

  If tLog Then                                              ' Log file output
   ' Trace Actions to Log file
   sw.WriteLine("Called with Method = " + Method.ToString().Trim())
   sw.Flush()
  End If
  If Request.InputStream.Length > 0 Then
   If Trace.IsEnabled Then Trace.Warn("offline", "Processing XML data")
   Dim FileBytes(Request.InputStream.Length) As Byte        ' Holds uploaded binary data
   Request.InputStream.Read(FileBytes, 0, Request.InputStream.Length()) ' Fills the FileBytes with the content
   MsgStr = Encoding.UTF8.GetString(FileBytes)              ' Contains xml document content Convert to XML DOM
   MsgStr = MsgStr.ToString().Substring(MsgStr.ToString().IndexOf("<?")) ' Remove any leading content
   MsgStr = MsgStr.ToString().Substring(0, MsgStr.Length - 1) ' Remove null delimiter
   ' Load XML data into document
   Try
    doc.LoadXml(MsgStr.ToString())
   Catch ex As Exception
    ErrorMsg = "Loading Document Error - " + ex.Message.ToString()
    If tLog Then                                            ' Log file output
     ' Trace Actions to Log file
     sw.WriteLine(ErrorMsg)
     sw.Flush()
    End If
   End Try
   If doc.HasChildNodes Then                                ' For Debug processing to send in email
    If tLog Then                                            ' Log file output
     ' Trace Actions to Log file
     sw.WriteLine("XML Sent: " + vbCrLf + MsgStr.ToString())
     sw.Flush()
    End If
    Try
     If Method.ToString().Trim().Contains("/SaveMessage/") Then
      '<?xml version="1.0" encoding="utf-8"?>
      ' <GridInstantMessage xmlns:xsd="https://www.w3.org/2001/XMLSchema" xmlns:xsi="https://www.w3.org/2001/XMLSchema-instance">
      ' <fromAgentID>5c5b6edb-e393-468a-8c3c-6a62c2b1d3f2</fromAgentID>
      ' <fromAgentName>Vinhold Starbrook</fromAgentName>
      ' <toAgentID>17c99029-bb97-4efd-8b71-4f4715321d49</toAgentID>
      ' <dialog>0</dialog>
      ' <fromGroup>false</fromGroup>
      ' <message>and again.</message>
      ' <imSessionID>4b92fef2-5804-0877-074d-2525d783cebb</imSessionID>
      ' <offline>1</offline>
      ' <Position>
      '  <X>0</X>
      '  <Y>0</Y>
      '  <Z>0</Z>
      ' </Position>
      ' <binaryBucket>AA==</binaryBucket>
      ' <ParentEstateID>100</ParentEstateID>
      ' <RegionID>5cd518ed-b58f-47ac-9630-bc7b5819f1c1</RegionID>
      ' <timestamp>1421725257</timestamp>
      '</GridInstantMessage>
      root = doc.Item("GridInstantMessage")
      position = root.Item("Position")
      Message = root.Item("fromAgentID").Name + ": " + root.Item("fromAgentID").InnerText + "<br>" +
      root.Item("fromAgentName").Name + ": " + root.Item("fromAgentName").InnerText + "<br>" +
      root.Item("toAgentID").Name + ": " + root.Item("toAgentID").InnerText + "<br>" +
      root.Item("dialog").Name + ": " + root.Item("dialog").InnerText + "<br>" +
      root.Item("fromGroup").Name + ": " + root.Item("fromGroup").InnerText + "<br>" +
      root.Item("message").Name + ": " + root.Item("message").InnerText + "<br>" +
      root.Item("imSessionID").Name + ": " + root.Item("imSessionID").InnerText + "<br>" +
      root.Item("offline").Name + ": " + root.Item("offline").InnerText + "<br>" +
      position.Item("X").Name + ": " + position.Item("X").InnerText + "<br>" +
      position.Item("Y").Name + ": " + position.Item("Y").InnerText + "<br>" +
      position.Item("Z").Name + ": " + position.Item("Z").InnerText + "<br>" +
      root.Item("binaryBucket").Name + ": " + root.Item("binaryBucket").InnerText + "<br>" +
      root.Item("ParentEstateID").Name + ": " + root.Item("ParentEstateID").InnerText + "<br>" +
      root.Item("RegionID").Name + ": " + root.Item("RegionID").InnerText + "<br>" +
      root.Item("timestamp").Name + ": " + root.Item("timestamp").InnerText + "<br>"
     ElseIf Method.ToString().Trim().Contains("/RetrieveMessages/") Then
      '<?xml version="1.0" encoding="utf-8"?>
      '<UUID xmlns:xsd="https://www.w3.org/2001/XMLSchema" xmlns:xsi="https://www.w3.org/2001/XMLSchema-instance">
      '<Guid>5c5b6edb-e393-468a-8c3c-6a62c2b1d3f2</Guid>
      '</UUID>
      root = doc.Item("UUID")
      Message = root.Item("Guid").Name + ": " + root.Item("Guid").InnerText + "<br>"
     End If
    Catch ex As Exception
     ErrorMsg = "Document parsing Error - " + ex.Message.ToString()
     If tLog Then                                           ' Log file output
      ' Trace Actions to Log file
      sw.WriteLine(ErrorMsg)
      sw.Flush()
     End If
    End Try
   End If
  End If

  If ErrorMsg.ToString().Trim().Length = 0 Then             ' No errors, continue processing
   If Not Trace.IsEnabled Then
    Response.Clear()
    Response.ContentType = "text/xml"                       ' Set Mime type to xml
   End If
   If Method.ToString().Trim().Contains("/SaveMessage/") Then
    If Trace.IsEnabled Then Trace.Warn("offline", "Process Save Message")
    If tLog Then                                            ' Log file output
     ' Trace Actions to Log file
     sw.WriteLine("Process Save Message")
     sw.Flush()
    End If
    If doc.HasChildNodes Then                               ' Has content in the message
     root = doc.Item("GridInstantMessage")
     position = root.Item("Position")
     Dim tMessage As String
     tMessage = root.Item("message").InnerText.ToString().Replace("<", "&lt;")

     ' Get Message Count
     Dim GetCount As MySqlDataReader
     SQLCmd = "Select Count(toAgentId) as Count " +
              "From offlines " +
              "Where toAgentId=" + MyDB.SQLStr(root.Item("toAgentID").InnerText)
     'Message = Message.ToString() + "GetCount SQL: " + SQLCmd.ToString() + "<br>"
     GetCount = MyDB.GetReader("MyData", SQLCmd)
     If MyDB.Error() Then
      ErrorMsg = "Get message count DB Error: " + MyDB.ErrMessage.ToString()
      If tLog Then                                          ' Log file output
       ' Trace Actions to Log file
       sw.WriteLine(ErrorMsg)
       sw.Flush()
      End If
      Response.Write("<?xml version=""1.0"" encoding=""utf-8""?><boolean>false</boolean>")
     Else
      GetCount.Read()
      If tLog Then                                          ' Log file output
       ' Trace Actions to Log file
       sw.WriteLine("Message Count: " + GetCount("Count").ToString())
       sw.Flush()
      End If
      If GetCount("Count") < 51 Then                        ' Message count cap limit?
       SQLCmd = "Insert into offlines (fromAgentId, fromAgentName, toAgentId, dialogVal," +
                " fromGroupVal, offlineMessage, messageId, xPos, yPos, zPos, binaryBucket," +
                " parentEstateId, regionId, messageTimestamp, offlineVal) " +
                "Values (" +
                MyDB.SQLStr(root.Item("fromAgentID").InnerText) + "," +
                MyDB.SQLStr(root.Item("fromAgentName").InnerText) + "," +
                MyDB.SQLStr(root.Item("toAgentID").InnerText) + "," +
                MyDB.SQLNo(root.Item("dialog").InnerText) + "," +
                MyDB.SQLStr(root.Item("fromGroup").InnerText) + "," +
                MyDB.SQLStr(tMessage) + "," +
                MyDB.SQLStr(root.Item("imSessionID").InnerText) + "," +
                MyDB.SQLStr(position.Item("X").InnerText) + "," +
                MyDB.SQLStr(position.Item("Y").InnerText) + "," +
                MyDB.SQLStr(position.Item("Z").InnerText) + "," +
                MyDB.SQLStr(root.Item("binaryBucket").InnerText) + "," +
                MyDB.SQLNo(root.Item("ParentEstateID").InnerText) + "," +
                MyDB.SQLStr(root.Item("RegionID").InnerText) + "," +
                MyDB.SQLNo(root.Item("timestamp").InnerText) + "," +
                MyDB.SQLNo(root.Item("offline").InnerText) + ")"
       'Message = Message.ToString() + "Insert Offlines SQL: " + SQLCmd.ToString() + "<br>"
       MyDB.DBCmd("MyData", SQLCmd)
       If MyDB.Error() Then                                 ' Return Procesing result to sending process.
        ErrorMsg = "Save message DB Error: " + MyDB.ErrMessage.ToString() + vbCrLf + vbCrLf + "SQLCmd: " + SQLCmd.ToString()
        If tLog Then                                        ' Log file output
         ' Trace Actions to Log file
         sw.WriteLine(ErrorMsg)
         sw.Flush()
        End If
        Response.Write("<?xml version=""1.0"" encoding=""utf-8""?><boolean>false</boolean>")
       Else
        If tLog Then                                        ' Log file output
         ' Trace Actions to Log file
         sw.WriteLine("DB write succeeded.")
         sw.Flush()
        End If
        Response.Write("<?xml version=""1.0"" encoding=""utf-8""?><boolean>true</boolean>")
       End If
       ' Process posting the offline message to email here.
       If ErrorMsg.ToString().Trim().Length = 0 Then
        Dim GetUser As MySqlDataReader
        SQLCmd = "Select username,lastname,email," +
                 " Case When (Select recv_ims_via_email From userpreferences Where user_id=users.UUID) is not null " +
                 " Then" +
                 "  (Select recv_ims_via_email From userpreferences Where user_id=users.UUID) " +
                 " Else " +
                 "  Cast(1 as unsigned) " +
                 " End as ImInEmail " +
                 "From users " +
                 "Where UUID=" + MyDB.SQLStr(root.Item("toAgentID").InnerText)
        Message = Message.ToString() + "Get Users SQL: " + SQLCmd.ToString() + "<br>"
        GetUser = MyDB.GetReader("MyData", SQLCmd)
        If MyDB.Error() Then
         ErrorMsg = "Get Users DB Error: " + MyDB.ErrMessage.ToString()
         If tLog Then                                       ' Log file output
          ' Trace Actions to Log file
          sw.WriteLine(ErrorMsg)
          sw.Flush()
         End If
        Else
         GetUser.Read()
         If tLog Then                                       ' Log file output
          ' Trace Actions to Log file
          sw.WriteLine("Saved message from " + root.Item("fromAgentName").InnerText + " to " + GetUser("username").ToString().Trim() + " " + GetUser("lastname").ToString().Trim() + ".")
          sw.Flush()
         End If
         If GetUser("ImInEmail") = 1 Then                   ' Respect user IM to Email Preference.
          If tMessage.ToString().Trim().Length > 0 Then     ' Must have a message content
           ' Send message as email
           Dim email As New SendEmail
           email.EmailServer = tSMTPServer.ToString()
           If tSMTPPort > 0 Then
            email.Port = tSMTPPort
           End If
           email.FromAddress = "My World Mail <mailer@" + Domain.ToString() + ">"
           email.ToAddress = GetUser("username").ToString().Trim() + " " + GetUser("lastname").ToString().Trim() + "<" + GetUser("email").ToString().Trim() + ">"
           email.Subject = "Message From My World"
           email.Body = "<br>" + GetUser("username").ToString().Trim() + " " + GetUser("lastname").ToString().Trim() + ", " +
                        "you have a message from <b>" + root.Item("fromAgentName").InnerText.ToString().Trim() + "</b>.<br><br>" +
                        "<b><i>" + tMessage.ToString() + "</i></b>"
           email.IsHTML = True
           If email.SendMail() Then
            If tLog Then                                    ' Log file output
             ' Trace Actions to Log file
             sw.WriteLine("Offline Email was sent.")
             sw.Flush()
            End If
           Else
            If tLog Then                                    ' Log file output
             ' Trace Actions to Log file
             sw.WriteLine("Offline Email send failed! " + email.ErrMessage())
             sw.Flush()
            End If
           End If
           email.Close()
           email = Nothing
           ' Need to set up how to post a temp hmailserver account to handle return posts to sender.
           ' https://www.hmailserver.com/documentation/latest/?page=overview (General Documentation)
           'NOTE: it is possible to create a script for hMailServer to handle return email messages to GW accounts. See
           ' https://www.hmailserver.com/documentation/latest/?page=reference_scripts
           ' The idea would be that if set, it would exist for up to 15-25 minutes from an email being sent out to being able to accept 
           ' a return email message and convert it into an IM posted to the sender in GW. This would require converting the email
           ' content into an IM message and posting it to the MyData DB and removing the email. Any other incoming emails would just be removed.
           ' https://www.hmailserver.com/documentation/latest/?page=com_object_database (Database Object operations)

          End If
         End If

         ' Attempt to post an out going email using the hMailServer API
         ' https://www.hmailserver.com/documentation/v5.4/?page=com_object_message (Send a message process)
         'Dim oMessage As Object
         'Try
         ' oMessage = CreateObject("hMailServer.Message")

         'Catch ex As Exception
         ' ErrorMsg = "hMailServer Object Failed: " + ex.Message().ToString()

         'End Try
         'If ErrorMsg.ToString().Trim().Length > 0 Then
         ' 'oMessage.From = root.Item("fromAgentName").InnerText.ToString()
         ' 'oMessage.FromAddress = root.Item("fromAgentName").InnerText.ToString().Trim() + "[" + root.Item("fromAgentID").InnerText.ToString() + "@gospellearningcenter.com]"
         ' 'oMessage.Subject = "Message From My World"
         ' 'oMessage.AddRecipient("", GetUser("email").ToString().Trim())
         ' 'oMessage.Body = GetUser("username").ToString().Trim() + " " + GetUser("lastname").ToString().Trim() + " " + _
         ' '                "you have a message from " + root.Item("fromAgentName").InnerText.ToString().Trim() + "." + vbCrLf + vbCrLf + _
         ' '                root.Item("message").InnerText.ToString().Trim()
         ' 'oMessage.Save()
         ' oMessage = Nothing
         'End If
        End If
        GetUser.Close()
       End If
      End If
     End If
     GetCount.Close()
    Else
     Response.Write("<?xml version=""1.0"" encoding=""utf-8""?><boolean>false</boolean>")
     If tLog Then                                           ' Log file output
      ' Trace Actions to Log file
      sw.WriteLine("Message did not have anything in it! Failed to save.")
      sw.Flush()
     End If
    End If
   ElseIf Method.ToString().Trim().Contains("/RetrieveMessages/") Then ' Get stored messages to send back
    If tLog Then                                            ' Log file output
     ' Trace Actions to Log file
     sw.WriteLine("Get stored messages to send on.")
     sw.Flush()
    End If
    If doc.HasChildNodes Then                               ' Has content in the message
     If Trace.IsEnabled Then Trace.Warn("offline", "Process get messages")
     root = doc.Item("UUID")

     Dim ToName As String
     ToName = "Unknown"
     Dim GetName As MySqlDataReader
     SQLCmd = "Select Concat(username,' ',lastname) as Name " +
              "From users Where UUID=" + MyDB.SQLStr(root.Item("Guid").InnerText)
     Message = Message.ToString() + "Get Offline messages SQL: " + SQLCmd.ToString() + "<br>"
     GetName = MyDB.GetReader("MyData", SQLCmd)
     GetName.Read()
     ToName = GetName("Name").ToString().Trim()
     GetName.Close()

     Dim GetIMs As MySqlDataReader
     SQLCmd = "Select fromAgentId, fromAgentName, toAgentId, dialogVal," +
              " fromGroupVal, offlineMessage, messageId, xPos, yPos, zPos, binaryBucket," +
              " parentEstateId, regionId, messageTimestamp, offlineVal " +
              "From offlines " +
              "Where toAgentId=" + MyDB.SQLStr(root.Item("Guid").InnerText)
     Message = Message.ToString() + "Get Offline messages SQL: " + SQLCmd.ToString() + "<br>"
     GetIMs = MyDB.GetReader("MyData", SQLCmd)
     If MyDB.Error() Then
      ErrorMsg = "Get offline messages DB Error: " + MyDB.ErrMessage.ToString()
      If tLog Then                                          ' Log file output
       ' Trace Actions to Log file
       sw.WriteLine(ErrorMsg)
       sw.Flush()
      End If
     Else
      Dim XMLOut As String
      Dim Count As Integer
      Count = 0
      XMLOut = "<?xml version=""1.0"" encoding=""utf-8""?><ArrayOfGridInstantMessage xmlns:xsi=""https://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""https://www.w3.org/2001/XMLSchema"">"
      If GetIMs.HasRows() Then
       While GetIMs.Read()
        Count = Count + 1
        XMLOut = XMLOut.ToString() +
                 "<GridInstantMessage xmlns:xsd=""https://www.w3.org/2001/XMLSchema"" xmlns:xsi=""https://www.w3.org/2001/XMLSchema-instance"">" +
                 "<fromAgentID>" + GetIMs("fromAgentId").ToString().Trim() + "</fromAgentID>" +
                 "<fromAgentName>" + GetIMs("fromAgentName").ToString().Trim() + "</fromAgentName>" +
                 "<toAgentID>" + GetIMs("toAgentId").ToString().Trim() + "</toAgentID>" +
                 "<dialog>" + GetIMs("dialogVal").ToString() + "</dialog>" +
                 "<fromGroup>" + GetIMs("fromGroupVal").ToString().Trim() + "</fromGroup>" +
                 "<message>" + GetIMs("offlineMessage").ToString().Trim() + "</message>" +
                 "<imSessionID>" + GetIMs("messageId").ToString().Trim() + "</imSessionID>" +
                 "<offline>" + GetIMs("offlineVal").ToString() + "</offline>" +
                 "<Position>" +
                 "<X>" + GetIMs("xPos").ToString().Trim() + "</X>" +
                 "<Y>" + GetIMs("yPos").ToString().Trim() + "</Y>" +
                 "<Z>" + GetIMs("zPos").ToString().Trim() + "</Z>" +
                 "</Position>" +
                 "<binaryBucket>" + GetIMs("binaryBucket").ToString().Trim() + "</binaryBucket>" +
                 "<ParentEstateID>" + GetIMs("parentEstateId").ToString() + "</ParentEstateID>" +
                 "<RegionID>" + GetIMs("regionId").ToString().Trim() + "</RegionID>" +
                 "<timestamp>" + GetIMs("messageTimestamp").ToString() + "</timestamp>" +
                 "</GridInstantMessage>"
       End While
      End If
      XMLOut = XMLOut.ToString() +
               "</ArrayOfGridInstantMessage>"
      Response.Write(XMLOut.ToString())
      If tLog Then                                          ' Log file output
       ' Trace Actions to Log file
       sw.WriteLine(Count.ToString() + " Saved messages delivered for " + ToName.ToString() + ".")
       sw.Flush()
      End If
      Message = Message.ToString() + "Return XML: " + XMLOut.ToString() + "<br>"
      If GetIMs.HasRows() Then
       ' Remove saved messages for this agent
       SQLCmd = "Delete From offlines " +
                "Where toAgentId=" + MyDB.SQLStr(root.Item("Guid").InnerText)
       Message = Message.ToString() + "Remove Offline messages SQL: " + SQLCmd.ToString() + "<br>"
       MyDB.DBCmd("MyData", SQLCmd)
       If MyDB.Error() Then
        ErrorMsg = "Remove offline messages DB Error: " + MyDB.ErrMessage.ToString()
        If tLog Then                                        ' Log file output
         ' Trace Actions to Log file
         sw.WriteLine(ErrorMsg)
         sw.Flush()
        End If
       End If
      End If
     End If
     GetIMs.Close()
    End If
   End If
  End If

  If ErrorMsg.ToString().Trim().Length > 0 Then             ' Email to me the error notice 
   If Trace.IsEnabled Then Trace.Warn("offline", "Send email data")
   If Message.ToString().Length > 0 Then
    Message = Message.ToString() + "<br><br> " + ErrorMsg.ToString()
   End If
   '' Debugging information when needed.
   'RawInput = Request.ContentLength.ToString() + "<br>" + _
   '           Request.ContentType.ToString() + "<br>"
   ' The Params collection is the entire ServerVariables list.
   'RawInput = RawInput.ToString() + _
   '           Request.HttpMethod.ToString() + "<br>" + _
   '           "Params Key Count: " + Request.Params.Count.ToString() + "<br>"
   'If Request.Params.HasKeys Then
   ' For Key = 0 To Request.Params.Count - 1
   '  RawInput = RawInput.ToString() + Request.Params.GetKey(Key).ToString() + ", " + Request.Params.Item(Key).ToString() + "<br>"
   ' Next
   'End If
   'RawInput = RawInput.ToString() + _
   '           "InputStream Length: " + Request.InputStream.Length.ToString() + "<br>"
   RawInput = RawInput.ToString() +
              "Message Raw: " + MsgStr.ToString() + "<br>" +
              "XML: " + Message.ToString() + "<br>" +
              "Total Bytes: " + Request.TotalBytes.ToString() + "<br> "

   Dim email As New SendEmail
   email.EmailServer = tSMTPServer.ToString()
   If tSMTPPort > 0 Then
    email.Port = tSMTPPort
   End If
   email.FromAddress = "My World Mail <mailer@" + Domain.ToString() + ">"
   email.ToAddress = "director@" + Domain.ToString()
   email.Subject = "Grid Offline IM Processing Error"
   email.Body = "Offline message was sent:<br><br>" +
                "Method: " + Method.ToString().Trim() + "<br><br>" +
                RawInput.ToString().Trim() + " <br> "
   If tLog Then                                             ' Log file output
    ' Trace Actions to Log file
    sw.WriteLine("Grid Offline IM Processing Error message was sent:" + vbCrLf + vbCrLf +
                 "Method: " + Method.ToString().Trim() + vbCrLf + vbCrLf +
                 RawInput.ToString().Trim() + vbCrLf)
    sw.Flush()
   End If
   email.IsHTML = True
   If email.SendMail() Then
    If tLog Then                                            ' Log file output
     ' Trace Actions to Log file
     sw.WriteLine("Email was sent.")
     sw.Flush()
    End If
   Else
    If tLog Then                                            ' Log error message.
     ' Trace Actions to Log file
     sw.WriteLine("Email send failed! " + email.ErrMessage())
     sw.Flush()
    End If
   End If
   email.Close()
   email = Nothing
  End If
  If tLog Then                                              ' Log file output
   ' Trace Actions to Log file
   sw.WriteLine("--Grid Offline IM Processing Completed!--" + vbCrLf)
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
