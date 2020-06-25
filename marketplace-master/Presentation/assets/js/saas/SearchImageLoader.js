var SearchImageLoader = (function () {

    var _matchHeightTimeout = null;

    function init() {
        $.fn.matchHeight._afterUpdate = function (event, groups) {
            $('.equal-height').removeClass('equal-height');
            console.log('remove class');
        };
    }

    function loadImages() {
        $('img.js-load-img').each(function (i, el) {
            var img = new Image();
            el = $(el);
            img.src = el.attr('data-src');
            img.onload = function () {
                el.attr('src', img.src);
                el.removeClass('js-load-img');
                img = null;

                if (_matchHeightTimeout) window.clearTimeout(_matchHeightTimeout);
                _matchHeightTimeout = window.setTimeout(matchHeight, 150);
            };
        });
    }

    function matchHeight() {
        $('.equal-height').matchHeight();
    }

    return {
        init: init,
        loadImages: loadImages
    };

}());