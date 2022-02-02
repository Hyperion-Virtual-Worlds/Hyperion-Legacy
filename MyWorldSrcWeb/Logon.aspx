<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Logon.aspx.vb" Inherits="Logon" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SysMenu" Src="~/SysMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html>
 <head>
  <title>Logon - My World</title>
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <link href="/Styles/TopMenu.css" type="text/css" rel="stylesheet" />
  <link href="/styles/TreeView.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript" src="/scripts/Cookie.js"></script>
  <script type="text/javascript" src="/scripts/TopMenu.js"></script>
  <script type="text/javascript" src="/scripts/TreeView.js"></script>
  <script type="text/javascript">

  </script>
 </head>
 <body id="BodyTag" runat="server">
  <!-- Built from WebSys Add/Edit template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="BodyAreaFull">
   <table class="BodyTable">
    <tr>
     <td class="BodyBg">
      <table style=" width:100%;">
       <tr>
        <td id="ShowContent" runat="server" style="height: 30px; min-height: 30px; width: 100%; vertical-align: top;">&nbsp;
        </td>
       </tr>
      </table>
      <div style="overflow: auto; margin: 0; padding: 15px; ">
       <div id="LogonBox">
        <div class="Content">
         <h1>Sign In</h1>
         <form id="aspnetForm" method="post" runat="server">
          <table style="width: 100%;">
           <tr>
            <td style="height: 30px; width: 30%; font-weight: bold; text-align: right; padding-right: 3px;">
             First Name:
            </td>
            <td style="height: 30px; width: 70%; padding-left: 3px;">
             <input type="text" id="FirstName" runat="server" size="20" maxlength="20" class="Form"/>
            </td>
           </tr>
           <tr>
            <td style="height: 30px; width: 30%; font-weight: bold; text-align: right; padding-right: 3px;">
             Last Name:
            </td>
            <td style="height: 30px; width: 70%; padding-left: 3px;">
             <input type="text" id="LastName" runat="server" size="20" maxlength="20" class="Form"/>
            </td>
           </tr>
           <tr>
            <td style="height: 30px; width: 30%; font-weight: bold; text-align: right; padding-right: 3px;">
             Password:
            </td>
            <td style="height: 30px; width: 70%; padding-left: 3px;">
             <input type="password" id="Password" runat="server" size="15" maxlength="20" class="Form"/>
            </td>
           </tr>
          </table>
          <table style="width: 100%;">
           <tr id="UpdDelBtn" runat="server"> 
            <td style="height: 30px; text-align: center;">
             <button id="Button1" runat="server" type="submit">Login</button>
            </td>
           </tr>
          </table>
          <div style="text-align:center; padding: 10px 0 10px 0;">
           <span style="font-weight: bold; cursor: pointer;" onclick="window.location='/ResetPass.aspx';">Reset Password</span>
          </div>
         </form>
        </div>
       </div>
       <div id="InfoBox">
        <div class="Content">
         <h1>Sign Up Today!</h1>
         <p>Join My World for any reason you like!</p>
         <ul>
          <li>Why not?</li>
          <li>Explore!</li>
          <li>Meet new friends!</li>
         </ul>
         <p><b>Membership is Free!</b></p>
          <table style="width: 100%;">
           <tr> 
            <td style="height: 30px; text-align: center;">
             <span class="SimButton" onclick="window.location='/Register.aspx';">Sign Up</span>
            </td>
           </tr>
          </table>
        </div>
       </div>
      </div>
     </td>
    </tr>
   </table>
  </div>
  <div id="FooterPos">
   <uc1:Footer id="Footer" runat="server"></uc1:Footer>
  </div>
 </body>
</html>
