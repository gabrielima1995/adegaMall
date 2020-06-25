(function ($, MaaS) {

    var isMobile = window.isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
    console.log('plugins.js | isMobile = ' + isMobile);

    MaaS.initPlugins = function () {
        $('.js-date, .date')
            .each(function () {
                var $this = $(this);

                var disableDatepicker = $this.hasClass("no-picker");
                !disableDatepicker && $this.datepicker({ language: Globalize.current.patterns.lang });

                if (!isMobile) $this.mask('00/00/0000');
            })
            .focus(function () {
                if (isMobile) unmask($(this));
            })
            .blur(function () {
                if (isMobile) setDateMask($(this));
            });

        $('.js-time, .time')
            .each(function () {
                if (!isMobile) {
                    var $this = $(this);
                    $this.mask('00:00');
                }
            })
            .focus(function () {
                if (isMobile) unmask($(this));
            })
            .blur(function () {
                if (isMobile) setTimeMask($(this));
            });

        $('.js-mask')
            .each(function () {
                if (!isMobile) {
                    var $this = $(this);
                    var mask = $this.data('mask') || null;
                    if (mask) $this.mask(mask);
                }
            })
            .focus(function () {
                if (isMobile) unmask($(this));
            })
            .blur(function () {
                if (isMobile) {
                    var mask = $this.data('mask') || null;
                    if (mask) $this.mask(mask);
                }
            });

        $('.js-decimal')
            .each(function () {
                var $this = $(this);
                var mask = Globalize.current.patterns.decimal;
                var places = $this.data('places') || null;

                if (places !== null && typeof places === "number")
                    mask = Globalize.current.patterns.integer + Globalize.current.patterns.decimalSeparator + padLeft(0, parseInt(places));

                $this.mask(mask, { reverse: true });
            });

        $('.js-money').mask(Globalize.current.patterns.money, { reverse: true });

        $('.js-zipcode')
            .each(function () {
                var $this = $(this);
                if (!isMobile) $this.mask('00000-000');
            })
            .focus(function () {
                if (isMobile) unmask($(this));
            })
            .blur(function () {
                if (isMobile) setZipCodeMask($(this));
            });

        $('.js-cpf')
            .each(function () {
                if (!isMobile) {
                    var $this = $(this);
                    setCpfCnpjMask($this, false, true);
                }
            })
            .keydown(function (e) {
                if (!isMobile) {
                    var $this = $(this);
                    fixMask(e, function () { setCpfCnpjMask($this, false, true); });
                }
            })
            .focus(function () {
                if (isMobile) unmask($(this));
            })
            .blur(function () {
                if (isMobile) setCpfCnpjMask($(this, false, true));
            });

        $('.js-cnpj')
            .each(function () {
                if (!isMobile) {
                    var $this = $(this);
                    setCpfCnpjMask($this, true, false);
                }
            })
            .keydown(function (e) {
                if (!isMobile) {
                    var $this = $(this);
                    fixMask(e, function () { setCpfCnpjMask($this, true, false); });
                }
            })
            .focus(function () {
                if (isMobile) unmask($(this));
            })
            .blur(function () {
                if (isMobile) setCpfCnpjMask($(this, true, false));
            });

        $('.js-cpfcnpj')
            .each(function () {
                if (!isMobile) {
                    var $this = $(this);
                    setCpfCnpjMask($this, false, false);
                }
            })
            .keydown(function (e) {
                if (!isMobile) {
                    var $this = $(this);
                    fixMask(e, function () { setCpfCnpjMask($this, false, false); });
                }
            })
            .focus(function () {
                if (isMobile) unmask($(this));
            })
            .blur(function () {
                if (isMobile) setCpfCnpjMask($(this, false, false));
            });

        $('.js-digits')
            .each(function () {
                var $this = $(this);
                var digits = parseInt($this.attr('maxlength') || 0);

                if (digits > 0 && !isNaN(digits)) {
                    var mask = padLeft(0, parseInt(digits));
                    $this.mask(mask);
                }
            });

        $('.js-credit-card')
            .each(function () {
                var $this = $(this);
                setCreditCardMask($this);
            })
            .keydown(function (e) {
                if (!isMobile) {
                    var $this = $(this);
                    fixMask(e, function () { setCreditCardMask($this); });
                }
            })
            .focus(function () {
                if (isMobile) unmask($(this));
            })
            .blur(function () {
                if (isMobile) setCreditCardMask($(this));
            });

        $('.js-select').select2({ language: Globalize.current.patterns.lang });

        autosize($('textarea'));

        $('[data-toggle="tooltip"]').tooltip();
    };

    function fixMask(e, callback) {
        var charCode = e.which || e.keyCode;

        if ((charCode >= 48 && charCode <= 57) || (charCode >= 96 && charCode <= 105) || charCode === 86)
            setTimeout(callback, 0);
    }

    function padLeft(nr, n, str) {
        return Array(n - String(nr).length + 1).join(str || '0') + nr;
    }

    function setCpfCnpjMask($this, cpfDisabled, cnpjDisabled) {
        unmask($this);
        var val = $this.val().replace(/\./g, '').replace(/\-/g, '').replace(/\//g, '');
        var len = val.length;

        if (len <= 11) !cpfDisabled && $this.mask("999.999.999-999");
        else !cnpjDisabled && $this.mask("99.999.999/9999-99");
    }

    function setCreditCardMask($this) {
        unmask($this);
        var val = $this.val();
        var isAmexOrDiners = /^(3[47]|3(?:0[0-5]|[68]))/.test(val);

        if (isAmexOrDiners) $this.mask("0000 000000 00000");
        else $this.mask("0000 0000 0000 0000");
    }

    function setDateMask($this) {
        unmask($this);
        $this.mask("00/00/0000");
        $this.trigger("change");
    }

    function setTimeMask($this) {
        unmask($this);
        $this.mask("00:00");
        $this.trigger("change");
    }

    function setZipCodeMask($this) {
        unmask($this);
        $this.mask("00000-000");
        $this.trigger("change");
    }

    function unmask($this) {
        try { $this.unmask(); }
        catch (e) { console.log("plugins.js -> unmask -> error:" + e); }
    }

})(window.jQuery, window.MaaS || {});