var StoreIndex = (function () {

    var _productId;
    var _configHideCurrentStock;

    function init(configHideCurrentStock) {
        _configHideCurrentStock = configHideCurrentStock;
        configureDataTable();
        handleProductDelete();
        showModalDeleteProduct();
    }

    function configureDataTable() {
        var columns = [];
        columns.push({
            "data": "coverImageUrl",
            "render": function (data, type, row, meta) {
                var custom = $("<img>").attr("src", data).attr("width", "80");
                return $("<div>").append(custom).html();
            },
            "orderable": false
        });
        columns.push({
            "data": "title",
            "render": function (data, type, row, meta) {
                var custom = $("<a>").attr("href", "/store/editproduct/" + row.id).text(data);
                return $("<div>").append(custom).html();
            },
            "orderable": false
        });
        columns.push({
            "data": "categoryName",
            "orderable": false
        });
        columns.push({
            "data": "price",
            "render": function (data, type, row, meta) {
                if (type === "display") {
                    var custom = $("<div>").append($("<span>").text(row.priceFormatted));
                    row.discountPriceFormatted && custom
                        .append($("<br>"))
                        .append($("<span>").addClass("label label-success").text(row.discountPriceFormatted));
                    return $("<div>").append(custom).html();
                }
                return data;
            },
            "orderable": false
        });
        !_configHideCurrentStock && columns.push({
            "data": "currentStock",
            "orderable": false
        });
        columns.push({
            "data": "id",
            "render": function (data, type, row, meta) {
                var custom = $("<a>").attr("href", "javascript:void(0)").attr("data-product-id", data).attr("class", "text-danger buttonDeleteProduct")
                    .append($("<i>").attr("class", "fa fa-trash").css("font-size", "20px"));
                return $("<div>").append(custom).html();
            },
            "orderable": false
        });

        $("#tblProducts").DataTable({
            info: false, serverSide: true,
            ajax: {
                url: "/store/getproductsforstore",
                dataSrc: "items"
            },
            columns: columns,
            pageLength: 25,
            initComplete: configureDataTable_initComplete
        });
    }

    function configureDataTable_initComplete() {
        var panel = $("<div>").addClass("panel panel-default m-b-xs");
        var table = $("#tblProducts");
        var tableContainer = $("#tblProducts").parent();
        tableContainer.prepend(panel);
        panel.prepend(table);

        table.find(".sorting_asc").removeClass("sorting_asc");
        table.css("width", "auto");
        $(".dataTables_length").remove();
    }

    function showModalDeleteProduct() {
        $(document).on("click", ".buttonDeleteProduct", function () {
            var self = $(this);
            _productId = self.data().productId;
            $("#modalDeleteProductAlert").modal("show");
        });
    }

    function handleProductDelete() {
        $(document).on("click", ".confirm-delete-product", function () {
            $("#deleteProduct").attr('href', "DeleteProduct/" + _productId);
        });
    }

    return {
        init: init
    };

})();
