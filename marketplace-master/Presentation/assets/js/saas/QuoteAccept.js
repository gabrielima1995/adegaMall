var QuoteAccept = (function () {

    function init() {

        $('.accept-quote-btn').click(function () {
            $(this).hide();
            $(this).parent().children('.accept-disclaimer').show();
        });

        $('.confirm-accept').click(function () {
            $(this).prop('disable');
        });

        $('#SendMessage').change(function () {
            if (this.checked) {
                $('#proposalFieldsContainer').show();
                $('#quoteProposalHelp').show();
            }
        });

        $('#CancelQuote').change(function () {
            if (this.checked) {
                $('#proposalFieldsContainer').hide();
                $('#quoteProposalHelp').hide();
            }
        });
    }

    return { init: init };

})();