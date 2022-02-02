<!--
/*
 This script allows the Cascade menu to execute an HTML form to call another page. 
 To use the showVals debugging, the page must have the following placed on the page and not inside another form.
 <span id="showVals"></span>
*/

CurrOpen='';
currOpen = new Array();
currActive = new Array();
TopMenuTimeout = null;
/*
 Menu operation theory:
  onmouseover placed on a menu entry will open the associated menu list. An onmouseout sets a timer event holding the 
  closewindow option for a time to allow the mouse to trigger the menu list onmouseover function allowing an array value 
  to be set indicating the mouse is in it. Placing the mouse over another menu entry will cancel the timeout, and close the open list.
  When the timer event fires, the flag is checked. If set the list stays open else the list is closed. 
  Each box in the currOpen array is checked for active mouseover. Each unflagged box in the list is closed.
  If the mouse did not trigger a onmouseover event, all open menu boxes will be closed. The timer holder is cleared.
*/

function TopMenuOver(aLevel,aParent,divID) {    // Activated by a Menu entry to open the associated menu list.
 //showVals.innerText=showVals.innerText+'TopMenuOver('+aLevel+','+aParent+','+divID+') called; ';
 if (aParent.substr(0,6)=='SubNav') {           // only on SubNav menus
  TopActivate(aLevel);                          // Keep Parent Level Active
 }
 document.getElementById(divID).className="TopListOn";
 if (currOpen[currOpen.length-1]!=divID) {      // Target menu list is not active open
  TopClose();                                   // close any open inactive menu list
  if (aParent.substr(0,6)=='TopNav') {          // TopNav menus
   document.getElementById(divID).style.left=document.getElementById(aParent).getBoundingClientRect().left+'px';
   //document.getElementById(divID).style.top=(document.getElementById(aParent).getBoundingClientRect().bottom-document.getElementById(aParent).getBoundingClientRect().top+document.getElementById(aParent).getBoundingClientRect().top)+'px';
   document.getElementById(divID).style.top=
    (document.getElementById(aParent).getBoundingClientRect().bottom - 
     document.getElementById(aParent).getBoundingClientRect().top+11)+'px';
   document.getElementById(divID).style.width=(document.getElementById(aParent).getBoundingClientRect().right-document.getElementById(aParent).getBoundingClientRect().left)+'px';
   currOpen.push(divID);
   currActive.push(0);
   //showVals.innerText=showVals.innerText+', opened:'+divID;
  }
  else {                                        // SubNav menus
   //showVals.innerText=showVals.innerText+', ParentRight='+document.getElementById(aParent).getBoundingClientRect().right+': Body Width='+document.body.offsetWidth;
   if (document.getElementById(aParent).getBoundingClientRect().right<=document.body.offsetWidth){// will fit in space to the right
    document.getElementById(divID).style.left=document.getElementById(aParent).getBoundingClientRect().right+'px';
   }
   else {                                      // Set cascade to the left
    document.getElementById(divID).style.left=(document.getElementById(aParent).getBoundingClientRect().left-(document.getElementById(aParent).getBoundingClientRect().right-document.getElementById(aParent).getBoundingClientRect().left))+'px';
   }
   document.getElementById(divID).style.top=(document.getElementById(aParent).getBoundingClientRect().top)+'px';
   document.getElementById(divID).style.width=document.getElementById(aParent).getBoundingClientRect().right-document.getElementById(aParent).getBoundingClientRect().left+'px';
   currOpen.push(divID);
   currActive.push(0);
   //showVals.innerText=showVals.innerText+', opened:'+divID;
  }
 }
 else {
  TopActivate(divID);                           // Activate the sublist 
 }
 //showVals.innerText=showVals.innerText+'; MenuOR|\r\n ';
};

function TopMenuOut(divID) {
 //showVals.innerText=showVals.innerText+' TopMenuOut('+divID+'), currOpen='+currOpen[currOpen.length-1]+' ';
 if (currOpen[currOpen.length-1]==divID) {      // Sub menu list is active open
  TopDeactivate(divID);                         // Deactivate it, so it will close if the mouse is not over it
 }
 TopMenuTimeout=setTimeout('TopClose()',100);
 //showVals.innerText=showVals.innerText+'; MenuOT|\r\n ';
}

function TopListOver(divID) {
 TopActivate(divID);
 //showVals.innerText=showVals.innerText+'; divOR|\r\n ';
}

function TopListOut(divID) {
 TopDeactivate(divID);
 TopMenuTimeout=setTimeout('TopClose()',100);
 //showVals.innerText=showVals.innerText+'; divOT|\r\n ';
}

function TopItemOver(divID) {
 TopActivate(divID);
 //showVals.innerText=showVals.innerText+'; ItemOR|\r\n ';
}

// Elementary functions called from the above functions
function TopClose() {
 if (currOpen.length!=0){                       // an array element must exist
  aMenu = currOpen[currOpen.length-1]
  //showVals.innerText=showVals.innerText+' TopClose('+aMenu+') ';
  for(i=currOpen.length-1; i>=0; i--) {         // process from last to first entry
   if (!currActive[i]){                         // if menulist is not active, close it
    aMenu = currOpen.pop();                     // Clear entry from the list
    aState = currActive.pop();
    document.getElementById(aMenu).className='TopListOff'; // Close open Menu list
    //showVals.innerText=showVals.innerText+', Closed:'+aMenu;
   }
   else {
    break;                                      // Process is done, found active level
   }
  }
 }
 //showVals.innerText=showVals.innerText+'; '
 if (TopMenuTimeout) clearTimeout(TopMenuTimeout); // clear timeout if active
 TopMenuTimeout = null;
}

function TopActivate(divID) {
 //showVals.innerText=showVals.innerText+'TopActivate('+divID+') ';
 for(i=currOpen.length-1; i>=0; i--) {          // process from last to first entry
  if (currOpen[i]==divID) {                     // if menulist is not active, close it
   if (currActive[i]==0){
    currActive[i]=1;                            // mark this entry as active, prevents Menu onmouseout from closing it
    //showVals.innerText=showVals.innerText+' activated:'+divID+', ';
   }
   else {
    //showVals.innerText=showVals.innerText+' already active:'+divID+', ';
   }
   break;                                       // Process is done, found selected level
  }
 }
}

function TopDeactivate(divID) {
 for(i=currOpen.length-1; i>=0; i--) {          // process from last to first entry
  if (currOpen[i]==divID) {                     // if menulist is not active, close it
   currActive[i]=0;                             // mark this entry as active, prevents Menu onmouseout from closing it
   //showVals.innerText=showVals.innerText+' Deactivated:'+divID+', ';
   break;                                       // Process is done, found selected level
  }
 }
}

function TopItemClick() {                       // process menu close when an Item is clicked: deactivates the active level and closes all
 //showVals.innerText=showVals.innerText+' TopItemClick('+currOpen[currOpen.length-1]+') Deactivated; ';
 currActive[currOpen.length-1]=0;               // Clear active level
 TopClose();
}

function PxToNum(pxStr) { // pxStr == 27px, we want 27.
 if (pxStr.length > 2) {
  n = Number(pxStr.substr(0, pxStr.length-2));
  return(n);
 }
 return(0);
}

// -->
