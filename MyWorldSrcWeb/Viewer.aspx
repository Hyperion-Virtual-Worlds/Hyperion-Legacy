<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Viewer.aspx.vb" Inherits="Viewer" %>

<!DOCTYPE html>
<html>
 <head>
  <title>Viewer - My World</title>
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript">
  

  </script>
 </head>
 <body id="BodyTag" runat="server" style="margin-top: 19px;">
  <!-- Built from Web Basic Page template v. 1.0 -->
  <div id="Background" runat="server" style="width: 100%; height: 900px; background-image: url('/Images/Site/Island.jpg'); background-repeat: no-repeat; background-size: 100%;">
  </div>
  <div id="GridStatus" style=" opacity: 0.6; position: absolute; top: 25px; left: 15px; height: 150px; width: 200px; vertical-align: top; background-color: #FFFFFF; border: 1px solid #000000; border-radius: 10px;">
   <table style="width: 100%;">
    <tr>
     <td colspan="2" style="height: 10px;"></td>
    </tr>
    <tr>
     <td style="width: 50%; font-weight: bold; padding-left: 5px;">
      World Status: 
     </td>
     <td style="width: 50%;">
      <span id="GridStats" runat="server"></span>
     </td>
    </tr>
    <tr>
     <td style="width: 50%; font-weight: bold; padding-left: 5px;">
      Total Users: 
     </td>
     <td style="width: 50%;">
      <span id="GridTUsers" runat="server"></span>
     </td>
    </tr>
    <tr>
     <td style="width: 50%; font-weight: bold; padding-left: 5px;">
      Users Online: 
     </td>
     <td style="width: 50%;">
      <span id="GridOnline" runat="server"></span>
     </td>
    </tr>
    <tr>
     <td style="width: 50%; font-weight: bold; padding-left: 5px;">
      Region Count: 
     </td>
     <td style="width: 50%;">
      <span id="GridRegions" runat="server"></span>
     </td>
    </tr>
    <tr>
     <td colspan="2">
      <ul>
       <li><i>Public Regions:</i> <span id="PubRegions" runat="server"></span></li>
       <li><i>Private Regions</i> <span id="PrivRegions" runat="server"></span></li>
      </ul>
     </td>
    </tr>
	   <tr>
	    <td colspan="2" style="padding-left: 5px;">
	     <a href="Register.aspx"><b>New Account</b></a>
	    </td>
	   </tr>
   </table>
  </div>
  <div id="GridNews" style=" opacity: 0.6; position: absolute; top: 190px; left: 15px; height: 300px; width: 250px; vertical-align: top; background-color: #FFFFFF; border: 1px solid #000000; border-radius: 10px;">
   <table style="width: 100%;">
    <tr id="ContentDisp" runat="server">
     <td id="ShowContent" runat="server" style="vertical-align: top;">
      <!-- Body Content here -->
     </td>
    </tr>
   </table>
  </div>
  <div id="Footer" style=" position: absolute; bottom: 0; left: 0px; height: 100px; width: 100%; vertical-align: top; background-color: #FFFFFF; ">
   <table style="width: 100%;">
    <tr>
     <td style="font-family: Times New Roman, serif; font-size: 20pt; text-align: center;">
      My World
     </td>
    </tr>
   </table>
  </div>
 </body>
</html>
