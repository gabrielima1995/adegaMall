var CustomCategory = (function () {

    var _customItemFormUrl;
    var _customItemFormDiv;
    var _categoryId;

    function init(customItemFormUrl, categoryId) {
        
        _customItemFormUrl = customItemFormUrl;
        _categoryId = categoryId;

        _customItemFormDiv = $('#custom-item-form');
        _customItemFormDiv.hide();

        $('.js-btn-edit-custom-item').click(btnEditCustomItem_click);
        $('#btnAddCustomItem').click(btnAddCustomItem_click);

        $('.image-zoom-btn').popover({
            html: true,
            content: function () {
                let res = $(this).parent().children('.popover-custom-image').html();
                return res;
            },
            title: "Detalhes Imagem",
        });
    }

    return {
        init: init
    };

    function btnEditCustomItem_click(e) {
        var id = $(this).attr('data-custom-item-id');
        showCustomItemForm(id, true);
    }

    function btnAddCustomItem_click(e) {
        showCustomItemForm(null, false);
    }

    function showCustomItemForm(id, edit) {
        $.get(_customItemFormUrl, { id: id, edit: edit }, function (data) {
            _customItemFormDiv.html(data);
            _customItemFormDiv.show();
        }).done(function () {
            $('#CategoryId').val(_categoryId);
            $('.datemask').mask('00/00/0000', { placeholder: '__/__/____' });
            $('.datepicker').datepicker({ format: "dd/mm/yyyy", language: 'pt-BR', autoclose: true });

            $('#image-upload-form').uploadfy({
                fileType: 'product',
                started: function () {
                    $("#uploadPercent, #uploadSpinner").show();
                    $('#custom-item-submit').button('loading');
                },
                progress: function (percentage) {
                    console.log(percentage);
                    $("#uploadPercent").text(percentage + "%");
                },
                completed: function (files) {
                    $("#uploadSpinner").hide();
                    $("#uploadPercent").text("imagem enviada");
                    $('#image-hidden-url').val(files[0]);
                },
                error: function () {
                    toastr.error("Erro ao fazer upload de arquivo");
                },
                finaly: function () {
                    $('#custom-item-submit').button('reset');
                }
            });
        });
    }

})();
