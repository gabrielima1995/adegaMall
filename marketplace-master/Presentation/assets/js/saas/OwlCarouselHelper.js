var OwlCarouselHelper = (function () {

    var navHandles = [
        '<span class="glyphicon glyphicon-triangle-left" aria-hidden="true"></span>',
        '<span class="glyphicon glyphicon-triangle-right" aria-hidden="true"></span>'
    ];

    var baseOptions = {
        margin: 40,
        responsiveClass: true,
        nav: true,
        onInitialized: initializedCallback,
        navText: navHandles,
        responsive: {
            0: { items: 1 },
            400: { items: 2 },
            600: { items: 3 },
            1000: { items: 4 }
        }
    };

    function initializedCallback() {
        var largura = $(window).width();
        if (largura > 768) {
            $('.owl-nav').css('position', 'relative').css({ top: 'calc(50% - 7px)', position: 'absolute' });
        } else {
            $('.owl-nav').attr('style', '');
        }
    }

    return {
        baseOptions: baseOptions,
        initializedCallback: initializedCallback,
        navHandles: navHandles
    };

})();