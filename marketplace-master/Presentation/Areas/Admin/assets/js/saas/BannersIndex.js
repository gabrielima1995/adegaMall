var BannersIndex = (function () {

    function init() {
        $('.js-btn-delete').click(btnDelete_click);
    }

    function btnDelete_click(e) {
        $(this).prev('form').submit();
    }

    return {
        init: init
    };

})();
