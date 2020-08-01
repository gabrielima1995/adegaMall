(function ($, MaaS) {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-center",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "8000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    $.validator.unobtrusive.adapters.addBool("mustbetrue", "required");

    Globalize.loadCldr.then(function () {
        console.log('Globalize queue loaded');

        $(document).off('.datepicker.data-api');

        MaaS.initPlugins();

        $.fn.button.Constructor.DEFAULTS = {
            loadingText: MaaS.messages.loading_text
        };
    });
    
    $(document).ready(function ($) {
        //MATCH HEIGHT
        $('.box, .equal-height').matchHeight();

        $('a[href="#"]').click(function (e) {
            e.preventDefault();
        });

        $('form').submit(function () {
            var $this = $(this);

            if ($this.valid()) {
                $this.find('button[type="submit"]').button('loading');
            }
        });

        $('.header-search input')
            .focus(function() {
                $('.header-search').addClass('active');
            })
            .blur(function () {
                $('.header-search').removeClass('active');
            });

        //EDIT PORTFOLIO
        $('[data-toggle="toggle"]').change(function () {
            checkToggle($(this));
        });

        //MENU MOBILE
        $('.abre-menu').click(function (e) {
            e.preventDefault();

            $('body').toggleClass('menu-aberto');

            $(this).find('span').toggleClass('glyphicon-menu-hamburger').toggleClass('glyphicon-remove');
        });

        $(window).resize(function () {
            $('body').removeClass('menu-aberto');
            $('.abre-menu span').addClass('glyphicon-menu-hamburger');

            callBackOwl();
        });

        //OWL NAV
        var owlNav = [
            "<span class=\"glyphicon glyphicon-triangle-left\" aria-hidden=\"true\"></span>",
            "<span class=\"glyphicon glyphicon-triangle-right\" aria-hidden=\"true\"></span>"
        ];

        //SELLERS
        var bestSellersCarousel = $('.best-sellers .owl-carousel');
        bestSellersCarousel.owlCarousel({
            loop: bestSellersCarousel.find('img').length > 4,
            margin: 40,
            responsiveClass: true,
            nav: true,
            onInitialized: callBackOwl,
            navText: owlNav,
            responsive: {
                0: {
                    items: 1
                },
                400: {
                    items: 2
                },
                600: {
                    items: 3
                },
                1000: {
                    items: 4
                }
            }
        });

        //MISC CAROUSEL
        var miscCarousel = $('.owl-carousel.misc');
        miscCarousel.owlCarousel({
            loop: miscCarousel.find('img').length > 4,
            margin: 40,
            responsiveClass: true,
            nav: true,
            onInitialized: callBackOwl,
            navText: owlNav,
            responsive: {
                0: {
                    items: 1
                },
                400: {
                    items: 2
                },
                600: {
                    items: 3
                },
                1000: {
                    items: 4
                }
            }
        });

        //MODAL MAIOR DE IDADE
        modalEighteenYearsOld();

        function checkToggle($this) {
            var tipo = $this.attr('toggle');

            $('.toggle-item').stop().fadeOut(function () {
                $('.' + tipo).fadeIn();
            });
        }

        //AJUSTE ARROWS OWL CAROUSEL
        function callBackOwl() {
            var largura = $(window).width();
            if (largura > 768) {
                $('.owl-nav').css('position', 'relative').css({ top: 'calc(50% - 7px)', position: 'absolute' });
            } else {
                $('.owl-nav').attr('style', '');
            }
        }

        function modalEighteenYearsOld() {
            if (localStorage.getItem("maiordeidade") === "sim") return;

            swal({
                title: "Você tem mais de 18 anos?",
                type: "warning",
                showCancelButton: true,
                cancelButtonText: "Não",
                confirmButtonText: "Sim",
                closeOnConfirm: true,
                closeOnCancel: false,
                allowEscapeKey: false,
                allowClickOutside: false
            }, function (value) {
                if (!value) {
                    window.location = "http://www.abrabe.org.br/responsabilidade-social/autorregulamentacao-publicitaria/";
                    return false;
                }

                localStorage.setItem("maiordeidade", "sim");
                return true;
            });
        }
    });
})(window.jQuery, window.MaaS || {});