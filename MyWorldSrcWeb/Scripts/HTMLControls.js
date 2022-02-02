// HTMLTools created by Bob Curtice for Amazing Facts. 
// Cross - browser support edition with language translation support.
// <!--
var HTMLToolsTxt = {
 "HTMLTxt1":"Enter a URL address",
 "HTMLTxt2":"Enter Named point",
 "HTMLTxt3":"Enter the email address",
 "HTMLTxt4":"Enter image path/filename"};

var otextRange;
var eParent;
lLevel = 0;                     // presume no access
// Parameters are StartTag,EndTag
function PutHTML(stag,etag) {
 lapply = false;               // presume no access
 stext="";
 
 // process options requiring selected text
 if (document.selection && document.selection.type == "Text") {
  // Create the TextRange object of selected text
  otextRange = document.selection.createRange();
  if (otextRange) {   // There is an object to work with!
   // get the selected text
   stext = otextRange.text;
  }
  // Get the parent object containing the selected text
  eParent = otextRange.parentElement();
 }
 else {
  tStart = eParent.selectionStart;
  tEnd = eParent.selectionEnd;
  stext = eParent.value.substring(tStart, tEnd);
 }

 if (stext.length>0) {
  // allow basic html controls to work
  if (lLevel==1 || lLevel==4 || lLevel==9) {
   if (stag=="<p>" || stag=="<b>" || stag=="<i>" || stag=="<u>") {
    lapply = true;
   }
   else if (etag=="<br>" || stag=="<sub>" || stag=="<sup>" || stag=="<li>" || stag=="<ul>") {
    lapply = true;
   }
   else if (stag=="<fmt>") {
    stag = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
    lapply = true;
   }
  }
  // translate object options if allowed
  if (lLevel==2 || lLevel==4 || lLevel==9) {
   if (stag == "<a href>") {
    var ptext = prompt(HTMLToolsTxt.HTMLTxt1+": ","http://"); // "Enter a URL address"
    if (ptext!=null && ptext!=''){
     stag = '<a href="'+ptext+'">';
     lapply = true;
    }
   }
   else if (stag == "<a name>") {
    var ptext = prompt(HTMLToolsTxt.HTMLTxt2+": ",""); // "Enter Named point"
    if (ptext!=null && ptext!='') {
     stag = '<a name="'+ptext+'">';
     lapply = true;
    }
   }
   else if (stag == "<a mail>") {
    var ptext = prompt(HTMLToolsTxt.HTMLTxt3+": ","");  // "Enter the email address"
    if (ptext!=null && ptext!='') {
     stag = '<a href="mailto:'+ptext+'">';
     lapply = true;
    }
   }
   else if (stag == "<img>") {
    var ptext = prompt(HTMLToolsTxt.HTMLTxt4+": ","");   // "Enter image path/filename"
    if (ptext!=null && ptext!='') {
     stag = '<img src="'+ptext+'" border="0">';
     lapply = true;
    }
   }
   else if ( stag=="<span>") {
    lapply = true;
   }
  }
  // process structures if allowed
  if (lLevel==3 || lLevel==9) {
   if (stag == "<center>") {
    stag = "\n"+stag+"\n";
    etag = "\n"+etag+"\n";
    lapply = true;
   }
   else if (stag == "<table>") {
    stag = "\n<table width=\x22100%\x22 cellpadding=\x220\x22 cellspacing=\x220\x22>\n <tr>\n  <td>\n";
    etag = "\n  </td>\n </tr>\r\n"+etag+"\n";
    lapply = true;
   }
   else if (stag.substr(0,6)=="<span ") {
    lapply = true;
   }
   else if (stag== "<ExtOpt>") {
    stag = "";
    lapply = true;
   }
   else if (stag== "class=") {
    stag="";
    etag = "class=\x22"+etag+"\x22";
    lapply = true;
   }
   else {                            // External definition process handling
    lapply=true;
   }
  }
 }
 if (lapply) {                      // Allow processing for which apply is still true
  // apply the HTML tag around the text
  if (document.selection && document.selection.type == "Text") {        // IE Only
   otextRange.text = stag+stext+etag;
  }
  else {                            //  Anyone but IE
   tBeforeSel = eParent.value.substring(0, tStart);
   tAfterSel = eParent.value.substring(tEnd, eParent.value.length);
   tNewSel = stag+stext+etag;
   eParent.value=tBeforeSel+tNewSel+tAfterSel;
   eParent.setSelectionRange(tStart, tStart+tNewSel.length);
   //eParent.setSelectionRange(tStart, tStart);   // Clear selection and leave cursor at start
  }
 }
 // reset focus to the parent object
 eParent.focus();
 //OptionsOff();                      // Close Options until more text is selected
}

// Enable options as specified for the field
function OptionsOn(level) {
 // Level 1 = (properties) only Simple html controls turn on P,B,I,U,BR,<?>
 // Level 2 = (objects) allow images, links, anchors, and emails
 // Level 3 = (structures) Allow Center, Tables
 // Level 4 = Combine levels 1 & 2
 // Level 9 = All enabled
 if (document.activeElement && document.activeElement.selectionStart >= 0) {
  eParent = document.activeElement;
 }
 if (level==1 || level==4 || level==9) {
  document.getElementById('HTMLParagraph').src="/images/HTMLControls/Paragraph.gif";
  document.getElementById('HTMLParagraph').className='HTMLButtonEn';
  document.getElementById('HTMLParagraph').onmouseover=function () {document.getElementById('HTMLParagraph').className='HTMLButtonOver';};
  document.getElementById('HTMLParagraph').onmouseout=function () {document.getElementById('HTMLParagraph').className='HTMLButtonEn';};
  document.getElementById('HTMLBold').src="/images/HTMLControls/Bold.gif";
  document.getElementById('HTMLBold').className='HTMLButtonEn';
  document.getElementById('HTMLBold').onmouseover=function () {document.getElementById('HTMLBold').className='HTMLButtonOver';};
  document.getElementById('HTMLBold').onmouseout=function () {document.getElementById('HTMLBold').className='HTMLButtonEn';};
  document.getElementById('HTMLItalic').src="/images/HTMLControls/Italic.gif";
  document.getElementById('HTMLItalic').className='HTMLButtonEn';
  document.getElementById('HTMLItalic').onmouseover=function () {document.getElementById('HTMLItalic').className='HTMLButtonOver';};
  document.getElementById('HTMLItalic').onmouseout=function () {document.getElementById('HTMLItalic').className='HTMLButtonEn';};
  document.getElementById('HTMLUnderline').src="/images/HTMLControls/Underline.gif";
  document.getElementById('HTMLUnderline').className='HTMLButtonEn';
  document.getElementById('HTMLUnderline').onmouseover=function () {document.getElementById('HTMLUnderline').className='HTMLButtonOver';};
  document.getElementById('HTMLUnderline').onmouseout=function () {document.getElementById('HTMLUnderline').className='HTMLButtonEn';};
  document.getElementById('HTMLIndent').src="/images/HTMLControls/Indent.gif";
  document.getElementById('HTMLIndent').className='HTMLButtonEn';
  document.getElementById('HTMLIndent').onmouseover=function () {document.getElementById('HTMLIndent').className='HTMLButtonOver';};
  document.getElementById('HTMLIndent').onmouseout=function () {document.getElementById('HTMLIndent').className='HTMLButtonEn';};
  document.getElementById('HTMLBreak').src="/images/HTMLControls/break.gif";
  document.getElementById('HTMLBreak').className='HTMLButtonEn';
  document.getElementById('HTMLBreak').onmouseover=function () {document.getElementById('HTMLBreak').className='HTMLButtonOver';};
  document.getElementById('HTMLBreak').onmouseout=function () {document.getElementById('HTMLBreak').className='HTMLButtonEn';};
  document.getElementById('HTMLSuper').src="/images/HTMLControls/Super.gif";
  document.getElementById('HTMLSuper').className='HTMLButtonEn';
  document.getElementById('HTMLSuper').onmouseover=function () {document.getElementById('HTMLSuper').className='HTMLButtonOver';};
  document.getElementById('HTMLSuper').onmouseout=function () {document.getElementById('HTMLSuper').className='HTMLButtonEn';};
  document.getElementById('HTMLSub').src="/images/HTMLControls/Sub.gif";
  document.getElementById('HTMLSub').className='HTMLButtonEn';
  document.getElementById('HTMLSub').onmouseover=function () {document.getElementById('HTMLSub').className='HTMLButtonOver';};
  document.getElementById('HTMLSub').onmouseout=function () {document.getElementById('HTMLSub').className='HTMLButtonEn';};
  document.getElementById('HTMLBullet').src="/images/HTMLControls/Bullet.gif";
  document.getElementById('HTMLBullet').className='HTMLButtonEn';
  document.getElementById('HTMLBullet').onmouseover=function () {document.getElementById('HTMLBullet').className='HTMLButtonOver';};
  document.getElementById('HTMLBullet').onmouseout=function () {document.getElementById('HTMLBullet').className='HTMLButtonEn';};
  document.getElementById('HTMLIndentBullets').src="/images/HTMLControls/IndentBullets.gif";
  document.getElementById('HTMLIndentBullets').className='HTMLButtonEn';
  document.getElementById('HTMLIndentBullets').onmouseover=function () {document.getElementById('HTMLIndentBullets').className='HTMLButtonOver';};
  document.getElementById('HTMLIndentBullets').onmouseout=function () {document.getElementById('HTMLIndentBullets').className='HTMLButtonEn';};
 }
 if (level==2 || level==4 || level==9) {
  if (document.getElementById('HTMLImage')!=null) {
   document.getElementById('HTMLImage').src="/images/HTMLControls/Image.gif";
   document.getElementById('HTMLImage').className='HTMLButtonEn';
   document.getElementById('HTMLImage').onmouseover=function () {document.getElementById('HTMLImage').className='HTMLButtonOver';};
   document.getElementById('HTMLImage').onmouseout=function () {document.getElementById('HTMLImage').className='HTMLButtonEn';};
  }
  if (document.getElementById('HTMLLink')!=null) {
   document.getElementById('HTMLLink').src="/images/HTMLControls/HyperLink.gif";
   document.getElementById('HTMLLink').className='HTMLButtonEn';
   document.getElementById('HTMLLink').onmouseover=function () {document.getElementById('HTMLLink').className='HTMLButtonOver';};
   document.getElementById('HTMLLink').onmouseout=function () {document.getElementById('HTMLLink').className='HTMLButtonEn';};
  }
  if (document.getElementById('HTMLAnchor')!=null) {
   document.getElementById('HTMLAnchor').src="/images/HTMLControls/Anchor.gif";
   document.getElementById('HTMLAnchor').className='HTMLButtonEn';
   document.getElementById('HTMLAnchor').onmouseover=function () {document.getElementById('HTMLAnchor').className='HTMLButtonOver';};
   document.getElementById('HTMLAnchor').onmouseout=function () {document.getElementById('HTMLAnchor').className='HTMLButtonEn';};
  }
  if (document.getElementById('HTMLEmail')!=null) {
   document.getElementById('HTMLEmail').src="/images/HTMLControls/EMail.gif";
   document.getElementById('HTMLEmail').className='HTMLButtonEn';
   document.getElementById('HTMLEmail').onmouseover=function () {document.getElementById('HTMLEmail').className='HTMLButtonOver';};
   document.getElementById('HTMLEmail').onmouseout=function () {document.getElementById('HTMLEmail').className='HTMLButtonEn';};
  }
  if (document.getElementById('HTMLSpan')!=null) {
   document.getElementById('HTMLSpan').src="/images/HTMLControls/Span.gif";
   document.getElementById('HTMLSpan').className='HTMLButtonEn';
   document.getElementById('HTMLSpan').onmouseover=function () {document.getElementById('HTMLSpan').className='HTMLButtonOver';};
   document.getElementById('HTMLSpan').onmouseout=function () {document.getElementById('HTMLSpan').className='HTMLButtonEn';};
  }
 }
 if (level==3 || level==9) {
  if (document.getElementById('HTMLCenter')!=null) {
   document.getElementById('HTMLCenter').src="/images/HTMLControls/Center.gif";
   document.getElementById('HTMLCenter').className='HTMLButtonEn';
   document.getElementById('HTMLCenter').onmouseover=function () {document.getElementById('HTMLCenter').className='HTMLButtonOver';};
   document.getElementById('HTMLCenter').onmouseout=function () {document.getElementById('HTMLCenter').className='HTMLButtonEn';};
  }
  if (document.getElementById('HTMLTable')!=null) {
   document.getElementById('HTMLTable').src="/images/HTMLControls/Table.gif";
   document.getElementById('HTMLTable').className='HTMLButtonEn';
   document.getElementById('HTMLTable').onmouseover=function () {document.getElementById('HTMLTable').className='HTMLButtonOver';};
   document.getElementById('HTMLTable').onmouseout=function () {document.getElementById('HTMLTable').className='HTMLButtonEn';};
  }
  if (document.getElementById('HTMLSetFont')!=null) {
   document.getElementById('HTMLSetFont').src="/images/HTMLControls/Font.gif";
   document.getElementById('HTMLSetFont').className='HTMLButtonEn';
   document.getElementById('HTMLSetFont').onmouseover=function () {document.getElementById('HTMLSetFont').className='HTMLButtonOver';};
   document.getElementById('HTMLSetFont').onmouseout=function () {document.getElementById('HTMLSetFont').className='HTMLButtonEn';};
  }
  if (document.getElementById('HTMLExtOpt')!=null) {
   document.getElementById('HTMLExtOpt').src="/images/HTMLControls/Insert.gif";
   document.getElementById('HTMLExtOpt').className='HTMLButtonEn';
   document.getElementById('HTMLExtOpt').onmouseover=function () {document.getElementById('HTMLExtOpt').className='HTMLButtonOver';};
   document.getElementById('HTMLExtOpt').onmouseout=function () {document.getElementById('HTMLExtOpt').className='HTMLButtonEn';};
  }
  if (document.getElementById('HTMLClassOpt')!=null) {
   document.getElementById('HTMLClassOpt').src="/images/HTMLControls/Class.gif";
   document.getElementById('HTMLClassOpt').className='HTMLButtonEn';
   document.getElementById('HTMLClassOpt').onmouseover=function () {document.getElementById('HTMLClassOpt').className='HTMLButtonOver';};
   document.getElementById('HTMLClassOpt').onmouseout=function () {document.getElementById('HTMLClassOpt').className='HTMLButtonEn';};
  }
  if (document.getElementById('HTMLCustom')!=null) {
   document.getElementById('HTMLCustom').src="/images/HTMLControls/Insert.gif";
   document.getElementById('HTMLCustom').className='HTMLButtonEn';
   document.getElementById('HTMLCustom').onmouseover=function () {document.getElementById('HTMLCustom').className='HTMLButtonOver';};
   document.getElementById('HTMLCustom').onmouseout=function () {document.getElementById('HTMLCustom').className='HTMLButtonEn';};
  }
 }
 lLevel = level;                       // set general level enabled
}

// Disable options 
function OptionsOff() {
 otextRange = null;
 document.getElementById('HTMLParagraph').src="/images/HTMLControls/ParagraphDis.gif";
 document.getElementById('HTMLParagraph').className="HTMLButtonDis"
 document.getElementById('HTMLParagraph').onmouseover=null;
 document.getElementById('HTMLParagraph').onmouseout=null;
 document.getElementById('HTMLBold').src="/images/HTMLControls/BoldDis.gif";
 document.getElementById('HTMLBold').className="HTMLButtonDis"
 document.getElementById('HTMLBold').onmouseover=null;
 document.getElementById('HTMLBold').onmouseout=null;
 document.getElementById('HTMLItalic').src="/images/HTMLControls/ItalicDis.gif";
 document.getElementById('HTMLItalic').className="HTMLButtonDis"
 document.getElementById('HTMLItalic').onmouseover=null;
 document.getElementById('HTMLItalic').onmouseout=null;
 document.getElementById('HTMLUnderline').src="/images/HTMLControls/UnderlineDis.gif";
 document.getElementById('HTMLUnderline').className="HTMLButtonDis"
 document.getElementById('HTMLUnderline').onmouseover=null;
 document.getElementById('HTMLUnderline').onmouseout=null;
 document.getElementById('HTMLIndent').src="/images/HTMLControls/IndentDis.gif";
 document.getElementById('HTMLIndent').className="HTMLButtonDis";
 document.getElementById('HTMLIndent').onmouseover=null;
 document.getElementById('HTMLIndent').onmouseout=null;
 document.getElementById('HTMLBreak').src="/images/HTMLControls/breakDis.gif";
 document.getElementById('HTMLBreak').className="HTMLButtonDis";
 document.getElementById('HTMLBreak').onmouseover=null;
 document.getElementById('HTMLBreak').onmouseout=null;
 document.getElementById('HTMLSuper').src="/images/HTMLControls/SuperDis.gif";
 document.getElementById('HTMLSuper').className="HTMLButtonDis";
 document.getElementById('HTMLSuper').onmouseover=null;
 document.getElementById('HTMLSuper').onmouseout=null;
 document.getElementById('HTMLSub').src="/images/HTMLControls/SubDis.gif";
 document.getElementById('HTMLSub').className="HTMLButtonDis";
 document.getElementById('HTMLSub').onmouseover=null;
 document.getElementById('HTMLSub').onmouseout=null;
 document.getElementById('HTMLBullet').src="/images/HTMLControls/BulletDis.gif";
 document.getElementById('HTMLBullet').className="HTMLButtonDis";
 document.getElementById('HTMLBullet').onmouseover=null;
 document.getElementById('HTMLBullet').onmouseout=null;
 document.getElementById('HTMLIndentBullets').src="/images/HTMLControls/IndentBulletsDis.gif";
 document.getElementById('HTMLIndentBullets').className="HTMLButtonDis";
 document.getElementById('HTMLIndentBullets').onmouseover=null;
 document.getElementById('HTMLIndentBullets').onmouseout=null;
 if (document.getElementById('HTMLImage')!=null) {
  document.getElementById('HTMLImage').src="/images/HTMLControls/ImageDis.gif";
  document.getElementById('HTMLImage').className="HTMLButtonDis";
  document.getElementById('HTMLImage').onmouseover=null;
  document.getElementById('HTMLImage').onmouseout=null;
 }
 if (document.getElementById('HTMLLink')!=null) {
  document.getElementById('HTMLLink').src="/images/HTMLControls/HyperLinkDis.gif";
  document.getElementById('HTMLLink').className="HTMLButtonDis";
  document.getElementById('HTMLLink').onmouseover=null;
  document.getElementById('HTMLLink').onmouseout=null;
 }
 if (document.getElementById('HTMLAnchor')!=null) {
  document.getElementById('HTMLAnchor').src="/images/HTMLControls/AnchorDis.gif";
  document.getElementById('HTMLAnchor').className="HTMLButtonDis";
  document.getElementById('HTMLAnchor').onmouseover=null;
  document.getElementById('HTMLAnchor').onmouseout=null;
 }
 if (document.getElementById('HTMLEmail')!=null) {
  document.getElementById('HTMLEmail').src="/images/HTMLControls/EMailDis.gif";
  document.getElementById('HTMLEmail').className="HTMLButtonDis";
  document.getElementById('HTMLEmail').onmouseover=null;
  document.getElementById('HTMLEmail').onmouseout=null;
 }
 if (document.getElementById('HTMLSpan')!=null) {
  document.getElementById('HTMLSpan').src="/images/HTMLControls/SpanDis.gif";
  document.getElementById('HTMLSpan').className="HTMLButtonDis";
  document.getElementById('HTMLSpan').onmouseover=null;
  document.getElementById('HTMLSpan').onmouseout=null;
 }
 if (document.getElementById('HTMLCenter')!=null) {
  document.getElementById('HTMLCenter').src="/images/HTMLControls/CenterDis.gif";
  document.getElementById('HTMLCenter').className="HTMLButtonDis";
  document.getElementById('HTMLCenter').onmouseover=null;
  document.getElementById('HTMLCenter').onmouseout=null;
 }
 if (document.getElementById('HTMLTable')!=null) {
  document.getElementById('HTMLTable').src="/images/HTMLControls/TableDis.gif";
  document.getElementById('HTMLTable').className="HTMLButtonDis";
  document.getElementById('HTMLTable').onmouseover=null;
  document.getElementById('HTMLTable').onmouseout=null;
 }
 if (document.getElementById('HTMLSetFont')!=null) {
  document.getElementById('HTMLSetFont').src="/images/HTMLControls/FontDis.gif";
  document.getElementById('HTMLSetFont').className="HTMLButtonDis";
  document.getElementById('HTMLSetFont').onmouseover=null;
  document.getElementById('HTMLSetFont').onmouseout=null;
 }
 if (document.getElementById('HTMLExtOpt')!=null) {
  document.getElementById('HTMLExtOpt').src="/images/HTMLControls/InsertDis.gif";
  document.getElementById('HTMLExtOpt').className="HTMLButtonDis";
  document.getElementById('HTMLExtOpt').onmouseover=null;
  document.getElementById('HTMLExtOpt').onmouseout=null;
 }
 if (document.getElementById('HTMLClassOpt')!=null) {
  document.getElementById('HTMLClassOpt').src="/images/HTMLControls/ClassDis.gif";
  document.getElementById('HTMLClassOpt').className="HTMLButtonDis";
  document.getElementById('HTMLClassOpt').onmouseover=null;
  document.getElementById('HTMLClassOpt').onmouseout=null;
 }
 if (document.getElementById('HTMLCustom')!=null) {
  document.getElementById('HTMLCustom').src="/images/HTMLControls/InsertDis.gif";
  document.getElementById('HTMLCustom').className="HTMLButtonDis";
  document.getElementById('HTMLCustom').onmouseover=null;
  document.getElementById('HTMLCustom').onmouseout=null;
 }
 lLevel = 0;                       // set general level disabled
}
//-->
