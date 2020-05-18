using PagarMe;
using PaymentIntegrator.Core.Plans;

namespace PaymentIntegrator.PagarMe.Plans
{
    public static class PlanHelper
    {
        public static Plan CreateToPagarMe(this IPlan plan, PagarMeClient client)
        {
            return new Plan(client.GetService())
            {
                Id = plan.GatewayId
            };
        }
    }
}