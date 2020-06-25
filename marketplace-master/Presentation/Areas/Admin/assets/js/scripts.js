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

    Globalize.loadCldr.then(function () {
        console.log('Globalize queue loaded');

        $(document).off('.datepicker.data-api');

        MaaS.initPlugins();

        $.fn.button.Constructor.DEFAULTS = {
            loadingText: MaaS.messages.loading_text
        }
    });

    $(document).ready(function ($) {

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

        //EDIT PORTFOLIO
        $('[data-toggle="toggle"]').change(function () {
            checkToggle($(this));
        });

        function checkToggle($this) {
            var tipo = $this.attr('toggle');

            $('.toggle-item').stop().fadeOut(function () {
                $('.' + tipo).fadeIn();
            });
        }
    });
})(window.jQuery, window.MaaS || {});