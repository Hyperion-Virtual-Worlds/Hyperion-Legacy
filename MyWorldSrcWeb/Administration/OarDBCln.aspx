<%@ Page Language="VB" AutoEventWireup="false" CodeFile="OarDBCln.aspx.vb" Inherits="Administration_OarDBCln" %>
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
  
   function AddOMatch() {
    document.getElementById('AddOwnMat').checked = true;
    setTimeout('__doPostBack(\'AddOwnMat\',\'\')', 0);
   }

   function RemOEntry(tNum) {
    document.getElementById('RemOwnEnt').value = tNum;
    setTimeout('__doPostBack(\'RemOwnEnt\',\'\')', 0);
   }

   function AddCMatch() {
    document.getElementById('AddCrtMat').checked = true;
    setTimeout('__doPostBack(\'AddCrtMat\',\'\')', 0);
   }

   function RemCEntry(tNum) {
    document.getElementById('RemCrtEnt').value = tNum;
    setTimeout('__doPostBack(\'RemCrtEnt\',\'\')', 0);
   }

   function ShowDelWin() {
    //alert('Show DelWin');
    document.getElementById("DivWinTrans").style.display = "block";
    document.getElementById("DivWinBox").style.display = "block";
   }

   function DoCan() {                   // Forces a form post with events. Data must be in the form.
    HideDelWin();
    document.getElementById('SetCan').checked = true;
    setTimeout('__doPostBack(\'SetCan\',\'\')', 0);
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
     <td class="ProgTitle">Oar Import<br />DB Cleanup</td>
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
      <div id="PageTitle" runat="server" class="PageTitle">
      </div>
      <form id="aspnetForm" method="post" runat="server">
       <p id="ShowRegName" runat="server"></p>
       <div id="Step1" runat="server" style="min-height:300px;">
        <h2>1. Region Selection Where the OAR was Loaded.</h2>
        <p>Region must be offline to run this process on it.</p>
        <b>Select Region:</b>
        <asp:DropDownList ID="GetRegion" runat="server">
        </asp:DropDownList><br />
        <div class="SubTitle" style="text-align: center;">
         <asp:Button ID="Button1" Text="Select" runat="server"/>
        </div>
        <p><b>NOTE:</b> This process may only be run once after loading an OAR into a region. 
         Subsequent attempts to rerun on the same region to change assignments will fail to find anything to change.
         If it was not done correctly, reload the OAR, then process again here.
        </p>
       </div>
       <div id="Step2" runat="server" style="min-height:300px;">
        <h2>2. Old Accounts Entry.</h2>
        <p>Unzip the oar file into a folder of the same name. Open that folder, rename the file there adding a .tar extension to it. 
         Extract again into the same folder. Now you will see a text file containing the list of the old accounts.
          (Only if oar is a more recent version!) Open it, copy entire contents and paste in the entry below.</p>
        <b>Paste in list of old accounts:</b>
        <textarea id="OldAccounts" runat="server" rows="50" cols="80">
        </textarea>
        <div class="SubTitle" style="text-align: center;">
         <asp:Button ID="Button2" Text="Next Step" runat="server"/>&nbsp; &nbsp; &nbsp; 
         <input type="button" value="Go Back" onclick="DoCan();"/>
        </div>
       </div>
       <div id="Step3" runat="server" style="min-height:300px;">
        <h2>3. Match Owner Accounts.</h2>
        <p>Match the old Owner account to the new Owner account.</p>
        <div id="OwnersList" runat="server">
        </div>
        <div id="ShowOMatching" runat="server" style="padding: 2px;">
         <b>Old Account:</b> 
         <asp:DropDownList ID="OldOwners" runat="server">
         </asp:DropDownList> 
         <b>Grid Account</b> 
         <asp:DropDownList ID="GridAccts1" runat="server">
         </asp:DropDownList>
         <input type="button" value="Add" onclick="AddOMatch();" />
        </div>
        <div class="SubTitle" style="text-align: center;">
         <asp:Button ID="Button3" Text="Next Step" runat="server"/>&nbsp; &nbsp; &nbsp; 
         <input type="button" value="Go Back" onclick="DoCan();"/>
        </div>
       </div>
       <div id="Step4" runat="server" style="min-height:300px;">
        <h2>4. Match Creator Accounts.</h2>
        <p>Make sure the new creator is the same person in this world! Verify with old creator any change requests to new creator.</p>
        <div id="CreatorsList" runat="server">
        </div>
        <div id="ShowCMatching" runat="server" style="padding: 2px;">
         <b>Old Account:</b> 
         <asp:DropDownList ID="OldCreators" runat="server">
         </asp:DropDownList> 
         <b>Grid Account</b> 
         <asp:DropDownList ID="GridAccts2" runat="server">
         </asp:DropDownList>
         <input type="button" value="Add" onclick="AddCMatch();" />
        </div>
        <div class="SubTitle" style="text-align: center;">
         <asp:Button ID="Button4" Text="Next Step" runat="server"/>&nbsp; &nbsp; &nbsp; 
         <input type="button" value="Go Back" onclick="DoCan();"/>
        </div>
       </div>
       <div id="Step5" runat="server" style="min-height:300px;">
        <h2>5. Process Summary</h2>
        <div id="ShowSummary" runat="server">
        </div>
        <p>If this is correct to do, click the Apply button. If not, click Cancel and restart this process.</p>
        <div class="SubTitle" style="text-align: center;">
         <asp:Button ID="Button5" Text="Apply" runat="server"/>&nbsp; &nbsp; &nbsp; 
         <input type="button" value="Go Back" onclick="DoCan();"/>&nbsp; &nbsp; &nbsp; 
         <asp:Button ID="Button6" Text="Cancel" runat="server"/>
        </div>
       </div>
       <div id="Step6" runat="server" style="min-height:300px;">
        <h2>Processing Completed!</h2>
        <div id="ShowRefAccts" runat="server">
        </div>
       </div>
       <input type="hidden" id="StepNum" value="" runat="server" />
       <asp:CheckBox ID="SetCan" runat="server" class="NoShow" AutoPostBack="true" />
       <asp:CheckBox ID="AddOwnMat" runat="server" class="NoShow" AutoPostBack="true" />
       <asp:TextBox ID="RemOwnEnt" runat="server" class="NoShow" AutoPostBack="true" />
       <asp:CheckBox ID="AddCrtMat" runat="server" class="NoShow" AutoPostBack="true" />
       <asp:TextBox ID="RemCrtEnt" runat="server" class="NoShow" AutoPostBack="true" />
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
    <table style="width:100%; height:100%;">
     <tr>
      <td>
       <table class="WarnTable">
        <tr>
         <td style="height:25px;"> </td>
        </tr>
        <tr>
         <td id="DelTitle" runat="server" class="WarnText">
         </td>
        </tr>
        <tr>
         <td id="DispError" runat="server" class="WarnMsg">
         </td>
        </tr>
        <tr>
         <td style="text-align: center;">
          <input type="button" value="Cancel" onclick="DoCan();"/>
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
