Imports System.Web.Services

<WebService(Namespace:="MMWebService.asmx")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<CompilerServices.DesignerGenerated()>
Public Class Service
 Inherits WebService
 ' This is the primary API access to the Halcyon simulator system for Mundos Market access. 
 ' Services can only be used by Mundos Market for registered World Owners. 
 ' NOTE: .Net Reference files have shown a new tendency to be case sensitive with the Service URL address!
 Private SQLCmd As String

 ' Security Key Validation:
 ' A Secure key is created in the MyWorld administration for the MMWebservice. That key is also to be placed in the 
 ' World owners Mundos Market account for this service entry along with its URL.
 ' An option next to the key can request to change the key and the result will have to be copied to the Mundos Market 
 ' account.

 Private Function ValidKey(ByVal AccessKey As String) As Boolean
  ' Verify has access to the web services
  Dim MyDB As New MySQLLib
  Dim Access As Boolean
  Access = False

  ' Get Mundos Market Access Key
  Dim GetMMKey As MySqlDataReader
  SQLCmd = "Select Parm2 " +
           "From control " +
           "Where Control='WebService' and Parm1='MMWebService'"
  GetMMKey = MyDB.GetReader("MySite", SQLCmd)
  If MyDB.Error() Then
   SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
            "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
   MyDB.DBCmd("MySite", SQLCmd)
  Else
   If GetMMKey.HasRows() Then
    GetMMKey.Read()
    If GetMMKey("Parm2").ToString().Trim().Length > 0 Then ' Key entry may not be empty
     Access = (GetMMKey("Parm2").ToString().Trim() = AccessKey.ToString().Trim())
    End If
   End If
  End If
  GetMMKey.Close()

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing

  Return Access
 End Function

 ' MakeInvFolder("UUID","foldername",Key) as string (FolderUUID)
 Private Function MakeInvFolder(ByVal UUID As String, ByVal FolderName As String) As String
  ' Provide Access to account notes for selected account.
  Dim MyDB As New MySQLLib
  Dim SQLFields, SQLValues, FolderID As String
  FolderID = ""

  ' Gets the folderID for the My Inventory top level folder in the Account to create a new user folder in.
  Dim GetInv As MySqlDataReader
  ' Get top inventory folder for the account
  SQLCmd = "Select folderID " +
           "From inventoryfolders " +
           "Where parentFolderID='00000000-0000-0000-0000-000000000000' and agentID=" + MyDB.SQLStr(UUID)
  GetInv = MyDB.GetReader("MyData", SQLCmd)
  If MyDB.Error() Then
   SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
            "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
   MyDB.DBCmd("MySite", SQLCmd)
  Else
   If GetInv.HasRows() Then
    GetInv.Read()
    ' Create new inventory folder with Name. Duplicate folder names allowed.
    FolderID = Guid.NewGuid().ToString()
    SQLFields = "folderName,type,version,folderID,agentID,parentFolderID"
    SQLValues = MyDB.SQLStr(FolderName) + ",-1,1," + MyDB.SQLStr(FolderID) + "," +
                MyDB.SQLStr(UUID) + "," + MyDB.SQLStr(GetInv("folderID"))
    SQLCmd = "Insert Into inventoryfolders (" + SQLFields + ") Values (" + SQLValues + ")"
    MyDB.DBCmd("MyData", SQLCmd)
    If MyDB.Error() Then
     SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
              "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
     MyDB.DBCmd("MySite", SQLCmd)
    End If
   End If
  End If
  GetInv.Close()
  GetInv.Dispose()

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing
  Return FolderID
 End Function

 ' ItemValid("UUID","ItemID",Key)) as integer
 <WebMethod(Description:="Validates existence of an inventory item for account.")>
 Public Function ItemValid(ByVal UUID As String, ByVal AssetID As String, ByVal AccessKey As String) As String
  Dim MyDB As New MySQLLib
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String
  Out(1) = "Error"

  If ValidKey(AccessKey) Then
   Dim ChkInv As MySqlDataReader
   SQLCmd = "Select Count(inventoryID) as ItmCnt From inventoryitems " +
            "Where avatarID=" + MyDB.SQLStr(UUID) + " and assetID=" + MyDB.SQLStr(AssetID)
   ChkInv = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
             "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
    MyDB.DBCmd("MySite", SQLCmd)
    ErrorCode = 4                                    ' DB Error
    Out(1) = "DB Error"
   Else
    If ChkInv.HasRows() Then
     ChkInv.Read()
     If ChkInv("ItmCnt") = 0 Then
      ErrorCode = 3                                  ' Item not found
      Out(1) = "Item not found"
     End If
    Else
     ErrorCode = 2                                   ' AssetID not found
     Out(1) = "AssetID not found"
    End If
   End If
   ChkInv.Close()
  Else
   ErrorCode = 1                                     ' Invalid Key
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing
  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 ' Authentication("first.last","pHash",AccessKey) as String (User UUID)
 <WebMethod(Description:="Validates account logon.")>
 Public Function Authentication(ByVal AcctName As String, ByVal pHash As String, ByVal AccessKey As String) As String
  Dim MyDB As New MySQLLib
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String
  Out(1) = "Error"

  If ValidKey(AccessKey) Then
   Dim Name() As String
   Name = AcctName.Split(".")
   Dim ChkUser As MySqlDataReader
   SQLCmd = "Select UUID,passwordHash From users " +
            "Where username=" + MyDB.SQLStr(Name(0)) + " and lastname=" + MyDB.SQLStr(Name(1))
   ChkUser = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
             "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
    MyDB.DBCmd("MySite", SQLCmd)
    ErrorCode = 4                                    ' DB Error
    Out(1) = "DB Error"
   Else
    If ChkUser.HasRows() Then
     ChkUser.Read()
     If pHash.ToString().Trim() = ChkUser("passwordHash").ToString().Trim() Then
      Out(1) = ChkUser("UUID").ToString()
     Else
      ' Bad Password
      ErrorCode = 3
      Out(1) = "Bad Password"
     End If
    Else
     ' Account name not found
     ErrorCode = 2
     Out(1) = "Account not found"
    End If
   End If
   ChkUser.Close()
  Else
   ' Invalid Key
   ErrorCode = 1
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing
  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 ' MakeInventory("UUID","ProdName",Key)
 <WebMethod(Description:="Creates a product folder for buyer. Returns folderID.")>
 Public Function MakeInventory(ByVal UUID As String, ByVal ProdName As String, ByVal AccessKey As String) As String
  ' Creates a new product folder in the buyer's inventory and returns that folderID.
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String

  If ValidKey(AccessKey) Then
   Out(1) = MakeInvFolder(UUID, ProdName)
   If Out(1).ToString().Trim().Length = 0 Then
    ErrorCode = 2                                    ' DB Error
    Out(1) = "DB Error"
   End If
  Else
   ErrorCode = 1                                     ' Invalid Key
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 ' LoadInventory("UUID","FolderID",ItemRow,Key) as integer
 <WebMethod(Description:="Load new individual inventory items for buyer.")>
 Public Function LoadInventory(ByVal UUID As String, ByVal FolderID As String, ByVal ItemLists As DataSet, ByVal AccessKey As String) As String
  ' Provide Access to account notes for selected account.
  Dim MyDB As New MySQLLib
  Dim SQLFields, SQLValues, InvUUID As String
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String
  Out(1) = "Success"

  If ValidKey(AccessKey) Then
   ' Place worn items into Clothing/Avatar folder - Assign the Next Permissions as Current Permissions
   Dim ItemRow As DataRow
   For Each ItemRow In ItemLists.Tables("Items").Rows()
    InvUUID = Guid.NewGuid().ToString()
    SQLFields = "assetID,assetType,inventoryName,inventoryDescription,inventoryNextPermissions," +
                "inventoryCurrentPermissions,invType,creatorID,inventoryBasePermissions," +
                "inventoryEveryOnePermissions,salePrice,saleType,creationDate,groupID,groupOwned,flags," +
                "inventoryID,avatarID,parentFolderID,inventoryGroupPermissions"
    SQLValues = MyDB.SQLStr(ItemRow("assetID")) + "," + MyDB.SQLNo(ItemRow("assetType")) + "," +
                MyDB.SQLStr(ItemRow("inventoryName")) + "," + MyDB.SQLStr(ItemRow("inventoryDescription")) + "," +
                MyDB.SQLNo(ItemRow("inventoryNextPermissions")) + "," + MyDB.SQLNo(ItemRow("inventoryNextPermissions")) + "," +
                MyDB.SQLNo(ItemRow("invType")) + "," + MyDB.SQLStr(ItemRow("creatorID")) + "," +
                MyDB.SQLNo(ItemRow("inventoryBasePermissions")) + "," + MyDB.SQLNo(ItemRow("inventoryEveryOnePermissions")) + "," +
                MyDB.SQLNo(ItemRow("salePrice")) + "," + MyDB.SQLNo(ItemRow("saleType")) + "," +
                MyDB.SQLStr(ItemRow("creationDate")) + "," + MyDB.SQLStr(ItemRow("groupID")) + "," +
                MyDB.SQLNo(ItemRow("groupOwned")) + "," + MyDB.SQLNo(ItemRow("flags")) + "," +
                MyDB.SQLStr(InvUUID) + "," + MyDB.SQLStr(UUID) + "," +
                MyDB.SQLStr(FolderID) + "," + MyDB.SQLNo(ItemRow("inventoryGroupPermissions"))
    SQLCmd = "Insert Into inventoryitems (" + SQLFields + ") Values (" + SQLValues + ")"
    MyDB.DBCmd("MyData", SQLCmd)
    If MyDB.Error() Then
     SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
              "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
     MyDB.DBCmd("MySite", SQLCmd)
     ErrorCode = 2                                    ' DB Error
     Out(1) = "DB Error"
    End If
   Next
  Else
   ErrorCode = 1                                     ' Invalid Key
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing

  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 'ListCreator("UUID",Key)
 <WebMethod(Description:="Determine if user account exists in this world.")>
 Public Function ListCreator(ByVal UUID As String, ByVal AccessKey As String) As String
  ' Return if account exists or not in this world. Checks UUID match.
  Dim MyDB As New MySQLLib
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String
  Out(1) = "Error"

  If ValidKey(AccessKey) Then
   Dim drList As MySqlDataReader
   ' NOTE: Returns Account info if exists.
   SQLCmd = "Select username,lastname,PasswordHash " +
            "From users " +
            "Where UUID=" + MyDB.SQLStr(UUID)
   drList = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
             "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
    MyDB.DBCmd("MySite", SQLCmd)
    ErrorCode = 3                                    ' DB Error
    Out(1) = "DB Error"
   Else
    If drList.HasRows() Then
     drList.Read()
     If drList("PasswordHash").ToString().Trim() = "NoPass" Then
      Out(1) = "Reference"
     Else
      Out(1) = drList("username").ToString().Trim() + "." + drList("lastname").ToString().Trim()
     End If
    Else
     ErrorCode = 2                                   ' Item Not found
     Out(1) = "Account Not found"
    End If
   End If
   drList.Close()
   drList.Dispose()
  Else
   ErrorCode = 1                                     ' Invalid Key
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing
  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 ' AddCreator("UUID","first.last",Key) 
 <WebMethod(Description:="Add Creator reference account.")>
 Public Function AddCreator(ByVal UUID As String, ByVal AcctName As String, ByVal AccessKey As String) As String
  ' Create Creator reference account for items added to the world buyers account.
  Dim MyDB As New MySQLLib
  Dim ExistUUID As String
  ExistUUID = ""
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String
  Out(1) = "Error"

  If ValidKey(AccessKey) Then
   Dim Name() As String
   Name = AcctName.Split(".")
   Dim ChkUser As MySqlDataReader
   SQLCmd = "Select UUID,passwordHash From users " +
            "Where username=" + MyDB.SQLStr(Name(0)) + " and lastname=" + MyDB.SQLStr(Name(1))
   ChkUser = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
             "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
    MyDB.DBCmd("MySite", SQLCmd)
    ErrorCode = 4                                    ' DB Error
    Out(1) = "DB Error - Get users"
   Else
    If ChkUser.HasRows() Then
     ChkUser.Read()
     If UUID = ChkUser("UUID").ToString() Then
      ErrorCode = 2                                  ' Account exists
      Out(1) = "Account exists"
     Else
      ErrorCode = 3                                  ' Account UUID Mismatch
      Out(1) = ChkUser("UUID").ToString()
     End If
    Else
     Dim SQLFields, SQLValues As String
     ' Create reference account.
     SQLFields = "UUID, username, lastname, passwordHash, passwordSalt, homeRegion, homeLocationX, " +
                 "homeLocationY, homeLocationZ, homeLookAtX, homeLookAtY, homeLookAtZ, created, " +
                 "lastLogin, userInventoryURI, userAssetURI, profileAboutText, profileFirstText, " +
                 "profileImage, profileFirstImage, webLoginKey, homeRegionID, userFlags, godLevel, " +
                 "iz_level, customType, partner, email, profileURL, skillsMask, skillsText, " +
                 "wantToMask, wantToText, languagesText"
     SQLValues = "(" + MyDB.SQLStr(UUID) + "," + MyDB.SQLStr(Name(0)) + "," + MyDB.SQLStr(Name(1)) + "," +
                 "'NoPass','',0,0,0,0,0,0,0,UNIX_TIMESTAMP(),0,'','','',''," +
                 "'00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000'," +
                 "'00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000',0,0,0,''," +
                 "'00000000-0000-0000-0000-000000000000','','',0,'None',0,'None','English')"
     SQLCmd = "Insert into users (" + SQLFields + ") Values (" + SQLValues + ")"
     MyDB.DBCmd("MyData", SQLCmd)
     If MyDB.Error() Then
      SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
               "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
      MyDB.DBCmd("MySite", SQLCmd)
      ErrorCode = 4                                  ' DB Error
      Out(1) = "DB Error - Insert users"
     Else
      ' Insert Reference Account into agents table
      SQLFields = "UUID,sessionID,secureSessionID,agentIP,agentPort,agentOnline,loginTime,logoutTime," +
                  "currentRegion,currentHandle,currentPos,currentLookAt"
      SQLValues = SQLValues.ToString() +
                  "(" + MyDB.SQLStr(UUID) + ",UUID(),UUID(),'127.0.0.1','0','0','0','0'," +
                  "'00000000-0000-0000-0000-000000000000','0','<0,0,0>','<0,0,0>')"
      SQLCmd = "Insert into agents (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
      MyDB.DBCmd("MyData", SQLCmd)
      If MyDB.Error() Then
       SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
                "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
       MyDB.DBCmd("MySite", SQLCmd)
       ErrorCode = 4                                 ' DB Error
       Out(1) = "DB Error - Insert agents"
      Else
       ' Insert Reference Account into userpreferences table
       SQLFields = "user_ID,recv_ims_via_email,listed_in_directory"
       SQLValues = SQLValues.ToString() +
                   "(" + MyDB.SQLStr(UUID) + ",1,0)"
       SQLCmd = "Insert into userpreferences (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
       MyDB.DBCmd("MyData", SQLCmd)
       If MyDB.Error() Then
        SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
                 "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
        MyDB.DBCmd("MySite", SQLCmd)
        ErrorCode = 4                                ' DB Error
        Out(1) = "DB Error - Insert userpreferences"
       Else
        ' Insert Reference Account into inventoryfolders table
        SQLFields = "folderName,type,version,folderID,agentID,parentFolderID"
        SQLValues = SQLValues.ToString() +
                    "('My Inventory',8,13,UUID()," + MyDB.SQLStr(UUID) + ",'00000000-0000-0000-0000-000000000000')"
        SQLCmd = "Insert into inventoryfolders (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
        MyDB.DBCmd("MyData", SQLCmd)
        If MyDB.Error() Then
         SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
                  "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
         MyDB.DBCmd("MySite", SQLCmd)
         ErrorCode = 4                               ' DB Error
         Out(1) = "DB Error - Insert inventoryfolders"
        End If
       End If
      End If
     End If
    End If
   End If
   ChkUser.Close()
   ChkUser.Dispose()
  Else
   ErrorCode = 1                                     ' Invalid Key
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing
  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 'UpdateCreator("UUID",Action,Key)
 <WebMethod(Description:="Update Creator Account Status.")>
 Public Function UpdateCreator(ByVal UUID As String, ByVal Action As String, ByVal AccessKey As String) As String
  ' Change Creator account status in this world from a Reference Account to Active Account OR Active Account to a Reference Account.
  Dim MyDB As New MySQLLib
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String
  Out(1) = "Error"

  ' Action - PasswordHash if active, "NoPass" if Reference
  If ValidKey(AccessKey) Then
   Dim drList As MySqlDataReader
   ' NOTE: Returns Account entry to update.
   SQLCmd = "Select passwordHash From users " +
            "Where UUID=" + MyDB.SQLStr(UUID)
   drList = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
             "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
    MyDB.DBCmd("MySite", SQLCmd)
    ErrorCode = 3                                    ' DB Error
    Out(1) = "DB Error - Get users"
   Else
    If drList.HasRows() Then
     drList.Read()
     If drList("passwordHash").ToString().Trim() <> Action.ToString().Trim() Then ' State to change...
      SQLCmd = "Update users Set passwordHash=" + MyDB.SQLStr(Action) + " Where UUID=" + MyDB.SQLStr(UUID)
      MyDB.DBCmd("MyData", SQLCmd)
      If MyDB.Error() Then
       SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
                "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
       MyDB.DBCmd("MySite", SQLCmd)
       ErrorCode = 3                                 ' DB Error
       Out(1) = "DB Error - Update users"
      End If
     End If
    Else
     ErrorCode = 2                                   ' Item Not found
     Out(1) = "Account was not found."
    End If
   End If
   drList.Close()
   drList.Dispose()
  Else
   ErrorCode = 1                                     ' Invalid Key
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing
  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 'ListGroups("UUID",GroupIDList,Key)
 <WebMethod(Description:="List of Store Groups.")>
 Public Function ListGroups(ByVal UUID As String, ByVal GroupIDList As String, ByVal AccessKey As String) As String
  ' Provide Access to account notes for selected account.
  Dim MyDB As New MySQLLib
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String
  Out(1) = "Error"

  If ValidKey(AccessKey) Then
   Dim drList As MySqlDataReader
   ' NOTE: Returns List of groups owned by UUID.
   SQLCmd = "Select GroupID,Name,Charter,InsigniaID,MembershipFee,OpenEnrollment,ShowInList,AllowPublish,MaturePublish,OwnerRoleID " +
            "From osgroup " +
            "Where FounderID=" + MyDB.SQLStr(UUID) + " and GroupID in (" + GroupIDList.ToString() + ") " +
            "Order by Name"
   drList = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
             "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
    MyDB.DBCmd("MySite", SQLCmd)
    ErrorCode = 3                                    ' DB Error
    Out(1) = "DB Error"
   Else
    If drList.HasRows() Then
     Dim dtDataOut As New DataTable
     dtDataOut.TableName = "Groups"                  ' Assign a table name
     dtDataOut.AcceptChanges()
     dtDataOut.Load(drList)                          ' Load processing table with records selected
     dtDataOut.AcceptChanges()                       ' Save added data to table
     Dim DataXML = New IO.StringWriter
     dtDataOut.WriteXml(DataXML, False)              ' Convert the XML doc to text
     Out(1) = DataXML.ToString()
     dtDataOut.Dispose()
    Else
     ErrorCode = 2                                   ' Item Not found
     Out(1) = "Groups Not found"
    End If
   End If
   drList.Close()
   drList.Dispose()
  Else
   ErrorCode = 1                                     ' Invalid Key
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing
  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 'AddGroup(Settings,Key)
 <WebMethod(Description:="Add a Store Group.")>
 Public Function AddGroup(ByVal Settings As DataSet, ByVal AccessKey As String) As String
  ' Provide Access to account notes for selected account.
  Dim MyDB As New MySQLLib
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String
  Out(1) = "Error"

  If ValidKey(AccessKey) Then
   Dim ItemRow As DataRow
   ItemRow = Settings.Tables("Settings").Rows().Item(0) ' One row only

   Dim drList As MySqlDataReader
   ' NOTE: Returns nothing or the group info.
   SQLCmd = "Select GroupID,Name " +
            "From osgroup " +
            "Where GroupID=" + MyDB.SQLStr(ItemRow("GroupID"))
   drList = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
             "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
    MyDB.DBCmd("MySite", SQLCmd)
    ErrorCode = 3                                    ' DB Error
    Out(1) = "DB Error - Get osgroup"
   Else
    If drList.HasRows() Then
     ErrorCode = 2                                   ' Group exists Error
     Out(1) = "Group exists!"
    Else                                             ' Create New group Entry
     Dim SQLFields, SQLValues As String
     SQLFields = "GroupID,Name,Charter,InsigniaID,FounderID,MembershipFee,OpenEnrollment,ShowInList," +
                 "AllowPublish,MaturePublish,OwnerRoleID"
     SQLValues = MyDB.SQLStr(ItemRow("GroupID")) + "," + MyDB.SQLStr(ItemRow("Name")) + "," +
                 MyDB.SQLStr(ItemRow("Charter")) + "," + MyDB.SQLStr(ItemRow("InsigniaID")) + "," +
                 MyDB.SQLStr(ItemRow("FounderID")) + "," + MyDB.SQLNo(ItemRow("MembershipFee")) + "," +
                 MyDB.SQLNo(ItemRow("OpenEnrollment")) + "," + MyDB.SQLNo(ItemRow("ShowInList")) + "," +
                 MyDB.SQLNo(ItemRow("AllowPublish")) + "," + MyDB.SQLNo(ItemRow("MaturePublish")) + "," +
                 MyDB.SQLStr(ItemRow("OwnerRoleID"))
     SQLCmd = "Insert into osgroup (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
     MyDB.DBCmd("MyData", SQLCmd)
     If MyDB.Error() Then
      SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
               "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
      MyDB.DBCmd("MySite", SQLCmd)
      ErrorCode = 3                                  ' DB Error
      Out(1) = "DB Error - Insert osgroup"
     End If
    End If
   End If
   drList.Close()
   drList.Dispose()
  Else
   ErrorCode = 1                                     ' Invalid Key
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing
  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 'UpdateGroup("GrpID",Settings,Key)
 '<WebMethod(Description:="Description of function purpose.")>
 'Public Function MyFunction(ByVal AccessKey As String) As String
 ' ' Provide Access to account notes for selected account.
 ' Dim MyDB As New MySQLLib
 ' Dim ErrorCode As Integer
 ' ErrorCode = 0
 ' Dim Out(1) As String
 ' Out(1) = "Error"

 ' If ValidKey(AccessKey) Then
 '  Dim drList As MySqlDataReader
 '  ' NOTE: Returns something.
 '  SQLCmd = "Select Fields " +
 '           "From table " +
 '           "Where Condition=" + MyDB.SQLStr(input) + " " +
 '           "Order by Field"
 '  drList = MyDB.GetReader("MyData", SQLCmd)
 '  If MyDB.Error() Then
 '   SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
 '            "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
 '   MyDB.DBCmd("MySite", SQLCmd)
 '   ErrorCode = 4                                    ' DB Error
 '   Out(1) = "DB Error"
 '  Else
 '   If drList.HasRows() Then
 '    drList.Read()
 '    If False Then
 '     ErrorCode = 3                                  ' Error
 '     Out(1) = ""
 '    End If
 '   Else
 '    ErrorCode = 2                                   ' Item Not found
 '    Out(1) = "Item Not found"
 '   End If
 '  End If
 '  drList.Close()
 '  drList.Dispose()
 ' Else
 '  ErrorCode = 1                                     ' Invalid Key
 '  Out(1) = "Invalid Key"
 ' End If

 ' ' Close open objects
 ' MyDB.Close()
 ' MyDB = Nothing
 ' Out(0) = ErrorCode.ToString()
 ' Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 'End Function

 'ListGroupRoles("GrpID",Key)
 '<WebMethod(Description:="Description of function purpose.")>
 'Public Function MyFunction(ByVal AccessKey As String) As String
 ' ' Provide Access to account notes for selected account.
 ' Dim MyDB As New MySQLLib
 ' Dim ErrorCode As Integer
 ' ErrorCode = 0
 ' Dim Out(1) As String
 ' Out(1) = "Error"

 ' If ValidKey(AccessKey) Then
 '  Dim drList As MySqlDataReader
 '  ' NOTE: Returns something.
 '  SQLCmd = "Select Fields " +
 '           "From table " +
 '           "Where Condition=" + MyDB.SQLStr(input) + " " +
 '           "Order by Field"
 '  drList = MyDB.GetReader("MyData", SQLCmd)
 '  If MyDB.Error() Then
 '   SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
 '            "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
 '   MyDB.DBCmd("MySite", SQLCmd)
 '   ErrorCode = 4                                    ' DB Error
 '   Out(1) = "DB Error"
 '  Else
 '   If drList.HasRows() Then
 '    drList.Read()
 '    If False Then
 '     ErrorCode = 3                                  ' Error
 '     Out(1) = ""
 '    End If
 '   Else
 '    ErrorCode = 2                                   ' Item Not found
 '    Out(1) = "Item Not found"
 '   End If
 '  End If
 '  drList.Close()
 '  drList.Dispose()
 ' Else
 '  ErrorCode = 1                                     ' Invalid Key
 '  Out(1) = "Invalid Key"
 ' End If

 ' ' Close open objects
 ' MyDB.Close()
 ' MyDB = Nothing
 ' Out(0) = ErrorCode.ToString()
 ' Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 'End Function

 'AddGroupRole(Settings,Key)
 <WebMethod(Description:="Add a Group Role to a Group.")>
 Public Function AddGroupRole(ByVal Settings As DataSet, ByVal AccessKey As String) As String
  ' Provide Access to account notes for selected account.
  Dim MyDB As New MySQLLib
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String
  Out(1) = "Error"

  If ValidKey(AccessKey) Then
   Dim ItemRow As DataRow
   ItemRow = Settings.Tables("Settings").Rows().Item(0) ' One row only

   Dim drList As MySqlDataReader
   ' NOTE: Returns something.
   SQLCmd = "Select GroupID,RoleID,Name,Description,Title,Powers " +
            "From osrole " +
            "Where GroupID=" + MyDB.SQLStr(ItemRow("GroupID")) + " and RoleID=" + MyDB.SQLStr(ItemRow("RoleID"))
   drList = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
             "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
    MyDB.DBCmd("MySite", SQLCmd)
    ErrorCode = 3                                   ' DB Error
    Out(1) = "DB Error - Get osrole"
   Else
    If drList.HasRows() Then
     ErrorCode = 2                                 ' Group role exists Error
     Out(1) = "Group role exists!"
    Else
     Dim SQLFields, SQLValues As String
     SQLFields = "GroupID,RoleID,Name,Description,Title,Powers"
     SQLValues = MyDB.SQLStr(ItemRow("GroupID")) + "," + MyDB.SQLStr(ItemRow("RoleID")) + "," +
                 MyDB.SQLStr(ItemRow("Name")) + "," + MyDB.SQLStr(ItemRow("Description")) + "," +
                 MyDB.SQLStr(ItemRow("Title")) + "," + MyDB.SQLNo(ItemRow("Powers"))
     SQLCmd = "Insert into osrole (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
     MyDB.DBCmd("MyData", SQLCmd)
     If MyDB.Error() Then
      SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
               "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
      MyDB.DBCmd("MySite", SQLCmd)
      ErrorCode = 3                                  ' DB Error
      Out(1) = "DB Error - Insert osrole"
     End If
    End If
   End If
   drList.Close()
   drList.Dispose()
  Else
   ErrorCode = 1                                     ' Invalid Key
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing
  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 'UpdateGroupRole("GrpID","RoleID",Settings,Key)
 '<WebMethod(Description:="Description of function purpose.")>
 'Public Function MyFunction(ByVal AccessKey As String) As String
 ' ' Provide Access to account notes for selected account.
 ' Dim MyDB As New MySQLLib
 ' Dim ErrorCode As Integer
 ' ErrorCode = 0
 ' Dim Out(1) As String
 ' Out(1) = "Error"

 ' If ValidKey(AccessKey) Then
 '  Dim drList As MySqlDataReader
 '  ' NOTE: Returns something.
 '  SQLCmd = "Select Fields " +
 '           "From table " +
 '           "Where Condition=" + MyDB.SQLStr(input) + " " +
 '           "Order by Field"
 '  drList = MyDB.GetReader("MyData", SQLCmd)
 '  If MyDB.Error() Then
 '   SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
 '            "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
 '   MyDB.DBCmd("MySite", SQLCmd)
 '   ErrorCode = 4                                    ' DB Error
 '   Out(1) = "DB Error"
 '  Else
 '   If drList.HasRows() Then
 '    drList.Read()
 '    If False Then
 '     ErrorCode = 3                                  ' Error
 '     Out(1) = ""
 '    End If
 '   Else
 '    ErrorCode = 2                                   ' Item Not found
 '    Out(1) = "Item Not found"
 '   End If
 '  End If
 '  drList.Close()
 '  drList.Dispose()
 ' Else
 '  ErrorCode = 1                                     ' Invalid Key
 '  Out(1) = "Invalid Key"
 ' End If

 ' ' Close open objects
 ' MyDB.Close()
 ' MyDB = Nothing
 ' Out(0) = ErrorCode.ToString()
 ' Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 'End Function

 'DelGroupRole("GrpID","RoleID",Key)
 '<WebMethod(Description:="Description of function purpose.")>
 'Public Function MyFunction(ByVal AccessKey As String) As String
 ' ' Provide Access to account notes for selected account.
 ' Dim MyDB As New MySQLLib
 ' Dim ErrorCode As Integer
 ' ErrorCode = 0
 ' Dim Out(1) As String
 ' Out(1) = "Error"

 ' If ValidKey(AccessKey) Then
 '  Dim drList As MySqlDataReader
 '  ' NOTE: Returns something.
 '  SQLCmd = "Select Fields " +
 '           "From table " +
 '           "Where Condition=" + MyDB.SQLStr(input) + " " +
 '           "Order by Field"
 '  drList = MyDB.GetReader("MyData", SQLCmd)
 '  If MyDB.Error() Then
 '   SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
 '            "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
 '   MyDB.DBCmd("MySite", SQLCmd)
 '   ErrorCode = 4                                    ' DB Error
 '   Out(1) = "DB Error"
 '  Else
 '   If drList.HasRows() Then
 '    drList.Read()
 '    If False Then
 '     ErrorCode = 3                                  ' Error
 '     Out(1) = ""
 '    End If
 '   Else
 '    ErrorCode = 2                                   ' Item Not found
 '    Out(1) = "Item Not found"
 '   End If
 '  End If
 '  drList.Close()
 '  drList.Dispose()
 ' Else
 '  ErrorCode = 1                                     ' Invalid Key
 '  Out(1) = "Invalid Key"
 ' End If

 ' ' Close open objects
 ' MyDB.Close()
 ' MyDB = Nothing
 ' Out(0) = ErrorCode.ToString()
 ' Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 'End Function

 'ListGroupMem("GrpID",Key)
 '<WebMethod(Description:="Description of function purpose.")>
 'Public Function MyFunction(ByVal AccessKey As String) As String
 ' ' Provide Access to account notes for selected account.
 ' Dim MyDB As New MySQLLib
 ' Dim ErrorCode As Integer
 ' ErrorCode = 0
 ' Dim Out(1) As String
 ' Out(1) = "Error"

 ' If ValidKey(AccessKey) Then
 '  Dim drList As MySqlDataReader
 '  ' NOTE: Returns something.
 '  SQLCmd = "Select Fields " +
 '           "From table " +
 '           "Where Condition=" + MyDB.SQLStr(input) + " " +
 '           "Order by Field"
 '  drList = MyDB.GetReader("MyData", SQLCmd)
 '  If MyDB.Error() Then
 '   SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
 '            "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
 '   MyDB.DBCmd("MySite", SQLCmd)
 '   ErrorCode = 4                                    ' DB Error
 '   Out(1) = "DB Error"
 '  Else
 '   If drList.HasRows() Then
 '    drList.Read()
 '    If False Then
 '     ErrorCode = 3                                  ' Error
 '     Out(1) = ""
 '    End If
 '   Else
 '    ErrorCode = 2                                   ' Item Not found
 '    Out(1) = "Item Not found"
 '   End If
 '  End If
 '  drList.Close()
 '  drList.Dispose()
 ' Else
 '  ErrorCode = 1                                     ' Invalid Key
 '  Out(1) = "Invalid Key"
 ' End If

 ' ' Close open objects
 ' MyDB.Close()
 ' MyDB = Nothing
 ' Out(0) = ErrorCode.ToString()
 ' Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 'End Function

 'JoinGroup("GrpID","UUID",Key)
 <WebMethod(Description:="Join a store group.")>
 Public Function JoinGroup(ByVal Settings As DataSet, ByVal AccessKey As String) As String
  ' Add a member to a store group.
  Dim MyDB As New MySQLLib
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String
  Out(1) = "Error"

  If ValidKey(AccessKey) Then
   Dim ItemRow As DataRow
   ItemRow = Settings.Tables("Settings").Rows().Item(0) ' One row only

   Dim drList As MySqlDataReader
   ' NOTE: Returns Group membership if exists.
   SQLCmd = "Select GroupID,AgentID,SelectedRoleID,Contribution,ListInProfile,AcceptNotices " +
            "From osgroupmembership " +
            "Where GroupID=" + MyDB.SQLStr(ItemRow("GroupID")) + " and RoleID=" + MyDB.SQLStr(ItemRow("RoleID"))
   drList = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
             "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
    MyDB.DBCmd("MySite", SQLCmd)
    ErrorCode = 3                                    ' DB Error
    Out(1) = "DB Error - Read osgroupmembership"
   Else
    If drList.HasRows() Then
     ErrorCode = 2                                   ' Error
     Out(1) = "Membership exists!"
    Else
     Dim SQLFields, SQLValues As String
     SQLFields = "GroupID,AgentID,SelectedRoleID,Contribution,ListInProfile,AcceptNotices"
     SQLValues = MyDB.SQLStr(ItemRow("GroupID")) + "," + MyDB.SQLStr(ItemRow("AgentID")) + "," +
                 MyDB.SQLStr(ItemRow("SelectedRoleID")) + "," + MyDB.SQLNo(ItemRow("Contribution")) + "," +
                 MyDB.SQLNo(ItemRow("ListInProfile")) + "," + MyDB.SQLNo(ItemRow("AcceptNotices"))
     SQLCmd = "Insert into osgroupmembership (" + SQLFields.ToString() + ") Values (" + SQLValues.ToString() + ")"
     MyDB.DBCmd("MyData", SQLCmd)
     If MyDB.Error() Then
      SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
               "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
      MyDB.DBCmd("MySite", SQLCmd)
      ErrorCode = 3                                  ' DB Error
      Out(1) = "DB Error- Insert osgroupmembership"
     End If
    End If
   End If
   drList.Close()
   drList.Dispose()
  Else
   ErrorCode = 1                                     ' Invalid Key
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing
  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 'LeaveGroup("GrpID","UUID",Key)
 '<WebMethod(Description:="Description of function purpose.")>
 'Public Function MyFunction(ByVal AccessKey As String) As String
 ' ' Provide Access to account notes for selected account.
 ' Dim MyDB As New MySQLLib
 ' Dim ErrorCode As Integer
 ' ErrorCode = 0
 ' Dim Out(1) As String
 ' Out(1) = "Error"

 ' If ValidKey(AccessKey) Then
 '  Dim drList As MySqlDataReader
 '  ' NOTE: Returns something.
 '  SQLCmd = "Select Fields " +
 '           "From table " +
 '           "Where Condition=" + MyDB.SQLStr(input) + " " +
 '           "Order by Field"
 '  drList = MyDB.GetReader("MyData", SQLCmd)
 '  If MyDB.Error() Then
 '   SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
 '            "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
 '   MyDB.DBCmd("MySite", SQLCmd)
 '   ErrorCode = 4                                    ' DB Error
 '   Out(1) = "DB Error"
 '  Else
 '   If drList.HasRows() Then
 '    drList.Read()
 '    If False Then
 '     ErrorCode = 3                                  ' Error
 '     Out(1) = ""
 '    End If
 '   Else
 '    ErrorCode = 2                                   ' Item Not found
 '    Out(1) = "Item Not found"
 '   End If
 '  End If
 '  drList.Close()
 '  drList.Dispose()
 ' Else
 '  ErrorCode = 1                                     ' Invalid Key
 '  Out(1) = "Invalid Key"
 ' End If

 ' ' Close open objects
 ' MyDB.Close()
 ' MyDB = Nothing
 ' Out(0) = ErrorCode.ToString()
 ' Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 'End Function

 ' GetUserKey(Name,Key)
 <WebMethod(Description:="Get User key.")>
 Public Function GetUserKey(ByVal Name As String, ByVal AccessKey As String) As String
  ' Provide Access to account notes for selected account.
  Dim MyDB As New MySQLLib
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String
  Out(1) = "Error"

  If ValidKey(AccessKey) Then
   Dim aName() = Name.ToString().Split(".")

   Dim drList As MySqlDataReader
   ' NOTE: Returns something.
   SQLCmd = "Select passwordSalt " +
            "From users " +
            "Where username=" + MyDB.SQLStr(aName(0)) + " and lastname=" + MyDB.SQLStr(aName(1))
   drList = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
             "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
    MyDB.DBCmd("MySite", SQLCmd)
    ErrorCode = 3                                    ' DB Error
    Out(1) = "DB Error"
   Else
    If drList.HasRows() Then
     drList.Read()
     Out(1) = drList("passwordSalt").ToString()
    Else
     ErrorCode = 2                                   ' Item Not found
     Out(1) = "Account Not found"
    End If
   End If
   drList.Close()
   drList.Dispose()
  Else
   ErrorCode = 1                                     ' Invalid Key
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing
  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 ' GetGroupID("Name",Key)
 <WebMethod(Description:="Get GroupID by name.")>
 Public Function GetGroupID(ByVal Name As String, ByVal AccessKey As String) As String
  ' Determine if UUID is a member in GroupID Role Name
  Dim MyDB As New MySQLLib
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String
  Out(1) = "Error"

  If ValidKey(AccessKey) Then
   Dim drList As MySqlDataReader
   ' NOTE: Returns GroupID if Name is found.
   SQLCmd = "Select GroupID " +
            "From osgroup " +
            "Where Name=" + MyDB.SQLStr(Name)
   drList = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
             "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
    MyDB.DBCmd("MySite", SQLCmd)
    ErrorCode = 3                                    ' DB Error
    Out(1) = "DB Error"
   Else
    If drList.HasRows() Then
     drList.Read()
     Out(1) = drList("GroupID").ToString()
    Else
     ErrorCode = 2                                   ' Item Not found
     Out(1) = "Group Name not found"
    End If
   End If
   drList.Close()
   drList.Dispose()
  Else
   ErrorCode = 1                                     ' Invalid Key
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing
  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 'IsGroupRoleMember("GroupName","UUID","Role",Key)
 <WebMethod(Description:="User has Group Role membership.")>
 Public Function IsGroupRoleMember(ByVal GroupName As String, ByVal UUID As String, Role As String, ByVal AccessKey As String) As String
  ' Determine if UUID is a member in GroupID Role Name
  Dim MyDB As New MySQLLib
  Dim GroupID As String
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String
  Out(1) = "False"

  If ValidKey(AccessKey) Then
   Dim Group As MySqlDataReader
   ' NOTE: Returns GroupID if Name is found.
   SQLCmd = "Select GroupID " +
            "From osgroup " +
            "Where Name=" + MyDB.SQLStr(GroupName)
   Group = MyDB.GetReader("MyData", SQLCmd)
   If MyDB.Error() Then
    SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
              "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
    MyDB.DBCmd("MySite", SQLCmd)
    ErrorCode = 5                                   ' DB Error
    Out(1) = "DB Error - osgroup"
   Else
    If Group.HasRows() Then
     Group.Read()
     Dim GetRole As MySqlDataReader
     SQLCmd = "Select RoleID From osrole " +
              "Where GroupID=" + MyDB.SQLStr(Group("GroupID")) + " and Name=" + MyDB.SQLStr(Role)
     GetRole = MyDB.GetReader("MyData", SQLCmd)
     If MyDB.Error() Then
      SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
              "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
      MyDB.DBCmd("MySite", SQLCmd)
      ErrorCode = 5                                   ' DB Error
      Out(1) = "DB Error - osrole"
     Else
      If GetRole.HasRows() Then
       GetRole.Read()
       Dim drList As MySqlDataReader
       ' NOTE: Returns True if entry is found or false.
       SQLCmd = "Select AgentID From osgrouprolemembership " +
                "Where GroupID=" + MyDB.SQLStr(Group("GroupID")) + " and " +
                " RoleID = " + MyDB.SQLStr(GetRole("RoleID")) + " and AgentID=" + MyDB.SQLStr(UUID)
       drList = MyDB.GetReader("MyData", SQLCmd)
       If MyDB.Error() Then
        SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
              "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
        MyDB.DBCmd("MySite", SQLCmd)
        ErrorCode = 5                                   ' DB Error
        Out(1) = "DB Error - osgrouprolemembership"
       Else
        If drList.HasRows() Then
         drList.Read()
         Out(1) = "True"
        End If
        ' If Not found, it is still a success, but answer is false
       End If
       drList.Close()
       drList.Dispose()
      Else
       ErrorCode = 3                                  ' Name Not found
       Out(1) = "Role Name Not found"
      End If
     End If
    Else
     ErrorCode = 2                                    ' Group Not found
     Out(1) = "Group Name Not found"
    End If
    GroupID = GetGroupID(GroupName, AccessKey)
   End If
  Else
   ErrorCode = 1                                     ' Invalid Key
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing
  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 ' GetWorldOwner(Key)
 <WebMethod(Description:="Get Grid Owner Account Name.")>
 Public Function GetWorldOwner(ByVal AccessKey As String) As String
  ' Provide Access to account notes for selected account.
  Dim MyDB As New MySQLLib
  Dim ErrorCode As Integer
  ErrorCode = 0
  Dim Out(1) As String
  Out(1) = "Error"

  If ValidKey(AccessKey) Then
   Dim drList As MySqlDataReader
   ' NOTE: Returns something.
   SQLCmd = "Select Parm2 " +
            "From control " +
            "Where Parm1='GridOwnerAcct'"
   drList = MyDB.GetReader("MySite", SQLCmd)
   If MyDB.Error() Then
    SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
             "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
    MyDB.DBCmd("MySite", SQLCmd)
    ErrorCode = 4                                    ' DB Error
    Out(1) = "DB Error"
   Else
    If drList.HasRows() Then
     drList.Read()
     If drList("Parm2").ToString().Trim().Length = 0 Then
      ErrorCode = 3                                  ' Error
      Out(1) = "Grid Owner Account is undefined."
     Else
      Out(1) = drList("Parm2").ToString().Replace(" ", ".")
     End If
    Else
     ErrorCode = 2                                   ' Item Not found
     Out(1) = "Account Not found"
    End If
   End If
   drList.Close()
   drList.Dispose()
  Else
   ErrorCode = 1                                     ' Invalid Key
   Out(1) = "Invalid Key"
  End If

  ' Close open objects
  MyDB.Close()
  MyDB = Nothing
  Out(0) = ErrorCode.ToString()
  Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 End Function

 'AssetCheck(AssetID,Key) as integer
 'LoadAsset(AssetID,Key,Asset) as integer

 '<WebMethod(Description:="Description of function purpose.")>
 'Public Function MyFunction(ByVal AccessKey As String) As String
 ' ' Provide Access to account notes for selected account.
 ' Dim MyDB As New MySQLLib
 ' Dim ErrorCode As Integer
 ' ErrorCode = 0
 ' Dim Out(1) As String
 ' Out(1) = "Error"

 ' If ValidKey(AccessKey) Then
 '  Dim drList As MySqlDataReader
 '  ' NOTE: Returns something.
 '  SQLCmd = "Select Fields " +
 '           "From table " +
 '           "Where Condition=" + MyDB.SQLStr(input) + " " +
 '           "Order by Field"
 '  drList = MyDB.GetReader("MyData", SQLCmd)
 '  If MyDB.Error() Then
 '   SQLCmd = "Insert into errorlog (SQLCmd,ErrMsg) " +
 '            "Values (" + MyDB.SQLStr(SQLCmd) + "," + MyDB.SQLStr(MyDB.ErrMessage()) + ")"
 '   MyDB.DBCmd("MySite", SQLCmd)
 '   ErrorCode = 2                                    ' DB Error
 '   Out(1) = "DB Error"
 '  Else
 '   If drList.HasRows() Then
 '    drList.Read()
 '    If False Then
 '     ErrorCode = 4                                  ' Error
 '     Out(1) = ""
 '    End If
 '   Else
 '    ErrorCode = 3                                   ' Item Not found
 '    Out(1) = "Item Not found"
 '   End If
 '  End If
 '  drList.Close()
 '  drList.Dispose()
 ' Else
 '  ErrorCode = 1                                     ' Invalid Key
 '  Out(1) = "Invalid Key"
 ' End If

 ' ' Close open objects
 ' MyDB.Close()
 ' MyDB = Nothing
 ' Out(0) = ErrorCode.ToString()
 ' Return Out(0).ToString() + Chr(10) + Out(1).ToString()
 'End Function

 Protected Overrides Sub Finalize()
  MyBase.Finalize()
 End Sub
End Class
