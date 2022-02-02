
Imports System.IO
Imports System.Threading

Partial Class MapImage
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
 '* This page provides map image data from Grid assets for land or event associated images.
 '* 
 '* Built from Website Basic Page template v. 1.0

 ' Define common properties and objects here for the page
 Private MyDB As New MySQLLib                              ' Provides data access methods and error handling
 Private GridLib As New GridLib
 Private SQLCmd As String

 Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  Dim AssetID, tLogOut As String
  AssetID = Request.ServerVariables("PATH_INFO").ToString().Replace("/MapImage.aspx/", "")
  AssetID = GridLib.CleanStr(AssetID, 36)             ' Only allows UUID as astring
  '          1         1         1
  ' 123456789012345678901234567890123456
  ' xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
  tLogOut = "mapFunctions Start: " + FormatDateTime(Date.Now(), DateFormat.GeneralDate) + vbCrLf
  tLogOut = tLogOut.ToString() + "Passed by URL: " + AssetID.ToString() + vbCrLf

  Dim AssetReader As Chattel.ChattelReader

  Dim WhipConfig As Chattel.AssetServerWHIPConfig
  WhipConfig.Host = "127.0.0.1"
  WhipConfig.Name = "Whip1"
  WhipConfig.Password = "WhipPass"
  WhipConfig.Port = "32700"
  'Setup CG Asset Access
  Dim parallel = New List(Of Chattel.IAssetServerConfig)
  Dim seriesParallel = New List(Of List(Of Chattel.IAssetServerConfig))
  parallel = New List(Of Chattel.IAssetServerConfig)
  seriesParallel = New List(Of List(Of Chattel.IAssetServerConfig))
  parallel.Add(WhipConfig)
  seriesParallel.Add(parallel)
  Dim AssetConfig As New Chattel.ChattelConfiguration(serialParallelServerConfigs:=seriesParallel)
  AssetConfig.DisableCache()
  AssetReader = New Chattel.ChattelReader(AssetConfig)

  Dim Asset As New InWorldz.Data.Assets.Stratus.StratusAsset
  Asset = AssetReader.ReadAssetSync(OpenMetaverse.UUID.Parse(AssetID))
  If Asset IsNot Nothing Then                          ' Process Checks
   Response.Clear()
   If Asset.IsImageAsset Then                          ' Required image content only
    ' Attempt to get image data from the asset
    If Asset.Type = OpenMetaverse.ImageCodec.JPEG Then
     Response.ContentType = "image/jpg"
    ElseIf Asset.Type = OpenMetaverse.ImageCodec.PNG Then
     Response.ContentType = "image/png"
    ElseIf Asset.Type = OpenMetaverse.ImageCodec.TGA Then
     Response.ContentType = "image/tga"
    End If
    Response.BinaryWrite(Asset.Data)
   Else
    Response.StatusCode = 307
    Response.AddHeader("Location", "/Images/Site/default-new.jpg")
    tLogOut = tLogOut.ToString() + "Sent /Images/Site/default-new.jpg"
   End If
   Response.End()
  Else
   ' Use default image from saved file location: /Images/Site/
   Response.Clear()
   Response.StatusCode = 307
   Response.AddHeader("Location", "/Images/Site/default-new.jpg")
   tLogOut = tLogOut.ToString() + "Sent /Images/Site/default-new.jpg"
  End If
  Asset = Nothing
  AssetReader = Nothing

  If True Then                                       ' Set true to turn on process logging
   Dim tURLPath, tFilePath As String
   tURLPath = ""                      ' Place your URL path to where this page is placed relative to the root.
   tFilePath = Server.MapPath(tURLPath.ToString().Replace("/", "\")).ToString()
   ' Trace Actions to Log file
   Dim tFileIsOpen As Boolean = False
   Do
    Try
     FileOpen(1, tFilePath.ToString() + "\MapImage.log", OpenMode.Append, OpenAccess.Write, OpenShare.LockWrite)
     tFileIsOpen = True
    Catch ex As IOException
     ' Don't care too much
     Thread.Sleep(10)
    End Try
   Loop While Not tFileIsOpen
   Lock(1)
   FilePut(1, tLogOut)
   Unlock(1)
   FileClose(1)
  End If

 End Sub

 Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
  ' Close open page objects
  MyDB.Close()
  MyDB = Nothing
  GridLib.Close()
  GridLib = Nothing
 End Sub
End Class
