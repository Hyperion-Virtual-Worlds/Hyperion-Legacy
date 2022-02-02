Imports System.ComponentModel
Imports System.Web.UI
Imports System.Web.HttpContext
Imports System.Net.Mail

'* This class provides simplified coding access to the .Net Email services provided through the System.Net.Mail namespace.
'* Written by Bob Curtice 11/26/2016 for use in the Halcyon grid website project.

Public Class SendEmail
 Private bTrace As Boolean
 Private SendSMTP As New SmtpClient
 Private Mail As New MailMessage
 Private tFrom, tErrMsg As String

 Public Sub New()
  ' Only to initialize a new object instance. Does nothing.
  bTrace = False
  tErrMsg = ""
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

 ' Error Message provided when an error occurs
 Public ReadOnly Property [ErrMessage]() As String
  Get
   Return tErrMsg
  End Get
 End Property

 ' Required options to send an email
 Public WriteOnly Property EmailServer() As String
  Set(ByVal value As String)
   SendSMTP.Host = value.ToString()
  End Set
 End Property

 Public WriteOnly Property Port() As Integer
  Set(ByVal value As Integer)
   SendSMTP.Port = value
  End Set
 End Property

 Public WriteOnly Property ToAddress() As String
  Set(ByVal value As String)
   Mail.To.Clear()
   Mail.To.Add(value.ToString())
  End Set
 End Property

 Public WriteOnly Property FromAddress() As String
  Set(ByVal value As String)
   Mail.From = New MailAddress(value.ToString(), value.ToString())
  End Set
 End Property

 Public WriteOnly Property Subject() As String
  Set(ByVal value As String)
   Mail.Subject = value.ToString()
  End Set
 End Property

 Public WriteOnly Property Body() As String
  Set(ByVal value As String)
   Mail.Body = value.ToString()
  End Set
 End Property

 Public WriteOnly Property IsHTML() As Boolean
  Set(ByVal value As Boolean)
   Mail.IsBodyHtml = value
  End Set
 End Property

 Public Function CC(ByVal Email As String) As Boolean
  Dim tRtn As Boolean
  tErrMsg = ""                                                   ' Make sure Error message is clear
  If Email.ToString().Trim().Length > 0 Then
   Mail.CC.Add(Email)
   tRtn = True
  Else
   tErrMsg = "SendMail CC Error: an empty Email address was sent!"
   tRtn = False
  End If
  Return tRtn
 End Function

 Public Function BCC(ByVal Email As String) As Boolean
  Dim tRtn As Boolean
  tErrMsg = ""                                                   ' Make sure Error message is clear
  If Email.ToString().Trim().Length > 0 Then
   Mail.Bcc.Add(Email)
   tRtn = True
  Else
   tErrMsg = "SendMail BCC Error: an empty Email address was sent!"
   tRtn = False
  End If
  Return tRtn
 End Function

 Public Function Attachfile(ByVal Filename As String) As Boolean
  Dim tRtn As Boolean
  tErrMsg = ""                                                   ' Make sure Error message is clear
  Try
   Mail.Attachments.Add(New System.Net.Mail.Attachment(Filename))
   tRtn = True
  Catch ex As Exception
   tErrMsg = ex.Message()
   tRtn = False
  End Try
  Return tRtn
 End Function

 Public Function SendMail() As Boolean
  Dim tRtn As Boolean

  tErrMsg = ""                                                   ' Make sure Error message is clear
  If Mail.To.ToString().Trim().Length > 0 Then                   ' Check required fields are present
   If Mail.From.ToString().Trim().Length > 0 Then
    If Mail.Body.ToString().Trim().Length > 0 Then
     If SendSMTP.Host.ToString().Trim().Length > 0 Then
      If bTrace Then System.Web.HttpContext.Current.Trace.Warn("SendMail", "Processing to Mail Server " + SendSMTP.Host.ToString().Trim())
      If bTrace Then System.Web.HttpContext.Current.Trace.Warn("SendMail", "Sending To: " + Mail.To.ToString().Trim() + " From: " + Mail.From.ToString().Trim())
      Try
       ' Process Sending the Email
       'SendSMTP.UseDefaultCredentials = True
       SendSMTP.Send(Mail)
       tRtn = True
       If bTrace Then System.Web.HttpContext.Current.Trace.Warn("SendMail", "SendMail Success!")
      Catch ex As Exception
       tErrMsg = ex.Message.ToString() '+ " To: " + Mail.To.ToString().Trim() + " From: " + Mail.From.ToString().Trim()
       tRtn = False
       If bTrace Then System.Web.HttpContext.Current.Trace.Warn("SendMail", "SendMail Failed: " + tErrMsg.ToString())
      End Try
      Mail.Bcc.Clear()
      Mail.CC.Clear()
      Mail.To.Clear()
     Else
      tErrMsg = "SendMail Error: Required SMTPServer URL parameter is missing!"
      tRtn = False
     End If
    Else                                                         ' Missing a message
     tErrMsg = "SendMail Error: Required Body Message is empty!"
     tRtn = False
    End If
   Else                                                          ' Missing From email address
    tErrMsg = "SendMail Error: Required FromAddress is missing!"
    tRtn = False
   End If
  Else                                                           ' Missing a Send To email address
   tErrMsg = "SendMail Error: Required ToAddress is missing!"
   tRtn = False
  End If

  Return tRtn
 End Function

 Public Sub Close()
  Mail.Dispose()
  SendSMTP = Nothing
  bTrace = Nothing
  Finalize()
 End Sub

 ' Used on close to terminate object
 Protected Overrides Sub Finalize()
  MyBase.Finalize()
 End Sub
End Class
