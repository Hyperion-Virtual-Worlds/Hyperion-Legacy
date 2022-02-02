<%@ Page Language="VB" AutoEventWireup="false" CodeFile="UserSelect.aspx.vb" Inherits="Administration_UserSelect" %>
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

   function SortID(tID) {
    document.getElementById('SetSort').value = tID;
    setTimeout('__doPostBack(\'SetSort\',\'\')', 0);
   }

   function SetFind(aName) {
    document.getElementById('FindName').value = aName;
    setTimeout('__doPostBack(\'FindName\',\'\')', 0);
   }
   
   function SetDisabled(aChk) {
    document.getElementById('DisAccounts').checked = aChk;
    setTimeout('__doPostBack(\'DisAccounts\',\'\')', 0);
   }

  </script>
 </head>
 <body>
  <!-- Built from WebSite Select template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle">World Accounts</td>
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
      <img src="/Images/TreeView/Dot.gif" alt=""> <span class="TreeVItem">Search for Account:</span><br />
      <input type="text" id="Search" runat="server" cols="30" maxlength="60" class="Form" onchange="SetFind(this.value);" />
     </td>
    </tr>
    <tr>
     <td class="SidebarSpacer">&nbsp;</td>
    </tr>
    <tr>
     <td>
      <img src="/Images/TreeView/Dot.gif" alt=""> <span class="TreeVItem">Show Disabled Accounts:</span>
      <input type="checkbox" id="ShowDisabled" runat="server" onchange="SetDisabled(this.checked);" />
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
         <asp:GridView ID="gvDisplay" runat="server" AllowPaging="False" AutoGenerateColumns="False" GridLines="None" CellSpacing="1" Width="100%" HeaderStyle-Height="28px" PagerStyle-HorizontalAlign="Center" AlternatingRowStyle-CssClass="AltLine">
          <Columns>
           <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" ItemStyle-Width="30%">
            <HeaderTemplate>
             <span onclick="SortID('Name');" style="cursor: pointer;">User Accounts</span>
            </HeaderTemplate>
            <ItemTemplate>
             <span onclick="CallEdit('<%#Container.DataItem("UUID")%>','UserForm.aspx');" class="NavLink" onmouseover="this.className='NOverLink';" onmouseout="this.className='NavLink';">
              <%#Container.DataItem("username").ToString().Trim()%> <%#Container.DataItem("lastname").ToString().Trim()%> <span class="Errors"><%#Container.DataItem("Access").ToString().Trim()%></span>
             </span>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" ItemStyle-Width="30%">
            <HeaderTemplate>
             <span onclick="SortID('Email');" style="cursor: pointer;">Email</span>
            </HeaderTemplate>
            <ItemTemplate>
             <a href="mailto://<%#Container.DataItem("email")%>?body=Greetings <%#Container.DataItem("username").ToString().Trim()%>,"><%#Container.DataItem("email")%></a>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" ItemStyle-Width="20%">
            <HeaderTemplate>
             <span onclick="SortID('Created');" style="cursor: pointer;">Created</span>
            </HeaderTemplate>
            <ItemTemplate>
             <%#ShowStatus(Container.DataItem("created"))%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" ItemStyle-Width="20%">
            <HeaderTemplate>
             <span onclick="SortID('Last');" style="cursor: pointer;">Last Logout</span>
            </HeaderTemplate>
            <ItemTemplate>
             <%#ShowStatus(Container.DataItem("Status"))%>
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
       <asp:TextBox ID="SetSort" runat="server" AutoPostBack="true" CssClass="NoShow" />
       <asp:TextBox ID="FindName" runat="server" AutoPostBack="true" CssClass="NoShow" />
       <asp:CheckBox ID="DisAccounts" runat="server" AutoPostBack="true" CssClass="NoShow" />
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
 </body>
</html>
