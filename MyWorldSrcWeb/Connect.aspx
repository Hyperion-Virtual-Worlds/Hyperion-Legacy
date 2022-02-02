<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Connect.aspx.vb" Inherits="Connect" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SysMenu" Src="~/SysMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html xmlns="https://www.w3.org/1999/xhtml" >
 <head>
  <title>Connect - My World</title>
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <link href="/Styles/TopMenu.css" type="text/css" rel="stylesheet" />
  <link href="/styles/TreeView.css" type="text/css" rel="stylesheet" />
  <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.8.1/css/solid.css" integrity="sha384-QokYePQSOwpBDuhlHOsX0ymF6R/vLk/UQVz3WHa6wygxI5oGTmDTv8wahFOSspdm" crossorigin="anonymous">
  <script type="text/javascript" src="/scripts/Cookie.js"></script>
  <script type="text/javascript" src="/scripts/TreeView.js"></script>
  <script type="text/javascript" src="/scripts/TopMenu.js"></script>
  <style type="text/css">
   /* Style to make the grid URL copyable by click */
   .copyable {
    position: relative;
    box-sizing: border-box;
    padding: 0.5em 2.5em 0.5em 0.5em;
    border-radius: 5px;
    border: 1px silver solid;
    background-color: white;
    line-height: 3em;
   }
   .copyable::before {
    position: absolute;
    top: 0;
    right: 0;
    bottom: 0;
    width: 2em;
    border-radius: 0 4px 4px 0;
    line-height: 2em;
    background-color: silver;
    color: black;
    cursor: pointer;
    content: "\f0c5"; /* fa-copy */
    font-family: "Font Awesome 5 Free";
    font-weight: 400;
    text-align: center;
   }
   .copyable.copied::after {
    position: absolute;
    top: calc(-50%);
    right: 0;
    padding: 0 1em;
    line-height: 2em;
    background: #333;
    color: #aaa;
    border-radius: 4px;
    content: 'Copied!';
   }
  </style>
  <script type="text/javascript">
   
   function ShowTOS() {
    window.open('/TOSPage.aspx','TOSPage','width=840,height=600,scrollbars=yes,resizable=yes');
   }
  
   window.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.copyable').forEach(copyableEl => {
     copyableEl.addEventListener('click', () => {
      const range=document.createRange();
      range.selectNodeContents(copyableEl);
      
      const selection = window.getSelection();
      selection.removeAllRanges();
      selection.addRange(range);
      document.execCommand('copy');
      selection.removeAllRanges();
      
      copyableEl.classList.add('copied');
      setTimeout(() => {
        copyableEl.classList.remove('copied');
      }, 1200);
      return false; // prevent any other action from happening because of this click.
     }, true);
    });
   });
  
  </script>
 </head>
 <body id="BodyTag" runat="server" class="School">
  <!-- Built from Web Basic Page Template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle">
      My World Access
     </td>
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
     <td id="ShowContent" runat="server" class="BodyBg">
     </td>
    </tr>
    <tr>
     <td class="BodyBg" style="text-align: center;">
      <a href="/Default.aspx">Return to My World</a>
     </td>
    </tr>
   </table>
  </div>
  <div id="FooterPos">
   <uc1:Footer id="Footer" runat="server"></uc1:Footer>
  </div>
 </body>
</html>
