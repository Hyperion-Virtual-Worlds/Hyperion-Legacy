<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TransHist.aspx.vb" Inherits="TransHist" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SysMenu" Src="~/SysMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html xmlns="https://www.w3.org/1999/xhtml" >
 <head>
  <title>History - My World</title>
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
     <td class="ProgTitle">Transaction History</td>
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
      <b class="MenuLink">Current Balance M$</b> <span id="MyTotal" runat="server"></span>
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
        <td id="ShowDates" runat="server" style="height: 30px;">
         <b>Select Month/Year transaction history:</b> 
         <asp:DropDownList ID="TransDate" runat="server" AutoPostBack="true">
         </asp:DropDownList> Lists only the months which contain transactions.
        </td>
       </tr>
       <tr>
        <td>
         <asp:GridView ID="gvDisplay" runat="server" AllowPaging="False" AutoGenerateColumns="False" GridLines="None" Width="100%" HeaderStyle-Height="28px" PagerStyle-HorizontalAlign="Center" BorderColor="#DDDDDD" BorderStyle="Solid" BorderWidth="2">
          <Columns>
           <asp:TemplateField HeaderText="Date / Time" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" HeaderStyle-BorderColor="#DDDDDD" HeaderStyle-BorderStyle="Solid" HeaderStyle-BorderWidth="2" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="15%" ItemStyle-BorderColor="#DDDDDD" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="2">
            <ItemTemplate>
             <%#FormatDateTime(Container.DataItem("DateTime"), DateFormat.ShortDate).ToString()%>
             <%#FormatDateTime(Container.DataItem("DateTime"), DateFormat.ShortTime).ToString()%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderText="Description" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" HeaderStyle-BorderColor="#DDDDDD" HeaderStyle-BorderStyle="Solid" HeaderStyle-BorderWidth="2" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="40%" ItemStyle-BorderColor="#DDDDDD" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="2">
            <ItemTemplate>
             <%#Container.DataItem("Description").ToString().Trim()%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderText="Debit" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" HeaderStyle-BorderColor="#DDDDDD" HeaderStyle-BorderStyle="Solid" HeaderStyle-BorderWidth="2" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="15%" ItemStyle-BorderColor="#DDDDDD" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="2">
            <ItemTemplate>
             <%#FmtMoney(Container.DataItem("Debit"), False).ToString().Trim()%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderText="Credit" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" HeaderStyle-BorderColor="#DDDDDD" HeaderStyle-BorderStyle="Solid" HeaderStyle-BorderWidth="2" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="15%" ItemStyle-BorderColor="#DDDDDD" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="2">
            <ItemTemplate>
             <%#FmtMoney(Container.DataItem("Credit"), False).ToString().Trim()%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderText="Balance" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" HeaderStyle-BorderColor="#DDDDDD" HeaderStyle-BorderStyle="Solid" HeaderStyle-BorderWidth="2" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="15%" ItemStyle-BorderColor="#DDDDDD" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="2">
            <ItemTemplate>
             <%#FmtMoney(Container.DataItem("Balance"), True).ToString().Trim()%>
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
 </body>
</html>
