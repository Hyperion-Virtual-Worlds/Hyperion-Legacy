<%@ Page Language="VB" AutoEventWireup="false" CodeFile="RegionConfig.aspx.vb" Inherits="Administration_RegionConfig" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SysMenu" Src="~/SysMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html xmlns="https://www.w3.org/1999/xhtml" >
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
  <!-- Built from MyWorld Form template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle">Grid Manager</td>
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
    <tr>
     <td>
      Map Location, IP Addresses and Port may only be changed when the region is offline.
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
        <td id="PageTitle" runat="server" class="PageTitle"> 
        </td>
       </tr>
       <!-- Repeat the following tr content for each field to be processed. -->
       <tr>
        <td>
         <table style="width: 100%; border-collapse: initial; border-spacing: 1px; border-color: #ffffff;">
          <tr>
           <td class="FldTitleCol">Region Name</td>
           <td class="FldEntryCol">
            <span id="regionName" runat="server"></span> :
            <span id="ShowUUID" runat="server" style="font-weight: bold;"></span>
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Map Location</td>
           <td class="FldEntryCol">
            <b>X:</b><span id="LocationX" runat="server"></span> &nbsp; 
            <b>Y:</b><span id="LocationY" runat="server"></span>
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Server Host</td>
           <td ID="ServerName" Runat="server" class="FldEntryCol">
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Port</td>
           <td class="FldEntryCol">
            <asp:DropDownList ID="Port" CssClass="Form" Runat="server"/>
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Owner</td>
           <td id="Owner" runat="server" class="FldEntryCol">
           </td>
          </tr>
         </table>
        </td>
       </tr>
       <tr id="UpdDelBtn" runat="server"> 
        <td class="SubTitle" style="text-align: center;">
         <asp:Button ID="Button1" Text="Update" runat="server"/>
        </td>
       </tr>
      </table>
       <input type="hidden" id="KeyID" runat="server" value="" />
       <input type="hidden" id="ExtIP" runat="server" value="" />
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
