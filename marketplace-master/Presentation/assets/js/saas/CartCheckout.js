var CartCheckout = (function () {
    $.validator.addMethod("creditCardValid", function (value, element) {
        return this.optional(element) || validateCreditCardNumber(value);
    });

    $.validator.unobtrusive.adapters.addBool('creditCardValid');

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

    function validateCreditCardNumber(cardNumber) {
        cardNumber = (cardNumber || '').split(' ').join('');
        if (parseInt(cardNumber) <= 0 || (!/\d{15,16}(~\W[a-zA-Z])*$/.test(cardNumber)) || cardNumber.length > 16) {
            return false;
        }
        var carray = new Array();
        for (var i = 0; i < cardNumber.length; i++) {
            carray[carray.length] = cardNumber.charCodeAt(i) - 48;
        }
        carray.reverse();
        var sum = 0;
        for (var i = 0; i < carray.length; i++) {
            var tmp = carray[i];
            if ((i % 2) !== 0) {
                tmp *= 2;
                if (tmp > 9) {
                    tmp -= 9;
                }
            }
            sum += tmp;
        }
        return ((sum % 10) === 0);
    }

    return {
        init: init
    };

}) ();