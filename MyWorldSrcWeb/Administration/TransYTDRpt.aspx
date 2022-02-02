<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TransYTDRpt.aspx.vb" Inherits="Administration_TransYTDRpt" %>
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
 <body>
  <!-- Built from MyWorld Select template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle">Transaction YTD</td>
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
      <form id="aspnetForm" runat="server">
      <table style="width:100%">
       <tr>
        <td>
         <b><span id="StartText" runat="server">Select Year</span></b>
         <asp:DropDownList id="StartYear" runat="server" AutoPostBack="true">
         </asp:DropDownList>
        </td>
       </tr>
      </table>
      </form>
     </td>
    </tr>
   </table>
  </div>
  <div id="BodyArea">
   <table class="BodyTable">
    <tr>
     <td class="BodyBg">
      <!-- Body Content here -->
      <table style="width:100%;">
       <tr>
        <td>
         <table style="width:100%;">
          <tr>
           <td id="ReportDate" runat="server" style="width:50%;"></td>
           <td style="width:50%; text-align: right; cursor: pointer;" onclick="window.print();" class="NoPrint">
            <span id="PrintTitle" runat="server"></span> <img src="/Images/Icons/print.gif" alt="" />
           </td>
          </tr>
          <tr>
           <td colspan="2" class="ReportTitle">Buy / Sell Transaction Year to Date by Year</td>
          </tr>
          <tr>
           <td colspan="2" style="text-align: center;"><b><span id="RptDate" runat="server"></span></b></td>
          </tr>
         </table>
        </td>
       </tr>
       <tr>
        <td id="ShowReport" runat="server" align="center"></td>
       </tr>
      </table>
     </td>
    </tr>
   </table>
  </div>
  <div id="FooterPos">
   <uc1:Footer id="Footer" runat="server"></uc1:Footer>
  </div>
 </body>
</html>
