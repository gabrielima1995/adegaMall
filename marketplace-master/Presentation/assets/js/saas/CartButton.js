var CartButton = (function () {
    var _productsCountBadge = null;
    var _countProdductsUrl = '/cart/countProducts';

    function countProducts() {
        $.ajax({
            type: 'GET',
            url: _countProdductsUrl,
            success: function (data) {
                _productsCountBadge.text(data.productsCount);
            },
            error: function () {
                _productsCountBadge.text(0);
            }
        });
    }

    function init() {
        _productsCountBadge = $('.js-cart-qty');
        countProducts();
    }

    return {
        countProducts: countProducts,
        init: init
    };

})();