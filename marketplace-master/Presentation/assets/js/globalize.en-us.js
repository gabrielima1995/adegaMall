(function ($, undefined) {
    Globalize.loadCldr =
        $.when(
            $.get("/assets/js/cldr/supplemental/likelySubtags.json"),
            $.get("/assets/js/cldr/main/en/numbers.json"),
            $.get("/assets/js/cldr/supplemental/numberingSystems.json"),
            $.get("/assets/js/cldr/supplemental/plurals.json"),
            $.get("/assets/js/cldr/supplemental/ordinals.json"),
            $.get("/assets/js/cldr/main/en/currencies.json"),
            $.get("/assets/js/cldr/supplemental/currencyData.json"),
            $.get("/assets/js/cldr/main/en/ca-gregorian.json"),
            $.get("/assets/js/cldr/main/en/timeZoneNames.json"),
            $.get("/assets/js/cldr/supplemental/timeData.json"),
            $.get("/assets/js/cldr/supplemental/weekData.json")
        ).then(function () {
            // Normalize $.get results, we only need the JSON, not the request statuses.
            return [].slice.apply(arguments, [0]).map(function (result) {
                return result[0];
            });
        }).then(Globalize.load).then(function () {
            console.log('Globalize loaded');

            // Initialise Globalize to the current UI culture
            Globalize.current = Globalize.locale("en");

            Globalize.current.patterns = {
                money: '#,###,##0.00',
                integer: '#,###,##0',
                decimal: '#,###,##0.0',
                decimalSeparator: '.',
                lang: 'en-US',
                date: 'MM/DD/YYYY',
                time: 'HH:mm',
                dateTime: 'MM/DD/YYYY HH:mm'
            };
        });
})(window.jQuery);
