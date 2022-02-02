<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Buy.aspx.vb" Inherits="Buy" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SysMenu" Src="~/SysMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html xmlns="https://www.w3.org/1999/xhtml" >
 <head>
  <title>Buy$ - My World</title>
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <link href="/Styles/TopMenu.css" type="text/css" rel="stylesheet" />
  <link href="/styles/TreeView.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript" src="/scripts/Cookie.js"></script>
  <script type="text/javascript" src="/scripts/TopMenu.js"></script>
  <script type="text/javascript" src="/scripts/TreeView.js"></script>
  <script type="text/javascript">

   function SendPayPal() {
    document.CallPayPal.submit();
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
     <td class="ProgTitle">Currency Exchange</td>
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
      <b class="MenuLink">Current Balance M$</b> <span id="MyTotal" runat="server"></span><br />
     </td>
    </tr>
    <tr id="ShowLimit" runat="server">
     <td>
      <b class="MenuLink">Purchase Limit:</b> $<span id="MyLimit" runat="server"></span> USD
      <span id="ShowHours" runat="server">in <span id="MyHours" runat="server"></span> hours between purchases.</span>
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
        <td class="PageTitle"> 
         Buy M$
        </td>
       </tr>
       <!-- Repeat the following tr content for each field to be processed. -->
       <tr>
        <td>
         <div style="width:50%; margin: 10px 25% 10px 25%; padding: 10px 10px 10px 10px; border: 2px solid black; border-radius: 8px;">
          <table style="width:100%;">
           <tr>
            <td colspan="2" style="vertical-align: bottom; text-align:center;">
             <div style="width:40%; text-align:center; display: inline-block;">
              My Dollars (M$)<br />
              <asp:TextBox id="MAmt" runat="server" Columns="10" maxLength="10" cssClass="Form" AutoPostBack="true" />
             </div>
             <div style="width:32px; display: inline-block; background-color: #00734c; color: #ffffff; font-size: 24px; border-radius: 8px; text-align: center;">
              =
             </div>
             <div style="width:40%; text-align:center; display: inline-block;">
              US Dollars (US$)<br />
              <asp:TextBox id="USDAmt" runat="server" Columns="10" maxLength="10" cssClass="Form" AutoPostBack="true" />
             </div>
            </td>
           </tr>
           <tr>
            <td style="width: 60%; height: 30px; font-size: 20px; font-weight: bold;">
             Purchasing
            </td>
            <td id="Purchase" runat="server" style="height: 50px; font-size: 20px; font-weight: bold; text-align: right;">
            </td>
           </tr>
           <tr>
            <td colspan="2" style="Height: 2px; border-bottom: 2px solid #e2e6e9;"></td>
           </tr>
           <tr>
            <td style="font-size: 20px; font-weight: normal;">
             Cost
            </td>
            <td id="USDCost" runat="server" style="height: 20px; font-size: 12px; font-weight: normal; text-align: right;">
            </td>
           </tr>
           <tr>
            <td style="font-size: 20px; font-weight: normal;">
             Transaction Fee<br />
             (May include tax rates)
            </td>
            <td id="Fee" runat="server" style="height: 20px; font-size: 12px; font-weight: normal; text-align: right;">
            </td>
           </tr>
           <tr>
            <td colspan="2" style="height: 2px; border-bottom: 2px solid #e2e6e9;"></td>
           </tr>
           <tr>
            <td style="height: 50px; font-size: 20px; font-weight: bold;">
             Total Purchase
            </td>
            <td id="USDTot" runat="server" style="height: 50px; font-size: 20px; font-weight: bold; text-align: right;">
            </td>
           </tr>
           <tr>
            <td colspan="2" style="text-align: right;">
             <asp:ImageButton ImageUrl="https://www.paypal.com/en_US/i/btn/btn_paynow_SM.gif" ID="PayPal" runat="server" OnClick="PayPal_Click" AlternateText="PayPal - The safer, easier way to pay online!" style="vertical-align: middle;" />
             <img alt="" src="https://www.paypal.com/en_US/i/scr/pixel.gif" width="1" height="1" />
            </td>
           </tr>
          </table>
         </div>
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
  <!-- https://www.paypal.com/cgi-bin/webscr -->
  <form name="CallPayPal" action="https://www.paypal.com/cgi-bin/webscr" method="post">
   <input type="hidden" name="cmd" value="_xclick" />
   <input type="hidden" id="business" runat="server" />
   <input type="hidden" name="lc" value="US" />
   <input type="hidden" name="item_name" value="Buy$" />
   <input type="hidden" id="item_number" runat="server" />
   <input type="hidden" id="custom" runat="server" />
   <input type="hidden" id="amount" runat="server" />
   <input type="hidden" name="currency_code" value="USD" />
   <input type="hidden" id="first_name" runat="server" />
   <input type="hidden" id="last_name" runat="server" />
   <input type="hidden" id="email" runat="server" />
   <input type="hidden" name="bn" value="MyWorld:btn_paynowCC_LG.gif:NonHosted" />
   <input type="hidden" name="image_url" value="https://<%=Request.ServerVariables("HTTP_HOST")%>/Images/Site/Logo.jpg" />
   <input type="hidden" name="notify_url" value="https://<%=Request.ServerVariables("HTTP_HOST")%>/IPN.aspx" />
   <input type="hidden" name="return" value="https://<%=Request.ServerVariables("HTTP_HOST")%>/BuyThanks.aspx"" />
   <input type="hidden" name="cbt" value="Return to My World" />
   <input type="hidden" name="rm" value="2" />
   <input type="hidden" name="cancel_return" value="http://<%=Request.ServerVariables("HTTP_HOST")%>/Buy.aspx" />
  </form>
 </body>
</html>
