var ProductRentalPublicPage = (function () {
    var productId = null;
    var $orderProductId = null;
    var $orderProductRentalInfoId = null;

    var $quantity = null;

    var $rentalStartDateTime = null;
    var $rentalStartDate = null;
    var $rentalStartTime = null;
    var $rentalEndDateTime = null;
    var $rentalEndDate = null;
    var $rentalEndTime = null;

    var $productAvailabilityId = null;

    var $datePickerStartContainer = null;
    var $datePickerEndContainer = null;

    var $btnValidateAvailability = null;
    var $actualAvailabilityInfo = null;
    var $unavailableAlertBox = null;
    var $validationLoader = null;
    var $rentalAddedInfoBox = null;
    var $addToCartContainer = null;
    var $btnPageAction = null;
    var $btnAddToCart = null;

    var isFixedPeriod = null;
    var dateTimeStartPicker = null;
    var dateTimeEndPicker = null;
    var fadeTime = 150;
    var validateAvailabilityUrl = '/products/validateAvailabilityForRental/?lang=' + window.MaaS.lang;

    var $containerSelected = $(document);

    function init($container) {
        $containerSelected = $container || $containerSelected;

        productId = $containerSelected.find('#ProductId').val();
        $orderProductId = $containerSelected.find('#OrderProductId'); // página de detalhes do produto do pedido
        $orderProductRentalInfoId = $containerSelected.find('#OrderProductRentalInfoId'); // página de detalhes do produto do pedido, quando produto for do tipo Pacote de créditos.

        $quantity = $containerSelected.find('#Quantity');

        isFixedPeriod = $containerSelected.find('#IsFixedRental').val() === 'True';

        $rentalStartDateTime = $containerSelected.find('[name="RentalStartDateTime"]');
        $rentalStartDate = $containerSelected.find('#RentalStartDate');
        $rentalStartTime = $containerSelected.find('#RentalStartTime');
        $rentalEndDateTime = $containerSelected.find('[name="RentalEndDateTime"]');
        $rentalEndDate = $containerSelected.find('#RentalEndDate');
        $rentalEndTime = $containerSelected.find('#RentalEndTime');

        $productAvailabilityId = $containerSelected.find('#ProductAvailabilityId');

        $datePickerStartContainer = $containerSelected.find('.js-datepicker-start-container');
        $datePickerEndContainer = $containerSelected.find('.js-datepicker-end-container');

        $btnValidateAvailability = $containerSelected.find('#btnValidateAvailabiliy');
        $validationLoader = $containerSelected.find('#validateAvailabilityLoader').hide();
        $actualAvailabilityInfo = $containerSelected.find('#actualAvailabilityInfo').hide();
        $unavailableAlertBox = $containerSelected.find('#unavailableAlertBox').hide();
        $rentalAddedInfoBox = $containerSelected.find('#rentalAddedInfoBox').hide();
        $addToCartContainer = $containerSelected.find('#addToCartContainer');

        if (window.addToCartHidden)
            $addToCartContainer.hide();

        $btnPageAction = $containerSelected.find('.js-page-action-button').hide();
        $btnAddToCart = $containerSelected.find('.js-cart-add-item');

        setupDateTimePicker();

        $rentalStartDateTime
            .change(rentalStartDateTime_change)
            .focus(rentalStartDateTime_focus)
            .mask('00/00/0000 00:00');

        $rentalEndDateTime
            .change(rentalEndDateTime_change)
            .focus(rentalEndDateTime_focus)
            .mask('00/00/0000 00:00');

        $btnValidateAvailability.find('a')
            .click(btnValidateAvailability_click);
    }

    function validateAvailability() {
        var startDateTime = $rentalStartDateTime.val();
        var endDateTime = $rentalEndDateTime.val();

        if (isFixedPeriod)
            endDateTime = startDateTime;

        if (!startDateTime || !endDateTime)
            return;

        $.ajax({
            type: 'POST',
            url: validateAvailabilityUrl,
            data: {
                productId: productId,
                quantity: $quantity.val(),
                startDateTime: startDateTime,
                endDateTime: endDateTime,
                productAvailabilityId: $productAvailabilityId.length >= 1 ? $productAvailabilityId.val() : null,
                ignoreOrderProductId: $orderProductId.length >= 1 ? $orderProductId.val() : null,
                ignoreOrderProductRentalInfoId: $orderProductRentalInfoId.length >= 1 ? $orderProductRentalInfoId.val() : null
            },
            beforeSend: function () {
                $.each([
                    $btnValidateAvailability, $actualAvailabilityInfo, $unavailableAlertBox,
                    $rentalAddedInfoBox, $addToCartContainer],
                    function () { this.hide() });

                $.each([
                    $rentalStartDateTime, $rentalEndDateTime, $btnAddToCart],
                    function () { this.attr('disabled', 'disabled') });

                $validationLoader.show();
            },
            complete: function () {
                $btnValidateAvailability.show();
                $validationLoader.hide();

                $.each([
                    $rentalStartDateTime, $rentalEndDateTime, $btnAddToCart],
                    function () { this.removeAttr('disabled') });
            },
            success: function (data /* { isAvailable: true, availabilityMessage: '' } */) {
                if (data.IsAvaiable) {
                    $actualAvailabilityInfo.show(fadeTime).find('span').text(data.AvailabilityMessage);
                    $addToCartContainer.show(fadeTime);
                    $btnPageAction.css('display', '');
                }
                else {
                    $unavailableAlertBox.show(fadeTime).find('span').text(data.AvailabilityMessage);
                }
            },
            error: function (req, status, error) {
                $unavailableAlertBox.show(fadeTime).find('span').text(status + ': ' + error);
            }
        });
    }

    function setupDateTimePicker() {
        moment.locale(Globalize.current.patterns.lang);

        dateTimeStartPicker = $containerSelected.find('#rentalDateTimeStartPicker')
            .datetimepicker({
                inline: true,
                sideBySide: true,
                locale: Globalize.current.patterns.lang,
                minDate: moment().format('YYYY-MM-DDTHH:00'),
                defaultDate: moment().format('YYYY-MM-DDTHH:00'),
                showClose: true
            })
            .show();

        dateTimeEndPicker = $containerSelected.find('#rentalDateTimeEndPicker')
            .datetimepicker({
                inline: true,
                sideBySide: true,
                locale: Globalize.current.patterns.lang,
                minDate: moment().format('YYYY-MM-DDTHH:00'),
                defaultDate: moment().format('YYYY-MM-DDTHH:00'),
                showClose: true
            })
            .show();

        dateTimeStartPicker.on('dp.change', function (e) {
            $rentalStartDateTime.val(e.date.format(Globalize.current.patterns.dateTime));
            $rentalStartDate.val(e.date.format(Globalize.current.patterns.date));
            $rentalStartTime.val(e.date.format(Globalize.current.patterns.time));
            validateAvailability();
            $rentalStartDateTime.focus();
        });

        dateTimeEndPicker.on('dp.change', function (e) {
            $rentalEndDateTime.val(e.date.format(Globalize.current.patterns.dateTime));
            $rentalEndDate.val(e.date.format(Globalize.current.patterns.date));
            $rentalEndTime.val(e.date.format(Globalize.current.patterns.time));
            validateAvailability();
            $rentalEndDateTime.focus();
        });

        $datePickerStartContainer.hide();
        $datePickerEndContainer.hide();
    }

    function btnValidateAvailability_click(e) {
        e.preventDefault();
        validateAvailability();
    }

    function rentalStartDateTime_change(e) {
        $containerSelected.find('#rentalDateTimeStartPicker').data("DateTimePicker").date(moment($rentalStartDateTime.val(), Globalize.current.patterns.dateTime));
    }

    function rentalStartDateTime_focus(e) {
        $datePickerStartContainer.show();
        $datePickerEndContainer.hide();
    }

    function rentalEndDateTime_change(e) {
        $containerSelected.find('#rentalDateTimeEndPicker').data("DateTimePicker").date(moment($rentalEndDateTime.val(), Globalize.current.patterns.dateTime));
    }

    function rentalEndDateTime_focus(e) {
        $datePickerStartContainer.hide();
        $datePickerEndContainer.show();
    }

    return {
        init: init
    };
})();