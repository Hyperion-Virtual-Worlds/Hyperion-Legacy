<%@ Page Language="VB" AutoEventWireup="false" CodeFile="WebSelect.aspx.vb" Inherits="Administration_WebSelect" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SysMenu" Src="~/SysMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html>
 <head>
  <title>Administration - My World</title>
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <link href="/Styles/TopMenu.css" type="text/css" rel="stylesheet" />
  <link href="/styles/TreeView.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript" src="/scripts/Cookie.js"></script>
  <script type="text/javascript" src="/scripts/TopMenu.js"></script>
  <script type="text/javascript" src="/scripts/TreeView.js"></script>
  <script type="text/javascript">
   
   function CallEdit(tID,tAction) { // Add / Edit a record
    document.EditPage.KeyID.value=tID;
    document.EditPage.action=tAction;
    document.EditPage.submit();
   }

   function ReSort(tAction, tID) { // trigger Sort action form
    //alert("Resort: "+tAction+", "+tID);
    document.getElementById('EID').value = tID;
    document.getElementById('Action').value = tAction;
    setTimeout('__doPostBack(\'EID\',\'\')', 0);
   }

   function SetPageID(aID) {
    document.getElementById("PageID").value = aID;
    setTimeout('__doPostBack(\'PageID\',\'\')', 0);
   }
   
  </script>
 </head>
 <body>
  <!-- Built from WebSys Select template v. 1.0 -->
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
    <tr>
     <td>
      <select size="1" class="Form" id="SelPageID" runat="server" onchange="SetPageID(this.value);">
      </select>
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
         <asp:GridView ID="gvDisplay" runat="server" AllowPaging="false" AutoGenerateColumns="False" GridLines="None" CellSpacing="1" style=" width:100%;" HeaderStyle-Height="28px" PagerStyle-HorizontalAlign="Center" AlternatingRowStyle-CssClass="AltLine">
          <Columns>
           <asp:TemplateField HeaderText="Content Title" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" ItemStyle-Width="85%">
            <ItemTemplate>
             <span onclick="CallEdit(<%#Container.DataItem("EntryID")%>,'WebForm.aspx');" class="NavLink" onmouseover="this.className='NOverLink';" onmouseout="this.className='NavLink';">
              <%#Container.DataItem("Name").ToString().Trim()%>
             </span><%#SetActive(Container.DataItem("Active"))%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderText="Display Order" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" ItemStyle-Width="15%">
            <ItemTemplate>
             <table style="width:100%;">
              <tr>
               <td onclick="ReSort('T','<%#Container.DataItem("EntryID")%>');" style="cursor: pointer;"><img src="/Images/Icons/MoveTop.gif" alt="Move to top" /></td>
               <td onclick="ReSort('U','<%#Container.DataItem("EntryID")%>');" style="cursor: pointer;"><img src="/Images/Icons/MoveUp.gif" alt="Move up" /></td>
               <td onclick="ReSort('D','<%#Container.DataItem("EntryID")%>');" style="cursor: pointer;"><img src="/Images/Icons/MoveDown.gif" alt="Move down" /></td>
               <td onclick="ReSort('B','<%#Container.DataItem("EntryID")%>');" style="cursor: pointer;"><img src="/Images/Icons/MoveBottom.gif" alt="Move to bottom" /></td>
              </tr>
             </table>
            </ItemTemplate>
           </asp:TemplateField>
          </Columns>
          <PagerSettings FirstPageImageUrl="/Images/paging/First.jpg" LastPageImageUrl="/Images/paging/Last.jpg" Mode="NextPreviousFirstLast" NextPageImageUrl="/Images/paging/Next.jpg" PreviousPageImageUrl="/Images/paging/Prior.jpg" />
         </asp:GridView>
        </td>
       </tr>
       <tr>
        <td id="ShowPages" runat="server" align="center"></td>
       </tr>
      </table>
       <asp:TextBox ID="PageID" runat="server" AutoPostBack="true" CssClass="NoShow" />
       <asp:TextBox ID="EID" runat="server" AutoPostBack="true" CssClass="NoShow" />
       <asp:TextBox ID="Action" runat="server" CssClass="NoShow" />
      </form>
     </td>
    </tr>
   </table>
   <table>
    <tr>
     <td><span class="Errors">*</span> Entry is Active on page.</td>
    </tr>
   </table>
  </div>
  <div id="FooterPos">
   <uc1:Footer id="Footer" runat="server"></uc1:Footer>
  </div>
  <form name="EditPage" action="" method="post">
   <input type="hidden" name="KeyID" value="" />
  </form>
 </body>
</html>
