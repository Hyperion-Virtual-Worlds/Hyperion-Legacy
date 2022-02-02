Imports System.ComponentModel
Imports System.Web.UI
Imports System.Web.HttpContext

'* This class provides a simple top cascade menu control that can be easily managed in the code of the page in which it will show.
'* Written by Bob Curtice 11/26/2016 for use in the Halcyon grid website project.

Public Class TopMenu

 'Processing Variables:
 Private MenuError As String
 Private aIndex As Integer
 Private bTrace As Boolean
 Private aMenu() As String                                  ' menu content array

 ' Initialize a new instance of the class
 Public Sub New()
  MenuError = ""
  aIndex = 1
  bTrace = False
  ReDim aMenu(1)
  aMenu(1) = ""
 End Sub

 ' Allow programmer setting of trace property
 Public Property SetTrace() As Boolean
  Set(ByVal Value As Boolean)
   bTrace = Value
  End Set
  Get
   Return bTrace
  End Get
 End Property

 ' Call to add an entry to the menu
 Public Sub AddItem(ByVal mMenuID As Int64, ByVal mParentID As Int64, ByVal mCmd As String, ByVal mCtrl As String, ByVal mTitle As String)
  If bTrace Then Current.Trace.Warn("TopMenu", "Add Menu Item")
  If mMenuID = 0 Then
   MenuError = "Missing MenuID Entry! (Entry=" + aIndex.ToString() + ")."
  End If
  If MenuError.Length = 0 Then                              ' Do not process if error exist
   If mCmd.ToUpper() = "M" Then                             ' Forward Entry
    If mTitle.Length = 0 Then
     MenuError = "Missing Menu Entry Title! (Entry=" + aIndex.ToString().PadLeft(4) + ")."
    End If
    ReDim Preserve aMenu(aIndex)                            ' Resize menu for new entry
    aMenu(aIndex) = mParentID.ToString().PadLeft(4) + "|" + aIndex.ToString().PadLeft(4) + "|" + mMenuID.ToString().PadLeft(4) + "|" + mCmd.ToUpper() + "| |" + mTitle.Trim()
    aIndex = aIndex + 1
   ElseIf mCmd.ToUpper() = "P" Then                         ' Direct program selection
    If mTitle.Length = 0 Then
     MenuError = "Missing Menu Page Title! (Entry=" + aIndex.ToString() + ")."
    End If
    ReDim Preserve aMenu(aIndex)                            ' Resize menu for new entry
    aMenu(aIndex) = mParentID.ToString().PadLeft(4) + "|" + aIndex.ToString().PadLeft(4) + "|" + mMenuID.ToString().PadLeft(4) + "|" + mCmd.ToUpper() + "|" + mCtrl.Trim() + "|" + mTitle.Trim()
    aIndex = aIndex + 1
   ElseIf mCmd.ToUpper() = "L" Then                         ' Onclick control link to java script
    If mTitle.Length = 0 Then
     MenuError = "Missing Menu Entry Title! (Entry=" + aIndex.ToString() + ")."
    End If
    ReDim Preserve aMenu(aIndex)                            ' Resize menu for new entry
    aMenu(aIndex) = mParentID.ToString().PadLeft(4) + "|" + aIndex.ToString().PadLeft(4) + "|" + mMenuID.ToString().PadLeft(4) + "|" + mCmd.ToUpper() + "|" + mCtrl.Trim() + "|" + mTitle.Trim()
    aIndex = aIndex + 1
   ElseIf mCmd.ToUpper() = "B" Then                         ' Direct program selection
    If mParentID > 0 Then
     ReDim Preserve aMenu(aIndex)                           ' Resize menu for new entry
     aMenu(aIndex) = mParentID.ToString().PadLeft(4) + "|" + aIndex.ToString().PadLeft(4) + "|" + mMenuID.ToString().PadLeft(4) + "|" + mCmd.ToUpper() + "| | "
     aIndex = aIndex + 1
    Else
     MenuError = "Blank entry is not allowed in the top level! (Entry=" + aIndex.ToString() + ")."
    End If
   Else
    If bTrace Then Current.Trace.Warn("TopMenu", "Invalid menu command=" + aIndex.ToString())
    MenuError = "Invalid menu command! (Entry=" + aIndex.ToString() + ")."
   End If
   If bTrace Then Current.Trace.Warn("TopMenu", "Entry Added: " + aMenu(aIndex - 1))
  End If
  If bTrace Then Current.Trace.Warn("TopMenu", "End Menu Add")
 End Sub

 ' Call to process and return HTML menu content
 Public Function BuildMenu(Optional ByVal iIndent As Integer = 0) As String
  Dim tIndent, mCommand(), OutHtml, DivHTML As String
  Dim tDivID As Int64
  Dim I As Integer
  tIndent = ("").PadLeft(iIndent)
  OutHtml = ""
  DivHTML = ""
  tDivID = 0
  Array.Sort(aMenu)                                        ' Sort Menu Arrray to correct format for rendering
  If bTrace Then Current.Trace.Warn("TopMenu", "Start HTML render")
  If MenuError.Length > 0 Then
   OutHtml = OutHtml.ToString() + (vbCrLf + tIndent + "<!-- Top Menu Control -->" + vbCrLf).ToString() +
             (tIndent + "<table cellpadding=" + Chr(34) + "0" + Chr(34) + " cellspacing=" + Chr(34) + "1" + Chr(34) + " class=" + Chr(34) + "TopTable" + Chr(34) + ">" + vbCrLf).ToString() +
             (tIndent + " <tr>" + vbCrLf).ToString() +
             (tIndent + "  <td class=" + Chr(34) + "TopError" + Chr(34) + ">" + MenuError + "</td>" + vbCrLf).ToString() +
             (tIndent + " </tr>" + vbCrLf).ToString() +
             (tIndent + "</table>" + vbCrLf).ToString() +
             (tIndent + "<!-- End Top Menu -->" + vbCrLf + tIndent).ToString()
   If bTrace Then Current.Trace.Warn("TopMenu", "Menu Error=" + MenuError.ToString())
  Else
   OutHtml = (vbCrLf + tIndent + "<!-- Top Menu Control -->" + vbCrLf).ToString() +
             (tIndent + "<table cellpadding=" + Chr(34) + "0" + Chr(34) + " cellspacing=" + Chr(34) + "1" + Chr(34) + " class=" + Chr(34) + "TopTable" + Chr(34) + ">" + vbCrLf).ToString() +
             (tIndent + " <!-- Top Row Entry -->" + vbCrLf).ToString() +
             (tIndent + " <tr>" + vbCrLf).ToString()

   If bTrace Then Current.Trace.Warn("TopMenu", "Process Menu contents")
   If UBound(aMenu) > 0 Then
    For I = 1 To UBound(aMenu)
     mCommand = aMenu(I).Split("|")                          ' Split Menu Commands into zero base array elements for processing
     'Parms options:
     ' "M", required Count, required Entry title
     ' "P", optional link, required Entry title
     ' "L", optional javascript, required Entry title
     If bTrace Then Current.Trace.Warn("TopMenu", "UBound(mCommand)=" + UBound(mCommand).ToString())
     '     Array value items:  0=ParentID,1=SortOrder,2=MenuID,3=Command,4=Control,5=Title
     If bTrace Then Current.Trace.Warn("TopMenu", "mCommand(0)=" + mCommand(0).ToString() + ", mCommand(1)=" + mCommand(1).ToString() + ", mCommand(2)=" + mCommand(2).ToString() + ", mCommand(3)=" + mCommand(3).ToString() + ", mCommand(4)=" + mCommand(4).ToString() + ", mCommand(5)=" + mCommand(5).ToString())
     If Val(mCommand(0)) = 0 Then                            ' Process Top level items
      If mCommand(3) = "M" Then                              ' Top Row entry, does not contain a link: presumed to be the active page!
       If bTrace Then Current.Trace.Warn("TopMenu", "HTML render subtitle row")
       OutHtml = OutHtml.ToString() +
               (tIndent + "  <!-- TopMenu Entry -->" + vbCrLf).ToString() +
               (tIndent + "  <td id=" + Chr(34) + "TopNav" + mCommand(2).Trim() + Chr(34) + " class=" + Chr(34) + "TopEntry" + Chr(34) + " onmouseover=" + Chr(34) + "TopMenuOver('','TopNav" + mCommand(2).Trim() + "','SubMenu" + mCommand(2).Trim() + "');" + Chr(34) + " onmouseout=" + Chr(34) + "TopMenuOut('SubMenu" + mCommand(2).Trim() + "');" + Chr(34) + ">" + vbCrLf).ToString() +
               (tIndent + "   " + mCommand(5) + vbCrLf).ToString() +
               (tIndent + "  </td>" + vbCrLf).ToString()
      ElseIf mCommand(3) = "P" Then
       If bTrace Then Current.Trace.Warn("TopMenu", "Processing Menu item")
       OutHtml = OutHtml.ToString() +
               (tIndent + "  <!-- Program Entry -->" + vbCrLf).ToString() +
               (tIndent + "  <td class=" + Chr(34) + "TopEntry" + Chr(34) + ">" + vbCrLf).ToString() +
               (tIndent + "   <a href=" + Chr(34) + mCommand(4) + Chr(34) + " class=" + Chr(34) + "TopALink" + Chr(34) + ">" + mCommand(5) + "</a>" + vbCrLf).ToString() +
               (tIndent + "  </td>" + vbCrLf).ToString()
      ElseIf mCommand(3) = "L" Then                          ' Menu JavaScript call entry
       If bTrace Then Current.Trace.Warn("TopMenu", "Processing Menu java option")
       OutHtml = OutHtml.ToString() +
               (tIndent + "  <!-- JavaScript call -->" + vbCrLf).ToString() +
               (tIndent + "  <td class=" + Chr(34) + "TopEntry" + Chr(34) + ">" + vbCrLf).ToString() +
               (tIndent + "   <span onclick=" + Chr(34) + mCommand(4) + Chr(34) + " class=" + Chr(34) + "TopLink" + Chr(34) + " onmouseover=" + Chr(34) + "this.className='TopLinkOver';" + Chr(34) + " onmouseout=" + Chr(34) + "this.className='TopLink';" + Chr(34) + ">" + mCommand(5) + "</span>" + vbCrLf).ToString() +
               (tIndent + "  </td>" + vbCrLf).ToString()
      Else                                                   ' Error processing check
       OutHtml = OutHtml.ToString() +
               (tIndent + "  <td class=" + Chr(34) + "TopError" + Chr(34) + ">Invalid Item=" + aMenu(I).ToString() + "</td>" + vbCrLf).ToString()
      End If
     Else
      If Val(mCommand(0)) <> tDivID Then                     ' Open a new submenu
       If tDivID = 0 Then                                    ' Close Top Menu table
        OutHtml = OutHtml +
               (tIndent + " </tr>" + vbCrLf).ToString() +
               (tIndent + "</table>" + vbCrLf).ToString()
       Else                                                  ' Close prior open Submenu
        DivHTML = DivHTML +
               (tIndent + " </table>" + vbCrLf).ToString() +
               (tIndent + "</div>" + vbCrLf).ToString()
       End If
       DivHTML = DivHTML +
               (tIndent + "<div id=" + Chr(34) + "SubMenu" + mCommand(0).Trim() + Chr(34) + " style=" + Chr(34) + "position: absolute; left: 0px; top: 0px; z-index: 100;" + Chr(34) + " class=" + Chr(34) + "TopListOff" + Chr(34) + " onmouseover=" + Chr(34) + "TopListOver('SubMenu" + mCommand(0).Trim() + "');" + Chr(34) + " onmouseout=" + Chr(34) + "TopListOut('SubMenu" + mCommand(0).Trim() + "');" + Chr(34) + ">" + vbCrLf).ToString() +
               (tIndent + " <table width=" + Chr(34) + "100%" + Chr(34) + " cellpadding=" + Chr(34) + "0" + Chr(34) + " cellspacing=" + Chr(34) + "0" + Chr(34) + " class=" + Chr(34) + "TopSubTable" + Chr(34) + ">" + vbCrLf).ToString()
       tDivID = Val(mCommand(0))
      End If
      If mCommand(3) = "M" Then                              ' Top Row entry, does not contain a link: presumed to be the active page!
       If bTrace Then Current.Trace.Warn("TopMenu", "HTML render subtitle row")
       DivHTML = DivHTML.ToString() +
               (tIndent + "  <!-- SubMenu Entry -->" + vbCrLf).ToString() +
               (tIndent + "  <tr>" + vbCrLf).ToString() +
               (tIndent + "   <td id=" + Chr(34) + "SubNav" + mCommand(2).Trim() + Chr(34) + " class=" + Chr(34) + "TopSubEntry" + Chr(34) + " onmouseover=" + Chr(34) + "TopMenuOver('SubMenu" + mCommand(0).Trim() + "','SubNav" + mCommand(2).Trim() + "','SubMenu" + mCommand(2).Trim() + "');" + Chr(34) + " onmouseout=" + Chr(34) + "TopMenuOut('SubMenu" + mCommand(2).Trim() + "');" + Chr(34) + ">" + vbCrLf).ToString() +
               (tIndent + "    " + mCommand(5) + vbCrLf).ToString() +
               (tIndent + "   </td>" + vbCrLf).ToString() +
               (tIndent + "  </tr>" + vbCrLf).ToString()
      ElseIf mCommand(3) = "P" Then                          ' Program item call entry
       If bTrace Then Current.Trace.Warn("TopMenu", "Processing Menu item")
       DivHTML = DivHTML.ToString() +
               (tIndent + "  <!-- Program Entry -->" + vbCrLf).ToString() +
               (tIndent + "  <tr>" + vbCrLf).ToString() +
               (tIndent + "   <td class=" + Chr(34) + "TopSubEntry" + Chr(34) + " onmouseover=" + Chr(34) + "TopItemOver('SubMenu" + mCommand(0).Trim() + "');" + Chr(34) + ">" + vbCrLf).ToString() +
               (tIndent + "    <a href=" + Chr(34) + mCommand(4) + Chr(34) + " class=" + Chr(34) + "TopALink" + Chr(34) + " onmouseover=" + Chr(34) + "TopItemOver('SubMenu" + mCommand(0).Trim() + "');" + Chr(34) + " onclick=" + Chr(34) + "TopItemClick();" + Chr(34) + ">" + mCommand(5) + "</a>" + vbCrLf).ToString() +
               (tIndent + "   </td>" + vbCrLf).ToString() +
               (tIndent + "  </tr>" + vbCrLf).ToString()
      ElseIf mCommand(3) = "L" Then                          ' Menu JavaScript call entry
       If bTrace Then Current.Trace.Warn("TopMenu", "Processing Menu java option")
       DivHTML = DivHTML.ToString() +
               (tIndent + "  <!-- JavaScript call -->" + vbCrLf).ToString() +
               (tIndent + "  <tr>" + vbCrLf).ToString() +
               (tIndent + "   <td class=" + Chr(34) + "TopSubEntry" + Chr(34) + " onmouseover=" + Chr(34) + "TopItemOver('SubMenu" + mCommand(0).Trim() + "');" + Chr(34) + ">" + vbCrLf).ToString() +
               (tIndent + "    <span onclick=" + Chr(34) + mCommand(4) + "TopItemClick();" + Chr(34) + " class=" + Chr(34) + "TopLink" + Chr(34) + " onmouseover=" + Chr(34) + "this.className='TopLinkOver';TopItemOver('SubMenu" + mCommand(0).Trim() + "');" + Chr(34) + " onmouseout=" + Chr(34) + "this.className='TopLink';" + Chr(34) + ">" + mCommand(5) + "</span>" + vbCrLf).ToString() +
               (tIndent + "   </td>" + vbCrLf).ToString() +
               (tIndent + "  </tr>" + vbCrLf).ToString()
      ElseIf mCommand(3) = "B" Then                          ' Blank line separator entry
       If bTrace Then Current.Trace.Warn("TopMenu", "Processing Separator option")
       DivHTML = DivHTML +
               (tIndent + "  <!-- Separator Entry -->" + vbCrLf).ToString() +
               (tIndent + "  <tr>" + vbCrLf).ToString() +
               (tIndent + "   <td class=" + Chr(34) + "TopSubEntry" + Chr(34) + ">" + vbCrLf).ToString() +
               (tIndent + "    --------------------" + vbCrLf).ToString() +
               (tIndent + "   </td>" + vbCrLf).ToString() +
               (tIndent + "  </tr>" + vbCrLf).ToString()
      Else                                                   ' Error processing check
       If bTrace Then Current.Trace.Warn("TopMenu", "Processing Entry Error")
       DivHTML = DivHTML +
               (tIndent + "  <!-- Entry Error -->" + vbCrLf).ToString() +
               (tIndent + "  <tr>" + vbCrLf).ToString() +
               (tIndent + "   <td class=" + Chr(34) + "TopError" + Chr(34) + ">Invalid Item=" + aMenu(I).ToString() + "</td>" + vbCrLf).ToString() +
               (tIndent + "  </tr>" + vbCrLf).ToString()
      End If
     End If
    Next
   Else
    OutHtml = OutHtml.ToString() +
               (tIndent + "  <td class=" + Chr(34) + "TopError" + Chr(34) + ">Invalid Item=" + aMenu(I).ToString() + "</td>" + vbCrLf).ToString()
   End If
   If bTrace Then Current.Trace.Warn("TopMenu", "End Render")
   If DivHTML.Length > 0 Then                            ' Last div was left open
    ' Close Menu List Table Structure
    DivHTML = DivHTML.ToString() +
              (tIndent + " </table>" + vbCrLf).ToString() +
              (tIndent + "</div>" + vbCrLf).ToString() +
              (tIndent + "<!-- End of Top Menu-->" + vbCrLf + tIndent.Substring(0, tIndent.Length - 1)).ToString()
   Else                                                  ' No Div entries were used
    If tDivID = 0 Then                                   ' Top Menu table was not closed...
     OutHtml = OutHtml +
              (tIndent + " </tr>" + vbCrLf).ToString() +
              (tIndent + "</table>" + vbCrLf).ToString() +
              (tIndent + "<!-- End of Top Menu-->" + vbCrLf + tIndent.Substring(0, tIndent.Length - 1)).ToString()
    End If
   End If
  End If
  Return OutHtml.ToString() + DivHTML.ToString()
 End Function

 ' Close object
 Public Sub Close()
  MenuError = Nothing
  aIndex = Nothing
  bTrace = Nothing
  aMenu = Nothing
  Finalize()
 End Sub

 Protected Overrides Sub Finalize()
  MyBase.Finalize()
 End Sub
End Class
