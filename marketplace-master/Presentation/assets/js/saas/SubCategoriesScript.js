var sellerId;

var SubCategoriesScript = (function () {

    function init() {
        $('body').on('change', '.js-category', function (e) {
            var $this = $(this);
            var $sub = $this.parent().parent().parent().find('.js-subCategory');
            var subValue = $sub.val();
            var url = '/subcategories/select/?lang=pt-br&category=' + $this.val();
            if (sellerId) url += '&seller=' + sellerId;

            $.ajax({
                url: url,
                success: function (data) {
                    $sub.html(data).val(subValue);
                }
            });
        });

        $('.js-category').each(function (ix, el) {
            var $cat = $(el);
            if ($cat.val().length) $cat.change();
        });
    }

    return { init: init };
}());