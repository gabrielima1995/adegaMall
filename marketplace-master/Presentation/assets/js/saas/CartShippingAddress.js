var CartShippingAddress = (function () {

    var _chooseShippingAddressUrl;

    function init(chooseShippingAddressUrl, openNewShippingAddressModal) {

        _chooseShippingAddressUrl = chooseShippingAddressUrl;

        $(document.body).ready(function () {
            openNewShippingAddressModal && openNewShippingAddressModal();
        });
    }

    $(document.body).on('click', '.js-new-shipping-address-button', function newShippingAddress_click(e) {
        e.preventDefault();
        openNewShippingAddressModal();
        return false;
    });

    $(document.body).on('click', '.js-delete-shipping-address-button', function deleteShippingAddress_click(e) {
        e.preventDefault();
        var self = $(this);
        var shippingAddressId = self.data().address;
        console.log(shippingAddressId);

        $.ajax({
            url: "/Cart/DeleteShippingAddress?shippingAddressId=" + shippingAddressId,
            type: 'POST'
        });
    });

    function openNewShippingAddressModal() {
        $('#js-new-shipping-address-modal').modal('show');
    }

    return {
        init: init
    };

})();