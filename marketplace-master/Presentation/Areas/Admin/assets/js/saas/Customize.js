var Customize = (function () {

    function init() {
        $('button[type=submit]').click(btnUpload_click);
        $('.js-customize-file-input').change(fileInput_change);
    }

    function btnUpload_click(e) {
        $(this).button('loading');
        $(this).next().show();
    }

    function fileInput_change(e) {
        var hasFiles = this.files.length > 0;
        $(this).parents('.js-customize-file-form').find('.js-customize-file-upload-btn')
            .removeClass(hasFiles ? 'btn-default' : 'btn-primary')
            .addClass(hasFiles ? 'btn-primary' : 'btn-default');
    }

    return {
        init: init
    };

})();