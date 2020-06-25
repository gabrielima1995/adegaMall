var AvatarEditor = (function () {

    var _changeAvatarUrl;
    var _removeAvatarUrl;
    var _processing = false;

    function init(changeAvatarUrl, removeAvatarUrl) {
        _changeAvatarUrl = changeAvatarUrl;
        _removeAvatarUrl = removeAvatarUrl;

        $('.js-avatar-file').change(fileInput_change);
        $('.js-change-avatar').click(btnChangeAvatar_click);
        $('.js-remove-avatar').click(btnRemoveAvatar_click);
    }

    function btnChangeAvatar_click(e) {
        $(this).parent().find('.js-avatar-file').click();
    }

    function btnRemoveAvatar_click(e) {
        $.ajax({
            type: 'POST',
            url: _removeAvatarUrl,
            success: function () {
                window.location.reload(true);
            }
        });
    }

    function fileInput_change(e) {
        if (_processing) return;
        _processing = true;

        var files = e.target.files;

        if (files && files.length > 0) {
            var form = new FormData();
            form.append('file', files[0]);

            $.ajax({
                type: 'POST',
                url: _changeAvatarUrl,
                data: form,
                processData: false,
                contentType: false,
                success: function () {
                    window.location.reload(true);
                    _processing = false;
                },
                error: function () {
                    $(e.target).val('');
                    _processing = false;
                }
            });
        }
    }

    return {
        init: init
    };

})();