<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Places.aspx.vb" Inherits="Places" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="~/Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SysMenu" Src="~/SysMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="~/Footer.ascx" %>

<!DOCTYPE html>
<html xmlns="https://www.w3.org/1999/xhtml" >
 <head>
  <title>Places - My World</title>
  <link href="/styles/Site.css" type="text/css" rel="stylesheet" />
  <link href="/Styles/TopMenu.css" type="text/css" rel="stylesheet" />
  <link href="/styles/TreeView.css" type="text/css" rel="stylesheet" />
  <link href="/styles/leaflet.css" type="text/css" rel="stylesheet" />
  <link href="/styles/map.css" type="text/css" rel="stylesheet" />
  <script type="text/javascript" src="/scripts/Cookie.js"></script>
  <script type="text/javascript" src="/scripts/TopMenu.js"></script>
  <script type="text/javascript" src="/scripts/TreeView.js"></script>
  <script type="text/javascript" src="/scripts/jquery-3.1.1.min.js"></script>
  <script type="text/javascript" src="/scripts/leaflet/leaflet.js"></script>
  <script type="text/javascript" src="/scripts/mustache.min.js"></script>
  <style type="text/css">
   /* Overrides for Site.css that apply only to the map page. */
   body { position: absolute; top: 0px; left: 0px; padding: 0; height: 100%; width: 100%; /*overflow: hidden;*/ }
   #HeaderPos { position: absolute; top: 0; margin: 0; padding: 0; }
   #LSideBar { bottom: 62px;}
   #BodyArea, 
   #BodyAreaFull { position: absolute; top: 81px; left: 0; bottom: 62px; right: 0; box-sizing: border-box; }
   #FooterPos { position: absolute; bottom: 0; }
   #map-container { height: 100%; box-sizing: border-box; color: #3A3C3F; background: black; /* TODO: change to color of ocean tiles */
                    padding-top: 127px; /*transition: padding-left 0.2s;*/ z-index: 10; }
   #__asptrace { position: absolute; top: 906px;  }

   .RegionLabel { font-family: sans-serif; font-weight: bold; color: white; text-shadow: 1px 1px black; font-size: 14px; }
  </style>
  <script type="text/javascript">
   /*
   L.CRS.scale = function(t) {
    return 512 * Math.Pow(2, t);
   }
   L.CRS.zoom = function(t) {
    return Math.log(t / 512) / Math.LN2
   }
   */

   $.namespace = function () {
    var o = null;
    var i, j, d;
    for (i = 0; i < arguments.length; i++) {
     d = arguments[i].split(".");
     o = window;
     for (j = 0; j < d.length; j++) {
      o[d[j]] = o[d[j]] || {};
      o = o[d[j]];
     }
    }
    return o;
   };

   $.namespace('$.my.maps.config');
   $.namespace('$.my.maps');
   $.my.maps.config = {
    my_base_url: "http://<%=Request.ServerVariables("HTTP_HOST")%>/",
    tile_url: "http://<%=Request.ServerVariables("HTTP_HOST")%>",
    default_title: "Welcome to My World",
    default_img: "/images/site/default-new.jpg",
    default_msg: "My World is a popular virtual world for education, entertainment and social interaction. <strong>If you have a virtual world viewer installed on your computer<strong>, sign up, teleport in and start exploring!",
    map_debug: false,
    map_grid_edge_size: 1048576,
    balloon_tmpl: ' \
        <div class="balloon-content"> \
            <h3> \
                {{#slurl}} \
                <a href="{{{slurl}}}"> \
                {{/slurl}} \
                    {{title}} \
                {{#slurl}} \
                </a> \
                {{/slurl}} \
            </h3> \
            {{#img}} \
            <a href="{{{slurl}}}"> \
                <img src="{{{img}}}" onError="this.onerror=null;this.src=\'/images/site/default-new.jpg\';" /> \
            </a> \
            {{/img}} \
            <p>{{{msg}}}</p> \
            <div class="buttons"> \
                {{#slurl}} \
                    <a class="HIGHLANDER_button_hot btn_large primary" title="visit this location" href="{{{slurl}}}">Visit this location</a> \
                {{/slurl}} \
                <a href="http://<%=Request.ServerVariables("HTTP_HOST")%>/" target="_top" class="HIGHLANDER_button_hot btn_large secondary join_button">Join Now, it&rsquo;s free!</a> \
            </div> \
        </div>',
    noexists_tmpl: ' \
        <div id="map-error"> \
            <div id="error-content"> \
                <span class="error-close">Hide message</span> \
                <span class="location-title">We are unable to locate the region "{{{region_name}}}"</span> \
                <p>This region may no longer exist, but please double check your spelling and coordinates to make sure there aren&rsquo;t any errors and try again.</p> \
                <p>If your problem persists, contact <a href="http://<%=Request.ServerVariables("HTTP_HOST")%>/ContactUs.aspx">MyWorld Support</a></p> \
            </div> \
        </div>'
   }

   var myDebugMap = $.my.maps.config.map_debug;
   var coordScalar = 1.0;/*HACK: when region tiles are any size other than 256 the map's scale gets wonky. This allows us to fix it. I like 512px tiles.*/
   var regionDataMemoryCache = {};

   var MIN_ZOOM_LEVEL = 1;
   var MAX_ZOOM_LEVEL = 8;
   // Special class structure to provide a click process
   function myMap(mapElement, mapOptions) {
    var mapDiv = document.createElement("div");
    mapDiv.style.height = "100%";
    mapElement.appendChild(mapDiv);

    var tileOptions = {
     crs: L.CRS.Simple,
     minZoom: MIN_ZOOM_LEVEL,
     maxZoom: MAX_ZOOM_LEVEL,
     zoomOffset: 1,
     zoomReverse: true,
     bounds: [[0, 0], [$.my.maps.config.map_grid_edge_size, $.my.maps.config.map_grid_edge_size]],
     attribution: "<a href='" + $.my.maps.config.sl_base_url + "'>MyWorld</a>",
     tileOptions: 256,
    };

    if (mapOptions.hasOwnProperty('tileSize') && is_number(mapOptions.tileSize)) {
     tileOptions.tileSize = mapOptions.tileSize;
    }

    var MyTileLayer = L.TileLayer.extend({
     getTileUrl: function (coords) {
      var data = {
       r: L.Browser.retina ? '@2x' : '',
       s: this._getSubdomain(coords),
       z: this._getZoomForUrl()
      };
      var regionsPerTileEdge = Math.pow(2, data['z'] - 1);
      data['region_x'] = coords.x * regionsPerTileEdge; // BUG: coords gets the wrong values when the tileSize is set to anything other than 256.  At 512 the values are half what they should be, but just scaling them up is incorrect as it causes tile skipping.
      data['region_y'] = (Math.abs(coords.y) - 1) * regionsPerTileEdge;
      return L.Util.template(this._url, L.extend(data, this.options));
     },
    });

    var tiles = new MyTileLayer("/map.aspx/map-{z}-{region_x}-{region_y}-objects.jpg", tileOptions);
    var map = L.map(mapElement, {
     crs: L.CRS.Simple,
     minZoom: MIN_ZOOM_LEVEL,
     maxZoom: MAX_ZOOM_LEVEL,
     maxBounds: [[0, 0], [$.my.maps.config.map_grid_edge_size, $.my.maps.config.map_grid_edge_size]],
     layers: [tiles]
    });
    map.on('click', function mapClickHandler(event) {
     event.latlng.lat *= coordScalar/*HACK*/;
     event.latlng.lng *= coordScalar/*HACK*/;
     gotoSLURL(event.latlng, map);
    });
    setupMapLabels(tiles, map);
    return map;
   }

   // Call mapFunctions page giving the region info clicked on, return in JSON. (multi-use processor)
   function gotoSLURL(latlng, lmap) {
    var int_x = Math.floor(latlng.lng);
    var int_y = Math.floor(latlng.lat);
    var local_x = Math.round((latlng.lng - int_x) * 256);
    var local_y = Math.round((latlng.lat - int_y) * 256);

    var regionData = regionDataMemoryCache[[int_x, int_y]];

    var debugInfo = '';
    if (myDebugMap) {
     debugInfo = ' x: ' + int_x + ' y: ' + int_y;
    }

    if (typeof(regionData) != "undefined" && regionData.hasOwnProperty("regionName")) {
     // query for the region welcome title, image, and message.
     // jQuery.get('/mapFunctions.aspx', { coordX: int_x, coordY: int_y, action: 'parcelWelcomeMessage'} ... All I need to have.
     // Return will be one of:
     // { "status": "not found" }
     // { "title":"A name","message":"A description","imageUrl":"/MapImage.aspx/00000000-0000-0000-0000-000000000000" }
     // { "title":"A name","message":"A description","imageUrl":"/Images/Site/default-new.jpg" }
     $.ajax({
      url: '/mapFunctions.aspx',
      data: {
       coordX: int_x,
       coordY: int_y,
       action: 'parcelWelcomeMessage',
       // Parcels do not have region location coordinates anywhere in the DB. Only identified by name and LocalLandID (a sequential integer).
       // A SLURL may point to a parcel by position, but it wont return parcel info; only the region.
       //positionX: local_x,
       //positionY: local_y
      },
      dataType: 'json',
      success: function WelcomeMessageHandler(welcomeMessage, status) {
       var slurl_data = generate_slurl_data(
        regionData.regionName,
        local_x, local_y, 0,
        welcomeMessage.title,
        welcomeMessage.imageUrl,
        welcomeMessage.message,
        false
       );

       showMarkerForClick(lmap, latlng, slurl_data['windowcontent']);
      },
      error: function WelcomeMessageErrorHandler(jqXHR, status, errorThrown) {
       var slurl_data = generate_slurl_data(
        regionData.regionName,
        local_x, local_y, 0,
        "Visit " + regionData.regionName + " now!",
        null,
        regionData.regionName + " welcomes you!",
        false
       );

       showMarkerForClick(lmap, latlng, slurl_data['windowcontent']);
      },
     });
    }
   }

   function AddDynamicDisplay(local_y, local_x, DisplayMapClick) {
    if (DisplayMapClick) {
     script.onreadystatechange = function () {
      if (script.readyState == 'complete' || script.readyState == 'loaded') {
       DisplayMapClick();
      }
     }
     script.onload = onLoadHandler;
    }
   }

   var OptTitle = "<%=RTitle.ToString()%>";
   var OptImg = "<%=RImg.ToString()%>";
   var OptMsg = "<%=RMsg.ToString()%>";

   function generate_slurl_data(regionName, targetX, targetY, targetZ, title, image, message, isDefaultRegion) {
    var slurlData = {};
    slurlData['region'] = {};
    slurlData['region']['name'] = regionName;
    slurlData['region']['target_x'] = targetX;
    slurlData['region']['target_y'] = targetY;
    slurlData['region']['target_z'] = targetZ;
    slurlData['region']['isDefault'] = isDefaultRegion;

    var slurl = 'hop://grid.MyWorld.com:8002/' + encodeURIComponent(slurlData['region']['name'].toUpperCase()) + '/' + slurlData['region']['target_x'] + '/' + slurlData['region']['target_y'] + '/' + slurlData['region']['target_z'];

    var templateData = {                           // Send content to display
     'slurl': slurl,
     'title': title,
     'img': image,
     'msg': message
    };

    // http://maps-lecs.lindenlab.com/agni/32/_img/default-new.jpg image size info
    Mustache.parse($.my.maps.config.balloon_tmpl);
    slurlData['windowcontent'] = Mustache.render($.my.maps.config.balloon_tmpl, templateData).replace('/\s+/', ' ');
    slurlData['sidebarcontent'] = templateData;
    Mustache.parse($.my.maps.config.noexists_tmpl);
    slurlData['noexist_windowcontent'] = Mustache.render($.my.maps.config.noexists_tmpl, {
     'region_name': slurlData['region']['name']
    });

    return slurlData;
   }

   function loadmap(firstJoinUrl, zoomLevel) {
    if (typeof(zoomLevel) === "undefined") {
     zoomLevel = 6;
    }
    var latlng = L.latLng(/*Y=*/1000.5*coordScalar/*HACK*/, /*X=*/1000.5*coordScalar/*HACK*/);
    var $body = $('body');

    var urlPath = window.location.pathname;
    var urlParts = urlPath.split("/");
    var regionName = 'Reg01';

    var SetTitle = $.my.maps.config.default_title; // Set to default contents
    var SetImg = $.my.maps.config.default_img;
    var SetMsg = $.my.maps.config.default_msg;
    if (OptTitle!="") {                            // Check for optional supplied content
     SetTitle = OptTitle;
    }
    if (OptImg!="") {
     SetImg = OptImg;
    }
    if (OptMsg!="") {
     SetMsg = OptMsg;
    }

    var slurl_data = {};

    if (urlParts[1] == '' || /*urlParts[1] == 'Places.aspx' {this is always true in this setup.  If this was Default.aspx we'd need this filter, with some extra caveats.} || */urlParts[3] == undefined) {
     slurl_data = generate_slurl_data(regionName, 0, 0, 0, SetTitle, SetImg, SetMsg, true);
    }
    else {
     regionName = (urlParts[2] == undefined) ? 'Reg01' : decodeURIComponent(urlParts[2]);
     slurl_data = generate_slurl_data(
      regionName,
      (urlParts[3] != undefined) ? check_coords(urlParts[3]) : 128,
      (urlParts[4] != undefined) ? check_coords(urlParts[4]) : 128,
      (urlParts[5] != undefined) ? urlParts[5] : 0,
      SetTitle,
      SetImg,
      SetMsg,
      false
     );
    }

    if ($body.data('region-coords-error')) { // The user provided bad coordinates, as indicated by serverside via the body tag's attribute data-region-coords-error
     var errorContent = slurl_data['noexist_windowcontent'];
     $('#marker0').remove();
     /*
     $('#location-heading').html('Destination Guide Picks');
     $('#dest-guide-title').remove();
     $('body').prepend(errorContent);
     $('#map-error #error-content .error-close').click(function () {
      $('#map-error').hide();
     });*/
     $mymap.setView(latlng, zoomLevel);
    }
    else if (!slurl_data['region']['isDefault']) { // The user provided a coordinate in the URL.
     // Convert from region-coordinates to map coordinates using the data-region-coords-x and data-region-coords-y attibutes of the body tag, provided by serverside for this selected region.
     var lon = (parseInt($body.data('region-coords-x')) + slurl_data['region']['target_x'] / 256) * coordScalar/*HACK*/;
     var lat = (parseInt($body.data('region-coords-y')) + slurl_data['region']['target_y'] / 256) * coordScalar/*HACK*/;

     var bubbleContent = slurl_data['windowcontent'];

     if (firstJoinUrl) {
      bubbleContent.find('a.join_button').attr('href', firstJoinUrl);
     }

     showMarkerForSidebar($mymap, [lat, lon], $('#marker0'), bubbleContent);
    }
    else { // Default. Body tag would have data-region-coords-default="true" but doesn't need it.
     $mymap.setView(latlng, zoomLevel);
    }
   }

   function showMarkerForClick(map, markerLatLng, bubbleContent) {
    var windowContent = $('<div/>').html(bubbleContent).get(0);
    L.popup({
     minWidth: 350
    }).setLatLng(markerLatLng).setContent(windowContent).openOn(map);
   }

   function showMarkerForSidebar(map, markerLatLng, $sidebarItem, bubbleContent, regionLocation) {
    var windowContent = $('<div/>').html(bubbleContent).get(0);
    var domId = $sidebarItem.attr('id');
    var marker = L.marker(markerLatLng, {
     /*icon: L.icon({
      iconUrl: "",
      iconSize: [53, 48],
      iconAnchor: [26, 48]
     })*/
    }).addTo($mymap);

    marker.bindPopup(windowContent, {
     minWidth: 350
    }).on('popupopen', function(event) {
     $('#' + domId).addClass('result-selected');
     if (!regionLocation) {
      return;
     }
     var $joinButton = $(event.popup.getPane()).find('a.join_button');
     if (0 < $joinButton.size()) {
      directSlurl($joinButton, regionLocation);
     }
    }).on('popupclose', function(event) {
     $('#' + domId).removeClass('result-selected');
    });

    $sidebarItem.click(function(evt) {
     evt.preventDefault();
     marker.togglePopup();
    });

    map.setView(markerLatLng, MAX_ZOOM_LEVEL);
    marker.openPopup();
   }

   function setupMapLabels(tileLayer, map) {
    let regionLabelLayerGroup = L.layerGroup();
    let regionAvatarMarkerLayerGroup = L.layerGroup();

    regionLabelLayerGroup.addTo(map)
    regionAvatarMarkerLayerGroup.addTo(map)

    //var avatarMarkersByRegion = {}; // for eventual live update idea.

    tileLayer.on('tileload', function(ev) {
     let tileMapX = ev.coords.x;
     let tileMapY = (Math.abs(ev.coords.y) - 1);
     let cacheKey = [tileMapX,tileMapY];

     if (this._getZoomForUrl() == 1 && !regionDataMemoryCache.hasOwnProperty(cacheKey)) {

      jQuery.get('/mapFunctions.aspx', { coordX: tileMapX, coordY: tileMapY, action: 'getRegionByCoordinates'}, function AddLabelHandler(regionData, status) {
       if (typeof(regionData.regionName) !== "undefined") {
        regionDataMemoryCache[cacheKey] = regionData;

        let labelText = regionData.regionName;

        //avatarMarkersByRegion[ev.coords] = [];

        if (regionData.hasOwnProperty("avatars") && regionData.avatars.length > 0) {
         labelText += " (" + regionData.avatars.length + ")";

         // Plot avatar positions on region as small green dots.
         for (let avatarIndex in regionData.avatars) if (regionData.avatars.hasOwnProperty(avatarIndex)) {
          let avatar = regionData.avatars[avatarIndex];
          if (avatar.hasOwnProperty("location")) {
           let avatarMapX = tileMapX + avatar.location.x / 256;
           let avatarMapY = tileMapY + avatar.location.y / 256;

           let avvieMarker = L.circleMarker([avatarMapY, avatarMapX], {
            keyboard: false,
            interactive: false,
            radius: 3,
            stroke: true,
            color: "black",
            weight: 0.8,
            fill: true,
            fillColor: "#0A0",
            fillOpacity: 1.0,
           });

           //avatarMarkersByRegion[ev.coords].push(avvieMarker);

           regionAvatarMarkerLayerGroup.addLayer(avvieMarker);
          }
         }
        }

        if (regionData.hasOwnProperty("maturityRating")) {
         labelText += " (" + regionData.maturityRating + ")";
        }

        regionLabelLayerGroup.addLayer(
         L.marker([tileMapY, tileMapX], {
          keyboard: false,
          interactive: false,
          icon: new L.divIcon({
           html: labelText,
           className: 'RegionLabel',
           iconSize: [256, 16],
           iconAnchor: [0, 16]
          })
         })
        );
       }
      });
     }
    });
    //tileLayer.on('tileunload', function(ev) {
     // I could remove the label here, or not.
    //});
    map.on('zoomend', function() {
     var zoomLevel = MAX_ZOOM_LEVEL - map.getZoom() + 1;

     if (zoomLevel <= MIN_ZOOM_LEVEL) {
      regionLabelLayerGroup.addTo(map)
      regionAvatarMarkerLayerGroup.addTo(map)
     }
     else {
      regionLabelLayerGroup.removeFrom(map);
      regionAvatarMarkerLayerGroup.removeFrom(map);
     }
    });
   }

   function check_coords(slurl_coord) {
    if (is_number(slurl_coord) && ((0 <= slurl_coord) && (slurl_coord <= 256))) {
     return slurl_coord;
    } else {
     return 128;
    }
   }

   function is_number(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
   }

   // *** Page setup ***

   myDebugMap = true;                // True turns on region map location display <x,y>
   var $mymap = {};
   $(function() {
    //var headerHeight = jQuery("#HeaderPos").outerHeight(true);
    //var footerHeight = jQuery("#FooterPos").outerHeight(true);
    //var mapDiv = document.getElementById("BodyArea");
    //mapDiv.style.paddingBottom = footerHeight + "px";
    //mapDiv.style.paddingTop = headerHeight + "px";

    // Get the tile size from the ocean tile.
    var oceanImage = new Image();
    oceanImage.onload = function() {
     var zoomLookup = {
      128: 7,
      256: 6,
      512: 5,
      1024: 4,
     }

     var options = {};

     if (this.width > 0) {
      options.tileSize = this.width;
     }

     $mymap = myMap(document.getElementById('map-container'), options);
     var firstJoinUrl = null; //'http://<%=Request.ServerVariables("HTTP_HOST")%>/Register.aspx'; // To be supplied by serverside if wanted.  Or hardcode, but meh.

     var zoomLevel = 6;
     if (zoomLookup.hasOwnProperty(options.tileSize)) {
      zoomLevel = zoomLookup[options.tileSize];
     }

     coordScalar = options.tileSize/256/*HACK*/;

     loadmap(firstJoinUrl, zoomLevel);
    };
    oceanImage.src = "/maptiles/ocean.jpg";
   });
   // References:
   // http://leafletjs.com/reference-1.1.0.html and http://leafletjs.com/examples/quick-start/
   // Compare with http://places.inworldz.com/ and http://maps.secondlife.com/secondlife/DayStar/153/112/25/?title=SDA%2520Church&img=http://www.bibleprophecyisland.com/Images/Church/Links/ChurchFrontMap.jpg&msg=Worship%2520with%2520us%2520in%2520our%2520church%2520service

   function GetSearch() {
    jQuery.get('/mapFunctions.aspx', { LookWhere: document.getElementById('LookWhere').value, LookFor: document.getElementById('LookFor').value, action: 'getSearch' }, function AddSearchHandler(regionData, status) {
     //TODO: parse out the returned JSON and update page elements: browser URL, 
     //      map with new maplink of first item returned and the list of returned items if any.
     if (regionData.hasOwnProperty(Search)) {                 // Content was returned
      // Add each entry to the sidebar display
      outputHTML = "";
      for (let searchIndex in regionData.Search) if (regionData.Search.hasOwnProperty(searchIndex)) {
       if (searchIndex == 0) {
        // TODO: Place first entry SLURL without secondlife:// into the address bar to trigger display marker of region
        // NOTE: SL also adds ?q=<search text&s=<location selection> and places a map marker for all entries listed.
       }
       let searchitem = regionData.Search[searchIndex];
       if (searchitem.hasOwnProperty("imageUrl")) {
        outputHTML += '<a href="' + searchitem.SLURL.replace("secondlife:/","") + '"><img src="' + searchitem.imageUrl + '" width="128" height="96"border="0"></a>';
       }
       if (searchitem.hasOwnProperty("title")) {
        // SL places the region map coordinates in the title and has a non-functioning parm listing=""
        outputHTML += '<h3><a title="" href="' + searchitem.SLURL.replace("secondlife:/", "") + '">' + searchitem.title + '</h3>';
       }
       if (searchitem.hasOwnProperty("message")) {
        outputHTML += '<p>' + searchitem.message + '</p>';
       }
       outputHTML += '<div id="marker' + searchIndex + '">' + outputHTML + '</div>';
      }
      outputHTML += '<p><b>' + searchIndex + '</b> results for "' + document.getElementById('LookFor').value + '"</p><br />' + outputHTML;
     }
     else {
      if (regionData.hasOwnProperty(status)) {
       // TODO: Display "Not found" in the sidebar search results.
       outputHTML += '<p> Your search for "' + document.getElementById('LookFor').value + '" returned no results.';
      }
     }
     document.getElementById('SearchResults').innerHTML = outputHTML;
    });
   }

  </script>
 </head>
 <body runat="server" id="Body">
  <!-- Built from MyWorld Basic Page template v. 1.0 -->
  <div id="HeaderPos">
   <uc1:Header id="Header1" runat="server"></uc1:Header>
   <uc1:SysMenu id="SysMenu1" runat="server"></uc1:SysMenu>
  </div>
  <div id="BodyArea">
   <div id="map-container"></div>
  </div>
  <div id="LSideBar">
   <table class="SidebarCtl">
    <tr>
     <td class="ProgTitle">My World Places</td>
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
    <tr id="ShowSearch" runat="server">
     <td>
      <select id="LookWhere" size="1">
       <option label="Places" value="places" />
       <option label="Events" value="events" />
      </select><br />
      <input id="LookFor" type="text" size="20" maxlength="64" /><br />
      <input type="Button" id="Button1" value="Search" onclick="GetSearch();" />
     </td>
    </tr>
    <tr id="ShowResults">
     <td id="SearchResults">
     </td>
    </tr>
   </table>
  </div>
  <div id="FooterPos">
   <uc1:Footer id="Footer" runat="server"></uc1:Footer>
  </div>
 </body>
</html>
