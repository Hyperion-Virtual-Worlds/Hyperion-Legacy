<%@ Page Language="VB" AutoEventWireup="false" CodeFile="$codebesiderootname$.vb" Inherits="$codebesideclassname$" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SysMenu" Src="~/SysMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html>
 <head>
  <title>Template - My World</title>
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
  <!-- Built from MyWorld Contact Email Page template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle">Page Title</td>
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
    <tr id="ShowContent" runat="server">
     <td id="Content" runat="server" class="BodyBg">
     </td>
    </tr>
    <tr id="ShowForm" runat="server">
     <td class="BodyBg">
      <form id="aspnetForm" runat="server">
      <table style="width:100%">
       <tr>
        <td colspan="2" style="height: 30px;" class="SecTitle">Send us your comment!</td>
       </tr>
       <tr>
        <td class="FldTitleCol">Select a Contact:</td>
        <td class="FldEntryCol">
         <select class="Form" id="eID" runat="server">
         </select>
        </td>
       </tr>
       <tr> 
        <td class="FldTitleCol">Your Email Address</td>
        <td class="FldEntryCol">
         <input id="Email" runat="server" class="Form" value="" size="52" maxlength="50" />
        </td>
       </tr>
       <tr> 
        <td class="FldTitleCol">Your Comment</td>
        <td class="FldEntryCol">
         <textarea id="Comment" runat="server" rows="20" cols="110" class="Form"></textarea>
        </td>
       </tr>
       <tr>
        <td colspan="2" class="SecTitle">
         <asp:button ID="Button1" runat="server" OnClick="Button1_Click" Text="Send Email" class="Button" />
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
