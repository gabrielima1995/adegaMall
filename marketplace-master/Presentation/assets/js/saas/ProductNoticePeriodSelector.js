function ProductNoticePeriodSelector(containerSelector) {
    this.container = $(containerSelector);
    this.currentText = this.container.find('.current-text');
    this.hidden = this.container.find('#MinRentalNoticeUnit');
    this.dropDownItemSelector = '.dropdown-menu a';

    this.applyChangeInteraction();
    this.setTextFromHiddenValue();
}

ProductNoticePeriodSelector.prototype.applyChangeInteraction = function () {
    var self = this;

    this.container.find(this.dropDownItemSelector).click(function (e) {
        var el = $(this);
        var newText = el.text();
        var newValue = el.attr('data-id');

        self.currentText.text(newText);
        self.hidden.val(newValue);
    });
};

ProductNoticePeriodSelector.prototype.setTextFromHiddenValue = function () {
    var self = this;
    var hiddenValue = this.hidden.val();

    this.container.find(this.dropDownItemSelector).each(function (i) {
        var el = $(this);
        if (el.attr('data-id') !== hiddenValue) return true;
        self.currentText.text(el.text()); return false;
    });
};
