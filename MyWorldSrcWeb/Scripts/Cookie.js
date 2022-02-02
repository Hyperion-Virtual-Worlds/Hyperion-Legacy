<!--

/*-- SetCookie Parameters:                                                       */
/*--  Required: Name - Name of cookie value                                      */
/*--            Value - The value to set to the name                             */
/*--  Optional: Expires - Date value for cookie to expire by                     */
/*--            Path - /pathname/ which is placed as part of the cookie filename */
/*--            domain - name of domain setting the cookie                       */
/*--            secure - sets security on or off, default=off                    */
function SetCookie(name, value) { // sets a named cookie value for this page
 var argv = SetCookie.arguments; // get all parms passed to this function
 var argc = SetCookie.arguments.length; // How many were given
 var expires = (2 < argc) ? argv[2] : null; // Define optional parms
 var path = (3 < argc) ? argv[3] : null;
 var domain = (4 < argc) ? argv[4] : null;
 var secure = (5 < argc) ? argv[5] : false;
 var CodedVal = escape (value); // encode passed value
 document.cookie = name+"="+value+
 ((expires == null) ? "" : ("; expires=" + expires.toGMTString())) +
 ((path == null) ? "" : ("; path=" + path)) +
 ((domain == null) ? "" : ("; domain=" + domain)) +
 ((secure == true) ? "; secure" : "");
}

// GetCookie function will return a named value set with Setcookie() function.
function GetCookie(name) { // retrieves prior set named cookie value
 var allcookies = document.cookie;
 var result = "";
 if (allcookies != "") { // a value was set
  var start = allcookies.indexOf(name+'=');
  if (start != -1) {
   start += name.length +1; // skip name and equals sign
   var end = allcookies.indexOf(';',start);
   if (end == -1) end = allcookies.length;
//   result = unescape(allcookies.substring(start,end)); // get value assigned unencoded
   result = allcookies.substring(start,end); // get value assigned unencoded
  }
 }
 return result;
}

//-->