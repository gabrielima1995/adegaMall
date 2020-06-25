var CustomFieldFile = (function () {

    function init() {
        $('.js-file').each(function () {
            let hiddenField = $(this).siblings('.js-file-hidden');

            $(this).uploadfy({
                type: 'platform',
                fileType: 'customFile',
                otherFiles: true,
                started: function () {

                },
                completed: function (files) {
                    hiddenField.val(files[0]);
                },

            });
        });
    }

    return {
        init: init
    };

})();

CustomFieldFile.init();