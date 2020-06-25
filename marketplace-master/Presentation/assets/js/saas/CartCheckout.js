var CartCheckout = (function () {

    function init() {
        $(document).ready(function () {
            $('.js-payment-method-selector').click(paymentMethod_click);
            setVisiblePaymentMethod();
        });
    }

    function paymentMethod_click(e) {
        setVisiblePaymentMethod();
    }

    function setVisiblePaymentMethod() {
        var current = $('.js-payment-method-selector').length ? $('.js-payment-method-selector:checked').val() : $('.js-payment-method').val();
        $('#boletoFields')[current === 'Boleto' ? 'show' : 'hide']();
        $('#creditCardFields')[current === 'CreditCard' ? 'show' : 'hide']();
        $('#offlinePaymentFields')[current === 'Offline' ? 'show' : 'hide']();
    }

    return {
        init: init
    };

}) ();