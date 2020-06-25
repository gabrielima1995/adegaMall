var StoreProduct = (function () {
    var _productId;

    function init() {
        $('.panel-collapse').on('show.bs.collapse', function () {
            $(this).siblings('.panel-heading').addClass('active');
        });

        $('.panel-collapse').on('hide.bs.collapse', function () {
            $(this).siblings('.panel-heading').removeClass('active');
        });

        configureRentalShiftPrice();
        handleMultiplierOption();
        configureModalDeleteProductAvailabity();
    }

    function configureRentalShiftPrice() {
        var foundRequiredFields = $("#Price").length > 0 && $(".js-shift-price-value").length > 0;
        if (!foundRequiredFields)
            return;

        function setPrices() {
            var cents = $(this).val().replace(/\D/g, "");

            $(".js-shift-option").each(function () {
                var duration = parseInt($(this).data("duration-minutes"), 10);
                var price = duration / 60.0 * cents / 100.0;
                var formattedPrice = price.toFixed(2).replace(".", ",");
                $(this).find(".js-shift-price-value").text(formattedPrice);
            });
        }

        var priceField = $("#Price").change(function () { setPrices.call(this); });
        setPrices.call(priceField[0]);
    }

    function handleMultiplierOption() {
        function applyMultiplierOption() {
            var optionIsFixed = $("#MultiplierOption").val() === "Fixed";
            var multiplierFields = $("#priceMultiplierFields");

            if (optionIsFixed) {
                multiplierFields.hide();
                multiplierFields.find("input").val("");
            }
            else multiplierFields.show();
        }
        applyMultiplierOption();
        $("#MultiplierOption").change(applyMultiplierOption);
    }

    function configureModalDeleteProductAvailabity() {
        $(".js-delete-availability-button").click(function () {
            var self = $(this);
            var deleteUrl = self.data('delete-url');

            var $modal = $('#modalDeleteAvailabilityAlert');

            $modal.find('.confirm-delete-availability').attr('href', deleteUrl);

            $modal.modal('show');
        });
    }

    return {
        init: init
    };
})();