var GoogleMapsAutocomplete = (function () {

    var _autocomplete = null;
    var _geocoding = null;

    var _useBrowserLocation = true;

    function init() {
        var input = $("#GoogleMapsAutocomplete").change(addressInput_change);
        if (!input.length) { console.log("GoogleMapsAutocomplete element not found."); return; }
        if (!(google && google.maps && google.maps.places)) { console.log("Plugin google.maps.places not loaded."); return; }
        _autocomplete = new google.maps.places.Autocomplete(input[0]);
        _autocomplete.addListener("place_changed", autocomplete_placeChanged);

        
        if (_useBrowserLocation && isHttps && navigator.geolocation)
        {
            if (!($("#hdnLocationAddress").val().length > 0))
            {
                _geocoding = new google.maps.Geocoder;
                navigator.geolocation.getCurrentPosition(function (position)
                {
                    var lat = position.coords.latitude;
                    var lng = position.coords.longitude;
                    _geocoding.geocode({ 'location': { 'lat': lat, 'lng': lng } }, onGeocode);
                });
            }
        }

        $('#js-clear-autocomplete').on('click', clearAutocomplete_click);
    }

    function initNoLocation() {
        _useBrowserLocation = false;
        init();
    }

    function clearAutocomplete_click() {
        $('#GoogleMapsAutocomplete').val('');
        $('#hdnLocationAddress').val('');
        $('#hdnLocationLat').val('');
        $('#hdnLocationLng').val('');
        $('#hdnLocationName').val('');
        $('#hdnBoundsAX').val('');
        $('#hdnBoundsBX').val('');
        $('#hdnBoundsAY').val('');
        $('#hdnBoundsBY').val('');
    }

    function onGeocode(results, status) {
        if (status === 'OK') {
            $('#GoogleMapsAutocomplete').val(results[0].formatted_address);
            var res = extractPlaceInfo(results[0]);
            $('#hdnLocationAddress').val(res.address);
            $('#hdnLocationLat').val(res.location.lat);
            $('#hdnLocationLng').val(res.location.lng);
            $('hdnLocationName').val('');
            $('hdnBoundsAX').val('');
            $('hdnBoundsBX').val('');
            $('hdnBoundsAY').val('');
            $('hdnBoundsBY').val('');
        }
    }

    function addressInput_change(e) {
        if ($(this).val().trim().length === 0)
            $(['#hdnLocationName', '#hdnLocationAddress', '#hdnLocationLat', '#hdnLocationLng']
                .join(', ')).val('');
    }

    function autocomplete_placeChanged() {
        var place = _autocomplete.getPlace();
        if (!place.geometry) {
            return; // requisitar que usuário informe um local, ou fazer a busca vazia(?)
        }
        else {
            var info = extractPlaceInfo(place);
            var container = $("#GoogleMapsAutocomplete").parent();

            container.find("#hdnLocationName").val(info.name);
            container.find("#hdnLocationAddress").val(info.address);

            if (place.geometry.viewport) {
                let ne = place.geometry.viewport.getNorthEast();
                let sw = place.geometry.viewport.getSouthWest();

                container.find("#hdnBoundsAY").val(ne.lat());
                container.find("#hdnBoundsAX").val(ne.lng());
                container.find("#hdnBoundsBY").val(sw.lat());
                container.find("#hdnBoundsBX").val(sw.lng());
                container.find("#hdnLocationLat").val(info.location.lat);
                container.find("#hdnLocationLng").val(info.location.lng);
            }
            else {
                container.find("#hdnLocationLat").val(info.location.lat);
                container.find("#hdnLocationLng").val(info.location.lng);
            }

            //frm.submit();
        }
    }

    function extractPlaceInfo(place) {
        console.log(place);
        if (place && place.geometry) {
            var address = "";
            if (place.address_components) {
                address = [
                    (place.address_components[0] && place.address_components[0].short_name || ""),
                    (place.address_components[1] && place.address_components[1].short_name || ""),
                    (place.address_components[2] && place.address_components[2].short_name || "")
                ].join(" ");
            }

            return {
                name: place.name,
                address: address,
                location: {
                    lat: place.geometry.location.lat(),
                    lng: place.geometry.location.lng()
                }
            };
        }
    }

    function isHttps() {
        return window.location.protocol === 'https:';
    }

    return {
        init: init,
        initNoLocation: initNoLocation
    };

})();