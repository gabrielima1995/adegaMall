var CartButton = (function () {
    var _button = null;
    var _productsCountBadge = null;
    var _countProdductsUrl = '/cart/countProducts';

    function countProducts() {
        $.ajax({
            type: 'GET',
            url: _countProdductsUrl,
            success: function (data) {
                _button[data.hasProducts ? 'show' : 'hide']();
                _productsCountBadge.text(data.productsCount);
            },
            error: function () {
                _button.show();
                _productsCountBadge.hide();
            }
        });
    }

    function init() {
        _button = $('#btnCart').hide();
        _productsCountBadge = _button.find('.js-cart-qty');
        countProducts();
    }

    return {
        countProducts: countProducts,
        init: init
    };

})();