<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Register.aspx.vb" Inherits="Register" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html>
 <head>
  <title>Register - My World</title>
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <link href="/styles/TreeView.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript" src="/Scripts/Cookie.js"></script>
  <script type="text/javascript" src="/Scripts/TreeView.js"></script>
  <script type="text/javascript">

   function ShowPOP(tPage) {
    window.open('/' + tPage + '.aspx', tPage, 'width=840,height=600,scrollbars=yes,resizable=yes');
   }

   var PriorAvatarEl = null;
   function ChangeAvatar(selectedEl) {
    // Add the selected class
    selectedEl.style.borderColor = "red";

    // Remove the selected class from the prior if there was a prior
    if (PriorAvatarEl !== null) {
     PriorAvatarEl.style.borderColor = null;
    }
    PriorAvatarEl = selectedEl;
    document.getElementById("Avatar").value = selectedEl.title;
   }
  </script>
 </head>
 <body id="BodyTag" runat="server" class="School">
  <!-- Built from Web Basic Page Template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle">
      My World New Account
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
     <td class="BodyBg">
      <form id="aspnetForm" runat="server">
       <table style="width: 100%;">
        <tr id="ContentDisp" runat="server">
         <td colspan="2" id="ShowContent" runat="server">
         </td>
        </tr>
        <tr>
         <td colspan="2" class="SubTitle">
          Avatar Selection:
         </td>
        </tr>
        <tr>
         <td class="FldTitleCol">
          Gender
         </td>
         <td class="FldEntryCol">
          <asp:RadioButtonList ID="Gender" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
          </asp:RadioButtonList>
         </td>
        </tr>
        <tr id="ShowAvatars" runat="server">
         <td id="Avatars" runat="server" colspan="2" style="min-height: 200px; background-color: #EEEEEE; vertical-align: top;">
         </td>
        </tr>
        <tr>
         <td colspan="2" class="SubTitle">
          Account Information:
         </td>
        </tr>
        <tr>
         <td class="FldTitleCol">
          First Name
         </td>
         <td class="FldEntryCol">
          <input type="text" id="FirstName" runat="server" size="20" maxlength="20" class="Form"/>
          <span class="Errors">*</span>
         </td>
        </tr>
        <tr>
         <td class="FldTitleCol">
          Last Name
         </td>
         <td class="FldEntryCol">
          <input type="text" id="LastName" runat="server" size="40" maxlength="40" class="Form"/>
          <span class="Errors">*</span>
         </td>
        </tr>
        <tr>
         <td class="FldTitleCol">
          Password
         </td>
         <td class="FldEntryCol">
          <input type="password" id="Password" runat="server" size="15" maxlength="30" class="Form"/> <span class="Errors">*</span> &nbsp;
          Confirm: <input type="password" id="Password2" runat="server" size="15" maxlength="30" class="Form"/> <span class="Errors">*</span>
         </td>
        </tr>
        <tr>
         <td class="FldTitleCol">
          Email Address
         </td>
         <td class="FldEntryCol">
          <input type="text" id="Email" runat="server" size="40" maxlength="40" class="Form"/>
          <span class="Errors">*</span>
         </td>
        </tr>
        <tr>
         <td class="FldTitleCol">
          Preference Settings
         </td>
         <td class="FldEntryCol">
          <asp:CheckBox ID="ImsInEmail" Checked="true" runat="server" Text="Receive Offline IMs in Email." />
          <asp:CheckBox ID="ListInDirect" runat="server" Text="List in Directory." /><br />
          <b>NOTE:</b> These may be set later in viewer Preferences, Chat tab.
         </td>
        </tr>
        <tr>
         <td colspan="2" style="text-align: center;" class="SubTitle">
          <asp:Button ID="Button1" runat="server" Text="Register" />&nbsp; &nbsp;
          <input type="button" onclick="window.location = '/Default.aspx';" value="Cancel" />
         </td>
        </tr>
        <tr>
         <td colspan="2">
          <span class="Errors">*</span> Required fields
         </td>
        </tr>
       </table>
       <input type="hidden" id="Avatar" runat="server" class="NoShow" />
       <input type="hidden" id="AvatarCnt" runat="server" class="NoShow" />
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
