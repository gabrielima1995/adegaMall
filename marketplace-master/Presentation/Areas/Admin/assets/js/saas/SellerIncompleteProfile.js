var SellerIncompleteProfile = (function () {

    function init() {
        popover();
    }

    function popover() {
        $("[data-toggle='popover']").hover(
            function () {
                var self = $(this);
                var completeProfile = self.data().profile;
                var pendingFields = '';
                if (!completeProfile.IsComplete) {
                    var doc = $("<ul>");
                    if (completeProfile.Registration.NeedsAvatar && !completeProfile.Registration.HasAvatar) {
                        doc.append($('<li>').text('Imagem de perfil não cadastrada'));
                    }
                    if (completeProfile.Registration.NeedsWhatsAppNumber && !completeProfile.Registration.HasWhatsAppNumber) {
                        doc.append($('<li>').text('Campo WhatsApp não preenchido'));
                    }
                    if (completeProfile.Registration.NeedsDocumentNumber && !completeProfile.Registration.HasDocumentNumber) {
                        doc.append($('<li>').text('Campo CPF/CNJP não preenchido'));
                    }
                    if (completeProfile.Registration.NeedsDateOfBirth && !completeProfile.Registration.HasDateOfBirth) {
                        doc.append($('<li>').text('Campo data de nascimento não preenchido'));
                    }
                    if (completeProfile.Registration.NeedsAddress && !completeProfile.Registration.HasAddress) {
                        doc.append($('<li>').text('Campo endereço não preenchido'));
                    }
                    if (completeProfile.Registration.PendingCustomFields.length > 0) {
                        doc.append($('<li>').text('Campo(s) personalizado(s) não preenchido(s)'));
                    }
                    if (!completeProfile.MembershipPlan.IsComplete) {
                        doc.append($('<li>').text('Não assinou um plano'));
                    }
                    if (!completeProfile.Gateway.IsComplete) {
                        doc.append($('<li>').text('Dados bancários não preenchidos'));
                    }
                    if (!completeProfile.Products.IsComplete) {
                        doc.append($('<li>').text('Produto não cadastrado'));
                    }
                    if (!completeProfile.PortfolioItems.IsComplete) {
                        doc.append($('<li>').text('Portfólio não cadastrado'));
                    }
                    if (!completeProfile.PortfolioCategories.IsComplete) {
                        doc.append($('<li>').text('Portfólio não categorizado'));
                    }
                }
                pendingFields = doc.html();
                self.attr('data-content', pendingFields);
                self.popover("show");
            },
            function () { $(this).popover("hide"); });
    }

    return {
        init: init
    };

})();