<%@ Page Language="VB" AutoEventWireup="false" CodeFile="UserForm.aspx.vb" Inherits="Administration_UserForm" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SysMenu" Src="~/SysMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html>
 <head>
  <title>Administration - My World</title>
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
  <!-- Built from WebSite Form template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle">World Accounts</td>
    </tr>
    <tr>
     <td class="SidebarSpacer">&nbsp;</td>
    </tr>
    <!-- Sidebar Menu Control -->
    <tr>
     <td id="SidebarMenu" runat="server">
      <!-- Sidebar menu content is set in code -->
     </td>
    </tr>
   </table>
  </div>
  <div id="BodyArea">
   <table class="BodyTable">
    <tr>
     <td class="BodyBg">
      <!-- Body Content here -->
      <form id="aspnetForm" method="post" runat="server">
      <table style="width:100%;">
       <tr> 
        <td class="PageTitle">Account Settings</td>
       </tr>
       <tr>
        <td>
         <span id="Name" runat="server"></span> &nbsp; 
         <span id="Email" runat="server"></span> 
         <input type="text" id="AddEmail" runat="server" size="40" maxlength="40" placeholder="Add email to activate" class="Form"/>
         <b><span id="UUID" runat="server"></span></b><br />
         <b>IP:</b> <span id="IP" runat="server"></span> &nbsp; <b>Home Region:</b> <span id="Home" runat="server"></span> &nbsp; 
         <b>Status:</b> <span id="Status" runat="server"></span>
        </td>
       </tr>
       <tr id="Admin1" runat="server">
        <td>
         Website Admin Level Assignment: 
         <asp:DropDownList ID="Access" runat="server" AutoPostBack="true">
         </asp:DropDownList>
        </td>
       </tr>
       <tr>
        <td style="height: 30px;">
         <b>NOTE:</b> Blocking an account, if reversed, will require the user to have a password change!
        </td>
       </tr>
       <tr> 
        <td class="SubTitle" style="text-align: center;">
         <asp:Button ID="Button1" Text="Block Account" runat="server"/>
         <asp:Button ID="Button2" Text="UnBlock Account" runat="server"/>
         <asp:Button ID="Button5" Text="Activate Account" runat="server"/>
         &nbsp; <asp:Button ID="Button4" Text="Delete Account" runat="server"/>
        </td>
       </tr>
       <tr>
        <td style="height: 5px;">&nbsp;</td>
       </tr>
       <tr id="Admin2" runat="server"> 
        <td class="PageTitle"> 
         Economy Limits
        </td>
       </tr>
       <tr id="Admin3" runat="server">
        <td>
         <b>Max Buy Amount:</b> $<asp:TextBox id="MaxBuy" runat="server" Columns="3" maxLength="6" cssClass="Form"/> USD
         <b>Max Sell Amount:</b> $<asp:TextBox id="MaxSell" runat="server" Columns="3" maxLength="6" cssClass="Form"/> USD<br />
         <b>Time between Transactions:</b> <asp:TextBox id="NumHours" runat="server" Columns="1" maxLength="3" cssClass="Form"/> <b>Hours</b><br />
        </td>
       </tr>
       <tr id="Admin4" runat="server"> 
        <td class="SubTitle" style="text-align: center;">
         <asp:Button ID="Button3" Text="Update Limits" runat="server"/>
        </td>
       </tr>
      </table>
       <input type="hidden" id="KeyID" value="" runat="server" />
      </form>
     </td>
    </tr>
   </table>
  </div>
  <div id="FooterPos">
   <uc1:Footer id="Footer" runat="server"></uc1:Footer>
  </div>
 </body>
</html>
