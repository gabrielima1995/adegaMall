var Main = (function () {

    function init() {
        datepickers();
    }

    function datepickers() {

        $('.date-picker').datepicker({ format: "dd/mm/yyyy", language: 'pt-BR', autoclose: true });
        $('.datemask').mask('00/00/0000', { placeholder: '__/__/____' });
        $('.datetimemask').mask('00/00/0000 00:00', { placeholder: '__/__/____ __:__' });
    }

    return {
        init: init
    };

}) ();