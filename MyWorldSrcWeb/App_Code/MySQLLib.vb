Imports System.Collections.Specialized
Imports System.Text
Imports System.ComponentModel
Imports MySQL.Data
Imports MySQL.Data.MySqlClient
Imports System.Web.Configuration.WebConfigurationManager

' Built from the .Net WebControls template.
'* Written by Bob Curtice 11/26/2016 for use in the Halcyon grid website project.

Public Class MySQLLib
 '<DefaultProperty("Text"), ToolboxData("<{0}:DataLib runat=server></{0}:DataLib>")> Public Class DataLib
 'Inherits System.Web.UI.WebControls.WebControl

 ' For every public property defined, a corresponding property definition must be added.
 Dim DataError As Boolean
 Dim PreMessage, Message, PostMessage As String
 Dim NumRows, Identity As Integer

 'Dim _text As String

 '<Bindable(True), Category("Appearance"), DefaultValue("")> Property [Text]() As String
 ' Get
 '  Return _text
 ' End Get

 ' Set(ByVal Value As String)
 '  _text = Value
 ' End Set
 'End Property

 'Protected Overrides Sub Render(ByVal output As System.Web.UI.HtmlTextWriter)
 ' output.Write([Text])
 'End Sub

 Public ReadOnly Property [Error]() As Boolean
  Get
   Return DataError
  End Get
 End Property

 Public ReadOnly Property [ErrSource]() As String
  Get
   Return PreMessage
  End Get
 End Property

 Public ReadOnly Property [ErrSQL]() As String
  Get
   Return Message
  End Get
 End Property

 Public ReadOnly Property [ErrMessage]() As String
  Get
   Return PostMessage
  End Get
 End Property

 Public ReadOnly Property [RowsAffected]() As Integer
  Get
   Return NumRows
  End Get
 End Property

 Public ReadOnly Property [ReturnValue]() As Integer
  Get
   Return Identity
  End Get
 End Property

 ' Defines a new instance of the class
 Public Sub New()
  DataError = False
  PreMessage = ""
  Message = ""
  PostMessage = ""
  NumRows = -1                               ' Initialize as undefined
  Identity = -1                              ' Initialize as undefined
 End Sub

 ' Function translates the Connection name to the actual connection settings
 Private Function Connect(ByVal ConnectionName As String) As MySqlConnection
  Dim sqlConnection As MySqlConnection = New MySqlConnection(System.Configuration.ConfigurationManager.AppSettings(ConnectionName))
  Return sqlConnection
 End Function

 Public Function GetConn(ByVal ConnectionName As String) As String
  Return System.Configuration.ConfigurationManager.ConnectionStrings(ConnectionName).ConnectionString()
 End Function

 Public Function GetReader(ByVal ConnectionName As String, ByVal SQL As String, Optional ByVal Timeout As Integer = 0) As MySqlDataReader
  ' Handles SELECTs that return a result set which is returned in the form
  ' of a disconnected recordset (dataset)
  Dim sqlConn As MySqlConnection
  sqlConn = Connect(ConnectionName)
  Dim sqlCommand As MySqlCommand = New MySqlCommand(SQL, sqlConn)
  Dim drResults As MySqlDataReader

  If Timeout > 0 Then
   sqlCommand.CommandTimeout = Timeout       ' Seconds to wait for server response. Default is 30 seconds.
  End If

  Try
   sqlConn.Open()
   drResults = sqlCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)
  Catch sqlException As MySqlException
   Call DBError("Function: GetReader", SQL, sqlException)
   sqlConn.Close()
  End Try

  Return drResults                           ' Send back datareader object reference for use.
 End Function

 Public Function GetReaderSP(ByVal ConnectionName As String, ByVal ProcName As String, Optional ByVal Parms As HybridDictionary = Nothing, Optional ByVal Timeout As Integer = 0) As MySqlDataReader
  ' Handles executing Stored Procs that return a result.
  ' Dictionary object was used for Parameters to the Stored procedure
  ' to make them Optional since you can't have an Optional ParmArray.
  Dim sqlConn As MySqlConnection
  sqlConn = Connect(ConnectionName)
  Dim sqlCommand As MySqlCommand = New MySqlCommand(ProcName, sqlConn)
  Dim drResults As MySqlClient.MySqlDataReader
  ' Set up Command object parms if any
  sqlCommand.CommandType = System.Data.CommandType.StoredProcedure
  If Timeout > 0 Then
   sqlCommand.CommandTimeout = Timeout       ' Seconds to wait for server response. Default is 30 seconds.
  End If

  If Not Parms Is Nothing Then
   Dim enumParms As System.Collections.IDictionaryEnumerator = Parms.GetEnumerator()
   While enumParms.MoveNext()
    Dim sqlParm As New MySqlParameter(enumParms.Key.ToString, enumParms.Value)
    sqlCommand.Parameters.Add(sqlParm)
   End While
  End If

  Try
   sqlConn.Open()
   drResults = sqlCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection)
  Catch sqlException As MySqlException
   Call DBError("Function: GetReaderSP", "Procedure Called: " + ProcName, sqlException)
   sqlConn.Close()
  End Try
  Return drResults
 End Function

 Public Function GetAdapter(ByVal ConnectionName As String, ByVal SQL As String, Optional ByVal Timeout As Integer = 0) As MySqlDataAdapter
  Dim daResults As MySqlDataAdapter = New MySqlDataAdapter(SQL, System.Configuration.ConfigurationManager.AppSettings(ConnectionName))
  If Timeout > 0 Then
   daResults.SelectCommand.CommandTimeout = Timeout ' Seconds to wait for server response. Default is 30 seconds.
  End If

  Return daResults                           ' Send back loaded Data Adapter to execute on fill
 End Function

 Public Function GetAdapterSP(ByVal ConnectionName As String, ByVal ProcName As String, Optional ByVal Parms As HybridDictionary = Nothing) As MySqlDataAdapter
  ' If optional parameters are supplied the data is placed in the named dataset with the supplied table name,
  ' otherwise the data Adapter is returned.
  ' Handles executing Stored Procs that return a result in a data adapter.
  ' Dictionary object was used for Parameters to the Stored procedure
  ' to make them Optional since you can't have an Optional ParmArray.

  Dim daResults As MySqlDataAdapter = New MySqlDataAdapter

  Try
   daResults = New MySqlDataAdapter(ProcName, System.Configuration.ConfigurationManager.AppSettings(ConnectionName))
   daResults.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure
   ' Set up Command object parms if any
   If Not Parms Is Nothing Then
    Dim enumParms As System.Collections.IDictionaryEnumerator = Parms.GetEnumerator()
    While enumParms.MoveNext()
     Dim sqlParm As New MySqlParameter(enumParms.Key.ToString, enumParms.Value)
     daResults.SelectCommand.Parameters.Add(sqlParm)
    End While
   End If
  Catch sqlException As MySqlException
   Call DBError("Function: GetAdapterSP", "Procedure Called: " + ProcName, sqlException)
  End Try

  Return daResults
 End Function

 Public Function DBCmd(ByVal ConnectionName As String, ByVal SQL As String, Optional ByVal Timeout As Integer = 0) As Boolean
  ' Handles DELETEs, UPDATEs and INSERTs.  No Result set is returned.
  Dim sqlConn As MySqlConnection
  sqlConn = Connect(ConnectionName)
  Dim sqlCommand As MySqlCommand = New MySqlCommand(SQL, sqlConn)
  Dim bFlag As Boolean
  bFlag = True                               ' Presume result will succeed
  If Timeout > 0 Then
   sqlCommand.CommandTimeout = Timeout
  End If

  Try
   sqlConn.Open()
   NumRows = sqlCommand.ExecuteNonQuery()
   sqlConn.Close()
  Catch sqlException As MySqlException
   sqlConn.Close()
   Call DBError("Function: DBCmd", SQL, sqlException)
   bFlag = False
  End Try
  Return bFlag
 End Function

 Public Function DBCmdSP(ByVal ConnectionName As String, ByVal ProcName As String, Optional ByVal Parms As HybridDictionary = Nothing, Optional ByVal Timeout As Integer = 0) As Boolean
  ' Handles executing Stored Procs that DO NOT return a result.
  ' Dictionary object was used for Parameters to the Stored procedure
  ' to make them Optional since you can't have an Optional ParmArray.

  Dim sqlConn As MySqlConnection
  sqlConn = Connect(ConnectionName)
  Dim sqlCommand As MySqlCommand = New MySqlCommand(ProcName, sqlConn)
  Dim bFlag As Boolean

  bFlag = True                               ' Presume result will succeed
  sqlCommand.CommandType = System.Data.CommandType.StoredProcedure
  If Timeout > 0 Then
   sqlCommand.CommandTimeout = Timeout       ' Seconds to wait for server response. Default is 30 seconds.
  End If

  If Not Parms Is Nothing Then
   Dim enumParms As System.Collections.IDictionaryEnumerator = Parms.GetEnumerator()
   While enumParms.MoveNext()
    Dim sqlParm As New System.Data.SqlClient.SqlParameter(enumParms.Key.ToString, enumParms.Value)
    sqlCommand.Parameters.Add(sqlParm)
   End While
  End If

  Dim prmReturnValue As MySqlParameter = New MySqlParameter("Return_Value", System.Data.SqlDbType.Int)
  prmReturnValue = sqlCommand.Parameters.AddWithValue("Return_Value", System.Data.SqlDbType.Int)
  prmReturnValue.Direction = System.Data.ParameterDirection.ReturnValue

  Try
   sqlConn.Open()
   NumRows = sqlCommand.ExecuteNonQuery()    ' Set number of rows affected value 
   Identity = sqlCommand.Parameters("Return_Value").Value ' Set identity value if applies. Not sure what will show if not applies.
   sqlConn.Close()
  Catch sqlException As MySqlException
   sqlConn.Close()
   Call DBError("Function: dbCmdSP", "Procedure Called: " & ProcName, sqlException)
   bFlag = False
  End Try
  Return bFlag
 End Function

 Public Sub NextPage(ByVal dr As MySqlDataReader, ByVal Page As Integer, ByVal Size As Integer)
  Dim iRecordCount As Integer
  Dim iTopofPage As Integer
  ' Calculate which record is top of current page
  iTopofPage = (Page * Size) - (Size - 1)
  iRecordCount = 0
  ' Move the record pointer to the calculated position
  Do While dr.Read()
   iRecordCount += 1
   If iRecordCount = iTopofPage Then
    Exit Do
   End If
  Loop
  ' Record pointer is set at top of display page.
 End Sub

 Protected Sub DBError(ByVal aMessage As String, ByVal SQLCommand As String, ByVal sqlException As MySqlException)
  Dim sErrorMessages As String
  sErrorMessages = vbCrLf + "Message: " + sqlException.Message().ToString() + vbCrLf + _
                "LineNumber: " + sqlException.Number.ToString() + vbCrLf + _
                "Source: " + sqlException.Source.ToString() + vbCrLf + _
                "Procedure: " + sqlException.TargetSite.ToString() + vbCrLf
  PreMessage = aMessage
  Message = SQLCommand
  PostMessage = sErrorMessages
  DataError = True
 End Sub

 '*------------------------------------------------------------------*
 '* UTF-8 DATA CONVERSION FUNCTIONS                                  *
 '*------------------------------------------------------------------*
 Public Function Iso8859ToUTF8(ByVal src As Object) As String
  Dim Rtn As String
  If src.ToString().Length > 0 Then          ' Convert to Database encoding for UTF8
   Rtn = Encoding.UTF8.GetString(Encoding.Default.GetBytes(src))
  Else                                       ' Return the Empty String
   Rtn = src.ToString()
  End If
  Return Rtn
 End Function

 Public Function UTF8ToIso8859(ByVal src As Object) As String
  Dim Rtn As String
  If src.ToString().Length > 0 Then          ' Convert to Database encoding for UTF8
   Rtn = Encoding.Default.GetString(Encoding.UTF8.GetBytes(src))
  Else                                       ' Return the Empty String
   Rtn = src.ToString()
  End If
  Return Rtn
 End Function

 '*------------------------------------------------------------------*
 '* SQL CONVERSION FUNCTIONS                                         *
 '*------------------------------------------------------------------*

 Public Function SQLStr(ByVal Sql As Object) As String
  ' format string for Sql command format with single quotes delimiting it. Double singles for embeded single quote.
  If Sql.ToString().Trim().Length = 0 Or Sql Is DBNull.Value Then    ' Empty string or null, pad with a space
   Sql = "''"
  Else
   Sql = Sql.ToString().Trim()
   If Sql.IndexOf("'", 1) > 0 Then           ' escape quotes
    Sql = Sql.Replace("'", "''")
   End If
   Sql = "'" + Sql + "'"
  End If
  Return Sql
 End Function

 Public Function SQLLike(ByVal Sql As Object) As String
  ' Use only with the SQL LIKE operator for testing content of memo (text) fields.
  ' format string for SQL LIKE format with single quotes delimiting it. Double singles for embeded single quote.
  If Sql.ToString().Trim().Length = 0 Or Sql Is DBNull.Value Then    ' Empty string or null, pad with a space
   Sql = ("'% %'").ToString()
  Else
   Sql = Sql.ToString().Trim()
   If Sql.IndexOf("'", 1) > 0 Then           ' escape quotes
    Sql = Sql.Replace("'", "''")
   End If
   Sql = "'%" + Replace(Sql, "'", "''") + "%'"
  End If
  Return Sql
 End Function

 Public Function SQLNo(ByVal Number As Object) As String
  ' Format any numeric value to a string value for use in SQL commands.
  Dim sReturnVal As String
  If IsDBNull(Number) Or Number.ToString().Trim().Length = 0 Then
   sReturnVal = "NULL"
  ElseIf Not IsNumeric(Number) Then          ' Prevent SQL injection with numeric value parms
   sReturnVal = "0"
  Else
   sReturnVal = Number.ToString()
  End If
  Return sReturnVal
 End Function

 Public Function SQLDate(ByVal DateIn As Object) As String
  'changes DateTime format to string form for SQL commands.
  Dim DateC As String
  If IsDBNull(DateIn) Or IsNothing(DateIn) Or Not IsDate(DateIn) Then
   DateC = "NULL"
  Else
   Dim DateType As Date
   DateType = CDate(DateIn)
   'DateC = "'" + CStr(Year(DateIn)) + "-" + LTrim(CStr(Month(DateIn))) + "-" + LTrim(CStr(Day(DateIn))) + _
   '        " " + LTrim(CStr(DatePart("h", DateIn))) + ":" + LTrim(CStr(DatePart("n", DateIn))) + ":" + _
   '        LTrim(CStr(DatePart("s", DateIn))) + "'"
   DateC = "'" + DateType.Year.ToString + ("-").ToString() + DateType.Month.ToString() + ("-").ToString() + DateType.Day.ToString() + (" ").ToString() + _
           DateType.Hour.ToString() + (":").ToString() + DateType.Minute.ToString() + (":").ToString() + DateType.Second.ToString() + "'"
  End If
  Return DateC
 End Function

 Public Function SQLTF(ByVal InTF As Boolean) As String
  ' Converts Boolean values to SQL Bit Values (1 | 0)
  Dim sValue As String
  If InTF Then
   sValue = "'true'"
  Else
   sValue = "'false'"
  End If
  Return sValue
 End Function

 Public Sub Close()
  Finalize()
 End Sub

 Protected Overrides Sub Finalize()
  MyBase.Finalize()
 End Sub
End Class
