var BuyerDetails = (function () {

    var btnCancelChangeStatus,
        btnChangeStatus,
        btnConfirmChangeStatus,
        changeStatusConfirmationContainer,
        frmChangeStatus;

    function init() {
        btnCancelChangeStatus = $('#btnCancelChangeStatus');
        btnChangeStatus = $('#btnChangeStatus');
        btnConfirmChangeStatus = $('#btnConfirmChangeStatus');
        changeStatusConfirmationContainer = $('#changeStatusConfirmationContainer');
        frmChangeStatus = $('#frmChangeStatus');

        btnCancelChangeStatus.click(btnCancelChangeStatus_click);
        btnChangeStatus.click(btnChangeStatus_click);
        btnConfirmChangeStatus.click(btnConfirmChangeStatus_click);
    }

    function btnCancelChangeStatus_click(e) {
        btnChangeStatus.show();
        changeStatusConfirmationContainer.hide();
    }

    function btnChangeStatus_click(e) {
        btnChangeStatus.hide();
        changeStatusConfirmationContainer.show();
    }

    function btnConfirmChangeStatus_click(e) {
        frmChangeStatus.submit();
    }

    return {
        init: init
    }

})();
