var SearchLoaderOnScroll = (function () {

    function init(callback) {
        var $win = $(window);
        var loading = false;
        var noResults = false;

        // Attempts to maintain scroll position when the plugin runs.
        $.fn.matchHeight._maintainScroll = true;

        $win.scroll(function () {
            if (!noResults && !loading && $('#js-result-wrapper').height() - $win.height() <= ($win.scrollTop() + 500)) {
                var nextUrl = $('#js-result-wrapper .js-next-url:last').val();

                if (nextUrl) {
                    loading = true;
                    $('#js-loading').show();

                    $.ajax({
                        url: nextUrl,
                        success: function (data) {
                            loading = false;
                            $('#js-loading').hide();

                            if (!data) noResults = true;
                            else {
                                $('#js-result-wrapper').append(data);
                                SearchImageLoader && SearchImageLoader.loadImages();
                            }

                            if (typeof (callback) === "function")
                                callback();
                        }
                    });
                }
                else noResults = true;
            }
        });
    }

    return {
        init: init
    };

})();