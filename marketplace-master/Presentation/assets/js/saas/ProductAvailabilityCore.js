"use strict";

var ProductAvailabilityCore = (function () {
    function timestrToSec(timestr) {
        var parts = timestr.split(":");

        var hours = parseInt(parts[0]);
        var minutes = parseInt(parts.length > 0 ? parts[1] : 0);

        if (isNaN(hours)) hours = 0;
        if (isNaN(minutes)) minutes = 0;

        return (hours * 3600) +
            (minutes * 60);
    }

    function pad(num) {
        if (num < 10) {
            return "0" + num;
        } else {
            return "" + num;
        }
    }

    function formatTime(seconds) {
        return [
            pad(Math.floor(seconds / 3600)),
            pad(Math.floor(seconds / 60) % 60)
        ].join(":");
    }

    function init(calculateEndTime, endTimeInterval) {
        if (calculateEndTime) {
            $('.js-availability-end-time:not([type=hidden])').attr('disabled', '');

            $('.js-availability-start-time').change(function (e) {
                var $this = $(this);
                var startTimeSeconds = timestrToSec($this.val());

                var startTime = formatTime(startTimeSeconds);
                var endTime = formatTime(startTimeSeconds + timestrToSec(endTimeInterval));

                $this.val(startTime);
                $('.js-availability-end-time').val(endTime);
            });
        }
    }
    return {
        init: init
    };
})();