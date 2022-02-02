<%@ Page Language="VB" AutoEventWireup="false" CodeFile="GridManager.aspx.vb" Inherits="Administration_GridManager" %>
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

   var aVar = setInterval(DoRefresh, 60000);  // trigger refresh once a minute
   var RefreshTF = true;

   function DoRestart(aUUID) {
    document.getElementById('ReStart').value = aUUID;
    // Disable calling icon
    document.getElementById(aUUID + "RES").src = "/Images/Icons/RestartIconDis.png";
    document.getElementById(aUUID + "RES").onclick = '';
    document.getElementById(aUUID + "RES").style = '';
    // disable stop button
    document.getElementById(aUUID + "ACT").src = "/Images/Icons/StopIconDis.png";
    document.getElementById(aUUID + "ACT").onclick = '';
    document.getElementById(aUUID + "ACT").style = '';
    document.getElementById(aUUID + "STA").src = "/Images/Icons/Closing.png";
    //setTimeout('__doPostBack(\'ReStart\',\'\')', 0);
    __doPostBack('ReStart', '');
   }

   function DoShutdown(aUUID) {
    document.getElementById('Quit').value = aUUID;
    // Disable calling icon
    document.getElementById(aUUID + "ACT").src = "/Images/Icons/StopIconDis.png";
    document.getElementById(aUUID + "ACT").onclick = '';
    document.getElementById(aUUID + "ACT").style = '';
    // Disable Restart button
    document.getElementById(aUUID + "RES").src = "/Images/Icons/RestartIconDis.png";
    document.getElementById(aUUID + "RES").onclick = '';
    document.getElementById(aUUID + "RES").style = '';
    document.getElementById(aUUID + "STA").src = "/Images/Icons/Closing.png";
    //setTimeout('__doPostBack(\'Quit\',\'\')', 0);
    __doPostBack('Quit', '')
   }

   function DoStart(aUUID) {
    alert("Start region on server!");
    //document.getElementById('Start').value = aUUID;
    //// Disable calling icon
    //document.getElementById(aUUID + "ACT").src = "/Images/Icons/StartIconDis.png";
    //document.getElementById(aUUID + "ACT").onclick = "";
    //document.getElementById(aUUID + "ACT").style = '';
    //// Disable Restart button
    //document.getElementById(aUUID + "RES").src = "/Images/Icons/RestartIconDis.png";
    //document.getElementById(aUUID + "RES").onclick = '';
    //document.getElementById(aUUID + "RES").style = '';
    //document.getElementById(aUUID + "STA").src = "/Images/Icons/Starting.png";
    //setTimeout('__doPostBack(\'Start\',\'\')', 0);
    //__doPostBack('Start', '')
   }

   function DoRefresh() {
    if (RefreshTF) {
     document.getElementById('Refresh').checked = true;
     //setTimeout('__doPostBack(\'Refresh\',\'\')', 0);
     __doPostBack('Refresh', '');
    }
   }

   function OpenMsg(tID,tTitle) {                  // Page called message access
    RefreshTF = false;
    document.getElementById("MsgTitle").value = tTitle;
    document.getElementById("RegID").value = tID;
    document.getElementById("DivWinTrans").style.display = "block";
    document.getElementById("DivWinBox").style.display = "block";
   }

   function CloseMsg() {
    RefreshTF = true;
    document.getElementById("RegID").value = "";
    document.getElementById("DivWinBox").style.display = "none";
    document.getElementById("DivWinTrans").style.display = "none";
   }

   function SendMsg() {                     // Send message
    document.getElementById('MsgOut').value = document.getElementById('MsgText').value;
    document.getElementById('Send').checked = true;
    setTimeout('__doPostBack(\'Send\',\'\')', 0);
   }

   function SetSort(tOrder) {               // Set Page display order
    document.getElementById("Order").value = tOrder;
    setTimeout('__doPostBack(\'Order\',\'\')', 0);
   }

   function CallEdit(tID,tAction) {         // Add / Edit a record
    document.EditPage.KeyID.value=tID;
    document.EditPage.action=tAction;
    document.EditPage.submit();
   }

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
     <td class="ProgTitle">Grid Manager</td>
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
         <asp:GridView ID="gvDisplay" runat="server" AllowPaging="False" AutoGenerateColumns="False" GridLines="None" CellSpacing="1" Width="100%" HeaderStyle-Height="28px" PagerStyle-HorizontalAlign="Center" AlternatingRowStyle-CssClass="AltLine">
          <Columns>
           <asp:TemplateField ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title">
            <HeaderTemplate>
             <div style="width: 100%; text-align:left; cursor: pointer;">
              <span onclick="SetSort('Name');" title="Order by Name">Name (MemCnt)</span>
             </div>
            </HeaderTemplate>
            <ItemTemplate>
             <span onclick="CallEdit('<%#Container.DataItem("UUID").ToString().Trim()%>','RegionConfig.aspx');" title="<%#Container.DataItem("UUID").ToString().Trim()%>" class="NavLink" onmouseover="this.className='NOverLink';" onmouseout="this.className='NavLink';">
              <%#Container.DataItem("regionName").ToString().Trim()%> <%#IIf(Container.DataItem("UserCnt") > 0, "(" + Container.DataItem("UserCnt").ToString().Trim() + ")", "")%>
             </span>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField ItemStyle-Width="10%" HeaderStyle-CssClass="Title">
            <HeaderTemplate>
             <span onclick="SetSort('Map');" title="Order by Map" style="cursor: pointer;">Map</span>
            </HeaderTemplate>
            <ItemTemplate>
             <%#Container.DataItem("Map").ToString().Trim()%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField ItemStyle-Width="6%" HeaderStyle-CssClass="Title">
            <HeaderTemplate>
             <span onclick="SetSort('Port');" title="Order by Port" style="cursor: pointer;">Port</span>
            </HeaderTemplate>
            <ItemTemplate>
             <%#Container.DataItem("Port").ToString().Trim()%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField ItemStyle-Width="16%" ItemStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title">
            <HeaderTemplate>
             <div style="width: 100%; text-align:left; cursor: pointer;">
              <span onclick="SetSort('Owner');" title="Order by Owner">Owner</span>
             </div>
            </HeaderTemplate>
            <ItemTemplate>
             <%#Container.DataItem("OwnerName").ToString().Trim()%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField ItemStyle-Width="14%" ItemStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title">
            <HeaderTemplate>
             <span onclick="SetSort('Status');" title="Order by Status" style="cursor: pointer;">Status</span>
            </HeaderTemplate>
            <ItemTemplate>
             <%#ShowStatus(Container.DataItem("UUID").ToString(), Container.DataItem("status")).ToString()%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="Title">
            <HeaderTemplate>
             Msg
            </HeaderTemplate>
            <ItemTemplate>
             <%#SetMsg(Container.DataItem("UUID").ToString(), Container.DataItem("status"), Container.DataItem("regionName").ToString().Trim()).ToString()%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="Title">
            <HeaderTemplate>
             Restart
            </HeaderTemplate>
            <ItemTemplate>
             <%#SetRestart(Container.DataItem("UUID").ToString(), Container.DataItem("status")).ToString()%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="Title">
            <HeaderTemplate>
             Cmd
            </HeaderTemplate>
            <ItemTemplate>
             <%#SetAction(Container.DataItem("UUID").ToString(), Container.DataItem("status")).ToString()%>
            </ItemTemplate>
           </asp:TemplateField>
          </Columns>
          <PagerSettings FirstPageImageUrl="/Images/paging/First.jpg" LastPageImageUrl="/Images/paging/Last.jpg" Mode="NextPreviousFirstLast" NextPageImageUrl="/Images/paging/Next.jpg" PreviousPageImageUrl="/Images/paging/Prior.jpg" />
         </asp:GridView>
        </td>
       </tr>
      </table>
       <asp:TextBox ID="ReStart" runat="server" Text="" AutoPostBack="true" CssClass="NoShow" />
       <asp:TextBox ID="Start" runat="server" Text="" AutoPostBack="true" CssClass="NoShow" />
       <asp:TextBox ID="Quit" runat="server" Text="" AutoPostBack="true" CssClass="NoShow" />
       <asp:CheckBox ID="Refresh" runat="server" Text="" AutoPostBack="true" CssClass="NoShow" />
       <asp:TextBox ID="Order" runat="server" AutoPostBack="true" CssClass="NoShow" />
       <asp:CheckBox ID="Send" runat="server" cssClass="NoShow" AutoPostBack="true" />
       <input type="text" id="RegID" runat="server" value="" class="NoShow" />
       <input type="text" id="MsgOut" runat="server" value="" class="NoShow" />
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
         <td id="MsgTitle" class="WarnText">
         </td>
        </tr>
        <tr>
         <td>
          <textarea id="MsgText" rows="2" cols="60"></textarea>
         </td>
        </tr>
        <tr>
         <td style="text-align: center;">
          <input type="button" value="Send" onclick="SendMsg();"/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
          <input type="button" value="Cancel" onclick="CloseMsg();"/>
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
  <form name="EditPage" action="" method="post">
   <input type="hidden" name="KeyID" value="" />
  </form>
 </body>
</html>
