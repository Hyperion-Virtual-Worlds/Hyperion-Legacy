<%@ Page Language="VB" AutoEventWireup="false" CodeFile="RegionForm.aspx.vb" Inherits="RegionForm" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SysMenu" Src="~/SysMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html xmlns="https://www.w3.org/1999/xhtml" >
 <head>
  <title>Estate - My World</title>
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <link href="/Styles/TopMenu.css" type="text/css" rel="stylesheet" />
  <link href="/styles/TreeView.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript" src="/scripts/Cookie.js"></script>
  <script type="text/javascript" src="/scripts/TopMenu.js"></script>
  <script type="text/javascript" src="/scripts/TreeView.js"></script>
  <script type="text/javascript">

   function CallEdit(tID, tAction) { // Add / Edit a record
    document.EditPage.KeyID.value = tID;
    document.EditPage.action = tAction;
    document.EditPage.submit();
   }

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
     <td class="ProgTitle">Estate Management</td>
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
      Region name change will only apply after region restart.
      That may be done inworld.
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
       <tr>
        <td>
         <table style="width: 100%;">
          <tr>
           <td class="FldTitleCol">Region Information</td>
           <td class="FldEntryCol">
            <b>Region Name:</b> <asp:TextBox id="RegionName" runat="server" Columns="20" maxLength="40" cssClass="Form"/>
            <span id="ShowUUID" runat="server" style="font-weight: bold;"></span><br />
            <span id="ShowDate" runat="server"><b>Created:</b> <span id="MadeDate" runat="server"></span></span>,
            <span id="ShowItems" runat="server"><b>Objects:</b> <span id="Objects" runat="server"></span></span>,<br />
            <b>Region Type:</b> <span id="RegType" runat="server"></span>,
            <b>Prim Maximum:</b> <span id="PrimMax" runat="server"></span>
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Map Location</td>
           <td class="FldEntryCol">
            <b>X:</b><span id="LocationX" runat="server"></span>, 
            <b>Y:</b><span id="LocationY" runat="server"></span>
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
       <input type="hidden" id="KeyID" value="" runat="server" />
      </form>
     </td>
    </tr>
   </table>
  </div>
  <div id="FooterPos">
   <uc1:Footer id="Footer" runat="server"></uc1:Footer>
  </div>
  <form name="EditPage" action="" method="post">
   <input type="hidden" name="KeyID" value="" />
  </form>
 </body>
</html>
