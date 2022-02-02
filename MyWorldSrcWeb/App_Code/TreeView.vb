Imports System.ComponentModel
Imports System.Web.UI
Imports System.Web.HttpContext

'* This class provides a simple tree view menu control that can be easily managed in the code of the page in which it will show.
'* Written by Bob Curtice 11/26/2016 for use in the Halcyon grid website project.

Public Class TreeView

 'Processing Variables:
 Private mControl, MenuError, mWidth As String
 Private MenuID, aIndex As Integer
 Private bTrace, mCenter As Boolean
 Private aMenu(), FontColor As String                       ' menu content array

 ' Initialize a new instance of the class
 Public Sub New()
  mControl = ""
  MenuError = ""
  MenuID = 0
  aIndex = 0
  bTrace = False
  mCenter = False
  ReDim aMenu(0)
  aMenu(0) = ""
  FontColor = ""
 End Sub

 ' Allow external access to MenuState. State settings may change by user action, updated by cookie.
 Public Property MenuState() As String
  Set(ByVal Value As String)
   mControl = Value
  End Set
  Get
   Return mControl
  End Get
 End Property

 ' Allow programmer setting of trace property
 Public Property SetTrace() As Boolean
  Set(ByVal Value As Boolean)
   bTrace = Value
  End Set
  Get
   Return bTrace
  End Get
 End Property

 ' Allow programmer to set Table centering mode 
 Public WriteOnly Property SetCenter() As Boolean
  Set(ByVal Value As Boolean)
   mCenter = Value
  End Set
 End Property

 ' Allow external access to MenuState. State settings may change by user action, updated by cookie.
 Public WriteOnly Property TextColor() As String
  Set(ByVal Value As String)
   FontColor = Value
  End Set
 End Property

 ' Allow programmer setting of Table Width if centering is active
 Public Property SetWidth() As String
  Set(ByVal Value As String)
   mWidth = Value
  End Set
  Get
   Return mWidth
  End Get
 End Property

 ' Call to add an entry to the menu
 Public Sub AddItem(ByVal mCmd As String, ByVal mCtrl As String, ByVal mTitle As String)
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Add Menu Item")
  If MenuError.Length = 0 Then                              ' Do not process if error exist
   If mCmd.ToUpper() = "M" Then
    If mCtrl.Length = 0 Or Not IsNumeric(mCtrl) Then
     MenuError = "Missing number of Menu Entries! (Entry=" + aIndex.ToString() + ")."
    ElseIf mTitle.Length = 0 Then
     MenuError = "Missing Menu Entry Title! (Entry=" + aIndex.ToString() + ")."
    End If
    MenuID = MenuID + 1                                     ' Menu ID is unique for each menu entry
    If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Add Menu Item: CurrLevel=" + MenuID.ToString() + ", Cmd=" + mCmd.ToString() + ", mCtrl=" + mCtrl.ToString() + ", mTitle=" + mTitle.ToString())
    If mControl.Length > 0 Then mControl = mControl.ToString() + ("&").ToString() ' Delimeter for Javascript processing
    mControl = mControl.ToString() + ("I").ToString() + Right("000" + MenuID.ToString(), 3) + (":0").ToString()
    ReDim Preserve aMenu(aIndex)                            ' Resize menu for new entry
    aMenu(aIndex) = mCmd.ToUpper() + "|" + mCtrl.ToString().Trim() + "|" + mTitle.ToString().Trim()
    aIndex = aIndex + 1
    If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "MenuControl=" + mControl)
   ElseIf mCmd.ToUpper() = "P" Then
    If mTitle.Length = 0 Then
     MenuError = "Missing Menu Page Title! (Entry=" + aIndex.ToString() + ")."
    End If
    ReDim Preserve aMenu(aIndex)                            ' Resize menu for new entry
    aMenu(aIndex) = mCmd.ToUpper() + "|" + mCtrl.ToString().Trim() + "|" + mTitle.ToString().Trim()
    aIndex = aIndex + 1
    If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Add Menu Item: Cmd=" + mCmd.ToString() + ", mCtrl=" + mCtrl.ToString() + ", mTitle=" + mTitle.ToString())
   ElseIf mCmd.ToUpper() = "L" Then                         ' Onclick control link to java script
    If mTitle.Length = 0 Then
     MenuError = "Missing Menu Entry Title! (Entry=" + aIndex.ToString() + ")."
    End If
    ReDim Preserve aMenu(aIndex)                            ' Resize menu for new entry
    aMenu(aIndex) = mCmd.ToUpper() + "|" + mCtrl.ToString().Trim() + "|" + mTitle.ToString().Trim()
    aIndex = aIndex + 1
    If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Add Menu Item: Cmd=" + mCmd.ToString() + ", mCtrl=" + mCtrl.ToString() + ", mTitle=" + mTitle.ToString())
   ElseIf mCmd.ToUpper() = "B" Then                         ' Blank line entry
    ReDim Preserve aMenu(aIndex)                            ' Resize menu for new entry
    aMenu(aIndex) = mCmd.ToUpper() + "| | "
    aIndex = aIndex + 1
    If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Add Menu Item: Cmd=" + mCmd.ToString() + ", mCtrl= , mTitle='Blank Line entry'")
   ElseIf mCmd.ToUpper() = "T" Then                         ' Sub Title entry
    ReDim Preserve aMenu(aIndex)                            ' Resize menu for new entry
    aMenu(aIndex) = mCmd.ToUpper() + "| |" + mTitle.ToString().Trim()
    aIndex = aIndex + 1
    If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Add Menu Item: Cmd=" + mCmd.ToString() + ", mCtrl='', mTitle=" + mTitle.ToString())
   Else
    If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Invalid menu command=" + aIndex.ToString())
    MenuError = "Invalid menu command! (Entry=" + aIndex.ToString() + ")."
   End If
  End If
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "End Menu Add")
 End Sub

 ' Call to process and return HTML menu content
 Public Function BuildMenu(Optional ByVal sTitle As String = "", Optional ByVal iIndent As Integer = 0) As String
  Dim tIndent, mCommand(), aMState(), sMenuID, OutHtml, tImg, tOpenState, tCenter, tStyle As String
  Dim I, mLevel As Integer
  Dim mCount() As Integer
  sMenuID = ""                                              ' String value of current MenuID
  tIndent = ("").PadLeft(iIndent)
  OutHtml = ""
  tImg = ""
  tOpenState = ""
  mLevel = 0                                                ' mCount Array index
  ReDim mCount(1)                                           ' Menu entry count array
  mCount(1) = 0
  tCenter = ""
  If mCenter Then
   tCenter = " style=""width:" + mWidth.ToString() + "px; text-align:center;""" + Chr(34)
  Else
   tCenter = " style=""width:100%;" + Chr(34)
  End If
  tStyle = ""
  If FontColor.ToString().Length > 0 Then
   tStyle = " style=""color: #" + FontColor.ToString() + """"
  End If

  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Start HTML render")
  If MenuError.Length > 0 Then
   OutHtml = OutHtml.ToString() + (vbCrLf + tIndent + "<!-- Sidebar Menu Control -->" + vbCrLf).ToString() +
             (tIndent + "<table " + tCenter + " cellpadding=""2"" cellspacing=""0"" class=""TreeViewTable""" + tStyle + ">" + vbCrLf).ToString() +
             (tIndent + " <tr>" + vbCrLf).ToString() +
             (tIndent + "  <td class=""TreeVError"">" + MenuError + "</td>" + vbCrLf).ToString() +
             (tIndent + " </tr>" + vbCrLf).ToString() +
             (tIndent + "</table>" + vbCrLf).ToString() +
             (tIndent + "<!-- End Sidebar Menu -->" + vbCrLf + tIndent).ToString()
   If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Menu Error=" + MenuError.ToString())
  Else
   MenuID = 0
   mLevel = 0                                               ' Base Menu Level
   OutHtml = (vbCrLf + tIndent + "<!-- Sidebar Menu Control -->" + vbCrLf).ToString() +
             (tIndent + "<table " + tCenter + " cellpadding=""2"" cellspacing=""0"" class=""TreeViewTable""" + tStyle + ">" + vbCrLf).ToString()

   If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Title exist test")
   If sTitle.Length > 0 Then
    If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "HTML render title row")
    OutHtml = OutHtml.ToString() +
            (tIndent + " <tr>" + vbCrLf).ToString() +
            (tIndent + "  <td colspan=""2"" class=""TreeVTitle""" + tStyle + ">" + sTitle + "</td>" + vbCrLf).ToString() +
            (tIndent + " </tr>" + vbCrLf).ToString()
   End If
   If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Process Menu contents")
   Dim Tracking As Boolean
   Tracking = False
   aMState = ("I000:0&" + mControl).Split("&")              ' Adjust for zero based array index, where 0=base level
   If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Menu Array Length = " + aMenu.Length().ToString() +
       ", UBound = " + UBound(aMenu).ToString())
   If aMenu(0).ToString().Trim().Length > 0 Then            ' Must have entries to process Menu
    For I = 0 To UBound(aMenu)
     mCommand = aMenu(I).Split("|")                         ' Split Menu Commands into zero base array elements for processing
     'Parms options:
     ' "M", Enter number of menu members, required Entry Title
     ' "P", optional link, required Entry title
     ' "L", optional javascript, required Entry title
     ' "B", ignored, ignored  - produces a blank entry
     ' "T", ignored, title entry -  produces a title line
     If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "UBound(mCommand)=" + UBound(mCommand).ToString())
     If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "mCommand(0)=" + mCommand(0).ToString() + ", mCommand(1)=" + mCommand(1).ToString() + ", mCommand(2)=" + mCommand(2).ToString())
     If aMenu(I).Substring(0, 1) = "M" Then                 ' New Menu Level entry
      Tracking = True
      If mLevel > 0 Then mCount(mLevel) = mCount(mLevel) - 1 ' Menu Entry is a member in this level, adjust count
      mLevel = mLevel + 1
      ReDim Preserve mCount(mLevel)
      mCount(mLevel) = CInt(mCommand(1))
      If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Open new Menu level")
      If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "MenuControl=" + "I000:0&" + mControl)

      MenuID = MenuID + 1                                   ' Menu ID is unique for each menu entry
      If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "MenuID=" + MenuID.ToString())
      If aMState(MenuID).Substring(aMState(MenuID).Length - 1, 1) = "1" Then ' Set menu state Open
       tOpenState = ""
       tImg = "/Images/TreeView/Minus.gif"
      Else                                                  ' Set Menu state Closed
       tOpenState = "none"
       tImg = "/Images/TreeView/Plus.gif"
      End If
      sMenuID = Right("000" + CStr(MenuID), 3)
      OutHtml = OutHtml.ToString() +
                (tIndent + " <!-- Menu Entry -->" + vbCrLf).ToString() +
                (tIndent + " <tr>" + vbCrLf).ToString() +
                (tIndent + "  <td class=""TreeVIcon""" + tStyle + "><img id=""I" + sMenuID + """ onclick=""MenuSelect('I" + sMenuID + "','L" + sMenuID + "','I" + sMenuID + "');"" src=""" + tImg + """ style=""cursor: pointer;"" alt=""""></td>" + vbCrLf).ToString() +
                (tIndent + "  <td title=""" + mCommand(2) + """ onclick=""MenuSelect('I" + sMenuID + "','L" + sMenuID + "','I" + sMenuID + "');"" class=""TreeVEntry""" + tStyle + ">" + vbCrLf).ToString() +
                (tIndent + "   " + mCommand(2) + vbCrLf).ToString() +
                (tIndent + "  </td>" + vbCrLf).ToString() +
                (tIndent + " </tr>" + vbCrLf).ToString() +
                (tIndent + " <tr id=""L" + sMenuID + """ style=""display: " + tOpenState + ";"">" + vbCrLf).ToString() +
                (tIndent + "  <td class=""TreeVIcon""" + tStyle + "><img src=""/Images/TreeView/Blank.gif"" alt="""" /></td>" + vbCrLf).ToString() +
                (tIndent + "  <td>" + vbCrLf).ToString() +
                (tIndent + "   <table width=""100%"" cellpadding=""2"" cellspacing=""0"">" + vbCrLf).ToString()
      tIndent = ("").PadLeft(iIndent + (mLevel * 3))        ' Indent HTML table the level of the active menu
     ElseIf aMenu(I).Substring(0, 1) = "P" Then             ' Menu Page call
      Tracking = True
      If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Processing Menu item")
      If mLevel > 0 Then mCount(mLevel) = mCount(mLevel) - 1
      OutHtml = OutHtml.ToString() +
                (tIndent + " <!-- Menu Link -->" + vbCrLf).ToString() +
                (tIndent + " <tr>" + vbCrLf).ToString() +
                (tIndent + "  <td class=""TreeVIcon""" + tStyle + "><img src=""/Images/TreeView/Dot.gif"" alt="""" /></td>" + vbCrLf).ToString() +
                (tIndent + "  <td>" + vbCrLf).ToString()
      If mCommand(1).IndexOf("http://") > 0 Then
       OutHtml = OutHtml.ToString() +
                 (tIndent + "   <span onclick=""window.open='" + mCommand(1) + "';"" class=""TreeVLink"">" + mCommand(2) + "</span>" + vbCrLf).ToString()
      Else
       OutHtml = OutHtml.ToString() +
                 (tIndent + "   <span onclick=""window.location='" + mCommand(1) + "';"" class=""TreeVItem""" + tStyle + ">" + mCommand(2) + "</span>" + vbCrLf).ToString()
      End If
      OutHtml = OutHtml.ToString() +
                (tIndent + "  </td>" + vbCrLf).ToString() +
                (tIndent + " </tr>" + vbCrLf).ToString()
     ElseIf aMenu(I).Substring(0, 1) = "L" Then             ' Menu JavaScript call entry
      Tracking = True
      If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Processing Menu JavaScript Link option")
      If mLevel > 0 Then mCount(mLevel) = mCount(mLevel) - 1
      OutHtml = OutHtml.ToString() +
                (tIndent + " <!-- Menu JavaScript call -->" + vbCrLf).ToString() +
                (tIndent + " <tr>" + vbCrLf).ToString() +
                (tIndent + "  <td class=""TreeVIcon""" + tStyle + "><img src=""/Images/TreeView/Dot.gif"" alt="""" /></td>" + vbCrLf).ToString() +
                (tIndent + "  <td>" + vbCrLf).ToString() +
                (tIndent + "   <span onclick=""" + mCommand(1) + """ class=""TreeVItem""" + tStyle + ">" + mCommand(2) + "</span>" + vbCrLf).ToString() +
                (tIndent + "  </td>" + vbCrLf).ToString() +
                (tIndent + " </tr>" + vbCrLf).ToString()
     ElseIf aMenu(I).Substring(0, 1) = "B" Then             ' Make a blank line entry
      Tracking = True
      If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Processing Blank line option")
      If mLevel > 0 Then mCount(mLevel) = mCount(mLevel) - 1
      OutHtml = OutHtml.ToString() +
                (tIndent + " <!-- Menu blank line -->" + vbCrLf).ToString() +
                (tIndent + " <tr>" + vbCrLf).ToString() +
                (tIndent + "  <td class=""TreeVIcon""" + tStyle + "><img src=""/Images/TreeView/Blank.gif"" alt="""" /></td>" + vbCrLf).ToString() +
                (tIndent + "  <td>&nbsp;</td>" + vbCrLf).ToString() +
                (tIndent + " </tr>" + vbCrLf).ToString()
     ElseIf aMenu(I).Substring(0, 1) = "T" Then             ' Make a sub title entry
      Tracking = True
      If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "HTML render subtitle row")
      OutHtml = OutHtml.ToString() +
              (tIndent + " <tr>" + vbCrLf).ToString() +
              (tIndent + "  <td colspan=""2"" class=""TreeVSubTitle""" + tStyle + ">" + mCommand(2) + "</td>" + vbCrLf).ToString() +
              (tIndent + " </tr>" + vbCrLf).ToString()
     Else                                                   ' Error processing check
      OutHtml = OutHtml.ToString() +
                (tIndent + " <tr>" + vbCrLf).ToString() +
                (tIndent + "  <td class=""TreeVIcon""" + tStyle + "><img src=""/Images/TreeView/Blank.gif"" alt="""" /></td>" + vbCrLf).ToString() +
                (tIndent + "  <td class=""TreeVItem""" + tStyle + ">" + vbCrLf).ToString() +
                (tIndent + "   Bad Menu Item=" + aMenu(I).ToString() + vbCrLf).ToString() +
                (tIndent + "  </td>" + vbCrLf).ToString() +
                (tIndent + " </tr>" + vbCrLf).ToString()
     End If
     If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Menu Level Count=" + mCount(mLevel).ToString() + "; Menu Level=" + mLevel.ToString())
     If mCount(mLevel) = 0 Then                             ' Close SubMenu level table
      If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "Close open Menu Levels")
      While mCount(mLevel) = 0 And mLevel > 0
       mLevel = mLevel - 1
       ReDim Preserve mCount(mLevel)
       If mLevel = 0 Then
        tIndent = ("").PadLeft(iIndent)
       Else
        tIndent = ("").PadLeft(iIndent + (mLevel * 3))      ' Indent HTML table the level of the active menu
       End If
       OutHtml = OutHtml.ToString() +
                 (tIndent + "    <!-- Close Menu Level -->" + vbCrLf).ToString() +
                 (tIndent + "   </table>" + vbCrLf).ToString() +
                 (tIndent + "  </td>" + vbCrLf).ToString() +
                 (tIndent + " </tr>" + vbCrLf).ToString()
      End While
     End If
    Next
   End If

   If Not Tracking Then
    OutHtml = OutHtml.ToString() +
              (tIndent + " <tr>" + vbCrLf).ToString() +
              (tIndent + "  <td style=""width: 15px; height: 15px;""><img src=""/Images/TreeView/Blank.gif"" alt="""" /></td>" + vbCrLf).ToString() +
              (tIndent + "  <td style=""width: 100%;"" class=""TreeVError"">" + vbCrLf).ToString() +
              (tIndent + "   Num Menu Items=" + UBound(aMenu).ToString() + vbCrLf).ToString() +
              (tIndent + "  </td>" + vbCrLf).ToString() +
              (tIndent + " </tr>" + vbCrLf).ToString()
   End If
   If bTrace Then System.Web.HttpContext.Current.Trace.Warn("TreeView", "End Render")
   OutHtml = OutHtml.ToString() +
             (tIndent + "</table>" + vbCrLf).ToString() +
             (tIndent + "<!-- End of Sidebar Menu-->" + vbCrLf + tIndent).ToString()
  End If
  Return OutHtml
 End Function

 ' Close object
 Public Sub Close()
  mControl = Nothing
  MenuError = Nothing
  MenuID = Nothing
  aIndex = Nothing
  bTrace = Nothing
  mCenter = Nothing
  mWidth = Nothing
  aMenu = Nothing
  Finalize()
 End Sub

 Protected Overrides Sub Finalize()
  MyBase.Finalize()
 End Sub
End Class
