<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PopupError.aspx.vb" Inherits="PopupError" %>

<!DOCTYPE html>
<html>
 <head>
  <title>My World</title>
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript">

  </script>
 </head>
 <body>
  <!-- Built from Website Popup page template v. 1.0 -->
  <div id="PopHeaderPos">
   <table class="PopHeader">
    <tr>
     <td id="PopTitle" runat="server" class="PopTitle"></td>
    </tr>
   </table>
  </div>
  <div id="PopBodyArea">
   <table class="PopTable">
    <tr>
     <td style="height: 500px; vertical-align: top;" class="PopBodyBg">
      <table>
       <tr>
        <td id="Message" runat="server" class="Errors">
        </td>
       </tr>
      </table>
     </td>
    </tr>
   </table>
  </div>
  <div id="PopFooterPos">
   <table class="PopFooter">
    <tr>
     <td class="PopCopyright">
      Copyright &copy; <script type="text/javascript">document.write(new Date().getFullYear());</script>
      by My World. All rights reserved.
     </td>
    </tr>
   </table>
  </div>
 </body>
</html>
