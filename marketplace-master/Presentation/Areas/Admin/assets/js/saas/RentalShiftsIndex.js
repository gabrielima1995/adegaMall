var RentalShiftsIndex = (function () {

    function init() {
        $(".js-remove").click(btnRemove_click);
        $("#formAddShift").submit(formAddShift_submit);
        createEditorForCurrentShifts();
    }

    function createEditorForCurrentShifts() {
        $("#currentShifts [contenteditable]").each(function () {
            var editable = $(this);
            var icon = editable.next();
            var line = editable.parents("tr");

            editable.focus(function () {
                editable.data("before", editable.text());
            });

            editable.blur(function () {
                if (editable.data("before") !== editable.text()) {
                    icon.show().attr("class", "fa fa-refresh fa-spin");

                    $.ajax({
                        type: "POST",
                        url: "/admin/rentalshifts/edit",
                        data: {
                            id: line.attr("data-shift-id"),
                            name: line.find(".js-shift-name").text(),
                            startTime: line.find(".js-shift-start").text(),
                            endTime: line.find(".js-shift-end").text()
                        }
                    })
                        .done(function (response) {
                            if (response.IsSuccess) {
                                icon.attr("class", "fa fa-check text-success");
                                setTimeout(function () { icon.fadeOut(600); }, 2000);
                            }
                            else {
                                icon.attr("class", "fa fa-remove text-danger");
                                toastr.error(response.Error);
                            }
                        });
                }
            });
        });
    }

    function btnRemove_click(e) {
        e.preventDefault();
        var btn = $(this);

        $.ajax({
            type: "POST",
            url: btn.attr("href")
        })
            .done(function (response) {
                if (response.IsSuccess) {
                    location = response.Data.redirectUrl;
                }
                else toastr.error(response.Error, "Não foi possível remover o turno");
            });
    }

    function formAddShift_submit(e) {
        e.preventDefault();
        var form = $("#formAddShift");
        var formData = {
            name: $("#txtAddName").val(),
            startTime: $("#txtAddStart").val(),
            endTime: $("#txtAddEnd").val()
        };

        $.ajax({
            type: "POST",
            url: form.attr("action"),
            data: formData
        })
            .done(function (response) {
                if (response.IsSuccess)
                    location = response.Data.redirectUrl;

                else {
                    toastr.error(response.Error, "Não foi possível salvar o turno");
                    $("#btnAddShift").button("reset");
                }
            });
    }

    return {
        init: init
    };

})();