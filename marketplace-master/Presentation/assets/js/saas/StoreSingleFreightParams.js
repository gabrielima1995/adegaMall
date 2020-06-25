var StoreSingleFreightParams = (function () {

    function init(addSingleFreightParamUrl, singleFreightEnabledSelector) {
        var $submitButton = $('#js-submit');
        var $body = $('body');
        var $singleFreightParamsContainer = $('.js-single-freight-params-container');
        var $singleFreight = $('.js-single-freight');

        $('.js-single-freight-param-add-button').click(function (e) {
            e.preventDefault();

            $.ajax({
                url: addSingleFreightParamUrl,
                success: function (data) {
                    $singleFreightParamsContainer.append(data);

                    $(document).ready(function () {
                        MaaS.initPlugins();
                    });
                }
            });
        });

        $body.on('click', '.js-single-freight-param .minus', function (e) {
            e.preventDefault();
            var $this = $(this);
            $this.parents('.js-single-freight-param').remove();
            return false;
        });

        $(singleFreightEnabledSelector).change(function () {
            $singleFreight.css('display', this.checked ? 'block' : 'none');
            if (!this.checked) $singleFreightParamsContainer.html('');
        });
    }

    return {
        init: init
    };

})();
