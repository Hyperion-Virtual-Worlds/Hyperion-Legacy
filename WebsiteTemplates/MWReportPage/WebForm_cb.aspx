<%@ Page Language="VB" AutoEventWireup="false" CodeFile="$codebesiderootname$.vb" Inherits="$codebesideclassname$" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SysMenu" Src="~/SysMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html>
 <head>
  <title>Template - My World</title>
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <link href="/Styles/TopMenu.css" type="text/css" rel="stylesheet" />
  <link href="/styles/TreeView.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript" src="/scripts/Cookie.js"></script>
  <script type="text/javascript" src="/scripts/TopMenu.js"></script>
  <script type="text/javascript" src="/scripts/TreeView.js"></script>
  <script type="text/javascript">

  </script>
 </head>
 <body>
  <!-- Built from MyWorld Select template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle">Page Title</td>
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
           <td id="ReportDate" runat="server" style="width:50%;"></td>
           <td style="width:50%; text-align: right; cursor: pointer;" onclick="window.print();" class="NoPrint">
            Print <img src="/Images/Icons/print.gif" alt="" />
           </td>
          </tr>
          <tr>
           <td colspan="2" class="ReportTitle">Report Title</td>
          </tr>
         </table>
        </td>
       </tr>
       <tr>
        <td>
         <asp:GridView ID="gvDisplay" runat="server" AllowPaging="False" AutoGenerateColumns="False" GridLines="None" CellSpacing="1" Width="100%" HeaderStyle-Height="28px" PagerStyle-HorizontalAlign="Center" AlternatingRowStyle-CssClass="AltLine">
          <Columns>
           <asp:TemplateField HeaderText="Col Title" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title">
            <ItemTemplate>
             <span onclick="CallEdit(<%#Container.DataItem("KeyID")%>,'TempForm.aspx');" class="NavLink" onmouseover="this.className='NOverLink';" onmouseout="this.className='NavLink';">
              <%#Container.DataItem("Name").ToString().Trim()%>
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
 </body>
</html>
