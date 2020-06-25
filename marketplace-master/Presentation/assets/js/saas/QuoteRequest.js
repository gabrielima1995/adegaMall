var QuoteRequest = (function () {

    var config;

    function init(settings) {

        config = settings;

        jQuery.validator.prototype.subset = function (container) {
            var ok = true;
            var self = this;
            $(container).find(':input').each(function () {
                if (!self.element($(this))) ok = false;
            });
            return ok;
        };

        handleBtnContinue();
        handleFormSubmit();
        handleAutoFillAddress();
        advanceToNextStepIfBuyerIsLoggedIn();
    }

    function advanceToNextStepIfBuyerIsLoggedIn() {
        if ($("#BuyerId").val().length > 0)
            nextStep();
    }

    function nextStep() {
        $('#js-step1').hide();
        $('#js-step2').show();
    }

    function handleBtnContinue() {
        $('#js-button-continue').click(function () {
            var step1Ok = true;
            var $step1Wrapper = $('#js-step1');
            if ($step1Wrapper.data('start') === '0')
                return;

            if ($('#quote-request-form').validate().subset('#js-step1')) {
                $('.js-step1-control.register-required').each(function (_, el) {
                    if ($(el).val() === '')
                        step1Ok = false;
                });

                if ($('#js-step1-pwd').val() && $('#js-step1-pwd').val().length < 6)
                    step1Ok = false;

                if ($('#js-step1-pwd-cfm').val() && $('#js-step1-pwd-cfm').val().length < 6)
                    step1Ok = false;

                if (step1Ok && $('#js-step1 .field-validation-error').length === 0)
                    registerBuyerLead(nextStep);
            }
        });
    }

    function registerBuyerLead(callback) {
        var data = {
            name: $('#BuyerName').val(),
            email: $('#BuyerEmail').val(),
            phone: $('#BuyerPhone').val(),
            password: $('#js-step1-pwd').val()
        };

        var btn = $('#quote-request-form button');
        $.ajax('/quotes/registerBuyerLead', {
            data: data,
            method: 'POST',
            beforeSend: function () {
                btn.button('loading');
            },
            success: function (response) {
                if (response.success === true) {
                    $("#BuyerId").val(response.id);
                    typeof callback === "function" && callback(response);
                } else {
                    console.log(response.errors)
                    response.errors.forEach(function (item) {
                        toastr.error(item.Value);
                    });
                }
            },
            complete: function() {
                btn.button('reset');
            }
        });
    }

    function handleFormSubmit() {
        $('#quote-request-form').submit(function (e) {
            if ($('#js-step1').is(':visible')) {
                e.preventDefault();
                e.stopPropagation();
                btnContinue_click();
                return false;
            }
        });
    }

    function handleAutoFillAddress() {
        function addOrSelectCityState(cityState) {
            var select = $("#address-f5a");
            var hasOption = select.find("option[value='" + cityState + "']").length;
            if (hasOption) select.val(cityState).trigger('change');
            else select.append(new Option(cityState, cityState, true, true)).trigger('change');
        }
        $('#get-address-button').on('click', function () {
            $.ajax({
                url: config.autoFillAddress.url,
                success: function (data) {

                    if (data === null || data.Street === null) {
                        $("#address-f-error > small").text(config.autoFillAddress.incompleteMessage);
                        $("#address-f-error").show();
                    }
                    else {
                        $("#address-f1").val(data.Street);
                        $("#address-f2").val(data.Number);
                        $("#address-f3").val(data.Neighborhood);
                        $("#address-f4").val(data.ZipCode);
                        addOrSelectCityState(data.CityAndState);
                    }
                }
            });
        });
    }

    return {
        init: init
    };

})();