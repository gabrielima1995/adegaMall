var SellersImport = (function () {

    function init() {
        $(".js-import-file").change(importFile_change);
        $("#frmUpload").submit(frmUpload_submit);
        $("#SourceTypeSelect").change(sourceTypeSelect_change);
    }

    function checkFileExtensionAndSetUI() {
        var format = $("#SourceType").val();
        var filename = $(".js-import-file").val();
        var extension = filename.replace(/^.*\./, "").toLowerCase();

        var isRightExtension = extension && (
            (format === "excel" && "xlsx".indexOf(extension) >= 0) ||
            (format === "csv" && extension === "csv"));

        $("#invalidFileType")[!extension || isRightExtension ? "hide" : "show"]();
        $("#btnUpload")[!isRightExtension ? "hide" : "fadeIn"]();
    }

    function importFile_change() {
        checkFileExtensionAndSetUI();
    }

    function frmUpload_submit() {
        window.setTimeout(function () {
            $("#dontLeavePage").fadeIn();
            $(".js-import-file").prop("disabled", true);
        }, 1000);

        $("#btnUpload").button("loading")
            .next(/*spinner*/).show();
    }

    function sourceTypeSelect_change() {
        var value = $(this).val();
        $("#steps")[value ? "fadeIn" : "hide"]();
        
        var btnTemplate = $("#btnDownloadTemplate");
        var baseUrl = btnTemplate.attr("data-baseurl");
        var templateUrl = baseUrl + (baseUrl.indexOf("?") > 0 ? "&" : "?") + "type=" + value;
        btnTemplate.attr("href", templateUrl);

        $("#SourceType").val(value);
        checkFileExtensionAndSetUI();
    }

    return {
        init: init
    };

})();