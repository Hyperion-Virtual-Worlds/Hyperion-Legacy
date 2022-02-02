Imports System.ComponentModel
Imports System.Web.UI
Imports System.Web.HttpContext

'* This class library provides a place for globally useful functions and methods used in several processes.
'* Written by Bob Curtice 11/26/2016 for use in the Halcyon grid website project.

Public Class GridLib
 Private bTrace As Boolean

 Public Sub New()
  ' Only to initialize a new object instance. Does nothing.
  bTrace = False
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

 ' Security validation of input data limited to specified length. Prevents SQL highjacking.
 ' Returns a Clean input string content.
 Public Function CleanStr(ByVal sInput As String, Optional ByVal iLen As Integer = 0) As String
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("CleanString", "Input String=" + sInput.ToString() + ", Length=" + iLen.ToString())
  Dim OutStr As String
  OutStr = sInput.Replace(Chr(39), "")
  OutStr = OutStr.Replace("=", "")
  If iLen > 0 Then                                      ' Length limit specified
   If OutStr.Length > iLen Then                         ' String is longer than limit
    OutStr = OutStr.Substring(1, iLen)                  ' Shorten to limit length
   End If
  End If
  OutStr = OutStr.Trim()
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("CleanString", "OutStr=" + OutStr.ToString())
  Return OutStr
 End Function

 ' Validates an email address of the correct form and character content
 Public Function ValidEmail(ByVal tEmail As String) As Boolean
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("ValidEmail", "")
  Dim tRtn As Boolean
  Dim Invalid As String
  Dim I As Integer
  tRtn = True
  If IsDBNull(tEmail) Then
   tRtn = False
  Else
   ' Allowed: !#$%&'*+-/=?^_`{|}~
   Invalid = "^;:, "                                    ' Invalid email characters
   For I = 0 To Len(Invalid) - 1                        ' Check for invalid characters (zero based array)
    If tEmail.IndexOf(Invalid.Chars(I)) > 0 Then
     tRtn = False
     Exit For
    End If
   Next
   If tRtn Then                                         ' Continue check if still valid
    If Not tEmail.Contains("@") Then                    ' Check for required @ symbol
     tRtn = False
    Else
     If tEmail.ToString().Trim().Length <= 1 Then       ' Must contain more than @
      tRtn = False
     Else
      Dim tDom As String                                ' Domain must exist with at least one period
      tDom = tEmail.ToString().Substring(tEmail.LastIndexOf("@"))
      If tDom.ToString().Trim().Length <= 4 Or Not tDom.ToString().Contains(".") Then
       tRtn = False
      End If
     End If
    End If
   End If
  End If
  Return tRtn
 End Function

 ' Correctly format a telephone number to (999) 999-9999 USA or CAN, or 01-999-999-9999 International
 ' forms regardless of input formatting.
 Function PhoneFormat(ByVal fPhone As String) As String
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("PhoneFormat", "Input Phone=" + fPhone)
  Dim temp, tNew, tchar As String
  Dim tArea As Boolean
  Dim tLen, I As Integer

  temp = fPhone.Trim()
  tNew = ""
  tArea = False
  tLen = temp.Length
  For I = 1 To tLen                                     ' Process all input characters
   tchar = temp.Substring(temp.Length - 1, 1)           ' process character from right
   temp = temp.Substring(0, temp.Length - 1)            ' Lose last character
   If InStr("0123456789", tchar) > 0 Then               ' Got a digit!
    tNew = tchar + tNew                                 ' Collect all digits right to left
   End If
   'If bTrace Then System.Web.HttpContext.Current.Trace.Warn("PhoneFormat", "Temp=" + temp.ToString() + ", tNew=" + tNew.ToString())
  Next
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("PhoneFormat", "Clean New=" + tNew.ToString())
  temp = tNew
  tLen = tNew.Length
  tNew = ""
  If tLen <= 10 Then                                    ' Standard USA format
   For I = 1 To tLen
    tNew = temp.Substring(temp.Length - 1, 1) + tNew
    temp = temp.Substring(0, temp.Length - 1)           ' Lose last character
    If I = 4 Then                                       ' Add dash
     tNew = "-" + tNew
    End If
    If I = 7 Then                                       ' Check if there is an area code
     If temp.Length > 0 Then                            ' more to process, must have an area code
      tArea = True
      tNew = ") " + tNew
     End If
    End If
    If bTrace Then System.Web.HttpContext.Current.Trace.Warn("PhoneFormat", "Processing Phone=" + tNew.ToString())
   Next
   If tArea Then                                        ' Close area code format
    tNew = "(" + tNew
   End If
   If bTrace Then System.Web.HttpContext.Current.Trace.Warn("PhoneFormat", "New USA Phone=" + tNew.ToString())
  Else                                                  ' format as International phone number
   For I = 1 To tLen
    tNew = temp.Substring(temp.Length - 1, 1) + tNew
    temp = temp.Substring(0, temp.Length - 1)           ' Lose last character
    If I = 4 Then
     tNew = "-" + tNew
    ElseIf I = 7 Then
     tNew = "-" + tNew
    ElseIf I = 10 Then
     tNew = "-" + tNew
    End If
   Next
   If bTrace Then System.Web.HttpContext.Current.Trace.Warn("PhoneFormat", "New International Phone=" + tNew.ToString())
  End If
  Return tNew
 End Function

 ' Encodes the password hash for user accounts.
 Public Function CodePassword(ByVal strPassword As String, ByVal strSalt As String) As String
  Dim PasswordHash As String
  PasswordHash = ""

  Dim bPass(), hPass(), bSalt() As Byte
  bPass = Encoding.UTF8.GetBytes(strPassword)
  Dim MD5 As New System.Security.Cryptography.MD5CryptoServiceProvider
  hPass = MD5.ComputeHash(bPass)
  bSalt = Encoding.UTF8.GetBytes(BitConverter.ToString(hPass).Replace("-", "").ToString().ToLower() + ":" + strSalt.ToString())
  PasswordHash = BitConverter.ToString(MD5.ComputeHash(bSalt)).Replace("-", "").ToString().ToLower()

  Return PasswordHash.ToString()
 End Function

 ' Activates a Javascript Alert message box to display a message when the page reloads.
 Public Sub AlertMessage(ByRef aspxPage As System.Web.UI.Page, ByVal strMessage As String, ByVal strKey As String)
  Dim strScript As String
  If strMessage.ToString().Trim().Length > 100 Then     ' Convert for web page display of content
   strMessage = strMessage.Replace(vbCrLf.ToString(), "<br />")
   strMessage = strMessage.Replace("\r\n", "<br />")
  Else                                                  ' Convert for Alert window display
   strMessage = strMessage.Replace(vbCrLf.ToString(), "\r\n")
  End If
  If strMessage.IndexOf("'", 1) > 0 Then                ' escape quotes
   strMessage = strMessage.Replace("'", "\'")
  End If
  strMessage = ("'").ToString() + strMessage + ("'").ToString()

  strScript = "var aMsg=" + strMessage.ToString() + ";" + vbCrLf +
              "function ShowMsg() {" + vbCrLf +
              " if (aMsg.length>0) {" + vbCrLf +
              "  if (aMsg.length<=100) {" + vbCrLf +
              "   alert(aMsg);" + vbCrLf +
              "  }" + vbCrLf +
              "  else {" + vbCrLf +
              "   oErrWin = window.open('','ErrWin','width=840,height=600,scrollbars=yes,resizable=yes');" + vbCrLf +
              "   oErrWin.document.write(aMsg);" + vbCrLf +
              "  }" + vbCrLf +
              " }" + vbCrLf +
              "}" + vbCrLf
  If (Not aspxPage.ClientScript.IsStartupScriptRegistered(strKey)) Then
   aspxPage.ClientScript.RegisterStartupScript(aspxPage.GetType, strKey, strScript, True) ' Wrap javascript in Script tags
  End If
 End Sub

 Public Sub LoadConfig(tLine As String, MyDB As MySQLLib)
  Dim tSQL, tFields(), SQLCmd, SQLFields, SQLValues As String
  tFields = tLine.Split(Chr(9))
  ' Check for existing record
  Dim GetControl As MySqlClient.MySqlDataReader
  SQLCmd = "Select Control,Name,Parm1,Parm2,Nbr1,Nbr2 " +
           "From control " +
           "Where Control=" + MyDB.SQLStr(tFields(0)) + " and Name=" + MyDB.SQLStr(tFields(1))
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("LoadConfig", "Get display Values SQLCmd: " + SQLCmd.ToString())
  GetControl = MyDB.GetReader("MySite", SQLCmd)
  If GetControl.HasRows() Then
   GetControl.Read()
   tSQL = ""
   If GetControl("Parm1").ToString().Trim() <> tFields(2).ToString().Trim() Then
    tSQL = tSQL.ToString() + "Parm1=" + MyDB.SQLStr(tFields(2))
   ElseIf GetControl("Parm2").ToString().Trim() <> tFields(3).ToString().Trim() Then
    If tSQL.ToString().Trim().Length > 0 Then
     tSQL = tSQL.ToString() + ","
    End If
    tSQL = tSQL.ToString() + "Parm2=" + MyDB.SQLStr(tFields(3))
   ElseIf GetControl("Nbr1").ToString().Trim() <> tFields(4).ToString().Trim() Then
    If tSQL.ToString().Trim().Length > 0 Then
     tSQL = tSQL.ToString() + ","
    End If
    tSQL = tSQL.ToString() + "Nbr1=" + MyDB.SQLNo(tFields(4))
   ElseIf GetControl("Nbr2").ToString().Trim() <> tFields(5).ToString().Trim() Then
    If tSQL.ToString().Trim().Length > 0 Then
     tSQL = tSQL.ToString() + ","
    End If
    tSQL = tSQL.ToString() + "Nbr2=" + MyDB.SQLNo(tFields(5))
   End If
   If tSQL.ToString().Trim().Length > 0 Then            ' There is some content difference to update!
    SQLCmd = "Update control Set " +
             tSQL.ToString() + " " +
             "Where Control=" + MyDB.SQLStr(tFields(0)) + " and Name=" + MyDB.SQLStr(tFields(1))
    If bTrace Then System.Web.HttpContext.Current.Trace.Warn("LoadConfig", "Update control SQLCmd: " + SQLCmd.ToString())
    MyDB.DBCmd("MySite", SQLCmd)
    If bTrace And MyDB.Error() Then System.Web.HttpContext.Current.Trace.Warn("LoadConfig", "DB Error: " + MyDB.ErrMessage())
   End If
  Else                                                  ' Entry was not found, insert it
   SQLFields = "Control,Name,Parm1,Parm2,Nbr1,Nbr2"
   SQLValues = MyDB.SQLStr(tFields(0)) + "," + MyDB.SQLStr(tFields(1)) + "," +
               MyDB.SQLStr(tFields(2)) + "," + MyDB.SQLStr(tFields(3)) + "," +
               MyDB.SQLNo(tFields(4)) + "," + MyDB.SQLNo(tFields(5))
   SQLCmd = "Insert Into control (" + SQLFields + ") Values (" + SQLValues + ")"
   If bTrace Then System.Web.HttpContext.Current.Trace.Warn("LoadConfig", "Insert control SQLCmd: " + SQLCmd.ToString())
   MyDB.DBCmd("MySite", SQLCmd)
   If bTrace And MyDB.Error() Then System.Web.HttpContext.Current.Trace.Warn("LoadConfig", "DB Error: " + MyDB.ErrMessage())
  End If
  GetControl.Close()

 End Sub

 Public Function CreateInvFolders(UserUUID As String, ClothingUUID As String, CurrentUUID As String, OutfitsUUID As String, MyDB As MySQLLib) As String
  Dim tErr, SQLCmd, SQLFields, SQLValues As String
  tErr = ""
  ' Create inventory folders default list for Avatar starting with the My Inventory as the root folder
  Dim FolderID As String
  FolderID = Guid.NewGuid().ToString()
  SQLFields = "folderName,type,version,folderID,agentID,parentFolderID"
  SQLValues = "'My Inventory',8,13," + MyDB.SQLStr(FolderID) + "," +
                MyDB.SQLStr(UserUUID) + ",'00000000-0000-0000-0000-000000000000'"
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create My Inventory folder DB Error: " + MyDB.ErrMessage() + "\r\n" +
               "SQLCmd: " + SQLCmd.ToString() + "\r\n"
  End If
  SQLValues = "'Animations',20,1," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Animations folder DB Error: " + MyDB.ErrMessage()
  End If
  SQLValues = "'Body Parts',13,1," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Body Parts folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'Calling Cards',2,1," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Calling Cards folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'Clothing',5,1," + MyDB.SQLStr(ClothingUUID) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Clothing folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'Current Outfit',46,1," + MyDB.SQLStr(CurrentUUID) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Current Outfit folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'Favorites',23,1," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Favorites folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'Gestures',21,1," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Gestures folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'Landmarks',3,1," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Landmarks folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'Lost And Found',16,1," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Lost And Found folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'Notecards',7,1," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Notecards folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'Objects',6,1," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Objects folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'My Outfits',48,1," + MyDB.SQLStr(OutfitsUUID) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create My Outfits folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'Photo Album',15,1," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Photo Album folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'Scripts',10,1," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Scripts folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'Sounds',1,1," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Sounds folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'Textures',0,1," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Textures folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If
  SQLValues = "'Trash',14,1," + MyDB.SQLStr(Guid.NewGuid().ToString()) + "," +
                 MyDB.SQLStr(UserUUID) + "," + MyDB.SQLStr(FolderID)
  SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
  If bTrace Then System.Web.HttpContext.Current.Trace.Warn("Register", "Insert inventoryfolders SQLCmd: " + SQLCmd.ToString())
  MyDB.DBCmd("MyData", SQLCmd)
  If MyDB.Error() Then
   tErr = tErr.ToString() + "** Create Trash folder DB Error: " + MyDB.ErrMessage() + "\r\n"
  End If

  Return tErr.ToString()
 End Function
 Public Sub Close()
  bTrace = Nothing
  Finalize()
 End Sub

 ' Used on close to terminate object
 Protected Overrides Sub Finalize()
  MyBase.Finalize()
 End Sub
End Class
