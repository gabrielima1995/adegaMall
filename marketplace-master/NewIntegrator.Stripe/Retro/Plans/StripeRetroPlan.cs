using NewIntegrator.Stripe.Plans;
using PaymentIntegrator.Core.Plans;

namespace NewIntegrator.Stripe.Retro.Plans
{
    public class StripeRetroPlan : IPlan
    {
        public string Name { get; private set; }
        public decimal Amount { get; private set; }
        public int Days { get; private set; }
        public int TrialDays { get; private set; }
        public string GatewayId { get; private set; }

        public StripeRetroPlan(StripePlan plan)
        {
            Name = plan.Nickname;
            Amount = plan.Amount / 100M;
            Days = (int)plan.GetIntervalDays();
            TrialDays = (int)plan.TrialPeriodDay;
            GatewayId = plan.Id;
        }
    }
}