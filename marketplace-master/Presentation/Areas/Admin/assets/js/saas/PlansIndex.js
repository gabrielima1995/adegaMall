var PlansIndex = (function () {

    function init() {
        $('table').on('click', '.js-toggle-status-button', btnChangePlanStatus_click);
        $('#js-import-plans-button').click(btnImportPlans_click);

        $('.js-edit-button').on('click', function (e) {
            $(this).parent('.js-edit-div-1').hide();
            $(this).parent().siblings('.js-edit-div-2').show();
        });

        $('.js-check-submit').on('click', onCheckSubmit);
    }

    function onCheckSubmit(e) {
        let isCredits = $(this).attr('data-value') === 'credits';
        let url = isCredits ? '/admin/plans/updatecredits' : '/admin/plans/updatedays';
        let textToUpdate = $(this).parent().siblings('.js-edit-div-1').children('.js-edit-text');
        let newValue = $(this).siblings('.js-text-form').val();
        let id = $(this).parent().parent().parent().attr('data-id');
        let varthis = $(this);

        $.ajax({
            type: 'POST',
            url: url,
            data: {
                planId: id,
                newValue: newValue
            },
            success: function () {
                toastr.success('Valor atualizado com sucesso');
                textToUpdate.text(newValue);
            },
            error: function (error) {
                alert('Error: ' + error);
            },
            complete: function () {
                varthis.parent('.js-edit-div-2').hide();
                varthis.parent().siblings('.js-edit-div-1').show();
            }
        });
    }

    function setStatusTextAndIcon(buttonElement, isEnabled) {
        var tdParent = buttonElement.parent('td');
        buttonElement.text(isEnabled ? 'Desativar' : 'Ativar');
        tdParent.find('.js-status-icon')
            .removeClass('text-' + (isEnabled ? 'danger' : 'success'))
            .addClass('text-' + (isEnabled ? 'success' : 'danger'));
    }

    function btnChangePlanStatus_click(e) {
        var $el = $(this);
        var action = $el.text() === 'Ativar' ? 'enable' : 'disable';
        var actionUrl = $el.attr('data-' + action);

        $.ajax({
            type: 'POST',
            url: actionUrl,
            success: function () {
                setStatusTextAndIcon($el, (action === 'enable'));
            }
        });
    }

    function btnImportPlans_click(e) {
        $.ajax({
            type: 'POST',
            url: '/admin/plans/import',
            success: function () {
                toastr.success('Planos importados com sucesso! Atualizando página...');

                setTimeout(function () {
                    window.location.reload(true);
                }, 1500);
            },
            error: function (error) {
                alert('Error: ' + error);
            }
        });
    }

    return {
        init: init
    };

})();