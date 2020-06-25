(function ($, undefined) {
    Globalize.loadCldr =
        $.when(
            $.get("/assets/js/cldr/supplemental/likelySubtags.json"),
            $.get("/assets/js/cldr/main/pt/numbers.json"),
            $.get("/assets/js/cldr/supplemental/numberingSystems.json"),
            $.get("/assets/js/cldr/supplemental/plurals.json"),
            $.get("/assets/js/cldr/supplemental/ordinals.json"),
            $.get("/assets/js/cldr/main/pt/currencies.json"),
            $.get("/assets/js/cldr/supplemental/currencyData.json"),
            $.get("/assets/js/cldr/main/pt/ca-gregorian.json"),
            $.get("/assets/js/cldr/main/pt/timeZoneNames.json"),
            $.get("/assets/js/cldr/supplemental/timeData.json"),
            $.get("/assets/js/cldr/supplemental/weekData.json")
        ).then(function () {
            // Normalize $.get results, we only need the JSON, not the request statuses.
            return [].slice.apply(arguments, [0]).map(function (result) {
                return result[0];
            });
        }).then(Globalize.load).then(function () {
            console.log('Globalize loaded');

            // Initialise Globalize to the current UI culture
            Globalize.current = Globalize.locale("pt");

            Globalize.current.patterns = {
                money: '#.###.##0,00',
                integer: '#.###.##0',
                decimal: '#.###.##0,0',
                decimalSeparator: ',',
                lang: 'pt-BR',
                date: 'DD/MM/YYYY',
                time: 'HH:mm',
                dateTime: 'DD/MM/YYYY HH:mm'
            };
        });

    $.fn.datepicker.dates['pt-BR'] = {
        days: ["Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado"],
        daysShort: ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sab"],
        daysMin: ["Do", "Se", "Te", "Qa", "Qi", "Se", "Sa"],
        months: ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"],
        monthsShort: ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"],
        today: "Hoje",
        clear: "Limpar",
        format: "dd/mm/yyyy",
        titleFormat: "MM yyyy", /* Leverages same syntax as 'format' */
        weekStart: 0
    };

    $.fn.select2.defaults.set('language', 'pt-BR');

    if ($.fn.dataTable) {
        $.extend(true, $.fn.dataTable.defaults, {
            "dom": '<<"pull-right"B><"pull-left"f>>rtip',
            "oLanguage": {
                "sEmptyTable": "Nenhum registro encontrado",
                "sInfo": "Mostrando de _START_ até _END_ de _TOTAL_ registros",
                "sInfoEmpty": "Mostrando 0 até 0 de 0 registros",
                "sInfoFiltered": "(Filtrados de _MAX_ registros)",
                "sInfoPostFix": "",
                "sInfoThousands": ".",
                "sLengthMenu": "_MENU_ resultados por página",
                "sLoadingRecords": "Carregando...",
                "sProcessing": "Processando...",
                "sZeroRecords": "Nenhum registro encontrado",
                "sSearch": "Pesquisar",
                "oPaginate": {
                    "sNext": "Próximo",
                    "sPrevious": "Anterior",
                    "sFirst": "Primeiro",
                    "sLast": "Último"
                },
                "oAria": {
                    "sSortAscending": ": Ordenar colunas de forma ascendente",
                    "sSortDescending": ": Ordenar colunas de forma descendente"
                }
            }
        });
    }

    if (typeof moment !== 'undefined') {
        moment.locale('pt-BR', {
            months: 'Jan_Fev_Mar_Abr_Mai_Jun_Jul_Ago_Set_Out_Nov_Dez'.split('_'),
            monthsShort: 'Jan_Fev_Mar_Abr_Mai_Jun_Jul_Ago_Set_Out_Nov_Dez'.split('_'),
            weekdaysMin: 'Do_Se_Te_Qa_Qi_Sx_Sb'.split('_')
        });
    }
})(window.jQuery);

