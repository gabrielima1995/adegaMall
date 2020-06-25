var TransferJournal = (function () {

    function init() {
        $('#frmTransferJournalFilter').find('input[type="radio"]').click(function (e) {
            $(this).parents('form').submit();
        })
    }

    return {
        init: init
    };

})();