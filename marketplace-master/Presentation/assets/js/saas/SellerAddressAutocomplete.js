var SellerAddressAutocomplete = (function () {

    var autocomplete = null;
    var map = null;
    var marker = null;

    function init() {
        blockAutoFilledFields(null);
        createAutocomplete();
        createMap();
        setHiddenFieldsToUpdateFromTextFields();
        setInitialVisibility();
        $('#btnChangeAddress').click(btnChangeAddress_click);
    }

    function createAutocomplete() {
        var input = document.getElementById('txtAddrAutocomplete');
        autocomplete = new google.maps.places.Autocomplete(input);
        autocomplete.addListener('place_changed', autocomplete_placeChanged);

        $('#txtAddrAutocomplete').keydown(function (e) {
            if (e.which === 13) {
                e.preventDefault();
                return false;
            }
        });
    }

    function createMap() {
        var el = document.getElementById('addressMap');
        map = new google.maps.Map(el, {
            center: { lat: -33.8688, lng: 151.2195 },
            zoom: 13
        });
        marker = new google.maps.Marker({
            map: map,
            anchorPoint: new google.maps.Point(0, -29)
        });
    }

    function autoFillAddressFields(place) {
        if (!place.geometry) return;
        var parts = new GMapsAddressHelper(place).extractParts();

        $('#AddressLat').val(place.geometry.location.lat);
        $('#AddressLng').val(place.geometry.location.lng);

        $('input[name=Address]').val(parts.route.longName);
        $('input[name=AddressNumber]').val(parts.streetNumber.longName);
        $('input[name=AddressNeighborhood]').val(parts.neighborhood.longName);
        $('input[name=AddressZipCode]').val(parts.zipCode.longName);

        var cityState = isPartFound(parts.city) && isPartFound(parts.state)
            ? parts.city.longName + ' - ' + parts.state.shortName
            : null;

        if (cityState !== null) {
            var newOption = new Option(cityState, cityState, true, true);
            $('select[name=CityAndState]').append(newOption).val(cityState).trigger('change');
            $('input[name=CityAndState]').val(cityState);
        }
    }

    function blockAutoFilledFields(place) {
        var parts = place && place.geometry ? new GMapsAddressHelper(place).extractParts() : null;

        if (parts) {
            var hasCityState = isPartFound(parts.city) && isPartFound(parts.state);
            $(':text[name=Address]').prop('disabled', isPartFound(parts.route));
            $(':text[name=AddressNumber]').prop('disabled', isPartFound(parts.streetNumber));
            $(':text[name=AddressNeighborhood]').prop('disabled', isPartFound(parts.neighborhood));
            $(':text[name=AddressZipCode]').prop('disabled', isPartFound(parts.zipCode));
            $('select[name=CityAndState]').prop('disabled', hasCityState);
        }

        if ($(':text[name=AddressZipCode]').val().length <= 5)
            $(':text[name=AddressZipCode]').prop('disabled', false);
    }

    function isPartFound(part) {
        return part && part.found &&
            part.shortName.trim().length > 0 &&
            part.longName.trim().length > 0;
    }

    function setHiddenFieldsToUpdateFromTextFields() {
        var selectors = $.map('AddressLat,AddressLng,Address,AddressNumber,AddressNeighborhood,AddressZipCode,CityAndState'.split(','),
            function (name) { return ':text[name=' + name + ']'; }).join(',');

        $(selectors).change(function (e) {
            var hiddenSelector = 'input[type=hidden][name=' + this.name + ']';
            $(hiddenSelector).val($(this).val());
        });
    }

    function setInitialVisibility() {
        var lat = $('input[name=AddressLat]').val(), lng = $('input[name=AddressLng]').val();
        var hasAddress = lat !== '0' && lat.length && lng !== '0' && lng.length;
        setVisibility(hasAddress, false);
    }

    function setVisibility(hasAddress, hasPlace) {
        $('#changeAddressContainer')[hasAddress ? 'show' : 'hide']();
        $('#addressAutocompleteContainer')[hasAddress ? 'hide' : 'show']();
        $('#addressFieldsContainer')[hasAddress ? 'show' : 'hide']();
        $('#addressMap')[hasPlace ? 'show' : 'hide']();
    }

    function updateMap(place) {
        marker.setVisible(false);
        if (!place.geometry) return;

        if (place.geometry.viewport) {
            map.fitBounds(place.geometry.viewport);
        } else {
            map.setCenter(place.geometry.location);
            map.setZoom(17);
        }

        marker.setPosition(place.geometry.location);
        marker.setVisible(true);
    }

    function autocomplete_placeChanged() {
        var place = autocomplete.getPlace();
        autoFillAddressFields(place);
        blockAutoFilledFields(place);
        setVisibility(place, place);
        updateMap(place);
    }

    function btnChangeAddress_click(e) {
        setVisibility(false, false);

        var inputNames = $.map('AddressLat,AddressLng,Address,AddressNumber,AddressNeighborhood,AddressZipCode,CityAndState,AddressAdditionalInfo'.split(','),
            function (name) { return 'input[name=' + name + ']' }).join(',');

        $(inputNames).val('');
        $('#txtAddrAutocomplete').val('').focus();
    }

    return {
        init: init
    };

})();
