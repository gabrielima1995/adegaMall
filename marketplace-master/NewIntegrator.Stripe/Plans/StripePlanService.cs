using Stripe;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewIntegrator.Stripe.Plans
{
    public class StripePlanService
    {
        public async Task<IEnumerable<StripePlan>> GetListByProductAsync(string productId)
        {
            var planService = new PlanService();

            var options = new PlanListOptions
            {
                Active = true,
                ProductId = productId,
                Limit = 10
            };

            var stripeList = await planService.ListAsync(options);

            return stripeList.Data.Select(i => i.FromStripe());
        }
    }
}