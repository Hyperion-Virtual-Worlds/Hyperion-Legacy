<%@ Page Language="VB" AutoEventWireup="false" CodeFile="GridSetup.aspx.vb" Inherits="Administration_GridSetup" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SysMenu" Src="~/SysMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html>
 <head>
  <title>Setup - My World</title>
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <link href="/Styles/TopMenu.css" type="text/css" rel="stylesheet" />
  <link href="/styles/TreeView.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript" src="/scripts/Cookie.js"></script>
  <script type="text/javascript" src="/scripts/TopMenu.js"></script>
  <script type="text/javascript" src="/scripts/TreeView.js"></script>
  <script type="text/javascript">
   var oPageWin = window;
   var oGroupWin = window;

   function CloseDown() {           // Close open child windows if any are open.
    if (oPageWin != window) {
     oPageWin.close();
    }
    if (oGroupWin != window) {
     oGroupWin.close();
    }
   }

   function GoRegion(tag) {
    if (oPageWin == window) {       // Create Window if not opened
     oPageWin = window.open('', 'GridPgPop', 'width=650,height=350,scrollbars=yes,resizable=yes');
    }
    else {                          // give window focus
     oPageWin.focus();
    }
    document.RegPage.KeyID.value = tag;
    document.RegPage.submit();
   }

   function GoGroup() {
    window.open('GridGpPop.aspx', 'GridGpPop', 'width=650,height=350,scrollbars=yes,resizable=yes');
   }
  </script>
 </head>
 <body id="BodyTag" runat="server">
  <!-- Built from MyWorld Basic Page template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle">World Setup</td>
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
      <form id="aspnetForm" method="post" runat="server">
       <table id="ShowItems" runat="server" style="width: 100%;">
        <tr>
         <td>
          The following entries are controls defining where a new account will arrive in world, 
          what will be the default home region, and optional default group.<br />
          Welcome Region is where the new arrivals will land on entry to the world.<br />
          Home is where they will go after learning how to operate in the world.<br />
          The optional default group provides a way to place people into a group to manage what lands and areas they may access. 
          It may also provide a communication option to give information to them.
         </td>
        </tr>
        <tr>
         <td onclick="GoRegion('WelcomeRegion');" style="cursor:pointer;">
          <b>Welcome Region:</b> <span id="Welcome" runat="server"></span>
         </td>
        </tr>
        <tr>
         <td onclick="GoRegion('HomeRegion');" style="cursor:pointer;">
          <b>Home Region:</b> <span id="Home" runat="server"></span>
         </td>
        </tr>
        <tr>
         <td onclick="GoGroup();" style="cursor:pointer;">
          <b>Default Group:</b> <span id="Group" runat="server"></span>
         </td>
        </tr>
        <tr>
         <td>
          <b>Allow World Promotion Uploads for Residents:</b> <asp:CheckBox ID="WPUpload" runat="server" AutoPostBack="true" /><br />
          Provides source pictures by world residents of their builds to display in the viewer splash page or use in world promotions.
         </td>
        </tr>
       </table>
      </form>
     </td>
    </tr>
   </table>
   <div id="ErrorMsg" runat="server" style="width:100%;height:50%;margin:auto,0,auto,0;">
   </div>
  </div>
  <div id="FooterPos">
   <uc1:Footer id="Footer" runat="server"></uc1:Footer>
  </div>
  <form name="RegPage" action="GridPgPop.aspx" method="post" target="GridPgPop">
   <input type="hidden" name="KeyID" value="" />
  </form>
 </body>
</html>
