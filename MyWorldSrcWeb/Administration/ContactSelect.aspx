<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ContactSelect.aspx.vb" Inherits="Administration_ContactSelect" %>
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

   function ReSort(tAction, tID) {   // trigger Sort action form
    //alert("Resort called: "+tAction);
    document.getElementById("KeyID").value = tID;
    document.getElementById("SortAction").value = tAction;
    setTimeout('__doPostBack(\'KeyID\',\'\')', 0);
   }

  </script>
 </head>
 <body id="BodyTag" runat="server">
  <!-- Built from WebSys Select template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle">Contact Us Maintenance</td>
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
         <asp:GridView ID="gvDisplay" runat="server" AllowPaging="False" AutoGenerateColumns="False" GridLines="None" CellSpacing="1" style=" width:100%;" HeaderStyle-Height="28px" PagerStyle-HorizontalAlign="Center" AlternatingRowStyle-CssClass="AltLine">
          <Columns>
           <asp:TemplateField HeaderText="Contact Entries" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" HeaderStyle-Width="90%">
            <ItemTemplate>
             <span onclick="CallEdit(<%#Container.DataItem("ContactID")%>,'ContactForm.aspx');" class="NavLink" onmouseover="this.className='NOverLink';" onmouseout="this.className='NavLink';">
              <%#Container.DataItem("Title").ToString().Trim()%>
             </span><%#ShowActive(Container.DataItem("Active"))%>
            </ItemTemplate>
           </asp:TemplateField>
           <asp:TemplateField HeaderText="Sorting" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="Title" HeaderStyle-Width="10%">
            <ItemTemplate>
             <table style=" width:100%;">
              <tr>
               <td onclick="ReSort('T','<%#Container.DataItem("ContactID")%>');" style="cursor: pointer;"><img src="/Images/Icons/MoveTop.gif" alt="Move to top" /></td>
               <td onclick="ReSort('U','<%#Container.DataItem("ContactID")%>');" style="cursor: pointer;"><img src="/Images/Icons/MoveUp.gif" alt="Move up" /></td>
               <td onclick="ReSort('D','<%#Container.DataItem("ContactID")%>');" style="cursor: pointer;"><img src="/Images/Icons/MoveDown.gif" alt="Move down" /></td>
               <td onclick="ReSort('B','<%#Container.DataItem("ContactID")%>');" style="cursor: pointer;"><img src="/Images/Icons/MoveBottom.gif" alt="Move to bottom" /></td>
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
       <input type="hidden" id="SortAction" runat="server" value="" />
       <asp:TextBox ID="KeyID" runat="server" AutoPostBack="true" CssClass="NoShow" />
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
