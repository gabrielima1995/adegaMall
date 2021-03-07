var EcommerceHome2 = (function () {

    function init() {
        createProductsCarousel();
    }

    function createProductsCarousel() {
        var owlNav = [
            "<span class=\"glyphicon glyphicon-triangle-left\" aria-hidden=\"true\"></span>",
            "<span class=\"glyphicon glyphicon-triangle-right\" aria-hidden=\"true\"></span>"
        ];

        var $carousels = $('.ecommerce-home-products');

        $carousels.each(function () {
            var nextButton = $(this).find('.carousel-next-button');
            var prevButton = $(this).find('.carousel-prev-button');
            var carousel = $(this).find('.owl-carousel');

            var options = {
                loop: true,
                autoplay: true,
                nav: false,
                dots: false,
                responsiveClass: true,
                responsive: {
                    0: {
                        items: 2,
                        margin: 15
                    },
                    426: {
                        items: 3,
                        margin: 20
                    },
                    769: {
                        items: 4,
                        margin: 25
                    }
                }
            };

            carousel.addClass('owl-carousel');

            var owl = carousel.owlCarousel(options);

            nextButton.click(function () {
                owl.trigger('next.owl.carousel');
            });

            prevButton.click(function () {
                owl.trigger('prev.owl.carousel');
            });
        });
    }

    return {
        init: init
    };

})();
