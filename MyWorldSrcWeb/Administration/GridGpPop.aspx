<%@ Page Language="VB" AutoEventWireup="false" CodeFile="GridGpPop.aspx.vb" Inherits="Administration_GridGpPop" %>

<!DOCTYPE html>
<html>
 <head>
  <title>Setup - My World</title>
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript">
  
  </script>
 </head>
 <body id="BodyTag" runat="server" class="PopBody">
  <!-- Built from MyWorld Popup page template v. 1.0 -->
  <div id="PopHeaderPos">
   <table class="PopHeader">
    <tr>
     <td id="PageTitle" runat="server" class="PopTitle"></td>
    </tr>
   </table>
  </div>
  <div id="PopBodyArea">
   <table class="PopTable">
    <tr>
     <td class="PopBodyBg" style="height: 300px; vertical-align: top;">
      <form id="aspnetForm" method="post" runat="server">
       <table style="width: 100%;">
        <tr id="ShowSelect" runat="server">
         <td>
          <b>Select Group:</b> 
          <asp:DropDownList ID="Groups" runat="server" AutoPostBack="true">
          </asp:DropDownList>
         </td>
        </tr>
        <tr id="UpdDelBtn" runat="server"> 
         <td class="SubTitle" style="text-align: center;">
          <asp:Button ID="Button1" Text="Update" runat="server"/>&nbsp;&nbsp;&nbsp;&nbsp;
          <asp:Button ID="Button2" Text="Remove" runat="server"/>
         </td>
        </tr>
        <tr id="ShowForm" runat="server">
         <td>
          <table style="width: 100%;">
           <tr>
            <td>
             <b>NOTE:</b> Edit group options in world viewer for Grid Owner account.<br />
             <b>Group Name:</b>
             <asp:TextBox ID="GroupName" runat="server" Columns="50" MaxLength="255" />
            </td>
           </tr>
          </table>
         </td>
        </tr>
        <tr id="AddBtn" runat="server">
         <td class="SubTitle" style="text-align: center;"> 
          <asp:Button id="Button3" text="Add" runat="server"/>
         </td>
        </tr>
       </table>
      </form>
     </td>
    </tr>
   </table>
  </div>
  <div id="PopFooterPos">
   <table style="width:100%;">
    <tr>
     <td class="PopCopyright">
      Copyright © <script type="text/javascript">document.write(new Date().getFullYear());</script>
      by My World. All rights reserved.
     </td>
    </tr>
   </table>
  </div>
 </body>
</html>
