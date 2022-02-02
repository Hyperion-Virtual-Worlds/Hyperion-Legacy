<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ErrorItems.aspx.vb" Inherits="Administration_ErrorItems" %>
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
     <td class="ProgTitle">Error Item Locator</td>
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
     <td>Locate items by region log error item UUID.</td>
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
        <td id="PageTitle" runat="server" class="PageTitle">Search for Error Item by UUID</td>
       </tr>
       <tr>
        <td>
         <b>Enter Error Item UUID:</b> <asp:TextBox id="ItemUUID" runat="server" Columns="30" maxLength="36" cssClass="Form"/>
        </td>
       </tr>
       <tr id="ShowMessage" runat="server">
        <td id="Message" runat="server" style="height: 30px; text-align: center; font-weight: bold;"></td>
       </tr>
       <tr id="ShowItem" runat="server">
        <td>
         <b>Element Name:</b> <span id="ItemName" runat="server"></span><br />
         <b>Prim Name:</b> <span id="PrimName" runat="server"></span><br />
         <b>Location:</b> <span id="Location" runat="server"></span><br />
         <b>Is in a Linkset:</b> <span id="IsLinkset" runat="server"></span><br />
        </td>
       </tr>
       <tr id="UpdDelBtn" runat="server"> 
        <td class="SubTitle" style="text-align: center;">
         <asp:Button ID="Button1" Text="Search" runat="server"/>
        </td>
       </tr>
      </table>
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
