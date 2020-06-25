var RentalGridView = (function () {
    var fadeTime = 150;
    var $containerSelected = $(document);

    function init($container) {
        $containerSelected = $container || $containerSelected;

        $containerSelected.on("change", ".js-rental-grid-period", rentalGridPeriod_changed);
        $containerSelected.on("change", ".js-rental-grid-load-option", rentalGridLoadOption_changed);
        $containerSelected.on("click", ".js-rental-grid-prev", rentalGridPrev_click);
        $containerSelected.on("click", ".js-rental-grid-next", rentalGridNext_click);
        $containerSelected.on("click", ".js-rental-grid-plus-button", rentalGridPlus_click);

        var $grid = $containerSelected.find(".js-rental-grid");

        if ($grid.data("auto-loading-disabled") === 0) {
            load($grid.data("starting-url"));
        }

        var $optionChecked = $containerSelected.find('.js-rental-grid-load-option:checked');

        if ($optionChecked.length > 0) {
            rentalGridLoadOption_changed.call($optionChecked);
        }
    }

    function load(url) {
        var $loading = $containerSelected.find(".js-rental-grid-loading").css("visibility", "visible");
        var $grid = $containerSelected.find(".js-rental-grid").html('');

        $.ajax({
            type: "GET",
            url: url
        }).done(function (response) {
            $loading.css("visibility", "hidden");
            $grid.html(response.html);

            var $periodChecked = $grid.find('.js-rental-grid-period:checked');

            if ($periodChecked.length > 0) {
                rentalGridPeriod_changed.call($periodChecked);
            }
        });
    }

    function rentalGridLoadOption_changed(e) {
        var $this = $(this);

        load($this.data("grid-url"));
    }

    function rentalGridPeriod_changed(e) {
        var $this = $(this);

        $containerSelected.find("#RentalStartDate").val($this.data("date"));
        $containerSelected.find("#RentalStartTime").val($this.data("start"));
        $containerSelected.find('[name="RentalStartDateTime"]').val($this.data("date") + " " + $this.data("start"));

        $containerSelected.find("#ProductAvailabilityId").val($this.data("availability"));

        $containerSelected.find('#addToCartContainer').show(fadeTime);
        $containerSelected.find('.js-page-action-button').css('display', '');
    }

    function rentalGridPrev_click(e) {
        e.preventDefault();
        load($(this).attr("href"));
    }

    function rentalGridNext_click(e) {
        e.preventDefault();
        load($(this).attr("href"));
    }

    function rentalGridPlus_click(e) {
        e.preventDefault();
        var $this = $(this);

        $containerSelected.find(".js-rental-grid").find('td').removeClass('hide');
        $this.addClass('hide');
    }

    return {
        init: init
    };
})();