// <!--
// Each menu container must be listed here in the form of: i###:9&
// Where i = fixed letter indicating it is the image ID, 
//     ### = number of the menu entry 1-999,
//      : = Separator between identity and the menustate value
//      9 = one of 0 = Closed, 1 = Open state, Usually the menu list is closed at first open of page
//var MenuState='<%=mList%>';  This line must be placed on the page before this script is called.
//var MenuName='UniqueName'; This Line must be placed with the above line to identify the page. For this instance of the menu in this page.
// This script allows the TreeView menu state to be retained when a user changes the menu state. 
// The code in the page reads the cookie value if it exists to set the initial menu state.
// Script must be used with the Cookie.js script.

//var MenuState='<%=mList%>';                        // This is set by VBScript when the MenuState is initialized on pageload.
//var MenuName='TreeView';                           // This identifies the menu control name for the cookie. Used in the VB code also.

function MenuSelect(tPID,tLID,tMID) {
 var state='';
 if (document.getElementById(tLID).style.display=='') {
  document.getElementById(tLID).style.display='none';
  document.getElementById(tPID).src='/Images/TreeView/Plus.gif';
  state = '0';
  } 
 else 
  {
  document.getElementById(tLID).style.display='';
  document.getElementById(tPID).src='/Images/TreeView/Minus.gif';
  state = '1';
 }
 var expdate = new Date()
 var tregexp = new RegExp(tMID+':\\d');
 MenuState=MenuState.replace(tregexp,tMID+':'+state);
 expdate.setTime(expdate.getTime() +  (24 * 60 * 60 * 1000)) // Set to 24 hours by milliseconds
 SetCookie(MenuName,MenuState,expdate,'/');         // Use site cookie definition
 if (typeof(DebugTxt2)=="object") {
  DebugTxt2.innerText=MenuState;
 }
}

// -->
