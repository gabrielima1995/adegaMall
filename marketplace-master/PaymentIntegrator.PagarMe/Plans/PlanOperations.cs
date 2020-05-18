using Newtonsoft.Json;
using PagarMe;
using PaymentIntegrator.Core.Plans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentIntegrator.PagarMe.Plans
{
    public class PlanOperations : IPlanOperations
    {
        public Task<IList<IPlan>> GetListAsync(int page = 1, int resultsByPage = 50)
        {
            var client = new SimpleImpl.Client();
            var request = new SimpleImpl.ListPlansRequest(page, resultsByPage);
            var response = client.Execute(request);

            var plans = JsonConvert.DeserializeAnonymousType(response.Content, new List<Plan>());
            var cast = ConvertForResponse(plans);

            return Task.FromResult(cast);
        }

        private IList<IPlan> ConvertForResponse(List<Plan> plans)
        {
            var converted = new List<IPlan>(plans.Count);

            foreach (var plan in plans)
                converted.Add(new PlanResponse(plan));

            return converted;
        }
    }
}