<%@ Page Language="VB" AutoEventWireup="false" CodeFile="WebForm.aspx.vb" Inherits="WebForm" %>
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
  <script type="text/javascript" src="/Scripts/HTMLControls.js"></script>
  <script type="text/javascript">

   var oPreview = window;           // Indicate if Preview window is open or not.

   function ShowPreview() {
    if (oPreview == window) {       // Create Window if not opened
     oPreview = window.open('', 'PreView', 'width=950,height=750,scrollbars=yes,resizable=yes');
    }
    else {                          // give window focus
     oPreview.focus();
    }
    document.PrevWin.KeyID.value = document.getElementById("KeyID").value;
    document.PrevWin.submit();
   }

   function CloseDown() {           // Close open child windows if any are open.
    if (oPreview != window) {
     oPreview.close();
    }
   }

   function DoDel() {               // Forces a form post with events. Data must be in the form.
    document.getElementById('SetDel').checked = true;
    setTimeout('__doPostBack(\'SetDel\',\'\')', 0);
   }

   function ShowDelWin() {
    //alert('Show DelWin');
    document.getElementById("DivWinTrans").style.display="block";
    document.getElementById("DivWinBox").style.display="block";
   }

   function HideDelWin() {
    document.getElementById("DivWinBox").style.display="none";
    document.getElementById("DivWinTrans").style.display="none";
   }

  </script>
 </head>
 <body id="BodyTag" runat="server">
  <!-- Built from WebSite Form template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle">Website Maintenance</td>
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
      <!-- Body Content here -->
      <form id="aspnetForm" method="post" runat="server">
      <table style="width:100%;">
       <tr>
        <td>
         <table style="width:100%;">
          <tr>
           <td class="HTMLToolBar" style="border: 1 outset;">
            <img id="HTMLParagraph" src="/images/HTMLControls/ParagraphDis.gif" alt="" title="Set Paragraph" onclick="PutHTML('<p>','</p>');" style="border: 1px solid #d4d0c8;" />
            <img id="HTMLBold" src="/images/HTMLControls/BoldDis.gif" alt="" title="Set Bold" onclick="PutHTML('<b>','</b>');" style="border: 1px solid #d4d0c8;" />
            <img id="HTMLItalic" src="/images/HTMLControls/ItalicDis.gif" alt="" title="Set Italic" onclick="PutHTML('<i>','</i>');" style="border: 1px solid #d4d0c8;" />
            <img id="HTMLUnderline" src="/images/HTMLControls/UnderlineDis.gif" alt="" title="Set Underline" onclick="PutHTML('<u>','</u>');" style="border: 1px solid #d4d0c8;" />
            <img id="HTMLIndent" src="/images/HTMLControls/IndentDis.gif" alt="" title="Indent a Paragraph" onclick="PutHTML('<fmt>','');" style="border: 1px solid #d4d0c8;" />
            <img id="HTMLBreak" src="/images/HTMLControls/breakDis.gif" alt="" title="Insert a Break" onclick="PutHTML('','<br>');" style="border: 1px solid #d4d0c8;" />
            <img id="HTMLSuper" src="/images/HTMLControls/SuperDis.gif" alt="" title="Set Superscript" onclick="PutHTML('<sup>','</sup>');" style="border: 1px solid #d4d0c8;" />
            <img id="HTMLSub" src="/images/HTMLControls/SubDis.gif" alt="" title="Set Subscript" onclick="PutHTML('<sub>','</sub>');" style="border: 1px solid #d4d0c8;" />
            <img id="HTMLBullet" src="/images/HTMLControls/BulletDis.gif" alt="" title="Place a Bullet" onclick="PutHTML('<li>','</li>');" style="border: 1px solid #d4d0c8;" />
            <img id="HTMLIndentBullets" src="/images/HTMLControls/IndentBulletsDis.gif" alt="" title="Indent Bullets" onclick="PutHTML('<ul>','</ul>');" style="border: 1px solid #d4d0c8;" />
            <img id="HTMLSpan" src="/images/HTMLControls/SpanDis.gif" alt="" title="Place a Class" onclick="PutHTML('<span>','</span>');" style="border: 1px solid #d4d0c8;" />
            <!-- options not needed here.
            <img id="HTMLImage" src="/images/HTMLControls/ImageDis.gif" alt="" title="Place an Image" onclick="PutHTML('<img>','');" style="border: 1px solid #d4d0c8;" />
            <img id="HTMLLink" src="/images/HTMLControls/HyperlinkDis.gif" alt="" title="Place a Link" onclick="PutHTML('<a href>','</a>');" style="border: 1px solid #d4d0c8;" />
            <img id="HTMLAnchor" src="/images/HTMLControls/AnchorDis.gif" alt="" title="Place an Anchor" onclick="PutHTML('<a name>','</a>');" style="border: 1px solid #d4d0c8;" />
            <img id="HTMLEmail" src="/images/HTMLControls/EMailDis.gif" alt="" title="Place an Email" onclick="PutHTML('<a mail>','</a>');" style="border: 1px solid #d4d0c8;" />
            -->
            <img id="HTMLCenter" src="/images/HTMLControls/CenterDis.gif" alt="" title="Center selected text" onclick="PutHTML('<center>','</center>');" style="border: 1px solid #d4d0c8;" />
            <img id="HTMLTable" src="/images/HTMLControls/TableDis.gif" alt="" title="Insert a Table" onclick="PutHTML('<table>','</table>');" style="border: 1px solid #d4d0c8;" />
           </td>
          </tr>
          <tr> 
           <td id="PageTitle" runat="server" class="PageTitle"> 
           </td>
          </tr>
         </table>
        </td>
       </tr>
       <tr>
        <td>
         <table style=" width:100%;" cellspacing="1" cellpadding="1">
          <tr>
           <td class="FldTitleCol">Name</td>
           <td class="FldEntryCol">
            <input type="text" size="40" maxlength="40" id="Name" runat="server" class="Form" onfocus="OptionsOff();"/>
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Title</td>
           <td class="FldEntryCol">
            <input type="text" size="80" maxlength="100" id="Title" runat="server" class="Form" onfocus="OptionsOff();"/>
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Content</td>
           <td class="FldEntryCol">
            <textarea id="Content" runat="server" name="Content" rows="40" cols="80" class="Form" onfocus="OptionsOff();" onselect="OptionsOn(9);"></textarea>
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Optional Auto Show On</td>
           <td class="FldEntryCol">
            <input type="text" size="12" maxlength="10" id="AutoStart" runat="server" class="Form" onfocus="OptionsOff();"/>
            Date this entry will Auto Show at Midnight AM on the page.
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Optional Auto Expire On</td>
           <td class="FldEntryCol">
            <input type="text" size="12" maxlength="10" id="AutoExpire" runat="server" class="Form" onfocus="OptionsOff();"/>
            Date this entry will Auto Expire at Midnight AM and not show on the page.
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Active</td>
           <td class="FldEntryCol">
            <asp:CheckBox ID="Active" Runat="server" onfocus="OptionsOff();"/> 
            Show content on page when checked. Overrides Auto show date if manually set.
           </td>
          </tr>
         </table>
        </td>
       </tr>
       <tr id="UpdDelBtn" runat="server"> 
        <td style="text-align: center;" class="SubTitle">
         <asp:Button ID="Button1" Text="Update" runat="server"/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
         <input type="button" id="Button2" runat="server" value="Delete" onclick="ShowDelWin();"/>
        </td>
       </tr>
       <tr id="AddBtn" runat="server">
        <td style="text-align: center;" class="SubTitle"> 
         <asp:Button id="Button3" text="Add" runat="server"/>
        </td>
       </tr>
      </table>
       <input type="hidden" id="KeyID" value="" runat="server" />
       <asp:CheckBox ID="SetDel" runat="server" style="display:none;" AutoPostBack="true" />
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
       <table class="WarnTable" style="text-align:center;">
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
         <td align="center">
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
  <form name="PrevWin" action="PagePreview.aspx" method="post" target="PreView">
   <input type="hidden" name="KeyID" value="" />
  </form>
 </body>
</html>
