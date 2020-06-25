var Components = (function () {

    function init() {
        autosizeTextArea('textarea');
        datepicker('.datepicker');
        selectpicker('.selectpicker');
        tooltip('[data-toggle="tooltip"]');
    }

    function autosizeTextArea(selector) {
        // autosize($(selector));
    }

    function datepicker(selector) {
        $(selector).datepicker({ format: "dd/mm/yyyy", language: 'pt-BR', autoclose: true });
    }

    function selectpicker(selector) {
        $(selector).select2();
    }

    function tooltip(selector) {
        $(selector).tooltip();
    }

    return {
        init: init,
        autosizeTextArea: autosizeTextArea,
        datepicker: datepicker,
        selectpicker: selectpicker,
        tooltip: tooltip
    };

})();