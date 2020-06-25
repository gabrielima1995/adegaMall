var DataTableCustom = (function () {

    function create(selector, options) {
        selector = selector || 'table';

        Globalize.loadCldr.then(function () {
            $(selector).DataTable($.extend(true, { //dom: '<"container"<"pull-right"B><"pull-left"f>>rtip',
                iDisplayLength: 50,
                buttons: [{
                    extend: 'excel',
                    text: 'Exportar como Planilha',
                    exportOptions: {
                        columns: ':visible',
                        format: {
                            body: function (data, row, column, node) {
                                data = $('<p>' + data + '</p>').text();
                                data = $.isNumeric(data.replace(',', '.')) ? data.replace(',', '.') : data;
                                data = data.replace(/,/g, '.');
                                return data;
                            }
                        }
                    }
                }]
            }, options, window.MaaS.dtOptions));

            makeResponsive(selector);
            fixFooter(selector);
        });
    }

    function makeResponsive(selector) {
        $(selector)
            .before($('<div>').addClass('table-responsive'))
            .each(function (e, el) {
                var container = $(this).prev();
                $(this).appendTo(container);
            });

        $(selector).parents('.dt-bootstrap').each(function (i, el) {
            $(el).children().first().addClass('clearfix');
        });
    }

    function fixFooter(selector) {
        $(selector).parents('.dt-bootstrap').each(function (i, el) {
            var parent = $(this),
                info = parent.find('.dataTables_info'),
                pager = parent.find('.dataTables_paginate'),
                row = $('<div>').addClass('row').insertBefore(info),
                colLeft = $('<div>').addClass('col-sm-5').appendTo(row),
                colRight = $('<div>').addClass('col-sm-7').appendTo(row);
            info.appendTo(colLeft);
            pager.appendTo(colRight);
        });
    }

    return {
        create: create
    };

}) ();