var SearchIndex = (function () {
    var Popup;

    var _bounds;
    var _infoWindows = [];
    var _map;
    var _markers = [];
    var _results;

    function init(isMap, results) {
        if (isMap) {
            initMapView(results);
        }
        else {
            SearchLoaderOnScroll.init();
            SearchImageLoader.init();
            SearchImageLoader.loadImages();
        }

        SwitchViewMode.init();
        SortingSelector.init();
    }

    function addMarkers() {
        for (var i = 0; i < _results.length; i++) {
            var item = _results[i];
            var marker = new google.maps.Marker({
                map: _map,
                position: new google.maps.LatLng(item.LocationLat, item.LocationLng),
                title: item.Title
            });
            attachInfoWindow(marker, item);
            _bounds.extend(marker.position);
            _markers.push(marker);
        }

        if (_markers.length) {
            google.maps.event.addListenerOnce(_map, 'zoom_changed', function () {
                if (_map.getZoom() > 18)
                    _map.setZoom(18);
            });

            _map.fitBounds(_bounds); // zoom
            _map.panToBounds(_bounds); // center
        }
    }

    function attachInfoWindow(marker, data) {
        var content = $('#infoWindowMarkup').html()
            .replaceAll('{DETAILS_URL}', data.DetailsUrl || '')
            .replaceAll('{MAPS_URL}', data.MapLinks.Search || '')
            .replaceAll('{DIRECTIONS_URL}', data.MapLinks.Directions || '')
            .replaceAll('{DISTANCE}', data.FriendlyDistance || '')
            .replaceAll('{IMAGE_URL}', data.ImageUrl || '')
            .replaceAll('{TITLE}', data.Title || '');
        var infoWindow = new Popup({
            content: content
        });
        marker.addListener('click', function (e) {
            offsetCenter(marker.getPosition(), 0, -150)
            closeAllInfoWindows();
            infoWindow.open(_map, marker);
        });
        google.maps.event.addListener(_map, 'click', function () {
            infoWindow.close();
        });
        _infoWindows.push(infoWindow);
    }

    function closeAllInfoWindows() {
        for (var i = 0; i < _infoWindows.length; i++)
            _infoWindows[i].close();
    }

    function createMap() {
        _map = new google.maps.Map(document.getElementById('map'), {
            center: { lat: -25.4289541, lng: -49.267137 },
            zoom: 13
        });
    }

    function initMapView(results) {
        Popup = defineGMapsPopupClass();

        _bounds = new google.maps.LatLngBounds();
        _results = results;

        createMap();
        addMarkers();
        new MarkerClusterer(_map, _markers, { imagePath: '/assets/images/map-cluster.png' })
    }

    function offsetCenter(latlng, offsetx, offsety) {
        // latlng is the apparent centre-point
        // offsetx is the distance you want that point to move to the right, in pixels
        // offsety is the distance you want that point to move upwards, in pixels
        // offsetx and offsety are both optional

        var scale = Math.pow(2, _map.getZoom());
        var worldCoordinateCenter = _map.getProjection().fromLatLngToPoint(latlng);
        var pixelOffset = new google.maps.Point((offsetx / scale) || 0, (offsety / scale) || 0);

        var worldCoordinateNewCenter = new google.maps.Point(
            worldCoordinateCenter.x - pixelOffset.x,
            worldCoordinateCenter.y + pixelOffset.y
        );

        var newCenter = _map.getProjection().fromPointToLatLng(worldCoordinateNewCenter);
        _map.panTo(newCenter);
    }

    return {
        init: init
    };

}());

var SwitchViewMode = (function () {

    var _frm; 

    function init() {
        _frm = $('#frmSwitchViewMode');
        _frm.find('button').click(btnViewMode_click);
    }

    function btnViewMode_click(e) {
        var self = $(this);
        var value = self.attr('data-view-mode');
        _frm.find('#hdnViewMode').val(value);
        _frm.submit();
    }

    return {
        init: init
    };

})();

var SortingSelector = (function () {

    var _frm; 

    function init() {
        _frm = $('#frmSorting');
        _frm.find('select').change(select_change);
    }

    function select_change(e) {
        var self = $(this);
        _frm.submit();
    }

    return {
        init: init
    };

})();
