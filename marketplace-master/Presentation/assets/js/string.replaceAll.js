String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    search = search.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    return target.replace(new RegExp(search, 'g'), replacement);
};
