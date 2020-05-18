using PagarMe;
using PaymentIntegrator.Core.Plans;

namespace PaymentIntegrator.PagarMe.Plans
{
    /// <summary>
    /// Representa um plano de assinatura já salvo no Pagar.me.
    /// </summary>
    public class PlanResponse : IPlan
    {
        public string Name { get; private set; }

        public decimal Amount { get; private set; }

        public int Days { get; private set; }

        public int TrialDays { get; private set; }

        public string GatewayId { get; private set; }

        public PlanResponse(Plan plan)
        {
            Name = plan.Name;
            Amount = plan.Amount / 100M;
            Days = plan.Days;
            TrialDays = plan.TrialDays;
            GatewayId = plan.Id;
        }
    }
}