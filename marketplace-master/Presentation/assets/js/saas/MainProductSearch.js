var MainProductSearch = (function () {

    var _frm;

    function init() {
        _frm = $('#frmProductSearch');
        initSearchOnEnter();
    }

    function initSearchOnEnter() {
        _frm.find(':text').keyup(function (e) {
            if (e.which === 13) _frm.submit(); 
        });
    }

    return {
        init: init
    };

})();
