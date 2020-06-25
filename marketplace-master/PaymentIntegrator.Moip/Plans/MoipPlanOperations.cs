using Newtonsoft.Json;
using PaymentIntegrator.Core.Plans;
using PaymentIntegrator.Moip.Common;
using PaymentIntegrator.Moip.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace PaymentIntegrator.Moip.Plans
{
    public class MoipPlanOperations : IPlanOperations
    {
        public Task<IList<IPlan>> GetListAsync(int page = 1, int resultsByPage = 50)
        {
            var getPlanListRequest = new MoipRequest("plans", Method.GET);
            var getPlanListResponse = MoipClient.SendV1Request(getPlanListRequest);

            if (getPlanListResponse.StatusCode == HttpStatusCode.OK)
            {
                dynamic plansObject = JsonConvert.DeserializeObject(getPlanListResponse.Content);
                IList<IPlan> result = new List<IPlan>();

                foreach (dynamic plan in plansObject.plans)
                {
                    //Ignore inactive plans
                    if (!((string)plan.status).Equals("ACTIVE", StringComparison.OrdinalIgnoreCase))
                        continue;

                    result.Add(new MoipPlan()
                    {
                        Name = (string)plan.name,
                        GatewayId = (string)plan.code,
                        Amount = ((int)plan.amount).FromCents(),
                        Days = PlanHelper.ToDays((string)plan.interval.unit, (int)plan.interval.length),
                        TrialDays = (int)plan.trial.days,
                    });
                }

                return Task.FromResult(result);
            }


            throw new HttpException((int)getPlanListResponse.StatusCode, $"Error getting plan list - API Responded with {getPlanListResponse.StatusCode}");
        }
    }

    public class MoipPlan : IPlan
    {
        public string Name { get; set; }

        public decimal Amount { get; set; }

        public int Days { get; set; }

        public int TrialDays { get; set; }

        public string GatewayId { get; set; }
    }
}
