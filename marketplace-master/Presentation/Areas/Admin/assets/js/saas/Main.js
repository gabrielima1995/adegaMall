var Main = (function () {

    function init() {
        applyMasks();
        fixSidebar();
        handleDeleteButtons();
        initTooltips();
    }

    function fixSidebar() {
        $(document).on('click', '.sidebar-toggle', function () {
            if ($('body').hasClass("sidebar-collapse") && $('body').hasClass("sidebar-open")) {
                $('body').removeClass("sidebar-collapse");
            }
        });
    }

    function applyMasks() {
        $('.datemask').mask('00/00/0000', { placeholder: '__/__/____' });
        $('.cepmask').mask('00000-000', { placeholder: '_____-___' });
        $('.cpfmask').mask('000.000.000-00');
        $('.cnpjmask').mask('00.000.000/0000-00');

        $('.moneymask').mask("#.##0,00", { reverse: true })
            .focus(function (e) {
                var el = $(this);
                if (el.val() === '0,00') el.val('');
            })
            .blur(function (e) {
                var el = $(this);
                if (el.val() === '' || el.val() === '0' || el.val() === '00') el.val('0,00');
            });

        var brPhoneMask = function (val) {
            return val.replace(/\D/g, '').length === 11
                ? '(00) 00000-0000'
                : '(00) 0000-00009';
        };

        $('.phonemask').mask(brPhoneMask, {
            onKeyPress: function (val, e, field, options) {
                field.mask(brPhoneMask.apply({}, arguments), options);
            }
        });
    }

    function handleDeleteButtons() {
        $('.delete-button').click(function (e) {
            var deleteButton = $(this).hide();
            deleteButton.next().show();
        });
        $('.confirm-delete-yes').click(function (e) {
            var formId = $(this).attr('data-form-id');
            $('#' + formId).submit();
        });
        $('.confirm-delete-cancel').click(function (e) {
            $(this).parents('.confirm-delete').hide().prev().show();
        });
    }

    function initTooltips() {
        $('[data-toggle="tooltip"], .js-tooltip').tooltip();
    }

    return {
        init: init
    };

})();