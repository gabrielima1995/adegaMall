var ManageAssistantIndex = (function () {

    function init() {
        bindHandlers();
    }

    function bindHandlers() {
        $("#checkAllPermissions").click(() => setCheckboxes(true));
        $("#uncheckAllPermissions").click(() => setCheckboxes(false));
    }

    function setCheckboxes(checked) {
        $(".seller-assistant-permission-box :checkbox").prop("checked", checked);
    }

    return {
        init
    };

})();