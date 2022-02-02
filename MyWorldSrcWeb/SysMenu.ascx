<%@ Control Language="vb" AutoEventWireup="false" Inherits="SysMenu" CodeFile="SysMenu.ascx.vb" %>
   <script type="text/javascript">
    function ShowPOP(tPage) {
     window.open('/' + tPage + '.aspx', tPage, 'width=840,height=600,scrollbars=yes,resizable=yes');
    }
   </script>
   <table class="TopControl">
    <tr>
     <td class="TopNavbar">
      <table class="TopContainer">
       <tr>
        <td id="TopMenuBar" runat="server" class="TopMenuBar"></td>
       </tr>
      </table>
     </td>
    </tr>
   </table>
