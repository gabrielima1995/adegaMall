using Application.Config;
using NewIntegrator.Stripe.Plans;
using PaymentIntegrator.Core.Plans;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewIntegrator.Stripe.Retro.Plans
{
    public class StripeRetroPlanOperations : IPlanOperations
    {
        private readonly StripePlanService _service;

        public StripeRetroPlanOperations()
        {
            _service = new StripePlanService();
        }

        public async Task<IList<IPlan>> GetListAsync(int page = 1, int resultsByPage = 50)
        {
            var list = await _service.GetListByProductAsync(MaaSConfig.Current.Subscription.Stripe.ProductId);

            return list
                .Select(plan => new StripeRetroPlan(plan))
                .Cast<IPlan>()
                .ToList();
        }
    }
}