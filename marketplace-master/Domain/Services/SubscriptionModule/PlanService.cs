using Domain.Entities.SubscriptionModule;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Modules.Subscriptions.Plans
{
    public class PlanService : ServiceBase<Plan>
    {
        public PlanService(
            IRepository<Plan> repository)
            : base(repository)
        {
        }

        public Plan Disable(Guid planId)
        {
            return Update(
                i => i.Id == planId,
                p => p.Disable());
        }

        public Plan Enable(Guid planId)
        {
            return Update(
                i => i.Id == planId,
                p => p.Enable());
        }

        public IList<Plan> GetPlans(bool onlyEnabled)
        {
            return Query()
                .Where(i => !onlyEnabled || i.Enabled)
                .OrderBy(i => i.Days)
                .ThenBy(i => i.Name)
                .ToList();
        }
    }
}