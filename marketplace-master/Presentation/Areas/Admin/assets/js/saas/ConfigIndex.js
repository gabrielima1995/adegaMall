var ConfigIndex = (function () {

    var _editor;

    function init() {
        var editorContainer = document.getElementById('jsoneditor');
        var editorOptions = { modes: ["tree", "code"], indentation: 4 };
        var json = JSON.parse($('#MaaSConfig').val());

        $('#frmConfig').submit(frmConfig_submit);

        _editor = new JSONEditor(editorContainer, editorOptions, json);
    }

    function frmConfig_submit(e) {
        $('#MaaSConfig').val(_editor.getText());
        $(this).find('button').button('loading');
    }

    return {
        init: init
    };

})();