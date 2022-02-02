<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TransMDRpt.aspx.vb" Inherits="Administration_TransMDRpt" %>
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
     <td class="ProgTitle">Monthly Transaction Detail</td>
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
        <td id="ShowDates" runat="server" style="height: 30px;" class="NoPrint">
         <b>Select Month/Year Transaction History:</b> 
         <asp:DropDownList ID="TransDate" runat="server" AutoPostBack="true">
         </asp:DropDownList> Lists only the months which contain transactions.
        </td>
       </tr>
       <tr>
        <td>
         <table style="width:100%;">
          <tr>
           <td id="ReportDate" runat="server" style="width:50%;"></td>
           <td style="width:50%; text-align: right; cursor: pointer;" onclick="window.print();" class="NoPrint">
            <span id="PrintTitle" runat="server"></span> <img src="/Images/Icons/print.gif" alt="" />
           </td>
          </tr>
          <tr>
           <td colspan="2" class="ReportTitle">Buy / Sell Transaction History Detail by Month/Year</td>
          </tr>
          <tr>
           <td colspan="2" style="text-align: center;"><b><span id="RptDate" runat="server"></span></b></td>
          </tr>
         </table>
        </td>
       </tr>
       <tr>
        <td>
         <asp:GridView ID="gvDisplay" runat="server" AllowPaging="False" AutoGenerateColumns="False" GridLines="None" CellSpacing="1" Width="100%" HeaderStyle-Height="28px" PagerStyle-HorizontalAlign="Center" AlternatingRowStyle-CssClass="AltLine">
          <Columns>
           <asp:TemplateField HeaderText="Date / Time" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" HeaderStyle-BorderColor="#DDDDDD" HeaderStyle-BorderStyle="Solid" HeaderStyle-BorderWidth="2" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="15%" ItemStyle-BorderColor="#DDDDDD" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="2">
            <ItemTemplate>
             <%#FormatDateTime(Container.DataItem("DateTime"), DateFormat.ShortDate).ToString()%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderText="Description" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" HeaderStyle-BorderColor="#DDDDDD" HeaderStyle-BorderStyle="Solid" HeaderStyle-BorderWidth="2" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="70%" ItemStyle-BorderColor="#DDDDDD" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="2">
            <ItemTemplate>
             <%#Container.DataItem("transactionDescription").ToString().Trim()%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderText="Debit" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" HeaderStyle-BorderColor="#DDDDDD" HeaderStyle-BorderStyle="Solid" HeaderStyle-BorderWidth="2" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="15%" ItemStyle-BorderColor="#DDDDDD" ItemStyle-BorderStyle="Solid" ItemStyle-BorderWidth="2">
            <ItemTemplate>
             <%#FmtMoney(Container.DataItem("TransactionAmount")).ToString().Trim()%>
            </ItemTemplate>
           </asp:TemplateField>
          </Columns>
          <PagerSettings FirstPageImageUrl="/Images/paging/First.jpg" LastPageImageUrl="/Images/paging/Last.jpg" Mode="NextPreviousFirstLast" NextPageImageUrl="/Images/paging/Next.jpg" PreviousPageImageUrl="/Images/paging/Prior.jpg" />
         </asp:GridView>
        </td>
       </tr>
       <tr>
        <td>
         <table style="width:100%;">
          <tr>
           <td style="width: 85%; text-align: right; padding-right: 2px; font-weight: bold;">Total:</td>
           <td id="ShowTotal" runat="server" style="width: 15%; border: 2px solid #DDDDDD; text-align: right;"></td>
          </tr>
         </table>
        </td>
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
