var ManageBalance = (function () {

    function init() {
        listWithdraws();
    }

    function listWithdraws() {
        $.get("/manage/listWithdraws", function (response) {
            $(".fa-spinner").hide();

            var source = $("#withdrawItemTemplate").html();
            var template = Handlebars.compile(source);
            var total = 0;
            if (response.IsSuccess && response.Data.length) {
                var container = $(".seller-withdraws-list");
                var totalAmountContainer = $(".seller-withdraws-total");
                for (var i = 0; i < response.Data.length; i++) {
                    total = response.Data[i].Amount + total;
                    response.Data[i].Amount = centsToCurrency(response.Data[i].Amount);
                    response.Data[i].Status = convertStatus(response.Data[i].Status);
                    response.Data[i].DateCreated = formatDate(response.Data[i].DateCreated);
                    var listItem = template(response.Data[i]);
                    $(listItem).insertBefore(totalAmountContainer);
                }
                if (total !== 0) {
                    var totalDisplay = centsToCurrency(total);
                    totalAmountContainer.show().find("strong").text(totalDisplay);
                }
                container.show();
                $(".seller-withdraws-message").show();
            }
            else {
                $(".seller-withdraws-empty").show();
                response.IsFailure && response.ErrorMessage !== null && $(".seller-withdraws-empty").text(response.ErrorMessage);
            }
        });
    }

    function centsToCurrency(cents) {
        var reais = cents / 100;
        return reais.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
    }

    function convertStatus(status) {
        if (status === "COMPLETED") return "Concluído com sucesso";
        if (status === "FAILED") return "Falha na tentativa de saque";
        return status;
    }

    function formatDate(value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        return dt.getDate() + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();
    }

    return {
        init: init
    };

})();