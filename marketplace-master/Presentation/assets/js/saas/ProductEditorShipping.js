var ProductEditorShipping = (function () {

    function init() {
        setShippingTypeContentVisibility();
        handleShippingTypeChange();
    }

    function handleShippingTypeChange() {
        $("[name=ShippingType]").change(setShippingTypeContentVisibility);
    }

    function setShippingTypeContentVisibility() {
        var isDigital = $("[name=ShippingType]:checked").val() === "Digital";
        $(".product-editor-digital-content")[isDigital ? "show" : "hide"]();
        $(".product-editor-physical-content, .product-editor-digital-hint")[isDigital ? "hide" : "show"]();
    }

    return {
        init: init
    };
})();