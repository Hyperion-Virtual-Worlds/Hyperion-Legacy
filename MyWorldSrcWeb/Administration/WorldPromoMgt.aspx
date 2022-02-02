<%@ Page Language="VB" AutoEventWireup="false" CodeFile="WorldPromoMgt.aspx.vb" Inherits="Administration_WorldPromoMgt" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SysMenu" Src="~/SysMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html>
 <head>
  <title>World Promo - My World</title>
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <link href="/Styles/TopMenu.css" type="text/css" rel="stylesheet" />
  <link href="/styles/TreeView.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript" src="/scripts/Cookie.js"></script>
  <script type="text/javascript" src="/scripts/TopMenu.js"></script>
  <script type="text/javascript" src="/scripts/TreeView.js"></script>
  <script type="text/javascript">

   function ShowImg(aSrc) {
    document.getElementById("ShowPic").src = aSrc;
    document.getElementById("File").value = aSrc;
    document.getElementById("DivWinTrans").style.display = "block";
    document.getElementById("DivWinBox").style.display = "block";
   }

   function HidePicWin() {
    document.getElementById("DivWinBox").style.display = "none";
    document.getElementById("DivWinTrans").style.display = "none";
   }

   function ActiveImg(Action) {
    document.getElementById("Action").value = Action;
    setTimeout('__doPostBack(\'Action\',\'\')', 0);
   }

  </script>
 </head>
 <body>
  <!-- Built from MyWorld Basic Page template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle">World Promo Management</td>
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
     <td class="PageTitle">Resident Image Upload List</td>
    </tr>
    <tr>
     <td id="ShowList" runat="server" style="min-height: 600px; vertical-align: top;">
     </td>
    </tr>
    <tr>
     <td class="BodyBg">
      <form id="aspnetForm" method="post" runat="server">
       <input type="hidden" id="File" runat="server" />
       <asp:TextBox ID="Action" runat="server" AutoPostBack="true" CssClass="NoShow" />
      </form>
     </td>
    </tr>
   </table>
  </div>
  <div id="FooterPos">
   <uc1:Footer id="Footer" runat="server"></uc1:Footer>
  </div>
  <div id="DivWinTrans" class="DivWinTrans"></div>
  <div id="DivWinBox" class="DivWinBox">
   <table style="width: 100%; height: 100%;">
    <tr>
     <td>
      <table align="center" class="WarnTable">
       <tr>
        <td style="width: 100%;">
         <table style="width: 100%;">
          <tr style="background-color: #eeeeee;">
           <td style="width: 100%; height: 20px; "></td>
           <td style="width: 20px; height: 20px; text-align: right; cursor: pointer;" onclick="HidePicWin()">[X]</td>
          </tr>
         </table>
        </td>
       </tr>
       <tr>
        <td style="text-align: center;">
         <img id="ShowPic" src="" width="800" alt="" />
        </td>
       </tr>
       <tr>
        <td style="text-align: center;" class="SubTitle">
         <input type="button" value="Move to Backgrounds" onclick="ActiveImg('MOV');"/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
         <input type="button" value="Delete" onclick="ActiveImg('DEL');"/>
        </td>
       </tr>
      </table>
     </td>
    </tr>
   </table>
  </div>
 </body>
</html>
