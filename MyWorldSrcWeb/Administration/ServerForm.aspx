<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ServerForm.aspx.vb" Inherits="Administration_ServerForm" %>
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
  
   function DoDel() {               // Forces a form post with events. Data must be in the form.
    document.getElementById('SetDel').checked = true;
    setTimeout('__doPostBack(\'SetDel\',\'\')', 0);
   }

   function ShowDelWin() {
    //alert('Show DelWin');
    document.getElementById("DivWinTrans").style.display = "block";
    document.getElementById("DivWinBox").style.display = "block";
   }

   function HideDelWin() {
    document.getElementById("DivWinBox").style.display = "none";
    document.getElementById("DivWinTrans").style.display = "none";
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
     <td class="ProgTitle">Server Manager</td>
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
      <b>NOTE:</b> Sever may only be removed when there are no regions assigned to it.
      Server has <span id="RegCnt" runat="server"></span> assigned.
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
           <td class="FldTitleCol">Server Name</td>
           <td id="FieldName" runat="server" class="FldEntryCol">
            <asp:TextBox id="ServerName" runat="server" Columns="32" maxLength="32" cssClass="Form"/>
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Account Name</td>
           <td class="FldEntryCol">
            <asp:TextBox id="AccountName" runat="server" Columns="32" maxLength="32" cssClass="Form"/>
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Password</td>
           <td class="FldEntryCol">
            <asp:TextBox id="AcctPassword" runat="server" Columns="32" maxLength="32" cssClass="Form"/>
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">External IP</td>
           <td class="FldEntryCol">
            <asp:TextBox id="externalIP" runat="server" Columns="20" maxLength="40" cssClass="Form"/>
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Internal IP</td>
           <td class="FldEntryCol">
            <asp:TextBox id="internalIP" runat="server" Columns="20" maxLength="40" cssClass="Form"/>
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Port List</td>
           <td class="FldEntryCol">
            <asp:TextBox id="PortList" runat="server" Columns="60" maxLength="120" cssClass="Form"/><br />
            List ports separated by commas. Range Start# - End #. ie: 9500-9509,9515
           </td>
          </tr>
         </table>
        </td>
       </tr>
       <tr id="UpdDelBtn" runat="server"> 
        <td class="SubTitle" style="text-align: center;">
         <asp:Button ID="Button1" Text="Update" runat="server"/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
         <input type="button" id="Button2" runat="server" value="Delete" onclick="ShowDelWin();"/>
        </td>
       </tr>
       <tr id="AddBtn" runat="server">
        <td class="SubTitle" style="text-align: center;"> 
         <asp:Button id="Button3" text="Add" runat="server"/>
        </td>
       </tr>
      </table>
       <input type="hidden" id="KeyID" value="" runat="server" />
       <input type="hidden" id="PriorPorts" value="" runat="server" />
       <input type="hidden" id="PriorExtIP" value="" runat="server" />
       <asp:CheckBox ID="SetDel" runat="server" cssClass="NoShow" AutoPostBack="true" />
      </form>
     </td>
    </tr>
   </table>
  </div>
  <div id="FooterPos">
   <uc1:Footer id="Footer" runat="server"></uc1:Footer>
  </div>
  <div id="DivWinTrans" runat="server" class="DivWinTrans"></div>
  <div id="DivWinBox" runat="server" class="DivWinBox">
   <div class="DivWin">
    <table style="width: 100%; height: 100%;">
     <tr>
      <td>
       <table class="WarnTable">
        <tr>
         <td style="height:25px;"> </td>
        </tr>
        <tr>
         <td class="WarnTitle">
          WARNING!
         </td>
        </tr>
        <tr>
         <td id="DelTitle" runat="server" class="WarnText">
         </td>
        </tr>
        <tr>
         <td class="WarnMsg">
          You are about to permanently remove this entry!
         </td>
        </tr>
        <tr>
         <td style="text-align: center;">
          <input type="button" value="Delete" onclick="DoDel();"/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
          <input type="button" value="Cancel" onclick="HideDelWin();"/>
         </td>
        </tr>
        <tr>
         <td style="height:25px;"> </td>
        </tr>
       </table>
      </td>
     </tr>
    </table>
   </div>
  </div>
 </body>
</html>
