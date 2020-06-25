var ProductTemplateCreateEdit = (function () {

    var newImagesInput, submitButton;

    function init() {
        bindElements();
        configureImageUpload();
        handleRemoveImage();
        $("#Variants").select2({ closeOnSelect: false });
    }

    function bindElements() {
        newImagesInput = $("#newImages");
        submitButton = $("#btnSubmit");
    }

    function configureImageUpload() {
        ProductImageUploadInfo.init();
        newImagesInput.uploadfy({
            fileType: "producttemplate",
            started: function () {
                setSubmitState(false);
            },
            progress: function (percentage) {
                console.log(percentage);
                ProductImageUploadInfo.showInfo(percentage);
            },
            completed: function (files) {
                ProductImageUploadInfo.showCompleted();
                displayImages(files);
            },
            error: function () {
                ProductImageUploadInfo.hide();
                toastr.error(MaaS.messages.upload_error);
            },
            finaly: function () {
                newImagesInput.val("");
                setSubmitState(true);
                window.setTimeout(ProductImageUploadInfo.hide, 1000);
            }
        });
    }

    function displayImages(files) {
        $.post("/Admin/ProductTemplates/ImagesHtml", { images: files })
            .done(function (html) {
                $("#imagesContainer").append(html);
            })
            .fail(function () {
                toastr.error(MaaS.messages.upload_error);
            })
            .always(function () {
                newImagesInput.val("");
                setSubmitState(true);
            });
    }

    function handleRemoveImage() {
        $(document).on("click", "#imagesContainer .trash", function () {
            $(this).parents(".template-image").remove();
        });
    }

    function setSubmitState(enable) {
        submitButton.button(enable ? "reset" : "loading");
    }

    return {
        init: init
    };

})();

var ProductImageUploadInfo = (function () {

    var _info, _spinner, _pct, _ok;

    function init() {
        _info = $('#uploadInfo');
        _spinner = $('#uploadSpinner');
        _pct = $('#uploadPct');
        _ok = $('#uploadOk');
    }

    function showInfo(pct) {
        var text = Math.round(pct) + '%';
        _info.show();
        _spinner.show();
        _ok.hide();
        _pct.show().text(text);
    }

    function showCompleted() {
        showInfo(100);
        _spinner.hide();
        _ok.show();
    }

    function hide() {
        _info.hide();
    }

    return {
        init: init,
        showInfo: showInfo,
        showCompleted: showCompleted,
        hide: hide
    };

})();