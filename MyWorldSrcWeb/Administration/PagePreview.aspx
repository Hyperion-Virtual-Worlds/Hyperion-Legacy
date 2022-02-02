<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PagePreview.aspx.vb" Inherits="Administration_PagePreview" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" >
 <head>
  <title>Administration - My World</title>
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript">
  
  </script>
 </head>
 <body id="BodyTag" runat="server" class="PopBody">
  <!-- Built from Website Popup page template v. 1.0 -->
  <div id="PopHeaderPos">
   <table class="PopHeader">
    <tr>
     <td class="PopTitle">Page Preview</td>
    </tr>
   </table>
  </div>
  <div id="PopBodyArea">
   <table class="PopTable">
    <tr>
     <td class="PopBodyBg" style="height: 500px; vertical-align: top;">
      <table style="width: 100%;">
       <tr id="ShowTitle" runat="server">
        <td id="Title" runat="server" class="TopicTitle">
        </td>
       </tr>
       <tr>
        <td id="Content" runat="server" class="TopicContent">
        </td>
       </tr>
      </table>
      <input type="hidden" id="KeyID" runat="server" name="KeyID" />
     </td>
    </tr>
   </table>
  </div>
  <div id="PopFooterPos">
   <table style="width:100%;">
    <tr>
     <td class="PopCopyright">
      Copyright © <script type="text/javascript">document.write(new Date().getFullYear());</script>
      by Curtice Enterprises. All rights reserved.
     </td>
    </tr>
   </table>
  </div>
 </body>
</html>
