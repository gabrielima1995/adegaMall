var ProductSearch = (function () {

    var $startDateInput = null;
    var $startDate = null;
    var $endDateInput = null;
    var $endDate = null;

    var dateTimeStartPicker = null;
    var dateTimeEndPicker = null;

    var $containerSelected = $(document);

    function init($container) {
        $containerSelected = $container || $containerSelected;

        handleExpandFacets();
        handleFacetClicks();
        handleSortOptionChange();
        handleDatesFilter();

        SearchLoaderOnScroll.init();
        SearchImageLoader.init();
        SearchImageLoader.loadImages();
    }

    function handleExpandFacets() {
        $(".search-result-facets-expand a").click(function () {
            $(this).hide();
            $(".search-result-facets-form").removeClass("hidden-xs");
        });
    }

    function handleFacetClicks() {
        $('#frmFacets :checkbox').click(submitParentForm);
    }

    function handleSortOptionChange() {
        $('#ddlSortType').change(submitParentForm);
    }

    function handleDatesFilter() {
        $startDateInput = $containerSelected.find('#StartDateInput');

        if ($startDateInput.length === 0) return;

        $startDate = $containerSelected.find('#StartDate');
        $endDateInput = $containerSelected.find('#EndDateInput');
        $endDate = $containerSelected.find('#EndDate');

        setupDateTimePicker();

        $startDateInput
            .mask('00/00/0000');

        $endDateInput
            .mask('00/00/0000');

        function setupDateTimePicker() {
            moment.locale(Globalize.current.patterns.lang);

            var startDateStr = $startDate.val();
            var endDateStr = $endDate.val();

            dateTimeStartPicker = $containerSelected.find('#startDatePicker')
                .datepicker({
                    language: Globalize.current.patterns.lang,
                    startDate: moment().toDate()
                })
                .show();

            if (startDateStr) {
                dateTimeStartPicker.datepicker("update", moment(startDateStr).toDate());
            }

            dateTimeEndPicker = $containerSelected.find('#endDatePicker')
                .datepicker({
                    language: Globalize.current.patterns.lang,
                    startDate: moment().toDate()
                })
                .show();

            if (endDateStr) {
                dateTimeEndPicker.datepicker("update", moment(endDateStr).toDate());
            }

            dateTimeStartPicker.on('changeDate', function (e) {
                var momentDate = moment(e.date);
                
                $startDateInput.val(momentDate.format(Globalize.current.patterns.date));
                $startDate.val(momentDate.format('YYYY-MM-DD'));
                $startDateInput.focus();
            });

            dateTimeEndPicker.on('changeDate', function (e) {
                var momentDate = moment(e.date);

                $endDateInput.val(momentDate.format(Globalize.current.patterns.date));
                $endDate.val(momentDate.format('YYYY-MM-DD'));
                $endDateInput.focus();
            });
        }
    }

    function submitParentForm() {
        var frm = $(this).parents('form');
        frm.submit();
    }

    return {
        init: init
    };

}());