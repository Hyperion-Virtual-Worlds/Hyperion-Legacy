<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Economy.aspx.vb" Inherits="Administration_Economy" %>
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
    document.getElementById("DivWinBox").removeAttribute('style');
    document.getElementById("DivWinTrans").removeAttribute('style');
    document.getElementById('SetDel').checked = true;
    setTimeout('__doPostBack(\'SetDel\',\'\')', 0);
   }

   function ShowDelWin() {
    //alert('Show DelWin');
    document.getElementById("DivWinTrans").style.display = "block";
    document.getElementById("DivWinBox").style.display = "block";
   }

   function HideDelWin() {
    document.getElementById("DivWinBox").removeAttribute('style');
    document.getElementById("DivWinTrans").removeAttribute('style');
   }

   function DoPrice(Type, Price) {
    document.getElementById('SetType').value = Type;
    document.getElementById('SetPrice').value = Price;
    setTimeout('__doPostBack(\'SetType\',\'\')', 0);
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
     <td class="ProgTitle">Economy Setup</td>
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
     <td><b>TIP:</b> The Economy settngs may be backed up and restored using the 
      <b>Website Administration,</b> <b>Download Config</b> and <b>Load Config</b> options.</td>
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
       <tr id="ShowUpdate" runat="server">
        <td>
         <table style="width:100%;">
          <tr>
           <td>
            <b>Update Economy Level:</b><br />
            You may change your Economy level as required for your world.<br />
            <asp:RadioButtonList ID="SetLevel" runat="server" RepeatDirection="Horizontal">
             <asp:ListItem Text="1. No Economy" Value="1"/>
             <asp:ListItem Text="2. Access Control" Value="2"/>
             <asp:ListItem Text="3. Full Economy" Value="3"/>
            </asp:RadioButtonList>
            <p><b>NOTE:</b> Selecting "1. No Economy" will remove all the Economy settings for your world!</p>
           </td>
          </tr>
          <tr id="ShowLvl3A" runat="server">
           <td style="height: 25px;" class="SubTitle">PayPal Account</td>
          </tr>
          <tr id="ShowLvl3B" runat="server">
           <td style="height: 40px; vertical-align: top;">
            <span id="EmailName" runat="server" style="font-weight: bold;"></span>: This is the email address for your PayPal account.<br />
            <asp:TextBox id="PayPalEmail" Columns="60" maxLength="150" cssClass="Form" runat="server"/>
           </td>
          </tr>
          <tr id="ShowLvl3C" runat="server">
           <td style="height: 30px; vertical-align: top;">
            <span id="PPMerchName" runat="server" style="font-weight: bold;"></span>:
            <asp:TextBox id="PPMerchantID" runat="server" Columns="20" maxLength="20" cssClass="Form"/>
            Get this from your PayPal account.
           </td>
          </tr>
          <tr id="ShowLvl3D" runat="server">
           <td style="height: 25px;" class="SubTitle">World $ Exchange Controls</td>
          </tr>
          <tr id="ShowLvl3E" runat="server">
           <td style="height: 40px; vertical-align: top;">
            <span id="MaxBuyName" runat="server" style="font-weight: bold;"></span>:
            <span id="MaxBuyText" runat="server"></span><br />
            $<asp:TextBox id="MaxBuy" runat="server" Columns="4" maxLength="4" cssClass="Form"/> Integer values only.
           </td>
          </tr>
          <tr id="ShowLvl3F" runat="server">
           <td style="height: 40px; vertical-align: top;">
            <span id="MaxSellName" runat="server" style="font-weight: bold;"></span>:
            <span id="MaxSellText" runat="server"></span><br />
            $<asp:TextBox id="MaxSell" runat="server" Columns="4" maxLength="4" cssClass="Form"/> Integer values only.
           </td>
          </tr>
          <tr id="ShowLvl3G" runat="server">
           <td style="height: 40px; vertical-align: top;">
            <span id="BuySellName" runat="server" style="font-weight: bold;"></span>:
            <span id="BuySellText" runat="server"></span><br />
            <asp:TextBox id="BuySellTime" runat="server" Columns="3" maxLength="3" cssClass="Form"/> Minimum hours between transaction
           </td>
          </tr>
          <tr id="ShowLvl3H" runat="server">
           <td style="height: 40px; vertical-align: top;">
            <span id="ExFeeName" runat="server" style="font-weight: bold;"></span>:
            <span id="ExFeeText" runat="server"></span><br />
            <asp:TextBox id="ExchangeFee" runat="server" Columns="1" maxLength="2" cssClass="Form"/> % Charged on transactions. Small profit plus any additional tax rates!
           </td>
          </tr>
          <tr id="ShowLvl3I" runat="server">
           <td style="height: 40px; vertical-align: top;">
            <span id="ExRateName" runat="server" style="font-weight: bold;"></span>:
            <span id="ExRateText" runat="server"></span><br />
            World $<asp:TextBox id="ExchangeRate" runat="server" Columns="3" maxLength="3" cssClass="Form"/> to $1 USD
           </td>
          </tr>
          <tr id="ShowLvl3J" runat="server">
           <td style="height: 25px;" class="SubTitle">Land Sale Settings</td>
          </tr>
          <tr id="ShowLvl3K" runat="server">
           <td>
            <asp:GridView ID="gvDisplay" runat="server" AllowPaging="false" AutoGenerateColumns="False" GridLines="None" CellSpacing="1" style=" width:50%;" HeaderStyle-Height="28px" PagerStyle-HorizontalAlign="Center">
             <Columns>
              <asp:TemplateField HeaderText="Region Type" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" ItemStyle-Width="60%">
               <ItemTemplate>
                <%#Container.DataItem("Name").ToString().Trim()%>
               </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField HeaderText="Price" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title">
               <ItemTemplate>
                $<input type="text" value="<%#Container.DataItem("Nbr2").ToString()%>" size="4" onchange="DoPrice(<%#Container.DataItem("Nbr1").ToString()%>,this.value);" />
               </ItemTemplate>
              </asp:TemplateField>
             </Columns>
             <PagerSettings FirstPageImageUrl="/Images/paging/First.jpg" LastPageImageUrl="/Images/paging/Last.jpg" Mode="NextPreviousFirstLast" NextPageImageUrl="/Images/paging/Next.jpg" PreviousPageImageUrl="/Images/paging/Prior.jpg" />
            </asp:GridView><br />
           </td>
          </tr>
          <tr>
           <td style="height: 25px;" class="SubTitle">World Bank Account</td>
          </tr>
          <tr>
           <td>
            <b>User Name:</b> <asp:TextBox id="WBUserName" runat="server" Columns="32" maxLength="32" cssClass="Form"/>
            <b>Last Name:</b> <asp:TextBox id="WBLastName" runat="server" Columns="32" maxLength="32" cssClass="Form"/><br />
            <b>UUID:</b> <span id="WBUUID" runat="server" style="font-weight: bold; font-size: 14px;"></span><br />
            Place this UUID in the Halcyon.ini, [Economy] section for EconomyBaseAccount.<br />
            If applicable also set the PriceUpload and PriceGroupCreate settings.<br />
            <b>Current Balance:</b> $<span id="Balance" runat="server"></span> &mdash; Logon to account to see Transaction History.
           </td>
          </tr>
          <tr id="ShowLvl2" runat="server">
           <td>
            Add to World Bank Funds: $<asp:TextBox id="WorldFunds" runat="server" Columns="8" maxLength="8" cssClass="Form"/>
           </td>
          </tr>
          <tr id="UpdDelBtn" runat="server"> 
           <td class="SubTitle" style="text-align: center;">
            <asp:Button ID="Button1" Text="Update" runat="server"/>
           </td>
          </tr>
         </table>
        </td>
       </tr>
       <tr id="ShowSetup" runat="server">
        <td>
         <table style="width:100%;">
          <tr>
           <td>
            <p>Before you enable Economy in your world, there are some things you need to know:</p>
            <p>There are three level of Economy implementation:</p>
            <p><b>1. No Economy</b> - This level does not use any world money, content may be bought and sold only for $0. Everything is free.
               This option does <b>NOT</b> require any setup.</p>
            <p><b>2. Use Money as Access Control</b> - This level does not use an economy, but it provides a way to manage who has access to these features:<br />
            &nbsp; &nbsp;<b>*</b> Upload any content - Mesh, textures or linksets, anything that may be imported.<br />
            &nbsp; &nbsp;<b>*</b> Create Groups - Allow persons to create and manage groups in World.<br />
            &nbsp; These may have prices set in the Halcyon.ini [Economy] section. If uploads were assigned $1, and Groups assigned $5, 
            those who may upload can be given $1 and those who may create groups be assigned $5 and allows upload. Use of these 
            options never reduce the amount the person has. This level requires the establishment of the World Bank user account and it 
            supplied with a starting amount of cash for assignment.</p>
            <p><b>3. Full Economy</b> - World Economy and Exchange is enabled. Website has been built to support transactions through use of a PayPal account online.
            Requires entry of your PayPal MerchantID for use in the Buy$ and Sell$ pages for sending transactions to PayPal, and put the PayPal
            email address in the IPN page. These pages may be modified to use your bank's Merchant Services API tools instead of PayPal.<br />
            <b>NOTE:</b> If you live or operate your world in the United States of America, and your world has a money
            exchange to buy and sell your world currency, you will be subject to IRS and FinCEN rules for operation. 
            In other countries there may also be laws governing operation and income reporting. See
            <a href="https://www.virtualcurrencyreport.com/2014/10/fincen-issues-new-rulings-covering-virtual-currency-exchanges-and-payment-processors/" target="_blank">
             FinCEN Issues New Rulings Covering Virtual Currency Exchanges and Payment Processors</a>.</p>
            <b>Economy Level:</b>
            <asp:RadioButtonList ID="Level" runat="server" RepeatDirection="Horizontal">
             <asp:ListItem Text="2. Access Control" Value="2"/>
             <asp:ListItem Text="3. Full Economy" Value="3"/>
            </asp:RadioButtonList>
            <p>Clicking the Install button Will set up the level of Economy selected. Then you may change settings for the selected level.</p>
           </td>
          </tr>
          <tr id="AddBtn" runat="server">
           <td class="SubTitle" style="text-align: center;"> 
            <asp:Button id="Button3" text="Install" runat="server"/>
            <asp:TextBox id="WBUserName1" runat="server" Columns="32" maxLength="32" cssClass="NoShow"/>
            <asp:TextBox id="WBLastName1" runat="server" Columns="32" maxLength="32" cssClass="NoShow"/>
           </td>
          </tr>
         </table>
        </td>
       </tr>
      </table>
       <asp:CheckBox ID="SetDel" runat="server" cssClass="NoShow" AutoPostBack="true" />
       <asp:TextBox ID="SetType" runat="server" CssClass="NoShow" AutoPostBack="true" />
       <asp:TextBox ID="SetPrice" runat="server" CssClass="NoShow" />
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
          You are about to permanently remove the Economy Setting!
         </td>
        </tr>
        <tr>
         <td style="text-align: center;">
          <input type="button" value="Remove" onclick="DoDel();"/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
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
