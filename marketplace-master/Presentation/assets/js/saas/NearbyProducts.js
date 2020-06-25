var NearbyProducts = (function () {
    var nearbyUrl = '/products/nearby/?lang=' + window.MaaS.lang;

    var $nearbyContainer = null;
    var $geolocationOffMessage = null;

    function init() {
        $nearbyContainer = $('.nearby-products-container');
        $geolocationOffMessage = $('.js-geolocation-off-message');

        loadNearbyProducts();
    }

    function loadNearbyProducts() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                function (position) {
                    $geolocationOffMessage.css('display', 'none');

                    var lat = position.coords.latitude;
                    var lng = position.coords.longitude;

                    loadProducts(lat, lng);
                },
                function () {
                    $geolocationOffMessage.css('display', 'block');
                });
        } else {
            $nearbyContainer.css('display', 'none');
        }
    }

    function loadProducts(lat, lng) {
        $.ajax({
            type: 'GET',
            url: nearbyUrl,
            data: {
                Lat: lat,
                Lng: lng
            },
            success: function (data) {
                $nearbyContainer.html(data.html);

                SearchImageLoader.loadImages();
            },
            error: function (req, status, error) {
                $nearbyContainer.find('.js-message-error').css('display', 'block').text(status + ': ' + error);
            }
        });
    }

    return {
        init: init
    };
})();