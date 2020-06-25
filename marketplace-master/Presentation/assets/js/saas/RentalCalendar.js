var RentalCalendar = (function () {
    var fadeTime = 150;
    var $containerSelected = $(document);

    function init($container) {
        $containerSelected = $container || $containerSelected;

        $containerSelected.on("click", ".js-rentalshift-addtocart", addToCart_click);
        $containerSelected.on("change", ".js-rental-calendar-month", rentalCalendarMonth_change);
        $containerSelected.on("click", ".js-rental-calendar-previous", rentalCalendarPrevious_click);
        $containerSelected.on("click", ".js-rental-calendar-next", rentalCalendarNext_click);

        getCalendar($containerSelected.find(".js-rental-calendar").data("starting-url"));
    }

    function getCalendar(url) {
        var $loading = $containerSelected.find(".js-rental-calendar-loading").css("visibility", "visible");

        $.ajax({
            type: "GET",
            url: url
        }).done(function (response) {
            $loading.css("visibility", "hidden");
            $containerSelected.find(".js-rental-calendar").html(response.html);
        });
    }

    function addToCart_click(e) {
        var $this = $(this);

        if ($this.is(":checked")) {
            $containerSelected.find(".js-rentalshift-addtocart").prop("checked", false);
            $this.prop("checked", true);

            $containerSelected.find("#RentalStartDate").val($this.data("date"));
            $containerSelected.find("#RentalStartTime").val($this.data("start"));
            $containerSelected.find('[name="RentalStartDateTime"]').val($this.data("date") + " " + $this.data("start"));

            $containerSelected.find("#RentalEndDate").val($this.data("date"));
            $containerSelected.find("#RentalEndTime").val($this.data("end"));
            $containerSelected.find('[name="RentalEndDateTime"]').val($this.data("date") + " " + $this.data("end"));

            $containerSelected.find('#addToCartContainer').show(fadeTime);
            $containerSelected.find('.js-page-action-button').css('display', '');
        }
        else {
            $containerSelected.find('#addToCartContainer').hide();
            $containerSelected.find('.js-page-action-button').hide();
        }
    }

    function rentalCalendarMonth_change() {
        getCalendar($(this).val());
    }

    function rentalCalendarPrevious_click(e) {
        e.preventDefault();
        getCalendar($(this).attr("href"));
    }

    function rentalCalendarNext_click(e) {
        e.preventDefault();
        getCalendar($(this).attr("href"));
    }

    return {
        init: init
    };
})();