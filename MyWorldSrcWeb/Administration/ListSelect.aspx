<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ListSelect.aspx.vb" Inherits="Administration_ListSelect" %>
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

   function CallEdit(tID,tAction) { // Add / Edit a record
    document.EditPage.KeyID.value=tID;
    document.EditPage.action=tAction;
    document.EditPage.submit();
   }

   function SetList(tList) {
    document.SelList.List.value = tList;
    document.SelList.submit();
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
     <td class="ProgTitle">Control Lists Maintenance</td>
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
        <td id="PageTitle" runat="server" class="PageTitle"> 
        </td>
       </tr>
       <tr>
        <td>
         <asp:GridView ID="gvDisplay" runat="server" AllowPaging="false" AutoGenerateColumns="False" GridLines="None" CellSpacing="1" style=" width:100%;" HeaderStyle-Height="28px" PagerStyle-HorizontalAlign="Center" AlternatingRowStyle-CssClass="AltLine">
          <Columns>
           <asp:TemplateField HeaderText="List Entries" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" ItemStyle-Width="75%">
            <ItemTemplate>
             <span onclick="CallEdit('<%#Container.DataItem("Name").ToString().Trim()%>','ListForm.aspx');" class="NavLink" onmouseover="this.className='NOverLink';" onmouseout="this.className='NavLink';">
              <%#Container.DataItem("Name").ToString().Trim()%>
             </span>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderText="Member Access" HeaderStyle-CssClass="Title" HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="25%">
            <ItemTemplate>
             <span onclick="SetList('<%#Container.DataItem("Parm1").ToString().Trim()%>');" class="NavLink" onmouseover="this.className='NOverLink';" onmouseout="this.className='NavLink';">
              Member List
             </span>
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
      </form>
     </td>
    </tr>
   </table>
  </div>
  <div id="FooterPos">
   <uc1:Footer id="Footer" runat="server"></uc1:Footer>
  </div>
  <form name="EditPage" action="" method="post">
   <input type="hidden" name="KeyID" value="" />
  </form>
  <form name="SelList" action="ListSelect.aspx" method="post">
   <input type="hidden" name="List" value="" />
  </form>
 </body>
</html>
