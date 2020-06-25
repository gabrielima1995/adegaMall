var DisableSellerAssistantActions = (function () {

    var selector = "a.action-disabled-for-assistant, button.action-disabled-for-assistant";

    function init(tooltipText) {
        console.log("Desabilitando links e botões das ações que o assistente não pode executar...")

        tooltipText && $(selector).attr("title", tooltipText).tooltip();

        $(selector).unbind("click").click(linkOrButtonClick); //.prop("disabled", true);
        $(document).on("click", selector, linkOrButtonClick);
    }

    function linkOrButtonClick(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    return {
        init: init
    };

})();