var ShowModalShippingWarning = (function () {

    var _shippingWarningMessage;

    function init(shippingWarningMessage) {
        console.log('testee');
        _shippingWarningMessage = shippingWarningMessage;
        console.lo(_shippingWarningMessage);
        if (_shippingWarningMessage !== null || _shippingWarningMessage !== "") {
            $('#modalShippingWarning').modal('show');
        }
    }

    return {
        init: init
    };

})();