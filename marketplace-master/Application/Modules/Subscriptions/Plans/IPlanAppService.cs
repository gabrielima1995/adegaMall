using Domain.Entities.SubscriptionModule;
using Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Modules.Subscriptions.Plans
{
    public interface IPlanAppService
    {
        /// <summary>
        /// Importa os planos de assinatura do gateway de pagamento.
        /// </summary>
        /// <returns></returns>
        Task<OperationResult> ImportFromGatewayAsync();

        /// <summary>
        /// Salva o plano de assinatura.
        /// </summary>
        /// <param name="model">Dados do plano de assinatura</param>
        /// <returns></returns>
        OperationResult<Plan> Save(PlanViewModel model);

        /// <summary>
        /// Obtem todos os planos de assinatura.
        /// </summary>
        /// <returns></returns>
        IList<Plan> GetList();

        /// <summary>
        /// Habilita assinatura,
        /// </summary>
        /// <param name="planId">Identificador do plano.</param>
        void Enable(Guid planId);

        /// <summary>
        /// Desabilita uma assinatura.
        /// </summary>
        /// <param name="planId">Identificador do plano.</param>
        void Disable(Guid planId);
    }
}