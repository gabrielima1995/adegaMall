var EcommerceHome = (function () {

    function init() {
        createProductsCarousel();
    }

    function createProductsCarousel() {
        var carousel = $('.ecommerce-home-products .owl-carousel');
        var options = $.extend(OwlCarouselHelper.baseOptions, { loop: carousel.find('img').length > 4 });
        carousel.owlCarousel(options);
    }

    return {
        init: init
    };

})();
