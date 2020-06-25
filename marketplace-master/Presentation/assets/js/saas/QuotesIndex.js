var QuotesIndex = (function () {

    var table = null;

    function init() {
        setupTable();
        $(document).on("click", ".js-close-quote", btnCloseQuote_click);
        $(document).on("click", ".button-cancel-all", btnCancelAll_click);
        handleShowQuoteDetailsButton();
    }

    function setupTable() {
        table = $(".quote-panel-wrapper").DataTable({
            "dom": '<"row"<"col-xs-12"p>><"hr"><"row orcamentos"t>',
            "ordering": false,
            "info": false,
            "iDisplayLength": 5,
            "sPrevious": "«",
            "sNext": "»",
            "oLanguage": {
                "oPaginate": {
                    "sPrevious": "«",
                    "sNext": "»"
                }
            }
        });
    }

    function btnCancelAll_click() {
        $(this).hide()
            .parent().find(".disclaimer").show();
    }

    function btnCloseQuote_click(e) {
        var btn = $(this);
        var btngroup = btn.parent().find("button").button("loading");

        var url = btn.attr("data-url");
        var postData = {
            id: btn.attr("data-id"),
            dealWon: btn.attr("data-deal-won")
        };

        $.ajax({
            type: "POST",
            url: url,
            data: $.param(postData),
            success: function (e) {
                $(".js-open-requests-badge").each(function () {
                    var badge = $(this);
                    var currentCount = parseInt(badge.text());
                    badge.text(currentCount - 1);
                });

                btn.parents("tr").fadeOut("fast", function () {
                    table.row("[data-id=" + postData.id + "]").remove().draw(false);
                });

                btngroup.button("reset");
            },
            error: function (e) {
                btngroup.button("reset");
                alert("Error: " + error);
            }
        });
    }

    function handleShowQuoteDetailsButton() {
        $(document).on("click", ".quote-show-details a", function (e) {
            var link = $(this).hide();
            link.parent(/*small.quote-show-details*/).next(/*ul.orcamento-details*/).show();
        });
    }

    return {
        init: init
    };

})();
