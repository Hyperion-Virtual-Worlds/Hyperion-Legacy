<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EstateForm.aspx.vb" Inherits="Administration_EstateForm" %>
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

   function CallEdit(tID, tAction) { // Add / Edit a record
    document.EditPage.KeyID.value = tID;
    document.EditPage.action = tAction;
    document.EditPage.submit();
   }

   function SetEstate(eID, regID) {
    document.getElementById('Estate').value = eID;
    document.getElementById('Region').value = regID;
    setTimeout('__doPostBack(\'Estate\',\'\')', 0);
   }

   function DoRefresh() {
    document.getElementById('Refresh').checked = true;
    //setTimeout('__doPostBack(\'Refresh\',\'\')', 0);
    __doPostBack('Refresh', '');
   }

   function DoDel() {               // Forces a form post with events. Data must be in the form.
    document.getElementById('SetDel').checked = true;
    setTimeout('__doPostBack(\'SetDel\',\'\')', 0);
   }

   function ShowDelWin() {
    //alert('Show DelWin');
    document.getElementById("DivWinTrans").style.display = "block";
    document.getElementById("DivWinBox").style.display = "block";
   }

   function HideDelWin() {
    document.getElementById("DivWinBox").style.display = "none";
    document.getElementById("DivWinTrans").style.display = "none";
   }

  </script>
 </head>
 <body id="BodyTag" runat="server">
  <!-- Built from MyWorld Form template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle">Estate Management</td>
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
         <table style="width: 100%;">
          <tr id="ShowID" runat="server">
           <td class="FldTitleCol">EstateID</td>
           <td id="EstateID" runat="server" class="FldEntryCol">
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Estate Name</td>
           <td class="FldEntryCol">
            <asp:TextBox id="EstateName" runat="server" Columns="20" maxLength="40" cssClass="Form"/>
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Owner</td>
           <td class="FldEntryCol">
            <asp:DropDownList ID="SelOwner" runat="server">
            </asp:DropDownList>
           </td>
          </tr>
          <tr>
           <td class="FldTitleCol">Allow Abuse Email</td>
           <td class="FldEntryCol">
            <asp:CheckBox id="AbuseEmail" runat="server" Text="Allow Abuse email to Estate Owner" AutoPostBack="true" />
           </td>
          </tr>
          <tr id="ShowEmail" runat="server">
           <td class="FldTitleCol">Email Address</td>
           <td class="FldEntryCol">
            <asp:TextBox id="EmailAddr" runat="server" Columns="30" maxLength="60" cssClass="Form"/>
           </td>
          </tr>
         </table>
        </td>
       </tr>
       <tr id="UpdDelBtn" runat="server"> 
        <td class="SubTitle" style="text-align: center;">
         <asp:Button ID="Button1" Text="Update" runat="server"/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
         <input type="button" id="Button2" runat="server" value="Delete" onclick="ShowDelWin();"/>
        </td>
       </tr>
       <tr id="AddBtn" runat="server">
        <td class="SubTitle" style="text-align: center;"> 
         <asp:Button id="Button3" text="Add" runat="server"/>
        </td>
       </tr>
       <tr id="ShowSubTitle" runat="server"> 
        <td class="PageTitle">Estate Regions</td>
       </tr>
       <tr id="ShowRegions" runat="server">
        <td>
         <asp:GridView ID="gvDisplay" runat="server" AllowPaging="false" AutoGenerateColumns="False" GridLines="None" CellSpacing="1" style=" width:100%;" HeaderStyle-Height="28px" PagerStyle-HorizontalAlign="Center" AlternatingRowStyle-CssClass="AltLine">
          <Columns>
           <asp:TemplateField HeaderText="Name" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" ItemStyle-Width="40%">
            <ItemTemplate>
             <span onclick="CallEdit('<%#Container.DataItem("RegionID")%>','RegionForm.aspx');" class="NavLink" onmouseover="this.className='NOverLink';" onmouseout="this.className='NavLink';">
              <%#Container.DataItem("RegionName").ToString().Trim()%>
             </span>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderText="Owner" ItemStyle-Width="20%" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title">
            <ItemTemplate>
             <%#Container.DataItem("Owner").ToString()%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderText="Estate" HeaderStyle-CssClass="Title" HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="25%">
            <ItemTemplate>
             <select name="<%#Container.DataItem("RegionID").ToString()%>" size="1" onchange="SetEstate(this.value,'<%#Container.DataItem("RegionID").ToString()%>');">
              <%#EstateList(Container.DataItem("RegionID").ToString())%>
             </select>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderText="Status" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title">
            <ItemTemplate>
             <%#ShowStatus(Container.DataItem("Status")).ToString()%>
            </ItemTemplate>
           </asp:TemplateField>
          </Columns>
          <PagerSettings FirstPageImageUrl="/Images/paging/First.jpg" LastPageImageUrl="/Images/paging/Last.jpg" Mode="NextPreviousFirstLast" NextPageImageUrl="/Images/paging/Next.jpg" PreviousPageImageUrl="/Images/paging/Prior.jpg" />
         </asp:GridView>
        </td>
       </tr>
      </table>
       <input type="hidden" id="KeyID" value="" runat="server" />
       <asp:CheckBox ID="SetDel" runat="server" style="display:none;" AutoPostBack="true" />
       <asp:TextBox ID="Estate" runat="server" cssClass="NoShow" AutoPostBack="true" />
       <input type="hidden" id="Region" value="" runat="server" />
       <asp:CheckBox ID="Refresh" runat="server" Text="" AutoPostBack="true" CssClass="NoShow" />
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
          You are about to permanently remove this Estate!
         </td>
        </tr>
        <tr>
         <td style="text-align: center;">
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
  <form name="EditPage" action="" method="post">
   <input type="hidden" name="KeyID" value="" />
  </form>
 </body>
</html>
