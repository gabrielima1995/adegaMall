using PaymentIntegrator.Core.Plans;

namespace Application.Modules.Subscriptions.PagarMe
{
    /// <summary>
    /// Dados do objeto Plan no postback enviado pelo Pagar.me.
    /// </summary>
    public class PlanPostbackData
    {
        public string id { get; set; }

        public int amount { get; set; }

        public int days { get; set; }

        public string name { get; set; }

        public int trial_days { get; set; }

        public int? charges { get; set; }

        public PlanPostbackData()
        {
        }

        public PlanPostbackData(IPlan plan)
        {
            id = plan.GatewayId;
            amount = (int)(plan.Amount * 100);
            days = plan.Days;
            name = plan.Name;
            trial_days = plan.TrialDays;
        }
    }
}