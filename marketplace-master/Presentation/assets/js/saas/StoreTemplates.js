var StoreTemplates = (function () {

    function init() {
        configureDataTable();
    }

    function configureDataTable() {
        $('#tblTemplates').DataTable({ info: false, initComplete: configureDataTable_initComplete, lengthChange: false, pageLength: 25 });
    }

    function configureDataTable_initComplete() {
        var panel = $('<div>').addClass('panel panel-default m-b-xs');
        var table = $('#tblTemplates');
        var tableContainer = $('#tblTemplates').parent();
        tableContainer.prepend(panel);
        panel.prepend(table);
    }

    return { init };

})();
