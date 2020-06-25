var QuoteDetails = (function () {

    function init(callback) {
        focusOnForm();
        $('#frmAddAnswer').submit(function(e) {
            frmAddAnswer_submit.call(this, e, callback);
        });
        $('.js-close-quote').click(btnCloseQuote_click);
        $('.button-cancel-all').click(btnCancelAll_click);
    }

    function focusOnForm() {
        var qs = (window.location.search || '').toLowerCase();
        if (qs.indexOf('addanswer') >= 0) $('#Text').focus();
    }
    
    function addAnswer_begin() {
        $('#btnAddAnswer').button('loading');
    }

    function addAnswer_complete() {
        $('#btnAddAnswer').button('reset');
    }

    function addAnswer_success() {
        $('#Text, #Price').val('');
    }

    function btnCancelAll_click() {
        $('#quoteProposalHelp').show();
    }

    function btnCloseQuote_click(e) {
        var btn = $(this);
        var btngroup = btn.parent().find('button').button('loading');

        var url = btn.attr('data-url');
        var postData = {
            id: btn.attr('data-id'),
            dealWon: btn.attr('data-deal-won')
        };

        $.ajax({
            type: 'POST',
            url: url,
            data: $.param(postData),
            success: function (e) {
                window.location.reload();
            },
            error: function (e) {
                alert('Error: ' + error);
                btngroup.button('reset');
            }
        });
    }

    function frmAddAnswer_submit(e, callback) {
        e.preventDefault();

        var frm = $(this);
        if (!frm.valid()) return false;

        var url = frm.attr('action');
        var formData = new FormData(this);

        $.ajax({
            url: url,
            data: formData,
            processData: false,
            contentType: false,
            type: 'POST',
            beforeSend: function () {
                $('#btnAddAnswer').button('loading');
            },
            success: function (data) {
                $('#Text, #Price').val('');
                $('#answersContainer').html(data);

                if (callback) {
                    callback();
                }
            },
            complete: function () {
                $('#btnAddAnswer').button('reset');
            }
        });
    }

    return {
        init: init
    };

})();
