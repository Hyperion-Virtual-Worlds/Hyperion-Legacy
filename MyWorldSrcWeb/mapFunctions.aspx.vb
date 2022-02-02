
Partial Class mapFunctions
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
 '* This page provides functions used in the Places and /Administration/WorldMap pages. Called by JavaScript functions.
 '* 
 '* Built from Website Basic Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  ' Get passed values
  Dim Action, tOutput, tFilePath As String
  tOutput = ""
  Dim tLog As Boolean
  tLog = False

  ' Log file output setup
  Dim sw As System.IO.StreamWriter

  Action = Request("action")                               ' Get what action is to be done

  If tLog Then                                             ' Log file output
   tFilePath = Server.MapPath("").ToString()
   sw = System.IO.File.AppendText(tFilePath.ToString() + "\mapFunctions.log")
   sw.WriteLine("mapFunctions Start: " + FormatDateTime(Date.Now(), DateFormat.GeneralDate))
   sw.WriteLine("mapFunctions Sent: Action=" + Action.ToString())
   sw.Flush()
  End If

  If Action.ToString().Trim().Length <> 0 Then
   If Action.ToString().Trim = "getRegionByCoordinates" Then ' Return all region display information
    Dim intX, intY As Integer
    intX = CInt(Request("coordX"))
    intY = CInt(Request("coordY"))

    ' Get Actual MyData DB name
    Dim DBName As String
    DBName = "MyData"
    Dim Control As MySqlDataReader
    SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='MyDataDBName'"
    If tLog Then                                            ' Log file output
     sw.WriteLine("Get control values SQLCmd: " + SQLCmd.ToString())
     sw.Flush()
    End If
    Control = MyDB.GetReader("MySite", SQLCmd)
    If MyDB.Error() Then                                           ' Show Error
     sw.WriteLine("DB Error: " + MyDB.ErrMessage().ToString())
     sw.Flush()
    End If
    If Control.HasRows() Then
     Control.Read()
     DBName = Control("Parm2").Trim().ToString()
    End If
    Control.Close()

    ' Anonymous type class to handle content for JSON output
    Dim output = New With {
     .regionName = "",
     .SLURL = "",
     .maturityRating = "",
     .avatars = "",
     .WriteJSON = Function(this As Object) As String
                   Dim tOut As String
                   tOut = Chr(34) + "regionName" + Chr(34) + ":" + Chr(34) + this.regionName.ToString() + Chr(34) + "," +
                   Chr(34) + "SLURL" + Chr(34) + ":" + Chr(34) + this.SLURL.ToString() + Chr(34) + "," +
                   Chr(34) + "maturityRating" + Chr(34) + ":" + Chr(34) + this.maturityRating.ToString() + Chr(34) + "," +
                   Chr(34) + "avatars" + Chr(34) + ":" + this.avatars.ToString()
                   Return "{" + tOut.ToString() + "}"
                  End Function
    }

    ' Get Region Data by Name
    Dim GetRegion As MySqlDataReader
    SQLCmd = "Select A.UUID,A.regionName,B.UserLocationX,B.UserLocationY,B.UserLocationZ,C.maturity," +
             " Case When D.regionName is null Then 'Offline' Else 'Online' End as Status " +
             "From regionxml A inner join " + DBName.ToString() + ".land B on A.UUID=B.RegionUUID " +
             " Inner Join " + DBName.ToString() + ".regionsettings C on A.UUID=C.regionUUID " +
             " Left Outer Join " + DBName.ToString() + ".regions D on A.UUID=D.UUID " +
             "Where A.locationX=" + MyDB.SQLNo(intX) + " and A.locationY=" + MyDB.SQLNo(intY)
    If tLog Then                                            ' Log file output
     sw.WriteLine("Get region values SQLCmd: " + SQLCmd.ToString())
     sw.Flush()
    End If
    GetRegion = MyDB.GetReader("MySite", SQLCmd)
    ' Used as a general purpose service to display region data. Send back as JSON data.
    If MyDB.Error() Then                                           ' Show Error
     If tLog Then                                            ' Log file output
      sw.WriteLine("DB Error: " + MyDB.ErrMessage().ToString())
      sw.Flush()
     End If
     ' { "status": "not found" } 
     tOutput = "{" + JSONPair("status", "not found") + "}"
    Else
     If GetRegion.HasRows() Then
      GetRegion.Read()
      If GetRegion("Status") = "Offline" Then
       ' {"RegName":"Delphi","Error":"REGION IS OFFLINE!"}
       tOutput = "{" + JSONPair("RegName", GetRegion("regionName").ToString().Trim()) + "," +
                 JSONPair("Error", "REGION IS OFFLINE!") + "}"
      Else
       Dim LocX, LocY, LocZ As Double
       If GetRegion("UserLocationX") = 0 And GetRegion("UserLocationX") = 0 Then ' Attempt default center landing
        LocX = 128
        LocY = 128
        LocZ = 20
       Else                                                    ' Use set landing point
        LocX = GetRegion("UserLocationX")
        LocY = GetRegion("UserLocationY")
        LocZ = GetRegion("UserLocationZ")
       End If
       Dim MR(2), temp As String
       MR(0) = "G"
       MR(1) = "M"
       MR(2) = "A"

       ' {"RegName":"Delphi","SLURL":"secondlife://Delphi/112/92/25"}
       output.regionName = GetRegion("regionName").ToString().Trim()
       output.SLURL = "secondlife://" + GetRegion("regionName").ToString().Trim() +
             "/" + LocX.ToString() + "/" + LocY.ToString() + "/" + LocZ.ToString()
       output.maturityRating = MR(GetRegion("maturity"))
       temp = ""
       ' Get list of avatar locations in the region
       Dim GetAvatars As MySqlDataReader
       SQLCmd = "Select currentPos From agents Where agentOnline>0 and currentRegion=" + MyDB.SQLStr(GetRegion("UUID"))
       If tLog Then                                            ' Log file output
        sw.WriteLine("Get agents SQLCmd: " + SQLCmd.ToString())
        sw.Flush()
       End If
       GetAvatars = MyDB.GetReader("MyData", SQLCmd)
       If GetAvatars.HasRows() Then
        Dim XYZ() As String
        temp = ""
        While GetAvatars.Read()
         XYZ = GetAvatars("currentPos").ToString().Replace("<", "").Replace(">", "").Split(",")
         If temp.ToString().Length > 0 Then
          temp = temp + ","
         End If
         temp = temp + "{" + JSONPair("location", "{" + JSONPair("x", XYZ(0)) + "," + JSONPair("y", XYZ(1)) + "}") + "}"
        End While
       End If
       GetAvatars.Close()
       output.avatars = "[" + temp + "]"
       tOutput = output.WriteJSON(output)
      End If
     Else
      ' { "status": "not found" } 
      tOutput = "{" + JSONPair("status", "not found") + "}"
     End If
    End If
    If tLog Then                                            ' Log file output
     sw.WriteLine("Response: " + tOutput.ToString())
     sw.Flush()
    End If
    Response.ContentType = "application/json"
    Response.Write(tOutput)
   ElseIf Action.ToString().Trim = "getSearch" Then    ' Return Region Name, list of avatar locations, Maturity rating
    Dim LookWhere, LookFor As String
    LookWhere = Request("LookWhere")
    LookFor = Request("LookFor")
    If LookWhere = "places" Then                    ' Search Places for locations
     Dim Search As MySqlDataReader
     SQLCmd = "Select Name,Description,SnapshotUUID,regionName,UserLocationX,UserLocationY,UserLocationZ " +
              "From " +
              " ((Select B.Name,B.Description,B.SnapshotUUID,A.regionName,B.UserLocationX,B.UserLocationY,B.UserLocationZ " +
              "   From regions A Inner Join land B on A.UUID=B.RegionUUID " +
              "   Where A.UUID in " +
              "    (Select RegionID From estate_map Where EstateID in " +
              "     (Select EstateID From estate_settings Where EstateName like " + MyDB.SQLStr(LookFor) + "))) " +
              "  Union " +
              "  (Select B.Name,B.Description,B.SnapshotUUID,A.regionName,B.UserLocationX,B.UserLocationY,B.UserLocationZ " +
              "   From regions A Inner Join land B on A.UUID=B.RegionUUID " +
              "   Where A.regionName like " + MyDB.SQLStr(LookFor) + ") " +
              "  Union " +
              "  (Select B.Name,B.Description,B.SnapshotUUID,A.regionName,B.UserLocationX,B.UserLocationY,B.UserLocationZ " +
              "   From regions A Inner Join land B on A.UUID=B.RegionUUID " +
              "   Where B.Name like " + MyDB.SQLStr(LookFor) + ")) as RawData  "
     If Trace.IsEnabled Then Trace.Warn("Places", "Get Region map location SQLCmd: " + SQLCmd.ToString())
     If tLog Then                                            ' Log file output
      sw.WriteLine("Get Region map locations SQLCmd: " + SQLCmd.ToString())
      sw.Flush()
     End If
     Search = MyDB.GetReader("MyData", SQLCmd)
     If Search.HasRows() Then
      While Search.Read()
       ' Package up data elements for JSON return
      End While
     Else
     End If
     Search.Close()
    Else                                                  ' Search Events for locations
     Dim Search As MySqlDataReader
     SQLCmd = "Select Name,Description,SnapshotUUID,regionName,UserLocationX,UserLocationY,UserLocationZ " +
              "From events " +
              "Where Name like " + MyDB.SQLStr(LookFor)
     If tLog Then                                            ' Log file output
      sw.WriteLine("Get event locations SQLCmd: " + SQLCmd.ToString())
      sw.Flush()
     End If
     Search = MyDB.GetReader("MySite", SQLCmd)
     If Search.HasRows() Then
      While Search.Read()
       ' Package up data elements for JSON return
      End While
     Else
     End If
     Search.Close()
    End If
   ElseIf Action.ToString().Trim = "parcelWelcomeMessage" Then    ' Return parcel name,title, description '
    Dim intX, intY As Integer
    intX = CInt(Request("coordX"))
    intY = CInt(Request("coordY"))

    ' Get Actual MyData DB name
    Dim DBName As String
    DBName = "MyData"
    Dim Control As MySqlDataReader
    SQLCmd = "Select Parm2 From control Where Control='ADMINSYSTEM' and Parm1='MyDataDBName'"
    If tLog Then                                            ' Log file output
     sw.WriteLineAsync("Get control values SQLCmd: " + SQLCmd.ToString())
     sw.Flush()
    End If
    Control = MyDB.GetReader("MySite", SQLCmd)
    If MyDB.Error() Then                                           ' Show Error
     sw.WriteLine("DB Error: " + MyDB.ErrMessage().ToString())
     sw.Flush()
    End If
    If Control.HasRows() Then
     Control.Read()
     DBName = Control("Parm2").Trim().ToString()
    End If
    Control.Close()

    Dim GetRegion As MySqlDataReader
    SQLCmd = "Select Name,Description,SnapshotUUID " +
             "From regionxml A inner join " + DBName.ToString() + ".land B on A.UUID=B.RegionUUID " +
             " Inner Join " + DBName.ToString() + ".regionsettings C on A.UUID=C.regionUUID " +
             " Left Outer Join " + DBName.ToString() + ".regions D on A.UUID=D.UUID " +
             "Where A.locationX=" + MyDB.SQLNo(intX) + " and A.locationY=" + MyDB.SQLNo(intY)
    If tLog Then                                            ' Log file output
     sw.WriteLine("Get region values SQLCmd: " + SQLCmd.ToString())
     sw.Flush()
    End If
    GetRegion = MyDB.GetReader("MySite", SQLCmd)


    GetRegion.Close()
   End If
  End If

  If tLog Then                                             ' Log file output
   sw.WriteLine("********* Completed Processing *********" + vbCrLf)
   sw.Flush()
   sw.Close()
  End If
 End Sub

 Function JSONPair(itmName As String, itmVal As String) As String
  Dim tOut As String
  tOut = Chr(34) + itmName + Chr(34) + ":"
  If IsNumeric(itmVal) Or itmVal.ToString().StartsWith("{") Or itmVal.ToString().StartsWith("[") Then
   tOut = tOut + itmVal
  Else
   tOut = tOut + Chr(34) + itmVal + Chr(34)
  End If
  Return tOut.ToString()
 End Function

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
 End Sub
End Class
