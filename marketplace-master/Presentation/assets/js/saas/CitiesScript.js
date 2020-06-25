var CitiesScript = (function () {

    var _cityStatePlaceholder;

    function init(cityStatePlaceholder) {
        _cityStatePlaceholder = cityStatePlaceholder;

        Globalize.loadCldr.then(function () {
            var $citySelect = $('.js-city-select');
            var $citySelectContainer = $('.js-city-select-container');

            $citySelect.select2({
                ajax: {
                    url: "/api/v1/cities/search",
                    dataType: 'json',
                    delay: 500,
                    credentials: true,
                    data: function (params) {
                        return {
                            query: params.term // search term
                        };
                    },
                    processResults: function (data, params) {
                        return {
                            results: data
                        };
                    },
                    cache: true
                },
                escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
                minimumInputLength: 2,
                placeholder: _cityStatePlaceholder || 'Cidade/UF',
                allowClear: true,
                templateSelection: function (data, container) {
                    return data.text;
                },
                language: Globalize.current.patterns.lang,
                dropdownParent: $citySelectContainer
            });

            $('.select2-container').addClass('form-control');

            $citySelect.on("select2:select", function (e) {
                $citySelectContainer.parent().find('.js-city-lat').val(e.params.data.lat || 0);
                $citySelectContainer.parent().find('.js-city-lng').val(e.params.data.lng || 0);
            });

            $citySelect.on("select2:unselect", function (e) {
                $citySelectContainer.parent().find('.js-city-lat').val('');
                $citySelectContainer.parent().find('.js-city-lng').val('');
            });
        });
    }
    
    return {
        init: init
    };

}) ();