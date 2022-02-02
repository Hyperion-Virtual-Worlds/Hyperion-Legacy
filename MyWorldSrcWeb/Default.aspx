<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SysMenu" Src="~/SysMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html xmlns="https://www.w3.org/1999/xhtml" >
 <head>
  <title>Home - My World</title>
  <meta id="description" name="description" content="My World virtual world." />
  <meta id="keywords" name="keywords" content="[%KeyWorldslist%]" />
  <meta id="robots" name="robots" content="index, follow" />
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <link href="/Styles/TopMenu.css" type="text/css" rel="stylesheet" />
  <link href="/styles/TreeView.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript" src="/scripts/Cookie.js"></script>
  <script type="text/javascript" src="/scripts/TreeView.js"></script>
  <script type="text/javascript" src="/scripts/TopMenu.js"></script>
  <script type="text/javascript">
   
   if (top != self) {
    top.location=self.document.location;
   }
   
  </script>
 </head>
 <body>
  <!-- Built from Web Basic Page template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle"></td>
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
    <tr>
     <td class="SidebarSpacer">&nbsp;</td>
    </tr>
   </table>
   <div id="GridStatus" style=" opacity: 0.6; min-height: 150px; width: 90%; margin-left: 5%; vertical-align: top; background-color: #FFFFFF; border: 1px solid #000000; border-radius: 10px;">
    <table style="width: 100%;">
     <tr>
      <td colspan="2" style="height: 10px;"></td>
     </tr>
     <tr>
      <td style="width: 50%; font-weight: bold; padding-left: 5px;">
       World Status: 
      </td>
      <td style="width: 50%;">
       <span id="GridStats" runat="server"></span>
      </td>
     </tr>
     <tr>
      <td style="width: 50%; font-weight: bold; padding-left: 5px;">
       Total Users: 
      </td>
      <td style="width: 50%;">
       <span id="GridTUsers" runat="server"></span>
      </td>
     </tr>
     <tr>
      <td style="width: 50%; font-weight: bold; padding-left: 5px;">
       Users Online: 
      </td>
      <td style="width: 50%;">
       <span id="GridOnline" runat="server"></span>
      </td>
     </tr>
     <tr>
      <td style="width: 50%; font-weight: bold; padding-left: 5px;">
       Region Count: 
      </td>
      <td style="width: 50%;">
       <span id="GridRegions" runat="server"></span>
      </td>
     </tr>
     <tr>
      <td colspan="2">
       <ul>
        <li><i>Public Regions:</i> <span id="PubRegions" runat="server"></span></li>
        <li><i>Private Regions</i> <span id="PrivRegions" runat="server"></span></li>
       </ul>
      </td>
     </tr>
    </table>
   </div>
  </div>
  <div id="BodyArea">
   <table class="BodyTable">
    <tr id="ContentDisp" runat="server">
     <td id="ShowContent" runat="server" class="BodyBg">
     </td>
    </tr>
   </table>
  </div>
  <div id="FooterPos">
   <uc1:Footer id="Footer" runat="server"></uc1:Footer>
  </div>
 </body>
</html>
