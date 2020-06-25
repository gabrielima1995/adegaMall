var SellerDetails = (function () {

    var btnCancelChangeStatus,
        btnCancelModifyFee,
        btnChangeStatus,
        btnConfirmChangeStatus,
        btnEditFeeForm,
        changeStatusConfirmationContainer,
        frmChangeStatus,
        editFeeForm,
        customFeeItems,
        feeInputForms;

    function init() {
        btnCancelChangeStatus = $('#btnCancelChangeStatus');
        btnChangeStatus = $('#btnChangeStatus');
        btnConfirmChangeStatus = $('#btnConfirmChangeStatus');
        btnCancelModifyFee = $('#btn-cancel-modify-fee');
        btnEditFeeForm = $('#btnEditFeeForm');

        changeStatusConfirmationContainer = $('#changeStatusConfirmationContainer');
        frmChangeStatus = $('#frmChangeStatus');
        editFeeForm = $('#editFeeForm');
        customFeeItems = $('.custom-fee-item');
        feeInputForms = $('.fee-input-form');

        $('.js-add-credits').on('click', function (e) {
            $('.js-credits-box').show();
        });


        btnCancelChangeStatus.click(btnCancelChangeStatus_click);
        btnChangeStatus.click(btnChangeStatus_click);
        btnConfirmChangeStatus.click(btnConfirmChangeStatus_click);
        btnEditFeeForm.click(btnEditFeeForm_click);
        btnCancelModifyFee.click(btnCancelModifyFee_click);
    }

    function btnCancelModifyFee_click(e) {
        editFeeForm.hide();
        btnEditFeeForm.parent().show();
        customFeeItems.show();
    }

    function btnEditFeeForm_click(e) {
        editFeeForm.show();
        btnEditFeeForm.parent().hide();
        customFeeItems.hide();
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
