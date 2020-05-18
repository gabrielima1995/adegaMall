using Stripe;
using System;

namespace NewIntegrator.Stripe.Plans
{
    internal static class PlanExtensions
    {
        public static StripePlan FromStripe(this Plan plan)
        {
            return new StripePlan
            {
                Id = plan.Id,
                Nickname = plan.Nickname,
                Amount = plan.Amount ?? 0,
                Currency = plan.Currency,
                Interval = plan.EnumerationInterval(),
                IntervalCount = plan.IntervalCount,
                TrialPeriodDay = plan.TrialPeriodDays ?? 0
            };
        }

        public static StripePlanInterval EnumerationInterval(this Plan plan)
        {
            switch (plan.Interval)
            {
                case "day":
                    return StripePlanInterval.Day;

                case "week":
                    return StripePlanInterval.Week;

                case "month":
                    return StripePlanInterval.Month;

                case "year":
                    return StripePlanInterval.Year;

                default:
                    throw new ArgumentOutOfRangeException(nameof(plan.Interval));
            }
        }
    }
}