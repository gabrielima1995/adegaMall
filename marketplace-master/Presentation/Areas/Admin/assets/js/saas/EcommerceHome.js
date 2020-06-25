var EcommerceHome = (function () {

    var _categoryModalActionUrl;
    var _modalContainer;

    function init(categoryModalActionUrl) {
        _categoryModalActionUrl = categoryModalActionUrl;

        _modalContainer = $('#modalCategory').modal({ show: false });

        $('#btnAddCategory').click(btnAddCategory_click);
        $('.js-btn-edit-category').click(btnEditCategory_click);
        $('.js-btn-remove-category').click(btnRemoveCategory_click);
        $(document).on('click change', '#radio-cc-no', onCheckProduct);
        $(document).on('click change', '#radio-cc-yes', onCheckCustomItem);
    }

    function showAddUpdateCategoryModal(categoryId) {
        $.get(_categoryModalActionUrl, { categoryId: categoryId }, function (data) {
            _modalContainer.html(data);
            _modalContainer.modal('show');
            initModal();
            Components.selectpicker(_modalContainer.find('.selectpicker'));
        });
    }

    function initModal() {
        if ($('#radio-cc-yes').attr('checked')) {
            onCheckCustomItem(null);
        } else {
            onCheckProduct(null);
        }
    }

    function onCheckProduct(e) {
        $('.custom-category-form').hide();
        $('.product-category-form').show();
    }

    function onCheckCustomItem(e) {
        $('.custom-category-form').show();
        $('.product-category-form').hide();
    }

    function btnAddCategory_click(e) {
        showAddUpdateCategoryModal();
    }

    function btnEditCategory_click(e) {
        var categoryId = $(this).attr('data-category-id');
        showAddUpdateCategoryModal(categoryId);
    }

    function btnRemoveCategory_click(e) {
        $(this).prev('form').submit();
    }

    return {
        init: init,
        initModal: initModal
    };

})();
